using Codex.Core;
using Codex.Types;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Codex.Lsp;

internal sealed class HoverHandler : HoverHandlerBase
{
    readonly DocumentStore m_store;

    public HoverHandler(DocumentStore store)
    {
        m_store = store;
    }

    public override Task<Hover?> Handle(HoverParams request, CancellationToken ct)
    {
        string uri = request.TextDocument.Uri.ToString();
        AnalysisResult? result = m_store.GetResult(uri);
        string? text = m_store.GetText(uri);
        if (result is null || text is null)
            return Task.FromResult<Hover?>(null);

        int line = (int)request.Position.Line;
        int col = (int)request.Position.Character;

        string? word = LspHelpers.GetWordAt(text, line, col);
        if (word is null)
            return Task.FromResult<Hover?>(null);

        CodexType? type = result.Types[word];
        if (type is null)
            return Task.FromResult<Hover?>(null);

        string content = $"**{word}** : `{type}`";
        return Task.FromResult<Hover?>(new Hover
        {
            Contents = new MarkedStringsOrMarkupContent(new MarkupContent
            {
                Kind = MarkupKind.Markdown,
                Value = content,
            }),
        });
    }

    protected override HoverRegistrationOptions CreateRegistrationOptions(
        HoverCapability capability,
        ClientCapabilities clientCapabilities)
    {
        return new HoverRegistrationOptions
        {
            DocumentSelector = LspHelpers.s_selector,
        };
    }
}
