using Codex.Core;

namespace Codex.Syntax;

public sealed partial class ProseParser
{
    void ValidateProseNotationConsistency(IReadOnlyList<ChapterNode> chapters)
    {
        foreach (ChapterNode chapter in chapters)
        {
            ValidateMembers(chapter.Members);
        }
    }

    void ValidateMembers(IReadOnlyList<DocumentMember> members)
    {
        for (int i = 0; i < members.Count; i++)
        {
            DocumentMember member = members[i];

            if (member is SectionNode section)
            {
                ValidateMembers(section.Members);
                continue;
            }

            if (member is not ProseBlockNode prose)
                continue;

            // Find the next notation block after this prose block
            NotationBlockNode? notation = null;
            for (int j = i + 1; j < members.Count; j++)
            {
                if (members[j] is NotationBlockNode nb)
                {
                    notation = nb;
                    break;
                }
                if (members[j] is ProseBlockNode or SectionNode)
                    break;
            }

            if (notation is null)
                continue;

            // Check function template against notation definitions
            if (prose.FunctionTemplate is not null && notation.Definitions.Count > 0)
            {
                ValidateFunctionTemplate(prose.FunctionTemplate, notation.Definitions[0]);
            }

            // Check record template (prose text contains the template, notation has the type def)
            // Record/variant templates generate their own NotationBlockNode, so the type def
            // IS the notation block. Check the prose text against adjacent type definitions.
        }
    }

    void ValidateFunctionTemplate(FunctionTemplateInfo template, DefinitionNode definition)
    {
        string defName = definition.Name.Text;
        if (!string.Equals(template.FunctionName, defName, StringComparison.OrdinalIgnoreCase))
        {
            m_diagnostics.Warning("CDX1101",
                $"Prose declares function '{template.FunctionName}' but notation defines '{defName}'",
                template.Span);
        }

        // Check parameter names
        IReadOnlyList<Token> defParams = definition.Parameters;
        for (int p = 0; p < template.Parameters.Count && p < defParams.Count; p++)
        {
            string proseName = template.Parameters[p].Name;
            string notationName = defParams[p].Text;
            if (!string.Equals(proseName, notationName, StringComparison.OrdinalIgnoreCase))
            {
                m_diagnostics.Warning("CDX1102",
                    $"Prose parameter '{proseName}' does not match notation parameter '{notationName}'",
                    template.Span);
            }
        }
    }
}
