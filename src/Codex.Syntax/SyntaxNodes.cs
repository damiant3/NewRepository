using Codex.Core;

namespace Codex.Syntax;

/// <summary>
/// Concrete Syntax Tree node kinds. The CST preserves the full structure of the source.
/// </summary>
public enum SyntaxKind
{
    // Top-level
    Document,
    Definition,

    // Prose structure
    Chapter,
    Section,
    ProseBlock,

    // Expressions
    LiteralExpression,
    NameExpression,
    ApplicationExpression,
    BinaryExpression,
    UnaryExpression,
    IfExpression,
    LetExpression,
    LambdaExpression,
    MatchExpression,
    MatchBranch,
    ListExpression,
    RecordExpression,
    RecordField,
    FieldAccessExpression,
    ParenthesizedExpression,
    DoExpression,
    DoBindStatement,
    DoExprStatement,

    // Patterns
    VariablePattern,
    LiteralPattern,
    ConstructorPattern,
    WildcardPattern,

    // Types
    NamedType,
    FunctionType,
    EffectfulType,
    ApplicationType,
    ParenthesizedType,
    RecordType,
    RecordTypeField,
    VariantType,
    VariantConstructor,

    // Type signature
    TypeAnnotation,

    // Parameters
    Parameter,

    // Error recovery
    ErrorNode,
}

/// <summary>
/// A node in the Concrete Syntax Tree.
/// </summary>
public abstract record SyntaxNode(SyntaxKind Kind, SourceSpan Span)
{
    public abstract IEnumerable<SyntaxNode> Children { get; }
}

/// <summary>
/// A terminal node — wraps a single token.
/// </summary>
public sealed record TokenNode(Token Token)
    : SyntaxNode(SyntaxKind.LiteralExpression, Token.Span)
{
    public override IEnumerable<SyntaxNode> Children => [];
}

/// <summary>
/// The entire document — the root of the CST.
/// May contain flat definitions (notation-only) or chapters (prose mode).
/// </summary>
public sealed record DocumentNode(
    IReadOnlyList<DefinitionNode> Definitions,
    IReadOnlyList<ChapterNode> Chapters,
    SourceSpan Span)
    : SyntaxNode(SyntaxKind.Document, Span)
{
    /// <summary>
    /// Convenience constructor for notation-only documents (no chapters).
    /// </summary>
    public DocumentNode(IReadOnlyList<DefinitionNode> Definitions, SourceSpan Span)
        : this(Definitions, Array.Empty<ChapterNode>(), Span) { }

    public override IEnumerable<SyntaxNode> Children
    {
        get
        {
            foreach (ChapterNode ch in Chapters) yield return ch;
            foreach (DefinitionNode def in Definitions) yield return def;
        }
    }
}

/// <summary>
/// A chapter in a prose-mode document. Contains a title, prose blocks, and notation blocks.
/// </summary>
public sealed record ChapterNode(
    string Title,
    IReadOnlyList<DocumentMember> Members,
    SourceSpan Span)
    : SyntaxNode(SyntaxKind.Chapter, Span)
{
    public override IEnumerable<SyntaxNode> Children => Members;
}

/// <summary>
/// A member of a chapter or section: either a prose block, a notation block (definitions), or a section.
/// </summary>
public abstract record DocumentMember(SyntaxKind Kind, SourceSpan Span) : SyntaxNode(Kind, Span);

/// <summary>
/// A block of prose text (natural language).
/// </summary>
public sealed record ProseBlockNode(string Text, SourceSpan Span)
    : DocumentMember(SyntaxKind.ProseBlock, Span)
{
    public override IEnumerable<SyntaxNode> Children => [];
}

/// <summary>
/// A section within a chapter. Contains prose and notation blocks.
/// </summary>
public sealed record SectionNode(
    string Title,
    IReadOnlyList<DocumentMember> Members,
    SourceSpan Span)
    : DocumentMember(SyntaxKind.Section, Span)
{
    public override IEnumerable<SyntaxNode> Children => Members;
}

/// <summary>
/// A notation block within a chapter or section — wraps one or more definitions.
/// </summary>
public sealed record NotationBlockNode(
    IReadOnlyList<DefinitionNode> Definitions,
    SourceSpan Span)
    : DocumentMember(SyntaxKind.Definition, Span)
{
    public override IEnumerable<SyntaxNode> Children => Definitions;
}

/// <summary>
/// A top-level definition: optional type annotation + name + params + body.
/// </summary>
public sealed record DefinitionNode(
    Token Name,
    IReadOnlyList<Token> Parameters,
    TypeAnnotationNode? TypeAnnotation,
    ExpressionNode Body,
    SourceSpan Span)
    : SyntaxNode(SyntaxKind.Definition, Span)
{
    public override IEnumerable<SyntaxNode> Children
    {
        get
        {
            if (TypeAnnotation is not null) yield return TypeAnnotation;
            yield return Body;
        }
    }
}

// --- Expression nodes ---

public abstract record ExpressionNode(SyntaxKind Kind, SourceSpan Span) : SyntaxNode(Kind, Span);

public sealed record LiteralExpressionNode(Token Literal)
    : ExpressionNode(SyntaxKind.LiteralExpression, Literal.Span)
{
    public override IEnumerable<SyntaxNode> Children => [];
}

public sealed record NameExpressionNode(Token Name)
    : ExpressionNode(SyntaxKind.NameExpression, Name.Span)
{
    public override IEnumerable<SyntaxNode> Children => [];
}

public sealed record ApplicationExpressionNode(ExpressionNode Function, ExpressionNode Argument, SourceSpan Span)
    : ExpressionNode(SyntaxKind.ApplicationExpression, Span)
{
    public override IEnumerable<SyntaxNode> Children => [Function, Argument];
}

public sealed record BinaryExpressionNode(ExpressionNode Left, Token Operator, ExpressionNode Right, SourceSpan Span)
    : ExpressionNode(SyntaxKind.BinaryExpression, Span)
{
    public override IEnumerable<SyntaxNode> Children => [Left, Right];
}

public sealed record UnaryExpressionNode(Token Operator, ExpressionNode Operand, SourceSpan Span)
    : ExpressionNode(SyntaxKind.UnaryExpression, Span)
{
    public override IEnumerable<SyntaxNode> Children => [Operand];
}

public sealed record IfExpressionNode(ExpressionNode Condition, ExpressionNode Then, ExpressionNode Else, SourceSpan Span)
    : ExpressionNode(SyntaxKind.IfExpression, Span)
{
    public override IEnumerable<SyntaxNode> Children => [Condition, Then, Else];
}

public sealed record LetExpressionNode(IReadOnlyList<LetBinding> Bindings, ExpressionNode Body, SourceSpan Span)
    : ExpressionNode(SyntaxKind.LetExpression, Span)
{
    public override IEnumerable<SyntaxNode> Children
    {
        get
        {
            foreach (var b in Bindings) yield return b.Value;
            yield return Body;
        }
    }
}

public sealed record LetBinding(Token Name, ExpressionNode Value);

public sealed record LambdaExpressionNode(IReadOnlyList<Token> Parameters, ExpressionNode Body, SourceSpan Span)
    : ExpressionNode(SyntaxKind.LambdaExpression, Span)
{
    public override IEnumerable<SyntaxNode> Children => [Body];
}

public sealed record MatchExpressionNode(ExpressionNode Scrutinee, IReadOnlyList<MatchBranchNode> Branches, SourceSpan Span)
    : ExpressionNode(SyntaxKind.MatchExpression, Span)
{
    public override IEnumerable<SyntaxNode> Children
    {
        get
        {
            yield return Scrutinee;
            foreach (var b in Branches) yield return b;
        }
    }
}

public sealed record MatchBranchNode(PatternNode Pattern, ExpressionNode Body, SourceSpan Span)
    : SyntaxNode(SyntaxKind.MatchBranch, Span)
{
    public override IEnumerable<SyntaxNode> Children => [Pattern, Body];
}

public sealed record ListExpressionNode(IReadOnlyList<ExpressionNode> Elements, SourceSpan Span)
    : ExpressionNode(SyntaxKind.ListExpression, Span)
{
    public override IEnumerable<SyntaxNode> Children => Elements;
}

public sealed record RecordExpressionNode(Token? TypeName, IReadOnlyList<RecordFieldNode> Fields, SourceSpan Span)
    : ExpressionNode(SyntaxKind.RecordExpression, Span)
{
    public override IEnumerable<SyntaxNode> Children => Fields;
}

public sealed record RecordFieldNode(Token Name, ExpressionNode Value, SourceSpan Span)
    : SyntaxNode(SyntaxKind.RecordField, Span)
{
    public override IEnumerable<SyntaxNode> Children => [Value];
}

public sealed record FieldAccessExpressionNode(ExpressionNode Record, Token FieldName, SourceSpan Span)
    : ExpressionNode(SyntaxKind.FieldAccessExpression, Span)
{
    public override IEnumerable<SyntaxNode> Children => [Record];
}

public sealed record ParenthesizedExpressionNode(ExpressionNode Inner, SourceSpan Span)
    : ExpressionNode(SyntaxKind.ParenthesizedExpression, Span)
{
    public override IEnumerable<SyntaxNode> Children => [Inner];
}

public sealed record ErrorExpressionNode(Token ErrorToken)
    : ExpressionNode(SyntaxKind.ErrorNode, ErrorToken.Span)
{
    public override IEnumerable<SyntaxNode> Children => [];
}

// --- Pattern nodes ---

public abstract record PatternNode(SyntaxKind Kind, SourceSpan Span) : SyntaxNode(Kind, Span);

public sealed record VariablePatternNode(Token Name)
    : PatternNode(SyntaxKind.VariablePattern, Name.Span)
{
    public override IEnumerable<SyntaxNode> Children => [];
}

public sealed record LiteralPatternNode(Token Literal)
    : PatternNode(SyntaxKind.LiteralPattern, Literal.Span)
{
    public override IEnumerable<SyntaxNode> Children => [];
}

public sealed record ConstructorPatternNode(Token Constructor, IReadOnlyList<PatternNode> SubPatterns, SourceSpan Span)
    : PatternNode(SyntaxKind.ConstructorPattern, Span)
{
    public override IEnumerable<SyntaxNode> Children => SubPatterns;
}

public sealed record WildcardPatternNode(Token Underscore)
    : PatternNode(SyntaxKind.WildcardPattern, Underscore.Span)
{
    public override IEnumerable<SyntaxNode> Children => [];
}

// --- Type nodes ---

public abstract record TypeNode(SyntaxKind Kind, SourceSpan Span) : SyntaxNode(Kind, Span);

public sealed record NamedTypeNode(Token Name)
    : TypeNode(SyntaxKind.NamedType, Name.Span)
{
    public override IEnumerable<SyntaxNode> Children => [];
}

public sealed record FunctionTypeNode(TypeNode Parameter, TypeNode Return, SourceSpan Span)
    : TypeNode(SyntaxKind.FunctionType, Span)
{
    public override IEnumerable<SyntaxNode> Children => [Parameter, Return];
}

public sealed record ApplicationTypeNode(TypeNode Constructor, IReadOnlyList<TypeNode> Arguments, SourceSpan Span)
    : TypeNode(SyntaxKind.ApplicationType, Span)
{
    public override IEnumerable<SyntaxNode> Children
    {
        get
        {
            yield return Constructor;
            foreach (var a in Arguments) yield return a;
        }
    }
}

public sealed record ParenthesizedTypeNode(TypeNode Inner, SourceSpan Span)
    : TypeNode(SyntaxKind.ParenthesizedType, Span)
{
    public override IEnumerable<SyntaxNode> Children => [Inner];
}

public sealed record TypeAnnotationNode(Token Name, TypeNode Type, SourceSpan Span)
    : SyntaxNode(SyntaxKind.TypeAnnotation, Span)
{
    public override IEnumerable<SyntaxNode> Children => [Type];
}
