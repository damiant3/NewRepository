using System.Collections.Immutable;
using Codex.Core;
using Codex.Types;

namespace Codex.IR;

/// <summary>
/// A compiled module in intermediate representation.
/// All types are resolved. Ready for emission to a target backend.
/// </summary>
public sealed record IRModule(
    QualifiedName Name,
    ImmutableArray<IRDefinition> Definitions);

/// <summary>
/// A top-level definition with fully resolved type.
/// </summary>
public sealed record IRDefinition(
    string Name,
    ImmutableArray<IRParameter> Parameters,
    CodexType Type,
    IRExpr Body);

/// <summary>
/// A typed parameter.
/// </summary>
public sealed record IRParameter(string Name, CodexType Type);

/// <summary>
/// Base type for all IR expressions. Every expression carries its resolved type.
/// </summary>
public abstract record IRExpr(CodexType Type);

/// <summary>
/// An integer literal.
/// </summary>
public sealed record IRIntegerLit(long Value) : IRExpr(IntegerType.s_instance);

/// <summary>
/// A decimal number literal.
/// </summary>
public sealed record IRNumberLit(decimal Value) : IRExpr(NumberType.s_instance);

/// <summary>
/// A text literal.
/// </summary>
public sealed record IRTextLit(string Value) : IRExpr(TextType.s_instance);

/// <summary>
/// A boolean literal.
/// </summary>
public sealed record IRBoolLit(bool Value) : IRExpr(BooleanType.s_instance);

/// <summary>
/// A reference to a named binding (local variable, parameter, or top-level definition).
/// </summary>
public sealed record IRName(string Name, CodexType Type) : IRExpr(Type);

/// <summary>
/// A binary operation with resolved operand types and result type.
/// </summary>
public sealed record IRBinary(IRBinaryOp Op, IRExpr Left, IRExpr Right, CodexType Type) : IRExpr(Type);

/// <summary>
/// The set of binary operations supported in IR.
/// </summary>
public enum IRBinaryOp
{
    AddInt, SubInt, MulInt, DivInt, PowInt,
    AddNum, SubNum, MulNum, DivNum,
    Eq, NotEq, Lt, Gt, LtEq, GtEq,
    And, Or,
    AppendText, AppendList,
    ConsList
}

/// <summary>
/// Unary negation.
/// </summary>
public sealed record IRNegate(IRExpr Operand) : IRExpr(Operand.Type);

/// <summary>
/// A conditional expression.
/// </summary>
public sealed record IRIf(IRExpr Condition, IRExpr Then, IRExpr Else, CodexType Type) : IRExpr(Type);

/// <summary>
/// A let binding: let name = value in body.
/// </summary>
public sealed record IRLet(string Name, CodexType NameType, IRExpr Value, IRExpr Body) : IRExpr(Body.Type);

/// <summary>
/// Function application: function applied to one argument.
/// </summary>
public sealed record IRApply(IRExpr Function, IRExpr Argument, CodexType Type) : IRExpr(Type);

/// <summary>
/// A lambda expression (anonymous function).
/// </summary>
public sealed record IRLambda(ImmutableArray<IRParameter> Parameters, IRExpr Body, CodexType Type) : IRExpr(Type);

/// <summary>
/// A list literal.
/// </summary>
public sealed record IRList(ImmutableArray<IRExpr> Elements, CodexType ElementType)
    : IRExpr(new ListType(ElementType));

/// <summary>
/// A match/when expression with typed branches.
/// </summary>
public sealed record IRMatch(IRExpr Scrutinee, ImmutableArray<IRMatchBranch> Branches, CodexType Type)
    : IRExpr(Type);

/// <summary>
/// A single branch of a match expression.
/// </summary>
public sealed record IRMatchBranch(IRPattern Pattern, IRExpr Body);

/// <summary>
/// Base type for IR patterns.
/// </summary>
public abstract record IRPattern;

/// <summary>
/// Binds the matched value to a name.
/// </summary>
public sealed record IRVarPattern(string Name, CodexType Type) : IRPattern;

/// <summary>
/// Matches a literal value.
/// </summary>
public sealed record IRLiteralPattern(object Value, CodexType Type) : IRPattern;

/// <summary>
/// Matches any value without binding.
/// </summary>
public sealed record IRWildcardPattern : IRPattern;

/// <summary>
/// An error expression — emitted when lowering fails. Backend should emit a runtime throw.
/// </summary>
public sealed record IRError(string Message, CodexType Type) : IRExpr(Type);
