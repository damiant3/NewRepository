using Codex.Ast;
using Codex.Core;
using Codex.Semantics;
using Codex.Syntax;

namespace Codex.Cli;

sealed class ProjectModuleLoader : IModuleLoader
{
    readonly string m_projectDirectory;
    readonly DiagnosticBag m_diagnostics;
    readonly string[] m_sourceFiles;
    Map<string, ResolvedModule> m_cache = Map<string, ResolvedModule>.s_empty;

    ProjectModuleLoader(
        string projectDirectory,
        string[] sourceFiles,
        DiagnosticBag diagnostics)
    {
        m_projectDirectory = projectDirectory;
        m_sourceFiles = sourceFiles;
        m_diagnostics = diagnostics;
    }

    public ResolvedModule? Load(string moduleName)
    {
        ResolvedModule? cached = m_cache[moduleName];
        if (cached is not null)
            return cached;

        string? filePath = FindSourceFile(moduleName);
        if (filePath is null)
            return null;

        string source = File.ReadAllText(filePath);
        SourceText src = new(filePath, source);
        DiagnosticBag compileDiag = new();

        DocumentNode document;
        if (ProseParser.IsProseDocument(source))
        {
            ProseParser proseParser = new(src, compileDiag);
            document = proseParser.ParseDocument();
        }
        else
        {
            Lexer lexer = new(src, compileDiag);
            IReadOnlyList<Token> tokens = lexer.TokenizeAll();
            Parser parser = new(tokens, compileDiag);
            document = parser.ParseDocument();
        }

        if (compileDiag.HasErrors)
            return null;

        Desugarer desugarer = new(compileDiag);
        Module module = desugarer.Desugar(document, moduleName);
        if (compileDiag.HasErrors)
            return null;

        FileModuleLoader transitiveLoader = new(
            Path.GetDirectoryName(filePath) ?? m_projectDirectory, compileDiag);
        NameResolver resolver = new(compileDiag, transitiveLoader);
        ResolvedModule resolved = resolver.Resolve(module);
        if (compileDiag.HasErrors)
            return null;

        m_cache = m_cache.Set(moduleName, resolved);
        return resolved;
    }

    string? FindSourceFile(string moduleName)
    {
        string target = moduleName + ".codex";
        string targetLower = moduleName.ToLowerInvariant() + ".codex";

        foreach (string file in m_sourceFiles)
        {
            string fileName = Path.GetFileName(file);
            if (string.Equals(fileName, target, StringComparison.OrdinalIgnoreCase)
                || string.Equals(fileName, targetLower, StringComparison.OrdinalIgnoreCase))
            {
                return file;
            }
        }

        return null;
    }

    public static ProjectModuleLoader? TryCreate(
        string dependencyPath,
        string relativeTo,
        DiagnosticBag diagnostics)
    {
        string fullPath = Path.GetFullPath(Path.Combine(relativeTo, dependencyPath));
        if (!Directory.Exists(fullPath))
            return null;

        string projectFile = Path.Combine(fullPath, "codex.project.json");
        if (!File.Exists(projectFile))
            return null;

        Program.CodexProject? project = Program.LoadProjectFile(fullPath);
        if (project is null)
            return null;

        string[] sources = Program.ResolveProjectSources(fullPath, project);
        if (sources.Length == 0)
            return null;

        return new ProjectModuleLoader(fullPath, sources, diagnostics);
    }
}
