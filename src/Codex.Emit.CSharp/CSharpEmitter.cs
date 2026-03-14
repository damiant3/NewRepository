namespace Codex.Emit.CSharp;

/// <summary>
/// Placeholder for the C# code emitter.
/// Will be implemented in Milestone 3.
/// </summary>
public sealed class CSharpEmitter : Emit.ICodeEmitter
{
    public string TargetName => "C#";
    public string FileExtension => ".cs";
}
