using System.Collections.Concurrent;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using Codex.Core;
using Codex.Syntax;
using Codex.Ast;
using Codex.Types;

namespace Codex.Cli;

public static partial class Program
{
    sealed class BuildManifest
    {
        public Dictionary<string, FileEntry> Files { get; set; } = [];
        public string? OutputHash { get; set; }
        public long LastBuildMs { get; set; }
    }

    sealed class FileEntry
    {
        public string Hash { get; set; } = "";
        public long LastModifiedTicks { get; set; }
    }

    sealed class PerFileFrontEndResult
    {
        public required string FilePath { get; init; }
        public required List<Definition> Definitions { get; init; }
        public required List<TypeDef> TypeDefinitions { get; init; }
        public required List<ClaimDef> Claims { get; init; }
        public required List<ProofDef> Proofs { get; init; }
    }

    static string ManifestPath(string directory) =>
        Path.Combine(directory, ".codex-build", "manifest.json");

    static BuildManifest LoadManifest(string directory)
    {
        string path = ManifestPath(directory);
        if (!File.Exists(path)) return new BuildManifest();
        string json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<BuildManifest>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new BuildManifest();
    }

    static void SaveManifest(string directory, BuildManifest manifest)
    {
        string buildDir = Path.Combine(directory, ".codex-build");
        if (!Directory.Exists(buildDir))
            Directory.CreateDirectory(buildDir);
        string json = JsonSerializer.Serialize(manifest, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(ManifestPath(directory), json);
    }

    static string ComputeFileHash(string filePath)
    {
        byte[] bytes = File.ReadAllBytes(filePath);
        byte[] hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash);
    }

    static bool IsFileChanged(string filePath, BuildManifest manifest)
    {
        string relativePath = filePath;
        if (!manifest.Files.TryGetValue(relativePath, out FileEntry? entry))
            return true;

        long ticks = File.GetLastWriteTimeUtc(filePath).Ticks;
        if (ticks != entry.LastModifiedTicks)
            return true;

        string hash = ComputeFileHash(filePath);
        return hash != entry.Hash;
    }

    static int RunIncrementalBuild(string directory, string target, string[] allFiles, string outputPath)
    {
        Stopwatch sw = Stopwatch.StartNew();
        BuildManifest manifest = LoadManifest(directory);

        List<string> changedFiles = [];
        List<string> unchangedFiles = [];
        foreach (string file in allFiles)
        {
            if (IsFileChanged(file, manifest))
                changedFiles.Add(file);
            else
                unchangedFiles.Add(file);
        }

        if (changedFiles.Count == 0 && File.Exists(outputPath))
        {
            sw.Stop();
            Console.WriteLine($"✓ Up to date ({allFiles.Length} file(s), {sw.ElapsedMilliseconds}ms)");
            return 0;
        }

        int changed = changedFiles.Count;
        int total = allFiles.Length;
        Console.WriteLine($"  Changed: {changed}/{total} file(s)");

        DiagnosticBag diagnostics = new();
        ConcurrentBag<PerFileFrontEndResult> results = [];

        Parallel.ForEach(allFiles, filePath =>
        {
            PerFileFrontEndResult? result = RunFrontEnd(filePath, diagnostics);
            if (result is not null)
                results.Add(result);
        });

        if (diagnostics.HasErrors)
        {
            PrintDiagnostics(diagnostics);
            return 1;
        }

        List<Definition> allDefinitions = [];
        List<TypeDef> allTypeDefinitions = [];
        List<ClaimDef> allClaims = [];
        List<ProofDef> allProofs = [];

        List<PerFileFrontEndResult> orderedResults = results
            .OrderBy(r => r.FilePath, StringComparer.Ordinal)
            .ToList();

        foreach (PerFileFrontEndResult r in orderedResults)
        {
            allDefinitions.AddRange(r.Definitions);
            allTypeDefinitions.AddRange(r.TypeDefinitions);
            allClaims.AddRange(r.Claims);
            allProofs.AddRange(r.Proofs);
        }

        string moduleName = Path.GetFileNameWithoutExtension(
            Path.GetFileName(outputPath).Replace(Path.GetExtension(outputPath), ""));

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

        IRCompilationResult? irResult = RunBackEnd(combined, diagnostics);
        if (irResult is null) return 1;

        Codex.Emit.ICodeEmitter emitter = CreateEmitter(target);
        string output = emitter.Emit(irResult.Module);
        File.WriteAllText(outputPath, output);

        BuildManifest newManifest = new()
        {
            OutputHash = Convert.ToHexString(SHA256.HashData(
                System.Text.Encoding.UTF8.GetBytes(output))),
            LastBuildMs = sw.ElapsedMilliseconds
        };
        foreach (string file in allFiles)
        {
            newManifest.Files[file] = new FileEntry
            {
                Hash = ComputeFileHash(file),
                LastModifiedTicks = File.GetLastWriteTimeUtc(file).Ticks
            };
        }
        SaveManifest(directory, newManifest);

        sw.Stop();
        Console.WriteLine($"✓ Compiled to {outputPath} ({target}, {sw.ElapsedMilliseconds}ms)");
        foreach (KeyValuePair<string, CodexType> kv in irResult.Types)
            Console.WriteLine($"  {kv.Key} : {kv.Value}");
        return 0;
    }

    static int RunParallelMultiTargetBuild(
        string directory, string[] targets, string[] allFiles, string outputDir, string moduleName)
    {
        Stopwatch sw = Stopwatch.StartNew();

        DiagnosticBag diagnostics = new();
        ConcurrentBag<PerFileFrontEndResult> results = [];

        Parallel.ForEach(allFiles, filePath =>
        {
            PerFileFrontEndResult? result = RunFrontEnd(filePath, diagnostics);
            if (result is not null)
                results.Add(result);
        });

        if (diagnostics.HasErrors)
        {
            PrintDiagnostics(diagnostics);
            return 1;
        }

        List<PerFileFrontEndResult> orderedResults = results
            .OrderBy(r => r.FilePath, StringComparer.Ordinal)
            .ToList();

        List<Definition> allDefinitions = [];
        List<TypeDef> allTypeDefinitions = [];
        List<ClaimDef> allClaims = [];
        List<ProofDef> allProofs = [];
        foreach (PerFileFrontEndResult r in orderedResults)
        {
            allDefinitions.AddRange(r.Definitions);
            allTypeDefinitions.AddRange(r.TypeDefinitions);
            allClaims.AddRange(r.Claims);
            allProofs.AddRange(r.Proofs);
        }

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

        IRCompilationResult? irResult = RunBackEnd(combined, diagnostics);
        if (irResult is null) return 1;

        if (!Directory.Exists(outputDir))
            Directory.CreateDirectory(outputDir);

        ConcurrentBag<string> emitResults = [];

        Parallel.ForEach(targets, target =>
        {
            Codex.Emit.ICodeEmitter emitter = CreateEmitter(target);
            string output = emitter.Emit(irResult.Module);
            string outputPath = Path.Combine(outputDir, moduleName + emitter.FileExtension);
            File.WriteAllText(outputPath, output);
            emitResults.Add($"  ✓ {outputPath} ({target})");
        });

        sw.Stop();
        Console.WriteLine($"✓ Built {targets.Length} target(s) in {sw.ElapsedMilliseconds}ms");
        foreach (string line in emitResults.OrderBy(s => s))
            Console.WriteLine(line);
        foreach (KeyValuePair<string, CodexType> kv in irResult.Types)
            Console.WriteLine($"  {kv.Key} : {kv.Value}");
        return 0;
    }

    static PerFileFrontEndResult? RunFrontEnd(string filePath, DiagnosticBag diagnostics)
    {
        string content = File.ReadAllText(filePath);
        SourceText source = new(filePath, content);

        DiagnosticBag localDiag = new();
        DocumentNode document = ParseSourceFile(source, content, localDiag);

        Desugarer desugarer = new(localDiag);
        string fileModule = Path.GetFileNameWithoutExtension(filePath);
        Module module = desugarer.Desugar(document, fileModule);

        foreach (Diagnostic d in localDiag.ToImmutable())
            diagnostics.Add(d);

        if (localDiag.HasErrors) return null;

        return new PerFileFrontEndResult
        {
            FilePath = filePath,
            Definitions = module.Definitions.ToList(),
            TypeDefinitions = module.TypeDefinitions.ToList(),
            Claims = module.Claims.ToList(),
            Proofs = module.Proofs.ToList()
        };
    }

    static IRCompilationResult? RunBackEnd(Module combined, DiagnosticBag diagnostics)
    {
        Codex.Semantics.NameResolver resolver = new(diagnostics);
        Codex.Semantics.ResolvedModule resolved = resolver.Resolve(combined);
        if (diagnostics.HasErrors) { PrintDiagnostics(diagnostics); return null; }

        Codex.Types.TypeChecker checker = new(diagnostics);
        Map<string, CodexType> types = checker.CheckModule(resolved.Module);
        if (diagnostics.HasErrors) { PrintDiagnostics(diagnostics); return null; }

        Codex.Types.LinearityChecker linearityChecker = new(diagnostics, types);
        linearityChecker.CheckModule(resolved.Module);
        if (diagnostics.HasErrors) { PrintDiagnostics(diagnostics); return null; }

        Codex.Proofs.ProofChecker proofChecker = new(diagnostics);
        proofChecker.CheckModule(resolved.Module, types);
        if (diagnostics.HasErrors) { PrintDiagnostics(diagnostics); return null; }

        Codex.IR.Lowering lowering = new(types, checker.ConstructorMap, checker.TypeDefMap, diagnostics);
        Codex.IR.IRModule irModule = lowering.Lower(resolved.Module);
        if (diagnostics.HasErrors) { PrintDiagnostics(diagnostics); return null; }

        return new IRCompilationResult(irModule, types);
    }
}
