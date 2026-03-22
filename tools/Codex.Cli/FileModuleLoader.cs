using Codex.Ast;
using Codex.Core;
using Codex.Semantics;
using Codex.Syntax;

namespace Codex.Cli;

sealed class FileModuleLoader(string baseDirectory, DiagnosticBag diagnostics) : IModuleLoader
{
    readonly string m_baseDirectory = baseDirectory;
    readonly DiagnosticBag m_diagnostics = diagnostics;
    Map<string, ResolvedModule> m_cache = Map<string, ResolvedModule>.s_empty;

    public ResolvedModule? Load(string moduleName)
    {
        ResolvedModule? cached = m_cache[moduleName];
        if (cached is not null)
            return cached;

        string filePath = Path.Combine(m_baseDirectory, moduleName + ".codex");
        if (!File.Exists(filePath))
        {
            // Try lowercase
            filePath = Path.Combine(m_baseDirectory,
                moduleName.ToLowerInvariant() + ".codex");
            if (!File.Exists(filePath))
                return null;
        }

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

        // Use a fresh FileModuleLoader for transitive imports from the same directory
        FileModuleLoader transitiveLoader = new(
            Path.GetDirectoryName(filePath) ?? m_baseDirectory, compileDiag);
        NameResolver resolver = new(compileDiag, transitiveLoader);
        ResolvedModule resolved = resolver.Resolve(module);
        if (compileDiag.HasErrors)
            return null;

        m_cache = m_cache.Set(moduleName, resolved);
        return resolved;
    }
}
