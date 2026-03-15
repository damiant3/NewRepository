using Codex.Core;

namespace Codex.Syntax;

public sealed class ProseParser
{
    readonly SourceText m_source;
    readonly DiagnosticBag m_diagnostics;
    readonly string[] m_lines;
    int m_lineIndex;

    public ProseParser(SourceText source, DiagnosticBag diagnostics)
    {
        m_source = source;
        m_diagnostics = diagnostics;
        m_lines = source.Content.Split('\n');
        m_lineIndex = 0;
    }

    public static bool IsProseDocument(string content)
    {
        foreach (string line in content.Split('\n'))
        {
            string trimmed = line.TrimStart();
            if (trimmed.Length == 0)
                continue;
            return trimmed.StartsWith("Chapter:", StringComparison.Ordinal);
        }
        return false;
    }

    public DocumentNode ParseDocument()
    {
        SourceSpan docSpan = MakeSpan(0, m_source.Content.Length);
        List<ChapterNode> chapters = [];

        SkipBlankLines();

        while (m_lineIndex < m_lines.Length)
        {
            string trimmed = m_lines[m_lineIndex].TrimStart();
            if (trimmed.StartsWith("Chapter:", StringComparison.Ordinal))
            {
                chapters.Add(ParseChapter());
            }
            else
            {
                SkipBlankLines();
                if (m_lineIndex < m_lines.Length)
                {
                    // Non-chapter content at top level — skip with warning
                    m_diagnostics.Warning("CDX1100",
                        "Expected a Chapter: header in a prose-mode document",
                        MakeLineSpan(m_lineIndex));
                    m_lineIndex++;
                }
            }
        }

        // Collect all definitions from all notation blocks
        List<DefinitionNode> allDefs = [];
        List<TypeDefinitionNode> allTypeDefs = [];
        foreach (ChapterNode chapter in chapters)
        {
            CollectDefinitions(chapter.Members, allDefs, allTypeDefs);
        }

        return new DocumentNode(allDefs, allTypeDefs, Array.Empty<ClaimNode>(),
            Array.Empty<ProofNode>(), chapters, docSpan);
    }

    ChapterNode ParseChapter()
    {
        int startOffset = LineOffset(m_lineIndex);
        string headerLine = m_lines[m_lineIndex].TrimStart();
        string title = headerLine["Chapter:".Length..].Trim();
        m_lineIndex++;

        List<DocumentMember> members = [];
        SkipBlankLines();

        while (m_lineIndex < m_lines.Length)
        {
            string trimmed = CurrentTrimmed();
            if (trimmed.Length == 0)
            {
                SkipBlankLines();
                continue;
            }

            // Another chapter starts — stop
            if (trimmed.StartsWith("Chapter:", StringComparison.Ordinal))
                break;

            // Section header
            if (trimmed.StartsWith("Section:", StringComparison.Ordinal))
            {
                members.Add(ParseSection());
                continue;
            }

            int indent = MeasureIndent(m_lines[m_lineIndex]);

            // Notation block: indented 4+ spaces (or deeper than prose)
            if (indent >= 4 && LooksLikeNotation(trimmed))
            {
                members.Add(ParseNotationBlock());
                continue;
            }

            // Prose block
            members.Add(ParseProseBlock());
        }

        int endOffset = m_lineIndex < m_lines.Length
            ? LineOffset(m_lineIndex)
            : m_source.Content.Length;
        SourceSpan span = MakeSpan(startOffset, endOffset);
        return new ChapterNode(title, members, span);
    }

    SectionNode ParseSection()
    {
        int startOffset = LineOffset(m_lineIndex);
        string headerLine = m_lines[m_lineIndex].TrimStart();
        string title = headerLine["Section:".Length..].Trim();
        m_lineIndex++;

        List<DocumentMember> members = [];
        SkipBlankLines();

        while (m_lineIndex < m_lines.Length)
        {
            string trimmed = CurrentTrimmed();
            if (trimmed.Length == 0)
            {
                SkipBlankLines();
                continue;
            }

            // Chapter or Section header ends this section
            if (trimmed.StartsWith("Chapter:", StringComparison.Ordinal) ||
                trimmed.StartsWith("Section:", StringComparison.Ordinal))
                break;

            int indent = MeasureIndent(m_lines[m_lineIndex]);

            if (indent >= 4 && LooksLikeNotation(trimmed))
            {
                members.Add(ParseNotationBlock());
                continue;
            }

            members.Add(ParseProseBlock());
        }

        int endOffset = m_lineIndex < m_lines.Length
            ? LineOffset(m_lineIndex)
            : m_source.Content.Length;
        SourceSpan span = MakeSpan(startOffset, endOffset);
        return new SectionNode(title, members, span);
    }

    ProseBlockNode ParseProseBlock()
    {
        int startOffset = LineOffset(m_lineIndex);
        List<string> proseLines = [];

        while (m_lineIndex < m_lines.Length)
        {
            string line = m_lines[m_lineIndex];
            string trimmed = line.TrimStart();

            // Stop at blank line
            if (trimmed.Length == 0)
                break;

            // Stop at structural headers
            if (trimmed.StartsWith("Chapter:", StringComparison.Ordinal) ||
                trimmed.StartsWith("Section:", StringComparison.Ordinal))
                break;

            int indent = MeasureIndent(line);

            // Stop at notation block (deeper indentation with code-like content)
            if (indent >= 4 && LooksLikeNotation(trimmed))
                break;

            proseLines.Add(trimmed);
            m_lineIndex++;
        }

        int endOffset = m_lineIndex < m_lines.Length
            ? LineOffset(m_lineIndex)
            : m_source.Content.Length;
        string text = string.Join("\n", proseLines);
        SourceSpan span = MakeSpan(startOffset, endOffset);
        return new ProseBlockNode(text, span);
    }

    NotationBlockNode ParseNotationBlock()
    {
        int startOffset = LineOffset(m_lineIndex);

        // Collect all notation lines (indented 4+)
        List<string> notationLines = [];
        int baseIndent = MeasureIndent(m_lines[m_lineIndex]);

        while (m_lineIndex < m_lines.Length)
        {
            string line = m_lines[m_lineIndex];
            string trimmed = line.TrimStart();

            // Blank lines within notation are kept
            if (trimmed.Length == 0)
            {
                // Check if the next non-blank line is still notation
                int peekIdx = m_lineIndex + 1;
                while (peekIdx < m_lines.Length && m_lines[peekIdx].TrimStart().Length == 0)
                    peekIdx++;

                if (peekIdx < m_lines.Length && MeasureIndent(m_lines[peekIdx]) >= baseIndent)
                {
                    notationLines.Add("");
                    m_lineIndex++;
                    continue;
                }
                break;
            }

            int indent = MeasureIndent(line);
            if (indent < baseIndent)
                break;

            // Structural headers break out
            if (trimmed.StartsWith("Chapter:", StringComparison.Ordinal) ||
                trimmed.StartsWith("Section:", StringComparison.Ordinal))
                break;

            // Dedent the notation to be relative (remove the base indent)
            string dedented = indent >= baseIndent ? line[baseIndent..] : trimmed;
            notationLines.Add(dedented);
            m_lineIndex++;
        }

        // Now parse the notation block using the standard Lexer + Parser pipeline
        string notationSource = string.Join("\n", notationLines);
        SourceText notationText = new(m_source.FileName, notationSource);
        DiagnosticBag notationDiag = new();
        Lexer lexer = new(notationText, notationDiag);
        IReadOnlyList<Token> tokens = lexer.TokenizeAll();
        Parser parser = new(tokens, notationDiag);
        DocumentNode notationDoc = parser.ParseDocument();

        // Propagate diagnostics
        foreach (Diagnostic diag in notationDiag.ToImmutable())
        {
            m_diagnostics.Add(diag);
        }

        int endOffset = m_lineIndex < m_lines.Length
            ? LineOffset(m_lineIndex)
            : m_source.Content.Length;
        SourceSpan span = MakeSpan(startOffset, endOffset);
        return new NotationBlockNode(notationDoc.Definitions, notationDoc.TypeDefinitions, span);
    }

    static bool LooksLikeNotation(string trimmed)
    {
        if (trimmed.Length == 0) return false;

        // Starts with a Codex identifier (lowercase or uppercase)
        if (char.IsLetter(trimmed[0]))
        {
            // Look for type annotation pattern: "name : Type"
            // or definition pattern: "name (param) = ..." or "name = ..."
            if (trimmed.Contains(" : ") || trimmed.Contains(" = ") || trimmed.Contains('('))
                return true;
        }

        return false;
    }

    static void CollectDefinitions(IReadOnlyList<DocumentMember> members, List<DefinitionNode> defs, List<TypeDefinitionNode> typeDefs)
    {
        foreach (DocumentMember member in members)
        {
            if (member is NotationBlockNode notation)
            {
                defs.AddRange(notation.Definitions);
                typeDefs.AddRange(notation.TypeDefinitions);
            }
            else if (member is SectionNode section)
            {
                CollectDefinitions(section.Members, defs, typeDefs);
            }
        }
    }

    string CurrentTrimmed()
    {
        return m_lineIndex < m_lines.Length ? m_lines[m_lineIndex].TrimStart() : "";
    }

    void SkipBlankLines()
    {
        while (m_lineIndex < m_lines.Length && m_lines[m_lineIndex].TrimStart().Length == 0)
        {
            m_lineIndex++;
        }
    }

    static int MeasureIndent(string line)
    {
        int count = 0;
        foreach (char c in line)
        {
            if (c == ' ') count++;
            else break;
        }
        return count;
    }

    int LineOffset(int lineIdx)
    {
        int offset = 0;
        for (int i = 0; i < lineIdx && i < m_lines.Length; i++)
        {
            offset += m_lines[i].Length + 1; // +1 for the \n
        }
        return offset;
    }

    SourceSpan MakeSpan(int startOffset, int endOffset)
    {
        SourcePosition start = OffsetToPosition(startOffset);
        SourcePosition end = OffsetToPosition(endOffset);
        return new SourceSpan(start, end);
    }

    SourcePosition OffsetToPosition(int offset)
    {
        int line = 1;
        int col = 1;
        for (int i = 0; i < offset && i < m_source.Content.Length; i++)
        {
            if (m_source.Content[i] == '\n')
            {
                line++;
                col = 1;
            }
            else
            {
                col++;
            }
        }
        return new SourcePosition(offset, line, col);
    }

    SourceSpan MakeLineSpan(int lineIdx)
    {
        int offset = LineOffset(lineIdx);
        int length = lineIdx < m_lines.Length ? m_lines[lineIdx].Length : 0;
        return MakeSpan(offset, offset + length);
    }
}
