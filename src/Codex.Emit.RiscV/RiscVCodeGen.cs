using System.Text;
using Codex.Core;
using Codex.IR;
using Codex.Types;

namespace Codex.Emit.RiscV;

sealed class RiscVCodeGen(RiscVTarget target = RiscVTarget.LinuxUser)
{
    readonly List<uint> m_instructions = [];
    readonly List<byte> m_rodata = [];
    readonly Dictionary<string, int> m_functionOffsets = [];
    readonly List<(int InsnIndex, string Target)> m_callPatches = [];
    readonly List<RodataFixup> m_rodataFixups = [];
    readonly Dictionary<string, int> m_stringOffsets = [];
    readonly RiscVTarget m_target = target;

    // QEMU virt machine UART0 address (NS16550A compatible)
    const long UartBase = 0x10000000;

    uint m_nextTemp = Reg.T0;
    uint m_nextLocal = Reg.S2;
    Dictionary<string, uint> m_locals = [];

    // S1 is reserved as the global heap pointer — NOT saved/restored by functions.
    static readonly uint[] CalleeSaved = {
        Reg.S2, Reg.S3, Reg.S4, Reg.S5, Reg.S6,
        Reg.S7, Reg.S8, Reg.S9, Reg.S10, Reg.S11
    };

    public void EmitModule(IRModule module)
    {
        // Bare metal: reserve first instruction slot for jump-to-start trampoline.
        // CPU begins execution at byte 0, but _start is emitted after all functions.
        int trampolineIndex = -1;
        if (m_target == RiscVTarget.BareMetal)
        {
            trampolineIndex = m_instructions.Count;
            Emit(RiscVEncoder.Nop()); // patched after _start is emitted
        }

        EmitRuntimeHelpers();

        foreach (IRDefinition def in module.Definitions)
            EmitFunction(def);

        EmitStart(module);

        // Patch the trampoline to jump to _start
        if (trampolineIndex >= 0 && m_functionOffsets.TryGetValue("__start", out int startIndex))
            m_instructions[trampolineIndex] = RiscVEncoder.J((startIndex - trampolineIndex) * 4);

        PatchCalls();
    }

    public byte[] BuildElf()
    {
        byte[] textSection = new byte[m_instructions.Count * 4];
        for (int i = 0; i < m_instructions.Count; i++)
        {
            uint insn = m_instructions[i];
            textSection[i * 4 + 0] = (byte)(insn & 0xFF);
            textSection[i * 4 + 1] = (byte)((insn >> 8) & 0xFF);
            textSection[i * 4 + 2] = (byte)((insn >> 16) & 0xFF);
            textSection[i * 4 + 3] = (byte)((insn >> 24) & 0xFF);
        }

        if (m_target == RiscVTarget.BareMetal)
            return ElfWriter.WriteFlatBinary(textSection, m_rodata.ToArray());

        int startOffset = m_functionOffsets["__start"] * 4;
        return ElfWriter.WriteExecutable(textSection, m_rodata.ToArray(), (ulong)startOffset, m_target);
    }

    // ── Function emission ────────────────────────────────────────

    void EmitFunction(IRDefinition def)
    {
        m_functionOffsets[def.Name] = m_instructions.Count;
        m_locals = new Dictionary<string, uint>();
        m_nextTemp = Reg.T3;
        m_nextLocal = Reg.S2;

        // Prologue: 96-byte frame = ra + s0 + s2-s11 (12 × 8)
        Emit(RiscVEncoder.Addi(Reg.Sp, Reg.Sp, -96));
        Emit(RiscVEncoder.Sd(Reg.Sp, Reg.Ra, 88));
        Emit(RiscVEncoder.Sd(Reg.Sp, Reg.S0, 80));
        for (int i = 0; i < CalleeSaved.Length; i++)
            Emit(RiscVEncoder.Sd(Reg.Sp, CalleeSaved[i], 72 - i * 8));
        Emit(RiscVEncoder.Addi(Reg.S0, Reg.Sp, 96));

        for (int i = 0; i < def.Parameters.Length && i < 8; i++)
        {
            uint savedReg = AllocLocal();
            Emit(RiscVEncoder.Mv(savedReg, Reg.A0 + (uint)i));
            m_locals[def.Parameters[i].Name] = savedReg;
        }

        uint resultReg = EmitExpr(def.Body);
        if (resultReg != Reg.A0)
            Emit(RiscVEncoder.Mv(Reg.A0, resultReg));

        // Epilogue
        for (int i = 0; i < CalleeSaved.Length; i++)
            Emit(RiscVEncoder.Ld(CalleeSaved[i], Reg.Sp, 72 - i * 8));
        Emit(RiscVEncoder.Ld(Reg.Ra, Reg.Sp, 88));
        Emit(RiscVEncoder.Ld(Reg.S0, Reg.Sp, 80));
        Emit(RiscVEncoder.Addi(Reg.Sp, Reg.Sp, 96));
        Emit(RiscVEncoder.Ret());
    }

    // ── Expression emission ──────────────────────────────────────

    uint EmitExpr(IRExpr expr) => expr switch
    {
        IRIntegerLit intLit => EmitIntegerLit(intLit.Value),
        IRBoolLit boolLit => EmitIntegerLit(boolLit.Value ? 1 : 0),
        IRTextLit textLit => EmitTextLit(textLit.Value),
        IRName name => EmitName(name),
        IRBinary bin => EmitBinary(bin),
        IRIf ifExpr => EmitIf(ifExpr),
        IRLet letExpr => EmitLet(letExpr),
        IRApply apply => EmitApply(apply),
        IRNegate neg => EmitNegate(neg),
        IRDo doExpr => EmitDo(doExpr),
        IRRecord rec => EmitRecord(rec),
        IRFieldAccess fa => EmitFieldAccess(fa),
        IRMatch match => EmitMatch(match),
        IRRegion region => EmitRegion(region),
        _ => Reg.Zero
    };

    uint EmitIntegerLit(long value)
    {
        uint rd = AllocTemp();
        foreach (uint insn in RiscVEncoder.Li(rd, value))
            Emit(insn);
        return rd;
    }

    uint EmitTextLit(string value)
    {
        if (!m_stringOffsets.TryGetValue(value, out int rodataOffset))
        {
            rodataOffset = m_rodata.Count;
            byte[] utf8 = Encoding.UTF8.GetBytes(value);
            // Length-prefixed: 8-byte i64 length + UTF-8 data, 8-byte aligned
            m_rodata.AddRange(BitConverter.GetBytes((long)utf8.Length));
            m_rodata.AddRange(utf8);
            while (m_rodata.Count % 8 != 0) m_rodata.Add(0);
            m_stringOffsets[value] = rodataOffset;
        }

        uint rd = AllocTemp();
        EmitLoadRodataAddress(rd, rodataOffset);
        return rd;
    }

    void EmitLoadRodataAddress(uint rd, int rodataOffset)
    {
        // Reserve 2 nop slots, patched in PatchCalls once text size is known
        m_rodataFixups.Add(new RodataFixup(m_instructions.Count, rd, rodataOffset));
        Emit(RiscVEncoder.Nop());
        Emit(RiscVEncoder.Nop());
    }

    uint EmitName(IRName name)
    {
        if (m_locals.TryGetValue(name.Name, out uint reg))
            return reg;

        if (m_functionOffsets.ContainsKey(name.Name) && name.Type is not FunctionType)
        {
            uint rd = AllocTemp();
            EmitCallTo(name.Name);
            Emit(RiscVEncoder.Mv(rd, Reg.A0));
            return rd;
        }

        // Zero-arg sum type constructor
        if (name.Type is SumType sumType)
        {
            int tag = -1;
            for (int i = 0; i < sumType.Constructors.Length; i++)
            {
                if (sumType.Constructors[i].Name.Value == name.Name)
                {
                    tag = i;
                    break;
                }
            }
            if (tag >= 0)
            {
                uint ptrReg = AllocTemp();
                Emit(RiscVEncoder.Mv(ptrReg, Reg.S1));
                Emit(RiscVEncoder.Addi(Reg.S1, Reg.S1, 8));
                uint tagReg = AllocTemp();
                foreach (uint insn in RiscVEncoder.Li(tagReg, tag)) Emit(insn);
                Emit(RiscVEncoder.Sd(ptrReg, tagReg, 0));
                return ptrReg;
            }
        }

        return Reg.Zero;
    }

    uint EmitBinary(IRBinary bin)
    {
        uint left = EmitExpr(bin.Left);
        uint savedLeft = AllocLocal();
        Emit(RiscVEncoder.Mv(savedLeft, left));

        uint right = EmitExpr(bin.Right);
        uint rd = AllocTemp();

        switch (bin.Op)
        {
            case IRBinaryOp.AddInt: Emit(RiscVEncoder.Add(rd, savedLeft, right)); break;
            case IRBinaryOp.SubInt: Emit(RiscVEncoder.Sub(rd, savedLeft, right)); break;
            case IRBinaryOp.MulInt: Emit(RiscVEncoder.Mul(rd, savedLeft, right)); break;
            case IRBinaryOp.DivInt: Emit(RiscVEncoder.Div(rd, savedLeft, right)); break;
            case IRBinaryOp.Eq:
                if (bin.Left.Type is TextType)
                {
                    Emit(RiscVEncoder.Mv(Reg.A0, savedLeft));
                    Emit(RiscVEncoder.Mv(Reg.A1, right));
                    EmitCallTo("__str_eq");
                    Emit(RiscVEncoder.Mv(rd, Reg.A0));
                }
                else
                {
                    Emit(RiscVEncoder.Sub(rd, savedLeft, right));
                    Emit(RiscVEncoder.Sltu(rd, Reg.Zero, rd));
                    Emit(RiscVEncoder.Xori(rd, rd, 1));
                }
                break;
            case IRBinaryOp.NotEq:
                if (bin.Left.Type is TextType)
                {
                    Emit(RiscVEncoder.Mv(Reg.A0, savedLeft));
                    Emit(RiscVEncoder.Mv(Reg.A1, right));
                    EmitCallTo("__str_eq");
                    Emit(RiscVEncoder.Xori(Reg.A0, Reg.A0, 1));
                    Emit(RiscVEncoder.Mv(rd, Reg.A0));
                }
                else
                {
                    Emit(RiscVEncoder.Sub(rd, savedLeft, right));
                    Emit(RiscVEncoder.Sltu(rd, Reg.Zero, rd));
                }
                break;
            case IRBinaryOp.AppendText:
                Emit(RiscVEncoder.Mv(Reg.A0, savedLeft));
                Emit(RiscVEncoder.Mv(Reg.A1, right));
                EmitCallTo("__str_concat");
                Emit(RiscVEncoder.Mv(rd, Reg.A0));
                break;
            case IRBinaryOp.Lt:  Emit(RiscVEncoder.Slt(rd, savedLeft, right)); break;
            case IRBinaryOp.Gt:  Emit(RiscVEncoder.Slt(rd, right, savedLeft)); break;
            case IRBinaryOp.LtEq:
                Emit(RiscVEncoder.Slt(rd, right, savedLeft));
                Emit(RiscVEncoder.Xori(rd, rd, 1));
                break;
            case IRBinaryOp.GtEq:
                Emit(RiscVEncoder.Slt(rd, savedLeft, right));
                Emit(RiscVEncoder.Xori(rd, rd, 1));
                break;
            case IRBinaryOp.And: Emit(RiscVEncoder.And(rd, savedLeft, right)); break;
            case IRBinaryOp.Or:  Emit(RiscVEncoder.Or(rd, savedLeft, right)); break;
            default: Emit(RiscVEncoder.Mv(rd, Reg.Zero)); break;
        }

        return rd;
    }

    uint EmitIf(IRIf ifExpr)
    {
        uint cond = EmitExpr(ifExpr.Condition);

        int beqzIndex = m_instructions.Count;
        Emit(RiscVEncoder.Nop()); // patched: beqz → else

        uint thenReg = EmitExpr(ifExpr.Then);
        uint resultReg = AllocLocal();
        Emit(RiscVEncoder.Mv(resultReg, thenReg));

        int jEndIndex = m_instructions.Count;
        Emit(RiscVEncoder.Nop()); // patched: j → end

        int elseStart = m_instructions.Count;
        uint elseReg = EmitExpr(ifExpr.Else);
        Emit(RiscVEncoder.Mv(resultReg, elseReg));

        int endIndex = m_instructions.Count;

        m_instructions[beqzIndex] = RiscVEncoder.Beq(cond, Reg.Zero, (elseStart - beqzIndex) * 4);
        m_instructions[jEndIndex] = RiscVEncoder.J((endIndex - jEndIndex) * 4);

        return resultReg;
    }

    uint EmitLet(IRLet letExpr)
    {
        uint valReg = EmitExpr(letExpr.Value);
        uint savedReg = AllocLocal();
        Emit(RiscVEncoder.Mv(savedReg, valReg));
        m_locals[letExpr.Name] = savedReg;
        return EmitExpr(letExpr.Body);
    }

    uint EmitApply(IRApply apply)
    {
        List<IRExpr> args = new();
        IRExpr func = apply;
        while (func is IRApply inner)
        {
            args.Add(inner.Argument);
            func = inner.Function;
        }
        args.Reverse();

        if (func is IRName funcName)
        {
            if (TryEmitBuiltin(funcName.Name, args))
                return Reg.A0;

            // Sum type constructor: allocate [tag][field0][field1]... on heap
            if (apply.Type is SumType sumType)
            {
                uint ctorResult = EmitConstructor(funcName.Name, args, sumType);
                if (ctorResult != Reg.Zero)
                    return ctorResult;
            }

            List<uint> argRegs = new();
            foreach (IRExpr arg in args)
            {
                uint r = EmitExpr(arg);
                uint saved = AllocLocal();
                Emit(RiscVEncoder.Mv(saved, r));
                argRegs.Add(saved);
            }

            for (int i = 0; i < argRegs.Count && i < 8; i++)
            {
                if (argRegs[i] != Reg.A0 + (uint)i)
                    Emit(RiscVEncoder.Mv(Reg.A0 + (uint)i, argRegs[i]));
            }

            EmitCallTo(funcName.Name);
            uint rd = AllocTemp();
            Emit(RiscVEncoder.Mv(rd, Reg.A0));
            return rd;
        }

        return Reg.Zero;
    }

    uint EmitNegate(IRNegate neg)
    {
        uint operand = EmitExpr(neg.Operand);
        uint rd = AllocTemp();
        Emit(RiscVEncoder.Sub(rd, Reg.Zero, operand));
        return rd;
    }

    uint EmitDo(IRDo doExpr)
    {
        uint lastReg = Reg.Zero;
        foreach (IRDoStatement stmt in doExpr.Statements)
        {
            switch (stmt)
            {
                case IRDoExec exec:
                    lastReg = EmitExpr(exec.Expression);
                    break;
                case IRDoBind bind:
                    uint valReg = EmitExpr(bind.Value);
                    uint savedReg = AllocLocal();
                    Emit(RiscVEncoder.Mv(savedReg, valReg));
                    m_locals[bind.Name] = savedReg;
                    break;
            }
        }
        return lastReg;
    }

    // ── Records, sum types, pattern matching ─────────────────────

    uint EmitRecord(IRRecord rec)
    {
        List<uint> fieldRegs = new();
        foreach ((string _, IRExpr value) in rec.Fields)
        {
            uint r = EmitExpr(value);
            uint saved = AllocLocal();
            Emit(RiscVEncoder.Mv(saved, r));
            fieldRegs.Add(saved);
        }

        int totalSize = rec.Fields.Length * 8;

        uint ptrReg = AllocTemp();
        Emit(RiscVEncoder.Mv(ptrReg, Reg.S1));
        Emit(RiscVEncoder.Addi(Reg.S1, Reg.S1, totalSize));

        for (int i = 0; i < fieldRegs.Count; i++)
            Emit(RiscVEncoder.Sd(ptrReg, fieldRegs[i], i * 8));

        return ptrReg;
    }

    uint EmitFieldAccess(IRFieldAccess fa)
    {
        uint baseReg = EmitExpr(fa.Record);

        int fieldIndex = 0;
        if (fa.Record.Type is RecordType rt)
        {
            for (int i = 0; i < rt.Fields.Length; i++)
            {
                if (rt.Fields[i].FieldName.Value == fa.FieldName)
                {
                    fieldIndex = i;
                    break;
                }
            }
        }

        uint rd = AllocTemp();
        Emit(RiscVEncoder.Ld(rd, baseReg, fieldIndex * 8));
        return rd;
    }

    uint EmitConstructor(string ctorName, List<IRExpr> args, SumType sumType)
    {
        int tag = -1;
        for (int i = 0; i < sumType.Constructors.Length; i++)
        {
            if (sumType.Constructors[i].Name.Value == ctorName)
            {
                tag = i;
                break;
            }
        }
        if (tag < 0) return Reg.Zero;

        List<uint> argRegs = new();
        foreach (IRExpr arg in args)
        {
            uint r = EmitExpr(arg);
            uint saved = AllocLocal();
            Emit(RiscVEncoder.Mv(saved, r));
            argRegs.Add(saved);
        }

        int totalSize = (1 + args.Count) * 8;

        uint ptrReg = AllocTemp();
        Emit(RiscVEncoder.Mv(ptrReg, Reg.S1));
        Emit(RiscVEncoder.Addi(Reg.S1, Reg.S1, totalSize));

        uint tagReg = AllocTemp();
        foreach (uint insn in RiscVEncoder.Li(tagReg, tag)) Emit(insn);
        Emit(RiscVEncoder.Sd(ptrReg, tagReg, 0));

        for (int i = 0; i < argRegs.Count; i++)
            Emit(RiscVEncoder.Sd(ptrReg, argRegs[i], 8 + i * 8));

        return ptrReg;
    }

    uint EmitMatch(IRMatch match)
    {
        uint scrutReg = EmitExpr(match.Scrutinee);
        uint savedScrut = AllocLocal();
        Emit(RiscVEncoder.Mv(savedScrut, scrutReg));

        uint resultReg = AllocTemp();
        EmitMatchBranches(match, 0, savedScrut, resultReg);
        return resultReg;
    }

    void EmitMatchBranches(IRMatch match, int index, uint scrutReg, uint resultReg)
    {
        if (index >= match.Branches.Length)
            return;

        IRMatchBranch branch = match.Branches[index];

        switch (branch.Pattern)
        {
            case IRWildcardPattern:
            {
                uint bodyReg = EmitExpr(branch.Body);
                Emit(RiscVEncoder.Mv(resultReg, bodyReg));
                break;
            }

            case IRVarPattern varPat:
            {
                uint localReg = AllocLocal();
                Emit(RiscVEncoder.Mv(localReg, scrutReg));
                m_locals[varPat.Name] = localReg;
                uint bodyReg = EmitExpr(branch.Body);
                Emit(RiscVEncoder.Mv(resultReg, bodyReg));
                break;
            }

            case IRLiteralPattern litPat:
            {
                uint litReg = EmitLiteralValue(litPat.Value);
                uint cmpReg = AllocTemp();
                Emit(RiscVEncoder.Sub(cmpReg, scrutReg, litReg));
                Emit(RiscVEncoder.Sltu(cmpReg, Reg.Zero, cmpReg));

                int branchIdx = m_instructions.Count;
                Emit(RiscVEncoder.Nop());

                uint bodyReg = EmitExpr(branch.Body);
                Emit(RiscVEncoder.Mv(resultReg, bodyReg));
                int jumpEndIdx = m_instructions.Count;
                Emit(RiscVEncoder.Nop());

                int nextStart = m_instructions.Count;
                EmitMatchBranches(match, index + 1, scrutReg, resultReg);
                int endIdx = m_instructions.Count;

                m_instructions[branchIdx] = RiscVEncoder.Bne(cmpReg, Reg.Zero,
                    (nextStart - branchIdx) * 4);
                m_instructions[jumpEndIdx] = RiscVEncoder.J(
                    (endIdx - jumpEndIdx) * 4);
                break;
            }

            case IRCtorPattern ctorPat:
            {
                uint tagReg = AllocTemp();
                Emit(RiscVEncoder.Ld(tagReg, scrutReg, 0));

                int expectedTag = 0;
                if (ctorPat.Type is SumType sumType)
                {
                    for (int i = 0; i < sumType.Constructors.Length; i++)
                    {
                        if (sumType.Constructors[i].Name.Value == ctorPat.Name)
                        {
                            expectedTag = i;
                            break;
                        }
                    }
                }

                uint expectedReg = AllocTemp();
                foreach (uint insn in RiscVEncoder.Li(expectedReg, expectedTag)) Emit(insn);

                int branchIdx = m_instructions.Count;
                Emit(RiscVEncoder.Nop());

                for (int i = 0; i < ctorPat.SubPatterns.Length; i++)
                {
                    if (ctorPat.SubPatterns[i] is IRVarPattern varPat)
                    {
                        uint fieldReg = AllocLocal();
                        Emit(RiscVEncoder.Ld(fieldReg, scrutReg, 8 + i * 8));
                        m_locals[varPat.Name] = fieldReg;
                    }
                }

                uint bodyReg = EmitExpr(branch.Body);
                Emit(RiscVEncoder.Mv(resultReg, bodyReg));
                int jumpEndIdx = m_instructions.Count;
                Emit(RiscVEncoder.Nop());

                int nextStart = m_instructions.Count;
                EmitMatchBranches(match, index + 1, scrutReg, resultReg);
                int endIdx = m_instructions.Count;

                m_instructions[branchIdx] = RiscVEncoder.Bne(tagReg, expectedReg,
                    (nextStart - branchIdx) * 4);
                m_instructions[jumpEndIdx] = RiscVEncoder.J(
                    (endIdx - jumpEndIdx) * 4);
                break;
            }
        }
    }

    uint EmitRegion(IRRegion region)
    {
        // Records, sum types, lists: skip region (deep copy needed)
        if (region.Type is RecordType or SumType or ListType)
            return EmitExpr(region.Body);

        // Enter region: save heap ptr on stack
        Emit(RiscVEncoder.Addi(Reg.Sp, Reg.Sp, -8));
        Emit(RiscVEncoder.Sd(Reg.Sp, Reg.S1, 0));

        uint bodyReg = EmitExpr(region.Body);

        if (region.Type is TextType)
        {
            // Text escape: copy string to parent region before restoring heap ptr
            Emit(RiscVEncoder.Mv(Reg.T4, bodyReg));

            // Exit region (restore heap ptr)
            Emit(RiscVEncoder.Ld(Reg.S1, Reg.Sp, 0));
            Emit(RiscVEncoder.Addi(Reg.Sp, Reg.Sp, 8));

            // Load length and alloc in parent region
            Emit(RiscVEncoder.Ld(Reg.T5, Reg.T4, 0));
            Emit(RiscVEncoder.Mv(Reg.A0, Reg.S1));
            Emit(RiscVEncoder.Addi(Reg.T0, Reg.T5, 15));
            Emit(RiscVEncoder.Andi(Reg.T0, Reg.T0, -8));
            Emit(RiscVEncoder.Add(Reg.S1, Reg.S1, Reg.T0));
            Emit(RiscVEncoder.Sd(Reg.A0, Reg.T5, 0));

            // Copy bytes: src=t4+8, dst=a0+8
            Emit(RiscVEncoder.Addi(Reg.T0, Reg.T4, 8));
            Emit(RiscVEncoder.Addi(Reg.T1, Reg.A0, 8));
            foreach (uint insn in RiscVEncoder.Li(Reg.T2, 0)) Emit(insn);
            int loopStart = m_instructions.Count;
            int exitIdx = m_instructions.Count;
            Emit(RiscVEncoder.Nop());
            Emit(RiscVEncoder.Add(Reg.T3, Reg.T0, Reg.T2));
            Emit(RiscVEncoder.Lbu(Reg.T3, Reg.T3, 0));
            Emit(RiscVEncoder.Add(Reg.T6, Reg.T1, Reg.T2));
            Emit(RiscVEncoder.Sb(Reg.T6, Reg.T3, 0));
            Emit(RiscVEncoder.Addi(Reg.T2, Reg.T2, 1));
            Emit(RiscVEncoder.J((loopStart - m_instructions.Count) * 4));
            int exitTarget = m_instructions.Count;
            m_instructions[exitIdx] = RiscVEncoder.Bge(Reg.T2, Reg.T5,
                (exitTarget - exitIdx) * 4);

            return Reg.A0;
        }
        else
        {
            // Scalar return: value in register survives region exit
            Emit(RiscVEncoder.Ld(Reg.S1, Reg.Sp, 0));
            Emit(RiscVEncoder.Addi(Reg.Sp, Reg.Sp, 8));
            return bodyReg;
        }
    }

    uint EmitLiteralValue(object value) => value switch
    {
        long l => EmitIntegerLit(l),
        bool b => EmitIntegerLit(b ? 1 : 0),
        string s => EmitTextLit(s),
        _ => Reg.Zero
    };

    // ── Builtins ─────────────────────────────────────────────────

    bool TryEmitBuiltin(string name, List<IRExpr> args)
    {
        switch (name)
        {
            case "print-line" when args.Count == 1:
                EmitPrintLine(EmitExpr(args[0]), args[0].Type);
                return true;

            case "text-length" when args.Count == 1:
            {
                uint ptr = EmitExpr(args[0]);
                Emit(RiscVEncoder.Ld(Reg.A0, ptr, 0));
                return true;
            }

            case "integer-to-text" when args.Count == 1:
            {
                uint val = EmitExpr(args[0]);
                Emit(RiscVEncoder.Mv(Reg.A0, val));
                EmitCallTo("__itoa");
                return true;
            }

            case "text-to-integer" when args.Count == 1:
            {
                uint ptr = EmitExpr(args[0]);
                Emit(RiscVEncoder.Mv(Reg.A0, ptr));
                EmitCallTo("__text_to_int");
                return true;
            }

            case "char-at" when args.Count == 2:
                EmitCharAt(args);
                return true;

            case "substring" when args.Count == 3:
                EmitSubstring(args);
                return true;

            case "show" when args.Count == 1:
                EmitShow(args[0]);
                return true;

            default:
                return false;
        }
    }

    void EmitPrintLine(uint valueReg, CodexType type)
    {
        switch (type)
        {
            case IntegerType: EmitPrintI64(valueReg); break;
            case BooleanType: EmitPrintBool(valueReg); break;
            case TextType:    EmitPrintText(valueReg); break;
        }
    }

    void EmitPrintI64(uint valueReg)
    {
        // Save heap pointer (s1 is used as scratch in itoa)
        Emit(RiscVEncoder.Addi(Reg.Sp, Reg.Sp, -16));
        Emit(RiscVEncoder.Sd(Reg.Sp, Reg.S1, 0));

        // itoa on stack: divide by 10, store digits right-to-left, then write(2, buf, len)
        Emit(RiscVEncoder.Mv(Reg.S1, valueReg));
        Emit(RiscVEncoder.Addi(Reg.Sp, Reg.Sp, -32));

        // t0 = is_negative
        Emit(RiscVEncoder.Slt(Reg.T0, Reg.S1, Reg.Zero));
        int skipNegIndex = m_instructions.Count;
        Emit(RiscVEncoder.Nop());
        Emit(RiscVEncoder.Sub(Reg.S1, Reg.Zero, Reg.S1));
        int skipNegTarget = m_instructions.Count;
        m_instructions[skipNegIndex] = RiscVEncoder.Beq(Reg.T0, Reg.Zero,
            (skipNegTarget - skipNegIndex) * 4);

        // t1 = write cursor (fills right-to-left from sp+30)
        Emit(RiscVEncoder.Addi(Reg.T1, Reg.Sp, 30));

        // Newline at end
        foreach (uint insn in RiscVEncoder.Li(Reg.T2, '\n')) Emit(insn);
        Emit(RiscVEncoder.Sb(Reg.T1, Reg.T2, 0));
        Emit(RiscVEncoder.Addi(Reg.T1, Reg.T1, -1));

        // Zero special case
        int skipZeroIndex = m_instructions.Count;
        Emit(RiscVEncoder.Nop());
        foreach (uint insn in RiscVEncoder.Li(Reg.T2, '0')) Emit(insn);
        Emit(RiscVEncoder.Sb(Reg.T1, Reg.T2, 0));
        Emit(RiscVEncoder.Addi(Reg.T1, Reg.T1, -1));
        int skipZeroJumpIndex = m_instructions.Count;
        Emit(RiscVEncoder.Nop());

        // Digit loop
        int digitLoopStart = m_instructions.Count;
        m_instructions[skipZeroIndex] = RiscVEncoder.Bne(Reg.S1, Reg.Zero,
            (digitLoopStart - skipZeroIndex) * 4);

        int loopExitIndex = m_instructions.Count;
        Emit(RiscVEncoder.Nop());

        foreach (uint insn in RiscVEncoder.Li(Reg.T3, 10)) Emit(insn);
        Emit(RiscVEncoder.Rem(Reg.T2, Reg.S1, Reg.T3));
        Emit(RiscVEncoder.Addi(Reg.T2, Reg.T2, '0'));
        Emit(RiscVEncoder.Sb(Reg.T1, Reg.T2, 0));
        Emit(RiscVEncoder.Addi(Reg.T1, Reg.T1, -1));
        Emit(RiscVEncoder.Div(Reg.S1, Reg.S1, Reg.T3));
        Emit(RiscVEncoder.J((digitLoopStart - m_instructions.Count) * 4));

        int doneDigits = m_instructions.Count;
        m_instructions[loopExitIndex] = RiscVEncoder.Beq(Reg.S1, Reg.Zero,
            (doneDigits - loopExitIndex) * 4);
        m_instructions[skipZeroJumpIndex] = RiscVEncoder.J(
            (doneDigits - skipZeroJumpIndex) * 4);

        // Negative sign
        int skipMinusIndex = m_instructions.Count;
        Emit(RiscVEncoder.Nop());
        foreach (uint insn in RiscVEncoder.Li(Reg.T2, '-')) Emit(insn);
        Emit(RiscVEncoder.Sb(Reg.T1, Reg.T2, 0));
        Emit(RiscVEncoder.Addi(Reg.T1, Reg.T1, -1));
        int skipMinusTarget = m_instructions.Count;
        m_instructions[skipMinusIndex] = RiscVEncoder.Beq(Reg.T0, Reg.Zero,
            (skipMinusTarget - skipMinusIndex) * 4);

        // write(1, buf_start, len)
        Emit(RiscVEncoder.Addi(Reg.T1, Reg.T1, 1));
        Emit(RiscVEncoder.Addi(Reg.T2, Reg.Sp, 31));
        Emit(RiscVEncoder.Sub(Reg.A2, Reg.T2, Reg.T1));
        Emit(RiscVEncoder.Mv(Reg.A1, Reg.T1));
        EmitSyscallWrite();

        Emit(RiscVEncoder.Addi(Reg.Sp, Reg.Sp, 32));

        // Restore heap pointer
        Emit(RiscVEncoder.Ld(Reg.S1, Reg.Sp, 0));
        Emit(RiscVEncoder.Addi(Reg.Sp, Reg.Sp, 16));
    }

    void EmitPrintBool(uint valueReg)
    {
        int trueOffset = AddRodataString("True\n");
        int falseOffset = AddRodataString("False\n");

        int branchIndex = m_instructions.Count;
        Emit(RiscVEncoder.Nop());

        uint trueReg = AllocTemp();
        EmitLoadRodataAddress(trueReg, trueOffset);
        Emit(RiscVEncoder.Ld(Reg.A2, trueReg, 0));
        Emit(RiscVEncoder.Addi(Reg.A1, trueReg, 8));
        EmitSyscallWrite();
        int jEndIndex = m_instructions.Count;
        Emit(RiscVEncoder.Nop());

        int falseStart = m_instructions.Count;
        uint falseReg = AllocTemp();
        EmitLoadRodataAddress(falseReg, falseOffset);
        Emit(RiscVEncoder.Ld(Reg.A2, falseReg, 0));
        Emit(RiscVEncoder.Addi(Reg.A1, falseReg, 8));
        EmitSyscallWrite();

        int endIndex = m_instructions.Count;
        m_instructions[branchIndex] = RiscVEncoder.Beq(valueReg, Reg.Zero,
            (falseStart - branchIndex) * 4);
        m_instructions[jEndIndex] = RiscVEncoder.J((endIndex - jEndIndex) * 4);
    }

    void EmitPrintText(uint ptrReg)
    {
        Emit(RiscVEncoder.Ld(Reg.A2, ptrReg, 0));
        Emit(RiscVEncoder.Addi(Reg.A1, ptrReg, 8));
        EmitSyscallWrite();

        int nlOffset = AddRodataString("\n");
        uint nlReg = AllocTemp();
        EmitLoadRodataAddress(nlReg, nlOffset);
        Emit(RiscVEncoder.Ld(Reg.A2, nlReg, 0));
        Emit(RiscVEncoder.Addi(Reg.A1, nlReg, 8));
        EmitSyscallWrite();
    }

    int AddRodataString(string value)
    {
        if (m_stringOffsets.TryGetValue(value, out int offset))
            return offset;
        offset = m_rodata.Count;
        byte[] utf8 = Encoding.UTF8.GetBytes(value);
        m_rodata.AddRange(BitConverter.GetBytes((long)utf8.Length));
        m_rodata.AddRange(utf8);
        while (m_rodata.Count % 8 != 0) m_rodata.Add(0);
        m_stringOffsets[value] = offset;
        return offset;
    }

    void EmitCharAt(List<IRExpr> args)
    {
        uint textReg = EmitExpr(args[0]);
        uint savedText = AllocLocal();
        Emit(RiscVEncoder.Mv(savedText, textReg));
        uint indexReg = EmitExpr(args[1]);

        // Alloc 16 bytes: [8-byte len=1][1 byte data][7 padding]
        Emit(RiscVEncoder.Mv(Reg.A0, Reg.S1));
        Emit(RiscVEncoder.Addi(Reg.S1, Reg.S1, 16));

        foreach (uint insn in RiscVEncoder.Li(Reg.T0, 1)) Emit(insn);
        Emit(RiscVEncoder.Sd(Reg.A0, Reg.T0, 0));

        Emit(RiscVEncoder.Add(Reg.T0, savedText, indexReg));
        Emit(RiscVEncoder.Lbu(Reg.T0, Reg.T0, 8));
        Emit(RiscVEncoder.Sb(Reg.A0, Reg.T0, 8));
    }

    void EmitSubstring(List<IRExpr> args)
    {
        uint textReg = EmitExpr(args[0]);
        uint savedText = AllocLocal();
        Emit(RiscVEncoder.Mv(savedText, textReg));

        uint startReg = EmitExpr(args[1]);
        uint savedStart = AllocLocal();
        Emit(RiscVEncoder.Mv(savedStart, startReg));

        uint lenReg = EmitExpr(args[2]);
        Emit(RiscVEncoder.Mv(Reg.T5, lenReg));

        // Alloc: result = s1; s1 += align8(8 + len)
        Emit(RiscVEncoder.Mv(Reg.A0, Reg.S1));
        Emit(RiscVEncoder.Addi(Reg.T0, Reg.T5, 15));
        Emit(RiscVEncoder.Andi(Reg.T0, Reg.T0, -8));
        Emit(RiscVEncoder.Add(Reg.S1, Reg.S1, Reg.T0));

        Emit(RiscVEncoder.Sd(Reg.A0, Reg.T5, 0));

        // Copy: src = text + 8 + start, dst = result + 8
        Emit(RiscVEncoder.Add(Reg.T0, savedText, savedStart));
        Emit(RiscVEncoder.Addi(Reg.T0, Reg.T0, 8));
        Emit(RiscVEncoder.Addi(Reg.T1, Reg.A0, 8));

        foreach (uint insn in RiscVEncoder.Li(Reg.T2, 0)) Emit(insn);
        int loopStart = m_instructions.Count;
        int exitIdx = m_instructions.Count;
        Emit(RiscVEncoder.Nop());
        Emit(RiscVEncoder.Add(Reg.T3, Reg.T0, Reg.T2));
        Emit(RiscVEncoder.Lbu(Reg.T3, Reg.T3, 0));
        Emit(RiscVEncoder.Add(Reg.T4, Reg.T1, Reg.T2));
        Emit(RiscVEncoder.Sb(Reg.T4, Reg.T3, 0));
        Emit(RiscVEncoder.Addi(Reg.T2, Reg.T2, 1));
        Emit(RiscVEncoder.J((loopStart - m_instructions.Count) * 4));
        int exitTarget = m_instructions.Count;
        m_instructions[exitIdx] = RiscVEncoder.Bge(Reg.T2, Reg.T5, (exitTarget - exitIdx) * 4);
    }

    void EmitShow(IRExpr arg)
    {
        uint valReg = EmitExpr(arg);
        switch (arg.Type)
        {
            case IntegerType:
                Emit(RiscVEncoder.Mv(Reg.A0, valReg));
                EmitCallTo("__itoa");
                break;
            case BooleanType:
            {
                int trueOff = AddRodataString("True");
                int falseOff = AddRodataString("False");
                int brIdx = m_instructions.Count;
                Emit(RiscVEncoder.Nop());
                uint trReg = AllocTemp();
                EmitLoadRodataAddress(trReg, trueOff);
                Emit(RiscVEncoder.Mv(Reg.A0, trReg));
                int jIdx = m_instructions.Count;
                Emit(RiscVEncoder.Nop());
                int flLbl = m_instructions.Count;
                uint flReg = AllocTemp();
                EmitLoadRodataAddress(flReg, falseOff);
                Emit(RiscVEncoder.Mv(Reg.A0, flReg));
                int endLbl = m_instructions.Count;
                m_instructions[brIdx] = RiscVEncoder.Beq(valReg, Reg.Zero, (flLbl - brIdx) * 4);
                m_instructions[jIdx] = RiscVEncoder.J((endLbl - jIdx) * 4);
                break;
            }
            default:
                Emit(RiscVEncoder.Mv(Reg.A0, valReg));
                break;
        }
    }

    // ── Runtime helper functions ─────────────────────────────────

    void EmitRuntimeHelpers()
    {
        EmitStrEqHelper();
        EmitStrConcatHelper();
        EmitItoaHelper();
        EmitTextToIntHelper();
    }

    void EmitStrEqHelper()
    {
        // __str_eq: a0=ptr1, a1=ptr2 → a0=1 if equal, 0 if not
        // Uses only caller-saved registers — no prologue needed.
        m_functionOffsets["__str_eq"] = m_instructions.Count;

        Emit(RiscVEncoder.Ld(Reg.T0, Reg.A0, 0));    // t0 = len1
        Emit(RiscVEncoder.Ld(Reg.T1, Reg.A1, 0));    // t1 = len2

        int bneLen = m_instructions.Count;
        Emit(RiscVEncoder.Nop()); // patched: bne t0, t1 → not_eq

        Emit(RiscVEncoder.Addi(Reg.A0, Reg.A0, 8));  // data1
        Emit(RiscVEncoder.Addi(Reg.A1, Reg.A1, 8));  // data2
        foreach (uint insn in RiscVEncoder.Li(Reg.T2, 0)) Emit(insn);

        int loopStart = m_instructions.Count;
        int bgeIdx = m_instructions.Count;
        Emit(RiscVEncoder.Nop()); // patched: bge t2, t0 → equal

        Emit(RiscVEncoder.Add(Reg.T3, Reg.A0, Reg.T2));
        Emit(RiscVEncoder.Lbu(Reg.T3, Reg.T3, 0));
        Emit(RiscVEncoder.Add(Reg.T4, Reg.A1, Reg.T2));
        Emit(RiscVEncoder.Lbu(Reg.T4, Reg.T4, 0));

        int bneByte = m_instructions.Count;
        Emit(RiscVEncoder.Nop()); // patched: bne t3, t4 → not_eq

        Emit(RiscVEncoder.Addi(Reg.T2, Reg.T2, 1));
        Emit(RiscVEncoder.J((loopStart - m_instructions.Count) * 4));

        int equalLabel = m_instructions.Count;
        foreach (uint insn in RiscVEncoder.Li(Reg.A0, 1)) Emit(insn);
        Emit(RiscVEncoder.Ret());

        int notEqLabel = m_instructions.Count;
        foreach (uint insn in RiscVEncoder.Li(Reg.A0, 0)) Emit(insn);
        Emit(RiscVEncoder.Ret());

        m_instructions[bneLen] = RiscVEncoder.Bne(Reg.T0, Reg.T1, (notEqLabel - bneLen) * 4);
        m_instructions[bgeIdx] = RiscVEncoder.Bge(Reg.T2, Reg.T0, (equalLabel - bgeIdx) * 4);
        m_instructions[bneByte] = RiscVEncoder.Bne(Reg.T3, Reg.T4, (notEqLabel - bneByte) * 4);
    }

    void EmitStrConcatHelper()
    {
        // __str_concat: a0=ptr1, a1=ptr2 → a0=new string on heap
        m_functionOffsets["__str_concat"] = m_instructions.Count;

        Emit(RiscVEncoder.Ld(Reg.T0, Reg.A0, 0));    // t0 = len1
        Emit(RiscVEncoder.Ld(Reg.T1, Reg.A1, 0));    // t1 = len2
        Emit(RiscVEncoder.Mv(Reg.T4, Reg.A0));        // t4 = ptr1
        Emit(RiscVEncoder.Mv(Reg.T5, Reg.A1));        // t5 = ptr2
        Emit(RiscVEncoder.Add(Reg.T6, Reg.T0, Reg.T1)); // t6 = total_len

        // Alloc: a0 = result = s1; s1 += align8(8 + total)
        Emit(RiscVEncoder.Mv(Reg.A0, Reg.S1));
        Emit(RiscVEncoder.Addi(Reg.A1, Reg.T6, 15));
        Emit(RiscVEncoder.Andi(Reg.A1, Reg.A1, -8));
        Emit(RiscVEncoder.Add(Reg.S1, Reg.S1, Reg.A1));

        // Store total length
        Emit(RiscVEncoder.Sd(Reg.A0, Reg.T6, 0));

        // Copy first string: src=t4+8, dst=a0+8, len=t0
        Emit(RiscVEncoder.Addi(Reg.A1, Reg.A0, 8));   // a1 = dst
        Emit(RiscVEncoder.Addi(Reg.A2, Reg.T4, 8));   // a2 = src
        foreach (uint insn in RiscVEncoder.Li(Reg.T2, 0)) Emit(insn);

        int loop1Start = m_instructions.Count;
        int exit1Idx = m_instructions.Count;
        Emit(RiscVEncoder.Nop());
        Emit(RiscVEncoder.Add(Reg.T3, Reg.A2, Reg.T2));
        Emit(RiscVEncoder.Lbu(Reg.T3, Reg.T3, 0));
        Emit(RiscVEncoder.Add(Reg.A3, Reg.A1, Reg.T2));
        Emit(RiscVEncoder.Sb(Reg.A3, Reg.T3, 0));
        Emit(RiscVEncoder.Addi(Reg.T2, Reg.T2, 1));
        Emit(RiscVEncoder.J((loop1Start - m_instructions.Count) * 4));
        int exit1Target = m_instructions.Count;
        m_instructions[exit1Idx] = RiscVEncoder.Bge(Reg.T2, Reg.T0, (exit1Target - exit1Idx) * 4);

        // Copy second string: src=t5+8, dst=a1+t0 (= result+8+len1), len=t1
        Emit(RiscVEncoder.Add(Reg.A1, Reg.A1, Reg.T0));  // dst += len1
        Emit(RiscVEncoder.Addi(Reg.A2, Reg.T5, 8));      // src = ptr2+8
        foreach (uint insn in RiscVEncoder.Li(Reg.T2, 0)) Emit(insn);

        int loop2Start = m_instructions.Count;
        int exit2Idx = m_instructions.Count;
        Emit(RiscVEncoder.Nop());
        Emit(RiscVEncoder.Add(Reg.T3, Reg.A2, Reg.T2));
        Emit(RiscVEncoder.Lbu(Reg.T3, Reg.T3, 0));
        Emit(RiscVEncoder.Add(Reg.A3, Reg.A1, Reg.T2));
        Emit(RiscVEncoder.Sb(Reg.A3, Reg.T3, 0));
        Emit(RiscVEncoder.Addi(Reg.T2, Reg.T2, 1));
        Emit(RiscVEncoder.J((loop2Start - m_instructions.Count) * 4));
        int exit2Target = m_instructions.Count;
        m_instructions[exit2Idx] = RiscVEncoder.Bge(Reg.T2, Reg.T1, (exit2Target - exit2Idx) * 4);

        Emit(RiscVEncoder.Ret());
    }

    void EmitItoaHelper()
    {
        // __itoa: a0=integer → a0=ptr to length-prefixed string on heap
        m_functionOffsets["__itoa"] = m_instructions.Count;

        Emit(RiscVEncoder.Addi(Reg.Sp, Reg.Sp, -32));
        Emit(RiscVEncoder.Mv(Reg.T4, Reg.A0));           // t4 = value
        Emit(RiscVEncoder.Slt(Reg.T5, Reg.T4, Reg.Zero)); // t5 = is_negative

        int skipNeg = m_instructions.Count;
        Emit(RiscVEncoder.Nop()); // patched: beqz t5 → positive
        Emit(RiscVEncoder.Sub(Reg.T4, Reg.Zero, Reg.T4));
        int posLabel = m_instructions.Count;
        m_instructions[skipNeg] = RiscVEncoder.Beq(Reg.T5, Reg.Zero, (posLabel - skipNeg) * 4);

        Emit(RiscVEncoder.Addi(Reg.T0, Reg.Sp, 30));     // t0 = cursor
        foreach (uint insn in RiscVEncoder.Li(Reg.T1, 0)) Emit(insn); // t1 = count

        // Zero check
        int skipZero = m_instructions.Count;
        Emit(RiscVEncoder.Nop());
        foreach (uint insn in RiscVEncoder.Li(Reg.T2, '0')) Emit(insn);
        Emit(RiscVEncoder.Sb(Reg.T0, Reg.T2, 0));
        Emit(RiscVEncoder.Addi(Reg.T0, Reg.T0, -1));
        Emit(RiscVEncoder.Addi(Reg.T1, Reg.T1, 1));
        int jSign = m_instructions.Count;
        Emit(RiscVEncoder.Nop()); // j → sign_check

        int digitLoop = m_instructions.Count;
        m_instructions[skipZero] = RiscVEncoder.Bne(Reg.T4, Reg.Zero, (digitLoop - skipZero) * 4);

        int loopExit = m_instructions.Count;
        Emit(RiscVEncoder.Nop());
        foreach (uint insn in RiscVEncoder.Li(Reg.T3, 10)) Emit(insn);
        Emit(RiscVEncoder.Rem(Reg.T2, Reg.T4, Reg.T3));
        Emit(RiscVEncoder.Addi(Reg.T2, Reg.T2, '0'));
        Emit(RiscVEncoder.Sb(Reg.T0, Reg.T2, 0));
        Emit(RiscVEncoder.Addi(Reg.T0, Reg.T0, -1));
        Emit(RiscVEncoder.Addi(Reg.T1, Reg.T1, 1));
        Emit(RiscVEncoder.Div(Reg.T4, Reg.T4, Reg.T3));
        Emit(RiscVEncoder.J((digitLoop - m_instructions.Count) * 4));

        int signCheck = m_instructions.Count;
        m_instructions[loopExit] = RiscVEncoder.Beq(Reg.T4, Reg.Zero, (signCheck - loopExit) * 4);
        m_instructions[jSign] = RiscVEncoder.J((signCheck - jSign) * 4);

        int skipMinus = m_instructions.Count;
        Emit(RiscVEncoder.Nop());
        foreach (uint insn in RiscVEncoder.Li(Reg.T2, '-')) Emit(insn);
        Emit(RiscVEncoder.Sb(Reg.T0, Reg.T2, 0));
        Emit(RiscVEncoder.Addi(Reg.T0, Reg.T0, -1));
        Emit(RiscVEncoder.Addi(Reg.T1, Reg.T1, 1));
        int afterMinus = m_instructions.Count;
        m_instructions[skipMinus] = RiscVEncoder.Beq(Reg.T5, Reg.Zero, (afterMinus - skipMinus) * 4);

        // t0+1 = start of digits, t1 = length. Alloc on heap.
        Emit(RiscVEncoder.Addi(Reg.T0, Reg.T0, 1));
        Emit(RiscVEncoder.Mv(Reg.A0, Reg.S1));
        Emit(RiscVEncoder.Addi(Reg.T2, Reg.T1, 15));
        Emit(RiscVEncoder.Andi(Reg.T2, Reg.T2, -8));
        Emit(RiscVEncoder.Add(Reg.S1, Reg.S1, Reg.T2));

        Emit(RiscVEncoder.Sd(Reg.A0, Reg.T1, 0));

        // Copy from stack to heap
        foreach (uint insn in RiscVEncoder.Li(Reg.T2, 0)) Emit(insn);
        int copyLoop = m_instructions.Count;
        int copyExit = m_instructions.Count;
        Emit(RiscVEncoder.Nop());
        Emit(RiscVEncoder.Add(Reg.T3, Reg.T0, Reg.T2));
        Emit(RiscVEncoder.Lbu(Reg.T3, Reg.T3, 0));
        Emit(RiscVEncoder.Add(Reg.T4, Reg.A0, Reg.T2));
        Emit(RiscVEncoder.Sb(Reg.T4, Reg.T3, 8));
        Emit(RiscVEncoder.Addi(Reg.T2, Reg.T2, 1));
        Emit(RiscVEncoder.J((copyLoop - m_instructions.Count) * 4));
        int copyExitTarget = m_instructions.Count;
        m_instructions[copyExit] = RiscVEncoder.Bge(Reg.T2, Reg.T1, (copyExitTarget - copyExit) * 4);

        Emit(RiscVEncoder.Addi(Reg.Sp, Reg.Sp, 32));
        Emit(RiscVEncoder.Ret());
    }

    void EmitTextToIntHelper()
    {
        // __text_to_int: a0=ptr → a0=integer
        m_functionOffsets["__text_to_int"] = m_instructions.Count;

        Emit(RiscVEncoder.Ld(Reg.T0, Reg.A0, 0));       // t0 = length
        Emit(RiscVEncoder.Addi(Reg.A0, Reg.A0, 8));      // a0 = data
        foreach (uint insn in RiscVEncoder.Li(Reg.T1, 0)) Emit(insn); // t1 = result
        foreach (uint insn in RiscVEncoder.Li(Reg.T2, 0)) Emit(insn); // t2 = index
        foreach (uint insn in RiscVEncoder.Li(Reg.T3, 0)) Emit(insn); // t3 = is_negative

        // Check for '-'
        int skipNegCheck = m_instructions.Count;
        Emit(RiscVEncoder.Nop()); // beqz t0 → done (empty string)
        Emit(RiscVEncoder.Lbu(Reg.T4, Reg.A0, 0));
        foreach (uint insn in RiscVEncoder.Li(Reg.T5, 45)) Emit(insn); // '-'
        int skipMinus = m_instructions.Count;
        Emit(RiscVEncoder.Nop()); // bne t4, t5 → parse
        foreach (uint insn in RiscVEncoder.Li(Reg.T3, 1)) Emit(insn);
        Emit(RiscVEncoder.Addi(Reg.T2, Reg.T2, 1));

        int parseLoop = m_instructions.Count;
        m_instructions[skipMinus] = RiscVEncoder.Bne(Reg.T4, Reg.T5, (parseLoop - skipMinus) * 4);

        int parseExit = m_instructions.Count;
        Emit(RiscVEncoder.Nop()); // bge t2, t0 → negate
        Emit(RiscVEncoder.Add(Reg.T4, Reg.A0, Reg.T2));
        Emit(RiscVEncoder.Lbu(Reg.T4, Reg.T4, 0));
        Emit(RiscVEncoder.Addi(Reg.T4, Reg.T4, -48));
        foreach (uint insn in RiscVEncoder.Li(Reg.T5, 10)) Emit(insn);
        Emit(RiscVEncoder.Mul(Reg.T1, Reg.T1, Reg.T5));
        Emit(RiscVEncoder.Add(Reg.T1, Reg.T1, Reg.T4));
        Emit(RiscVEncoder.Addi(Reg.T2, Reg.T2, 1));
        Emit(RiscVEncoder.J((parseLoop - m_instructions.Count) * 4));

        int negateCheck = m_instructions.Count;
        m_instructions[parseExit] = RiscVEncoder.Bge(Reg.T2, Reg.T0, (negateCheck - parseExit) * 4);

        int skipNegate = m_instructions.Count;
        Emit(RiscVEncoder.Nop()); // beqz t3 → done
        Emit(RiscVEncoder.Sub(Reg.T1, Reg.Zero, Reg.T1));

        int doneLabel = m_instructions.Count;
        m_instructions[skipNegCheck] = RiscVEncoder.Beq(Reg.T0, Reg.Zero, (doneLabel - skipNegCheck) * 4);
        m_instructions[skipNegate] = RiscVEncoder.Beq(Reg.T3, Reg.Zero, (doneLabel - skipNegate) * 4);

        Emit(RiscVEncoder.Mv(Reg.A0, Reg.T1));
        Emit(RiscVEncoder.Ret());
    }

    // ── Syscalls / IO ────────────────────────────────────────────

    void EmitSyscallWrite()
    {
        if (m_target == RiscVTarget.BareMetal)
        {
            EmitUartWriteBuffer();
            return;
        }
        foreach (uint insn in RiscVEncoder.Li(Reg.A7, 64)) Emit(insn);  // SYS_write
        foreach (uint insn in RiscVEncoder.Li(Reg.A0, 1)) Emit(insn);   // stdout
        Emit(RiscVEncoder.Ecall());
    }

    void EmitSyscallExit(uint codeReg)
    {
        if (m_target == RiscVTarget.BareMetal)
        {
            // Bare metal: infinite loop (halt)
            Emit(RiscVEncoder.J(0)); // j .  (spin forever)
            return;
        }
        Emit(RiscVEncoder.Mv(Reg.A0, codeReg));
        foreach (uint insn in RiscVEncoder.Li(Reg.A7, 93)) Emit(insn);  // SYS_exit
        Emit(RiscVEncoder.Ecall());
    }

    // ── UART output (bare metal) ─────────────────────────────────

    void EmitUartWriteBuffer()
    {
        // On entry: a1 = buffer pointer, a2 = length
        // Uses t0 = UART base, t1 = end pointer, t2 = current byte
        foreach (uint insn in RiscVEncoder.Li(Reg.T0, UartBase)) Emit(insn);
        Emit(RiscVEncoder.Add(Reg.T1, Reg.A1, Reg.A2)); // t1 = buf + len

        int loopStart = m_instructions.Count;
        // if (a1 >= t1) break
        int exitIndex = m_instructions.Count;
        Emit(RiscVEncoder.Nop()); // patched: bge a1, t1 → exit

        // load byte, store to UART
        Emit(RiscVEncoder.Lbu(Reg.T2, Reg.A1, 0));
        Emit(RiscVEncoder.Sb(Reg.T0, Reg.T2, 0)); // UART THR at offset 0
        Emit(RiscVEncoder.Addi(Reg.A1, Reg.A1, 1));
        Emit(RiscVEncoder.J((loopStart - m_instructions.Count) * 4));

        int exitTarget = m_instructions.Count;
        m_instructions[exitIndex] = RiscVEncoder.Bge(Reg.A1, Reg.T1,
            (exitTarget - exitIndex) * 4);
    }

    // ── _start ───────────────────────────────────────────────────

    void EmitStart(IRModule module)
    {
        m_functionOffsets["__start"] = m_instructions.Count;

        if (m_target == RiscVTarget.BareMetal)
        {
            foreach (uint insn in RiscVEncoder.Li(Reg.Sp, 0x80100000L)) Emit(insn);
            foreach (uint insn in RiscVEncoder.Li(Reg.S1, 0x80200000L)) Emit(insn);
        }
        else
        {
            // Linux: allocate heap via brk syscall (214)
            foreach (uint insn in RiscVEncoder.Li(Reg.A0, 0)) Emit(insn);
            foreach (uint insn in RiscVEncoder.Li(Reg.A7, 214)) Emit(insn);
            Emit(RiscVEncoder.Ecall());
            Emit(RiscVEncoder.Mv(Reg.S1, Reg.A0));
            foreach (uint insn in RiscVEncoder.Li(Reg.T0, 1048576)) Emit(insn);
            Emit(RiscVEncoder.Add(Reg.A0, Reg.S1, Reg.T0));
            foreach (uint insn in RiscVEncoder.Li(Reg.A7, 214)) Emit(insn);
            Emit(RiscVEncoder.Ecall());
        }

        IRDefinition? mainDef = null;
        foreach (IRDefinition def in module.Definitions)
        {
            if (def.Name == "main") { mainDef = def; break; }
        }

        if (mainDef is null)
        {
            EmitSyscallExit(Reg.Zero);
            return;
        }

        EmitCallTo("main");

        CodexType returnType = ComputeReturnType(mainDef.Type, mainDef.Parameters.Length);
        switch (returnType)
        {
            case IntegerType: EmitPrintI64(Reg.A0); break;
            case BooleanType: EmitPrintBool(Reg.A0); break;
            case TextType:    EmitPrintText(Reg.A0); break;
        }

        EmitSyscallExit(Reg.Zero);
    }

    // ── Call patching ────────────────────────────────────────────

    void EmitCallTo(string targetName)
    {
        m_callPatches.Add((m_instructions.Count, targetName));
        Emit(RiscVEncoder.Nop());
    }

    void PatchCalls()
    {
        foreach ((int insnIndex, string target) in m_callPatches)
        {
            if (m_functionOffsets.TryGetValue(target, out int targetIndex))
                m_instructions[insnIndex] = RiscVEncoder.Call((targetIndex - insnIndex) * 4);
        }

        int textSizeBytes = m_instructions.Count * 4;
        ulong rodataVaddr = ElfWriter.ComputeRodataVaddr(textSizeBytes, m_target);

        foreach (RodataFixup fixup in m_rodataFixups)
        {
            long addr = (long)(rodataVaddr + (ulong)fixup.RodataOffset);
            uint[] insns = RiscVEncoder.Li(fixup.Register, addr);
            for (int i = 0; i < 2 && i < insns.Length; i++)
                m_instructions[fixup.InstructionIndex + i] = insns[i];
        }
    }

    // ── Register allocation ──────────────────────────────────────

    uint AllocTemp()
    {
        uint reg = m_nextTemp;
        m_nextTemp++;
        if (m_nextTemp > Reg.T6) m_nextTemp = Reg.T3;
        return reg;
    }

    uint AllocLocal()
    {
        uint reg = m_nextLocal;
        m_nextLocal++;
        if (m_nextLocal > Reg.S11) m_nextLocal = Reg.S11; // saturate — no wrap
        return reg;
    }

    void Emit(uint instruction) => m_instructions.Add(instruction);

    static CodexType ComputeReturnType(CodexType type, int paramCount)
    {
        CodexType current = type;
        for (int i = 0; i < paramCount; i++)
        {
            if (current is FunctionType ft) current = ft.Return;
            else break;
        }
        return current;
    }
}

sealed record RodataFixup(int InstructionIndex, uint Register, int RodataOffset);
