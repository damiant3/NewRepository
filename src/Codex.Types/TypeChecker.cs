using System.Collections.Immutable;
using Codex.Core;
using Codex.Ast;

namespace Codex.Types;

// Bidirectional type checker for Codex.
// Infers types for expressions and checks them against declared annotations.
public sealed class TypeChecker
{
    private readonly DiagnosticBag m_diagnostics;
    private readonly Unifier m_unifier;
    private TypeEnvironment m_env;

    public TypeChecker(DiagnosticBag diagnostics)
    {
        m_diagnostics = diagnostics;
        m_unifier = new Unifier(diagnostics);
        m_env = TypeEnvironment.WithBuiltins();
    }

    public ImmutableDictionary<string, CodexType> CheckModule(Module module)
    {
        // First pass: register all top-level names with fresh type variables
        // so definitions can reference each other (mutual recursion).
        Dictionary<string, CodexType> topLevelTypes = new();
        foreach (Definition def in module.Definitions)
        {
            CodexType declaredType = def.DeclaredType is not null
                ? ResolveTypeExpr(def.DeclaredType)
                : m_unifier.FreshVar();
            topLevelTypes[def.Name.Value] = declaredType;
            m_env = m_env.Bind(def.Name, declaredType);
        }

        // Second pass: infer/check the body of each definition
        foreach (Definition def in module.Definitions)
        {
            CodexType expectedType = topLevelTypes[def.Name.Value];
            CodexType bodyType = InferDefinition(def, expectedType);

            m_unifier.Unify(expectedType, bodyType, def.Span);
        }

        // Resolve all types to their final form
        ImmutableDictionary<string, CodexType>.Builder result =
            ImmutableDictionary.CreateBuilder<string, CodexType>();
        foreach (KeyValuePair<string, CodexType> kv in topLevelTypes)
        {
            result[kv.Key] = m_unifier.DeepResolve(kv.Value);
        }

        return result.ToImmutable();
    }

    private CodexType InferDefinition(Definition def, CodexType expectedType)
    {
        TypeEnvironment savedEnv = m_env;

        // Peel off function parameters from the expected type
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

        // Reconstruct full function type from params
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

            case RecordExpr:
                // Records need type definitions to check properly — defer for now
                return m_unifier.FreshVar();

            case FieldAccessExpr:
                // Field access needs record type info — defer for now
                return m_unifier.FreshVar();

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
        ListType listType = new ListType(left);
        m_unifier.Unify(right, listType, bin.Span);
        return listType;
    }

    private CodexType InferApplication(ApplyExpr app)
    {
        CodexType funcType = InferExpr(app.Function);
        CodexType argType = InferExpr(app.Argument);

        CodexType returnType = m_unifier.FreshVar();
        FunctionType expected = new FunctionType(argType, returnType);

        if (!m_unifier.Unify(funcType, expected, app.Span))
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

        return resultType;
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
                foreach (Pattern sub in ctor.SubPatterns)
                {
                    CodexType subType = m_unifier.FreshVar();
                    CheckPattern(sub, subType);
                }
                break;

            case WildcardPattern:
                break;
        }
    }

    private CodexType InferList(ListExpr list)
    {
        if (list.Elements.Count == 0)
        {
            return new ListType(m_unifier.FreshVar());
        }

        CodexType elementType = InferExpr(list.Elements[0]);
        for (int i = 1; i < list.Elements.Count; i++)
        {
            CodexType itemType = InferExpr(list.Elements[i]);
            m_unifier.Unify(elementType, itemType, list.Elements[i].Span);
        }

        return new ListType(elementType);
    }

    // Resolve a surface TypeExpr to an internal CodexType
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

    private static CodexType ResolveNamedType(Name name)
    {
        return name.Value switch
        {
            "Integer" => IntegerType.s_instance,
            "Number" => NumberType.s_instance,
            "Text" => TextType.s_instance,
            "Boolean" => BooleanType.s_instance,
            "Nothing" => NothingType.s_instance,
            "Void" => VoidType.s_instance,
            _ => new ConstructedType(name, [])
        };
    }

    private CodexType ResolveAppliedType(AppliedTypeExpr app)
    {
        if (app.Constructor is NamedTypeExpr namedCtor && namedCtor.Name.Value == "List"
            && app.Arguments.Count == 1)
        {
            return new ListType(ResolveTypeExpr(app.Arguments[0]));
        }

        Name ctorName = app.Constructor is NamedTypeExpr n
            ? n.Name
            : new Name("?");

        ImmutableArray<CodexType> args = [.. app.Arguments.Select(ResolveTypeExpr)];
        return new ConstructedType(ctorName, args);
    }

    private CodexType Instantiate(CodexType type)
    {
        if (type is ForAllType forAll)
        {
            TypeVariable fresh = m_unifier.FreshVar();
            return SubstituteVar(forAll.Body, forAll.VariableId, fresh);
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
            _ => type
        };
    }
}
