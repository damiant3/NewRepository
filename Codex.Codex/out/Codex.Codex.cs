using System;
using System.Collections.Generic;
using System.Linq;

Codex_Codex_Codex.main();

public sealed record Def(Token name, List<Token> @params, List<TypeAnn> ann, Expr body);

public sealed record DefParamResult(UnificationState state, TypeEnv env, CodexType remaining_type);

public sealed record AVariantCtorDef(Name name, List<ATypeExpr> fields);

public sealed record RecordField(Name name, CodexType type_val);

public abstract record LiteralKind;

public sealed record IntLit : LiteralKind;
public sealed record NumLit : LiteralKind;
public sealed record TextLit : LiteralKind;
public sealed record BoolLit : LiteralKind;

public sealed record Token(TokenKind kind, string text, long offset, long line, long column);

public sealed record IRFieldVal(string name, IRExpr value);

public sealed record LambdaBindResult(UnificationState state, TypeEnv env, List<CodexType> param_types);

public abstract record IRExpr;

public sealed record IrIntLit(long Field0) : IRExpr;
public sealed record IrNumLit(long Field0) : IRExpr;
public sealed record IrTextLit(string Field0) : IRExpr;
public sealed record IrBoolLit(bool Field0) : IRExpr;
public sealed record IrName(string Field0, CodexType Field1) : IRExpr;
public sealed record IrBinary(IRBinaryOp Field0, IRExpr Field1, IRExpr Field2, CodexType Field3) : IRExpr;
public sealed record IrNegate(IRExpr Field0) : IRExpr;
public sealed record IrIf(IRExpr Field0, IRExpr Field1, IRExpr Field2, CodexType Field3) : IRExpr;
public sealed record IrLet(string Field0, CodexType Field1, IRExpr Field2, IRExpr Field3) : IRExpr;
public sealed record IrApply(IRExpr Field0, IRExpr Field1, CodexType Field2) : IRExpr;
public sealed record IrLambda(List<IRParam> Field0, IRExpr Field1, CodexType Field2) : IRExpr;
public sealed record IrList(List<IRExpr> Field0, CodexType Field1) : IRExpr;
public sealed record IrMatch(IRExpr Field0, List<IRBranch> Field1, CodexType Field2) : IRExpr;
public sealed record IrDo(List<IRDoStmt> Field0, CodexType Field1) : IRExpr;
public sealed record IrRecord(string Field0, List<IRFieldVal> Field1, CodexType Field2) : IRExpr;
public sealed record IrFieldAccess(IRExpr Field0, string Field1, CodexType Field2) : IRExpr;
public sealed record IrError(string Field0, CodexType Field1) : IRExpr;

public sealed record ParseState(List<Token> tokens, long pos);

public sealed record TypeBinding(string name, CodexType bound_type);

public abstract record APat;

public sealed record AVarPat(Name Field0) : APat;
public sealed record ALitPat(string Field0, LiteralKind Field1) : APat;
public sealed record ACtorPat(Name Field0, List<APat> Field1) : APat;
public sealed record AWildPat : APat;

public abstract record TypeExpr;

public sealed record NamedType(Token Field0) : TypeExpr;
public sealed record FunType(TypeExpr Field0, TypeExpr Field1) : TypeExpr;
public sealed record AppType(TypeExpr Field0, List<TypeExpr> Field1) : TypeExpr;
public sealed record ParenType(TypeExpr Field0) : TypeExpr;
public sealed record ListType(TypeExpr Field0) : TypeExpr;
public sealed record LinearTypeExpr(TypeExpr Field0) : TypeExpr;

public sealed record Scope(List<string> names);

public abstract record TypeBody;

public sealed record RecordBody(List<RecordFieldDef> Field0) : TypeBody;
public sealed record VariantBody(List<VariantCtorDef> Field0) : TypeBody;

public sealed record MatchArm(Pat pattern, Expr body);

public abstract record ATypeExpr;

public sealed record ANamedType(Name Field0) : ATypeExpr;
public sealed record AFunType(ATypeExpr Field0, ATypeExpr Field1) : ATypeExpr;
public sealed record AAppType(ATypeExpr Field0, List<ATypeExpr> Field1) : ATypeExpr;

public sealed record IRDef(string name, List<IRParam> @params, CodexType type_val, IRExpr body);

public sealed record ALetBind(Name name, AExpr value);

public sealed record LetBind(Token name, Expr value);

public abstract record ADoStmt;

public sealed record ADoBindStmt(Name Field0, AExpr Field1) : ADoStmt;
public sealed record ADoExprStmt(AExpr Field0) : ADoStmt;

public sealed record TypeAnn(Token name, TypeExpr type_expr);

public abstract record IRDoStmt;

public sealed record IrDoBind(string Field0, CodexType Field1, IRExpr Field2) : IRDoStmt;
public sealed record IrDoExec(IRExpr Field0) : IRDoStmt;

public sealed record AParam(Name name);

public abstract record ParsePatResult;

public sealed record PatOk(Pat Field0, ParseState Field1) : ParsePatResult;

public sealed record UnifyResult(bool success, UnificationState state);

public sealed record LetBindResult(UnificationState state, TypeEnv env);

public abstract record LexResult;

public sealed record LexToken(Token Field0, LexState Field1) : LexResult;
public sealed record LexEnd : LexResult;

public sealed record CtorCollectResult(List<string> type_names, List<string> ctor_names);

public sealed record SumCtor(Name name, List<CodexType> fields);

public abstract record Pat;

public sealed record VarPat(Token Field0) : Pat;
public sealed record LitPat(Token Field0) : Pat;
public sealed record CtorPat(Token Field0, List<Pat> Field1) : Pat;
public sealed record WildPat(Token Field0) : Pat;

public sealed record SourcePosition(long line, long column, long offset);

public sealed record ModuleResult(List<TypeBinding> types, UnificationState state);

public sealed record AFieldExpr(Name name, AExpr value);

public abstract record DiagnosticSeverity;

public sealed record Error : DiagnosticSeverity;
public sealed record Warning : DiagnosticSeverity;
public sealed record Info : DiagnosticSeverity;

public sealed record IRParam(string name, CodexType type_val);

public abstract record CodexType;

public sealed record IntegerTy : CodexType;
public sealed record NumberTy : CodexType;
public sealed record TextTy : CodexType;
public sealed record BooleanTy : CodexType;
public sealed record VoidTy : CodexType;
public sealed record NothingTy : CodexType;
public sealed record ErrorTy : CodexType;
public sealed record FunTy(CodexType Field0, CodexType Field1) : CodexType;
public sealed record ListTy(CodexType Field0) : CodexType;
public sealed record TypeVar(long Field0) : CodexType;
public sealed record ForAllTy(long Field0, CodexType Field1) : CodexType;
public sealed record SumTy(Name Field0, List<SumCtor> Field1) : CodexType;
public sealed record RecordTy(Name Field0, List<RecordField> Field1) : CodexType;
public sealed record ConstructedTy(Name Field0, List<CodexType> Field1) : CodexType;

public sealed record VariantCtorDef(Token name, List<TypeExpr> fields);

public sealed record AMatchArm(APat pattern, AExpr body);

public sealed record CollectResult(List<string> names, List<Diagnostic> errors);

public sealed record TypeDef(Token name, List<Token> type_params, TypeBody body);

public sealed record PatBindResult(UnificationState state, TypeEnv env);

public abstract record Expr;

public sealed record LitExpr(Token Field0) : Expr;
public sealed record NameExpr(Token Field0) : Expr;
public sealed record AppExpr(Expr Field0, Expr Field1) : Expr;
public sealed record BinExpr(Expr Field0, Token Field1, Expr Field2) : Expr;
public sealed record UnaryExpr(Token Field0, Expr Field1) : Expr;
public sealed record IfExpr(Expr Field0, Expr Field1, Expr Field2) : Expr;
public sealed record LetExpr(List<LetBind> Field0, Expr Field1) : Expr;
public sealed record MatchExpr(Expr Field0, List<MatchArm> Field1) : Expr;
public sealed record ListExpr(List<Expr> Field0) : Expr;
public sealed record RecordExpr(Token Field0, List<RecordFieldExpr> Field1) : Expr;
public sealed record FieldExpr(Expr Field0, Token Field1) : Expr;
public sealed record ParenExpr(Expr Field0) : Expr;
public sealed record DoExpr(List<DoStmt> Field0) : Expr;
public sealed record ErrExpr(Token Field0) : Expr;

public sealed record FreshResult(CodexType var_type, UnificationState state);

public sealed record TypeEnv(List<TypeBinding> bindings);

public sealed record Name(string value);

public sealed record LexState(string source, long offset, long line, long column);

public sealed record SubstEntry(long var_id, CodexType resolved_type);

public abstract record ParseExprResult;

public sealed record ExprOk(Expr Field0, ParseState Field1) : ParseExprResult;

public abstract record DoStmt;

public sealed record DoBindStmt(Token Field0, Expr Field1) : DoStmt;
public sealed record DoExprStmt(Expr Field0) : DoStmt;

public sealed record SourceSpan(SourcePosition start, SourcePosition end, string file);

public abstract record ParseTypeResult;

public sealed record TypeOk(TypeExpr Field0, ParseState Field1) : ParseTypeResult;

public abstract record ParseTypeDefResult;

public sealed record TypeDefOk(TypeDef Field0, ParseState Field1) : ParseTypeDefResult;
public sealed record TypeDefNone(ParseState Field0) : ParseTypeDefResult;

public abstract record BinaryOp;

public sealed record OpAdd : BinaryOp;
public sealed record OpSub : BinaryOp;
public sealed record OpMul : BinaryOp;
public sealed record OpDiv : BinaryOp;
public sealed record OpPow : BinaryOp;
public sealed record OpEq : BinaryOp;
public sealed record OpNotEq : BinaryOp;
public sealed record OpLt : BinaryOp;
public sealed record OpGt : BinaryOp;
public sealed record OpLtEq : BinaryOp;
public sealed record OpGtEq : BinaryOp;
public sealed record OpDefEq : BinaryOp;
public sealed record OpAppend : BinaryOp;
public sealed record OpCons : BinaryOp;
public sealed record OpAnd : BinaryOp;
public sealed record OpOr : BinaryOp;

public sealed record RecordFieldDef(Token name, TypeExpr type_expr);

public abstract record IRPat;

public sealed record IrVarPat(string Field0, CodexType Field1) : IRPat;
public sealed record IrLitPat(string Field0, CodexType Field1) : IRPat;
public sealed record IrCtorPat(string Field0, List<IRPat> Field1, CodexType Field2) : IRPat;
public sealed record IrWildPat : IRPat;

public sealed record UnificationState(List<SubstEntry> substitutions, long next_id, List<Diagnostic> errors);

public sealed record Document(List<Def> defs, List<TypeDef> type_defs);

public abstract record ATypeDef;

public sealed record ARecordTypeDef(Name Field0, List<Name> Field1, List<ARecordFieldDef> Field2) : ATypeDef;
public sealed record AVariantTypeDef(Name Field0, List<Name> Field1, List<AVariantCtorDef> Field2) : ATypeDef;

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
public sealed record Equals_ : TokenKind;
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

public sealed record DefSetup(CodexType expected_type, CodexType remaining_type, UnificationState state, TypeEnv env);

public sealed record VarBinding(string var_name, string access, CodexType var_type);

public sealed record CheckResult(CodexType inferred_type, UnificationState state);

public sealed record Diagnostic(string code, string message, DiagnosticSeverity severity);

public sealed record ARecordFieldDef(Name name, ATypeExpr type_expr);

public sealed record RecordFieldExpr(Token name, Expr value);

public abstract record CompileResult;

public sealed record CompileOk(string Field0, ModuleResult Field1) : CompileResult;
public sealed record CompileError(List<Diagnostic> Field0) : CompileResult;

public abstract record ParseDefResult;

public sealed record DefOk(Def Field0, ParseState Field1) : ParseDefResult;
public sealed record DefNone(ParseState Field0) : ParseDefResult;

public abstract record AExpr;

public sealed record ALitExpr(string Field0, LiteralKind Field1) : AExpr;
public sealed record ANameExpr(Name Field0) : AExpr;
public sealed record AApplyExpr(AExpr Field0, AExpr Field1) : AExpr;
public sealed record ABinaryExpr(AExpr Field0, BinaryOp Field1, AExpr Field2) : AExpr;
public sealed record AUnaryExpr(AExpr Field0) : AExpr;
public sealed record AIfExpr(AExpr Field0, AExpr Field1, AExpr Field2) : AExpr;
public sealed record ALetExpr(List<ALetBind> Field0, AExpr Field1) : AExpr;
public sealed record ALambdaExpr(List<Name> Field0, AExpr Field1) : AExpr;
public sealed record AMatchExpr(AExpr Field0, List<AMatchArm> Field1) : AExpr;
public sealed record AListExpr(List<AExpr> Field0) : AExpr;
public sealed record ARecordExpr(Name Field0, List<AFieldExpr> Field1) : AExpr;
public sealed record AFieldAccess(AExpr Field0, Name Field1) : AExpr;
public sealed record ADoExpr(List<ADoStmt> Field0) : AExpr;
public sealed record AErrorExpr(string Field0) : AExpr;

public sealed record ADef(Name name, List<AParam> @params, List<ATypeExpr> declared_type, AExpr body);

public sealed record IRModule(Name name, List<IRDef> defs);

public sealed record ArityEntry(string def_name, long arity);

public sealed record AModule(Name name, List<ADef> defs, List<ATypeDef> type_defs);

public sealed record ResolveResult(List<Diagnostic> errors, List<string> top_level_names, List<string> type_names, List<string> ctor_names);

public sealed record IRBranch(IRPat pattern, IRExpr body);

public abstract record IRBinaryOp;

public sealed record IrAddInt : IRBinaryOp;
public sealed record IrSubInt : IRBinaryOp;
public sealed record IrMulInt : IRBinaryOp;
public sealed record IrDivInt : IRBinaryOp;
public sealed record IrPowInt : IRBinaryOp;
public sealed record IrAddNum : IRBinaryOp;
public sealed record IrSubNum : IRBinaryOp;
public sealed record IrMulNum : IRBinaryOp;
public sealed record IrDivNum : IRBinaryOp;
public sealed record IrEq : IRBinaryOp;
public sealed record IrNotEq : IRBinaryOp;
public sealed record IrLt : IRBinaryOp;
public sealed record IrGt : IRBinaryOp;
public sealed record IrLtEq : IRBinaryOp;
public sealed record IrGtEq : IRBinaryOp;
public sealed record IrAnd : IRBinaryOp;
public sealed record IrOr : IRBinaryOp;
public sealed record IrAppendText : IRBinaryOp;
public sealed record IrAppendList : IRBinaryOp;
public sealed record IrConsList : IRBinaryOp;

public static class Codex_Codex_Codex
{
    public static AExpr desugar_expr(Expr node)
    {
        while (true)
        {
            var _tco_s = node;
            if (_tco_s is LitExpr _tco_m0)
            {
                var tok = _tco_m0.Field0;
                return desugar_literal(tok);
            }
            else if (_tco_s is NameExpr _tco_m1)
            {
                var tok = _tco_m1.Field0;
                return new ANameExpr(make_name(tok.text));
            }
            else if (_tco_s is AppExpr _tco_m2)
            {
                var f = _tco_m2.Field0;
                var a = _tco_m2.Field1;
                return new AApplyExpr(desugar_expr(f), desugar_expr(a));
            }
            else if (_tco_s is BinExpr _tco_m3)
            {
                var l = _tco_m3.Field0;
                var op = _tco_m3.Field1;
                var r = _tco_m3.Field2;
                return new ABinaryExpr(desugar_expr(l), desugar_bin_op(op.kind), desugar_expr(r));
            }
            else if (_tco_s is UnaryExpr _tco_m4)
            {
                var op = _tco_m4.Field0;
                var operand = _tco_m4.Field1;
                return new AUnaryExpr(desugar_expr(operand));
            }
            else if (_tco_s is IfExpr _tco_m5)
            {
                var c = _tco_m5.Field0;
                var t = _tco_m5.Field1;
                var e = _tco_m5.Field2;
                return new AIfExpr(desugar_expr(c), desugar_expr(t), desugar_expr(e));
            }
            else if (_tco_s is LetExpr _tco_m6)
            {
                var bindings = _tco_m6.Field0;
                var body = _tco_m6.Field1;
                return new ALetExpr(map_list(new Func<LetBind, ALetBind>(desugar_let_bind), bindings), desugar_expr(body));
            }
            else if (_tco_s is MatchExpr _tco_m7)
            {
                var scrut = _tco_m7.Field0;
                var arms = _tco_m7.Field1;
                return new AMatchExpr(desugar_expr(scrut), map_list(new Func<MatchArm, AMatchArm>(desugar_match_arm), arms));
            }
            else if (_tco_s is ListExpr _tco_m8)
            {
                var elems = _tco_m8.Field0;
                return new AListExpr(map_list(new Func<Expr, AExpr>(desugar_expr), elems));
            }
            else if (_tco_s is RecordExpr _tco_m9)
            {
                var type_tok = _tco_m9.Field0;
                var fields = _tco_m9.Field1;
                return new ARecordExpr(make_name(type_tok.text), map_list(new Func<RecordFieldExpr, AFieldExpr>(desugar_field_expr), fields));
            }
            else if (_tco_s is FieldExpr _tco_m10)
            {
                var rec = _tco_m10.Field0;
                var field_tok = _tco_m10.Field1;
                return new AFieldAccess(desugar_expr(rec), make_name(field_tok.text));
            }
            else if (_tco_s is ParenExpr _tco_m11)
            {
                var inner = _tco_m11.Field0;
                var _tco_0 = inner;
                node = _tco_0;
                continue;
            }
            else if (_tco_s is DoExpr _tco_m12)
            {
                var stmts = _tco_m12.Field0;
                return new ADoExpr(map_list(new Func<DoStmt, ADoStmt>(desugar_do_stmt), stmts));
            }
            else if (_tco_s is ErrExpr _tco_m13)
            {
                var tok = _tco_m13.Field0;
                return new AErrorExpr(tok.text);
            }
        }
    }

    public static AExpr desugar_literal(Token tok)
    {
        return (is_literal(tok.kind) ? new ALitExpr(tok.text, classify_literal(tok.kind)) : new AErrorExpr(tok.text));
    }

    public static LiteralKind classify_literal(TokenKind k)
    {
        return ((Func<TokenKind, LiteralKind>)((_scrutinee0_) => (_scrutinee0_ is IntegerLiteral _mIntegerLiteral0_ ? new IntLit() : (_scrutinee0_ is NumberLiteral _mNumberLiteral0_ ? new NumLit() : (_scrutinee0_ is TextLiteral _mTextLiteral0_ ? new TextLit() : (_scrutinee0_ is TrueKeyword _mTrueKeyword0_ ? new BoolLit() : (_scrutinee0_ is FalseKeyword _mFalseKeyword0_ ? new BoolLit() : ((Func<TokenKind, LiteralKind>)((_) => new TextLit()))(_scrutinee0_))))))))(k);
    }

    public static ALetBind desugar_let_bind(LetBind b)
    {
        return new ALetBind(make_name(b.name.text), desugar_expr(b.value));
    }

    public static AMatchArm desugar_match_arm(MatchArm arm)
    {
        return new AMatchArm(desugar_pattern(arm.pattern), desugar_expr(arm.body));
    }

    public static AFieldExpr desugar_field_expr(RecordFieldExpr f)
    {
        return new AFieldExpr(make_name(f.name.text), desugar_expr(f.value));
    }

    public static ADoStmt desugar_do_stmt(DoStmt s)
    {
        return ((Func<DoStmt, ADoStmt>)((_scrutinee1_) => (_scrutinee1_ is DoBindStmt _mDoBindStmt1_ ? ((Func<Expr, ADoStmt>)((val) => ((Func<Token, ADoStmt>)((tok) => new ADoBindStmt(make_name(tok.text), desugar_expr(val))))((Token)_mDoBindStmt1_.Field0)))((Expr)_mDoBindStmt1_.Field1) : (_scrutinee1_ is DoExprStmt _mDoExprStmt1_ ? ((Func<Expr, ADoStmt>)((e) => new ADoExprStmt(desugar_expr(e))))((Expr)_mDoExprStmt1_.Field0) : throw new InvalidOperationException("Non-exhaustive match")))))(s);
    }

    public static BinaryOp desugar_bin_op(TokenKind k)
    {
        return ((Func<TokenKind, BinaryOp>)((_scrutinee2_) => (_scrutinee2_ is Plus _mPlus2_ ? new OpAdd() : (_scrutinee2_ is Minus _mMinus2_ ? new OpSub() : (_scrutinee2_ is Star _mStar2_ ? new OpMul() : (_scrutinee2_ is Slash _mSlash2_ ? new OpDiv() : (_scrutinee2_ is Caret _mCaret2_ ? new OpPow() : (_scrutinee2_ is DoubleEquals _mDoubleEquals2_ ? new OpEq() : (_scrutinee2_ is NotEquals _mNotEquals2_ ? new OpNotEq() : (_scrutinee2_ is LessThan _mLessThan2_ ? new OpLt() : (_scrutinee2_ is GreaterThan _mGreaterThan2_ ? new OpGt() : (_scrutinee2_ is LessOrEqual _mLessOrEqual2_ ? new OpLtEq() : (_scrutinee2_ is GreaterOrEqual _mGreaterOrEqual2_ ? new OpGtEq() : (_scrutinee2_ is TripleEquals _mTripleEquals2_ ? new OpDefEq() : (_scrutinee2_ is PlusPlus _mPlusPlus2_ ? new OpAppend() : (_scrutinee2_ is ColonColon _mColonColon2_ ? new OpCons() : (_scrutinee2_ is Ampersand _mAmpersand2_ ? new OpAnd() : (_scrutinee2_ is Pipe _mPipe2_ ? new OpOr() : ((Func<TokenKind, BinaryOp>)((_) => new OpAdd()))(_scrutinee2_)))))))))))))))))))(k);
    }

    public static APat desugar_pattern(Pat p)
    {
        return ((Func<Pat, APat>)((_scrutinee3_) => (_scrutinee3_ is VarPat _mVarPat3_ ? ((Func<Token, APat>)((tok) => new AVarPat(make_name(tok.text))))((Token)_mVarPat3_.Field0) : (_scrutinee3_ is LitPat _mLitPat3_ ? ((Func<Token, APat>)((tok) => new ALitPat(tok.text, classify_literal(tok.kind))))((Token)_mLitPat3_.Field0) : (_scrutinee3_ is CtorPat _mCtorPat3_ ? ((Func<List<Pat>, APat>)((subs) => ((Func<Token, APat>)((tok) => new ACtorPat(make_name(tok.text), map_list(new Func<Pat, APat>(desugar_pattern), subs))))((Token)_mCtorPat3_.Field0)))((List<Pat>)_mCtorPat3_.Field1) : (_scrutinee3_ is WildPat _mWildPat3_ ? ((Func<Token, APat>)((tok) => new AWildPat()))((Token)_mWildPat3_.Field0) : throw new InvalidOperationException("Non-exhaustive match")))))))(p);
    }

    public static ATypeExpr desugar_type_expr(TypeExpr t)
    {
        while (true)
        {
            var _tco_s = t;
            if (_tco_s is NamedType _tco_m0)
            {
                var tok = _tco_m0.Field0;
                return new ANamedType(make_name(tok.text));
            }
            else if (_tco_s is FunType _tco_m1)
            {
                var param = _tco_m1.Field0;
                var ret = _tco_m1.Field1;
                return new AFunType(desugar_type_expr(param), desugar_type_expr(ret));
            }
            else if (_tco_s is AppType _tco_m2)
            {
                var ctor = _tco_m2.Field0;
                var args = _tco_m2.Field1;
                return new AAppType(desugar_type_expr(ctor), map_list(new Func<TypeExpr, ATypeExpr>(desugar_type_expr), args));
            }
            else if (_tco_s is ParenType _tco_m3)
            {
                var inner = _tco_m3.Field0;
                var _tco_0 = inner;
                t = _tco_0;
                continue;
            }
            else if (_tco_s is ListType _tco_m4)
            {
                var elem = _tco_m4.Field0;
                return new AAppType(new ANamedType(make_name("List")), new List<ATypeExpr>() { desugar_type_expr(elem) });
            }
            else if (_tco_s is LinearTypeExpr _tco_m5)
            {
                var inner = _tco_m5.Field0;
                var _tco_0 = inner;
                t = _tco_0;
                continue;
            }
        }
    }

    public static ADef desugar_def(Def d)
    {
        return ((Func<List<ATypeExpr>, ADef>)((ann_types) => new ADef(make_name(d.name.text), map_list(new Func<Token, AParam>(desugar_param), d.@params), ann_types, desugar_expr(d.body))))(desugar_annotations(d.ann));
    }

    public static List<ATypeExpr> desugar_annotations(List<TypeAnn> anns)
    {
        return ((((long)anns.Count) == 0L) ? new List<ATypeExpr>() : ((Func<TypeAnn, List<ATypeExpr>>)((a) => new List<ATypeExpr>() { desugar_type_expr(a.type_expr) }))(anns[(int)0L]));
    }

    public static AParam desugar_param(Token tok)
    {
        return new AParam(make_name(tok.text));
    }

    public static ATypeDef desugar_type_def(TypeDef td)
    {
        return ((Func<TypeBody, ATypeDef>)((_scrutinee4_) => (_scrutinee4_ is RecordBody _mRecordBody4_ ? ((Func<List<RecordFieldDef>, ATypeDef>)((fields) => new ARecordTypeDef(make_name(td.name.text), map_list(new Func<Token, Name>(make_type_param_name), td.type_params), map_list(new Func<RecordFieldDef, ARecordFieldDef>(desugar_record_field_def), fields))))((List<RecordFieldDef>)_mRecordBody4_.Field0) : (_scrutinee4_ is VariantBody _mVariantBody4_ ? ((Func<List<VariantCtorDef>, ATypeDef>)((ctors) => new AVariantTypeDef(make_name(td.name.text), map_list(new Func<Token, Name>(make_type_param_name), td.type_params), map_list(new Func<VariantCtorDef, AVariantCtorDef>(desugar_variant_ctor_def), ctors))))((List<VariantCtorDef>)_mVariantBody4_.Field0) : throw new InvalidOperationException("Non-exhaustive match")))))(td.body);
    }

    public static Name make_type_param_name(Token tok)
    {
        return make_name(tok.text);
    }

    public static ARecordFieldDef desugar_record_field_def(RecordFieldDef f)
    {
        return new ARecordFieldDef(make_name(f.name.text), desugar_type_expr(f.type_expr));
    }

    public static AVariantCtorDef desugar_variant_ctor_def(VariantCtorDef c)
    {
        return new AVariantCtorDef(make_name(c.name.text), map_list(new Func<TypeExpr, ATypeExpr>(desugar_type_expr), c.fields));
    }

    public static AModule desugar_document(Document doc, string module_name)
    {
        return new AModule(make_name(module_name), map_list(new Func<Def, ADef>(desugar_def), doc.defs), map_list(new Func<TypeDef, ATypeDef>(desugar_type_def), doc.type_defs));
    }

    public static List<T1> map_list<T0, T1>(Func<T0, T1> f, List<T0> xs)
    {
        return map_list_loop(f, xs, 0L, ((long)xs.Count), new List<T1>());
    }

    public static List<T3> map_list_loop<T2, T3>(Func<T2, T3> f, List<T2> xs, long i, long len, List<T3> acc)
    {
        while (true)
        {
            if ((i == len))
            {
                return acc;
            }
            else
            {
                var _tco_0 = f;
                var _tco_1 = xs;
                var _tco_2 = (i + 1L);
                var _tco_3 = len;
                var _tco_4 = Enumerable.Concat(acc, new List<T3>() { f(xs[(int)i]) }).ToList();
                f = _tco_0;
                xs = _tco_1;
                i = _tco_2;
                len = _tco_3;
                acc = _tco_4;
                continue;
            }
        }
    }

    public static T4 fold_list<T4, T5>(Func<T4, Func<T5, T4>> f, T4 z, List<T5> xs)
    {
        return fold_list_loop(f, z, xs, 0L, ((long)xs.Count));
    }

    public static T6 fold_list_loop<T6, T7>(Func<T6, Func<T7, T6>> f, T6 z, List<T7> xs, long i, long len)
    {
        while (true)
        {
            if ((i == len))
            {
                return z;
            }
            else
            {
                var _tco_0 = f;
                var _tco_1 = f(z)(xs[(int)i]);
                var _tco_2 = xs;
                var _tco_3 = (i + 1L);
                var _tco_4 = len;
                f = _tco_0;
                z = _tco_1;
                xs = _tco_2;
                i = _tco_3;
                len = _tco_4;
                continue;
            }
        }
    }

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
        return ((Func<DiagnosticSeverity, string>)((_scrutinee5_) => (_scrutinee5_ is Error _mError5_ ? "error" : (_scrutinee5_ is Warning _mWarning5_ ? "warning" : (_scrutinee5_ is Info _mInfo5_ ? "info" : throw new InvalidOperationException("Non-exhaustive match"))))))(s);
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

    public static bool is_cs_keyword(string n)
    {
        return ((n == "class") ? true : ((n == "static") ? true : ((n == "void") ? true : ((n == "return") ? true : ((n == "if") ? true : ((n == "else") ? true : ((n == "for") ? true : ((n == "while") ? true : ((n == "do") ? true : ((n == "switch") ? true : ((n == "case") ? true : ((n == "break") ? true : ((n == "continue") ? true : ((n == "new") ? true : ((n == "this") ? true : ((n == "base") ? true : ((n == "null") ? true : ((n == "true") ? true : ((n == "false") ? true : ((n == "int") ? true : ((n == "long") ? true : ((n == "string") ? true : ((n == "bool") ? true : ((n == "double") ? true : ((n == "decimal") ? true : ((n == "object") ? true : ((n == "in") ? true : ((n == "is") ? true : ((n == "as") ? true : ((n == "typeof") ? true : ((n == "default") ? true : ((n == "throw") ? true : ((n == "try") ? true : ((n == "catch") ? true : ((n == "finally") ? true : ((n == "using") ? true : ((n == "namespace") ? true : ((n == "public") ? true : ((n == "private") ? true : ((n == "protected") ? true : ((n == "internal") ? true : ((n == "abstract") ? true : ((n == "sealed") ? true : ((n == "override") ? true : ((n == "virtual") ? true : ((n == "event") ? true : ((n == "delegate") ? true : ((n == "out") ? true : ((n == "ref") ? true : ((n == "params") ? true : false))))))))))))))))))))))))))))))))))))))))))))))))));
    }

    public static string sanitize(string name)
    {
        return ((Func<string, string>)((s) => (is_cs_keyword(s) ? string.Concat("@", s) : s)))(name.Replace("-", "_"));
    }

    public static string cs_type(CodexType ty)
    {
        while (true)
        {
            var _tco_s = ty;
            if (_tco_s is IntegerTy _tco_m0)
            {
                return "long";
            }
            else if (_tco_s is NumberTy _tco_m1)
            {
                return "decimal";
            }
            else if (_tco_s is TextTy _tco_m2)
            {
                return "string";
            }
            else if (_tco_s is BooleanTy _tco_m3)
            {
                return "bool";
            }
            else if (_tco_s is VoidTy _tco_m4)
            {
                return "void";
            }
            else if (_tco_s is NothingTy _tco_m5)
            {
                return "object";
            }
            else if (_tco_s is ErrorTy _tco_m6)
            {
                return "object";
            }
            else if (_tco_s is FunTy _tco_m7)
            {
                var p = _tco_m7.Field0;
                var r = _tco_m7.Field1;
                return string.Concat("Func<", string.Concat(cs_type(p), string.Concat(", ", string.Concat(cs_type(r), ">"))));
            }
            else if (_tco_s is ListTy _tco_m8)
            {
                var elem = _tco_m8.Field0;
                return string.Concat("List<", string.Concat(cs_type(elem), ">"));
            }
            else if (_tco_s is TypeVar _tco_m9)
            {
                var id = _tco_m9.Field0;
                return string.Concat("T", (id).ToString());
            }
            else if (_tco_s is ForAllTy _tco_m10)
            {
                var id = _tco_m10.Field0;
                var body = _tco_m10.Field1;
                var _tco_0 = body;
                ty = _tco_0;
                continue;
            }
            else if (_tco_s is SumTy _tco_m11)
            {
                var name = _tco_m11.Field0;
                var ctors = _tco_m11.Field1;
                return sanitize(name.value);
            }
            else if (_tco_s is RecordTy _tco_m12)
            {
                var name = _tco_m12.Field0;
                var fields = _tco_m12.Field1;
                return sanitize(name.value);
            }
            else if (_tco_s is ConstructedTy _tco_m13)
            {
                var name = _tco_m13.Field0;
                var args = _tco_m13.Field1;
                return sanitize(name.value);
            }
        }
    }

    public static List<ArityEntry> build_arities(List<IRDef> defs, long i)
    {
        return ((i == ((long)defs.Count)) ? new List<ArityEntry>() : ((Func<IRDef, List<ArityEntry>>)((d) => Enumerable.Concat(new List<ArityEntry>() { new ArityEntry(d.name, ((long)d.@params.Count)) }, build_arities(defs, (i + 1L))).ToList()))(defs[(int)i]));
    }

    public static long lookup_arity(List<ArityEntry> entries, string name)
    {
        return lookup_arity_loop(entries, name, 0L);
    }

    public static long lookup_arity_loop(List<ArityEntry> entries, string name, long i)
    {
        while (true)
        {
            if ((i == ((long)entries.Count)))
            {
                return (0L - 1L);
            }
            else
            {
                var e = entries[(int)i];
                if ((e.def_name == name))
                {
                    return e.arity;
                }
                else
                {
                    var _tco_0 = entries;
                    var _tco_1 = name;
                    var _tco_2 = (i + 1L);
                    entries = _tco_0;
                    name = _tco_1;
                    i = _tco_2;
                    continue;
                }
            }
        }
    }

    public static bool is_ctor_name(string n)
    {
        return ((((long)n.Length) == 0L) ? false : is_upper_char(n[(int)0L].ToString()));
    }

    public static string emit_expr(IRExpr e, List<ArityEntry> arities, long mid)
    {
        return ((Func<IRExpr, string>)((_scrutinee6_) => (_scrutinee6_ is IrIntLit _mIrIntLit6_ ? ((Func<long, string>)((n) => string.Concat((n).ToString(), "L")))((long)_mIrIntLit6_.Field0) : (_scrutinee6_ is IrNumLit _mIrNumLit6_ ? ((Func<long, string>)((n) => string.Concat((n).ToString(), "m")))((long)_mIrNumLit6_.Field0) : (_scrutinee6_ is IrTextLit _mIrTextLit6_ ? ((Func<string, string>)((s) => string.Concat("\"", string.Concat(escape_text(s), "\""))))((string)_mIrTextLit6_.Field0) : (_scrutinee6_ is IrBoolLit _mIrBoolLit6_ ? ((Func<bool, string>)((b) => (b ? "true" : "false")))((bool)_mIrBoolLit6_.Field0) : (_scrutinee6_ is IrName _mIrName6_ ? ((Func<CodexType, string>)((ty) => ((Func<string, string>)((n) => emit_name(n, ty, arities)))((string)_mIrName6_.Field0)))((CodexType)_mIrName6_.Field1) : (_scrutinee6_ is IrBinary _mIrBinary6_ ? ((Func<CodexType, string>)((ty) => ((Func<IRExpr, string>)((r) => ((Func<IRExpr, string>)((l) => ((Func<IRBinaryOp, string>)((op) => emit_binary(op, l, r, ty, arities, mid)))((IRBinaryOp)_mIrBinary6_.Field0)))((IRExpr)_mIrBinary6_.Field1)))((IRExpr)_mIrBinary6_.Field2)))((CodexType)_mIrBinary6_.Field3) : (_scrutinee6_ is IrNegate _mIrNegate6_ ? ((Func<IRExpr, string>)((operand) => string.Concat("(-", string.Concat(emit_expr(operand, arities, mid), ")"))))((IRExpr)_mIrNegate6_.Field0) : (_scrutinee6_ is IrIf _mIrIf6_ ? ((Func<CodexType, string>)((ty) => ((Func<IRExpr, string>)((el) => ((Func<IRExpr, string>)((t) => ((Func<IRExpr, string>)((c) => string.Concat("(", string.Concat(emit_expr(c, arities, mid), string.Concat(" ? ", string.Concat(emit_expr(t, arities, mid), string.Concat(" : ", string.Concat(emit_expr(el, arities, mid), ")"))))))))((IRExpr)_mIrIf6_.Field0)))((IRExpr)_mIrIf6_.Field1)))((IRExpr)_mIrIf6_.Field2)))((CodexType)_mIrIf6_.Field3) : (_scrutinee6_ is IrLet _mIrLet6_ ? ((Func<IRExpr, string>)((body) => ((Func<IRExpr, string>)((val) => ((Func<CodexType, string>)((ty) => ((Func<string, string>)((name) => emit_let(name, ty, val, body, arities, mid)))((string)_mIrLet6_.Field0)))((CodexType)_mIrLet6_.Field1)))((IRExpr)_mIrLet6_.Field2)))((IRExpr)_mIrLet6_.Field3) : (_scrutinee6_ is IrApply _mIrApply6_ ? ((Func<CodexType, string>)((ty) => ((Func<IRExpr, string>)((a) => ((Func<IRExpr, string>)((f) => emit_apply(f, a, ty, arities, mid)))((IRExpr)_mIrApply6_.Field0)))((IRExpr)_mIrApply6_.Field1)))((CodexType)_mIrApply6_.Field2) : (_scrutinee6_ is IrLambda _mIrLambda6_ ? ((Func<CodexType, string>)((ty) => ((Func<IRExpr, string>)((body) => ((Func<List<IRParam>, string>)((@params) => emit_lambda(@params, body, arities, mid)))((List<IRParam>)_mIrLambda6_.Field0)))((IRExpr)_mIrLambda6_.Field1)))((CodexType)_mIrLambda6_.Field2) : (_scrutinee6_ is IrList _mIrList6_ ? ((Func<CodexType, string>)((ty) => ((Func<List<IRExpr>, string>)((elems) => emit_list(elems, ty, arities, mid)))((List<IRExpr>)_mIrList6_.Field0)))((CodexType)_mIrList6_.Field1) : (_scrutinee6_ is IrMatch _mIrMatch6_ ? ((Func<CodexType, string>)((ty) => ((Func<List<IRBranch>, string>)((branches) => ((Func<IRExpr, string>)((scrut) => emit_match(scrut, branches, ty, arities, mid)))((IRExpr)_mIrMatch6_.Field0)))((List<IRBranch>)_mIrMatch6_.Field1)))((CodexType)_mIrMatch6_.Field2) : (_scrutinee6_ is IrDo _mIrDo6_ ? ((Func<CodexType, string>)((ty) => ((Func<List<IRDoStmt>, string>)((stmts) => emit_do(stmts, arities, mid)))((List<IRDoStmt>)_mIrDo6_.Field0)))((CodexType)_mIrDo6_.Field1) : (_scrutinee6_ is IrRecord _mIrRecord6_ ? ((Func<CodexType, string>)((ty) => ((Func<List<IRFieldVal>, string>)((fields) => ((Func<string, string>)((name) => emit_record(name, fields, arities, mid)))((string)_mIrRecord6_.Field0)))((List<IRFieldVal>)_mIrRecord6_.Field1)))((CodexType)_mIrRecord6_.Field2) : (_scrutinee6_ is IrFieldAccess _mIrFieldAccess6_ ? ((Func<CodexType, string>)((ty) => ((Func<string, string>)((field) => ((Func<IRExpr, string>)((rec) => string.Concat(emit_expr(rec, arities, mid), string.Concat(".", sanitize(field)))))((IRExpr)_mIrFieldAccess6_.Field0)))((string)_mIrFieldAccess6_.Field1)))((CodexType)_mIrFieldAccess6_.Field2) : (_scrutinee6_ is IrError _mIrError6_ ? ((Func<CodexType, string>)((ty) => ((Func<string, string>)((msg) => string.Concat("throw new InvalidOperationException(\"", string.Concat(escape_text(msg), "\")"))))((string)_mIrError6_.Field0)))((CodexType)_mIrError6_.Field1) : throw new InvalidOperationException("Non-exhaustive match"))))))))))))))))))))(e);
    }

    public static string emit_name(string n, CodexType ty, List<ArityEntry> arities)
    {
        return ((n == "read-line") ? "Console.ReadLine()" : ((n == "show") ? "new Func<object, string>(x => Convert.ToString(x))" : ((n == "negate") ? "new Func<long, long>(x => -x)" : (is_ctor_name(n) ? emit_ctor_name_ref(n, ty) : emit_def_name_ref(n, ty, arities)))));
    }

    public static string emit_ctor_name_ref(string n, CodexType ty)
    {
        return (ty is FunTy _mFunTy7_ ? ((Func<CodexType, string>)((r) => ((Func<CodexType, string>)((p) => sanitize(n)))((CodexType)_mFunTy7_.Field0)))((CodexType)_mFunTy7_.Field1) : ((Func<CodexType, string>)((_) => string.Concat("new ", string.Concat(sanitize(n), "()"))))(ty));
    }

    public static string emit_def_name_ref(string n, CodexType ty, List<ArityEntry> arities)
    {
        return ((Func<long, string>)((ar) => ((ar == 0L) ? emit_zero_arity_name_ref(n, ty) : sanitize(n))))(lookup_arity(arities, n));
    }

    public static string emit_zero_arity_name_ref(string n, CodexType ty)
    {
        return (ty is FunTy _mFunTy8_ ? ((Func<CodexType, string>)((r) => ((Func<CodexType, string>)((p) => sanitize(n)))((CodexType)_mFunTy8_.Field0)))((CodexType)_mFunTy8_.Field1) : ((Func<CodexType, string>)((_) => string.Concat(sanitize(n), "()")))(ty));
    }

    public static string emit_binary(IRBinaryOp op, IRExpr l, IRExpr r, CodexType ty, List<ArityEntry> arities, long mid)
    {
        return ((Func<IRBinaryOp, string>)((_scrutinee9_) => (_scrutinee9_ is IrAppendText _mIrAppendText9_ ? string.Concat("string.Concat(", string.Concat(emit_expr(l, arities, mid), string.Concat(", ", string.Concat(emit_expr(r, arities, mid), ")")))) : (_scrutinee9_ is IrAppendList _mIrAppendList9_ ? string.Concat("Enumerable.Concat(", string.Concat(emit_expr(l, arities, mid), string.Concat(", ", string.Concat(emit_expr(r, arities, mid), ").ToList()")))) : (_scrutinee9_ is IrConsList _mIrConsList9_ ? string.Concat("new List<", string.Concat(cs_type(ir_expr_type(l)), string.Concat("> { ", string.Concat(emit_expr(l, arities, mid), string.Concat(" }.Concat(", string.Concat(emit_expr(r, arities, mid), ").ToList()")))))) : (_scrutinee9_ is IrPowInt _mIrPowInt9_ ? string.Concat("(long)Math.Pow((double)", string.Concat(emit_expr(l, arities, mid), string.Concat(", (double)", string.Concat(emit_expr(r, arities, mid), ")")))) : ((Func<IRBinaryOp, string>)((_) => string.Concat("(", string.Concat(emit_expr(l, arities, mid), string.Concat(" ", string.Concat(emit_bin_op(op), string.Concat(" ", string.Concat(emit_expr(r, arities, mid), ")"))))))))(_scrutinee9_)))))))(op);
    }

    public static string escape_text(string s)
    {
        return s.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }

    public static CodexType ir_expr_type(IRExpr e)
    {
        while (true)
        {
            var _tco_s = e;
            if (_tco_s is IrIntLit _tco_m0)
            {
                var n = _tco_m0.Field0;
                return new IntegerTy();
            }
            else if (_tco_s is IrNumLit _tco_m1)
            {
                var n = _tco_m1.Field0;
                return new NumberTy();
            }
            else if (_tco_s is IrTextLit _tco_m2)
            {
                var s = _tco_m2.Field0;
                return new TextTy();
            }
            else if (_tco_s is IrBoolLit _tco_m3)
            {
                var b = _tco_m3.Field0;
                return new BooleanTy();
            }
            else if (_tco_s is IrName _tco_m4)
            {
                var n = _tco_m4.Field0;
                var ty = _tco_m4.Field1;
                return ty;
            }
            else if (_tco_s is IrBinary _tco_m5)
            {
                var op = _tco_m5.Field0;
                var l = _tco_m5.Field1;
                var r = _tco_m5.Field2;
                var ty = _tco_m5.Field3;
                return ty;
            }
            else if (_tco_s is IrNegate _tco_m6)
            {
                var operand = _tco_m6.Field0;
                return new IntegerTy();
            }
            else if (_tco_s is IrIf _tco_m7)
            {
                var c = _tco_m7.Field0;
                var t = _tco_m7.Field1;
                var el = _tco_m7.Field2;
                var ty = _tco_m7.Field3;
                return ty;
            }
            else if (_tco_s is IrLet _tco_m8)
            {
                var name = _tco_m8.Field0;
                var ty = _tco_m8.Field1;
                var val = _tco_m8.Field2;
                var body = _tco_m8.Field3;
                var _tco_0 = body;
                e = _tco_0;
                continue;
            }
            else if (_tco_s is IrApply _tco_m9)
            {
                var f = _tco_m9.Field0;
                var a = _tco_m9.Field1;
                var ty = _tco_m9.Field2;
                return ty;
            }
            else if (_tco_s is IrLambda _tco_m10)
            {
                var @params = _tco_m10.Field0;
                var body = _tco_m10.Field1;
                var ty = _tco_m10.Field2;
                return ty;
            }
            else if (_tco_s is IrList _tco_m11)
            {
                var elems = _tco_m11.Field0;
                var ty = _tco_m11.Field1;
                return new ListTy(ty);
            }
            else if (_tco_s is IrMatch _tco_m12)
            {
                var scrut = _tco_m12.Field0;
                var branches = _tco_m12.Field1;
                var ty = _tco_m12.Field2;
                return ty;
            }
            else if (_tco_s is IrDo _tco_m13)
            {
                var stmts = _tco_m13.Field0;
                var ty = _tco_m13.Field1;
                return ty;
            }
            else if (_tco_s is IrRecord _tco_m14)
            {
                var name = _tco_m14.Field0;
                var fields = _tco_m14.Field1;
                var ty = _tco_m14.Field2;
                return ty;
            }
            else if (_tco_s is IrFieldAccess _tco_m15)
            {
                var rec = _tco_m15.Field0;
                var field = _tco_m15.Field1;
                var ty = _tco_m15.Field2;
                return ty;
            }
            else if (_tco_s is IrError _tco_m16)
            {
                var msg = _tco_m16.Field0;
                var ty = _tco_m16.Field1;
                return ty;
            }
        }
    }

    public static string emit_bin_op(IRBinaryOp op)
    {
        return ((Func<IRBinaryOp, string>)((_scrutinee10_) => (_scrutinee10_ is IrAddInt _mIrAddInt10_ ? "+" : (_scrutinee10_ is IrSubInt _mIrSubInt10_ ? "-" : (_scrutinee10_ is IrMulInt _mIrMulInt10_ ? "*" : (_scrutinee10_ is IrDivInt _mIrDivInt10_ ? "/" : (_scrutinee10_ is IrPowInt _mIrPowInt10_ ? "^" : (_scrutinee10_ is IrAddNum _mIrAddNum10_ ? "+" : (_scrutinee10_ is IrSubNum _mIrSubNum10_ ? "-" : (_scrutinee10_ is IrMulNum _mIrMulNum10_ ? "*" : (_scrutinee10_ is IrDivNum _mIrDivNum10_ ? "/" : (_scrutinee10_ is IrEq _mIrEq10_ ? "==" : (_scrutinee10_ is IrNotEq _mIrNotEq10_ ? "!=" : (_scrutinee10_ is IrLt _mIrLt10_ ? "<" : (_scrutinee10_ is IrGt _mIrGt10_ ? ">" : (_scrutinee10_ is IrLtEq _mIrLtEq10_ ? "<=" : (_scrutinee10_ is IrGtEq _mIrGtEq10_ ? ">=" : (_scrutinee10_ is IrAnd _mIrAnd10_ ? "&&" : (_scrutinee10_ is IrOr _mIrOr10_ ? "||" : (_scrutinee10_ is IrAppendText _mIrAppendText10_ ? "+" : (_scrutinee10_ is IrAppendList _mIrAppendList10_ ? "+" : (_scrutinee10_ is IrConsList _mIrConsList10_ ? "+" : throw new InvalidOperationException("Non-exhaustive match")))))))))))))))))))))))(op);
    }

    public static string emit_let(string name, CodexType ty, IRExpr val, IRExpr body, List<ArityEntry> arities, long mid)
    {
        return ((Func<CodexType, string>)((body_ty) => string.Concat("((Func<", string.Concat(cs_type(ty), string.Concat(", ", string.Concat(cs_type(body_ty), string.Concat(">)((", string.Concat(sanitize(name), string.Concat(") => ", string.Concat(emit_expr(body, arities, mid), string.Concat("))(", string.Concat(emit_expr(val, arities, mid), ")"))))))))))))(ir_expr_type(body));
    }

    public static string emit_lambda(List<IRParam> @params, IRExpr body, List<ArityEntry> arities, long mid)
    {
        return ((((long)@params.Count) == 0L) ? string.Concat("(() => ", string.Concat(emit_expr(body, arities, mid), ")")) : ((((long)@params.Count) == 1L) ? ((Func<IRParam, string>)((p) => string.Concat("((", string.Concat(cs_type(p.type_val), string.Concat(" ", string.Concat(sanitize(p.name), string.Concat(") => ", string.Concat(emit_expr(body, arities, mid), ")"))))))))(@params[(int)0L]) : string.Concat("(() => ", string.Concat(emit_expr(body, arities, mid), ")"))));
    }

    public static string emit_list(List<IRExpr> elems, CodexType ty, List<ArityEntry> arities, long mid)
    {
        return ((((long)elems.Count) == 0L) ? string.Concat("new List<", string.Concat(cs_type(ty), ">()")) : string.Concat("new List<", string.Concat(cs_type(ty), string.Concat(">() { ", string.Concat(emit_list_elems(elems, 0L, arities, mid), " }")))));
    }

    public static string emit_list_elems(List<IRExpr> elems, long i, List<ArityEntry> arities, long mid)
    {
        return ((i == ((long)elems.Count)) ? "" : ((i == (((long)elems.Count) - 1L)) ? emit_expr(elems[(int)i], arities, mid) : string.Concat(emit_expr(elems[(int)i], arities, mid), string.Concat(", ", emit_list_elems(elems, (i + 1L), arities, mid)))));
    }

    public static string find_apply_root(IRExpr e)
    {
        while (true)
        {
            var _tco_s = e;
            if (_tco_s is IrApply _tco_m0)
            {
                var f = _tco_m0.Field0;
                var a = _tco_m0.Field1;
                var ty = _tco_m0.Field2;
                var _tco_0 = f;
                e = _tco_0;
                continue;
            }
            else if (_tco_s is IrName _tco_m1)
            {
                var n = _tco_m1.Field0;
                var ty = _tco_m1.Field1;
                return n;
            }
            {
                var _ = _tco_s;
                return "";
            }
        }
    }

    public static List<IRExpr> collect_apply_args(IRExpr e, List<IRExpr> acc)
    {
        while (true)
        {
            var _tco_s = e;
            if (_tco_s is IrApply _tco_m0)
            {
                var f = _tco_m0.Field0;
                var a = _tco_m0.Field1;
                var ty = _tco_m0.Field2;
                var _tco_0 = f;
                var _tco_1 = Enumerable.Concat(new List<IRExpr>() { a }, acc).ToList();
                e = _tco_0;
                acc = _tco_1;
                continue;
            }
            {
                var _ = _tco_s;
                return acc;
            }
        }
    }

    public static bool is_single_builtin(IRExpr f)
    {
        return (f is IrName _mIrName11_ ? ((Func<CodexType, bool>)((ty) => ((Func<string, bool>)((n) => ((n == "show") ? true : ((n == "negate") ? true : ((n == "print-line") ? true : ((n == "text-length") ? true : ((n == "integer-to-text") ? true : ((n == "text-to-integer") ? true : ((n == "is-letter") ? true : ((n == "is-digit") ? true : ((n == "is-whitespace") ? true : ((n == "char-code") ? true : ((n == "code-to-char") ? true : ((n == "list-length") ? true : ((n == "open-file") ? true : ((n == "read-all") ? true : ((n == "close-file") ? true : false)))))))))))))))))((string)_mIrName11_.Field0)))((CodexType)_mIrName11_.Field1) : ((Func<IRExpr, bool>)((_) => false))(f));
    }

    public static string emit_single_builtin(IRExpr f, IRExpr a, List<ArityEntry> arities, long mid)
    {
        return (f is IrName _mIrName12_ ? ((Func<CodexType, string>)((ty) => ((Func<string, string>)((n) => ((n == "show") ? string.Concat("Convert.ToString(", string.Concat(emit_expr(a, arities, mid), ")")) : ((n == "negate") ? string.Concat("(-", string.Concat(emit_expr(a, arities, mid), ")")) : ((n == "print-line") ? string.Concat("Console.WriteLine(", string.Concat(emit_expr(a, arities, mid), ")")) : ((n == "text-length") ? string.Concat("((long)", string.Concat(emit_expr(a, arities, mid), ".Length)")) : ((n == "integer-to-text") ? string.Concat("(", string.Concat(emit_expr(a, arities, mid), ").ToString()")) : ((n == "text-to-integer") ? string.Concat("long.Parse(", string.Concat(emit_expr(a, arities, mid), ")")) : ((n == "is-letter") ? string.Concat("(", string.Concat(emit_expr(a, arities, mid), string.Concat(".Length > 0 && char.IsLetter(", string.Concat(emit_expr(a, arities, mid), "[0]))")))) : ((n == "is-digit") ? string.Concat("(", string.Concat(emit_expr(a, arities, mid), string.Concat(".Length > 0 && char.IsDigit(", string.Concat(emit_expr(a, arities, mid), "[0]))")))) : ((n == "is-whitespace") ? string.Concat("(", string.Concat(emit_expr(a, arities, mid), string.Concat(".Length > 0 && char.IsWhiteSpace(", string.Concat(emit_expr(a, arities, mid), "[0]))")))) : ((n == "char-code") ? string.Concat("((long)", string.Concat(emit_expr(a, arities, mid), "[0])")) : ((n == "code-to-char") ? string.Concat("((char)", string.Concat(emit_expr(a, arities, mid), ").ToString()")) : ((n == "list-length") ? string.Concat("((long)", string.Concat(emit_expr(a, arities, mid), ".Count)")) : ((n == "open-file") ? string.Concat("File.OpenRead(", string.Concat(emit_expr(a, arities, mid), ")")) : ((n == "read-all") ? string.Concat("new System.IO.StreamReader(", string.Concat(emit_expr(a, arities, mid), ").ReadToEnd()")) : ((n == "close-file") ? string.Concat(emit_expr(a, arities, mid), ".Dispose()") : string.Concat(emit_expr(f, arities, mid), string.Concat("(", string.Concat(emit_expr(a, arities, mid), ")"))))))))))))))))))))((string)_mIrName12_.Field0)))((CodexType)_mIrName12_.Field1) : ((Func<IRExpr, string>)((_) => string.Concat(emit_expr(f, arities, mid), string.Concat("(", string.Concat(emit_expr(a, arities, mid), ")")))))(f));
    }

    public static bool is_multi_builtin(IRExpr f)
    {
        return ((Func<string, bool>)((root) => ((root == "char-at") ? true : ((root == "substring") ? true : ((root == "list-at") ? true : ((root == "text-replace") ? true : false))))))(find_apply_root(f));
    }

    public static string emit_multi_builtin(IRExpr f, IRExpr a, List<ArityEntry> arities, long mid)
    {
        return ((Func<string, string>)((root) => ((Func<List<IRExpr>, string>)((args) => (((root == "char-at") && (((long)args.Count) == 2L)) ? string.Concat(emit_expr(args[(int)0L], arities, mid), string.Concat("[(int)", string.Concat(emit_expr(args[(int)1L], arities, mid), "].ToString()"))) : (((root == "substring") && (((long)args.Count) == 3L)) ? string.Concat(emit_expr(args[(int)0L], arities, mid), string.Concat(".Substring((int)", string.Concat(emit_expr(args[(int)1L], arities, mid), string.Concat(", (int)", string.Concat(emit_expr(args[(int)2L], arities, mid), ")"))))) : (((root == "list-at") && (((long)args.Count) == 2L)) ? string.Concat(emit_expr(args[(int)0L], arities, mid), string.Concat("[(int)", string.Concat(emit_expr(args[(int)1L], arities, mid), "]"))) : (((root == "text-replace") && (((long)args.Count) == 3L)) ? string.Concat(emit_expr(args[(int)0L], arities, mid), string.Concat(".Replace(", string.Concat(emit_expr(args[(int)1L], arities, mid), string.Concat(", ", string.Concat(emit_expr(args[(int)2L], arities, mid), ")"))))) : string.Concat(emit_expr(f, arities, mid), string.Concat("(", string.Concat(emit_expr(a, arities, mid), ")")))))))))(collect_apply_args(f, new List<IRExpr>() { a }))))(find_apply_root(f));
    }

    public static string emit_apply(IRExpr f, IRExpr a, CodexType ty, List<ArityEntry> arities, long mid)
    {
        return (is_single_builtin(f) ? emit_single_builtin(f, a, arities, mid) : (is_multi_builtin(f) ? emit_multi_builtin(f, a, arities, mid) : ((Func<string, string>)((root) => ((Func<List<IRExpr>, string>)((args) => (is_ctor_name(root) ? string.Concat("new ", string.Concat(sanitize(root), string.Concat("(", string.Concat(emit_expr_list(args, arities, 0L, mid), ")")))) : ((Func<long, string>)((ar) => (((ar > 1L) && (((long)args.Count) == ar)) ? string.Concat(sanitize(root), string.Concat("(", string.Concat(emit_arg_list(args, arities, 0L, mid), ")"))) : (((ar > 1L) && (((long)args.Count) < ar)) ? emit_partial_application(root, ar, args, arities, mid) : string.Concat(emit_expr(f, arities, mid), string.Concat("(", string.Concat(emit_argument(a, arities, mid), ")")))))))(lookup_arity(arities, root)))))(collect_apply_args(f, new List<IRExpr>() { a }))))(find_apply_root(f))));
    }

    public static string emit_expr_list(List<IRExpr> args, List<ArityEntry> arities, long i, long mid)
    {
        return ((i == ((long)args.Count)) ? "" : string.Concat(emit_expr(args[(int)i], arities, mid), string.Concat(((i < (((long)args.Count) - 1L)) ? ", " : ""), emit_expr_list(args, arities, (i + 1L), mid))));
    }

    public static string emit_arg_list(List<IRExpr> args, List<ArityEntry> arities, long i, long mid)
    {
        return ((i == ((long)args.Count)) ? "" : string.Concat(emit_argument(args[(int)i], arities, mid), string.Concat(((i < (((long)args.Count) - 1L)) ? ", " : ""), emit_arg_list(args, arities, (i + 1L), mid))));
    }

    public static string emit_argument(IRExpr a, List<ArityEntry> arities, long mid)
    {
        return (a is IrName _mIrName13_ ? ((Func<CodexType, string>)((ty) => ((Func<string, string>)((n) => emit_name_argument(n, ty, arities, mid)))((string)_mIrName13_.Field0)))((CodexType)_mIrName13_.Field1) : ((Func<IRExpr, string>)((_) => emit_expr(a, arities, mid)))(a));
    }

    public static bool has_type_variable(CodexType ty)
    {
        while (true)
        {
            var _tco_s = ty;
            if (_tco_s is TypeVar _tco_m0)
            {
                var id = _tco_m0.Field0;
                return true;
            }
            else if (_tco_s is FunTy _tco_m1)
            {
                var p = _tco_m1.Field0;
                var r = _tco_m1.Field1;
                return (has_type_variable(p) || has_type_variable(r));
            }
            else if (_tco_s is ListTy _tco_m2)
            {
                var elem = _tco_m2.Field0;
                var _tco_0 = elem;
                ty = _tco_0;
                continue;
            }
            {
                var _ = _tco_s;
                return false;
            }
        }
    }

    public static string emit_name_argument(string n, CodexType ty, List<ArityEntry> arities, long mid)
    {
        return ((n == "show") ? "new Func<object, string>(x => Convert.ToString(x))" : ((n == "negate") ? "new Func<long, long>(x => -x)" : emit_name_argument_by_type(n, ty, arities, mid)));
    }

    public static string emit_name_argument_by_type(string n, CodexType ty, List<ArityEntry> arities, long mid)
    {
        return (ty is FunTy _mFunTy14_ ? ((Func<CodexType, string>)((r) => ((Func<CodexType, string>)((p) => (is_ctor_name(n) ? sanitize(n) : (has_type_variable(ty) ? sanitize(n) : ((Func<long, string>)((ar) => ((ar > 1L) ? emit_partial_application(n, ar, new List<IRExpr>(), arities, mid) : string.Concat("new Func<", string.Concat(cs_type(p), string.Concat(", ", string.Concat(cs_type(r), string.Concat(">(", string.Concat(sanitize(n), ")")))))))))(lookup_arity(arities, n))))))((CodexType)_mFunTy14_.Field0)))((CodexType)_mFunTy14_.Field1) : ((Func<CodexType, string>)((_) => sanitize(n)))(ty));
    }

    public static string emit_partial_application(string name, long arity, List<IRExpr> applied, List<ArityEntry> arities, long mid)
    {
        return ((Func<long, string>)((remaining) => string.Concat(emit_partial_params(remaining, 0L), string.Concat(sanitize(name), string.Concat("(", string.Concat(emit_partial_applied(applied, arities, 0L, mid), string.Concat(emit_partial_placeholders(remaining, 0L), ")")))))))((arity - ((long)applied.Count)));
    }

    public static string emit_partial_params(long remaining, long i)
    {
        return ((i == remaining) ? "" : string.Concat("(_p", string.Concat((i).ToString(), string.Concat("_) => ", emit_partial_params(remaining, (i + 1L))))));
    }

    public static string emit_partial_applied(List<IRExpr> args, List<ArityEntry> arities, long i, long mid)
    {
        return ((i == ((long)args.Count)) ? "" : string.Concat(emit_argument(args[(int)i], arities, mid), string.Concat(", ", emit_partial_applied(args, arities, (i + 1L), mid))));
    }

    public static string emit_partial_placeholders(long remaining, long i)
    {
        return ((i == remaining) ? "" : string.Concat(((i > 0L) ? ", " : ""), string.Concat("_p", string.Concat((i).ToString(), string.Concat("_", emit_partial_placeholders(remaining, (i + 1L)))))));
    }

    public static long count_ctor_lit_branches(List<IRBranch> branches, long i)
    {
        return ((i == ((long)branches.Count)) ? 0L : ((Func<IRBranch, long>)((b) => ((Func<long, long>)((rest) => ((Func<IRPat, long>)((_scrutinee15_) => (_scrutinee15_ is IrCtorPat _mIrCtorPat15_ ? ((Func<CodexType, long>)((ty) => ((Func<List<IRPat>, long>)((s) => ((Func<string, long>)((n) => (1L + rest)))((string)_mIrCtorPat15_.Field0)))((List<IRPat>)_mIrCtorPat15_.Field1)))((CodexType)_mIrCtorPat15_.Field2) : (_scrutinee15_ is IrLitPat _mIrLitPat15_ ? ((Func<CodexType, long>)((ty) => ((Func<string, long>)((t) => (1L + rest)))((string)_mIrLitPat15_.Field0)))((CodexType)_mIrLitPat15_.Field1) : ((Func<IRPat, long>)((_) => rest))(_scrutinee15_)))))(b.pattern)))(count_ctor_lit_branches(branches, (i + 1L)))))(branches[(int)i]));
    }

    public static string emit_match(IRExpr scrut, List<IRBranch> branches, CodexType ty, List<ArityEntry> arities, long mid)
    {
        return ((Func<long, string>)((match_id) => ((Func<bool, string>)((has_multi) => ((Func<string, string>)((scrut_ref) => ((Func<string, string>)((prefix) => ((Func<string, string>)((body) => ((Func<string, string>)((suffix) => string.Concat(prefix, string.Concat(body, suffix))))((has_multi ? string.Concat("))(", string.Concat(emit_expr(scrut, arities, (mid + 1L)), ")")) : ""))))(emit_match_branches(scrut, branches, ty, arities, (mid + 1L), 0L, has_multi, scrut_ref, 0L))))((has_multi ? string.Concat("((Func<", string.Concat(cs_type(ir_expr_type(scrut)), string.Concat(", ", string.Concat(cs_type(ty), string.Concat(">)((", string.Concat(scrut_ref, ") => ")))))) : ""))))(string.Concat("_scrutinee", string.Concat((match_id).ToString(), "_")))))((count_ctor_lit_branches(branches, 0L) > 1L))))(mid);
    }

    public static string emit_match_branches(IRExpr scrut, List<IRBranch> branches, CodexType ty, List<ArityEntry> arities, long mid, long i, bool has_multi, string scrut_ref, long open_parens)
    {
        return ((i == ((long)branches.Count)) ? string.Concat(" : throw new InvalidOperationException(\"Non-exhaustive match\")", string.Concat(repeat_close_parens(open_parens), "")) : ((Func<IRBranch, string>)((b) => ((Func<string, string>)((sep) => ((Func<IRPat, string>)((_scrutinee16_) => (_scrutinee16_ is IrCtorPat _mIrCtorPat16_ ? ((Func<CodexType, string>)((cty) => ((Func<List<IRPat>, string>)((subs) => ((Func<string, string>)((name) => ((Func<string, string>)((ctor_id) => ((Func<string, string>)((binding) => ((Func<string, string>)((scrut_text) => ((Func<string, string>)((ctor_body) => ((Func<string, string>)((rest) => string.Concat(sep, string.Concat("(", string.Concat(scrut_text, string.Concat(" is ", string.Concat(ctor_id, string.Concat(" ", string.Concat(binding, string.Concat(" ? ", string.Concat(ctor_body, rest)))))))))))(emit_match_branches(scrut, branches, ty, arities, mid, (i + 1L), has_multi, scrut_ref, (open_parens + 1L)))))(emit_ctor_pattern_body(subs, binding, b.body, ty, arities, mid))))((has_multi ? scrut_ref : emit_expr(scrut, arities, mid)))))(string.Concat("_m", string.Concat(ctor_id, string.Concat(((mid - 1L)).ToString(), "_"))))))(sanitize(name))))((string)_mIrCtorPat16_.Field0)))((List<IRPat>)_mIrCtorPat16_.Field1)))((CodexType)_mIrCtorPat16_.Field2) : (_scrutinee16_ is IrLitPat _mIrLitPat16_ ? ((Func<CodexType, string>)((lty) => ((Func<string, string>)((text) => ((Func<string, string>)((scrut_text) => ((Func<string, string>)((lit_cmp) => ((Func<string, string>)((rest) => string.Concat(sep, string.Concat("(", string.Concat(lit_cmp, string.Concat(" ? ", string.Concat(emit_expr(b.body, arities, mid), rest)))))))(emit_match_branches(scrut, branches, ty, arities, mid, (i + 1L), has_multi, scrut_ref, (open_parens + 1L)))))(emit_lit_compare(scrut_text, text, lty))))((has_multi ? scrut_ref : emit_expr(scrut, arities, mid)))))((string)_mIrLitPat16_.Field0)))((CodexType)_mIrLitPat16_.Field1) : (_scrutinee16_ is IrVarPat _mIrVarPat16_ ? ((Func<CodexType, string>)((vty) => ((Func<string, string>)((vname) => ((Func<string, string>)((var_func_type) => ((Func<string, string>)((scrut_text) => string.Concat(sep, string.Concat("((", string.Concat(var_func_type, string.Concat(")((", string.Concat(sanitize(vname), string.Concat(") => ", string.Concat(emit_expr(b.body, arities, mid), string.Concat("))(", string.Concat(scrut_text, string.Concat(")", repeat_close_parens(open_parens)))))))))))))((has_multi ? scrut_ref : emit_expr(scrut, arities, mid)))))(string.Concat("Func<", string.Concat(cs_type(vty), string.Concat(", ", string.Concat(cs_type(ir_expr_type(b.body)), ">")))))))((string)_mIrVarPat16_.Field0)))((CodexType)_mIrVarPat16_.Field1) : (_scrutinee16_ is IrWildPat _mIrWildPat16_ ? string.Concat(sep, string.Concat(emit_expr(b.body, arities, mid), repeat_close_parens(open_parens))) : throw new InvalidOperationException("Non-exhaustive match")))))))(b.pattern)))(((i > 0L) ? " : " : ""))))(branches[(int)i]));
    }

    public static string emit_lit_compare(string scrut_text, string lit_text, CodexType ty)
    {
        return ((Func<CodexType, string>)((_scrutinee17_) => (_scrutinee17_ is IntegerTy _mIntegerTy17_ ? string.Concat(scrut_text, string.Concat(" == ", string.Concat(lit_text, "L"))) : (_scrutinee17_ is TextTy _mTextTy17_ ? string.Concat(scrut_text, string.Concat(" == ", lit_text)) : (_scrutinee17_ is BooleanTy _mBooleanTy17_ ? string.Concat(scrut_text, string.Concat(".Equals(", string.Concat(lit_text, ")"))) : ((Func<CodexType, string>)((_) => string.Concat(scrut_text, string.Concat(".Equals(", string.Concat(lit_text, ")")))))(_scrutinee17_))))))(ty);
    }

    public static string repeat_close_parens(long n)
    {
        return ((n == 0L) ? "" : string.Concat(")", repeat_close_parens((n - 1L))));
    }

    public static string emit_ctor_pattern_body(List<IRPat> subs, string binding, IRExpr body, CodexType ty, List<ArityEntry> arities, long mid)
    {
        return ((Func<List<VarBinding>, string>)((bindings) => emit_var_bindings_and_body(bindings, body, arities, mid)))(collect_var_bindings(subs, binding, 0L));
    }

    public static List<VarBinding> collect_var_bindings(List<IRPat> subs, string binding, long i)
    {
        return ((i == ((long)subs.Count)) ? new List<VarBinding>() : ((Func<IRPat, List<VarBinding>>)((sub) => ((Func<List<VarBinding>, List<VarBinding>>)((rest) => (sub is IrVarPat _mIrVarPat18_ ? ((Func<CodexType, List<VarBinding>>)((ty) => ((Func<string, List<VarBinding>>)((name) => Enumerable.Concat(new List<VarBinding>() { new VarBinding(name, string.Concat(binding, string.Concat(".Field", (i).ToString())), ty) }, rest).ToList()))((string)_mIrVarPat18_.Field0)))((CodexType)_mIrVarPat18_.Field1) : ((Func<IRPat, List<VarBinding>>)((_) => rest))(sub))))(collect_var_bindings(subs, binding, (i + 1L)))))(subs[(int)i]));
    }

    public static string emit_var_bindings_and_body(List<VarBinding> bindings, IRExpr body, List<ArityEntry> arities, long mid)
    {
        return ((((long)bindings.Count) == 0L) ? emit_expr(body, arities, mid) : ((Func<string, string>)((open) => ((Func<string, string>)((body_text) => ((Func<string, string>)((close) => string.Concat(open, string.Concat(body_text, close))))(emit_var_binding_closes(bindings, 0L))))(emit_expr(body, arities, mid))))(emit_var_binding_opens(bindings, body, arities, mid, (((long)bindings.Count) - 1L))));
    }

    public static string emit_var_binding_opens(List<VarBinding> bindings, IRExpr body, List<ArityEntry> arities, long mid, long i)
    {
        return ((i < 0L) ? "" : ((Func<VarBinding, string>)((b) => ((Func<string, string>)((func_type) => string.Concat("((", string.Concat(func_type, string.Concat(")((", string.Concat(sanitize(b.var_name), string.Concat(") => ", emit_var_binding_opens(bindings, body, arities, mid, (i - 1L)))))))))(string.Concat("Func<", string.Concat(cs_type(b.var_type), string.Concat(", ", string.Concat(cs_type(ir_expr_type(body)), ">")))))))(bindings[(int)i]));
    }

    public static string emit_var_binding_closes(List<VarBinding> bindings, long i)
    {
        return ((i == ((long)bindings.Count)) ? "" : ((Func<VarBinding, string>)((b) => ((Func<string, string>)((cast) => string.Concat("))(", string.Concat(cast, string.Concat(b.access, string.Concat(")", emit_var_binding_closes(bindings, (i + 1L))))))))(emit_cast(b.var_type))))(bindings[(int)i]));
    }

    public static string emit_cast(CodexType ty)
    {
        return ((Func<CodexType, string>)((_scrutinee19_) => (_scrutinee19_ is ErrorTy _mErrorTy19_ ? "" : (_scrutinee19_ is TypeVar _mTypeVar19_ ? ((Func<long, string>)((id) => ""))((long)_mTypeVar19_.Field0) : (_scrutinee19_ is NothingTy _mNothingTy19_ ? "" : (_scrutinee19_ is VoidTy _mVoidTy19_ ? "" : ((Func<CodexType, string>)((_) => string.Concat("(", string.Concat(cs_type(ty), ")"))))(_scrutinee19_)))))))(ty);
    }

    public static string emit_do(List<IRDoStmt> stmts, List<ArityEntry> arities, long mid)
    {
        return string.Concat("((Func<object>)(() => {\n", string.Concat(emit_do_stmts(stmts, 0L, arities, mid), "        return null;\n    }))()"));
    }

    public static string emit_do_stmts(List<IRDoStmt> stmts, long i, List<ArityEntry> arities, long mid)
    {
        return ((i == ((long)stmts.Count)) ? "" : ((Func<IRDoStmt, string>)((s) => string.Concat(emit_do_stmt(s, arities, mid), emit_do_stmts(stmts, (i + 1L), arities, mid))))(stmts[(int)i]));
    }

    public static string emit_do_stmt(IRDoStmt s, List<ArityEntry> arities, long mid)
    {
        return ((Func<IRDoStmt, string>)((_scrutinee20_) => (_scrutinee20_ is IrDoBind _mIrDoBind20_ ? ((Func<IRExpr, string>)((val) => ((Func<CodexType, string>)((ty) => ((Func<string, string>)((name) => string.Concat("        var ", string.Concat(sanitize(name), string.Concat(" = ", string.Concat(emit_expr(val, arities, mid), ";\n"))))))((string)_mIrDoBind20_.Field0)))((CodexType)_mIrDoBind20_.Field1)))((IRExpr)_mIrDoBind20_.Field2) : (_scrutinee20_ is IrDoExec _mIrDoExec20_ ? ((Func<IRExpr, string>)((e) => string.Concat("        ", string.Concat(emit_expr(e, arities, mid), ";\n"))))((IRExpr)_mIrDoExec20_.Field0) : throw new InvalidOperationException("Non-exhaustive match")))))(s);
    }

    public static string emit_record(string name, List<IRFieldVal> fields, List<ArityEntry> arities, long mid)
    {
        return string.Concat("new ", string.Concat(sanitize(name), string.Concat("(", string.Concat(emit_record_fields(fields, 0L, arities, mid), ")"))));
    }

    public static string emit_record_fields(List<IRFieldVal> fields, long i, List<ArityEntry> arities, long mid)
    {
        return ((i == ((long)fields.Count)) ? "" : ((Func<IRFieldVal, string>)((f) => string.Concat(emit_expr(f.value, arities, mid), string.Concat(((i < (((long)fields.Count) - 1L)) ? ", " : ""), emit_record_fields(fields, (i + 1L), arities, mid)))))(fields[(int)i]));
    }

    public static string emit_type_defs(List<ATypeDef> tds, long i)
    {
        return ((i == ((long)tds.Count)) ? "" : string.Concat(emit_type_def(tds[(int)i]), string.Concat("\n", emit_type_defs(tds, (i + 1L)))));
    }

    public static string emit_type_def(ATypeDef td)
    {
        return ((Func<ATypeDef, string>)((_scrutinee21_) => (_scrutinee21_ is ARecordTypeDef _mARecordTypeDef21_ ? ((Func<List<ARecordFieldDef>, string>)((fields) => ((Func<List<Name>, string>)((tparams) => ((Func<Name, string>)((name) => ((Func<string, string>)((gen) => string.Concat("public sealed record ", string.Concat(sanitize(name.value), string.Concat(gen, string.Concat("(", string.Concat(emit_record_field_defs(fields, tparams, 0L), ");\n")))))))(emit_tparam_suffix(tparams))))((Name)_mARecordTypeDef21_.Field0)))((List<Name>)_mARecordTypeDef21_.Field1)))((List<ARecordFieldDef>)_mARecordTypeDef21_.Field2) : (_scrutinee21_ is AVariantTypeDef _mAVariantTypeDef21_ ? ((Func<List<AVariantCtorDef>, string>)((ctors) => ((Func<List<Name>, string>)((tparams) => ((Func<Name, string>)((name) => ((Func<string, string>)((gen) => string.Concat("public abstract record ", string.Concat(sanitize(name.value), string.Concat(gen, string.Concat(";\n", string.Concat(emit_variant_ctors(ctors, name, tparams, 0L), "\n")))))))(emit_tparam_suffix(tparams))))((Name)_mAVariantTypeDef21_.Field0)))((List<Name>)_mAVariantTypeDef21_.Field1)))((List<AVariantCtorDef>)_mAVariantTypeDef21_.Field2) : throw new InvalidOperationException("Non-exhaustive match")))))(td);
    }

    public static string emit_tparam_suffix(List<Name> tparams)
    {
        return ((((long)tparams.Count) == 0L) ? "" : string.Concat("<", string.Concat(emit_tparam_names(tparams, 0L), ">")));
    }

    public static string emit_tparam_names(List<Name> tparams, long i)
    {
        return ((i == ((long)tparams.Count)) ? "" : ((i == (((long)tparams.Count) - 1L)) ? string.Concat("T", (i).ToString()) : string.Concat("T", string.Concat((i).ToString(), string.Concat(", ", emit_tparam_names(tparams, (i + 1L)))))));
    }

    public static string emit_record_field_defs(List<ARecordFieldDef> fields, List<Name> tparams, long i)
    {
        return ((i == ((long)fields.Count)) ? "" : ((Func<ARecordFieldDef, string>)((f) => string.Concat(emit_type_expr_tp(f.type_expr, tparams), string.Concat(" ", string.Concat(sanitize(f.name.value), string.Concat(((i < (((long)fields.Count) - 1L)) ? ", " : ""), emit_record_field_defs(fields, tparams, (i + 1L))))))))(fields[(int)i]));
    }

    public static string emit_variant_ctors(List<AVariantCtorDef> ctors, Name base_name, List<Name> tparams, long i)
    {
        return ((i == ((long)ctors.Count)) ? "" : ((Func<AVariantCtorDef, string>)((c) => string.Concat(emit_variant_ctor(c, base_name, tparams), emit_variant_ctors(ctors, base_name, tparams, (i + 1L)))))(ctors[(int)i]));
    }

    public static string emit_variant_ctor(AVariantCtorDef c, Name base_name, List<Name> tparams)
    {
        return ((Func<string, string>)((gen) => ((((long)c.fields.Count) == 0L) ? string.Concat("public sealed record ", string.Concat(sanitize(c.name.value), string.Concat(gen, string.Concat(" : ", string.Concat(sanitize(base_name.value), string.Concat(gen, ";\n")))))) : string.Concat("public sealed record ", string.Concat(sanitize(c.name.value), string.Concat(gen, string.Concat("(", string.Concat(emit_ctor_fields(c.fields, tparams, 0L), string.Concat(") : ", string.Concat(sanitize(base_name.value), string.Concat(gen, ";\n")))))))))))(emit_tparam_suffix(tparams));
    }

    public static string emit_ctor_fields(List<ATypeExpr> fields, List<Name> tparams, long i)
    {
        return ((i == ((long)fields.Count)) ? "" : string.Concat(emit_type_expr_tp(fields[(int)i], tparams), string.Concat(" Field", string.Concat((i).ToString(), string.Concat(((i < (((long)fields.Count) - 1L)) ? ", " : ""), emit_ctor_fields(fields, tparams, (i + 1L)))))));
    }

    public static string emit_type_expr(ATypeExpr te)
    {
        return emit_type_expr_tp(te, new List<Name>());
    }

    public static string emit_type_expr_tp(ATypeExpr te, List<Name> tparams)
    {
        return ((Func<ATypeExpr, string>)((_scrutinee22_) => (_scrutinee22_ is ANamedType _mANamedType22_ ? ((Func<Name, string>)((name) => ((Func<long, string>)((idx) => ((idx >= 0L) ? string.Concat("T", (idx).ToString()) : when_type_name(name.value))))(find_tparam_index(tparams, name.value, 0L))))((Name)_mANamedType22_.Field0) : (_scrutinee22_ is AFunType _mAFunType22_ ? ((Func<ATypeExpr, string>)((r) => ((Func<ATypeExpr, string>)((p) => string.Concat("Func<", string.Concat(emit_type_expr_tp(p, tparams), string.Concat(", ", string.Concat(emit_type_expr_tp(r, tparams), ">"))))))((ATypeExpr)_mAFunType22_.Field0)))((ATypeExpr)_mAFunType22_.Field1) : (_scrutinee22_ is AAppType _mAAppType22_ ? ((Func<List<ATypeExpr>, string>)((args) => ((Func<ATypeExpr, string>)((@base) => string.Concat(emit_type_expr_tp(@base, tparams), string.Concat("<", string.Concat(emit_type_expr_list_tp(args, tparams, 0L), ">")))))((ATypeExpr)_mAAppType22_.Field0)))((List<ATypeExpr>)_mAAppType22_.Field1) : throw new InvalidOperationException("Non-exhaustive match"))))))(te);
    }

    public static long find_tparam_index(List<Name> tparams, string name, long i)
    {
        while (true)
        {
            if ((i == ((long)tparams.Count)))
            {
                return (0L - 1L);
            }
            else
            {
                if ((tparams[(int)i].value == name))
                {
                    return i;
                }
                else
                {
                    var _tco_0 = tparams;
                    var _tco_1 = name;
                    var _tco_2 = (i + 1L);
                    tparams = _tco_0;
                    name = _tco_1;
                    i = _tco_2;
                    continue;
                }
            }
        }
    }

    public static string when_type_name(string n)
    {
        return ((n == "Integer") ? "long" : ((n == "Number") ? "decimal" : ((n == "Text") ? "string" : ((n == "Boolean") ? "bool" : ((n == "List") ? "List" : sanitize(n))))));
    }

    public static string emit_type_expr_list(List<ATypeExpr> args, long i)
    {
        return ((i == ((long)args.Count)) ? "" : string.Concat(emit_type_expr(args[(int)i]), string.Concat(((i < (((long)args.Count) - 1L)) ? ", " : ""), emit_type_expr_list(args, (i + 1L)))));
    }

    public static string emit_type_expr_list_tp(List<ATypeExpr> args, List<Name> tparams, long i)
    {
        return ((i == ((long)args.Count)) ? "" : string.Concat(emit_type_expr_tp(args[(int)i], tparams), string.Concat(((i < (((long)args.Count) - 1L)) ? ", " : ""), emit_type_expr_list_tp(args, tparams, (i + 1L)))));
    }

    public static List<long> collect_type_var_ids(CodexType ty, List<long> acc)
    {
        while (true)
        {
            var _tco_s = ty;
            if (_tco_s is TypeVar _tco_m0)
            {
                var id = _tco_m0.Field0;
                if (list_contains_int(acc, id))
                {
                    return acc;
                }
                else
                {
                    return list_append_int(acc, id);
                }
            }
            else if (_tco_s is FunTy _tco_m1)
            {
                var p = _tco_m1.Field0;
                var r = _tco_m1.Field1;
                var _tco_0 = r;
                var _tco_1 = collect_type_var_ids(p, acc);
                ty = _tco_0;
                acc = _tco_1;
                continue;
            }
            else if (_tco_s is ListTy _tco_m2)
            {
                var elem = _tco_m2.Field0;
                var _tco_0 = elem;
                var _tco_1 = acc;
                ty = _tco_0;
                acc = _tco_1;
                continue;
            }
            else if (_tco_s is ForAllTy _tco_m3)
            {
                var id = _tco_m3.Field0;
                var body = _tco_m3.Field1;
                var _tco_0 = body;
                var _tco_1 = acc;
                ty = _tco_0;
                acc = _tco_1;
                continue;
            }
            else if (_tco_s is ConstructedTy _tco_m4)
            {
                var name = _tco_m4.Field0;
                var args = _tco_m4.Field1;
                return collect_type_var_ids_list(args, acc);
            }
            {
                var _ = _tco_s;
                return acc;
            }
        }
    }

    public static List<long> collect_type_var_ids_list(List<CodexType> types, List<long> acc)
    {
        return collect_type_var_ids_list_loop(types, acc, 0L, ((long)types.Count));
    }

    public static List<long> collect_type_var_ids_list_loop(List<CodexType> types, List<long> acc, long i, long len)
    {
        while (true)
        {
            if ((i == len))
            {
                return acc;
            }
            else
            {
                var _tco_0 = types;
                var _tco_1 = collect_type_var_ids(types[(int)i], acc);
                var _tco_2 = (i + 1L);
                var _tco_3 = len;
                types = _tco_0;
                acc = _tco_1;
                i = _tco_2;
                len = _tco_3;
                continue;
            }
        }
    }

    public static bool list_contains_int(List<long> xs, long n)
    {
        return list_contains_int_loop(xs, n, 0L, ((long)xs.Count));
    }

    public static bool list_contains_int_loop(List<long> xs, long n, long i, long len)
    {
        while (true)
        {
            if ((i == len))
            {
                return false;
            }
            else
            {
                if ((xs[(int)i] == n))
                {
                    return true;
                }
                else
                {
                    var _tco_0 = xs;
                    var _tco_1 = n;
                    var _tco_2 = (i + 1L);
                    var _tco_3 = len;
                    xs = _tco_0;
                    n = _tco_1;
                    i = _tco_2;
                    len = _tco_3;
                    continue;
                }
            }
        }
    }

    public static List<long> list_append_int(List<long> xs, long n)
    {
        return Enumerable.Concat(xs, new List<long>() { n }).ToList();
    }

    public static string generic_suffix(CodexType ty)
    {
        return ((Func<List<long>, string>)((ids) => ((((long)ids.Count) == 0L) ? "" : string.Concat("<", string.Concat(emit_type_params(ids, 0L), ">")))))(collect_type_var_ids(ty, new List<long>()));
    }

    public static string emit_type_params(List<long> ids, long i)
    {
        return ((i == ((long)ids.Count)) ? "" : ((i == (((long)ids.Count) - 1L)) ? string.Concat("T", (ids[(int)i]).ToString()) : string.Concat("T", string.Concat((ids[(int)i]).ToString(), string.Concat(", ", emit_type_params(ids, (i + 1L)))))));
    }

    public static bool has_self_tail_call(IRDef d)
    {
        return ((((long)d.@params.Count) == 0L) ? false : expr_has_tail_call(d.body, d.name));
    }

    public static bool expr_has_tail_call(IRExpr e, string name)
    {
        while (true)
        {
            var _tco_s = e;
            if (_tco_s is IrIf _tco_m0)
            {
                var c = _tco_m0.Field0;
                var t = _tco_m0.Field1;
                var el = _tco_m0.Field2;
                var ty = _tco_m0.Field3;
                return (expr_has_tail_call(t, name) || expr_has_tail_call(el, name));
            }
            else if (_tco_s is IrLet _tco_m1)
            {
                var n = _tco_m1.Field0;
                var ty = _tco_m1.Field1;
                var v = _tco_m1.Field2;
                var body = _tco_m1.Field3;
                var _tco_0 = body;
                var _tco_1 = name;
                e = _tco_0;
                name = _tco_1;
                continue;
            }
            else if (_tco_s is IrMatch _tco_m2)
            {
                var scrut = _tco_m2.Field0;
                var branches = _tco_m2.Field1;
                var ty = _tco_m2.Field2;
                return branches_have_tail_call(branches, name, 0L);
            }
            else if (_tco_s is IrApply _tco_m3)
            {
                var f = _tco_m3.Field0;
                var a = _tco_m3.Field1;
                var ty = _tco_m3.Field2;
                return is_self_call(f, name);
            }
            {
                var _ = _tco_s;
                return false;
            }
        }
    }

    public static bool branches_have_tail_call(List<IRBranch> branches, string name, long i)
    {
        return ((i == ((long)branches.Count)) ? false : (expr_has_tail_call(branches[(int)i].body, name) || branches_have_tail_call(branches, name, (i + 1L))));
    }

    public static bool is_self_call(IRExpr e, string name)
    {
        while (true)
        {
            var _tco_s = e;
            if (_tco_s is IrApply _tco_m0)
            {
                var f = _tco_m0.Field0;
                var a = _tco_m0.Field1;
                var ty = _tco_m0.Field2;
                var _tco_0 = f;
                var _tco_1 = name;
                e = _tco_0;
                name = _tco_1;
                continue;
            }
            else if (_tco_s is IrName _tco_m1)
            {
                var n = _tco_m1.Field0;
                var ty = _tco_m1.Field1;
                return (n == name);
            }
            {
                var _ = _tco_s;
                return false;
            }
        }
    }

    public static string emit_tco_def(IRDef d, List<ArityEntry> arities)
    {
        return ((Func<CodexType, string>)((ret) => ((Func<string, string>)((gen) => string.Concat("    public static ", string.Concat(cs_type(ret), string.Concat(" ", string.Concat(sanitize(d.name), string.Concat(gen, string.Concat("(", string.Concat(emit_def_params(d.@params, 0L), string.Concat(")\n    {\n        while (true)\n        {\n", string.Concat(emit_tco_body(d.body, d.name, d.@params, arities, 3L, 0L), "        }\n    }\n")))))))))))(generic_suffix(d.type_val))))(get_return_type(d.type_val, ((long)d.@params.Count)));
    }

    public static string make_pad(long indent)
    {
        return ((indent == 0L) ? "" : string.Concat("    ", make_pad((indent - 1L))));
    }

    public static string emit_tco_body(IRExpr e, string func_name, List<IRParam> @params, List<ArityEntry> arities, long indent, long mid)
    {
        return ((Func<string, string>)((pad) => ((Func<IRExpr, string>)((_scrutinee23_) => (_scrutinee23_ is IrIf _mIrIf23_ ? ((Func<CodexType, string>)((ty) => ((Func<IRExpr, string>)((el) => ((Func<IRExpr, string>)((t) => ((Func<IRExpr, string>)((c) => string.Concat(pad, string.Concat("if (", string.Concat(emit_expr(c, arities, mid), string.Concat(")\n", string.Concat(pad, string.Concat("{\n", string.Concat(emit_tco_body(t, func_name, @params, arities, (indent + 1L), mid), string.Concat(pad, string.Concat("}\n", string.Concat(pad, string.Concat("else\n", string.Concat(pad, string.Concat("{\n", string.Concat(emit_tco_body(el, func_name, @params, arities, (indent + 1L), mid), string.Concat(pad, "}\n")))))))))))))))))((IRExpr)_mIrIf23_.Field0)))((IRExpr)_mIrIf23_.Field1)))((IRExpr)_mIrIf23_.Field2)))((CodexType)_mIrIf23_.Field3) : (_scrutinee23_ is IrLet _mIrLet23_ ? ((Func<IRExpr, string>)((body) => ((Func<IRExpr, string>)((val) => ((Func<CodexType, string>)((ty) => ((Func<string, string>)((name) => string.Concat(pad, string.Concat("var ", string.Concat(sanitize(name), string.Concat(" = ", string.Concat(emit_expr(val, arities, mid), string.Concat(";\n", emit_tco_body(body, func_name, @params, arities, indent, mid)))))))))((string)_mIrLet23_.Field0)))((CodexType)_mIrLet23_.Field1)))((IRExpr)_mIrLet23_.Field2)))((IRExpr)_mIrLet23_.Field3) : (_scrutinee23_ is IrMatch _mIrMatch23_ ? ((Func<CodexType, string>)((ty) => ((Func<List<IRBranch>, string>)((branches) => ((Func<IRExpr, string>)((scrut) => emit_tco_match(scrut, branches, func_name, @params, arities, indent, mid)))((IRExpr)_mIrMatch23_.Field0)))((List<IRBranch>)_mIrMatch23_.Field1)))((CodexType)_mIrMatch23_.Field2) : (_scrutinee23_ is IrApply _mIrApply23_ ? ((Func<CodexType, string>)((ty) => ((Func<IRExpr, string>)((a) => ((Func<IRExpr, string>)((f) => (is_self_call(f, func_name) ? emit_tco_jump(f, a, @params, arities, indent, mid) : string.Concat(pad, string.Concat("return ", string.Concat(emit_expr(e, arities, mid), ";\n"))))))((IRExpr)_mIrApply23_.Field0)))((IRExpr)_mIrApply23_.Field1)))((CodexType)_mIrApply23_.Field2) : ((Func<IRExpr, string>)((_) => string.Concat(pad, string.Concat("return ", string.Concat(emit_expr(e, arities, mid), ";\n")))))(_scrutinee23_)))))))(e)))(make_pad(indent));
    }

    public static string emit_tco_match(IRExpr scrut, List<IRBranch> branches, string func_name, List<IRParam> @params, List<ArityEntry> arities, long indent, long mid)
    {
        return ((Func<string, string>)((pad) => string.Concat(pad, string.Concat("var _tco_s = ", string.Concat(emit_expr(scrut, arities, mid), string.Concat(";\n", emit_tco_match_branches(branches, func_name, @params, arities, indent, mid, 0L, true)))))))(make_pad(indent));
    }

    public static string emit_tco_match_branches(List<IRBranch> branches, string func_name, List<IRParam> @params, List<ArityEntry> arities, long indent, long mid, long i, bool first)
    {
        return ((i == ((long)branches.Count)) ? "" : ((Func<IRBranch, string>)((b) => ((Func<string, string>)((pad) => ((Func<string, string>)((rest) => ((Func<IRPat, string>)((_scrutinee24_) => (_scrutinee24_ is IrCtorPat _mIrCtorPat24_ ? ((Func<CodexType, string>)((cty) => ((Func<List<IRPat>, string>)((subs) => ((Func<string, string>)((name) => ((Func<string, string>)((keyword) => ((Func<string, string>)((match_var) => string.Concat(pad, string.Concat(keyword, string.Concat(" (_tco_s is ", string.Concat(sanitize(name), string.Concat(" ", string.Concat(match_var, string.Concat(")\n", string.Concat(pad, string.Concat("{\n", string.Concat(emit_tco_ctor_bindings(subs, match_var, (indent + 1L), 0L), string.Concat(emit_tco_body(b.body, func_name, @params, arities, (indent + 1L), mid), string.Concat(pad, string.Concat("}\n", rest)))))))))))))))(string.Concat("_tco_m", (i).ToString()))))((first ? "if" : "else if"))))((string)_mIrCtorPat24_.Field0)))((List<IRPat>)_mIrCtorPat24_.Field1)))((CodexType)_mIrCtorPat24_.Field2) : (_scrutinee24_ is IrLitPat _mIrLitPat24_ ? ((Func<CodexType, string>)((lty) => ((Func<string, string>)((text) => ((Func<string, string>)((keyword) => ((Func<string, string>)((lit_val) => string.Concat(pad, string.Concat(keyword, string.Concat(" (object.Equals(_tco_s, ", string.Concat(lit_val, string.Concat("))\n", string.Concat(pad, string.Concat("{\n", string.Concat(emit_tco_body(b.body, func_name, @params, arities, (indent + 1L), mid), string.Concat(pad, string.Concat("}\n", rest))))))))))))(emit_tco_lit_val(text, lty))))((first ? "if" : "else if"))))((string)_mIrLitPat24_.Field0)))((CodexType)_mIrLitPat24_.Field1) : (_scrutinee24_ is IrVarPat _mIrVarPat24_ ? ((Func<CodexType, string>)((vty) => ((Func<string, string>)((vname) => string.Concat(pad, string.Concat("{\n", string.Concat(pad, string.Concat("    var ", string.Concat(sanitize(vname), string.Concat(" = _tco_s;\n", string.Concat(emit_tco_body(b.body, func_name, @params, arities, (indent + 1L), mid), string.Concat(pad, string.Concat("}\n", rest)))))))))))((string)_mIrVarPat24_.Field0)))((CodexType)_mIrVarPat24_.Field1) : (_scrutinee24_ is IrWildPat _mIrWildPat24_ ? string.Concat(pad, string.Concat("{\n", string.Concat(emit_tco_body(b.body, func_name, @params, arities, (indent + 1L), mid), string.Concat(pad, string.Concat("}\n", rest))))) : throw new InvalidOperationException("Non-exhaustive match")))))))(b.pattern)))(emit_tco_match_branches(branches, func_name, @params, arities, indent, mid, (i + 1L), false))))(make_pad(indent))))(branches[(int)i]));
    }

    public static string emit_tco_lit_val(string text, CodexType ty)
    {
        return ((Func<CodexType, string>)((_scrutinee25_) => (_scrutinee25_ is IntegerTy _mIntegerTy25_ ? string.Concat(text, "L") : (_scrutinee25_ is BooleanTy _mBooleanTy25_ ? text : (_scrutinee25_ is TextTy _mTextTy25_ ? text : ((Func<CodexType, string>)((_) => text))(_scrutinee25_))))))(ty);
    }

    public static string emit_tco_ctor_bindings(List<IRPat> subs, string match_var, long indent, long i)
    {
        return ((i == ((long)subs.Count)) ? "" : ((Func<IRPat, string>)((sub) => ((Func<string, string>)((pad) => ((Func<string, string>)((rest) => (sub is IrVarPat _mIrVarPat26_ ? ((Func<CodexType, string>)((ty) => ((Func<string, string>)((name) => string.Concat(pad, string.Concat("var ", string.Concat(sanitize(name), string.Concat(" = ", string.Concat(match_var, string.Concat(".Field", string.Concat((i).ToString(), string.Concat(";\n", rest))))))))))((string)_mIrVarPat26_.Field0)))((CodexType)_mIrVarPat26_.Field1) : ((Func<IRPat, string>)((_) => rest))(sub))))(emit_tco_ctor_bindings(subs, match_var, indent, (i + 1L)))))(make_pad(indent))))(subs[(int)i]));
    }

    public static string emit_tco_jump(IRExpr f, IRExpr a, List<IRParam> @params, List<ArityEntry> arities, long indent, long mid)
    {
        return ((Func<List<IRExpr>, string>)((args) => ((Func<string, string>)((pad) => string.Concat(emit_tco_temp_assigns(args, arities, indent, mid, 0L), string.Concat(emit_tco_param_assigns(@params, indent, 0L, ((long)args.Count)), string.Concat(pad, "continue;\n")))))(make_pad(indent))))(collect_apply_args(f, new List<IRExpr>() { a }));
    }

    public static string emit_tco_temp_assigns(List<IRExpr> args, List<ArityEntry> arities, long indent, long mid, long i)
    {
        return ((i == ((long)args.Count)) ? "" : ((Func<string, string>)((pad) => string.Concat(pad, string.Concat("var _tco_", string.Concat((i).ToString(), string.Concat(" = ", string.Concat(emit_expr(args[(int)i], arities, mid), string.Concat(";\n", emit_tco_temp_assigns(args, arities, indent, mid, (i + 1L))))))))))(make_pad(indent)));
    }

    public static string emit_tco_param_assigns(List<IRParam> @params, long indent, long i, long count)
    {
        return ((i == count) ? "" : ((i == ((long)@params.Count)) ? "" : ((Func<string, string>)((pad) => ((Func<IRParam, string>)((p) => string.Concat(pad, string.Concat(sanitize(p.name), string.Concat(" = _tco_", string.Concat((i).ToString(), string.Concat(";\n", emit_tco_param_assigns(@params, indent, (i + 1L), count))))))))(@params[(int)i])))(make_pad(indent))));
    }

    public static bool is_effectful_def(IRDef d)
    {
        return is_effectful_type(d.type_val, ((long)d.@params.Count));
    }

    public static bool is_effectful_type(CodexType ty, long n)
    {
        while (true)
        {
            if ((n == 0L))
            {
                return is_do_type(ty);
            }
            else
            {
                var _tco_s = ty;
                if (_tco_s is FunTy _tco_m0)
                {
                    var p = _tco_m0.Field0;
                    var r = _tco_m0.Field1;
                    var _tco_0 = r;
                    var _tco_1 = (n - 1L);
                    ty = _tco_0;
                    n = _tco_1;
                    continue;
                }
                else if (_tco_s is ForAllTy _tco_m1)
                {
                    var id = _tco_m1.Field0;
                    var body = _tco_m1.Field1;
                    var _tco_0 = body;
                    var _tco_1 = n;
                    ty = _tco_0;
                    n = _tco_1;
                    continue;
                }
                {
                    var _ = _tco_s;
                    return false;
                }
            }
        }
    }

    public static bool is_do_type(CodexType ty)
    {
        return (ty is VoidTy _mVoidTy27_ ? true : ((Func<CodexType, bool>)((_) => false))(ty));
    }

    public static string emit_def(IRDef d, List<ArityEntry> arities)
    {
        return (has_self_tail_call(d) ? emit_tco_def(d, arities) : ((Func<CodexType, string>)((ret) => ((Func<string, string>)((gen) => (is_effectful_def(d) ? string.Concat("    public static ", string.Concat(cs_type(ret), string.Concat(" ", string.Concat(sanitize(d.name), string.Concat(gen, string.Concat("(", string.Concat(emit_def_params(d.@params, 0L), string.Concat(")\n    {\n        ", string.Concat(emit_expr(d.body, arities, 0L), ";\n        return null;\n    }\n"))))))))) : string.Concat("    public static ", string.Concat(cs_type(ret), string.Concat(" ", string.Concat(sanitize(d.name), string.Concat(gen, string.Concat("(", string.Concat(emit_def_params(d.@params, 0L), string.Concat(")\n    {\n        return ", string.Concat(emit_expr(d.body, arities, 0L), ";\n    }\n"))))))))))))(generic_suffix(d.type_val))))(get_return_type(d.type_val, ((long)d.@params.Count))));
    }

    public static CodexType get_return_type(CodexType ty, long n)
    {
        while (true)
        {
            if ((n == 0L))
            {
                return strip_forall(ty);
            }
            else
            {
                var _tco_s = strip_forall(ty);
                if (_tco_s is FunTy _tco_m0)
                {
                    var p = _tco_m0.Field0;
                    var r = _tco_m0.Field1;
                    var _tco_0 = r;
                    var _tco_1 = (n - 1L);
                    ty = _tco_0;
                    n = _tco_1;
                    continue;
                }
                {
                    var _ = _tco_s;
                    return ty;
                }
            }
        }
    }

    public static CodexType strip_forall(CodexType ty)
    {
        while (true)
        {
            var _tco_s = ty;
            if (_tco_s is ForAllTy _tco_m0)
            {
                var id = _tco_m0.Field0;
                var body = _tco_m0.Field1;
                var _tco_0 = body;
                ty = _tco_0;
                continue;
            }
            {
                var _ = _tco_s;
                return ty;
            }
        }
    }

    public static string emit_def_params(List<IRParam> @params, long i)
    {
        return ((i == ((long)@params.Count)) ? "" : ((Func<IRParam, string>)((p) => string.Concat(cs_type(p.type_val), string.Concat(" ", string.Concat(sanitize(p.name), string.Concat(((i < (((long)@params.Count) - 1L)) ? ", " : ""), emit_def_params(@params, (i + 1L))))))))(@params[(int)i]));
    }

    public static IRDef find_main_def(List<IRDef> defs, long i)
    {
        while (true)
        {
            if ((i == ((long)defs.Count)))
            {
                return new IRDef("", new List<IRParam>(), new VoidTy(), new IrBoolLit(false));
            }
            else
            {
                var d = defs[(int)i];
                if ((d.name == "main"))
                {
                    return d;
                }
                else
                {
                    var _tco_0 = defs;
                    var _tco_1 = (i + 1L);
                    defs = _tco_0;
                    i = _tco_1;
                    continue;
                }
            }
        }
    }

    public static string emit_main_call(List<IRDef> defs, string class_name)
    {
        return ((Func<IRDef, string>)((main_def) => (((main_def.name == "main") && (((long)main_def.@params.Count) == 0L)) ? (is_effectful_def(main_def) ? string.Concat(class_name, ".main();\n\n") : string.Concat("Console.WriteLine(", string.Concat(class_name, ".main());\n\n"))) : "")))(find_main_def(defs, 0L));
    }

    public static string emit_full_module(IRModule m, List<ATypeDef> type_defs)
    {
        return ((Func<List<ArityEntry>, string>)((arities) => ((Func<string, string>)((class_name) => string.Concat("using System;\nusing System.Collections.Generic;\nusing System.Linq;\n\n", string.Concat(emit_main_call(m.defs, class_name), string.Concat(emit_type_defs(type_defs, 0L), string.Concat("public static class ", string.Concat(class_name, string.Concat("\n{\n", string.Concat(emit_defs(m.defs, 0L, arities), "}\n")))))))))(string.Concat("Codex_", sanitize(m.name.value)))))(build_arities(m.defs, 0L));
    }

    public static string emit_module(IRModule m)
    {
        return ((Func<List<ArityEntry>, string>)((arities) => ((Func<string, string>)((class_name) => string.Concat("using System;\nusing System.Collections.Generic;\nusing System.Linq;\n\n", string.Concat(emit_main_call(m.defs, class_name), string.Concat("public static class ", string.Concat(class_name, string.Concat("\n{\n", string.Concat(emit_defs(m.defs, 0L, arities), "}\n"))))))))(string.Concat("Codex_", sanitize(m.name.value)))))(build_arities(m.defs, 0L));
    }

    public static string emit_defs(List<IRDef> defs, long i, List<ArityEntry> arities)
    {
        return ((i == ((long)defs.Count)) ? "" : string.Concat(emit_def(defs[(int)i], arities), string.Concat("\n", emit_defs(defs, (i + 1L), arities))));
    }

    public static CodexType lookup_type(List<TypeBinding> bindings, string name)
    {
        return lookup_type_loop(bindings, name, 0L, ((long)bindings.Count));
    }

    public static CodexType lookup_type_loop(List<TypeBinding> bindings, string name, long i, long len)
    {
        while (true)
        {
            if ((i == len))
            {
                return new ErrorTy();
            }
            else
            {
                var b = bindings[(int)i];
                if ((b.name == name))
                {
                    return b.bound_type;
                }
                else
                {
                    var _tco_0 = bindings;
                    var _tco_1 = name;
                    var _tco_2 = (i + 1L);
                    var _tco_3 = len;
                    bindings = _tco_0;
                    name = _tco_1;
                    i = _tco_2;
                    len = _tco_3;
                    continue;
                }
            }
        }
    }

    public static CodexType peel_fun_param(CodexType ty)
    {
        while (true)
        {
            var _tco_s = ty;
            if (_tco_s is FunTy _tco_m0)
            {
                var p = _tco_m0.Field0;
                var r = _tco_m0.Field1;
                return p;
            }
            else if (_tco_s is ForAllTy _tco_m1)
            {
                var id = _tco_m1.Field0;
                var body = _tco_m1.Field1;
                var _tco_0 = body;
                ty = _tco_0;
                continue;
            }
            {
                var _ = _tco_s;
                return new ErrorTy();
            }
        }
    }

    public static CodexType peel_fun_return(CodexType ty)
    {
        while (true)
        {
            var _tco_s = ty;
            if (_tco_s is FunTy _tco_m0)
            {
                var p = _tco_m0.Field0;
                var r = _tco_m0.Field1;
                return r;
            }
            else if (_tco_s is ForAllTy _tco_m1)
            {
                var id = _tco_m1.Field0;
                var body = _tco_m1.Field1;
                var _tco_0 = body;
                ty = _tco_0;
                continue;
            }
            {
                var _ = _tco_s;
                return new ErrorTy();
            }
        }
    }

    public static CodexType strip_forall_ty(CodexType ty)
    {
        while (true)
        {
            var _tco_s = ty;
            if (_tco_s is ForAllTy _tco_m0)
            {
                var id = _tco_m0.Field0;
                var body = _tco_m0.Field1;
                var _tco_0 = body;
                ty = _tco_0;
                continue;
            }
            {
                var _ = _tco_s;
                return ty;
            }
        }
    }

    public static IRBinaryOp lower_bin_op(BinaryOp op, CodexType ty)
    {
        return ((Func<BinaryOp, IRBinaryOp>)((_scrutinee28_) => (_scrutinee28_ is OpAdd _mOpAdd28_ ? new IrAddInt() : (_scrutinee28_ is OpSub _mOpSub28_ ? new IrSubInt() : (_scrutinee28_ is OpMul _mOpMul28_ ? new IrMulInt() : (_scrutinee28_ is OpDiv _mOpDiv28_ ? new IrDivInt() : (_scrutinee28_ is OpPow _mOpPow28_ ? new IrPowInt() : (_scrutinee28_ is OpEq _mOpEq28_ ? new IrEq() : (_scrutinee28_ is OpNotEq _mOpNotEq28_ ? new IrNotEq() : (_scrutinee28_ is OpLt _mOpLt28_ ? new IrLt() : (_scrutinee28_ is OpGt _mOpGt28_ ? new IrGt() : (_scrutinee28_ is OpLtEq _mOpLtEq28_ ? new IrLtEq() : (_scrutinee28_ is OpGtEq _mOpGtEq28_ ? new IrGtEq() : (_scrutinee28_ is OpDefEq _mOpDefEq28_ ? new IrEq() : (_scrutinee28_ is OpAppend _mOpAppend28_ ? (is_text_type(ty) ? new IrAppendText() : new IrAppendList()) : (_scrutinee28_ is OpCons _mOpCons28_ ? new IrConsList() : (_scrutinee28_ is OpAnd _mOpAnd28_ ? new IrAnd() : (_scrutinee28_ is OpOr _mOpOr28_ ? new IrOr() : throw new InvalidOperationException("Non-exhaustive match")))))))))))))))))))(op);
    }

    public static bool is_text_type(CodexType ty)
    {
        return (ty is TextTy _mTextTy29_ ? true : ((Func<CodexType, bool>)((_) => false))(ty));
    }

    public static IRExpr lower_expr(AExpr e, CodexType ty)
    {
        return ((Func<AExpr, IRExpr>)((_scrutinee30_) => (_scrutinee30_ is ALitExpr _mALitExpr30_ ? ((Func<LiteralKind, IRExpr>)((kind) => ((Func<string, IRExpr>)((text) => lower_literal(text, kind)))((string)_mALitExpr30_.Field0)))((LiteralKind)_mALitExpr30_.Field1) : (_scrutinee30_ is ANameExpr _mANameExpr30_ ? ((Func<Name, IRExpr>)((name) => new IrName(name.value, ty)))((Name)_mANameExpr30_.Field0) : (_scrutinee30_ is AApplyExpr _mAApplyExpr30_ ? ((Func<AExpr, IRExpr>)((a) => ((Func<AExpr, IRExpr>)((f) => lower_apply(f, a, ty)))((AExpr)_mAApplyExpr30_.Field0)))((AExpr)_mAApplyExpr30_.Field1) : (_scrutinee30_ is ABinaryExpr _mABinaryExpr30_ ? ((Func<AExpr, IRExpr>)((r) => ((Func<BinaryOp, IRExpr>)((op) => ((Func<AExpr, IRExpr>)((l) => new IrBinary(lower_bin_op(op, ty), lower_expr(l, ty), lower_expr(r, ty), ty)))((AExpr)_mABinaryExpr30_.Field0)))((BinaryOp)_mABinaryExpr30_.Field1)))((AExpr)_mABinaryExpr30_.Field2) : (_scrutinee30_ is AUnaryExpr _mAUnaryExpr30_ ? ((Func<AExpr, IRExpr>)((operand) => new IrNegate(lower_expr(operand, new IntegerTy()))))((AExpr)_mAUnaryExpr30_.Field0) : (_scrutinee30_ is AIfExpr _mAIfExpr30_ ? ((Func<AExpr, IRExpr>)((e2) => ((Func<AExpr, IRExpr>)((t) => ((Func<AExpr, IRExpr>)((c) => new IrIf(lower_expr(c, new BooleanTy()), lower_expr(t, ty), lower_expr(e2, ty), ty)))((AExpr)_mAIfExpr30_.Field0)))((AExpr)_mAIfExpr30_.Field1)))((AExpr)_mAIfExpr30_.Field2) : (_scrutinee30_ is ALetExpr _mALetExpr30_ ? ((Func<AExpr, IRExpr>)((body) => ((Func<List<ALetBind>, IRExpr>)((binds) => lower_let(binds, body, ty)))((List<ALetBind>)_mALetExpr30_.Field0)))((AExpr)_mALetExpr30_.Field1) : (_scrutinee30_ is ALambdaExpr _mALambdaExpr30_ ? ((Func<AExpr, IRExpr>)((body) => ((Func<List<Name>, IRExpr>)((@params) => lower_lambda(@params, body, ty)))((List<Name>)_mALambdaExpr30_.Field0)))((AExpr)_mALambdaExpr30_.Field1) : (_scrutinee30_ is AMatchExpr _mAMatchExpr30_ ? ((Func<List<AMatchArm>, IRExpr>)((arms) => ((Func<AExpr, IRExpr>)((scrut) => lower_match(scrut, arms, ty)))((AExpr)_mAMatchExpr30_.Field0)))((List<AMatchArm>)_mAMatchExpr30_.Field1) : (_scrutinee30_ is AListExpr _mAListExpr30_ ? ((Func<List<AExpr>, IRExpr>)((elems) => lower_list(elems, ty)))((List<AExpr>)_mAListExpr30_.Field0) : (_scrutinee30_ is ARecordExpr _mARecordExpr30_ ? ((Func<List<AFieldExpr>, IRExpr>)((fields) => ((Func<Name, IRExpr>)((name) => lower_record(name, fields, ty)))((Name)_mARecordExpr30_.Field0)))((List<AFieldExpr>)_mARecordExpr30_.Field1) : (_scrutinee30_ is AFieldAccess _mAFieldAccess30_ ? ((Func<Name, IRExpr>)((field) => ((Func<AExpr, IRExpr>)((rec) => new IrFieldAccess(lower_expr(rec, ty), field.value, ty)))((AExpr)_mAFieldAccess30_.Field0)))((Name)_mAFieldAccess30_.Field1) : (_scrutinee30_ is ADoExpr _mADoExpr30_ ? ((Func<List<ADoStmt>, IRExpr>)((stmts) => lower_do(stmts, ty)))((List<ADoStmt>)_mADoExpr30_.Field0) : (_scrutinee30_ is AErrorExpr _mAErrorExpr30_ ? ((Func<string, IRExpr>)((msg) => new IrError(msg, ty)))((string)_mAErrorExpr30_.Field0) : throw new InvalidOperationException("Non-exhaustive match")))))))))))))))))(e);
    }

    public static IRExpr lower_literal(string text, LiteralKind kind)
    {
        return ((Func<LiteralKind, IRExpr>)((_scrutinee31_) => (_scrutinee31_ is IntLit _mIntLit31_ ? new IrIntLit(long.Parse(text)) : (_scrutinee31_ is NumLit _mNumLit31_ ? new IrIntLit(long.Parse(text)) : (_scrutinee31_ is TextLit _mTextLit31_ ? new IrTextLit(text) : (_scrutinee31_ is BoolLit _mBoolLit31_ ? new IrBoolLit((text == "True")) : throw new InvalidOperationException("Non-exhaustive match")))))))(kind);
    }

    public static IRExpr lower_apply(AExpr f, AExpr a, CodexType ty)
    {
        return new IrApply(lower_expr(f, ty), lower_expr(a, ty), ty);
    }

    public static IRExpr lower_let(List<ALetBind> binds, AExpr body, CodexType ty)
    {
        return ((((long)binds.Count) == 0L) ? lower_expr(body, ty) : ((Func<ALetBind, IRExpr>)((b) => new IrLet(b.name.value, ty, lower_expr(b.value, new ErrorTy()), lower_let_rest(binds, body, ty, 1L))))(binds[(int)0L]));
    }

    public static IRExpr lower_let_rest(List<ALetBind> binds, AExpr body, CodexType ty, long i)
    {
        return ((i == ((long)binds.Count)) ? lower_expr(body, ty) : ((Func<ALetBind, IRExpr>)((b) => new IrLet(b.name.value, ty, lower_expr(b.value, new ErrorTy()), lower_let_rest(binds, body, ty, (i + 1L)))))(binds[(int)i]));
    }

    public static IRExpr lower_lambda(List<Name> @params, AExpr body, CodexType ty)
    {
        return ((Func<CodexType, IRExpr>)((stripped) => new IrLambda(lower_lambda_params(@params, stripped, 0L), lower_expr(body, get_lambda_return(stripped, ((long)@params.Count))), ty)))(strip_forall_ty(ty));
    }

    public static List<IRParam> lower_lambda_params(List<Name> @params, CodexType ty, long i)
    {
        return ((i == ((long)@params.Count)) ? new List<IRParam>() : ((Func<Name, List<IRParam>>)((p) => ((Func<CodexType, List<IRParam>>)((param_ty) => ((Func<CodexType, List<IRParam>>)((rest_ty) => Enumerable.Concat(new List<IRParam>() { new IRParam(p.value, param_ty) }, lower_lambda_params(@params, rest_ty, (i + 1L))).ToList()))(peel_fun_return(ty))))(peel_fun_param(ty))))(@params[(int)i]));
    }

    public static CodexType get_lambda_return(CodexType ty, long n)
    {
        while (true)
        {
            if ((n == 0L))
            {
                return ty;
            }
            else
            {
                var _tco_s = ty;
                if (_tco_s is FunTy _tco_m0)
                {
                    var p = _tco_m0.Field0;
                    var r = _tco_m0.Field1;
                    var _tco_0 = r;
                    var _tco_1 = (n - 1L);
                    ty = _tco_0;
                    n = _tco_1;
                    continue;
                }
                {
                    var _ = _tco_s;
                    return new ErrorTy();
                }
            }
        }
    }

    public static IRExpr lower_match(AExpr scrut, List<AMatchArm> arms, CodexType ty)
    {
        return new IrMatch(lower_expr(scrut, ty), map_list((_p0_) => lower_arm(ty, _p0_), arms), ty);
    }

    public static IRBranch lower_arm(CodexType ty, AMatchArm arm)
    {
        return new IRBranch(lower_pattern(arm.pattern), lower_expr(arm.body, ty));
    }

    public static IRPat lower_pattern(APat p)
    {
        return ((Func<APat, IRPat>)((_scrutinee32_) => (_scrutinee32_ is AVarPat _mAVarPat32_ ? ((Func<Name, IRPat>)((name) => new IrVarPat(name.value, new ErrorTy())))((Name)_mAVarPat32_.Field0) : (_scrutinee32_ is ALitPat _mALitPat32_ ? ((Func<LiteralKind, IRPat>)((kind) => ((Func<string, IRPat>)((text) => new IrLitPat(text, new ErrorTy())))((string)_mALitPat32_.Field0)))((LiteralKind)_mALitPat32_.Field1) : (_scrutinee32_ is ACtorPat _mACtorPat32_ ? ((Func<List<APat>, IRPat>)((subs) => ((Func<Name, IRPat>)((name) => new IrCtorPat(name.value, map_list(new Func<APat, IRPat>(lower_pattern), subs), new ErrorTy())))((Name)_mACtorPat32_.Field0)))((List<APat>)_mACtorPat32_.Field1) : (_scrutinee32_ is AWildPat _mAWildPat32_ ? new IrWildPat() : throw new InvalidOperationException("Non-exhaustive match")))))))(p);
    }

    public static IRExpr lower_list(List<AExpr> elems, CodexType ty)
    {
        return ((Func<CodexType, IRExpr>)((elem_ty) => new IrList(map_list((_p0_) => lower_elem(elem_ty, _p0_), elems), elem_ty)))((ty is ListTy _mListTy33_ ? ((Func<CodexType, CodexType>)((e) => e))((CodexType)_mListTy33_.Field0) : ((Func<CodexType, CodexType>)((_) => new ErrorTy()))(ty)));
    }

    public static IRExpr lower_elem(CodexType ty, AExpr e)
    {
        return lower_expr(e, ty);
    }

    public static IRExpr lower_record(Name name, List<AFieldExpr> fields, CodexType ty)
    {
        return new IrRecord(name.value, map_list((_p0_) => lower_field_val(ty, _p0_), fields), ty);
    }

    public static IRFieldVal lower_field_val(CodexType ty, AFieldExpr f)
    {
        return new IRFieldVal(f.name.value, lower_expr(f.value, ty));
    }

    public static IRExpr lower_do(List<ADoStmt> stmts, CodexType ty)
    {
        return new IrDo(map_list((_p0_) => lower_do_stmt(ty, _p0_), stmts), ty);
    }

    public static IRDoStmt lower_do_stmt(CodexType ty, ADoStmt s)
    {
        return ((Func<ADoStmt, IRDoStmt>)((_scrutinee34_) => (_scrutinee34_ is ADoBindStmt _mADoBindStmt34_ ? ((Func<AExpr, IRDoStmt>)((val) => ((Func<Name, IRDoStmt>)((name) => new IrDoBind(name.value, ty, lower_expr(val, ty))))((Name)_mADoBindStmt34_.Field0)))((AExpr)_mADoBindStmt34_.Field1) : (_scrutinee34_ is ADoExprStmt _mADoExprStmt34_ ? ((Func<AExpr, IRDoStmt>)((e) => new IrDoExec(lower_expr(e, ty))))((AExpr)_mADoExprStmt34_.Field0) : throw new InvalidOperationException("Non-exhaustive match")))))(s);
    }

    public static IRDef lower_def(ADef d, List<TypeBinding> types, UnificationState ust)
    {
        return ((Func<CodexType, IRDef>)((raw_type) => ((Func<CodexType, IRDef>)((full_type) => ((Func<CodexType, IRDef>)((stripped) => ((Func<List<IRParam>, IRDef>)((@params) => ((Func<CodexType, IRDef>)((ret_type) => new IRDef(d.name.value, @params, full_type, lower_expr(d.body, ret_type))))(get_return_type_n(stripped, ((long)d.@params.Count)))))(lower_def_params(d.@params, stripped, 0L))))(strip_forall_ty(full_type))))(deep_resolve(ust, raw_type))))(lookup_type(types, d.name.value));
    }

    public static List<IRParam> lower_def_params(List<AParam> @params, CodexType ty, long i)
    {
        return ((i == ((long)@params.Count)) ? new List<IRParam>() : ((Func<AParam, List<IRParam>>)((p) => ((Func<CodexType, List<IRParam>>)((param_ty) => ((Func<CodexType, List<IRParam>>)((rest_ty) => Enumerable.Concat(new List<IRParam>() { new IRParam(p.name.value, param_ty) }, lower_def_params(@params, rest_ty, (i + 1L))).ToList()))(peel_fun_return(ty))))(peel_fun_param(ty))))(@params[(int)i]));
    }

    public static CodexType get_return_type_n(CodexType ty, long n)
    {
        while (true)
        {
            if ((n == 0L))
            {
                return ty;
            }
            else
            {
                var _tco_s = ty;
                if (_tco_s is FunTy _tco_m0)
                {
                    var p = _tco_m0.Field0;
                    var r = _tco_m0.Field1;
                    var _tco_0 = r;
                    var _tco_1 = (n - 1L);
                    ty = _tco_0;
                    n = _tco_1;
                    continue;
                }
                {
                    var _ = _tco_s;
                    return new ErrorTy();
                }
            }
        }
    }

    public static IRModule lower_module(AModule m, List<TypeBinding> types, UnificationState ust)
    {
        return new IRModule(m.name, lower_defs(m.defs, types, ust, 0L));
    }

    public static List<IRDef> lower_defs(List<ADef> defs, List<TypeBinding> types, UnificationState ust, long i)
    {
        return ((i == ((long)defs.Count)) ? new List<IRDef>() : Enumerable.Concat(new List<IRDef>() { lower_def(defs[(int)i], types, ust) }, lower_defs(defs, types, ust, (i + 1L))).ToList());
    }

    public static Scope empty_scope()
    {
        return new Scope(new List<string>());
    }

    public static bool scope_has(Scope sc, string name)
    {
        return scope_has_loop(sc.names, name, 0L, ((long)sc.names.Count));
    }

    public static bool scope_has_loop(List<string> names, string name, long i, long len)
    {
        while (true)
        {
            if ((i == len))
            {
                return false;
            }
            else
            {
                if ((names[(int)i] == name))
                {
                    return true;
                }
                else
                {
                    var _tco_0 = names;
                    var _tco_1 = name;
                    var _tco_2 = (i + 1L);
                    var _tco_3 = len;
                    names = _tco_0;
                    name = _tco_1;
                    i = _tco_2;
                    len = _tco_3;
                    continue;
                }
            }
        }
    }

    public static Scope scope_add(Scope sc, string name)
    {
        return new Scope(Enumerable.Concat(new List<string>() { name }, sc.names).ToList());
    }

    public static List<string> builtin_names()
    {
        return new List<string>() { "show", "negate", "True", "False", "Nothing", "print-line", "read-line", "open-file", "read-all", "close-file", "char-at", "text-length", "substring", "is-letter", "is-digit", "is-whitespace", "text-to-integer", "integer-to-text", "text-replace", "char-code", "code-to-char", "list-length", "list-at", "map", "filter", "fold" };
    }

    public static bool is_type_name(string name)
    {
        return ((((long)name.Length) == 0L) ? false : ((name[(int)0L].ToString().Length > 0 && char.IsLetter(name[(int)0L].ToString()[0])) && is_upper_char(name[(int)0L].ToString())));
    }

    public static bool is_upper_char(string c)
    {
        return ((Func<long, bool>)((code) => ((code >= 65L) && (code <= 90L))))(((long)c[0]));
    }

    public static CollectResult collect_top_level_names(List<ADef> defs, long i, long len, List<string> acc, List<Diagnostic> errs)
    {
        while (true)
        {
            if ((i == len))
            {
                return new CollectResult(acc, errs);
            }
            else
            {
                var def = defs[(int)i];
                var name = def.name.value;
                if (list_contains(acc, name))
                {
                    var _tco_0 = defs;
                    var _tco_1 = (i + 1L);
                    var _tco_2 = len;
                    var _tco_3 = acc;
                    var _tco_4 = Enumerable.Concat(errs, new List<Diagnostic>() { make_error("CDX3001", string.Concat("Duplicate definition: ", name)) }).ToList();
                    defs = _tco_0;
                    i = _tco_1;
                    len = _tco_2;
                    acc = _tco_3;
                    errs = _tco_4;
                    continue;
                }
                else
                {
                    var _tco_0 = defs;
                    var _tco_1 = (i + 1L);
                    var _tco_2 = len;
                    var _tco_3 = Enumerable.Concat(acc, new List<string>() { name }).ToList();
                    var _tco_4 = errs;
                    defs = _tco_0;
                    i = _tco_1;
                    len = _tco_2;
                    acc = _tco_3;
                    errs = _tco_4;
                    continue;
                }
            }
        }
    }

    public static bool list_contains(List<string> xs, string name)
    {
        return list_contains_loop(xs, name, 0L, ((long)xs.Count));
    }

    public static bool list_contains_loop(List<string> xs, string name, long i, long len)
    {
        while (true)
        {
            if ((i == len))
            {
                return false;
            }
            else
            {
                if ((xs[(int)i] == name))
                {
                    return true;
                }
                else
                {
                    var _tco_0 = xs;
                    var _tco_1 = name;
                    var _tco_2 = (i + 1L);
                    var _tco_3 = len;
                    xs = _tco_0;
                    name = _tco_1;
                    i = _tco_2;
                    len = _tco_3;
                    continue;
                }
            }
        }
    }

    public static CtorCollectResult collect_ctor_names(List<ATypeDef> type_defs, long i, long len, List<string> type_acc, List<string> ctor_acc)
    {
        while (true)
        {
            if ((i == len))
            {
                return new CtorCollectResult(type_acc, ctor_acc);
            }
            else
            {
                var td = type_defs[(int)i];
                var _tco_s = td;
                if (_tco_s is AVariantTypeDef _tco_m0)
                {
                    var name = _tco_m0.Field0;
                    var @params = _tco_m0.Field1;
                    var ctors = _tco_m0.Field2;
                    var new_type_acc = Enumerable.Concat(type_acc, new List<string>() { name.value }).ToList();
                    var new_ctor_acc = collect_variant_ctors(ctors, 0L, ((long)ctors.Count), ctor_acc);
                    var _tco_0 = type_defs;
                    var _tco_1 = (i + 1L);
                    var _tco_2 = len;
                    var _tco_3 = new_type_acc;
                    var _tco_4 = new_ctor_acc;
                    type_defs = _tco_0;
                    i = _tco_1;
                    len = _tco_2;
                    type_acc = _tco_3;
                    ctor_acc = _tco_4;
                    continue;
                }
                else if (_tco_s is ARecordTypeDef _tco_m1)
                {
                    var name = _tco_m1.Field0;
                    var @params = _tco_m1.Field1;
                    var fields = _tco_m1.Field2;
                    var _tco_0 = type_defs;
                    var _tco_1 = (i + 1L);
                    var _tco_2 = len;
                    var _tco_3 = Enumerable.Concat(type_acc, new List<string>() { name.value }).ToList();
                    var _tco_4 = ctor_acc;
                    type_defs = _tco_0;
                    i = _tco_1;
                    len = _tco_2;
                    type_acc = _tco_3;
                    ctor_acc = _tco_4;
                    continue;
                }
            }
        }
    }

    public static List<string> collect_variant_ctors(List<AVariantCtorDef> ctors, long i, long len, List<string> acc)
    {
        while (true)
        {
            if ((i == len))
            {
                return acc;
            }
            else
            {
                var ctor = ctors[(int)i];
                var _tco_0 = ctors;
                var _tco_1 = (i + 1L);
                var _tco_2 = len;
                var _tco_3 = Enumerable.Concat(acc, new List<string>() { ctor.name.value }).ToList();
                ctors = _tco_0;
                i = _tco_1;
                len = _tco_2;
                acc = _tco_3;
                continue;
            }
        }
    }

    public static Scope build_all_names_scope(List<string> top_names, List<string> ctor_names, List<string> builtins)
    {
        return ((Func<Scope, Scope>)((sc) => ((Func<Scope, Scope>)((sc2) => add_names_to_scope(sc2, builtins, 0L, ((long)builtins.Count))))(add_names_to_scope(sc, ctor_names, 0L, ((long)ctor_names.Count)))))(add_names_to_scope(empty_scope(), top_names, 0L, ((long)top_names.Count)));
    }

    public static Scope add_names_to_scope(Scope sc, List<string> names, long i, long len)
    {
        while (true)
        {
            if ((i == len))
            {
                return sc;
            }
            else
            {
                var _tco_0 = scope_add(sc, names[(int)i]);
                var _tco_1 = names;
                var _tco_2 = (i + 1L);
                var _tco_3 = len;
                sc = _tco_0;
                names = _tco_1;
                i = _tco_2;
                len = _tco_3;
                continue;
            }
        }
    }

    public static List<Diagnostic> resolve_expr(Scope sc, AExpr expr)
    {
        while (true)
        {
            var _tco_s = expr;
            if (_tco_s is ALitExpr _tco_m0)
            {
                var val = _tco_m0.Field0;
                var kind = _tco_m0.Field1;
                return new List<Diagnostic>();
            }
            else if (_tco_s is ANameExpr _tco_m1)
            {
                var name = _tco_m1.Field0;
                if ((scope_has(sc, name.value) || is_type_name(name.value)))
                {
                    return new List<Diagnostic>();
                }
                else
                {
                    return new List<Diagnostic>() { make_error("CDX3002", string.Concat("Undefined name: ", name.value)) };
                }
            }
            else if (_tco_s is ABinaryExpr _tco_m2)
            {
                var left = _tco_m2.Field0;
                var op = _tco_m2.Field1;
                var right = _tco_m2.Field2;
                return Enumerable.Concat(resolve_expr(sc, left), resolve_expr(sc, right)).ToList();
            }
            else if (_tco_s is AUnaryExpr _tco_m3)
            {
                var operand = _tco_m3.Field0;
                var _tco_0 = sc;
                var _tco_1 = operand;
                sc = _tco_0;
                expr = _tco_1;
                continue;
            }
            else if (_tco_s is AApplyExpr _tco_m4)
            {
                var func = _tco_m4.Field0;
                var arg = _tco_m4.Field1;
                return Enumerable.Concat(resolve_expr(sc, func), resolve_expr(sc, arg)).ToList();
            }
            else if (_tco_s is AIfExpr _tco_m5)
            {
                var cond = _tco_m5.Field0;
                var then_e = _tco_m5.Field1;
                var else_e = _tco_m5.Field2;
                return Enumerable.Concat(resolve_expr(sc, cond), Enumerable.Concat(resolve_expr(sc, then_e), resolve_expr(sc, else_e)).ToList()).ToList();
            }
            else if (_tco_s is ALetExpr _tco_m6)
            {
                var bindings = _tco_m6.Field0;
                var body = _tco_m6.Field1;
                return resolve_let(sc, bindings, body, 0L, ((long)bindings.Count), new List<Diagnostic>());
            }
            else if (_tco_s is ALambdaExpr _tco_m7)
            {
                var @params = _tco_m7.Field0;
                var body = _tco_m7.Field1;
                var sc2 = add_lambda_params(sc, @params, 0L, ((long)@params.Count));
                var _tco_0 = sc2;
                var _tco_1 = body;
                sc = _tco_0;
                expr = _tco_1;
                continue;
            }
            else if (_tco_s is AMatchExpr _tco_m8)
            {
                var scrutinee = _tco_m8.Field0;
                var arms = _tco_m8.Field1;
                return Enumerable.Concat(resolve_expr(sc, scrutinee), resolve_match_arms(sc, arms, 0L, ((long)arms.Count), new List<Diagnostic>())).ToList();
            }
            else if (_tco_s is AListExpr _tco_m9)
            {
                var elems = _tco_m9.Field0;
                return resolve_list_elems(sc, elems, 0L, ((long)elems.Count), new List<Diagnostic>());
            }
            else if (_tco_s is ARecordExpr _tco_m10)
            {
                var name = _tco_m10.Field0;
                var fields = _tco_m10.Field1;
                return resolve_record_fields(sc, fields, 0L, ((long)fields.Count), new List<Diagnostic>());
            }
            else if (_tco_s is AFieldAccess _tco_m11)
            {
                var obj = _tco_m11.Field0;
                var field = _tco_m11.Field1;
                var _tco_0 = sc;
                var _tco_1 = obj;
                sc = _tco_0;
                expr = _tco_1;
                continue;
            }
            else if (_tco_s is ADoExpr _tco_m12)
            {
                var stmts = _tco_m12.Field0;
                return resolve_do_stmts(sc, stmts, 0L, ((long)stmts.Count), new List<Diagnostic>());
            }
            else if (_tco_s is AErrorExpr _tco_m13)
            {
                var msg = _tco_m13.Field0;
                return new List<Diagnostic>();
            }
        }
    }

    public static List<Diagnostic> resolve_let(Scope sc, List<ALetBind> bindings, AExpr body, long i, long len, List<Diagnostic> errs)
    {
        while (true)
        {
            if ((i == len))
            {
                return Enumerable.Concat(errs, resolve_expr(sc, body)).ToList();
            }
            else
            {
                var b = bindings[(int)i];
                var bind_errs = resolve_expr(sc, b.value);
                var sc2 = scope_add(sc, b.name.value);
                var _tco_0 = sc2;
                var _tco_1 = bindings;
                var _tco_2 = body;
                var _tco_3 = (i + 1L);
                var _tco_4 = len;
                var _tco_5 = Enumerable.Concat(errs, bind_errs).ToList();
                sc = _tco_0;
                bindings = _tco_1;
                body = _tco_2;
                i = _tco_3;
                len = _tco_4;
                errs = _tco_5;
                continue;
            }
        }
    }

    public static Scope add_lambda_params(Scope sc, List<Name> @params, long i, long len)
    {
        while (true)
        {
            if ((i == len))
            {
                return sc;
            }
            else
            {
                var p = @params[(int)i];
                var _tco_0 = scope_add(sc, p.value);
                var _tco_1 = @params;
                var _tco_2 = (i + 1L);
                var _tco_3 = len;
                sc = _tco_0;
                @params = _tco_1;
                i = _tco_2;
                len = _tco_3;
                continue;
            }
        }
    }

    public static List<Diagnostic> resolve_match_arms(Scope sc, List<AMatchArm> arms, long i, long len, List<Diagnostic> errs)
    {
        while (true)
        {
            if ((i == len))
            {
                return errs;
            }
            else
            {
                var arm = arms[(int)i];
                var sc2 = collect_pattern_names(sc, arm.pattern);
                var arm_errs = resolve_expr(sc2, arm.body);
                var _tco_0 = sc;
                var _tco_1 = arms;
                var _tco_2 = (i + 1L);
                var _tco_3 = len;
                var _tco_4 = Enumerable.Concat(errs, arm_errs).ToList();
                sc = _tco_0;
                arms = _tco_1;
                i = _tco_2;
                len = _tco_3;
                errs = _tco_4;
                continue;
            }
        }
    }

    public static Scope collect_pattern_names(Scope sc, APat pat)
    {
        return ((Func<APat, Scope>)((_scrutinee35_) => (_scrutinee35_ is AVarPat _mAVarPat35_ ? ((Func<Name, Scope>)((name) => scope_add(sc, name.value)))((Name)_mAVarPat35_.Field0) : (_scrutinee35_ is ACtorPat _mACtorPat35_ ? ((Func<List<APat>, Scope>)((subs) => ((Func<Name, Scope>)((name) => collect_ctor_pat_names(sc, subs, 0L, ((long)subs.Count))))((Name)_mACtorPat35_.Field0)))((List<APat>)_mACtorPat35_.Field1) : (_scrutinee35_ is ALitPat _mALitPat35_ ? ((Func<LiteralKind, Scope>)((kind) => ((Func<string, Scope>)((val) => sc))((string)_mALitPat35_.Field0)))((LiteralKind)_mALitPat35_.Field1) : (_scrutinee35_ is AWildPat _mAWildPat35_ ? sc : throw new InvalidOperationException("Non-exhaustive match")))))))(pat);
    }

    public static Scope collect_ctor_pat_names(Scope sc, List<APat> subs, long i, long len)
    {
        while (true)
        {
            if ((i == len))
            {
                return sc;
            }
            else
            {
                var sub = subs[(int)i];
                var _tco_0 = collect_pattern_names(sc, sub);
                var _tco_1 = subs;
                var _tco_2 = (i + 1L);
                var _tco_3 = len;
                sc = _tco_0;
                subs = _tco_1;
                i = _tco_2;
                len = _tco_3;
                continue;
            }
        }
    }

    public static List<Diagnostic> resolve_list_elems(Scope sc, List<AExpr> elems, long i, long len, List<Diagnostic> errs)
    {
        while (true)
        {
            if ((i == len))
            {
                return errs;
            }
            else
            {
                var errs2 = resolve_expr(sc, elems[(int)i]);
                var _tco_0 = sc;
                var _tco_1 = elems;
                var _tco_2 = (i + 1L);
                var _tco_3 = len;
                var _tco_4 = Enumerable.Concat(errs, errs2).ToList();
                sc = _tco_0;
                elems = _tco_1;
                i = _tco_2;
                len = _tco_3;
                errs = _tco_4;
                continue;
            }
        }
    }

    public static List<Diagnostic> resolve_record_fields(Scope sc, List<AFieldExpr> fields, long i, long len, List<Diagnostic> errs)
    {
        while (true)
        {
            if ((i == len))
            {
                return errs;
            }
            else
            {
                var f = fields[(int)i];
                var errs2 = resolve_expr(sc, f.value);
                var _tco_0 = sc;
                var _tco_1 = fields;
                var _tco_2 = (i + 1L);
                var _tco_3 = len;
                var _tco_4 = Enumerable.Concat(errs, errs2).ToList();
                sc = _tco_0;
                fields = _tco_1;
                i = _tco_2;
                len = _tco_3;
                errs = _tco_4;
                continue;
            }
        }
    }

    public static List<Diagnostic> resolve_do_stmts(Scope sc, List<ADoStmt> stmts, long i, long len, List<Diagnostic> errs)
    {
        while (true)
        {
            if ((i == len))
            {
                return errs;
            }
            else
            {
                var stmt = stmts[(int)i];
                var _tco_s = stmt;
                if (_tco_s is ADoExprStmt _tco_m0)
                {
                    var e = _tco_m0.Field0;
                    var errs2 = resolve_expr(sc, e);
                    var _tco_0 = sc;
                    var _tco_1 = stmts;
                    var _tco_2 = (i + 1L);
                    var _tco_3 = len;
                    var _tco_4 = Enumerable.Concat(errs, errs2).ToList();
                    sc = _tco_0;
                    stmts = _tco_1;
                    i = _tco_2;
                    len = _tco_3;
                    errs = _tco_4;
                    continue;
                }
                else if (_tco_s is ADoBindStmt _tco_m1)
                {
                    var name = _tco_m1.Field0;
                    var e = _tco_m1.Field1;
                    var errs2 = resolve_expr(sc, e);
                    var sc2 = scope_add(sc, name.value);
                    var _tco_0 = sc2;
                    var _tco_1 = stmts;
                    var _tco_2 = (i + 1L);
                    var _tco_3 = len;
                    var _tco_4 = Enumerable.Concat(errs, errs2).ToList();
                    sc = _tco_0;
                    stmts = _tco_1;
                    i = _tco_2;
                    len = _tco_3;
                    errs = _tco_4;
                    continue;
                }
            }
        }
    }

    public static List<Diagnostic> resolve_all_defs(Scope sc, List<ADef> defs, long i, long len, List<Diagnostic> errs)
    {
        while (true)
        {
            if ((i == len))
            {
                return errs;
            }
            else
            {
                var def = defs[(int)i];
                var def_scope = add_def_params(sc, def.@params, 0L, ((long)def.@params.Count));
                var errs2 = resolve_expr(def_scope, def.body);
                var _tco_0 = sc;
                var _tco_1 = defs;
                var _tco_2 = (i + 1L);
                var _tco_3 = len;
                var _tco_4 = Enumerable.Concat(errs, errs2).ToList();
                sc = _tco_0;
                defs = _tco_1;
                i = _tco_2;
                len = _tco_3;
                errs = _tco_4;
                continue;
            }
        }
    }

    public static Scope add_def_params(Scope sc, List<AParam> @params, long i, long len)
    {
        while (true)
        {
            if ((i == len))
            {
                return sc;
            }
            else
            {
                var p = @params[(int)i];
                var _tco_0 = scope_add(sc, p.name.value);
                var _tco_1 = @params;
                var _tco_2 = (i + 1L);
                var _tco_3 = len;
                sc = _tco_0;
                @params = _tco_1;
                i = _tco_2;
                len = _tco_3;
                continue;
            }
        }
    }

    public static ResolveResult resolve_module(AModule mod)
    {
        return ((Func<CollectResult, ResolveResult>)((top) => ((Func<CtorCollectResult, ResolveResult>)((ctors) => ((Func<Scope, ResolveResult>)((sc) => ((Func<List<Diagnostic>, ResolveResult>)((expr_errs) => new ResolveResult(Enumerable.Concat(top.errors, expr_errs).ToList(), top.names, ctors.type_names, ctors.ctor_names)))(resolve_all_defs(sc, mod.defs, 0L, ((long)mod.defs.Count), new List<Diagnostic>()))))(build_all_names_scope(top.names, ctors.ctor_names, builtin_names()))))(collect_ctor_names(mod.type_defs, 0L, ((long)mod.type_defs.Count), new List<string>(), new List<string>()))))(collect_top_level_names(mod.defs, 0L, ((long)mod.defs.Count), new List<string>(), new List<Diagnostic>()));
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
        while (true)
        {
            if (is_at_end(st))
            {
                return st;
            }
            else
            {
                if ((peek_char(st) == " "))
                {
                    var _tco_0 = advance_char(st);
                    st = _tco_0;
                    continue;
                }
                else
                {
                    return st;
                }
            }
        }
    }

    public static LexState scan_ident_rest(LexState st)
    {
        while (true)
        {
            if (is_at_end(st))
            {
                return st;
            }
            else
            {
                var ch = peek_char(st);
                if ((ch.Length > 0 && char.IsLetter(ch[0])))
                {
                    var _tco_0 = advance_char(st);
                    st = _tco_0;
                    continue;
                }
                else
                {
                    if ((ch.Length > 0 && char.IsDigit(ch[0])))
                    {
                        var _tco_0 = advance_char(st);
                        st = _tco_0;
                        continue;
                    }
                    else
                    {
                        if ((ch == "_"))
                        {
                            var _tco_0 = advance_char(st);
                            st = _tco_0;
                            continue;
                        }
                        else
                        {
                            if ((ch == "-"))
                            {
                                var next = advance_char(st);
                                if (is_at_end(next))
                                {
                                    return st;
                                }
                                else
                                {
                                    if ((peek_char(next).Length > 0 && char.IsLetter(peek_char(next)[0])))
                                    {
                                        var _tco_0 = next;
                                        st = _tco_0;
                                        continue;
                                    }
                                    else
                                    {
                                        return st;
                                    }
                                }
                            }
                            else
                            {
                                return st;
                            }
                        }
                    }
                }
            }
        }
    }

    public static LexState scan_digits(LexState st)
    {
        while (true)
        {
            if (is_at_end(st))
            {
                return st;
            }
            else
            {
                var ch = peek_char(st);
                if ((ch.Length > 0 && char.IsDigit(ch[0])))
                {
                    var _tco_0 = advance_char(st);
                    st = _tco_0;
                    continue;
                }
                else
                {
                    if ((ch == "_"))
                    {
                        var _tco_0 = advance_char(st);
                        st = _tco_0;
                        continue;
                    }
                    else
                    {
                        return st;
                    }
                }
            }
        }
    }

    public static LexState scan_string_body(LexState st)
    {
        while (true)
        {
            if (is_at_end(st))
            {
                return st;
            }
            else
            {
                var ch = peek_char(st);
                if ((ch == "\""))
                {
                    return advance_char(st);
                }
                else
                {
                    if ((ch == "\n"))
                    {
                        return st;
                    }
                    else
                    {
                        if ((ch == "\\"))
                        {
                            var _tco_0 = advance_char(advance_char(st));
                            st = _tco_0;
                            continue;
                        }
                        else
                        {
                            var _tco_0 = advance_char(st);
                            st = _tco_0;
                            continue;
                        }
                    }
                }
            }
        }
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
        return ((Func<LexState, LexResult>)((s) => (is_at_end(s) ? new LexEnd() : ((Func<string, LexResult>)((ch) => ((ch == "\n") ? new LexToken(make_token(new Newline(), "\n", s), advance_char(s)) : ((ch == "\"") ? ((Func<long, LexResult>)((start) => ((Func<LexState, LexResult>)((after) => ((Func<long, LexResult>)((text_len) => new LexToken(make_token(new TextLiteral(), s.source.Substring((int)start, (int)text_len), s), after)))(((after.offset - start) - 1L))))(scan_string_body(advance_char(s)))))((s.offset + 1L)) : ((ch.Length > 0 && char.IsLetter(ch[0])) ? ((Func<long, LexResult>)((start) => ((Func<LexState, LexResult>)((after) => ((Func<string, LexResult>)((word) => new LexToken(make_token(classify_word(word), word, s), after)))(extract_text(s, start, after))))(scan_ident_rest(advance_char(s)))))(s.offset) : ((ch == "_") ? ((Func<long, LexResult>)((start) => ((Func<LexState, LexResult>)((after) => ((Func<string, LexResult>)((word) => ((((long)word.Length) == 1L) ? new LexToken(make_token(new Underscore(), "_", s), after) : new LexToken(make_token(classify_word(word), word, s), after))))(extract_text(s, start, after))))(scan_ident_rest(advance_char(s)))))(s.offset) : ((ch.Length > 0 && char.IsDigit(ch[0])) ? ((Func<long, LexResult>)((start) => ((Func<LexState, LexResult>)((after) => (is_at_end(after) ? new LexToken(make_token(new IntegerLiteral(), extract_text(s, start, after), s), after) : ((peek_char(after) == ".") ? ((Func<LexState, LexResult>)((after2) => new LexToken(make_token(new NumberLiteral(), extract_text(s, start, after2), s), after2)))(scan_digits(advance_char(after))) : new LexToken(make_token(new IntegerLiteral(), extract_text(s, start, after), s), after)))))(scan_digits(advance_char(s)))))(s.offset) : scan_operator(s))))))))(peek_char(s)))))(skip_spaces(st));
    }

    public static LexResult scan_operator(LexState s)
    {
        return ((Func<string, LexResult>)((ch) => ((Func<LexState, LexResult>)((next) => ((ch == "(") ? new LexToken(make_token(new LeftParen(), "(", s), next) : ((ch == ")") ? new LexToken(make_token(new RightParen(), ")", s), next) : ((ch == "[") ? new LexToken(make_token(new LeftBracket(), "[", s), next) : ((ch == "]") ? new LexToken(make_token(new RightBracket(), "]", s), next) : ((ch == "{") ? new LexToken(make_token(new LeftBrace(), "{", s), next) : ((ch == "}") ? new LexToken(make_token(new RightBrace(), "}", s), next) : ((ch == ",") ? new LexToken(make_token(new Comma(), ",", s), next) : ((ch == ".") ? new LexToken(make_token(new Dot(), ".", s), next) : ((ch == "^") ? new LexToken(make_token(new Caret(), "^", s), next) : ((ch == "&") ? new LexToken(make_token(new Ampersand(), "&", s), next) : scan_multi_char_operator(s)))))))))))))(advance_char(s))))(peek_char(s));
    }

    public static LexResult scan_multi_char_operator(LexState s)
    {
        return ((Func<string, LexResult>)((ch) => ((Func<LexState, LexResult>)((next) => ((Func<string, LexResult>)((next_ch) => ((ch == "+") ? ((next_ch == "+") ? new LexToken(make_token(new PlusPlus(), "++", s), advance_char(next)) : new LexToken(make_token(new Plus(), "+", s), next)) : ((ch == "-") ? ((next_ch == ">") ? new LexToken(make_token(new Arrow(), "->", s), advance_char(next)) : new LexToken(make_token(new Minus(), "-", s), next)) : ((ch == "*") ? new LexToken(make_token(new Star(), "*", s), next) : ((ch == "/") ? ((next_ch == "=") ? new LexToken(make_token(new NotEquals(), "/=", s), advance_char(next)) : new LexToken(make_token(new Slash(), "/", s), next)) : ((ch == "=") ? ((next_ch == "=") ? ((Func<LexState, LexResult>)((next2) => ((Func<string, LexResult>)((next2_ch) => ((next2_ch == "=") ? new LexToken(make_token(new TripleEquals(), "===", s), advance_char(next2)) : new LexToken(make_token(new DoubleEquals(), "==", s), next2))))((is_at_end(next2) ? "" : peek_char(next2)))))(advance_char(next)) : new LexToken(make_token(new Equals_(), "=", s), next)) : ((ch == ":") ? ((next_ch == ":") ? new LexToken(make_token(new ColonColon(), "::", s), advance_char(next)) : new LexToken(make_token(new Colon(), ":", s), next)) : ((ch == "|") ? ((next_ch == "-") ? new LexToken(make_token(new Turnstile(), "|-", s), advance_char(next)) : new LexToken(make_token(new Pipe(), "|", s), next)) : ((ch == "<") ? ((next_ch == "=") ? new LexToken(make_token(new LessOrEqual(), "<=", s), advance_char(next)) : ((next_ch == "-") ? new LexToken(make_token(new LeftArrow(), "<-", s), advance_char(next)) : new LexToken(make_token(new LessThan(), "<", s), next))) : ((ch == ">") ? ((next_ch == "=") ? new LexToken(make_token(new GreaterOrEqual(), ">=", s), advance_char(next)) : new LexToken(make_token(new GreaterThan(), ">", s), next)) : new LexToken(make_token(new ErrorToken(), s.source[(int)s.offset].ToString(), s), next))))))))))))((is_at_end(next) ? "" : peek_char(next)))))(advance_char(s))))(peek_char(s));
    }

    public static List<Token> tokenize_loop(LexState st, List<Token> acc)
    {
        while (true)
        {
            var _tco_s = scan_token(st);
            if (_tco_s is LexToken _tco_m0)
            {
                var tok = _tco_m0.Field0;
                var next = _tco_m0.Field1;
                if ((tok.kind == new EndOfFile()))
                {
                    return Enumerable.Concat(acc, new List<Token>() { tok }).ToList();
                }
                else
                {
                    var _tco_0 = next;
                    var _tco_1 = Enumerable.Concat(acc, new List<Token>() { tok }).ToList();
                    st = _tco_0;
                    acc = _tco_1;
                    continue;
                }
            }
            else if (_tco_s is LexEnd _tco_m1)
            {
                return Enumerable.Concat(acc, new List<Token>() { make_token(new EndOfFile(), "", st) }).ToList();
            }
        }
    }

    public static List<Token> tokenize(string src)
    {
        return tokenize_loop(make_lex_state(src), new List<Token>());
    }

    public static ParseState make_parse_state(List<Token> toks)
    {
        return new ParseState(toks, 0L);
    }

    public static Token current(ParseState st)
    {
        return st.tokens[(int)st.pos];
    }

    public static TokenKind current_kind(ParseState st)
    {
        return current(st).kind;
    }

    public static ParseState advance(ParseState st)
    {
        return new ParseState(st.tokens, (st.pos + 1L));
    }

    public static bool is_done(ParseState st)
    {
        return (current_kind(st) is EndOfFile _mEndOfFile36_ ? true : ((Func<TokenKind, bool>)((_) => false))(current_kind(st)));
    }

    public static TokenKind peek_kind(ParseState st, long offset)
    {
        return st.tokens[(int)(st.pos + offset)].kind;
    }

    public static bool is_ident(TokenKind k)
    {
        return (k is Identifier _mIdentifier37_ ? true : ((Func<TokenKind, bool>)((_) => false))(k));
    }

    public static bool is_type_ident(TokenKind k)
    {
        return (k is TypeIdentifier _mTypeIdentifier38_ ? true : ((Func<TokenKind, bool>)((_) => false))(k));
    }

    public static bool is_arrow(TokenKind k)
    {
        return (k is Arrow _mArrow39_ ? true : ((Func<TokenKind, bool>)((_) => false))(k));
    }

    public static bool is_equals(TokenKind k)
    {
        return (k is Equals_ _mEquals_40_ ? true : ((Func<TokenKind, bool>)((_) => false))(k));
    }

    public static bool is_colon(TokenKind k)
    {
        return (k is Colon _mColon41_ ? true : ((Func<TokenKind, bool>)((_) => false))(k));
    }

    public static bool is_comma(TokenKind k)
    {
        return (k is Comma _mComma42_ ? true : ((Func<TokenKind, bool>)((_) => false))(k));
    }

    public static bool is_pipe(TokenKind k)
    {
        return (k is Pipe _mPipe43_ ? true : ((Func<TokenKind, bool>)((_) => false))(k));
    }

    public static bool is_dot(TokenKind k)
    {
        return (k is Dot _mDot44_ ? true : ((Func<TokenKind, bool>)((_) => false))(k));
    }

    public static bool is_left_paren(TokenKind k)
    {
        return (k is LeftParen _mLeftParen45_ ? true : ((Func<TokenKind, bool>)((_) => false))(k));
    }

    public static bool is_left_brace(TokenKind k)
    {
        return (k is LeftBrace _mLeftBrace46_ ? true : ((Func<TokenKind, bool>)((_) => false))(k));
    }

    public static bool is_left_bracket(TokenKind k)
    {
        return (k is LeftBracket _mLeftBracket47_ ? true : ((Func<TokenKind, bool>)((_) => false))(k));
    }

    public static bool is_right_brace(TokenKind k)
    {
        return (k is RightBrace _mRightBrace48_ ? true : ((Func<TokenKind, bool>)((_) => false))(k));
    }

    public static bool is_right_bracket(TokenKind k)
    {
        return (k is RightBracket _mRightBracket49_ ? true : ((Func<TokenKind, bool>)((_) => false))(k));
    }

    public static bool is_if_keyword(TokenKind k)
    {
        return (k is IfKeyword _mIfKeyword50_ ? true : ((Func<TokenKind, bool>)((_) => false))(k));
    }

    public static bool is_let_keyword(TokenKind k)
    {
        return (k is LetKeyword _mLetKeyword51_ ? true : ((Func<TokenKind, bool>)((_) => false))(k));
    }

    public static bool is_when_keyword(TokenKind k)
    {
        return (k is WhenKeyword _mWhenKeyword52_ ? true : ((Func<TokenKind, bool>)((_) => false))(k));
    }

    public static bool is_do_keyword(TokenKind k)
    {
        return (k is DoKeyword _mDoKeyword53_ ? true : ((Func<TokenKind, bool>)((_) => false))(k));
    }

    public static bool is_in_keyword(TokenKind k)
    {
        return (k is InKeyword _mInKeyword54_ ? true : ((Func<TokenKind, bool>)((_) => false))(k));
    }

    public static bool is_minus(TokenKind k)
    {
        return (k is Minus _mMinus55_ ? true : ((Func<TokenKind, bool>)((_) => false))(k));
    }

    public static bool is_dedent(TokenKind k)
    {
        return (k is Dedent _mDedent56_ ? true : ((Func<TokenKind, bool>)((_) => false))(k));
    }

    public static bool is_left_arrow(TokenKind k)
    {
        return (k is LeftArrow _mLeftArrow57_ ? true : ((Func<TokenKind, bool>)((_) => false))(k));
    }

    public static bool is_record_keyword(TokenKind k)
    {
        return (k is RecordKeyword _mRecordKeyword58_ ? true : ((Func<TokenKind, bool>)((_) => false))(k));
    }

    public static bool is_underscore(TokenKind k)
    {
        return (k is Underscore _mUnderscore59_ ? true : ((Func<TokenKind, bool>)((_) => false))(k));
    }

    public static bool is_literal(TokenKind k)
    {
        return ((Func<TokenKind, bool>)((_scrutinee60_) => (_scrutinee60_ is IntegerLiteral _mIntegerLiteral60_ ? true : (_scrutinee60_ is NumberLiteral _mNumberLiteral60_ ? true : (_scrutinee60_ is TextLiteral _mTextLiteral60_ ? true : (_scrutinee60_ is TrueKeyword _mTrueKeyword60_ ? true : (_scrutinee60_ is FalseKeyword _mFalseKeyword60_ ? true : ((Func<TokenKind, bool>)((_) => false))(_scrutinee60_))))))))(k);
    }

    public static bool is_app_start(TokenKind k)
    {
        return ((Func<TokenKind, bool>)((_scrutinee61_) => (_scrutinee61_ is Identifier _mIdentifier61_ ? true : (_scrutinee61_ is TypeIdentifier _mTypeIdentifier61_ ? true : (_scrutinee61_ is IntegerLiteral _mIntegerLiteral61_ ? true : (_scrutinee61_ is NumberLiteral _mNumberLiteral61_ ? true : (_scrutinee61_ is TextLiteral _mTextLiteral61_ ? true : (_scrutinee61_ is TrueKeyword _mTrueKeyword61_ ? true : (_scrutinee61_ is FalseKeyword _mFalseKeyword61_ ? true : (_scrutinee61_ is LeftParen _mLeftParen61_ ? true : (_scrutinee61_ is LeftBracket _mLeftBracket61_ ? true : ((Func<TokenKind, bool>)((_) => false))(_scrutinee61_))))))))))))(k);
    }

    public static bool is_compound(Expr e)
    {
        return ((Func<Expr, bool>)((_scrutinee62_) => (_scrutinee62_ is MatchExpr _mMatchExpr62_ ? ((Func<List<MatchArm>, bool>)((arms) => ((Func<Expr, bool>)((s) => true))((Expr)_mMatchExpr62_.Field0)))((List<MatchArm>)_mMatchExpr62_.Field1) : (_scrutinee62_ is IfExpr _mIfExpr62_ ? ((Func<Expr, bool>)((el) => ((Func<Expr, bool>)((t) => ((Func<Expr, bool>)((c) => true))((Expr)_mIfExpr62_.Field0)))((Expr)_mIfExpr62_.Field1)))((Expr)_mIfExpr62_.Field2) : (_scrutinee62_ is LetExpr _mLetExpr62_ ? ((Func<Expr, bool>)((body) => ((Func<List<LetBind>, bool>)((binds) => true))((List<LetBind>)_mLetExpr62_.Field0)))((Expr)_mLetExpr62_.Field1) : (_scrutinee62_ is DoExpr _mDoExpr62_ ? ((Func<List<DoStmt>, bool>)((stmts) => true))((List<DoStmt>)_mDoExpr62_.Field0) : ((Func<Expr, bool>)((_) => false))(_scrutinee62_)))))))(e);
    }

    public static bool is_type_arg_start(TokenKind k)
    {
        return ((Func<TokenKind, bool>)((_scrutinee63_) => (_scrutinee63_ is TypeIdentifier _mTypeIdentifier63_ ? true : (_scrutinee63_ is Identifier _mIdentifier63_ ? true : (_scrutinee63_ is LeftParen _mLeftParen63_ ? true : ((Func<TokenKind, bool>)((_) => false))(_scrutinee63_))))))(k);
    }

    public static long operator_precedence(TokenKind k)
    {
        return ((Func<TokenKind, long>)((_scrutinee64_) => (_scrutinee64_ is PlusPlus _mPlusPlus64_ ? 5L : (_scrutinee64_ is ColonColon _mColonColon64_ ? 5L : (_scrutinee64_ is Plus _mPlus64_ ? 6L : (_scrutinee64_ is Minus _mMinus64_ ? 6L : (_scrutinee64_ is Star _mStar64_ ? 7L : (_scrutinee64_ is Slash _mSlash64_ ? 7L : (_scrutinee64_ is Caret _mCaret64_ ? 8L : (_scrutinee64_ is DoubleEquals _mDoubleEquals64_ ? 4L : (_scrutinee64_ is NotEquals _mNotEquals64_ ? 4L : (_scrutinee64_ is LessThan _mLessThan64_ ? 4L : (_scrutinee64_ is GreaterThan _mGreaterThan64_ ? 4L : (_scrutinee64_ is LessOrEqual _mLessOrEqual64_ ? 4L : (_scrutinee64_ is GreaterOrEqual _mGreaterOrEqual64_ ? 4L : (_scrutinee64_ is TripleEquals _mTripleEquals64_ ? 4L : (_scrutinee64_ is Ampersand _mAmpersand64_ ? 3L : (_scrutinee64_ is Pipe _mPipe64_ ? 2L : ((Func<TokenKind, long>)((_) => (0L - 1L)))(_scrutinee64_)))))))))))))))))))(k);
    }

    public static bool is_right_assoc(TokenKind k)
    {
        return ((Func<TokenKind, bool>)((_scrutinee65_) => (_scrutinee65_ is PlusPlus _mPlusPlus65_ ? true : (_scrutinee65_ is ColonColon _mColonColon65_ ? true : (_scrutinee65_ is Caret _mCaret65_ ? true : (_scrutinee65_ is Arrow _mArrow65_ ? true : ((Func<TokenKind, bool>)((_) => false))(_scrutinee65_)))))))(k);
    }

    public static ParseState expect(TokenKind kind, ParseState st)
    {
        return (is_done(st) ? st : advance(st));
    }

    public static ParseState skip_newlines(ParseState st)
    {
        while (true)
        {
            if (is_done(st))
            {
                return st;
            }
            else
            {
                var _tco_s = current_kind(st);
                if (_tco_s is Newline _tco_m0)
                {
                    var _tco_0 = advance(st);
                    st = _tco_0;
                    continue;
                }
                else if (_tco_s is Indent _tco_m1)
                {
                    var _tco_0 = advance(st);
                    st = _tco_0;
                    continue;
                }
                else if (_tco_s is Dedent _tco_m2)
                {
                    var _tco_0 = advance(st);
                    st = _tco_0;
                    continue;
                }
                {
                    var _ = _tco_s;
                    return st;
                }
            }
        }
    }

    public static ParseTypeResult parse_type(ParseState st)
    {
        return ((Func<ParseTypeResult, ParseTypeResult>)((result) => unwrap_type_ok(result, (_p0_) => (_p1_) => parse_type_continue(_p0_, _p1_))))(parse_type_atom(st));
    }

    public static ParseTypeResult parse_type_continue(TypeExpr left, ParseState st)
    {
        return (is_arrow(current_kind(st)) ? ((Func<ParseState, ParseTypeResult>)((st2) => ((Func<ParseTypeResult, ParseTypeResult>)((right_result) => unwrap_type_ok(right_result, (_p0_) => (_p1_) => make_fun_type(left, _p0_, _p1_))))(parse_type(st2))))(advance(st)) : new TypeOk(left, st));
    }

    public static ParseTypeResult make_fun_type(TypeExpr left, TypeExpr right, ParseState st)
    {
        return new TypeOk(new FunType(left, right), st);
    }

    public static ParseTypeResult unwrap_type_ok(ParseTypeResult r, Func<TypeExpr, Func<ParseState, ParseTypeResult>> f)
    {
        return (r is TypeOk _mTypeOk66_ ? ((Func<ParseState, ParseTypeResult>)((st) => ((Func<TypeExpr, ParseTypeResult>)((t) => f(t)(st)))((TypeExpr)_mTypeOk66_.Field0)))((ParseState)_mTypeOk66_.Field1) : throw new InvalidOperationException("Non-exhaustive match"));
    }

    public static ParseTypeResult parse_type_atom(ParseState st)
    {
        return (is_ident(current_kind(st)) ? ((Func<Token, ParseTypeResult>)((tok) => parse_type_args(new NamedType(tok), advance(st))))(current(st)) : (is_type_ident(current_kind(st)) ? ((Func<Token, ParseTypeResult>)((tok) => parse_type_args(new NamedType(tok), advance(st))))(current(st)) : (is_left_paren(current_kind(st)) ? parse_paren_type(advance(st)) : ((Func<Token, ParseTypeResult>)((tok) => new TypeOk(new NamedType(tok), advance(st))))(current(st)))));
    }

    public static ParseTypeResult parse_paren_type(ParseState st)
    {
        return ((Func<ParseTypeResult, ParseTypeResult>)((inner) => unwrap_type_ok(inner, (_p0_) => (_p1_) => finish_paren_type(_p0_, _p1_))))(parse_type(st));
    }

    public static ParseTypeResult finish_paren_type(TypeExpr t, ParseState st)
    {
        return ((Func<ParseState, ParseTypeResult>)((st2) => new TypeOk(new ParenType(t), st2)))(expect(new RightParen(), st));
    }

    public static ParseTypeResult parse_type_args(TypeExpr base_type, ParseState st)
    {
        return (is_done(st) ? new TypeOk(base_type, st) : (is_type_arg_start(current_kind(st)) ? parse_type_arg_next(base_type, st) : new TypeOk(base_type, st)));
    }

    public static ParseTypeResult parse_type_arg_next(TypeExpr base_type, ParseState st)
    {
        return ((Func<ParseTypeResult, ParseTypeResult>)((arg_result) => unwrap_type_ok(arg_result, (_p0_) => (_p1_) => continue_type_args(base_type, _p0_, _p1_))))(parse_type_atom(st));
    }

    public static ParseTypeResult continue_type_args(TypeExpr base_type, TypeExpr arg, ParseState st)
    {
        return parse_type_args(new AppType(base_type, new List<TypeExpr>() { arg }), st);
    }

    public static ParsePatResult parse_pattern(ParseState st)
    {
        return (is_underscore(current_kind(st)) ? new PatOk(new WildPat(current(st)), advance(st)) : (is_literal(current_kind(st)) ? new PatOk(new LitPat(current(st)), advance(st)) : (is_type_ident(current_kind(st)) ? ((Func<Token, ParsePatResult>)((tok) => parse_ctor_pattern_fields(tok, new List<Pat>(), advance(st))))(current(st)) : (is_ident(current_kind(st)) ? new PatOk(new VarPat(current(st)), advance(st)) : new PatOk(new WildPat(current(st)), advance(st))))));
    }

    public static ParsePatResult parse_ctor_pattern_fields(Token ctor, List<Pat> acc, ParseState st)
    {
        return (is_left_paren(current_kind(st)) ? ((Func<ParseState, ParsePatResult>)((st2) => ((Func<ParsePatResult, ParsePatResult>)((sub) => unwrap_pat_ok(sub, (_p0_) => (_p1_) => continue_ctor_fields(ctor, acc, _p0_, _p1_))))(parse_pattern(st2))))(advance(st)) : new PatOk(new CtorPat(ctor, acc), st));
    }

    public static ParsePatResult continue_ctor_fields(Token ctor, List<Pat> acc, Pat p, ParseState st)
    {
        return ((Func<ParseState, ParsePatResult>)((st2) => parse_ctor_pattern_fields(ctor, Enumerable.Concat(acc, new List<Pat>() { p }).ToList(), st2)))(expect(new RightParen(), st));
    }

    public static ParsePatResult unwrap_pat_ok(ParsePatResult r, Func<Pat, Func<ParseState, ParsePatResult>> f)
    {
        return (r is PatOk _mPatOk67_ ? ((Func<ParseState, ParsePatResult>)((st) => ((Func<Pat, ParsePatResult>)((p) => f(p)(st)))((Pat)_mPatOk67_.Field0)))((ParseState)_mPatOk67_.Field1) : throw new InvalidOperationException("Non-exhaustive match"));
    }

    public static ParseExprResult parse_expr(ParseState st)
    {
        return parse_binary(st, 0L);
    }

    public static ParseExprResult unwrap_expr_ok(ParseExprResult r, Func<Expr, Func<ParseState, ParseExprResult>> f)
    {
        return (r is ExprOk _mExprOk68_ ? ((Func<ParseState, ParseExprResult>)((st) => ((Func<Expr, ParseExprResult>)((e) => f(e)(st)))((Expr)_mExprOk68_.Field0)))((ParseState)_mExprOk68_.Field1) : throw new InvalidOperationException("Non-exhaustive match"));
    }

    public static ParseExprResult parse_binary(ParseState st, long min_prec)
    {
        return ((Func<ParseExprResult, ParseExprResult>)((left_result) => unwrap_expr_ok(left_result, (_p0_) => (_p1_) => start_binary_loop(min_prec, _p0_, _p1_))))(parse_unary(st));
    }

    public static ParseExprResult start_binary_loop(long min_prec, Expr left, ParseState st)
    {
        return parse_binary_loop(left, st, min_prec);
    }

    public static ParseExprResult parse_binary_loop(Expr left, ParseState st, long min_prec)
    {
        return (is_done(st) ? new ExprOk(left, st) : ((Func<long, ParseExprResult>)((prec) => ((prec < min_prec) ? new ExprOk(left, st) : ((Func<Token, ParseExprResult>)((op) => ((Func<ParseState, ParseExprResult>)((st2) => ((Func<long, ParseExprResult>)((next_min) => ((Func<ParseExprResult, ParseExprResult>)((right_result) => unwrap_expr_ok(right_result, (_p0_) => (_p1_) => continue_binary(left, op, min_prec, _p0_, _p1_))))(parse_binary(st2, next_min))))((is_right_assoc(op.kind) ? prec : (prec + 1L)))))(skip_newlines(advance(st)))))(current(st)))))(operator_precedence(current_kind(st))));
    }

    public static ParseExprResult continue_binary(Expr left, Token op, long min_prec, Expr right, ParseState st)
    {
        return parse_binary_loop(new BinExpr(left, op, right), st, min_prec);
    }

    public static ParseExprResult parse_unary(ParseState st)
    {
        return (is_minus(current_kind(st)) ? ((Func<Token, ParseExprResult>)((op) => ((Func<ParseExprResult, ParseExprResult>)((result) => unwrap_expr_ok(result, (_p0_) => (_p1_) => finish_unary(op, _p0_, _p1_))))(parse_unary(advance(st)))))(current(st)) : parse_application(st));
    }

    public static ParseExprResult finish_unary(Token op, Expr operand, ParseState st)
    {
        return new ExprOk(new UnaryExpr(op, operand), st);
    }

    public static ParseExprResult parse_application(ParseState st)
    {
        return ((Func<ParseExprResult, ParseExprResult>)((func_result) => unwrap_expr_ok(func_result, (_p0_) => (_p1_) => parse_app_loop(_p0_, _p1_))))(parse_atom(st));
    }

    public static ParseExprResult parse_app_loop(Expr func, ParseState st)
    {
        return (is_compound(func) ? parse_field_access(func, st) : (is_done(st) ? new ExprOk(func, st) : (is_app_start(current_kind(st)) ? ((Func<ParseExprResult, ParseExprResult>)((arg_result) => unwrap_expr_ok(arg_result, (_p0_) => (_p1_) => continue_app(func, _p0_, _p1_))))(parse_atom(st)) : parse_field_access(func, st))));
    }

    public static ParseExprResult continue_app(Expr func, Expr arg, ParseState st)
    {
        return parse_app_loop(new AppExpr(func, arg), st);
    }

    public static ParseExprResult parse_atom(ParseState st)
    {
        return (is_literal(current_kind(st)) ? new ExprOk(new LitExpr(current(st)), advance(st)) : (is_ident(current_kind(st)) ? parse_field_access(new NameExpr(current(st)), advance(st)) : (is_type_ident(current_kind(st)) ? parse_atom_type_ident(st) : (is_left_paren(current_kind(st)) ? parse_paren_expr(advance(st)) : (is_left_bracket(current_kind(st)) ? parse_list_expr(st) : (is_if_keyword(current_kind(st)) ? parse_if_expr(st) : (is_let_keyword(current_kind(st)) ? parse_let_expr(st) : (is_when_keyword(current_kind(st)) ? parse_match_expr(st) : (is_do_keyword(current_kind(st)) ? parse_do_expr(st) : new ExprOk(new ErrExpr(current(st)), advance(st)))))))))));
    }

    public static ParseExprResult parse_field_access(Expr node, ParseState st)
    {
        while (true)
        {
            if (is_dot(current_kind(st)))
            {
                var st2 = advance(st);
                var field = current(st2);
                var st3 = advance(st2);
                var _tco_0 = new FieldExpr(node, field);
                var _tco_1 = st3;
                node = _tco_0;
                st = _tco_1;
                continue;
            }
            else
            {
                if (is_app_start(current_kind(st)))
                {
                    var arg_result = parse_atom(st);
                    return unwrap_expr_ok(arg_result, (_p0_) => (_p1_) => continue_app(node, _p0_, _p1_));
                }
                else
                {
                    return new ExprOk(node, st);
                }
            }
        }
    }

    public static ParseExprResult parse_atom_type_ident(ParseState st)
    {
        return ((Func<Token, ParseExprResult>)((tok) => ((Func<ParseState, ParseExprResult>)((st2) => (is_left_brace(current_kind(st2)) ? parse_record_expr(tok, st2) : new ExprOk(new NameExpr(tok), st2))))(advance(st))))(current(st));
    }

    public static ParseExprResult parse_paren_expr(ParseState st)
    {
        return ((Func<ParseState, ParseExprResult>)((st2) => ((Func<ParseExprResult, ParseExprResult>)((inner) => unwrap_expr_ok(inner, (_p0_) => (_p1_) => finish_paren_expr(_p0_, _p1_))))(parse_expr(st2))))(skip_newlines(st));
    }

    public static ParseExprResult finish_paren_expr(Expr e, ParseState st)
    {
        return ((Func<ParseState, ParseExprResult>)((st2) => ((Func<ParseState, ParseExprResult>)((st3) => new ExprOk(new ParenExpr(e), st3)))(expect(new RightParen(), st2))))(skip_newlines(st));
    }

    public static ParseExprResult parse_record_expr(Token type_name, ParseState st)
    {
        return ((Func<ParseState, ParseExprResult>)((st2) => ((Func<ParseState, ParseExprResult>)((st3) => parse_record_expr_fields(type_name, new List<RecordFieldExpr>(), st3)))(skip_newlines(st2))))(advance(st));
    }

    public static ParseExprResult parse_record_expr_fields(Token type_name, List<RecordFieldExpr> acc, ParseState st)
    {
        return (is_right_brace(current_kind(st)) ? new ExprOk(new RecordExpr(type_name, acc), advance(st)) : (is_ident(current_kind(st)) ? parse_record_field(type_name, acc, st) : new ExprOk(new RecordExpr(type_name, acc), st)));
    }

    public static ParseExprResult parse_record_field(Token type_name, List<RecordFieldExpr> acc, ParseState st)
    {
        return ((Func<Token, ParseExprResult>)((field_name) => ((Func<ParseState, ParseExprResult>)((st2) => ((Func<ParseState, ParseExprResult>)((st3) => ((Func<ParseExprResult, ParseExprResult>)((val_result) => unwrap_expr_ok(val_result, (_p0_) => (_p1_) => finish_record_field(type_name, acc, field_name, _p0_, _p1_))))(parse_expr(st3))))(expect(new Equals_(), st2))))(advance(st))))(current(st));
    }

    public static ParseExprResult finish_record_field(Token type_name, List<RecordFieldExpr> acc, Token field_name, Expr v, ParseState st)
    {
        return ((Func<RecordFieldExpr, ParseExprResult>)((field) => ((Func<ParseState, ParseExprResult>)((st2) => (is_comma(current_kind(st2)) ? parse_record_expr_fields(type_name, Enumerable.Concat(acc, new List<RecordFieldExpr>() { field }).ToList(), skip_newlines(advance(st2))) : parse_record_expr_fields(type_name, Enumerable.Concat(acc, new List<RecordFieldExpr>() { field }).ToList(), st2))))(skip_newlines(st))))(new RecordFieldExpr(field_name, v));
    }

    public static ParseExprResult parse_list_expr(ParseState st)
    {
        return ((Func<ParseState, ParseExprResult>)((st2) => ((Func<ParseState, ParseExprResult>)((st3) => parse_list_elements(new List<Expr>(), st3)))(skip_newlines(st2))))(advance(st));
    }

    public static ParseExprResult parse_list_elements(List<Expr> acc, ParseState st)
    {
        return (is_right_bracket(current_kind(st)) ? new ExprOk(new ListExpr(acc), advance(st)) : ((Func<ParseExprResult, ParseExprResult>)((elem) => unwrap_expr_ok(elem, (_p0_) => (_p1_) => finish_list_element(acc, _p0_, _p1_))))(parse_expr(st)));
    }

    public static ParseExprResult finish_list_element(List<Expr> acc, Expr e, ParseState st)
    {
        return ((Func<ParseState, ParseExprResult>)((st2) => (is_comma(current_kind(st2)) ? parse_list_elements(Enumerable.Concat(acc, new List<Expr>() { e }).ToList(), skip_newlines(advance(st2))) : parse_list_elements(Enumerable.Concat(acc, new List<Expr>() { e }).ToList(), st2))))(skip_newlines(st));
    }

    public static ParseExprResult parse_if_expr(ParseState st)
    {
        return ((Func<ParseState, ParseExprResult>)((st2) => ((Func<ParseExprResult, ParseExprResult>)((cond) => unwrap_expr_ok(cond, (_p0_) => (_p1_) => parse_if_then(_p0_, _p1_))))(parse_expr(st2))))(skip_newlines(advance(st)));
    }

    public static ParseExprResult parse_if_then(Expr c, ParseState st)
    {
        return ((Func<ParseState, ParseExprResult>)((st2) => ((Func<ParseState, ParseExprResult>)((st3) => ((Func<ParseState, ParseExprResult>)((st4) => ((Func<ParseExprResult, ParseExprResult>)((then_result) => unwrap_expr_ok(then_result, (_p0_) => (_p1_) => parse_if_else(c, _p0_, _p1_))))(parse_expr(st4))))(skip_newlines(st3))))(expect(new ThenKeyword(), st2))))(skip_newlines(st));
    }

    public static ParseExprResult parse_if_else(Expr c, Expr t, ParseState st)
    {
        return ((Func<ParseState, ParseExprResult>)((st2) => ((Func<ParseState, ParseExprResult>)((st3) => ((Func<ParseState, ParseExprResult>)((st4) => ((Func<ParseExprResult, ParseExprResult>)((else_result) => unwrap_expr_ok(else_result, (_p0_) => (_p1_) => finish_if(c, t, _p0_, _p1_))))(parse_expr(st4))))(skip_newlines(st3))))(expect(new ElseKeyword(), st2))))(skip_newlines(st));
    }

    public static ParseExprResult finish_if(Expr c, Expr t, Expr e, ParseState st)
    {
        return new ExprOk(new IfExpr(c, t, e), st);
    }

    public static ParseExprResult parse_let_expr(ParseState st)
    {
        return ((Func<ParseState, ParseExprResult>)((st2) => parse_let_bindings(new List<LetBind>(), st2)))(skip_newlines(advance(st)));
    }

    public static ParseExprResult parse_let_bindings(List<LetBind> acc, ParseState st)
    {
        return (is_ident(current_kind(st)) ? parse_let_binding(acc, st) : (is_in_keyword(current_kind(st)) ? ((Func<ParseState, ParseExprResult>)((st2) => ((Func<ParseExprResult, ParseExprResult>)((body) => unwrap_expr_ok(body, (_p0_) => (_p1_) => finish_let(acc, _p0_, _p1_))))(parse_expr(st2))))(skip_newlines(advance(st))) : ((Func<ParseExprResult, ParseExprResult>)((body) => unwrap_expr_ok(body, (_p0_) => (_p1_) => finish_let(acc, _p0_, _p1_))))(parse_expr(st))));
    }

    public static ParseExprResult finish_let(List<LetBind> acc, Expr b, ParseState st)
    {
        return new ExprOk(new LetExpr(acc, b), st);
    }

    public static ParseExprResult parse_let_binding(List<LetBind> acc, ParseState st)
    {
        return ((Func<Token, ParseExprResult>)((name_tok) => ((Func<ParseState, ParseExprResult>)((st2) => ((Func<ParseState, ParseExprResult>)((st3) => ((Func<ParseExprResult, ParseExprResult>)((val_result) => unwrap_expr_ok(val_result, (_p0_) => (_p1_) => finish_let_binding(acc, name_tok, _p0_, _p1_))))(parse_expr(st3))))(expect(new Equals_(), st2))))(advance(st))))(current(st));
    }

    public static ParseExprResult finish_let_binding(List<LetBind> acc, Token name_tok, Expr v, ParseState st)
    {
        return ((Func<LetBind, ParseExprResult>)((binding) => ((Func<ParseState, ParseExprResult>)((st2) => (is_comma(current_kind(st2)) ? parse_let_bindings(Enumerable.Concat(acc, new List<LetBind>() { binding }).ToList(), skip_newlines(advance(st2))) : parse_let_bindings(Enumerable.Concat(acc, new List<LetBind>() { binding }).ToList(), st2))))(skip_newlines(st))))(new LetBind(name_tok, v));
    }

    public static ParseExprResult parse_match_expr(ParseState st)
    {
        return ((Func<ParseState, ParseExprResult>)((st2) => ((Func<ParseExprResult, ParseExprResult>)((scrut) => unwrap_expr_ok(scrut, (_p0_) => (_p1_) => start_match_branches(_p0_, _p1_))))(parse_expr(st2))))(advance(st));
    }

    public static ParseExprResult start_match_branches(Expr s, ParseState st)
    {
        return ((Func<ParseState, ParseExprResult>)((st2) => parse_match_branches(s, new List<MatchArm>(), st2)))(skip_newlines(st));
    }

    public static ParseExprResult parse_match_branches(Expr scrut, List<MatchArm> acc, ParseState st)
    {
        return (is_if_keyword(current_kind(st)) ? parse_one_match_branch(scrut, acc, st) : new ExprOk(new MatchExpr(scrut, acc), st));
    }

    public static ParseExprResult unwrap_pat_for_expr(ParsePatResult r, Func<Pat, Func<ParseState, ParseExprResult>> f)
    {
        return (r is PatOk _mPatOk69_ ? ((Func<ParseState, ParseExprResult>)((st) => ((Func<Pat, ParseExprResult>)((p) => f(p)(st)))((Pat)_mPatOk69_.Field0)))((ParseState)_mPatOk69_.Field1) : throw new InvalidOperationException("Non-exhaustive match"));
    }

    public static ParseExprResult parse_one_match_branch(Expr scrut, List<MatchArm> acc, ParseState st)
    {
        return ((Func<ParseState, ParseExprResult>)((st2) => ((Func<ParsePatResult, ParseExprResult>)((pat) => unwrap_pat_for_expr(pat, (_p0_) => (_p1_) => parse_match_branch_body(scrut, acc, _p0_, _p1_))))(parse_pattern(st2))))(advance(st));
    }

    public static ParseExprResult parse_match_branch_body(Expr scrut, List<MatchArm> acc, Pat p, ParseState st)
    {
        return ((Func<ParseState, ParseExprResult>)((st2) => ((Func<ParseState, ParseExprResult>)((st3) => ((Func<ParseExprResult, ParseExprResult>)((body) => unwrap_expr_ok(body, (_p0_) => (_p1_) => finish_match_branch(scrut, acc, p, _p0_, _p1_))))(parse_expr(st3))))(skip_newlines(st2))))(expect(new Arrow(), st));
    }

    public static ParseExprResult finish_match_branch(Expr scrut, List<MatchArm> acc, Pat p, Expr b, ParseState st)
    {
        return ((Func<MatchArm, ParseExprResult>)((arm) => ((Func<ParseState, ParseExprResult>)((st2) => parse_match_branches(scrut, Enumerable.Concat(acc, new List<MatchArm>() { arm }).ToList(), st2)))(skip_newlines(st))))(new MatchArm(p, b));
    }

    public static ParseExprResult parse_do_expr(ParseState st)
    {
        return ((Func<ParseState, ParseExprResult>)((st2) => parse_do_stmts(new List<DoStmt>(), st2)))(skip_newlines(advance(st)));
    }

    public static ParseExprResult parse_do_stmts(List<DoStmt> acc, ParseState st)
    {
        return (is_done(st) ? new ExprOk(new DoExpr(acc), st) : (is_dedent(current_kind(st)) ? new ExprOk(new DoExpr(acc), st) : (is_do_bind(st) ? parse_do_bind_stmt(acc, st) : parse_do_expr_stmt(acc, st))));
    }

    public static bool is_do_bind(ParseState st)
    {
        return (is_ident(current_kind(st)) ? is_left_arrow(peek_kind(st, 1L)) : false);
    }

    public static ParseExprResult parse_do_bind_stmt(List<DoStmt> acc, ParseState st)
    {
        return ((Func<Token, ParseExprResult>)((name_tok) => ((Func<ParseState, ParseExprResult>)((st2) => ((Func<ParseExprResult, ParseExprResult>)((val_result) => unwrap_expr_ok(val_result, (_p0_) => (_p1_) => finish_do_bind(acc, name_tok, _p0_, _p1_))))(parse_expr(st2))))(advance(advance(st)))))(current(st));
    }

    public static ParseExprResult finish_do_bind(List<DoStmt> acc, Token name_tok, Expr v, ParseState st)
    {
        return ((Func<ParseState, ParseExprResult>)((st2) => parse_do_stmts(Enumerable.Concat(acc, new List<DoStmt>() { new DoBindStmt(name_tok, v) }).ToList(), st2)))(skip_newlines(st));
    }

    public static ParseExprResult parse_do_expr_stmt(List<DoStmt> acc, ParseState st)
    {
        return ((Func<ParseExprResult, ParseExprResult>)((expr_result) => unwrap_expr_ok(expr_result, (_p0_) => (_p1_) => finish_do_expr(acc, _p0_, _p1_))))(parse_expr(st));
    }

    public static ParseExprResult finish_do_expr(List<DoStmt> acc, Expr e, ParseState st)
    {
        return ((Func<ParseState, ParseExprResult>)((st2) => parse_do_stmts(Enumerable.Concat(acc, new List<DoStmt>() { new DoExprStmt(e) }).ToList(), st2)))(skip_newlines(st));
    }

    public static ParseTypeResult parse_type_annotation(ParseState st)
    {
        return ((Func<ParseState, ParseTypeResult>)((st2) => ((Func<ParseState, ParseTypeResult>)((st3) => parse_type(st3)))(expect(new Colon(), st2))))(advance(st));
    }

    public static ParseDefResult parse_definition(ParseState st)
    {
        return (is_done(st) ? new DefNone(st) : (is_ident(current_kind(st)) ? try_parse_def(st) : (is_type_ident(current_kind(st)) ? try_parse_def(st) : new DefNone(st))));
    }

    public static ParseDefResult try_parse_def(ParseState st)
    {
        return (is_colon(peek_kind(st, 1L)) ? ((Func<ParseTypeResult, ParseDefResult>)((ann_result) => unwrap_type_for_def(ann_result)))(parse_type_annotation(st)) : parse_def_body_with_ann(new List<TypeAnn>(), st));
    }

    public static ParseDefResult unwrap_type_for_def(ParseTypeResult r)
    {
        return (r is TypeOk _mTypeOk70_ ? ((Func<ParseState, ParseDefResult>)((st) => ((Func<TypeExpr, ParseDefResult>)((ann_type) => ((Func<Token, ParseDefResult>)((name_tok) => ((Func<List<TypeAnn>, ParseDefResult>)((ann) => ((Func<ParseState, ParseDefResult>)((st2) => parse_def_body_with_ann(ann, st2)))(skip_newlines(st))))(new List<TypeAnn>() { new TypeAnn(name_tok, ann_type) })))(new Token(new Identifier(), "", 0L, 0L, 0L))))((TypeExpr)_mTypeOk70_.Field0)))((ParseState)_mTypeOk70_.Field1) : throw new InvalidOperationException("Non-exhaustive match"));
    }

    public static ParseDefResult parse_def_body_with_ann(List<TypeAnn> ann, ParseState st)
    {
        return ((Func<Token, ParseDefResult>)((name_tok) => ((Func<ParseState, ParseDefResult>)((st2) => parse_def_params_then(ann, name_tok, new List<Token>(), st2)))(advance(st))))(current(st));
    }

    public static ParseDefResult parse_def_params_then(List<TypeAnn> ann, Token name_tok, List<Token> acc, ParseState st)
    {
        while (true)
        {
            if (is_left_paren(current_kind(st)))
            {
                var st2 = advance(st);
                if (is_ident(current_kind(st2)))
                {
                    var param = current(st2);
                    var st3 = advance(st2);
                    var st4 = expect(new RightParen(), st3);
                    var _tco_0 = ann;
                    var _tco_1 = name_tok;
                    var _tco_2 = Enumerable.Concat(acc, new List<Token>() { param }).ToList();
                    var _tco_3 = st4;
                    ann = _tco_0;
                    name_tok = _tco_1;
                    acc = _tco_2;
                    st = _tco_3;
                    continue;
                }
                else
                {
                    return finish_def(ann, name_tok, acc, st);
                }
            }
            else
            {
                return finish_def(ann, name_tok, acc, st);
            }
        }
    }

    public static ParseDefResult finish_def(List<TypeAnn> ann, Token name_tok, List<Token> @params, ParseState st)
    {
        return ((Func<ParseState, ParseDefResult>)((st2) => ((Func<ParseState, ParseDefResult>)((st3) => ((Func<ParseExprResult, ParseDefResult>)((body_result) => unwrap_def_body(body_result, ann, name_tok, @params)))(parse_expr(st3))))(skip_newlines(st2))))(expect(new Equals_(), st));
    }

    public static ParseDefResult unwrap_def_body(ParseExprResult r, List<TypeAnn> ann, Token name_tok, List<Token> @params)
    {
        return (r is ExprOk _mExprOk71_ ? ((Func<ParseState, ParseDefResult>)((st) => ((Func<Expr, ParseDefResult>)((b) => new DefOk(new Def(name_tok, @params, ann, b), st)))((Expr)_mExprOk71_.Field0)))((ParseState)_mExprOk71_.Field1) : throw new InvalidOperationException("Non-exhaustive match"));
    }

    public static ParseTypeDefResult parse_type_def(ParseState st)
    {
        return (is_type_ident(current_kind(st)) ? ((Func<Token, ParseTypeDefResult>)((name_tok) => ((Func<ParseState, ParseTypeDefResult>)((st2) => (is_equals(current_kind(st2)) ? ((Func<ParseState, ParseTypeDefResult>)((st3) => (is_record_keyword(current_kind(st3)) ? parse_record_type(name_tok, st3) : (is_pipe(current_kind(st3)) ? parse_variant_type(name_tok, st3) : new TypeDefNone(st)))))(skip_newlines(advance(st2))) : new TypeDefNone(st))))(advance(st))))(current(st)) : new TypeDefNone(st));
    }

    public static ParseTypeDefResult parse_record_type(Token name_tok, ParseState st)
    {
        return ((Func<ParseState, ParseTypeDefResult>)((st2) => ((Func<ParseState, ParseTypeDefResult>)((st3) => ((Func<ParseState, ParseTypeDefResult>)((st4) => parse_record_fields_loop(name_tok, new List<RecordFieldDef>(), st4)))(skip_newlines(st3))))(expect(new LeftBrace(), st2))))(advance(st));
    }

    public static ParseTypeDefResult parse_record_fields_loop(Token name_tok, List<RecordFieldDef> acc, ParseState st)
    {
        return (is_right_brace(current_kind(st)) ? new TypeDefOk(new TypeDef(name_tok, new List<Token>(), new RecordBody(acc)), advance(st)) : (is_ident(current_kind(st)) ? parse_one_record_field(name_tok, acc, st) : new TypeDefOk(new TypeDef(name_tok, new List<Token>(), new RecordBody(acc)), st)));
    }

    public static ParseTypeDefResult parse_one_record_field(Token name_tok, List<RecordFieldDef> acc, ParseState st)
    {
        return ((Func<Token, ParseTypeDefResult>)((field_name) => ((Func<ParseState, ParseTypeDefResult>)((st2) => ((Func<ParseState, ParseTypeDefResult>)((st3) => ((Func<ParseTypeResult, ParseTypeDefResult>)((field_type_result) => unwrap_record_field_type(name_tok, acc, field_name, field_type_result)))(parse_type(st3))))(expect(new Colon(), st2))))(advance(st))))(current(st));
    }

    public static ParseTypeDefResult unwrap_record_field_type(Token name_tok, List<RecordFieldDef> acc, Token field_name, ParseTypeResult r)
    {
        return (r is TypeOk _mTypeOk72_ ? ((Func<ParseState, ParseTypeDefResult>)((st) => ((Func<TypeExpr, ParseTypeDefResult>)((ft) => ((Func<RecordFieldDef, ParseTypeDefResult>)((field) => ((Func<ParseState, ParseTypeDefResult>)((st2) => (is_comma(current_kind(st2)) ? parse_record_fields_loop(name_tok, Enumerable.Concat(acc, new List<RecordFieldDef>() { field }).ToList(), skip_newlines(advance(st2))) : parse_record_fields_loop(name_tok, Enumerable.Concat(acc, new List<RecordFieldDef>() { field }).ToList(), st2))))(skip_newlines(st))))(new RecordFieldDef(field_name, ft))))((TypeExpr)_mTypeOk72_.Field0)))((ParseState)_mTypeOk72_.Field1) : throw new InvalidOperationException("Non-exhaustive match"));
    }

    public static ParseTypeDefResult parse_variant_type(Token name_tok, ParseState st)
    {
        return parse_variant_ctors(name_tok, new List<VariantCtorDef>(), st);
    }

    public static ParseTypeDefResult parse_variant_ctors(Token name_tok, List<VariantCtorDef> acc, ParseState st)
    {
        return (is_pipe(current_kind(st)) ? ((Func<ParseState, ParseTypeDefResult>)((st2) => ((Func<Token, ParseTypeDefResult>)((ctor_name) => ((Func<ParseState, ParseTypeDefResult>)((st3) => parse_ctor_fields(ctor_name, new List<TypeExpr>(), st3, name_tok, acc)))(advance(st2))))(current(st2))))(skip_newlines(advance(st))) : new TypeDefOk(new TypeDef(name_tok, new List<Token>(), new VariantBody(acc)), st));
    }

    public static ParseTypeDefResult parse_ctor_fields(Token ctor_name, List<TypeExpr> fields, ParseState st, Token name_tok, List<VariantCtorDef> acc)
    {
        return (is_left_paren(current_kind(st)) ? ((Func<ParseTypeResult, ParseTypeDefResult>)((field_result) => unwrap_ctor_field(field_result, ctor_name, fields, name_tok, acc)))(parse_type(advance(st))) : ((Func<ParseState, ParseTypeDefResult>)((st2) => ((Func<VariantCtorDef, ParseTypeDefResult>)((ctor) => parse_variant_ctors(name_tok, Enumerable.Concat(acc, new List<VariantCtorDef>() { ctor }).ToList(), st2)))(new VariantCtorDef(ctor_name, fields))))(skip_newlines(st)));
    }

    public static ParseTypeDefResult unwrap_ctor_field(ParseTypeResult r, Token ctor_name, List<TypeExpr> fields, Token name_tok, List<VariantCtorDef> acc)
    {
        return (r is TypeOk _mTypeOk73_ ? ((Func<ParseState, ParseTypeDefResult>)((st) => ((Func<TypeExpr, ParseTypeDefResult>)((ty) => ((Func<ParseState, ParseTypeDefResult>)((st2) => parse_ctor_fields(ctor_name, Enumerable.Concat(fields, new List<TypeExpr>() { ty }).ToList(), st2, name_tok, acc)))(expect(new RightParen(), st))))((TypeExpr)_mTypeOk73_.Field0)))((ParseState)_mTypeOk73_.Field1) : throw new InvalidOperationException("Non-exhaustive match"));
    }

    public static Document parse_document(ParseState st)
    {
        return ((Func<ParseState, Document>)((st2) => parse_top_level(new List<Def>(), new List<TypeDef>(), st2)))(skip_newlines(st));
    }

    public static Document parse_top_level(List<Def> defs, List<TypeDef> type_defs, ParseState st)
    {
        return (is_done(st) ? new Document(defs, type_defs) : try_top_level_type_def(defs, type_defs, st));
    }

    public static Document try_top_level_type_def(List<Def> defs, List<TypeDef> type_defs, ParseState st)
    {
        return ((Func<ParseTypeDefResult, Document>)((td_result) => ((Func<ParseTypeDefResult, Document>)((_scrutinee74_) => (_scrutinee74_ is TypeDefOk _mTypeDefOk74_ ? ((Func<ParseState, Document>)((st2) => ((Func<TypeDef, Document>)((td) => parse_top_level(defs, Enumerable.Concat(type_defs, new List<TypeDef>() { td }).ToList(), skip_newlines(st2))))((TypeDef)_mTypeDefOk74_.Field0)))((ParseState)_mTypeDefOk74_.Field1) : (_scrutinee74_ is TypeDefNone _mTypeDefNone74_ ? ((Func<ParseState, Document>)((st2) => try_top_level_def(defs, type_defs, st)))((ParseState)_mTypeDefNone74_.Field0) : throw new InvalidOperationException("Non-exhaustive match")))))(td_result)))(parse_type_def(st));
    }

    public static Document try_top_level_def(List<Def> defs, List<TypeDef> type_defs, ParseState st)
    {
        return ((Func<ParseDefResult, Document>)((def_result) => ((Func<ParseDefResult, Document>)((_scrutinee75_) => (_scrutinee75_ is DefOk _mDefOk75_ ? ((Func<ParseState, Document>)((st2) => ((Func<Def, Document>)((d) => parse_top_level(Enumerable.Concat(defs, new List<Def>() { d }).ToList(), type_defs, skip_newlines(st2))))((Def)_mDefOk75_.Field0)))((ParseState)_mDefOk75_.Field1) : (_scrutinee75_ is DefNone _mDefNone75_ ? ((Func<ParseState, Document>)((st2) => parse_top_level(defs, type_defs, skip_newlines(advance(st2)))))((ParseState)_mDefNone75_.Field0) : throw new InvalidOperationException("Non-exhaustive match")))))(def_result)))(parse_definition(st));
    }

    public static long token_length(Token t)
    {
        return ((long)t.text.Length);
    }

    public static CheckResult infer_literal(UnificationState st, LiteralKind kind)
    {
        return ((Func<LiteralKind, CheckResult>)((_scrutinee76_) => (_scrutinee76_ is IntLit _mIntLit76_ ? new CheckResult(new IntegerTy(), st) : (_scrutinee76_ is NumLit _mNumLit76_ ? new CheckResult(new NumberTy(), st) : (_scrutinee76_ is TextLit _mTextLit76_ ? new CheckResult(new TextTy(), st) : (_scrutinee76_ is BoolLit _mBoolLit76_ ? new CheckResult(new BooleanTy(), st) : throw new InvalidOperationException("Non-exhaustive match")))))))(kind);
    }

    public static CheckResult infer_name(UnificationState st, TypeEnv env, string name)
    {
        return (env_has(env, name) ? ((Func<CodexType, CheckResult>)((raw) => ((Func<FreshResult, CheckResult>)((inst) => new CheckResult(inst.var_type, inst.state)))(instantiate_type(st, raw))))(env_lookup(env, name)) : new CheckResult(new ErrorTy(), add_unify_error(st, "CDX2002", string.Concat("Unknown name: ", name))));
    }

    public static FreshResult instantiate_type(UnificationState st, CodexType ty)
    {
        while (true)
        {
            var _tco_s = ty;
            if (_tco_s is ForAllTy _tco_m0)
            {
                var var_id = _tco_m0.Field0;
                var body = _tco_m0.Field1;
                var fr = fresh_and_advance(st);
                var substituted = subst_type_var(body, var_id, fr.var_type);
                var _tco_0 = fr.state;
                var _tco_1 = substituted;
                st = _tco_0;
                ty = _tco_1;
                continue;
            }
            {
                var _ = _tco_s;
                return new FreshResult(ty, st);
            }
        }
    }

    public static CodexType subst_type_var(CodexType ty, long var_id, CodexType replacement)
    {
        return ((Func<CodexType, CodexType>)((_scrutinee77_) => (_scrutinee77_ is TypeVar _mTypeVar77_ ? ((Func<long, CodexType>)((id) => ((id == var_id) ? replacement : ty)))((long)_mTypeVar77_.Field0) : (_scrutinee77_ is FunTy _mFunTy77_ ? ((Func<CodexType, CodexType>)((ret) => ((Func<CodexType, CodexType>)((param) => new FunTy(subst_type_var(param, var_id, replacement), subst_type_var(ret, var_id, replacement))))((CodexType)_mFunTy77_.Field0)))((CodexType)_mFunTy77_.Field1) : (_scrutinee77_ is ListTy _mListTy77_ ? ((Func<CodexType, CodexType>)((elem) => new ListTy(subst_type_var(elem, var_id, replacement))))((CodexType)_mListTy77_.Field0) : (_scrutinee77_ is ForAllTy _mForAllTy77_ ? ((Func<CodexType, CodexType>)((body) => ((Func<long, CodexType>)((inner_id) => ((inner_id == var_id) ? ty : new ForAllTy(inner_id, subst_type_var(body, var_id, replacement)))))((long)_mForAllTy77_.Field0)))((CodexType)_mForAllTy77_.Field1) : (_scrutinee77_ is ConstructedTy _mConstructedTy77_ ? ((Func<List<CodexType>, CodexType>)((args) => ((Func<Name, CodexType>)((name) => new ConstructedTy(name, map_subst_type_var(args, var_id, replacement, 0L, ((long)args.Count), new List<CodexType>()))))((Name)_mConstructedTy77_.Field0)))((List<CodexType>)_mConstructedTy77_.Field1) : (_scrutinee77_ is SumTy _mSumTy77_ ? ((Func<List<SumCtor>, CodexType>)((ctors) => ((Func<Name, CodexType>)((name) => ty))((Name)_mSumTy77_.Field0)))((List<SumCtor>)_mSumTy77_.Field1) : (_scrutinee77_ is RecordTy _mRecordTy77_ ? ((Func<List<RecordField>, CodexType>)((fields) => ((Func<Name, CodexType>)((name) => ty))((Name)_mRecordTy77_.Field0)))((List<RecordField>)_mRecordTy77_.Field1) : ((Func<CodexType, CodexType>)((_) => ty))(_scrutinee77_))))))))))(ty);
    }

    public static List<CodexType> map_subst_type_var(List<CodexType> args, long var_id, CodexType replacement, long i, long len, List<CodexType> acc)
    {
        while (true)
        {
            if ((i == len))
            {
                return acc;
            }
            else
            {
                var _tco_0 = args;
                var _tco_1 = var_id;
                var _tco_2 = replacement;
                var _tco_3 = (i + 1L);
                var _tco_4 = len;
                var _tco_5 = Enumerable.Concat(acc, new List<CodexType>() { subst_type_var(args[(int)i], var_id, replacement) }).ToList();
                args = _tco_0;
                var_id = _tco_1;
                replacement = _tco_2;
                i = _tco_3;
                len = _tco_4;
                acc = _tco_5;
                continue;
            }
        }
    }

    public static CheckResult infer_binary(UnificationState st, TypeEnv env, AExpr left, BinaryOp op, AExpr right)
    {
        return ((Func<CheckResult, CheckResult>)((lr) => ((Func<CheckResult, CheckResult>)((rr) => infer_binary_op(rr.state, lr.inferred_type, rr.inferred_type, op)))(infer_expr(lr.state, env, right))))(infer_expr(st, env, left));
    }

    public static CheckResult infer_binary_op(UnificationState st, CodexType lt, CodexType rt, BinaryOp op)
    {
        return ((Func<BinaryOp, CheckResult>)((_scrutinee78_) => (_scrutinee78_ is OpAdd _mOpAdd78_ ? infer_arithmetic(st, lt, rt) : (_scrutinee78_ is OpSub _mOpSub78_ ? infer_arithmetic(st, lt, rt) : (_scrutinee78_ is OpMul _mOpMul78_ ? infer_arithmetic(st, lt, rt) : (_scrutinee78_ is OpDiv _mOpDiv78_ ? infer_arithmetic(st, lt, rt) : (_scrutinee78_ is OpPow _mOpPow78_ ? infer_arithmetic(st, lt, rt) : (_scrutinee78_ is OpEq _mOpEq78_ ? infer_comparison(st, lt, rt) : (_scrutinee78_ is OpNotEq _mOpNotEq78_ ? infer_comparison(st, lt, rt) : (_scrutinee78_ is OpLt _mOpLt78_ ? infer_comparison(st, lt, rt) : (_scrutinee78_ is OpGt _mOpGt78_ ? infer_comparison(st, lt, rt) : (_scrutinee78_ is OpLtEq _mOpLtEq78_ ? infer_comparison(st, lt, rt) : (_scrutinee78_ is OpGtEq _mOpGtEq78_ ? infer_comparison(st, lt, rt) : (_scrutinee78_ is OpAnd _mOpAnd78_ ? infer_logical(st, lt, rt) : (_scrutinee78_ is OpOr _mOpOr78_ ? infer_logical(st, lt, rt) : (_scrutinee78_ is OpAppend _mOpAppend78_ ? infer_append(st, lt, rt) : (_scrutinee78_ is OpCons _mOpCons78_ ? infer_cons(st, lt, rt) : (_scrutinee78_ is OpDefEq _mOpDefEq78_ ? infer_comparison(st, lt, rt) : throw new InvalidOperationException("Non-exhaustive match")))))))))))))))))))(op);
    }

    public static CheckResult infer_arithmetic(UnificationState st, CodexType lt, CodexType rt)
    {
        return ((Func<UnifyResult, CheckResult>)((r) => new CheckResult(lt, r.state)))(unify(st, lt, rt));
    }

    public static CheckResult infer_comparison(UnificationState st, CodexType lt, CodexType rt)
    {
        return ((Func<UnifyResult, CheckResult>)((r) => new CheckResult(new BooleanTy(), r.state)))(unify(st, lt, rt));
    }

    public static CheckResult infer_logical(UnificationState st, CodexType lt, CodexType rt)
    {
        return ((Func<UnifyResult, CheckResult>)((r1) => ((Func<UnifyResult, CheckResult>)((r2) => new CheckResult(new BooleanTy(), r2.state)))(unify(r1.state, rt, new BooleanTy()))))(unify(st, lt, new BooleanTy()));
    }

    public static CheckResult infer_append(UnificationState st, CodexType lt, CodexType rt)
    {
        return ((Func<CodexType, CheckResult>)((resolved) => (resolved is TextTy _mTextTy79_ ? ((Func<UnifyResult, CheckResult>)((r) => new CheckResult(new TextTy(), r.state)))(unify(st, rt, new TextTy())) : ((Func<CodexType, CheckResult>)((_) => ((Func<UnifyResult, CheckResult>)((r) => new CheckResult(lt, r.state)))(unify(st, lt, rt))))(resolved))))(resolve(st, lt));
    }

    public static CheckResult infer_cons(UnificationState st, CodexType lt, CodexType rt)
    {
        return ((Func<CodexType, CheckResult>)((list_ty) => ((Func<UnifyResult, CheckResult>)((r) => new CheckResult(list_ty, r.state)))(unify(st, rt, list_ty))))(new ListTy(lt));
    }

    public static CheckResult infer_if(UnificationState st, TypeEnv env, AExpr cond, AExpr then_e, AExpr else_e)
    {
        return ((Func<CheckResult, CheckResult>)((cr) => ((Func<UnifyResult, CheckResult>)((r1) => ((Func<CheckResult, CheckResult>)((tr) => ((Func<CheckResult, CheckResult>)((er) => ((Func<UnifyResult, CheckResult>)((r2) => new CheckResult(tr.inferred_type, r2.state)))(unify(er.state, tr.inferred_type, er.inferred_type))))(infer_expr(tr.state, env, else_e))))(infer_expr(r1.state, env, then_e))))(unify(cr.state, cr.inferred_type, new BooleanTy()))))(infer_expr(st, env, cond));
    }

    public static CheckResult infer_let(UnificationState st, TypeEnv env, List<ALetBind> bindings, AExpr body)
    {
        return ((Func<LetBindResult, CheckResult>)((env2) => infer_expr(env2.state, env2.env, body)))(infer_let_bindings(st, env, bindings, 0L, ((long)bindings.Count)));
    }

    public static LetBindResult infer_let_bindings(UnificationState st, TypeEnv env, List<ALetBind> bindings, long i, long len)
    {
        while (true)
        {
            if ((i == len))
            {
                return new LetBindResult(st, env);
            }
            else
            {
                var b = bindings[(int)i];
                var vr = infer_expr(st, env, b.value);
                var env2 = env_bind(env, b.name.value, vr.inferred_type);
                var _tco_0 = vr.state;
                var _tco_1 = env2;
                var _tco_2 = bindings;
                var _tco_3 = (i + 1L);
                var _tco_4 = len;
                st = _tco_0;
                env = _tco_1;
                bindings = _tco_2;
                i = _tco_3;
                len = _tco_4;
                continue;
            }
        }
    }

    public static CheckResult infer_lambda(UnificationState st, TypeEnv env, List<Name> @params, AExpr body)
    {
        return ((Func<LambdaBindResult, CheckResult>)((pr) => ((Func<CheckResult, CheckResult>)((br) => ((Func<CodexType, CheckResult>)((fun_ty) => new CheckResult(fun_ty, br.state)))(wrap_fun_type(pr.param_types, br.inferred_type))))(infer_expr(pr.state, pr.env, body))))(bind_lambda_params(st, env, @params, 0L, ((long)@params.Count), new List<CodexType>()));
    }

    public static LambdaBindResult bind_lambda_params(UnificationState st, TypeEnv env, List<Name> @params, long i, long len, List<CodexType> acc)
    {
        while (true)
        {
            if ((i == len))
            {
                return new LambdaBindResult(st, env, acc);
            }
            else
            {
                var p = @params[(int)i];
                var fr = fresh_and_advance(st);
                var env2 = env_bind(env, p.value, fr.var_type);
                var _tco_0 = fr.state;
                var _tco_1 = env2;
                var _tco_2 = @params;
                var _tco_3 = (i + 1L);
                var _tco_4 = len;
                var _tco_5 = Enumerable.Concat(acc, new List<CodexType>() { fr.var_type }).ToList();
                st = _tco_0;
                env = _tco_1;
                @params = _tco_2;
                i = _tco_3;
                len = _tco_4;
                acc = _tco_5;
                continue;
            }
        }
    }

    public static CodexType wrap_fun_type(List<CodexType> param_types, CodexType result)
    {
        return wrap_fun_type_loop(param_types, result, (((long)param_types.Count) - 1L));
    }

    public static CodexType wrap_fun_type_loop(List<CodexType> param_types, CodexType result, long i)
    {
        while (true)
        {
            if ((i < 0L))
            {
                return result;
            }
            else
            {
                var _tco_0 = param_types;
                var _tco_1 = new FunTy(param_types[(int)i], result);
                var _tco_2 = (i - 1L);
                param_types = _tco_0;
                result = _tco_1;
                i = _tco_2;
                continue;
            }
        }
    }

    public static CheckResult infer_application(UnificationState st, TypeEnv env, AExpr func, AExpr arg)
    {
        return ((Func<CheckResult, CheckResult>)((fr) => ((Func<CheckResult, CheckResult>)((ar) => ((Func<FreshResult, CheckResult>)((ret) => ((Func<UnifyResult, CheckResult>)((r) => new CheckResult(ret.var_type, r.state)))(unify(ret.state, fr.inferred_type, new FunTy(ar.inferred_type, ret.var_type)))))(fresh_and_advance(ar.state))))(infer_expr(fr.state, env, arg))))(infer_expr(st, env, func));
    }

    public static CheckResult infer_list(UnificationState st, TypeEnv env, List<AExpr> elems)
    {
        return ((((long)elems.Count) == 0L) ? ((Func<FreshResult, CheckResult>)((fr) => new CheckResult(new ListTy(fr.var_type), fr.state)))(fresh_and_advance(st)) : ((Func<CheckResult, CheckResult>)((first) => ((Func<UnificationState, CheckResult>)((st2) => new CheckResult(new ListTy(first.inferred_type), st2)))(unify_list_elems(first.state, env, elems, first.inferred_type, 1L, ((long)elems.Count)))))(infer_expr(st, env, elems[(int)0L])));
    }

    public static UnificationState unify_list_elems(UnificationState st, TypeEnv env, List<AExpr> elems, CodexType elem_ty, long i, long len)
    {
        while (true)
        {
            if ((i == len))
            {
                return st;
            }
            else
            {
                var er = infer_expr(st, env, elems[(int)i]);
                var r = unify(er.state, er.inferred_type, elem_ty);
                var _tco_0 = r.state;
                var _tco_1 = env;
                var _tco_2 = elems;
                var _tco_3 = elem_ty;
                var _tco_4 = (i + 1L);
                var _tco_5 = len;
                st = _tco_0;
                env = _tco_1;
                elems = _tco_2;
                elem_ty = _tco_3;
                i = _tco_4;
                len = _tco_5;
                continue;
            }
        }
    }

    public static CheckResult infer_match(UnificationState st, TypeEnv env, AExpr scrutinee, List<AMatchArm> arms)
    {
        return ((Func<CheckResult, CheckResult>)((sr) => ((Func<FreshResult, CheckResult>)((fr) => ((Func<UnificationState, CheckResult>)((st2) => new CheckResult(fr.var_type, st2)))(infer_match_arms(fr.state, env, sr.inferred_type, fr.var_type, arms, 0L, ((long)arms.Count)))))(fresh_and_advance(sr.state))))(infer_expr(st, env, scrutinee));
    }

    public static UnificationState infer_match_arms(UnificationState st, TypeEnv env, CodexType scrut_ty, CodexType result_ty, List<AMatchArm> arms, long i, long len)
    {
        while (true)
        {
            if ((i == len))
            {
                return st;
            }
            else
            {
                var arm = arms[(int)i];
                var pr = bind_pattern(st, env, arm.pattern, scrut_ty);
                var br = infer_expr(pr.state, pr.env, arm.body);
                var r = unify(br.state, br.inferred_type, result_ty);
                var _tco_0 = r.state;
                var _tco_1 = env;
                var _tco_2 = scrut_ty;
                var _tco_3 = result_ty;
                var _tco_4 = arms;
                var _tco_5 = (i + 1L);
                var _tco_6 = len;
                st = _tco_0;
                env = _tco_1;
                scrut_ty = _tco_2;
                result_ty = _tco_3;
                arms = _tco_4;
                i = _tco_5;
                len = _tco_6;
                continue;
            }
        }
    }

    public static PatBindResult bind_pattern(UnificationState st, TypeEnv env, APat pat, CodexType ty)
    {
        return ((Func<APat, PatBindResult>)((_scrutinee80_) => (_scrutinee80_ is AVarPat _mAVarPat80_ ? ((Func<Name, PatBindResult>)((name) => new PatBindResult(st, env_bind(env, name.value, ty))))((Name)_mAVarPat80_.Field0) : (_scrutinee80_ is AWildPat _mAWildPat80_ ? new PatBindResult(st, env) : (_scrutinee80_ is ALitPat _mALitPat80_ ? ((Func<LiteralKind, PatBindResult>)((kind) => ((Func<string, PatBindResult>)((val) => new PatBindResult(st, env)))((string)_mALitPat80_.Field0)))((LiteralKind)_mALitPat80_.Field1) : (_scrutinee80_ is ACtorPat _mACtorPat80_ ? ((Func<List<APat>, PatBindResult>)((sub_pats) => ((Func<Name, PatBindResult>)((ctor_name) => ((Func<FreshResult, PatBindResult>)((ctor_lookup) => bind_ctor_sub_patterns(ctor_lookup.state, env, sub_pats, ctor_lookup.var_type, 0L, ((long)sub_pats.Count))))(instantiate_type(st, env_lookup(env, ctor_name.value)))))((Name)_mACtorPat80_.Field0)))((List<APat>)_mACtorPat80_.Field1) : throw new InvalidOperationException("Non-exhaustive match")))))))(pat);
    }

    public static PatBindResult bind_ctor_sub_patterns(UnificationState st, TypeEnv env, List<APat> sub_pats, CodexType ctor_ty, long i, long len)
    {
        while (true)
        {
            if ((i == len))
            {
                return new PatBindResult(st, env);
            }
            else
            {
                var _tco_s = ctor_ty;
                if (_tco_s is FunTy _tco_m0)
                {
                    var param_ty = _tco_m0.Field0;
                    var ret_ty = _tco_m0.Field1;
                    var pr = bind_pattern(st, env, sub_pats[(int)i], param_ty);
                    var _tco_0 = pr.state;
                    var _tco_1 = pr.env;
                    var _tco_2 = sub_pats;
                    var _tco_3 = ret_ty;
                    var _tco_4 = (i + 1L);
                    var _tco_5 = len;
                    st = _tco_0;
                    env = _tco_1;
                    sub_pats = _tco_2;
                    ctor_ty = _tco_3;
                    i = _tco_4;
                    len = _tco_5;
                    continue;
                }
                {
                    var _ = _tco_s;
                    var fr = fresh_and_advance(st);
                    var pr = bind_pattern(fr.state, env, sub_pats[(int)i], fr.var_type);
                    var _tco_0 = pr.state;
                    var _tco_1 = pr.env;
                    var _tco_2 = sub_pats;
                    var _tco_3 = ctor_ty;
                    var _tco_4 = (i + 1L);
                    var _tco_5 = len;
                    st = _tco_0;
                    env = _tco_1;
                    sub_pats = _tco_2;
                    ctor_ty = _tco_3;
                    i = _tco_4;
                    len = _tco_5;
                    continue;
                }
            }
        }
    }

    public static CheckResult infer_do(UnificationState st, TypeEnv env, List<ADoStmt> stmts)
    {
        return infer_do_loop(st, env, stmts, 0L, ((long)stmts.Count), new NothingTy());
    }

    public static CheckResult infer_do_loop(UnificationState st, TypeEnv env, List<ADoStmt> stmts, long i, long len, CodexType last_ty)
    {
        while (true)
        {
            if ((i == len))
            {
                return new CheckResult(last_ty, st);
            }
            else
            {
                var stmt = stmts[(int)i];
                var _tco_s = stmt;
                if (_tco_s is ADoExprStmt _tco_m0)
                {
                    var e = _tco_m0.Field0;
                    var er = infer_expr(st, env, e);
                    var _tco_0 = er.state;
                    var _tco_1 = env;
                    var _tco_2 = stmts;
                    var _tco_3 = (i + 1L);
                    var _tco_4 = len;
                    var _tco_5 = er.inferred_type;
                    st = _tco_0;
                    env = _tco_1;
                    stmts = _tco_2;
                    i = _tco_3;
                    len = _tco_4;
                    last_ty = _tco_5;
                    continue;
                }
                else if (_tco_s is ADoBindStmt _tco_m1)
                {
                    var name = _tco_m1.Field0;
                    var e = _tco_m1.Field1;
                    var er = infer_expr(st, env, e);
                    var env2 = env_bind(env, name.value, er.inferred_type);
                    var _tco_0 = er.state;
                    var _tco_1 = env2;
                    var _tco_2 = stmts;
                    var _tco_3 = (i + 1L);
                    var _tco_4 = len;
                    var _tco_5 = er.inferred_type;
                    st = _tco_0;
                    env = _tco_1;
                    stmts = _tco_2;
                    i = _tco_3;
                    len = _tco_4;
                    last_ty = _tco_5;
                    continue;
                }
            }
        }
    }

    public static CheckResult infer_expr(UnificationState st, TypeEnv env, AExpr expr)
    {
        return ((Func<AExpr, CheckResult>)((_scrutinee81_) => (_scrutinee81_ is ALitExpr _mALitExpr81_ ? ((Func<LiteralKind, CheckResult>)((kind) => ((Func<string, CheckResult>)((val) => infer_literal(st, kind)))((string)_mALitExpr81_.Field0)))((LiteralKind)_mALitExpr81_.Field1) : (_scrutinee81_ is ANameExpr _mANameExpr81_ ? ((Func<Name, CheckResult>)((name) => infer_name(st, env, name.value)))((Name)_mANameExpr81_.Field0) : (_scrutinee81_ is ABinaryExpr _mABinaryExpr81_ ? ((Func<AExpr, CheckResult>)((right) => ((Func<BinaryOp, CheckResult>)((op) => ((Func<AExpr, CheckResult>)((left) => infer_binary(st, env, left, op, right)))((AExpr)_mABinaryExpr81_.Field0)))((BinaryOp)_mABinaryExpr81_.Field1)))((AExpr)_mABinaryExpr81_.Field2) : (_scrutinee81_ is AUnaryExpr _mAUnaryExpr81_ ? ((Func<AExpr, CheckResult>)((operand) => ((Func<CheckResult, CheckResult>)((r) => ((Func<UnifyResult, CheckResult>)((u) => new CheckResult(new IntegerTy(), u.state)))(unify(r.state, r.inferred_type, new IntegerTy()))))(infer_expr(st, env, operand))))((AExpr)_mAUnaryExpr81_.Field0) : (_scrutinee81_ is AApplyExpr _mAApplyExpr81_ ? ((Func<AExpr, CheckResult>)((arg) => ((Func<AExpr, CheckResult>)((func) => infer_application(st, env, func, arg)))((AExpr)_mAApplyExpr81_.Field0)))((AExpr)_mAApplyExpr81_.Field1) : (_scrutinee81_ is AIfExpr _mAIfExpr81_ ? ((Func<AExpr, CheckResult>)((else_e) => ((Func<AExpr, CheckResult>)((then_e) => ((Func<AExpr, CheckResult>)((cond) => infer_if(st, env, cond, then_e, else_e)))((AExpr)_mAIfExpr81_.Field0)))((AExpr)_mAIfExpr81_.Field1)))((AExpr)_mAIfExpr81_.Field2) : (_scrutinee81_ is ALetExpr _mALetExpr81_ ? ((Func<AExpr, CheckResult>)((body) => ((Func<List<ALetBind>, CheckResult>)((bindings) => infer_let(st, env, bindings, body)))((List<ALetBind>)_mALetExpr81_.Field0)))((AExpr)_mALetExpr81_.Field1) : (_scrutinee81_ is ALambdaExpr _mALambdaExpr81_ ? ((Func<AExpr, CheckResult>)((body) => ((Func<List<Name>, CheckResult>)((@params) => infer_lambda(st, env, @params, body)))((List<Name>)_mALambdaExpr81_.Field0)))((AExpr)_mALambdaExpr81_.Field1) : (_scrutinee81_ is AMatchExpr _mAMatchExpr81_ ? ((Func<List<AMatchArm>, CheckResult>)((arms) => ((Func<AExpr, CheckResult>)((scrutinee) => infer_match(st, env, scrutinee, arms)))((AExpr)_mAMatchExpr81_.Field0)))((List<AMatchArm>)_mAMatchExpr81_.Field1) : (_scrutinee81_ is AListExpr _mAListExpr81_ ? ((Func<List<AExpr>, CheckResult>)((elems) => infer_list(st, env, elems)))((List<AExpr>)_mAListExpr81_.Field0) : (_scrutinee81_ is ADoExpr _mADoExpr81_ ? ((Func<List<ADoStmt>, CheckResult>)((stmts) => infer_do(st, env, stmts)))((List<ADoStmt>)_mADoExpr81_.Field0) : (_scrutinee81_ is AFieldAccess _mAFieldAccess81_ ? ((Func<Name, CheckResult>)((field) => ((Func<AExpr, CheckResult>)((obj) => ((Func<CheckResult, CheckResult>)((r) => ((Func<FreshResult, CheckResult>)((fr) => new CheckResult(fr.var_type, fr.state)))(fresh_and_advance(r.state))))(infer_expr(st, env, obj))))((AExpr)_mAFieldAccess81_.Field0)))((Name)_mAFieldAccess81_.Field1) : (_scrutinee81_ is ARecordExpr _mARecordExpr81_ ? ((Func<List<AFieldExpr>, CheckResult>)((fields) => ((Func<Name, CheckResult>)((name) => ((Func<UnificationState, CheckResult>)((st2) => new CheckResult(new ConstructedTy(name, new List<CodexType>()), st2)))(infer_record_fields(st, env, fields, 0L, ((long)fields.Count)))))((Name)_mARecordExpr81_.Field0)))((List<AFieldExpr>)_mARecordExpr81_.Field1) : (_scrutinee81_ is AErrorExpr _mAErrorExpr81_ ? ((Func<string, CheckResult>)((msg) => new CheckResult(new ErrorTy(), st)))((string)_mAErrorExpr81_.Field0) : throw new InvalidOperationException("Non-exhaustive match")))))))))))))))))(expr);
    }

    public static UnificationState infer_record_fields(UnificationState st, TypeEnv env, List<AFieldExpr> fields, long i, long len)
    {
        while (true)
        {
            if ((i == len))
            {
                return st;
            }
            else
            {
                var f = fields[(int)i];
                var r = infer_expr(st, env, f.value);
                var _tco_0 = r.state;
                var _tco_1 = env;
                var _tco_2 = fields;
                var _tco_3 = (i + 1L);
                var _tco_4 = len;
                st = _tco_0;
                env = _tco_1;
                fields = _tco_2;
                i = _tco_3;
                len = _tco_4;
                continue;
            }
        }
    }

    public static CodexType resolve_type_expr(ATypeExpr texpr)
    {
        return ((Func<ATypeExpr, CodexType>)((_scrutinee82_) => (_scrutinee82_ is ANamedType _mANamedType82_ ? ((Func<Name, CodexType>)((name) => resolve_type_name(name.value)))((Name)_mANamedType82_.Field0) : (_scrutinee82_ is AFunType _mAFunType82_ ? ((Func<ATypeExpr, CodexType>)((ret) => ((Func<ATypeExpr, CodexType>)((param) => new FunTy(resolve_type_expr(param), resolve_type_expr(ret))))((ATypeExpr)_mAFunType82_.Field0)))((ATypeExpr)_mAFunType82_.Field1) : (_scrutinee82_ is AAppType _mAAppType82_ ? ((Func<List<ATypeExpr>, CodexType>)((args) => ((Func<ATypeExpr, CodexType>)((ctor) => resolve_applied_type(ctor, args)))((ATypeExpr)_mAAppType82_.Field0)))((List<ATypeExpr>)_mAAppType82_.Field1) : throw new InvalidOperationException("Non-exhaustive match"))))))(texpr);
    }

    public static CodexType resolve_applied_type(ATypeExpr ctor, List<ATypeExpr> args)
    {
        return (ctor is ANamedType _mANamedType83_ ? ((Func<Name, CodexType>)((name) => ((name.value == "List") ? ((((long)args.Count) == 1L) ? new ListTy(resolve_type_expr(args[(int)0L])) : new ListTy(new ErrorTy())) : new ConstructedTy(name, resolve_type_expr_list(args, 0L, ((long)args.Count), new List<CodexType>())))))((Name)_mANamedType83_.Field0) : ((Func<ATypeExpr, CodexType>)((_) => resolve_type_expr(ctor)))(ctor));
    }

    public static List<CodexType> resolve_type_expr_list(List<ATypeExpr> args, long i, long len, List<CodexType> acc)
    {
        while (true)
        {
            if ((i == len))
            {
                return acc;
            }
            else
            {
                var _tco_0 = args;
                var _tco_1 = (i + 1L);
                var _tco_2 = len;
                var _tco_3 = Enumerable.Concat(acc, new List<CodexType>() { resolve_type_expr(args[(int)i]) }).ToList();
                args = _tco_0;
                i = _tco_1;
                len = _tco_2;
                acc = _tco_3;
                continue;
            }
        }
    }

    public static CodexType resolve_type_name(string name)
    {
        return ((name == "Integer") ? new IntegerTy() : ((name == "Number") ? new NumberTy() : ((name == "Text") ? new TextTy() : ((name == "Boolean") ? new BooleanTy() : ((name == "Nothing") ? new NothingTy() : new ConstructedTy(new Name(name), new List<CodexType>()))))));
    }

    public static CheckResult check_def(UnificationState st, TypeEnv env, ADef def)
    {
        return ((Func<DefSetup, CheckResult>)((declared) => ((Func<DefParamResult, CheckResult>)((env2) => ((Func<CheckResult, CheckResult>)((body_r) => ((Func<UnifyResult, CheckResult>)((u) => new CheckResult(declared.expected_type, u.state)))(unify(body_r.state, env2.remaining_type, body_r.inferred_type))))(infer_expr(env2.state, env2.env, def.body))))(bind_def_params(declared.state, declared.env, def.@params, declared.expected_type, 0L, ((long)def.@params.Count)))))(resolve_declared_type(st, env, def));
    }

    public static DefSetup resolve_declared_type(UnificationState st, TypeEnv env, ADef def)
    {
        return ((((long)def.declared_type.Count) == 0L) ? ((Func<FreshResult, DefSetup>)((fr) => new DefSetup(fr.var_type, fr.var_type, fr.state, env)))(fresh_and_advance(st)) : ((Func<CodexType, DefSetup>)((ty) => new DefSetup(ty, ty, st, env)))(resolve_type_expr(def.declared_type[(int)0L])));
    }

    public static DefParamResult bind_def_params(UnificationState st, TypeEnv env, List<AParam> @params, CodexType remaining, long i, long len)
    {
        while (true)
        {
            if ((i == len))
            {
                return new DefParamResult(st, env, remaining);
            }
            else
            {
                var p = @params[(int)i];
                var _tco_s = remaining;
                if (_tco_s is FunTy _tco_m0)
                {
                    var param_ty = _tco_m0.Field0;
                    var ret_ty = _tco_m0.Field1;
                    var env2 = env_bind(env, p.name.value, param_ty);
                    var _tco_0 = st;
                    var _tco_1 = env2;
                    var _tco_2 = @params;
                    var _tco_3 = ret_ty;
                    var _tco_4 = (i + 1L);
                    var _tco_5 = len;
                    st = _tco_0;
                    env = _tco_1;
                    @params = _tco_2;
                    remaining = _tco_3;
                    i = _tco_4;
                    len = _tco_5;
                    continue;
                }
                {
                    var _ = _tco_s;
                    var fr = fresh_and_advance(st);
                    var env2 = env_bind(env, p.name.value, fr.var_type);
                    var _tco_0 = fr.state;
                    var _tco_1 = env2;
                    var _tco_2 = @params;
                    var _tco_3 = remaining;
                    var _tco_4 = (i + 1L);
                    var _tco_5 = len;
                    st = _tco_0;
                    env = _tco_1;
                    @params = _tco_2;
                    remaining = _tco_3;
                    i = _tco_4;
                    len = _tco_5;
                    continue;
                }
            }
        }
    }

    public static ModuleResult check_module(AModule mod)
    {
        return ((Func<LetBindResult, ModuleResult>)((tenv) => ((Func<LetBindResult, ModuleResult>)((env) => check_all_defs(env.state, env.env, mod.defs, 0L, ((long)mod.defs.Count), new List<TypeBinding>())))(register_all_defs(tenv.state, tenv.env, mod.defs, 0L, ((long)mod.defs.Count)))))(register_type_defs(empty_unification_state(), builtin_type_env(), mod.type_defs, 0L, ((long)mod.type_defs.Count)));
    }

    public static LetBindResult register_all_defs(UnificationState st, TypeEnv env, List<ADef> defs, long i, long len)
    {
        while (true)
        {
            if ((i == len))
            {
                return new LetBindResult(st, env);
            }
            else
            {
                var def = defs[(int)i];
                var ty = ((((long)def.declared_type.Count) == 0L) ? ((Func<FreshResult, LetBindResult>)((fr) => ((Func<TypeEnv, LetBindResult>)((env2) => new LetBindResult(fr.state, env2)))(env_bind(env, def.name.value, fr.var_type))))(fresh_and_advance(st)) : ((Func<CodexType, LetBindResult>)((resolved) => new LetBindResult(st, env_bind(env, def.name.value, resolved))))(resolve_type_expr(def.declared_type[(int)0L])));
                var _tco_0 = ty.state;
                var _tco_1 = ty.env;
                var _tco_2 = defs;
                var _tco_3 = (i + 1L);
                var _tco_4 = len;
                st = _tco_0;
                env = _tco_1;
                defs = _tco_2;
                i = _tco_3;
                len = _tco_4;
                continue;
            }
        }
    }

    public static ModuleResult check_all_defs(UnificationState st, TypeEnv env, List<ADef> defs, long i, long len, List<TypeBinding> acc)
    {
        while (true)
        {
            if ((i == len))
            {
                return new ModuleResult(acc, st);
            }
            else
            {
                var def = defs[(int)i];
                var r = check_def(st, env, def);
                var resolved = deep_resolve(r.state, r.inferred_type);
                var entry = new TypeBinding(def.name.value, resolved);
                var _tco_0 = r.state;
                var _tco_1 = env;
                var _tco_2 = defs;
                var _tco_3 = (i + 1L);
                var _tco_4 = len;
                var _tco_5 = Enumerable.Concat(acc, new List<TypeBinding>() { entry }).ToList();
                st = _tco_0;
                env = _tco_1;
                defs = _tco_2;
                i = _tco_3;
                len = _tco_4;
                acc = _tco_5;
                continue;
            }
        }
    }

    public static LetBindResult register_type_defs(UnificationState st, TypeEnv env, List<ATypeDef> tdefs, long i, long len)
    {
        while (true)
        {
            if ((i == len))
            {
                return new LetBindResult(st, env);
            }
            else
            {
                var td = tdefs[(int)i];
                var r = register_one_type_def(st, env, td);
                var _tco_0 = r.state;
                var _tco_1 = r.env;
                var _tco_2 = tdefs;
                var _tco_3 = (i + 1L);
                var _tco_4 = len;
                st = _tco_0;
                env = _tco_1;
                tdefs = _tco_2;
                i = _tco_3;
                len = _tco_4;
                continue;
            }
        }
    }

    public static LetBindResult register_one_type_def(UnificationState st, TypeEnv env, ATypeDef td)
    {
        return ((Func<ATypeDef, LetBindResult>)((_scrutinee84_) => (_scrutinee84_ is AVariantTypeDef _mAVariantTypeDef84_ ? ((Func<List<AVariantCtorDef>, LetBindResult>)((ctors) => ((Func<List<Name>, LetBindResult>)((type_params) => ((Func<Name, LetBindResult>)((name) => ((Func<CodexType, LetBindResult>)((result_ty) => register_variant_ctors(st, env, ctors, result_ty, 0L, ((long)ctors.Count))))(new ConstructedTy(name, new List<CodexType>()))))((Name)_mAVariantTypeDef84_.Field0)))((List<Name>)_mAVariantTypeDef84_.Field1)))((List<AVariantCtorDef>)_mAVariantTypeDef84_.Field2) : (_scrutinee84_ is ARecordTypeDef _mARecordTypeDef84_ ? ((Func<List<ARecordFieldDef>, LetBindResult>)((fields) => ((Func<List<Name>, LetBindResult>)((type_params) => ((Func<Name, LetBindResult>)((name) => ((Func<CodexType, LetBindResult>)((result_ty) => ((Func<CodexType, LetBindResult>)((ctor_ty) => new LetBindResult(st, env_bind(env, name.value, ctor_ty))))(build_record_ctor_type(fields, result_ty, 0L, ((long)fields.Count)))))(new ConstructedTy(name, new List<CodexType>()))))((Name)_mARecordTypeDef84_.Field0)))((List<Name>)_mARecordTypeDef84_.Field1)))((List<ARecordFieldDef>)_mARecordTypeDef84_.Field2) : throw new InvalidOperationException("Non-exhaustive match")))))(td);
    }

    public static LetBindResult register_variant_ctors(UnificationState st, TypeEnv env, List<AVariantCtorDef> ctors, CodexType result_ty, long i, long len)
    {
        while (true)
        {
            if ((i == len))
            {
                return new LetBindResult(st, env);
            }
            else
            {
                var ctor = ctors[(int)i];
                var ctor_ty = build_ctor_type(ctor.fields, result_ty, 0L, ((long)ctor.fields.Count));
                var env2 = env_bind(env, ctor.name.value, ctor_ty);
                var _tco_0 = st;
                var _tco_1 = env2;
                var _tco_2 = ctors;
                var _tco_3 = result_ty;
                var _tco_4 = (i + 1L);
                var _tco_5 = len;
                st = _tco_0;
                env = _tco_1;
                ctors = _tco_2;
                result_ty = _tco_3;
                i = _tco_4;
                len = _tco_5;
                continue;
            }
        }
    }

    public static CodexType build_ctor_type(List<ATypeExpr> fields, CodexType result, long i, long len)
    {
        return ((i == len) ? result : ((Func<CodexType, CodexType>)((rest) => new FunTy(resolve_type_expr(fields[(int)i]), rest)))(build_ctor_type(fields, result, (i + 1L), len)));
    }

    public static CodexType build_record_ctor_type(List<ARecordFieldDef> fields, CodexType result, long i, long len)
    {
        return ((i == len) ? result : ((Func<ARecordFieldDef, CodexType>)((f) => ((Func<CodexType, CodexType>)((rest) => new FunTy(resolve_type_expr(f.type_expr), rest)))(build_record_ctor_type(fields, result, (i + 1L), len))))(fields[(int)i]));
    }

    public static TypeEnv empty_type_env()
    {
        return new TypeEnv(new List<TypeBinding>());
    }

    public static CodexType env_lookup(TypeEnv env, string name)
    {
        return env_lookup_loop(env.bindings, name, 0L, ((long)env.bindings.Count));
    }

    public static CodexType env_lookup_loop(List<TypeBinding> bindings, string name, long i, long len)
    {
        while (true)
        {
            if ((i == len))
            {
                return new ErrorTy();
            }
            else
            {
                var b = bindings[(int)i];
                if ((b.name == name))
                {
                    return b.bound_type;
                }
                else
                {
                    var _tco_0 = bindings;
                    var _tco_1 = name;
                    var _tco_2 = (i + 1L);
                    var _tco_3 = len;
                    bindings = _tco_0;
                    name = _tco_1;
                    i = _tco_2;
                    len = _tco_3;
                    continue;
                }
            }
        }
    }

    public static bool env_has(TypeEnv env, string name)
    {
        return env_has_loop(env.bindings, name, 0L, ((long)env.bindings.Count));
    }

    public static bool env_has_loop(List<TypeBinding> bindings, string name, long i, long len)
    {
        while (true)
        {
            if ((i == len))
            {
                return false;
            }
            else
            {
                var b = bindings[(int)i];
                if ((b.name == name))
                {
                    return true;
                }
                else
                {
                    var _tco_0 = bindings;
                    var _tco_1 = name;
                    var _tco_2 = (i + 1L);
                    var _tco_3 = len;
                    bindings = _tco_0;
                    name = _tco_1;
                    i = _tco_2;
                    len = _tco_3;
                    continue;
                }
            }
        }
    }

    public static TypeEnv env_bind(TypeEnv env, string name, CodexType ty)
    {
        return new TypeEnv(Enumerable.Concat(new List<TypeBinding>() { new TypeBinding(name, ty) }, env.bindings).ToList());
    }

    public static TypeEnv builtin_type_env()
    {
        return ((Func<TypeEnv, TypeEnv>)((e) => ((Func<TypeEnv, TypeEnv>)((e2) => ((Func<TypeEnv, TypeEnv>)((e3) => ((Func<TypeEnv, TypeEnv>)((e4) => ((Func<TypeEnv, TypeEnv>)((e5) => ((Func<TypeEnv, TypeEnv>)((e6) => ((Func<TypeEnv, TypeEnv>)((e7) => ((Func<TypeEnv, TypeEnv>)((e8) => ((Func<TypeEnv, TypeEnv>)((e9) => ((Func<TypeEnv, TypeEnv>)((e10) => ((Func<TypeEnv, TypeEnv>)((e11) => ((Func<TypeEnv, TypeEnv>)((e12) => ((Func<TypeEnv, TypeEnv>)((e13) => ((Func<TypeEnv, TypeEnv>)((e14) => ((Func<TypeEnv, TypeEnv>)((e15) => ((Func<TypeEnv, TypeEnv>)((e16) => ((Func<TypeEnv, TypeEnv>)((e17) => ((Func<TypeEnv, TypeEnv>)((e18) => ((Func<TypeEnv, TypeEnv>)((e19) => ((Func<TypeEnv, TypeEnv>)((e20) => ((Func<TypeEnv, TypeEnv>)((e21) => e21))(env_bind(e20, "read-line", new TextTy()))))(env_bind(e19, "fold", new ForAllTy(0L, new ForAllTy(1L, new FunTy(new FunTy(new TypeVar(1L), new FunTy(new TypeVar(0L), new TypeVar(1L))), new FunTy(new TypeVar(1L), new FunTy(new ListTy(new TypeVar(0L)), new TypeVar(1L))))))))))(env_bind(e18, "filter", new ForAllTy(0L, new FunTy(new FunTy(new TypeVar(0L), new BooleanTy()), new FunTy(new ListTy(new TypeVar(0L)), new ListTy(new TypeVar(0L)))))))))(env_bind(e17, "map", new ForAllTy(0L, new ForAllTy(1L, new FunTy(new FunTy(new TypeVar(0L), new TypeVar(1L)), new FunTy(new ListTy(new TypeVar(0L)), new ListTy(new TypeVar(1L))))))))))(env_bind(e16, "list-at", new ForAllTy(0L, new FunTy(new ListTy(new TypeVar(0L)), new FunTy(new IntegerTy(), new TypeVar(0L))))))))(env_bind(e15, "list-length", new ForAllTy(0L, new FunTy(new ListTy(new TypeVar(0L)), new IntegerTy()))))))(env_bind(e14, "print-line", new FunTy(new TextTy(), new NothingTy())))))(env_bind(e13, "show", new ForAllTy(0L, new FunTy(new TypeVar(0L), new TextTy()))))))(env_bind(e12, "text-to-integer", new FunTy(new TextTy(), new IntegerTy())))))(env_bind(e11, "text-replace", new FunTy(new TextTy(), new FunTy(new TextTy(), new FunTy(new TextTy(), new TextTy())))))))(env_bind(e10, "code-to-char", new FunTy(new IntegerTy(), new TextTy())))))(env_bind(e9, "char-code", new FunTy(new TextTy(), new IntegerTy())))))(env_bind(e8, "is-whitespace", new FunTy(new TextTy(), new BooleanTy())))))(env_bind(e7, "is-digit", new FunTy(new TextTy(), new BooleanTy())))))(env_bind(e6, "is-letter", new FunTy(new TextTy(), new BooleanTy())))))(env_bind(e5, "substring", new FunTy(new TextTy(), new FunTy(new IntegerTy(), new FunTy(new IntegerTy(), new TextTy())))))))(env_bind(e4, "char-at", new FunTy(new TextTy(), new FunTy(new IntegerTy(), new TextTy()))))))(env_bind(e3, "integer-to-text", new FunTy(new IntegerTy(), new TextTy())))))(env_bind(e2, "text-length", new FunTy(new TextTy(), new IntegerTy())))))(env_bind(e, "negate", new FunTy(new IntegerTy(), new IntegerTy())))))(empty_type_env());
    }

    public static UnificationState empty_unification_state()
    {
        return new UnificationState(new List<SubstEntry>(), 0L, new List<Diagnostic>());
    }

    public static CodexType fresh_var(UnificationState st)
    {
        return new TypeVar(st.next_id);
    }

    public static UnificationState advance_id(UnificationState st)
    {
        return new UnificationState(st.substitutions, (st.next_id + 1L), st.errors);
    }

    public static FreshResult fresh_and_advance(UnificationState st)
    {
        return new FreshResult(new TypeVar(st.next_id), advance_id(st));
    }

    public static CodexType subst_lookup(long var_id, List<SubstEntry> entries)
    {
        return subst_lookup_loop(var_id, entries, 0L, ((long)entries.Count));
    }

    public static CodexType subst_lookup_loop(long var_id, List<SubstEntry> entries, long i, long len)
    {
        while (true)
        {
            if ((i == len))
            {
                return new ErrorTy();
            }
            else
            {
                var entry = entries[(int)i];
                if ((entry.var_id == var_id))
                {
                    return entry.resolved_type;
                }
                else
                {
                    var _tco_0 = var_id;
                    var _tco_1 = entries;
                    var _tco_2 = (i + 1L);
                    var _tco_3 = len;
                    var_id = _tco_0;
                    entries = _tco_1;
                    i = _tco_2;
                    len = _tco_3;
                    continue;
                }
            }
        }
    }

    public static bool has_subst(long var_id, List<SubstEntry> entries)
    {
        return has_subst_loop(var_id, entries, 0L, ((long)entries.Count));
    }

    public static bool has_subst_loop(long var_id, List<SubstEntry> entries, long i, long len)
    {
        while (true)
        {
            if ((i == len))
            {
                return false;
            }
            else
            {
                var entry = entries[(int)i];
                if ((entry.var_id == var_id))
                {
                    return true;
                }
                else
                {
                    var _tco_0 = var_id;
                    var _tco_1 = entries;
                    var _tco_2 = (i + 1L);
                    var _tco_3 = len;
                    var_id = _tco_0;
                    entries = _tco_1;
                    i = _tco_2;
                    len = _tco_3;
                    continue;
                }
            }
        }
    }

    public static CodexType resolve(UnificationState st, CodexType ty)
    {
        while (true)
        {
            var _tco_s = ty;
            if (_tco_s is TypeVar _tco_m0)
            {
                var id = _tco_m0.Field0;
                if (has_subst(id, st.substitutions))
                {
                    var _tco_0 = st;
                    var _tco_1 = subst_lookup(id, st.substitutions);
                    st = _tco_0;
                    ty = _tco_1;
                    continue;
                }
                else
                {
                    return ty;
                }
            }
            {
                var _ = _tco_s;
                return ty;
            }
        }
    }

    public static UnificationState add_subst(UnificationState st, long var_id, CodexType ty)
    {
        return new UnificationState(Enumerable.Concat(st.substitutions, new List<SubstEntry>() { new SubstEntry(var_id, ty) }).ToList(), st.next_id, st.errors);
    }

    public static UnificationState add_unify_error(UnificationState st, string code, string msg)
    {
        return new UnificationState(st.substitutions, st.next_id, Enumerable.Concat(st.errors, new List<Diagnostic>() { make_error(code, msg) }).ToList());
    }

    public static bool occurs_in(UnificationState st, long var_id, CodexType ty)
    {
        while (true)
        {
            var resolved = resolve(st, ty);
            var _tco_s = resolved;
            if (_tco_s is TypeVar _tco_m0)
            {
                var id = _tco_m0.Field0;
                return (id == var_id);
            }
            else if (_tco_s is FunTy _tco_m1)
            {
                var param = _tco_m1.Field0;
                var ret = _tco_m1.Field1;
                return (occurs_in(st, var_id, param) || occurs_in(st, var_id, ret));
            }
            else if (_tco_s is ListTy _tco_m2)
            {
                var elem = _tco_m2.Field0;
                var _tco_0 = st;
                var _tco_1 = var_id;
                var _tco_2 = elem;
                st = _tco_0;
                var_id = _tco_1;
                ty = _tco_2;
                continue;
            }
            {
                var _ = _tco_s;
                return false;
            }
        }
    }

    public static UnifyResult unify(UnificationState st, CodexType a, CodexType b)
    {
        return ((Func<CodexType, UnifyResult>)((ra) => ((Func<CodexType, UnifyResult>)((rb) => unify_resolved(st, ra, rb)))(resolve(st, b))))(resolve(st, a));
    }

    public static UnifyResult unify_resolved(UnificationState st, CodexType a, CodexType b)
    {
        return (a is TypeVar _mTypeVar85_ ? ((Func<long, UnifyResult>)((id_a) => (occurs_in(st, id_a, b) ? new UnifyResult(false, add_unify_error(st, "CDX2010", "Infinite type")) : new UnifyResult(true, add_subst(st, id_a, b)))))((long)_mTypeVar85_.Field0) : ((Func<CodexType, UnifyResult>)((_) => unify_rhs(st, a, b)))(a));
    }

    public static UnifyResult unify_rhs(UnificationState st, CodexType a, CodexType b)
    {
        return (b is TypeVar _mTypeVar86_ ? ((Func<long, UnifyResult>)((id_b) => (occurs_in(st, id_b, a) ? new UnifyResult(false, add_unify_error(st, "CDX2010", "Infinite type")) : new UnifyResult(true, add_subst(st, id_b, a)))))((long)_mTypeVar86_.Field0) : ((Func<CodexType, UnifyResult>)((_) => unify_structural(st, a, b)))(b));
    }

    public static UnifyResult unify_structural(UnificationState st, CodexType a, CodexType b)
    {
        return ((Func<CodexType, UnifyResult>)((_scrutinee87_) => (_scrutinee87_ is IntegerTy _mIntegerTy87_ ? ((Func<CodexType, UnifyResult>)((_scrutinee88_) => (_scrutinee88_ is IntegerTy _mIntegerTy88_ ? new UnifyResult(true, st) : (_scrutinee88_ is ErrorTy _mErrorTy88_ ? new UnifyResult(true, st) : ((Func<CodexType, UnifyResult>)((_) => unify_mismatch(st, a, b)))(_scrutinee88_)))))(b) : (_scrutinee87_ is NumberTy _mNumberTy87_ ? ((Func<CodexType, UnifyResult>)((_scrutinee89_) => (_scrutinee89_ is NumberTy _mNumberTy89_ ? new UnifyResult(true, st) : (_scrutinee89_ is ErrorTy _mErrorTy89_ ? new UnifyResult(true, st) : ((Func<CodexType, UnifyResult>)((_) => unify_mismatch(st, a, b)))(_scrutinee89_)))))(b) : (_scrutinee87_ is TextTy _mTextTy87_ ? ((Func<CodexType, UnifyResult>)((_scrutinee90_) => (_scrutinee90_ is TextTy _mTextTy90_ ? new UnifyResult(true, st) : (_scrutinee90_ is ErrorTy _mErrorTy90_ ? new UnifyResult(true, st) : ((Func<CodexType, UnifyResult>)((_) => unify_mismatch(st, a, b)))(_scrutinee90_)))))(b) : (_scrutinee87_ is BooleanTy _mBooleanTy87_ ? ((Func<CodexType, UnifyResult>)((_scrutinee91_) => (_scrutinee91_ is BooleanTy _mBooleanTy91_ ? new UnifyResult(true, st) : (_scrutinee91_ is ErrorTy _mErrorTy91_ ? new UnifyResult(true, st) : ((Func<CodexType, UnifyResult>)((_) => unify_mismatch(st, a, b)))(_scrutinee91_)))))(b) : (_scrutinee87_ is NothingTy _mNothingTy87_ ? ((Func<CodexType, UnifyResult>)((_scrutinee92_) => (_scrutinee92_ is NothingTy _mNothingTy92_ ? new UnifyResult(true, st) : (_scrutinee92_ is ErrorTy _mErrorTy92_ ? new UnifyResult(true, st) : ((Func<CodexType, UnifyResult>)((_) => unify_mismatch(st, a, b)))(_scrutinee92_)))))(b) : (_scrutinee87_ is VoidTy _mVoidTy87_ ? ((Func<CodexType, UnifyResult>)((_scrutinee93_) => (_scrutinee93_ is VoidTy _mVoidTy93_ ? new UnifyResult(true, st) : (_scrutinee93_ is ErrorTy _mErrorTy93_ ? new UnifyResult(true, st) : ((Func<CodexType, UnifyResult>)((_) => unify_mismatch(st, a, b)))(_scrutinee93_)))))(b) : (_scrutinee87_ is ErrorTy _mErrorTy87_ ? new UnifyResult(true, st) : (_scrutinee87_ is FunTy _mFunTy87_ ? ((Func<CodexType, UnifyResult>)((ra) => ((Func<CodexType, UnifyResult>)((pa) => ((Func<CodexType, UnifyResult>)((_scrutinee94_) => (_scrutinee94_ is FunTy _mFunTy94_ ? ((Func<CodexType, UnifyResult>)((rb) => ((Func<CodexType, UnifyResult>)((pb) => unify_fun(st, pa, ra, pb, rb)))((CodexType)_mFunTy94_.Field0)))((CodexType)_mFunTy94_.Field1) : (_scrutinee94_ is ErrorTy _mErrorTy94_ ? new UnifyResult(true, st) : ((Func<CodexType, UnifyResult>)((_) => unify_mismatch(st, a, b)))(_scrutinee94_)))))(b)))((CodexType)_mFunTy87_.Field0)))((CodexType)_mFunTy87_.Field1) : (_scrutinee87_ is ListTy _mListTy87_ ? ((Func<CodexType, UnifyResult>)((ea) => ((Func<CodexType, UnifyResult>)((_scrutinee95_) => (_scrutinee95_ is ListTy _mListTy95_ ? ((Func<CodexType, UnifyResult>)((eb) => unify(st, ea, eb)))((CodexType)_mListTy95_.Field0) : (_scrutinee95_ is ErrorTy _mErrorTy95_ ? new UnifyResult(true, st) : ((Func<CodexType, UnifyResult>)((_) => unify_mismatch(st, a, b)))(_scrutinee95_)))))(b)))((CodexType)_mListTy87_.Field0) : (_scrutinee87_ is ConstructedTy _mConstructedTy87_ ? ((Func<List<CodexType>, UnifyResult>)((args_a) => ((Func<Name, UnifyResult>)((na) => ((Func<CodexType, UnifyResult>)((_scrutinee96_) => (_scrutinee96_ is ConstructedTy _mConstructedTy96_ ? ((Func<List<CodexType>, UnifyResult>)((args_b) => ((Func<Name, UnifyResult>)((nb) => ((na.value == nb.value) ? unify_constructed_args(st, args_a, args_b, 0L, ((long)args_a.Count)) : unify_mismatch(st, a, b))))((Name)_mConstructedTy96_.Field0)))((List<CodexType>)_mConstructedTy96_.Field1) : (_scrutinee96_ is SumTy _mSumTy96_ ? ((Func<List<SumCtor>, UnifyResult>)((sb_ctors) => ((Func<Name, UnifyResult>)((sb_name) => ((na.value == sb_name.value) ? new UnifyResult(true, st) : unify_mismatch(st, a, b))))((Name)_mSumTy96_.Field0)))((List<SumCtor>)_mSumTy96_.Field1) : (_scrutinee96_ is RecordTy _mRecordTy96_ ? ((Func<List<RecordField>, UnifyResult>)((rb_fields) => ((Func<Name, UnifyResult>)((rb_name) => ((na.value == rb_name.value) ? new UnifyResult(true, st) : unify_mismatch(st, a, b))))((Name)_mRecordTy96_.Field0)))((List<RecordField>)_mRecordTy96_.Field1) : (_scrutinee96_ is ErrorTy _mErrorTy96_ ? new UnifyResult(true, st) : ((Func<CodexType, UnifyResult>)((_) => unify_mismatch(st, a, b)))(_scrutinee96_)))))))(b)))((Name)_mConstructedTy87_.Field0)))((List<CodexType>)_mConstructedTy87_.Field1) : (_scrutinee87_ is SumTy _mSumTy87_ ? ((Func<List<SumCtor>, UnifyResult>)((sa_ctors) => ((Func<Name, UnifyResult>)((sa_name) => ((Func<CodexType, UnifyResult>)((_scrutinee97_) => (_scrutinee97_ is SumTy _mSumTy97_ ? ((Func<List<SumCtor>, UnifyResult>)((sb_ctors) => ((Func<Name, UnifyResult>)((sb_name) => ((sa_name.value == sb_name.value) ? new UnifyResult(true, st) : unify_mismatch(st, a, b))))((Name)_mSumTy97_.Field0)))((List<SumCtor>)_mSumTy97_.Field1) : (_scrutinee97_ is ConstructedTy _mConstructedTy97_ ? ((Func<List<CodexType>, UnifyResult>)((args_b) => ((Func<Name, UnifyResult>)((nb) => ((sa_name.value == nb.value) ? new UnifyResult(true, st) : unify_mismatch(st, a, b))))((Name)_mConstructedTy97_.Field0)))((List<CodexType>)_mConstructedTy97_.Field1) : (_scrutinee97_ is ErrorTy _mErrorTy97_ ? new UnifyResult(true, st) : ((Func<CodexType, UnifyResult>)((_) => unify_mismatch(st, a, b)))(_scrutinee97_))))))(b)))((Name)_mSumTy87_.Field0)))((List<SumCtor>)_mSumTy87_.Field1) : (_scrutinee87_ is RecordTy _mRecordTy87_ ? ((Func<List<RecordField>, UnifyResult>)((ra_fields) => ((Func<Name, UnifyResult>)((ra_name) => ((Func<CodexType, UnifyResult>)((_scrutinee98_) => (_scrutinee98_ is RecordTy _mRecordTy98_ ? ((Func<List<RecordField>, UnifyResult>)((rb_fields) => ((Func<Name, UnifyResult>)((rb_name) => ((ra_name.value == rb_name.value) ? new UnifyResult(true, st) : unify_mismatch(st, a, b))))((Name)_mRecordTy98_.Field0)))((List<RecordField>)_mRecordTy98_.Field1) : (_scrutinee98_ is ConstructedTy _mConstructedTy98_ ? ((Func<List<CodexType>, UnifyResult>)((args_b) => ((Func<Name, UnifyResult>)((nb) => ((ra_name.value == nb.value) ? new UnifyResult(true, st) : unify_mismatch(st, a, b))))((Name)_mConstructedTy98_.Field0)))((List<CodexType>)_mConstructedTy98_.Field1) : (_scrutinee98_ is ErrorTy _mErrorTy98_ ? new UnifyResult(true, st) : ((Func<CodexType, UnifyResult>)((_) => unify_mismatch(st, a, b)))(_scrutinee98_))))))(b)))((Name)_mRecordTy87_.Field0)))((List<RecordField>)_mRecordTy87_.Field1) : (_scrutinee87_ is ForAllTy _mForAllTy87_ ? ((Func<CodexType, UnifyResult>)((body) => ((Func<long, UnifyResult>)((id) => unify(st, body, b)))((long)_mForAllTy87_.Field0)))((CodexType)_mForAllTy87_.Field1) : ((Func<CodexType, UnifyResult>)((_) => (b is ErrorTy _mErrorTy99_ ? new UnifyResult(true, st) : ((Func<CodexType, UnifyResult>)((_) => unify_mismatch(st, a, b)))(b))))(_scrutinee87_))))))))))))))))(a);
    }

    public static UnifyResult unify_constructed_args(UnificationState st, List<CodexType> args_a, List<CodexType> args_b, long i, long len)
    {
        while (true)
        {
            if ((i == len))
            {
                return new UnifyResult(true, st);
            }
            else
            {
                if ((i >= ((long)args_b.Count)))
                {
                    return new UnifyResult(true, st);
                }
                else
                {
                    var r = unify(st, args_a[(int)i], args_b[(int)i]);
                    if (r.success)
                    {
                        var _tco_0 = r.state;
                        var _tco_1 = args_a;
                        var _tco_2 = args_b;
                        var _tco_3 = (i + 1L);
                        var _tco_4 = len;
                        st = _tco_0;
                        args_a = _tco_1;
                        args_b = _tco_2;
                        i = _tco_3;
                        len = _tco_4;
                        continue;
                    }
                    else
                    {
                        return r;
                    }
                }
            }
        }
    }

    public static UnifyResult unify_fun(UnificationState st, CodexType pa, CodexType ra, CodexType pb, CodexType rb)
    {
        return ((Func<UnifyResult, UnifyResult>)((r1) => (r1.success ? unify(r1.state, ra, rb) : r1)))(unify(st, pa, pb));
    }

    public static UnifyResult unify_mismatch(UnificationState st, CodexType a, CodexType b)
    {
        return new UnifyResult(false, add_unify_error(st, "CDX2001", "Type mismatch"));
    }

    public static CodexType deep_resolve(UnificationState st, CodexType ty)
    {
        return ((Func<CodexType, CodexType>)((resolved) => ((Func<CodexType, CodexType>)((_scrutinee100_) => (_scrutinee100_ is FunTy _mFunTy100_ ? ((Func<CodexType, CodexType>)((ret) => ((Func<CodexType, CodexType>)((param) => new FunTy(deep_resolve(st, param), deep_resolve(st, ret))))((CodexType)_mFunTy100_.Field0)))((CodexType)_mFunTy100_.Field1) : (_scrutinee100_ is ListTy _mListTy100_ ? ((Func<CodexType, CodexType>)((elem) => new ListTy(deep_resolve(st, elem))))((CodexType)_mListTy100_.Field0) : (_scrutinee100_ is ConstructedTy _mConstructedTy100_ ? ((Func<List<CodexType>, CodexType>)((args) => ((Func<Name, CodexType>)((name) => new ConstructedTy(name, deep_resolve_list(st, args, 0L, ((long)args.Count), new List<CodexType>()))))((Name)_mConstructedTy100_.Field0)))((List<CodexType>)_mConstructedTy100_.Field1) : (_scrutinee100_ is ForAllTy _mForAllTy100_ ? ((Func<CodexType, CodexType>)((body) => ((Func<long, CodexType>)((id) => new ForAllTy(id, deep_resolve(st, body))))((long)_mForAllTy100_.Field0)))((CodexType)_mForAllTy100_.Field1) : (_scrutinee100_ is SumTy _mSumTy100_ ? ((Func<List<SumCtor>, CodexType>)((ctors) => ((Func<Name, CodexType>)((name) => resolved))((Name)_mSumTy100_.Field0)))((List<SumCtor>)_mSumTy100_.Field1) : (_scrutinee100_ is RecordTy _mRecordTy100_ ? ((Func<List<RecordField>, CodexType>)((fields) => ((Func<Name, CodexType>)((name) => resolved))((Name)_mRecordTy100_.Field0)))((List<RecordField>)_mRecordTy100_.Field1) : ((Func<CodexType, CodexType>)((_) => resolved))(_scrutinee100_)))))))))(resolved)))(resolve(st, ty));
    }

    public static List<CodexType> deep_resolve_list(UnificationState st, List<CodexType> args, long i, long len, List<CodexType> acc)
    {
        while (true)
        {
            if ((i == len))
            {
                return acc;
            }
            else
            {
                var _tco_0 = st;
                var _tco_1 = args;
                var _tco_2 = (i + 1L);
                var _tco_3 = len;
                var _tco_4 = Enumerable.Concat(acc, new List<CodexType>() { deep_resolve(st, args[(int)i]) }).ToList();
                st = _tco_0;
                args = _tco_1;
                i = _tco_2;
                len = _tco_3;
                acc = _tco_4;
                continue;
            }
        }
    }

    public static string compile(string source, string module_name)
    {
        return ((Func<List<Token>, string>)((tokens) => ((Func<ParseState, string>)((st) => ((Func<Document, string>)((doc) => ((Func<AModule, string>)((ast) => ((Func<ModuleResult, string>)((check_result) => ((Func<IRModule, string>)((ir) => emit_full_module(ir, ast.type_defs)))(lower_module(ast, check_result.types, check_result.state))))(check_module(ast))))(desugar_document(doc, module_name))))(parse_document(st))))(make_parse_state(tokens))))(tokenize(source));
    }

    public static CompileResult compile_checked(string source, string module_name)
    {
        return ((Func<List<Token>, CompileResult>)((tokens) => ((Func<ParseState, CompileResult>)((st) => ((Func<Document, CompileResult>)((doc) => ((Func<AModule, CompileResult>)((ast) => ((Func<ResolveResult, CompileResult>)((resolve_result) => ((((long)resolve_result.errors.Count) > 0L) ? new CompileError(resolve_result.errors) : ((Func<ModuleResult, CompileResult>)((check_result) => ((Func<IRModule, CompileResult>)((ir) => new CompileOk(emit_full_module(ir, ast.type_defs), check_result)))(lower_module(ast, check_result.types, check_result.state))))(check_module(ast)))))(resolve_module(ast))))(desugar_document(doc, module_name))))(parse_document(st))))(make_parse_state(tokens))))(tokenize(source));
    }

    public static string test_source()
    {
        return "square : Integer -> Integer\nsquare (x) = x * x\nmain = square 5";
    }

    public static object main()
    {
        ((Func<object>)(() => {
                Console.WriteLine(compile(test_source(), "test"));
                return null;
            }))();
        return null;
    }

}
