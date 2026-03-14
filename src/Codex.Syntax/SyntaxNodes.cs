using Codex.Core;

namespace Codex.Syntax;

public enum SyntaxKind
{
    // Top-level
    Document,
    Definition,
    TypeDefinition,

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
    LinearType,
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

public abstract record SyntaxNode(SyntaxKind Kind, SourceSpan Span)
{
    public abstract IEnumerable<SyntaxNode> Children { get; }
}

public sealed record TokenNode(Token Token)
    : SyntaxNode(SyntaxKind.LiteralExpression, Token.Span)
{
    public override IEnumerable<SyntaxNode> Children => [];
}

public sealed record DocumentNode(
    IReadOnlyList<DefinitionNode> Definitions,
    IReadOnlyList<TypeDefinitionNode> TypeDefinitions,
    IReadOnlyList<ChapterNode> Chapters,
    SourceSpan Span)
    : SyntaxNode(SyntaxKind.Document, Span)
{
    public DocumentNode(IReadOnlyList<DefinitionNode> Definitions, SourceSpan Span)
        : this(Definitions, Array.Empty<TypeDefinitionNode>(), Array.Empty<ChapterNode>(), Span) { }

    public DocumentNode(IReadOnlyList<DefinitionNode> Definitions, IReadOnlyList<ChapterNode> Chapters, SourceSpan Span)
        : this(Definitions, Array.Empty<TypeDefinitionNode>(), Chapters, Span) { }

    public override IEnumerable<SyntaxNode> Children
    {
        get
        {
            foreach (ChapterNode ch in Chapters) yield return ch;
            foreach (TypeDefinitionNode td in TypeDefinitions) yield return td;
            foreach (DefinitionNode def in Definitions) yield return def;
        }
    }
}

public sealed record ChapterNode(
    string Title,
    IReadOnlyList<DocumentMember> Members,
    SourceSpan Span)
    : SyntaxNode(SyntaxKind.Chapter, Span)
{
    public override IEnumerable<SyntaxNode> Children => Members;
}

public abstract record DocumentMember(SyntaxKind Kind, SourceSpan Span) : SyntaxNode(Kind, Span);

public sealed record ProseBlockNode(string Text, SourceSpan Span)
    : DocumentMember(SyntaxKind.ProseBlock, Span)
{
    public override IEnumerable<SyntaxNode> Children => [];
}

public sealed record SectionNode(
    string Title,
    IReadOnlyList<DocumentMember> Members,
    SourceSpan Span)
    : DocumentMember(SyntaxKind.Section, Span)
{
    public override IEnumerable<SyntaxNode> Children => Members;
}

public sealed record NotationBlockNode(
    IReadOnlyList<DefinitionNode> Definitions,
    IReadOnlyList<TypeDefinitionNode> TypeDefinitions,
    SourceSpan Span)
    : DocumentMember(SyntaxKind.Definition, Span)
{
    public NotationBlockNode(IReadOnlyList<DefinitionNode> Definitions, SourceSpan Span)
        : this(Definitions, Array.Empty<TypeDefinitionNode>(), Span) { }

    public override IEnumerable<SyntaxNode> Children => Definitions;
}

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
            foreach (LetBinding b in Bindings) yield return b.Value;
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
            foreach (MatchBranchNode b in Branches) yield return b;
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

public sealed record DoExpressionNode(IReadOnlyList<DoStatementNode> Statements, SourceSpan Span)
    : ExpressionNode(SyntaxKind.DoExpression, Span)
{
    public override IEnumerable<SyntaxNode> Children => Statements;
}

public abstract record DoStatementNode(SyntaxKind Kind, SourceSpan Span) : SyntaxNode(Kind, Span);

public sealed record DoBindStatementNode(Token Name, ExpressionNode Value, SourceSpan Span)
    : DoStatementNode(SyntaxKind.DoBindStatement, Span)
{
    public override IEnumerable<SyntaxNode> Children => [Value];
}

public sealed record DoExprStatementNode(ExpressionNode Expression, SourceSpan Span)
    : DoStatementNode(SyntaxKind.DoExprStatement, Span)
{
    public override IEnumerable<SyntaxNode> Children => [Expression];
}

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
            foreach (TypeNode a in Arguments) yield return a;
        }
    }
}

public sealed record ParenthesizedTypeNode(TypeNode Inner, SourceSpan Span)
    : TypeNode(SyntaxKind.ParenthesizedType, Span)
{
    public override IEnumerable<SyntaxNode> Children => [Inner];
}

public sealed record EffectfulTypeNode(IReadOnlyList<TypeNode> Effects, TypeNode Return, SourceSpan Span)
    : TypeNode(SyntaxKind.EffectfulType, Span)
{
    public override IEnumerable<SyntaxNode> Children
    {
        get
        {
            foreach (TypeNode e in Effects) yield return e;
            yield return Return;
        }
    }
}

public sealed record LinearTypeNode(TypeNode Inner, SourceSpan Span)
    : TypeNode(SyntaxKind.LinearType, Span)
{
    public override IEnumerable<SyntaxNode> Children => [Inner];
}

public sealed record TypeAnnotationNode(Token Name, TypeNode Type, SourceSpan Span)
    : SyntaxNode(SyntaxKind.TypeAnnotation, Span)
{
    public override IEnumerable<SyntaxNode> Children => [Type];
}

public sealed record TypeDefinitionNode(
    Token Name,
    IReadOnlyList<Token> TypeParameters,
    TypeDefinitionBody Body,
    SourceSpan Span)
    : SyntaxNode(SyntaxKind.TypeDefinition, Span)
{
    public override IEnumerable<SyntaxNode> Children => [Body];
}

public abstract record TypeDefinitionBody(SyntaxKind Kind, SourceSpan Span) : SyntaxNode(Kind, Span);

public sealed record RecordTypeBody(IReadOnlyList<RecordTypeFieldNode> Fields, SourceSpan Span)
    : TypeDefinitionBody(SyntaxKind.RecordType, Span)
{
    public override IEnumerable<SyntaxNode> Children => Fields;
}

public sealed record RecordTypeFieldNode(Token Name, TypeNode Type, SourceSpan Span)
    : SyntaxNode(SyntaxKind.RecordTypeField, Span)
{
    public override IEnumerable<SyntaxNode> Children => [Type];
}

public sealed record VariantTypeBody(IReadOnlyList<VariantConstructorNode> Constructors, SourceSpan Span)
    : TypeDefinitionBody(SyntaxKind.VariantType, Span)
{
    public override IEnumerable<SyntaxNode> Children => Constructors;
}

public sealed record VariantConstructorNode(
    Token Name,
    IReadOnlyList<VariantFieldNode> Fields,
    SourceSpan Span)
    : SyntaxNode(SyntaxKind.VariantConstructor, Span)
{
    public override IEnumerable<SyntaxNode> Children => Fields;
}

public sealed record VariantFieldNode(Token? FieldName, TypeNode Type, SourceSpan Span)
    : SyntaxNode(SyntaxKind.RecordTypeField, Span)
{
    public override IEnumerable<SyntaxNode> Children => [Type];
}
