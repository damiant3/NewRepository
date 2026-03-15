using Codex.Core;
using Codex.Ast;

namespace Codex.Semantics;

public interface IModuleLoader
{
    ResolvedModule? Load(string moduleName);
}
