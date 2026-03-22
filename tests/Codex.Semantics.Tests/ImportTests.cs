using Codex.Core;
using Codex.Syntax;
using Codex.Ast;
using Codex.Semantics;
using Xunit;

namespace Codex.Semantics.Tests;

public class ImportTests
{
    sealed class MockModuleLoader(Map<string, ResolvedModule> modules) : IModuleLoader
    {
        readonly Map<string, ResolvedModule> m_modules = modules;

        public ResolvedModule? Load(string moduleName) => m_modules[moduleName];
    }

    static ResolvedModule CompileModule(string source, string name)
    {
        SourceText src = new($"{name}.codex", source);
        DiagnosticBag bag = new();
        Lexer lexer = new(src, bag);
        IReadOnlyList<Token> tokens = lexer.TokenizeAll();
        Parser parser = new(tokens, bag);
        DocumentNode doc = parser.ParseDocument();
        Desugarer desugarer = new(bag);
        Module module = desugarer.Desugar(doc, name);
        NameResolver resolver = new(bag);
        return resolver.Resolve(module);
    }

    static (ResolvedModule Resolved, DiagnosticBag Diags) ResolveWithLoader(
        string source, IModuleLoader loader)
    {
        SourceText src = new("test.codex", source);
        DiagnosticBag bag = new();
        Lexer lexer = new(src, bag);
        IReadOnlyList<Token> tokens = lexer.TokenizeAll();
        Parser parser = new(tokens, bag);
        DocumentNode doc = parser.ParseDocument();
        Desugarer desugarer = new(bag);
        Module module = desugarer.Desugar(doc, "Test");
        NameResolver resolver = new(bag, loader);
        ResolvedModule resolved = resolver.Resolve(module);
        return (resolved, bag);
    }

    [Fact]
    public void Import_makes_definitions_available()
    {
        ResolvedModule mathModule = CompileModule(
            "double (x) = x + x\ntriple (x) = x + x + x", "Math");

        Map<string, ResolvedModule> modules =
            Map<string, ResolvedModule>.s_empty.Set("Math", mathModule);
        MockModuleLoader loader = new(modules);

        string source = "import Math\nresult = double 21";
        (ResolvedModule _, DiagnosticBag diags) = ResolveWithLoader(source, loader);
        Assert.False(diags.HasErrors, string.Join("; ", diags.ToImmutable()));
    }

    [Fact]
    public void Unresolved_import_reports_error()
    {
        Map<string, ResolvedModule> modules = Map<string, ResolvedModule>.s_empty;
        MockModuleLoader loader = new(modules);

        string source = "import Missing\nresult = 42";
        (ResolvedModule _, DiagnosticBag diags) = ResolveWithLoader(source, loader);
        Assert.True(diags.HasErrors);
        Assert.Contains(diags.ToImmutable(), d => d.Code == "CDX3010");
    }

    [Fact]
    public void Import_without_loader_reports_error()
    {
        SourceText src = new("test.codex", "import Math\nresult = 42");
        DiagnosticBag bag = new();
        Lexer lexer = new(src, bag);
        IReadOnlyList<Token> tokens = lexer.TokenizeAll();
        Parser parser = new(tokens, bag);
        DocumentNode doc = parser.ParseDocument();
        Desugarer desugarer = new(bag);
        Module module = desugarer.Desugar(doc, "Test");
        NameResolver resolver = new(bag);
        resolver.Resolve(module);
        Assert.True(bag.HasErrors);
        Assert.Contains(bag.ToImmutable(), d => d.Code == "CDX3010");
    }

    [Fact]
    public void Import_parses_correctly()
    {
        SourceText src = new("test.codex", "import Math\nx = 1");
        DiagnosticBag bag = new();
        Lexer lexer = new(src, bag);
        IReadOnlyList<Token> tokens = lexer.TokenizeAll();
        Parser parser = new(tokens, bag);
        DocumentNode doc = parser.ParseDocument();
        Assert.False(bag.HasErrors, string.Join("; ", bag.ToImmutable()));
        Assert.Single(doc.Imports);
        Assert.Equal("Math", doc.Imports[0].Name.Text);
        Assert.Single(doc.Definitions);
    }

    [Fact]
    public void Imported_name_used_in_body_resolves()
    {
        ResolvedModule helperModule = CompileModule(
            "helper (x) = x + 1", "Helpers");

        Map<string, ResolvedModule> modules =
            Map<string, ResolvedModule>.s_empty.Set("Helpers", helperModule);
        MockModuleLoader loader = new(modules);

        string source = "import Helpers\nresult = helper 5";
        (ResolvedModule resolved, DiagnosticBag diags) =
            ResolveWithLoader(source, loader);
        Assert.False(diags.HasErrors, string.Join("; ", diags.ToImmutable()));
        Assert.Single(resolved.ImportedModules);
    }
}
