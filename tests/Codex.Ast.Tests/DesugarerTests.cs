using Codex.Core;
using Codex.Syntax;
using Codex.Ast;
using Xunit;

namespace Codex.Ast.Tests;

public class DesugarerTests
{
    private static Module ParseAndDesugar(string source, string moduleName = "Test")
    {
        SourceText src = new("test.codex", source);
        DiagnosticBag bag = new();
        Lexer lexer = new(src, bag);
        IReadOnlyList<Token> tokens = lexer.TokenizeAll();
        Parser parser = new(tokens, bag);
        DocumentNode doc = parser.ParseDocument();
        Desugarer desugarer = new(bag);
        return desugarer.Desugar(doc, moduleName);
    }

    [Fact]
    public void Desugar_simple_definition()
    {
        Module module = ParseAndDesugar("x = 42");
        Assert.Single(module.Definitions);
        Assert.Equal("x", module.Definitions[0].Name.Value);
        Assert.IsType<LiteralExpr>(module.Definitions[0].Body);
    }

    [Fact]
    public void Desugar_preserves_module_name()
    {
        Module module = ParseAndDesugar("x = 1", "MyModule");
        Assert.Equal("MyModule", module.Name.Leaf.Value);
    }

    [Fact]
    public void Desugar_binary_expression()
    {
        Module module = ParseAndDesugar("x = 1 + 2");
        Assert.IsType<BinaryExpr>(module.Definitions[0].Body);
        BinaryExpr bin = (BinaryExpr)module.Definitions[0].Body;
        Assert.Equal(BinaryOp.Add, bin.Op);
    }

    [Fact]
    public void Desugar_if_expression()
    {
        Module module = ParseAndDesugar("x = if True then 1 else 0");
        Assert.IsType<IfExpr>(module.Definitions[0].Body);
    }

    [Fact]
    public void Desugar_let_expression()
    {
        Module module = ParseAndDesugar("x = let a = 1 in a + 2");
        Assert.IsType<LetExpr>(module.Definitions[0].Body);
        LetExpr letExpr = (LetExpr)module.Definitions[0].Body;
        Assert.Single(letExpr.Bindings);
        Assert.Equal("a", letExpr.Bindings[0].Name.Value);
    }

    [Fact]
    public void Desugar_with_type_annotation()
    {
        string source = "square : Integer -> Integer\nsquare (x) = x * x";
        Module module = ParseAndDesugar(source);
        Assert.Single(module.Definitions);
        Assert.NotNull(module.Definitions[0].DeclaredType);
        Assert.IsType<FunctionTypeExpr>(module.Definitions[0].DeclaredType);
    }

    [Fact]
    public void Desugar_list_expression()
    {
        Module module = ParseAndDesugar("xs = [1, 2, 3]");
        Assert.IsType<ListExpr>(module.Definitions[0].Body);
        ListExpr list = (ListExpr)module.Definitions[0].Body;
        Assert.Equal(3, list.Elements.Count);
    }

    [Fact]
    public void Desugar_function_application()
    {
        Module module = ParseAndDesugar("x = f 1");
        Assert.IsType<ApplyExpr>(module.Definitions[0].Body);
    }

    [Fact]
    public void Desugar_multiple_definitions()
    {
        Module module = ParseAndDesugar("a = 1\nb = 2\nc = 3");
        Assert.Equal(3, module.Definitions.Count);
    }

    [Fact]
    public void Desugar_match_expression()
    {
        string source = "x = when y if True -> 1 if False -> 0";
        Module module = ParseAndDesugar(source);
        Assert.IsType<MatchExpr>(module.Definitions[0].Body);
        MatchExpr match = (MatchExpr)module.Definitions[0].Body;
        Assert.Equal(2, match.Branches.Count);
    }

    [Fact]
    public void Desugar_record_expression()
    {
        Module module = ParseAndDesugar("p = Point { x = 1, y = 2 }");
        Assert.IsType<RecordExpr>(module.Definitions[0].Body);
    }

    [Fact]
    public void Desugar_interpolated_string_to_append_chain()
    {
        Module module = ParseAndDesugar("x = \"hello {name}!\"");
        Expr body = module.Definitions[0].Body;
        Assert.IsType<BinaryExpr>(body);
        BinaryExpr outer = (BinaryExpr)body;
        Assert.Equal(BinaryOp.Append, outer.Op);

        Assert.IsType<BinaryExpr>(outer.Left);
        BinaryExpr inner = (BinaryExpr)outer.Left;
        Assert.Equal(BinaryOp.Append, inner.Op);

        Assert.IsType<LiteralExpr>(inner.Left);
        LiteralExpr hello = (LiteralExpr)inner.Left;
        Assert.Equal("hello ", hello.Value);

        Assert.IsType<ApplyExpr>(inner.Right);
        ApplyExpr showCall = (ApplyExpr)inner.Right;
        Assert.IsType<NameExpr>(showCall.Function);
        Assert.Equal("show", ((NameExpr)showCall.Function).Name.Value);
        Assert.IsType<NameExpr>(showCall.Argument);
        Assert.Equal("name", ((NameExpr)showCall.Argument).Name.Value);

        Assert.IsType<LiteralExpr>(outer.Right);
        LiteralExpr excl = (LiteralExpr)outer.Right;
        Assert.Equal("!", excl.Value);
    }

    [Fact]
    public void Desugar_interpolated_string_single_text_to_literal()
    {
        Module module = ParseAndDesugar("x = \"just text\"");
        Expr body = module.Definitions[0].Body;
        Assert.IsType<LiteralExpr>(body);
        LiteralExpr lit = (LiteralExpr)body;
        Assert.Equal("just text", lit.Value);
    }

    [Fact]
    public void Desugar_interpolated_string_single_expr()
    {
        Module module = ParseAndDesugar("x = \"{name}\"");
        Expr body = module.Definitions[0].Body;
        Assert.IsType<ApplyExpr>(body);
        ApplyExpr showCall = (ApplyExpr)body;
        Assert.IsType<NameExpr>(showCall.Function);
        Assert.Equal("show", ((NameExpr)showCall.Function).Name.Value);
        Assert.IsType<NameExpr>(showCall.Argument);
        Assert.Equal("name", ((NameExpr)showCall.Argument).Name.Value);
    }

    [Fact]
    public void Desugar_interpolated_string_empty()
    {
        Module module = ParseAndDesugar("x = \"\"");
        Expr body = module.Definitions[0].Body;
        Assert.IsType<LiteralExpr>(body);
        LiteralExpr lit = (LiteralExpr)body;
        Assert.Equal("", lit.Value);
    }

    [Fact]
    public void Desugar_interpolated_integer_expr_wraps_in_show()
    {
        Module module = ParseAndDesugar("x = \"value: {42}\"");
        Expr body = module.Definitions[0].Body;
        Assert.IsType<BinaryExpr>(body);
        BinaryExpr bin = (BinaryExpr)body;
        Assert.Equal(BinaryOp.Append, bin.Op);

        Assert.IsType<LiteralExpr>(bin.Left);
        Assert.Equal("value: ", ((LiteralExpr)bin.Left).Value);

        Assert.IsType<ApplyExpr>(bin.Right);
        ApplyExpr showCall = (ApplyExpr)bin.Right;
        Assert.IsType<NameExpr>(showCall.Function);
        Assert.Equal("show", ((NameExpr)showCall.Function).Name.Value);
        Assert.IsType<LiteralExpr>(showCall.Argument);
    }
}
