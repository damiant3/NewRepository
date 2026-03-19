using Codex.Core;
using Codex.Semantics;

namespace Codex.Cli;

sealed class PreludeModuleLoader : IModuleLoader
{
    readonly FileModuleLoader m_inner;

    PreludeModuleLoader(string preludeDir, DiagnosticBag diagnostics)
    {
        m_inner = new FileModuleLoader(preludeDir, diagnostics);
    }

    public ResolvedModule? Load(string moduleName) => m_inner.Load(moduleName);

    public static PreludeModuleLoader? TryCreate(DiagnosticBag diagnostics)
    {
        string? preludeDir = FindPreludeDirectory();
        if (preludeDir is null) return null;
        return new PreludeModuleLoader(preludeDir, diagnostics);
    }

    static string? FindPreludeDirectory()
    {
        string? dir = Path.GetDirectoryName(typeof(PreludeModuleLoader).Assembly.Location);
        while (dir is not null)
        {
            string candidate = Path.Combine(dir, "prelude");
            if (Directory.Exists(candidate))
                return candidate;
            dir = Path.GetDirectoryName(dir);
        }

        string cwdCandidate = Path.Combine(Directory.GetCurrentDirectory(), "prelude");
        if (Directory.Exists(cwdCandidate))
            return cwdCandidate;

        return null;
    }
}
