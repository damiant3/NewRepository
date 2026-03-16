using System;
using System.Collections.Generic;
using System.Linq;

Console.WriteLine(Codex_Codex_Codex.main());

public abstract record LiteralKind;
public sealed record IntLit : LiteralKind;
public sealed record NumLit : LiteralKind;
public sealed record TextLit : LiteralKind;
public sealed record BoolLit : LiteralKind;


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


public sealed record ALetBind(Name name, AExpr value);

public sealed record AMatchArm(APat pattern, AExpr body);

public sealed record AFieldExpr(Name name, AExpr value);

public abstract record ADoStmt;
public sealed record ADoBindStmt(Name Field0, AExpr Field1) : ADoStmt;
public sealed record ADoExprStmt(AExpr Field0) : ADoStmt;


public abstract record APat;
public sealed record AVarPat(Name Field0) : APat;
public sealed record ALitPat(string Field0, LiteralKind Field1) : APat;
public sealed record ACtorPat(Name Field0, List<APat> Field1) : APat;
public sealed record AWildPat : APat;


public abstract record ATypeExpr;
public sealed record ANamedType(Name Field0) : ATypeExpr;
public sealed record AFunType(ATypeExpr Field0, ATypeExpr Field1) : ATypeExpr;
public sealed record AAppType(ATypeExpr Field0, List<ATypeExpr> Field1) : ATypeExpr;


public sealed record AParam(Name name);

public sealed record ADef(Name name, List<AParam> @params, List<ATypeExpr> declared_type, AExpr body);

public sealed record ARecordFieldDef(Name name, ATypeExpr type_expr);

public sealed record AVariantCtorDef(Name name, List<ATypeExpr> fields);

public abstract record ATypeDef;
public sealed record ARecordTypeDef(Name Field0, List<Name> Field1, List<ARecordFieldDef> Field2) : ATypeDef;
public sealed record AVariantTypeDef(Name Field0, List<Name> Field1, List<AVariantCtorDef> Field2) : ATypeDef;


public sealed record AModule(Name name, List<ADef> defs, List<ATypeDef> type_defs);

public abstract record DiagnosticSeverity;
public sealed record Error : DiagnosticSeverity;
public sealed record Warning : DiagnosticSeverity;
public sealed record Info : DiagnosticSeverity;


public sealed record Diagnostic(string code, string message, DiagnosticSeverity severity);

public sealed record Name(string value);

public sealed record SourcePosition(long line, long column, long offset);

public sealed record SourceSpan(SourcePosition start, SourcePosition end, string file);

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


public sealed record IRParam(string name, CodexType type_val);

public sealed record IRBranch(IRPat pattern, IRExpr body);

public abstract record IRPat;
public sealed record IrVarPat(string Field0, CodexType Field1) : IRPat;
public sealed record IrLitPat(string Field0, CodexType Field1) : IRPat;
public sealed record IrCtorPat(string Field0, List<IRPat> Field1, CodexType Field2) : IRPat;
public sealed record IrWildPat : IRPat;


public abstract record IRDoStmt;
public sealed record IrDoBind(string Field0, CodexType Field1, IRExpr Field2) : IRDoStmt;
public sealed record IrDoExec(IRExpr Field0) : IRDoStmt;


public sealed record IRFieldVal(string name, IRExpr value);

public sealed record IRDef(string name, List<IRParam> @params, CodexType type_val, IRExpr body);

public sealed record IRModule(Name name, List<IRDef> defs);

public sealed record Scope(List<string> names);

public sealed record ResolveResult(List<Diagnostic> errors, List<string> top_level_names, List<string> type_names, List<string> ctor_names);

public sealed record CollectResult(List<string> names, List<Diagnostic> errors);

public sealed record LexState(string source, long offset, long line, long column);

public abstract record LexResult;
public sealed record LexToken(Token Field0, LexState Field1) : LexResult;
public sealed record LexEnd : LexResult;


public sealed record ParseState(List<Token> tokens, long pos);

public abstract record ParseTypeDefResult;
public sealed record TypeDefOk(TypeDef Field0, ParseState Field1) : ParseTypeDefResult;
public sealed record TypeDefNone(ParseState Field0) : ParseTypeDefResult;


public sealed record LetBind(Token name, Expr value);

public sealed record MatchArm(Pat pattern, Expr body);

public sealed record RecordFieldExpr(Token name, Expr value);

public abstract record DoStmt;
public sealed record DoBindStmt(Token Field0, Expr Field1) : DoStmt;
public sealed record DoExprStmt(Expr Field0) : DoStmt;


public abstract record Pat;
public sealed record VarPat(Token Field0) : Pat;
public sealed record LitPat(Token Field0) : Pat;
public sealed record CtorPat(Token Field0, List<Pat> Field1) : Pat;
public sealed record WildPat(Token Field0) : Pat;


public abstract record TypeExpr;
public sealed record NamedType(Token Field0) : TypeExpr;
public sealed record FunType(TypeExpr Field0, TypeExpr Field1) : TypeExpr;
public sealed record AppType(TypeExpr Field0, List<TypeExpr> Field1) : TypeExpr;
public sealed record ParenType(TypeExpr Field0) : TypeExpr;
public sealed record ListType(TypeExpr Field0) : TypeExpr;
public sealed record LinearTypeExpr(TypeExpr Field0) : TypeExpr;


public sealed record TypeAnn(Token name, TypeExpr type_expr);

public sealed record Def(Token name, List<Token> @params, List<TypeAnn> ann, Expr body);

public sealed record RecordFieldDef(Token name, TypeExpr type_expr);

public sealed record VariantCtorDef(Token name, List<TypeExpr> fields);

public abstract record TypeBody;
public sealed record RecordBody(List<RecordFieldDef> Field0) : TypeBody;
public sealed record VariantBody(List<VariantCtorDef> Field0) : TypeBody;


public sealed record TypeDef(Token name, List<Token> type_params, TypeBody body);

public sealed record Document(List<Def> defs, List<TypeDef> type_defs);

public sealed record Token(TokenKind kind, string text, long offset, long line, long column);

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


public sealed record SumCtor(Name name, List<CodexType> fields);

public sealed record RecordField(Name name, CodexType type_val);

public sealed record CheckResult(CodexType inferred_type, UnificationState state);

public sealed record LetBindResult(UnificationState state, TypeEnv env);

public sealed record LambdaBindResult(UnificationState state, TypeEnv env, List<CodexType> param_types);

public sealed record PatBindResult(UnificationState state, TypeEnv env);

public sealed record DefSetup(CodexType expected_type, CodexType remaining_type, UnificationState state, TypeEnv env);

public sealed record DefParamResult(UnificationState state, TypeEnv env, CodexType remaining_type);

public sealed record TypeEnv(List<TypeBinding> bindings);

public sealed record TypeBinding(string name, CodexType bound_type);

public sealed record UnificationState(List<SubstEntry> substitutions, long next_id, List<Diagnostic> errors);

public sealed record SubstEntry(long var_id, CodexType resolved_type);

public sealed record UnifyResult(bool success, UnificationState state);

public sealed record FreshResult(CodexType var_type, UnificationState state);

public abstract record CompileResult;
public sealed record CompileOk(string Field0, ModuleResult Field1) : CompileResult;
public sealed record CompileError(List<Diagnostic> Field0) : CompileResult;


public static class Codex_Codex_Codex
{
    public static AExpr desugar_expr(Expr node)
    {
        return ((Func<AExpr, AExpr>)((_scrutinee0_) => (_scrutinee0_ is LitExpr _mLitExpr0_ ? ((Func<object, AExpr>)((tok) => desugar_literal(tok)))(_mLitExpr0_.Field0) : (_scrutinee0_ is NameExpr _mNameExpr0_ ? ((Func<object, AExpr>)((tok) => new ANameExpr(make_name(tok.text))))(_mNameExpr0_.Field0) : (_scrutinee0_ is AppExpr _mAppExpr0_ ? ((Func<object, AExpr>)((a) => ((Func<object, AExpr>)((f) => new AApplyExpr(desugar_expr(f), desugar_expr(a))))(_mAppExpr0_.Field0)))(_mAppExpr0_.Field1) : (_scrutinee0_ is BinExpr _mBinExpr0_ ? ((Func<object, AExpr>)((r) => ((Func<object, AExpr>)((op) => ((Func<object, AExpr>)((l) => new ABinaryExpr(desugar_expr(l), desugar_bin_op(op().kind), desugar_expr(r))))(_mBinExpr0_.Field0)))(_mBinExpr0_.Field1)))(_mBinExpr0_.Field2) : (_scrutinee0_ is UnaryExpr _mUnaryExpr0_ ? ((Func<object, AExpr>)((operand) => ((Func<object, AExpr>)((op) => new AUnaryExpr(desugar_expr(operand))))(_mUnaryExpr0_.Field0)))(_mUnaryExpr0_.Field1) : (_scrutinee0_ is IfExpr _mIfExpr0_ ? ((Func<object, AExpr>)((e) => ((Func<object, AExpr>)((t) => ((Func<object, AExpr>)((c) => new AIfExpr(desugar_expr(c), desugar_expr(t), desugar_expr(e))))(_mIfExpr0_.Field0)))(_mIfExpr0_.Field1)))(_mIfExpr0_.Field2) : (_scrutinee0_ is LetExpr _mLetExpr0_ ? ((Func<object, AExpr>)((body) => ((Func<object, AExpr>)((bindings) => new ALetExpr((_p0_) => map_list(desugar_let_bind(bindings), _p0_), desugar_expr(body))))(_mLetExpr0_.Field0)))(_mLetExpr0_.Field1) : (_scrutinee0_ is MatchExpr _mMatchExpr0_ ? ((Func<object, AExpr>)((arms) => ((Func<object, AExpr>)((scrut) => new AMatchExpr(desugar_expr(scrut), (_p0_) => map_list(desugar_match_arm(arms), _p0_))))(_mMatchExpr0_.Field0)))(_mMatchExpr0_.Field1) : (_scrutinee0_ is ListExpr _mListExpr0_ ? ((Func<object, AExpr>)((elems) => new AListExpr((_p0_) => map_list(desugar_expr(elems), _p0_))))(_mListExpr0_.Field0) : (_scrutinee0_ is RecordExpr _mRecordExpr0_ ? ((Func<object, AExpr>)((fields) => ((Func<object, AExpr>)((type_tok) => new ARecordExpr(make_name(type_tok.text), (_p0_) => map_list(desugar_field_expr(fields), _p0_))))(_mRecordExpr0_.Field0)))(_mRecordExpr0_.Field1) : (_scrutinee0_ is FieldExpr _mFieldExpr0_ ? ((Func<object, AExpr>)((field_tok) => ((Func<object, AExpr>)((rec) => new AFieldAccess(desugar_expr(rec), make_name(field_tok.text))))(_mFieldExpr0_.Field0)))(_mFieldExpr0_.Field1) : (_scrutinee0_ is ParenExpr _mParenExpr0_ ? ((Func<object, AExpr>)((inner) => desugar_expr(inner)))(_mParenExpr0_.Field0) : (_scrutinee0_ is DoExpr _mDoExpr0_ ? ((Func<object, AExpr>)((stmts) => new ADoExpr((_p0_) => map_list(desugar_do_stmt(stmts), _p0_))))(_mDoExpr0_.Field0) : (_scrutinee0_ is ErrExpr _mErrExpr0_ ? ((Func<object, AExpr>)((tok) => new AErrorExpr(tok.text)))(_mErrExpr0_.Field0) : throw new InvalidOperationException("Non-exhaustive match")))))))))))))))))(node)(desugar_literal);
    }

    public static T271 Token<T271>()
    {
        return new AExpr();
    }

    public static AExpr desugar_literal(LiteralKind tok)
    {
        return (is_literal(tok.kind) ? new ALitExpr(tok.text(classify_literal(tok.kind))) : new AErrorExpr(tok.text));
    }

    public static LiteralKind classify_literal(TokenKind k)
    {
        return ((Func<LiteralKind, LiteralKind>)((_scrutinee0_) => (_scrutinee0_ is IntegerLiteral _mIntegerLiteral0_ ? new IntLit() : (_scrutinee0_ is NumberLiteral _mNumberLiteral0_ ? new NumLit() : (_scrutinee0_ is TextLiteral _mTextLiteral0_ ? new TextLit() : (_scrutinee0_ is TrueKeyword _mTrueKeyword0_ ? new BoolLit() : (_scrutinee0_ is FalseKeyword _mFalseKeyword0_ ? new BoolLit() : new TextLit())))))))(k)(desugar_let_bind);
    }

    public static Func<Name, Func<AExpr, ALetBind>> LetBind()
    {
        return ALetBind;
    }

    public static ALetBind desugar_let_bind(object b)
    {
        return new ALetBind(make_name(b().name.text), desugar_expr(b().value));
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
        return ((Func<ADoStmt, ADoStmt>)((_scrutinee0_) => (_scrutinee0_ is DoBindStmt _mDoBindStmt0_ ? ((Func<object, ADoStmt>)((val) => ((Func<object, ADoStmt>)((tok) => new ADoBindStmt(make_name(tok.text), desugar_expr(val))))(_mDoBindStmt0_.Field0)))(_mDoBindStmt0_.Field1) : (_scrutinee0_ is DoExprStmt _mDoExprStmt0_ ? ((Func<object, ADoStmt>)((e) => new ADoExprStmt(desugar_expr(e))))(_mDoExprStmt0_.Field0) : throw new InvalidOperationException("Non-exhaustive match")))))(s)(desugar_bin_op);
    }

    public static T53 TokenKind<T53>()
    {
        return new BinaryOp();
    }

    public static T443 desugar_bin_op<T443>(object k)
    {
        return ((Func<object, object>)((_scrutinee0_) => (_scrutinee0_ is Plus _mPlus0_ ? new OpAdd() : (_scrutinee0_ is Minus _mMinus0_ ? new OpSub() : (_scrutinee0_ is Star _mStar0_ ? new OpMul() : (_scrutinee0_ is Slash _mSlash0_ ? new OpDiv() : (_scrutinee0_ is Caret _mCaret0_ ? new OpPow() : (_scrutinee0_ is DoubleEquals _mDoubleEquals0_ ? new OpEq() : (_scrutinee0_ is NotEquals _mNotEquals0_ ? new OpNotEq() : (_scrutinee0_ is LessThan _mLessThan0_ ? new OpLt() : (_scrutinee0_ is GreaterThan _mGreaterThan0_ ? new OpGt() : (_scrutinee0_ is LessOrEqual _mLessOrEqual0_ ? new OpLtEq() : (_scrutinee0_ is GreaterOrEqual _mGreaterOrEqual0_ ? new OpGtEq() : (_scrutinee0_ is TripleEquals _mTripleEquals0_ ? new OpDefEq() : (_scrutinee0_ is PlusPlus _mPlusPlus0_ ? new OpAppend() : (_scrutinee0_ is ColonColon _mColonColon0_ ? new OpCons() : (_scrutinee0_ is Ampersand _mAmpersand0_ ? new OpAnd() : (_scrutinee0_ is Pipe _mPipe0_ ? new OpOr() : new OpAdd()))))))))))))))))))(k)(desugar_pattern);
    }

    public static object Pat()
    {
        return new APat();
    }

    public static T462 desugar_pattern<T462>(object p)
    {
        return ((Func<object, object>)((_scrutinee0_) => (_scrutinee0_ is VarPat _mVarPat0_ ? ((Func<object, object>)((tok) => new AVarPat(make_name(tok.text))))(_mVarPat0_.Field0) : (_scrutinee0_ is LitPat _mLitPat0_ ? ((Func<object, object>)((tok) => new ALitPat(tok.text(classify_literal(tok.kind)))))(_mLitPat0_.Field0) : (_scrutinee0_ is CtorPat _mCtorPat0_ ? ((Func<object, object>)((subs) => ((Func<object, object>)((tok) => new ACtorPat(make_name(tok.text), (_p0_) => map_list(desugar_pattern(subs), _p0_))))(_mCtorPat0_.Field0)))(_mCtorPat0_.Field1) : (_scrutinee0_ is WildPat _mWildPat0_ ? ((Func<object, object>)((tok) => new AWildPat()))(_mWildPat0_.Field0) : throw new InvalidOperationException("Non-exhaustive match")))))))(p())(desugar_type_expr);
    }

    public static Func<string, Func<IRExpr, IRFieldVal>> TypeExpr()
    {
        return ATypeExpr;
    }

    public static T486 desugar_type_expr<T486>(object t)
    {
        return ((Func<object, object>)((_scrutinee0_) => (_scrutinee0_ is NamedType _mNamedType0_ ? ((Func<object, object>)((tok) => new ANamedType(make_name(tok.text))))(_mNamedType0_.Field0) : (_scrutinee0_ is FunType _mFunType0_ ? ((Func<object, object>)((ret) => ((Func<object, object>)((param) => new AFunType(desugar_type_expr(param), desugar_type_expr(ret))))(_mFunType0_.Field0)))(_mFunType0_.Field1) : (_scrutinee0_ is AppType _mAppType0_ ? ((Func<object, object>)((args) => ((Func<object, object>)((ctor) => new AAppType(desugar_type_expr(ctor), (_p0_) => map_list(desugar_type_expr(args), _p0_))))(_mAppType0_.Field0)))(_mAppType0_.Field1) : (_scrutinee0_ is ParenType _mParenType0_ ? ((Func<object, object>)((inner) => desugar_type_expr(inner)))(_mParenType0_.Field0) : (_scrutinee0_ is ListType _mListType0_ ? ((Func<object, object>)((elem) => new AAppType(new ANamedType(make_name("List")), new List<object>() { desugar_type_expr(elem) })))(_mListType0_.Field0) : (_scrutinee0_ is LinearTypeExpr _mLinearTypeExpr0_ ? ((Func<object, object>)((inner) => desugar_type_expr(inner)))(_mLinearTypeExpr0_.Field0) : throw new InvalidOperationException("Non-exhaustive match")))))))))(t)(desugar_def);
    }

    public static Func<string, Func<IRExpr, IRFieldVal>> Def()
    {
        return ADef;
    }

    public static ADef desugar_def(object d)
    {
        return ((Func<object, object>)((ann_types) => new ADef(make_name(d.name.text), (_p0_) => map_list(desugar_param(d.@params), _p0_), ann_types, desugar_expr(d.body))))(desugar_annotations(d.ann));
    }

    public static List<ATypeExpr> desugar_annotations(List<TypeAnn> anns)
    {
        return ((((long)anns.Count) == 0L) ? new List<ATypeExpr>() : ((Func<List<ATypeExpr>, List<ATypeExpr>>)((a) => new List<ATypeExpr>() { desugar_type_expr(a.type_expr) }))(list_at()(anns(0L))));
    }

    public static AParam desugar_param(Token tok)
    {
        return new AParam(make_name(tok.text));
    }

    public static ATypeDef desugar_type_def(TypeDef td)
    {
        return ((Func<ATypeDef, ATypeDef>)((_scrutinee0_) => (_scrutinee0_ is RecordBody _mRecordBody0_ ? ((Func<object, ATypeDef>)((fields) => new ARecordTypeDef(make_name(td.name.text), (_p0_) => map_list(make_type_param_name(td.type_params), _p0_), (_p0_) => map_list(desugar_record_field_def(fields), _p0_))))(_mRecordBody0_.Field0) : (_scrutinee0_ is VariantBody _mVariantBody0_ ? ((Func<object, ATypeDef>)((ctors) => new AVariantTypeDef(make_name(td.name.text), (_p0_) => map_list(make_type_param_name(td.type_params), _p0_), (_p0_) => map_list(desugar_variant_ctor_def(ctors), _p0_))))(_mVariantBody0_.Field0) : throw new InvalidOperationException("Non-exhaustive match")))))(td.body)(make_type_param_name);
    }

    public static T271 Token<T271>()
    {
        return new Name();
    }

    public static Name make_type_param_name(object tok)
    {
        return make_name(tok.text);
    }

    public static ARecordFieldDef desugar_record_field_def(RecordFieldDef f)
    {
        return new ARecordFieldDef(make_name(f.name.text), desugar_type_expr(f.type_expr));
    }

    public static AVariantCtorDef desugar_variant_ctor_def(VariantCtorDef c)
    {
        return new AVariantCtorDef(make_name(c.name.text), (_p0_) => map_list(desugar_type_expr(c.fields), _p0_));
    }

    public static AModule desugar_document(Document doc, string module_name)
    {
        return new AModule(make_name(module_name), (_p0_) => map_list(desugar_def(doc.defs), _p0_), (_p0_) => map_list(desugar_type_def(doc.type_defs), _p0_));
    }

    public static List<b> map_list(Func<a, b> f, List<a> xs)
    {
        return (_p0_) => (_p1_) => (_p2_) => (_p3_) => map_list_loop(f(xs(0L)(((long)xs.Count))(new List<b>())), _p0_, _p1_, _p2_, _p3_);
    }

    public static List<b> map_list_loop(Func<a, b> f, List<a> xs, long i, long len, List<b> acc)
    {
        while (true)
        {
            if ((i() == len))
            {
                return acc();
            }
            else
            {
                var _tco_0 = f(xs((i() + 1L))(len(Enumerable.Concat(acc(), new List<b>() { f(list_at()(xs(i))) }).ToList())));
                f = _tco_0;
                continue;
            }
        }
    }

    public static b fold_list(Func<b, Func<a, b>> f, b z, List<a> xs)
    {
        return (_p0_) => (_p1_) => (_p2_) => (_p3_) => fold_list_loop(f(z(xs(0L)(((long)xs.Count)))), _p0_, _p1_, _p2_, _p3_);
    }

    public static b fold_list_loop(Func<b, Func<a, b>> f, b z, List<a> xs, long i, long len)
    {
        while (true)
        {
            if ((i() == len))
            {
                return z;
            }
            else
            {
                var _tco_0 = f(f(z(list_at()(xs(i)))))(xs((i() + 1L))(len));
                f = _tco_0;
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
        return ((Func<string, string>)((_scrutinee0_) => (_scrutinee0_ is Error _mError0_ ? "error" : (_scrutinee0_ is Warning _mWarning0_ ? "warning" : (_scrutinee0_ is Info _mInfo0_ ? "info" : throw new InvalidOperationException("Non-exhaustive match"))))))(s)(diagnostic_display);
    }

    public static T241 Diagnostic<T241>()
    {
        return new Text();
    }

    public static string diagnostic_display(object d)
    {
        return Enumerable.Concat(severity_label(d.severity), Enumerable.Concat(" ", Enumerable.Concat(d.code, Enumerable.Concat(": ", d.message).ToList()).ToList()).ToList()).ToList();
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
        return new SourceSpan(s, e(), f);
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
        return ((Func<string, string>)((s) => (is_cs_keyword(s) ? string.Concat("@", s) : s)))(text_replace(name("-")("_")));
    }

    public static string cs_type(CodexType ty)
    {
        return ((Func<string, string>)((_scrutinee0_) => (_scrutinee0_ is IntegerTy _mIntegerTy0_ ? "long" : (_scrutinee0_ is NumberTy _mNumberTy0_ ? "decimal" : (_scrutinee0_ is TextTy _mTextTy0_ ? "string" : (_scrutinee0_ is BooleanTy _mBooleanTy0_ ? "bool" : (_scrutinee0_ is VoidTy _mVoidTy0_ ? "void" : (_scrutinee0_ is NothingTy _mNothingTy0_ ? "object" : (_scrutinee0_ is ErrorTy _mErrorTy0_ ? "object" : (_scrutinee0_ is FunTy _mFunTy0_ ? ((Func<object, string>)((r) => ((Func<object, string>)((p) => string.Concat("Func<", string.Concat(cs_type(p), string.Concat(", ", string.Concat(cs_type(r), ">"))))))(_mFunTy0_.Field0)))(_mFunTy0_.Field1) : (_scrutinee0_ is ListTy _mListTy0_ ? ((Func<object, string>)((elem) => string.Concat("List<", string.Concat(cs_type(elem), ">"))))(_mListTy0_.Field0) : (_scrutinee0_ is TypeVar _mTypeVar0_ ? ((Func<object, string>)((id) => string.Concat("T", (id).ToString())))(_mTypeVar0_.Field0) : (_scrutinee0_ is ForAllTy _mForAllTy0_ ? ((Func<object, string>)((body) => ((Func<object, string>)((id) => cs_type(body)))(_mForAllTy0_.Field0)))(_mForAllTy0_.Field1) : (_scrutinee0_ is SumTy _mSumTy0_ ? ((Func<object, string>)((ctors) => ((Func<object, string>)((name) => sanitize(name.value)))(_mSumTy0_.Field0)))(_mSumTy0_.Field1) : (_scrutinee0_ is RecordTy _mRecordTy0_ ? ((Func<object, string>)((fields) => ((Func<object, string>)((name) => sanitize(name.value)))(_mRecordTy0_.Field0)))(_mRecordTy0_.Field1) : (_scrutinee0_ is ConstructedTy _mConstructedTy0_ ? ((Func<object, string>)((args) => ((Func<object, string>)((name) => sanitize(name.value)))(_mConstructedTy0_.Field0)))(_mConstructedTy0_.Field1) : throw new InvalidOperationException("Non-exhaustive match")))))))))))))))))(ty)(emit_expr);
    }

    public static T241 IRExpr<T241>()
    {
        return new Text();
    }

    public static T657 emit_expr<T657>(object e)
    {
        return ((Func<object, object>)((_scrutinee0_) => (_scrutinee0_ is IrIntLit _mIrIntLit0_ ? ((Func<object, object>)((n) => (n).ToString()))(_mIrIntLit0_.Field0) : (_scrutinee0_ is IrNumLit _mIrNumLit0_ ? ((Func<object, object>)((n) => (n).ToString()))(_mIrNumLit0_.Field0) : (_scrutinee0_ is IrTextLit _mIrTextLit0_ ? ((Func<object, object>)((s) => Enumerable.Concat("\\\"", Enumerable.Concat(escape_text(s), "\\\"").ToList()).ToList()))(_mIrTextLit0_.Field0) : (_scrutinee0_ is IrBoolLit _mIrBoolLit0_ ? ((Func<object, object>)((b) => (b() ? "true" : "false")))(_mIrBoolLit0_.Field0) : (_scrutinee0_ is IrName _mIrName0_ ? ((Func<object, object>)((ty) => ((Func<object, object>)((n) => sanitize(n)))(_mIrName0_.Field0)))(_mIrName0_.Field1) : (_scrutinee0_ is IrBinary _mIrBinary0_ ? ((Func<object, object>)((ty) => ((Func<object, object>)((r) => ((Func<object, object>)((l) => ((Func<object, object>)((op) => Enumerable.Concat("(", Enumerable.Concat(emit_expr(l), Enumerable.Concat(" ", Enumerable.Concat(emit_bin_op(op), Enumerable.Concat(" ", Enumerable.Concat(emit_expr(r), ")").ToList()).ToList()).ToList()).ToList()).ToList()).ToList()))(_mIrBinary0_.Field0)))(_mIrBinary0_.Field1)))(_mIrBinary0_.Field2)))(_mIrBinary0_.Field3) : (_scrutinee0_ is IrNegate _mIrNegate0_ ? ((Func<object, object>)((operand) => Enumerable.Concat("(-", Enumerable.Concat(emit_expr(operand), ")").ToList()).ToList()))(_mIrNegate0_.Field0) : (_scrutinee0_ is IrIf _mIrIf0_ ? ((Func<object, object>)((ty) => ((Func<object, object>)((el) => ((Func<object, object>)((t) => ((Func<object, object>)((c) => Enumerable.Concat("(", Enumerable.Concat(emit_expr(c), Enumerable.Concat(" ? ", Enumerable.Concat(emit_expr(t), Enumerable.Concat(" : ", Enumerable.Concat(emit_expr(el), ")").ToList()).ToList()).ToList()).ToList()).ToList()).ToList()))(_mIrIf0_.Field0)))(_mIrIf0_.Field1)))(_mIrIf0_.Field2)))(_mIrIf0_.Field3) : (_scrutinee0_ is IrLet _mIrLet0_ ? ((Func<object, object>)((body) => ((Func<object, object>)((val) => ((Func<object, object>)((ty) => ((Func<object, object>)((name) => emit_let(name(ty(val(body))))))(_mIrLet0_.Field0)))(_mIrLet0_.Field1)))(_mIrLet0_.Field2)))(_mIrLet0_.Field3) : (_scrutinee0_ is IrApply _mIrApply0_ ? ((Func<object, object>)((ty) => ((Func<object, object>)((a) => ((Func<object, object>)((f) => Enumerable.Concat(emit_expr(f), Enumerable.Concat("(", Enumerable.Concat(emit_expr(a), ")").ToList()).ToList()).ToList()))(_mIrApply0_.Field0)))(_mIrApply0_.Field1)))(_mIrApply0_.Field2) : (_scrutinee0_ is IrLambda _mIrLambda0_ ? ((Func<object, object>)((ty) => ((Func<object, object>)((body) => ((Func<object, object>)((@params) => (_p0_) => emit_lambda(@params(body), _p0_)))(_mIrLambda0_.Field0)))(_mIrLambda0_.Field1)))(_mIrLambda0_.Field2) : (_scrutinee0_ is IrList _mIrList0_ ? ((Func<object, object>)((ty) => ((Func<object, object>)((elems) => (_p0_) => emit_list(elems()(ty), _p0_)))(_mIrList0_.Field0)))(_mIrList0_.Field1) : (_scrutinee0_ is IrMatch _mIrMatch0_ ? ((Func<object, object>)((ty) => ((Func<object, object>)((branches) => ((Func<object, object>)((scrut) => (_p0_) => (_p1_) => emit_match(scrut(branches(ty)), _p0_, _p1_)))(_mIrMatch0_.Field0)))(_mIrMatch0_.Field1)))(_mIrMatch0_.Field2) : (_scrutinee0_ is IrDo _mIrDo0_ ? ((Func<object, object>)((ty) => ((Func<object, object>)((stmts) => emit_do(stmts)))(_mIrDo0_.Field0)))(_mIrDo0_.Field1) : (_scrutinee0_ is IrRecord _mIrRecord0_ ? ((Func<object, object>)((ty) => ((Func<object, object>)((fields) => ((Func<object, object>)((name) => emit_record(name(fields))))(_mIrRecord0_.Field0)))(_mIrRecord0_.Field1)))(_mIrRecord0_.Field2) : (_scrutinee0_ is IrFieldAccess _mIrFieldAccess0_ ? ((Func<object, object>)((ty) => ((Func<object, object>)((field) => ((Func<object, object>)((rec) => Enumerable.Concat(emit_expr(rec), Enumerable.Concat(".", sanitize(field)).ToList()).ToList()))(_mIrFieldAccess0_.Field0)))(_mIrFieldAccess0_.Field1)))(_mIrFieldAccess0_.Field2) : (_scrutinee0_ is IrError _mIrError0_ ? ((Func<object, object>)((ty) => ((Func<object, object>)((msg) => Enumerable.Concat("/* error: ", Enumerable.Concat(msg, " */ default").ToList()).ToList()))(_mIrError0_.Field0)))(_mIrError0_.Field1) : throw new InvalidOperationException("Non-exhaustive match"))))))))))))))))))))(e())(escape_text);
    }

    public static T241 Text<T241>()
    {
        return new Text();
    }

    public static T666 escape_text<T666>(object s)
    {
        return text_replace(s("\\\\")("\\\\\\\\")).Replace("\\\"", "\\\\\\\"");
    }

    public static string emit_bin_op(IRBinaryOp op)
    {
        return ((Func<string, string>)((_scrutinee0_) => (_scrutinee0_ is IrAddInt _mIrAddInt0_ ? "+" : (_scrutinee0_ is IrSubInt _mIrSubInt0_ ? "-" : (_scrutinee0_ is IrMulInt _mIrMulInt0_ ? "*" : (_scrutinee0_ is IrDivInt _mIrDivInt0_ ? "/" : (_scrutinee0_ is IrPowInt _mIrPowInt0_ ? "^" : (_scrutinee0_ is IrAddNum _mIrAddNum0_ ? "+" : (_scrutinee0_ is IrSubNum _mIrSubNum0_ ? "-" : (_scrutinee0_ is IrMulNum _mIrMulNum0_ ? "*" : (_scrutinee0_ is IrDivNum _mIrDivNum0_ ? "/" : (_scrutinee0_ is IrEq _mIrEq0_ ? "==" : (_scrutinee0_ is IrNotEq _mIrNotEq0_ ? "!=" : (_scrutinee0_ is IrLt _mIrLt0_ ? "<" : (_scrutinee0_ is IrGt _mIrGt0_ ? ">" : (_scrutinee0_ is IrLtEq _mIrLtEq0_ ? "<=" : (_scrutinee0_ is IrGtEq _mIrGtEq0_ ? ">=" : (_scrutinee0_ is IrAnd _mIrAnd0_ ? "&&" : (_scrutinee0_ is IrOr _mIrOr0_ ? "||" : (_scrutinee0_ is IrAppendText _mIrAppendText0_ ? "+" : (_scrutinee0_ is IrAppendList _mIrAppendList0_ ? "+" : (_scrutinee0_ is IrConsList _mIrConsList0_ ? "+" : throw new InvalidOperationException("Non-exhaustive match")))))))))))))))))))))))(op())(emit_let);
    }

    public static T241 Text<T241>()
    {
        return new CodexType();
    }

    public static T241 IRExpr<T241>()
    {
        return new IRExpr();
    }

    public static T241 Text<T241>()
    {
        return emit_let(name)(ty)(val)(body);
    }

    public static Func<CodexType, string> cs_type()
    {
        return Enumerable.Concat(throw new InvalidOperationException("++")(" "), Enumerable.Concat(sanitize(new Func<CodexType, string>(name)), Enumerable.Concat(" = ", Enumerable.Concat(emit_expr(new Func<CodexType, string>(val)), Enumerable.Concat(") is var _ ? ", Enumerable.Concat(emit_expr(new Func<CodexType, string>(body)), " : default)").ToList()).ToList()).ToList()).ToList()).ToList()).ToList();
    }

    public static string emit_lambda(List<IRParam> @params, IRExpr body)
    {
        return ((((long)@params.Count) == 0L) ? string.Concat("(() => ", string.Concat(emit_expr(body), ")")) : ((((long)@params.Count) == 1L) ? ((Func<string, string>)((p) => string.Concat("((", string.Concat(cs_type(p().type_val), string.Concat(" ", string.Concat(sanitize(p().name), string.Concat(") => ", string.Concat(emit_expr(body), ")"))))))))(list_at()(@params(0L))) : string.Concat("(() => ", string.Concat(emit_expr(body), ")"))));
    }

    public static string emit_list(List<IRExpr> elems, CodexType ty)
    {
        return ((((long)elems().Count) == 0L) ? string.Concat("new List<", string.Concat(cs_type(ty), ">()")) : string.Concat("new List<", string.Concat(cs_type(ty), string.Concat("> { ", string.Concat((_p0_) => emit_list_elems(elems()(0L), _p0_), " }")))));
    }

    public static string emit_list_elems(List<IRExpr> elems, long i)
    {
        return ((i() == ((long)elems().Count)) ? "" : ((i() == (((long)elems().Count) - 1L)) ? emit_expr(list_at()(elems()(i))) : string.Concat(emit_expr(list_at()(elems()(i))), string.Concat(", ", (_p0_) => emit_list_elems(elems()((i() + 1L)), _p0_)))));
    }

    public static string emit_match(IRExpr scrut, List<IRBranch> branches, CodexType ty)
    {
        return string.Concat(emit_expr(scrut), string.Concat(" switch { ", string.Concat((_p0_) => emit_match_arms(branches(0L), _p0_), " }")));
    }

    public static string emit_match_arms(List<IRBranch> branches, long i)
    {
        return ((i() == ((long)branches.Count)) ? "" : ((Func<string, string>)((arm) => string.Concat(emit_pattern(arm.pattern), string.Concat(" => ", string.Concat(emit_expr(arm.body), string.Concat(", ", (_p0_) => emit_match_arms(branches((i() + 1L)), _p0_)))))))(list_at()(branches(i))));
    }

    public static string emit_pattern(IRPat p)
    {
        return ((Func<string, string>)((_scrutinee0_) => (_scrutinee0_ is IrVarPat _mIrVarPat0_ ? ((Func<object, string>)((ty) => ((Func<object, string>)((name) => string.Concat(cs_type(ty), string.Concat(" ", sanitize(name)))))(_mIrVarPat0_.Field0)))(_mIrVarPat0_.Field1) : (_scrutinee0_ is IrLitPat _mIrLitPat0_ ? ((Func<object, string>)((ty) => ((Func<object, string>)((text) => text))(_mIrLitPat0_.Field0)))(_mIrLitPat0_.Field1) : (_scrutinee0_ is IrCtorPat _mIrCtorPat0_ ? ((Func<object, string>)((ty) => ((Func<object, string>)((subs) => ((Func<object, string>)((name) => ((((long)subs.Count) == 0L) ? string.Concat(sanitize(name), " { }") : string.Concat(sanitize(name), string.Concat("(", string.Concat(emit_sub_patterns(subs(0L)), ")"))))))(_mIrCtorPat0_.Field0)))(_mIrCtorPat0_.Field1)))(_mIrCtorPat0_.Field2) : (_scrutinee0_ is IrWildPat _mIrWildPat0_ ? "_" : throw new InvalidOperationException("Non-exhaustive match")))))))(p())(emit_sub_patterns);
    }

    public static T736 List<T736>()
    {
        return throw new InvalidOperationException("->")(Integer);
    }

    public static T241 Text<T241>()
    {
        return emit_sub_patterns(subs)(i);
    }

    public static long i()
    {
        return ((long)subs.Count);
    }

    public static a sub()
    {
        return list_at()(subs(i));
    }

    public static T751 emit_sub_pattern<T751>()
    {
        return Enumerable.Concat(throw new InvalidOperationException("++")(((i() < (((long)subs.Count) - 1L)) ? ", " : "")), emit_sub_patterns(subs((i() + 1L)))).ToList();
    }

    public static T751 emit_sub_pattern<T751>(object p)
    {
        return ((Func<object, object>)((_scrutinee0_) => (_scrutinee0_ is IrVarPat _mIrVarPat0_ ? ((Func<object, object>)((ty) => ((Func<object, object>)((name) => Enumerable.Concat("var ", sanitize(name)).ToList()))(_mIrVarPat0_.Field0)))(_mIrVarPat0_.Field1) : (_scrutinee0_ is IrCtorPat _mIrCtorPat0_ ? ((Func<object, object>)((ty) => ((Func<object, object>)((subs) => ((Func<object, object>)((name) => emit_pattern(p)))(_mIrCtorPat0_.Field0)))(_mIrCtorPat0_.Field1)))(_mIrCtorPat0_.Field2) : (_scrutinee0_ is IrWildPat _mIrWildPat0_ ? "_" : (_scrutinee0_ is IrLitPat _mIrLitPat0_ ? ((Func<object, object>)((ty) => ((Func<object, object>)((text) => text))(_mIrLitPat0_.Field0)))(_mIrLitPat0_.Field1) : throw new InvalidOperationException("Non-exhaustive match")))))))(p())(emit_do);
    }

    public static T736 List<T736>()
    {
        return throw new InvalidOperationException("->")(Text);
    }

    public static string emit_do(object stmts)
    {
        return Enumerable.Concat("{ ", Enumerable.Concat((_p0_) => emit_do_stmts(stmts(0L), _p0_), " }").ToList()).ToList();
    }

    public static string emit_do_stmts(List<IRDoStmt> stmts, long i)
    {
        return ((i() == ((long)stmts.Count)) ? "" : ((Func<string, string>)((s) => string.Concat(emit_do_stmt(s), string.Concat(" ", (_p0_) => emit_do_stmts(stmts((i() + 1L)), _p0_)))))(list_at()(stmts(i))));
    }

    public static string emit_do_stmt(IRDoStmt s)
    {
        return ((Func<string, string>)((_scrutinee0_) => (_scrutinee0_ is IrDoBind _mIrDoBind0_ ? ((Func<object, string>)((val) => ((Func<object, string>)((ty) => ((Func<object, string>)((name) => string.Concat("var ", string.Concat(sanitize(name), string.Concat(" = ", string.Concat(emit_expr(val), ";"))))))(_mIrDoBind0_.Field0)))(_mIrDoBind0_.Field1)))(_mIrDoBind0_.Field2) : (_scrutinee0_ is IrDoExec _mIrDoExec0_ ? ((Func<object, string>)((e) => string.Concat(emit_expr(e), ";")))(_mIrDoExec0_.Field0) : throw new InvalidOperationException("Non-exhaustive match")))))(s)(emit_record);
    }

    public static T241 Text<T241>()
    {
        return new List(new IRFieldVal());
    }

    public static T241 Text<T241>()
    {
        return emit_record(name)(fields);
    }

    public static Func<string, string> sanitize()
    {
        return Enumerable.Concat(throw new InvalidOperationException("++")("("), Enumerable.Concat((_p0_) => emit_record_fields(fields(0L), _p0_), ")").ToList()).ToList();
    }

    public static string emit_record_fields(List<IRFieldVal> fields, long i)
    {
        return ((i() == ((long)fields.Count)) ? "" : ((Func<string, string>)((f) => string.Concat(sanitize(f.name), string.Concat(": ", string.Concat(emit_expr(f.value), string.Concat(((i() < (((long)fields.Count) - 1L)) ? ", " : ""), (_p0_) => emit_record_fields(fields((i() + 1L)), _p0_)))))))(list_at()(fields(i))));
    }

    public static string emit_type_defs(List<ATypeDef> tds, long i)
    {
        return ((i() == ((long)tds.Count)) ? "" : string.Concat(emit_type_def(list_at()(tds(i))), string.Concat("\\n", (_p0_) => emit_type_defs(tds((i() + 1L)), _p0_))));
    }

    public static string emit_type_def(ATypeDef td)
    {
        return ((Func<string, string>)((_scrutinee0_) => (_scrutinee0_ is ARecordTypeDef _mARecordTypeDef0_ ? ((Func<object, string>)((fields) => ((Func<object, string>)((tparams) => ((Func<object, string>)((name) => ((Func<string, string>)((gen) => string.Concat("public sealed record ", string.Concat(sanitize(name.value), string.Concat(gen, string.Concat("(", string.Concat((_p0_) => (_p1_) => emit_record_field_defs(fields(tparams(0L)), _p0_, _p1_), ");\\n")))))))(emit_tparam_suffix(tparams))))(_mARecordTypeDef0_.Field0)))(_mARecordTypeDef0_.Field1)))(_mARecordTypeDef0_.Field2) : (_scrutinee0_ is AVariantTypeDef _mAVariantTypeDef0_ ? ((Func<object, string>)((ctors) => ((Func<object, string>)((tparams) => ((Func<object, string>)((name) => ((Func<string, string>)((gen) => string.Concat("public abstract record ", string.Concat(sanitize(name.value), string.Concat(gen, string.Concat(";\\n", string.Concat((_p0_) => (_p1_) => (_p2_) => emit_variant_ctors(ctors(name(tparams(0L))), _p0_, _p1_, _p2_), "\\n")))))))(emit_tparam_suffix(tparams))))(_mAVariantTypeDef0_.Field0)))(_mAVariantTypeDef0_.Field1)))(_mAVariantTypeDef0_.Field2) : throw new InvalidOperationException("Non-exhaustive match")))))(td)(emit_tparam_suffix);
    }

    public static T736 List<T736>()
    {
        return throw new InvalidOperationException("->")(Text);
    }

    public static string emit_tparam_suffix(object tparams)
    {
        return ((((long)tparams.Count) == 0L) ? "" : Enumerable.Concat("<", Enumerable.Concat((_p0_) => emit_tparam_names(tparams(0L), _p0_), ">").ToList()).ToList());
    }

    public static string emit_tparam_names(List<Name> tparams, long i)
    {
        return ((i() == ((long)tparams.Count)) ? "" : ((i() == (((long)tparams.Count) - 1L)) ? string.Concat("T", (i()).ToString()) : string.Concat("T", string.Concat((i()).ToString(), string.Concat(", ", (_p0_) => emit_tparam_names(tparams((i() + 1L)), _p0_))))));
    }

    public static string emit_record_field_defs(List<ARecordFieldDef> fields, List<Name> tparams, long i)
    {
        return ((i() == ((long)fields.Count)) ? "" : ((Func<string, string>)((f) => string.Concat((_p0_) => emit_type_expr_tp(f.type_expr(tparams), _p0_), string.Concat(" ", string.Concat(sanitize(f.name.value), string.Concat(((i() < (((long)fields.Count) - 1L)) ? ", " : ""), (_p0_) => (_p1_) => emit_record_field_defs(fields(tparams((i() + 1L))), _p0_, _p1_)))))))(list_at()(fields(i))));
    }

    public static string emit_variant_ctors(List<AVariantCtorDef> ctors, Name base_name, List<Name> tparams, long i)
    {
        return ((i() == ((long)ctors.Count)) ? "" : ((Func<string, string>)((c) => string.Concat((_p0_) => (_p1_) => emit_variant_ctor(c(base_name(tparams)), _p0_, _p1_), (_p0_) => (_p1_) => (_p2_) => emit_variant_ctors(ctors(base_name(tparams((i() + 1L)))), _p0_, _p1_, _p2_))))(list_at()(ctors(i))));
    }

    public static string emit_variant_ctor(AVariantCtorDef c, Name base_name, List<Name> tparams)
    {
        return ((Func<string, string>)((gen) => ((((long)c.fields.Count) == 0L) ? string.Concat("public sealed record ", string.Concat(sanitize(c.name.value), string.Concat(gen, string.Concat(" : ", string.Concat(sanitize(base_name.value), string.Concat(gen, ";\\n")))))) : string.Concat("public sealed record ", string.Concat(sanitize(c.name.value), string.Concat(gen, string.Concat("(", string.Concat((_p0_) => (_p1_) => emit_ctor_fields(c.fields(tparams(0L)), _p0_, _p1_), string.Concat(") : ", string.Concat(sanitize(base_name.value), string.Concat(gen, ";\\n")))))))))))(emit_tparam_suffix(tparams));
    }

    public static string emit_ctor_fields(List<ATypeExpr> fields, List<Name> tparams, long i)
    {
        return ((i() == ((long)fields.Count)) ? "" : string.Concat(emit_type_expr_tp(list_at()(fields(i)), tparams), string.Concat(" Field", string.Concat((i()).ToString(), string.Concat(((i() < (((long)fields.Count) - 1L)) ? ", " : ""), (_p0_) => (_p1_) => emit_ctor_fields(fields(tparams((i() + 1L))), _p0_, _p1_))))));
    }

    public static string emit_type_expr(ATypeExpr te)
    {
        return (_p0_) => emit_type_expr_tp(te(new List<object>()), _p0_);
    }

    public static string emit_type_expr_tp(ATypeExpr te, List<Name> tparams)
    {
        return ((Func<string, string>)((_scrutinee0_) => (_scrutinee0_ is ANamedType _mANamedType0_ ? ((Func<object, string>)((name) => ((Func<string, string>)((idx) => ((idx >= 0L) ? string.Concat("T", (idx).ToString()) : when_type_name(name.value))))((_p0_) => (_p1_) => find_tparam_index(tparams(name.value(0L)), _p0_, _p1_))))(_mANamedType0_.Field0) : (_scrutinee0_ is AFunType _mAFunType0_ ? ((Func<object, string>)((r) => ((Func<object, string>)((p) => string.Concat("Func<", string.Concat((_p0_) => emit_type_expr_tp(p()(tparams), _p0_), string.Concat(", ", string.Concat((_p0_) => emit_type_expr_tp(r(tparams), _p0_), ">"))))))(_mAFunType0_.Field0)))(_mAFunType0_.Field1) : (_scrutinee0_ is AAppType _mAAppType0_ ? ((Func<object, string>)((args) => ((Func<object, string>)((@base) => string.Concat((_p0_) => emit_type_expr_tp(@base(tparams), _p0_), string.Concat("<", string.Concat((_p0_) => (_p1_) => emit_type_expr_list_tp(args(tparams(0L)), _p0_, _p1_), ">")))))(_mAAppType0_.Field0)))(_mAAppType0_.Field1) : throw new InvalidOperationException("Non-exhaustive match"))))))(te)(find_tparam_index);
    }

    public static T736 List<T736>()
    {
        return throw new InvalidOperationException("->")(Text);
    }

    public static T316 Integer<T316>()
    {
        return new Integer();
    }

    public static long find_tparam_index(object tparams, object name, object i)
    {
        while (true)
        {
            if ((i() == ((long)tparams.Count)))
            {
                return (0L - 1L);
            }
            else
            {
                if ((list_at()(tparams(i)).value == name))
                {
                    return i();
                }
                else
                {
                    var _tco_0 = tparams(name((i() + 1L)));
                    tparams = _tco_0;
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
        return ((i() == ((long)args.Count)) ? "" : string.Concat(emit_type_expr(list_at()(args(i))), string.Concat(((i() < (((long)args.Count) - 1L)) ? ", " : ""), (_p0_) => emit_type_expr_list(args((i() + 1L)), _p0_))));
    }

    public static string emit_type_expr_list_tp(List<ATypeExpr> args, List<Name> tparams, long i)
    {
        return ((i() == ((long)args.Count)) ? "" : string.Concat(emit_type_expr_tp(list_at()(args(i)), tparams), string.Concat(((i() < (((long)args.Count) - 1L)) ? ", " : ""), (_p0_) => (_p1_) => emit_type_expr_list_tp(args(tparams((i() + 1L))), _p0_, _p1_))));
    }

    public static List<long> collect_type_var_ids(CodexType ty, List<long> acc)
    {
        return ((Func<List<long>, List<long>>)((_scrutinee0_) => (_scrutinee0_ is TypeVar _mTypeVar0_ ? ((Func<object, List<long>>)((id) => ((_p0_) => list_contains_int(acc()(id), _p0_) ? acc() : (_p0_) => list_append_int(acc()(id), _p0_))))(_mTypeVar0_.Field0) : (_scrutinee0_ is FunTy _mFunTy0_ ? ((Func<object, List<long>>)((r) => ((Func<object, List<long>>)((p) => (_p0_) => collect_type_var_ids(r((_p0_) => collect_type_var_ids(p()(acc), _p0_)), _p0_)))(_mFunTy0_.Field0)))(_mFunTy0_.Field1) : (_scrutinee0_ is ListTy _mListTy0_ ? ((Func<object, List<long>>)((elem) => (_p0_) => collect_type_var_ids(elem(acc), _p0_)))(_mListTy0_.Field0) : (_scrutinee0_ is ForAllTy _mForAllTy0_ ? ((Func<object, List<long>>)((body) => ((Func<object, List<long>>)((id) => (_p0_) => collect_type_var_ids(body(acc), _p0_)))(_mForAllTy0_.Field0)))(_mForAllTy0_.Field1) : (_scrutinee0_ is ConstructedTy _mConstructedTy0_ ? ((Func<object, List<long>>)((args) => ((Func<object, List<long>>)((name) => collect_type_var_ids_list(args(acc))))(_mConstructedTy0_.Field0)))(_mConstructedTy0_.Field1) : acc())))))))(ty)(collect_type_var_ids_list);
    }

    public static T736 List<T736>()
    {
        return throw new InvalidOperationException("->")(List)(Integer);
    }

    public static T736 List<T736>()
    {
        return collect_type_var_ids_list(types)(acc);
    }

    public static T972 collect_type_var_ids_list_loop<T972>()
    {
        return acc()(0L)(((long)types.Count));
    }

    public static T972 collect_type_var_ids_list_loop<T972>(object types, object acc, object i, object len)
    {
        while (true)
        {
            if ((i() == len))
            {
                return acc();
            }
            else
            {
                var _tco_0 = types(collect_type_var_ids(list_at()(types(i)), acc))((i() + 1L))(len);
                types = _tco_0;
                continue;
            }
        }
    }

    public static bool list_contains_int(List<long> xs, long n)
    {
        return (_p0_) => (_p1_) => (_p2_) => list_contains_int_loop(xs(n(0L)(((long)xs.Count))), _p0_, _p1_, _p2_);
    }

    public static bool list_contains_int_loop(List<long> xs, long n, long i, long len)
    {
        while (true)
        {
            if ((i() == len))
            {
                return false;
            }
            else
            {
                if ((list_at()(xs(i)) == n))
                {
                    return true;
                }
                else
                {
                    var _tco_0 = xs(n((i() + 1L))(len));
                    xs = _tco_0;
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
        return ((Func<string, string>)((ids) => ((((long)ids.Count) == 0L) ? "" : string.Concat("<", string.Concat((_p0_) => emit_type_params(ids(0L), _p0_), ">")))))((_p0_) => collect_type_var_ids(ty(new List<object>()), _p0_));
    }

    public static string emit_type_params(List<long> ids, long i)
    {
        return ((i() == ((long)ids.Count)) ? "" : ((i() == (((long)ids.Count) - 1L)) ? string.Concat("T", (list_at()(ids(i))).ToString()) : string.Concat("T", string.Concat((list_at()(ids(i))).ToString(), string.Concat(", ", (_p0_) => emit_type_params(ids((i() + 1L)), _p0_))))));
    }

    public static string emit_def(IRDef d)
    {
        return ((Func<string, string>)((ret) => ((Func<string, string>)((gen) => string.Concat("    public static ", string.Concat(cs_type(ret), string.Concat(" ", string.Concat(sanitize(d.name), string.Concat(gen, string.Concat("(", string.Concat(emit_def_params(d.@params(0L)), string.Concat(") => ", string.Concat(emit_expr(d.body), ";\\n")))))))))))(generic_suffix(d.type_val))))((_p0_) => get_return_type(d.type_val(((long)d.@params.Count)), _p0_));
    }

    public static CodexType get_return_type(CodexType ty, long n)
    {
        return ((n == 0L) ? strip_forall(ty) : (strip_forall(ty) is FunTy _mFunTy0_ ? ((Func<object, CodexType>)((r) => ((Func<object, CodexType>)((p) => (_p0_) => get_return_type(r((n - 1L)), _p0_)))(_mFunTy0_.Field0)))(_mFunTy0_.Field1) : ty)(strip_forall));
    }

    public static Func<string, Func<IRExpr, IRFieldVal>> CodexType()
    {
        return CodexType;
    }

    public static T1039 strip_forall<T1039>(object ty)
    {
        return (ty is ForAllTy _mForAllTy0_ ? ((Func<object, object>)((body) => ((Func<object, object>)((id) => strip_forall(body)))(_mForAllTy0_.Field0)))(_mForAllTy0_.Field1) : ty)(emit_def_params);
    }

    public static T736 List<T736>()
    {
        return throw new InvalidOperationException("->")(Integer);
    }

    public static T241 Text<T241>()
    {
        return emit_def_params(@params)(i);
    }

    public static long i()
    {
        return ((long)@params.Count);
    }

    public static a p()
    {
        return list_at()(@params(i));
    }

    public static Func<CodexType, string> cs_type()
    {
        return Enumerable.Concat(throw new InvalidOperationException(".")(new Func<CodexType, string>(type_val)), Enumerable.Concat(" ", Enumerable.Concat(sanitize(p.name), Enumerable.Concat(((i() < (((long)@params.Count) - 1L)) ? ", " : ""), emit_def_params(@params((i + 1L)))).ToList()).ToList()).ToList()).ToList();
    }

    public static string emit_full_module(IRModule m, List<ATypeDef> type_defs)
    {
        return string.Concat("using System;\\nusing System.Collections.Generic;\\nusing System.Linq;\\n\\n", string.Concat((_p0_) => emit_type_defs(type_defs(0L), _p0_), string.Concat(emit_class_header(m.name.value), string.Concat((_p0_) => emit_defs(m.defs(0L), _p0_), "}\\n"))));
    }

    public static string emit_module(IRModule m)
    {
        return string.Concat("using System;\\nusing System.Collections.Generic;\\nusing System.Linq;\\n\\n", string.Concat(emit_class_header(m.name.value), string.Concat((_p0_) => emit_defs(m.defs(0L), _p0_), "}\\n")));
    }

    public static string emit_class_header(string name)
    {
        return string.Concat("public static class Codex_", string.Concat(sanitize(name), "\\n{\\n"));
    }

    public static string emit_defs(List<IRDef> defs, long i)
    {
        return ((i() == ((long)defs.Count)) ? "" : string.Concat(emit_def(list_at()(defs(i))), string.Concat("\\n", (_p0_) => emit_defs(defs((i() + 1L)), _p0_))));
    }

    public static CodexType lookup_type(List<TypeBinding> bindings, string name)
    {
        return (_p0_) => (_p1_) => (_p2_) => lookup_type_loop(bindings(name(0L)(((long)bindings.Count))), _p0_, _p1_, _p2_);
    }

    public static CodexType lookup_type_loop(List<TypeBinding> bindings, string name, long i, long len)
    {
        while (true)
        {
            if ((i() == len))
            {
                return new ErrorTy();
            }
            else
            {
                var b = list_at()(bindings(i));
                if ((b().name == name))
                {
                    return b().bound_type;
                }
                else
                {
                    var _tco_0 = bindings(name((i() + 1L))(len));
                    bindings = _tco_0;
                    continue;
                }
            }
        }
    }

    public static CodexType peel_fun_param(CodexType ty)
    {
        return ((Func<CodexType, CodexType>)((_scrutinee0_) => (_scrutinee0_ is FunTy _mFunTy0_ ? ((Func<object, CodexType>)((r) => ((Func<object, CodexType>)((p) => p()))(_mFunTy0_.Field0)))(_mFunTy0_.Field1) : (_scrutinee0_ is ForAllTy _mForAllTy0_ ? ((Func<object, CodexType>)((body) => ((Func<object, CodexType>)((id) => peel_fun_param(body)))(_mForAllTy0_.Field0)))(_mForAllTy0_.Field1) : new ErrorTy()))))(ty)(peel_fun_return);
    }

    public static Func<string, Func<IRExpr, IRFieldVal>> CodexType()
    {
        return CodexType;
    }

    public static T1103 peel_fun_return<T1103>(object ty)
    {
        return ((Func<object, object>)((_scrutinee0_) => (_scrutinee0_ is FunTy _mFunTy0_ ? ((Func<object, object>)((r) => ((Func<object, object>)((p) => r))(_mFunTy0_.Field0)))(_mFunTy0_.Field1) : (_scrutinee0_ is ForAllTy _mForAllTy0_ ? ((Func<object, object>)((body) => ((Func<object, object>)((id) => peel_fun_return(body)))(_mForAllTy0_.Field0)))(_mForAllTy0_.Field1) : new ErrorTy()))))(ty)(strip_forall_ty);
    }

    public static Func<string, Func<IRExpr, IRFieldVal>> CodexType()
    {
        return CodexType;
    }

    public static CodexType strip_forall_ty(object ty)
    {
        return (ty is ForAllTy _mForAllTy0_ ? ((Func<object, object>)((body) => ((Func<object, object>)((id) => strip_forall_ty(body)))(_mForAllTy0_.Field0)))(_mForAllTy0_.Field1) : ty)(lower_bin_op);
    }

    public static Func<string, Func<IRExpr, IRFieldVal>> BinaryOp()
    {
        return CodexType;
    }

    public static T1113 IRBinaryOp<T1113>()
    {
        return lower_bin_op(op)(ty);
    }

    public static IRBinaryOp op()
    {
        return (new OpAdd() ? new IrAddInt() : new OpSub());
    }

    public static IRBinaryOp IrSubInt()
    {
        return (new OpMul() ? new IrMulInt() : new OpDiv());
    }

    public static IRBinaryOp IrDivInt()
    {
        return (new OpPow() ? new IrPowInt() : new OpEq());
    }

    public static IRBinaryOp IrEq()
    {
        return (new OpNotEq() ? new IrNotEq() : new OpLt());
    }

    public static IRBinaryOp IrLt()
    {
        return (new OpGt() ? new IrGt() : new OpLtEq());
    }

    public static IRBinaryOp IrLtEq()
    {
        return (new OpGtEq() ? new IrGtEq() : new OpDefEq());
    }

    public static IRBinaryOp IrEq()
    {
        return (new OpAppend() ? (is_text_type(ty) ? new IrAppendText() : new IrAppendList()) : new OpCons());
    }

    public static IRBinaryOp IrConsList()
    {
        return (new OpAnd() ? new IrAnd() : new OpOr());
    }

    public static Func<object, bool> IrOr()
    {
        return is_text_type;
    }

    public static Func<string, Func<IRExpr, IRFieldVal>> CodexType()
    {
        return Boolean;
    }

    public static T1128 is_text_type<T1128>(object ty)
    {
        return (ty is TextTy _mTextTy0_ ? true : false)(lower_expr);
    }

    public static Func<string, Func<IRExpr, IRFieldVal>> AExpr()
    {
        return CodexType;
    }

    public static T241 IRExpr<T241>()
    {
        return lower_expr(e)(ty);
    }

    public static Func<LiteralKind, IRExpr> e()
    {
        return (new ALitExpr(text, kind()) ? (_p0_) => lower_literal(text(new Func<LiteralKind, IRExpr>(kind)), _p0_) : new ANameExpr(name));
    }

    public static T1141 IrName<T1141>()
    {
        return throw new InvalidOperationException(".")(value(ty));
    }

    public static T1147 AApplyExpr<T1147>(object f, object a)
    {
        return (_p0_) => (_p1_) => lower_apply(f(a(ty)), _p0_, _p1_);
    }

    public static IRExpr ABinaryExpr(object l, object op, object r)
    {
        return new IrBinary(lower_bin_op(op()(ty)), lower_expr(l(ty)), lower_expr(r(ty)), ty);
    }

    public static IRExpr AUnaryExpr(object operand)
    {
        return new IrNegate(lower_expr(operand(IntegerTy)));
    }

    public static IRExpr AIfExpr(object c, object t, object e2)
    {
        return new IrIf(lower_expr(c(BooleanTy)), lower_expr(t(ty)), lower_expr(e2(ty)), ty);
    }

    public static IRExpr ALetExpr(AExpr binds, CodexType body)
    {
        return (_p0_) => (_p1_) => lower_let(binds(body(ty)), _p0_, _p1_);
    }

    public static IRExpr ALambdaExpr(AExpr @params, CodexType body)
    {
        return (_p0_) => (_p1_) => lower_lambda(@params(body(ty)), _p0_, _p1_);
    }

    public static T1198 AMatchExpr<T1198>(object scrut, object arms)
    {
        return (_p0_) => (_p1_) => lower_match(scrut(arms(ty)), _p0_, _p1_);
    }

    public static T1202 AListExpr<T1202>(object elems)
    {
        return lower_list(elems()(ty));
    }

    public static IRExpr ARecordExpr(List<AFieldExpr> name, CodexType fields)
    {
        return (_p0_) => (_p1_) => lower_record(name(fields(ty)), _p0_, _p1_);
    }

    public static IRExpr AFieldAccess(CodexType rec, object field)
    {
        return new IrFieldAccess(lower_expr(rec(ty)), field.value(ty));
    }

    public static IRExpr ADoExpr(CodexType stmts)
    {
        return (_p0_) => lower_do(stmts(ty), _p0_);
    }

    public static IRExpr AErrorExpr(CodexType msg)
    {
        return new IrError(msg(ty));
    }

    public static IRExpr lower_literal(string text, LiteralKind kind)
    {
        return ((Func<IRExpr, IRExpr>)((_scrutinee0_) => (_scrutinee0_ is IntLit _mIntLit0_ ? new IrIntLit(long.Parse(text)) : (_scrutinee0_ is NumLit _mNumLit0_ ? new IrIntLit(long.Parse(text)) : (_scrutinee0_ is TextLit _mTextLit0_ ? new IrTextLit(text) : (_scrutinee0_ is BoolLit _mBoolLit0_ ? new IrBoolLit((text == "True")) : throw new InvalidOperationException("Non-exhaustive match")))))))(kind())(lower_apply);
    }

    public static Func<string, Func<IRExpr, IRFieldVal>> AExpr()
    {
        return AExpr;
    }

    public static Func<string, Func<IRExpr, IRFieldVal>> CodexType()
    {
        return IRExpr;
    }

    public static IRExpr lower_apply(object f, object a, object ty)
    {
        return new IrApply(lower_expr(f(ty)), lower_expr(a(ty)), ty);
    }

    public static IRExpr lower_let(List<ALetBind> binds, AExpr body, CodexType ty)
    {
        return ((((long)binds.Count) == 0L) ? lower_expr(body(ty)) : ((Func<IRExpr, IRExpr>)((b) => new IrLet(b().name.value(ty(lower_expr(b().value(ErrorTy)))((_p0_) => (_p1_) => (_p2_) => lower_let_rest(binds(body(ty(1L))), _p0_, _p1_, _p2_))))))(list_at()(binds(0L))));
    }

    public static IRExpr lower_let_rest(List<ALetBind> binds, AExpr body, CodexType ty, long i)
    {
        return ((i() == ((long)binds.Count)) ? lower_expr(body(ty)) : ((Func<IRExpr, IRExpr>)((b) => new IrLet(b().name.value(ty(lower_expr(b().value(ErrorTy)))((_p0_) => (_p1_) => (_p2_) => lower_let_rest(binds(body(ty((i() + 1L)))), _p0_, _p1_, _p2_))))))(list_at()(binds(i))));
    }

    public static IRExpr lower_lambda(List<Name> @params, AExpr body, CodexType ty)
    {
        return ((Func<IRExpr, IRExpr>)((stripped) => new IrLambda((_p0_) => (_p1_) => lower_lambda_params(@params(stripped(0L)), _p0_, _p1_), lower_expr(body((_p0_) => get_lambda_return(stripped(((long)@params.Count)), _p0_))), ty)))(strip_forall_ty(ty));
    }

    public static List<IRParam> lower_lambda_params(List<Name> @params, CodexType ty, long i)
    {
        return ((i() == ((long)@params.Count)) ? new List<IRParam>() : ((Func<List<IRParam>, List<IRParam>>)((p) => ((Func<List<IRParam>, List<IRParam>>)((param_ty) => ((Func<List<IRParam>, List<IRParam>>)((rest_ty) => Enumerable.Concat(new List<IRParam>() { new IRParam(p().value, param_ty) }, (_p0_) => (_p1_) => lower_lambda_params(@params(rest_ty((i() + 1L))), _p0_, _p1_)).ToList()))(peel_fun_return(ty))))(peel_fun_param(ty))))(list_at()(@params(i))));
    }

    public static CodexType get_lambda_return(CodexType ty, long n)
    {
        return ((n == 0L) ? ty : (ty is FunTy _mFunTy0_ ? ((Func<object, CodexType>)((r) => ((Func<object, CodexType>)((p) => (_p0_) => get_lambda_return(r((n - 1L)), _p0_)))(_mFunTy0_.Field0)))(_mFunTy0_.Field1) : new ErrorTy())(lower_match));
    }

    public static Func<string, Func<IRExpr, IRFieldVal>> AExpr()
    {
        return new List(AMatchArm);
    }

    public static Func<string, Func<IRExpr, IRFieldVal>> CodexType()
    {
        return IRExpr;
    }

    public static IRExpr lower_match(object scrut, object arms, object ty)
    {
        return new IrMatch(lower_expr(scrut(ty)), map_list((_p0_) => lower_arm(ty, _p0_), arms), ty);
    }

    public static IRBranch lower_arm(CodexType ty, AMatchArm arm)
    {
        return new IRBranch(lower_pattern(arm.pattern), lower_expr(arm.body(ty)));
    }

    public static IRPat lower_pattern(APat p)
    {
        return ((Func<IRPat, IRPat>)((_scrutinee0_) => (_scrutinee0_ is AVarPat _mAVarPat0_ ? ((Func<object, IRPat>)((name) => new IrVarPat(name.value(ErrorTy))))(_mAVarPat0_.Field0) : (_scrutinee0_ is ALitPat _mALitPat0_ ? ((Func<object, IRPat>)((kind) => ((Func<object, IRPat>)((text) => new IrLitPat(text(ErrorTy))))(_mALitPat0_.Field0)))(_mALitPat0_.Field1) : (_scrutinee0_ is ACtorPat _mACtorPat0_ ? ((Func<object, IRPat>)((subs) => ((Func<object, IRPat>)((name) => new IrCtorPat(name.value((_p0_) => map_list(lower_pattern(subs), _p0_))(ErrorTy))))(_mACtorPat0_.Field0)))(_mACtorPat0_.Field1) : (_scrutinee0_ is AWildPat _mAWildPat0_ ? new IrWildPat() : throw new InvalidOperationException("Non-exhaustive match")))))))(p())(lower_list);
    }

    public static T736 List<T736>()
    {
        return throw new InvalidOperationException("->")(CodexType);
    }

    public static T241 IRExpr<T241>()
    {
        return lower_list(elems)(ty);
    }

    public static CodexType elem_ty()
    {
        return (ty is ListTy _mListTy0_ ? ((Func<object, CodexType>)((e) => e()))(_mListTy0_.Field0) : new ErrorTy());
    }

    public static T90 IrList<T90>(object map_list)
    {
        return elem_ty();
    }

    public static T90 elems<T90>()
    {
        return elem_ty();
    }

    public static IRExpr lower_elem(CodexType ty, AExpr e)
    {
        return lower_expr(e()(ty));
    }

    public static IRExpr lower_record(Name name, List<AFieldExpr> fields, CodexType ty)
    {
        return new IrRecord(name.value(map_list((_p0_) => lower_field_val(ty, _p0_), fields))(ty));
    }

    public static IRFieldVal lower_field_val(CodexType ty, AFieldExpr f)
    {
        return new IRFieldVal(f.name.value, lower_expr(f.value(ty)));
    }

    public static IRExpr lower_do(List<ADoStmt> stmts, CodexType ty)
    {
        return new IrDo(map_list((_p0_) => lower_do_stmt(ty, _p0_), stmts), ty);
    }

    public static IRDoStmt lower_do_stmt(CodexType ty, ADoStmt s)
    {
        return ((Func<IRDoStmt, IRDoStmt>)((_scrutinee0_) => (_scrutinee0_ is ADoBindStmt _mADoBindStmt0_ ? ((Func<object, IRDoStmt>)((val) => ((Func<object, IRDoStmt>)((name) => new IrDoBind(name.value(ty(lower_expr(val(ty)))))))(_mADoBindStmt0_.Field0)))(_mADoBindStmt0_.Field1) : (_scrutinee0_ is ADoExprStmt _mADoExprStmt0_ ? ((Func<object, IRDoStmt>)((e) => new IrDoExec(lower_expr(e()(ty)))))(_mADoExprStmt0_.Field0) : throw new InvalidOperationException("Non-exhaustive match")))))(s)(lower_def);
    }

    public static T1387 ADef<T1387>()
    {
        return new List(new TypeBinding());
    }

    public static Func<string, Func<List<IRParam>, Func<CodexType, Func<IRExpr, IRDef>>>> UnificationState()
    {
        return IRDef;
    }

    public static IRDef lower_def(object d, object types, object ust)
    {
        return ((Func<object, object>)((raw_type) => ((Func<object, object>)((full_type) => ((Func<object, object>)((stripped) => ((Func<object, object>)((@params) => ((Func<object, object>)((ret_type) => new IRDef(d.name.value, @params, full_type, lower_expr(d.body(ret_type)))))((_p0_) => get_return_type_n(stripped(((long)d.@params.Count)), _p0_))))((_p0_) => (_p1_) => lower_def_params(d.@params(stripped(0L)), _p0_, _p1_))))(strip_forall_ty(full_type))))((_p0_) => deep_resolve(ust(raw_type), _p0_))))((_p0_) => lookup_type(types(d.name.value), _p0_));
    }

    public static List<IRParam> lower_def_params(List<AParam> @params, CodexType ty, long i)
    {
        return ((i() == ((long)@params.Count)) ? new List<IRParam>() : ((Func<List<IRParam>, List<IRParam>>)((p) => ((Func<List<IRParam>, List<IRParam>>)((param_ty) => ((Func<List<IRParam>, List<IRParam>>)((rest_ty) => Enumerable.Concat(new List<IRParam>() { new IRParam(p().name.value, param_ty) }, (_p0_) => (_p1_) => lower_def_params(@params(rest_ty((i() + 1L))), _p0_, _p1_)).ToList()))(peel_fun_return(ty))))(peel_fun_param(ty))))(list_at()(@params(i))));
    }

    public static CodexType get_return_type_n(CodexType ty, long n)
    {
        return ((n == 0L) ? ty : (ty is FunTy _mFunTy0_ ? ((Func<object, CodexType>)((r) => ((Func<object, CodexType>)((p) => (_p0_) => get_return_type_n(r((n - 1L)), _p0_)))(_mFunTy0_.Field0)))(_mFunTy0_.Field1) : new ErrorTy())(lower_module));
    }

    public static T1431 AModule<T1431>()
    {
        return new List(new TypeBinding());
    }

    public static Func<string, Func<List<IRParam>, Func<CodexType, Func<IRExpr, IRDef>>>> UnificationState()
    {
        return IRModule;
    }

    public static IRModule lower_module(object m, object types, object ust)
    {
        return new IRModule(m.name, (_p0_) => (_p1_) => (_p2_) => lower_defs(m.defs(types(ust(0L))), _p0_, _p1_, _p2_));
    }

    public static List<IRDef> lower_defs(List<ADef> defs, List<TypeBinding> types, UnificationState ust, long i)
    {
        return ((i() == ((long)defs.Count)) ? new List<IRDef>() : Enumerable.Concat(new List<IRDef>() { (_p0_) => lower_def(list_at()(defs(i)), types(ust), _p0_) }, (_p0_) => (_p1_) => (_p2_) => lower_defs(defs(types(ust((i() + 1L)))), _p0_, _p1_, _p2_)).ToList());
    }

    public static Scope empty_scope()
    {
        return new Scope(new List<object>());
    }

    public static bool scope_has(Scope sc, string name)
    {
        return (_p0_) => (_p1_) => (_p2_) => scope_has_loop(sc().names(name(0L)(((long)sc().names.Count))), _p0_, _p1_, _p2_);
    }

    public static bool scope_has_loop(List<string> names, string name, long i, long len)
    {
        while (true)
        {
            if ((i() == len))
            {
                return false;
            }
            else
            {
                if ((list_at()(names(i)) == name))
                {
                    return true;
                }
                else
                {
                    var _tco_0 = names(name((i() + 1L))(len));
                    names = _tco_0;
                    continue;
                }
            }
        }
    }

    public static Scope scope_add(Scope sc, string name)
    {
        return new Scope(Enumerable.Concat(new List<object>() { name }, sc().names).ToList());
    }

    public static List<string> builtin_names()
    {
        return new List<string>() { "show", "negate", "True", "False", "Nothing", "print-line", "read-line", "open-file", "read-all", "close-file", "char-at", "text-length", "substring", "is-letter", "is-digit", "is-whitespace", "text-to-integer", "integer-to-text", "text-replace", "char-code", "code-to-char", "list-length", "list-at", "map", "filter", "fold" };
    }

    public static bool is_type_name(string name)
    {
        return ((((long)name.Length) == 0L) ? false : ((char_at(name(0L)).Length > 0 && char.IsLetter(char_at(name(0L))[0])) && is_upper_char(char_at(name(0L)))));
    }

    public static bool is_upper_char(string c)
    {
        return ((Func<bool, bool>)((code) => ((code >= 65L) && (code <= 90L))))(((long)c[0]));
    }

    public static CollectResult collect_top_level_names(List<ADef> defs, long i, long len, List<string> acc, List<Diagnostic> errs)
    {
        while (true)
        {
            if ((i() == len))
            {
                return new CollectResult(acc(), errs());
            }
            else
            {
                var def = list_at()(defs(i));
                var name = def.name.value;
                if ((_p0_) => list_contains(acc()(name), _p0_))
                {
                    var _tco_0 = defs((i() + 1L))(len(acc()(Enumerable.Concat(errs(), new List<object>() { make_error("CDX3001", Enumerable.Concat("Duplicate definition: ", name).ToList()) }).ToList())));
                    defs = _tco_0;
                    continue;
                }
                else
                {
                    var _tco_0 = defs((i() + 1L))(len(Enumerable.Concat(acc(), new List<object>() { name }).ToList())(errs));
                    defs = _tco_0;
                    continue;
                }
            }
        }
    }

    public static bool list_contains(List<string> xs, string name)
    {
        return (_p0_) => (_p1_) => (_p2_) => list_contains_loop(xs(name(0L)(((long)xs.Count))), _p0_, _p1_, _p2_);
    }

    public static bool list_contains_loop(List<string> xs, string name, long i, long len)
    {
        while (true)
        {
            if ((i() == len))
            {
                return false;
            }
            else
            {
                if ((list_at()(xs(i)) == name))
                {
                    return true;
                }
                else
                {
                    var _tco_0 = xs(name((i() + 1L))(len));
                    xs = _tco_0;
                    continue;
                }
            }
        }
    }

    public static CtorCollectResult collect_ctor_names(List<ATypeDef> type_defs, long i, long len, List<string> type_acc, List<string> ctor_acc)
    {
        return ((i() == len) ? new CtorCollectResult(type_acc, ctor_acc) : ((Func<CtorCollectResult, CtorCollectResult>)((td) => ((Func<CtorCollectResult, CtorCollectResult>)((_scrutinee0_) => (_scrutinee0_ is AVariantTypeDef _mAVariantTypeDef0_ ? ((Func<object, CtorCollectResult>)((ctors) => ((Func<object, CtorCollectResult>)((@params) => ((Func<object, CtorCollectResult>)((name) => ((Func<CtorCollectResult, CtorCollectResult>)((new_type_acc) => ((Func<CtorCollectResult, CtorCollectResult>)((new_ctor_acc) => (_p0_) => (_p1_) => (_p2_) => (_p3_) => collect_ctor_names(type_defs((i() + 1L))(len(new_type_acc(new_ctor_acc))), _p0_, _p1_, _p2_, _p3_)))((_p0_) => (_p1_) => (_p2_) => collect_variant_ctors(ctors(0L)(((long)ctors.Count))(ctor_acc), _p0_, _p1_, _p2_))))(Enumerable.Concat(type_acc, new List<object>() { name.value }).ToList())))(_mAVariantTypeDef0_.Field0)))(_mAVariantTypeDef0_.Field1)))(_mAVariantTypeDef0_.Field2) : (_scrutinee0_ is ARecordTypeDef _mARecordTypeDef0_ ? ((Func<object, CtorCollectResult>)((fields) => ((Func<object, CtorCollectResult>)((@params) => ((Func<object, CtorCollectResult>)((name) => (_p0_) => (_p1_) => (_p2_) => (_p3_) => collect_ctor_names(type_defs((i() + 1L))(len(Enumerable.Concat(type_acc, new List<object>() { name.value }).ToList())(ctor_acc)), _p0_, _p1_, _p2_, _p3_)))(_mARecordTypeDef0_.Field0)))(_mARecordTypeDef0_.Field1)))(_mARecordTypeDef0_.Field2) : throw new InvalidOperationException("Non-exhaustive match")))))(td)(CtorCollectResult)))(list_at()(type_defs(i))));
    }

    public static List<string> ,()
    {
        return ctor_names;
    }

    public static T736 List<T736>()
    {
        return throw new InvalidOperationException("}");
    }

    public static List<string> collect_variant_ctors(List<AVariantCtorDef> ctors, long i, long len, List<string> acc)
    {
        while (true)
        {
            if ((i() == len))
            {
                return acc();
            }
            else
            {
                var ctor = list_at()(ctors(i));
                var _tco_0 = ctors((i() + 1L))(len(Enumerable.Concat(acc(), new List<string>() { ctor().name.value }).ToList()));
                ctors = _tco_0;
                continue;
            }
        }
    }

    public static Scope build_all_names_scope(List<string> top_names, List<string> ctor_names, List<string> builtins)
    {
        return ((Func<Scope, Scope>)((sc) => ((Func<Scope, Scope>)((sc2) => (_p0_) => (_p1_) => (_p2_) => add_names_to_scope(sc2()(builtins(0L)(((long)builtins.Count))), _p0_, _p1_, _p2_)))((_p0_) => (_p1_) => (_p2_) => add_names_to_scope(sc()(ctor_names(0L)(((long)ctor_names.Count))), _p0_, _p1_, _p2_))))((_p0_) => (_p1_) => (_p2_) => add_names_to_scope(empty_scope()(top_names(0L)(((long)top_names.Count))), _p0_, _p1_, _p2_));
    }

    public static Scope add_names_to_scope(Scope sc, List<string> names, long i, long len)
    {
        while (true)
        {
            if ((i() == len))
            {
                return sc();
            }
            else
            {
                var _tco_0 = (_p0_) => scope_add(sc()(list_at()(names(i))), _p0_);
                var _tco_1 = names((i() + 1L))(len);
                sc = _tco_0;
                names = _tco_1;
                continue;
            }
        }
    }

    public static List<Diagnostic> resolve_expr(Scope sc, AExpr expr)
    {
        return ((Func<List<Diagnostic>, List<Diagnostic>>)((_scrutinee0_) => (_scrutinee0_ is ALitExpr _mALitExpr0_ ? ((Func<object, List<Diagnostic>>)((kind) => ((Func<object, List<Diagnostic>>)((val) => new List<Diagnostic>()))(_mALitExpr0_.Field0)))(_mALitExpr0_.Field1) : (_scrutinee0_ is ANameExpr _mANameExpr0_ ? ((Func<object, List<Diagnostic>>)((name) => (((_p0_) => scope_has(sc()(name.value), _p0_) || is_type_name(name.value)) ? new List<Diagnostic>() : new List<Diagnostic>() { make_error("CDX3002", Enumerable.Concat("Undefined name: ", name.value).ToList()) })))(_mANameExpr0_.Field0) : (_scrutinee0_ is ABinaryExpr _mABinaryExpr0_ ? ((Func<object, List<Diagnostic>>)((right) => ((Func<object, List<Diagnostic>>)((op) => ((Func<object, List<Diagnostic>>)((left) => Enumerable.Concat((_p0_) => resolve_expr(sc()(left), _p0_), (_p0_) => resolve_expr(sc()(right), _p0_)).ToList()))(_mABinaryExpr0_.Field0)))(_mABinaryExpr0_.Field1)))(_mABinaryExpr0_.Field2) : (_scrutinee0_ is AUnaryExpr _mAUnaryExpr0_ ? ((Func<object, List<Diagnostic>>)((operand) => (_p0_) => resolve_expr(sc()(operand), _p0_)))(_mAUnaryExpr0_.Field0) : (_scrutinee0_ is AApplyExpr _mAApplyExpr0_ ? ((Func<object, List<Diagnostic>>)((arg) => ((Func<object, List<Diagnostic>>)((func) => Enumerable.Concat((_p0_) => resolve_expr(sc()(func), _p0_), (_p0_) => resolve_expr(sc()(arg), _p0_)).ToList()))(_mAApplyExpr0_.Field0)))(_mAApplyExpr0_.Field1) : (_scrutinee0_ is AIfExpr _mAIfExpr0_ ? ((Func<object, List<Diagnostic>>)((else_e) => ((Func<object, List<Diagnostic>>)((then_e) => ((Func<object, List<Diagnostic>>)((cond) => Enumerable.Concat((_p0_) => resolve_expr(sc()(cond), _p0_), Enumerable.Concat((_p0_) => resolve_expr(sc()(then_e), _p0_), (_p0_) => resolve_expr(sc()(else_e), _p0_)).ToList()).ToList()))(_mAIfExpr0_.Field0)))(_mAIfExpr0_.Field1)))(_mAIfExpr0_.Field2) : (_scrutinee0_ is ALetExpr _mALetExpr0_ ? ((Func<object, List<Diagnostic>>)((body) => ((Func<object, List<Diagnostic>>)((bindings) => resolve_let()(sc()(bindings(body(0L)(((long)bindings.Count))(new List<Diagnostic>()))))))(_mALetExpr0_.Field0)))(_mALetExpr0_.Field1) : (_scrutinee0_ is ALambdaExpr _mALambdaExpr0_ ? ((Func<object, List<Diagnostic>>)((body) => ((Func<object, List<Diagnostic>>)((@params) => ((Func<List<Diagnostic>, List<Diagnostic>>)((sc2) => (_p0_) => resolve_expr(sc2()(body), _p0_)))((_p0_) => (_p1_) => (_p2_) => add_lambda_params(sc()(@params(0L)(((long)@params.Count))), _p0_, _p1_, _p2_))))(_mALambdaExpr0_.Field0)))(_mALambdaExpr0_.Field1) : (_scrutinee0_ is AMatchExpr _mAMatchExpr0_ ? ((Func<object, List<Diagnostic>>)((arms) => ((Func<object, List<Diagnostic>>)((scrutinee) => Enumerable.Concat((_p0_) => resolve_expr(sc()(scrutinee), _p0_), (_p0_) => (_p1_) => (_p2_) => (_p3_) => resolve_match_arms(sc()(arms(0L)(((long)arms.Count))(new List<Diagnostic>())), _p0_, _p1_, _p2_, _p3_)).ToList()))(_mAMatchExpr0_.Field0)))(_mAMatchExpr0_.Field1) : (_scrutinee0_ is AListExpr _mAListExpr0_ ? ((Func<object, List<Diagnostic>>)((elems) => (_p0_) => (_p1_) => (_p2_) => (_p3_) => resolve_list_elems(sc()(elems()(0L)(((long)elems().Count))(new List<Diagnostic>())), _p0_, _p1_, _p2_, _p3_)))(_mAListExpr0_.Field0) : (_scrutinee0_ is ARecordExpr _mARecordExpr0_ ? ((Func<object, List<Diagnostic>>)((fields) => ((Func<object, List<Diagnostic>>)((name) => (_p0_) => (_p1_) => (_p2_) => (_p3_) => resolve_record_fields(sc()(fields(0L)(((long)fields.Count))(new List<Diagnostic>())), _p0_, _p1_, _p2_, _p3_)))(_mARecordExpr0_.Field0)))(_mARecordExpr0_.Field1) : (_scrutinee0_ is AFieldAccess _mAFieldAccess0_ ? ((Func<object, List<Diagnostic>>)((field) => ((Func<object, List<Diagnostic>>)((obj) => (_p0_) => resolve_expr(sc()(obj), _p0_)))(_mAFieldAccess0_.Field0)))(_mAFieldAccess0_.Field1) : (_scrutinee0_ is ADoExpr _mADoExpr0_ ? ((Func<object, List<Diagnostic>>)((stmts) => (_p0_) => (_p1_) => (_p2_) => (_p3_) => resolve_do_stmts(sc()(stmts(0L)(((long)stmts.Count))(new List<Diagnostic>())), _p0_, _p1_, _p2_, _p3_)))(_mADoExpr0_.Field0) : (_scrutinee0_ is AErrorExpr _mAErrorExpr0_ ? ((Func<object, List<Diagnostic>>)((msg) => new List<Diagnostic>()))(_mAErrorExpr0_.Field0) : throw new InvalidOperationException("Non-exhaustive match")))))))))))))))))(expr)(resolve_let);
    }

    public static T1671 Scope<T1671>()
    {
        return new List(new ALetBind());
    }

    public static Func<string, Func<IRExpr, IRFieldVal>> AExpr()
    {
        return Integer;
    }

    public static T316 Integer<T316>()
    {
        return new List(new Diagnostic());
    }

    public static T736 List<T736>()
    {
        return resolve_let()(sc)(bindings)(body)(i)(len)(errs);
    }

    public static long i()
    {
        return len;
    }

    public static Func<AExpr, List<Diagnostic>> errs()
    {
        return (_p0_) => resolve_expr(sc(new Func<AExpr, List<Diagnostic>>(body)), _p0_);
    }

    public static a b()
    {
        return list_at()(bindings(i));
    }

    public static Func<AExpr, List<Diagnostic>> bind_errs()
    {
        return (_p0_) => resolve_expr(sc(b.value), _p0_);
    }

    public static Func<string, Scope> sc2()
    {
        return (_p0_) => scope_add(sc(b.name.value), _p0_);
    }

    public static T1702 resolve_let<T1702>()
    {
        return bindings(body((i() + 1L))(len(Enumerable.Concat(errs(), bind_errs()).ToList())));
    }

    public static Scope add_lambda_params(Scope sc, List<Name> @params, long i, long len)
    {
        while (true)
        {
            if ((i() == len))
            {
                return sc();
            }
            else
            {
                var p = list_at()(@params(i));
                var _tco_0 = (_p0_) => scope_add(sc()(p().value), _p0_);
                var _tco_1 = @params((i() + 1L))(len);
                sc = _tco_0;
                @params = _tco_1;
                continue;
            }
        }
    }

    public static List<Diagnostic> resolve_match_arms(Scope sc, List<AMatchArm> arms, long i, long len, List<Diagnostic> errs)
    {
        while (true)
        {
            if ((i() == len))
            {
                return errs();
            }
            else
            {
                var arm = list_at()(arms(i));
                var sc2 = (_p0_) => collect_pattern_names(sc()(arm.pattern), _p0_);
                var arm_errs = (_p0_) => resolve_expr(sc2()(arm.body), _p0_);
                var _tco_0 = sc()(arms((i() + 1L))(len(Enumerable.Concat(errs(), arm_errs).ToList())));
                sc = _tco_0;
                continue;
            }
        }
    }

    public static Scope collect_pattern_names(Scope sc, APat pat)
    {
        return ((Func<Scope, Scope>)((_scrutinee0_) => (_scrutinee0_ is AVarPat _mAVarPat0_ ? ((Func<object, Scope>)((name) => (_p0_) => scope_add(sc()(name.value), _p0_)))(_mAVarPat0_.Field0) : (_scrutinee0_ is ACtorPat _mACtorPat0_ ? ((Func<object, Scope>)((subs) => ((Func<object, Scope>)((name) => collect_ctor_pat_names(sc()(subs(0L)(((long)subs.Count))))))(_mACtorPat0_.Field0)))(_mACtorPat0_.Field1) : (_scrutinee0_ is ALitPat _mALitPat0_ ? ((Func<object, Scope>)((kind) => ((Func<object, Scope>)((val) => sc()))(_mALitPat0_.Field0)))(_mALitPat0_.Field1) : (_scrutinee0_ is AWildPat _mAWildPat0_ ? sc() : throw new InvalidOperationException("Non-exhaustive match")))))))(pat)(collect_ctor_pat_names);
    }

    public static T1671 Scope<T1671>()
    {
        return new List(new APat());
    }

    public static T316 Integer<T316>()
    {
        return new Integer();
    }

    public static T1671 Scope<T1671>()
    {
        return collect_ctor_pat_names(sc)(subs)(i)(len);
    }

    public static long i()
    {
        return len;
    }

    public static object sc()
    {
        return throw new InvalidOperationException("else");
    }

    public static a sub()
    {
        return list_at()(subs(i));
    }

    public static T1753 collect_ctor_pat_names<T1753>(object collect_pattern_names)
    {
        return throw new InvalidOperationException(")")(subs((i() + 1L))(len));
    }

    public static List<Diagnostic> resolve_list_elems(Scope sc, List<AExpr> elems, long i, long len, List<Diagnostic> errs)
    {
        while (true)
        {
            if ((i() == len))
            {
                return errs();
            }
            else
            {
                var errs2 = (_p0_) => resolve_expr(sc()(list_at()(elems()(i))), _p0_);
                var _tco_0 = sc()(elems()((i() + 1L))(len(Enumerable.Concat(errs(), errs2).ToList())));
                sc = _tco_0;
                continue;
            }
        }
    }

    public static List<Diagnostic> resolve_record_fields(Scope sc, List<AFieldExpr> fields, long i, long len, List<Diagnostic> errs)
    {
        while (true)
        {
            if ((i() == len))
            {
                return errs();
            }
            else
            {
                var f = list_at()(fields(i));
                var errs2 = (_p0_) => resolve_expr(sc()(f.value), _p0_);
                var _tco_0 = sc()(fields((i() + 1L))(len(Enumerable.Concat(errs(), errs2).ToList())));
                sc = _tco_0;
                continue;
            }
        }
    }

    public static List<Diagnostic> resolve_do_stmts(Scope sc, List<ADoStmt> stmts, long i, long len, List<Diagnostic> errs)
    {
        return ((i() == len) ? errs() : ((Func<List<Diagnostic>, List<Diagnostic>>)((stmt) => ((Func<List<Diagnostic>, List<Diagnostic>>)((_scrutinee0_) => (_scrutinee0_ is ADoExprStmt _mADoExprStmt0_ ? ((Func<object, List<Diagnostic>>)((e) => ((Func<List<Diagnostic>, List<Diagnostic>>)((errs2) => (_p0_) => (_p1_) => (_p2_) => (_p3_) => resolve_do_stmts(sc()(stmts((i() + 1L))(len(Enumerable.Concat(errs(), errs2).ToList()))), _p0_, _p1_, _p2_, _p3_)))((_p0_) => resolve_expr(sc()(e), _p0_))))(_mADoExprStmt0_.Field0) : (_scrutinee0_ is ADoBindStmt _mADoBindStmt0_ ? ((Func<object, List<Diagnostic>>)((e) => ((Func<object, List<Diagnostic>>)((name) => ((Func<List<Diagnostic>, List<Diagnostic>>)((errs2) => ((Func<List<Diagnostic>, List<Diagnostic>>)((sc2) => (_p0_) => (_p1_) => (_p2_) => (_p3_) => resolve_do_stmts(sc2()(stmts((i() + 1L))(len(Enumerable.Concat(errs(), errs2).ToList()))), _p0_, _p1_, _p2_, _p3_)))((_p0_) => scope_add(sc()(name.value), _p0_))))((_p0_) => resolve_expr(sc()(e), _p0_))))(_mADoBindStmt0_.Field0)))(_mADoBindStmt0_.Field1) : throw new InvalidOperationException("Non-exhaustive match")))))(stmt)(resolve_all_defs)))(list_at()(stmts(i))));
    }

    public static T1671 Scope<T1671>()
    {
        return new List(new ADef());
    }

    public static T316 Integer<T316>()
    {
        return new Integer();
    }

    public static T736 List<T736>()
    {
        return throw new InvalidOperationException("->")(List)(Diagnostic);
    }

    public static T1823 resolve_all_defs<T1823>(object sc, object defs, object i, object len, object errs)
    {
        while (true)
        {
            if ((i() == len))
            {
                return errs();
            }
            else
            {
                var def = list_at()(defs(i));
                var def_scope = (_p0_) => (_p1_) => (_p2_) => add_def_params(sc()(def.@params(0L)(((long)def.@params.Count))), _p0_, _p1_, _p2_);
                var errs2 = (_p0_) => resolve_expr(def_scope(def.body), _p0_);
                var _tco_0 = sc()(defs((i() + 1L))(len(Enumerable.Concat(errs(), errs2).ToList())));
                sc = _tco_0;
                continue;
            }
        }
    }

    public static Scope add_def_params(Scope sc, List<AParam> @params, long i, long len)
    {
        while (true)
        {
            if ((i() == len))
            {
                return sc();
            }
            else
            {
                var p = list_at()(@params(i));
                var _tco_0 = (_p0_) => scope_add(sc()(p().name.value), _p0_);
                var _tco_1 = @params((i() + 1L))(len);
                sc = _tco_0;
                @params = _tco_1;
                continue;
            }
        }
    }

    public static ResolveResult resolve_module(AModule mod)
    {
        return ((Func<ResolveResult, ResolveResult>)((top) => ((Func<ResolveResult, ResolveResult>)((ctors) => ((Func<ResolveResult, ResolveResult>)((sc) => ((Func<ResolveResult, ResolveResult>)((expr_errs) => new ResolveResult(Enumerable.Concat(top.errors, expr_errs).ToList(), top.names, ctors.type_names, ctors.ctor_names)))((_p0_) => (_p1_) => (_p2_) => (_p3_) => resolve_all_defs(sc()(mod.defs(0L)(((long)mod.defs.Count))(new List<object>())), _p0_, _p1_, _p2_, _p3_))))((_p0_) => (_p1_) => build_all_names_scope(top.names(ctors.ctor_names(builtin_names)), _p0_, _p1_))))((_p0_) => (_p1_) => (_p2_) => (_p3_) => collect_ctor_names(mod.type_defs(0L)(((long)mod.type_defs.Count))(new List<object>())(new List<object>()), _p0_, _p1_, _p2_, _p3_))))((_p0_) => (_p1_) => (_p2_) => (_p3_) => collect_top_level_names(mod.defs(0L)(((long)mod.defs.Count))(new List<object>())(new List<object>()), _p0_, _p1_, _p2_, _p3_));
    }

    public static LexState make_lex_state(string src)
    {
        return new LexState(src, 0L, 1L, 1L);
    }

    public static bool is_at_end(LexState st)
    {
        return (st().offset >= ((long)st().source.Length));
    }

    public static string peek_char(LexState st)
    {
        return (is_at_end(st) ? "" : char_at(st().source(st().offset)));
    }

    public static LexState advance_char(LexState st)
    {
        return ((peek_char(st) == "\\n") ? new LexState(st().source, (st().offset + 1L), (st().line + 1L), 1L) : new LexState(st().source, (st().offset + 1L), st().line, (st().column + 1L)));
    }

    public static LexState skip_spaces(LexState st)
    {
        while (true)
        {
            if (is_at_end(st))
            {
                return st();
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
                    return st();
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
                return st();
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
                                    return st();
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
                                        return st();
                                    }
                                }
                            }
                            else
                            {
                                return st();
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
                return st();
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
                        return st();
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
                return st();
            }
            else
            {
                var ch = peek_char(st);
                if ((ch == "\\\""))
                {
                    return advance_char(st);
                }
                else
                {
                    if ((ch == "\\n"))
                    {
                        return st();
                    }
                    else
                    {
                        if ((ch == "\\\\"))
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
        return ((w == "let") ? new LetKeyword() : ((w == "in") ? new InKeyword() : ((w == "if") ? new IfKeyword() : ((w == "then") ? new ThenKeyword() : ((w == "else") ? new ElseKeyword() : ((w == "when") ? new WhenKeyword() : ((w == "where") ? new WhereKeyword() : ((w == "do") ? new DoKeyword() : ((w == "record") ? new RecordKeyword() : ((w == "import") ? new ImportKeyword() : ((w == "export") ? new ExportKeyword() : ((w == "claim") ? new ClaimKeyword() : ((w == "proof") ? new ProofKeyword() : ((w == "forall") ? new ForAllKeyword() : ((w == "exists") ? new ThereExistsKeyword() : ((w == "linear") ? new LinearKeyword() : ((w == "True") ? new TrueKeyword() : ((w == "False") ? new FalseKeyword() : ((Func<TokenKind, TokenKind>)((first_code) => ((first_code >= 65L) ? ((first_code <= 90L) ? new TypeIdentifier() : new Identifier()) : new Identifier())))(((long)char_at(w(0L))[0]))))))))))))))))))));
    }

    public static Token make_token(TokenKind kind, string text, LexState st)
    {
        return new Token(kind(), text, st().offset, st().line, st().column);
    }

    public static string extract_text(LexState st, long start, LexState end_st)
    {
        return substring(st().source(start((end_st.offset - start))));
    }

    public static LexResult scan_token(LexState st)
    {
        return ((Func<LexResult, LexResult>)((s) => (is_at_end(s) ? new LexEnd() : ((Func<LexResult, LexResult>)((ch) => ((ch == "\\n") ? new LexToken(make_token(Newline, "\\n", s), advance_char(s)) : ((ch == "\\\"") ? ((Func<LexResult, LexResult>)((start) => ((Func<LexResult, LexResult>)((after) => ((Func<LexResult, LexResult>)((text_len) => new LexToken(make_token(TextLiteral, substring(s.source(start(text_len))), s), after)))(((after.offset - start) - 1L))))(scan_string_body(advance_char(s)))))((s.offset + 1L)) : ((ch.Length > 0 && char.IsLetter(ch[0])) ? ((Func<LexResult, LexResult>)((start) => ((Func<LexResult, LexResult>)((after) => ((Func<LexResult, LexResult>)((word) => new LexToken((_p0_) => make_token(classify_word(word), word(s), _p0_), after)))((_p0_) => (_p1_) => extract_text(s(start(after)), _p0_, _p1_))))(scan_ident_rest(advance_char(s)))))(s.offset) : ((ch == "_") ? ((Func<LexResult, LexResult>)((start) => ((Func<LexResult, LexResult>)((after) => ((Func<LexResult, LexResult>)((word) => ((((long)word.Length) == 1L) ? new LexToken(make_token(Underscore, "_", s), after) : new LexToken((_p0_) => make_token(classify_word(word), word(s), _p0_), after))))((_p0_) => (_p1_) => extract_text(s(start(after)), _p0_, _p1_))))(scan_ident_rest(advance_char(s)))))(s.offset) : ((ch.Length > 0 && char.IsDigit(ch[0])) ? ((Func<LexResult, LexResult>)((start) => ((Func<LexResult, LexResult>)((after) => (is_at_end(after) ? new LexToken(make_token(IntegerLiteral, (_p0_) => (_p1_) => extract_text(s(start(after)), _p0_, _p1_), s), after) : ((peek_char(after) == ".") ? ((Func<LexResult, LexResult>)((after2) => new LexToken(make_token(NumberLiteral, (_p0_) => (_p1_) => extract_text(s(start(after2)), _p0_, _p1_), s), after2)))(scan_digits(advance_char(after))) : new LexToken(make_token(IntegerLiteral, (_p0_) => (_p1_) => extract_text(s(start(after)), _p0_, _p1_), s), after)))))(scan_digits(advance_char(s)))))(s.offset) : scan_operator(s))))))))(peek_char(s)))))(skip_spaces(st));
    }

    public static LexResult scan_operator(LexState s)
    {
        return ((Func<LexResult, LexResult>)((ch) => ((Func<LexResult, LexResult>)((next) => ((ch == "(") ? new LexToken(make_token(LeftParen, "(", s), next) : ((ch == ")") ? new LexToken(make_token(RightParen, ")", s), next) : ((ch == "[") ? new LexToken(make_token(LeftBracket, "[", s), next) : ((ch == "]") ? new LexToken(make_token(RightBracket, "]", s), next) : ((ch == "{") ? new LexToken(make_token(LeftBrace, "{", s), next) : ((ch == "}") ? new LexToken(make_token(RightBrace, "}", s), next) : ((ch == ",") ? new LexToken(make_token(Comma, ",", s), next) : ((ch == ".") ? new LexToken(make_token(Dot, ".", s), next) : ((ch == "^") ? new LexToken(make_token(Caret, "^", s), next) : ((ch == "&") ? new LexToken(make_token(Ampersand, "&", s), next) : scan_multi_char_operator(s)))))))))))))(advance_char(s))))(peek_char(s));
    }

    public static LexResult scan_multi_char_operator(LexState s)
    {
        return ((Func<LexResult, LexResult>)((ch) => ((Func<LexResult, LexResult>)((next) => ((Func<LexResult, LexResult>)((next_ch) => ((ch == "+") ? ((next_ch == "+") ? new LexToken(make_token(PlusPlus, "++", s), advance_char(next)) : new LexToken(make_token(Plus, "+", s), next)) : ((ch == "-") ? ((next_ch == ">") ? new LexToken(make_token(Arrow, "->", s), advance_char(next)) : new LexToken(make_token(Minus, "-", s), next)) : ((ch == "*") ? new LexToken(make_token(Star, "*", s), next) : ((ch == "/") ? ((next_ch == "=") ? new LexToken(make_token(NotEquals, "/=", s), advance_char(next)) : new LexToken(make_token(Slash, "/", s), next)) : ((ch == "=") ? ((next_ch == "=") ? ((Func<LexResult, LexResult>)((next2) => ((Func<LexResult, LexResult>)((next2_ch) => ((next2_ch == "=") ? new LexToken(make_token(TripleEquals, "===", s), advance_char(next2)) : new LexToken(make_token(DoubleEquals, "==", s), next2))))((is_at_end(next2) ? "" : peek_char(next2)))))(advance_char(next)) : new LexToken(make_token(Equals, "=", s), next)) : ((ch == ":") ? ((next_ch == ":") ? new LexToken(make_token(ColonColon, "::", s), advance_char(next)) : new LexToken(make_token(Colon, ":", s), next)) : ((ch == "|") ? ((next_ch == "-") ? new LexToken(make_token(Turnstile, "|-", s), advance_char(next)) : new LexToken(make_token(Pipe, "|", s), next)) : ((ch == "<") ? ((next_ch == "=") ? new LexToken(make_token(LessOrEqual, "<=", s), advance_char(next)) : ((next_ch == "-") ? new LexToken(make_token(LeftArrow, "<-", s), advance_char(next)) : new LexToken(make_token(LessThan, "<", s), next))) : ((ch == ">") ? ((next_ch == "=") ? new LexToken(make_token(GreaterOrEqual, ">=", s), advance_char(next)) : new LexToken(make_token(GreaterThan, ">", s), next)) : new LexToken(make_token(ErrorToken, char_at(s.source(s.offset)), s), next))))))))))))((is_at_end(next) ? "" : peek_char(next)))))(advance_char(s))))(peek_char(s));
    }

    public static List<Token> tokenize_loop(LexState st, List<Token> acc)
    {
        return ((Func<List<Token>, List<Token>>)((_scrutinee0_) => (_scrutinee0_ is LexToken _mLexToken0_ ? ((Func<object, List<Token>>)((next) => ((Func<object, List<Token>>)((tok) => ((tok.kind == new EndOfFile()) ? Enumerable.Concat(acc(), new List<Token>() { tok }).ToList() : (_p0_) => tokenize_loop(next(Enumerable.Concat(acc(), new List<Token>() { tok }).ToList()), _p0_))))(_mLexToken0_.Field0)))(_mLexToken0_.Field1) : (_scrutinee0_ is LexEnd _mLexEnd0_ ? Enumerable.Concat(acc(), new List<Token>() { make_token(EndOfFile, "", st) }).ToList() : throw new InvalidOperationException("Non-exhaustive match")))))(scan_token(st))(tokenize);
    }

    public static T241 Text<T241>()
    {
        return new List(new Token());
    }

    public static List<Token> tokenize(object src)
    {
        return tokenize_loop(make_lex_state(src), new List<object>());
    }

    public static ParseState make_parse_state(List<Token> toks)
    {
        return new ParseState(toks, 0L);
    }

    public static Token current(ParseState st)
    {
        return list_at()(st().tokens(st().pos));
    }

    public static TokenKind current_kind(ParseState st)
    {
        return current(st).kind;
    }

    public static ParseState advance(ParseState st)
    {
        return new ParseState(st().tokens, (st().pos + 1L));
    }

    public static bool is_done(ParseState st)
    {
        return (current_kind(st) is EndOfFile _mEndOfFile0_ ? true : false)(peek_kind);
    }

    public static T316 ParseState<T316>()
    {
        return new Integer();
    }

    public static T53 TokenKind<T53>()
    {
        return peek_kind(st)(offset);
    }

    public static T2230 list_at<T2230>()
    {
        return throw new InvalidOperationException(".")(tokens((st().pos + offset)));
    }

    public static T204 kind<T204>()
    {
        return new ParseExprResult();
    }

    public static ParseState ExprOk()
    {
        return new Expr();
    }

    public static T316 ParseState<T316>()
    {
        return new ParsePatResult();
    }

    public static T6 PatOk<T6>()
    {
        return new Pat();
    }

    public static T316 ParseState<T316>()
    {
        return new ParseTypeResult();
    }

    public static T8 TypeOk<T8>()
    {
        return new TypeExpr();
    }

    public static T316 ParseState<T316>()
    {
        return new ParseDefResult();
    }

    public static T10 DefOk<T10>()
    {
        return new Def();
    }

    public static T316 ParseState<T316>()
    {
        return throw new InvalidOperationException("|")(DefNone)(ParseState);
    }

    public static bool is_ident(TokenKind k)
    {
        return (k is Identifier _mIdentifier0_ ? true : false)(is_type_ident);
    }

    public static T53 TokenKind<T53>()
    {
        return new Boolean();
    }

    public static T2248 is_type_ident<T2248>(object k)
    {
        return (k is TypeIdentifier _mTypeIdentifier0_ ? true : false)(is_arrow);
    }

    public static T53 TokenKind<T53>()
    {
        return new Boolean();
    }

    public static T2253 is_arrow<T2253>(object k)
    {
        return (k is Arrow _mArrow0_ ? true : false)(is_equals);
    }

    public static T53 TokenKind<T53>()
    {
        return new Boolean();
    }

    public static T2258 is_equals<T2258>(object k)
    {
        return (k is Equals _mEquals0_ ? true : false)(is_colon);
    }

    public static T53 TokenKind<T53>()
    {
        return new Boolean();
    }

    public static T2263 is_colon<T2263>(object k)
    {
        return (k is Colon _mColon0_ ? true : false)(is_comma);
    }

    public static T53 TokenKind<T53>()
    {
        return new Boolean();
    }

    public static T2268 is_comma<T2268>(object k)
    {
        return (k is Comma _mComma0_ ? true : false)(is_pipe);
    }

    public static T53 TokenKind<T53>()
    {
        return new Boolean();
    }

    public static T2273 is_pipe<T2273>(object k)
    {
        return (k is Pipe _mPipe0_ ? true : false)(is_dot);
    }

    public static T53 TokenKind<T53>()
    {
        return new Boolean();
    }

    public static T2278 is_dot<T2278>(object k)
    {
        return (k is Dot _mDot0_ ? true : false)(is_left_paren);
    }

    public static T53 TokenKind<T53>()
    {
        return new Boolean();
    }

    public static T2283 is_left_paren<T2283>(object k)
    {
        return (k is LeftParen _mLeftParen0_ ? true : false)(is_left_brace);
    }

    public static T53 TokenKind<T53>()
    {
        return new Boolean();
    }

    public static T2288 is_left_brace<T2288>(object k)
    {
        return (k is LeftBrace _mLeftBrace0_ ? true : false)(is_left_bracket);
    }

    public static T53 TokenKind<T53>()
    {
        return new Boolean();
    }

    public static T2293 is_left_bracket<T2293>(object k)
    {
        return (k is LeftBracket _mLeftBracket0_ ? true : false)(is_right_brace);
    }

    public static T53 TokenKind<T53>()
    {
        return new Boolean();
    }

    public static T2298 is_right_brace<T2298>(object k)
    {
        return (k is RightBrace _mRightBrace0_ ? true : false)(is_right_bracket);
    }

    public static T53 TokenKind<T53>()
    {
        return new Boolean();
    }

    public static T2303 is_right_bracket<T2303>(object k)
    {
        return (k is RightBracket _mRightBracket0_ ? true : false)(is_if_keyword);
    }

    public static T53 TokenKind<T53>()
    {
        return new Boolean();
    }

    public static T2308 is_if_keyword<T2308>(object k)
    {
        return (k is IfKeyword _mIfKeyword0_ ? true : false)(is_let_keyword);
    }

    public static T53 TokenKind<T53>()
    {
        return new Boolean();
    }

    public static T2313 is_let_keyword<T2313>(object k)
    {
        return (k is LetKeyword _mLetKeyword0_ ? true : false)(is_when_keyword);
    }

    public static T53 TokenKind<T53>()
    {
        return new Boolean();
    }

    public static T2318 is_when_keyword<T2318>(object k)
    {
        return (k is WhenKeyword _mWhenKeyword0_ ? true : false)(is_do_keyword);
    }

    public static T53 TokenKind<T53>()
    {
        return new Boolean();
    }

    public static T2323 is_do_keyword<T2323>(object k)
    {
        return (k is DoKeyword _mDoKeyword0_ ? true : false)(is_in_keyword);
    }

    public static T53 TokenKind<T53>()
    {
        return new Boolean();
    }

    public static T2328 is_in_keyword<T2328>(object k)
    {
        return (k is InKeyword _mInKeyword0_ ? true : false)(is_minus);
    }

    public static T53 TokenKind<T53>()
    {
        return new Boolean();
    }

    public static T2333 is_minus<T2333>(object k)
    {
        return (k is Minus _mMinus0_ ? true : false)(is_dedent);
    }

    public static T53 TokenKind<T53>()
    {
        return new Boolean();
    }

    public static T2338 is_dedent<T2338>(object k)
    {
        return (k is Dedent _mDedent0_ ? true : false)(is_left_arrow);
    }

    public static T53 TokenKind<T53>()
    {
        return new Boolean();
    }

    public static T2343 is_left_arrow<T2343>(object k)
    {
        return (k is LeftArrow _mLeftArrow0_ ? true : false)(is_record_keyword);
    }

    public static T53 TokenKind<T53>()
    {
        return new Boolean();
    }

    public static T2348 is_record_keyword<T2348>(object k)
    {
        return (k is RecordKeyword _mRecordKeyword0_ ? true : false)(is_underscore);
    }

    public static T53 TokenKind<T53>()
    {
        return new Boolean();
    }

    public static T2353 is_underscore<T2353>(object k)
    {
        return (k is Underscore _mUnderscore0_ ? true : false)(is_literal);
    }

    public static T53 TokenKind<T53>()
    {
        return new Boolean();
    }

    public static T2358 is_literal<T2358>(object k)
    {
        return ((Func<object, object>)((_scrutinee0_) => (_scrutinee0_ is IntegerLiteral _mIntegerLiteral0_ ? true : (_scrutinee0_ is NumberLiteral _mNumberLiteral0_ ? true : (_scrutinee0_ is TextLiteral _mTextLiteral0_ ? true : (_scrutinee0_ is TrueKeyword _mTrueKeyword0_ ? true : (_scrutinee0_ is FalseKeyword _mFalseKeyword0_ ? true : false)))))))(k)(is_app_start);
    }

    public static T53 TokenKind<T53>()
    {
        return new Boolean();
    }

    public static T2363 is_app_start<T2363>(object k)
    {
        return ((Func<object, object>)((_scrutinee0_) => (_scrutinee0_ is Identifier _mIdentifier0_ ? true : (_scrutinee0_ is TypeIdentifier _mTypeIdentifier0_ ? true : (_scrutinee0_ is IntegerLiteral _mIntegerLiteral0_ ? true : (_scrutinee0_ is NumberLiteral _mNumberLiteral0_ ? true : (_scrutinee0_ is TextLiteral _mTextLiteral0_ ? true : (_scrutinee0_ is TrueKeyword _mTrueKeyword0_ ? true : (_scrutinee0_ is FalseKeyword _mFalseKeyword0_ ? true : (_scrutinee0_ is LeftParen _mLeftParen0_ ? true : (_scrutinee0_ is LeftBracket _mLeftBracket0_ ? true : false)))))))))))(k)(is_compound);
    }

    public static object Expr()
    {
        return new Boolean();
    }

    public static T2376 is_compound<T2376>(object e)
    {
        return ((Func<object, object>)((_scrutinee0_) => (_scrutinee0_ is MatchExpr _mMatchExpr0_ ? ((Func<object, bool>)((arms) => ((Func<object, bool>)((s) => true))(_mMatchExpr0_.Field0)))(_mMatchExpr0_.Field1) : (_scrutinee0_ is IfExpr _mIfExpr0_ ? ((Func<object, bool>)((el) => ((Func<object, bool>)((t) => ((Func<object, bool>)((c) => true))(_mIfExpr0_.Field0)))(_mIfExpr0_.Field1)))(_mIfExpr0_.Field2) : (_scrutinee0_ is LetExpr _mLetExpr0_ ? ((Func<object, bool>)((body) => ((Func<object, bool>)((binds) => true))(_mLetExpr0_.Field0)))(_mLetExpr0_.Field1) : (_scrutinee0_ is DoExpr _mDoExpr0_ ? ((Func<object, bool>)((stmts) => true))(_mDoExpr0_.Field0) : false))))))(e())(is_type_arg_start);
    }

    public static T53 TokenKind<T53>()
    {
        return new Boolean();
    }

    public static T2381 is_type_arg_start<T2381>(object k)
    {
        return ((Func<object, object>)((_scrutinee0_) => (_scrutinee0_ is TypeIdentifier _mTypeIdentifier0_ ? true : (_scrutinee0_ is Identifier _mIdentifier0_ ? true : (_scrutinee0_ is LeftParen _mLeftParen0_ ? true : false)))))(k)(operator_precedence);
    }

    public static T53 TokenKind<T53>()
    {
        return new Integer();
    }

    public static T2386 operator_precedence<T2386>(object k)
    {
        return ((Func<object, object>)((_scrutinee0_) => (_scrutinee0_ is PlusPlus _mPlusPlus0_ ? 5L : (_scrutinee0_ is ColonColon _mColonColon0_ ? 5L : (_scrutinee0_ is Plus _mPlus0_ ? 6L : (_scrutinee0_ is Minus _mMinus0_ ? 6L : (_scrutinee0_ is Star _mStar0_ ? 7L : (_scrutinee0_ is Slash _mSlash0_ ? 7L : (_scrutinee0_ is Caret _mCaret0_ ? 8L : (_scrutinee0_ is DoubleEquals _mDoubleEquals0_ ? 4L : (_scrutinee0_ is NotEquals _mNotEquals0_ ? 4L : (_scrutinee0_ is LessThan _mLessThan0_ ? 4L : (_scrutinee0_ is GreaterThan _mGreaterThan0_ ? 4L : (_scrutinee0_ is LessOrEqual _mLessOrEqual0_ ? 4L : (_scrutinee0_ is GreaterOrEqual _mGreaterOrEqual0_ ? 4L : (_scrutinee0_ is TripleEquals _mTripleEquals0_ ? 4L : (_scrutinee0_ is Ampersand _mAmpersand0_ ? 3L : (_scrutinee0_ is Pipe _mPipe0_ ? 2L : (0L - 1L)))))))))))))))))))(k)(is_right_assoc);
    }

    public static T53 TokenKind<T53>()
    {
        return new Boolean();
    }

    public static T2391 is_right_assoc<T2391>(object k)
    {
        return ((Func<object, object>)((_scrutinee0_) => (_scrutinee0_ is PlusPlus _mPlusPlus0_ ? true : (_scrutinee0_ is ColonColon _mColonColon0_ ? true : (_scrutinee0_ is Caret _mCaret0_ ? true : (_scrutinee0_ is Arrow _mArrow0_ ? true : false))))))(k)(expect);
    }

    public static T53 TokenKind<T53>()
    {
        return new ParseState();
    }

    public static T316 ParseState<T316>()
    {
        return expect(kind)(st);
    }

    public static Func<ParseState, bool> is_done()
    {
        return throw new InvalidOperationException("then")(new Func<ParseState, bool>(st));
    }

    public static Func<ParseState, ParseState> advance()
    {
        return skip_newlines;
    }

    public static T316 ParseState<T316>()
    {
        return new ParseState();
    }

    public static ParseState skip_newlines(object st)
    {
        return (is_done(st) ? st() : ((Func<object, object>)((_scrutinee0_) => (_scrutinee0_ is Newline _mNewline0_ ? skip_newlines(advance(st)) : (_scrutinee0_ is Indent _mIndent0_ ? skip_newlines(advance(st)) : (_scrutinee0_ is Dedent _mDedent0_ ? skip_newlines(advance(st)) : st())))))(current_kind(st))(parse_type));
    }

    public static T316 ParseState<T316>()
    {
        return new ParseTypeResult();
    }

    public static ParseTypeResult parse_type(Func<TypeExpr, Func<ParseState, ParseTypeResult>> st)
    {
        return ((Func<ParseTypeResult, ParseTypeResult>)((result) => (_p0_) => unwrap_type_ok(result(parse_type_continue), _p0_)))(parse_type_atom(st));
    }

    public static ParseTypeResult parse_type_continue(TypeExpr left, ParseState st)
    {
        return (is_arrow(current_kind(st)) ? ((Func<ParseTypeResult, ParseTypeResult>)((st2) => ((Func<ParseTypeResult, ParseTypeResult>)((right_result) => (_p0_) => unwrap_type_ok(right_result((_p0_) => (_p1_) => make_fun_type(left, _p0_, _p1_)), _p0_)))(parse_type(st2))))(advance(st)) : new TypeOk(left(st)));
    }

    public static ParseTypeResult make_fun_type(TypeExpr left, TypeExpr right, ParseState st)
    {
        return new TypeOk(new FunType(left(right)), st());
    }

    public static ParseTypeResult unwrap_type_ok(ParseTypeResult r, Func<TypeExpr, Func<ParseState, ParseTypeResult>> f)
    {
        return (r is TypeOk _mTypeOk0_ ? ((Func<object, ParseTypeResult>)((st) => ((Func<object, ParseTypeResult>)((t) => f(t(st))))(_mTypeOk0_.Field0)))(_mTypeOk0_.Field1) : throw new InvalidOperationException("Non-exhaustive match"))(parse_type_atom);
    }

    public static T316 ParseState<T316>()
    {
        return new ParseTypeResult();
    }

    public static ParseTypeResult parse_type_atom(object st)
    {
        return (is_ident(current_kind(st)) ? ((Func<object, object>)((tok) => parse_type_args(new NamedType(tok), advance(st))))(current(st)) : (is_type_ident(current_kind(st)) ? ((Func<object, object>)((tok) => parse_type_args(new NamedType(tok), advance(st))))(current(st)) : (is_left_paren(current_kind(st)) ? parse_paren_type(advance(st)) : ((Func<object, object>)((tok) => new TypeOk(new NamedType(tok), advance(st))))(current(st)))));
    }

    public static ParseTypeResult parse_paren_type(ParseState st)
    {
        return ((Func<ParseTypeResult, ParseTypeResult>)((inner) => (_p0_) => unwrap_type_ok(inner(finish_paren_type), _p0_)))(parse_type(st));
    }

    public static ParseTypeResult finish_paren_type(TypeExpr t, ParseState st)
    {
        return ((Func<ParseTypeResult, ParseTypeResult>)((st2) => new TypeOk(new ParenType(t), st2())))(expect(RightParen)(st));
    }

    public static ParseTypeResult parse_type_args(TypeExpr base_type, ParseState st)
    {
        return (is_done(st) ? new TypeOk(base_type(st)) : (is_type_arg_start(current_kind(st)) ? (_p0_) => parse_type_arg_next(base_type(st), _p0_) : new TypeOk(base_type(st))));
    }

    public static ParseTypeResult parse_type_arg_next(TypeExpr base_type, ParseState st)
    {
        return ((Func<ParseTypeResult, ParseTypeResult>)((arg_result) => (_p0_) => unwrap_type_ok(arg_result((_p0_) => (_p1_) => continue_type_args(base_type, _p0_, _p1_)), _p0_)))(parse_type_atom(st));
    }

    public static ParseTypeResult continue_type_args(TypeExpr base_type, TypeExpr arg, ParseState st)
    {
        return parse_type_args(new AppType(base_type(new List<object>() { arg })), st);
    }

    public static ParsePatResult parse_pattern(ParseState st)
    {
        return (is_underscore(current_kind(st)) ? new PatOk(new WildPat(current(st)), advance(st)) : (is_literal(current_kind(st)) ? new PatOk(new LitPat(current(st)), advance(st)) : (is_type_ident(current_kind(st)) ? ((Func<ParsePatResult, ParsePatResult>)((tok) => (_p0_) => (_p1_) => parse_ctor_pattern_fields(tok(new List<object>())(advance(st)), _p0_, _p1_)))(current(st)) : (is_ident(current_kind(st)) ? new PatOk(new VarPat(current(st)), advance(st)) : new PatOk(new WildPat(current(st)), advance(st))))));
    }

    public static ParsePatResult parse_ctor_pattern_fields(Token ctor, List<Pat> acc, ParseState st)
    {
        return (is_left_paren(current_kind(st)) ? ((Func<ParsePatResult, ParsePatResult>)((st2) => ((Func<ParsePatResult, ParsePatResult>)((sub) => (_p0_) => unwrap_pat_ok(sub()((_p0_) => (_p1_) => (_p2_) => continue_ctor_fields(ctor()(acc), _p0_, _p1_, _p2_)), _p0_)))(parse_pattern(st2))))(advance(st)) : new PatOk(new CtorPat(ctor()(acc)), st()));
    }

    public static ParsePatResult continue_ctor_fields(Token ctor, List<Pat> acc, Pat p, ParseState st)
    {
        return ((Func<ParsePatResult, ParsePatResult>)((st2) => (_p0_) => (_p1_) => parse_ctor_pattern_fields(ctor()(Enumerable.Concat(acc(), new List<object>() { p() }).ToList())(st2), _p0_, _p1_)))(expect(RightParen)(st));
    }

    public static ParsePatResult unwrap_pat_ok(ParsePatResult r, Func<Pat, Func<ParseState, ParsePatResult>> f)
    {
        return (r is PatOk _mPatOk0_ ? ((Func<object, ParsePatResult>)((st) => ((Func<object, ParsePatResult>)((p) => f(p()(st))))(_mPatOk0_.Field0)))(_mPatOk0_.Field1) : throw new InvalidOperationException("Non-exhaustive match"))(parse_expr);
    }

    public static T316 ParseState<T316>()
    {
        return new ParseExprResult();
    }

    public static T2549 parse_expr<T2549>(object st)
    {
        return parse_binary(st()(0L));
    }

    public static ParseExprResult unwrap_expr_ok(ParseExprResult r, Func<Expr, Func<ParseState, ParseExprResult>> f)
    {
        return (r is ExprOk _mExprOk0_ ? ((Func<object, ParseExprResult>)((st) => ((Func<object, ParseExprResult>)((e) => f(e()(st))))(_mExprOk0_.Field0)))(_mExprOk0_.Field1) : throw new InvalidOperationException("Non-exhaustive match"))(parse_binary);
    }

    public static T316 ParseState<T316>()
    {
        return new Integer();
    }

    public static T2559 ParseExprResult<T2559>()
    {
        return parse_binary(st)(min_prec);
    }

    public static ParseExprResult left_result()
    {
        return parse_unary(st);
    }

    public static Func<ParseExprResult, Func<Func<Expr, Func<ParseState, ParseExprResult>>, ParseExprResult>> unwrap_expr_ok()
    {
        return (_p0_) => (_p1_) => start_binary_loop(new Func<ParseExprResult, Func<Func<Expr, Func<ParseState, ParseExprResult>>, ParseExprResult>>(min_prec), _p0_, _p1_);
    }

    public static ParseExprResult start_binary_loop(long min_prec, Expr left, ParseState st)
    {
        return (_p0_) => (_p1_) => parse_binary_loop(left(st()(min_prec)), _p0_, _p1_);
    }

    public static ParseExprResult parse_binary_loop(Expr left, ParseState st, long min_prec)
    {
        return (is_done(st) ? new ExprOk(left(st)) : ((Func<ParseExprResult, ParseExprResult>)((prec) => ((prec < min_prec) ? new ExprOk(left(st)) : ((Func<ParseExprResult, ParseExprResult>)((op) => ((Func<ParseExprResult, ParseExprResult>)((st2) => ((Func<ParseExprResult, ParseExprResult>)((next_min) => ((Func<ParseExprResult, ParseExprResult>)((right_result) => (_p0_) => unwrap_expr_ok(right_result((_p0_) => (_p1_) => (_p2_) => (_p3_) => continue_binary(left(op()(min_prec)), _p0_, _p1_, _p2_, _p3_)), _p0_)))(parse_binary(st2()(next_min)))))((is_right_assoc(op().kind) ? prec : (prec + 1L)))))(skip_newlines(advance(st)))))(current(st)))))(operator_precedence(current_kind(st))));
    }

    public static ParseExprResult continue_binary(Expr left, Token op, long min_prec, Expr right, ParseState st)
    {
        return (_p0_) => parse_binary_loop(new BinExpr(left(op()(right))), st()(min_prec), _p0_);
    }

    public static ParseExprResult parse_unary(ParseState st)
    {
        return (is_minus(current_kind(st)) ? ((Func<ParseExprResult, ParseExprResult>)((op) => ((Func<ParseExprResult, ParseExprResult>)((result) => (_p0_) => unwrap_expr_ok(result((_p0_) => (_p1_) => finish_unary(op, _p0_, _p1_)), _p0_)))(parse_unary(advance(st)))))(current(st)) : parse_application(st));
    }

    public static ParseExprResult finish_unary(Token op, Expr operand, ParseState st)
    {
        return new ExprOk(new UnaryExpr(op()(operand)), st());
    }

    public static ParseExprResult parse_application(ParseState st)
    {
        return ((Func<ParseExprResult, ParseExprResult>)((func_result) => (_p0_) => unwrap_expr_ok(func_result(parse_app_loop), _p0_)))(parse_atom(st));
    }

    public static ParseExprResult parse_app_loop(Expr func, ParseState st)
    {
        return (is_compound(func) ? (_p0_) => parse_field_access(func(st), _p0_) : (is_done(st) ? new ExprOk(func(st)) : (is_app_start(current_kind(st)) ? ((Func<ParseExprResult, ParseExprResult>)((arg_result) => (_p0_) => unwrap_expr_ok(arg_result((_p0_) => (_p1_) => continue_app(func, _p0_, _p1_)), _p0_)))(parse_atom(st)) : (_p0_) => parse_field_access(func(st), _p0_))));
    }

    public static ParseExprResult continue_app(Expr func, Expr arg, ParseState st)
    {
        return parse_app_loop(new AppExpr(func(arg)), st);
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
                var _tco_0 = new FieldExpr(node(field));
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
                    return (_p0_) => unwrap_expr_ok(arg_result((_p0_) => (_p1_) => continue_app(node, _p0_, _p1_)), _p0_);
                }
                else
                {
                    return new ExprOk(node(st));
                }
            }
        }
    }

    public static ParseExprResult parse_atom_type_ident(ParseState st)
    {
        return ((Func<ParseExprResult, ParseExprResult>)((tok) => ((Func<ParseExprResult, ParseExprResult>)((st2) => (is_left_brace(current_kind(st2)) ? (_p0_) => parse_record_expr(tok(st2), _p0_) : new ExprOk(new NameExpr(tok), st2()))))(advance(st))))(current(st));
    }

    public static ParseExprResult parse_paren_expr(ParseState st)
    {
        return ((Func<ParseExprResult, ParseExprResult>)((st2) => ((Func<ParseExprResult, ParseExprResult>)((inner) => (_p0_) => unwrap_expr_ok(inner(finish_paren_expr), _p0_)))(parse_expr(st2))))(skip_newlines(st));
    }

    public static ParseExprResult finish_paren_expr(Expr e, ParseState st)
    {
        return ((Func<ParseExprResult, ParseExprResult>)((st2) => ((Func<ParseExprResult, ParseExprResult>)((st3) => new ExprOk(new ParenExpr(e()), st3)))(expect(RightParen)(st2))))(skip_newlines(st));
    }

    public static ParseExprResult parse_record_expr(Token type_name, ParseState st)
    {
        return ((Func<ParseExprResult, ParseExprResult>)((st2) => ((Func<ParseExprResult, ParseExprResult>)((st3) => (_p0_) => (_p1_) => parse_record_expr_fields(type_name(new List<object>())(st3), _p0_, _p1_)))(skip_newlines(st2))))(advance(st));
    }

    public static ParseExprResult parse_record_expr_fields(Token type_name, List<RecordFieldExpr> acc, ParseState st)
    {
        return (is_right_brace(current_kind(st)) ? new ExprOk(new RecordExpr(type_name(acc)), advance(st)) : (is_ident(current_kind(st)) ? (_p0_) => (_p1_) => parse_record_field(type_name(acc()(st)), _p0_, _p1_) : new ExprOk(new RecordExpr(type_name(acc)), st())));
    }

    public static ParseExprResult parse_record_field(Token type_name, List<RecordFieldExpr> acc, ParseState st)
    {
        return ((Func<ParseExprResult, ParseExprResult>)((field_name) => ((Func<ParseExprResult, ParseExprResult>)((st2) => ((Func<ParseExprResult, ParseExprResult>)((st3) => ((Func<ParseExprResult, ParseExprResult>)((val_result) => (_p0_) => unwrap_expr_ok(val_result((_p0_) => (_p1_) => (_p2_) => (_p3_) => finish_record_field(type_name(acc()(field_name)), _p0_, _p1_, _p2_, _p3_)), _p0_)))(parse_expr(st3))))(expect(Equals)(st2))))(advance(st))))(current(st));
    }

    public static ParseExprResult finish_record_field(Token type_name, List<RecordFieldExpr> acc, Token field_name, Expr v, ParseState st)
    {
        return ((Func<ParseExprResult, ParseExprResult>)((field) => ((Func<ParseExprResult, ParseExprResult>)((st2) => (is_comma(current_kind(st2)) ? (_p0_) => (_p1_) => parse_record_expr_fields(type_name(Enumerable.Concat(acc(), new List<object>() { field }).ToList())(skip_newlines(advance(st2))), _p0_, _p1_) : (_p0_) => (_p1_) => parse_record_expr_fields(type_name(Enumerable.Concat(acc(), new List<object>() { field }).ToList())(st2), _p0_, _p1_))))(skip_newlines(st))))(new RecordFieldExpr(field_name, v));
    }

    public static ParseExprResult parse_list_expr(ParseState st)
    {
        return ((Func<ParseExprResult, ParseExprResult>)((st2) => ((Func<ParseExprResult, ParseExprResult>)((st3) => parse_list_elements(new List<object>(), st3)))(skip_newlines(st2))))(advance(st));
    }

    public static ParseExprResult parse_list_elements(List<Expr> acc, ParseState st)
    {
        return (is_right_bracket(current_kind(st)) ? new ExprOk(new ListExpr(acc()), advance(st)) : ((Func<ParseExprResult, ParseExprResult>)((elem) => (_p0_) => unwrap_expr_ok(elem((_p0_) => (_p1_) => finish_list_element(acc, _p0_, _p1_)), _p0_)))(parse_expr(st)));
    }

    public static ParseExprResult finish_list_element(List<Expr> acc, Expr e, ParseState st)
    {
        return ((Func<ParseExprResult, ParseExprResult>)((st2) => (is_comma(current_kind(st2)) ? parse_list_elements(Enumerable.Concat(acc(), new List<object>() { e() }).ToList(), skip_newlines(advance(st2))) : parse_list_elements(Enumerable.Concat(acc(), new List<object>() { e() }).ToList(), st2))))(skip_newlines(st));
    }

    public static ParseExprResult parse_if_expr(ParseState st)
    {
        return ((Func<ParseExprResult, ParseExprResult>)((st2) => ((Func<ParseExprResult, ParseExprResult>)((cond) => (_p0_) => unwrap_expr_ok(cond(parse_if_then), _p0_)))(parse_expr(st2))))(skip_newlines(advance(st)));
    }

    public static ParseExprResult parse_if_then(Expr c, ParseState st)
    {
        return ((Func<ParseExprResult, ParseExprResult>)((st2) => ((Func<ParseExprResult, ParseExprResult>)((st3) => ((Func<ParseExprResult, ParseExprResult>)((st4) => ((Func<ParseExprResult, ParseExprResult>)((then_result) => (_p0_) => unwrap_expr_ok(then_result((_p0_) => (_p1_) => parse_if_else(c, _p0_, _p1_)), _p0_)))(parse_expr(st4))))(skip_newlines(st3))))(expect(ThenKeyword)(st2))))(skip_newlines(st));
    }

    public static ParseExprResult parse_if_else(Expr c, Expr t, ParseState st)
    {
        return ((Func<ParseExprResult, ParseExprResult>)((st2) => ((Func<ParseExprResult, ParseExprResult>)((st3) => ((Func<ParseExprResult, ParseExprResult>)((st4) => ((Func<ParseExprResult, ParseExprResult>)((else_result) => (_p0_) => unwrap_expr_ok(else_result((_p0_) => (_p1_) => (_p2_) => finish_if(c(t), _p0_, _p1_, _p2_)), _p0_)))(parse_expr(st4))))(skip_newlines(st3))))(expect(ElseKeyword)(st2))))(skip_newlines(st));
    }

    public static ParseExprResult finish_if(Expr c, Expr t, Expr e, ParseState st)
    {
        return new ExprOk(new IfExpr(c(t(e))), st());
    }

    public static ParseExprResult parse_let_expr(ParseState st)
    {
        return ((Func<ParseExprResult, ParseExprResult>)((st2) => parse_let_bindings(new List<object>(), st2)))(skip_newlines(advance(st)));
    }

    public static ParseExprResult parse_let_bindings(List<LetBind> acc, ParseState st)
    {
        return (is_ident(current_kind(st)) ? (_p0_) => parse_let_binding(acc()(st), _p0_) : (is_in_keyword(current_kind(st)) ? ((Func<ParseExprResult, ParseExprResult>)((st2) => ((Func<ParseExprResult, ParseExprResult>)((body) => (_p0_) => unwrap_expr_ok(body((_p0_) => (_p1_) => finish_let(acc, _p0_, _p1_)), _p0_)))(parse_expr(st2))))(skip_newlines(advance(st))) : ((Func<ParseExprResult, ParseExprResult>)((body) => (_p0_) => unwrap_expr_ok(body((_p0_) => (_p1_) => finish_let(acc, _p0_, _p1_)), _p0_)))(parse_expr(st))));
    }

    public static ParseExprResult finish_let(List<LetBind> acc, Expr b, ParseState st)
    {
        return new ExprOk(new LetExpr(acc()(b)), st());
    }

    public static ParseExprResult parse_let_binding(List<LetBind> acc, ParseState st)
    {
        return ((Func<ParseExprResult, ParseExprResult>)((name_tok) => ((Func<ParseExprResult, ParseExprResult>)((st2) => ((Func<ParseExprResult, ParseExprResult>)((st3) => ((Func<ParseExprResult, ParseExprResult>)((val_result) => (_p0_) => unwrap_expr_ok(val_result((_p0_) => (_p1_) => (_p2_) => finish_let_binding(acc()(name_tok), _p0_, _p1_, _p2_)), _p0_)))(parse_expr(st3))))(expect(Equals)(st2))))(advance(st))))(current(st));
    }

    public static ParseExprResult finish_let_binding(List<LetBind> acc, Token name_tok, Expr v, ParseState st)
    {
        return ((Func<ParseExprResult, ParseExprResult>)((binding) => ((Func<ParseExprResult, ParseExprResult>)((st2) => (is_comma(current_kind(st2)) ? parse_let_bindings(Enumerable.Concat(acc(), new List<object>() { binding }).ToList(), skip_newlines(advance(st2))) : parse_let_bindings(Enumerable.Concat(acc(), new List<object>() { binding }).ToList(), st2))))(skip_newlines(st))))(new LetBind(name_tok(), v));
    }

    public static ParseExprResult parse_match_expr(ParseState st)
    {
        return ((Func<ParseExprResult, ParseExprResult>)((st2) => ((Func<ParseExprResult, ParseExprResult>)((scrut) => (_p0_) => unwrap_expr_ok(scrut(start_match_branches), _p0_)))(parse_expr(st2))))(advance(st));
    }

    public static ParseExprResult start_match_branches(Expr s, ParseState st)
    {
        return ((Func<ParseExprResult, ParseExprResult>)((st2) => (_p0_) => (_p1_) => parse_match_branches(s(new List<object>())(st2), _p0_, _p1_)))(skip_newlines(st));
    }

    public static ParseExprResult parse_match_branches(Expr scrut, List<MatchArm> acc, ParseState st)
    {
        return (is_if_keyword(current_kind(st)) ? (_p0_) => (_p1_) => parse_one_match_branch(scrut(acc()(st)), _p0_, _p1_) : new ExprOk(new MatchExpr(scrut(acc)), st()));
    }

    public static ParseExprResult unwrap_pat_for_expr(ParsePatResult r, Func<Pat, Func<ParseState, ParseExprResult>> f)
    {
        return (r is PatOk _mPatOk0_ ? ((Func<object, ParseExprResult>)((st) => ((Func<object, ParseExprResult>)((p) => f(p()(st))))(_mPatOk0_.Field0)))(_mPatOk0_.Field1) : throw new InvalidOperationException("Non-exhaustive match"))(parse_one_match_branch);
    }

    public static object Expr()
    {
        return new List(new MatchArm());
    }

    public static T316 ParseState<T316>()
    {
        return new ParseExprResult();
    }

    public static ParseExprResult parse_one_match_branch(Func<Pat, Func<ParseState, ParseExprResult>> scrut, object acc, object st)
    {
        return ((Func<object, object>)((st2) => ((Func<object, object>)((pat) => (_p0_) => unwrap_pat_for_expr(pat((_p0_) => (_p1_) => (_p2_) => parse_match_branch_body(scrut(acc), _p0_, _p1_, _p2_)), _p0_)))(parse_pattern(st2))))(advance(st));
    }

    public static ParseExprResult parse_match_branch_body(Expr scrut, List<MatchArm> acc, Pat p, ParseState st)
    {
        return ((Func<ParseExprResult, ParseExprResult>)((st2) => ((Func<ParseExprResult, ParseExprResult>)((st3) => ((Func<ParseExprResult, ParseExprResult>)((body) => (_p0_) => unwrap_expr_ok(body((_p0_) => (_p1_) => (_p2_) => (_p3_) => finish_match_branch(scrut(acc()(p)), _p0_, _p1_, _p2_, _p3_)), _p0_)))(parse_expr(st3))))(skip_newlines(st2))))(expect(Arrow)(st));
    }

    public static ParseExprResult finish_match_branch(Expr scrut, List<MatchArm> acc, Pat p, Expr b, ParseState st)
    {
        return ((Func<ParseExprResult, ParseExprResult>)((arm) => ((Func<ParseExprResult, ParseExprResult>)((st2) => (_p0_) => (_p1_) => parse_match_branches(scrut(Enumerable.Concat(acc(), new List<object>() { arm }).ToList())(st2), _p0_, _p1_)))(skip_newlines(st))))(new MatchArm(p(), b()));
    }

    public static ParseExprResult parse_do_expr(ParseState st)
    {
        return ((Func<ParseExprResult, ParseExprResult>)((st2) => parse_do_stmts(new List<object>(), st2)))(skip_newlines(advance(st)));
    }

    public static ParseExprResult parse_do_stmts(List<DoStmt> acc, ParseState st)
    {
        return (is_done(st) ? new ExprOk(new DoExpr(acc()), st()) : (is_dedent(current_kind(st)) ? new ExprOk(new DoExpr(acc()), st()) : (is_do_bind(st) ? (_p0_) => parse_do_bind_stmt(acc()(st), _p0_) : (_p0_) => parse_do_expr_stmt(acc()(st), _p0_))));
    }

    public static bool is_do_bind(ParseState st)
    {
        return (is_ident(current_kind(st)) ? is_left_arrow(peek_kind(st()(1L))) : false);
    }

    public static ParseExprResult parse_do_bind_stmt(List<DoStmt> acc, ParseState st)
    {
        return ((Func<ParseExprResult, ParseExprResult>)((name_tok) => ((Func<ParseExprResult, ParseExprResult>)((st2) => ((Func<ParseExprResult, ParseExprResult>)((val_result) => (_p0_) => unwrap_expr_ok(val_result((_p0_) => (_p1_) => (_p2_) => finish_do_bind(acc()(name_tok), _p0_, _p1_, _p2_)), _p0_)))(parse_expr(st2))))(advance(advance(st)))))(current(st));
    }

    public static ParseExprResult finish_do_bind(List<DoStmt> acc, Token name_tok, Expr v, ParseState st)
    {
        return ((Func<ParseExprResult, ParseExprResult>)((st2) => parse_do_stmts(Enumerable.Concat(acc(), new List<object>() { new DoBindStmt(name_tok()(v)) }).ToList(), st2)))(skip_newlines(st));
    }

    public static ParseExprResult parse_do_expr_stmt(List<DoStmt> acc, ParseState st)
    {
        return ((Func<ParseExprResult, ParseExprResult>)((expr_result) => (_p0_) => unwrap_expr_ok(expr_result((_p0_) => (_p1_) => finish_do_expr(acc, _p0_, _p1_)), _p0_)))(parse_expr(st));
    }

    public static ParseExprResult finish_do_expr(List<DoStmt> acc, Expr e, ParseState st)
    {
        return ((Func<ParseExprResult, ParseExprResult>)((st2) => parse_do_stmts(Enumerable.Concat(acc(), new List<object>() { new DoExprStmt(e()) }).ToList(), st2)))(skip_newlines(st));
    }

    public static ParseTypeResult parse_type_annotation(ParseState st)
    {
        return ((Func<ParseTypeResult, ParseTypeResult>)((st2) => ((Func<ParseTypeResult, ParseTypeResult>)((st3) => parse_type(st3)))(expect(Colon)(st2))))(advance(st));
    }

    public static ParseDefResult parse_definition(ParseState st)
    {
        return (is_done(st) ? new DefNone(st()) : (is_ident(current_kind(st)) ? try_parse_def(st) : (is_type_ident(current_kind(st)) ? try_parse_def(st) : new DefNone(st()))));
    }

    public static ParseDefResult try_parse_def(ParseState st)
    {
        return (is_colon(peek_kind(st()(1L))) ? ((Func<ParseDefResult, ParseDefResult>)((ann_result) => unwrap_type_for_def(ann_result)))(parse_type_annotation(st)) : parse_def_body_with_ann(new List<object>())(st));
    }

    public static ParseDefResult unwrap_type_for_def(ParseTypeResult r)
    {
        return (r is TypeOk _mTypeOk0_ ? ((Func<object, ParseDefResult>)((st) => ((Func<object, ParseDefResult>)((ann_type) => ((Func<ParseDefResult, ParseDefResult>)((name_tok) => ((Func<ParseDefResult, ParseDefResult>)((ann) => ((Func<ParseDefResult, ParseDefResult>)((st2) => parse_def_body_with_ann(ann(st2))))(skip_newlines(st))))(new List<object>() { new TypeAnn(name_tok(), ann_type) })))(new Token(new Identifier(), "", 0L, 0L, 0L))))(_mTypeOk0_.Field0)))(_mTypeOk0_.Field1) : throw new InvalidOperationException("Non-exhaustive match"))(parse_def_body_with_ann);
    }

    public static T736 List<T736>()
    {
        return throw new InvalidOperationException("->")(ParseState);
    }

    public static T2967 ParseDefResult<T2967>()
    {
        return parse_def_body_with_ann(ann)(st);
    }

    public static Token name_tok()
    {
        return current(st);
    }

    public static ParseState st2()
    {
        return advance(st);
    }

    public static T2975 parse_def_params_then<T2975>()
    {
        return name_tok()(new List<object>())(st2);
    }

    public static T2975 parse_def_params_then<T2975>(object ann, object name_tok, object acc, object st)
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
                    var st4 = expect(RightParen)(st3);
                    var _tco_0 = ann(name_tok()(Enumerable.Concat(acc(), new List<object>() { param }).ToList())(st4));
                    ann = _tco_0;
                    continue;
                }
                else
                {
                    return (_p0_) => (_p1_) => (_p2_) => finish_def(ann(name_tok()(acc()(st))), _p0_, _p1_, _p2_);
                }
            }
            else
            {
                return (_p0_) => (_p1_) => (_p2_) => finish_def(ann(name_tok()(acc()(st))), _p0_, _p1_, _p2_);
            }
        }
    }

    public static ParseDefResult finish_def(List<TypeAnn> ann, Token name_tok, List<Token> @params, ParseState st)
    {
        return ((Func<ParseDefResult, ParseDefResult>)((st2) => ((Func<ParseDefResult, ParseDefResult>)((st3) => ((Func<ParseDefResult, ParseDefResult>)((body_result) => (_p0_) => (_p1_) => (_p2_) => unwrap_def_body(body_result(ann(name_tok()(@params))), _p0_, _p1_, _p2_)))(parse_expr(st3))))(skip_newlines(st2))))(expect(Equals)(st));
    }

    public static ParseDefResult unwrap_def_body(ParseExprResult r, List<TypeAnn> ann, Token name_tok, List<Token> @params)
    {
        return (r is ExprOk _mExprOk0_ ? ((Func<object, ParseDefResult>)((st) => ((Func<object, ParseDefResult>)((b) => new DefOk(new Def(name_tok(), @params, ann, b()), st())))(_mExprOk0_.Field0)))(_mExprOk0_.Field1) : throw new InvalidOperationException("Non-exhaustive match"))(parse_type_def);
    }

    public static T316 ParseState<T316>()
    {
        return new ParseTypeDefResult();
    }

    public static ParseTypeDefResult parse_type_def(ParseState st)
    {
        return (is_type_ident(current_kind(st)) ? ((Func<ParseTypeDefResult, ParseTypeDefResult>)((name_tok) => ((Func<ParseTypeDefResult, ParseTypeDefResult>)((st2) => (is_equals(current_kind(st2)) ? ((Func<ParseTypeDefResult, ParseTypeDefResult>)((st3) => (is_record_keyword(current_kind(st3)) ? (_p0_) => parse_record_type(name_tok()(st3), _p0_) : (is_pipe(current_kind(st3)) ? parse_variant_type(name_tok()(st3)) : new TypeDefNone(st())))))(skip_newlines(advance(st2))) : new TypeDefNone(st()))))(advance(st))))(current(st)) : new TypeDefNone(st()));
    }

    public static ParseTypeDefResult parse_record_type(Token name_tok, ParseState st)
    {
        return ((Func<ParseTypeDefResult, ParseTypeDefResult>)((st2) => ((Func<ParseTypeDefResult, ParseTypeDefResult>)((st3) => ((Func<ParseTypeDefResult, ParseTypeDefResult>)((st4) => (_p0_) => (_p1_) => parse_record_fields_loop(name_tok()(new List<object>())(st4), _p0_, _p1_)))(skip_newlines(st3))))(expect(LeftBrace)(st2))))(advance(st));
    }

    public static ParseTypeDefResult parse_record_fields_loop(Token name_tok, List<RecordFieldDef> acc, ParseState st)
    {
        return (is_right_brace(current_kind(st)) ? new TypeDefOk(new TypeDef(name_tok(), new List<object>(), new RecordBody(acc())), advance(st)) : (is_ident(current_kind(st)) ? (_p0_) => (_p1_) => parse_one_record_field(name_tok()(acc()(st)), _p0_, _p1_) : new TypeDefOk(new TypeDef(name_tok(), new List<object>(), new RecordBody(acc())), st())));
    }

    public static ParseTypeDefResult parse_one_record_field(Token name_tok, List<RecordFieldDef> acc, ParseState st)
    {
        return ((Func<ParseTypeDefResult, ParseTypeDefResult>)((field_name) => ((Func<ParseTypeDefResult, ParseTypeDefResult>)((st2) => ((Func<ParseTypeDefResult, ParseTypeDefResult>)((st3) => ((Func<ParseTypeDefResult, ParseTypeDefResult>)((field_type_result) => (_p0_) => (_p1_) => (_p2_) => unwrap_record_field_type(name_tok()(acc()(field_name(field_type_result))), _p0_, _p1_, _p2_)))(parse_type(st3))))(expect(Colon)(st2))))(advance(st))))(current(st));
    }

    public static ParseTypeDefResult unwrap_record_field_type(Token name_tok, List<RecordFieldDef> acc, Token field_name, ParseTypeResult r)
    {
        return (r is TypeOk _mTypeOk0_ ? ((Func<object, ParseTypeDefResult>)((st) => ((Func<object, ParseTypeDefResult>)((ft) => ((Func<ParseTypeDefResult, ParseTypeDefResult>)((field) => ((Func<ParseTypeDefResult, ParseTypeDefResult>)((st2) => (is_comma(current_kind(st2)) ? (_p0_) => (_p1_) => parse_record_fields_loop(name_tok()(Enumerable.Concat(acc(), new List<object>() { field }).ToList())(skip_newlines(advance(st2))), _p0_, _p1_) : (_p0_) => (_p1_) => parse_record_fields_loop(name_tok()(Enumerable.Concat(acc(), new List<object>() { field }).ToList())(st2), _p0_, _p1_))))(skip_newlines(st))))(new RecordFieldDef(field_name, ft))))(_mTypeOk0_.Field0)))(_mTypeOk0_.Field1) : throw new InvalidOperationException("Non-exhaustive match"))(parse_variant_type);
    }

    public static T271 Token<T271>()
    {
        return new ParseState();
    }

    public static T3084 ParseTypeDefResult<T3084>()
    {
        return parse_variant_type(name_tok)(st);
    }

    public static T3087 parse_variant_ctors<T3087>()
    {
        return new List<object>()(st);
    }

    public static T3087 parse_variant_ctors<T3087>(object name_tok, object acc, object st)
    {
        return (is_pipe(current_kind(st)) ? ((Func<object, object>)((st2) => ((Func<object, object>)((ctor_name) => ((Func<object, object>)((st3) => (_p0_) => (_p1_) => (_p2_) => (_p3_) => parse_ctor_fields(ctor_name(new List<object>())(st3(name_tok()(acc))), _p0_, _p1_, _p2_, _p3_)))(advance(st2))))(current(st2))))(skip_newlines(advance(st))) : new TypeDefOk(new TypeDef(name_tok(), new List<object>(), new VariantBody(acc())), st()));
    }

    public static ParseTypeDefResult parse_ctor_fields(Token ctor_name, List<TypeExpr> fields, ParseState st, Token name_tok, List<VariantCtorDef> acc)
    {
        return (is_left_paren(current_kind(st)) ? ((Func<ParseTypeDefResult, ParseTypeDefResult>)((field_result) => (_p0_) => (_p1_) => (_p2_) => (_p3_) => unwrap_ctor_field(field_result(ctor_name(fields(name_tok()(acc)))), _p0_, _p1_, _p2_, _p3_)))(parse_type(advance(st))) : ((Func<ParseTypeDefResult, ParseTypeDefResult>)((st2) => ((Func<ParseTypeDefResult, ParseTypeDefResult>)((ctor) => parse_variant_ctors()(name_tok()(Enumerable.Concat(acc(), new List<object>() { ctor() }).ToList())(st2))))(new VariantCtorDef(ctor_name, fields))))(skip_newlines(st)));
    }

    public static ParseTypeDefResult unwrap_ctor_field(ParseTypeResult r, Token ctor_name, List<TypeExpr> fields, Token name_tok, List<VariantCtorDef> acc)
    {
        return (r is TypeOk _mTypeOk0_ ? ((Func<object, ParseTypeDefResult>)((st) => ((Func<object, ParseTypeDefResult>)((ty) => ((Func<ParseTypeDefResult, ParseTypeDefResult>)((st2) => (_p0_) => (_p1_) => (_p2_) => (_p3_) => parse_ctor_fields(ctor_name(Enumerable.Concat(fields, new List<object>() { ty }).ToList())(st2()(name_tok()(acc))), _p0_, _p1_, _p2_, _p3_)))(expect(RightParen)(st))))(_mTypeOk0_.Field0)))(_mTypeOk0_.Field1) : throw new InvalidOperationException("Non-exhaustive match"))(parse_document);
    }

    public static T316 ParseState<T316>()
    {
        return new Document();
    }

    public static Document parse_document(object st)
    {
        return ((Func<object, object>)((st2) => parse_top_level(new List<object>(), new List<object>(), st2)))(skip_newlines(st));
    }

    public static Document parse_top_level(List<Def> defs, List<TypeDef> type_defs, ParseState st)
    {
        return (is_done(st) ? new Document(defs, type_defs) : (_p0_) => (_p1_) => try_top_level_type_def(defs(type_defs(st)), _p0_, _p1_));
    }

    public static Document try_top_level_type_def(List<Def> defs, List<TypeDef> type_defs, ParseState st)
    {
        return ((Func<Document, Document>)((td_result) => ((Func<Document, Document>)((_scrutinee0_) => (_scrutinee0_ is TypeDefOk _mTypeDefOk0_ ? ((Func<object, Document>)((st2) => ((Func<object, Document>)((td) => (_p0_) => (_p1_) => parse_top_level(defs(Enumerable.Concat(type_defs, new List<object>() { td }).ToList())(skip_newlines(st2)), _p0_, _p1_)))(_mTypeDefOk0_.Field0)))(_mTypeDefOk0_.Field1) : (_scrutinee0_ is TypeDefNone _mTypeDefNone0_ ? ((Func<object, Document>)((st2) => (_p0_) => (_p1_) => try_top_level_def(defs(type_defs(st)), _p0_, _p1_)))(_mTypeDefNone0_.Field0) : throw new InvalidOperationException("Non-exhaustive match")))))(td_result)(try_top_level_def)))(parse_type_def(st));
    }

    public static T736 List<T736>()
    {
        return throw new InvalidOperationException("->")(List)(TypeDef);
    }

    public static T316 ParseState<T316>()
    {
        return new Document();
    }

    public static Document try_top_level_def(object defs, object type_defs, object st)
    {
        return ((Func<object, object>)((def_result) => ((Func<object, object>)((_scrutinee0_) => (_scrutinee0_ is DefOk _mDefOk0_ ? ((Func<object, object>)((st2) => ((Func<object, object>)((d) => (_p0_) => parse_top_level(Enumerable.Concat(defs, new List<object>() { d }).ToList(), type_defs(skip_newlines(st2)), _p0_)))(_mDefOk0_.Field0)))(_mDefOk0_.Field1) : (_scrutinee0_ is DefNone _mDefNone0_ ? ((Func<object, object>)((st2) => (_p0_) => (_p1_) => parse_top_level(defs(type_defs(skip_newlines(advance(st2)))), _p0_, _p1_)))(_mDefNone0_.Field0) : throw new InvalidOperationException("Non-exhaustive match")))))(def_result)(Expr)))(parse_definition(st));
    }

    public static Func<string, Func<IRExpr, IRFieldVal>> LitExpr()
    {
        return Token;
    }

    public static Func<string, Func<IRExpr, IRFieldVal>> NameExpr()
    {
        return Token;
    }

    public static ParseState AppExpr()
    {
        return new Expr();
    }

    public static object Expr()
    {
        return throw new InvalidOperationException("|")(BinExpr)(Expr)(Token)(Expr);
    }

    public static Func<string, Func<IRExpr, IRFieldVal>> UnaryExpr()
    {
        return Token;
    }

    public static object Expr()
    {
        return throw new InvalidOperationException("|")(IfExpr)(Expr)(Expr)(Expr);
    }

    public static T4638 LetExpr<T4638>()
    {
        return new List(new LetBind());
    }

    public static object Expr()
    {
        return throw new InvalidOperationException("|")(MatchExpr)(Expr)(new List(new MatchArm()));
    }

    public static T3196 ListExpr<T3196>()
    {
        return new List(new Expr());
    }

    public static Func<string, Func<IRExpr, IRFieldVal>> RecordExpr()
    {
        return Token;
    }

    public static T736 List<T736>()
    {
        return throw new InvalidOperationException(")");
    }

    public static ParseState FieldExpr()
    {
        return new Expr();
    }

    public static T271 Token<T271>()
    {
        return throw new InvalidOperationException("|")(ParenExpr)(Expr);
    }

    public static T4638 DoExpr<T4638>()
    {
        return new List(new DoStmt());
    }

    public static Func<string, Func<IRExpr, IRFieldVal>> ErrExpr()
    {
        return Token;
    }

    public static long token_length(Token t)
    {
        return ((long)t.text.Length);
    }

    public static CheckResult infer_literal(UnificationState st, LiteralKind kind)
    {
        return ((Func<CheckResult, CheckResult>)((_scrutinee0_) => (_scrutinee0_ is IntLit _mIntLit0_ ? new CheckResult(new IntegerTy(), st()) : (_scrutinee0_ is NumLit _mNumLit0_ ? new CheckResult(new NumberTy(), st()) : (_scrutinee0_ is TextLit _mTextLit0_ ? new CheckResult(new TextTy(), st()) : (_scrutinee0_ is BoolLit _mBoolLit0_ ? new CheckResult(new BooleanTy(), st()) : throw new InvalidOperationException("Non-exhaustive match")))))))(kind())(infer_name);
    }

    public static Func<string, Func<List<IRParam>, Func<CodexType, Func<IRExpr, IRDef>>>> UnificationState()
    {
        return TypeEnv;
    }

    public static T241 Text<T241>()
    {
        return new CheckResult();
    }

    public static CheckResult infer_name(object st, object env, object name)
    {
        return ((_p0_) => env_has(env()(name), _p0_) ? ((Func<object, object>)((raw) => ((Func<object, object>)((inst) => new CheckResult(inst.var_type, inst.state)))((_p0_) => instantiate_type(st()(raw), _p0_))))((_p0_) => env_lookup(env()(name), _p0_)) : new CheckResult(new ErrorTy(), (_p0_) => (_p1_) => add_unify_error(st()("CDX2002")(Enumerable.Concat("Unknown name: ", name).ToList()), _p0_, _p1_)));
    }

    public static FreshResult instantiate_type(UnificationState st, CodexType ty)
    {
        return (ty is ForAllTy _mForAllTy0_ ? ((Func<object, FreshResult>)((body) => ((Func<object, FreshResult>)((var_id) => ((Func<FreshResult, FreshResult>)((fr) => ((Func<FreshResult, FreshResult>)((substituted) => (_p0_) => instantiate_type(fr().state(substituted), _p0_)))((_p0_) => (_p1_) => subst_type_var(body(var_id(fr().var_type)), _p0_, _p1_))))(fresh_and_advance(st))))(_mForAllTy0_.Field0)))(_mForAllTy0_.Field1) : new FreshResult(ty, st()))(subst_type_var);
    }

    public static Func<string, Func<IRExpr, IRFieldVal>> CodexType()
    {
        return Integer;
    }

    public static Func<string, Func<IRExpr, IRFieldVal>> CodexType()
    {
        return CodexType;
    }

    public static T3272 subst_type_var<T3272>(object ty, object var_id, object replacement)
    {
        return ((Func<object, object>)((_scrutinee0_) => (_scrutinee0_ is TypeVar _mTypeVar0_ ? ((Func<object, object>)((id) => ((id == var_id) ? replacement : ty)))(_mTypeVar0_.Field0) : (_scrutinee0_ is FunTy _mFunTy0_ ? ((Func<object, object>)((ret) => ((Func<object, object>)((param) => new FunTy((_p0_) => (_p1_) => subst_type_var(param(var_id(replacement)), _p0_, _p1_), (_p0_) => (_p1_) => subst_type_var(ret(var_id(replacement)), _p0_, _p1_))))(_mFunTy0_.Field0)))(_mFunTy0_.Field1) : (_scrutinee0_ is ListTy _mListTy0_ ? ((Func<object, object>)((elem) => new ListTy((_p0_) => (_p1_) => subst_type_var(elem(var_id(replacement)), _p0_, _p1_))))(_mListTy0_.Field0) : (_scrutinee0_ is ForAllTy _mForAllTy0_ ? ((Func<object, object>)((body) => ((Func<object, object>)((inner_id) => ((inner_id == var_id) ? ty : new ForAllTy(inner_id((_p0_) => (_p1_) => subst_type_var(body(var_id(replacement)), _p0_, _p1_))))))(_mForAllTy0_.Field0)))(_mForAllTy0_.Field1) : (_scrutinee0_ is ConstructedTy _mConstructedTy0_ ? ((Func<object, object>)((args) => ((Func<object, object>)((name) => new ConstructedTy(name(map_subst_type_var(args(var_id(replacement(0L)(((long)args.Count))(new List<object>()))))))))(_mConstructedTy0_.Field0)))(_mConstructedTy0_.Field1) : (_scrutinee0_ is SumTy _mSumTy0_ ? ((Func<object, object>)((ctors) => ((Func<object, object>)((name) => ty))(_mSumTy0_.Field0)))(_mSumTy0_.Field1) : (_scrutinee0_ is RecordTy _mRecordTy0_ ? ((Func<object, object>)((fields) => ((Func<object, object>)((name) => ty))(_mRecordTy0_.Field0)))(_mRecordTy0_.Field1) : ty)))))))))(ty)(map_subst_type_var);
    }

    public static T736 List<T736>()
    {
        return throw new InvalidOperationException("->")(Integer);
    }

    public static Func<string, Func<IRExpr, IRFieldVal>> CodexType()
    {
        return Integer;
    }

    public static T316 Integer<T316>()
    {
        return new List(new CodexType());
    }

    public static T736 List<T736>()
    {
        return map_subst_type_var(args)(var_id)(replacement)(i)(len)(acc);
    }

    public static long i()
    {
        return len;
    }

    public static T3298 acc<T3298>()
    {
        return throw new InvalidOperationException("else")(map_subst_type_var(args(var_id(replacement((i() + 1L))(len(Enumerable.Concat(acc(), new List<object>() { (_p0_) => subst_type_var(list_at()(args(i)), var_id(replacement), _p0_) }).ToList()))))));
    }

    public static CheckResult infer_binary(UnificationState st, TypeEnv env, AExpr left, BinaryOp op, AExpr right)
    {
        return ((Func<CheckResult, CheckResult>)((lr) => ((Func<CheckResult, CheckResult>)((rr) => (_p0_) => (_p1_) => (_p2_) => infer_binary_op(rr.state(lr.inferred_type(rr.inferred_type(op))), _p0_, _p1_, _p2_)))((_p0_) => (_p1_) => infer_expr(lr.state(env()(right)), _p0_, _p1_))))((_p0_) => (_p1_) => infer_expr(st()(env()(left)), _p0_, _p1_));
    }

    public static CheckResult infer_binary_op(UnificationState st, CodexType lt, CodexType rt, BinaryOp op)
    {
        return ((Func<CheckResult, CheckResult>)((_scrutinee0_) => (_scrutinee0_ is OpAdd _mOpAdd0_ ? (_p0_) => (_p1_) => infer_arithmetic(st()(lt(rt)), _p0_, _p1_) : (_scrutinee0_ is OpSub _mOpSub0_ ? (_p0_) => (_p1_) => infer_arithmetic(st()(lt(rt)), _p0_, _p1_) : (_scrutinee0_ is OpMul _mOpMul0_ ? (_p0_) => (_p1_) => infer_arithmetic(st()(lt(rt)), _p0_, _p1_) : (_scrutinee0_ is OpDiv _mOpDiv0_ ? (_p0_) => (_p1_) => infer_arithmetic(st()(lt(rt)), _p0_, _p1_) : (_scrutinee0_ is OpPow _mOpPow0_ ? (_p0_) => (_p1_) => infer_arithmetic(st()(lt(rt)), _p0_, _p1_) : (_scrutinee0_ is OpEq _mOpEq0_ ? (_p0_) => (_p1_) => infer_comparison(st()(lt(rt)), _p0_, _p1_) : (_scrutinee0_ is OpNotEq _mOpNotEq0_ ? (_p0_) => (_p1_) => infer_comparison(st()(lt(rt)), _p0_, _p1_) : (_scrutinee0_ is OpLt _mOpLt0_ ? (_p0_) => (_p1_) => infer_comparison(st()(lt(rt)), _p0_, _p1_) : (_scrutinee0_ is OpGt _mOpGt0_ ? (_p0_) => (_p1_) => infer_comparison(st()(lt(rt)), _p0_, _p1_) : (_scrutinee0_ is OpLtEq _mOpLtEq0_ ? (_p0_) => (_p1_) => infer_comparison(st()(lt(rt)), _p0_, _p1_) : (_scrutinee0_ is OpGtEq _mOpGtEq0_ ? (_p0_) => (_p1_) => infer_comparison(st()(lt(rt)), _p0_, _p1_) : (_scrutinee0_ is OpAnd _mOpAnd0_ ? (_p0_) => (_p1_) => infer_logical(st()(lt(rt)), _p0_, _p1_) : (_scrutinee0_ is OpOr _mOpOr0_ ? (_p0_) => (_p1_) => infer_logical(st()(lt(rt)), _p0_, _p1_) : (_scrutinee0_ is OpAppend _mOpAppend0_ ? (_p0_) => (_p1_) => infer_append(st()(lt(rt)), _p0_, _p1_) : (_scrutinee0_ is OpCons _mOpCons0_ ? (_p0_) => (_p1_) => infer_cons(st()(lt(rt)), _p0_, _p1_) : (_scrutinee0_ is OpDefEq _mOpDefEq0_ ? (_p0_) => (_p1_) => infer_comparison(st()(lt(rt)), _p0_, _p1_) : throw new InvalidOperationException("Non-exhaustive match")))))))))))))))))))(op())(infer_arithmetic);
    }

    public static Func<string, Func<List<IRParam>, Func<CodexType, Func<IRExpr, IRDef>>>> UnificationState()
    {
        return CodexType;
    }

    public static Func<string, Func<IRExpr, IRFieldVal>> CodexType()
    {
        return CheckResult;
    }

    public static CheckResult infer_arithmetic(object st, object lt, object rt)
    {
        return ((Func<object, object>)((r) => new CheckResult(lt, r.state)))((_p0_) => (_p1_) => unify(st()(lt(rt)), _p0_, _p1_));
    }

    public static CheckResult infer_comparison(UnificationState st, CodexType lt, CodexType rt)
    {
        return ((Func<CheckResult, CheckResult>)((r) => new CheckResult(new BooleanTy(), r.state)))((_p0_) => (_p1_) => unify(st()(lt(rt)), _p0_, _p1_));
    }

    public static CheckResult infer_logical(UnificationState st, CodexType lt, CodexType rt)
    {
        return ((Func<CheckResult, CheckResult>)((r1) => ((Func<CheckResult, CheckResult>)((r2) => new CheckResult(new BooleanTy(), r2.state)))((_p0_) => (_p1_) => unify(r1.state(rt(BooleanTy)), _p0_, _p1_))))((_p0_) => (_p1_) => unify(st()(lt(BooleanTy)), _p0_, _p1_));
    }

    public static CheckResult infer_append(UnificationState st, CodexType lt, CodexType rt)
    {
        return ((Func<CheckResult, CheckResult>)((resolved) => (resolved is TextTy _mTextTy0_ ? ((Func<CheckResult, CheckResult>)((r) => new CheckResult(new TextTy(), r.state)))((_p0_) => (_p1_) => unify(st()(rt(TextTy)), _p0_, _p1_)) : ((Func<CheckResult, CheckResult>)((r) => new CheckResult(lt, r.state)))((_p0_) => (_p1_) => unify(st()(lt(rt)), _p0_, _p1_)))(infer_cons)))((_p0_) => resolve(st()(lt), _p0_));
    }

    public static Func<string, Func<List<IRParam>, Func<CodexType, Func<IRExpr, IRDef>>>> UnificationState()
    {
        return CodexType;
    }

    public static Func<string, Func<IRExpr, IRFieldVal>> CodexType()
    {
        return CheckResult;
    }

    public static CheckResult infer_cons(object st, object lt, object rt)
    {
        return ((Func<object, object>)((list_ty) => ((Func<object, object>)((r) => new CheckResult(list_ty, r.state)))((_p0_) => (_p1_) => unify(st()(rt(list_ty)), _p0_, _p1_))))(new ListTy(lt));
    }

    public static CheckResult infer_if(UnificationState st, TypeEnv env, AExpr cond, AExpr then_e, AExpr else_e)
    {
        return ((Func<CheckResult, CheckResult>)((cr) => ((Func<CheckResult, CheckResult>)((r1) => ((Func<CheckResult, CheckResult>)((tr) => ((Func<CheckResult, CheckResult>)((er) => ((Func<CheckResult, CheckResult>)((r2) => new CheckResult(tr.inferred_type, r2.state)))((_p0_) => (_p1_) => unify(er.state(tr.inferred_type(er.inferred_type)), _p0_, _p1_))))((_p0_) => (_p1_) => infer_expr(tr.state(env()(else_e)), _p0_, _p1_))))((_p0_) => (_p1_) => infer_expr(r1.state(env()(then_e)), _p0_, _p1_))))((_p0_) => (_p1_) => unify(cr.state(cr.inferred_type(BooleanTy)), _p0_, _p1_))))((_p0_) => (_p1_) => infer_expr(st()(env()(cond)), _p0_, _p1_));
    }

    public static CheckResult infer_let(UnificationState st, TypeEnv env, List<ALetBind> bindings, AExpr body)
    {
        return ((Func<CheckResult, CheckResult>)((env2) => (_p0_) => (_p1_) => infer_expr(env2().state(env2().env(body)), _p0_, _p1_)))((_p0_) => (_p1_) => (_p2_) => (_p3_) => infer_let_bindings(st()(env()(bindings(0L)(((long)bindings.Count)))), _p0_, _p1_, _p2_, _p3_));
    }

    public static LetBindResult infer_let_bindings(UnificationState st, TypeEnv env, List<ALetBind> bindings, long i, long len)
    {
        while (true)
        {
            if ((i() == len))
            {
                return new LetBindResult(st(), env());
            }
            else
            {
                var b = list_at()(bindings(i));
                var vr = (_p0_) => (_p1_) => infer_expr(st()(env()(b().value)), _p0_, _p1_);
                var env2 = (_p0_) => (_p1_) => env_bind(env()(b().name.value(vr.inferred_type)), _p0_, _p1_);
                var _tco_0 = vr.state(env2()(bindings((i() + 1L))(len)));
                st = _tco_0;
                continue;
            }
        }
    }

    public static CheckResult infer_lambda(UnificationState st, TypeEnv env, List<Name> @params, AExpr body)
    {
        return ((Func<CheckResult, CheckResult>)((pr) => ((Func<CheckResult, CheckResult>)((br) => ((Func<CheckResult, CheckResult>)((fun_ty) => new CheckResult(fun_ty, br.state)))((_p0_) => wrap_fun_type(pr().param_types(br.inferred_type), _p0_))))((_p0_) => (_p1_) => infer_expr(pr().state(pr().env(body)), _p0_, _p1_))))((_p0_) => (_p1_) => (_p2_) => (_p3_) => (_p4_) => bind_lambda_params(st()(env()(@params(0L)(((long)@params.Count))(new List<object>()))), _p0_, _p1_, _p2_, _p3_, _p4_));
    }

    public static LambdaBindResult bind_lambda_params(UnificationState st, TypeEnv env, List<Name> @params, long i, long len, List<CodexType> acc)
    {
        while (true)
        {
            if ((i() == len))
            {
                return new LambdaBindResult(st(), env(), acc());
            }
            else
            {
                var p = list_at()(@params(i));
                var fr = fresh_and_advance(st);
                var env2 = (_p0_) => (_p1_) => env_bind(env()(p().value(fr().var_type)), _p0_, _p1_);
                var _tco_0 = fr().state(env2()(@params((i() + 1L))(len(Enumerable.Concat(acc(), new List<object>() { fr().var_type }).ToList()))));
                st = _tco_0;
                continue;
            }
        }
    }

    public static CodexType wrap_fun_type(List<CodexType> param_types, CodexType result)
    {
        return (_p0_) => (_p1_) => wrap_fun_type_loop(param_types(result((((long)param_types.Count) - 1L))), _p0_, _p1_);
    }

    public static CodexType wrap_fun_type_loop(List<CodexType> param_types, CodexType result, long i)
    {
        while (true)
        {
            if ((i() < 0L))
            {
                return result;
            }
            else
            {
                var _tco_0 = param_types(new FunTy(list_at()(param_types(i)), result))((i() - 1L));
                param_types = _tco_0;
                continue;
            }
        }
    }

    public static CheckResult infer_application(UnificationState st, TypeEnv env, AExpr func, AExpr arg)
    {
        return ((Func<CheckResult, CheckResult>)((fr) => ((Func<CheckResult, CheckResult>)((ar) => ((Func<CheckResult, CheckResult>)((ret) => ((Func<CheckResult, CheckResult>)((r) => new CheckResult(ret.var_type, r.state)))((_p0_) => (_p1_) => unify(ret.state(fr().inferred_type(new FunTy(ar.inferred_type(ret.var_type)))), _p0_, _p1_))))(fresh_and_advance(ar.state))))((_p0_) => (_p1_) => infer_expr(fr().state(env()(arg)), _p0_, _p1_))))((_p0_) => (_p1_) => infer_expr(st()(env()(func)), _p0_, _p1_));
    }

    public static CheckResult infer_list(UnificationState st, TypeEnv env, List<AExpr> elems)
    {
        return ((((long)elems().Count) == 0L) ? ((Func<CheckResult, CheckResult>)((fr) => new CheckResult(new ListTy(fr().var_type), fr().state)))(fresh_and_advance(st)) : ((Func<CheckResult, CheckResult>)((first) => ((Func<CheckResult, CheckResult>)((st2) => new CheckResult(new ListTy(first.inferred_type), st2())))((_p0_) => (_p1_) => (_p2_) => (_p3_) => (_p4_) => unify_list_elems(first.state(env()(elems()(first.inferred_type(1L)(((long)elems().Count))))), _p0_, _p1_, _p2_, _p3_, _p4_))))((_p0_) => (_p1_) => infer_expr(st()(env()(list_at()(elems()(0L)))), _p0_, _p1_)));
    }

    public static UnificationState unify_list_elems(UnificationState st, TypeEnv env, List<AExpr> elems, CodexType elem_ty, long i, long len)
    {
        while (true)
        {
            if ((i() == len))
            {
                return st();
            }
            else
            {
                var er = (_p0_) => (_p1_) => infer_expr(st()(env()(list_at()(elems()(i)))), _p0_, _p1_);
                var r = (_p0_) => (_p1_) => unify(er.state(er.inferred_type(elem_ty)), _p0_, _p1_);
                var _tco_0 = r.state(env()(elems()(elem_ty()((i() + 1L))(len))));
                st = _tco_0;
                continue;
            }
        }
    }

    public static CheckResult infer_match(UnificationState st, TypeEnv env, AExpr scrutinee, List<AMatchArm> arms)
    {
        return ((Func<CheckResult, CheckResult>)((sr) => ((Func<CheckResult, CheckResult>)((fr) => ((Func<CheckResult, CheckResult>)((st2) => new CheckResult(fr().var_type, st2())))((_p0_) => (_p1_) => (_p2_) => (_p3_) => (_p4_) => (_p5_) => infer_match_arms(fr().state(env()(sr.inferred_type(fr().var_type(arms(0L)(((long)arms.Count)))))), _p0_, _p1_, _p2_, _p3_, _p4_, _p5_))))(fresh_and_advance(sr.state))))((_p0_) => (_p1_) => infer_expr(st()(env()(scrutinee)), _p0_, _p1_));
    }

    public static UnificationState infer_match_arms(UnificationState st, TypeEnv env, CodexType scrut_ty, CodexType result_ty, List<AMatchArm> arms, long i, long len)
    {
        while (true)
        {
            if ((i() == len))
            {
                return st();
            }
            else
            {
                var arm = list_at()(arms(i));
                var pr = (_p0_) => (_p1_) => (_p2_) => bind_pattern(st()(env()(arm.pattern(scrut_ty))), _p0_, _p1_, _p2_);
                var br = (_p0_) => (_p1_) => infer_expr(pr().state(pr().env(arm.body)), _p0_, _p1_);
                var r = (_p0_) => (_p1_) => unify(br.state(br.inferred_type(result_ty)), _p0_, _p1_);
                var _tco_0 = r.state(env()(scrut_ty(result_ty(arms((i() + 1L))(len)))));
                st = _tco_0;
                continue;
            }
        }
    }

    public static PatBindResult bind_pattern(UnificationState st, TypeEnv env, APat pat, CodexType ty)
    {
        return ((Func<PatBindResult, PatBindResult>)((_scrutinee0_) => (_scrutinee0_ is AVarPat _mAVarPat0_ ? ((Func<object, PatBindResult>)((name) => new PatBindResult(st(), (_p0_) => (_p1_) => env_bind(env()(name.value(ty)), _p0_, _p1_))))(_mAVarPat0_.Field0) : (_scrutinee0_ is AWildPat _mAWildPat0_ ? new PatBindResult(st(), env()) : (_scrutinee0_ is ALitPat _mALitPat0_ ? ((Func<object, PatBindResult>)((kind) => ((Func<object, PatBindResult>)((val) => new PatBindResult(st(), env())))(_mALitPat0_.Field0)))(_mALitPat0_.Field1) : (_scrutinee0_ is ACtorPat _mACtorPat0_ ? ((Func<object, PatBindResult>)((sub_pats) => ((Func<object, PatBindResult>)((ctor_name) => ((Func<PatBindResult, PatBindResult>)((ctor_lookup) => bind_ctor_sub_patterns()(ctor_lookup.state(env()(sub_pats(ctor_lookup.var_type(0L)(((long)sub_pats.Count))))))))((_p0_) => instantiate_type(st()((_p0_) => env_lookup(env()(ctor_name.value), _p0_)), _p0_))))(_mACtorPat0_.Field0)))(_mACtorPat0_.Field1) : throw new InvalidOperationException("Non-exhaustive match")))))))(pat)(bind_ctor_sub_patterns);
    }

    public static Func<string, Func<List<IRParam>, Func<CodexType, Func<IRExpr, IRDef>>>> UnificationState()
    {
        return TypeEnv;
    }

    public static T736 List<T736>()
    {
        return throw new InvalidOperationException("->")(CodexType);
    }

    public static T316 Integer<T316>()
    {
        return new Integer();
    }

    public static T3644 PatBindResult<T3644>()
    {
        return bind_ctor_sub_patterns()(st)(env)(sub_pats)(ctor_ty)(i)(len);
    }

    public static long i()
    {
        return len;
    }

    public static T3644 PatBindResult<T3644>()
    {
        return state;
    }

    public static Func<a, Func<object, T3674>> st<T3674>()
    {
        return env;
    }

    public static object env()
    {
        return throw new InvalidOperationException("else");
    }

    public static PatBindResult ctor_ty()
    {
        return (new FunTy(param_ty, ret_ty) ? ((Func<PatBindResult, PatBindResult>)((pr) => bind_ctor_sub_patterns()(pr().state(pr().env(sub_pats(ret_ty((i() + 1L))(len)))))))((_p0_) => (_p1_) => (_p2_) => bind_pattern(st()(env()(list_at()(sub_pats(i)))(param_ty)), _p0_, _p1_, _p2_)) : throw new InvalidOperationException("_"));
    }

    public static T3667 fr<T3667>()
    {
        return fresh_and_advance(st);
    }

    public static Func<TypeEnv, Func<APat, Func<CodexType, PatBindResult>>> pr()
    {
        return (_p0_) => (_p1_) => (_p2_) => bind_pattern(fr.state(env(list_at(sub_pats(new Func<TypeEnv, Func<APat, Func<CodexType, PatBindResult>>>(i))))(fr.var_type)), _p0_, _p1_, _p2_);
    }

    public static T3684 bind_ctor_sub_patterns<T3684>()
    {
        return throw new InvalidOperationException(".")(state(pr().env(sub_pats(ctor_ty()((i() + 1L))(len)))));
    }

    public static CheckResult infer_do(UnificationState st, TypeEnv env, List<ADoStmt> stmts)
    {
        return (_p0_) => (_p1_) => (_p2_) => (_p3_) => (_p4_) => infer_do_loop(st()(env()(stmts(0L)(((long)stmts.Count))(NothingTy))), _p0_, _p1_, _p2_, _p3_, _p4_);
    }

    public static CheckResult infer_do_loop(UnificationState st, TypeEnv env, List<ADoStmt> stmts, long i, long len, CodexType last_ty)
    {
        return ((i() == len) ? new CheckResult(last_ty, st()) : ((Func<CheckResult, CheckResult>)((stmt) => ((Func<CheckResult, CheckResult>)((_scrutinee0_) => (_scrutinee0_ is ADoExprStmt _mADoExprStmt0_ ? ((Func<object, CheckResult>)((e) => ((Func<CheckResult, CheckResult>)((er) => (_p0_) => (_p1_) => (_p2_) => (_p3_) => (_p4_) => infer_do_loop(er.state(env()(stmts((i() + 1L))(len(er.inferred_type)))), _p0_, _p1_, _p2_, _p3_, _p4_)))((_p0_) => (_p1_) => infer_expr(st()(env()(e)), _p0_, _p1_))))(_mADoExprStmt0_.Field0) : (_scrutinee0_ is ADoBindStmt _mADoBindStmt0_ ? ((Func<object, CheckResult>)((e) => ((Func<object, CheckResult>)((name) => ((Func<CheckResult, CheckResult>)((er) => ((Func<CheckResult, CheckResult>)((env2) => (_p0_) => (_p1_) => (_p2_) => (_p3_) => (_p4_) => infer_do_loop(er.state(env2()(stmts((i() + 1L))(len(er.inferred_type)))), _p0_, _p1_, _p2_, _p3_, _p4_)))((_p0_) => (_p1_) => env_bind(env()(name.value(er.inferred_type)), _p0_, _p1_))))((_p0_) => (_p1_) => infer_expr(st()(env()(e)), _p0_, _p1_))))(_mADoBindStmt0_.Field0)))(_mADoBindStmt0_.Field1) : throw new InvalidOperationException("Non-exhaustive match")))))(stmt)(infer_expr)))(list_at()(stmts(i))));
    }

    public static Func<string, Func<List<IRParam>, Func<CodexType, Func<IRExpr, IRDef>>>> UnificationState()
    {
        return TypeEnv;
    }

    public static Func<string, Func<IRExpr, IRFieldVal>> AExpr()
    {
        return CheckResult;
    }

    public static T3817 infer_expr<T3817>(object st, object env, object expr)
    {
        return ((Func<object, object>)((_scrutinee0_) => (_scrutinee0_ is ALitExpr _mALitExpr0_ ? ((Func<object, object>)((kind) => ((Func<object, object>)((val) => (_p0_) => infer_literal(st()(kind), _p0_)))(_mALitExpr0_.Field0)))(_mALitExpr0_.Field1) : (_scrutinee0_ is ANameExpr _mANameExpr0_ ? ((Func<object, object>)((name) => (_p0_) => (_p1_) => infer_name(st()(env()(name.value)), _p0_, _p1_)))(_mANameExpr0_.Field0) : (_scrutinee0_ is ABinaryExpr _mABinaryExpr0_ ? ((Func<object, object>)((right) => ((Func<object, object>)((op) => ((Func<object, object>)((left) => (_p0_) => (_p1_) => (_p2_) => (_p3_) => infer_binary(st()(env()(left(op()(right)))), _p0_, _p1_, _p2_, _p3_)))(_mABinaryExpr0_.Field0)))(_mABinaryExpr0_.Field1)))(_mABinaryExpr0_.Field2) : (_scrutinee0_ is AUnaryExpr _mAUnaryExpr0_ ? ((Func<object, object>)((operand) => ((Func<object, object>)((r) => ((Func<object, object>)((u) => new CheckResult(new IntegerTy(), u.state)))((_p0_) => (_p1_) => unify(r.state(r.inferred_type(IntegerTy)), _p0_, _p1_))))((_p0_) => (_p1_) => infer_expr(st()(env()(operand)), _p0_, _p1_))))(_mAUnaryExpr0_.Field0) : (_scrutinee0_ is AApplyExpr _mAApplyExpr0_ ? ((Func<object, object>)((arg) => ((Func<object, object>)((func) => (_p0_) => (_p1_) => (_p2_) => infer_application(st()(env()(func(arg))), _p0_, _p1_, _p2_)))(_mAApplyExpr0_.Field0)))(_mAApplyExpr0_.Field1) : (_scrutinee0_ is AIfExpr _mAIfExpr0_ ? ((Func<object, object>)((else_e) => ((Func<object, object>)((then_e) => ((Func<object, object>)((cond) => (_p0_) => (_p1_) => (_p2_) => (_p3_) => infer_if(st()(env()(cond(then_e(else_e)))), _p0_, _p1_, _p2_, _p3_)))(_mAIfExpr0_.Field0)))(_mAIfExpr0_.Field1)))(_mAIfExpr0_.Field2) : (_scrutinee0_ is ALetExpr _mALetExpr0_ ? ((Func<object, object>)((body) => ((Func<object, object>)((bindings) => (_p0_) => (_p1_) => (_p2_) => infer_let(st()(env()(bindings(body))), _p0_, _p1_, _p2_)))(_mALetExpr0_.Field0)))(_mALetExpr0_.Field1) : (_scrutinee0_ is ALambdaExpr _mALambdaExpr0_ ? ((Func<object, object>)((body) => ((Func<object, object>)((@params) => (_p0_) => (_p1_) => (_p2_) => infer_lambda(st()(env()(@params(body))), _p0_, _p1_, _p2_)))(_mALambdaExpr0_.Field0)))(_mALambdaExpr0_.Field1) : (_scrutinee0_ is AMatchExpr _mAMatchExpr0_ ? ((Func<object, object>)((arms) => ((Func<object, object>)((scrutinee) => (_p0_) => (_p1_) => (_p2_) => infer_match(st()(env()(scrutinee(arms))), _p0_, _p1_, _p2_)))(_mAMatchExpr0_.Field0)))(_mAMatchExpr0_.Field1) : (_scrutinee0_ is AListExpr _mAListExpr0_ ? ((Func<object, object>)((elems) => (_p0_) => (_p1_) => infer_list(st()(env()(elems)), _p0_, _p1_)))(_mAListExpr0_.Field0) : (_scrutinee0_ is ADoExpr _mADoExpr0_ ? ((Func<object, object>)((stmts) => (_p0_) => (_p1_) => infer_do(st()(env()(stmts)), _p0_, _p1_)))(_mADoExpr0_.Field0) : (_scrutinee0_ is AFieldAccess _mAFieldAccess0_ ? ((Func<object, object>)((field) => ((Func<object, object>)((obj) => ((Func<object, object>)((r) => ((Func<object, object>)((fr) => new CheckResult(fr().var_type, fr().state)))(fresh_and_advance(r.state))))((_p0_) => (_p1_) => infer_expr(st()(env()(obj)), _p0_, _p1_))))(_mAFieldAccess0_.Field0)))(_mAFieldAccess0_.Field1) : (_scrutinee0_ is ARecordExpr _mARecordExpr0_ ? ((Func<object, object>)((fields) => ((Func<object, object>)((name) => ((Func<object, object>)((st2) => new CheckResult(new ConstructedTy(name(new List<object>())), st2())))((_p0_) => (_p1_) => (_p2_) => (_p3_) => infer_record_fields(st()(env()(fields(0L)(((long)fields.Count)))), _p0_, _p1_, _p2_, _p3_))))(_mARecordExpr0_.Field0)))(_mARecordExpr0_.Field1) : (_scrutinee0_ is AErrorExpr _mAErrorExpr0_ ? ((Func<object, object>)((msg) => new CheckResult(new ErrorTy(), st())))(_mAErrorExpr0_.Field0) : throw new InvalidOperationException("Non-exhaustive match")))))))))))))))))(expr)(infer_record_fields);
    }

    public static Func<string, Func<List<IRParam>, Func<CodexType, Func<IRExpr, IRDef>>>> UnificationState()
    {
        return TypeEnv;
    }

    public static T736 List<T736>()
    {
        return throw new InvalidOperationException("->")(Integer);
    }

    public static T316 Integer<T316>()
    {
        return new UnificationState();
    }

    public static UnificationState infer_record_fields<T3837>(T3837 st, object env, object fields, object i, object len)
    {
        while (true)
        {
            if ((i() == len))
            {
                return st();
            }
            else
            {
                var f = list_at()(fields(i));
                var r = (_p0_) => (_p1_) => infer_expr(st()(env()(f.value)), _p0_, _p1_);
                var _tco_0 = r.state(env()(fields((i() + 1L))(len)));
                st = _tco_0;
                continue;
            }
        }
    }

    public static CodexType resolve_type_expr(ATypeExpr texpr)
    {
        return ((Func<CodexType, CodexType>)((_scrutinee0_) => (_scrutinee0_ is ANamedType _mANamedType0_ ? ((Func<object, CodexType>)((name) => resolve_type_name(name.value)))(_mANamedType0_.Field0) : (_scrutinee0_ is AFunType _mAFunType0_ ? ((Func<object, CodexType>)((ret) => ((Func<object, CodexType>)((param) => new FunTy(resolve_type_expr(param), resolve_type_expr(ret))))(_mAFunType0_.Field0)))(_mAFunType0_.Field1) : (_scrutinee0_ is AAppType _mAAppType0_ ? ((Func<object, CodexType>)((args) => ((Func<object, CodexType>)((ctor) => resolve_applied_type(ctor()(args))))(_mAAppType0_.Field0)))(_mAAppType0_.Field1) : throw new InvalidOperationException("Non-exhaustive match"))))))(texpr)(resolve_applied_type);
    }

    public static T4638 ATypeExpr<T4638>()
    {
        return new List(new ATypeExpr());
    }

    public static Func<string, Func<IRExpr, IRFieldVal>> CodexType()
    {
        return resolve_applied_type(new Func<string, Func<IRExpr, IRFieldVal>>(ctor))(new Func<string, Func<IRExpr, IRFieldVal>>(args));
    }

    public static CodexType ctor()
    {
        return (new ANamedType(name) ? ((name.value == "List") ? ((((long)args.Count) == 1L) ? new ListTy(resolve_type_expr(list_at()(args(0L)))) : new ListTy(new ErrorTy())) : new ConstructedTy(name(resolve_type_expr_list(args(0L)(((long)args.Count))(new List<object>()))))) : throw new InvalidOperationException("_"));
    }

    public static Func<ATypeExpr, CodexType> resolve_type_expr()
    {
        return resolve_type_expr_list;
    }

    public static T736 List<T736>()
    {
        return throw new InvalidOperationException("->")(Integer);
    }

    public static T316 Integer<T316>()
    {
        return new List(new CodexType());
    }

    public static T736 List<T736>()
    {
        return resolve_type_expr_list(args)(i)(len)(acc);
    }

    public static long i()
    {
        return len;
    }

    public static T3298 acc<T3298>()
    {
        return throw new InvalidOperationException("else")(resolve_type_expr_list(args((i() + 1L))(len(Enumerable.Concat(acc(), new List<object>() { resolve_type_expr(list_at()(args(i))) }).ToList()))));
    }

    public static CodexType resolve_type_name(string name)
    {
        return ((name == "Integer") ? new IntegerTy() : ((name == "Number") ? new NumberTy() : ((name == "Text") ? new TextTy() : ((name == "Boolean") ? new BooleanTy() : ((name == "Nothing") ? new NothingTy() : new ConstructedTy(new Name(name), new List<object>()))))));
    }

    public static CheckResult check_def(UnificationState st, TypeEnv env, ADef def)
    {
        return ((Func<CheckResult, CheckResult>)((declared) => ((Func<CheckResult, CheckResult>)((env2) => ((Func<CheckResult, CheckResult>)((body_r) => ((Func<CheckResult, CheckResult>)((u) => new CheckResult(declared.expected_type, u.state)))((_p0_) => (_p1_) => unify(body_r.state(env2().remaining_type(body_r.inferred_type)), _p0_, _p1_))))((_p0_) => (_p1_) => infer_expr(env2().state(env2().env(def.body)), _p0_, _p1_))))((_p0_) => (_p1_) => (_p2_) => (_p3_) => (_p4_) => bind_def_params(declared.state(declared.env(def.@params(declared.expected_type(0L)(((long)def.@params.Count))))), _p0_, _p1_, _p2_, _p3_, _p4_))))((_p0_) => (_p1_) => resolve_declared_type(st()(env()(def)), _p0_, _p1_));
    }

    public static DefSetup resolve_declared_type(UnificationState st, TypeEnv env, ADef def)
    {
        return ((((long)def.declared_type.Count) == 0L) ? ((Func<DefSetup, DefSetup>)((fr) => new DefSetup(fr().var_type, fr().var_type, fr().state, env())))(fresh_and_advance(st)) : ((Func<DefSetup, DefSetup>)((ty) => new DefSetup(ty, ty, st(), env())))(resolve_type_expr(list_at()(def.declared_type(0L)))));
    }

    public static DefParamResult bind_def_params(UnificationState st, TypeEnv env, List<AParam> @params, CodexType remaining, long i, long len)
    {
        return ((i() == len) ? new DefParamResult(st(), env(), remaining) : ((Func<DefParamResult, DefParamResult>)((p) => (remaining is FunTy _mFunTy0_ ? ((Func<object, DefParamResult>)((ret_ty) => ((Func<object, DefParamResult>)((param_ty) => ((Func<DefParamResult, DefParamResult>)((env2) => (_p0_) => (_p1_) => (_p2_) => (_p3_) => (_p4_) => bind_def_params(st()(env2()(@params(ret_ty((i() + 1L))(len)))), _p0_, _p1_, _p2_, _p3_, _p4_)))((_p0_) => (_p1_) => env_bind(env()(p().name.value(param_ty)), _p0_, _p1_))))(_mFunTy0_.Field0)))(_mFunTy0_.Field1) : ((Func<DefParamResult, DefParamResult>)((fr) => ((Func<DefParamResult, DefParamResult>)((env2) => (_p0_) => (_p1_) => (_p2_) => (_p3_) => (_p4_) => bind_def_params(fr().state(env2()(@params(remaining((i() + 1L))(len)))), _p0_, _p1_, _p2_, _p3_, _p4_)))((_p0_) => (_p1_) => env_bind(env()(p().name.value(fr().var_type)), _p0_, _p1_))))(fresh_and_advance(st)))(ModuleResult)))(list_at()(@params(i))));
    }

    public static List<string> ,()
    {
        return state;
    }

    public static Func<string, Func<List<IRParam>, Func<CodexType, Func<IRExpr, IRDef>>>> UnificationState()
    {
        return throw new InvalidOperationException("}");
    }

    public static ModuleResult check_module(AModule mod)
    {
        return ((Func<ModuleResult, ModuleResult>)((tenv) => ((Func<ModuleResult, ModuleResult>)((env) => (_p0_) => (_p1_) => (_p2_) => (_p3_) => (_p4_) => check_all_defs(env().state(env().env(mod.defs(0L)(((long)mod.defs.Count))(new List<object>()))), _p0_, _p1_, _p2_, _p3_, _p4_)))((_p0_) => (_p1_) => (_p2_) => (_p3_) => register_all_defs(tenv.state(tenv.env(mod.defs(0L)(((long)mod.defs.Count)))), _p0_, _p1_, _p2_, _p3_))))((_p0_) => (_p1_) => (_p2_) => (_p3_) => register_type_defs(empty_unification_state()(builtin_type_env()(mod.type_defs(0L)(((long)mod.type_defs.Count)))), _p0_, _p1_, _p2_, _p3_));
    }

    public static LetBindResult register_all_defs(UnificationState st, TypeEnv env, List<ADef> defs, long i, long len)
    {
        while (true)
        {
            if ((i() == len))
            {
                return new LetBindResult(st(), env());
            }
            else
            {
                var def = list_at()(defs(i));
                var ty = ((((long)def.declared_type.Count) == 0L) ? ((Func<object, object>)((fr) => ((Func<object, object>)((env2) => new LetBindResult(fr().state, env2())))((_p0_) => (_p1_) => env_bind(env()(def.name.value(fr().var_type)), _p0_, _p1_))))(fresh_and_advance(st)) : ((Func<object, object>)((resolved) => new LetBindResult(st(), (_p0_) => (_p1_) => env_bind(env()(def.name.value(resolved)), _p0_, _p1_))))(resolve_type_expr(list_at()(def.declared_type(0L)))));
                var _tco_0 = ty.state(ty.env(defs((i() + 1L))(len)));
                st = _tco_0;
                continue;
            }
        }
    }

    public static ModuleResult check_all_defs(UnificationState st, TypeEnv env, List<ADef> defs, long i, long len, List<TypeBinding> acc)
    {
        while (true)
        {
            if ((i() == len))
            {
                return new ModuleResult(acc(), st());
            }
            else
            {
                var def = list_at()(defs(i));
                var r = (_p0_) => (_p1_) => check_def(st()(env()(def)), _p0_, _p1_);
                var resolved = (_p0_) => deep_resolve(r.state(r.inferred_type), _p0_);
                var entry = new TypeBinding(def.name.value, resolved);
                var _tco_0 = r.state(env()(defs((i() + 1L))(len(Enumerable.Concat(acc(), new List<object>() { entry }).ToList()))));
                st = _tco_0;
                continue;
            }
        }
    }

    public static LetBindResult register_type_defs(UnificationState st, TypeEnv env, List<ATypeDef> tdefs, long i, long len)
    {
        while (true)
        {
            if ((i() == len))
            {
                return new LetBindResult(st(), env());
            }
            else
            {
                var td = list_at()(tdefs(i));
                var r = (_p0_) => (_p1_) => register_one_type_def(st()(env()(td)), _p0_, _p1_);
                var _tco_0 = r.state(r.env(tdefs((i() + 1L))(len)));
                st = _tco_0;
                continue;
            }
        }
    }

    public static LetBindResult register_one_type_def(UnificationState st, TypeEnv env, ATypeDef td)
    {
        return ((Func<LetBindResult, LetBindResult>)((_scrutinee0_) => (_scrutinee0_ is AVariantTypeDef _mAVariantTypeDef0_ ? ((Func<object, LetBindResult>)((ctors) => ((Func<object, LetBindResult>)((type_params) => ((Func<object, LetBindResult>)((name) => ((Func<LetBindResult, LetBindResult>)((result_ty) => register_variant_ctors()(st()(env()(ctors(result_ty(0L)(((long)ctors.Count))))))))(new ConstructedTy(name(new List<object>())))))(_mAVariantTypeDef0_.Field0)))(_mAVariantTypeDef0_.Field1)))(_mAVariantTypeDef0_.Field2) : (_scrutinee0_ is ARecordTypeDef _mARecordTypeDef0_ ? ((Func<object, LetBindResult>)((fields) => ((Func<object, LetBindResult>)((type_params) => ((Func<object, LetBindResult>)((name) => ((Func<LetBindResult, LetBindResult>)((result_ty) => ((Func<LetBindResult, LetBindResult>)((ctor_ty) => new LetBindResult(st(), (_p0_) => (_p1_) => env_bind(env()(name.value(ctor_ty)), _p0_, _p1_))))((_p0_) => (_p1_) => (_p2_) => build_record_ctor_type(fields(result_ty(0L)(((long)fields.Count))), _p0_, _p1_, _p2_))))(new ConstructedTy(name(new List<object>())))))(_mARecordTypeDef0_.Field0)))(_mARecordTypeDef0_.Field1)))(_mARecordTypeDef0_.Field2) : throw new InvalidOperationException("Non-exhaustive match")))))(td)(register_variant_ctors);
    }

    public static Func<string, Func<List<IRParam>, Func<CodexType, Func<IRExpr, IRDef>>>> UnificationState()
    {
        return TypeEnv;
    }

    public static T736 List<T736>()
    {
        return throw new InvalidOperationException("->")(CodexType);
    }

    public static T316 Integer<T316>()
    {
        return new Integer();
    }

    public static T4096 LetBindResult<T4096>()
    {
        return register_variant_ctors()(st)(env)(ctors)(result_ty)(i)(len);
    }

    public static long i()
    {
        return len;
    }

    public static T4096 LetBindResult<T4096>()
    {
        return state;
    }

    public static Func<a, Func<object, T3674>> st<T3674>()
    {
        return env;
    }

    public static object env()
    {
        return throw new InvalidOperationException("else");
    }

    public static CodexType ctor()
    {
        return list_at()(ctors(i));
    }

    public static PatBindResult ctor_ty()
    {
        return (_p0_) => (_p1_) => (_p2_) => build_ctor_type(ctor().fields(result_ty(0L)(((long)ctor().fields.Count))), _p0_, _p1_, _p2_);
    }

    public static T4118 env2<T4118>()
    {
        return (_p0_) => (_p1_) => env_bind(env()(ctor().name.value(ctor_ty)), _p0_, _p1_);
    }

    public static T4123 register_variant_ctors<T4123>()
    {
        return env2()(ctors(result_ty((i() + 1L))(len)));
    }

    public static CodexType build_ctor_type(List<ATypeExpr> fields, CodexType result, long i, long len)
    {
        return ((i() == len) ? result : ((Func<CodexType, CodexType>)((rest) => new FunTy(resolve_type_expr(list_at()(fields(i))), rest)))((_p0_) => (_p1_) => (_p2_) => build_ctor_type(fields(result((i() + 1L))(len)), _p0_, _p1_, _p2_)));
    }

    public static CodexType build_record_ctor_type(List<ARecordFieldDef> fields, CodexType result, long i, long len)
    {
        return ((i() == len) ? result : ((Func<CodexType, CodexType>)((f) => ((Func<CodexType, CodexType>)((rest) => new FunTy(resolve_type_expr(f.type_expr), rest)))((_p0_) => (_p1_) => (_p2_) => build_record_ctor_type(fields(result((i() + 1L))(len)), _p0_, _p1_, _p2_))))(list_at()(fields(i))));
    }

    public static TypeEnv empty_type_env()
    {
        return new TypeEnv(new List<object>());
    }

    public static CodexType env_lookup(TypeEnv env, string name)
    {
        return (_p0_) => (_p1_) => (_p2_) => env_lookup_loop(env().bindings(name(0L)(((long)env().bindings.Count))), _p0_, _p1_, _p2_);
    }

    public static CodexType env_lookup_loop(List<TypeBinding> bindings, string name, long i, long len)
    {
        while (true)
        {
            if ((i() == len))
            {
                return new ErrorTy();
            }
            else
            {
                var b = list_at()(bindings(i));
                if ((b().name == name))
                {
                    return b().bound_type;
                }
                else
                {
                    var _tco_0 = bindings(name((i() + 1L))(len));
                    bindings = _tco_0;
                    continue;
                }
            }
        }
    }

    public static bool env_has(TypeEnv env, string name)
    {
        return (_p0_) => (_p1_) => (_p2_) => env_has_loop(env().bindings(name(0L)(((long)env().bindings.Count))), _p0_, _p1_, _p2_);
    }

    public static bool env_has_loop(List<TypeBinding> bindings, string name, long i, long len)
    {
        while (true)
        {
            if ((i() == len))
            {
                return false;
            }
            else
            {
                var b = list_at()(bindings(i));
                if ((b().name == name))
                {
                    return true;
                }
                else
                {
                    var _tco_0 = bindings(name((i() + 1L))(len));
                    bindings = _tco_0;
                    continue;
                }
            }
        }
    }

    public static TypeEnv env_bind(TypeEnv env, string name, CodexType ty)
    {
        return new TypeEnv(Enumerable.Concat(new List<object>() { new TypeBinding(name, ty) }, env().bindings).ToList());
    }

    public static TypeEnv builtin_type_env()
    {
        return ((Func<TypeEnv, TypeEnv>)((e) => ((Func<TypeEnv, TypeEnv>)((e2) => ((Func<TypeEnv, TypeEnv>)((e3) => ((Func<TypeEnv, TypeEnv>)((e4) => ((Func<TypeEnv, TypeEnv>)((e5) => ((Func<TypeEnv, TypeEnv>)((e6) => ((Func<TypeEnv, TypeEnv>)((e7) => ((Func<TypeEnv, TypeEnv>)((e8) => ((Func<TypeEnv, TypeEnv>)((e9) => ((Func<TypeEnv, TypeEnv>)((e10) => ((Func<TypeEnv, TypeEnv>)((e11) => ((Func<TypeEnv, TypeEnv>)((e12) => ((Func<TypeEnv, TypeEnv>)((e13) => ((Func<TypeEnv, TypeEnv>)((e14) => ((Func<TypeEnv, TypeEnv>)((e15) => ((Func<TypeEnv, TypeEnv>)((e16) => ((Func<TypeEnv, TypeEnv>)((e17) => ((Func<TypeEnv, TypeEnv>)((e18) => ((Func<TypeEnv, TypeEnv>)((e19) => ((Func<TypeEnv, TypeEnv>)((e20) => ((Func<TypeEnv, TypeEnv>)((e21) => e21))((_p0_) => (_p1_) => env_bind(e20("read-line")(TextTy), _p0_, _p1_))))((_p0_) => (_p1_) => env_bind(e19("fold")(new ForAllTy(0L, new ForAllTy(1L, new FunTy(new FunTy(new TypeVar(1L), new FunTy(new TypeVar(0L), new TypeVar(1L))), new FunTy(new TypeVar(1L), new FunTy(new ListTy(new TypeVar(0L)), new TypeVar(1L))))))), _p0_, _p1_))))((_p0_) => (_p1_) => env_bind(e18("filter")(new ForAllTy(0L, new FunTy(new FunTy(new TypeVar(0L), new BooleanTy()), new FunTy(new ListTy(new TypeVar(0L)), new ListTy(new TypeVar(0L)))))), _p0_, _p1_))))((_p0_) => (_p1_) => env_bind(e17("map")(new ForAllTy(0L, new ForAllTy(1L, new FunTy(new FunTy(new TypeVar(0L), new TypeVar(1L)), new FunTy(new ListTy(new TypeVar(0L)), new ListTy(new TypeVar(1L))))))), _p0_, _p1_))))((_p0_) => (_p1_) => env_bind(e16("list-at")(new ForAllTy(0L, new FunTy(new ListTy(new TypeVar(0L)), new FunTy(new IntegerTy(), new TypeVar(0L))))), _p0_, _p1_))))((_p0_) => (_p1_) => env_bind(e15("list-length")(new ForAllTy(0L, new FunTy(new ListTy(new TypeVar(0L)), new IntegerTy()))), _p0_, _p1_))))((_p0_) => (_p1_) => env_bind(e14("print-line")(new FunTy(new TextTy(), new NothingTy())), _p0_, _p1_))))((_p0_) => (_p1_) => env_bind(e13("show")(new ForAllTy(0L, new FunTy(new TypeVar(0L), new TextTy()))), _p0_, _p1_))))((_p0_) => (_p1_) => env_bind(e12("text-to-integer")(new FunTy(new TextTy(), new IntegerTy())), _p0_, _p1_))))((_p0_) => (_p1_) => env_bind(e11("text-replace")(new FunTy(new TextTy(), new FunTy(new TextTy(), new FunTy(new TextTy(), new TextTy())))), _p0_, _p1_))))((_p0_) => (_p1_) => env_bind(e10("code-to-char")(new FunTy(new IntegerTy(), new TextTy())), _p0_, _p1_))))((_p0_) => (_p1_) => env_bind(e9("char-code")(new FunTy(new TextTy(), new IntegerTy())), _p0_, _p1_))))((_p0_) => (_p1_) => env_bind(e8("is-whitespace")(new FunTy(new TextTy(), new BooleanTy())), _p0_, _p1_))))((_p0_) => (_p1_) => env_bind(e7("is-digit")(new FunTy(new TextTy(), new BooleanTy())), _p0_, _p1_))))((_p0_) => (_p1_) => env_bind(e6("is-letter")(new FunTy(new TextTy(), new BooleanTy())), _p0_, _p1_))))((_p0_) => (_p1_) => env_bind(e5("substring")(new FunTy(new TextTy(), new FunTy(new IntegerTy(), new FunTy(new IntegerTy(), new TextTy())))), _p0_, _p1_))))((_p0_) => (_p1_) => env_bind(e4("char-at")(new FunTy(new TextTy(), new FunTy(new IntegerTy(), new TextTy()))), _p0_, _p1_))))((_p0_) => (_p1_) => env_bind(e3("integer-to-text")(new FunTy(new IntegerTy(), new TextTy())), _p0_, _p1_))))((_p0_) => (_p1_) => env_bind(e2("text-length")(new FunTy(new TextTy(), new IntegerTy())), _p0_, _p1_))))((_p0_) => (_p1_) => env_bind(e()("negate")(new FunTy(new IntegerTy(), new IntegerTy())), _p0_, _p1_))))(empty_type_env());
    }

    public static UnificationState empty_unification_state()
    {
        return new UnificationState(new List<object>(), 0L, new List<object>());
    }

    public static CodexType fresh_var(UnificationState st)
    {
        return new TypeVar(st().next_id);
    }

    public static UnificationState advance_id(UnificationState st)
    {
        return new UnificationState(st().substitutions, (st().next_id + 1L), st().errors);
    }

    public static FreshResult fresh_and_advance(UnificationState st)
    {
        return new FreshResult(new TypeVar(st().next_id), advance_id(st));
    }

    public static CodexType subst_lookup(long var_id, List<SubstEntry> entries)
    {
        return (_p0_) => (_p1_) => (_p2_) => subst_lookup_loop(var_id(entries(0L)(((long)entries.Count))), _p0_, _p1_, _p2_);
    }

    public static CodexType subst_lookup_loop(long var_id, List<SubstEntry> entries, long i, long len)
    {
        while (true)
        {
            if ((i() == len))
            {
                return new ErrorTy();
            }
            else
            {
                var entry = list_at()(entries(i));
                if ((entry.var_id == var_id))
                {
                    return entry.resolved_type;
                }
                else
                {
                    var _tco_0 = var_id(entries((i() + 1L))(len));
                    var_id = _tco_0;
                    continue;
                }
            }
        }
    }

    public static bool has_subst(long var_id, List<SubstEntry> entries)
    {
        return (_p0_) => (_p1_) => (_p2_) => has_subst_loop(var_id(entries(0L)(((long)entries.Count))), _p0_, _p1_, _p2_);
    }

    public static bool has_subst_loop(long var_id, List<SubstEntry> entries, long i, long len)
    {
        while (true)
        {
            if ((i() == len))
            {
                return false;
            }
            else
            {
                var entry = list_at()(entries(i));
                if ((entry.var_id == var_id))
                {
                    return true;
                }
                else
                {
                    var _tco_0 = var_id(entries((i() + 1L))(len));
                    var_id = _tco_0;
                    continue;
                }
            }
        }
    }

    public static CodexType resolve(UnificationState st, CodexType ty)
    {
        return (ty is TypeVar _mTypeVar0_ ? ((Func<object, CodexType>)((id) => ((_p0_) => has_subst(id(st().substitutions), _p0_) ? (_p0_) => resolve(st()((_p0_) => subst_lookup(id(st().substitutions), _p0_)), _p0_) : ty)))(_mTypeVar0_.Field0) : ty)(add_subst);
    }

    public static Func<string, Func<List<IRParam>, Func<CodexType, Func<IRExpr, IRDef>>>> UnificationState()
    {
        return Integer;
    }

    public static Func<string, Func<IRExpr, IRFieldVal>> CodexType()
    {
        return UnificationState;
    }

    public static UnificationState add_subst(object st, object var_id, object ty)
    {
        return new UnificationState(Enumerable.Concat(st().substitutions, new List<object>() { new SubstEntry(var_id, ty) }).ToList(), st().next_id, st().errors);
    }

    public static UnificationState add_unify_error(UnificationState st, string code, string msg)
    {
        return new UnificationState(st().substitutions, st().next_id, Enumerable.Concat(st().errors, new List<object>() { (_p0_) => make_error(code(msg), _p0_) }).ToList());
    }

    public static bool occurs_in(UnificationState st, long var_id, CodexType ty)
    {
        return ((Func<bool, bool>)((resolved) => ((Func<bool, bool>)((_scrutinee0_) => (_scrutinee0_ is TypeVar _mTypeVar0_ ? ((Func<object, bool>)((id) => (id == var_id)))(_mTypeVar0_.Field0) : (_scrutinee0_ is FunTy _mFunTy0_ ? ((Func<object, bool>)((ret) => ((Func<object, bool>)((param) => ((_p0_) => (_p1_) => occurs_in(st()(var_id(param)), _p0_, _p1_) || (_p0_) => (_p1_) => occurs_in(st()(var_id(ret)), _p0_, _p1_))))(_mFunTy0_.Field0)))(_mFunTy0_.Field1) : (_scrutinee0_ is ListTy _mListTy0_ ? ((Func<object, bool>)((elem) => (_p0_) => (_p1_) => occurs_in(st()(var_id(elem)), _p0_, _p1_)))(_mListTy0_.Field0) : false)))))(resolved)(unify)))((_p0_) => resolve(st()(ty), _p0_));
    }

    public static Func<string, Func<List<IRParam>, Func<CodexType, Func<IRExpr, IRDef>>>> UnificationState()
    {
        return CodexType;
    }

    public static Func<string, Func<IRExpr, IRFieldVal>> CodexType()
    {
        return UnifyResult;
    }

    public static UnifyResult unify(CodexType st, CodexType a, object b)
    {
        return ((Func<object, object>)((ra) => ((Func<object, object>)((rb) => (_p0_) => (_p1_) => unify_resolved(st()(ra(rb)), _p0_, _p1_)))((_p0_) => resolve(st()(b), _p0_))))((_p0_) => resolve(st()(a), _p0_));
    }

    public static UnifyResult unify_resolved(UnificationState st, CodexType a, CodexType b)
    {
        return (a is TypeVar _mTypeVar0_ ? ((Func<object, UnifyResult>)((id_a) => ((_p0_) => (_p1_) => occurs_in(st()(id_a(b)), _p0_, _p1_) ? new UnifyResult(false, (_p0_) => (_p1_) => add_unify_error(st()("CDX2010")("Infinite type"), _p0_, _p1_)) : new UnifyResult(true, (_p0_) => (_p1_) => add_subst(st()(id_a(b)), _p0_, _p1_)))))(_mTypeVar0_.Field0) : (_p0_) => (_p1_) => unify_rhs(st()(a(b)), _p0_, _p1_))(unify_rhs);
    }

    public static Func<string, Func<List<IRParam>, Func<CodexType, Func<IRExpr, IRDef>>>> UnificationState()
    {
        return CodexType;
    }

    public static Func<string, Func<IRExpr, IRFieldVal>> CodexType()
    {
        return UnifyResult;
    }

    public static T4462 unify_rhs<T4462>(object st, object a, object b)
    {
        return (b() is TypeVar _mTypeVar0_ ? ((Func<object, object>)((id_b) => ((_p0_) => (_p1_) => occurs_in(st()(id_b(a)), _p0_, _p1_) ? new UnifyResult(false, (_p0_) => (_p1_) => add_unify_error(st()("CDX2010")("Infinite type"), _p0_, _p1_)) : new UnifyResult(true, (_p0_) => (_p1_) => add_subst(st()(id_b(a)), _p0_, _p1_)))))(_mTypeVar0_.Field0) : (_p0_) => (_p1_) => unify_structural(st()(a(b)), _p0_, _p1_))(unify_structural);
    }

    public static Func<string, Func<List<IRParam>, Func<CodexType, Func<IRExpr, IRDef>>>> UnificationState()
    {
        return CodexType;
    }

    public static Func<string, Func<IRExpr, IRFieldVal>> CodexType()
    {
        return UnifyResult;
    }

    public static UnifyResult unify_structural(object st, object a, object b)
    {
        return (a is IntegerTy _mIntegerTy0_ ? ((Func<object, object>)((_scrutinee1_) => (_scrutinee1_ is IntegerTy _mIntegerTy1_ ? new UnifyResult(true, st()) : (_scrutinee1_ is ErrorTy _mErrorTy1_ ? new UnifyResult(true, st()) : (_p0_) => (_p1_) => unify_mismatch(st()(a(b)), _p0_, _p1_)))))(b()) : throw new InvalidOperationException("Non-exhaustive match"));
    }

    public static Func<string, Func<List<IRParam>, Func<CodexType, Func<IRExpr, IRDef>>>> UnificationState()
    {
        return new List(CodexType);
    }

    public static T736 List<T736>()
    {
        return throw new InvalidOperationException("->")(Integer);
    }

    public static T316 Integer<T316>()
    {
        return new UnifyResult();
    }

    public static UnifyResult unify_constructed_args(object st, object args_a, object args_b, object i, object len)
    {
        while (true)
        {
            if ((i() == len))
            {
                return new UnifyResult(true, st());
            }
            else
            {
                if ((i() >= ((long)args_b.Count)))
                {
                    return new UnifyResult(true, st());
                }
                else
                {
                    var r = (_p0_) => (_p1_) => unify(st()(list_at()(args_a(i)))(list_at()(args_b(i))), _p0_, _p1_);
                    if (r.success)
                    {
                        var _tco_0 = r.state(args_a(args_b((i() + 1L))(len)));
                        st = _tco_0;
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
        return ((Func<UnifyResult, UnifyResult>)((r1) => (r1.success ? (_p0_) => (_p1_) => unify(r1.state(ra(rb)), _p0_, _p1_) : r1)))((_p0_) => (_p1_) => unify(st()(pa(pb)), _p0_, _p1_));
    }

    public static UnifyResult unify_mismatch(UnificationState st, CodexType a, CodexType b)
    {
        return new UnifyResult(false, (_p0_) => (_p1_) => add_unify_error(st()("CDX2001")("Type mismatch"), _p0_, _p1_));
    }

    public static CodexType deep_resolve(UnificationState st, CodexType ty)
    {
        return ((Func<CodexType, CodexType>)((resolved) => ((Func<CodexType, CodexType>)((_scrutinee0_) => (_scrutinee0_ is FunTy _mFunTy0_ ? ((Func<object, CodexType>)((ret) => ((Func<object, CodexType>)((param) => new FunTy((_p0_) => deep_resolve(st()(param), _p0_), (_p0_) => deep_resolve(st()(ret), _p0_))))(_mFunTy0_.Field0)))(_mFunTy0_.Field1) : (_scrutinee0_ is ListTy _mListTy0_ ? ((Func<object, CodexType>)((elem) => new ListTy((_p0_) => deep_resolve(st()(elem), _p0_))))(_mListTy0_.Field0) : (_scrutinee0_ is ConstructedTy _mConstructedTy0_ ? ((Func<object, CodexType>)((args) => ((Func<object, CodexType>)((name) => new ConstructedTy(name((_p0_) => (_p1_) => (_p2_) => (_p3_) => deep_resolve_list(st()(args(0L)(((long)args.Count))(new List<object>())), _p0_, _p1_, _p2_, _p3_)))))(_mConstructedTy0_.Field0)))(_mConstructedTy0_.Field1) : (_scrutinee0_ is ForAllTy _mForAllTy0_ ? ((Func<object, CodexType>)((body) => ((Func<object, CodexType>)((id) => new ForAllTy(id((_p0_) => deep_resolve(st()(body), _p0_)))))(_mForAllTy0_.Field0)))(_mForAllTy0_.Field1) : (_scrutinee0_ is SumTy _mSumTy0_ ? ((Func<object, CodexType>)((ctors) => ((Func<object, CodexType>)((name) => resolved))(_mSumTy0_.Field0)))(_mSumTy0_.Field1) : (_scrutinee0_ is RecordTy _mRecordTy0_ ? ((Func<object, CodexType>)((fields) => ((Func<object, CodexType>)((name) => resolved))(_mRecordTy0_.Field0)))(_mRecordTy0_.Field1) : resolved))))))))(resolved)(deep_resolve_list)))((_p0_) => resolve(st()(ty), _p0_));
    }

    public static Func<string, Func<List<IRParam>, Func<CodexType, Func<IRExpr, IRDef>>>> UnificationState()
    {
        return new List(CodexType);
    }

    public static T316 Integer<T316>()
    {
        return new Integer();
    }

    public static T736 List<T736>()
    {
        return throw new InvalidOperationException("->")(List)(CodexType);
    }

    public static List<Func<CodexType, CodexType>> deep_resolve_list(object st, object args, object i, object len, object acc)
    {
        while (true)
        {
            if ((i() == len))
            {
                return acc();
            }
            else
            {
                var _tco_0 = st()(args((i() + 1L))(len(Enumerable.Concat(acc(), new List<object>() { (_p0_) => deep_resolve(st()(list_at()(args(i))), _p0_) }).ToList())));
                st = _tco_0;
                continue;
            }
        }
    }

    public static string compile(string source, string module_name)
    {
        return ((Func<string, string>)((tokens) => ((Func<string, string>)((st) => ((Func<string, string>)((doc) => ((Func<string, string>)((ast) => ((Func<string, string>)((check_result) => ((Func<string, string>)((ir) => (_p0_) => emit_full_module(ir(ast.type_defs), _p0_)))((_p0_) => (_p1_) => lower_module(ast(check_result.types(check_result.state)), _p0_, _p1_))))(check_module(ast))))((_p0_) => desugar_document(doc(module_name), _p0_))))(parse_document(st))))(make_parse_state(tokens))))(tokenize(source));
    }

    public static CompileResult compile_checked(string source, string module_name)
    {
        return ((Func<CompileResult, CompileResult>)((tokens) => ((Func<CompileResult, CompileResult>)((st) => ((Func<CompileResult, CompileResult>)((doc) => ((Func<CompileResult, CompileResult>)((ast) => ((Func<CompileResult, CompileResult>)((resolve_result) => ((((long)resolve_result.errors.Count) > 0L) ? new CompileError(resolve_result.errors) : ((Func<CompileResult, CompileResult>)((check_result) => ((Func<CompileResult, CompileResult>)((ir) => new CompileOk((_p0_) => emit_full_module(ir(ast.type_defs), _p0_), check_result)))((_p0_) => (_p1_) => lower_module(ast(check_result.types(check_result.state)), _p0_, _p1_))))(check_module(ast)))))(resolve_module(ast))))((_p0_) => desugar_document(doc(module_name), _p0_))))(parse_document(st))))(make_parse_state(tokens))))(tokenize(source));
    }

    public static string test_source()
    {
        return "square : Integer -> Integer\\nsquare (x) = x * x\\nmain = square 5";
    }

    public static [ Console()
    {
        return new Nothing();
    }

    public static T4697 main<T4697>()
    {
        return ((Func<object>)(() => {
        Console.WriteLine((_p0_) => compile(test_source()("test"), _p0_));
        return null;
    }))();
    }

}
