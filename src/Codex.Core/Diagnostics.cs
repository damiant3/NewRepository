using System.Collections.Immutable;

namespace Codex.Core;

public enum DiagnosticSeverity
{
    Hint,
    Info,
    Warning,
    Error
}

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

public sealed class DiagnosticBag
{
    readonly List<Diagnostic> m_diagnostics = [];
    readonly object m_lock = new();

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

    public void Error(string code, string message, SourceSpan span, params SourceSpan[] relatedSpans)
    {
        Add(new Diagnostic(DiagnosticSeverity.Error, code, message, span,
            System.Collections.Immutable.ImmutableArray.Create(relatedSpans)));
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
