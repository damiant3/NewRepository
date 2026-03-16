using System.Collections.Immutable;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using Codex.IR;
using Codex.Types;

namespace Codex.Emit.IL;

sealed class ILAssemblyBuilder
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

    TypeDefinitionHandle m_moduleClassDef;
    readonly Dictionary<string, MethodDefinitionHandle> m_definedMethods = new();

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

        m_moduleClassDef = m_metadata.AddTypeDefinition(
            TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit,
            m_metadata.GetOrAddString(""),
            m_metadata.GetOrAddString("<Module>"),
            m_objectRef,
            MetadataTokens.FieldDefinitionHandle(1),
            MetadataTokens.MethodDefinitionHandle(1));

        // Pre-register method handles so recursive/forward calls resolve correctly.
        int row = 1;
        foreach (IRDefinition def in module.Definitions)
        {
            m_definedMethods[def.Name] = MetadataTokens.MethodDefinitionHandle(row);
            row++;
        }
        // Reserve a row for the synthetic Main entry point.
        m_definedMethods["__entryMain"] = MetadataTokens.MethodDefinitionHandle(row);

        foreach (IRDefinition def in module.Definitions)
        {
            EmitDefinition(def);
        }

        EmitEntryPoint(module);
    }

    void EmitDefinition(IRDefinition def)
    {
        BlobBuilder sig = new();
        BlobEncoder sigEncoder = new(sig);

        int paramCount = def.Parameters.Length;
        CodexType returnType = ComputeReturnType(def.Type, paramCount);
        MethodSignatureEncoder methodSig = sigEncoder.MethodSignature();
        methodSig.Parameters(paramCount,
            rt => EncodeType(rt.Type(), returnType),
            parameters =>
            {
                foreach (IRParameter param in def.Parameters)
                {
                    EncodeType(parameters.AddParameter().Type(), param.Type);
                }
            });

        ControlFlowBuilder controlFlow = new();
        InstructionEncoder il = new(new BlobBuilder(), controlFlow);
        LocalsBuilder locals = new(m_metadata);

        EmitExpr(il, def.Body, locals, def.Parameters);

        il.OpCode(ILOpCode.Ret);

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
            il.Call(m_definedMethods["main"]);
        }
        else
        {
            il.Call(m_definedMethods["main"]);
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
                else if (m_definedMethods.TryGetValue(name.Name, out MethodDefinitionHandle methodRef))
                {
                    il.Call(methodRef);
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

        if (func is IRName funcName && m_definedMethods.TryGetValue(funcName.Name, out MethodDefinitionHandle methodDef))
        {
            foreach (IRExpr arg in args)
            {
                EmitExpr(il, arg, locals, parameters);
            }
            il.Call(methodDef);
        }
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

        MethodDefinitionHandle entryPoint = m_definedMethods.TryGetValue("__entryMain", out MethodDefinitionHandle ep)
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
            default:
                encoder.Object();
                break;
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
}
