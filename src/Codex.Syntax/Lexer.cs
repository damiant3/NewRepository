using Codex.Core;

namespace Codex.Syntax;

/// <summary>
/// The mode the lexer is currently in. Prose mode recognizes natural language.
/// Notation mode recognizes formal code.
/// </summary>
public enum LexerMode
{
    Prose,
    Notation
}

/// <summary>
/// Hand-written lexer for Codex source. Indentation-sensitive with prose/notation mode switching.
/// </summary>
public sealed class Lexer
{
    private readonly SourceText m_source;
    private readonly DiagnosticBag m_diagnostics;
    private readonly string m_text;

    private int m_position;
    private int m_line;
    private int m_column;

    private readonly Stack<int> m_indentStack = new();
    private int m_currentLineIndent;
    private bool m_atLineStart = true;

    private readonly List<Token> m_pending = [];

    public Lexer(SourceText source, DiagnosticBag diagnostics)
    {
        m_source = source;
        m_diagnostics = diagnostics;
        m_text = source.Content;
        m_position = 0;
        m_line = 1;
        m_column = 1;
        m_indentStack.Push(0);
    }

    public DiagnosticBag Diagnostics => m_diagnostics;

    /// <summary>Tokenize the entire source into a list of tokens.</summary>
    public IReadOnlyList<Token> TokenizeAll()
    {
        List<Token> tokens = new List<Token>();
        while (true)
        {
            Token token = NextToken();
            tokens.Add(token);
            if (token.Kind == TokenKind.EndOfFile)
            {
                break;
            }
        }
        return tokens;
    }

    /// <summary>Produce the next token.</summary>
    public Token NextToken()
    {
        if (m_pending.Count > 0)
        {
            Token t = m_pending[0];
            m_pending.RemoveAt(0);
            return t;
        }

        if (m_atLineStart)
        {
            SkipBlankLines();
            if (IsAtEnd)
            {
                return EmitDedentsToZero();
            }

            m_currentLineIndent = MeasureIndentation();
            EmitIndentationTokens();
            m_atLineStart = false;

            if (m_pending.Count > 0)
            {
                Token t = m_pending[0];
                m_pending.RemoveAt(0);
                return t;
            }
        }

        if (IsAtEnd)
        {
            return EmitDedentsToZero();
        }

        return ScanToken();
    }

    private Token ScanToken()
    {
        SkipSpaces();

        if (IsAtEnd)
        {
            return EmitDedentsToZero();
        }

        char c = Current;

        if (c == '\n' || c == '\r')
        {
            SourcePosition start = MakePosition();
            ConsumeNewline();
            m_atLineStart = true;
            return new Token(TokenKind.Newline, "\\n", MakeSpan(start));
        }

        if (c == '"')
        {
            return ScanTextLiteral();
        }

        if (char.IsDigit(c))
        {
            return ScanNumber();
        }

        if (char.IsLetter(c) || c == '_')
        {
            return ScanIdentifierOrKeyword();
        }

        return ScanOperator();
    }

    private Token ScanTextLiteral()
    {
        SourcePosition start = MakePosition();
        Advance();

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        while (!IsAtEnd && Current != '"' && Current != '\n')
        {
            if (Current == '\\' && m_position + 1 < m_text.Length)
            {
                Advance();
                sb.Append(Current switch
                {
                    'n' => '\n',
                    't' => '\t',
                    'r' => '\r',
                    '\\' => '\\',
                    '"' => '"',
                    _ => Current
                });
                Advance();
            }
            else
            {
                sb.Append(Current);
                Advance();
            }
        }

        if (IsAtEnd || Current == '\n')
        {
            m_diagnostics.Error("CDX0001", "Unterminated text literal", MakeSpan(start));
        }
        else
        {
            Advance();
        }

        SourceSpan span = MakeSpan(start);
        return new Token(TokenKind.TextLiteral, m_text[start.Offset..span.End.Offset], span)
        {
            LiteralValue = sb.ToString()
        };
    }

    private Token ScanNumber()
    {
        SourcePosition start = MakePosition();
        bool isFloat = false;

        while (!IsAtEnd && (char.IsDigit(Current) || Current == '_'))
        {
            Advance();
        }

        if (!IsAtEnd && Current == '.' && m_position + 1 < m_text.Length && char.IsDigit(m_text[m_position + 1]))
        {
            isFloat = true;
            Advance();
            while (!IsAtEnd && (char.IsDigit(Current) || Current == '_'))
            {
                Advance();
            }
        }

        SourceSpan span = MakeSpan(start);
        string text = m_text[start.Offset..span.End.Offset];
        string cleanText = text.Replace("_", "");

        if (isFloat)
        {
            return new Token(TokenKind.NumberLiteral, text, span)
            {
                LiteralValue = decimal.Parse(cleanText, System.Globalization.CultureInfo.InvariantCulture)
            };
        }
        else
        {
            return new Token(TokenKind.IntegerLiteral, text, span)
            {
                LiteralValue = long.Parse(cleanText, System.Globalization.CultureInfo.InvariantCulture)
            };
        }
    }

    private Token ScanIdentifierOrKeyword()
    {
        SourcePosition start = MakePosition();

        while (!IsAtEnd && (char.IsLetterOrDigit(Current) || Current == '_'))
        {
            Advance();
        }

        while (!IsAtEnd && Current == '-' && m_position + 1 < m_text.Length && char.IsLetter(m_text[m_position + 1]))
        {
            Advance();
            while (!IsAtEnd && (char.IsLetterOrDigit(Current) || Current == '_'))
            {
                Advance();
            }
        }

        SourceSpan span = MakeSpan(start);
        string text = m_text[start.Offset..span.End.Offset];

        TokenKind kind = ClassifyWord(text);
        Token token = new Token(kind, text, span);

        if (kind == TokenKind.TrueKeyword)
        {
            return token with { LiteralValue = true };
        }
        if (kind == TokenKind.FalseKeyword)
        {
            return token with { LiteralValue = false };
        }

        return token;
    }

    private static TokenKind ClassifyWord(string text) => text switch
    {
        "let" => TokenKind.LetKeyword,
        "in" => TokenKind.InKeyword,
        "if" => TokenKind.IfKeyword,
        "then" => TokenKind.ThenKeyword,
        "else" => TokenKind.ElseKeyword,
        "when" => TokenKind.WhenKeyword,
        "where" => TokenKind.WhereKeyword,
        "do" => TokenKind.DoKeyword,
        "record" => TokenKind.RecordKeyword,
        "import" => TokenKind.ImportKeyword,
        "export" => TokenKind.ExportKeyword,
        "claim" => TokenKind.ClaimKeyword,
        "proof" => TokenKind.ProofKeyword,
        "forall" => TokenKind.ForAllKeyword,
        "exists" => TokenKind.ThereExistsKeyword,
        "linear" => TokenKind.LinearKeyword,
        "True" => TokenKind.TrueKeyword,
        "False" => TokenKind.FalseKeyword,
        _ => char.IsUpper(text[0]) ? TokenKind.TypeIdentifier : TokenKind.Identifier
    };

    private Token ScanOperator()
    {
        SourcePosition start = MakePosition();
        char c = Current;
        Advance();

        switch (c)
        {
            case '(' : return new Token(TokenKind.LeftParen, "(", MakeSpan(start));
            case ')' : return new Token(TokenKind.RightParen, ")", MakeSpan(start));
            case '[' : return new Token(TokenKind.LeftBracket, "[", MakeSpan(start));
            case ']' : return new Token(TokenKind.RightBracket, "]", MakeSpan(start));
            case '{' : return new Token(TokenKind.LeftBrace, "{", MakeSpan(start));
            case '}' : return new Token(TokenKind.RightBrace, "}", MakeSpan(start));
            case ',' : return new Token(TokenKind.Comma, ",", MakeSpan(start));
            case '.' : return new Token(TokenKind.Dot, ".", MakeSpan(start));
            case '^' : return new Token(TokenKind.Caret, "^", MakeSpan(start));
            case '_' : return new Token(TokenKind.Underscore, "_", MakeSpan(start));

            case '+':
                if (!IsAtEnd && Current == '+') { Advance(); return new Token(TokenKind.PlusPlus, "++", MakeSpan(start)); }
                return new Token(TokenKind.Plus, "+", MakeSpan(start));

            case '-':
                if (!IsAtEnd && Current == '>') { Advance(); return new Token(TokenKind.Arrow, "->", MakeSpan(start)); }
                return new Token(TokenKind.Minus, "-", MakeSpan(start));

            case '*' : return new Token(TokenKind.Star, "*", MakeSpan(start));

            case '/' :
                if (!IsAtEnd && Current == '=') { Advance(); return new Token(TokenKind.NotEquals, "/=", MakeSpan(start)); }
                return new Token(TokenKind.Slash, "/", MakeSpan(start));

            case '=' :
                if (!IsAtEnd && Current == '=')
                {
                    Advance();
                    if (!IsAtEnd && Current == '=') { Advance(); return new Token(TokenKind.TripleEquals, "===", MakeSpan(start)); }
                    return new Token(TokenKind.DoubleEquals, "==", MakeSpan(start));
                }
                return new Token(TokenKind.Equals, "=", MakeSpan(start));

            case ':':
                if (!IsAtEnd && Current == ':') { Advance(); return new Token(TokenKind.ColonColon, "::", MakeSpan(start)); }
                return new Token(TokenKind.Colon, ":", MakeSpan(start));

            case '|':
                if (!IsAtEnd && Current == '-') { Advance(); return new Token(TokenKind.Turnstile, "|-", MakeSpan(start)); }
                return new Token(TokenKind.Pipe, "|", MakeSpan(start));

            case '&' : return new Token(TokenKind.Ampersand, "&", MakeSpan(start));

            case '<':
                if (!IsAtEnd && Current == '=') { Advance(); return new Token(TokenKind.LessOrEqual, "<=", MakeSpan(start)); }
                if (!IsAtEnd && Current == '-') { Advance(); return new Token(TokenKind.LeftArrow, "<-", MakeSpan(start)); }
                return new Token(TokenKind.LessThan, "<", MakeSpan(start));

            case '>':
                if (!IsAtEnd && Current == '=') { Advance(); return new Token(TokenKind.GreaterOrEqual, ">=", MakeSpan(start)); }
                return new Token(TokenKind.GreaterThan, ">", MakeSpan(start));

            case '→' : return new Token(TokenKind.Arrow, "→", MakeSpan(start));
            case '←' : return new Token(TokenKind.LeftArrow, "←", MakeSpan(start));
            case '∀' : return new Token(TokenKind.ForAllSymbol, "∀", MakeSpan(start));
            case '∃' : return new Token(TokenKind.ExistsSymbol, "∃", MakeSpan(start));
            case '≡' : return new Token(TokenKind.TripleEquals, "≡", MakeSpan(start));
            case '≠' : return new Token(TokenKind.NotEquals, "≠", MakeSpan(start));
            case '≤' : return new Token(TokenKind.LessOrEqual, "≤", MakeSpan(start));
            case '≥' : return new Token(TokenKind.GreaterOrEqual, "≥", MakeSpan(start));
            case '⊢' : return new Token(TokenKind.Turnstile, "⊢", MakeSpan(start));
            case '⊗' : return new Token(TokenKind.LinearProduct, "⊗", MakeSpan(start));

            default:
                m_diagnostics.Error("CDX0002", $"Unexpected character '{c}'", MakeSpan(start));
                return new Token(TokenKind.Error, c.ToString(), MakeSpan(start));
        }
    }

    // --- Indentation handling ---

    private void SkipBlankLines()
    {
        while (!IsAtEnd)
        {
            int saved = m_position;
            int savedLine = m_line;
            int savedCol = m_column;

            while (!IsAtEnd && Current == ' ')
            {
                Advance();
            }

            if (IsAtEnd || Current == '\n' || Current == '\r')
            {
                if (!IsAtEnd)
                {
                    ConsumeNewline();
                }
                continue;
            }

            m_position = saved;
            m_line = savedLine;
            m_column = savedCol;
            break;
        }
    }

    private int MeasureIndentation()
    {
        int indent = 0;
        while (!IsAtEnd && Current == ' ')
        {
            indent++;
            Advance();
        }
        return indent;
    }

    private void EmitIndentationTokens()
    {
        int indent = m_currentLineIndent;
        int currentIndent = m_indentStack.Peek();
        SourcePosition pos = MakePosition();
        SourceSpan span = SourceSpan.Single(pos.Offset, pos.Line, pos.Column);

        if (indent > currentIndent)
        {
            m_indentStack.Push(indent);
            m_pending.Add(new Token(TokenKind.Indent, "<indent>", span));
        }
        else
        {
            while (indent < m_indentStack.Peek())
            {
                m_indentStack.Pop();
                m_pending.Add(new Token(TokenKind.Dedent, "<dedent>", span));
            }

            if (indent != m_indentStack.Peek())
            {
                m_diagnostics.Warning("CDX0003", "Indentation does not match any outer level", span);
            }
        }
    }

    private Token EmitDedentsToZero()
    {
        SourcePosition pos = MakePosition();
        SourceSpan span = SourceSpan.Single(pos.Offset, pos.Line, pos.Column);

        while (m_indentStack.Count > 1)
        {
            m_indentStack.Pop();
            m_pending.Add(new Token(TokenKind.Dedent, "<dedent>", span));
        }

        if (m_pending.Count > 0)
        {
            Token t = m_pending[0];
            m_pending.RemoveAt(0);
            m_pending.Add(new Token(TokenKind.EndOfFile, "", span));
            return t;
        }

        return new Token(TokenKind.EndOfFile, "", span);
    }

    // --- Character-level helpers ---

    private bool IsAtEnd => m_position >= m_text.Length;

    private char Current => m_text[m_position];

    private void Advance()
    {
        if (m_position < m_text.Length)
        {
            m_column++;
            m_position++;
        }
    }

    private void ConsumeNewline()
    {
        if (Current == '\r')
        {
            m_position++;
            if (m_position < m_text.Length && m_text[m_position] == '\n')
            {
                m_position++;
            }
        }
        else if (Current == '\n')
        {
            m_position++;
        }
        m_line++;
        m_column = 1;
    }

    private void SkipSpaces()
    {
        while (!IsAtEnd && Current == ' ')
        {
            Advance();
        }
    }

    private SourcePosition MakePosition() => new(m_position, m_line, m_column);

    private SourceSpan MakeSpan(SourcePosition start) =>
        new(start, MakePosition());
}
