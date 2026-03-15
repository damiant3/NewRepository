using Codex.Core;
using Codex.Syntax;
using Codex.Ast;
using Codex.Semantics;
using Codex.Types;
using Codex.IR;
using Codex.Emit.CSharp;
using Codex.Repository;

namespace Codex.Cli;

public static partial class Program  // this file is locked.  use a partial.
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
            "read" => RunRead(args.Skip(1).ToArray()),
            "init" => RunInit(args.Skip(1).ToArray()),
            "publish" => RunPublish(args.Skip(1).ToArray()),
            "history" => RunHistory(args.Skip(1).ToArray()),
            "propose" => RunPropose(args.Skip(1).ToArray()),
            "verdict" => RunVerdict(args.Skip(1).ToArray()),
            "proposals" => RunProposals(args.Skip(1).ToArray()),
            "vouch" => RunVouch(args.Skip(1).ToArray()),
            "sync" => RunSync(args.Skip(1).ToArray()),
            "version" => RunVersion(),
            "--help" or "-h" => RunHelp(),
            _ => UnknownCommand(command)
        };
    }

    static int RunParse(string[] args)
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
        SourceText source = new(filePath, content);
        DiagnosticBag diagnostics = new();

        DocumentNode document;
        if (ProseParser.IsProseDocument(content))
        {
            Console.WriteLine("(prose-mode document detected)");
            ProseParser proseParser = new(source, diagnostics);
            document = proseParser.ParseDocument();

            Console.WriteLine("\n=== Chapters ===");
            foreach (ChapterNode chapter in document.Chapters)
            {
                Console.WriteLine($"  Chapter: {chapter.Title}");
                PrintMembers(chapter.Members, "    ");
            }
        }
        else
        {
            Lexer lexer = new(source, diagnostics);
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

            Parser parser = new(tokens, diagnostics);
            document = parser.ParseDocument();
        }

        Console.WriteLine("\n=== Definitions ===");
        foreach (DefinitionNode def in document.Definitions)
        {
            string typeStr = def.TypeAnnotation is not null ? $" : {FormatType(def.TypeAnnotation.Type)}" : "";
            string paramsStr = def.Parameters.Count > 0
                ? " (" + string.Join(") (", def.Parameters.Select(p => p.Text)) + ")"
                : "";
            Console.WriteLine($"  {def.Name.Text}{paramsStr}{typeStr}");
        }

        Desugarer desugarer = new(diagnostics);
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

    static int RunCheck(string[] args)
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
        SourceText source = new(filePath, content);
        DiagnosticBag diagnostics = new();

        DocumentNode document = ParseSourceFile(source, content, diagnostics);

        Desugarer desugarer = new(diagnostics);
        string moduleName = Path.GetFileNameWithoutExtension(filePath);
        Module module = desugarer.Desugar(document, moduleName);

        if (diagnostics.HasErrors)
        {
            PrintDiagnostics(diagnostics);
            return 1;
        }

        NameResolver resolver = new(diagnostics);
        ResolvedModule resolved = resolver.Resolve(module);

        if (diagnostics.HasErrors)
        {
            PrintDiagnostics(diagnostics);
            return 1;
        }

        TypeChecker checker = new(diagnostics);
        Map<string, CodexType> types = checker.CheckModule(resolved.Module);

        if (diagnostics.HasErrors) { PrintDiagnostics(diagnostics); return 1; }

        LinearityChecker linearityChecker = new(diagnostics, types);
        linearityChecker.CheckModule(resolved.Module);

        if (diagnostics.HasErrors) { PrintDiagnostics(diagnostics); return 1; }

        Codex.Proofs.ProofChecker proofChecker = new(diagnostics);
        proofChecker.CheckModule(resolved.Module, types);

        if (diagnostics.HasErrors) { PrintDiagnostics(diagnostics); return 1; }

        if (!diagnostics.HasErrors)
        {
            int claimCount = resolved.Module.Claims.Count;
            int proofCount = resolved.Module.Proofs.Count;
            string proofInfo = claimCount > 0 ? $", {claimCount} claim(s), {proofCount} proof(s)" : "";
            Console.WriteLine($"✓ {module.Name}: {module.Definitions.Count} definition(s){proofInfo}, no errors.");
            foreach (KeyValuePair<string, CodexType> kv in types)
            {
                Console.WriteLine($"  {kv.Key} : {kv.Value}");
            }
        }

        PrintDiagnostics(diagnostics);
        return diagnostics.HasErrors ? 1 : 0;
    }

    static int RunBuild(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("Usage: codex build <file.codex|directory> [--target cs|js|rust]");
            return 1;
        }

        string filePath = args[0];
        string target = "cs";
        for (int i = 1; i < args.Length; i++)
        {
            if (args[i] == "--target" && i + 1 < args.Length)
                target = args[++i].ToLowerInvariant();
        }

        IRCompilationResult? irResult;
        string outputPath;

        if (Directory.Exists(filePath))
        {
            string[] files = Directory.GetFiles(filePath, "*.codex", SearchOption.AllDirectories);
            if (files.Length == 0)
            {
                Console.Error.WriteLine($"No .codex files found in {filePath}");
                return 1;
            }
            Array.Sort(files, StringComparer.Ordinal);
            irResult = CompileMultipleToIR(files, Path.GetFileName(Path.GetFullPath(filePath)));
            if (irResult is null) return 1;

            Codex.Emit.ICodeEmitter dirEmitter = target switch
            {
                "js" or "javascript" => new Codex.Emit.JavaScript.JavaScriptEmitter(),
                "rust" or "rs" => new Codex.Emit.Rust.RustEmitter(),
                "python" or "py" => new Codex.Emit.Python.PythonEmitter(),
                "cpp" or "c++" => new Codex.Emit.Cpp.CppEmitter(),
                _ => new CSharpEmitter()
            };
            string output = dirEmitter.Emit(irResult.Module);
            outputPath = Path.Combine(filePath, "output" + dirEmitter.FileExtension);
            File.WriteAllText(outputPath, output);
        }
        else
        {
            irResult = CompileToIR(filePath);
            if (irResult is null) return 1;

            Codex.Emit.ICodeEmitter emitter = target switch
            {
                "js" or "javascript" => new Codex.Emit.JavaScript.JavaScriptEmitter(),
                "rust" or "rs" => new Codex.Emit.Rust.RustEmitter(),
                "python" or "py" => new Codex.Emit.Python.PythonEmitter(),
                "cpp" or "c++" => new Codex.Emit.Cpp.CppEmitter(),
                _ => new CSharpEmitter()
            };
            string output = emitter.Emit(irResult.Module);
            outputPath = Path.ChangeExtension(filePath, emitter.FileExtension);
            File.WriteAllText(outputPath, output);
        }

        Console.WriteLine($"✓ Compiled to {outputPath} ({target})");
        foreach (KeyValuePair<string, CodexType> kv in irResult.Types)
        {
            Console.WriteLine($"  {kv.Key} : {kv.Value}");
        }
        return 0;
    }

    static int RunRun(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("Usage: codex run <file.codex>");
            return 1;
        }

        string filePath = args[0];
        CompilationResult? result = CompileFile(filePath);
        if (result is null) return 1;

        string tempDir = Path.Combine(Path.GetTempPath(), "codex_run_" + Guid.NewGuid().ToString("N")[..8]);
        Directory.CreateDirectory(tempDir);

        try
        {
            string csFile = Path.Combine(tempDir, "Program.cs");
            File.WriteAllText(csFile, result.CSharpSource);

            string csproj = Path.Combine(tempDir, "CodexOutput.csproj");
            File.WriteAllText(csproj, GenerateCsproj());

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

    static int RunRead(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("Usage: codex read <file.codex>");
            return 1;
        }

        string filePath = args[0];
        if (!File.Exists(filePath))
        {
            Console.Error.WriteLine($"File not found: {filePath}");
            return 1;
        }

        string content = File.ReadAllText(filePath);
        SourceText source = new(filePath, content);
        DiagnosticBag diagnostics = new();

        if (!ProseParser.IsProseDocument(content))
        {
            Console.Error.WriteLine("Not a prose-mode document. Use 'codex parse' for notation-only files.");
            return 1;
        }

        ProseParser proseParser = new(source, diagnostics);
        DocumentNode document = proseParser.ParseDocument();

        if (diagnostics.HasErrors)
        {
            PrintDiagnostics(diagnostics);
            return 1;
        }

        foreach (ChapterNode chapter in document.Chapters)
        {
            Console.WriteLine($"═══ {chapter.Title} ═══");
            Console.WriteLine();
            RenderMembers(chapter.Members, "  ");
        }

        return 0;
    }

    static void RenderMembers(IReadOnlyList<DocumentMember> members, string indent)
    {
        foreach (DocumentMember member in members)
        {
            switch (member)
            {
                case ProseBlockNode prose:
                    foreach (string line in prose.Text.Split('\n'))
                    {
                        Console.WriteLine($"{indent}{line}");
                    }
                    Console.WriteLine();
                    break;

                case NotationBlockNode notation:
                    foreach (DefinitionNode def in notation.Definitions)
                    {
                        string typeStr = def.TypeAnnotation is not null
                            ? $" : {FormatType(def.TypeAnnotation.Type)}"
                            : "";
                        string paramsStr = def.Parameters.Count > 0
                            ? " (" + string.Join(") (", def.Parameters.Select(p => p.Text)) + ")"
                            : "";
                        Console.WriteLine($"{indent}  {def.Name.Text}{paramsStr}{typeStr}");
                    }
                    Console.WriteLine();
                    break;

                case SectionNode section:
                    Console.WriteLine($"{indent}--- {section.Title} ---");
                    Console.WriteLine();
                    RenderMembers(section.Members, indent + "  ");
                    break;
            }
        }
    }

    static CompilationResult? CompileFile(string filePath)
    {
        IRCompilationResult? irResult = CompileToIR(filePath);
        if (irResult is null) return null;

        CSharpEmitter emitter = new();
        string csharpSource = emitter.Emit(irResult.Module);
        return new CompilationResult(csharpSource, irResult.Types);
    }

    static IRCompilationResult? CompileToIR(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Console.Error.WriteLine($"File not found: {filePath}");
            return null;
        }

        string content = File.ReadAllText(filePath);
        SourceText source = new(filePath, content);
        DiagnosticBag diagnostics = new();

        DocumentNode document = ParseSourceFile(source, content, diagnostics);

        Desugarer desugarer = new(diagnostics);
        string moduleName = Path.GetFileNameWithoutExtension(filePath);
        Module module = desugarer.Desugar(document, moduleName);

        if (diagnostics.HasErrors) { PrintDiagnostics(diagnostics); return null; }

        NameResolver resolver = new(diagnostics);
        ResolvedModule resolved = resolver.Resolve(module);

        if (diagnostics.HasErrors) { PrintDiagnostics(diagnostics); return null; }

        TypeChecker checker = new(diagnostics);
        Map<string, CodexType> types = checker.CheckModule(resolved.Module);

        if (diagnostics.HasErrors) { PrintDiagnostics(diagnostics); return null; }

        LinearityChecker linearityChecker = new(diagnostics, types);
        linearityChecker.CheckModule(resolved.Module);

        if (diagnostics.HasErrors) { PrintDiagnostics(diagnostics); return null; }

        Codex.Proofs.ProofChecker proofChecker = new(diagnostics);
        proofChecker.CheckModule(resolved.Module, types);

        if (diagnostics.HasErrors) { PrintDiagnostics(diagnostics); return null; }

        Lowering lowering = new(types, checker.ConstructorMap, checker.TypeDefMap, diagnostics);
        IRModule irModule = lowering.Lower(resolved.Module);

        if (diagnostics.HasErrors) { PrintDiagnostics(diagnostics); return null; }

        return new IRCompilationResult(irModule, types);
    }

    static IRCompilationResult? CompileMultipleToIR(string[] filePaths, string moduleName)
    {
        DiagnosticBag diagnostics = new();
        Desugarer desugarer = new(diagnostics);
        List<Definition> allDefinitions = [];
        List<TypeDef> allTypeDefinitions = [];
        List<ClaimDef> allClaims = [];
        List<ProofDef> allProofs = [];

        foreach (string filePath in filePaths)
        {
            if (!File.Exists(filePath))
            {
                Console.Error.WriteLine($"File not found: {filePath}");
                return null;
            }

            string content = File.ReadAllText(filePath);
            SourceText source = new(filePath, content);
            DocumentNode document = ParseSourceFile(source, content, diagnostics);
            string fileModule = Path.GetFileNameWithoutExtension(filePath);
            Module module = desugarer.Desugar(document, fileModule);

            allDefinitions.AddRange(module.Definitions);
            allTypeDefinitions.AddRange(module.TypeDefinitions);
            allClaims.AddRange(module.Claims);
            allProofs.AddRange(module.Proofs);
        }

        if (diagnostics.HasErrors) { PrintDiagnostics(diagnostics); return null; }

        SourceSpan combinedSpan = allDefinitions.Count > 0
            ? allDefinitions[0].Span
            : SourceSpan.Single(0, 1, 1);
        Module combined = new(
            QualifiedName.Simple(moduleName),
            allDefinitions,
            allTypeDefinitions,
            allClaims,
            allProofs,
            combinedSpan);

        NameResolver resolver = new(diagnostics);
        ResolvedModule resolved = resolver.Resolve(combined);

        if (diagnostics.HasErrors) { PrintDiagnostics(diagnostics); return null; }

        TypeChecker checker = new(diagnostics);
        Map<string, CodexType> types = checker.CheckModule(resolved.Module);

        if (diagnostics.HasErrors) { PrintDiagnostics(diagnostics); return null; }

        LinearityChecker linearityChecker = new(diagnostics, types);
        linearityChecker.CheckModule(resolved.Module);

        if (diagnostics.HasErrors) { PrintDiagnostics(diagnostics); return null; }

        Codex.Proofs.ProofChecker proofChecker = new(diagnostics);
        proofChecker.CheckModule(resolved.Module, types);

        if (diagnostics.HasErrors) { PrintDiagnostics(diagnostics); return null; }

        Lowering lowering = new(types, checker.ConstructorMap, checker.TypeDefMap, diagnostics);
        IRModule irModule = lowering.Lower(resolved.Module);

        if (diagnostics.HasErrors) { PrintDiagnostics(diagnostics); return null; }

        return new IRCompilationResult(irModule, types);
    }

    static string GenerateCsproj()
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

    static int RunVersion()
    {
        Console.WriteLine("Codex 0.1.0-bootstrap");
        Console.WriteLine("The beginning.");
        return 0;
    }

    static int RunHelp()
    {
        PrintUsage();
        return 0;
    }

    static int UnknownCommand(string command)
    {
        Console.Error.WriteLine($"Unknown command: {command}");
        PrintUsage();
        return 1;
    }

    static int RunInit(string[] args)
    {
        string dir = args.Length > 0 ? args[0] : Directory.GetCurrentDirectory();
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        FactStore store = FactStore.Init(dir);
        Console.WriteLine($"✓ Initialized Codex repository in {Path.GetFullPath(dir)}");
        return 0;
    }

    static int RunPublish(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("Usage: codex publish <file.codex>");
            return 1;
        }

        string filePath = args[0];
        if (!File.Exists(filePath))
        {
            Console.Error.WriteLine($"File not found: {filePath}");
            return 1;
        }

        string repoDir = FindRepositoryRoot(Directory.GetCurrentDirectory());
        if (repoDir == "")
        {
            Console.Error.WriteLine("No Codex repository found. Run 'codex init' first.");
            return 1;
        }

        FactStore? store = FactStore.Open(repoDir);
        if (store is null)
        {
            Console.Error.WriteLine("Failed to open Codex repository.");
            return 1;
        }

        CompilationResult? result = CompileFile(filePath);
        if (result is null)
        {
            Console.Error.WriteLine("Compilation failed. Fix errors before publishing.");
            return 1;
        }

        string source = File.ReadAllText(filePath);
        string moduleName = Path.GetFileNameWithoutExtension(filePath);
        string author = Environment.UserName;
        string justification = args.Length > 1 ? args[1] : "Published from CLI";

        ContentHash? existing = store.LookupView(moduleName);

        Fact fact = Fact.CreateDefinition(source, author, justification);
        ContentHash hash = store.Store(fact);

        if (existing is not null && !existing.Value.Equals(hash))
        {
            Fact supersession = Fact.CreateSupersession(hash, existing.Value, author,
                $"Updated {moduleName}");
            store.Store(supersession);
        }

        store.UpdateView(moduleName, hash);

        Console.WriteLine($"✓ Published {moduleName} ({hash})");
        foreach (KeyValuePair<string, CodexType> kv in result.Types)
        {
            Console.WriteLine($"  {kv.Key} : {kv.Value}");
        }

        return 0;
    }

    static int RunHistory(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("Usage: codex history <name>");
            return 1;
        }

        string name = args[0];
        string repoDir = FindRepositoryRoot(Directory.GetCurrentDirectory());
        if (repoDir == "")
        {
            Console.Error.WriteLine("No Codex repository found. Run 'codex init' first.");
            return 1;
        }

        FactStore? store = FactStore.Open(repoDir);
        if (store is null)
        {
            Console.Error.WriteLine("Failed to open Codex repository.");
            return 1;
        }

        ContentHash? current = store.LookupView(name);
        if (current is null)
        {
            Console.Error.WriteLine($"No published definition found for '{name}'.");
            return 1;
        }

        IReadOnlyList<Fact> history = store.GetHistory(name);
        Console.WriteLine($"History of '{name}':");
        Console.WriteLine();
        for (int i = 0; i < history.Count; i++)
        {
            Fact fact = history[i];
            string marker = i == 0 ? " (current)" : "";
            Console.WriteLine($"  {fact.Hash}{marker}");
            Console.WriteLine($"    by {fact.Author} at {fact.Timestamp:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"    \"{fact.Justification}\"");
            Console.WriteLine();
        }

        return 0;
    }

    static string FindRepositoryRoot(string startDir)
    {
        string? dir = startDir;
        while (dir is not null)
        {
            if (Directory.Exists(Path.Combine(dir, ".codex")))
                return dir;
            dir = Path.GetDirectoryName(dir);
        }
        return "";
    }

    static void PrintUsage()
    {
        Console.WriteLine("Codex — A language for the rest of human time");
        Console.WriteLine();
        Console.WriteLine("Usage: codex <command> [arguments]");
        Console.WriteLine();
        Console.WriteLine("Commands:");
        Console.WriteLine("  parse <file>      Lex, parse, and display the structure of a Codex file");
        Console.WriteLine("  check <file>      Parse and type-check a Codex file");
        Console.WriteLine("  build <file>      Compile a Codex file (--target cs|js|rust)");
        Console.WriteLine("  run <file>        Compile and execute a Codex file");
        Console.WriteLine("  read <file>       Display a prose-mode document as formatted text");
        Console.WriteLine("  init [dir]        Initialize a Codex repository in the given directory");
        Console.WriteLine("  publish <file>    Publish a .codex file to the local repository");
        Console.WriteLine("  history <name>    Show the history of a published definition");
        Console.WriteLine("  propose <file>    Propose a new definition or change");
        Console.WriteLine("  verdict <hash> <decision>  Post a verdict on a proposal");
        Console.WriteLine("  proposals         List all proposals");
        Console.WriteLine("  vouch <hash> <degree>  Vouch for a fact (trust)");
        Console.WriteLine("  sync <path>       Sync facts with another repository");
        Console.WriteLine("  version           Display the Codex version");
        Console.WriteLine("  --help, -h        Display this help message");
    }

    static void PrintDiagnostics(DiagnosticBag diagnostics)
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

    static string FormatType(TypeNode node) => node switch
    {
        NamedTypeNode n => n.Name.Text,
        FunctionTypeNode f => $"{FormatType(f.Parameter)} → {FormatType(f.Return)}",
        ApplicationTypeNode a => $"{FormatType(a.Constructor)} {string.Join(" ", a.Arguments.Select(FormatType))}",
        ParenthesizedTypeNode p => $"({FormatType(p.Inner)})",
        EffectfulTypeNode e => $"[{string.Join(", ", e.Effects.Select(FormatType))}] {FormatType(e.Return)}",
        LinearTypeNode l => $"linear {FormatType(l.Inner)}",
        DependentTypeNode d => $"({d.ParamName.Text} : {FormatType(d.ParamType)}) → {FormatType(d.Body)}",
        IntegerTypeNode i => i.Literal.Text,
        BinaryTypeNode b => $"({FormatType(b.Left)} {b.Operator.Text} {FormatType(b.Right)})",
        _ => "?"
    };

    static string FormatTypeExpr(TypeExpr node) => node switch
    {
        NamedTypeExpr n => n.Name.Value,
        FunctionTypeExpr f => $"{FormatTypeExpr(f.Parameter)} → {FormatTypeExpr(f.Return)}",
        AppliedTypeExpr a => $"{FormatTypeExpr(a.Constructor)} {string.Join(" ", a.Arguments.Select(FormatTypeExpr))}",
        EffectfulTypeExpr e => $"[{string.Join(", ", e.Effects.Select(FormatTypeExpr))}] {FormatTypeExpr(e.Return)}",
        LinearTypeExpr l => $"linear {FormatTypeExpr(l.Inner)}",
        DependentTypeExpr d => $"({d.ParamName.Value} : {FormatTypeExpr(d.ParamType)}) → {FormatTypeExpr(d.Body)}",
        IntegerLiteralTypeExpr i => i.Value.ToString(),
        BinaryTypeExpr b => $"({FormatTypeExpr(b.Left)} {b.Op} {FormatTypeExpr(b.Right)})",
        _ => "?"
    };

    sealed record CompilationResult(
        string CSharpSource,
        Map<string, CodexType> Types);

    sealed record IRCompilationResult(
        IRModule Module,
        Map<string, CodexType> Types);

    static DocumentNode ParseSourceFile(SourceText source, string content, DiagnosticBag diagnostics)
    {
        if (ProseParser.IsProseDocument(content))
        {
            ProseParser proseParser = new(source, diagnostics);
            return proseParser.ParseDocument();
        }
        else
        {
            Lexer lexer = new(source, diagnostics);
            IReadOnlyList<Token> tokens = lexer.TokenizeAll();
            Parser parser = new(tokens, diagnostics);
            return parser.ParseDocument();
        }
    }

    static void PrintMembers(IReadOnlyList<DocumentMember> members, string indent)
    {
        foreach (DocumentMember member in members)
        {
            switch (member)
            {
                case ProseBlockNode prose:
                    Console.WriteLine($"{indent}[prose] {prose.Text.Split('\n')[0]}...");
                    break;
                case NotationBlockNode notation:
                    Console.WriteLine($"{indent}[notation] {notation.Definitions.Count} definition(s)");
                    foreach (DefinitionNode def in notation.Definitions)
                    {
                        Console.WriteLine($"{indent}  {def.Name.Text}");
                    }
                    break;
                case SectionNode section:
                    Console.WriteLine($"{indent}Section: {section.Title}");
                    PrintMembers(section.Members, indent + "  ");
                    break;
            }
        }
    }
}
