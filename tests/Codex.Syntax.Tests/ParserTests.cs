using Codex.Core;
using Codex.Syntax;
using Xunit;

namespace Codex.Syntax.Tests;

public class ParserTests
{
    private static DocumentNode Parse(string source)
    {
        SourceText src = new SourceText("test.codex", source);
        DiagnosticBag bag = new DiagnosticBag();
        Lexer lexer = new Lexer(src, bag);
        IReadOnlyList<Token> tokens = lexer.TokenizeAll();
        Parser parser = new Parser(tokens, bag);
        return parser.ParseDocument();
    }

    private static (DocumentNode Doc, DiagnosticBag Diags) ParseWithDiags(string source)
    {
        SourceText src = new SourceText("test.codex", source);
        DiagnosticBag bag = new DiagnosticBag();
        Lexer lexer = new Lexer(src, bag);
        IReadOnlyList<Token> tokens = lexer.TokenizeAll();
        Parser parser = new Parser(tokens, bag);
        return (parser.ParseDocument(), bag);
    }

    [Fact]
    public void Parse_simple_definition()
    {
        DocumentNode doc = Parse("x = 42");
        Assert.Single(doc.Definitions);
        Assert.Equal("x", doc.Definitions[0].Name.Text);
        Assert.IsType<LiteralExpressionNode>(doc.Definitions[0].Body);
    }

    [Fact]
    public void Parse_definition_with_type_annotation()
    {
        string source = "square : Integer -> Integer\nsquare (x) = x * x";
        DocumentNode doc = Parse(source);
        Assert.Single(doc.Definitions);
        Assert.NotNull(doc.Definitions[0].TypeAnnotation);
        Assert.Equal("square", doc.Definitions[0].Name.Text);
    }

    [Fact]
    public void Parse_definition_with_parameters()
    {
        DocumentNode doc = Parse("add (x) (y) = x + y");
        Assert.Single(doc.Definitions);
        Assert.Equal(2, doc.Definitions[0].Parameters.Count);
    }

    [Fact]
    public void Parse_if_expression()
    {
        DocumentNode doc = Parse("x = if True then 1 else 0");
        Assert.IsType<IfExpressionNode>(doc.Definitions[0].Body);
    }

    [Fact]
    public void Parse_let_expression()
    {
        DocumentNode doc = Parse("x = let a = 1 in a + 2");
        Assert.IsType<LetExpressionNode>(doc.Definitions[0].Body);
    }

    [Fact]
    public void Parse_binary_expression()
    {
        DocumentNode doc = Parse("x = 1 + 2 * 3");
        ExpressionNode body = doc.Definitions[0].Body;
        Assert.IsType<BinaryExpressionNode>(body);
        BinaryExpressionNode add = (BinaryExpressionNode)body;
        Assert.Equal(TokenKind.Plus, add.Operator.Kind);
        Assert.IsType<BinaryExpressionNode>(add.Right);
    }

    [Fact]
    public void Parse_list_literal()
    {
        DocumentNode doc = Parse("xs = [1, 2, 3]");
        Assert.IsType<ListExpressionNode>(doc.Definitions[0].Body);
        ListExpressionNode list = (ListExpressionNode)doc.Definitions[0].Body;
        Assert.Equal(3, list.Elements.Count);
    }

    [Fact]
    public void Parse_function_application()
    {
        DocumentNode doc = Parse("x = f 1 2");
        ExpressionNode body = doc.Definitions[0].Body;
        Assert.IsType<ApplicationExpressionNode>(body);
    }

    [Fact]
    public void Parse_record_construction()
    {
        DocumentNode doc = Parse("x = Point { x = 1, y = 2 }");
        Assert.IsType<RecordExpressionNode>(doc.Definitions[0].Body);
    }

    [Fact]
    public void Parse_match_expression()
    {
        string source = "x = when y if True -> 1 if False -> 0";
        (DocumentNode doc, DiagnosticBag diags) = ParseWithDiags(source);
        ExpressionNode body = doc.Definitions[0].Body;
        Assert.IsType<MatchExpressionNode>(body);
        MatchExpressionNode match = (MatchExpressionNode)body;
        Assert.Equal(2, match.Branches.Count);
    }

    [Fact]
    public void Parse_multiple_definitions()
    {
        string source = "a = 1\nb = 2\nc = 3";
        DocumentNode doc = Parse(source);
        Assert.Equal(3, doc.Definitions.Count);
    }

    [Fact]
    public void Parse_function_type()
    {
        string source = "f : Integer -> Integer -> Integer\nf (x) (y) = x + y";
        DocumentNode doc = Parse(source);
        Assert.NotNull(doc.Definitions[0].TypeAnnotation);
        Assert.IsType<FunctionTypeNode>(doc.Definitions[0].TypeAnnotation!.Type);
    }

    [Fact]
    public void Parse_negative_number()
    {
        DocumentNode doc = Parse("x = -42");
        Assert.IsType<UnaryExpressionNode>(doc.Definitions[0].Body);
    }

    [Fact]
    public void Parse_parenthesized_expression()
    {
        DocumentNode doc = Parse("x = (1 + 2) * 3");
        ExpressionNode body = doc.Definitions[0].Body;
        Assert.IsType<BinaryExpressionNode>(body);
        BinaryExpressionNode mul = (BinaryExpressionNode)body;
        Assert.Equal(TokenKind.Star, mul.Operator.Kind);
    }
}
