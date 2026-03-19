using Codex.Ast;
using Codex.Core;
using Codex.Semantics;
using Codex.Syntax;
using Codex.Types;
using Xunit;

namespace Codex.Types.Tests;

public class PreludeTests
{
    static string FindPreludeDir()
    {
        string dir = AppContext.BaseDirectory;
        while (dir is not null)
        {
            string candidate = Path.Combine(dir, "prelude");
            if (Directory.Exists(candidate))
                return candidate;
            dir = Path.GetDirectoryName(dir)!;
        }
        throw new DirectoryNotFoundException("Cannot find prelude/ directory");
    }

    static DiagnosticBag CompilePreludeFile(string fileName)
    {
        string path = Path.Combine(FindPreludeDir(), fileName);
        string source = File.ReadAllText(path);
        string moduleName = Path.GetFileNameWithoutExtension(fileName);

        SourceText src = new(path, source);
        DiagnosticBag diagnostics = new();

        Lexer lexer = new(src, diagnostics);
        IReadOnlyList<Token> tokens = lexer.TokenizeAll();
        Parser parser = new(tokens, diagnostics);
        DocumentNode document = parser.ParseDocument();
        if (diagnostics.HasErrors) return diagnostics;

        Desugarer desugarer = new(diagnostics);
        Module module = desugarer.Desugar(document, moduleName);
        if (diagnostics.HasErrors) return diagnostics;

        NameResolver resolver = new(diagnostics);
        ResolvedModule resolved = resolver.Resolve(module);
        if (diagnostics.HasErrors) return diagnostics;

        TypeChecker checker = new(diagnostics);
        checker.CheckModule(resolved.Module);
        return diagnostics;
    }

    [Fact]
    public void Maybe_compiles()
    {
        DiagnosticBag diag = CompilePreludeFile("Maybe.codex");
        Assert.False(diag.HasErrors, string.Join("; ", diag.ToImmutable()));
    }

    [Fact]
    public void Result_compiles()
    {
        DiagnosticBag diag = CompilePreludeFile("Result.codex");
        Assert.False(diag.HasErrors, string.Join("; ", diag.ToImmutable()));
    }

    [Fact]
    public void Either_compiles()
    {
        DiagnosticBag diag = CompilePreludeFile("Either.codex");
        Assert.False(diag.HasErrors, string.Join("; ", diag.ToImmutable()));
    }

    [Fact]
    public void Pair_compiles()
    {
        DiagnosticBag diag = CompilePreludeFile("Pair.codex");
        Assert.False(diag.HasErrors, string.Join("; ", diag.ToImmutable()));
    }

    [Fact]
    public void Maybe_used_in_program()
    {
        string source = """
            import Maybe

            safe-div : Integer -> Integer -> Maybe Integer
            safe-div (a) (b) = if b == 0 then None else Just (a / b)

            main : Integer
            main = from-maybe (safe-div 10 2) 0
            """;
        DiagnosticBag diag = CompileWithPrelude(source);
        Assert.False(diag.HasErrors, string.Join("; ", diag.ToImmutable()));
    }

    [Fact]
    public void Result_used_in_program()
    {
        string source = """
            import Result

            parse-nat : Text -> Result Integer Text
            parse-nat (t) = Ok (text-to-integer t)

            main : Integer
            main = from-ok (parse-nat "42") 0
            """;
        DiagnosticBag diag = CompileWithPrelude(source);
        Assert.False(diag.HasErrors, string.Join("; ", diag.ToImmutable()));
    }

    [Fact]
    public void Either_used_in_program()
    {
        string source = """
            import Either

            to-zero : Text -> Integer
            to-zero (s) = 0

            identity : Integer -> Integer
            identity (n) = n

            classify : Integer -> Either Text Integer
            classify (n) = if n > 0 then Right n else Left "negative"

            main : Integer
            main = either to-zero identity (classify 5)
            """;
        DiagnosticBag diag = CompileWithPrelude(source);
        Assert.False(diag.HasErrors, string.Join("; ", diag.ToImmutable()));
    }

    [Fact]
    public void Pair_used_in_program()
    {
        string source = """
            import Pair

            main : Integer
            main = pair-fst (make-pair 1 2)
            """;
        DiagnosticBag diag = CompileWithPrelude(source);
        Assert.False(diag.HasErrors, string.Join("; ", diag.ToImmutable()));
    }

    static DiagnosticBag CompileWithPrelude(string source)
    {
        string preludeDir = FindPreludeDir();
        SourceText src = new("test.codex", source);
        DiagnosticBag diagnostics = new();

        Lexer lexer = new(src, diagnostics);
        IReadOnlyList<Token> tokens = lexer.TokenizeAll();
        Parser parser = new(tokens, diagnostics);
        DocumentNode document = parser.ParseDocument();
        if (diagnostics.HasErrors) return diagnostics;

        Desugarer desugarer = new(diagnostics);
        Module module = desugarer.Desugar(document, "test");
        if (diagnostics.HasErrors) return diagnostics;

        PreludeTestLoader preludeLoader = new(preludeDir, diagnostics);
        NameResolver resolver = new(diagnostics, preludeLoader);
        ResolvedModule resolved = resolver.Resolve(module);
        if (diagnostics.HasErrors) return diagnostics;

        TypeChecker checker = new(diagnostics);

        foreach (ResolvedModule imported in resolved.ImportedModules)
            checker.CheckModule(imported.Module);

        checker.CheckModule(resolved.Module);
        return diagnostics;
    }
}

sealed class PreludeTestLoader(string preludeDir, DiagnosticBag diagnostics) : IModuleLoader
{
    readonly string m_preludeDir = preludeDir;
    readonly DiagnosticBag m_diagnostics = diagnostics;
    Map<string, ResolvedModule> m_cache = Map<string, ResolvedModule>.s_empty;

    public ResolvedModule? Load(string moduleName)
    {
        ResolvedModule? cached = m_cache[moduleName];
        if (cached is not null)
            return cached;

        string filePath = Path.Combine(m_preludeDir, moduleName + ".codex");
        if (!File.Exists(filePath))
            return null;

        string source = File.ReadAllText(filePath);
        SourceText src = new(filePath, source);
        DiagnosticBag compileDiag = new();

        Lexer lexer = new(src, compileDiag);
        IReadOnlyList<Token> tokens = lexer.TokenizeAll();
        Parser parser = new(tokens, compileDiag);
        DocumentNode document = parser.ParseDocument();
        if (compileDiag.HasErrors) return null;

        Desugarer desugarer = new(compileDiag);
        Module module = desugarer.Desugar(document, moduleName);
        if (compileDiag.HasErrors) return null;

        NameResolver resolver = new(compileDiag);
        ResolvedModule resolved = resolver.Resolve(module);
        if (compileDiag.HasErrors) return null;

        m_cache = m_cache.Set(moduleName, resolved);
        return resolved;
    }
}
