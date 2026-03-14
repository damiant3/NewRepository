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

    [Fact]
    public void Parse_record_type_definition()
    {
        string source = "Point = record { x : Number, y : Number }";
        DocumentNode doc = Parse(source);
        Assert.Single(doc.TypeDefinitions);
        Assert.Equal("Point", doc.TypeDefinitions[0].Name.Text);
        Assert.IsType<RecordTypeBody>(doc.TypeDefinitions[0].Body);
        RecordTypeBody body = (RecordTypeBody)doc.TypeDefinitions[0].Body;
        Assert.Equal(2, body.Fields.Count);
        Assert.Equal("x", body.Fields[0].Name.Text);
        Assert.Equal("y", body.Fields[1].Name.Text);
    }

    [Fact]
    public void Parse_variant_type_definition()
    {
        string source = "Color =\n  | Red\n  | Green\n  | Blue";
        DocumentNode doc = Parse(source);
        Assert.Single(doc.TypeDefinitions);
        Assert.Equal("Color", doc.TypeDefinitions[0].Name.Text);
        Assert.IsType<VariantTypeBody>(doc.TypeDefinitions[0].Body);
        VariantTypeBody body = (VariantTypeBody)doc.TypeDefinitions[0].Body;
        Assert.Equal(3, body.Constructors.Count);
        Assert.Equal("Red", body.Constructors[0].Name.Text);
        Assert.Equal("Green", body.Constructors[1].Name.Text);
        Assert.Equal("Blue", body.Constructors[2].Name.Text);
    }

    [Fact]
    public void Parse_variant_with_fields()
    {
        string source = "Shape =\n  | Circle (radius : Number)\n  | Rect (width : Number) (height : Number)";
        DocumentNode doc = Parse(source);
        Assert.Single(doc.TypeDefinitions);
        VariantTypeBody body = (VariantTypeBody)doc.TypeDefinitions[0].Body;
        Assert.Equal(2, body.Constructors.Count);
        Assert.Single(body.Constructors[0].Fields);
        Assert.Equal(2, body.Constructors[1].Fields.Count);
    }

    [Fact]
    public void Parse_variant_with_type_parameters()
    {
        string source = "Maybe (a) =\n  | Just (value : a)\n  | None";
        DocumentNode doc = Parse(source);
        Assert.Single(doc.TypeDefinitions);
        Assert.Single(doc.TypeDefinitions[0].TypeParameters);
        Assert.Equal("a", doc.TypeDefinitions[0].TypeParameters[0].Text);
    }

    [Fact]
    public void Parse_type_def_and_value_def_together()
    {
        string source =
            "Color =\n  | Red\n  | Green\n  | Blue\n\n" +
            "favorite : Color\nfavorite = Blue\n";
        DocumentNode doc = Parse(source);
        Assert.Single(doc.TypeDefinitions);
        Assert.Single(doc.Definitions);
        Assert.Equal("favorite", doc.Definitions[0].Name.Text);
    }

    [Fact]
    public void Parse_effectful_type_annotation()
    {
        DocumentNode doc = Parse("main : [Console] Nothing\nmain = 42");
        Assert.NotNull(doc.Definitions[0].TypeAnnotation);
        Assert.IsType<EffectfulTypeNode>(doc.Definitions[0].TypeAnnotation!.Type);
        EffectfulTypeNode eft = (EffectfulTypeNode)doc.Definitions[0].TypeAnnotation!.Type;
        Assert.Single(eft.Effects);
        Assert.IsType<NamedTypeNode>(eft.Effects[0]);
        Assert.IsType<NamedTypeNode>(eft.Return);
    }

    [Fact]
    public void Parse_effectful_type_with_multiple_effects()
    {
        DocumentNode doc = Parse("main : [Console, FileSystem] Nothing\nmain = 42");
        Assert.NotNull(doc.Definitions[0].TypeAnnotation);
        Assert.IsType<EffectfulTypeNode>(doc.Definitions[0].TypeAnnotation!.Type);
        EffectfulTypeNode eft = (EffectfulTypeNode)doc.Definitions[0].TypeAnnotation!.Type;
        Assert.Equal(2, eft.Effects.Count);
    }

    [Fact]
    public void Parse_do_expression()
    {
        string source = "main : [Console] Nothing\nmain = do\n  print-line \"hello\"\n";
        DocumentNode doc = Parse(source);
        Assert.IsType<DoExpressionNode>(doc.Definitions[0].Body);
        DoExpressionNode doExpr = (DoExpressionNode)doc.Definitions[0].Body;
        Assert.Single(doExpr.Statements);
        Assert.IsType<DoExprStatementNode>(doExpr.Statements[0]);
    }

    [Fact]
    public void Parse_do_bind_statement()
    {
        string source = "main : [Console] Nothing\nmain = do\n  x <- read-line\n  print-line x\n";
        DocumentNode doc = Parse(source);
        Assert.IsType<DoExpressionNode>(doc.Definitions[0].Body);
        DoExpressionNode doExpr = (DoExpressionNode)doc.Definitions[0].Body;
        Assert.Equal(2, doExpr.Statements.Count);
        Assert.IsType<DoBindStatementNode>(doExpr.Statements[0]);
        DoBindStatementNode bind = (DoBindStatementNode)doExpr.Statements[0];
        Assert.Equal("x", bind.Name.Text);
    }

    [Fact]
    public void Parse_function_with_effectful_return()
    {
        string source = "greet : Text -> [Console] Nothing\ngreet (name) = print-line name\n";
        DocumentNode doc = Parse(source);
        Assert.NotNull(doc.Definitions[0].TypeAnnotation);
        TypeNode typeNode = doc.Definitions[0].TypeAnnotation!.Type;
        Assert.IsType<FunctionTypeNode>(typeNode);
        FunctionTypeNode fn = (FunctionTypeNode)typeNode;
        Assert.IsType<NamedTypeNode>(fn.Parameter);
        Assert.IsType<EffectfulTypeNode>(fn.Return);
    }

    [Fact]
    public void Parse_linear_type_annotation()
    {
        DocumentNode doc = Parse("consume : linear FileHandle -> Nothing\nconsume (h) = h");
        Assert.NotNull(doc.Definitions[0].TypeAnnotation);
        TypeNode typeNode = doc.Definitions[0].TypeAnnotation!.Type;
        Assert.IsType<FunctionTypeNode>(typeNode);
        FunctionTypeNode fn = (FunctionTypeNode)typeNode;
        Assert.IsType<LinearTypeNode>(fn.Parameter);
        LinearTypeNode lin = (LinearTypeNode)fn.Parameter;
        Assert.IsType<NamedTypeNode>(lin.Inner);
    }

    [Fact]
    public void Parse_linear_type_with_effect()
    {
        DocumentNode doc = Parse("f : linear FileHandle -> [FileSystem] Nothing\nf (h) = close-file h");
        Assert.NotNull(doc.Definitions[0].TypeAnnotation);
        TypeNode typeNode = doc.Definitions[0].TypeAnnotation!.Type;
        Assert.IsType<FunctionTypeNode>(typeNode);
        FunctionTypeNode fn = (FunctionTypeNode)typeNode;
        Assert.IsType<LinearTypeNode>(fn.Parameter);
        Assert.IsType<EffectfulTypeNode>(fn.Return);
    }
}
