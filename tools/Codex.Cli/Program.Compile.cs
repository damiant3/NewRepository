using Codex.Core;
using Codex.Syntax;
using Codex.Ast;
using Codex.Semantics;
using Codex.Types;
using Codex.IR;
using Codex.Emit.CSharp;

namespace Codex.Cli;

public static partial class Program
{
    sealed record CompilationResult(
        string CSharpSource,
        Map<string, CodexType> Types);

    sealed record IRCompilationResult(
        IRModule Module,
        Map<string, CodexType> Types);

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

        NameResolver resolver = CreateResolver(diagnostics,
            Path.GetDirectoryName(Path.GetFullPath(filePath)));
        ResolvedModule resolved = resolver.Resolve(module);

        if (diagnostics.HasErrors) { PrintDiagnostics(diagnostics); return null; }

        TypeChecker checker = new(diagnostics);
        Map<string, CodexType> types = checker.CheckModule(resolved.Module);

        // Register imported type definitions
        foreach (ResolvedModule imported in resolved.ImportedModules)
        {
            TypeChecker importChecker = new(diagnostics);
            importChecker.CheckModule(imported.Module);
        }

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
            : SourceSpan.Single(0, 1, 1, "<combined>");
        Module combined = new(
            QualifiedName.Simple(moduleName),
            allDefinitions,
            allTypeDefinitions,
            allClaims,
            allProofs,
            combinedSpan);

        string? baseDir = filePaths.Length > 0
            ? Path.GetDirectoryName(Path.GetFullPath(filePaths[0]))
            : null;
        NameResolver resolver = CreateResolver(diagnostics, baseDir);
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
            foreach (SourceSpan related in diag.RelatedSpans)
            {
                Console.Error.WriteLine($"  note: see {related}");
            }
        }
    }

    static NameResolver CreateResolver(DiagnosticBag diagnostics, string? baseDirectory = null)
    {
        string dir = baseDirectory ?? Directory.GetCurrentDirectory();
        List<IModuleLoader> loaders = [new FileModuleLoader(dir, diagnostics)];

        Codex.Repository.FactStore? store =
            Codex.Repository.FactStore.Open(Directory.GetCurrentDirectory());
        if (store is not null)
            loaders.Add(new RepositoryModuleLoader(store, diagnostics));

        return new NameResolver(diagnostics, new CompositeModuleLoader([.. loaders]));
    }
}
