using Codex.Core;
using Codex.Syntax;
using Codex.Ast;
using Codex.Semantics;
using Xunit;

namespace Codex.Semantics.Tests;

public class NameResolverTests
{
    private static (ResolvedModule Resolved, DiagnosticBag Diags) ResolveSource(string source)
    {
        SourceText src = new("test.codex", source);
        DiagnosticBag bag = new();
        Lexer lexer = new(src, bag);
        IReadOnlyList<Token> tokens = lexer.TokenizeAll();
        Parser parser = new(tokens, bag);
        DocumentNode doc = parser.ParseDocument();
        Desugarer desugarer = new(bag);
        Module module = desugarer.Desugar(doc, "Test");
        NameResolver resolver = new(bag);
        ResolvedModule resolved = resolver.Resolve(module);
        return (resolved, bag);
    }

    [Fact]
    public void Self_referencing_definition_resolves()
    {
        (ResolvedModule resolved, DiagnosticBag diags) = ResolveSource("x = 42");
        Assert.False(diags.HasErrors);
        Assert.Contains("x", resolved.TopLevelNames);
    }

    [Fact]
    public void Undefined_name_reports_error()
    {
        (ResolvedModule _, DiagnosticBag diags) = ResolveSource("x = y");
        Assert.True(diags.HasErrors);
        Assert.Contains(diags.ToImmutable(), d => d.Code == "CDX3002");
    }

    [Fact]
    public void Parameter_is_in_scope()
    {
        (ResolvedModule _, DiagnosticBag diags) = ResolveSource("f (x) = x + 1");
        Assert.False(diags.HasErrors);
    }

    [Fact]
    public void Let_binding_is_in_scope()
    {
        (ResolvedModule _, DiagnosticBag diags) = ResolveSource("f = let a = 1 in a + 2");
        Assert.False(diags.HasErrors);
    }

    [Fact]
    public void Cross_definition_reference_resolves()
    {
        string source = "a = 1\nb = a + 2";
        (ResolvedModule _, DiagnosticBag diags) = ResolveSource(source);
        Assert.False(diags.HasErrors);
    }

    [Fact]
    public void Forward_reference_resolves()
    {
        string source = "a = b\nb = 42";
        (ResolvedModule _, DiagnosticBag diags) = ResolveSource(source);
        Assert.False(diags.HasErrors);
    }

    [Fact]
    public void Duplicate_definition_reports_error()
    {
        string source = "x = 1\nx = 2";
        (ResolvedModule _, DiagnosticBag diags) = ResolveSource(source);
        Assert.True(diags.HasErrors);
        Assert.Contains(diags.ToImmutable(), d => d.Code == "CDX3001");
    }

    [Fact]
    public void Type_name_as_expression_does_not_error()
    {
        (ResolvedModule _, DiagnosticBag diags) = ResolveSource("x = True");
        Assert.False(diags.HasErrors);
    }

    [Fact]
    public void Match_pattern_binds_variable()
    {
        string source = "f = when True if y -> y";
        (ResolvedModule _, DiagnosticBag diags) = ResolveSource(source);
        Assert.False(diags.HasErrors);
    }

    [Fact]
    public void Builtin_show_is_in_scope()
    {
        (ResolvedModule _, DiagnosticBag diags) = ResolveSource("x = show 42");
        Assert.False(diags.HasErrors);
    }
}
