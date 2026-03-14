using Codex.Ast;
using Codex.Core;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Codex.Lsp;

internal sealed class DefinitionHandler : DefinitionHandlerBase
{
    private readonly DocumentStore m_store;

    public DefinitionHandler(DocumentStore store)
    {
        m_store = store;
    }

    public override Task<LocationOrLocationLinks?> Handle(
        DefinitionParams request,
        CancellationToken ct)
    {
        string uri = request.TextDocument.Uri.ToString();
        AnalysisResult? result = m_store.GetResult(uri);
        string? text = m_store.GetText(uri);
        if (result is null || text is null)
            return Task.FromResult<LocationOrLocationLinks?>(null);

        int line = (int)request.Position.Line;
        int col = (int)request.Position.Character;

        string? word = LspHelpers.GetWordAt(text, line, col);
        if (word is null)
            return Task.FromResult<LocationOrLocationLinks?>(null);

        Definition? target = null;
        foreach (Definition def in result.Definitions)
        {
            if (def.Name.Value == word)
            {
                target = def;
                break;
            }
        }

        if (target is null)
            return Task.FromResult<LocationOrLocationLinks?>(null);

        Location location = new()
        {
            Uri = DocumentUri.Parse(uri),
            Range = TextDocumentSyncHandler.SpanToRange(target.Span),
        };

        return Task.FromResult<LocationOrLocationLinks?>(new LocationOrLocationLinks(location));
    }

    protected override DefinitionRegistrationOptions CreateRegistrationOptions(
        DefinitionCapability capability,
        ClientCapabilities clientCapabilities)
    {
        return new DefinitionRegistrationOptions
        {
            DocumentSelector = LspHelpers.s_selector,
        };
    }
}
