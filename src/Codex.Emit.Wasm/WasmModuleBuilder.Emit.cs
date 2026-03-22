using System.Collections.Immutable;
using Codex.Core;
using Codex.IR;
using Codex.Types;

namespace Codex.Emit.Wasm;

sealed partial class WasmModuleBuilder
{
    void EmitImports()
    {
        int fdWriteType = AddFuncType(
            [WasmI32, WasmI32, WasmI32, WasmI32],
            [WasmI32]);
        m_fdWriteIndex = m_nextFuncIndex++;
        m_imports.Add(new WasmImport("wasi_snapshot_preview1", "fd_write", ImportFunc, fdWriteType));
        m_importCount = m_nextFuncIndex;
    }

    void EmitRuntimeGlobals()
    {
        m_heapPtrGlobalIndex = m_globals.Count;
        m_globals.Add(new WasmGlobal(WasmI32, GlobalMut, m_dataOffset));

        m_regionSpGlobalIndex = m_globals.Count;
        m_globals.Add(new WasmGlobal(WasmI32, GlobalMut, 0));
    }

    void PreRegisterFunctions(IRModule module)
    {
        m_printI64Index = m_nextFuncIndex++;
        m_functionTypeIndices.Add(-1);

        m_printBoolIndex = m_nextFuncIndex++;
        m_functionTypeIndices.Add(-1);

        m_strEqIndex = m_nextFuncIndex++;
        m_functionTypeIndices.Add(-1);

        foreach (IRDefinition def in module.Definitions)
        {
            m_functionIndex = m_functionIndex.Set(def.Name, m_nextFuncIndex);
            m_nextFuncIndex++;
            m_functionTypeIndices.Add(-1);
        }

        m_functionIndex = m_functionIndex.Set("__wasm_start", m_nextFuncIndex);
        m_nextFuncIndex++;
        m_functionTypeIndices.Add(-1);
    }

    void EmitDefinition(IRDefinition def)
    {
        int paramCount = def.Parameters.Length;
        CodexType returnType = ComputeReturnType(def.Type, paramCount);

        byte[] paramTypes = new byte[paramCount];
        for (int i = 0; i < paramCount; i++)
            paramTypes[i] = WasmTypeFor(def.Parameters[i].Type);

        byte[] resultTypes = returnType is VoidType or NothingType
            ? [] : [WasmTypeFor(returnType)];

        int typeIndex = AddFuncType(paramTypes, resultTypes);
        int funcSlot = m_functionIndex.Get(def.Name, 0) - m_importCount;
        m_functionTypeIndices[funcSlot] = typeIndex;

        List<byte> localTypes = [];
        ValueMap<string, int> localMap = ValueMap<string, int>.s_empty;
        for (int i = 0; i < paramCount; i++)
            localMap = localMap.Set(def.Parameters[i].Name, i);

        MemoryStream body = new();
        int nextLocal = paramCount;
        EmitExpr(body, def.Body, localMap, ref nextLocal, localTypes, returnType);
        body.WriteByte(OpEnd);

        m_functionBodies.Add(EncodeFunctionBody(body.ToArray(), localTypes));
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
            int voidType = AddFuncType([], []);
            m_functionTypeIndices[startSlot] = voidType;
            MemoryStream body = new();
            body.WriteByte(OpEnd);
            m_functionBodies.Add(EncodeFunctionBody(body.ToArray(), []));
        }
        else
        {
            int voidType = AddFuncType([], []);
            m_functionTypeIndices[startSlot] = voidType;

            MemoryStream body = new();
            List<byte> localTypes = [];
            CodexType returnType = ComputeReturnType(mainDef.Type, mainDef.Parameters.Length);

            int mainIdx = m_functionIndex.Get("main", 0);
            body.WriteByte(OpCall);
            WriteUnsignedLeb128(body, mainIdx);

            EmitPrintResult(body, returnType, localTypes);

            body.WriteByte(OpEnd);
            m_functionBodies.Add(EncodeFunctionBody(body.ToArray(), localTypes));
        }

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

            case IRRegion region:
                EmitRegion(body, region, localMap, ref nextLocal, localTypes);
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
                body.WriteByte(OpCall);
                WriteUnsignedLeb128(body, m_strEqIndex);
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
        List<IRExpr> args = [];
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

    void EmitRegion(MemoryStream body, IRRegion region,
        ValueMap<string, int> localMap, ref int nextLocal, List<byte> localTypes)
    {
        // Records, sum types, lists: skip region (deep copy needed — Phase 3)
        if (region.Type is RecordType or SumType or ListType)
        {
            EmitExpr(body, region.Body, localMap, ref nextLocal, localTypes, region.Type);
            return;
        }

        // Enter region
        EmitRegionEnter(body);

        // Emit body
        EmitExpr(body, region.Body, localMap, ref nextLocal, localTypes, region.Type);

        if (region.Type is TextType)
        {
            // Text escape promotion: copy the returned string to the parent region
            // The old data is still physically in memory after region exit
            int oldPtr = nextLocal++; localTypes.Add(WasmI32);
            int len = nextLocal++; localTypes.Add(WasmI32);
            int totalSize = nextLocal++; localTypes.Add(WasmI32);
            int newPtr = nextLocal++; localTypes.Add(WasmI32);

            // Save result pointer
            body.WriteByte(OpLocalSet); WriteUnsignedLeb128(body, oldPtr);

            // Load string length (4-byte prefix)
            body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, oldPtr);
            body.WriteByte(OpI32Load); body.WriteByte(0x02); WriteUnsignedLeb128(body, 0);
            body.WriteByte(OpLocalSet); WriteUnsignedLeb128(body, len);

            // totalSize = 4 + len
            body.WriteByte(OpI32Const); WriteSignedLeb128(body, 4);
            body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, len);
            body.WriteByte(OpI32Add);
            body.WriteByte(OpLocalSet); WriteUnsignedLeb128(body, totalSize);

            // Exit region (heap_ptr restored — old data still physically present)
            EmitRegionExit(body);

            // Bump-allocate in parent region: newPtr = heap_ptr; heap_ptr += totalSize
            body.WriteByte(OpGlobalGet); WriteUnsignedLeb128(body, m_heapPtrGlobalIndex);
            body.WriteByte(OpLocalSet); WriteUnsignedLeb128(body, newPtr);
            body.WriteByte(OpGlobalGet); WriteUnsignedLeb128(body, m_heapPtrGlobalIndex);
            body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, totalSize);
            body.WriteByte(OpI32Add);
            body.WriteByte(OpGlobalSet); WriteUnsignedLeb128(body, m_heapPtrGlobalIndex);

            // Copy bytes from oldPtr to newPtr (length prefix + data)
            EmitMemCopyDirect(body, newPtr, oldPtr, 0, totalSize, ref nextLocal, localTypes);

            // Push new pointer as the result
            body.WriteByte(OpLocalGet); WriteUnsignedLeb128(body, newPtr);
        }
        else
        {
            // Scalar return — value is on the WASM stack, survives region exit
            EmitRegionExit(body);
        }
    }

    void EmitRegionEnter(MemoryStream body)
    {
        // region_stack[region_sp] = heap_ptr; region_sp++
        body.WriteByte(OpGlobalGet); WriteUnsignedLeb128(body, m_regionSpGlobalIndex);
        body.WriteByte(OpI32Const); WriteSignedLeb128(body, 4);
        body.WriteByte(OpI32Mul);
        body.WriteByte(OpI32Const); WriteSignedLeb128(body, RegionStackBase);
        body.WriteByte(OpI32Add);
        body.WriteByte(OpGlobalGet); WriteUnsignedLeb128(body, m_heapPtrGlobalIndex);
        body.WriteByte(OpI32Store); body.WriteByte(0x02); WriteUnsignedLeb128(body, 0);

        body.WriteByte(OpGlobalGet); WriteUnsignedLeb128(body, m_regionSpGlobalIndex);
        body.WriteByte(OpI32Const); WriteSignedLeb128(body, 1);
        body.WriteByte(OpI32Add);
        body.WriteByte(OpGlobalSet); WriteUnsignedLeb128(body, m_regionSpGlobalIndex);
    }

    void EmitRegionExit(MemoryStream body)
    {
        // region_sp--; heap_ptr = region_stack[region_sp]
        body.WriteByte(OpGlobalGet); WriteUnsignedLeb128(body, m_regionSpGlobalIndex);
        body.WriteByte(OpI32Const); WriteSignedLeb128(body, 1);
        body.WriteByte(OpI32Sub);
        body.WriteByte(OpGlobalSet); WriteUnsignedLeb128(body, m_regionSpGlobalIndex);

        body.WriteByte(OpGlobalGet); WriteUnsignedLeb128(body, m_regionSpGlobalIndex);
        body.WriteByte(OpI32Const); WriteSignedLeb128(body, 4);
        body.WriteByte(OpI32Mul);
        body.WriteByte(OpI32Const); WriteSignedLeb128(body, RegionStackBase);
        body.WriteByte(OpI32Add);
        body.WriteByte(OpI32Load); body.WriteByte(0x02); WriteUnsignedLeb128(body, 0);
        body.WriteByte(OpGlobalSet); WriteUnsignedLeb128(body, m_heapPtrGlobalIndex);
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
}
