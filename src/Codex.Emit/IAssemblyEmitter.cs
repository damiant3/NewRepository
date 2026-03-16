using Codex.IR;

namespace Codex.Emit;

public interface IAssemblyEmitter
{
    string TargetName { get; }

    byte[] EmitAssembly(IRModule module, string assemblyName);
}
