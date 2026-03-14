namespace Codex.Syntax;

/// <summary>
/// Every kind of token the Codex lexer can produce.
/// </summary>
public enum TokenKind
{
    // Structural
    EndOfFile,
    Newline,
    Indent,
    Dedent,

    // Literals
    IntegerLiteral,
    NumberLiteral,
    TextLiteral,
    TrueKeyword,
    FalseKeyword,

    // Identifiers
    Identifier,         // lowercase-hyphenated
    TypeIdentifier,     // Capitalized

    // Prose
    ProseText,          // natural language text in prose mode
    ChapterHeader,      // "Chapter: ..."
    SectionHeader,      // "Section: ..."

    // Keywords
    LetKeyword,
    InKeyword,
    IfKeyword,
    ThenKeyword,
    ElseKeyword,
    WhenKeyword,
    WhereKeyword,
    SuchThatKeyword,
    DoKeyword,
    RecordKeyword,
    ImportKeyword,
    ExportKeyword,
    ClaimKeyword,
    ProofKeyword,
    ForAllKeyword,
    ThereExistsKeyword,
    LinearKeyword,

    // Operators
    Equals,             // =
    Colon,              // :
    Arrow,              // → or ->
    LeftArrow,          // ← or <-
    Pipe,               // |
    Ampersand,          // &
    Plus,               // +
    Minus,              // -
    Star,               // *
    Slash,              // /
    Caret,              // ^
    PlusPlus,           // ++
    ColonColon,         // ::
    DoubleEquals,       // ==
    NotEquals,          // ≠ or /=
    LessThan,           // <
    GreaterThan,        // >
    LessOrEqual,        // ≤ or <=
    GreaterOrEqual,     // ≥ or >=
    TripleEquals,       // ≡ or ===
    Turnstile,          // ⊢ or |-
    LinearProduct,      // ⊗ or (**)
    ForAllSymbol,       // ∀
    ExistsSymbol,       // ∃

    // Delimiters
    LeftParen,          // (
    RightParen,         // )
    LeftBracket,        // [
    RightBracket,       // ]
    LeftBrace,          // {
    RightBrace,         // }
    Comma,              // ,
    Dot,                // .
    DashGreater,        // -> (ASCII arrow)
    Underscore,         // _

    // Special
    Error,              // unrecognized input
}
