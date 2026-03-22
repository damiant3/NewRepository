using System.Text;
using Codex.Core;
using Codex.IR;
using Codex.Types;

namespace Codex.Emit.RiscV;

sealed class RiscVCodeGen
{
    readonly List<uint> m_instructions = new();
    readonly List<byte> m_rodata = new();

    // Function name → instruction index (offset = index * 4)
    readonly Dictionary<string, int> m_functionOffsets = new();

    // Forward references: (instruction index, target function name)
    // These are call sites where we emitted a placeholder jal and need to patch.
    readonly List<(int InsnIndex, string Target)> m_callPatches = new();

    // Register allocator state (trivial: next available temp)
    uint m_nextTemp = Reg.T0;

    // Callee-saved registers we use for locals/temps: s1-s11
    static readonly uint[] CalleeSaved = {
        Reg.S1, Reg.S2, Reg.S3, Reg.S4, Reg.S5, Reg.S6,
        Reg.S7, Reg.S8, Reg.S9, Reg.S10, Reg.S11
    };

    // Local variable name → register
    Dictionary<string, uint> m_locals = new();

    // String literal offset in rodata → already allocated
    readonly Dictionary<string, int> m_stringOffsets = new();

    public void EmitModule(IRModule module)
    {
        // Pre-register all function offsets (so forward calls work)
        // We'll emit them in order, but need to know names up front.
        // Actual offset patching happens after all code is emitted.

        // Emit each definition as a function
        foreach (IRDefinition def in module.Definitions)
        {
            EmitFunction(def);
        }

        // Emit _start: call main, print result, exit
        EmitStart(module);

        // Patch call targets
        PatchCalls();
    }

    public byte[] BuildElf()
    {
        // Encode instructions to bytes
        byte[] textSection = new byte[m_instructions.Count * 4];
        for (int i = 0; i < m_instructions.Count; i++)
        {
            uint insn = m_instructions[i];
            textSection[i * 4 + 0] = (byte)(insn & 0xFF);
            textSection[i * 4 + 1] = (byte)((insn >> 8) & 0xFF);
            textSection[i * 4 + 2] = (byte)((insn >> 16) & 0xFF);
            textSection[i * 4 + 3] = (byte)((insn >> 24) & 0xFF);
        }

        byte[] rodataSection = m_rodata.ToArray();

        // _start is the last function we emitted — find its offset
        int startOffset = m_functionOffsets["__start"] * 4;

        return ElfWriter.WriteExecutable(textSection, rodataSection, (ulong)startOffset);
    }

    // ── Function emission ────────────────────────────────────────

    void EmitFunction(IRDefinition def)
    {
        m_functionOffsets[def.Name] = m_instructions.Count;
        m_locals = new Dictionary<string, uint>();
        m_nextTemp = Reg.S2; // use callee-saved for locals

        // Prologue: save ra, and s0
        Emit(RiscVEncoder.Addi(Reg.Sp, Reg.Sp, -112));
        Emit(RiscVEncoder.Sd(Reg.Sp, Reg.Ra, 104));
        Emit(RiscVEncoder.Sd(Reg.Sp, Reg.S0, 96));

        // s1-s11: save all callee-saved registers (s1-s11)
        for (int i = 0; i < CalleeSaved.Length; i++)
        {
            Emit(RiscVEncoder.Sd(Reg.Sp, CalleeSaved[i], 88 - i * 8));
        }
        Emit(RiscVEncoder.Addi(Reg.S0, Reg.Sp, 112));

        // Map parameters to argument registers
        for (int i = 0; i < def.Parameters.Length && i < 8; i++)
        {
            uint argReg = Reg.A0 + (uint)i;
            uint savedReg = AllocLocal();
            Emit(RiscVEncoder.Mv(savedReg, argReg));
            m_locals[def.Parameters[i].Name] = savedReg;
        }

        // Emit body — result goes into a0
        uint resultReg = EmitExpr(def.Body);
        if (resultReg != Reg.A0)
            Emit(RiscVEncoder.Mv(Reg.A0, resultReg));

        // Epilogue: restore all callee-saved registers, ra, s0, return
        for (int i = 0; i < CalleeSaved.Length; i++)
        {
            Emit(RiscVEncoder.Ld(CalleeSaved[i], Reg.Sp, 88 - i * 8));
        }
        Emit(RiscVEncoder.Ld(Reg.Ra, Reg.Sp, 104));
        Emit(RiscVEncoder.Ld(Reg.S0, Reg.Sp, 96));
        Emit(RiscVEncoder.Addi(Reg.Sp, Reg.Sp, 112));
        Emit(RiscVEncoder.Ret());
    }

    // ── Expression emission (returns register holding result) ────

    uint EmitExpr(IRExpr expr)
    {
        switch (expr)
        {
            case IRIntegerLit intLit:
                return EmitIntegerLit(intLit.Value);

            case IRBoolLit boolLit:
                return EmitIntegerLit(boolLit.Value ? 1 : 0);

            case IRTextLit textLit:
                return EmitTextLit(textLit.Value);

            case IRName name:
                return EmitName(name);

            case IRBinary bin:
                return EmitBinary(bin);

            case IRIf ifExpr:
                return EmitIf(ifExpr);

            case IRLet letExpr:
                return EmitLet(letExpr);

            case IRApply apply:
                return EmitApply(apply);

            case IRNegate neg:
                return EmitNegate(neg);

            case IRDo doExpr:
                return EmitDo(doExpr);

            default:
                // Unsupported node — return zero
                return Reg.Zero;
        }
    }

    uint EmitIntegerLit(long value)
    {
        uint rd = AllocTemp();
        uint[] insns = RiscVEncoder.Li(rd, value);
        foreach (uint insn in insns)
            Emit(insn);
        return rd;
    }

    uint EmitTextLit(string value)
    {
        if (!m_stringOffsets.TryGetValue(value, out int rodataOffset))
        {
            rodataOffset = m_rodata.Count;
            byte[] utf8 = Encoding.UTF8.GetBytes(value);
            // Length-prefixed: 8 bytes for length (i64), then UTF-8 data
            m_rodata.AddRange(BitConverter.GetBytes((long)utf8.Length));
            m_rodata.AddRange(utf8);
            // Align to 8 bytes
            while (m_rodata.Count % 8 != 0) m_rodata.Add(0);
            m_stringOffsets[value] = rodataOffset;
        }

        // We need the runtime virtual address of this string.
        // We don't know the text section size yet, so emit a placeholder
        // that we'll patch later. For now, use a marker approach:
        // Store the rodata offset in a way we can resolve in BuildElf.
        // Simplification: compute approximate address using current code size.
        // This will be patched by a fixup pass.
        uint rd = AllocTemp();
        // Placeholder: load rodata offset as immediate. 
        // Actual address = rodata_vaddr + offset, resolved at patch time.
        EmitLoadRodataAddress(rd, rodataOffset);
        return rd;
    }

    void EmitLoadRodataAddress(uint rd, int rodataOffset)
    {
        // We don't know the final rodata vaddr yet.
        // Emit auipc + addi pair as placeholders — patched in BuildElf.
        // For now, emit li with a sentinel value that we'll fix up.
        // Use a simple approach: record fixup, emit nop slots.
        // Actually, simplest: just emit li with 0 and record a fixup.
        m_rodataFixups.Add(new RodataFixup(m_instructions.Count, rd, rodataOffset));
        // Reserve space for up to 2 instructions (lui + addi)
        Emit(RiscVEncoder.Nop());
        Emit(RiscVEncoder.Nop());
    }

    readonly List<RodataFixup> m_rodataFixups = new();

    uint EmitName(IRName name)
    {
        if (m_locals.TryGetValue(name.Name, out uint reg))
            return reg;

        // Might be a zero-arg function call
        if (m_functionOffsets.ContainsKey(name.Name) && name.Type is not FunctionType)
        {
            uint rd = AllocTemp();
            EmitCallTo(name.Name);
            Emit(RiscVEncoder.Mv(rd, Reg.A0));
            return rd;
        }

        return Reg.Zero;
    }

    uint EmitBinary(IRBinary bin)
    {
        uint left = EmitExpr(bin.Left);
        uint savedLeft = AllocTemp();
        Emit(RiscVEncoder.Mv(savedLeft, left));

        uint right = EmitExpr(bin.Right);

        uint rd = AllocTemp();

        switch (bin.Op)
        {
            case IRBinaryOp.AddInt:
                Emit(RiscVEncoder.Add(rd, savedLeft, right));
                break;
            case IRBinaryOp.SubInt:
                Emit(RiscVEncoder.Sub(rd, savedLeft, right));
                break;
            case IRBinaryOp.MulInt:
                Emit(RiscVEncoder.Mul(rd, savedLeft, right));
                break;
            case IRBinaryOp.DivInt:
                Emit(RiscVEncoder.Div(rd, savedLeft, right));
                break;
            case IRBinaryOp.Eq:
                // rd = (left == right) ? 1 : 0
                // sub tmp, left, right; seqz rd, tmp
                Emit(RiscVEncoder.Sub(rd, savedLeft, right));
                Emit(RiscVEncoder.Slti(rd, rd, 1)); // sltiu rd, rd, 1 (seqz pseudo)
                // Fix: seqz is sltiu rd, rs, 1
                // Actually for signed: xor then sltiu
                // Simple approach: sub, then check zero
                break;
            case IRBinaryOp.NotEq:
                Emit(RiscVEncoder.Sub(rd, savedLeft, right));
                Emit(RiscVEncoder.Sltu(rd, Reg.Zero, rd)); // snez rd, tmp
                break;
            case IRBinaryOp.Lt:
                Emit(RiscVEncoder.Slt(rd, savedLeft, right));
                break;
            case IRBinaryOp.Gt:
                Emit(RiscVEncoder.Slt(rd, right, savedLeft));
                break;
            case IRBinaryOp.LtEq:
                // !(right < left)
                Emit(RiscVEncoder.Slt(rd, right, savedLeft));
                Emit(RiscVEncoder.Xori(rd, rd, 1));
                break;
            case IRBinaryOp.GtEq:
                // !(left < right)
                Emit(RiscVEncoder.Slt(rd, savedLeft, right));
                Emit(RiscVEncoder.Xori(rd, rd, 1));
                break;
            case IRBinaryOp.And:
                Emit(RiscVEncoder.And(rd, savedLeft, right));
                break;
            case IRBinaryOp.Or:
                Emit(RiscVEncoder.Or(rd, savedLeft, right));
                break;
            default:
                Emit(RiscVEncoder.Mv(rd, Reg.Zero));
                break;
        }

        return rd;
    }

    uint EmitIf(IRIf ifExpr)
    {
        uint cond = EmitExpr(ifExpr.Condition);

        // beqz cond, else_branch
        int beqzIndex = m_instructions.Count;
        Emit(RiscVEncoder.Nop()); // placeholder for beq

        uint thenReg = EmitExpr(ifExpr.Then);
        uint resultReg = AllocTemp();
        Emit(RiscVEncoder.Mv(resultReg, thenReg));

        // j end
        int jEndIndex = m_instructions.Count;
        Emit(RiscVEncoder.Nop()); // placeholder for j

        // else:
        int elseStart = m_instructions.Count;
        uint elseReg = EmitExpr(ifExpr.Else);
        Emit(RiscVEncoder.Mv(resultReg, elseReg));

        // end:
        int endIndex = m_instructions.Count;

        // Patch beqz: branch to else if cond == 0
        int beqzOffset = (elseStart - beqzIndex) * 4;
        m_instructions[beqzIndex] = RiscVEncoder.Beq(cond, Reg.Zero, beqzOffset);

        // Patch j: jump to end
        int jOffset = (endIndex - jEndIndex) * 4;
        m_instructions[jEndIndex] = RiscVEncoder.J(jOffset);

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
        // Flatten curried application
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

            // Evaluate args into a0..a7
            List<uint> argRegs = new();
            foreach (IRExpr arg in args)
            {
                uint r = EmitExpr(arg);
                argRegs.Add(r);
            }

            // Move results into argument registers
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

    // ── Builtins ─────────────────────────────────────────────────

    bool TryEmitBuiltin(string name, List<IRExpr> args)
    {
        switch (name)
        {
            case "print-line" when args.Count == 1:
                uint argReg = EmitExpr(args[0]);
                CodexType argType = args[0].Type;
                EmitPrintLine(argReg, argType);
                return true;

            default:
                return false;
        }
    }

    void EmitPrintLine(uint valueReg, CodexType type)
    {
        switch (type)
        {
            case IntegerType:
                EmitPrintI64(valueReg);
                break;
            case BooleanType:
                EmitPrintBool(valueReg);
                break;
            case TextType:
                EmitPrintText(valueReg);
                break;
        }
    }

    void EmitPrintI64(uint valueReg)
    {
        // Convert integer to decimal string on stack, then write syscall
        // Strategy: divide repeatedly by 10, store digits right-to-left on stack,
        // then write(1, buf, len).

        // Save the value
        Emit(RiscVEncoder.Mv(Reg.S1, valueReg));

        // Allocate 24 bytes on stack for digit buffer
        Emit(RiscVEncoder.Addi(Reg.Sp, Reg.Sp, -32));

        // Handle negative
        // t0 = is_negative (1 if negative)
        Emit(RiscVEncoder.Slt(Reg.T0, Reg.S1, Reg.Zero));

        // if negative, negate
        int skipNegIndex = m_instructions.Count;
        Emit(RiscVEncoder.Nop()); // beq t0, zero, skip_neg
        Emit(RiscVEncoder.Sub(Reg.S1, Reg.Zero, Reg.S1)); // negate
        int skipNegTarget = m_instructions.Count;
        m_instructions[skipNegIndex] = RiscVEncoder.Beq(Reg.T0, Reg.Zero,
            (skipNegTarget - skipNegIndex) * 4);

        // t1 = buffer position (points to end, we fill right-to-left)
        Emit(RiscVEncoder.Addi(Reg.T1, Reg.Sp, 30)); // buf end

        // Store newline at the end
        uint[] nlInsns = RiscVEncoder.Li(Reg.T2, 10); // '\n'
        foreach (uint insn in nlInsns) Emit(insn);
        Emit(RiscVEncoder.Sb(Reg.T1, Reg.T2, 0));
        Emit(RiscVEncoder.Addi(Reg.T1, Reg.T1, -1));

        // Handle zero special case
        int skipZeroIndex = m_instructions.Count;
        Emit(RiscVEncoder.Nop()); // bne s1, zero, digit_loop
        // Store '0'
        uint[] zeroInsns = RiscVEncoder.Li(Reg.T2, 48); // '0'
        foreach (uint insn in zeroInsns) Emit(insn);
        Emit(RiscVEncoder.Sb(Reg.T1, Reg.T2, 0));
        Emit(RiscVEncoder.Addi(Reg.T1, Reg.T1, -1));
        int skipZeroJumpIndex = m_instructions.Count;
        Emit(RiscVEncoder.Nop()); // j done_digits

        // digit_loop:
        int digitLoopStart = m_instructions.Count;
        m_instructions[skipZeroIndex] = RiscVEncoder.Bne(Reg.S1, Reg.Zero,
            (digitLoopStart - skipZeroIndex) * 4);

        // while s1 != 0
        int loopExitIndex = m_instructions.Count;
        Emit(RiscVEncoder.Nop()); // beq s1, zero, done_digits

        // digit = s1 % 10
        uint[] tenInsns = RiscVEncoder.Li(Reg.T3, 10);
        foreach (uint insn in tenInsns) Emit(insn);
        Emit(RiscVEncoder.Rem(Reg.T2, Reg.S1, Reg.T3));
        // char = digit + '0'
        Emit(RiscVEncoder.Addi(Reg.T2, Reg.T2, 48));
        // store at buf position
        Emit(RiscVEncoder.Sb(Reg.T1, Reg.T2, 0));
        Emit(RiscVEncoder.Addi(Reg.T1, Reg.T1, -1));
        // s1 /= 10
        Emit(RiscVEncoder.Div(Reg.S1, Reg.S1, Reg.T3));
        // loop
        int loopBackOffset = (digitLoopStart - m_instructions.Count) * 4;
        Emit(RiscVEncoder.J(loopBackOffset));

        // done_digits:
        int doneDigits = m_instructions.Count;
        m_instructions[loopExitIndex] = RiscVEncoder.Beq(Reg.S1, Reg.Zero,
            (doneDigits - loopExitIndex) * 4);
        m_instructions[skipZeroJumpIndex] = RiscVEncoder.J(
            (doneDigits - skipZeroJumpIndex) * 4);

        // If negative, store '-'
        int skipMinusIndex = m_instructions.Count;
        Emit(RiscVEncoder.Nop()); // beq t0, zero, skip_minus
        uint[] minusInsns = RiscVEncoder.Li(Reg.T2, 45); // '-'
        foreach (uint insn in minusInsns) Emit(insn);
        Emit(RiscVEncoder.Sb(Reg.T1, Reg.T2, 0));
        Emit(RiscVEncoder.Addi(Reg.T1, Reg.T1, -1));
        int skipMinusTarget = m_instructions.Count;
        m_instructions[skipMinusIndex] = RiscVEncoder.Beq(Reg.T0, Reg.Zero,
            (skipMinusTarget - skipMinusIndex) * 4);

        // write(1, t1+1, sp+31 - (t1+1))
        Emit(RiscVEncoder.Addi(Reg.T1, Reg.T1, 1));  // buf start

        // len = sp + 31 - t1
        Emit(RiscVEncoder.Addi(Reg.T2, Reg.Sp, 31));
        Emit(RiscVEncoder.Sub(Reg.A2, Reg.T2, Reg.T1)); // len

        Emit(RiscVEncoder.Mv(Reg.A1, Reg.T1)); // buf
        EmitSyscallWrite();

        // Restore stack
        Emit(RiscVEncoder.Addi(Reg.Sp, Reg.Sp, 32));
    }

    void EmitPrintBool(uint valueReg)
    {
        // if value == 1 → print "True\n", else "False\n"
        int trueOffset = AddRodataString("True\n");
        int falseOffset = AddRodataString("False\n");

        int branchIndex = m_instructions.Count;
        Emit(RiscVEncoder.Nop()); // beq value, zero, false_branch

        // True branch
        uint trueReg = AllocTemp();
        EmitLoadRodataAddress(trueReg, trueOffset);
        Emit(RiscVEncoder.Ld(Reg.A2, trueReg, 0));  // length
        Emit(RiscVEncoder.Addi(Reg.A1, trueReg, 8)); // data
        EmitSyscallWrite();
        int jEndIndex = m_instructions.Count;
        Emit(RiscVEncoder.Nop()); // j end

        // False branch
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
        // ptrReg points to length-prefixed string: [i64 len][utf8 data...]
        Emit(RiscVEncoder.Ld(Reg.A2, ptrReg, 0));      // len
        Emit(RiscVEncoder.Addi(Reg.A1, ptrReg, 8));     // buf = ptr + 8
        EmitSyscallWrite();
        // Also write newline
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

    // ── Syscall helpers ──────────────────────────────────────────

    void EmitSyscallWrite()
    {
        // Linux RISC-V: write(fd, buf, len)
        // a7 = 64 (SYS_write), a0 = fd (1=stdout), a1 = buf, a2 = len
        uint[] sysInsns = RiscVEncoder.Li(Reg.A7, 64);
        foreach (uint insn in sysInsns) Emit(insn);
        uint[] fdInsns = RiscVEncoder.Li(Reg.A0, 1);
        foreach (uint insn in fdInsns) Emit(insn);
        Emit(RiscVEncoder.Ecall());
    }

    void EmitSyscallExit(uint codeReg)
    {
        // Linux RISC-V: exit(code)
        // a7 = 93 (SYS_exit), a0 = exit code
        Emit(RiscVEncoder.Mv(Reg.A0, codeReg));
        uint[] sysInsns = RiscVEncoder.Li(Reg.A7, 93);
        foreach (uint insn in sysInsns) Emit(insn);
        Emit(RiscVEncoder.Ecall());
    }

    // ── _start ───────────────────────────────────────────────────

    void EmitStart(IRModule module)
    {
        m_functionOffsets["__start"] = m_instructions.Count;

        // Find main
        IRDefinition? mainDef = null;
        foreach (IRDefinition def in module.Definitions)
        {
            if (def.Name == "main")
            {
                mainDef = def;
                break;
            }
        }

        if (mainDef is null)
        {
            EmitSyscallExit(Reg.Zero);
            return;
        }

        // Call main
        EmitCallTo("main");

        // Print result based on type
        CodexType returnType = ComputeReturnType(mainDef.Type, mainDef.Parameters.Length);
        switch (returnType)
        {
            case IntegerType:
                EmitPrintI64(Reg.A0);
                break;
            case BooleanType:
                EmitPrintBool(Reg.A0);
                break;
            case TextType:
                EmitPrintText(Reg.A0);
                break;
        }

        // exit(0)
        EmitSyscallExit(Reg.Zero);
    }

    // ── Call emission ────────────────────────────────────────────

    void EmitCallTo(string targetName)
    {
        // Save caller-saved temps we care about
        // For now, emit jal ra, <offset> with a patch entry
        m_callPatches.Add((m_instructions.Count, targetName));
        Emit(RiscVEncoder.Nop()); // placeholder — patched later
    }

    void PatchCalls()
    {
        foreach ((int insnIndex, string target) in m_callPatches)
        {
            if (m_functionOffsets.TryGetValue(target, out int targetIndex))
            {
                int offset = (targetIndex - insnIndex) * 4;
                m_instructions[insnIndex] = RiscVEncoder.Call(offset);
            }
        }

        // Patch rodata address fixups
        int textSizeBytes = m_instructions.Count * 4;
        ulong rodataVaddr = ElfWriter.ComputeRodataVaddr(textSizeBytes);

        foreach (RodataFixup fixup in m_rodataFixups)
        {
            long addr = (long)(rodataVaddr + (ulong)fixup.RodataOffset);
            uint[] insns = RiscVEncoder.Li(fixup.Register, addr);
            // We reserved 2 nop slots. Fill what we can.
            for (int i = 0; i < 2; i++)
            {
                if (i < insns.Length)
                    m_instructions[fixup.InstructionIndex + i] = insns[i];
            }
        }
    }

    // ── Register allocation (trivial for Phase 1) ────────────────

    uint AllocTemp()
    {
        uint reg = m_nextTemp;
        m_nextTemp++;
        // Wrap around using callee-saved range s2..s11 + temps
        if (m_nextTemp > Reg.S11)
            m_nextTemp = Reg.T3; // overflow into t3..t6
        if (m_nextTemp > Reg.T6)
            m_nextTemp = Reg.S2; // wrap
        return reg;
    }

    uint AllocLocal() => AllocTemp();

    // ── Helpers ──────────────────────────────────────────────────

    void Emit(uint instruction) => m_instructions.Add(instruction);

    static CodexType ComputeReturnType(CodexType type, int paramCount)
    {
        CodexType current = type;
        for (int i = 0; i < paramCount; i++)
        {
            if (current is FunctionType ft)
                current = ft.Return;
            else
                break;
        }
        return current;
    }
}

sealed record RodataFixup(int InstructionIndex, uint Register, int RodataOffset);
