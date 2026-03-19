using Codex.Semantics;

namespace Codex.Cli;

sealed class CompositeModuleLoader : IModuleLoader
{
    readonly IModuleLoader[] m_loaders;

    public CompositeModuleLoader(params IModuleLoader[] loaders)
    {
        m_loaders = loaders;
    }

    public ResolvedModule? Load(string moduleName)
    {
        foreach (IModuleLoader loader in m_loaders)
        {
            ResolvedModule? result = loader.Load(moduleName);
            if (result is not null)
                return result;
        }
        return null;
    }
}
