using System.Collections.Immutable;
using Codex.Core;
using Codex.Ast;

namespace Codex.Types;

public sealed class TypeChecker
{
    private readonly DiagnosticBag m_diagnostics;
    private readonly Unifier m_unifier;
    private TypeEnvironment m_env;
    private Map<string, CodexType> m_typeDefMap;
    private Map<string, CtorInfo> m_ctorMap;
    private Map<string, CodexType> m_typeParamEnv;

    public TypeChecker(DiagnosticBag diagnostics)
    {
        m_diagnostics = diagnostics;
        m_unifier = new(diagnostics);
        m_env = TypeEnvironment.WithBuiltins();
        m_typeDefMap = Map<string, CodexType>.s_empty;
        m_ctorMap = Map<string, CtorInfo>.s_empty;
        m_typeParamEnv = Map<string, CodexType>.s_empty;
    }

    public ImmutableDictionary<string, CodexType> CheckModule(Module module)
    {
        RegisterTypeDefinitions(module.TypeDefinitions);

        Dictionary<string, CodexType> topLevelTypes = new();
        foreach (Definition def in module.Definitions)
        {
            CodexType declaredType = def.DeclaredType is not null
                ? ResolveTypeExpr(def.DeclaredType)
                : m_unifier.FreshVar();
            topLevelTypes[def.Name.Value] = declaredType;
            m_env = m_env.Bind(def.Name, declaredType);
        }

        foreach (Definition def in module.Definitions)
        {
            CodexType expectedType = topLevelTypes[def.Name.Value];
            CodexType bodyType = InferDefinition(def, expectedType);
            m_unifier.Unify(expectedType, bodyType, def.Span);
        }

        ImmutableDictionary<string, CodexType>.Builder result =
            ImmutableDictionary.CreateBuilder<string, CodexType>();
        foreach (KeyValuePair<string, CodexType> kv in topLevelTypes)
        {
            result[kv.Key] = m_unifier.DeepResolve(kv.Value);
        }

        return result.ToImmutable();
    }

    private void RegisterTypeDefinitions(IReadOnlyList<TypeDef> typeDefs)
    {
        foreach (TypeDef td in typeDefs)
        {
            switch (td)
            {
                case RecordTypeDef rec:
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
                    break;
                }

                case VariantTypeDef variant:
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
                    break;
                }
            }
        }
    }

    public Map<string, CodexType> TypeDefMap => m_typeDefMap;

    public Map<string, CtorInfo> ConstructorMap => m_ctorMap;

    private CodexType InferDefinition(Definition def, CodexType expectedType)
    {
        TypeEnvironment savedEnv = m_env;

        CodexType currentExpected = expectedType;
        foreach (Parameter param in def.Parameters)
        {
            CodexType paramType;
            if (currentExpected is FunctionType ft)
            {
                paramType = ft.Parameter;
                currentExpected = ft.Return;
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

        CodexType bodyType = InferExpr(def.Body);
        m_unifier.Unify(currentExpected, bodyType, def.Body.Span);

        CodexType fullType = bodyType;
        for (int i = def.Parameters.Count - 1; i >= 0; i--)
        {
            CodexType paramType = m_env.Lookup(def.Parameters[i].Name) ?? ErrorType.s_instance;
            fullType = new FunctionType(paramType, fullType);
        }

        m_env = savedEnv;
        return fullType;
    }

    private CodexType InferExpr(Expr expr)
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

            case ErrorExpr:
                return ErrorType.s_instance;

            default:
                return ErrorType.s_instance;
        }
    }

    private CodexType InferBinary(BinaryExpr bin)
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

    private CodexType InferArithmetic(CodexType left, CodexType right, BinaryExpr bin)
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

    private CodexType InferComparison(CodexType left, CodexType right, BinaryExpr bin)
    {
        m_unifier.Unify(left, right, bin.Span);
        return BooleanType.s_instance;
    }

    private CodexType InferLogical(CodexType left, CodexType right, BinaryExpr bin)
    {
        m_unifier.Unify(left, BooleanType.s_instance, bin.Left.Span);
        m_unifier.Unify(right, BooleanType.s_instance, bin.Right.Span);
        return BooleanType.s_instance;
    }

    private CodexType InferAppend(CodexType left, CodexType right, BinaryExpr bin)
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

    private CodexType InferCons(CodexType left, CodexType right, BinaryExpr bin)
    {
        ListType listType = new(left);
        m_unifier.Unify(right, listType, bin.Span);
        return listType;
    }

    private CodexType InferApplication(ApplyExpr app)
    {
        CodexType funcType = InferExpr(app.Function);
        CodexType argType = InferExpr(app.Argument);
        CodexType returnType = m_unifier.FreshVar();

        if (!m_unifier.Unify(funcType, new FunctionType(argType, returnType), app.Span))
        {
            return ErrorType.s_instance;
        }

        return returnType;
    }

    private CodexType InferLet(LetExpr let)
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

    private CodexType InferLambda(LambdaExpr lam)
    {
        TypeEnvironment savedEnv = m_env;

        List<CodexType> paramTypes = new();
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

    private CodexType InferMatch(MatchExpr match)
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

    private void CheckExhaustiveness(MatchExpr match, CodexType scrutineeType)
    {
        CodexType resolved = m_unifier.Resolve(scrutineeType);
        if (resolved is not SumType sumType)
            return;

        bool hasCatchAll = match.Branches.Any(b =>
            b.Pattern is VarPattern or WildcardPattern);
        if (hasCatchAll)
            return;

        HashSet<string> covered = new();
        foreach (MatchBranch branch in match.Branches)
        {
            if (branch.Pattern is CtorPattern cp)
                covered.Add(cp.Constructor.Value);
        }

        List<string> missing = new();
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

    private void CheckPattern(Pattern pattern, CodexType expectedType)
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
                    List<CodexType> fieldTypes = new();
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

    private CodexType InferRecord(RecordExpr rec)
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

    private CodexType InferFieldAccess(FieldAccessExpr fa)
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

    private CodexType InferList(ListExpr list)
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

    private CodexType ResolveTypeExpr(TypeExpr typeExpr)
    {
        return typeExpr switch
        {
            NamedTypeExpr named => ResolveNamedType(named.Name),
            FunctionTypeExpr func => new FunctionType(
                ResolveTypeExpr(func.Parameter),
                ResolveTypeExpr(func.Return)),
            AppliedTypeExpr app => ResolveAppliedType(app),
            _ => ErrorType.s_instance
        };
    }

    private CodexType ResolveNamedType(Name name)
    {
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

        return new ConstructedType(name, []);
    }

    private CodexType ResolveAppliedType(AppliedTypeExpr app)
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
                return InstantiateParametricType(userDef, args);
            }
        }

        Name ctorName = app.Constructor is NamedTypeExpr n
            ? n.Name
            : new("?");

        ImmutableArray<CodexType> fallbackArgs = [.. app.Arguments.Select(ResolveTypeExpr)];
        return new ConstructedType(ctorName, fallbackArgs);
    }

    private static CodexType InstantiateParametricType(CodexType typeDef, ImmutableArray<CodexType> args)
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

    private CodexType Instantiate(CodexType type)
    {
        while (type is ForAllType forAll)
        {
            TypeVariable fresh = m_unifier.FreshVar();
            type = SubstituteVar(forAll.Body, forAll.VariableId, fresh);
        }
        return type;
    }

    private static CodexType SubstituteVar(CodexType type, int varId, CodexType replacement)
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
                Arguments = [.. c.Arguments.Select(a => SubstituteVar(a, varId, replacement))]
            },
            ForAllType fa when fa.VariableId != varId =>
                fa with { Body = SubstituteVar(fa.Body, varId, replacement) },
            SumType s => s with
            {
                Constructors = [.. s.Constructors.Select(c => c with
                {
                    Fields = [.. c.Fields.Select(f => SubstituteVar(f, varId, replacement))]
                })]
            },
            RecordType r => r with
            {
                Fields = [.. r.Fields.Select(f => f with
                {
                    Type = SubstituteVar(f.Type, varId, replacement)
                })]
            },
            _ => type
        };
    }
}

public sealed record CtorInfo(CodexType ConstructorType, CodexType OwnerType);
