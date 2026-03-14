using Codex.Core;

namespace Codex.Ast;

public sealed record Module(
    QualifiedName Name,
    IReadOnlyList<Definition> Definitions,
    IReadOnlyList<TypeDef> TypeDefinitions,
    SourceSpan Span);

public sealed record Definition(
    Name Name,
    IReadOnlyList<Parameter> Parameters,
    TypeExpr? DeclaredType,
    Expr Body,
    SourceSpan Span);

public sealed record Parameter(Name Name, TypeExpr? TypeAnnotation, SourceSpan Span);

public abstract record Expr(SourceSpan Span);

public sealed record LiteralExpr(object Value, LiteralKind Kind, SourceSpan Span) : Expr(Span);

public enum LiteralKind
{
    Integer,
    Number,
    Text,
    Boolean
}

public sealed record NameExpr(Name Name, SourceSpan Span) : Expr(Span);

public sealed record ApplyExpr(Expr Function, Expr Argument, SourceSpan Span) : Expr(Span);

public sealed record BinaryExpr(Expr Left, BinaryOp Op, Expr Right, SourceSpan Span) : Expr(Span);

public enum BinaryOp
{
    Add, Sub, Mul, Div, Pow,
    Eq, NotEq, Lt, Gt, LtEq, GtEq, DefEq,
    Append, Cons,
    And, Or
}

public sealed record UnaryExpr(UnaryOp Op, Expr Operand, SourceSpan Span) : Expr(Span);

public enum UnaryOp { Negate }

public sealed record IfExpr(Expr Condition, Expr Then, Expr Else, SourceSpan Span) : Expr(Span);

public sealed record LetExpr(IReadOnlyList<LetBinding> Bindings, Expr Body, SourceSpan Span) : Expr(Span);

public sealed record LetBinding(Name Name, Expr Value);

public sealed record LambdaExpr(IReadOnlyList<Parameter> Parameters, Expr Body, SourceSpan Span) : Expr(Span);

public sealed record MatchExpr(Expr Scrutinee, IReadOnlyList<MatchBranch> Branches, SourceSpan Span) : Expr(Span);

public sealed record MatchBranch(Pattern Pattern, Expr Body, SourceSpan Span);

public sealed record ListExpr(IReadOnlyList<Expr> Elements, SourceSpan Span) : Expr(Span);

public sealed record RecordExpr(Name? TypeName, IReadOnlyList<RecordFieldExpr> Fields, SourceSpan Span) : Expr(Span);

public sealed record RecordFieldExpr(Name FieldName, Expr Value, SourceSpan Span);

public sealed record FieldAccessExpr(Expr Record, Name FieldName, SourceSpan Span) : Expr(Span);

public sealed record ErrorExpr(string Message, SourceSpan Span) : Expr(Span);

public abstract record Pattern(SourceSpan Span);

public sealed record VarPattern(Name Name, SourceSpan Span) : Pattern(Span);

public sealed record LiteralPattern(object Value, LiteralKind Kind, SourceSpan Span) : Pattern(Span);

public sealed record CtorPattern(Name Constructor, IReadOnlyList<Pattern> SubPatterns, SourceSpan Span) : Pattern(Span);

public sealed record WildcardPattern(SourceSpan Span) : Pattern(Span);

public abstract record TypeExpr(SourceSpan Span);

public sealed record NamedTypeExpr(Name Name, SourceSpan Span) : TypeExpr(Span);

public sealed record FunctionTypeExpr(TypeExpr Parameter, TypeExpr Return, SourceSpan Span) : TypeExpr(Span);

public sealed record AppliedTypeExpr(TypeExpr Constructor, IReadOnlyList<TypeExpr> Arguments, SourceSpan Span) : TypeExpr(Span);

public abstract record TypeDef(Name Name, IReadOnlyList<Name> TypeParameters, SourceSpan Span);

public sealed record RecordTypeDef(
    Name Name,
    IReadOnlyList<Name> TypeParameters,
    IReadOnlyList<RecordFieldDef> Fields,
    SourceSpan Span)
    : TypeDef(Name, TypeParameters, Span);

public sealed record RecordFieldDef(Name FieldName, TypeExpr Type, SourceSpan Span);

public sealed record VariantTypeDef(
    Name Name,
    IReadOnlyList<Name> TypeParameters,
    IReadOnlyList<VariantCtorDef> Constructors,
    SourceSpan Span)
    : TypeDef(Name, TypeParameters, Span);

public sealed record VariantCtorDef(
    Name Name,
    IReadOnlyList<VariantFieldDef> Fields,
    SourceSpan Span);

public sealed record VariantFieldDef(Name? FieldName, TypeExpr Type, SourceSpan Span);
