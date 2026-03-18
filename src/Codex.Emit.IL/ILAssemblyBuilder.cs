using System.Collections.Immutable;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using Codex.Core;
using Codex.IR;
using Codex.Types;

namespace Codex.Emit.IL;

sealed partial class ILAssemblyBuilder
{
    readonly string m_assemblyName;
    readonly MetadataBuilder m_metadata = new();
    readonly BlobBuilder m_ilStream = new();
    readonly MethodBodyStreamEncoder m_methodBodies;

    AssemblyReferenceHandle m_corlibRef;
    TypeReferenceHandle m_consoleRef;
    TypeReferenceHandle m_stringRef;
    TypeReferenceHandle m_objectRef;
    MemberReferenceHandle m_writeLineStringRef;
    MemberReferenceHandle m_writeLineInt64Ref;
    MemberReferenceHandle m_writeLineBoolRef;
    MemberReferenceHandle m_stringConcatRef;
    MemberReferenceHandle m_int64ToStringRef;
    MemberReferenceHandle m_boolToStringRef;
    MemberReferenceHandle m_objectCtorRef;

    TypeDefinitionHandle m_moduleClassDef;
    ValueMap<string, MethodDefinitionHandle> m_definedMethods = ValueMap<string, MethodDefinitionHandle>.s_empty;
    ValueMap<string, TypeDefinitionHandle> m_emittedTypes = ValueMap<string, TypeDefinitionHandle>.s_empty;
    ValueMap<string, MethodDefinitionHandle> m_ctorDefs = ValueMap<string, MethodDefinitionHandle>.s_empty;
    Map<string, List<(string Name, CodexType Type)>> m_typeFields = Map<string, List<(string Name, CodexType Type)>>.s_empty;
    ValueMap<string, FieldDefinitionHandle> m_fieldDefs = ValueMap<string, FieldDefinitionHandle>.s_empty;
    Map<string, string> m_ctorToBaseType = Map<string, string>.s_empty;

    ValueMap<string, int> m_definitionGenericArity = ValueMap<string, int>.s_empty;
    ValueMap<string, ImmutableArray<int>> m_definitionTypeVarIds = ValueMap<string, ImmutableArray<int>>.s_empty;
    ImmutableArray<int> m_currentTypeVarIds = ImmutableArray<int>.Empty;

    public ILAssemblyBuilder(string assemblyName)
    {
        m_assemblyName = assemblyName;
        m_methodBodies = new MethodBodyStreamEncoder(m_ilStream);
        BuildCorlibReferences();
    }

    void BuildCorlibReferences()
    {
        m_corlibRef = m_metadata.AddAssemblyReference(
            m_metadata.GetOrAddString("System.Runtime"),
            new Version(8, 0, 0, 0),
            default,
            m_metadata.GetOrAddBlob(
                ImmutableArray.Create<byte>(
                    0xB0, 0x3F, 0x5F, 0x7F, 0x11, 0xD5, 0x0A, 0x3A)),
            default,
            default);

        AssemblyReferenceHandle consoleAsmRef = m_metadata.AddAssemblyReference(
            m_metadata.GetOrAddString("System.Console"),
            new Version(8, 0, 0, 0),
            default,
            m_metadata.GetOrAddBlob(
                ImmutableArray.Create<byte>(
                    0xB0, 0x3F, 0x5F, 0x7F, 0x11, 0xD5, 0x0A, 0x3A)),
            default,
            default);

        m_consoleRef = m_metadata.AddTypeReference(
            consoleAsmRef,
            m_metadata.GetOrAddString("System"),
            m_metadata.GetOrAddString("Console"));

        m_stringRef = m_metadata.AddTypeReference(
            m_corlibRef,
            m_metadata.GetOrAddString("System"),
            m_metadata.GetOrAddString("String"));

        m_objectRef = m_metadata.AddTypeReference(
            m_corlibRef,
            m_metadata.GetOrAddString("System"),
            m_metadata.GetOrAddString("Object"));

        m_writeLineStringRef = m_metadata.AddMemberReference(
            m_consoleRef,
            m_metadata.GetOrAddString("WriteLine"),
            EncodeMethodSignature(SignatureCallingConvention.Default, true,
                returnType: b => b.Void(),
                parameters: new Action<ParameterTypeEncoder>[] { p => p.Type().String() }));

        m_writeLineInt64Ref = m_metadata.AddMemberReference(
            m_consoleRef,
            m_metadata.GetOrAddString("WriteLine"),
            EncodeMethodSignature(SignatureCallingConvention.Default, true,
                returnType: b => b.Void(),
                parameters: new Action<ParameterTypeEncoder>[] { p => p.Type().Int64() }));

        m_writeLineBoolRef = m_metadata.AddMemberReference(
            m_consoleRef,
            m_metadata.GetOrAddString("WriteLine"),
            EncodeMethodSignature(SignatureCallingConvention.Default, true,
                returnType: b => b.Void(),
                parameters: new Action<ParameterTypeEncoder>[] { p => p.Type().Boolean() }));

        m_stringConcatRef = m_metadata.AddMemberReference(
            m_stringRef,
            m_metadata.GetOrAddString("Concat"),
            EncodeMethodSignature(SignatureCallingConvention.Default, true,
                returnType: b => b.Type().String(),
                parameters: new Action<ParameterTypeEncoder>[]
                {
                    p => p.Type().String(),
                    p => p.Type().String()
                }));

        m_int64ToStringRef = m_metadata.AddMemberReference(
            m_objectRef,
            m_metadata.GetOrAddString("ToString"),
            EncodeMethodSignature(SignatureCallingConvention.Default, false,
                returnType: b => b.Type().String(),
                parameters: Array.Empty<Action<ParameterTypeEncoder>>()));

        m_boolToStringRef = m_metadata.AddMemberReference(
            m_objectRef,
            m_metadata.GetOrAddString("ToString"),
            EncodeMethodSignature(SignatureCallingConvention.Default, false,
                returnType: b => b.Type().String(),
                parameters: Array.Empty<Action<ParameterTypeEncoder>>()));

        m_objectCtorRef = m_metadata.AddMemberReference(
            m_objectRef,
            m_metadata.GetOrAddString(".ctor"),
            EncodeCtorSignature(Array.Empty<Action<ParameterTypeEncoder>>()));
    }

    public void EmitModule(IRModule module)
    {
        m_metadata.AddModule(
            0,
            m_metadata.GetOrAddString(m_assemblyName + ".dll"),
            m_metadata.GetOrAddGuid(Guid.NewGuid()),
            default,
            default);

        m_metadata.AddAssembly(
            m_metadata.GetOrAddString(m_assemblyName),
            new Version(1, 0, 0, 0),
            default,
            default,
            default,
            AssemblyHashAlgorithm.None);

        // <Module> must be the first type (row 1). It has no methods or fields of its own.
        m_metadata.AddTypeDefinition(
            default,
            default,
            m_metadata.GetOrAddString("<Module>"),
            default,
            MetadataTokens.FieldDefinitionHandle(1),
            MetadataTokens.MethodDefinitionHandle(1));

        EmitTypeDefinitions(module);

        // The static "Program" class holds all user-defined methods and the entry point.
        // It must be defined after type definitions so its MethodList points past the ctors.
        int firstMethodRow = m_metadata.GetRowCount(TableIndex.MethodDef) + 1;
        int firstFieldRow = m_metadata.GetRowCount(TableIndex.Field) + 1;
        m_moduleClassDef = m_metadata.AddTypeDefinition(
            TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit,
            m_metadata.GetOrAddString(""),
            m_metadata.GetOrAddString("Program"),
            m_objectRef,
            MetadataTokens.FieldDefinitionHandle(firstFieldRow),
            MetadataTokens.MethodDefinitionHandle(firstMethodRow));

        // Pre-collect generic arities for all definitions
        foreach (IRDefinition def in module.Definitions)
        {
            ImmutableArray<int> typeVarIds = CollectTypeVarIds(def.Type);
            m_definitionGenericArity = m_definitionGenericArity.Set(def.Name, typeVarIds.Length);
            m_definitionTypeVarIds = m_definitionTypeVarIds.Set(def.Name, typeVarIds);
        }

        // Pre-register method handles so recursive/forward calls resolve correctly.
        int methodRow = firstMethodRow;
        foreach (IRDefinition def in module.Definitions)
        {
            m_definedMethods = m_definedMethods.Set(def.Name, MetadataTokens.MethodDefinitionHandle(methodRow));
            methodRow++;
        }
        // Reserve a row for the synthetic Main entry point.
        m_definedMethods = m_definedMethods.Set("__entryMain", MetadataTokens.MethodDefinitionHandle(methodRow));

        foreach (IRDefinition def in module.Definitions)
        {
            EmitDefinition(def);
        }

        EmitEntryPoint(module);
    }

    void EmitDefinition(IRDefinition def)
    {
        ImmutableArray<int> typeVarIds = ImmutableArray<int>.Empty;
        m_definitionTypeVarIds.TryGet(def.Name, out typeVarIds);
        if (typeVarIds.IsDefault) typeVarIds = ImmutableArray<int>.Empty;
        m_currentTypeVarIds = typeVarIds;
        int genericArity = typeVarIds.Length;

        BlobBuilder sig = new();
        BlobEncoder sigEncoder = new(sig);

        int paramCount = def.Parameters.Length;
        CodexType returnType = ComputeReturnType(def.Type, paramCount);
        MethodSignatureEncoder methodSig = sigEncoder.MethodSignature(
            SignatureCallingConvention.Default, genericArity, false);
        methodSig.Parameters(paramCount,
            rt => EncodeType(rt.Type(), returnType),
            parameters =>
            {
                foreach (IRParameter param in def.Parameters)
                {
                    EncodeType(parameters.AddParameter().Type(), param.Type);
                }
            });

        bool isTco = HasSelfTailCall(def);

        ControlFlowBuilder controlFlow = new();
        InstructionEncoder il = new(new BlobBuilder(), controlFlow);
        LocalsBuilder locals = new(m_metadata, EncodeType);

        if (isTco)
        {
            EmitTailCallBody(il, def, locals);
        }
        else
        {
            EmitExpr(il, def.Body, locals, def.Parameters);
            il.OpCode(ILOpCode.Ret);
        }

        int bodyOffset;
        if (locals.Count > 0)
        {
            StandaloneSignatureHandle localSig = locals.BuildSignature();
            bodyOffset = m_methodBodies.AddMethodBody(il, localVariablesSignature: localSig);
        }
        else
        {
            bodyOffset = m_methodBodies.AddMethodBody(il);
        }

        MethodDefinitionHandle methodDef = m_metadata.AddMethodDefinition(
            MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig,
            MethodImplAttributes.IL | MethodImplAttributes.Managed,
            m_metadata.GetOrAddString(SanitizeName(def.Name)),
            m_metadata.GetOrAddBlob(sig),
            bodyOffset,
            default);

        for (int i = 0; i < def.Parameters.Length; i++)
        {
            m_metadata.AddParameter(
                ParameterAttributes.None,
                m_metadata.GetOrAddString(SanitizeName(def.Parameters[i].Name)),
                i + 1);
        }

        for (int i = 0; i < genericArity; i++)
        {
            m_metadata.AddGenericParameter(
                methodDef,
                GenericParameterAttributes.None,
                m_metadata.GetOrAddString($"T{typeVarIds[i]}"),
                i);
        }

        m_currentTypeVarIds = ImmutableArray<int>.Empty;
    }

    void EmitEntryPoint(IRModule module)
    {
        IRDefinition? mainDef = null;
        foreach (IRDefinition d in module.Definitions)
        {
            if (d.Name == "main" && d.Parameters.Length == 0)
            {
                mainDef = d;
                break;
            }
        }

        if (mainDef is null) return;

        ControlFlowBuilder entryControlFlow = new();
        InstructionEncoder il = new(new BlobBuilder(), entryControlFlow);

        bool isEffectful = mainDef.Type is EffectfulType or VoidType;

        if (isEffectful)
        {
            il.Call(m_definedMethods["main"]!.Value);
        }
        else
        {
            il.Call(m_definedMethods["main"]!.Value);
            MemberReferenceHandle writeLine = mainDef.Type switch
            {
                IntegerType => m_writeLineInt64Ref,
                BooleanType => m_writeLineBoolRef,
                _ => m_writeLineStringRef,
            };
            il.Call(writeLine);
        }

        il.OpCode(ILOpCode.Ret);

        int bodyOffset = m_methodBodies.AddMethodBody(il);

        BlobBuilder sig = new();
        BlobEncoder sigEncoder = new(sig);
        sigEncoder.MethodSignature().Parameters(0,
            returnType => returnType.Void(),
            _ => { });

        m_metadata.AddMethodDefinition(
            MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig,
            MethodImplAttributes.IL | MethodImplAttributes.Managed,
            m_metadata.GetOrAddString("Main"),
            m_metadata.GetOrAddBlob(sig),
            bodyOffset,
            default);
    }

    void EmitExpr(InstructionEncoder il, IRExpr expr, LocalsBuilder locals, ImmutableArray<IRParameter> parameters)
    {
        switch (expr)
        {
            case IRTextLit t:
                il.LoadString(m_metadata.GetOrAddUserString(t.Value));
                break;

            case IRIntegerLit i:
                il.LoadConstantI8(i.Value);
                break;

            case IRNumberLit n:
                il.LoadConstantR8((double)n.Value);
                break;

            case IRBoolLit b:
                il.LoadConstantI4(b.Value ? 1 : 0);
                break;

            case IRNegate neg:
                EmitExpr(il, neg.Operand, locals, parameters);
                il.OpCode(ILOpCode.Neg);
                break;

            case IRName name:
                int paramIndex = FindParameter(name.Name, parameters);
                if (paramIndex >= 0)
                {
                    il.LoadArgument(paramIndex);
                }
                else if (locals.TryGetLocal(name.Name, out int localIndex))
                {
                    il.LoadLocal(localIndex);
                }
                else if (m_ctorDefs.TryGet(name.Name, out MethodDefinitionHandle ctorDef)
                    && name.Type is not FunctionType)
                {
                    il.OpCode(ILOpCode.Newobj);
                    il.Token(ctorDef);
                }
                else if (m_definedMethods.TryGet(name.Name, out MethodDefinitionHandle methodRef))
                {
                    EmitCallToMethod(il, name.Name, methodRef, ImmutableArray<IRExpr>.Empty);
                }
                break;

            case IRBinary bin:
                EmitBinary(il, bin, locals, parameters);
                break;

            case IRIf ifExpr:
                EmitIf(il, ifExpr, locals, parameters);
                break;

            case IRLet letExpr:
                EmitLet(il, letExpr, locals, parameters);
                break;

            case IRApply apply:
                EmitApply(il, apply, locals, parameters);
                break;

            case IRDo doExpr:
                EmitDo(il, doExpr, locals, parameters);
                break;

            case IRRecord rec:
                EmitRecordConstruction(il, rec, locals, parameters);
                break;

            case IRFieldAccess fa:
                EmitFieldAccess(il, fa, locals, parameters);
                break;

            case IRMatch match:
                EmitMatch(il, match, locals, parameters);
                break;
        }
    }

    void EmitBinary(InstructionEncoder il, IRBinary bin, LocalsBuilder locals,
        ImmutableArray<IRParameter> parameters)
    {
        if (bin.Op == IRBinaryOp.AppendText)
        {
            EmitExpr(il, bin.Left, locals, parameters);
            EmitExpr(il, bin.Right, locals, parameters);
            il.Call(m_stringConcatRef);
            return;
        }

        EmitExpr(il, bin.Left, locals, parameters);
        EmitExpr(il, bin.Right, locals, parameters);

        switch (bin.Op)
        {
            case IRBinaryOp.AddInt or IRBinaryOp.AddNum:
                il.OpCode(ILOpCode.Add);
                break;
            case IRBinaryOp.SubInt or IRBinaryOp.SubNum:
                il.OpCode(ILOpCode.Sub);
                break;
            case IRBinaryOp.MulInt or IRBinaryOp.MulNum:
                il.OpCode(ILOpCode.Mul);
                break;
            case IRBinaryOp.DivInt or IRBinaryOp.DivNum:
                il.OpCode(ILOpCode.Div);
                break;
            case IRBinaryOp.Eq:
                il.OpCode(ILOpCode.Ceq);
                break;
            case IRBinaryOp.NotEq:
                il.OpCode(ILOpCode.Ceq);
                il.LoadConstantI4(0);
                il.OpCode(ILOpCode.Ceq);
                break;
            case IRBinaryOp.Lt:
                il.OpCode(ILOpCode.Clt);
                break;
            case IRBinaryOp.Gt:
                il.OpCode(ILOpCode.Cgt);
                break;
            case IRBinaryOp.LtEq:
                il.OpCode(ILOpCode.Cgt);
                il.LoadConstantI4(0);
                il.OpCode(ILOpCode.Ceq);
                break;
            case IRBinaryOp.GtEq:
                il.OpCode(ILOpCode.Clt);
                il.LoadConstantI4(0);
                il.OpCode(ILOpCode.Ceq);
                break;
            case IRBinaryOp.And:
                il.OpCode(ILOpCode.And);
                break;
            case IRBinaryOp.Or:
                il.OpCode(ILOpCode.Or);
                break;
        }
    }

    void EmitIf(InstructionEncoder il, IRIf ifExpr, LocalsBuilder locals,
        ImmutableArray<IRParameter> parameters)
    {
        LabelHandle elseLabel = il.DefineLabel();
        LabelHandle endLabel = il.DefineLabel();

        EmitExpr(il, ifExpr.Condition, locals, parameters);
        il.Branch(ILOpCode.Brfalse, elseLabel);

        EmitExpr(il, ifExpr.Then, locals, parameters);
        il.Branch(ILOpCode.Br, endLabel);

        il.MarkLabel(elseLabel);
        EmitExpr(il, ifExpr.Else, locals, parameters);

        il.MarkLabel(endLabel);
    }

    void EmitLet(InstructionEncoder il, IRLet letExpr, LocalsBuilder locals,
        ImmutableArray<IRParameter> parameters)
    {
        EmitExpr(il, letExpr.Value, locals, parameters);
        int localIndex = locals.AddLocal(letExpr.Name, letExpr.Value.Type);
        il.StoreLocal(localIndex);
        EmitExpr(il, letExpr.Body, locals, parameters);
    }

    void EmitApply(InstructionEncoder il, IRApply apply, LocalsBuilder locals,
        ImmutableArray<IRParameter> parameters)
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
            if (m_ctorDefs.TryGet(funcName.Name, out MethodDefinitionHandle ctorDef))
            {
                foreach (IRExpr arg in args)
                {
                    EmitExpr(il, arg, locals, parameters);
                }
                il.OpCode(ILOpCode.Newobj);
                il.Token(ctorDef);
                return;
            }

            if (m_definedMethods.TryGet(funcName.Name, out MethodDefinitionHandle methodDef))
            {
                foreach (IRExpr arg in args)
                {
                    EmitExpr(il, arg, locals, parameters);
                }
                ImmutableArray<IRExpr> argArray = args.ToImmutableArray();
                EmitCallToMethod(il, funcName.Name, methodDef, argArray);
            }
        }
    }

    void EmitCallToMethod(InstructionEncoder il, string name,
        MethodDefinitionHandle methodDef, ImmutableArray<IRExpr> args)
    {
        if (!m_definitionGenericArity.TryGet(name, out int genericArity) || genericArity == 0)
        {
            il.Call(methodDef);
            return;
        }

        // Build a MethodSpec with instantiated type arguments.
        // For erased generics, all type args become object.
        BlobBuilder specSig = new();
        BlobEncoder specEncoder = new(specSig);
        GenericTypeArgumentsEncoder genSig = specEncoder.MethodSpecificationSignature(genericArity);
        for (int i = 0; i < genericArity; i++)
        {
            genSig.AddArgument().Object();
        }

        MethodSpecificationHandle methodSpec = m_metadata.AddMethodSpecification(
            methodDef,
            m_metadata.GetOrAddBlob(specSig));
        il.Call(methodSpec);
    }

    void EmitDo(InstructionEncoder il, IRDo doExpr, LocalsBuilder locals,
        ImmutableArray<IRParameter> parameters)
    {
        for (int i = 0; i < doExpr.Statements.Length; i++)
        {
            IRDoStatement stmt = doExpr.Statements[i];
            switch (stmt)
            {
                case IRDoBind bind:
                    EmitExpr(il, bind.Value, locals, parameters);
                    int localIndex = locals.AddLocal(bind.Name, bind.NameType);
                    il.StoreLocal(localIndex);
                    break;
                case IRDoExec exec:
                    EmitExpr(il, exec.Expression, locals, parameters);
                    bool isLast = i == doExpr.Statements.Length - 1;
                    if (!isLast && exec.Expression.Type is not VoidType)
                    {
                        il.OpCode(ILOpCode.Pop);
                    }
                    break;
            }
        }
    }

    public byte[] Build()
    {
        BlobBuilder peBlob = new();

        MethodDefinitionHandle entryPoint = m_definedMethods.TryGet("__entryMain", out MethodDefinitionHandle ep)
            ? ep
            : MetadataTokens.MethodDefinitionHandle(m_metadata.GetRowCount(TableIndex.MethodDef));

        ManagedPEBuilder peBuilder = new(
            header: new PEHeaderBuilder(imageCharacteristics: Characteristics.ExecutableImage),
            metadataRootBuilder: new MetadataRootBuilder(m_metadata),
            ilStream: m_ilStream,
            entryPoint: entryPoint);

        peBuilder.Serialize(peBlob);
        return peBlob.ToArray();
    }

    void EncodeType(SignatureTypeEncoder encoder, CodexType type)
    {
        switch (type)
        {
            case IntegerType:
                encoder.Int64();
                break;
            case NumberType:
                encoder.Double();
                break;
            case TextType:
                encoder.String();
                break;
            case BooleanType:
                encoder.Boolean();
                break;
            case VoidType or NothingType:
                encoder.Builder.WriteByte((byte)SignatureTypeCode.Void);
                break;
            case RecordType rec:
                EncodeUserType(encoder, SanitizeName(rec.TypeName.Value));
                break;
            case SumType sum:
                EncodeUserType(encoder, SanitizeName(sum.TypeName.Value));
                break;
            case ConstructedType ct:
                EncodeUserType(encoder, SanitizeName(ct.Constructor.Value));
                break;
            case TypeVariable tv:
                int mvarIndex = m_currentTypeVarIds.IndexOf(tv.Id);
                if (mvarIndex >= 0)
                {
                    encoder.Builder.WriteByte((byte)SignatureTypeCode.GenericMethodParameter);
                    encoder.Builder.WriteCompressedInteger(mvarIndex);
                }
                else
                {
                    encoder.Object();
                }
                break;
            case ForAllType fa:
                EncodeType(encoder, fa.Body);
                break;
            default:
                encoder.Object();
                break;
        }
    }

    void EncodeUserType(SignatureTypeEncoder encoder, string typeName)
    {
        if (m_emittedTypes.TryGet(typeName, out TypeDefinitionHandle handle))
        {
            encoder.Type(handle, false);
        }
        else
        {
            encoder.Object();
        }
    }

    BlobHandle EncodeMethodSignature(
        SignatureCallingConvention convention,
        bool isStatic,
        Action<ReturnTypeEncoder> returnType,
        Action<ParameterTypeEncoder>[] parameters)
    {
        BlobBuilder sig = new();
        BlobEncoder encoder = new(sig);
        MethodSignatureEncoder methodSig = encoder.MethodSignature(convention, 0, !isStatic);
        methodSig.Parameters(parameters.Length,
            returnType,
            p =>
            {
                foreach (Action<ParameterTypeEncoder> param in parameters)
                {
                    ParameterTypeEncoder paramEncoder = p.AddParameter();
                    param(paramEncoder);
                }
            });
        return m_metadata.GetOrAddBlob(sig);
    }

    static int FindParameter(string name, ImmutableArray<IRParameter> parameters)
    {
        for (int i = 0; i < parameters.Length; i++)
        {
            if (parameters[i].Name == name) return i;
        }
        return -1;
    }

    static string SanitizeName(string name) => name.Replace('-', '_').Replace('.', '_');

    static CodexType ComputeReturnType(CodexType fullType, int parameterCount)
    {
        CodexType current = fullType;
        // Unwrap ForAllType wrappers
        while (current is ForAllType fa)
            current = fa.Body;
        for (int i = 0; i < parameterCount; i++)
        {
            if (current is FunctionType ft)
                current = ft.Return;
            else
                break;
        }
        if (current is EffectfulType eft)
            current = eft.Return;
        return current;
    }

    // ── Generics helpers ──────────────────────────────────────────

    static ImmutableArray<int> CollectTypeVarIds(CodexType type)
    {
        HashSet<int> ids = [];
        CollectTypeVarIdsInto(type, ids);
        return ids.Order().ToImmutableArray();
    }

    static void CollectTypeVarIdsInto(CodexType type, HashSet<int> ids)
    {
        switch (type)
        {
            case TypeVariable tv:
                ids.Add(tv.Id);
                break;
            case FunctionType ft:
                CollectTypeVarIdsInto(ft.Parameter, ids);
                CollectTypeVarIdsInto(ft.Return, ids);
                break;
            case ListType lt:
                CollectTypeVarIdsInto(lt.Element, ids);
                break;
            case ForAllType fa:
                CollectTypeVarIdsInto(fa.Body, ids);
                break;
            case ConstructedType ct:
                foreach (CodexType arg in ct.Arguments)
                    CollectTypeVarIdsInto(arg, ids);
                break;
        }
    }

    // ── Tail-call optimization ────────────────────────────────────

    static bool HasSelfTailCall(IRDefinition def)
    {
        if (def.Parameters.Length == 0)
            return false;
        return ExprHasTailCall(def.Body, def.Name);
    }

    static bool ExprHasTailCall(IRExpr expr, string funcName)
    {
        return expr switch
        {
            IRIf iff => ExprHasTailCall(iff.Then, funcName)
                     || ExprHasTailCall(iff.Else, funcName),
            IRLet let => ExprHasTailCall(let.Body, funcName),
            IRMatch match => match.Branches.Any(b => ExprHasTailCall(b.Body, funcName)),
            IRApply app => IsSelfCall(app, funcName),
            _ => false
        };
    }

    static bool IsSelfCall(IRApply app, string funcName)
    {
        IRExpr root = app.Function;
        while (root is IRApply inner)
            root = inner.Function;
        return root is IRName name && name.Name == funcName;
    }

    void EmitTailCallBody(InstructionEncoder il, IRDefinition def, LocalsBuilder locals)
    {
        // Allocate locals that mirror each parameter so we can reassign them in the loop
        int[] paramLocals = new int[def.Parameters.Length];
        for (int i = 0; i < def.Parameters.Length; i++)
        {
            paramLocals[i] = locals.AddLocal($"__tco_p{i}", def.Parameters[i].Type);
            il.LoadArgument(i);
            il.StoreLocal(paramLocals[i]);
        }

        // Build a modified parameter list that reads from locals instead of args
        ImmutableArray<IRParameter> tcoParams = def.Parameters;

        LabelHandle loopStart = il.DefineLabel();
        il.MarkLabel(loopStart);

        EmitTailCallExpr(il, def.Body, def.Name, tcoParams, paramLocals, locals, loopStart);
    }

    void EmitTailCallExpr(InstructionEncoder il, IRExpr expr, string funcName,
        ImmutableArray<IRParameter> parameters, int[] paramLocals,
        LocalsBuilder locals, LabelHandle loopStart)
    {
        switch (expr)
        {
            case IRIf iff:
            {
                LabelHandle elseLabel = il.DefineLabel();

                EmitTcoExpr(il, iff.Condition, locals, parameters, paramLocals);
                il.Branch(ILOpCode.Brfalse, elseLabel);

                EmitTailCallExpr(il, iff.Then, funcName, parameters, paramLocals, locals, loopStart);

                il.MarkLabel(elseLabel);
                EmitTailCallExpr(il, iff.Else, funcName, parameters, paramLocals, locals, loopStart);
                break;
            }

            case IRLet let:
            {
                EmitTcoExpr(il, let.Value, locals, parameters, paramLocals);
                int localIndex = locals.AddLocal(let.Name, let.Value.Type);
                il.StoreLocal(localIndex);
                EmitTailCallExpr(il, let.Body, funcName, parameters, paramLocals, locals, loopStart);
                break;
            }

            case IRMatch match:
            {
                EmitTcoMatch(il, match, funcName, parameters, paramLocals, locals, loopStart);
                break;
            }

            case IRApply app when IsSelfCall(app, funcName):
            {
                List<IRExpr> args = new();
                IRExpr func = app;
                while (func is IRApply inner)
                {
                    args.Add(inner.Argument);
                    func = inner.Function;
                }
                args.Reverse();

                // Evaluate all arguments into temp locals
                int[] tempLocals = new int[args.Count];
                for (int i = 0; i < args.Count && i < parameters.Length; i++)
                {
                    EmitTcoExpr(il, args[i], locals, parameters, paramLocals);
                    tempLocals[i] = locals.AddLocal($"__tco_t{i}", parameters[i].Type);
                    il.StoreLocal(tempLocals[i]);
                }
                // Assign temps to parameter locals
                for (int i = 0; i < args.Count && i < parameters.Length; i++)
                {
                    il.LoadLocal(tempLocals[i]);
                    il.StoreLocal(paramLocals[i]);
                }
                il.Branch(ILOpCode.Br, loopStart);
                break;
            }

            default:
            {
                EmitTcoExpr(il, expr, locals, parameters, paramLocals);
                il.OpCode(ILOpCode.Ret);
                break;
            }
        }
    }

    void EmitTcoMatch(InstructionEncoder il, IRMatch match, string funcName,
        ImmutableArray<IRParameter> parameters, int[] paramLocals,
        LocalsBuilder locals, LabelHandle loopStart)
    {
        EmitTcoExpr(il, match.Scrutinee, locals, parameters, paramLocals);
        int scrutineeLocal = locals.AddLocal("__tco_scrutinee", match.Scrutinee.Type);
        il.StoreLocal(scrutineeLocal);

        int resultLocal = locals.AddLocal("__tco_match_result", match.Type);

        LabelHandle endLabel = il.DefineLabel();
        bool anyBranchIsTailCall = match.Branches.Any(b => ExprHasTailCall(b.Body, funcName));

        for (int i = 0; i < match.Branches.Length; i++)
        {
            IRMatchBranch branch = match.Branches[i];
            bool isLast = i == match.Branches.Length - 1;

            switch (branch.Pattern)
            {
                case IRWildcardPattern:
                    if (ExprHasTailCall(branch.Body, funcName))
                    {
                        EmitTailCallExpr(il, branch.Body, funcName, parameters, paramLocals, locals, loopStart);
                    }
                    else
                    {
                        EmitTcoExpr(il, branch.Body, locals, parameters, paramLocals);
                        il.StoreLocal(resultLocal);
                        if (!isLast) il.Branch(ILOpCode.Br, endLabel);
                    }
                    break;

                case IRVarPattern varPat:
                    il.LoadLocal(scrutineeLocal);
                    int varLocal = locals.AddLocal(varPat.Name, varPat.Type);
                    il.StoreLocal(varLocal);
                    if (ExprHasTailCall(branch.Body, funcName))
                    {
                        EmitTailCallExpr(il, branch.Body, funcName, parameters, paramLocals, locals, loopStart);
                    }
                    else
                    {
                        EmitTcoExpr(il, branch.Body, locals, parameters, paramLocals);
                        il.StoreLocal(resultLocal);
                        if (!isLast) il.Branch(ILOpCode.Br, endLabel);
                    }
                    break;

                case IRLiteralPattern litPat:
                {
                    LabelHandle nextLabel = il.DefineLabel();
                    il.LoadLocal(scrutineeLocal);
                    switch (litPat.Value)
                    {
                        case long l:
                            il.LoadConstantI8(l);
                            break;
                        case bool b:
                            il.LoadConstantI4(b ? 1 : 0);
                            break;
                        case string s:
                            il.LoadString(m_metadata.GetOrAddUserString(s));
                            break;
                        default:
                            il.LoadConstantI8(0);
                            break;
                    }
                    il.OpCode(ILOpCode.Ceq);
                    il.Branch(ILOpCode.Brfalse, nextLabel);

                    if (ExprHasTailCall(branch.Body, funcName))
                    {
                        EmitTailCallExpr(il, branch.Body, funcName, parameters, paramLocals, locals, loopStart);
                    }
                    else
                    {
                        EmitTcoExpr(il, branch.Body, locals, parameters, paramLocals);
                        il.StoreLocal(resultLocal);
                        il.Branch(ILOpCode.Br, endLabel);
                    }

                    il.MarkLabel(nextLabel);
                    break;
                }

                case IRCtorPattern ctorPat:
                {
                    string ctorName = SanitizeName(ctorPat.Name);
                    LabelHandle nextLabel = il.DefineLabel();

                    if (!m_emittedTypes.TryGet(ctorName, out TypeDefinitionHandle ctorTypeDef))
                    {
                        il.MarkLabel(nextLabel);
                        break;
                    }

                    il.LoadLocal(scrutineeLocal);
                    il.OpCode(ILOpCode.Isinst);
                    il.Token(ctorTypeDef);
                    il.OpCode(ILOpCode.Dup);
                    il.Branch(ILOpCode.Brfalse, nextLabel);

                    int castLocal = locals.AddLocal($"__tco_cast_{ctorName}", ctorPat.Type);
                    il.StoreLocal(castLocal);

                    BindCtorSubPatterns(il, ctorPat, ctorName, castLocal, locals, parameters);

                    if (ExprHasTailCall(branch.Body, funcName))
                    {
                        EmitTailCallExpr(il, branch.Body, funcName, parameters, paramLocals, locals, loopStart);
                    }
                    else
                    {
                        EmitTcoExpr(il, branch.Body, locals, parameters, paramLocals);
                        il.StoreLocal(resultLocal);
                        il.Branch(ILOpCode.Br, endLabel);
                    }

                    il.MarkLabel(nextLabel);
                    il.OpCode(ILOpCode.Pop);
                    break;
                }
            }
        }

        il.MarkLabel(endLabel);
        il.LoadLocal(resultLocal);
        il.OpCode(ILOpCode.Ret);
    }

    void EmitTcoExpr(InstructionEncoder il, IRExpr expr, LocalsBuilder locals,
        ImmutableArray<IRParameter> parameters, int[] paramLocals)
    {
        // In TCO context, parameter references need to read from locals instead of args
        if (expr is IRName name)
        {
            int paramIndex = FindParameter(name.Name, parameters);
            if (paramIndex >= 0)
            {
                il.LoadLocal(paramLocals[paramIndex]);
                return;
            }
        }
        // For compound expressions that contain IRName references to params,
        // we swap out the EmitExpr call temporarily. We handle this by wrapping
        // with a special parameters mapping.
        EmitExprWithParamLocals(il, expr, locals, parameters, paramLocals);
    }

    void EmitExprWithParamLocals(InstructionEncoder il, IRExpr expr, LocalsBuilder locals,
        ImmutableArray<IRParameter> parameters, int[] paramLocals)
    {
        switch (expr)
        {
            case IRTextLit t:
                il.LoadString(m_metadata.GetOrAddUserString(t.Value));
                break;

            case IRIntegerLit i:
                il.LoadConstantI8(i.Value);
                break;

            case IRNumberLit n:
                il.LoadConstantR8((double)n.Value);
                break;

            case IRBoolLit b:
                il.LoadConstantI4(b.Value ? 1 : 0);
                break;

            case IRNegate neg:
                EmitTcoExpr(il, neg.Operand, locals, parameters, paramLocals);
                il.OpCode(ILOpCode.Neg);
                break;

            case IRName name:
            {
                int paramIndex = FindParameter(name.Name, parameters);
                if (paramIndex >= 0)
                {
                    il.LoadLocal(paramLocals[paramIndex]);
                }
                else if (locals.TryGetLocal(name.Name, out int localIndex))
                {
                    il.LoadLocal(localIndex);
                }
                else if (m_ctorDefs.TryGet(name.Name, out MethodDefinitionHandle ctorDef)
                    && name.Type is not FunctionType)
                {
                    il.OpCode(ILOpCode.Newobj);
                    il.Token(ctorDef);
                }
                else if (m_definedMethods.TryGet(name.Name, out MethodDefinitionHandle methodRef))
                {
                    EmitCallToMethod(il, name.Name, methodRef, ImmutableArray<IRExpr>.Empty);
                }
                break;
            }

            case IRBinary bin:
                EmitTcoBinary(il, bin, locals, parameters, paramLocals);
                break;

            case IRIf ifExpr:
            {
                LabelHandle elseLabel = il.DefineLabel();
                LabelHandle endLabel = il.DefineLabel();
                EmitTcoExpr(il, ifExpr.Condition, locals, parameters, paramLocals);
                il.Branch(ILOpCode.Brfalse, elseLabel);
                EmitTcoExpr(il, ifExpr.Then, locals, parameters, paramLocals);
                il.Branch(ILOpCode.Br, endLabel);
                il.MarkLabel(elseLabel);
                EmitTcoExpr(il, ifExpr.Else, locals, parameters, paramLocals);
                il.MarkLabel(endLabel);
                break;
            }

            case IRLet letExpr:
                EmitTcoExpr(il, letExpr.Value, locals, parameters, paramLocals);
                int letLocal = locals.AddLocal(letExpr.Name, letExpr.Value.Type);
                il.StoreLocal(letLocal);
                EmitTcoExpr(il, letExpr.Body, locals, parameters, paramLocals);
                break;

            case IRApply apply:
                EmitTcoApply(il, apply, locals, parameters, paramLocals);
                break;

            case IRDo doExpr:
                for (int i = 0; i < doExpr.Statements.Length; i++)
                {
                    IRDoStatement stmt = doExpr.Statements[i];
                    switch (stmt)
                    {
                        case IRDoBind bind:
                            EmitTcoExpr(il, bind.Value, locals, parameters, paramLocals);
                            int doLocal = locals.AddLocal(bind.Name, bind.NameType);
                            il.StoreLocal(doLocal);
                            break;
                        case IRDoExec exec:
                            EmitTcoExpr(il, exec.Expression, locals, parameters, paramLocals);
                            bool isLast = i == doExpr.Statements.Length - 1;
                            if (!isLast && exec.Expression.Type is not VoidType)
                            {
                                il.OpCode(ILOpCode.Pop);
                            }
                            break;
                    }
                }
                break;

            case IRRecord rec:
            {
                string typeName = SanitizeName(rec.TypeName);
                foreach ((string _, IRExpr value) in rec.Fields)
                {
                    EmitTcoExpr(il, value, locals, parameters, paramLocals);
                }
                if (m_ctorDefs.TryGet(typeName, out MethodDefinitionHandle ctorDef2))
                {
                    il.OpCode(ILOpCode.Newobj);
                    il.Token(ctorDef2);
                }
                break;
            }

            case IRFieldAccess fa:
            {
                EmitTcoExpr(il, fa.Record, locals, parameters, paramLocals);
                string ownerTypeName = ResolveOwnerTypeName(fa.Record.Type);
                string fieldName = SanitizeName(fa.FieldName);
                string fieldKey = $"{ownerTypeName}.{fieldName}";
                if (m_fieldDefs.TryGet(fieldKey, out FieldDefinitionHandle fieldHandle))
                {
                    il.OpCode(ILOpCode.Ldfld);
                    il.Token(fieldHandle);
                }
                break;
            }

            case IRMatch match:
                EmitTcoMatchExpr(il, match, locals, parameters, paramLocals);
                break;

            default:
                EmitExpr(il, expr, locals, parameters);
                break;
        }
    }

    void EmitTcoBinary(InstructionEncoder il, IRBinary bin, LocalsBuilder locals,
        ImmutableArray<IRParameter> parameters, int[] paramLocals)
    {
        if (bin.Op == IRBinaryOp.AppendText)
        {
            EmitTcoExpr(il, bin.Left, locals, parameters, paramLocals);
            EmitTcoExpr(il, bin.Right, locals, parameters, paramLocals);
            il.Call(m_stringConcatRef);
            return;
        }

        EmitTcoExpr(il, bin.Left, locals, parameters, paramLocals);
        EmitTcoExpr(il, bin.Right, locals, parameters, paramLocals);

        switch (bin.Op)
        {
            case IRBinaryOp.AddInt or IRBinaryOp.AddNum:
                il.OpCode(ILOpCode.Add);
                break;
            case IRBinaryOp.SubInt or IRBinaryOp.SubNum:
                il.OpCode(ILOpCode.Sub);
                break;
            case IRBinaryOp.MulInt or IRBinaryOp.MulNum:
                il.OpCode(ILOpCode.Mul);
                break;
            case IRBinaryOp.DivInt or IRBinaryOp.DivNum:
                il.OpCode(ILOpCode.Div);
                break;
            case IRBinaryOp.Eq:
                il.OpCode(ILOpCode.Ceq);
                break;
            case IRBinaryOp.NotEq:
                il.OpCode(ILOpCode.Ceq);
                il.LoadConstantI4(0);
                il.OpCode(ILOpCode.Ceq);
                break;
            case IRBinaryOp.Lt:
                il.OpCode(ILOpCode.Clt);
                break;
            case IRBinaryOp.Gt:
                il.OpCode(ILOpCode.Cgt);
                break;
            case IRBinaryOp.LtEq:
                il.OpCode(ILOpCode.Cgt);
                il.LoadConstantI4(0);
                il.OpCode(ILOpCode.Ceq);
                break;
            case IRBinaryOp.GtEq:
                il.OpCode(ILOpCode.Clt);
                il.LoadConstantI4(0);
                il.OpCode(ILOpCode.Ceq);
                break;
            case IRBinaryOp.And:
                il.OpCode(ILOpCode.And);
                break;
            case IRBinaryOp.Or:
                il.OpCode(ILOpCode.Or);
                break;
        }
    }

    void EmitTcoApply(InstructionEncoder il, IRApply apply, LocalsBuilder locals,
        ImmutableArray<IRParameter> parameters, int[] paramLocals)
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
            if (m_ctorDefs.TryGet(funcName.Name, out MethodDefinitionHandle ctorDef))
            {
                foreach (IRExpr arg in args)
                {
                    EmitTcoExpr(il, arg, locals, parameters, paramLocals);
                }
                il.OpCode(ILOpCode.Newobj);
                il.Token(ctorDef);
                return;
            }

            if (m_definedMethods.TryGet(funcName.Name, out MethodDefinitionHandle methodDef))
            {
                foreach (IRExpr arg in args)
                {
                    EmitTcoExpr(il, arg, locals, parameters, paramLocals);
                }
                ImmutableArray<IRExpr> argArray = args.ToImmutableArray();
                EmitCallToMethod(il, funcName.Name, methodDef, argArray);
            }
        }
    }

    void EmitTcoMatchExpr(InstructionEncoder il, IRMatch match, LocalsBuilder locals,
        ImmutableArray<IRParameter> parameters, int[] paramLocals)
    {
        EmitTcoExpr(il, match.Scrutinee, locals, parameters, paramLocals);
        int scrutineeLocal = locals.AddLocal("__tco_scr", match.Scrutinee.Type);
        il.StoreLocal(scrutineeLocal);

        int resultLocal = locals.AddLocal("__tco_mr", match.Type);
        LabelHandle endLabel = il.DefineLabel();

        for (int i = 0; i < match.Branches.Length; i++)
        {
            IRMatchBranch branch = match.Branches[i];
            bool isLast = i == match.Branches.Length - 1;

            switch (branch.Pattern)
            {
                case IRWildcardPattern:
                    EmitTcoExpr(il, branch.Body, locals, parameters, paramLocals);
                    il.StoreLocal(resultLocal);
                    if (!isLast) il.Branch(ILOpCode.Br, endLabel);
                    break;

                case IRVarPattern varPat:
                    il.LoadLocal(scrutineeLocal);
                    int varLocal = locals.AddLocal(varPat.Name, varPat.Type);
                    il.StoreLocal(varLocal);
                    EmitTcoExpr(il, branch.Body, locals, parameters, paramLocals);
                    il.StoreLocal(resultLocal);
                    if (!isLast) il.Branch(ILOpCode.Br, endLabel);
                    break;

                case IRLiteralPattern litPat:
                {
                    LabelHandle nextLabel = il.DefineLabel();
                    il.LoadLocal(scrutineeLocal);
                    switch (litPat.Value)
                    {
                        case long l:
                            il.LoadConstantI8(l);
                            break;
                        case bool b:
                            il.LoadConstantI4(b ? 1 : 0);
                            break;
                        case string s:
                            il.LoadString(m_metadata.GetOrAddUserString(s));
                            break;
                        default:
                            il.LoadConstantI8(0);
                            break;
                    }
                    il.OpCode(ILOpCode.Ceq);
                    il.Branch(ILOpCode.Brfalse, nextLabel);

                    EmitTcoExpr(il, branch.Body, locals, parameters, paramLocals);
                    il.StoreLocal(resultLocal);
                    il.Branch(ILOpCode.Br, endLabel);

                    il.MarkLabel(nextLabel);
                    break;
                }

                case IRCtorPattern ctorPat:
                {
                    string ctorName = SanitizeName(ctorPat.Name);
                    LabelHandle nextLabel = il.DefineLabel();

                    if (!m_emittedTypes.TryGet(ctorName, out TypeDefinitionHandle ctorTypeDef))
                    {
                        il.MarkLabel(nextLabel);
                        break;
                    }

                    il.LoadLocal(scrutineeLocal);
                    il.OpCode(ILOpCode.Isinst);
                    il.Token(ctorTypeDef);
                    il.OpCode(ILOpCode.Dup);
                    il.Branch(ILOpCode.Brfalse, nextLabel);

                    int castLocal = locals.AddLocal($"__tco_c_{ctorName}", ctorPat.Type);
                    il.StoreLocal(castLocal);

                    BindCtorSubPatterns(il, ctorPat, ctorName, castLocal, locals, parameters);

                    EmitTcoExpr(il, branch.Body, locals, parameters, paramLocals);
                    il.StoreLocal(resultLocal);
                    il.Branch(ILOpCode.Br, endLabel);

                    il.MarkLabel(nextLabel);
                    il.OpCode(ILOpCode.Pop);
                    break;
                }
            }
        }

        il.MarkLabel(endLabel);
        il.LoadLocal(resultLocal);
    }
}
