using Codex.Core;
using Codex.Types;
using Codex.Emit.CSharp;

namespace Codex.Cli;

public static partial class Program
{
    static int RunBuild(string[] args)
    {
        if (args.Length == 0)
        {
            if (File.Exists("codex.project.json"))
            {
                return RunBuildProject(".", null, false, null);
            }
            Console.Error.WriteLine("Usage: codex build <file|dir>  Compile a Codex file or project");
            Console.WriteLine("                    --target <t>        Target backend (cs|js|rust|py|cpp|go|java|ada|fortran|cobol|babbage)");
            Console.WriteLine("                    --targets <t1,t2>   Emit to multiple backends in parallel");
            Console.WriteLine("                    --incremental, -i   Skip unchanged files (uses .codex-build/manifest.json)");
            return 1;
        }

        string filePath = args[0];
        string? targetOverride = null;
        string[]? multiTargets = null;
        bool incremental = false;
        for (int i = 1; i < args.Length; i++)
        {
            if (args[i] == "--target" && i + 1 < args.Length)
                targetOverride = args[++i].ToLowerInvariant();
            else if (args[i] == "--targets" && i + 1 < args.Length)
                multiTargets = args[++i].ToLowerInvariant().Split(',', StringSplitOptions.RemoveEmptyEntries);
            else if (args[i] == "--incremental" || args[i] == "-i")
                incremental = true;
        }

        if (filePath == "." || filePath == "./" || filePath == ".\\" 
            || (Directory.Exists(filePath) && File.Exists(Path.Combine(filePath, "codex.project.json"))))
        {
            return RunBuildProject(filePath, targetOverride, incremental, multiTargets);
        }

        if (Directory.Exists(filePath))
        {
            return RunBuildDirectory(filePath, targetOverride ?? "cs");
        }

        return RunBuildFile(filePath, targetOverride ?? "cs");
    }

    static int RunBuildProject(string directory, string? targetOverride, bool incremental, string[]? multiTargets)
    {
        CodexProject? project = LoadProjectFile(directory);
        if (project is null)
        {
            Console.Error.WriteLine($"No codex.project.json found in {Path.GetFullPath(directory)}");
            return 1;
        }

        Console.WriteLine($"Building project: {project.Name} v{project.Version}");

        string[] files = ResolveProjectSources(directory, project);
        if (files.Length == 0)
        {
            Console.Error.WriteLine("No .codex source files matched by project sources.");
            return 1;
        }

        // Resolve project dependencies (packages + path deps + prelude)
        string fullDir = Path.GetFullPath(directory);
        DiagnosticBag depDiag = new();
        PackageResolver packageResolver = new(fullDir, depDiag);
        List<Codex.Semantics.IModuleLoader> depLoaders = packageResolver.ResolveAll(project);
        IReadOnlyList<Codex.Semantics.IModuleLoader>? extraLoaders =
            depLoaders.Count > 0 ? depLoaders : null;

        if (depLoaders.Count > 0)
            Console.WriteLine($"  Dependencies: {depLoaders.Count} loader(s)");

        string outputDir = Path.GetFullPath(Path.Combine(directory, project.Output));
        if (!Directory.Exists(outputDir))
            Directory.CreateDirectory(outputDir);

        if (multiTargets is not null && multiTargets.Length > 1)
        {
            Console.WriteLine($"  Sources: {files.Length} file(s)");
            Console.WriteLine($"  Targets: {string.Join(", ", multiTargets)}");
            return RunParallelMultiTargetBuild(directory, multiTargets, files, outputDir, project.Name, extraLoaders);
        }

        string[]? projectTargets = project.Targets.Length > 1 ? project.Targets : null;
        if (projectTargets is not null && targetOverride is null)
        {
            Console.WriteLine($"  Sources: {files.Length} file(s)");
            Console.WriteLine($"  Targets: {string.Join(", ", projectTargets)}");
            return RunParallelMultiTargetBuild(directory, projectTargets, files, outputDir, project.Name, extraLoaders);
        }

        string target = targetOverride ?? project.Target;
        Console.WriteLine($"  Sources: {files.Length} file(s)");
        Console.WriteLine($"  Target:  {target}");

        if (incremental && !IsAssemblyTarget(target))
        {
            string outputPath = Path.Combine(outputDir, project.Name + CreateEmitter(target).FileExtension);
            return RunIncrementalBuild(directory, target, files, outputPath, extraLoaders);
        }

        IRCompilationResult? irResult = CompileMultipleToIR(files, project.Name, extraLoaders);
        if (irResult is null) return 1;

        if (IsAssemblyTarget(target))
        {
            return EmitAssembly(irResult, outputDir, project.Name, target);
        }

        Codex.Emit.ICodeEmitter emitter = CreateEmitter(target);
        string output = emitter.Emit(irResult.Module);
        string fullOutputPath = Path.Combine(outputDir, project.Name + emitter.FileExtension);
        File.WriteAllText(fullOutputPath, output);

        Console.WriteLine($"✓ Compiled to {fullOutputPath} ({target})");
        foreach (KeyValuePair<string, CodexType> kv in irResult.Types)
            Console.WriteLine($"  {kv.Key} : {kv.Value}");
        return 0;
    }
    static int RunBuildDirectory(string directory, string target)
    {
        string[] files = Directory.GetFiles(directory, "*.codex", SearchOption.AllDirectories);
        if (files.Length == 0)
        {
            Console.Error.WriteLine($"No .codex files found in {directory}");
            return 1;
        }
        Array.Sort(files, StringComparer.Ordinal);
        string moduleName = Path.GetFileName(Path.GetFullPath(directory));
        IRCompilationResult? irResult = CompileMultipleToIR(files, moduleName);
        if (irResult is null) return 1;

        if (IsAssemblyTarget(target))
        {
            return EmitAssembly(irResult, directory, moduleName, target);
        }

        Codex.Emit.ICodeEmitter emitter = CreateEmitter(target);
        string output = emitter.Emit(irResult.Module);
        string outputPath = Path.Combine(directory, "output" + emitter.FileExtension);
        File.WriteAllText(outputPath, output);

        Console.WriteLine($"✓ Compiled to {outputPath} ({target})");
        foreach (KeyValuePair<string, CodexType> kv in irResult.Types)
            Console.WriteLine($"  {kv.Key} : {kv.Value}");
        return 0;
    }

    static int RunBuildFile(string filePath, string target)
    {
        IRCompilationResult? irResult = CompileToIR(filePath);
        if (irResult is null) return 1;

        if (IsAssemblyTarget(target))
        {
            string outputDir = Path.GetDirectoryName(Path.GetFullPath(filePath)) ?? ".";
            string moduleName = Path.GetFileNameWithoutExtension(filePath);
            return EmitAssembly(irResult, outputDir, moduleName, target);
        }

        Codex.Emit.ICodeEmitter emitter = CreateEmitter(target);
        string output = emitter.Emit(irResult.Module);
        string outputPath = Path.ChangeExtension(filePath, emitter.FileExtension);
        File.WriteAllText(outputPath, output);

        Console.WriteLine($"✓ Compiled to {outputPath} ({target})");
        foreach (KeyValuePair<string, CodexType> kv in irResult.Types)
            Console.WriteLine($"  {kv.Key} : {kv.Value}");
        return 0;
    }

    static bool IsAssemblyTarget(string target) => target is "il" or "exe";

    static int EmitAssembly(IRCompilationResult irResult, string outputDir, string moduleName, string target)
    {
        Emit.IL.ILEmitter emitter = new();
        byte[] assembly = emitter.EmitAssembly(irResult.Module, moduleName);
        string outputPath = Path.Combine(outputDir, moduleName + ".exe");
        File.WriteAllBytes(outputPath, assembly);

        string runtimeConfigPath = Path.Combine(outputDir, moduleName + ".runtimeconfig.json");
        File.WriteAllText(runtimeConfigPath, """
            {
              "runtimeOptions": {
                "tfm": "net8.0",
                "framework": {
                  "name": "Microsoft.NETCore.App",
                  "version": "8.0.0"
                }
              }
            }
            """);

        Console.WriteLine($"✓ Compiled to {outputPath} ({target})");
        foreach (KeyValuePair<string, CodexType> kv in irResult.Types)
            Console.WriteLine($"  {kv.Key} : {kv.Value}");
        return 0;
    }

    static Emit.ICodeEmitter CreateEmitter(string target) => target switch
    {
        "js" or "javascript" => new Emit.JavaScript.JavaScriptEmitter(),
        "rust" or "rs" => new Emit.Rust.RustEmitter(),
        "python" or "py" => new Emit.Python.PythonEmitter(),
        "cpp" or "c++" => new Emit.Cpp.CppEmitter(),
        "go" => new Emit.Go.GoEmitter(),
        "java" => new Emit.Java.JavaEmitter(),
        "ada" => new Emit.Ada.AdaEmitter(),
        "babbage" or "ae" => new Emit.Babbage.BabbageEmitter(),
        "fortran" or "f90" => new Emit.Fortran.FortranEmitter(),
        "cobol" or "cob" => new Emit.Cobol.CobolEmitter(),
        _ => new CSharpEmitter()
    };
}
