using System.Collections.Immutable;
using System.Text;
using Codex.Core;
using Codex.IR;
using Codex.Types;

namespace Codex.Emit.Wasm;

sealed partial class WasmModuleBuilder
{
    // ── WASM Section IDs ─────────────────────────────────────────
    const byte SectionType = 1;
    const byte SectionImport = 2;
    const byte SectionFunction = 3;
    const byte SectionMemory = 5;
    const byte SectionGlobal = 6;
    const byte SectionExport = 7;
    const byte SectionStart = 8;
    const byte SectionCode = 10;
    const byte SectionData = 11;

    // ── WASM Value Types ─────────────────────────────────────────
    const byte WasmI32 = 0x7F;
    const byte WasmI64 = 0x7E;
    const byte WasmF64 = 0x7C;

    // ── WASM Op Codes ────────────────────────────────────────────
    const byte OpUnreachable = 0x00;
    const byte OpEnd = 0x0B;
    const byte OpReturn = 0x0F;
    const byte OpCall = 0x10;
    const byte OpDrop = 0x1A;
    const byte OpLocalGet = 0x20;
    const byte OpLocalSet = 0x21;
    const byte OpLocalTee = 0x22;
    const byte OpGlobalGet = 0x23;
    const byte OpGlobalSet = 0x24;
    const byte OpI32Load = 0x28;
    const byte OpI64Load = 0x29;
    const byte OpF64Load = 0x2B;
    const byte OpI32Load8U = 0x2D;
    const byte OpI32Store = 0x36;
    const byte OpI64Store = 0x37;
    const byte OpF64Store = 0x39;
    const byte OpI32Store8 = 0x3A;
    const byte OpI32Const = 0x41;
    const byte OpI64Const = 0x42;
    const byte OpF64Const = 0x44;
    const byte OpI32Eqz = 0x45;
    const byte OpI32Eq = 0x46;
    const byte OpI32Ne = 0x47;
    const byte OpI32LtS = 0x48;
    const byte OpI32GtS = 0x4A;
    const byte OpI32LeS = 0x4C;
    const byte OpI32GeS = 0x4E;
    const byte OpI64Eqz = 0x50;
    const byte OpI64Eq = 0x51;
    const byte OpI64Ne = 0x52;
    const byte OpI64LtS = 0x53;
    const byte OpI64GtS = 0x55;
    const byte OpI64LeS = 0x57;
    const byte OpI64GeS = 0x59;
    const byte OpF64Eq = 0x61;
    const byte OpF64Ne = 0x62;
    const byte OpF64Lt = 0x63;
    const byte OpF64Gt = 0x64;
    const byte OpF64Le = 0x65;
    const byte OpF64Ge = 0x66;
    const byte OpI32Add = 0x6A;
    const byte OpI32Sub = 0x6B;
    const byte OpI32Mul = 0x6C;
    const byte OpI64Add = 0x7C;
    const byte OpI64Sub = 0x7D;
    const byte OpI64Mul = 0x7E;
    const byte OpI64DivS = 0x7F;
    const byte OpF64Add = 0xA0;
    const byte OpF64Sub = 0xA1;
    const byte OpF64Mul = 0xA2;
    const byte OpF64Div = 0xA3;
    const byte OpI64ExtendI32S = 0xAC;
    const byte OpI32WrapI64 = 0xA7;
    const byte OpF64ConvertI64S = 0xB9;

    // ── WASM Block/If opcodes ────────────────────────────────────
    const byte OpBlock = 0x02;
    const byte OpLoop = 0x03;
    const byte OpIf = 0x04;
    const byte OpElse = 0x05;
    const byte OpBr = 0x0C;
    const byte OpBrIf = 0x0D;
    const byte OpBrTable = 0x0E;

    // ── WASM block types ─────────────────────────────────────────
    const byte BlockTypeVoid = 0x40;
    const byte BlockTypeI32 = 0x7F;
    const byte BlockTypeI64 = 0x7E;
    const byte BlockTypeF64 = 0x7C;

    // ── Export kinds ─────────────────────────────────────────────
    const byte ExportFunc = 0x00;
    const byte ExportMemory = 0x02;

    // ── Import kinds ─────────────────────────────────────────────
    const byte ImportFunc = 0x00;

    // ── Global mutability ────────────────────────────────────────
    const byte GlobalConst = 0x00;
    const byte GlobalMut = 0x01;

    // ── Data ─────────────────────────────────────────────────────
    readonly List<byte[]> m_dataSegments = new();
    int m_dataOffset = 1024; // data starts at offset 1024, leaving room for iov structs

    readonly List<WasmFuncType> m_types = new();
    readonly List<WasmImport> m_imports = new();
    readonly List<int> m_functionTypeIndices = new();
    readonly List<byte[]> m_functionBodies = new();
    readonly List<WasmExport> m_exports = new();
    readonly List<WasmGlobal> m_globals = new();

    ValueMap<string, int> m_functionIndex = ValueMap<string, int>.s_empty;
    ValueMap<string, int> m_stringOffsets = ValueMap<string, int>.s_empty;
    ValueMap<string, int> m_stringLengths = ValueMap<string, int>.s_empty;

    int m_nextFuncIndex;
    int m_importCount;

    // ── WASI import function indices ─────────────────────────────
    int m_fdWriteIndex;

    // ── Heap pointer global index ────────────────────────────────
    int m_heapPtrGlobalIndex;

    // ── Runtime helper function indices ──────────────────────────
    int m_printI64Index;
    int m_printBoolIndex;

    public void EmitModule(IRModule module)
    {
        EmitImports();
        EmitRuntimeGlobals();
        PreRegisterFunctions(module);
        EmitRuntimeHelpers();

        foreach (IRDefinition def in module.Definitions)
        {
            EmitDefinition(def);
        }

        EmitStartFunction(module);
    }

    public byte[] Build()
    {
        MemoryStream ms = new();
        BinaryWriter w = new(ms);

        // Magic + version
        w.Write((byte)0x00); w.Write((byte)0x61); w.Write((byte)0x73); w.Write((byte)0x6D);
        w.Write((byte)0x01); w.Write((byte)0x00); w.Write((byte)0x00); w.Write((byte)0x00);

        WriteTypeSection(w);
        WriteImportSection(w);
        WriteFunctionSection(w);
        WriteMemorySection(w);
        WriteGlobalSection(w);
        WriteExportSection(w);
        WriteCodeSection(w);
        WriteDataSection(w);

        return ms.ToArray();
    }

    // ── Imports ──────────────────────────────────────────────────

    void EmitImports()
    {
        // fd_write(fd: i32, iovs: i32, iovs_len: i32, nwritten: i32) -> i32
        int fdWriteType = AddFuncType(
            new byte[] { WasmI32, WasmI32, WasmI32, WasmI32 },
            new byte[] { WasmI32 });
        m_fdWriteIndex = m_nextFuncIndex++;
        m_imports.Add(new WasmImport("wasi_snapshot_preview1", "fd_write", ImportFunc, fdWriteType));
        m_importCount = m_nextFuncIndex;
    }

    void EmitRuntimeGlobals()
    {
        // Heap pointer — starts after data segments (will be patched in Build)
        m_heapPtrGlobalIndex = m_globals.Count;
        m_globals.Add(new WasmGlobal(WasmI32, GlobalMut, m_dataOffset));
    }

    // ── Pre-register functions ───────────────────────────────────

    void PreRegisterFunctions(IRModule module)
    {
        // Reserve indices for runtime helpers first
        m_printI64Index = m_nextFuncIndex++;
        m_functionTypeIndices.Add(-1); // placeholder, filled by EmitRuntimeHelpers

        m_printBoolIndex = m_nextFuncIndex++;
        m_functionTypeIndices.Add(-1); // placeholder

        // Reserve indices for user definitions
        foreach (IRDefinition def in module.Definitions)
        {
            m_functionIndex = m_functionIndex.Set(def.Name, m_nextFuncIndex);
            m_nextFuncIndex++;
            m_functionTypeIndices.Add(-1); // placeholder
        }

        // Reserve index for _start
        m_functionIndex = m_functionIndex.Set("__wasm_start", m_nextFuncIndex);
        m_nextFuncIndex++;
        m_functionTypeIndices.Add(-1); // placeholder
    }

    // ── Emit a definition ────────────────────────────────────────

    void EmitDefinition(IRDefinition def)
    {
        int paramCount = def.Parameters.Length;
        CodexType returnType = ComputeReturnType(def.Type, paramCount);

        byte[] paramTypes = new byte[paramCount];
        for (int i = 0; i < paramCount; i++)
        {
            paramTypes[i] = WasmTypeFor(def.Parameters[i].Type);
        }

        byte[] resultTypes;
        if (returnType is VoidType or NothingType)
            resultTypes = Array.Empty<byte>();
        else
            resultTypes = new byte[] { WasmTypeFor(returnType) };

        int typeIndex = AddFuncType(paramTypes, resultTypes);

        int funcSlot = m_functionIndex.Get(def.Name, 0) - m_importCount;
        m_functionTypeIndices[funcSlot] = typeIndex;

        // Build locals list: parameters are implicit (local 0..n-1)
        // Additional locals declared during body emit
        List<byte> localTypes = new();
        ValueMap<string, int> localMap = ValueMap<string, int>.s_empty;
        for (int i = 0; i < paramCount; i++)
        {
            localMap = localMap.Set(def.Parameters[i].Name, i);
        }

        MemoryStream body = new();
        int nextLocal = paramCount;

        EmitExpr(body, def.Body, localMap, ref nextLocal, localTypes, returnType);

        body.WriteByte(OpEnd);

        // Encode function body with locals
        byte[] bodyBytes = EncodeFunctionBody(body.ToArray(), localTypes);
        m_functionBodies.Add(bodyBytes);
    }

    void EmitStartFunction(IRModule module)
    {
        // _start calls main and prints the result
        IRDefinition? mainDef = null;
        foreach (IRDefinition def in module.Definitions)
        {
            if (def.Name == "main")
            {
                mainDef = def;
                break;
            }
        }

        int startSlot = m_functionIndex.Get("__wasm_start", 0) - m_importCount;

        if (mainDef is null)
        {
            // Empty start
            int voidType = AddFuncType(Array.Empty<byte>(), Array.Empty<byte>());
            m_functionTypeIndices[startSlot] = voidType;
            MemoryStream body = new();
            body.WriteByte(OpEnd);
            m_functionBodies.Add(EncodeFunctionBody(body.ToArray(), new List<byte>()));
        }
        else
        {
            int voidType = AddFuncType(Array.Empty<byte>(), Array.Empty<byte>());
            m_functionTypeIndices[startSlot] = voidType;

            MemoryStream body = new();
            List<byte> localTypes = new();
            CodexType returnType = ComputeReturnType(mainDef.Type, mainDef.Parameters.Length);

            // Call main
            int mainIdx = m_functionIndex.Get("main", 0);

            body.WriteByte(OpCall);
            WriteUnsignedLeb128(body, mainIdx);

            // Print result based on type
            EmitPrintResult(body, returnType, localTypes);

            body.WriteByte(OpEnd);
            m_functionBodies.Add(EncodeFunctionBody(body.ToArray(), localTypes));
        }

        // Export _start
        int startFuncIndex = m_functionIndex.Get("__wasm_start", 0);
        m_exports.Add(new WasmExport("_start", ExportFunc, startFuncIndex));
        m_exports.Add(new WasmExport("memory", ExportMemory, 0));
    }

    void EmitPrintResult(MemoryStream body, CodexType type, List<byte> localTypes)
    {
        switch (type)
        {
            case IntegerType:
                body.WriteByte(OpCall);
                WriteUnsignedLeb128(body, m_printI64Index);
                break;

            case BooleanType:
                body.WriteByte(OpCall);
                WriteUnsignedLeb128(body, m_printBoolIndex);
                break;

            case TextType:
                // Stack has i32 (ptr to length-prefixed string)
                // Store ptr in a temp local
                int ptrLocal = localTypes.Count;
                localTypes.Add(WasmI32);

                body.WriteByte(OpLocalSet);
                WriteUnsignedLeb128(body, ptrLocal);

                EmitFdWriteFromLengthPrefixed(body, ptrLocal);
                EmitWriteNewline(body, localTypes);
                break;

            case VoidType or NothingType:
                break;

            default:
                // For unknown types, drop the value
                body.WriteByte(OpDrop);
                break;
        }
    }

    // ── Expression emission ──────────────────────────────────────

    void EmitExpr(MemoryStream body, IRExpr expr,
        ValueMap<string, int> localMap, ref int nextLocal,
        List<byte> localTypes, CodexType expectedReturn)
    {
        switch (expr)
        {
            case IRIntegerLit intLit:
                body.WriteByte(OpI64Const);
                WriteSignedLeb128(body, intLit.Value);
                break;

            case IRNumberLit numLit:
                body.WriteByte(OpF64Const);
                byte[] f64Bytes = BitConverter.GetBytes((double)numLit.Value);
                body.Write(f64Bytes, 0, 8);
                break;

            case IRBoolLit boolLit:
                body.WriteByte(OpI32Const);
                WriteSignedLeb128(body, boolLit.Value ? 1 : 0);
                break;

            case IRTextLit textLit:
                EmitTextLiteral(body, textLit.Value);
                break;

            case IRName name:
                EmitName(body, name, localMap);
                break;

            case IRBinary bin:
                EmitBinary(body, bin, localMap, ref nextLocal, localTypes);
                break;

            case IRIf ifExpr:
                EmitIf(body, ifExpr, localMap, ref nextLocal, localTypes);
                break;

            case IRLet letExpr:
                EmitLet(body, letExpr, localMap, ref nextLocal, localTypes);
                break;

            case IRApply apply:
                EmitApply(body, apply, localMap, ref nextLocal, localTypes);
                break;

            case IRDo doExpr:
                EmitDo(body, doExpr, localMap, ref nextLocal, localTypes);
                break;

            case IRNegate neg:
                // 0 - operand
                body.WriteByte(OpI64Const);
                WriteSignedLeb128(body, 0);
                EmitExpr(body, neg.Operand, localMap, ref nextLocal, localTypes, neg.Type);
                body.WriteByte(OpI64Sub);
                break;

            case IRMatch match:
                EmitMatch(body, match, localMap, ref nextLocal, localTypes);
                break;
        }
    }

    void EmitName(MemoryStream body, IRName name, ValueMap<string, int> localMap)
    {
        if (localMap.TryGet(name.Name, out int localIdx))
        {
            body.WriteByte(OpLocalGet);
            WriteUnsignedLeb128(body, localIdx);
        }
        else if (m_functionIndex.TryGet(name.Name, out int funcIdx))
        {
            // Zero-arg function call
            if (name.Type is not FunctionType)
            {
                body.WriteByte(OpCall);
                WriteUnsignedLeb128(body, funcIdx);
            }
        }
    }

    void EmitBinary(MemoryStream body, IRBinary bin,
        ValueMap<string, int> localMap, ref int nextLocal, List<byte> localTypes)
    {
        if (bin.Op == IRBinaryOp.AppendText)
        {
            EmitTextAppend(body, bin.Left, bin.Right, localMap, ref nextLocal, localTypes);
            return;
        }

        EmitExpr(body, bin.Left, localMap, ref nextLocal, localTypes, bin.Left.Type);
        EmitExpr(body, bin.Right, localMap, ref nextLocal, localTypes, bin.Right.Type);

        switch (bin.Op)
        {
            case IRBinaryOp.AddInt: body.WriteByte(OpI64Add); break;
            case IRBinaryOp.SubInt: body.WriteByte(OpI64Sub); break;
            case IRBinaryOp.MulInt: body.WriteByte(OpI64Mul); break;
            case IRBinaryOp.DivInt: body.WriteByte(OpI64DivS); break;
            case IRBinaryOp.AddNum: body.WriteByte(OpF64Add); break;
            case IRBinaryOp.SubNum: body.WriteByte(OpF64Sub); break;
            case IRBinaryOp.MulNum: body.WriteByte(OpF64Mul); break;
            case IRBinaryOp.DivNum: body.WriteByte(OpF64Div); break;
            case IRBinaryOp.Eq:
                EmitEq(body, bin.Left.Type);
                break;
            case IRBinaryOp.NotEq:
                EmitEq(body, bin.Left.Type);
                body.WriteByte(OpI32Eqz);
                break;
            case IRBinaryOp.Lt:
                if (bin.Left.Type is NumberType) body.WriteByte(OpF64Lt);
                else body.WriteByte(OpI64LtS);
                break;
            case IRBinaryOp.Gt:
                if (bin.Left.Type is NumberType) body.WriteByte(OpF64Gt);
                else body.WriteByte(OpI64GtS);
                break;
            case IRBinaryOp.LtEq:
                if (bin.Left.Type is NumberType) body.WriteByte(OpF64Le);
                else body.WriteByte(OpI64LeS);
                break;
            case IRBinaryOp.GtEq:
                if (bin.Left.Type is NumberType) body.WriteByte(OpF64Ge);
                else body.WriteByte(OpI64GeS);
                break;
            case IRBinaryOp.And:
                body.WriteByte(OpI32Mul); // both are i32 0/1
                break;
            case IRBinaryOp.Or:
                body.WriteByte(OpI32Add);
                body.WriteByte(OpI32Const); WriteSignedLeb128(body, 0);
                body.WriteByte(OpI32Ne);
                break;
        }
    }

    void EmitEq(MemoryStream body, CodexType type)
    {
        switch (type)
        {
            case NumberType:
                body.WriteByte(OpF64Eq);
                break;
            case BooleanType:
                body.WriteByte(OpI32Eq);
                break;
            case TextType:
                // String equality: compare length-prefixed byte sequences
                // For now, compare pointers (identity). Full string eq is Phase 2.
                body.WriteByte(OpI32Eq);
                break;
            default:
                body.WriteByte(OpI64Eq);
                break;
        }
    }

    void EmitIf(MemoryStream body, IRIf ifExpr,
        ValueMap<string, int> localMap, ref int nextLocal, List<byte> localTypes)
    {
        EmitExpr(body, ifExpr.Condition, localMap, ref nextLocal, localTypes, BooleanType.s_instance);

        // If condition is i64 (from comparison), wrap to i32
        if (ifExpr.Condition.Type is IntegerType)
        {
            body.WriteByte(OpI32WrapI64);
        }

        byte blockType = WasmBlockTypeFor(ifExpr.Type);
        body.WriteByte(OpIf);
        body.WriteByte(blockType);

        EmitExpr(body, ifExpr.Then, localMap, ref nextLocal, localTypes, ifExpr.Type);

        body.WriteByte(OpElse);

        EmitExpr(body, ifExpr.Else, localMap, ref nextLocal, localTypes, ifExpr.Type);

        body.WriteByte(OpEnd);
    }

    void EmitLet(MemoryStream body, IRLet letExpr,
        ValueMap<string, int> localMap, ref int nextLocal, List<byte> localTypes)
    {
        EmitExpr(body, letExpr.Value, localMap, ref nextLocal, localTypes, letExpr.NameType);

        int localIdx = nextLocal++;
        localTypes.Add(WasmTypeFor(letExpr.NameType));
        body.WriteByte(OpLocalSet);
        WriteUnsignedLeb128(body, localIdx);

        ValueMap<string, int> innerMap = localMap.Set(letExpr.Name, localIdx);
        EmitExpr(body, letExpr.Body, innerMap, ref nextLocal, localTypes, letExpr.Body.Type);
    }

    void EmitApply(MemoryStream body, IRApply apply,
        ValueMap<string, int> localMap, ref int nextLocal, List<byte> localTypes)
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
            if (TryEmitBuiltin(body, funcName.Name, args, localMap, ref nextLocal, localTypes))
                return;

            if (m_functionIndex.TryGet(funcName.Name, out int funcIdx))
            {
                foreach (IRExpr arg in args)
                {
                    EmitExpr(body, arg, localMap, ref nextLocal, localTypes, arg.Type);
                }
                body.WriteByte(OpCall);
                WriteUnsignedLeb128(body, funcIdx);
            }
        }
    }

    void EmitDo(MemoryStream body, IRDo doExpr,
        ValueMap<string, int> localMap, ref int nextLocal, List<byte> localTypes)
    {
        for (int i = 0; i < doExpr.Statements.Length; i++)
        {
            IRDoStatement stmt = doExpr.Statements[i];
            switch (stmt)
            {
                case IRDoExec doExecStmt:
                    EmitExpr(body, doExecStmt.Expression, localMap, ref nextLocal, localTypes, doExecStmt.Expression.Type);
                    // Drop result if not the last statement or if void
                    if (i < doExpr.Statements.Length - 1 && doExecStmt.Expression.Type is not (VoidType or NothingType))
                        body.WriteByte(OpDrop);
                    break;

                case IRDoBind doBind:
                    EmitExpr(body, doBind.Value, localMap, ref nextLocal, localTypes, doBind.NameType);
                    int bindLocal = nextLocal++;
                    localTypes.Add(WasmTypeFor(doBind.NameType));
                    body.WriteByte(OpLocalSet);
                    WriteUnsignedLeb128(body, bindLocal);
                    localMap = localMap.Set(doBind.Name, bindLocal);
                    break;
            }
        }
    }

    void EmitMatch(MemoryStream body, IRMatch match,
        ValueMap<string, int> localMap, ref int nextLocal, List<byte> localTypes)
    {
        // Evaluate scrutinee and store in local
        EmitExpr(body, match.Scrutinee, localMap, ref nextLocal, localTypes, match.Scrutinee.Type);
        int scrutLocal = nextLocal++;
        localTypes.Add(WasmTypeFor(match.Scrutinee.Type));
        body.WriteByte(OpLocalSet);
        WriteUnsignedLeb128(body, scrutLocal);

        // Emit as nested if/else chain
        byte blockType = WasmBlockTypeFor(match.Type);
        EmitMatchBranches(body, match.Branches, 0, scrutLocal, localMap,
            ref nextLocal, localTypes, blockType);
    }

    void EmitMatchBranches(MemoryStream body, ImmutableArray<IRMatchBranch> branches,
        int index, int scrutLocal, ValueMap<string, int> localMap,
        ref int nextLocal, List<byte> localTypes, byte blockType)
    {
        if (index >= branches.Length)
        {
            body.WriteByte(OpUnreachable);
            return;
        }

        IRMatchBranch branch = branches[index];
        bool isLast = index == branches.Length - 1;

        switch (branch.Pattern)
        {
            case IRWildcardPattern:
                EmitExpr(body, branch.Body, localMap, ref nextLocal, localTypes, branch.Body.Type);
                break;

            case IRVarPattern varPat:
                body.WriteByte(OpLocalGet);
                WriteUnsignedLeb128(body, scrutLocal);
                int varLocal = nextLocal++;
                localTypes.Add(WasmTypeFor(varPat.Type));
                body.WriteByte(OpLocalSet);
                WriteUnsignedLeb128(body, varLocal);
                ValueMap<string, int> varMap = localMap.Set(varPat.Name, varLocal);
                EmitExpr(body, branch.Body, varMap, ref nextLocal, localTypes, branch.Body.Type);
                break;

            case IRLiteralPattern litPat:
                // Load scrutinee and compare
                body.WriteByte(OpLocalGet);
                WriteUnsignedLeb128(body, scrutLocal);
                EmitLiteralValue(body, litPat.Value);
                EmitEq(body, litPat.Type);

                body.WriteByte(OpIf);
                body.WriteByte(blockType);
                EmitExpr(body, branch.Body, localMap, ref nextLocal, localTypes, branch.Body.Type);
                body.WriteByte(OpElse);
                EmitMatchBranches(body, branches, index + 1, scrutLocal, localMap,
                    ref nextLocal, localTypes, blockType);
                body.WriteByte(OpEnd);
                break;

            case IRCtorPattern:
                // Constructor pattern matching — Phase 3 (types)
                // For now, fall through to next branch
                EmitMatchBranches(body, branches, index + 1, scrutLocal, localMap,
                    ref nextLocal, localTypes, blockType);
                break;
        }
    }

    void EmitLiteralValue(MemoryStream body, object value)
    {
        switch (value)
        {
            case long l:
                body.WriteByte(OpI64Const);
                WriteSignedLeb128(body, l);
                break;
            case bool b:
                body.WriteByte(OpI32Const);
                WriteSignedLeb128(body, b ? 1 : 0);
                break;
            case string s:
                EmitTextLiteral(body, s);
                break;
        }
    }

    // ── Builtins ─────────────────────────────────────────────────

    bool TryEmitBuiltin(MemoryStream body, string name, List<IRExpr> args,
        ValueMap<string, int> localMap, ref int nextLocal, List<byte> localTypes)
    {
        switch (name)
        {
            case "print-line" when args.Count == 1:
                EmitExpr(body, args[0], localMap, ref nextLocal, localTypes, args[0].Type);
                EmitPrintLineForType(body, args[0].Type, ref nextLocal, localTypes);
                return true;

            case "show" when args.Count == 1:
                EmitExpr(body, args[0], localMap, ref nextLocal, localTypes, args[0].Type);
                EmitShow(body, args[0].Type, ref nextLocal, localTypes);
                return true;

            case "text-length" when args.Count == 1:
                EmitExpr(body, args[0], localMap, ref nextLocal, localTypes, args[0].Type);
                // Load length from pointer (first 4 bytes)
                body.WriteByte(OpI32Load);
                body.WriteByte(0x02); // align 4
                WriteUnsignedLeb128(body, 0); // offset 0
                body.WriteByte(OpI64ExtendI32S);
                return true;

            default:
                return false;
        }
    }

    void EmitPrintLineForType(MemoryStream body, CodexType type,
        ref int nextLocal, List<byte> localTypes)
    {
        switch (type)
        {
            case IntegerType:
                body.WriteByte(OpCall);
                WriteUnsignedLeb128(body, m_printI64Index);
                break;

            case BooleanType:
                body.WriteByte(OpCall);
                WriteUnsignedLeb128(body, m_printBoolIndex);
                break;

            case TextType:
                int ptrLocal = nextLocal++;
                localTypes.Add(WasmI32);
                body.WriteByte(OpLocalSet);
                WriteUnsignedLeb128(body, ptrLocal);
                EmitFdWriteFromLengthPrefixed(body, ptrLocal);
                EmitWriteNewline(body, localTypes);
                break;

            default:
                body.WriteByte(OpDrop);
                break;
        }
    }

    void EmitShow(MemoryStream body, CodexType type,
        ref int nextLocal, List<byte> localTypes)
    {
        switch (type)
        {
            case IntegerType:
                // Convert i64 to string in memory, return pointer
                EmitI64ToString(body, ref nextLocal, localTypes);
                break;

            case BooleanType:
                // If true → "True", else → "False"
                int truePtr = AddDataSegment(EncodeLengthPrefixedString("True"));
                int falsePtr = AddDataSegment(EncodeLengthPrefixedString("False"));

                body.WriteByte(OpIf);
                body.WriteByte(BlockTypeI32);
                body.WriteByte(OpI32Const); WriteSignedLeb128(body, truePtr);
                body.WriteByte(OpElse);
                body.WriteByte(OpI32Const); WriteSignedLeb128(body, falsePtr);
                body.WriteByte(OpEnd);
                break;

            case TextType:
                // Already a string pointer, nothing to do
                break;

            default:
                break;
        }
    }

    // ── Text operations ──────────────────────────────────────────

    void EmitTextLiteral(MemoryStream body, string value)
    {
        string key = value;
        if (!m_stringOffsets.TryGet(key, out int offset))
        {
            byte[] encoded = EncodeLengthPrefixedString(value);
            offset = AddDataSegment(encoded);
            m_stringOffsets = m_stringOffsets.Set(key, offset);
            m_stringLengths = m_stringLengths.Set(key, Encoding.UTF8.GetByteCount(value));
        }

        body.WriteByte(OpI32Const);
        WriteSignedLeb128(body, offset);
    }

    void EmitTextAppend(MemoryStream body, IRExpr left, IRExpr right,
        ValueMap<string, int> localMap, ref int nextLocal, List<byte> localTypes)
    {
        // Evaluate both sides
        EmitExpr(body, left, localMap, ref nextLocal, localTypes, left.Type);
        int leftPtr = nextLocal++;
        localTypes.Add(WasmI32);
        body.WriteByte(OpLocalSet);
        WriteUnsignedLeb128(body, leftPtr);

        EmitExpr(body, right, localMap, ref nextLocal, localTypes, right.Type);
        int rightPtr = nextLocal++;
        localTypes.Add(WasmI32);
        body.WriteByte(OpLocalSet);
        WriteUnsignedLeb128(body, rightPtr);

        // Load lengths
        int leftLen = nextLocal++;
        localTypes.Add(WasmI32);
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, leftPtr);
        body.WriteByte(OpI32Load); body.WriteByte(0x02); WriteUnsignedLeb128(body, 0);
        body.WriteByte(OpLocalSet); WriteUnsignedLeb128(body, leftLen);

        int rightLen = nextLocal++;
        localTypes.Add(WasmI32);
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, rightPtr);
        body.WriteByte(OpI32Load); body.WriteByte(0x02); WriteUnsignedLeb128(body, 0);
        body.WriteByte(OpLocalSet); WriteUnsignedLeb128(body, rightLen);

        // Total length
        int totalLen = nextLocal++;
        localTypes.Add(WasmI32);
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, leftLen);
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, rightLen);
        body.WriteByte(OpI32Add);
        body.WriteByte(OpLocalSet); WriteUnsignedLeb128(body, totalLen);

        // Bump-allocate: 4 bytes for length prefix + totalLen bytes for data
        int resultPtr = nextLocal++;
        localTypes.Add(WasmI32);
        EmitBumpAlloc(body, totalLen, 4, resultPtr);

        // Store total length at resultPtr
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, resultPtr);
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, totalLen);
        body.WriteByte(OpI32Store); body.WriteByte(0x02); WriteUnsignedLeb128(body, 0);

        // Copy left bytes: memory.copy is bulk-memory proposal, use byte loop instead
        // For simplicity, use a loop
        EmitMemCopy(body, resultPtr, 4, leftPtr, 4, leftLen, ref nextLocal, localTypes);

        // Copy right bytes after left
        // dest offset = resultPtr + 4 + leftLen
        int rightDest = nextLocal++;
        localTypes.Add(WasmI32);
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, resultPtr);
        body.WriteByte(OpI32Const); WriteSignedLeb128(body, 4);
        body.WriteByte(OpI32Add);
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, leftLen);
        body.WriteByte(OpI32Add);
        body.WriteByte(OpLocalSet); WriteUnsignedLeb128(body, rightDest);

        EmitMemCopyDirect(body, rightDest, rightPtr, 4, rightLen, ref nextLocal, localTypes);

        // Result is the new pointer
        body.WriteByte(OpLocalGet);
        WriteUnsignedLeb128(body, resultPtr);
    }

    void EmitBumpAlloc(MemoryStream body, int sizeLocal, int extraBytes, int resultLocal)
    {
        // resultLocal = heap_ptr
        body.WriteByte(OpGlobalGet);
        WriteUnsignedLeb128(body, m_heapPtrGlobalIndex);
        body.WriteByte(OpLocalSet);
        WriteUnsignedLeb128(body, resultLocal);

        // heap_ptr += extraBytes + sizeLocal
        body.WriteByte(OpGlobalGet);
        WriteUnsignedLeb128(body, m_heapPtrGlobalIndex);
        body.WriteByte(OpI32Const);
        WriteSignedLeb128(body, extraBytes);
        body.WriteByte(OpI32Add);
        body.WriteByte(OpLocalGet);
        WriteUnsignedLeb128(body, sizeLocal);
        body.WriteByte(OpI32Add);
        body.WriteByte(OpGlobalSet);
        WriteUnsignedLeb128(body, m_heapPtrGlobalIndex);
    }

    void EmitMemCopy(MemoryStream body, int destBase, int destOffset,
        int srcBase, int srcOffset, int lenLocal,
        ref int nextLocal, List<byte> localTypes)
    {
        // Byte-by-byte copy loop
        int idx = nextLocal++;
        localTypes.Add(WasmI32);

        body.WriteByte(OpI32Const); WriteSignedLeb128(body, 0);
        body.WriteByte(OpLocalSet); WriteUnsignedLeb128(body, idx);

        // block { loop {
        body.WriteByte(OpBlock); body.WriteByte(BlockTypeVoid);
        body.WriteByte(OpLoop); body.WriteByte(BlockTypeVoid);

        // if idx >= len: br 1 (break out of block)
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, idx);
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, lenLocal);
        body.WriteByte(OpI32GeS);
        body.WriteByte(OpBrIf); WriteUnsignedLeb128(body, 1);

        // dest[destBase + destOffset + idx] = src[srcBase + srcOffset + idx]
        // Store address
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, destBase);
        body.WriteByte(OpI32Const); WriteSignedLeb128(body, destOffset);
        body.WriteByte(OpI32Add);
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, idx);
        body.WriteByte(OpI32Add);

        // Load value
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, srcBase);
        body.WriteByte(OpI32Const); WriteSignedLeb128(body, srcOffset);
        body.WriteByte(OpI32Add);
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, idx);
        body.WriteByte(OpI32Add);
        body.WriteByte(OpI32Load8U); // i32.load8_u
        body.WriteByte(0x00); WriteUnsignedLeb128(body, 0);

        body.WriteByte(OpI32Store8); // i32.store8
        body.WriteByte(0x00); WriteUnsignedLeb128(body, 0);

        // idx++
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, idx);
        body.WriteByte(OpI32Const); WriteSignedLeb128(body, 1);
        body.WriteByte(OpI32Add);
        body.WriteByte(OpLocalSet); WriteUnsignedLeb128(body, idx);

        // br 0 (continue loop)
        body.WriteByte(OpBr); WriteUnsignedLeb128(body, 0);

        body.WriteByte(OpEnd); // end loop
        body.WriteByte(OpEnd); // end block
    }

    void EmitMemCopyDirect(MemoryStream body, int destLocal, int srcBase, int srcOffset,
        int lenLocal, ref int nextLocal, List<byte> localTypes)
    {
        int idx = nextLocal++;
        localTypes.Add(WasmI32);

        body.WriteByte(OpI32Const); WriteSignedLeb128(body, 0);
        body.WriteByte(OpLocalSet); WriteUnsignedLeb128(body, idx);

        body.WriteByte(OpBlock); body.WriteByte(BlockTypeVoid);
        body.WriteByte(OpLoop); body.WriteByte(BlockTypeVoid);

        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, idx);
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, lenLocal);
        body.WriteByte(OpI32GeS);
        body.WriteByte(OpBrIf); WriteUnsignedLeb128(body, 1);

        // dest[destLocal + idx]
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, destLocal);
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, idx);
        body.WriteByte(OpI32Add);

        // src[srcBase + srcOffset + idx]
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, srcBase);
        body.WriteByte(OpI32Const); WriteSignedLeb128(body, srcOffset);
        body.WriteByte(OpI32Add);
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, idx);
        body.WriteByte(OpI32Add);
        body.WriteByte(OpI32Load8U); // i32.load8_u
        body.WriteByte(0x00); WriteUnsignedLeb128(body, 0);

        body.WriteByte(OpI32Store8); // i32.store8
        body.WriteByte(0x00); WriteUnsignedLeb128(body, 0);

        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, idx);
        body.WriteByte(OpI32Const); WriteSignedLeb128(body, 1);
        body.WriteByte(OpI32Add);
        body.WriteByte(OpLocalSet); WriteUnsignedLeb128(body, idx);

        body.WriteByte(OpBr); WriteUnsignedLeb128(body, 0);

        body.WriteByte(OpEnd);
        body.WriteByte(OpEnd);
    }

    // ── fd_write helper ──────────────────────────────────────────

    void EmitFdWriteFromLengthPrefixed(MemoryStream body, int ptrLocal)
    {
        // WASI fd_write needs: iov at a scratch address
        // iov = { buf_ptr: i32, buf_len: i32 }
        // We use memory address 0 as scratch for iov struct (before data section starts)

        // iov[0].buf = ptr + 4 (skip length prefix)
        body.WriteByte(OpI32Const); WriteSignedLeb128(body, 0); // scratch addr for iov
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, ptrLocal);
        body.WriteByte(OpI32Const); WriteSignedLeb128(body, 4);
        body.WriteByte(OpI32Add);
        body.WriteByte(OpI32Store); body.WriteByte(0x02); WriteUnsignedLeb128(body, 0);

        // iov[0].len = *ptr (the length prefix)
        body.WriteByte(OpI32Const); WriteSignedLeb128(body, 4); // scratch addr + 4
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, ptrLocal);
        body.WriteByte(OpI32Load); body.WriteByte(0x02); WriteUnsignedLeb128(body, 0);
        body.WriteByte(OpI32Store); body.WriteByte(0x02); WriteUnsignedLeb128(body, 0);

        // fd_write(1, iov=0, iovs_len=1, nwritten=8)
        body.WriteByte(OpI32Const); WriteSignedLeb128(body, 1); // stdout
        body.WriteByte(OpI32Const); WriteSignedLeb128(body, 0); // iov ptr
        body.WriteByte(OpI32Const); WriteSignedLeb128(body, 1); // 1 iov
        body.WriteByte(OpI32Const); WriteSignedLeb128(body, 8); // nwritten scratch
        body.WriteByte(OpCall); WriteUnsignedLeb128(body, m_fdWriteIndex);
        body.WriteByte(OpDrop); // drop return value
    }

    void EmitWriteNewline(MemoryStream body, List<byte> localTypes)
    {
        // Write "\n"
        int nlOffset = AddDataSegment(EncodeLengthPrefixedString("\n"));

        // iov[0].buf = nlOffset + 4
        body.WriteByte(OpI32Const); WriteSignedLeb128(body, 0);
        body.WriteByte(OpI32Const); WriteSignedLeb128(body, nlOffset + 4);
        body.WriteByte(OpI32Store); body.WriteByte(0x02); WriteUnsignedLeb128(body, 0);

        // iov[0].len = 1
        body.WriteByte(OpI32Const); WriteSignedLeb128(body, 4);
        body.WriteByte(OpI32Const); WriteSignedLeb128(body, 1);
        body.WriteByte(OpI32Store); body.WriteByte(0x02); WriteUnsignedLeb128(body, 0);

        body.WriteByte(OpI32Const); WriteSignedLeb128(body, 1);
        body.WriteByte(OpI32Const); WriteSignedLeb128(body, 0);
        body.WriteByte(OpI32Const); WriteSignedLeb128(body, 1);
        body.WriteByte(OpI32Const); WriteSignedLeb128(body, 8);
        body.WriteByte(OpCall); WriteUnsignedLeb128(body, m_fdWriteIndex);
        body.WriteByte(OpDrop);
    }

    // ── Runtime helpers ──────────────────────────────────────────

    void EmitRuntimeHelpers()
    {
        EmitPrintI64Helper();
        EmitPrintBoolHelper();
    }

    void EmitPrintI64Helper()
    {
        // __print_i64(value: i64) -> void
        // Converts i64 to decimal string in memory, then calls fd_write
        int typeIndex = AddFuncType(new byte[] { WasmI64 }, Array.Empty<byte>());
        int funcSlot = m_printI64Index - m_importCount;
        m_functionTypeIndices[funcSlot] = typeIndex;

        MemoryStream body = new();
        List<byte> localTypes = new();

        // Locals: value is param 0
        int bufStart = 0; localTypes.Add(WasmI32); // scratch buffer start
        int bufPos = 1; localTypes.Add(WasmI32);   // current write position
        int isNeg = 2; localTypes.Add(WasmI32);    // is negative flag
        int digit = 3; localTypes.Add(WasmI32);    // temp digit
        int absVal = 4; localTypes.Add(WasmI64);   // absolute value

        // Allocate 24 bytes on heap for digit buffer
        body.WriteByte(OpGlobalGet); WriteUnsignedLeb128(body, m_heapPtrGlobalIndex);
        body.WriteByte(OpLocalSet); WriteUnsignedLeb128(body, bufStart + 1);

        body.WriteByte(OpGlobalGet); WriteUnsignedLeb128(body, m_heapPtrGlobalIndex);
        body.WriteByte(OpI32Const); WriteSignedLeb128(body, 24);
        body.WriteByte(OpI32Add);
        body.WriteByte(OpGlobalSet); WriteUnsignedLeb128(body, m_heapPtrGlobalIndex);

        // bufPos = bufStart + 23 (write right-to-left)
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, bufStart + 1);
        body.WriteByte(OpI32Const); WriteSignedLeb128(body, 23);
        body.WriteByte(OpI32Add);
        body.WriteByte(OpLocalSet); WriteUnsignedLeb128(body, bufPos + 1);

        // Check if negative
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, 0); // param value
        body.WriteByte(OpI64Const); WriteSignedLeb128(body, 0);
        body.WriteByte(OpI64LtS);
        body.WriteByte(OpLocalSet); WriteUnsignedLeb128(body, isNeg + 1);

        // absVal = if neg then -value else value
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, isNeg + 1);
        body.WriteByte(OpIf); body.WriteByte(BlockTypeVoid);
        body.WriteByte(OpI64Const); WriteSignedLeb128(body, 0);
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, 0);
        body.WriteByte(OpI64Sub);
        body.WriteByte(OpLocalSet); WriteUnsignedLeb128(body, absVal + 1);
        body.WriteByte(OpElse);
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, 0);
        body.WriteByte(OpLocalSet); WriteUnsignedLeb128(body, absVal + 1);
        body.WriteByte(OpEnd);

        // Handle zero case
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, absVal + 1);
        body.WriteByte(OpI64Eqz);
        body.WriteByte(OpIf); body.WriteByte(BlockTypeVoid);
        // Store '0' at bufPos
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, bufPos + 1);
        body.WriteByte(OpI32Const); WriteSignedLeb128(body, 48); // '0'
        body.WriteByte(OpI32Store8); body.WriteByte(0x00); WriteUnsignedLeb128(body, 0); // i32.store8
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, bufPos + 1);
        body.WriteByte(OpI32Const); WriteSignedLeb128(body, 1);
        body.WriteByte(OpI32Sub);
        body.WriteByte(OpLocalSet); WriteUnsignedLeb128(body, bufPos + 1);
        body.WriteByte(OpEnd);
        body.WriteByte(OpElse);

        // Loop: while absVal > 0
        body.WriteByte(OpBlock); body.WriteByte(BlockTypeVoid);
        body.WriteByte(OpLoop); body.WriteByte(BlockTypeVoid);
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, absVal + 1);
        body.WriteByte(OpI64Eqz);
        body.WriteByte(OpBrIf); WriteUnsignedLeb128(body, 1);

        // digit = (i32)(absVal % 10)
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, absVal + 1);
        body.WriteByte(OpI64Const); WriteSignedLeb128(body, 10);
        body.WriteByte((byte)0x81); // i64.rem_s
        body.WriteByte(OpI32WrapI64);
        body.WriteByte(OpLocalSet); WriteUnsignedLeb128(body, digit + 1);

        // store '0' + digit at bufPos
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, bufPos + 1);
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, digit + 1);
        body.WriteByte(OpI32Const); WriteSignedLeb128(body, 48);
        body.WriteByte(OpI32Add);
        body.WriteByte(OpI32Store8); body.WriteByte(0x00); WriteUnsignedLeb128(body, 0); // i32.store8

        // bufPos--
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, bufPos + 1);
        body.WriteByte(OpI32Const); WriteSignedLeb128(body, 1);
        body.WriteByte(OpI32Sub);
        body.WriteByte(OpLocalSet); WriteUnsignedLeb128(body, bufPos + 1);

        // absVal /= 10
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, absVal + 1);
        body.WriteByte(OpI64Const); WriteSignedLeb128(body, 10);
        body.WriteByte(OpI64DivS);
        body.WriteByte(OpLocalSet); WriteUnsignedLeb128(body, absVal + 1);

        body.WriteByte(OpBr); WriteUnsignedLeb128(body, 0);
        body.WriteByte(OpEnd); // end loop
        body.WriteByte(OpEnd); // end block

        body.WriteByte(OpEnd); // end if/else (zero check)

        // If negative, store '-'
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, isNeg + 1);
        body.WriteByte(OpIf); body.WriteByte(BlockTypeVoid);
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, bufPos + 1);
        body.WriteByte(OpI32Const); WriteSignedLeb128(body, 45); // '-'
        body.WriteByte(OpI32Store8); body.WriteByte(0x00); WriteUnsignedLeb128(body, 0);
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, bufPos + 1);
        body.WriteByte(OpI32Const); WriteSignedLeb128(body, 1);
        body.WriteByte(OpI32Sub);
        body.WriteByte(OpLocalSet); WriteUnsignedLeb128(body, bufPos + 1);
        body.WriteByte(OpEnd);

        // Now write from bufPos+1 to bufStart+23 via fd_write
        // iov.buf = bufPos + 1
        // iov.len = bufStart + 23 - bufPos
        body.WriteByte(OpI32Const); WriteSignedLeb128(body, 0); // iov scratch
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, bufPos + 1);
        body.WriteByte(OpI32Const); WriteSignedLeb128(body, 1);
        body.WriteByte(OpI32Add);
        body.WriteByte(OpI32Store); body.WriteByte(0x02); WriteUnsignedLeb128(body, 0);

        body.WriteByte(OpI32Const); WriteSignedLeb128(body, 4);
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, bufStart + 1);
        body.WriteByte(OpI32Const); WriteSignedLeb128(body, 24);
        body.WriteByte(OpI32Add);
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, bufPos + 1);
        body.WriteByte(OpI32Const); WriteSignedLeb128(body, 1);
        body.WriteByte(OpI32Add);
        body.WriteByte(OpI32Sub);
        body.WriteByte(OpI32Store); body.WriteByte(0x02); WriteUnsignedLeb128(body, 0);

        body.WriteByte(OpI32Const); WriteSignedLeb128(body, 1); // stdout
        body.WriteByte(OpI32Const); WriteSignedLeb128(body, 0); // iov
        body.WriteByte(OpI32Const); WriteSignedLeb128(body, 1); // 1 iov
        body.WriteByte(OpI32Const); WriteSignedLeb128(body, 8); // nwritten
        body.WriteByte(OpCall); WriteUnsignedLeb128(body, m_fdWriteIndex);
        body.WriteByte(OpDrop);

        // Write newline
        EmitWriteNewline(body, localTypes);

        body.WriteByte(OpEnd);
        m_functionBodies.Add(EncodeFunctionBody(body.ToArray(), localTypes));
    }

    void EmitPrintBoolHelper()
    {
        // __print_bool(value: i32) -> void
        int typeIndex = AddFuncType(new byte[] { WasmI32 }, Array.Empty<byte>());
        int funcSlot = m_printBoolIndex - m_importCount;
        m_functionTypeIndices[funcSlot] = typeIndex;

        int trueOffset = AddDataSegment(EncodeLengthPrefixedString("True"));
        int falseOffset = AddDataSegment(EncodeLengthPrefixedString("False"));

        MemoryStream body = new();
        List<byte> localTypes = new();

        int ptrLocal = 0; localTypes.Add(WasmI32);

        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, 0); // param
        body.WriteByte(OpIf); body.WriteByte(BlockTypeVoid);
        body.WriteByte(OpI32Const); WriteSignedLeb128(body, trueOffset);
        body.WriteByte(OpLocalSet); WriteUnsignedLeb128(body, ptrLocal + 1);
        body.WriteByte(OpElse);
        body.WriteByte(OpI32Const); WriteSignedLeb128(body, falseOffset);
        body.WriteByte(OpLocalSet); WriteUnsignedLeb128(body, ptrLocal + 1);
        body.WriteByte(OpEnd);

        EmitFdWriteFromLengthPrefixed(body, ptrLocal + 1);
        EmitWriteNewline(body, localTypes);

        body.WriteByte(OpEnd);
        m_functionBodies.Add(EncodeFunctionBody(body.ToArray(), localTypes));
    }

    void EmitI64ToString(MemoryStream body, ref int nextLocal, List<byte> localTypes)
    {
        // Simple approach: call __print_i64-like logic but write to heap and return pointer
        // For Phase 1, just use show on integers — we can optimize later
        // Write digits to heap, return length-prefixed pointer

        int valLocal = nextLocal++;
        localTypes.Add(WasmI64);
        body.WriteByte(OpLocalSet); WriteUnsignedLeb128(body, valLocal);

        int bufStart = nextLocal++;
        localTypes.Add(WasmI32);
        int bufPos = nextLocal++;
        localTypes.Add(WasmI32);
        int absVal = nextLocal++;
        localTypes.Add(WasmI64);
        int isNeg = nextLocal++;
        localTypes.Add(WasmI32);
        int digitLocal = nextLocal++;
        localTypes.Add(WasmI32);
        int resultPtr = nextLocal++;
        localTypes.Add(WasmI32);
        int numLen = nextLocal++;
        localTypes.Add(WasmI32);

        // Allocate 24-byte scratch on heap
        body.WriteByte(OpGlobalGet); WriteUnsignedLeb128(body, m_heapPtrGlobalIndex);
        body.WriteByte(OpLocalSet); WriteUnsignedLeb128(body, bufStart);
        body.WriteByte(OpGlobalGet); WriteUnsignedLeb128(body, m_heapPtrGlobalIndex);
        body.WriteByte(OpI32Const); WriteSignedLeb128(body, 24);
        body.WriteByte(OpI32Add);
        body.WriteByte(OpGlobalSet); WriteUnsignedLeb128(body, m_heapPtrGlobalIndex);

        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, bufStart);
        body.WriteByte(OpI32Const); WriteSignedLeb128(body, 23);
        body.WriteByte(OpI32Add);
        body.WriteByte(OpLocalSet); WriteUnsignedLeb128(body, bufPos);

        // isNeg
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, valLocal);
        body.WriteByte(OpI64Const); WriteSignedLeb128(body, 0);
        body.WriteByte(OpI64LtS);
        body.WriteByte(OpLocalSet); WriteUnsignedLeb128(body, isNeg);

        // absVal
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, isNeg);
        body.WriteByte(OpIf); body.WriteByte(BlockTypeVoid);
        body.WriteByte(OpI64Const); WriteSignedLeb128(body, 0);
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, valLocal);
        body.WriteByte(OpI64Sub);
        body.WriteByte(OpLocalSet); WriteUnsignedLeb128(body, absVal);
        body.WriteByte(OpElse);
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, valLocal);
        body.WriteByte(OpLocalSet); WriteUnsignedLeb128(body, absVal);
        body.WriteByte(OpEnd);

        // Zero case
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, absVal);
        body.WriteByte(OpI64Eqz);
        body.WriteByte(OpIf); body.WriteByte(BlockTypeVoid);
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, bufPos);
        body.WriteByte(OpI32Const); WriteSignedLeb128(body, 48); // '0'
        body.WriteByte(OpI32Store8); body.WriteByte(0x00); WriteUnsignedLeb128(body, 0); // i32.store8
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, bufPos);
        body.WriteByte(OpI32Const); WriteSignedLeb128(body, 1);
        body.WriteByte(OpI32Sub);
        body.WriteByte(OpLocalSet); WriteUnsignedLeb128(body, bufPos);
        body.WriteByte(OpEnd);
        body.WriteByte(OpElse);

        // Digit loop
        body.WriteByte(OpBlock); body.WriteByte(BlockTypeVoid);
        body.WriteByte(OpLoop); body.WriteByte(BlockTypeVoid);
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, absVal);
        body.WriteByte(OpI64Eqz);
        body.WriteByte(OpBrIf); WriteUnsignedLeb128(body, 1);

        // digit = (i32)(absVal % 10)
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, absVal);
        body.WriteByte(OpI64Const); WriteSignedLeb128(body, 10);
        body.WriteByte((byte)0x81); // i64.rem_s
        body.WriteByte(OpI32WrapI64);
        body.WriteByte(OpLocalSet); WriteUnsignedLeb128(body, digitLocal);

        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, bufPos);
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, digitLocal);
        body.WriteByte(OpI32Const); WriteSignedLeb128(body, 48);
        body.WriteByte(OpI32Add);
        body.WriteByte(OpI32Store8); body.WriteByte(0x00); WriteUnsignedLeb128(body, 0); // i32.store8

        // bufPos--
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, bufPos);
        body.WriteByte(OpI32Const); WriteSignedLeb128(body, 1);
        body.WriteByte(OpI32Sub);
        body.WriteByte(OpLocalSet); WriteUnsignedLeb128(body, bufPos);

        // absVal /= 10
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, absVal);
        body.WriteByte(OpI64Const); WriteSignedLeb128(body, 10);
        body.WriteByte(OpI64DivS);
        body.WriteByte(OpLocalSet); WriteUnsignedLeb128(body, absVal);

        body.WriteByte(OpBr); WriteUnsignedLeb128(body, 0);
        body.WriteByte(OpEnd); // end loop
        body.WriteByte(OpEnd); // end block

        body.WriteByte(OpEnd); // end if/else (zero check)

        // If negative, store '-'
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, isNeg);
        body.WriteByte(OpIf); body.WriteByte(BlockTypeVoid);
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, bufPos);
        body.WriteByte(OpI32Const); WriteSignedLeb128(body, 45); // '-'
        body.WriteByte(OpI32Store8); body.WriteByte(0x00); WriteUnsignedLeb128(body, 0);
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, bufPos);
        body.WriteByte(OpI32Const); WriteSignedLeb128(body, 1);
        body.WriteByte(OpI32Sub);
        body.WriteByte(OpLocalSet); WriteUnsignedLeb128(body, bufPos);
        body.WriteByte(OpEnd);

        // numLen = bufStart + 23 - bufPos
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, bufStart);
        body.WriteByte(OpI32Const); WriteSignedLeb128(body, 23);
        body.WriteByte(OpI32Add);
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, bufPos);
        body.WriteByte(OpI32Sub);
        body.WriteByte(OpLocalSet); WriteUnsignedLeb128(body, numLen);

        // Allocate length-prefixed result: 4 + numLen
        body.WriteByte(OpGlobalGet); WriteUnsignedLeb128(body, m_heapPtrGlobalIndex);
        body.WriteByte(OpLocalSet); WriteUnsignedLeb128(body, resultPtr);
        body.WriteByte(OpGlobalGet); WriteUnsignedLeb128(body, m_heapPtrGlobalIndex);
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, numLen);
        body.WriteByte(OpI32Const); WriteSignedLeb128(body, 4);
        body.WriteByte(OpI32Add);
        body.WriteByte(OpI32Add);
        body.WriteByte(OpGlobalSet); WriteUnsignedLeb128(body, m_heapPtrGlobalIndex);

        // Store length
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, resultPtr);
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, numLen);
        body.WriteByte(OpI32Store); body.WriteByte(0x02); WriteUnsignedLeb128(body, 0);

        // Copy digits from buf to result+4 (skip length prefix)
        int destLocal = nextLocal++;
        localTypes.Add(WasmI32);
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, resultPtr);
        body.WriteByte(OpI32Const); WriteSignedLeb128(body, 4);
        body.WriteByte(OpI32Add);
        body.WriteByte(OpLocalSet); WriteUnsignedLeb128(body, destLocal);

        EmitMemCopyDirect(body, destLocal, bufPos, 1, numLen, ref nextLocal, localTypes);

        // Return resultPtr
        body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, resultPtr);
    }

    // ── Encoding helpers ─────────────────────────────────────────

    byte[] EncodeLengthPrefixedString(string value)
    {
        byte[] utf8 = Encoding.UTF8.GetBytes(value);
        byte[] result = new byte[4 + utf8.Length];
        BitConverter.TryWriteBytes(result.AsSpan(0, 4), utf8.Length);
        utf8.CopyTo(result, 4);
        return result;
    }

    int AddDataSegment(byte[] data)
    {
        int offset = m_dataOffset;
        m_dataSegments.Add(data);
        m_dataOffset += data.Length;
        // Update the global heap pointer initial value
        if (m_globals.Count > 0)
        {
            m_globals[0] = new WasmGlobal(WasmI32, GlobalMut, m_dataOffset);
        }
        return offset;
    }

    int AddFuncType(byte[] paramTypes, byte[] resultTypes)
    {
        // Check for existing identical type
        for (int i = 0; i < m_types.Count; i++)
        {
            if (m_types[i].Params.SequenceEqual(paramTypes) &&
                m_types[i].Results.SequenceEqual(resultTypes))
                return i;
        }
        m_types.Add(new WasmFuncType(paramTypes, resultTypes));
        return m_types.Count - 1;
    }

    byte WasmTypeFor(CodexType type)
    {
        return type switch
        {
            IntegerType => WasmI64,
            NumberType => WasmF64,
            BooleanType => WasmI32,
            TextType => WasmI32,  // pointer to length-prefixed string
            VoidType or NothingType => WasmI32, // shouldn't appear, but safe default
            _ => WasmI32 // heap pointer for records, sum types, lists
        };
    }

    byte WasmBlockTypeFor(CodexType type)
    {
        return type switch
        {
            IntegerType => BlockTypeI64,
            NumberType => BlockTypeF64,
            BooleanType => BlockTypeI32,
            TextType => BlockTypeI32,
            VoidType or NothingType => BlockTypeVoid,
            _ => BlockTypeI32
        };
    }

    CodexType ComputeReturnType(CodexType type, int paramCount)
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

    byte[] EncodeFunctionBody(byte[] code, List<byte> localTypes)
    {
        // Group consecutive identical local types
        List<(int Count, byte Type)> localGroups = new();
        for (int i = 0; i < localTypes.Count; i++)
        {
            if (localGroups.Count > 0 && localGroups[^1].Type == localTypes[i])
            {
                (int c, byte t) = localGroups[^1];
                localGroups[^1] = (c + 1, t);
            }
            else
            {
                localGroups.Add((1, localTypes[i]));
            }
        }

        MemoryStream bodyStream = new();
        WriteUnsignedLeb128(bodyStream, localGroups.Count);
        foreach ((int count, byte valType) in localGroups)
        {
            WriteUnsignedLeb128(bodyStream, count);
            bodyStream.WriteByte(valType);
        }
        bodyStream.Write(code, 0, code.Length);

        byte[] bodyBytes = bodyStream.ToArray();
        MemoryStream result = new();
        WriteUnsignedLeb128(result, bodyBytes.Length);
        result.Write(bodyBytes, 0, bodyBytes.Length);
        return result.ToArray();
    }

    // ── Section writers ──────────────────────────────────────────

    void WriteTypeSection(BinaryWriter w)
    {
        MemoryStream section = new();
        WriteUnsignedLeb128(section, m_types.Count);
        foreach (WasmFuncType ft in m_types)
        {
            section.WriteByte(0x60); // func type
            WriteUnsignedLeb128(section, ft.Params.Length);
            section.Write(ft.Params, 0, ft.Params.Length);
            WriteUnsignedLeb128(section, ft.Results.Length);
            section.Write(ft.Results, 0, ft.Results.Length);
        }
        WriteSection(w, SectionType, section.ToArray());
    }

    void WriteImportSection(BinaryWriter w)
    {
        if (m_imports.Count == 0) return;
        MemoryStream section = new();
        WriteUnsignedLeb128(section, m_imports.Count);
        foreach (WasmImport imp in m_imports)
        {
            WriteString(section, imp.Module);
            WriteString(section, imp.Name);
            section.WriteByte(imp.Kind);
            WriteUnsignedLeb128(section, imp.TypeIndex);
        }
        WriteSection(w, SectionImport, section.ToArray());
    }

    void WriteFunctionSection(BinaryWriter w)
    {
        MemoryStream section = new();
        WriteUnsignedLeb128(section, m_functionTypeIndices.Count);
        foreach (int typeIdx in m_functionTypeIndices)
        {
            WriteUnsignedLeb128(section, typeIdx);
        }
        WriteSection(w, SectionFunction, section.ToArray());
    }

    void WriteMemorySection(BinaryWriter w)
    {
        MemoryStream section = new();
        WriteUnsignedLeb128(section, 1); // 1 memory
        section.WriteByte(0x00); // no max
        WriteUnsignedLeb128(section, 1); // initial 1 page (64KB)
        WriteSection(w, SectionMemory, section.ToArray());
    }

    void WriteGlobalSection(BinaryWriter w)
    {
        if (m_globals.Count == 0) return;
        MemoryStream section = new();
        WriteUnsignedLeb128(section, m_globals.Count);
        foreach (WasmGlobal g in m_globals)
        {
            section.WriteByte(g.ValType);
            section.WriteByte(g.Mutability);
            // Init expression: i32.const <value> end
            section.WriteByte(OpI32Const);
            WriteSignedLeb128(section, g.InitValue);
            section.WriteByte(OpEnd);
        }
        WriteSection(w, SectionGlobal, section.ToArray());
    }

    void WriteExportSection(BinaryWriter w)
    {
        MemoryStream section = new();
        WriteUnsignedLeb128(section, m_exports.Count);
        foreach (WasmExport exp in m_exports)
        {
            WriteString(section, exp.Name);
            section.WriteByte(exp.Kind);
            WriteUnsignedLeb128(section, exp.Index);
        }
        WriteSection(w, SectionExport, section.ToArray());
    }

    void WriteCodeSection(BinaryWriter w)
    {
        MemoryStream section = new();
        WriteUnsignedLeb128(section, m_functionBodies.Count);
        foreach (byte[] body in m_functionBodies)
        {
            section.Write(body, 0, body.Length);
        }
        WriteSection(w, SectionCode, section.ToArray());
    }

    void WriteDataSection(BinaryWriter w)
    {
        if (m_dataSegments.Count == 0) return;
        MemoryStream section = new();
        WriteUnsignedLeb128(section, m_dataSegments.Count);
        int currentOffset = 1024;
        foreach (byte[] data in m_dataSegments)
        {
            section.WriteByte(0x00); // active segment, memory 0
            // Offset expression: i32.const <offset> end
            section.WriteByte(OpI32Const);
            WriteSignedLeb128(section, currentOffset);
            section.WriteByte(OpEnd);
            WriteUnsignedLeb128(section, data.Length);
            section.Write(data, 0, data.Length);
            currentOffset += data.Length;
        }
        WriteSection(w, SectionData, section.ToArray());
    }

    void WriteSection(BinaryWriter w, byte sectionId, byte[] content)
    {
        w.Write(sectionId);
        MemoryStream lenStream = new();
        WriteUnsignedLeb128(lenStream, content.Length);
        w.Write(lenStream.ToArray());
        w.Write(content);
    }

    // ── LEB128 encoding ──────────────────────────────────────────

    static void WriteUnsignedLeb128(MemoryStream stream, int value)
    {
        uint v = (uint)value;
        do
        {
            byte b = (byte)(v & 0x7F);
            v >>= 7;
            if (v != 0) b |= 0x80;
            stream.WriteByte(b);
        } while (v != 0);
    }

    static void WriteSignedLeb128(MemoryStream stream, long value)
    {
        bool more = true;
        while (more)
        {
            byte b = (byte)(value & 0x7F);
            value >>= 7;
            if ((value == 0 && (b & 0x40) == 0) || (value == -1 && (b & 0x40) != 0))
                more = false;
            else
                b |= 0x80;
            stream.WriteByte(b);
        }
    }

    static void WriteString(MemoryStream stream, string value)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(value);
        WriteUnsignedLeb128(stream, bytes.Length);
        stream.Write(bytes, 0, bytes.Length);
    }
}

// ── Internal records ─────────────────────────────────────────

sealed record WasmFuncType(byte[] Params, byte[] Results);

sealed record WasmImport(string Module, string Name, byte Kind, int TypeIndex);

sealed record WasmExport(string Name, byte Kind, int Index);

sealed record WasmGlobal(byte ValType, byte Mutability, int InitValue);
