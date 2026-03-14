using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Codex.Lsp;

internal sealed class DocumentStore
{
    private readonly Dictionary<string, string> m_documents = new();
    private readonly Dictionary<string, AnalysisResult> m_results = new();
    private readonly object m_lock = new();

    internal static readonly DocumentSelector s_selector = DocumentSelector.ForLanguage("codex");

    public void Update(string uri, string text)
    {
        lock (m_lock)
        {
            m_documents[uri] = text;
        }
    }

    public void UpdateResult(string uri, AnalysisResult result)
    {
        lock (m_lock)
        {
            m_results[uri] = result;
        }
    }

    public void Remove(string uri)
    {
        lock (m_lock)
        {
            m_documents.Remove(uri);
            m_results.Remove(uri);
        }
    }

    public string? GetText(string uri)
    {
        lock (m_lock)
        {
            return m_documents.TryGetValue(uri, out string? text) ? text : null;
        }
    }

    public AnalysisResult? GetResult(string uri)
    {
        lock (m_lock)
        {
            return m_results.TryGetValue(uri, out AnalysisResult? result) ? result : null;
        }
    }
}
