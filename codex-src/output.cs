using System;
using System.Collections.Generic;
using System.Linq;

public sealed record Name(string value);

public sealed record LexState(string source, long offset, long line, long column);

public sealed record Token(TokenKind kind, string text, long offset, long line, long column);

public sealed record SourcePosition(long line, long column, long offset);

public abstract record DiagnosticSeverity;

public sealed record Error : DiagnosticSeverity;
public sealed record Warning : DiagnosticSeverity;
public sealed record Info : DiagnosticSeverity;

public abstract record TokenKind;

public sealed record EndOfFile : TokenKind;
public sealed record Newline : TokenKind;
public sealed record Indent : TokenKind;
public sealed record Dedent : TokenKind;
public sealed record IntegerLiteral : TokenKind;
public sealed record NumberLiteral : TokenKind;
public sealed record TextLiteral : TokenKind;
public sealed record TrueKeyword : TokenKind;
public sealed record FalseKeyword : TokenKind;
public sealed record Identifier : TokenKind;
public sealed record TypeIdentifier : TokenKind;
public sealed record ProseText : TokenKind;
public sealed record ChapterHeader : TokenKind;
public sealed record SectionHeader : TokenKind;
public sealed record LetKeyword : TokenKind;
public sealed record InKeyword : TokenKind;
public sealed record IfKeyword : TokenKind;
public sealed record ThenKeyword : TokenKind;
public sealed record ElseKeyword : TokenKind;
public sealed record WhenKeyword : TokenKind;
public sealed record WhereKeyword : TokenKind;
public sealed record SuchThatKeyword : TokenKind;
public sealed record DoKeyword : TokenKind;
public sealed record RecordKeyword : TokenKind;
public sealed record ImportKeyword : TokenKind;
public sealed record ExportKeyword : TokenKind;
public sealed record ClaimKeyword : TokenKind;
public sealed record ProofKeyword : TokenKind;
public sealed record ForAllKeyword : TokenKind;
public sealed record ThereExistsKeyword : TokenKind;
public sealed record LinearKeyword : TokenKind;
public sealed record Equals : TokenKind;
public sealed record Colon : TokenKind;
public sealed record Arrow : TokenKind;
public sealed record LeftArrow : TokenKind;
public sealed record Pipe : TokenKind;
public sealed record Ampersand : TokenKind;
public sealed record Plus : TokenKind;
public sealed record Minus : TokenKind;
public sealed record Star : TokenKind;
public sealed record Slash : TokenKind;
public sealed record Caret : TokenKind;
public sealed record PlusPlus : TokenKind;
public sealed record ColonColon : TokenKind;
public sealed record DoubleEquals : TokenKind;
public sealed record NotEquals : TokenKind;
public sealed record LessThan : TokenKind;
public sealed record GreaterThan : TokenKind;
public sealed record LessOrEqual : TokenKind;
public sealed record GreaterOrEqual : TokenKind;
public sealed record TripleEquals : TokenKind;
public sealed record Turnstile : TokenKind;
public sealed record LinearProduct : TokenKind;
public sealed record ForAllSymbol : TokenKind;
public sealed record ExistsSymbol : TokenKind;
public sealed record LeftParen : TokenKind;
public sealed record RightParen : TokenKind;
public sealed record LeftBracket : TokenKind;
public sealed record RightBracket : TokenKind;
public sealed record LeftBrace : TokenKind;
public sealed record RightBrace : TokenKind;
public sealed record Comma : TokenKind;
public sealed record Dot : TokenKind;
public sealed record DashGreater : TokenKind;
public sealed record Underscore : TokenKind;
public sealed record ErrorToken : TokenKind;

public sealed record Diagnostic(string code, string message, DiagnosticSeverity severity);

public abstract record LexResult;

public sealed record LexToken(Token Field0, LexState Field1) : LexResult;
public sealed record LexEnd : LexResult;

public sealed record SourceSpan(SourcePosition start, SourcePosition end, string file);

public static class Codex_codex_src
{
    public static Diagnostic make_error(string code, string msg)
    {
        return new Diagnostic(code, msg, new Error());
    }

    public static Diagnostic make_warning(string code, string msg)
    {
        return new Diagnostic(code, msg, new Warning());
    }

    public static string severity_label(DiagnosticSeverity s)
    {
        return ((Func<DiagnosticSeverity, string>)((_scrutinee_) => (_scrutinee_ is Error _mError_ ? "error" : (_scrutinee_ is Warning _mWarning_ ? "warning" : (_scrutinee_ is Info _mInfo_ ? "info" : throw new InvalidOperationException("Non-exhaustive match"))))))(s);
    }

    public static string diagnostic_display(Diagnostic d)
    {
        return string.Concat(severity_label(d.severity), string.Concat(" ", string.Concat(d.code, string.Concat(": ", d.message))));
    }

    public static Name make_name(string s)
    {
        return new Name(s);
    }

    public static string name_value(Name n)
    {
        return n.value;
    }

    public static SourcePosition make_position(long line, long col, long offset)
    {
        return new SourcePosition(line, col, offset);
    }

    public static SourceSpan make_span(SourcePosition s, SourcePosition e, string f)
    {
        return new SourceSpan(s, e, f);
    }

    public static long span_length(SourceSpan span)
    {
        return (span.end.offset - span.start.offset);
    }

    public static LexState make_lex_state(string src)
    {
        return new LexState(src, 0L, 1L, 1L);
    }

    public static bool is_at_end(LexState st)
    {
        return (st.offset >= ((long)st.source.Length));
    }

    public static string peek_char(LexState st)
    {
        return (is_at_end(st) ? "" : st.source[(int)st.offset].ToString());
    }

    public static LexState advance_char(LexState st)
    {
        return ((peek_char(st) == "\n") ? new LexState(st.source, (st.offset + 1L), (st.line + 1L), 1L) : new LexState(st.source, (st.offset + 1L), st.line, (st.column + 1L)));
    }

    public static LexState skip_spaces(LexState st)
    {
        return (is_at_end(st) ? st : ((peek_char(st) == " ") ? skip_spaces(advance_char(st)) : st));
    }

    public static LexState scan_ident_rest(LexState st)
    {
        return (is_at_end(st) ? st : ((Func<string, LexState>)((ch) => ((ch.Length > 0 && char.IsLetter(ch[0])) ? scan_ident_rest(advance_char(st)) : ((ch.Length > 0 && char.IsDigit(ch[0])) ? scan_ident_rest(advance_char(st)) : ((ch == "_") ? scan_ident_rest(advance_char(st)) : ((ch == "-") ? ((Func<LexState, LexState>)((next) => (is_at_end(next) ? st : ((peek_char(next).Length > 0 && char.IsLetter(peek_char(next)[0])) ? scan_ident_rest(next) : st))))(advance_char(st)) : st))))))(peek_char(st)));
    }

    public static LexState scan_digits(LexState st)
    {
        return (is_at_end(st) ? st : ((Func<string, LexState>)((ch) => ((ch.Length > 0 && char.IsDigit(ch[0])) ? scan_digits(advance_char(st)) : ((ch == "_") ? scan_digits(advance_char(st)) : st))))(peek_char(st)));
    }

    public static LexState scan_string_body(LexState st)
    {
        return (is_at_end(st) ? st : ((Func<string, LexState>)((ch) => ((ch == "\"") ? advance_char(st) : ((ch == "\n") ? st : ((ch == "\\") ? scan_string_body(advance_char(advance_char(st))) : scan_string_body(advance_char(st)))))))(peek_char(st)));
    }

    public static TokenKind classify_word(string w)
    {
        return ((w == "let") ? new LetKeyword() : ((w == "in") ? new InKeyword() : ((w == "if") ? new IfKeyword() : ((w == "then") ? new ThenKeyword() : ((w == "else") ? new ElseKeyword() : ((w == "when") ? new WhenKeyword() : ((w == "where") ? new WhereKeyword() : ((w == "do") ? new DoKeyword() : ((w == "record") ? new RecordKeyword() : ((w == "import") ? new ImportKeyword() : ((w == "export") ? new ExportKeyword() : ((w == "claim") ? new ClaimKeyword() : ((w == "proof") ? new ProofKeyword() : ((w == "forall") ? new ForAllKeyword() : ((w == "exists") ? new ThereExistsKeyword() : ((w == "linear") ? new LinearKeyword() : ((w == "True") ? new TrueKeyword() : ((w == "False") ? new FalseKeyword() : ((Func<long, TokenKind>)((first_code) => ((first_code >= 65L) ? ((first_code <= 90L) ? new TypeIdentifier() : new Identifier()) : new Identifier())))(((long)w[(int)0L].ToString()[0]))))))))))))))))))));
    }

    public static Token make_token(TokenKind kind, string text, LexState st)
    {
        return new Token(kind, text, st.offset, st.line, st.column);
    }

    public static string extract_text(LexState st, long start, LexState end_st)
    {
        return st.source.Substring((int)start, (int)(end_st.offset - start));
    }

    public static LexResult scan_token(LexState st)
    {
        return ((Func<LexState, LexResult>)((s) => (is_at_end(s) ? new LexEnd() : ((Func<string, LexResult>)((ch) => ((ch == "\n") ? new LexToken(make_token(new Newline(), "\n", s), advance_char(s)) : ((ch == "\"") ? ((Func<long, LexResult>)((start) => ((Func<LexState, LexResult>)((after) => new LexToken(make_token(new TextLiteral(), extract_text(s, start, after), s), after)))(scan_string_body(advance_char(s)))))(s.offset) : ((ch.Length > 0 && char.IsLetter(ch[0])) ? ((Func<long, LexResult>)((start) => ((Func<LexState, LexResult>)((after) => ((Func<string, LexResult>)((word) => new LexToken(make_token(classify_word(word), word, s), after)))(extract_text(s, start, after))))(scan_ident_rest(advance_char(s)))))(s.offset) : ((ch == "_") ? ((Func<long, LexResult>)((start) => ((Func<LexState, LexResult>)((after) => ((Func<string, LexResult>)((word) => ((((long)word.Length) == 1L) ? new LexToken(make_token(new Underscore(), "_", s), after) : new LexToken(make_token(classify_word(word), word, s), after))))(extract_text(s, start, after))))(scan_ident_rest(advance_char(s)))))(s.offset) : ((ch.Length > 0 && char.IsDigit(ch[0])) ? ((Func<long, LexResult>)((start) => ((Func<LexState, LexResult>)((after) => (is_at_end(after) ? new LexToken(make_token(new IntegerLiteral(), extract_text(s, start, after), s), after) : ((peek_char(after) == ".") ? ((Func<LexState, LexResult>)((after2) => new LexToken(make_token(new NumberLiteral(), extract_text(s, start, after2), s), after2)))(scan_digits(advance_char(after))) : new LexToken(make_token(new IntegerLiteral(), extract_text(s, start, after), s), after)))))(scan_digits(advance_char(s)))))(s.offset) : scan_operator(s))))))))(peek_char(s)))))(skip_spaces(st));
    }

    public static LexResult scan_operator(LexState s)
    {
        return ((Func<string, LexResult>)((ch) => ((Func<LexState, LexResult>)((next) => ((ch == "(") ? new LexToken(make_token(new LeftParen(), "(", s), next) : ((ch == ")") ? new LexToken(make_token(new RightParen(), ")", s), next) : ((ch == "[") ? new LexToken(make_token(new LeftBracket(), "[", s), next) : ((ch == "]") ? new LexToken(make_token(new RightBracket(), "]", s), next) : ((ch == "{") ? new LexToken(make_token(new LeftBrace(), "{", s), next) : ((ch == "}") ? new LexToken(make_token(new RightBrace(), "}", s), next) : ((ch == ",") ? new LexToken(make_token(new Comma(), ",", s), next) : ((ch == ".") ? new LexToken(make_token(new Dot(), ".", s), next) : ((ch == "^") ? new LexToken(make_token(new Caret(), "^", s), next) : ((ch == "&") ? new LexToken(make_token(new Ampersand(), "&", s), next) : scan_multi_char_operator(s)))))))))))))(advance_char(s))))(peek_char(s));
    }

    public static LexResult scan_multi_char_operator(LexState s)
    {
        return ((Func<string, LexResult>)((ch) => ((Func<LexState, LexResult>)((next) => ((Func<string, LexResult>)((next_ch) => ((ch == "+") ? ((next_ch == "+") ? new LexToken(make_token(new PlusPlus(), "++", s), advance_char(next)) : new LexToken(make_token(new Plus(), "+", s), next)) : ((ch == "-") ? ((next_ch == ">") ? new LexToken(make_token(new Arrow(), "->", s), advance_char(next)) : new LexToken(make_token(new Minus(), "-", s), next)) : ((ch == "*") ? new LexToken(make_token(new Star(), "*", s), next) : ((ch == "/") ? ((next_ch == "=") ? new LexToken(make_token(new NotEquals(), "/=", s), advance_char(next)) : new LexToken(make_token(new Slash(), "/", s), next)) : ((ch == "=") ? ((next_ch == "=") ? ((Func<LexState, LexResult>)((next2) => ((Func<string, LexResult>)((next2_ch) => ((next2_ch == "=") ? new LexToken(make_token(new TripleEquals(), "===", s), advance_char(next2)) : new LexToken(make_token(new DoubleEquals(), "==", s), next2))))((is_at_end(next2) ? "" : peek_char(next2)))))(advance_char(next)) : new LexToken(make_token(new Equals(), "=", s), next)) : ((ch == ":") ? ((next_ch == ":") ? new LexToken(make_token(new ColonColon(), "::", s), advance_char(next)) : new LexToken(make_token(new Colon(), ":", s), next)) : ((ch == "|") ? ((next_ch == "-") ? new LexToken(make_token(new Turnstile(), "|-", s), advance_char(next)) : new LexToken(make_token(new Pipe(), "|", s), next)) : ((ch == "<") ? ((next_ch == "=") ? new LexToken(make_token(new LessOrEqual(), "<=", s), advance_char(next)) : ((next_ch == "-") ? new LexToken(make_token(new LeftArrow(), "<-", s), advance_char(next)) : new LexToken(make_token(new LessThan(), "<", s), next))) : ((ch == ">") ? ((next_ch == "=") ? new LexToken(make_token(new GreaterOrEqual(), ">=", s), advance_char(next)) : new LexToken(make_token(new GreaterThan(), ">", s), next)) : new LexToken(make_token(new ErrorToken(), s.source[(int)s.offset].ToString(), s), next))))))))))))((is_at_end(next) ? "" : peek_char(next)))))(advance_char(s))))(peek_char(s));
    }

    public static List<Token> tokenize_loop(LexState st, List<Token> acc)
    {
        return ((Func<LexResult, List<Token>>)((_scrutinee_) => (_scrutinee_ is LexToken _mLexToken_ ? ((Func<LexState, List<Token>>)((next) => ((Func<Token, List<Token>>)((tok) => ((tok.kind == new EndOfFile()) ? Enumerable.Concat(acc, new List<Token>() { tok }).ToList() : tokenize_loop(next, Enumerable.Concat(acc, new List<Token>() { tok }).ToList()))))((Token)_mLexToken_.Field0)))((LexState)_mLexToken_.Field1) : (_scrutinee_ is LexEnd _mLexEnd_ ? Enumerable.Concat(acc, new List<Token>() { make_token(new EndOfFile(), "", st) }).ToList() : throw new InvalidOperationException("Non-exhaustive match")))))(scan_token(st));
    }

    public static List<Token> tokenize(string src)
    {
        return tokenize_loop(make_lex_state(src), new List<Token>());
    }

    public static long token_length(Token t)
    {
        return ((long)t.text.Length);
    }

}
