using Codex.Core;
using Codex.Syntax;
using Codex.Ast;
using Codex.Semantics;
using Codex.Types;
using Codex.IR;
using Codex.Emit.CSharp;
using System.Collections.Immutable;

namespace Codex.Cli;

/// <summary>
/// The Codex command-line interface.
/// This is the primary tool for compiling and running Codex programs.
/// </summary>
public static class Program
{
    public static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            PrintUsage();
            return 0;
        }

        string command = args[0];
        return command switch
        {
            "check" => RunCheck(args.Skip(1).ToArray()),
            "parse" => RunParse(args.Skip(1).ToArray()),
            "build" => RunBuild(args.Skip(1).ToArray()),
            "run" => RunRun(args.Skip(1).ToArray()),
            "version" => RunVersion(),
            "--help" or "-h" => RunHelp(),
            _ => UnknownCommand(command)
        };
    }

    private static int RunParse(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("Usage: codex parse <file.codex>");
            return 1;
        }

        string filePath = args[0];
        if (!File.Exists(filePath))
        {
            Console.Error.WriteLine($"File not found: {filePath}");
            return 1;
        }

        string content = File.ReadAllText(filePath);
        SourceText source = new SourceText(filePath, content);
        DiagnosticBag diagnostics = new DiagnosticBag();

        Lexer lexer = new Lexer(source, diagnostics);
        IReadOnlyList<Token> tokens = lexer.TokenizeAll();

        Console.WriteLine("=== Tokens ===");
        foreach (Token token in tokens)
        {
            if (token.Kind is TokenKind.Newline or TokenKind.Indent or TokenKind.Dedent or TokenKind.EndOfFile)
            {
                Console.WriteLine($"  {token.Kind}");
            }
            else
            {
                Console.WriteLine($"  {token.Kind,-20} {token.Text}");
            }
        }

        Parser parser = new Parser(tokens, diagnostics);
        DocumentNode document = parser.ParseDocument();

        Console.WriteLine("\n=== Definitions ===");
        foreach (DefinitionNode def in document.Definitions)
        {
            string typeStr = def.TypeAnnotation is not null ? $" : {FormatType(def.TypeAnnotation.Type)}" : "";
            string paramsStr = def.Parameters.Count > 0
                ? " (" + string.Join(") (", def.Parameters.Select(p => p.Text)) + ")"
                : "";
            Console.WriteLine($"  {def.Name.Text}{paramsStr}{typeStr}");
        }

        Desugarer desugarer = new Desugarer(diagnostics);
        string moduleName = Path.GetFileNameWithoutExtension(filePath);
        Module module = desugarer.Desugar(document, moduleName);

        Console.WriteLine($"\n=== Module: {module.Name} ===");
        foreach (Definition def in module.Definitions)
        {
            Console.WriteLine($"  {def.Name} : {(def.DeclaredType is not null ? FormatTypeExpr(def.DeclaredType) : "?")}");
        }

        PrintDiagnostics(diagnostics);

        return diagnostics.HasErrors ? 1 : 0;
    }

    private static int RunCheck(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("Usage: codex check <file.codex>");
            return 1;
        }

        string filePath = args[0];
        if (!File.Exists(filePath))
        {
            Console.Error.WriteLine($"File not found: {filePath}");
            return 1;
        }

        string content = File.ReadAllText(filePath);
        SourceText source = new SourceText(filePath, content);
        DiagnosticBag diagnostics = new DiagnosticBag();

        Lexer lexer = new Lexer(source, diagnostics);
        IReadOnlyList<Token> tokens = lexer.TokenizeAll();

        Parser parser = new Parser(tokens, diagnostics);
        DocumentNode document = parser.ParseDocument();

        Desugarer desugarer = new Desugarer(diagnostics);
        string moduleName = Path.GetFileNameWithoutExtension(filePath);
        Module module = desugarer.Desugar(document, moduleName);

        if (diagnostics.HasErrors)
        {
            PrintDiagnostics(diagnostics);
            return 1;
        }

        // Name resolution
        NameResolver resolver = new NameResolver(diagnostics);
        ResolvedModule resolved = resolver.Resolve(module);

        if (diagnostics.HasErrors)
        {
            PrintDiagnostics(diagnostics);
            return 1;
        }

        // Type checking
        TypeChecker checker = new TypeChecker(diagnostics);
        ImmutableDictionary<string, CodexType> types = checker.CheckModule(resolved.Module);

        if (!diagnostics.HasErrors)
        {
            Console.WriteLine($"✓ {module.Name}: {module.Definitions.Count} definition(s), no errors.");
            foreach (KeyValuePair<string, CodexType> kv in types)
            {
                Console.WriteLine($"  {kv.Key} : {kv.Value}");
            }
        }

        PrintDiagnostics(diagnostics);
        return diagnostics.HasErrors ? 1 : 0;
    }

    private static int RunBuild(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("Usage: codex build <file.codex>");
            return 1;
        }

        string filePath = args[0];
        CompilationResult? result = CompileFile(filePath);
        if (result is null) return 1;

        string outputPath = Path.ChangeExtension(filePath, ".cs");
        File.WriteAllText(outputPath, result.CSharpSource);
        Console.WriteLine($"✓ Compiled to {outputPath}");
        foreach (KeyValuePair<string, CodexType> kv in result.Types)
        {
            Console.WriteLine($"  {kv.Key} : {kv.Value}");
        }
        return 0;
    }

    private static int RunRun(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("Usage: codex run <file.codex>");
            return 1;
        }

        string filePath = args[0];
        CompilationResult? result = CompileFile(filePath);
        if (result is null) return 1;

        // Write the generated C# to a temp directory and compile+run it
        string tempDir = Path.Combine(Path.GetTempPath(), "codex_run_" + Guid.NewGuid().ToString("N")[..8]);
        Directory.CreateDirectory(tempDir);

        try
        {
            string csFile = Path.Combine(tempDir, "Program.cs");
            File.WriteAllText(csFile, result.CSharpSource);

            string csproj = Path.Combine(tempDir, "CodexOutput.csproj");
            File.WriteAllText(csproj, GenerateCsproj());

            // Build
            System.Diagnostics.ProcessStartInfo buildInfo = new("dotnet", "build --nologo --verbosity quiet")
            {
                WorkingDirectory = tempDir,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            System.Diagnostics.Process? buildProc = System.Diagnostics.Process.Start(buildInfo);
            if (buildProc is null)
            {
                Console.Error.WriteLine("Failed to start dotnet build");
                return 1;
            }

            string buildStdout = buildProc.StandardOutput.ReadToEnd();
            string buildStderr = buildProc.StandardError.ReadToEnd();
            buildProc.WaitForExit();

            if (buildProc.ExitCode != 0)
            {
                Console.Error.WriteLine("C# compilation failed:");
                Console.Error.WriteLine(buildStdout);
                Console.Error.WriteLine(buildStderr);
                return 1;
            }

            // Run
            System.Diagnostics.ProcessStartInfo runInfo = new("dotnet", "run --no-build --nologo")
            {
                WorkingDirectory = tempDir,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            System.Diagnostics.Process? runProc = System.Diagnostics.Process.Start(runInfo);
            if (runProc is null)
            {
                Console.Error.WriteLine("Failed to start dotnet run");
                return 1;
            }

            string output = runProc.StandardOutput.ReadToEnd();
            string errOutput = runProc.StandardError.ReadToEnd();
            runProc.WaitForExit();

            if (output.Length > 0)
                Console.Write(output);
            if (errOutput.Length > 0)
                Console.Error.Write(errOutput);

            return runProc.ExitCode;
        }
        finally
        {
            try { Directory.Delete(tempDir, true); } catch { /* best effort cleanup */ }
        }
    }

    /// <summary>
    /// Full compilation pipeline: source → lex → parse → desugar → resolve → typecheck → lower → emit.
    /// Returns null on failure.
    /// </summary>
    private static CompilationResult? CompileFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Console.Error.WriteLine($"File not found: {filePath}");
            return null;
        }

        string content = File.ReadAllText(filePath);
        SourceText source = new SourceText(filePath, content);
        DiagnosticBag diagnostics = new DiagnosticBag();

        // Lex
        Lexer lexer = new Lexer(source, diagnostics);
        IReadOnlyList<Token> tokens = lexer.TokenizeAll();

        // Parse
        Parser parser = new Parser(tokens, diagnostics);
        DocumentNode document = parser.ParseDocument();

        // Desugar
        Desugarer desugarer = new Desugarer(diagnostics);
        string moduleName = Path.GetFileNameWithoutExtension(filePath);
        Module module = desugarer.Desugar(document, moduleName);

        if (diagnostics.HasErrors) { PrintDiagnostics(diagnostics); return null; }

        // Name resolution
        NameResolver resolver = new NameResolver(diagnostics);
        ResolvedModule resolved = resolver.Resolve(module);

        if (diagnostics.HasErrors) { PrintDiagnostics(diagnostics); return null; }

        // Type checking
        TypeChecker checker = new TypeChecker(diagnostics);
        ImmutableDictionary<string, CodexType> types = checker.CheckModule(resolved.Module);

        if (diagnostics.HasErrors) { PrintDiagnostics(diagnostics); return null; }

        // Lower to IR
        Lowering lowering = new Lowering(types, diagnostics);
        IRModule irModule = lowering.Lower(resolved.Module);

        if (diagnostics.HasErrors) { PrintDiagnostics(diagnostics); return null; }

        // Emit C#
        CSharpEmitter emitter = new CSharpEmitter();
        string csharpSource = emitter.Emit(irModule);

        return new CompilationResult(csharpSource, types);
    }

    private static string GenerateCsproj()
    {
        return """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <OutputType>Exe</OutputType>
                <TargetFramework>net8.0</TargetFramework>
                <ImplicitUsings>enable</ImplicitUsings>
                <Nullable>enable</Nullable>
              </PropertyGroup>
            </Project>
            """;
    }

    private static int RunVersion()
    {
        Console.WriteLine("Codex 0.1.0-bootstrap");
        Console.WriteLine("The beginning.");
        return 0;
    }

    private static int RunHelp()
    {
        PrintUsage();
        return 0;
    }

    private static int UnknownCommand(string command)
    {
        Console.Error.WriteLine($"Unknown command: {command}");
        PrintUsage();
        return 1;
    }

    private static void PrintUsage()
    {
        Console.WriteLine("Codex — A language for the rest of human time");
        Console.WriteLine();
        Console.WriteLine("Usage: codex <command> [arguments]");
        Console.WriteLine();
        Console.WriteLine("Commands:");
        Console.WriteLine("  parse <file>    Lex, parse, and display the structure of a Codex file");
        Console.WriteLine("  check <file>    Parse and type-check a Codex file");
        Console.WriteLine("  build <file>    Compile a Codex file to C#");
        Console.WriteLine("  run <file>      Compile and execute a Codex file");
        Console.WriteLine("  version         Display the Codex version");
        Console.WriteLine("  --help, -h      Display this help message");
    }

    private static void PrintDiagnostics(DiagnosticBag diagnostics)
    {
        foreach (Diagnostic diag in diagnostics.ToImmutable())
        {
            string severity = diag.Severity switch
            {
                DiagnosticSeverity.Error => "error",
                DiagnosticSeverity.Warning => "warning",
                DiagnosticSeverity.Info => "info",
                DiagnosticSeverity.Hint => "hint",
                _ => "?"
            };
            Console.Error.WriteLine($"{severity} {diag.Code}: {diag.Message} {diag.Span}");
        }
    }

    private static string FormatType(TypeNode node) => node switch
    {
        NamedTypeNode n => n.Name.Text,
        FunctionTypeNode f => $"{FormatType(f.Parameter)} → {FormatType(f.Return)}",
        ApplicationTypeNode a => $"{FormatType(a.Constructor)} {string.Join(" ", a.Arguments.Select(FormatType))}",
        ParenthesizedTypeNode p => $"({FormatType(p.Inner)})",
        _ => "?"
    };

    private static string FormatTypeExpr(TypeExpr node) => node switch
    {
        NamedTypeExpr n => n.Name.Value,
        FunctionTypeExpr f => $"{FormatTypeExpr(f.Parameter)} → {FormatTypeExpr(f.Return)}",
        AppliedTypeExpr a => $"{FormatTypeExpr(a.Constructor)} {string.Join(" ", a.Arguments.Select(FormatTypeExpr))}",
        _ => "?"
    };

    private sealed record CompilationResult(
        string CSharpSource,
        ImmutableDictionary<string, CodexType> Types);
}
