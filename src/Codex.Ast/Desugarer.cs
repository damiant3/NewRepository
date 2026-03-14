using Codex.Core;
using Codex.Syntax;

namespace Codex.Ast;

public sealed class Desugarer
{
    private readonly DiagnosticBag m_diagnostics;

    public Desugarer(DiagnosticBag diagnostics)
    {
        m_diagnostics = diagnostics;
    }

    public Module Desugar(DocumentNode document, string moduleName)
    {
        QualifiedName name = QualifiedName.Simple(moduleName);
        List<Definition> definitions = document.Definitions.Select(DesugarDefinition).ToList();
        return new Module(name, definitions, document.Span);
    }

    private Definition DesugarDefinition(DefinitionNode node)
    {
        Name name = new Name(node.Name.Text);
        List<Parameter> parameters = node.Parameters.Select(p =>
            new Parameter(new Name(p.Text), null, p.Span)).ToList();
        TypeExpr? declaredType = node.TypeAnnotation is not null
            ? DesugarType(node.TypeAnnotation.Type)
            : null;
        Expr body = DesugarExpr(node.Body);

        return new Definition(name, parameters, declaredType, body, node.Span);
    }

    private Expr DesugarExpr(ExpressionNode node) => node switch
    {
        LiteralExpressionNode lit => new LiteralExpr(
            lit.Literal.LiteralValue ?? lit.Literal.Text,
            lit.Literal.Kind switch
            {
                TokenKind.IntegerLiteral => LiteralKind.Integer,
                TokenKind.NumberLiteral => LiteralKind.Number,
                TokenKind.TextLiteral => LiteralKind.Text,
                TokenKind.TrueKeyword or TokenKind.FalseKeyword => LiteralKind.Boolean,
                _ => LiteralKind.Text
            },
            lit.Span),

        NameExpressionNode name => new NameExpr(new Name(name.Name.Text), name.Span),

        ApplicationExpressionNode app => new ApplyExpr(
            DesugarExpr(app.Function),
            DesugarExpr(app.Argument),
            app.Span),

        BinaryExpressionNode bin => new BinaryExpr(
            DesugarExpr(bin.Left),
            DesugarBinaryOp(bin.Operator.Kind),
            DesugarExpr(bin.Right),
            bin.Span),

        UnaryExpressionNode un => new UnaryExpr(
            UnaryOp.Negate,
            DesugarExpr(un.Operand),
            un.Span),

        IfExpressionNode iff => new IfExpr(
            DesugarExpr(iff.Condition),
            DesugarExpr(iff.Then),
            DesugarExpr(iff.Else),
            iff.Span),

        LetExpressionNode let => new LetExpr(
            let.Bindings.Select(b =>
                new Ast.LetBinding(new Name(b.Name.Text), DesugarExpr(b.Value))).ToList(),
            DesugarExpr(let.Body),
            let.Span),

        LambdaExpressionNode lam => new LambdaExpr(
            lam.Parameters.Select(p =>
                new Parameter(new Name(p.Text), null, p.Span)).ToList(),
            DesugarExpr(lam.Body),
            lam.Span),

        MatchExpressionNode m => new MatchExpr(
            DesugarExpr(m.Scrutinee),
            m.Branches.Select(b => new MatchBranch(
                DesugarPattern(b.Pattern),
                DesugarExpr(b.Body),
                b.Span)).ToList(),
            m.Span),

        ListExpressionNode list => new ListExpr(
            list.Elements.Select(DesugarExpr).ToList(),
            list.Span),

        RecordExpressionNode rec => new RecordExpr(
            rec.TypeName is not null ? new Name(rec.TypeName.Text) : null,
            rec.Fields.Select(f => new RecordFieldExpr(
                new Name(f.Name.Text),
                DesugarExpr(f.Value),
                f.Span)).ToList(),
            rec.Span),

        FieldAccessExpressionNode fa => new FieldAccessExpr(
            DesugarExpr(fa.Record),
            new Name(fa.FieldName.Text),
            fa.Span),

        ParenthesizedExpressionNode paren => DesugarExpr(paren.Inner),

        ErrorExpressionNode err => new ErrorExpr("parse error", err.Span),

        _ => new ErrorExpr($"unknown expression node: {node.Kind}", node.Span)
    };

    private Pattern DesugarPattern(PatternNode node) => node switch
    {
        VariablePatternNode v => new VarPattern(new Name(v.Name.Text), v.Span),
        LiteralPatternNode l => new LiteralPattern(
            l.Literal.LiteralValue ?? l.Literal.Text,
            l.Literal.Kind switch
            {
                TokenKind.IntegerLiteral => LiteralKind.Integer,
                TokenKind.TextLiteral => LiteralKind.Text,
                TokenKind.TrueKeyword or TokenKind.FalseKeyword => LiteralKind.Boolean,
                _ => LiteralKind.Text
            },
            l.Span),
        ConstructorPatternNode c => new CtorPattern(
            new Name(c.Constructor.Text),
            c.SubPatterns.Select(DesugarPattern).ToList(),
            c.Span),
        WildcardPatternNode w => new WildcardPattern(w.Span),
        _ => new WildcardPattern(node.Span)
    };

    private TypeExpr DesugarType(TypeNode node) => node switch
    {
        NamedTypeNode n => new NamedTypeExpr(new Name(n.Name.Text), n.Span),
        FunctionTypeNode f => new FunctionTypeExpr(
            DesugarType(f.Parameter),
            DesugarType(f.Return),
            f.Span),
        ApplicationTypeNode a => new AppliedTypeExpr(
            DesugarType(a.Constructor),
            a.Arguments.Select(DesugarType).ToList(),
            a.Span),
        ParenthesizedTypeNode p => DesugarType(p.Inner),
        _ => new NamedTypeExpr(new Name("?"), node.Span)
    };

    private static BinaryOp DesugarBinaryOp(TokenKind kind) => kind switch
    {
        TokenKind.Plus => BinaryOp.Add,
        TokenKind.Minus => BinaryOp.Sub,
        TokenKind.Star => BinaryOp.Mul,
        TokenKind.Slash => BinaryOp.Div,
        TokenKind.Caret => BinaryOp.Pow,
        TokenKind.DoubleEquals => BinaryOp.Eq,
        TokenKind.NotEquals => BinaryOp.NotEq,
        TokenKind.LessThan => BinaryOp.Lt,
        TokenKind.GreaterThan => BinaryOp.Gt,
        TokenKind.LessOrEqual => BinaryOp.LtEq,
        TokenKind.GreaterOrEqual => BinaryOp.GtEq,
        TokenKind.TripleEquals => BinaryOp.DefEq,
        TokenKind.PlusPlus => BinaryOp.Append,
        TokenKind.ColonColon => BinaryOp.Cons,
        TokenKind.Ampersand => BinaryOp.And,
        TokenKind.Pipe => BinaryOp.Or,
        _ => BinaryOp.Add
    };
}
