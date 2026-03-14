using System.Collections.Immutable;

namespace Codex.Core;

/// <summary>
/// The severity of a compiler diagnostic.
/// </summary>
public enum DiagnosticSeverity
{
    Hint,
    Info,
    Warning,
    Error
}

/// <summary>
/// A compiler diagnostic — an error, warning, or suggestion with a source location.
/// Diagnostics are values, not exceptions. They accumulate during compilation.
/// </summary>
public sealed record Diagnostic(
    DiagnosticSeverity Severity,
    string Code,
    string Message,
    SourceSpan Span,
    ImmutableArray<SourceSpan> RelatedSpans)
{
    public Diagnostic(DiagnosticSeverity severity, string code, string message, SourceSpan span)
        : this(severity, code, message, span, ImmutableArray<SourceSpan>.Empty)
    {
    }

    public bool IsError => Severity == DiagnosticSeverity.Error;

    public override string ToString() =>
        $"{Severity} {Code}: {Message} at {Span}";
}

/// <summary>
/// Accumulator for diagnostics. Used throughout the compiler pipeline.
/// Thread-safe for parallel compilation phases.
/// </summary>
public sealed class DiagnosticBag
{
    private readonly List<Diagnostic> m_diagnostics = [];
    private readonly object m_lock = new();

    public void Add(Diagnostic diagnostic)
    {
        lock (m_lock)
        {
            m_diagnostics.Add(diagnostic);
        }
    }

    public void Error(string code, string message, SourceSpan span)
    {
        Add(new Diagnostic(DiagnosticSeverity.Error, code, message, span));
    }

    public void Warning(string code, string message, SourceSpan span)
    {
        Add(new Diagnostic(DiagnosticSeverity.Warning, code, message, span));
    }

    public void Info(string code, string message, SourceSpan span)
    {
        Add(new Diagnostic(DiagnosticSeverity.Info, code, message, span));
    }

    public bool HasErrors
    {
        get
        {
            lock (m_lock) { return m_diagnostics.Any(d => d.IsError); }
        }
    }

    public int Count
    {
        get
        {
            lock (m_lock) { return m_diagnostics.Count; }
        }
    }

    public ImmutableArray<Diagnostic> ToImmutable()
    {
        lock (m_lock) { return m_diagnostics.ToImmutableArray(); }
    }
}
