using System.Collections.Immutable;
using Codex.Core;
using Codex.Ast;

namespace Codex.Types;

public sealed class TypeChecker(DiagnosticBag diagnostics)
{
    readonly DiagnosticBag m_diagnostics = diagnostics;
    readonly Unifier m_unifier = new(diagnostics);
    TypeEnvironment m_env = TypeEnvironment.WithBuiltins();
    Map<string, CodexType> m_typeDefMap = Map<string, CodexType>.s_empty;
    Map<string, CtorInfo> m_ctorMap = Map<string, CtorInfo>.s_empty;
    Map<string, CodexType> m_typeParamEnv = Map<string, CodexType>.s_empty;
    Map<string, CodexType> m_typeLevelEnv = Map<string, CodexType>.s_empty;
    Set<string> m_currentEffects = Set<string>.s_empty;

    public Map<string, CodexType> CheckModule(Module module)
    {
        RegisterTypeDefinitions(module.TypeDefinitions);

        Map<string, CodexType> topLevelTypes = Map<string, CodexType>.s_empty;
        foreach (Definition def in module.Definitions)
        {
            Map<string, CodexType> savedTypeParams = m_typeParamEnv;
            m_typeParamEnv = Map<string, CodexType>.s_empty;
            CodexType declaredType = def.DeclaredType is not null
                ? ResolveTypeExpr(def.DeclaredType)
                : m_unifier.FreshVar();
            m_typeParamEnv = savedTypeParams;

            CodexType envType = def.DeclaredType is not null
                ? Generalize(declaredType)
                : declaredType;
            topLevelTypes = topLevelTypes.Set(def.Name.Value, declaredType);
            m_env = m_env.Bind(def.Name, envType);
        }

        foreach (Definition def in module.Definitions)
        {
            CodexType expectedType = topLevelTypes[def.Name.Value]!;
            CodexType envType = m_env.Lookup(def.Name)!;
            CodexType checkType = envType is ForAllType
                ? Instantiate(envType)
                : expectedType;
            CodexType bodyType = InferDefinition(def, checkType);
            m_unifier.Unify(checkType, bodyType, def.Span);
        }

        Map<string, CodexType> result = Map<string, CodexType>.s_empty;
        foreach (KeyValuePair<string, CodexType> kv in topLevelTypes)
        {
            CodexType t = m_unifier.DeepResolve(kv.Value);
            while (t is ForAllType fa)
                t = fa.Body;
            result = result.Set(kv.Key, t);
        }

        return result;
    }

    void RegisterTypeDefinitions(IReadOnlyList<TypeDef> typeDefs)
    {
        foreach (TypeDef td in typeDefs)
        {
            switch (td)
            {
                case RecordTypeDef rec:
                    m_typeDefMap = m_typeDefMap.Set(rec.Name.Value, new ConstructedType(rec.Name, []));
                    break;
                case VariantTypeDef variant:
                    m_typeDefMap = m_typeDefMap.Set(variant.Name.Value, new ConstructedType(variant.Name, []));
                    break;
            }
        }

        foreach (TypeDef td in typeDefs)
        {
            if (td is RecordTypeDef rec)
                RegisterRecord(rec);
        }

        foreach (TypeDef td in typeDefs)
        {
            if (td is VariantTypeDef variant)
                RegisterVariant(variant);
        }
    }

    void RegisterRecord(RecordTypeDef rec)
    {
        Map<string, CodexType> typeParamEnv = Map<string, CodexType>.s_empty;
        ImmutableArray<int>.Builder paramIds = ImmutableArray.CreateBuilder<int>();
        foreach (Name tp in rec.TypeParameters)
        {
            TypeVariable tv = m_unifier.FreshVar();
            typeParamEnv = typeParamEnv.Set(tp.Value, tv);
            paramIds.Add(tv.Id);
        }

        Map<string, CodexType> savedTypeParams = m_typeParamEnv;
        m_typeParamEnv = typeParamEnv;

        ImmutableArray<RecordFieldType>.Builder fields =
            ImmutableArray.CreateBuilder<RecordFieldType>();
        foreach (RecordFieldDef f in rec.Fields)
        {
            fields.Add(new(f.FieldName, ResolveTypeExpr(f.Type)));
        }
        RecordType recordType = new(rec.Name, paramIds.ToImmutable(), fields.ToImmutable());
        m_typeDefMap = m_typeDefMap.Set(rec.Name.Value, recordType);

        m_typeParamEnv = savedTypeParams;
    }

    void RegisterVariant(VariantTypeDef variant)
    {
        Map<string, CodexType> typeParamEnv = Map<string, CodexType>.s_empty;
        ImmutableArray<int>.Builder paramIds = ImmutableArray.CreateBuilder<int>();
        foreach (Name tp in variant.TypeParameters)
        {
            TypeVariable tv = m_unifier.FreshVar();
            typeParamEnv = typeParamEnv.Set(tp.Value, tv);
            paramIds.Add(tv.Id);
        }

        Map<string, CodexType> savedTypeParams = m_typeParamEnv;
        m_typeParamEnv = typeParamEnv;

        ImmutableArray<SumConstructorType>.Builder ctors =
            ImmutableArray.CreateBuilder<SumConstructorType>();
        foreach (VariantCtorDef c in variant.Constructors)
        {
            ImmutableArray<CodexType>.Builder ctorFields =
                ImmutableArray.CreateBuilder<CodexType>();
            foreach (VariantFieldDef f in c.Fields)
            {
                ctorFields.Add(ResolveTypeExpr(f.Type));
            }
            ctors.Add(new(c.Name, ctorFields.ToImmutable()));
        }
        SumType sumType = new(variant.Name, paramIds.ToImmutable(), ctors.ToImmutable());
        m_typeDefMap = m_typeDefMap.Set(variant.Name.Value, sumType);

        foreach (SumConstructorType ctor in sumType.Constructors)
        {
            CodexType ctorType = sumType;
            for (int i = ctor.Fields.Length - 1; i >= 0; i--)
            {
                ctorType = new FunctionType(ctor.Fields[i], ctorType);
            }
            for (int i = paramIds.Count - 1; i >= 0; i--)
            {
                ctorType = new ForAllType(paramIds[i], ctorType);
            }
            m_ctorMap = m_ctorMap.Set(ctor.Name.Value, new(ctorType, sumType));
            m_env = m_env.Bind(ctor.Name, ctorType);
        }

        m_typeParamEnv = savedTypeParams;
    }

    public Map<string, CodexType> TypeDefMap => m_typeDefMap;

    public Map<string, CtorInfo> ConstructorMap => m_ctorMap;

    CodexType InferDefinition(Definition def, CodexType expectedType)
    {
        TypeEnvironment savedEnv = m_env;
        Set<string> savedEffects = m_currentEffects;

        m_currentEffects = ExtractEffects(expectedType);

        CodexType currentExpected = expectedType;
        foreach (Parameter param in def.Parameters)
        {
            while (currentExpected is FunctionType skipFt && skipFt.Parameter is ProofType)
                currentExpected = skipFt.Return;

            CodexType paramType;
            if (currentExpected is FunctionType ft)
            {
                paramType = ft.Parameter;
                currentExpected = ft.Return;
            }
            else if (currentExpected is DependentFunctionType dep)
            {
                paramType = dep.ParamType;
                currentExpected = dep.Body;
            }
            else
            {
                paramType = m_unifier.FreshVar();
                CodexType returnType = m_unifier.FreshVar();
                m_unifier.Unify(currentExpected, new FunctionType(paramType, returnType), def.Span);
                currentExpected = returnType;
            }

            m_env = m_env.Bind(param.Name, paramType);
        }

        while (currentExpected is FunctionType skipFt2 && skipFt2.Parameter is ProofType)
            currentExpected = skipFt2.Return;

        CodexType bodyType = InferExpr(def.Body);
        m_unifier.Unify(currentExpected, bodyType, def.Body.Span);

        m_env = savedEnv;
        m_currentEffects = savedEffects;
        return expectedType;
    }

    CodexType InferExpr(Expr expr)
    {
        switch (expr)
        {
            case LiteralExpr lit:
                return lit.Kind switch
                {
                    LiteralKind.Integer => IntegerType.s_instance,
                    LiteralKind.Number => NumberType.s_instance,
                    LiteralKind.Text => TextType.s_instance,
                    LiteralKind.Boolean => BooleanType.s_instance,
                    _ => ErrorType.s_instance
                };

            case NameExpr name:
                CodexType? looked = m_env.Lookup(name.Name);
                if (looked is null)
                {
                    m_diagnostics.Error("CDX2002",
                        $"Unknown name '{name.Name.Value}' during type checking",
                        name.Span);
                    return ErrorType.s_instance;
                }
                return Instantiate(looked);

            case BinaryExpr bin:
                return InferBinary(bin);

            case UnaryExpr un:
                CodexType operandType = InferExpr(un.Operand);
                m_unifier.Unify(operandType, IntegerType.s_instance, un.Span);
                return IntegerType.s_instance;

            case ApplyExpr app:
                return InferApplication(app);

            case IfExpr iff:
                CodexType condType = InferExpr(iff.Condition);
                m_unifier.Unify(condType, BooleanType.s_instance, iff.Condition.Span);
                CodexType thenType = InferExpr(iff.Then);
                CodexType elseType = InferExpr(iff.Else);
                m_unifier.Unify(thenType, elseType, iff.Span);
                return thenType;

            case LetExpr let:
                return InferLet(let);

            case LambdaExpr lam:
                return InferLambda(lam);

            case MatchExpr match:
                return InferMatch(match);

            case ListExpr list:
                return InferList(list);

            case RecordExpr rec:
                return InferRecord(rec);

            case FieldAccessExpr fa:
                return InferFieldAccess(fa);

            case DoExpr doExpr:
                return InferDoExpr(doExpr);

            case ErrorExpr:
                return ErrorType.s_instance;

            default:
                return ErrorType.s_instance;
        }
    }

    CodexType InferBinary(BinaryExpr bin)
    {
        CodexType leftType = InferExpr(bin.Left);
        CodexType rightType = InferExpr(bin.Right);

        return bin.Op switch
        {
            BinaryOp.Add or BinaryOp.Sub or BinaryOp.Mul or BinaryOp.Div or BinaryOp.Pow =>
                InferArithmetic(leftType, rightType, bin),

            BinaryOp.Eq or BinaryOp.NotEq or BinaryOp.Lt or BinaryOp.Gt
                or BinaryOp.LtEq or BinaryOp.GtEq or BinaryOp.DefEq =>
                InferComparison(leftType, rightType, bin),

            BinaryOp.And or BinaryOp.Or =>
                InferLogical(leftType, rightType, bin),

            BinaryOp.Append =>
                InferAppend(leftType, rightType, bin),

            BinaryOp.Cons =>
                InferCons(leftType, rightType, bin),

            _ => ErrorType.s_instance
        };
    }

    CodexType InferArithmetic(CodexType left, CodexType right, BinaryExpr bin)
    {
        m_unifier.Unify(left, right, bin.Span);
        CodexType resolved = m_unifier.Resolve(left);
        if (resolved is not (IntegerType or NumberType or TypeVariable or ErrorType))
        {
            m_diagnostics.Error("CDX2003",
                $"Arithmetic operator requires Integer or Number, but found {resolved}",
                bin.Span);
        }
        return left;
    }

    BooleanType InferComparison(CodexType left, CodexType right, BinaryExpr bin)
    {
        m_unifier.Unify(left, right, bin.Span);
        return BooleanType.s_instance;
    }

    BooleanType InferLogical(CodexType left, CodexType right, BinaryExpr bin)
    {
        m_unifier.Unify(left, BooleanType.s_instance, bin.Left.Span);
        m_unifier.Unify(right, BooleanType.s_instance, bin.Right.Span);
        return BooleanType.s_instance;
    }

    CodexType InferAppend(CodexType left, CodexType right, BinaryExpr bin)
    {
        CodexType resolved = m_unifier.Resolve(left);
        if (resolved is TextType)
        {
            m_unifier.Unify(right, TextType.s_instance, bin.Right.Span);
            return TextType.s_instance;
        }
        m_unifier.Unify(left, right, bin.Span);
        return left;
    }

    ListType InferCons(CodexType left, CodexType right, BinaryExpr bin)
    {
        ListType listType = new(left);
        m_unifier.Unify(right, listType, bin.Span);
        return listType;
    }

    CodexType InferApplication(ApplyExpr app)
    {
        CodexType funcType = InferExpr(app.Function);
        CodexType resolvedFunc = m_unifier.Resolve(funcType);

        if (resolvedFunc is DependentFunctionType dep)
        {
            CodexType argType = InferExpr(app.Argument);
            m_unifier.Unify(argType, dep.ParamType, app.Span);
            CodexType? argValue = TryExtractTypeLevelValue(app.Argument);
            CodexType body = argValue is not null
                ? SubstituteTypeLevelVar(dep.Body, dep.ParamName, argValue)
                : dep.Body;
            body = TryDischargeProofParams(body, app.Span);
            CheckEffectAllowed(body, app.Span);
            return body;
        }

        CodexType argType2 = InferExpr(app.Argument);
        CodexType returnType = m_unifier.FreshVar();

        if (!m_unifier.Unify(funcType, new FunctionType(argType2, returnType), app.Span))
        {
            return ErrorType.s_instance;
        }

        CodexType resolved = m_unifier.Resolve(returnType);
        resolved = TryDischargeProofParams(resolved, app.Span);
        CheckEffectAllowed(resolved, app.Span);

        return resolved;
    }

    CodexType InferLet(LetExpr let)
    {
        TypeEnvironment savedEnv = m_env;

        foreach (LetBinding binding in let.Bindings)
        {
            CodexType valueType = InferExpr(binding.Value);
            m_env = m_env.Bind(binding.Name, valueType);
        }

        CodexType bodyType = InferExpr(let.Body);
        m_env = savedEnv;
        return bodyType;
    }

    CodexType InferLambda(LambdaExpr lam)
    {
        TypeEnvironment savedEnv = m_env;

        List<CodexType> paramTypes = [];
        foreach (Parameter p in lam.Parameters)
        {
            CodexType paramType = m_unifier.FreshVar();
            paramTypes.Add(paramType);
            m_env = m_env.Bind(p.Name, paramType);
        }

        CodexType bodyType = InferExpr(lam.Body);
        m_env = savedEnv;

        CodexType result = bodyType;
        for (int i = paramTypes.Count - 1; i >= 0; i--)
        {
            result = new FunctionType(paramTypes[i], result);
        }

        return result;
    }

    CodexType InferMatch(MatchExpr match)
    {
        CodexType scrutineeType = InferExpr(match.Scrutinee);
        CodexType resultType = m_unifier.FreshVar();

        foreach (MatchBranch branch in match.Branches)
        {
            TypeEnvironment savedEnv = m_env;
            CheckPattern(branch.Pattern, scrutineeType);
            CodexType branchType = InferExpr(branch.Body);
            m_unifier.Unify(resultType, branchType, branch.Span);
            m_env = savedEnv;
        }

        CheckExhaustiveness(match, scrutineeType);

        return resultType;
    }

    void CheckExhaustiveness(MatchExpr match, CodexType scrutineeType)
    {
        CodexType resolved = m_unifier.Resolve(scrutineeType);
        if (resolved is not SumType sumType)
            return;

        bool hasCatchAll = match.Branches.Any(b =>
            b.Pattern is VarPattern or WildcardPattern);
        if (hasCatchAll)
            return;

        Set<string> covered = Set<string>.s_empty;
        foreach (MatchBranch branch in match.Branches)
        {
            if (branch.Pattern is CtorPattern cp)
                covered = covered.Add(cp.Constructor.Value);
        }

        List<string> missing = [];
        foreach (SumConstructorType ctor in sumType.Constructors)
        {
            if (!covered.Contains(ctor.Name.Value))
                missing.Add(ctor.Name.Value);
        }

        if (missing.Count > 0)
        {
            string missingStr = string.Join(", ", missing);
            m_diagnostics.Warning("CDX2020",
                $"Non-exhaustive match: missing constructor(s) {missingStr}",
                match.Span);
        }
    }

    void CheckPattern(Pattern pattern, CodexType expectedType)
    {
        switch (pattern)
        {
            case VarPattern v:
                m_env = m_env.Bind(v.Name, expectedType);
                break;

            case LiteralPattern lit:
                CodexType litType = lit.Kind switch
                {
                    LiteralKind.Integer => IntegerType.s_instance,
                    LiteralKind.Text => TextType.s_instance,
                    LiteralKind.Boolean => BooleanType.s_instance,
                    _ => ErrorType.s_instance
                };
                m_unifier.Unify(expectedType, litType, lit.Span);
                break;

            case CtorPattern ctor:
                CtorInfo? info = m_ctorMap[ctor.Constructor.Value];
                if (info is not null)
                {
                    CodexType ctorType = Instantiate(info.ConstructorType);
                    CodexType resultType = ctorType;
                    List<CodexType> fieldTypes = [];
                    while (resultType is FunctionType ft)
                    {
                        fieldTypes.Add(ft.Parameter);
                        resultType = ft.Return;
                    }
                    m_unifier.Unify(expectedType, resultType, ctor.Span);

                    int count = Math.Min(ctor.SubPatterns.Count, fieldTypes.Count);
                    for (int i = 0; i < count; i++)
                    {
                        CheckPattern(ctor.SubPatterns[i], fieldTypes[i]);
                    }
                }
                else
                {
                    foreach (Pattern sub in ctor.SubPatterns)
                        CheckPattern(sub, m_unifier.FreshVar());
                }
                break;

            case WildcardPattern:
                break;
        }
    }

    CodexType InferRecord(RecordExpr rec)
    {
        if (rec.TypeName is null)
            return m_unifier.FreshVar();

        CodexType? typeDef = m_typeDefMap[rec.TypeName.Value.Value];
        if (typeDef is not RecordType recordType)
            return m_unifier.FreshVar();

        foreach (RecordFieldExpr field in rec.Fields)
        {
            RecordFieldType? expectedField = recordType.Fields
                .FirstOrDefault(f => f.FieldName.Value == field.FieldName.Value);
            CodexType fieldType = InferExpr(field.Value);
            if (expectedField is not null)
            {
                m_unifier.Unify(fieldType, expectedField.Type, field.Span);
            }
        }
        return recordType;
    }

    CodexType InferFieldAccess(FieldAccessExpr fa)
    {
        CodexType recordType = InferExpr(fa.Record);
        CodexType resolved = m_unifier.Resolve(recordType);
        if (resolved is not RecordType rt)
            return m_unifier.FreshVar();

        RecordFieldType? field = rt.Fields.FirstOrDefault(f => f.FieldName.Value == fa.FieldName.Value);
        if (field is not null)
            return field.Type;

        m_diagnostics.Error("CDX2005",
            $"Record type '{rt.TypeName.Value}' has no field '{fa.FieldName.Value}'",
            fa.Span);
        return ErrorType.s_instance;
    }

    CodexType InferDoExpr(DoExpr doExpr)
    {
        TypeEnvironment savedEnv = m_env;
        Set<string> collectedEffects = Set<string>.s_empty;
        CodexType lastType = NothingType.s_instance;

        foreach (DoStatement stmt in doExpr.Statements)
        {
            switch (stmt)
            {
                case DoBindStatement bind:
                {
                    CodexType valueType = InferExpr(bind.Value);
                    CodexType unwrapped = UnwrapEffectful(valueType, ref collectedEffects);
                    m_env = m_env.Bind(bind.Name, unwrapped);
                    lastType = NothingType.s_instance;
                    break;
                }
                case DoExprStatement exprStmt:
                {
                    CodexType stmtType = InferExpr(exprStmt.Expression);
                    UnwrapEffectful(stmtType, ref collectedEffects);
                    lastType = stmtType;
                    break;
                }
            }
        }

        m_env = savedEnv;

        if (collectedEffects.Count > 0)
        {
            ImmutableArray<EffectType> effects = [.. collectedEffects.Select(
                e => new EffectType(new Name(e)))];
            CodexType returnType = lastType is EffectfulType eft ? eft.Return : lastType;
            return new EffectfulType(effects, returnType);
        }

        return lastType;
    }

    static CodexType UnwrapEffectful(CodexType type, ref Set<string> effects)
    {
        if (type is EffectfulType eft)
        {
            foreach (EffectType effect in eft.Effects)
                effects = effects.Add(effect.EffectName.Value);
            return eft.Return;
        }
        return type;
    }

    ListType InferList(ListExpr list)
    {
        if (list.Elements.Count == 0)
            return new ListType(m_unifier.FreshVar());

        CodexType elementType = InferExpr(list.Elements[0]);
        for (int i = 1; i < list.Elements.Count; i++)
        {
            CodexType itemType = InferExpr(list.Elements[i]);
            m_unifier.Unify(elementType, itemType, list.Elements[i].Span);
        }

        return new ListType(elementType);
    }

    static Set<string> ExtractEffects(CodexType type)
    {
        CodexType current = type;
        while (current is FunctionType ft)
            current = ft.Return;
        while (current is DependentFunctionType dep)
            current = dep.Body;

        if (current is EffectfulType eft)
        {
            Set<string> result = Set<string>.s_empty;
            foreach (EffectType e in eft.Effects)
                result = result.Add(e.EffectName.Value);
            return result;
        }
        return Set<string>.s_empty;
    }

    void CheckEffectAllowed(CodexType type, SourceSpan span)
    {
        if (type is not EffectfulType eft)
            return;

        foreach (EffectType effect in eft.Effects)
        {
            if (!m_currentEffects.Contains(effect.EffectName.Value))
            {
                m_diagnostics.Error("CDX2031",
                    $"Effect '{effect.EffectName.Value}' is not allowed in this context. " +
                    $"Declare it in the function's type signature.",
                    span);
            }
        }
    }

    CodexType ResolveTypeExpr(TypeExpr typeExpr)
    {
        return typeExpr switch
        {
            NamedTypeExpr named => ResolveNamedType(named.Name),
            FunctionTypeExpr func => new FunctionType(
                ResolveTypeExpr(func.Parameter),
                ResolveTypeExpr(func.Return)),
            AppliedTypeExpr app => ResolveAppliedType(app),
            EffectfulTypeExpr eff => ResolveEffectfulType(eff),
            LinearTypeExpr lin => new LinearType(ResolveTypeExpr(lin.Inner)),
            DependentTypeExpr dep => ResolveDependentType(dep),
            IntegerLiteralTypeExpr intLit => new TypeLevelValue(intLit.Value),
            BinaryTypeExpr bin => ResolveTypeLevelBinary(bin),
            ProofConstraintExpr proof => ResolveProofConstraint(proof),
            _ => ErrorType.s_instance
        };
    }

    CodexType ResolveNamedType(Name name)
    {
        CodexType? fromTypeLevelEnv = m_typeLevelEnv[name.Value];
        if (fromTypeLevelEnv is not null)
            return fromTypeLevelEnv;

        CodexType? fromTypeParam = m_typeParamEnv[name.Value];
        if (fromTypeParam is not null)
            return fromTypeParam;

        CodexType? fromPrimitive = name.Value switch
        {
            "Integer" => IntegerType.s_instance,
            "Number" => NumberType.s_instance,
            "Text" => TextType.s_instance,
            "Boolean" => BooleanType.s_instance,
            "Nothing" => NothingType.s_instance,
            "Void" => VoidType.s_instance,
            _ => null
        };
        if (fromPrimitive is not null)
            return fromPrimitive;

        CodexType? userDef = m_typeDefMap[name.Value];
        if (userDef is not null)
            return userDef;

        if (name.IsValueName)
        {
            TypeVariable tv = m_unifier.FreshVar();
            m_typeParamEnv = m_typeParamEnv.Set(name.Value, tv);
            return tv;
        }

        return new ConstructedType(name, []);
    }

    CodexType ResolveAppliedType(AppliedTypeExpr app)
    {
        if (app.Constructor is NamedTypeExpr namedCtor && namedCtor.Name.Value == "List"
            && app.Arguments.Count == 1)
        {
            return new ListType(ResolveTypeExpr(app.Arguments[0]));
        }

        if (app.Constructor is NamedTypeExpr named)
        {
            CodexType? userDef = m_typeDefMap[named.Name.Value];
            if (userDef is not null)
            {
                ImmutableArray<CodexType> args = [.. app.Arguments.Select(ResolveTypeExpr)];
                int expectedArity = userDef switch
                {
                    SumType s => s.TypeParamIds.Length,
                    RecordType r => r.TypeParamIds.Length,
                    _ => -1
                };
                if (expectedArity >= 0 && args.Length != expectedArity)
                {
                    m_diagnostics.Error("CDX2032",
                        $"Type '{named.Name.Value}' expects {expectedArity} type argument(s), " +
                        $"but received {args.Length}",
                        app.Span);
                }
                return InstantiateParametricType(userDef, args);
            }
        }

        Name ctorName = app.Constructor is NamedTypeExpr n
            ? n.Name
            : new("?");

        ImmutableArray<CodexType> fallbackArgs = [.. app.Arguments.Select(ResolveTypeExpr)];
        return new ConstructedType(ctorName, fallbackArgs);
    }

    EffectfulType ResolveEffectfulType(EffectfulTypeExpr eff)
    {
        ImmutableArray<EffectType>.Builder effects = ImmutableArray.CreateBuilder<EffectType>();
        foreach (TypeExpr e in eff.Effects)
        {
            if (e is NamedTypeExpr named)
            {
                effects.Add(new EffectType(named.Name));
            }
            else
            {
                m_diagnostics.Error("CDX2030",
                    "Effect label must be a name", e.Span);
            }
        }
        CodexType returnType = ResolveTypeExpr(eff.Return);
        return new EffectfulType(effects.ToImmutable(), returnType);
    }

    static CodexType InstantiateParametricType(CodexType typeDef, ImmutableArray<CodexType> args)
    {
        ImmutableArray<int> paramIds = typeDef switch
        {
            SumType s => s.TypeParamIds,
            RecordType r => r.TypeParamIds,
            _ => []
        };

        CodexType result = typeDef;
        int count = Math.Min(paramIds.Length, args.Length);
        for (int i = 0; i < count; i++)
        {
            result = SubstituteVar(result, paramIds[i], args[i]);
        }
        return result;
    }

    CodexType Instantiate(CodexType type)
    {
        while (type is ForAllType forAll)
        {
            TypeVariable fresh = m_unifier.FreshVar();
            type = SubstituteVar(forAll.Body, forAll.VariableId, fresh);
        }
        return type;
    }

    static CodexType Generalize(CodexType type)
    {
        HashSet<int> freeVars = [];
        CollectFreeTypeVars(type, freeVars);
        CodexType result = type;
        foreach (int varId in freeVars)
        {
            result = new ForAllType(varId, result);
        }
        return result;
    }

    static void CollectFreeTypeVars(CodexType type, HashSet<int> vars)
    {
        switch (type)
        {
            case TypeVariable tv:
                vars.Add(tv.Id);
                break;
            case FunctionType ft:
                CollectFreeTypeVars(ft.Parameter, vars);
                CollectFreeTypeVars(ft.Return, vars);
                break;
            case ListType lt:
                CollectFreeTypeVars(lt.Element, vars);
                break;
            case ForAllType fa:
                CollectFreeTypeVars(fa.Body, vars);
                vars.Remove(fa.VariableId);
                break;
            case ConstructedType ct:
                foreach (CodexType arg in ct.Arguments)
                    CollectFreeTypeVars(arg, vars);
                break;
            case EffectfulType eft:
                foreach (EffectType e in eft.Effects)
                    CollectFreeTypeVars(e, vars);
                CollectFreeTypeVars(eft.Return, vars);
                break;
            case DependentFunctionType dep:
                CollectFreeTypeVars(dep.ParamType, vars);
                CollectFreeTypeVars(dep.Body, vars);
                break;
        }
    }

    CodexType ResolveDependentType(DependentTypeExpr dep)
    {
        CodexType paramType = ResolveTypeExpr(dep.ParamType);
        Map<string, CodexType> savedTypeLevelEnv = m_typeLevelEnv;
        m_typeLevelEnv = m_typeLevelEnv.Set(dep.ParamName.Value, new TypeLevelVar(dep.ParamName.Value));
        CodexType body = ResolveTypeExpr(dep.Body);
        m_typeLevelEnv = savedTypeLevelEnv;
        return new DependentFunctionType(dep.ParamName.Value, paramType, body);
    }

    CodexType ResolveTypeLevelBinary(BinaryTypeExpr bin)
    {
        CodexType left = ResolveTypeExpr(bin.Left);
        CodexType right = ResolveTypeExpr(bin.Right);
        TypeLevelOp op = bin.Op switch
        {
            BinaryOp.Add => TypeLevelOp.Add,
            BinaryOp.Sub => TypeLevelOp.Sub,
            BinaryOp.Mul => TypeLevelOp.Mul,
            _ => TypeLevelOp.Add
        };
        return NormalizeTypeLevelExpr(new TypeLevelBinary(op, left, right));
    }

    CodexType ResolveProofConstraint(ProofConstraintExpr proof)
    {
        CodexType left = ResolveTypeExpr(proof.Left);
        CodexType right = ResolveTypeExpr(proof.Right);
        return new ProofType(new LessThanClaim(left, right));
    }

    CodexType TryDischargeProofParams(CodexType type, SourceSpan span)
    {
        while (type is FunctionType ft && ft.Parameter is ProofType proof)
        {
            if (TryDischargeProof(proof.Claim))
            {
                type = ft.Return;
            }
            else
            {
                m_diagnostics.Error("CDX2040",
                    $"Cannot discharge proof obligation: {proof.Claim}",
                    span);
                type = ft.Return;
            }
        }
        return type;
    }

    static bool TryDischargeProof(CodexType claim)
    {
        if (claim is LessThanClaim lt)
        {
            CodexType left = NormalizeTypeLevelExpr(lt.Left);
            CodexType right = NormalizeTypeLevelExpr(lt.Right);
            if (left is TypeLevelValue lv && right is TypeLevelValue rv)
                return lv.Value < rv.Value;
        }
        return false;
    }

    static CodexType NormalizeTypeLevelExpr(CodexType type)
    {
        if (type is TypeLevelBinary bin)
        {
            CodexType left = NormalizeTypeLevelExpr(bin.Left);
            CodexType right = NormalizeTypeLevelExpr(bin.Right);

            if (left is TypeLevelValue lv && right is TypeLevelValue rv)
            {
                long result = bin.Op switch
                {
                    TypeLevelOp.Add => lv.Value + rv.Value,
                    TypeLevelOp.Sub => lv.Value - rv.Value,
                    TypeLevelOp.Mul => lv.Value * rv.Value,
                    _ => 0
                };
                return new TypeLevelValue(result);
            }

            return new TypeLevelBinary(bin.Op, left, right);
        }
        return type;
    }

    static CodexType SubstituteVar(CodexType type, int varId, CodexType replacement)
    {
        return type switch
        {
            TypeVariable tv when tv.Id == varId => replacement,
            FunctionType f => new FunctionType(
                SubstituteVar(f.Parameter, varId, replacement),
                SubstituteVar(f.Return, varId, replacement)),
            ListType l => new ListType(SubstituteVar(l.Element, varId, replacement)),
            ConstructedType c => c with
            {
                Arguments = [.. c.Arguments.Select(a => SubstituteVar(a, varId, replacement))
                ]
            },
            ForAllType fa when fa.VariableId != varId =>
                fa with { Body = SubstituteVar(fa.Body, varId, replacement) },
            SumType s => s with
            {
                Constructors = [.. s.Constructors.Select(c => c with
                {
                    Fields = [.. c.Fields.Select(f => SubstituteVar(f, varId, replacement))
                    ]
                })]
            },
            RecordType r => r with
            {
                Fields = [.. r.Fields.Select(f => f with
                {
                    Type = SubstituteVar(f.Type, varId, replacement)
                })]
            },
            EffectfulType eft => eft with
            {
                Return = SubstituteVar(eft.Return, varId, replacement)
            },
            LinearType lin => new LinearType(SubstituteVar(lin.Inner, varId, replacement)),
            DependentFunctionType dep => new DependentFunctionType(
                dep.ParamName,
                SubstituteVar(dep.ParamType, varId, replacement),
                SubstituteVar(dep.Body, varId, replacement)),
            TypeLevelBinary bin => NormalizeTypeLevelExpr(new TypeLevelBinary(
                bin.Op,
                SubstituteVar(bin.Left, varId, replacement),
                SubstituteVar(bin.Right, varId, replacement))),
            ProofType proof => new ProofType(SubstituteVar(proof.Claim, varId, replacement)),
            LessThanClaim lt => new LessThanClaim(
                SubstituteVar(lt.Left, varId, replacement),
                SubstituteVar(lt.Right, varId, replacement)),
            _ => type
        };
    }

    CodexType? TryExtractTypeLevelValue(Expr expr)
    {
        if (expr is LiteralExpr lit && lit.Kind == LiteralKind.Integer)
            return new TypeLevelValue(Convert.ToInt64(lit.Value));
        if (expr is ListExpr list)
            return new TypeLevelValue(list.Elements.Count);
        return null;
    }

    static CodexType SubstituteTypeLevelVar(CodexType type, string varName, CodexType replacement)
    {
        return type switch
        {
            TypeLevelVar tv when tv.Name == varName => replacement,
            FunctionType f => new FunctionType(
                SubstituteTypeLevelVar(f.Parameter, varName, replacement),
                SubstituteTypeLevelVar(f.Return, varName, replacement)),
            DependentFunctionType dep when dep.ParamName != varName =>
                new DependentFunctionType(
                    dep.ParamName,
                    SubstituteTypeLevelVar(dep.ParamType, varName, replacement),
                    SubstituteTypeLevelVar(dep.Body, varName, replacement)),
            ConstructedType c => c with
            {
                Arguments = [.. c.Arguments.Select(
                    a => SubstituteTypeLevelVar(a, varName, replacement))
                ]
            },
            ListType l => new ListType(SubstituteTypeLevelVar(l.Element, varName, replacement)),
            TypeLevelBinary bin => NormalizeTypeLevelExpr(new TypeLevelBinary(
                bin.Op,
                SubstituteTypeLevelVar(bin.Left, varName, replacement),
                SubstituteTypeLevelVar(bin.Right, varName, replacement))),
            EffectfulType eft => eft with
            {
                Return = SubstituteTypeLevelVar(eft.Return, varName, replacement)
            },
            ProofType proof => new ProofType(
                SubstituteTypeLevelVar(proof.Claim, varName, replacement)),
            LessThanClaim lt => new LessThanClaim(
                SubstituteTypeLevelVar(lt.Left, varName, replacement),
                SubstituteTypeLevelVar(lt.Right, varName, replacement)),
            _ => type
        };
    }
}

public sealed record CtorInfo(CodexType ConstructorType, CodexType OwnerType);
