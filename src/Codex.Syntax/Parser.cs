using Codex.Core;

namespace Codex.Syntax;

public sealed class Parser(IReadOnlyList<Token> tokens, DiagnosticBag diagnostics)
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
            Array.Empty<ChapterNode>(), startSpan.Through(endSpan));
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

        if (Current.Kind == TokenKind.Pipe)
        {
            VariantTypeBody body = ParseVariantTypeBody();
            SourceSpan span = nameToken.Span.Through(body.Span);
            return new TypeDefinitionNode(nameToken, typeParams, body, span);
        }

        m_position = savedPos;
        return null;
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

        while (Current.Kind == TokenKind.Pipe)
        {
            Advance();
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
                    Advance(); // skip colon
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

    TypeNode ParseTypeAtom()
    {
        if (Current.Kind == TokenKind.LinearKeyword)
        {
            Token start = Current;
            Advance();
            TypeNode inner = ParseTypeAtom();
            return new LinearTypeNode(inner, start.Span.Through(inner.Span));
        }

        if (Current.Kind == TokenKind.LeftBrace)
        {
            Token start = Current;
            Advance();
            if ((Current.Kind == TokenKind.Identifier || Current.Kind == TokenKind.ProofKeyword)
                && Current.Text == "proof"
                && Peek(1)?.Kind == TokenKind.Colon)
            {
                Advance(); // skip "proof"
                Advance(); // skip ":"
            }
            TypeNode left = ParseTypeAtom();
            Token op = Current;
            if (op.Kind is TokenKind.LessThan or TokenKind.GreaterThan
                or TokenKind.LessOrEqual or TokenKind.GreaterOrEqual)
            {
                Advance();
                TypeNode right = ParseTypeAtom();
                Expect(TokenKind.RightBrace);
                return new ProofConstraintNode(left, op, right, start.Span.Through(Previous.Span));
            }
            Expect(TokenKind.RightBrace);
            return new ProofConstraintNode(left, op, left, start.Span.Through(Previous.Span));
        }

        if (Current.Kind == TokenKind.LeftBracket)
        {
            Token start = Current;
            Advance();
            List<TypeNode> effects = [];
            while (Current.Kind != TokenKind.RightBracket && !IsAtEnd)
            {
                effects.Add(ParseTypeAtom());
                if (Current.Kind == TokenKind.Comma)
                {
                    Advance();
                }
            }
            Expect(TokenKind.RightBracket);
            TypeNode returnType = ParseType();
            return new EffectfulTypeNode(effects, returnType, start.Span.Through(returnType.Span));
        }

        if (Current.Kind == TokenKind.LeftParen)
        {
            Token start = Current;
            if (IsDependentTypeLookahead())
            {
                Advance(); // skip (
                Token paramName = Current;
                Advance(); // skip name
                Expect(TokenKind.Colon);
                TypeNode paramType = ParseType();
                Expect(TokenKind.RightParen);
                Expect(TokenKind.Arrow);
                TypeNode body = ParseType();
                return new DependentTypeNode(paramName, paramType, body, start.Span.Through(body.Span));
            }
            Advance();
            TypeNode inner = ParseType();
            if (Current.Kind is TokenKind.Plus or TokenKind.Minus or TokenKind.Star)
            {
                Token op = Current;
                Advance();
                TypeNode right = ParseType();
                Expect(TokenKind.RightParen);
                return new BinaryTypeNode(inner, op, right, start.Span.Through(Previous.Span));
            }
            Expect(TokenKind.RightParen);
            return new ParenthesizedTypeNode(inner, start.Span.Through(Previous.Span));
        }

        if (Current.Kind == TokenKind.IntegerLiteral)
        {
            Token lit = Current;
            Advance();
            return new IntegerTypeNode(lit, lit.Span);
        }

        if (Current.Kind is TokenKind.TypeIdentifier or TokenKind.Identifier)
        {
            Token nameToken = Current;
            Advance();
            TypeNode baseType = new NamedTypeNode(nameToken);

            List<TypeNode> args = [];
            while (Current.Kind is TokenKind.TypeIdentifier or TokenKind.Identifier
                       or TokenKind.LeftParen or TokenKind.IntegerLiteral
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

    public ExpressionNode ParseExpression()
    {
        return ParseBinary(0);
    }

    ExpressionNode ParseBinary(int minPrecedence)
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

    ExpressionNode ParseUnary()
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

    ExpressionNode ParseApplication()
    {
        ExpressionNode func = ParseAtom();

        bool isCompound = func is MatchExpressionNode
            or IfExpressionNode
            or LetExpressionNode
            or DoExpressionNode;
        if (isCompound) return func;

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

    bool IsApplicationStart()
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

    ExpressionNode ParseAtom()
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

            case TokenKind.DoKeyword:
                return ParseDoExpression();

            default:
            {
                m_diagnostics.Error("CDX1020", $"Expected an expression, found {Current.Kind}", Current.Span);
                Token err = Current;
                Advance();
                return new ErrorExpressionNode(err);
            }
        }
    }

    ExpressionNode ParseRecordExpression(Token typeName)
    {
        SourceSpan start = typeName.Span;
        Expect(TokenKind.LeftBrace);
        SkipNewlines();

        List<RecordFieldNode> fields = [];
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

    ExpressionNode ParseListExpression()
    {
        Token start = Current;
        Advance();
        SkipNewlines();

        List<ExpressionNode> elements = [];
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

    ExpressionNode ParseIfExpression()
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

    ExpressionNode ParseLetExpression()
    {
        Token start = Current;
        Advance();
        SkipNewlines();

        List<Syntax.LetBinding> bindings = [];
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

    ExpressionNode ParseMatchExpression()
    {
        Token start = Current;
        Advance();
        ExpressionNode scrutinee = ParseExpression();
        SkipNewlines();

        List<MatchBranchNode> branches = [];
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

    ExpressionNode ParseDoExpression()
    {
        Token start = Current;
        Advance();
        SkipNewlines();

        List<DoStatementNode> statements = [];
        while (!IsAtEnd
            && Current.Kind is not (TokenKind.EndOfFile or TokenKind.Dedent)
            && !(Current.Kind == TokenKind.Identifier && Peek(1)?.Kind == TokenKind.Colon))
        {
            if (Current.Kind == TokenKind.Identifier && Peek(1)?.Kind == TokenKind.LeftArrow)
            {
                Token name = Current;
                Advance();
                Advance();
                ExpressionNode value = ParseExpression();
                statements.Add(new DoBindStatementNode(name, value, name.Span.Through(value.Span)));
            }
            else
            {
                ExpressionNode expr = ParseExpression();
                statements.Add(new DoExprStatementNode(expr, expr.Span));
            }
            SkipNewlines();
        }

        if (statements.Count == 0)
        {
            m_diagnostics.Error("CDX1040", "do expression requires at least one statement", start.Span);
        }

        SourceSpan endSpan = statements.Count > 0 ? statements[^1].Span : start.Span;
        return new DoExpressionNode(statements, start.Span.Through(endSpan));
    }

    PatternNode ParsePattern()
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
                List<PatternNode> subPatterns = [];

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

    ClaimNode? TryParseClaim()
    {
        if (Current.Kind != TokenKind.ClaimKeyword)
            return null;

        Token claimKw = Current;
        Advance();

        Token name = Expect(TokenKind.Identifier);

        List<Token> parameters = [];
        while (Current.Kind == TokenKind.LeftParen)
        {
            Advance();
            parameters.Add(Expect(TokenKind.Identifier));
            Expect(TokenKind.RightParen);
        }

        Expect(TokenKind.Colon);
        TypeNode left = ParseType();
        Expect(TokenKind.TripleEquals);
        TypeNode right = ParseType();

        return new ClaimNode(name, parameters, left, right,
            claimKw.Span.Through(right.Span));
    }

    ProofNode? TryParseProof()
    {
        if (Current.Kind != TokenKind.ProofKeyword)
            return null;

        Token proofKw = Current;
        Advance();

        Token name = Expect(TokenKind.Identifier);

        List<Token> parameters = [];
        while (Current.Kind == TokenKind.LeftParen)
        {
            Advance();
            parameters.Add(Expect(TokenKind.Identifier));
            Expect(TokenKind.RightParen);
        }

        Expect(TokenKind.Equals);
        SkipNewlines();

        ProofExprNode body = ParseProofExpr();

        return new ProofNode(name, parameters, body,
            proofKw.Span.Through(body.Span));
    }

    ProofExprNode ParseProofExpr()
    {
        if (Current.Kind == TokenKind.TypeIdentifier && Current.Text == "Refl")
        {
            Token t = Current;
            Advance();
            return new ReflNode(t.Span);
        }

        if (Current.Kind == TokenKind.Identifier && Current.Text == "sym")
        {
            Token t = Current;
            Advance();
            ProofExprNode inner = ParseProofExpr();
            return new SymNode(inner, t.Span.Through(inner.Span));
        }

        if (Current.Kind == TokenKind.Identifier && Current.Text == "trans")
        {
            Token t = Current;
            Advance();
            ProofExprNode left = ParseProofAtom();
            ProofExprNode right = ParseProofAtom();
            return new TransNode(left, right, t.Span.Through(right.Span));
        }

        if (Current.Kind == TokenKind.Identifier && Current.Text == "cong")
        {
            Token t = Current;
            Advance();
            Token funcName = Current;
            Advance();
            ProofExprNode inner = ParseProofExpr();
            return new CongNode(funcName, inner, t.Span.Through(inner.Span));
        }

        if (Current.Kind == TokenKind.Identifier && Current.Text == "induction")
        {
            Token t = Current;
            Advance();
            Token variable = Current;
            Advance();
            SkipNewlines();

            List<ProofCaseNode> cases = [];
            while (Current.Kind == TokenKind.IfKeyword)
            {
                Advance();
                SkipNewlines();
                PatternNode pattern = ParsePattern();
                Expect(TokenKind.Arrow);
                SkipNewlines();
                ProofExprNode caseBody = ParseProofExpr();
                SourceSpan caseSpan = pattern.Span.Through(caseBody.Span);
                cases.Add(new ProofCaseNode(pattern, caseBody, caseSpan));
                SkipNewlines();
            }

            SourceSpan endSpan = cases.Count > 0 ? cases[^1].Span : variable.Span;
            return new InductionNode(variable, cases, t.Span.Through(endSpan));
        }

        return ParseProofAtom();
    }

    ProofExprNode ParseProofAtom()
    {
        if (Current.Kind == TokenKind.TypeIdentifier && Current.Text == "Refl")
        {
            Token t = Current;
            Advance();
            return new ReflNode(t.Span);
        }

        if (Current.Kind == TokenKind.LeftParen)
        {
            Advance();
            ProofExprNode inner = ParseProofExpr();
            Expect(TokenKind.RightParen);
            return inner;
        }

        if (Current.Kind is TokenKind.Identifier or TokenKind.TypeIdentifier)
        {
            Token name = Current;
            Advance();

            List<ExpressionNode> args = [];
            while (Current.Kind is TokenKind.Identifier or TokenKind.TypeIdentifier
                   or TokenKind.IntegerLiteral or TokenKind.LeftParen)
            {
                if (Current.Kind == TokenKind.LeftParen)
                {
                    Advance();
                    args.Add(ParseExpression());
                    Expect(TokenKind.RightParen);
                }
                else
                {
                    Token argTok = Current;
                    Advance();
                    args.Add(new NameExpressionNode(argTok));
                }
            }

            if (args.Count > 0)
                return new ProofApplyNode(name, args, name.Span.Through(args[^1].Span));

            return new ProofNameNode(name, name.Span);
        }

        m_diagnostics.Error("CDX1020", $"Expected a proof expression, found {Current.Kind}", Current.Span);
        Token err = Current;
        Advance();
        return new ReflNode(err.Span);
    }
}
