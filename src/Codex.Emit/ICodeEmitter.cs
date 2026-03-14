namespace Codex.Emit;

/// <summary>
/// The interface every code generation backend implements.
/// </summary>
public interface ICodeEmitter
{
    string TargetName { get; }
    string FileExtension { get; }
}
