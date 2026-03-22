using Codex.IR;

namespace Codex.Emit.RiscV;

public enum RiscVTarget
{
    LinuxUser,
    BareMetal
}

public sealed class RiscVEmitter : IAssemblyEmitter
{
    readonly RiscVTarget m_target;

    public RiscVEmitter(RiscVTarget target = RiscVTarget.LinuxUser)
    {
        m_target = target;
    }

    public string TargetName => m_target == RiscVTarget.BareMetal ? "RiscV-BareMetal" : "RiscV";

    public byte[] EmitAssembly(IRModule module, string assemblyName)
    {
        RiscVCodeGen codeGen = new(m_target);
        codeGen.EmitModule(module);
        return codeGen.BuildElf();
    }
}