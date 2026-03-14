using Codex.Core;

namespace Codex.Syntax;

/// <summary>
/// A single token produced by the lexer.
/// Tokens are immutable values. Every token knows where it came from in the source.
/// </summary>
public sealed record Token(
    TokenKind Kind,
    string Text,
    SourceSpan Span)
{
    /// <summary>For literal tokens, the parsed value (e.g., the integer value, the unescaped string).</summary>
    public object? LiteralValue { get; init; }

    public override string ToString() => $"{Kind}({Text})";
}
