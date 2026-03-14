using System.Collections.Immutable;
using Codex.Core;
using Codex.Ast;
using Codex.Types;

namespace Codex.IR;

public sealed class Lowering
{
    private readonly ImmutableDictionary<string, CodexType> m_typeMap;
    private readonly DiagnosticBag m_diagnostics;
    private ImmutableDictionary<string, CodexType> m_localEnv;

    public Lowering(ImmutableDictionary<string, CodexType> typeMap, DiagnosticBag diagnostics)
    {
        m_typeMap = typeMap;
        m_diagnostics = diagnostics;
        m_localEnv = ImmutableDictionary<string, CodexType>.Empty;
    }

    public IRModule Lower(Module module)
    {
        ImmutableArray<IRDefinition>.Builder defs = ImmutableArray.CreateBuilder<IRDefinition>();
        foreach (Definition def in module.Definitions)
        {
            defs.Add(LowerDefinition(def));
        }
        return new IRModule(module.Name, defs.ToImmutable());
    }

    private IRDefinition LowerDefinition(Definition def)
    {
        CodexType fullType = m_typeMap.TryGetValue(def.Name.Value, out CodexType? t)
            ? t
            : ErrorType.s_instance;

        ImmutableDictionary<string, CodexType> savedEnv = m_localEnv;

        ImmutableArray<IRParameter>.Builder parameters = ImmutableArray.CreateBuilder<IRParameter>();
        CodexType currentType = fullType;
        foreach (Parameter param in def.Parameters)
        {
            CodexType paramType;
            if (currentType is FunctionType ft)
            {
                paramType = ft.Parameter;
                currentType = ft.Return;
            }
            else
            {
                paramType = ErrorType.s_instance;
            }
            parameters.Add(new IRParameter(param.Name.Value, paramType));
            m_localEnv = m_localEnv.SetItem(param.Name.Value, paramType);
        }

        IRExpr body = LowerExpr(def.Body, currentType);
        m_localEnv = savedEnv;
        return new IRDefinition(def.Name.Value, parameters.ToImmutable(), fullType, body);
    }

    private IRExpr LowerExpr(Expr expr, CodexType expectedType)
    {
        switch (expr)
        {
            case LiteralExpr lit:
                return LowerLiteral(lit);

            case NameExpr name:
                CodexType nameType = LookupName(name.Name.Value, expectedType);
                return new IRName(name.Name.Value, nameType);

            case BinaryExpr bin:
                return LowerBinary(bin, expectedType);

            case UnaryExpr un:
                return new IRNegate(LowerExpr(un.Operand, IntegerType.s_instance));

            case IfExpr iff:
                return new IRIf(
                    LowerExpr(iff.Condition, BooleanType.s_instance),
                    LowerExpr(iff.Then, expectedType),
                    LowerExpr(iff.Else, expectedType),
                    expectedType);

            case LetExpr let:
                return LowerLet(let, expectedType);

            case ApplyExpr app:
                return LowerApply(app, expectedType);

            case LambdaExpr lam:
                return LowerLambda(lam, expectedType);

            case MatchExpr match:
                return LowerMatch(match, expectedType);

            case ListExpr list:
                return LowerList(list, expectedType);

            case ErrorExpr err:
                return new IRError(err.Message, expectedType);

            default:
                return new IRError($"Unsupported expression: {expr.GetType().Name}", expectedType);
        }
    }

    private static IRExpr LowerLiteral(LiteralExpr lit)
    {
        return lit.Kind switch
        {
            LiteralKind.Integer => new IRIntegerLit(Convert.ToInt64(lit.Value)),
            LiteralKind.Number => new IRNumberLit(Convert.ToDecimal(lit.Value)),
            LiteralKind.Text => new IRTextLit((string)lit.Value),
            LiteralKind.Boolean => new IRBoolLit((bool)lit.Value),
            _ => new IRError("Unknown literal kind", ErrorType.s_instance)
        };
    }

    private IRExpr LowerBinary(BinaryExpr bin, CodexType expectedType)
    {
        IRExpr left = LowerExpr(bin.Left, expectedType);
        IRExpr right = LowerExpr(bin.Right, expectedType);

        IRBinaryOp op = bin.Op switch
        {
            BinaryOp.Add when IsNumeric(left.Type) => IsInteger(left.Type) ? IRBinaryOp.AddInt : IRBinaryOp.AddNum,
            BinaryOp.Sub when IsNumeric(left.Type) => IsInteger(left.Type) ? IRBinaryOp.SubInt : IRBinaryOp.SubNum,
            BinaryOp.Mul when IsNumeric(left.Type) => IsInteger(left.Type) ? IRBinaryOp.MulInt : IRBinaryOp.MulNum,
            BinaryOp.Div when IsNumeric(left.Type) => IsInteger(left.Type) ? IRBinaryOp.DivInt : IRBinaryOp.DivNum,
            BinaryOp.Pow => IRBinaryOp.PowInt,
            BinaryOp.Add => IRBinaryOp.AddInt,
            BinaryOp.Sub => IRBinaryOp.SubInt,
            BinaryOp.Mul => IRBinaryOp.MulInt,
            BinaryOp.Div => IRBinaryOp.DivInt,
            BinaryOp.Eq => IRBinaryOp.Eq,
            BinaryOp.NotEq => IRBinaryOp.NotEq,
            BinaryOp.Lt => IRBinaryOp.Lt,
            BinaryOp.Gt => IRBinaryOp.Gt,
            BinaryOp.LtEq => IRBinaryOp.LtEq,
            BinaryOp.GtEq => IRBinaryOp.GtEq,
            BinaryOp.DefEq => IRBinaryOp.Eq,
            BinaryOp.And => IRBinaryOp.And,
            BinaryOp.Or => IRBinaryOp.Or,
            BinaryOp.Append when IsText(left.Type) || IsText(expectedType) => IRBinaryOp.AppendText,
            BinaryOp.Append => IRBinaryOp.AppendList,
            BinaryOp.Cons => IRBinaryOp.ConsList,
            _ => IRBinaryOp.AddInt
        };

        CodexType resultType = bin.Op switch
        {
            BinaryOp.Eq or BinaryOp.NotEq or BinaryOp.Lt or BinaryOp.Gt
                or BinaryOp.LtEq or BinaryOp.GtEq or BinaryOp.DefEq => BooleanType.s_instance,
            BinaryOp.And or BinaryOp.Or => BooleanType.s_instance,
            BinaryOp.Append when IsText(left.Type) || IsText(expectedType) => TextType.s_instance,
            _ => left.Type
        };

        return new IRBinary(op, left, right, resultType);
    }

    private IRExpr LowerLet(LetExpr let, CodexType expectedType)
    {
        ImmutableDictionary<string, CodexType> savedEnv = m_localEnv;
        IRExpr body = LowerExpr(let.Body, expectedType);

        for (int i = let.Bindings.Count - 1; i >= 0; i--)
        {
            Ast.LetBinding binding = let.Bindings[i];
            CodexType bindingType = LookupName(binding.Name.Value, ErrorType.s_instance);
            IRExpr value = LowerExpr(binding.Value, bindingType);
            body = new IRLet(binding.Name.Value, value.Type, value, body);
            m_localEnv = m_localEnv.SetItem(binding.Name.Value, value.Type);
        }

        m_localEnv = savedEnv;

        // Re-lower with bindings in scope
        m_localEnv = savedEnv;
        foreach (Ast.LetBinding binding in let.Bindings)
        {
            IRExpr value = LowerExpr(binding.Value, ErrorType.s_instance);
            m_localEnv = m_localEnv.SetItem(binding.Name.Value, value.Type);
        }

        body = LowerExpr(let.Body, expectedType);

        // Wrap in nested lets
        for (int i = let.Bindings.Count - 1; i >= 0; i--)
        {
            Ast.LetBinding binding = let.Bindings[i];
            IRExpr value = LowerExpr(binding.Value, ErrorType.s_instance);
            body = new IRLet(binding.Name.Value, value.Type, value, body);
        }

        m_localEnv = savedEnv;
        return body;
    }

    private IRExpr LowerApply(ApplyExpr app, CodexType expectedType)
    {
        IRExpr func = LowerExpr(app.Function, ErrorType.s_instance);
        CodexType argType = func.Type is FunctionType ft ? ft.Parameter : ErrorType.s_instance;
        IRExpr arg = LowerExpr(app.Argument, argType);
        CodexType returnType = func.Type is FunctionType ft2 ? ft2.Return : expectedType;
        return new IRApply(func, arg, returnType);
    }

    private IRExpr LowerLambda(LambdaExpr lam, CodexType expectedType)
    {
        ImmutableDictionary<string, CodexType> savedEnv = m_localEnv;

        ImmutableArray<IRParameter>.Builder parameters = ImmutableArray.CreateBuilder<IRParameter>();
        CodexType currentType = expectedType;
        foreach (Parameter p in lam.Parameters)
        {
            CodexType paramType = currentType is FunctionType ft ? ft.Parameter : ErrorType.s_instance;
            parameters.Add(new IRParameter(p.Name.Value, paramType));
            m_localEnv = m_localEnv.SetItem(p.Name.Value, paramType);
            currentType = currentType is FunctionType ft2 ? ft2.Return : currentType;
        }

        IRExpr body = LowerExpr(lam.Body, currentType);
        m_localEnv = savedEnv;
        return new IRLambda(parameters.ToImmutable(), body, expectedType);
    }

    private IRExpr LowerMatch(MatchExpr match, CodexType expectedType)
    {
        IRExpr scrutinee = LowerExpr(match.Scrutinee, ErrorType.s_instance);
        ImmutableArray<IRMatchBranch>.Builder branches = ImmutableArray.CreateBuilder<IRMatchBranch>();

        foreach (MatchBranch branch in match.Branches)
        {
            ImmutableDictionary<string, CodexType> savedEnv = m_localEnv;
            IRPattern pattern = LowerPattern(branch.Pattern, scrutinee.Type);
            IRExpr body = LowerExpr(branch.Body, expectedType);
            branches.Add(new IRMatchBranch(pattern, body));
            m_localEnv = savedEnv;
        }

        return new IRMatch(scrutinee, branches.ToImmutable(), expectedType);
    }

    private IRPattern LowerPattern(Pattern pattern, CodexType scrutineeType)
    {
        switch (pattern)
        {
            case VarPattern v:
                m_localEnv = m_localEnv.SetItem(v.Name.Value, scrutineeType);
                return new IRVarPattern(v.Name.Value, scrutineeType);
            case LiteralPattern lit:
                return new IRLiteralPattern(lit.Value, scrutineeType);
            case WildcardPattern:
                return new IRWildcardPattern();
            default:
                return new IRWildcardPattern();
        }
    }

    private IRExpr LowerList(ListExpr list, CodexType expectedType)
    {
        CodexType elementType = expectedType is ListType lt
            ? lt.Element
            : (list.Elements.Count > 0 ? InferElementType(list) : ErrorType.s_instance);

        ImmutableArray<IRExpr>.Builder elements = ImmutableArray.CreateBuilder<IRExpr>();
        foreach (Expr elem in list.Elements)
        {
            elements.Add(LowerExpr(elem, elementType));
        }

        return new IRList(elements.ToImmutable(), elementType);
    }

    private CodexType InferElementType(ListExpr list)
    {
        if (list.Elements.Count == 0) return ErrorType.s_instance;
        Expr first = list.Elements[0];
        return first switch
        {
            LiteralExpr lit => lit.Kind switch
            {
                LiteralKind.Integer => IntegerType.s_instance,
                LiteralKind.Number => NumberType.s_instance,
                LiteralKind.Text => TextType.s_instance,
                LiteralKind.Boolean => BooleanType.s_instance,
                _ => ErrorType.s_instance
            },
            _ => ErrorType.s_instance
        };
    }

    private CodexType LookupName(string name, CodexType fallback)
    {
        if (m_localEnv.TryGetValue(name, out CodexType? localType))
            return localType;
        if (m_typeMap.TryGetValue(name, out CodexType? type))
            return type;
        return fallback;
    }

    private static bool IsNumeric(CodexType type) => type is IntegerType or NumberType;
    private static bool IsInteger(CodexType type) => type is IntegerType;
    private static bool IsText(CodexType type) => type is TextType;
}
