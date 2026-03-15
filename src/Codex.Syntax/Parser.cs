using Codex.Core;

namespace Codex.Syntax;

public sealed partial class Parser(IReadOnlyList<Token> tokens, DiagnosticBag diagnostics)
{
    readonly IReadOnlyList<Token> m_tokens = tokens;
    readonly DiagnosticBag m_diagnostics = diagnostics;
    int m_position = 0;

    public DiagnosticBag Diagnostics => m_diagnostics;

    public DocumentNode ParseDocument()
    {
        SourceSpan startSpan = Current.Span;
        List<DefinitionNode> definitions = [];
        List<TypeDefinitionNode> typeDefinitions = [];
        List<ClaimNode> claims = [];
        List<ProofNode> proofs = [];

        SkipNewlines();
        while (!IsAtEnd)
        {
            if (Current.Kind == TokenKind.ClaimKeyword)
            {
                ClaimNode? claim = TryParseClaim();
                if (claim is not null)
                {
                    claims.Add(claim);
                    SkipNewlines();
                    continue;
                }
            }

            if (Current.Kind == TokenKind.ProofKeyword)
            {
                ProofNode? proof = TryParseProof();
                if (proof is not null)
                {
                    proofs.Add(proof);
                    SkipNewlines();
                    continue;
                }
            }

            TypeDefinitionNode? typeDef = TryParseTypeDefinition();
            if (typeDef is not null)
            {
                typeDefinitions.Add(typeDef);
            }
            else
            {
                DefinitionNode? def = TryParseDefinition();
                if (def is not null)
                {
                    definitions.Add(def);
                }
                else
                {
                    m_diagnostics.Error("CDX1001", $"Expected a definition, found {Current.Kind}", Current.Span);
                    SkipToNextDefinition();
                }
            }
            SkipNewlines();
        }

        SourceSpan endSpan = Previous.Span;
        return new DocumentNode(definitions, typeDefinitions, claims, proofs,
            [], startSpan.Through(endSpan));
    }

    TypeDefinitionNode? TryParseTypeDefinition()
    {
        if (Current.Kind != TokenKind.TypeIdentifier)
            return null;

        int savedPos = m_position;

        Token nameToken = Current;
        Advance();

        List<Token> typeParams = [];
        while (Current.Kind == TokenKind.LeftParen
            && Peek(1)?.Kind is TokenKind.Identifier or TokenKind.TypeIdentifier
            && Peek(2)?.Kind == TokenKind.RightParen)
        {
            Advance();
            typeParams.Add(Current);
            Advance();
            Expect(TokenKind.RightParen);
        }

        if (Current.Kind != TokenKind.Equals)
        {
            m_position = savedPos;
            return null;
        }

        Advance();
        SkipNewlines();

        if (Current.Kind == TokenKind.RecordKeyword)
        {
            RecordTypeBody body = ParseRecordTypeBody();
            SourceSpan span = nameToken.Span.Through(body.Span);
            return new TypeDefinitionNode(nameToken, typeParams, body, span);
        }

        if (Current.Kind == TokenKind.Pipe
            || (Current.Kind == TokenKind.TypeIdentifier && LooksLikeVariantBody()))
        {
            VariantTypeBody body = ParseVariantTypeBody();
            SourceSpan span = nameToken.Span.Through(body.Span);
            return new TypeDefinitionNode(nameToken, typeParams, body, span);
        }

        m_position = savedPos;
        return null;
    }

    bool LooksLikeVariantBody()
    {
        int lookahead = m_position;
        while (lookahead < m_tokens.Count)
        {
            TokenKind kind = m_tokens[lookahead].Kind;
            if (kind == TokenKind.Pipe) return true;
            if (kind is TokenKind.Newline or TokenKind.EndOfFile) return false;
            lookahead++;
        }
        return false;
    }

    RecordTypeBody ParseRecordTypeBody()
    {
        Token recordKw = Expect(TokenKind.RecordKeyword);
        Expect(TokenKind.LeftBrace);
        SkipNewlines();

        List<RecordTypeFieldNode> fields = [];
        while (Current.Kind == TokenKind.Identifier && !IsAtEnd)
        {
            Token fieldName = Current;
            Advance();
            Expect(TokenKind.Colon);
            TypeNode fieldType = ParseType();
            fields.Add(new RecordTypeFieldNode(fieldName, fieldType, fieldName.Span.Through(fieldType.Span)));
            SkipNewlines();
            if (Current.Kind == TokenKind.Comma)
            {
                Advance();
                SkipNewlines();
            }
        }

        Token closeBrace = Expect(TokenKind.RightBrace);
        return new RecordTypeBody(fields, recordKw.Span.Through(closeBrace.Span));
    }

    VariantTypeBody ParseVariantTypeBody()
    {
        SourceSpan startSpan = Current.Span;
        List<VariantConstructorNode> constructors = [];

        bool firstCtor = Current.Kind == TokenKind.TypeIdentifier;

        while (Current.Kind == TokenKind.Pipe || firstCtor)
        {
            if (Current.Kind == TokenKind.Pipe)
                Advance();
            firstCtor = false;
            SkipNewlines();

            Token ctorName = Expect(TokenKind.TypeIdentifier);
            List<VariantFieldNode> fields = [];

            while (Current.Kind == TokenKind.LeftParen)
            {
                Advance();
                Token? fieldName = null;
                if (Current.Kind == TokenKind.Identifier && Peek(1)?.Kind == TokenKind.Colon)
                {
                    fieldName = Current;
                    Advance();
                    Advance();
                }
                TypeNode fieldType = ParseType();
                SourceSpan fieldSpan = (fieldName?.Span ?? fieldType.Span).Through(fieldType.Span);
                fields.Add(new VariantFieldNode(fieldName, fieldType, fieldSpan));
                Expect(TokenKind.RightParen);
            }

            SourceSpan ctorSpan = ctorName.Span;
            if (fields.Count > 0)
                ctorSpan = ctorName.Span.Through(fields[^1].Span);
            constructors.Add(new VariantConstructorNode(ctorName, fields, ctorSpan));
            SkipNewlines();
        }

        SourceSpan endSpan = constructors.Count > 0 ? constructors[^1].Span : startSpan;
        return new VariantTypeBody(constructors, startSpan.Through(endSpan));
    }

    DefinitionNode? TryParseDefinition()
    {
        if (Current.Kind is not (TokenKind.Identifier or TokenKind.TypeIdentifier))
        {
            return null;
        }

        int startPos = m_position;

        TypeAnnotationNode? annotation = null;
        if (Peek(1)?.Kind == TokenKind.Colon)
        {
            annotation = ParseTypeAnnotation();
            SkipNewlines();
        }

        if (Current.Kind is not (TokenKind.Identifier or TokenKind.TypeIdentifier))
        {
            if (annotation is not null)
            {
                return new DefinitionNode(
                    annotation.Name,
                    [],
                    annotation,
                    new ErrorExpressionNode(Current),
                    annotation.Span);
            }
            m_position = startPos;
            return null;
        }

        Token nameToken = Expect(Current.Kind is TokenKind.Identifier ? TokenKind.Identifier : TokenKind.TypeIdentifier);

        List<Token> parameters = [];
        while (Current.Kind == TokenKind.LeftParen)
        {
            Advance();
            if (Current.Kind is TokenKind.Identifier or TokenKind.TypeIdentifier)
            {
                parameters.Add(Current);
                Advance();
            }
            Expect(TokenKind.RightParen);
        }

        while (Current.Kind == TokenKind.Identifier && Peek(1)?.Kind != TokenKind.Colon)
        {
            if (Current.Kind == TokenKind.Identifier && Current.Text == "=")
            {
                break;
            }
            TokenKind? nextKind = Peek(1)?.Kind;
            if (nextKind is TokenKind.Equals or TokenKind.Newline or TokenKind.EndOfFile or null)
            {
                break;
            }
            parameters.Add(Current);
            Advance();
        }

        Expect(TokenKind.Equals);

        SkipNewlines();
        ExpressionNode body = ParseExpression();

        SourceSpan span = (annotation?.Span ?? nameToken.Span).Through(body.Span);
        return new DefinitionNode(nameToken, parameters, annotation, body, span);
    }

    TypeAnnotationNode ParseTypeAnnotation()
    {
        Token nameToken = Current;
        Advance();
        Expect(TokenKind.Colon);
        TypeNode type = ParseType();
        return new TypeAnnotationNode(nameToken, type, nameToken.Span.Through(type.Span));
    }

    enum Associativity { Left, Right }

    static (int Precedence, Associativity Assoc) GetPrecedence(TokenKind kind) => kind switch
    {
        TokenKind.PlusPlus      => (5, Associativity.Right),
        TokenKind.ColonColon    => (5, Associativity.Right),
        TokenKind.Plus          => (6, Associativity.Left),
        TokenKind.Minus         => (6, Associativity.Left),
        TokenKind.Star          => (7, Associativity.Left),
        TokenKind.Slash         => (7, Associativity.Left),
        TokenKind.Caret         => (8, Associativity.Right),
        TokenKind.DoubleEquals  => (4, Associativity.Left),
        TokenKind.NotEquals     => (4, Associativity.Left),
        TokenKind.LessThan      => (4, Associativity.Left),
        TokenKind.GreaterThan   => (4, Associativity.Left),
        TokenKind.LessOrEqual   => (4, Associativity.Left),
        TokenKind.GreaterOrEqual => (4, Associativity.Left),
        TokenKind.TripleEquals  => (4, Associativity.Left),
        TokenKind.Ampersand     => (3, Associativity.Left),
        TokenKind.Pipe          => (2, Associativity.Left),
        TokenKind.Arrow         => (1, Associativity.Right),
        _                       => (-1, Associativity.Left),
    };

    Token Current => m_position < m_tokens.Count ? m_tokens[m_position] : m_tokens[^1];

    Token Previous => m_position > 0 ? m_tokens[m_position - 1] : m_tokens[0];

    bool IsAtEnd => Current.Kind == TokenKind.EndOfFile;

    Token? Peek(int offset)
    {
        int idx = m_position + offset;
        return idx >= 0 && idx < m_tokens.Count ? m_tokens[idx] : null;
    }

    void Advance()
    {
        if (!IsAtEnd)
        {
            m_position++;
        }
    }

    Token Expect(TokenKind kind)
    {
        if (Current.Kind == kind)
        {
            Token token = Current;
            Advance();
            return token;
        }

        m_diagnostics.Error("CDX1000", $"Expected {kind}, found {Current.Kind}", Current.Span);
        return Current;
    }

    void SkipNewlines()
    {
        while (Current.Kind is TokenKind.Newline or TokenKind.Indent or TokenKind.Dedent)
        {
            Advance();
        }
    }

    bool IsDependentTypeLookahead()
    {
        Token? t1 = Peek(1);
        Token? t2 = Peek(2);
        return t1 is not null && t2 is not null
            && t1.Kind is TokenKind.Identifier or TokenKind.TypeIdentifier
            && t2.Kind == TokenKind.Colon;
    }

    void SkipToNextDefinition()
    {
        while (!IsAtEnd)
        {
            if (Current.Kind == TokenKind.Newline)
            {
                Advance();
                if (Current.Kind is TokenKind.Identifier or TokenKind.TypeIdentifier
                    or TokenKind.ClaimKeyword or TokenKind.ProofKeyword)
                {
                    return;
                }
            }
            else
            {
                Advance();
            }
        }
    }

    void Synchronize()
    {
        while (!IsAtEnd)
        {
            if (Current.Kind is TokenKind.Newline or TokenKind.Dedent)
                return;

            if (Current.Kind is TokenKind.ThenKeyword or TokenKind.ElseKeyword
                or TokenKind.InKeyword or TokenKind.IfKeyword
                or TokenKind.WhenKeyword or TokenKind.DoKeyword
                or TokenKind.LetKeyword
                or TokenKind.RightParen or TokenKind.RightBracket or TokenKind.RightBrace)
            {
                return;
            }

            Advance();
        }
    }
}
