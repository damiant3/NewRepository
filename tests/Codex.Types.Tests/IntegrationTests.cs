using System.Collections.Immutable;
using Codex.Core;
using Codex.Syntax;
using Codex.Ast;
using Codex.Semantics;
using Codex.Types;
using Codex.IR;
using Codex.Emit.CSharp;
using Xunit;

namespace Codex.Types.Tests;

public class IntegrationTests
{
    private static string? CompileToCS(string source, string moduleName = "test")
    {
        SourceText src = new SourceText("test.codex", source);
        DiagnosticBag diagnostics = new DiagnosticBag();

        DocumentNode document;
        if (ProseParser.IsProseDocument(source))
        {
            ProseParser proseParser = new ProseParser(src, diagnostics);
            document = proseParser.ParseDocument();
        }
        else
        {
            Lexer lexer = new Lexer(src, diagnostics);
            IReadOnlyList<Token> tokens = lexer.TokenizeAll();
            Parser parser = new Parser(tokens, diagnostics);
            document = parser.ParseDocument();
        }

        Desugarer desugarer = new Desugarer(diagnostics);
        Module module = desugarer.Desugar(document, moduleName);
        if (diagnostics.HasErrors) return null;

        NameResolver resolver = new NameResolver(diagnostics);
        ResolvedModule resolved = resolver.Resolve(module);
        if (diagnostics.HasErrors) return null;

        TypeChecker checker = new TypeChecker(diagnostics);
        ImmutableDictionary<string, CodexType> types = checker.CheckModule(resolved.Module);
        if (diagnostics.HasErrors) return null;

        Lowering lowering = new Lowering(types, diagnostics);
        IRModule irModule = lowering.Lower(resolved.Module);
        if (diagnostics.HasErrors) return null;

        CSharpEmitter emitter = new CSharpEmitter();
        return emitter.Emit(irModule);
    }

    private static ImmutableDictionary<string, CodexType>? TypeCheck(
        string source, string moduleName = "test")
    {
        SourceText src = new SourceText("test.codex", source);
        DiagnosticBag diagnostics = new DiagnosticBag();

        DocumentNode document;
        if (ProseParser.IsProseDocument(source))
        {
            ProseParser proseParser = new ProseParser(src, diagnostics);
            document = proseParser.ParseDocument();
        }
        else
        {
            Lexer lexer = new Lexer(src, diagnostics);
            IReadOnlyList<Token> tokens = lexer.TokenizeAll();
            Parser parser = new Parser(tokens, diagnostics);
            document = parser.ParseDocument();
        }

        Desugarer desugarer = new Desugarer(diagnostics);
        Module module = desugarer.Desugar(document, moduleName);
        if (diagnostics.HasErrors) return null;

        NameResolver resolver = new NameResolver(diagnostics);
        ResolvedModule resolved = resolver.Resolve(module);
        if (diagnostics.HasErrors) return null;

        TypeChecker checker = new TypeChecker(diagnostics);
        ImmutableDictionary<string, CodexType> types = checker.CheckModule(resolved.Module);
        return diagnostics.HasErrors ? null : types;
    }

    // --- Notation-only programs ---

    [Fact]
    public void Hello_compiles_to_csharp()
    {
        string source = "square : Integer -> Integer\nsquare (x) = x * x\n\nmain : Integer\nmain = square 5";
        string? cs = CompileToCS(source, "hello");
        Assert.NotNull(cs);
        Assert.Contains("square", cs);
        Assert.Contains("main", cs);
        Assert.Contains("Console.WriteLine", cs);
    }

    [Fact]
    public void Factorial_compiles_to_csharp()
    {
        string source =
            "factorial : Integer -> Integer\n" +
            "factorial (n) = if n == 0 then 1 else n * factorial (n - 1)\n\n" +
            "main : Integer\nmain = factorial 10";
        string? cs = CompileToCS(source, "factorial");
        Assert.NotNull(cs);
        Assert.Contains("factorial", cs);
    }

    [Fact]
    public void Greeting_compiles_to_csharp()
    {
        string source =
            "greeting : Text -> Text\n" +
            "greeting (name) = \"Hello, \" ++ name ++ \"!\"\n\n" +
            "main : Text\nmain = greeting \"World\"";
        string? cs = CompileToCS(source, "greeting");
        Assert.NotNull(cs);
        Assert.Contains("string.Concat", cs);
    }

    [Fact]
    public void Let_binding_compiles()
    {
        string source =
            "clamp : Integer -> Integer -> Integer -> Integer\n" +
            "clamp (lo) (hi) (x) =\n" +
            "  let clamped = if x < lo then lo else if x > hi then hi else x\n" +
            "  in clamped";
        string? cs = CompileToCS(source, "clamp");
        Assert.NotNull(cs);
        Assert.Contains("clamped", cs);
    }

    [Fact]
    public void Fibonacci_type_checks()
    {
        string source =
            "fib (n) = if n == 0 then 0 else if n == 1 then 1 else fib (n - 1) + fib (n - 2)\n\n" +
            "main : Integer\nmain = fib 20";
        ImmutableDictionary<string, CodexType>? types = TypeCheck(source);
        Assert.NotNull(types);
        Assert.True(types!.ContainsKey("fib"));
        Assert.True(types.ContainsKey("main"));
    }

    // --- Prose-mode programs ---

    [Fact]
    public void Prose_greeting_compiles_to_csharp()
    {
        string source =
            "Chapter: Greeting\n\n" +
            "  This module greets people.\n\n" +
            "  We say:\n\n" +
            "    greet : Text -> Text\n" +
            "    greet (name) = \"Hello, \" ++ name ++ \"!\"\n\n" +
            "    main : Text\n" +
            "    main = greet \"World\"\n";
        string? cs = CompileToCS(source, "greeting");
        Assert.NotNull(cs);
        Assert.Contains("greet", cs);
        Assert.Contains("Console.WriteLine", cs);
    }

    [Fact]
    public void Prose_document_type_checks()
    {
        string source =
            "Chapter: Math\n\n" +
            "    double : Integer -> Integer\n" +
            "    double (x) = x + x\n\n" +
            "    main : Integer\n" +
            "    main = double 21\n";
        ImmutableDictionary<string, CodexType>? types = TypeCheck(source);
        Assert.NotNull(types);
        Assert.True(types!.ContainsKey("double"));
        Assert.Equal("Integer → Integer", types["double"].ToString());
    }

    // --- Generated C# structure ---

    [Fact]
    public void Emitted_csharp_has_class_prefix()
    {
        string source = "main : Integer\nmain = 42";
        string? cs = CompileToCS(source, "mymodule");
        Assert.NotNull(cs);
        Assert.Contains("Codex_mymodule", cs);
    }

    [Fact]
    public void Emitted_csharp_has_top_level_console_writeline()
    {
        string source = "main : Integer\nmain = 42";
        string? cs = CompileToCS(source);
        Assert.NotNull(cs);
        string beforeClass = cs!.Substring(0, cs.IndexOf("public static class"));
        Assert.Contains("Console.WriteLine", beforeClass);
    }
}
