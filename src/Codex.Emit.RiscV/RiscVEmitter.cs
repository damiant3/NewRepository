using Codex.IR;

namespace Codex.Emit.RiscV;

public sealed class RiscVEmitter : IAssemblyEmitter
{
    public string TargetName => "RiscV";

    public byte[] EmitAssembly(IRModule module, string assemblyName)
    {
        RiscVCodeGen codeGen = new();
        codeGen.EmitModule(module);
        return codeGen.BuildElf();
    }
}
