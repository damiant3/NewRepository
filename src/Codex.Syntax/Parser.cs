using Codex.Core;

namespace Codex.Syntax;

/// <summary>
/// Recursive descent parser for Codex notation.
/// Produces a Concrete Syntax Tree from a token stream.
/// </summary>
public sealed class Parser
{
    private readonly IReadOnlyList<Token> m_tokens;
    private readonly DiagnosticBag m_diagnostics;
    private int m_position;

    public Parser(IReadOnlyList<Token> tokens, DiagnosticBag diagnostics)
    {
        m_tokens = tokens;
        m_diagnostics = diagnostics;
        m_position = 0;
    }

    public DiagnosticBag Diagnostics => m_diagnostics;

    /// <summary>Parse an entire document (a sequence of top-level definitions).</summary>
    public DocumentNode ParseDocument()
    {
        SourceSpan startSpan = Current.Span;
        List<DefinitionNode> definitions = new List<DefinitionNode>();

        SkipNewlines();
        while (!IsAtEnd)
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
            SkipNewlines();
        }

        SourceSpan endSpan = Previous.Span;
        return new DocumentNode(definitions, startSpan.Through(endSpan));
    }

    private DefinitionNode? TryParseDefinition()
    {
        if (Current.Kind is not (TokenKind.Identifier or TokenKind.TypeIdentifier))
        {
            return null;
        }

        Token startToken = Current;
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

        List<Token> parameters = new List<Token>();
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

    private TypeAnnotationNode ParseTypeAnnotation()
    {
        Token nameToken = Current;
        Advance();
        Expect(TokenKind.Colon);
        TypeNode type = ParseType();
        return new TypeAnnotationNode(nameToken, type, nameToken.Span.Through(type.Span));
    }

    // --- Type parsing ---

    public TypeNode ParseType()
    {
        TypeNode left = ParseTypeAtom();

        if (Current.Kind == TokenKind.Arrow)
        {
            Advance();
            TypeNode right = ParseType();
            return new FunctionTypeNode(left, right, left.Span.Through(right.Span));
        }

        return left;
    }

    private TypeNode ParseTypeAtom()
    {
        if (Current.Kind == TokenKind.LeftParen)
        {
            Token start = Current;
            Advance();
            TypeNode inner = ParseType();
            Expect(TokenKind.RightParen);
            return new ParenthesizedTypeNode(inner, start.Span.Through(Previous.Span));
        }

        if (Current.Kind is TokenKind.TypeIdentifier or TokenKind.Identifier)
        {
            Token nameToken = Current;
            Advance();
            TypeNode baseType = new NamedTypeNode(nameToken);

            List<TypeNode> args = new List<TypeNode>();
            while (Current.Kind is TokenKind.TypeIdentifier or TokenKind.Identifier or TokenKind.LeftParen
                   && !IsAtEnd
                   && Current.Kind != TokenKind.Arrow
                   && Current.Kind != TokenKind.Newline
                   && Current.Kind != TokenKind.Equals
                   && Current.Kind != TokenKind.RightParen
                   && Current.Kind != TokenKind.Comma)
            {
                args.Add(ParseTypeAtom());
            }

            if (args.Count > 0)
            {
                return new ApplicationTypeNode(baseType, args, nameToken.Span.Through(args[^1].Span));
            }

            return baseType;
        }

        m_diagnostics.Error("CDX1010", $"Expected a type, found {Current.Kind}", Current.Span);
        Token errToken = Current;
        Advance();
        return new NamedTypeNode(errToken);
    }

    // --- Expression parsing (Pratt) ---

    public ExpressionNode ParseExpression()
    {
        return ParseBinary(0);
    }

    private ExpressionNode ParseBinary(int minPrecedence)
    {
        ExpressionNode left = ParseUnary();

        while (true)
        {
            (int prec, Associativity assoc) = GetPrecedence(Current.Kind);
            if (prec < 0 || prec < minPrecedence)
            {
                break;
            }

            Token op = Current;
            Advance();
            SkipNewlines();

            int nextMin = assoc == Associativity.Right ? prec : prec + 1;
            ExpressionNode right = ParseBinary(nextMin);

            left = new BinaryExpressionNode(left, op, right, left.Span.Through(right.Span));
        }

        return left;
    }

    private ExpressionNode ParseUnary()
    {
        if (Current.Kind == TokenKind.Minus)
        {
            Token op = Current;
            Advance();
            ExpressionNode operand = ParseUnary();
            return new UnaryExpressionNode(op, operand, op.Span.Through(operand.Span));
        }

        return ParseApplication();
    }

    private ExpressionNode ParseApplication()
    {
        ExpressionNode func = ParseAtom();

        while (IsApplicationStart())
        {
            ExpressionNode arg = ParseAtom();
            func = new ApplicationExpressionNode(func, arg, func.Span.Through(arg.Span));
        }

        while (Current.Kind == TokenKind.Dot)
        {
            Advance();
            Token field = Expect(TokenKind.Identifier);
            func = new FieldAccessExpressionNode(func, field, func.Span.Through(field.Span));
        }

        return func;
    }

    private bool IsApplicationStart()
    {
        return Current.Kind is TokenKind.Identifier
            or TokenKind.TypeIdentifier
            or TokenKind.IntegerLiteral
            or TokenKind.NumberLiteral
            or TokenKind.TextLiteral
            or TokenKind.TrueKeyword
            or TokenKind.FalseKeyword
            or TokenKind.LeftParen
            or TokenKind.LeftBracket;
    }

    private ExpressionNode ParseAtom()
    {
        switch (Current.Kind)
        {
            case TokenKind.IntegerLiteral:
            case TokenKind.NumberLiteral:
            case TokenKind.TextLiteral:
            case TokenKind.TrueKeyword:
            case TokenKind.FalseKeyword:
            {
                Token token = Current;
                Advance();
                return new LiteralExpressionNode(token);
            }

            case TokenKind.Identifier:
            case TokenKind.TypeIdentifier:
            {
                Token token = Current;
                Advance();

                if (token.Kind == TokenKind.TypeIdentifier && Current.Kind == TokenKind.LeftBrace)
                {
                    return ParseRecordExpression(token);
                }

                return new NameExpressionNode(token);
            }

            case TokenKind.LeftParen:
            {
                Token start = Current;
                Advance();
                SkipNewlines();
                ExpressionNode inner = ParseExpression();
                SkipNewlines();
                Expect(TokenKind.RightParen);
                return new ParenthesizedExpressionNode(inner, start.Span.Through(Previous.Span));
            }

            case TokenKind.LeftBracket:
                return ParseListExpression();

            case TokenKind.IfKeyword:
                return ParseIfExpression();

            case TokenKind.LetKeyword:
                return ParseLetExpression();

            case TokenKind.WhenKeyword:
                return ParseMatchExpression();

            default:
            {
                m_diagnostics.Error("CDX1020", $"Expected an expression, found {Current.Kind}", Current.Span);
                Token err = Current;
                Advance();
                return new ErrorExpressionNode(err);
            }
        }
    }

    private ExpressionNode ParseRecordExpression(Token typeName)
    {
        SourceSpan start = typeName.Span;
        Expect(TokenKind.LeftBrace);
        SkipNewlines();

        List<RecordFieldNode> fields = new List<RecordFieldNode>();
        while (Current.Kind != TokenKind.RightBrace && !IsAtEnd)
        {
            Token fieldName = Expect(TokenKind.Identifier);
            Expect(TokenKind.Equals);
            ExpressionNode fieldValue = ParseExpression();
            fields.Add(new RecordFieldNode(fieldName, fieldValue, fieldName.Span.Through(fieldValue.Span)));

            SkipNewlines();
            if (Current.Kind == TokenKind.Comma)
            {
                Advance();
                SkipNewlines();
            }
        }

        Expect(TokenKind.RightBrace);
        return new RecordExpressionNode(typeName, fields, start.Through(Previous.Span));
    }

    private ExpressionNode ParseListExpression()
    {
        Token start = Current;
        Advance();
        SkipNewlines();

        List<ExpressionNode> elements = new List<ExpressionNode>();
        while (Current.Kind != TokenKind.RightBracket && !IsAtEnd)
        {
            elements.Add(ParseExpression());
            SkipNewlines();
            if (Current.Kind == TokenKind.Comma)
            {
                Advance();
                SkipNewlines();
            }
        }

        Expect(TokenKind.RightBracket);
        return new ListExpressionNode(elements, start.Span.Through(Previous.Span));
    }

    private ExpressionNode ParseIfExpression()
    {
        Token start = Current;
        Advance();
        SkipNewlines();
        ExpressionNode condition = ParseExpression();
        SkipNewlines();
        Expect(TokenKind.ThenKeyword);
        SkipNewlines();
        ExpressionNode thenExpr = ParseExpression();
        SkipNewlines();
        Expect(TokenKind.ElseKeyword);
        SkipNewlines();
        ExpressionNode elseExpr = ParseExpression();
        return new IfExpressionNode(condition, thenExpr, elseExpr, start.Span.Through(elseExpr.Span));
    }

    private ExpressionNode ParseLetExpression()
    {
        Token start = Current;
        Advance();
        SkipNewlines();

        List<Syntax.LetBinding> bindings = new List<Syntax.LetBinding>();
        while (Current.Kind == TokenKind.Identifier)
        {
            Token name = Current;
            Advance();
            Expect(TokenKind.Equals);
            ExpressionNode value = ParseExpression();
            bindings.Add(new Syntax.LetBinding(name, value));
            SkipNewlines();

            if (Current.Kind == TokenKind.Comma)
            {
                Advance();
                SkipNewlines();
            }
        }

        Expect(TokenKind.InKeyword);
        SkipNewlines();
        ExpressionNode body = ParseExpression();
        return new LetExpressionNode(bindings, body, start.Span.Through(body.Span));
    }

    private ExpressionNode ParseMatchExpression()
    {
        Token start = Current;
        Advance();
        ExpressionNode scrutinee = ParseExpression();
        SkipNewlines();

        List<MatchBranchNode> branches = new List<MatchBranchNode>();
        while (Current.Kind == TokenKind.IfKeyword)
        {
            Advance();
            PatternNode pattern = ParsePattern();
            Expect(TokenKind.Arrow);
            SkipNewlines();
            ExpressionNode body = ParseExpression();
            branches.Add(new MatchBranchNode(pattern, body, pattern.Span.Through(body.Span)));
            SkipNewlines();
        }

        if (branches.Count == 0)
        {
            m_diagnostics.Error("CDX1030", "Match expression requires at least one branch", start.Span);
        }

        SourceSpan endSpan = branches.Count > 0 ? branches[^1].Span : scrutinee.Span;
        return new MatchExpressionNode(scrutinee, branches, start.Span.Through(endSpan));
    }

    private PatternNode ParsePattern()
    {
        switch (Current.Kind)
        {
            case TokenKind.Underscore:
            {
                Token token = Current;
                Advance();
                return new WildcardPatternNode(token);
            }

            case TokenKind.IntegerLiteral:
            case TokenKind.TextLiteral:
            case TokenKind.TrueKeyword:
            case TokenKind.FalseKeyword:
            {
                Token token = Current;
                Advance();
                return new LiteralPatternNode(token);
            }

            case TokenKind.TypeIdentifier:
            {
                Token ctor = Current;
                Advance();
                List<PatternNode> subPatterns = new List<PatternNode>();

                while (Current.Kind == TokenKind.LeftParen)
                {
                    Advance();
                    subPatterns.Add(ParsePattern());
                    Expect(TokenKind.RightParen);
                }

                if (subPatterns.Count > 0)
                {
                    return new ConstructorPatternNode(ctor, subPatterns, ctor.Span.Through(Previous.Span));
                }

                return new ConstructorPatternNode(ctor, subPatterns, ctor.Span);
            }

            case TokenKind.Identifier:
            {
                Token token = Current;
                Advance();
                return new VariablePatternNode(token);
            }

            default:
            {
                m_diagnostics.Error("CDX1031", $"Expected a pattern, found {Current.Kind}", Current.Span);
                Token err = Current;
                Advance();
                return new WildcardPatternNode(err);
            }
        }
    }

    // --- Precedence table ---

    private enum Associativity { Left, Right }

    private static (int Precedence, Associativity Assoc) GetPrecedence(TokenKind kind) => kind switch
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

    // --- Token stream helpers ---

    private Token Current => m_position < m_tokens.Count ? m_tokens[m_position] : m_tokens[^1];

    private Token Previous => m_position > 0 ? m_tokens[m_position - 1] : m_tokens[0];

    private bool IsAtEnd => Current.Kind == TokenKind.EndOfFile;

    private Token? Peek(int offset)
    {
        int idx = m_position + offset;
        return idx >= 0 && idx < m_tokens.Count ? m_tokens[idx] : null;
    }

    private void Advance()
    {
        if (!IsAtEnd)
        {
            m_position++;
        }
    }

    private Token Expect(TokenKind kind)
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

    private void SkipNewlines()
    {
        while (Current.Kind is TokenKind.Newline or TokenKind.Indent or TokenKind.Dedent)
        {
            Advance();
        }
    }

    private void SkipToNextDefinition()
    {
        while (!IsAtEnd)
        {
            if (Current.Kind == TokenKind.Newline)
            {
                Advance();
                if (Current.Kind is TokenKind.Identifier or TokenKind.TypeIdentifier)
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
}
