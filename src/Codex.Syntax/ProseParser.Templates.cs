using Codex.Core;

namespace Codex.Syntax;

public sealed partial class ProseParser
{
    static string? TryMatchRecordTemplate(string line)
    {
        string lower = line.TrimEnd();
        if (!lower.EndsWith(":", StringComparison.Ordinal))
            return null;
        lower = lower[..^1].Trim();

        if (!lower.EndsWith("containing", StringComparison.OrdinalIgnoreCase))
            return null;
        lower = lower[..^"containing".Length].Trim();

        if (!lower.EndsWith("record", StringComparison.OrdinalIgnoreCase))
            return null;
        lower = lower[..^"record".Length].Trim();

        if (!lower.EndsWith("a", StringComparison.OrdinalIgnoreCase))
            return null;
        lower = lower[..^1].Trim();

        int isIdx = lower.LastIndexOf(" is", StringComparison.OrdinalIgnoreCase);
        if (isIdx < 0)
            isIdx = lower.LastIndexOf(" Is", StringComparison.OrdinalIgnoreCase);
        if (isIdx < 0)
            return null;
        string nameSegment = lower[..isIdx].Trim();

        if (nameSegment.StartsWith("An ", StringComparison.OrdinalIgnoreCase))
            nameSegment = nameSegment[3..].Trim();
        else if (nameSegment.StartsWith("A ", StringComparison.OrdinalIgnoreCase))
            nameSegment = nameSegment[2..].Trim();

        return nameSegment.Length > 0 ? nameSegment : null;
    }

    static string? TryMatchVariantTemplate(string line)
    {
        string lower = line.TrimEnd();
        if (!lower.EndsWith(":", StringComparison.Ordinal))
            return null;
        lower = lower[..^1].Trim();

        if (!lower.EndsWith("either", StringComparison.OrdinalIgnoreCase))
            return null;
        lower = lower[..^"either".Length].Trim();

        int isIdx = lower.LastIndexOf(" is", StringComparison.OrdinalIgnoreCase);
        if (isIdx < 0)
            return null;
        string nameSegment = lower[..isIdx].Trim();

        return nameSegment.Length > 0 ? nameSegment : null;
    }

    static bool IsTemplateMatch(string line)
    {
        return TryMatchRecordTemplate(line) is not null
            || TryMatchVariantTemplate(line) is not null;
    }

    void ParseProseOrTemplate(List<DocumentMember> members)
    {
        int startOffset = LineOffset(m_lineIndex);
        List<string> proseLines = [];
        string? templateLine = null;
        int templateLineIdx = -1;

        while (m_lineIndex < m_lines.Length)
        {
            string line = m_lines[m_lineIndex];
            string trimmed = line.Trim();

            if (trimmed.Length == 0)
                break;
            if (trimmed.StartsWith("Chapter:", StringComparison.Ordinal)
                || trimmed.StartsWith("Section:", StringComparison.Ordinal))
                break;

            int indent = MeasureIndent(line);
            if (indent >= 4 && LooksLikeNotation(trimmed))
                break;

            if (trimmed.StartsWith("-", StringComparison.Ordinal)
                && templateLine is not null)
                break;

            if (IsTemplateMatch(trimmed))
            {
                templateLine = trimmed;
                templateLineIdx = m_lineIndex;
                proseLines.Add(trimmed);
                m_lineIndex++;
                break;
            }

            proseLines.Add(trimmed);
            m_lineIndex++;
        }

        int endOffset = m_lineIndex < m_lines.Length
            ? LineOffset(m_lineIndex) : m_source.Content.Length;
        string text = string.Join("\n", proseLines);
        SourceSpan proseSpan = MakeSpan(startOffset, endOffset);
        members.Add(new ProseBlockNode(text, proseSpan));

        if (templateLine is null)
            return;

        SourceSpan templateSpan = MakeLineSpan(templateLineIdx);

        string? recordName = TryMatchRecordTemplate(templateLine);
        if (recordName is not null)
        {
            string typeName = ToPascalCase(recordName);
            TypeDefinitionNode? typeDef =
                TryParseRecordFromBullets(typeName, templateSpan);
            if (typeDef is not null)
                members.Add(new NotationBlockNode(
                    [], [typeDef], typeDef.Span));
            return;
        }

        string? variantName = TryMatchVariantTemplate(templateLine);
        if (variantName is not null)
        {
            string typeName = ToPascalCase(variantName);
            TypeDefinitionNode? typeDef =
                TryParseVariantFromBullets(typeName, templateSpan);
            if (typeDef is not null)
                members.Add(new NotationBlockNode(
                    [], [typeDef], typeDef.Span));
        }
    }

    TypeDefinitionNode? TryParseRecordFromBullets(
        string typeName, SourceSpan headerSpan)
    {
        SkipBlankLines();
        int startOffset = m_lineIndex < m_lines.Length
            ? LineOffset(m_lineIndex) : headerSpan.End.Offset;

        List<RecordTypeFieldNode> fields = [];
        while (m_lineIndex < m_lines.Length)
        {
            string line = m_lines[m_lineIndex];
            string trimmed = line.Trim();
            if (trimmed.Length == 0)
                break;
            if (!trimmed.StartsWith("-", StringComparison.Ordinal))
                break;

            SourceSpan lineSpan = MakeLineSpan(m_lineIndex);
            string bullet = trimmed[1..].Trim();
            RecordTypeFieldNode? field = ParseRecordBullet(bullet, lineSpan);
            if (field is not null)
                fields.Add(field);
            m_lineIndex++;
        }

        if (fields.Count == 0)
            return null;

        int endOffset = m_lineIndex < m_lines.Length
            ? LineOffset(m_lineIndex) : m_source.Content.Length;
        SourceSpan bodySpan = MakeSpan(startOffset, endOffset);
        RecordTypeBody body = new(fields, bodySpan);

        SourceSpan fullSpan = headerSpan.Through(bodySpan);
        Token nameToken = MakeSyntheticToken(
            TokenKind.TypeIdentifier, typeName, headerSpan);
        return new TypeDefinitionNode(nameToken, [], body, fullSpan);
    }

    TypeDefinitionNode? TryParseVariantFromBullets(
        string typeName, SourceSpan headerSpan)
    {
        SkipBlankLines();
        int startOffset = m_lineIndex < m_lines.Length
            ? LineOffset(m_lineIndex) : headerSpan.End.Offset;

        List<VariantConstructorNode> ctors = [];
        while (m_lineIndex < m_lines.Length)
        {
            string line = m_lines[m_lineIndex];
            string trimmed = line.Trim();
            if (trimmed.Length == 0)
                break;
            if (!trimmed.StartsWith("-", StringComparison.Ordinal))
                break;

            SourceSpan lineSpan = MakeLineSpan(m_lineIndex);
            string bullet = trimmed[1..].Trim();
            VariantConstructorNode? ctor =
                ParseVariantBullet(bullet, lineSpan);
            if (ctor is not null)
                ctors.Add(ctor);
            m_lineIndex++;
        }

        if (ctors.Count == 0)
            return null;

        int endOffset = m_lineIndex < m_lines.Length
            ? LineOffset(m_lineIndex) : m_source.Content.Length;
        SourceSpan bodySpan = MakeSpan(startOffset, endOffset);
        VariantTypeBody body = new(ctors, bodySpan);

        SourceSpan fullSpan = headerSpan.Through(bodySpan);
        Token nameToken = MakeSyntheticToken(
            TokenKind.TypeIdentifier, typeName, headerSpan);
        return new TypeDefinitionNode(nameToken, [], body, fullSpan);
    }

    RecordTypeFieldNode? ParseRecordBullet(string bullet, SourceSpan span)
    {
        int colonIdx = bullet.IndexOf(':');
        if (colonIdx < 0)
        {
            m_diagnostics.Warning("CDX1101",
                $"Record field bullet should be '- name : Type', got '- {bullet}'",
                span);
            return null;
        }

        string fieldName = bullet[..colonIdx].Trim();
        string typeText = bullet[(colonIdx + 1)..].Trim();

        if (fieldName.Length == 0 || typeText.Length == 0)
            return null;

        string normalizedField = ToFieldName(fieldName);
        Token fieldToken = MakeSyntheticToken(
            TokenKind.Identifier, normalizedField, span);
        TypeNode typeNode = ParseTypeFromText(typeText, span);
        return new RecordTypeFieldNode(fieldToken, typeNode, span);
    }

    VariantConstructorNode? ParseVariantBullet(string bullet, SourceSpan span)
    {
        int parenIdx = bullet.IndexOf('(');
        string ctorName;
        List<VariantFieldNode> fields = [];

        if (parenIdx >= 0)
        {
            ctorName = ToPascalCase(bullet[..parenIdx].Trim());
            int closeIdx = bullet.LastIndexOf(')');
            if (closeIdx > parenIdx)
            {
                string fieldList = bullet[(parenIdx + 1)..closeIdx];
                foreach (string part in fieldList.Split(','))
                {
                    string trimPart = part.Trim();
                    if (trimPart.Length == 0)
                        continue;

                    int colonIdx = trimPart.IndexOf(':');
                    if (colonIdx >= 0)
                    {
                        string fName = ToFieldName(trimPart[..colonIdx].Trim());
                        string fType = trimPart[(colonIdx + 1)..].Trim();
                        Token fToken = MakeSyntheticToken(
                            TokenKind.Identifier, fName, span);
                        TypeNode typeNode = ParseTypeFromText(fType, span);
                        fields.Add(new VariantFieldNode(fToken, typeNode, span));
                    }
                    else
                    {
                        TypeNode typeNode = ParseTypeFromText(trimPart, span);
                        fields.Add(new VariantFieldNode(null, typeNode, span));
                    }
                }
            }
        }
        else
            ctorName = ToPascalCase(bullet.Trim());

        if (ctorName.Length == 0)
            return null;

        Token ctorToken = MakeSyntheticToken(
            TokenKind.TypeIdentifier, ctorName, span);
        return new VariantConstructorNode(ctorToken, fields, span);
    }

    TypeNode ParseTypeFromText(string typeText, SourceSpan span)
    {
        SourceText src = new(m_source.FileName, typeText);
        DiagnosticBag diag = new();
        Lexer lexer = new(src, diag);
        IReadOnlyList<Token> tokens = lexer.TokenizeAll();
        Parser parser = new(tokens, diag);
        TypeNode? result = parser.TryParseType();
        if (result is not null)
            return result;
        Token synth = MakeSyntheticToken(
            TokenKind.TypeIdentifier, typeText.Trim(), span);
        return new NamedTypeNode(synth);
    }

    static Token MakeSyntheticToken(
        TokenKind kind, string text, SourceSpan span)
    {
        return new Token(kind, text, span);
    }

    static string ToPascalCase(string text)
    {
        if (text.Length == 0) return text;
        string[] words = text.Split([' ', '-'], StringSplitOptions.RemoveEmptyEntries);
        return string.Concat(words.Select(w =>
            char.ToUpperInvariant(w[0]) + w[1..].ToLowerInvariant()));
    }

    static string ToFieldName(string text)
    {
        string[] words = text.Split([' ', '-'], StringSplitOptions.RemoveEmptyEntries);
        if (words.Length == 0) return text;
        string first = words[0].ToLowerInvariant();
        string rest = string.Concat(words.Skip(1).Select(w =>
            char.ToUpperInvariant(w[0]) + w[1..].ToLowerInvariant()));
        return first + rest;
    }
}
