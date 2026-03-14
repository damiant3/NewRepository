namespace Codex.Core;

/// <summary>
/// A position in source text. Zero-indexed offset, one-indexed line and column for display.
/// </summary>
public readonly record struct SourcePosition(int Offset, int Line, int Column)
{
    public override string ToString() => $"({Line}:{Column})";
}

/// <summary>
/// A contiguous span of source text, defined by start and end positions.
/// Every token, every AST node, every diagnostic carries a span back to the source.
/// </summary>
public readonly record struct SourceSpan(SourcePosition Start, SourcePosition End)
{
    /// <summary>The length of this span in characters.</summary>
    public int Length => End.Offset - Start.Offset;

    /// <summary>A synthetic span for generated nodes that have no source.</summary>
    public static readonly SourceSpan s_synthetic = new(
        new SourcePosition(0, 0, 0),
        new SourcePosition(0, 0, 0));

    /// <summary>Create a span covering a single character.</summary>
    public static SourceSpan Single(int offset, int line, int column) =>
        new(new SourcePosition(offset, line, column),
            new SourcePosition(offset + 1, line, column + 1));

    /// <summary>Create a span that covers both this span and another.</summary>
    public SourceSpan Through(SourceSpan other) =>
        new(Start.Offset <= other.Start.Offset ? Start : other.Start,
            End.Offset >= other.End.Offset ? End : other.End);

    public override string ToString() => $"{Start}-{End}";
}

/// <summary>
/// A named source — the file or input stream that source text came from.
/// </summary>
public sealed class SourceText
{
    private int[]? m_lineStarts;

    public string FileName { get; }
    public string Content { get; }

    public SourceText(string fileName, string content)
    {
        FileName = fileName;
        Content = content;
    }

    /// <summary>Get the text within a span.</summary>
    public string GetText(SourceSpan span)
    {
        return Content.Substring(span.Start.Offset, span.Length);
    }

    /// <summary>Get the line starts array (lazily computed).</summary>
    public ReadOnlySpan<int> LineStarts
    {
        get
        {
            m_lineStarts ??= ComputeLineStarts();
            return m_lineStarts;
        }
    }

    /// <summary>Get the SourcePosition for a given character offset.</summary>
    public SourcePosition GetPosition(int offset)
    {
        ReadOnlySpan<int> starts = LineStarts;
        int line = BinarySearchLineStarts(starts, offset);
        int column = offset - starts[line] + 1;
        return new SourcePosition(offset, line + 1, column);
    }

    private static int BinarySearchLineStarts(ReadOnlySpan<int> starts, int offset)
    {
        int lo = 0, hi = starts.Length - 1;
        while (lo <= hi)
        {
            int mid = lo + (hi - lo) / 2;
            if (starts[mid] <= offset)
            {
                lo = mid + 1;
            }
            else
            {
                hi = mid - 1;
            }
        }
        return hi;
    }

    private int[] ComputeLineStarts()
    {
        List<int> starts = new List<int> { 0 };
        for (int i = 0; i < Content.Length; i++)
        {
            if (Content[i] == '\n')
            {
                starts.Add(i + 1);
            }
            else if (Content[i] == '\r')
            {
                if (i + 1 < Content.Length && Content[i + 1] == '\n')
                {
                    i++;
                }
                starts.Add(i + 1);
            }
        }
        return starts.ToArray();
    }
}
