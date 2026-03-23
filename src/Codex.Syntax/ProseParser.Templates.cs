using Codex.Core;

namespace Codex.Syntax;

public sealed partial class ProseParser
{
    static string? TryMatchRecordTemplate(string line)
    {
        string lower = line.TrimEnd();
        if (!lower.EndsWith(':'))
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
        if (!lower.EndsWith(':'))
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

    static FunctionTemplateInfo? TryMatchFunctionTemplate(string line, SourceSpan span)
    {
        string trimmed = line.TrimEnd();
        if (!trimmed.EndsWith(':'))
            return null;
        trimmed = trimmed[..^1].Trim();

        if (!trimmed.StartsWith("To ", StringComparison.OrdinalIgnoreCase))
            return null;
        string body = trimmed[3..].Trim();
        if (body.Length == 0)
            return null;

        // Extract return type from "gives Type" suffix
        string? returnType = null;
        int givesIdx = body.LastIndexOf(" gives ", StringComparison.OrdinalIgnoreCase);
        if (givesIdx >= 0)
        {
            returnType = body[(givesIdx + " gives ".Length)..].Trim();
            body = body[..givesIdx].Trim();
        }

        // Extract function name (words before first paren) and parameters
        List<(string Name, string Type)> parameters = [];
        List<string> nameWords = [];
        bool seenParam = false;
        int i = 0;
        while (i < body.Length)
        {
            if (body[i] == '(')
            {
                seenParam = true;
                int close = body.IndexOf(')', i);
                if (close < 0)
                    break;
                string paramText = body[(i + 1)..close].Trim();
                int colonIdx = paramText.IndexOf(':');
                if (colonIdx >= 0)
                {
                    string pName = paramText[..colonIdx].Trim();
                    string pType = paramText[(colonIdx + 1)..].Trim();
                    if (pName.Length > 0 && pType.Length > 0)
                        parameters.Add((ToFieldName(pName), pType));
                }
                i = close + 1;
            }
            else if (char.IsWhiteSpace(body[i]))
            {
                i++;
            }
            else
            {
                int wordEnd = i;
                while (wordEnd < body.Length && body[wordEnd] != '(' && !char.IsWhiteSpace(body[wordEnd]))
                    wordEnd++;
                // Only words before the first parameter are the function name
                if (!seenParam)
                    nameWords.Add(body[i..wordEnd].ToLowerInvariant());
                i = wordEnd;
            }
        }

        if (nameWords.Count == 0)
            return null;

        string funcName = string.Join("-", nameWords);
        return new FunctionTemplateInfo(funcName, parameters, returnType, span);
    }

    static bool IsFunctionTemplateMatch(string line)
    {
        string trimmed = line.TrimEnd();
        return trimmed.EndsWith(':')
            && trimmed.TrimStart().StartsWith("To ", StringComparison.OrdinalIgnoreCase);
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
            if (indent >= 2 && LooksLikeNotation(trimmed))
                break;

            if (trimmed.StartsWith('-')
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

        // Detect transition markers
        ProseTransitionKind transition = ProseTransitionKind.None;
        if (text.TrimEnd().EndsWith("We say:", StringComparison.OrdinalIgnoreCase))
            transition = ProseTransitionKind.WeSay;
        else if (text.TrimEnd().EndsWith("This is written:", StringComparison.OrdinalIgnoreCase))
            transition = ProseTransitionKind.ThisIsWritten;

        // Check for function template on the last non-empty line
        FunctionTemplateInfo? funcTemplate = null;
        string lastLine = proseLines.Count > 0 ? proseLines[^1] : "";
        if (lastLine.TrimEnd().EndsWith(':') && lastLine.TrimStart().StartsWith("To ", StringComparison.OrdinalIgnoreCase))
        {
            funcTemplate = TryMatchFunctionTemplate(lastLine, proseSpan);
        }

        // Extract inline references from prose text
        List<InlineCodeRef> codeRefs = ExtractCodeRefs(text);
        List<InlineTypeRef> typeRefs = ExtractTypeRefs(text);

        members.Add(new ProseBlockNode(text, proseSpan)
        {
            Transition = transition,
            FunctionTemplate = funcTemplate,
            CodeRefs = codeRefs,
            TypeRefs = typeRefs
        });

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
            if (!trimmed.StartsWith('-'))
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
            if (!trimmed.StartsWith('-'))
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
            return null;

        string fieldName = ToFieldName(bullet[..colonIdx].Trim());
        string fieldType = bullet[(colonIdx + 1)..].Trim();
        if (fieldName.Length == 0 || fieldType.Length == 0)
            return null;

        Token nameToken = MakeSyntheticToken(
            TokenKind.Identifier, fieldName, span);
        TypeNode typeNode = ParseTypeFromText(fieldType, span);
        return new RecordTypeFieldNode(nameToken, typeNode, span);
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
        string[] words = text.Split(new char[] { ' ', '-' }, StringSplitOptions.RemoveEmptyEntries);
        return string.Concat(words.Select(w =>
            char.ToUpperInvariant(w[0]) + w[1..].ToLowerInvariant()));
    }

    static string ToFieldName(string text)
    {
        string[] words = text.Split(new char[] { ' ', '-' }, StringSplitOptions.RemoveEmptyEntries);
        if (words.Length == 0) return text;
        string first = words[0].ToLowerInvariant();
        string rest = string.Concat(words.Skip(1).Select(w =>
            char.ToUpperInvariant(w[0]) + w[1..].ToLowerInvariant()));
        return first + rest;
    }

    static List<InlineCodeRef> ExtractCodeRefs(string text)
    {
        List<InlineCodeRef> refs = [];
        int i = 0;
        while (i < text.Length)
        {
            if (text[i] == '`')
            {
                int end = text.IndexOf('`', i + 1);
                if (end > i + 1)
                {
                    string code = text[(i + 1)..end];
                    if (code.Length > 0 && !code.Contains('`'))
                        refs.Add(new InlineCodeRef(code, i, end + 1));
                    i = end + 1;
                }
                else
                {
                    i++;
                }
            }
            else
            {
                i++;
            }
        }
        return refs;
    }

    static List<InlineTypeRef> ExtractTypeRefs(string text)
    {
        List<InlineTypeRef> refs = [];
        int i = 0;
        while (i < text.Length)
        {
            // Look for standalone PascalCase words (not inside backticks)
            if (char.IsUpper(text[i]) && (i == 0 || !char.IsLetterOrDigit(text[i - 1])))
            {
                int end = i + 1;
                while (end < text.Length && (char.IsLetterOrDigit(text[end]) || text[end] == '-'))
                    end++;
                string word = text[i..end];
                // Must be PascalCase (starts upper, has at least one lower)
                if (word.Length >= 2 && word.Any(char.IsLower)
                    && !IsCommonEnglishWord(word))
                {
                    refs.Add(new InlineTypeRef(word, i, end));
                }
                i = end;
            }
            else
            {
                i++;
            }
        }
        return refs;
    }

    static bool IsCommonEnglishWord(string word)
    {
        return word is "The" or "This" or "That" or "These" or "Those"
            or "An" or "And" or "Are" or "But" or "Can" or "Did"
            or "For" or "From" or "Has" or "Have" or "How" or "Its"
            or "Let" or "May" or "Not" or "Our" or "She" or "Was"
            or "We" or "When" or "Who" or "Will" or "With" or "You"
            or "Also" or "Each" or "Every" or "Given" or "Here"
            or "Into" or "Just" or "More" or "Must" or "Note"
            or "Only" or "Over" or "Some" or "Such" or "Than"
            or "Then" or "They" or "Very" or "What" or "Your"
            or "After" or "Before" or "Below" or "First" or "Never"
            or "Section" or "Chapter" or "Claim" or "Proof" or "True" or "False";
    }
}
