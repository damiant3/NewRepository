using Codex.IR;

namespace Codex.Emit;

/// <summary>
/// The interface every code generation backend implements.
/// Takes an IR module and produces target source code.
/// </summary>
public interface ICodeEmitter
{
    /// <summary>The human-readable name of the target (e.g., "C#", "Rust").</summary>
    string TargetName { get; }

    /// <summary>The file extension for emitted files (e.g., ".cs").</summary>
    string FileExtension { get; }

    /// <summary>
    /// Emit an IR module as target source code.
    /// </summary>
    string Emit(IRModule module);
}
