using Codex.Ast;
using Codex.Core;
using Codex.Repository;
using Codex.Semantics;
using Codex.Syntax;

namespace Codex.Cli;

sealed class RepositoryModuleLoader(FactStore store, DiagnosticBag diagnostics) : IModuleLoader
{
    readonly FactStore m_store = store;
    readonly DiagnosticBag m_diagnostics = diagnostics;
    Map<string, ResolvedModule> m_cache = Map<string, ResolvedModule>.s_empty;

    public ResolvedModule? Load(string moduleName)
    {
        ResolvedModule? cached = m_cache[moduleName];
        if (cached is not null)
            return cached;

        ContentHash? hash = m_store.LookupView(moduleName);
        if (hash is null)
            return null;

        Fact? fact = m_store.Load(hash.Value);
        if (fact is null || fact.Kind != FactKind.Definition)
            return null;

        string source = fact.Content;
        SourceText src = new($"{moduleName}.codex", source);
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

        NameResolver resolver = new(compileDiag);
        ResolvedModule resolved = resolver.Resolve(module);
        if (compileDiag.HasErrors)
            return null;

        m_cache = m_cache.Set(moduleName, resolved);
        return resolved;
    }
}
