using System;
using System.Collections.Generic;
using System.Linq;



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
public sealed record EffectfulTy(List<Name> Field0, CodexType Field1) : CodexType;

public sealed record TypeBinding(string name, CodexType bound_type);

public sealed record PatBindResult(UnificationState state, TypeEnv env);

public sealed record AParam(Name name);

public abstract record IRPat;

public sealed record IrVarPat(string Field0, CodexType Field1) : IRPat;
public sealed record IrLitPat(string Field0, CodexType Field1) : IRPat;
public sealed record IrCtorPat(string Field0, List<IRPat> Field1, CodexType Field2) : IRPat;
public sealed record IrWildPat : IRPat;

public abstract record Pat;

public sealed record VarPat(Token Field0) : Pat;
public sealed record LitPat(Token Field0) : Pat;
public sealed record CtorPat(Token Field0, List<Pat> Field1) : Pat;
public sealed record WildPat(Token Field0) : Pat;

public sealed record SourceSpan(SourcePosition start, SourcePosition end, string file);

public sealed record CheckResult(CodexType inferred_type, UnificationState state);

public sealed record AHandleClause(Name op_name, Name resume_name, AExpr body);

public abstract record TypeExpr;

public sealed record NamedType(Token Field0) : TypeExpr;
public sealed record FunType(TypeExpr Field0, TypeExpr Field1) : TypeExpr;
public sealed record AppType(TypeExpr Field0, List<TypeExpr> Field1) : TypeExpr;
public sealed record ParenType(TypeExpr Field0) : TypeExpr;
public sealed record ListType(TypeExpr Field0) : TypeExpr;
public sealed record LinearTypeExpr(TypeExpr Field0) : TypeExpr;
public sealed record EffectTypeExpr(List<Token> Field0, TypeExpr Field1) : TypeExpr;

public abstract record ATypeDef;

public sealed record ARecordTypeDef(Name Field0, List<Name> Field1, List<ARecordFieldDef> Field2) : ATypeDef;
public sealed record AVariantTypeDef(Name Field0, List<Name> Field1, List<AVariantCtorDef> Field2) : ATypeDef;

public sealed record LetBindResult(UnificationState state, TypeEnv env);

public sealed record WalkListResult(List<CodexType> walked_list, List<ParamEntry> entries, UnificationState state);

public sealed record LambdaBindResult(UnificationState state, TypeEnv env, List<CodexType> param_types);

public sealed record LetBind(Token name, Expr value);

public abstract record DiagnosticSeverity;

public sealed record Error : DiagnosticSeverity;
public sealed record Warning : DiagnosticSeverity;
public sealed record Info : DiagnosticSeverity;

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

public abstract record IRDoStmt;

public sealed record IrDoBind(string Field0, CodexType Field1, IRExpr Field2) : IRDoStmt;
public sealed record IrDoExec(IRExpr Field0) : IRDoStmt;

public sealed record WalkResult(CodexType walked, List<ParamEntry> entries, UnificationState state);

public sealed record TypeEnv(List<TypeBinding> bindings);

public sealed record HandleParamsResult(List<Token> toks, ParseState state);

public abstract record LexResult;

public sealed record LexToken(Token Field0, LexState Field1) : LexResult;
public sealed record LexEnd : LexResult;

public sealed record Scope(List<string> names);

public sealed record ALetBind(Name name, AExpr value);

public sealed record ADef(Name name, List<AParam> @params, List<ATypeExpr> declared_type, AExpr body);

public sealed record SourcePosition(long line, long column, long offset);

public sealed record EffectOpDef(Token name, TypeExpr type_expr);

public sealed record EffectOpsResult(List<EffectOpDef> ops, ParseState state);

public sealed record ARecordFieldDef(Name name, ATypeExpr type_expr);

public sealed record AFieldExpr(Name name, AExpr value);

public sealed record ImportParseResult(List<ImportDecl> imports, ParseState state);

public sealed record ParseState(List<Token> tokens, long pos);

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

public sealed record Token(TokenKind kind, string text, long offset, long line, long column);

public sealed record IRParam(string name, CodexType type_val);

public abstract record TypeBody;

public sealed record RecordBody(List<RecordFieldDef> Field0) : TypeBody;
public sealed record VariantBody(List<VariantCtorDef> Field0) : TypeBody;

public sealed record ApplyChain(IRExpr root, List<IRExpr> args);

public sealed record IRFieldVal(string name, IRExpr value);

public abstract record ParseDefResult;

public sealed record DefOk(Def Field0, ParseState Field1) : ParseDefResult;
public sealed record DefNone(ParseState Field0) : ParseDefResult;

public sealed record SumCtor(Name name, List<CodexType> fields);

public sealed record TypeAnn(Token name, TypeExpr type_expr);

public sealed record ArityEntry(string name, long arity);

public sealed record AModule(Name name, List<ADef> defs, List<ATypeDef> type_defs, List<AEffectDef> effect_defs, List<AImportDecl> imports);

public sealed record CollectResult(List<string> names, List<Diagnostic> errors);

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
public sealed record IrHandle(string Field0, IRExpr Field1, List<IRHandleClause> Field2, CodexType Field3) : IRExpr;
public sealed record IrRecord(string Field0, List<IRFieldVal> Field1, CodexType Field2) : IRExpr;
public sealed record IrFieldAccess(IRExpr Field0, string Field1, CodexType Field2) : IRExpr;
public sealed record IrError(string Field0, CodexType Field1) : IRExpr;

public sealed record IRHandleClause(string op_name, string resume_name, IRExpr body);

public sealed record TypeDef(Token name, List<Token> type_params, TypeBody body);

public abstract record ParsePatResult;

public sealed record PatOk(Pat Field0, ParseState Field1) : ParsePatResult;

public abstract record CompileResult;

public sealed record CompileOk(string Field0, ModuleResult Field1) : CompileResult;
public sealed record CompileError(List<Diagnostic> Field0) : CompileResult;

public sealed record HandleParseResult(List<HandleClause> clauses, ParseState state);

public sealed record ModuleResult(List<TypeBinding> types, UnificationState state);

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
public sealed record EffectKeyword : TokenKind;
public sealed record WithKeyword : TokenKind;
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

public sealed record IRModule(Name name, List<IRDef> defs);

public sealed record AImportDecl(Name module_name);

public sealed record IRDef(string name, List<IRParam> @params, CodexType type_val, IRExpr body);

public sealed record IRBranch(IRPat pattern, IRExpr body);

public sealed record Diagnostic(string code, string message, DiagnosticSeverity severity);

public abstract record ATypeExpr;

public sealed record ANamedType(Name Field0) : ATypeExpr;
public sealed record AFunType(ATypeExpr Field0, ATypeExpr Field1) : ATypeExpr;
public sealed record AAppType(ATypeExpr Field0, List<ATypeExpr> Field1) : ATypeExpr;
public sealed record AEffectType(List<Name> Field0, ATypeExpr Field1) : ATypeExpr;

public abstract record ADoStmt;

public sealed record ADoBindStmt(Name Field0, AExpr Field1) : ADoStmt;
public sealed record ADoExprStmt(AExpr Field0) : ADoStmt;

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
public sealed record AHandleExpr(Name Field0, AExpr Field1, List<AHandleClause> Field2) : AExpr;
public sealed record AErrorExpr(string Field0) : AExpr;

public sealed record ParamEntry(string param_name, long var_id);

public sealed record AVariantCtorDef(Name name, List<ATypeExpr> fields);

public sealed record MatchArm(Pat pattern, Expr body);

public sealed record RecordFieldDef(Token name, TypeExpr type_expr);

public sealed record AEffectDef(Name name, List<AEffectOpDef> ops);

public sealed record ResolveResult(List<Diagnostic> errors, List<string> top_level_names, List<string> type_names, List<string> ctor_names);

public sealed record FreshResult(CodexType var_type, UnificationState state);

public sealed record AEffectOpDef(Name name, ATypeExpr type_expr);

public sealed record ImportDecl(Token module_name);

public abstract record DoStmt;

public sealed record DoBindStmt(Token Field0, Expr Field1) : DoStmt;
public sealed record DoExprStmt(Expr Field0) : DoStmt;

public abstract record ParseExprResult;

public sealed record ExprOk(Expr Field0, ParseState Field1) : ParseExprResult;

public sealed record CtorCollectResult(List<string> type_names, List<string> ctor_names);

public sealed record DefParamResult(UnificationState state, TypeEnv env, CodexType remaining_type);

public sealed record UnifyResult(bool success, UnificationState state);

public sealed record EffectDef(Token name, List<EffectOpDef> ops);

public sealed record LowerCtx(List<TypeBinding> types, UnificationState ust);

public sealed record SubstEntry(long var_id, CodexType resolved_type);

public sealed record ParamResult(CodexType parameterized, List<ParamEntry> entries, UnificationState state);

public sealed record Name(string value);

public sealed record Def(Token name, List<Token> @params, List<TypeAnn> ann, Expr body);

public abstract record ParseTypeDefResult;

public sealed record TypeDefOk(TypeDef Field0, ParseState Field1) : ParseTypeDefResult;
public sealed record TypeDefNone(ParseState Field0) : ParseTypeDefResult;

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
public sealed record HandleExpr(Token Field0, Expr Field1, List<HandleClause> Field2) : Expr;
public sealed record ErrExpr(Token Field0) : Expr;

public sealed record UnificationState(List<SubstEntry> substitutions, long next_id, List<Diagnostic> errors);

public sealed record LexState(string source, long offset, long line, long column);

public sealed record HandleClause(Token op_name, Token resume_name, Expr body);

public abstract record APat;

public sealed record AVarPat(Name Field0) : APat;
public sealed record ALitPat(string Field0, LiteralKind Field1) : APat;
public sealed record ACtorPat(Name Field0, List<APat> Field1) : APat;
public sealed record AWildPat : APat;

public sealed record VariantCtorDef(Token name, List<TypeExpr> fields);

public sealed record DefSetup(CodexType expected_type, CodexType remaining_type, UnificationState state, TypeEnv env);

public sealed record RecordField(Name name, CodexType type_val);

public sealed record AMatchArm(APat pattern, AExpr body);

public abstract record LiteralKind;

public sealed record IntLit : LiteralKind;
public sealed record NumLit : LiteralKind;
public sealed record TextLit : LiteralKind;
public sealed record BoolLit : LiteralKind;

public sealed record Document(List<Def> defs, List<TypeDef> type_defs, List<EffectDef> effect_defs, List<ImportDecl> imports);

public abstract record ParseTypeResult;

public sealed record TypeOk(TypeExpr Field0, ParseState Field1) : ParseTypeResult;

public sealed record RecordFieldExpr(Token name, Expr value);

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
            else if (_tco_s is HandleExpr _tco_m13)
            {
                var eff_tok = _tco_m13.Field0;
                var body = _tco_m13.Field1;
                var clauses = _tco_m13.Field2;
                return new AHandleExpr(make_name(eff_tok.text), desugar_expr(body), map_list(new Func<HandleClause, AHandleClause>(desugar_handle_clause), clauses));
            }
            else if (_tco_s is ErrExpr _tco_m14)
            {
                var tok = _tco_m14.Field0;
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

    public static AHandleClause desugar_handle_clause(HandleClause c)
    {
        return new AHandleClause(make_name(c.op_name.text), make_name(c.resume_name.text), desugar_expr(c.body));
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
        return new AModule(make_name(module_name), map_list(new Func<Def, ADef>(desugar_def), doc.defs), map_list(new Func<TypeDef, ATypeDef>(desugar_type_def), doc.type_defs), map_list(new Func<EffectDef, AEffectDef>(desugar_effect_def), doc.effect_defs), map_list(new Func<ImportDecl, AImportDecl>(desugar_import), doc.imports));
    }

    public static AImportDecl desugar_import(ImportDecl imp)
    {
        return new AImportDecl(make_name(imp.module_name.text));
    }

    public static AEffectDef desugar_effect_def(EffectDef ed)
    {
        return new AEffectDef(make_name(ed.name.text), map_list(new Func<EffectOpDef, AEffectOpDef>(desugar_effect_op), ed.ops));
    }

    public static AEffectOpDef desugar_effect_op(EffectOpDef op)
    {
        return new AEffectOpDef(make_name(op.name.text), desugar_type_expr(op.type_expr));
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

    public static string emit_type_defs(List<ATypeDef> tds, long i)
    {
        return ((i == ((long)tds.Count)) ? "" : string.Concat(emit_type_def(tds[(int)i]), string.Concat("\n", emit_type_defs(tds, (i + 1L)))));
    }

    public static string emit_type_def(ATypeDef td)
    {
        return ((Func<ATypeDef, string>)((_scrutinee6_) => (_scrutinee6_ is ARecordTypeDef _mARecordTypeDef6_ ? ((Func<List<ARecordFieldDef>, string>)((fields) => ((Func<List<Name>, string>)((tparams) => ((Func<Name, string>)((name) => ((Func<string, string>)((gen) => string.Concat("public sealed record ", string.Concat(sanitize(name.value), string.Concat(gen, string.Concat("(", string.Concat(emit_record_field_defs(fields, tparams, 0L), ");\n")))))))(emit_tparameter_suffix(tparams))))((Name)_mARecordTypeDef6_.Field0)))((List<Name>)_mARecordTypeDef6_.Field1)))((List<ARecordFieldDef>)_mARecordTypeDef6_.Field2) : (_scrutinee6_ is AVariantTypeDef _mAVariantTypeDef6_ ? ((Func<List<AVariantCtorDef>, string>)((ctors) => ((Func<List<Name>, string>)((tparams) => ((Func<Name, string>)((name) => ((Func<string, string>)((gen) => string.Concat("public abstract record ", string.Concat(sanitize(name.value), string.Concat(gen, string.Concat(";\n", string.Concat(emit_variant_ctors(ctors, name, tparams, 0L), "\n")))))))(emit_tparameter_suffix(tparams))))((Name)_mAVariantTypeDef6_.Field0)))((List<Name>)_mAVariantTypeDef6_.Field1)))((List<AVariantCtorDef>)_mAVariantTypeDef6_.Field2) : throw new InvalidOperationException("Non-exhaustive match")))))(td);
    }

    public static string emit_tparameter_suffix(List<Name> tparams)
    {
        return ((((long)tparams.Count) == 0L) ? "" : string.Concat("<", string.Concat(emit_tparameter_names(tparams, 0L), ">")));
    }

    public static string emit_tparameter_names(List<Name> tparams, long i)
    {
        return ((i == ((long)tparams.Count)) ? "" : ((i == (((long)tparams.Count) - 1L)) ? string.Concat("T", (i).ToString()) : string.Concat("T", string.Concat((i).ToString(), string.Concat(", ", emit_tparameter_names(tparams, (i + 1L)))))));
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
        return ((Func<string, string>)((gen) => ((((long)c.fields.Count) == 0L) ? string.Concat("public sealed record ", string.Concat(sanitize(c.name.value), string.Concat(gen, string.Concat(" : ", string.Concat(sanitize(base_name.value), string.Concat(gen, ";\n")))))) : string.Concat("public sealed record ", string.Concat(sanitize(c.name.value), string.Concat(gen, string.Concat("(", string.Concat(emit_ctor_fields(c.fields, tparams, 0L), string.Concat(") : ", string.Concat(sanitize(base_name.value), string.Concat(gen, ";\n")))))))))))(emit_tparameter_suffix(tparams));
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
        return ((Func<ATypeExpr, string>)((_scrutinee7_) => (_scrutinee7_ is ANamedType _mANamedType7_ ? ((Func<Name, string>)((name) => ((Func<long, string>)((idx) => ((idx >= 0L) ? string.Concat("T", (idx).ToString()) : when_type_name(name.value))))(find_tparam_index(tparams, name.value, 0L))))((Name)_mANamedType7_.Field0) : (_scrutinee7_ is AFunType _mAFunType7_ ? ((Func<ATypeExpr, string>)((r) => ((Func<ATypeExpr, string>)((p) => string.Concat("Func<", string.Concat(emit_type_expr_tp(p, tparams), string.Concat(", ", string.Concat(emit_type_expr_tp(r, tparams), ">"))))))((ATypeExpr)_mAFunType7_.Field0)))((ATypeExpr)_mAFunType7_.Field1) : (_scrutinee7_ is AAppType _mAAppType7_ ? ((Func<List<ATypeExpr>, string>)((args) => ((Func<ATypeExpr, string>)((@base) => string.Concat(emit_type_expr_tp(@base, tparams), string.Concat("<", string.Concat(emit_type_expr_list_tp(args, tparams, 0L), ">")))))((ATypeExpr)_mAAppType7_.Field0)))((List<ATypeExpr>)_mAAppType7_.Field1) : throw new InvalidOperationException("Non-exhaustive match"))))))(te);
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

    public static string extract_ctor_type_args(CodexType ty)
    {
        return (ty is ConstructedTy _mConstructedTy8_ ? ((Func<List<CodexType>, string>)((args) => ((Func<Name, string>)((name) => ((((long)args.Count) == 0L) ? "" : string.Concat("<", string.Concat(emit_cs_type_args(args, 0L), ">")))))((Name)_mConstructedTy8_.Field0)))((List<CodexType>)_mConstructedTy8_.Field1) : ((Func<CodexType, string>)((_) => ""))(ty));
    }

    public static bool is_self_call(IRExpr e, string func_name)
    {
        return ((Func<ApplyChain, bool>)((chain) => is_self_call_root(chain.root, func_name)))(collect_apply_chain(e, new List<IRExpr>()));
    }

    public static bool is_self_call_root(IRExpr e, string func_name)
    {
        return (e is IrName _mIrName9_ ? ((Func<CodexType, bool>)((ty) => ((Func<string, bool>)((n) => (n == func_name)))((string)_mIrName9_.Field0)))((CodexType)_mIrName9_.Field1) : ((Func<IRExpr, bool>)((_) => false))(e));
    }

    public static bool has_tail_call(IRExpr e, string func_name)
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
                return (has_tail_call(t, func_name) || has_tail_call(el, func_name));
            }
            else if (_tco_s is IrLet _tco_m1)
            {
                var name = _tco_m1.Field0;
                var ty = _tco_m1.Field1;
                var val = _tco_m1.Field2;
                var body = _tco_m1.Field3;
                var _tco_0 = body;
                var _tco_1 = func_name;
                e = _tco_0;
                func_name = _tco_1;
                continue;
            }
            else if (_tco_s is IrMatch _tco_m2)
            {
                var scrut = _tco_m2.Field0;
                var branches = _tco_m2.Field1;
                var ty = _tco_m2.Field2;
                return has_tail_call_branches(branches, func_name, 0L);
            }
            else if (_tco_s is IrApply _tco_m3)
            {
                var f = _tco_m3.Field0;
                var a = _tco_m3.Field1;
                var ty = _tco_m3.Field2;
                return is_self_call(e, func_name);
            }
            {
                var _ = _tco_s;
                return false;
            }
        }
    }

    public static bool has_tail_call_branches(List<IRBranch> branches, string func_name, long i)
    {
        while (true)
        {
            if ((i == ((long)branches.Count)))
            {
                return false;
            }
            else
            {
                var b = branches[(int)i];
                if (has_tail_call(b.body, func_name))
                {
                    return true;
                }
                else
                {
                    var _tco_0 = branches;
                    var _tco_1 = func_name;
                    var _tco_2 = (i + 1L);
                    branches = _tco_0;
                    func_name = _tco_1;
                    i = _tco_2;
                    continue;
                }
            }
        }
    }

    public static bool should_tco(IRDef d)
    {
        return ((((long)d.@params.Count) == 0L) ? false : has_tail_call(d.body, d.name));
    }

    public static string emit_tco_def(IRDef d, List<ArityEntry> arities)
    {
        return ((Func<CodexType, string>)((ret) => ((Func<string, string>)((gen) => string.Concat("    public static ", string.Concat(cs_type(ret), string.Concat(" ", string.Concat(sanitize(d.name), string.Concat(gen, string.Concat("(", string.Concat(emit_def_params(d.@params, 0L), string.Concat(")\n    {\n        while (true)\n        {\n", string.Concat(emit_tco_body(d.body, d.name, d.@params, arities), "        }\n    }\n")))))))))))(generic_suffix(d.type_val))))(get_return_type(d.type_val, ((long)d.@params.Count)));
    }

    public static string emit_tco_body(IRExpr e, string func_name, List<IRParam> @params, List<ArityEntry> arities)
    {
        return ((Func<IRExpr, string>)((_scrutinee10_) => (_scrutinee10_ is IrIf _mIrIf10_ ? ((Func<CodexType, string>)((ty) => ((Func<IRExpr, string>)((el) => ((Func<IRExpr, string>)((t) => ((Func<IRExpr, string>)((c) => emit_tco_if(c, t, el, func_name, @params, arities)))((IRExpr)_mIrIf10_.Field0)))((IRExpr)_mIrIf10_.Field1)))((IRExpr)_mIrIf10_.Field2)))((CodexType)_mIrIf10_.Field3) : (_scrutinee10_ is IrLet _mIrLet10_ ? ((Func<IRExpr, string>)((body) => ((Func<IRExpr, string>)((val) => ((Func<CodexType, string>)((ty) => ((Func<string, string>)((name) => emit_tco_let(name, ty, val, body, func_name, @params, arities)))((string)_mIrLet10_.Field0)))((CodexType)_mIrLet10_.Field1)))((IRExpr)_mIrLet10_.Field2)))((IRExpr)_mIrLet10_.Field3) : (_scrutinee10_ is IrMatch _mIrMatch10_ ? ((Func<CodexType, string>)((ty) => ((Func<List<IRBranch>, string>)((branches) => ((Func<IRExpr, string>)((scrut) => emit_tco_match(scrut, branches, func_name, @params, arities)))((IRExpr)_mIrMatch10_.Field0)))((List<IRBranch>)_mIrMatch10_.Field1)))((CodexType)_mIrMatch10_.Field2) : (_scrutinee10_ is IrApply _mIrApply10_ ? ((Func<CodexType, string>)((rty) => ((Func<IRExpr, string>)((a) => ((Func<IRExpr, string>)((f) => emit_tco_apply(e, func_name, @params, arities)))((IRExpr)_mIrApply10_.Field0)))((IRExpr)_mIrApply10_.Field1)))((CodexType)_mIrApply10_.Field2) : ((Func<IRExpr, string>)((_) => string.Concat("            return ", string.Concat(emit_expr(e, arities), ";\n"))))(_scrutinee10_)))))))(e);
    }

    public static string emit_tco_apply(IRExpr e, string func_name, List<IRParam> @params, List<ArityEntry> arities)
    {
        return (is_self_call(e, func_name) ? emit_tco_jump(e, @params, arities) : string.Concat("            return ", string.Concat(emit_expr(e, arities), ";\n")));
    }

    public static string emit_tco_if(IRExpr cond, IRExpr t, IRExpr el, string func_name, List<IRParam> @params, List<ArityEntry> arities)
    {
        return string.Concat("            if (", string.Concat(emit_expr(cond, arities), string.Concat(")\n            {\n", string.Concat(emit_tco_body(t, func_name, @params, arities), string.Concat("            }\n            else\n            {\n", string.Concat(emit_tco_body(el, func_name, @params, arities), "            }\n"))))));
    }

    public static string emit_tco_let(string name, CodexType ty, IRExpr val, IRExpr body, string func_name, List<IRParam> @params, List<ArityEntry> arities)
    {
        return string.Concat("            var ", string.Concat(sanitize(name), string.Concat(" = ", string.Concat(emit_expr(val, arities), string.Concat(";\n", emit_tco_body(body, func_name, @params, arities))))));
    }

    public static string emit_tco_match(IRExpr scrut, List<IRBranch> branches, string func_name, List<IRParam> @params, List<ArityEntry> arities)
    {
        return string.Concat("            var _tco_s = ", string.Concat(emit_expr(scrut, arities), string.Concat(";\n", emit_tco_match_branches(branches, func_name, @params, arities, 0L, true))));
    }

    public static string emit_tco_match_branches(List<IRBranch> branches, string func_name, List<IRParam> @params, List<ArityEntry> arities, long i, bool is_first)
    {
        return ((i == ((long)branches.Count)) ? "" : ((Func<IRBranch, string>)((b) => string.Concat(emit_tco_match_branch(b, func_name, @params, arities, i, is_first), emit_tco_match_branches(branches, func_name, @params, arities, (i + 1L), false))))(branches[(int)i]));
    }

    public static string emit_tco_match_branch(IRBranch b, string func_name, List<IRParam> @params, List<ArityEntry> arities, long idx, bool is_first)
    {
        return ((Func<IRPat, string>)((_scrutinee11_) => (_scrutinee11_ is IrWildPat _mIrWildPat11_ ? string.Concat("            {\n", string.Concat(emit_tco_body(b.body, func_name, @params, arities), "            }\n")) : (_scrutinee11_ is IrVarPat _mIrVarPat11_ ? ((Func<CodexType, string>)((ty) => ((Func<string, string>)((name) => string.Concat("            {\n                var ", string.Concat(sanitize(name), string.Concat(" = _tco_s;\n", string.Concat(emit_tco_body(b.body, func_name, @params, arities), "            }\n"))))))((string)_mIrVarPat11_.Field0)))((CodexType)_mIrVarPat11_.Field1) : (_scrutinee11_ is IrCtorPat _mIrCtorPat11_ ? ((Func<CodexType, string>)((ty) => ((Func<List<IRPat>, string>)((subs) => ((Func<string, string>)((name) => ((Func<string, string>)((keyword) => ((Func<string, string>)((match_var) => string.Concat("            ", string.Concat(keyword, string.Concat(" (_tco_s is ", string.Concat(sanitize(name), string.Concat(" ", string.Concat(match_var, string.Concat(")\n            {\n", string.Concat(emit_tco_ctor_bindings(subs, match_var, 0L), string.Concat(emit_tco_body(b.body, func_name, @params, arities), "            }\n")))))))))))(string.Concat("_tco_m", (idx).ToString()))))((is_first ? "if" : "else if"))))((string)_mIrCtorPat11_.Field0)))((List<IRPat>)_mIrCtorPat11_.Field1)))((CodexType)_mIrCtorPat11_.Field2) : (_scrutinee11_ is IrLitPat _mIrLitPat11_ ? ((Func<CodexType, string>)((ty) => ((Func<string, string>)((text) => ((Func<string, string>)((keyword) => string.Concat("            ", string.Concat(keyword, string.Concat(" (object.Equals(_tco_s, ", string.Concat(text, string.Concat("))\n            {\n", string.Concat(emit_tco_body(b.body, func_name, @params, arities), "            }\n"))))))))((is_first ? "if" : "else if"))))((string)_mIrLitPat11_.Field0)))((CodexType)_mIrLitPat11_.Field1) : throw new InvalidOperationException("Non-exhaustive match")))))))(b.pattern);
    }

    public static string emit_tco_ctor_bindings(List<IRPat> subs, string match_var, long i)
    {
        return ((i == ((long)subs.Count)) ? "" : ((Func<IRPat, string>)((sub) => string.Concat(emit_tco_ctor_binding(sub, match_var, i), emit_tco_ctor_bindings(subs, match_var, (i + 1L)))))(subs[(int)i]));
    }

    public static string emit_tco_ctor_binding(IRPat sub, string match_var, long i)
    {
        return (sub is IrVarPat _mIrVarPat12_ ? ((Func<CodexType, string>)((ty) => ((Func<string, string>)((name) => string.Concat("                var ", string.Concat(sanitize(name), string.Concat(" = ", string.Concat(match_var, string.Concat(".Field", string.Concat((i).ToString(), ";\n"))))))))((string)_mIrVarPat12_.Field0)))((CodexType)_mIrVarPat12_.Field1) : ((Func<IRPat, string>)((_) => ""))(sub));
    }

    public static string emit_tco_jump(IRExpr e, List<IRParam> @params, List<ArityEntry> arities)
    {
        return ((Func<ApplyChain, string>)((chain) => string.Concat(emit_tco_temps(chain.args, arities, 0L), string.Concat(emit_tco_assigns(@params, 0L), "            continue;\n"))))(collect_apply_chain(e, new List<IRExpr>()));
    }

    public static string emit_tco_temps(List<IRExpr> args, List<ArityEntry> arities, long i)
    {
        return ((i == ((long)args.Count)) ? "" : string.Concat("            var _tco_", string.Concat((i).ToString(), string.Concat(" = ", string.Concat(emit_expr(args[(int)i], arities), string.Concat(";\n", emit_tco_temps(args, arities, (i + 1L))))))));
    }

    public static string emit_tco_assigns(List<IRParam> @params, long i)
    {
        return ((i == ((long)@params.Count)) ? "" : ((Func<IRParam, string>)((p) => string.Concat("            ", string.Concat(sanitize(p.name), string.Concat(" = _tco_", string.Concat((i).ToString(), string.Concat(";\n", emit_tco_assigns(@params, (i + 1L)))))))))(@params[(int)i]));
    }

    public static string emit_def(IRDef d, List<ArityEntry> arities)
    {
        return (should_tco(d) ? emit_tco_def(d, arities) : ((Func<CodexType, string>)((ret) => ((Func<string, string>)((gen) => string.Concat("    public static ", string.Concat(cs_type(ret), string.Concat(" ", string.Concat(sanitize(d.name), string.Concat(gen, string.Concat("(", string.Concat(emit_def_params(d.@params, 0L), string.Concat(") => ", string.Concat(emit_expr(d.body, arities), ";\n")))))))))))(generic_suffix(d.type_val))))(get_return_type(d.type_val, ((long)d.@params.Count))));
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

    public static string emit_full_module(IRModule m, List<ATypeDef> type_defs)
    {
        return ((Func<List<ArityEntry>, string>)((arities) => string.Concat("using System;\nusing System.Collections.Generic;\nusing System.Linq;\n\n", string.Concat("Codex_", string.Concat(sanitize(m.name.value), string.Concat(".main();\n\n", string.Concat(emit_type_defs(type_defs, 0L), string.Concat(emit_class_header(m.name.value), string.Concat(emit_defs(m.defs, 0L, arities), "}\n")))))))))(build_arity_map(m.defs, 0L));
    }

    public static string emit_module(IRModule m)
    {
        return ((Func<List<ArityEntry>, string>)((arities) => string.Concat("using System;\nusing System.Collections.Generic;\nusing System.Linq;\n\n", string.Concat("Codex_", string.Concat(sanitize(m.name.value), string.Concat(".main();\n\n", string.Concat(emit_class_header(m.name.value), string.Concat(emit_defs(m.defs, 0L, arities), "}\n"))))))))(build_arity_map(m.defs, 0L));
    }

    public static string emit_class_header(string name)
    {
        return string.Concat("public static class Codex_", string.Concat(sanitize(name), "\n{\n"));
    }

    public static string emit_defs(List<IRDef> defs, long i, List<ArityEntry> arities)
    {
        return ((i == ((long)defs.Count)) ? "" : string.Concat(emit_def(defs[(int)i], arities), string.Concat("\n", emit_defs(defs, (i + 1L), arities))));
    }

    public static bool is_cs_keyword(string n)
    {
        return ((n == "class") ? true : ((n == "static") ? true : ((n == "void") ? true : ((n == "return") ? true : ((n == "if") ? true : ((n == "else") ? true : ((n == "for") ? true : ((n == "while") ? true : ((n == "do") ? true : ((n == "switch") ? true : ((n == "case") ? true : ((n == "break") ? true : ((n == "continue") ? true : ((n == "new") ? true : ((n == "this") ? true : ((n == "base") ? true : ((n == "null") ? true : ((n == "true") ? true : ((n == "false") ? true : ((n == "int") ? true : ((n == "long") ? true : ((n == "string") ? true : ((n == "bool") ? true : ((n == "double") ? true : ((n == "decimal") ? true : ((n == "object") ? true : ((n == "in") ? true : ((n == "is") ? true : ((n == "as") ? true : ((n == "typeof") ? true : ((n == "default") ? true : ((n == "throw") ? true : ((n == "try") ? true : ((n == "catch") ? true : ((n == "finally") ? true : ((n == "using") ? true : ((n == "namespace") ? true : ((n == "public") ? true : ((n == "private") ? true : ((n == "protected") ? true : ((n == "internal") ? true : ((n == "abstract") ? true : ((n == "sealed") ? true : ((n == "override") ? true : ((n == "virtual") ? true : ((n == "event") ? true : ((n == "delegate") ? true : ((n == "out") ? true : ((n == "ref") ? true : ((n == "params") ? true : false))))))))))))))))))))))))))))))))))))))))))))))))));
    }

    public static string sanitize(string name)
    {
        return ((Func<string, string>)((s) => (is_cs_keyword(s) ? string.Concat("@", s) : (is_cs_member_name(s) ? string.Concat(s, "_") : s))))(name.Replace("-", "_"));
    }

    public static bool is_cs_member_name(string n)
    {
        return ((n == "Equals") ? true : ((n == "GetHashCode") ? true : ((n == "ToString") ? true : ((n == "GetType") ? true : ((n == "MemberwiseClone") ? true : false)))));
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
                if ((((long)args.Count) == 0L))
                {
                    return sanitize(name.value);
                }
                else
                {
                    return string.Concat(sanitize(name.value), string.Concat("<", string.Concat(emit_cs_type_args(args, 0L), ">")));
                }
            }
            else if (_tco_s is EffectfulTy _tco_m14)
            {
                var effects = _tco_m14.Field0;
                var ret = _tco_m14.Field1;
                var _tco_0 = ret;
                ty = _tco_0;
                continue;
            }
        }
    }

    public static string emit_cs_type_args(List<CodexType> args, long i)
    {
        return ((i == ((long)args.Count)) ? "" : ((Func<string, string>)((t) => ((i == (((long)args.Count) - 1L)) ? t : string.Concat(t, string.Concat(", ", emit_cs_type_args(args, (i + 1L)))))))(cs_type(args[(int)i])));
    }

    public static List<ArityEntry> build_arity_map(List<IRDef> defs, long i)
    {
        return ((i == ((long)defs.Count)) ? new List<ArityEntry>() : ((Func<IRDef, List<ArityEntry>>)((d) => Enumerable.Concat(new List<ArityEntry>() { new ArityEntry(d.name, ((long)d.@params.Count)) }, build_arity_map(defs, (i + 1L))).ToList()))(defs[(int)i]));
    }

    public static long lookup_arity(List<ArityEntry> entries, string name)
    {
        return lookup_arity_loop(entries, name, 0L, ((long)entries.Count));
    }

    public static long lookup_arity_loop(List<ArityEntry> entries, string name, long i, long len)
    {
        while (true)
        {
            if ((i == len))
            {
                return (0L - 1L);
            }
            else
            {
                var e = entries[(int)i];
                if ((e.name == name))
                {
                    return e.arity;
                }
                else
                {
                    var _tco_0 = entries;
                    var _tco_1 = name;
                    var _tco_2 = (i + 1L);
                    var _tco_3 = len;
                    entries = _tco_0;
                    name = _tco_1;
                    i = _tco_2;
                    len = _tco_3;
                    continue;
                }
            }
        }
    }

    public static ApplyChain collect_apply_chain(IRExpr e, List<IRExpr> acc)
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
                return new ApplyChain(e, acc);
            }
        }
    }

    public static bool is_upper_letter(string c)
    {
        return ((Func<long, bool>)((code) => ((code >= 65L) && (code <= 90L))))(((long)c[0]));
    }

    public static string emit_apply_args(List<IRExpr> args, List<ArityEntry> arities, long i)
    {
        return ((i == ((long)args.Count)) ? "" : ((i == (((long)args.Count) - 1L)) ? emit_expr(args[(int)i], arities) : string.Concat(emit_expr(args[(int)i], arities), string.Concat(", ", emit_apply_args(args, arities, (i + 1L))))));
    }

    public static string emit_partial_params(long i, long count)
    {
        return ((i == count) ? "" : ((i == (count - 1L)) ? string.Concat("_p", string.Concat((i).ToString(), "_")) : string.Concat("_p", string.Concat((i).ToString(), string.Concat("_", string.Concat(", ", emit_partial_params((i + 1L), count)))))));
    }

    public static string emit_partial_wrappers(long i, long count)
    {
        return ((i == count) ? "" : string.Concat("(_p", string.Concat((i).ToString(), string.Concat("_) => ", emit_partial_wrappers((i + 1L), count)))));
    }

    public static bool is_builtin_name(string n)
    {
        return ((n == "show") ? true : ((n == "negate") ? true : ((n == "print-line") ? true : ((n == "text-length") ? true : ((n == "is-letter") ? true : ((n == "is-digit") ? true : ((n == "is-whitespace") ? true : ((n == "text-to-integer") ? true : ((n == "integer-to-text") ? true : ((n == "char-code") ? true : ((n == "char-code-at") ? true : ((n == "code-to-char") ? true : ((n == "list-length") ? true : ((n == "char-at") ? true : ((n == "substring") ? true : ((n == "list-at") ? true : ((n == "text-replace") ? true : ((n == "open-file") ? true : ((n == "read-all") ? true : ((n == "close-file") ? true : ((n == "read-line") ? true : ((n == "read-file") ? true : false))))))))))))))))))))));
    }

    public static string emit_builtin(string n, List<IRExpr> args, List<ArityEntry> arities)
    {
        return ((n == "show") ? string.Concat("Convert.ToString(", string.Concat(emit_expr(args[(int)0L], arities), ")")) : ((n == "negate") ? string.Concat("(-", string.Concat(emit_expr(args[(int)0L], arities), ")")) : ((n == "print-line") ? string.Concat("Console.WriteLine(", string.Concat(emit_expr(args[(int)0L], arities), ")")) : ((n == "text-length") ? string.Concat("((long)", string.Concat(emit_expr(args[(int)0L], arities), ".Length)")) : ((n == "is-letter") ? string.Concat("(", string.Concat(emit_expr(args[(int)0L], arities), string.Concat(".Length > 0 && char.IsLetter(", string.Concat(emit_expr(args[(int)0L], arities), "[0]))")))) : ((n == "is-digit") ? string.Concat("(", string.Concat(emit_expr(args[(int)0L], arities), string.Concat(".Length > 0 && char.IsDigit(", string.Concat(emit_expr(args[(int)0L], arities), "[0]))")))) : ((n == "is-whitespace") ? string.Concat("(", string.Concat(emit_expr(args[(int)0L], arities), string.Concat(".Length > 0 && char.IsWhiteSpace(", string.Concat(emit_expr(args[(int)0L], arities), "[0]))")))) : ((n == "text-to-integer") ? string.Concat("long.Parse(", string.Concat(emit_expr(args[(int)0L], arities), ")")) : ((n == "integer-to-text") ? string.Concat("(", string.Concat(emit_expr(args[(int)0L], arities), ").ToString()")) : ((n == "char-code") ? string.Concat("((long)", string.Concat(emit_expr(args[(int)0L], arities), "[0])")) : ((n == "char-code-at") ? string.Concat("((long)", string.Concat(emit_expr(args[(int)0L], arities), string.Concat("[(int)", string.Concat(emit_expr(args[(int)1L], arities), "])")))) : ((n == "code-to-char") ? string.Concat("((char)", string.Concat(emit_expr(args[(int)0L], arities), ").ToString()")) : ((n == "list-length") ? string.Concat("((long)", string.Concat(emit_expr(args[(int)0L], arities), ".Count)")) : ((n == "char-at") ? string.Concat(emit_expr(args[(int)0L], arities), string.Concat("[(int)", string.Concat(emit_expr(args[(int)1L], arities), "].ToString()"))) : ((n == "substring") ? string.Concat(emit_expr(args[(int)0L], arities), string.Concat(".Substring((int)", string.Concat(emit_expr(args[(int)1L], arities), string.Concat(", (int)", string.Concat(emit_expr(args[(int)2L], arities), ")"))))) : ((n == "list-at") ? string.Concat(emit_expr(args[(int)0L], arities), string.Concat("[(int)", string.Concat(emit_expr(args[(int)1L], arities), "]"))) : ((n == "text-replace") ? string.Concat(emit_expr(args[(int)0L], arities), string.Concat(".Replace(", string.Concat(emit_expr(args[(int)1L], arities), string.Concat(", ", string.Concat(emit_expr(args[(int)2L], arities), ")"))))) : ((n == "open-file") ? string.Concat("File.OpenRead(", string.Concat(emit_expr(args[(int)0L], arities), ")")) : ((n == "read-all") ? string.Concat("new System.IO.StreamReader(", string.Concat(emit_expr(args[(int)0L], arities), ").ReadToEnd()")) : ((n == "close-file") ? string.Concat(emit_expr(args[(int)0L], arities), ".Dispose()") : ((n == "read-line") ? "Console.ReadLine()" : ((n == "read-file") ? string.Concat("File.ReadAllText(", string.Concat(emit_expr(args[(int)0L], arities), ")")) : ""))))))))))))))))))))));
    }

    public static string emit_apply(IRExpr e, List<ArityEntry> arities)
    {
        return ((Func<ApplyChain, string>)((chain) => ((Func<IRExpr, string>)((root) => ((Func<List<IRExpr>, string>)((args) => (root is IrName _mIrName13_ ? ((Func<CodexType, string>)((ty) => ((Func<string, string>)((n) => (is_builtin_name(n) ? emit_builtin(n, args, arities) : (((((long)n.Length) > 0L) && is_upper_letter(n[(int)0L].ToString())) ? ((Func<CodexType, string>)((result_ty) => ((Func<string, string>)((ctor_type_args) => string.Concat("new ", string.Concat(sanitize(n), string.Concat(ctor_type_args, string.Concat("(", string.Concat(emit_apply_args(args, arities, 0L), ")")))))))(extract_ctor_type_args(result_ty))))(ir_expr_type(e)) : ((Func<long, string>)((ar) => (((ar > 1L) && (((long)args.Count) == ar)) ? string.Concat(sanitize(n), string.Concat("(", string.Concat(emit_apply_args(args, arities, 0L), ")"))) : (((ar > 1L) && (((long)args.Count) < ar)) ? ((Func<long, string>)((remaining) => string.Concat(emit_partial_wrappers(0L, remaining), string.Concat(sanitize(n), string.Concat("(", string.Concat(emit_apply_args(args, arities, 0L), string.Concat(", ", string.Concat(emit_partial_params(0L, remaining), ")"))))))))((ar - ((long)args.Count))) : emit_expr_curried(e, arities)))))(lookup_arity(arities, n))))))((string)_mIrName13_.Field0)))((CodexType)_mIrName13_.Field1) : ((Func<IRExpr, string>)((_) => emit_expr_curried(e, arities)))(root))))(chain.args)))(chain.root)))(collect_apply_chain(e, new List<IRExpr>()));
    }

    public static string emit_expr_curried(IRExpr e, List<ArityEntry> arities)
    {
        return (e is IrApply _mIrApply14_ ? ((Func<CodexType, string>)((ty) => ((Func<IRExpr, string>)((a) => ((Func<IRExpr, string>)((f) => string.Concat(emit_expr(f, arities), string.Concat("(", string.Concat(emit_expr(a, arities), ")")))))((IRExpr)_mIrApply14_.Field0)))((IRExpr)_mIrApply14_.Field1)))((CodexType)_mIrApply14_.Field2) : ((Func<IRExpr, string>)((_) => emit_expr(e, arities)))(e));
    }

    public static string emit_expr(IRExpr e, List<ArityEntry> arities)
    {
        return ((Func<IRExpr, string>)((_scrutinee15_) => (_scrutinee15_ is IrIntLit _mIrIntLit15_ ? ((Func<long, string>)((n) => (n).ToString()))((long)_mIrIntLit15_.Field0) : (_scrutinee15_ is IrNumLit _mIrNumLit15_ ? ((Func<long, string>)((n) => (n).ToString()))((long)_mIrNumLit15_.Field0) : (_scrutinee15_ is IrTextLit _mIrTextLit15_ ? ((Func<string, string>)((s) => string.Concat("\"", string.Concat(escape_text(s), "\""))))((string)_mIrTextLit15_.Field0) : (_scrutinee15_ is IrBoolLit _mIrBoolLit15_ ? ((Func<bool, string>)((b) => (b ? "true" : "false")))((bool)_mIrBoolLit15_.Field0) : (_scrutinee15_ is IrName _mIrName15_ ? ((Func<CodexType, string>)((ty) => ((Func<string, string>)((n) => ((n == "read-line") ? "Console.ReadLine()" : (((((long)n.Length) > 0L) && is_upper_letter(n[(int)0L].ToString())) ? string.Concat("new ", string.Concat(sanitize(n), "()")) : ((lookup_arity(arities, n) == 0L) ? string.Concat(sanitize(n), "()") : ((Func<long, string>)((ar) => ((ar >= 2L) ? string.Concat(emit_partial_wrappers(0L, ar), string.Concat(sanitize(n), string.Concat("(", string.Concat(emit_partial_params(0L, ar), ")")))) : sanitize(n))))(lookup_arity(arities, n)))))))((string)_mIrName15_.Field0)))((CodexType)_mIrName15_.Field1) : (_scrutinee15_ is IrBinary _mIrBinary15_ ? ((Func<CodexType, string>)((ty) => ((Func<IRExpr, string>)((r) => ((Func<IRExpr, string>)((l) => ((Func<IRBinaryOp, string>)((op) => emit_binary(op, l, r, ty, arities)))((IRBinaryOp)_mIrBinary15_.Field0)))((IRExpr)_mIrBinary15_.Field1)))((IRExpr)_mIrBinary15_.Field2)))((CodexType)_mIrBinary15_.Field3) : (_scrutinee15_ is IrNegate _mIrNegate15_ ? ((Func<IRExpr, string>)((operand) => string.Concat("(-", string.Concat(emit_expr(operand, arities), ")"))))((IRExpr)_mIrNegate15_.Field0) : (_scrutinee15_ is IrIf _mIrIf15_ ? ((Func<CodexType, string>)((ty) => ((Func<IRExpr, string>)((el) => ((Func<IRExpr, string>)((t) => ((Func<IRExpr, string>)((c) => string.Concat("(", string.Concat(emit_expr(c, arities), string.Concat(" ? ", string.Concat(emit_expr(t, arities), string.Concat(" : ", string.Concat(emit_expr(el, arities), ")"))))))))((IRExpr)_mIrIf15_.Field0)))((IRExpr)_mIrIf15_.Field1)))((IRExpr)_mIrIf15_.Field2)))((CodexType)_mIrIf15_.Field3) : (_scrutinee15_ is IrLet _mIrLet15_ ? ((Func<IRExpr, string>)((body) => ((Func<IRExpr, string>)((val) => ((Func<CodexType, string>)((ty) => ((Func<string, string>)((name) => emit_let(name, ty, val, body, arities)))((string)_mIrLet15_.Field0)))((CodexType)_mIrLet15_.Field1)))((IRExpr)_mIrLet15_.Field2)))((IRExpr)_mIrLet15_.Field3) : (_scrutinee15_ is IrApply _mIrApply15_ ? ((Func<CodexType, string>)((ty) => ((Func<IRExpr, string>)((a) => ((Func<IRExpr, string>)((f) => emit_apply(e, arities)))((IRExpr)_mIrApply15_.Field0)))((IRExpr)_mIrApply15_.Field1)))((CodexType)_mIrApply15_.Field2) : (_scrutinee15_ is IrLambda _mIrLambda15_ ? ((Func<CodexType, string>)((ty) => ((Func<IRExpr, string>)((body) => ((Func<List<IRParam>, string>)((@params) => emit_lambda(@params, body, arities)))((List<IRParam>)_mIrLambda15_.Field0)))((IRExpr)_mIrLambda15_.Field1)))((CodexType)_mIrLambda15_.Field2) : (_scrutinee15_ is IrList _mIrList15_ ? ((Func<CodexType, string>)((ty) => ((Func<List<IRExpr>, string>)((elems) => emit_list(elems, ty, arities)))((List<IRExpr>)_mIrList15_.Field0)))((CodexType)_mIrList15_.Field1) : (_scrutinee15_ is IrMatch _mIrMatch15_ ? ((Func<CodexType, string>)((ty) => ((Func<List<IRBranch>, string>)((branches) => ((Func<IRExpr, string>)((scrut) => emit_match(scrut, branches, ty, arities)))((IRExpr)_mIrMatch15_.Field0)))((List<IRBranch>)_mIrMatch15_.Field1)))((CodexType)_mIrMatch15_.Field2) : (_scrutinee15_ is IrDo _mIrDo15_ ? ((Func<CodexType, string>)((ty) => ((Func<List<IRDoStmt>, string>)((stmts) => emit_do(stmts, ty, arities)))((List<IRDoStmt>)_mIrDo15_.Field0)))((CodexType)_mIrDo15_.Field1) : (_scrutinee15_ is IrHandle _mIrHandle15_ ? ((Func<CodexType, string>)((ty) => ((Func<List<IRHandleClause>, string>)((clauses) => ((Func<IRExpr, string>)((body) => ((Func<string, string>)((eff) => emit_handle(eff, body, clauses, ty, arities)))((string)_mIrHandle15_.Field0)))((IRExpr)_mIrHandle15_.Field1)))((List<IRHandleClause>)_mIrHandle15_.Field2)))((CodexType)_mIrHandle15_.Field3) : (_scrutinee15_ is IrRecord _mIrRecord15_ ? ((Func<CodexType, string>)((ty) => ((Func<List<IRFieldVal>, string>)((fields) => ((Func<string, string>)((name) => emit_record(name, fields, arities)))((string)_mIrRecord15_.Field0)))((List<IRFieldVal>)_mIrRecord15_.Field1)))((CodexType)_mIrRecord15_.Field2) : (_scrutinee15_ is IrFieldAccess _mIrFieldAccess15_ ? ((Func<CodexType, string>)((ty) => ((Func<string, string>)((field) => ((Func<IRExpr, string>)((rec) => string.Concat(emit_expr(rec, arities), string.Concat(".", sanitize(field)))))((IRExpr)_mIrFieldAccess15_.Field0)))((string)_mIrFieldAccess15_.Field1)))((CodexType)_mIrFieldAccess15_.Field2) : (_scrutinee15_ is IrError _mIrError15_ ? ((Func<CodexType, string>)((ty) => ((Func<string, string>)((msg) => string.Concat("/* error: ", string.Concat(msg, " */ default"))))((string)_mIrError15_.Field0)))((CodexType)_mIrError15_.Field1) : throw new InvalidOperationException("Non-exhaustive match")))))))))))))))))))))(e);
    }

    public static string escape_text(string s)
    {
        return ((Func<string, string>)((s1) => ((Func<string, string>)((s2) => ((Func<string, string>)((s3) => s3.Replace("\"", "\\\"")))(s2.Replace(((char)13L).ToString(), "\\r"))))(s1.Replace(((char)10L).ToString(), "\\n"))))(s.Replace("\\", "\\\\"));
    }

    public static string emit_bin_op(IRBinaryOp op)
    {
        return ((Func<IRBinaryOp, string>)((_scrutinee16_) => (_scrutinee16_ is IrAddInt _mIrAddInt16_ ? "+" : (_scrutinee16_ is IrSubInt _mIrSubInt16_ ? "-" : (_scrutinee16_ is IrMulInt _mIrMulInt16_ ? "*" : (_scrutinee16_ is IrDivInt _mIrDivInt16_ ? "/" : (_scrutinee16_ is IrPowInt _mIrPowInt16_ ? "^" : (_scrutinee16_ is IrAddNum _mIrAddNum16_ ? "+" : (_scrutinee16_ is IrSubNum _mIrSubNum16_ ? "-" : (_scrutinee16_ is IrMulNum _mIrMulNum16_ ? "*" : (_scrutinee16_ is IrDivNum _mIrDivNum16_ ? "/" : (_scrutinee16_ is IrEq _mIrEq16_ ? "==" : (_scrutinee16_ is IrNotEq _mIrNotEq16_ ? "!=" : (_scrutinee16_ is IrLt _mIrLt16_ ? "<" : (_scrutinee16_ is IrGt _mIrGt16_ ? ">" : (_scrutinee16_ is IrLtEq _mIrLtEq16_ ? "<=" : (_scrutinee16_ is IrGtEq _mIrGtEq16_ ? ">=" : (_scrutinee16_ is IrAnd _mIrAnd16_ ? "&&" : (_scrutinee16_ is IrOr _mIrOr16_ ? "||" : (_scrutinee16_ is IrAppendText _mIrAppendText16_ ? "+" : (_scrutinee16_ is IrAppendList _mIrAppendList16_ ? "+" : (_scrutinee16_ is IrConsList _mIrConsList16_ ? "+" : throw new InvalidOperationException("Non-exhaustive match")))))))))))))))))))))))(op);
    }

    public static string emit_binary(IRBinaryOp op, IRExpr l, IRExpr r, CodexType ty, List<ArityEntry> arities)
    {
        return ((Func<IRBinaryOp, string>)((_scrutinee17_) => (_scrutinee17_ is IrAppendList _mIrAppendList17_ ? string.Concat("Enumerable.Concat(", string.Concat(emit_expr(l, arities), string.Concat(", ", string.Concat(emit_expr(r, arities), ").ToList()")))) : (_scrutinee17_ is IrConsList _mIrConsList17_ ? string.Concat("new List<", string.Concat(cs_type(ir_expr_type(l)), string.Concat("> { ", string.Concat(emit_expr(l, arities), string.Concat(" }.Concat(", string.Concat(emit_expr(r, arities), ").ToList()")))))) : ((Func<IRBinaryOp, string>)((_) => string.Concat("(", string.Concat(emit_expr(l, arities), string.Concat(" ", string.Concat(emit_bin_op(op), string.Concat(" ", string.Concat(emit_expr(r, arities), ")"))))))))(_scrutinee17_)))))(op);
    }

    public static string emit_let(string name, CodexType ty, IRExpr val, IRExpr body, List<ArityEntry> arities)
    {
        return string.Concat("((Func<", string.Concat(cs_type(ty), string.Concat(", ", string.Concat(cs_type(ir_expr_type(body)), string.Concat(">)((", string.Concat(sanitize(name), string.Concat(") => ", string.Concat(emit_expr(body, arities), string.Concat("))(", string.Concat(emit_expr(val, arities), ")"))))))))));
    }

    public static string emit_lambda(List<IRParam> @params, IRExpr body, List<ArityEntry> arities)
    {
        return ((((long)@params.Count) == 0L) ? string.Concat("(() => ", string.Concat(emit_expr(body, arities), ")")) : ((((long)@params.Count) == 1L) ? ((Func<IRParam, string>)((p) => string.Concat("((", string.Concat(cs_type(p.type_val), string.Concat(" ", string.Concat(sanitize(p.name), string.Concat(") => ", string.Concat(emit_expr(body, arities), ")"))))))))(@params[(int)0L]) : string.Concat("(() => ", string.Concat(emit_expr(body, arities), ")"))));
    }

    public static string emit_list(List<IRExpr> elems, CodexType ty, List<ArityEntry> arities)
    {
        return ((((long)elems.Count) == 0L) ? string.Concat("new List<", string.Concat(cs_type(ty), ">()")) : string.Concat("new List<", string.Concat(cs_type(ty), string.Concat("> { ", string.Concat(emit_list_elems(elems, 0L, arities), " }")))));
    }

    public static string emit_list_elems(List<IRExpr> elems, long i, List<ArityEntry> arities)
    {
        return ((i == ((long)elems.Count)) ? "" : ((i == (((long)elems.Count) - 1L)) ? emit_expr(elems[(int)i], arities) : string.Concat(emit_expr(elems[(int)i], arities), string.Concat(", ", emit_list_elems(elems, (i + 1L), arities)))));
    }

    public static string emit_match(IRExpr scrut, List<IRBranch> branches, CodexType ty, List<ArityEntry> arities)
    {
        return ((Func<string, string>)((arms) => ((Func<bool, string>)((needs_wild) => string.Concat(emit_expr(scrut, arities), string.Concat(" switch { ", string.Concat(arms, string.Concat((needs_wild ? "_ => throw new InvalidOperationException(\"Non-exhaustive match\"), " : ""), "}"))))))((has_any_catch_all(branches, 0L) ? false : true))))(emit_match_arms(branches, 0L, arities));
    }

    public static string emit_match_arms(List<IRBranch> branches, long i, List<ArityEntry> arities)
    {
        return ((i == ((long)branches.Count)) ? "" : ((Func<IRBranch, string>)((arm) => ((Func<string, string>)((this_arm) => (is_catch_all(arm.pattern) ? this_arm : string.Concat(this_arm, emit_match_arms(branches, (i + 1L), arities)))))(string.Concat(emit_pattern(arm.pattern), string.Concat(" => ", string.Concat(emit_expr(arm.body, arities), ", "))))))(branches[(int)i]));
    }

    public static bool is_catch_all(IRPat p)
    {
        return ((Func<IRPat, bool>)((_scrutinee18_) => (_scrutinee18_ is IrWildPat _mIrWildPat18_ ? true : (_scrutinee18_ is IrVarPat _mIrVarPat18_ ? ((Func<CodexType, bool>)((ty) => ((Func<string, bool>)((name) => true))((string)_mIrVarPat18_.Field0)))((CodexType)_mIrVarPat18_.Field1) : ((Func<IRPat, bool>)((_) => false))(_scrutinee18_)))))(p);
    }

    public static bool has_any_catch_all(List<IRBranch> branches, long i)
    {
        while (true)
        {
            if ((i == ((long)branches.Count)))
            {
                return false;
            }
            else
            {
                var b = branches[(int)i];
                if (is_catch_all(b.pattern))
                {
                    return true;
                }
                else
                {
                    var _tco_0 = branches;
                    var _tco_1 = (i + 1L);
                    branches = _tco_0;
                    i = _tco_1;
                    continue;
                }
            }
        }
    }

    public static string emit_pattern(IRPat p)
    {
        return ((Func<IRPat, string>)((_scrutinee19_) => (_scrutinee19_ is IrVarPat _mIrVarPat19_ ? ((Func<CodexType, string>)((ty) => ((Func<string, string>)((name) => string.Concat(cs_type(ty), string.Concat(" ", sanitize(name)))))((string)_mIrVarPat19_.Field0)))((CodexType)_mIrVarPat19_.Field1) : (_scrutinee19_ is IrLitPat _mIrLitPat19_ ? ((Func<CodexType, string>)((ty) => ((Func<string, string>)((text) => text))((string)_mIrLitPat19_.Field0)))((CodexType)_mIrLitPat19_.Field1) : (_scrutinee19_ is IrCtorPat _mIrCtorPat19_ ? ((Func<CodexType, string>)((ty) => ((Func<List<IRPat>, string>)((subs) => ((Func<string, string>)((name) => ((((long)subs.Count) == 0L) ? string.Concat(sanitize(name), " { }") : string.Concat(sanitize(name), string.Concat("(", string.Concat(emit_sub_patterns(subs, 0L), ")"))))))((string)_mIrCtorPat19_.Field0)))((List<IRPat>)_mIrCtorPat19_.Field1)))((CodexType)_mIrCtorPat19_.Field2) : (_scrutinee19_ is IrWildPat _mIrWildPat19_ ? "_" : throw new InvalidOperationException("Non-exhaustive match")))))))(p);
    }

    public static string emit_sub_patterns(List<IRPat> subs, long i)
    {
        return ((i == ((long)subs.Count)) ? "" : ((Func<IRPat, string>)((sub) => string.Concat(emit_sub_pattern(sub), string.Concat(((i < (((long)subs.Count) - 1L)) ? ", " : ""), emit_sub_patterns(subs, (i + 1L))))))(subs[(int)i]));
    }

    public static string emit_sub_pattern(IRPat p)
    {
        return ((Func<IRPat, string>)((_scrutinee20_) => (_scrutinee20_ is IrVarPat _mIrVarPat20_ ? ((Func<CodexType, string>)((ty) => ((Func<string, string>)((name) => string.Concat("var ", sanitize(name))))((string)_mIrVarPat20_.Field0)))((CodexType)_mIrVarPat20_.Field1) : (_scrutinee20_ is IrCtorPat _mIrCtorPat20_ ? ((Func<CodexType, string>)((ty) => ((Func<List<IRPat>, string>)((subs) => ((Func<string, string>)((name) => emit_pattern(p)))((string)_mIrCtorPat20_.Field0)))((List<IRPat>)_mIrCtorPat20_.Field1)))((CodexType)_mIrCtorPat20_.Field2) : (_scrutinee20_ is IrWildPat _mIrWildPat20_ ? "_" : (_scrutinee20_ is IrLitPat _mIrLitPat20_ ? ((Func<CodexType, string>)((ty) => ((Func<string, string>)((text) => text))((string)_mIrLitPat20_.Field0)))((CodexType)_mIrLitPat20_.Field1) : throw new InvalidOperationException("Non-exhaustive match")))))))(p);
    }

    public static string emit_do(List<IRDoStmt> stmts, CodexType ty, List<ArityEntry> arities)
    {
        return ((Func<string, string>)((ret_type) => ((Func<long, string>)((len) => ((Func<CodexType, string>)((_scrutinee21_) => (_scrutinee21_ is VoidTy _mVoidTy21_ ? string.Concat("((Func<object>)(() => { ", string.Concat(emit_do_stmts(stmts, 0L, len, false, arities), " return null; }))()")) : (_scrutinee21_ is NothingTy _mNothingTy21_ ? string.Concat("((Func<object>)(() => { ", string.Concat(emit_do_stmts(stmts, 0L, len, false, arities), " return null; }))()")) : (_scrutinee21_ is ErrorTy _mErrorTy21_ ? string.Concat("((Func<object>)(() => { ", string.Concat(emit_do_stmts(stmts, 0L, len, false, arities), " return null; }))()")) : ((Func<CodexType, string>)((_) => ((len == 0L) ? string.Concat("((Func<", string.Concat(ret_type, ">)(() => { return null; }))()")) : string.Concat("((Func<", string.Concat(ret_type, string.Concat(">)(() => { ", string.Concat(emit_do_stmts(stmts, 0L, len, true, arities), " }))()")))))))(_scrutinee21_))))))(ty)))(((long)stmts.Count))))(cs_type(ty));
    }

    public static string emit_do_stmts(List<IRDoStmt> stmts, long i, long len, bool needs_return, List<ArityEntry> arities)
    {
        return ((i == len) ? "" : ((Func<IRDoStmt, string>)((s) => ((Func<bool, string>)((is_last) => ((Func<bool, string>)((use_return) => string.Concat(emit_do_stmt(s, use_return, arities), string.Concat(" ", emit_do_stmts(stmts, (i + 1L), len, needs_return, arities)))))((is_last ? needs_return : false))))((i == (len - 1L)))))(stmts[(int)i]));
    }

    public static string emit_do_stmt(IRDoStmt s, bool use_return, List<ArityEntry> arities)
    {
        return ((Func<IRDoStmt, string>)((_scrutinee22_) => (_scrutinee22_ is IrDoBind _mIrDoBind22_ ? ((Func<IRExpr, string>)((val) => ((Func<CodexType, string>)((ty) => ((Func<string, string>)((name) => string.Concat("var ", string.Concat(sanitize(name), string.Concat(" = ", string.Concat(emit_expr(val, arities), ";"))))))((string)_mIrDoBind22_.Field0)))((CodexType)_mIrDoBind22_.Field1)))((IRExpr)_mIrDoBind22_.Field2) : (_scrutinee22_ is IrDoExec _mIrDoExec22_ ? ((Func<IRExpr, string>)((e) => (use_return ? string.Concat("return ", string.Concat(emit_expr(e, arities), ";")) : string.Concat(emit_expr(e, arities), ";"))))((IRExpr)_mIrDoExec22_.Field0) : throw new InvalidOperationException("Non-exhaustive match")))))(s);
    }

    public static string emit_record(string name, List<IRFieldVal> fields, List<ArityEntry> arities)
    {
        return string.Concat("new ", string.Concat(sanitize(name), string.Concat("(", string.Concat(emit_record_fields(fields, 0L, arities), ")"))));
    }

    public static string emit_record_fields(List<IRFieldVal> fields, long i, List<ArityEntry> arities)
    {
        return ((i == ((long)fields.Count)) ? "" : ((Func<IRFieldVal, string>)((f) => string.Concat(sanitize(f.name), string.Concat(": ", string.Concat(emit_expr(f.value, arities), string.Concat(((i < (((long)fields.Count) - 1L)) ? ", " : ""), emit_record_fields(fields, (i + 1L), arities)))))))(fields[(int)i]));
    }

    public static string emit_handle(string eff, IRExpr body, List<IRHandleClause> clauses, CodexType ty, List<ArityEntry> arities)
    {
        return ((Func<string, string>)((ret_type) => string.Concat("((Func<", string.Concat(ret_type, string.Concat(">)(() => { ", string.Concat(emit_handle_clauses(clauses, ret_type, arities), string.Concat("return ", string.Concat(emit_expr(body, arities), "; }))()"))))))))(cs_type(ty));
    }

    public static string emit_handle_clauses(List<IRHandleClause> clauses, string ret_type, List<ArityEntry> arities)
    {
        return emit_handle_clauses_loop(clauses, 0L, ret_type, arities);
    }

    public static string emit_handle_clauses_loop(List<IRHandleClause> clauses, long i, string ret_type, List<ArityEntry> arities)
    {
        return ((i == ((long)clauses.Count)) ? "" : ((Func<IRHandleClause, string>)((c) => string.Concat("Func<Func<", string.Concat(ret_type, string.Concat(", ", string.Concat(ret_type, string.Concat(">, ", string.Concat(ret_type, string.Concat("> _handle_", string.Concat(sanitize(c.op_name), string.Concat("_ = (", string.Concat(sanitize(c.resume_name), string.Concat(") => { return ", string.Concat(emit_expr(c.body, arities), string.Concat("; }; ", emit_handle_clauses_loop(clauses, (i + 1L), ret_type, arities))))))))))))))))(clauses[(int)i]));
    }

    public static CodexType ir_expr_type(IRExpr e)
    {
        while (true)
        {
            var _tco_s = e;
            if (_tco_s is IrIntLit _tco_m0)
            {
                var v = _tco_m0.Field0;
                return new IntegerTy();
            }
            else if (_tco_s is IrNumLit _tco_m1)
            {
                var v = _tco_m1.Field0;
                return new IntegerTy();
            }
            else if (_tco_s is IrTextLit _tco_m2)
            {
                var v = _tco_m2.Field0;
                return new TextTy();
            }
            else if (_tco_s is IrBoolLit _tco_m3)
            {
                var v = _tco_m3.Field0;
                return new BooleanTy();
            }
            else if (_tco_s is IrName _tco_m4)
            {
                var n = _tco_m4.Field0;
                var t = _tco_m4.Field1;
                return t;
            }
            else if (_tco_s is IrBinary _tco_m5)
            {
                var op = _tco_m5.Field0;
                var l = _tco_m5.Field1;
                var r = _tco_m5.Field2;
                var t = _tco_m5.Field3;
                return t;
            }
            else if (_tco_s is IrNegate _tco_m6)
            {
                var x = _tco_m6.Field0;
                return new IntegerTy();
            }
            else if (_tco_s is IrIf _tco_m7)
            {
                var c = _tco_m7.Field0;
                var th = _tco_m7.Field1;
                var el = _tco_m7.Field2;
                var t = _tco_m7.Field3;
                return t;
            }
            else if (_tco_s is IrLet _tco_m8)
            {
                var n = _tco_m8.Field0;
                var t = _tco_m8.Field1;
                var v = _tco_m8.Field2;
                var b = _tco_m8.Field3;
                var _tco_0 = b;
                e = _tco_0;
                continue;
            }
            else if (_tco_s is IrApply _tco_m9)
            {
                var f = _tco_m9.Field0;
                var a = _tco_m9.Field1;
                var t = _tco_m9.Field2;
                return t;
            }
            else if (_tco_s is IrLambda _tco_m10)
            {
                var ps = _tco_m10.Field0;
                var b = _tco_m10.Field1;
                var t = _tco_m10.Field2;
                return t;
            }
            else if (_tco_s is IrList _tco_m11)
            {
                var es = _tco_m11.Field0;
                var t = _tco_m11.Field1;
                return new ListTy(t);
            }
            else if (_tco_s is IrMatch _tco_m12)
            {
                var s = _tco_m12.Field0;
                var bs = _tco_m12.Field1;
                var t = _tco_m12.Field2;
                return t;
            }
            else if (_tco_s is IrDo _tco_m13)
            {
                var ss = _tco_m13.Field0;
                var t = _tco_m13.Field1;
                return t;
            }
            else if (_tco_s is IrHandle _tco_m14)
            {
                var eff = _tco_m14.Field0;
                var h = _tco_m14.Field1;
                var cs = _tco_m14.Field2;
                var t = _tco_m14.Field3;
                return t;
            }
            else if (_tco_s is IrRecord _tco_m15)
            {
                var n = _tco_m15.Field0;
                var fs = _tco_m15.Field1;
                var t = _tco_m15.Field2;
                return t;
            }
            else if (_tco_s is IrFieldAccess _tco_m16)
            {
                var r = _tco_m16.Field0;
                var f = _tco_m16.Field1;
                var t = _tco_m16.Field2;
                return t;
            }
            else if (_tco_s is IrError _tco_m17)
            {
                var m = _tco_m17.Field0;
                var t = _tco_m17.Field1;
                return t;
            }
        }
    }

    public static IRExpr lower_expr(AExpr e, CodexType ty, LowerCtx ctx)
    {
        return ((Func<AExpr, IRExpr>)((_scrutinee23_) => (_scrutinee23_ is ALitExpr _mALitExpr23_ ? ((Func<LiteralKind, IRExpr>)((kind) => ((Func<string, IRExpr>)((text) => lower_literal(text, kind)))((string)_mALitExpr23_.Field0)))((LiteralKind)_mALitExpr23_.Field1) : (_scrutinee23_ is ANameExpr _mANameExpr23_ ? ((Func<Name, IRExpr>)((name) => lower_name(name.value, ty, ctx)))((Name)_mANameExpr23_.Field0) : (_scrutinee23_ is AApplyExpr _mAApplyExpr23_ ? ((Func<AExpr, IRExpr>)((a) => ((Func<AExpr, IRExpr>)((f) => lower_apply(f, a, ty, ctx)))((AExpr)_mAApplyExpr23_.Field0)))((AExpr)_mAApplyExpr23_.Field1) : (_scrutinee23_ is ABinaryExpr _mABinaryExpr23_ ? ((Func<AExpr, IRExpr>)((r) => ((Func<BinaryOp, IRExpr>)((op) => ((Func<AExpr, IRExpr>)((l) => ((Func<IRExpr, IRExpr>)((left_ir) => ((Func<CodexType, IRExpr>)((left_ty) => ((Func<IRExpr, IRExpr>)((right_ir) => new IrBinary(lower_bin_op(op, left_ty), left_ir, right_ir, binary_result_type(op, left_ty, ty))))(lower_expr(r, ty, ctx))))(ir_expr_type(left_ir))))(lower_expr(l, ty, ctx))))((AExpr)_mABinaryExpr23_.Field0)))((BinaryOp)_mABinaryExpr23_.Field1)))((AExpr)_mABinaryExpr23_.Field2) : (_scrutinee23_ is AUnaryExpr _mAUnaryExpr23_ ? ((Func<AExpr, IRExpr>)((operand) => new IrNegate(lower_expr(operand, new IntegerTy(), ctx))))((AExpr)_mAUnaryExpr23_.Field0) : (_scrutinee23_ is AIfExpr _mAIfExpr23_ ? ((Func<AExpr, IRExpr>)((e2) => ((Func<AExpr, IRExpr>)((t) => ((Func<AExpr, IRExpr>)((c) => ((Func<IRExpr, IRExpr>)((then_ir) => ((Func<CodexType, IRExpr>)((then_ty) => ((Func<CodexType, IRExpr>)((result_ty) => ((Func<IRExpr, IRExpr>)((else_ir) => new IrIf(lower_expr(c, new BooleanTy(), ctx), then_ir, else_ir, result_ty)))(lower_expr(e2, result_ty, ctx))))((ty is ErrorTy _mErrorTy24_ ? then_ty : ((Func<CodexType, CodexType>)((_) => ty))(ty)))))(ir_expr_type(then_ir))))(lower_expr(t, ty, ctx))))((AExpr)_mAIfExpr23_.Field0)))((AExpr)_mAIfExpr23_.Field1)))((AExpr)_mAIfExpr23_.Field2) : (_scrutinee23_ is ALetExpr _mALetExpr23_ ? ((Func<AExpr, IRExpr>)((body) => ((Func<List<ALetBind>, IRExpr>)((binds) => lower_let(binds, body, ty, ctx)))((List<ALetBind>)_mALetExpr23_.Field0)))((AExpr)_mALetExpr23_.Field1) : (_scrutinee23_ is ALambdaExpr _mALambdaExpr23_ ? ((Func<AExpr, IRExpr>)((body) => ((Func<List<Name>, IRExpr>)((@params) => lower_lambda(@params, body, ty, ctx)))((List<Name>)_mALambdaExpr23_.Field0)))((AExpr)_mALambdaExpr23_.Field1) : (_scrutinee23_ is AMatchExpr _mAMatchExpr23_ ? ((Func<List<AMatchArm>, IRExpr>)((arms) => ((Func<AExpr, IRExpr>)((scrut) => lower_match(scrut, arms, ty, ctx)))((AExpr)_mAMatchExpr23_.Field0)))((List<AMatchArm>)_mAMatchExpr23_.Field1) : (_scrutinee23_ is AListExpr _mAListExpr23_ ? ((Func<List<AExpr>, IRExpr>)((elems) => lower_list(elems, ty, ctx)))((List<AExpr>)_mAListExpr23_.Field0) : (_scrutinee23_ is ARecordExpr _mARecordExpr23_ ? ((Func<List<AFieldExpr>, IRExpr>)((fields) => ((Func<Name, IRExpr>)((name) => lower_record(name, fields, ty, ctx)))((Name)_mARecordExpr23_.Field0)))((List<AFieldExpr>)_mARecordExpr23_.Field1) : (_scrutinee23_ is AFieldAccess _mAFieldAccess23_ ? ((Func<Name, IRExpr>)((field) => ((Func<AExpr, IRExpr>)((rec) => ((Func<IRExpr, IRExpr>)((rec_ir) => ((Func<CodexType, IRExpr>)((rec_ty) => ((Func<CodexType, IRExpr>)((field_ty) => ((Func<CodexType, IRExpr>)((actual_field_ty) => new IrFieldAccess(rec_ir, field.value, actual_field_ty)))((field_ty is ErrorTy _mErrorTy25_ ? ty : ((Func<CodexType, CodexType>)((_) => field_ty))(field_ty)))))(((Func<CodexType, CodexType>)((_scrutinee26_) => (_scrutinee26_ is RecordTy _mRecordTy26_ ? ((Func<List<RecordField>, CodexType>)((rfields) => ((Func<Name, CodexType>)((rname) => lookup_record_field(rfields, field.value)))((Name)_mRecordTy26_.Field0)))((List<RecordField>)_mRecordTy26_.Field1) : (_scrutinee26_ is ConstructedTy _mConstructedTy26_ ? ((Func<List<CodexType>, CodexType>)((cargs) => ((Func<Name, CodexType>)((cname) => ((Func<CodexType, CodexType>)((ctor_raw) => ((Func<CodexType, CodexType>)((resolved_record) => (resolved_record is RecordTy _mRecordTy27_ ? ((Func<List<RecordField>, CodexType>)((rf) => ((Func<Name, CodexType>)((rn) => lookup_record_field(rf, field.value)))((Name)_mRecordTy27_.Field0)))((List<RecordField>)_mRecordTy27_.Field1) : ((Func<CodexType, CodexType>)((_) => ty))(resolved_record))))((ctor_raw is ErrorTy _mErrorTy28_ ? new ErrorTy() : ((Func<CodexType, CodexType>)((_) => strip_fun_args_lower(deep_resolve(ctx.ust, ctor_raw))))(ctor_raw)))))(lookup_type(ctx.types, cname.value))))((Name)_mConstructedTy26_.Field0)))((List<CodexType>)_mConstructedTy26_.Field1) : ((Func<CodexType, CodexType>)((_) => ty))(_scrutinee26_)))))(rec_ty))))(deep_resolve(ctx.ust, ir_expr_type(rec_ir)))))(lower_expr(rec, new ErrorTy(), ctx))))((AExpr)_mAFieldAccess23_.Field0)))((Name)_mAFieldAccess23_.Field1) : (_scrutinee23_ is ADoExpr _mADoExpr23_ ? ((Func<List<ADoStmt>, IRExpr>)((stmts) => lower_do(stmts, ty, ctx)))((List<ADoStmt>)_mADoExpr23_.Field0) : (_scrutinee23_ is AHandleExpr _mAHandleExpr23_ ? ((Func<List<AHandleClause>, IRExpr>)((clauses) => ((Func<AExpr, IRExpr>)((body) => ((Func<Name, IRExpr>)((eff) => lower_handle(eff, body, clauses, ty, ctx)))((Name)_mAHandleExpr23_.Field0)))((AExpr)_mAHandleExpr23_.Field1)))((List<AHandleClause>)_mAHandleExpr23_.Field2) : (_scrutinee23_ is AErrorExpr _mAErrorExpr23_ ? ((Func<string, IRExpr>)((msg) => new IrError(msg, ty)))((string)_mAErrorExpr23_.Field0) : throw new InvalidOperationException("Non-exhaustive match"))))))))))))))))))(e);
    }

    public static IRExpr lower_name(string name, CodexType ty, LowerCtx ctx)
    {
        return ((Func<CodexType, IRExpr>)((raw) => (raw is ErrorTy _mErrorTy29_ ? new IrName(name, ty) : ((Func<CodexType, IRExpr>)((_) => ((Func<CodexType, IRExpr>)((resolved) => ((Func<CodexType, IRExpr>)((stripped) => new IrName(name, stripped)))(strip_forall_ty(resolved))))(deep_resolve(ctx.ust, raw))))(raw))))(lookup_type(ctx.types, name));
    }

    public static IRExpr lower_literal(string text, LiteralKind kind)
    {
        return ((Func<LiteralKind, IRExpr>)((_scrutinee30_) => (_scrutinee30_ is IntLit _mIntLit30_ ? new IrIntLit(long.Parse(text)) : (_scrutinee30_ is NumLit _mNumLit30_ ? new IrIntLit(long.Parse(text)) : (_scrutinee30_ is TextLit _mTextLit30_ ? new IrTextLit(text) : (_scrutinee30_ is BoolLit _mBoolLit30_ ? new IrBoolLit((text == "True")) : throw new InvalidOperationException("Non-exhaustive match")))))))(kind);
    }

    public static IRExpr lower_apply(AExpr f, AExpr a, CodexType ty, LowerCtx ctx)
    {
        return ((Func<IRExpr, IRExpr>)((func_ir) => ((Func<CodexType, IRExpr>)((func_ty) => ((Func<CodexType, IRExpr>)((arg_ty) => ((Func<CodexType, IRExpr>)((ret_ty) => ((Func<IRExpr, IRExpr>)((arg_ir) => ((Func<CodexType, IRExpr>)((resolved_ret) => ((Func<CodexType, IRExpr>)((actual_ret) => new IrApply(func_ir, arg_ir, actual_ret)))((resolved_ret is ErrorTy _mErrorTy31_ ? ty : ((Func<CodexType, CodexType>)((_) => resolved_ret))(resolved_ret)))))(subst_type_vars_from_arg(arg_ty, ir_expr_type(arg_ir), ret_ty))))(lower_expr(a, arg_ty, ctx))))(peel_fun_return(func_ty))))(peel_fun_param(func_ty))))(deep_resolve(ctx.ust, ir_expr_type(func_ir)))))(lower_expr(f, new ErrorTy(), ctx));
    }

    public static IRExpr lower_let(List<ALetBind> binds, AExpr body, CodexType ty, LowerCtx ctx)
    {
        return ((((long)binds.Count) == 0L) ? lower_expr(body, ty, ctx) : ((Func<ALetBind, IRExpr>)((b) => ((Func<IRExpr, IRExpr>)((val_ir) => ((Func<CodexType, IRExpr>)((val_ty) => ((Func<LowerCtx, IRExpr>)((ctx2) => new IrLet(b.name.value, val_ty, val_ir, lower_let_rest(binds, body, ty, ctx2, 1L))))(new LowerCtx(Enumerable.Concat(new List<TypeBinding>() { new TypeBinding(b.name.value, val_ty) }, ctx.types).ToList(), ctx.ust))))(deep_resolve(ctx.ust, ir_expr_type(val_ir)))))(lower_expr(b.value, new ErrorTy(), ctx))))(binds[(int)0L]));
    }

    public static IRExpr lower_let_rest(List<ALetBind> binds, AExpr body, CodexType ty, LowerCtx ctx, long i)
    {
        return ((i == ((long)binds.Count)) ? lower_expr(body, ty, ctx) : ((Func<ALetBind, IRExpr>)((b) => ((Func<IRExpr, IRExpr>)((val_ir) => ((Func<CodexType, IRExpr>)((val_ty) => ((Func<LowerCtx, IRExpr>)((ctx2) => new IrLet(b.name.value, val_ty, val_ir, lower_let_rest(binds, body, ty, ctx2, (i + 1L)))))(new LowerCtx(Enumerable.Concat(new List<TypeBinding>() { new TypeBinding(b.name.value, val_ty) }, ctx.types).ToList(), ctx.ust))))(deep_resolve(ctx.ust, ir_expr_type(val_ir)))))(lower_expr(b.value, new ErrorTy(), ctx))))(binds[(int)i]));
    }

    public static IRExpr lower_lambda(List<Name> @params, AExpr body, CodexType ty, LowerCtx ctx)
    {
        return ((Func<CodexType, IRExpr>)((stripped) => ((Func<List<IRParam>, IRExpr>)((lparams) => ((Func<LowerCtx, IRExpr>)((lctx) => new IrLambda(lparams, lower_expr(body, get_lambda_return(stripped, ((long)@params.Count)), lctx), ty)))(bind_lambda_to_ctx(ctx, @params, stripped, 0L))))(lower_lambda_params(@params, stripped, 0L))))(strip_forall_ty(ty));
    }

    public static LowerCtx bind_lambda_to_ctx(LowerCtx ctx, List<Name> @params, CodexType ty, long i)
    {
        while (true)
        {
            if ((i == ((long)@params.Count)))
            {
                return ctx;
            }
            else
            {
                var p = @params[(int)i];
                var param_ty = peel_fun_param(ty);
                var rest_ty = peel_fun_return(ty);
                var ctx2 = new LowerCtx(Enumerable.Concat(new List<TypeBinding>() { new TypeBinding(p.value, param_ty) }, ctx.types).ToList(), ctx.ust);
                var _tco_0 = ctx2;
                var _tco_1 = @params;
                var _tco_2 = rest_ty;
                var _tco_3 = (i + 1L);
                ctx = _tco_0;
                @params = _tco_1;
                ty = _tco_2;
                i = _tco_3;
                continue;
            }
        }
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

    public static IRExpr lower_match(AExpr scrut, List<AMatchArm> arms, CodexType ty, LowerCtx ctx)
    {
        return ((Func<IRExpr, IRExpr>)((scrut_ir) => ((Func<CodexType, IRExpr>)((scrut_ty) => ((Func<List<IRBranch>, IRExpr>)((branches) => ((Func<CodexType, IRExpr>)((result_ty) => new IrMatch(scrut_ir, branches, result_ty)))((ty is ErrorTy _mErrorTy32_ ? infer_match_type(branches, 0L, ((long)branches.Count)) : ((Func<CodexType, CodexType>)((_) => ty))(ty)))))(lower_match_arms_loop(arms, ty, scrut_ty, ctx, 0L, ((long)arms.Count)))))(ir_expr_type(scrut_ir))))(lower_expr(scrut, new ErrorTy(), ctx));
    }

    public static CodexType infer_match_type(List<IRBranch> branches, long i, long len)
    {
        while (true)
        {
            if ((i == len))
            {
                return new ErrorTy();
            }
            else
            {
                var b = branches[(int)i];
                var body_ty = ir_expr_type(b.body);
                var _tco_s = body_ty;
                if (_tco_s is ErrorTy _tco_m0)
                {
                    var _tco_0 = branches;
                    var _tco_1 = (i + 1L);
                    var _tco_2 = len;
                    branches = _tco_0;
                    i = _tco_1;
                    len = _tco_2;
                    continue;
                }
                {
                    var _ = _tco_s;
                    return body_ty;
                }
            }
        }
    }

    public static List<IRBranch> lower_match_arms_loop(List<AMatchArm> arms, CodexType ty, CodexType scrut_ty, LowerCtx ctx, long i, long len)
    {
        return ((i == len) ? new List<IRBranch>() : ((Func<AMatchArm, List<IRBranch>>)((arm) => ((Func<LowerCtx, List<IRBranch>>)((arm_ctx) => Enumerable.Concat(new List<IRBranch>() { new IRBranch(lower_pattern(arm.pattern), lower_expr(arm.body, ty, arm_ctx)) }, lower_match_arms_loop(arms, ty, scrut_ty, ctx, (i + 1L), len)).ToList()))(bind_pattern_to_ctx(ctx, arm.pattern, scrut_ty))))(arms[(int)i]));
    }

    public static LowerCtx bind_pattern_to_ctx(LowerCtx ctx, APat pat, CodexType ty)
    {
        return ((Func<APat, LowerCtx>)((_scrutinee33_) => (_scrutinee33_ is AVarPat _mAVarPat33_ ? ((Func<Name, LowerCtx>)((name) => new LowerCtx(Enumerable.Concat(new List<TypeBinding>() { new TypeBinding(name.value, ty) }, ctx.types).ToList(), ctx.ust)))((Name)_mAVarPat33_.Field0) : (_scrutinee33_ is ACtorPat _mACtorPat33_ ? ((Func<List<APat>, LowerCtx>)((sub_pats) => ((Func<Name, LowerCtx>)((ctor_name) => ((Func<CodexType, LowerCtx>)((ctor_raw) => ((Func<CodexType, LowerCtx>)((ctor_ty) => ((Func<CodexType, LowerCtx>)((ctor_stripped) => bind_ctor_pattern_fields(ctx, sub_pats, ctor_stripped, 0L, ((long)sub_pats.Count))))(strip_forall_ty(ctor_ty))))(deep_resolve(ctx.ust, ctor_raw))))(lookup_type(ctx.types, ctor_name.value))))((Name)_mACtorPat33_.Field0)))((List<APat>)_mACtorPat33_.Field1) : (_scrutinee33_ is AWildPat _mAWildPat33_ ? ctx : (_scrutinee33_ is ALitPat _mALitPat33_ ? ((Func<LiteralKind, LowerCtx>)((kind) => ((Func<string, LowerCtx>)((text) => ctx))((string)_mALitPat33_.Field0)))((LiteralKind)_mALitPat33_.Field1) : throw new InvalidOperationException("Non-exhaustive match")))))))(pat);
    }

    public static LowerCtx bind_ctor_pattern_fields(LowerCtx ctx, List<APat> sub_pats, CodexType ctor_ty, long i, long len)
    {
        while (true)
        {
            if ((i == len))
            {
                return ctx;
            }
            else
            {
                var _tco_s = ctor_ty;
                if (_tco_s is FunTy _tco_m0)
                {
                    var param_ty = _tco_m0.Field0;
                    var ret_ty = _tco_m0.Field1;
                    var ctx2 = bind_pattern_to_ctx(ctx, sub_pats[(int)i], param_ty);
                    var _tco_0 = ctx2;
                    var _tco_1 = sub_pats;
                    var _tco_2 = ret_ty;
                    var _tco_3 = (i + 1L);
                    var _tco_4 = len;
                    ctx = _tco_0;
                    sub_pats = _tco_1;
                    ctor_ty = _tco_2;
                    i = _tco_3;
                    len = _tco_4;
                    continue;
                }
                {
                    var _ = _tco_s;
                    var ctx2 = bind_pattern_to_ctx(ctx, sub_pats[(int)i], new ErrorTy());
                    var _tco_0 = ctx2;
                    var _tco_1 = sub_pats;
                    var _tco_2 = ctor_ty;
                    var _tco_3 = (i + 1L);
                    var _tco_4 = len;
                    ctx = _tco_0;
                    sub_pats = _tco_1;
                    ctor_ty = _tco_2;
                    i = _tco_3;
                    len = _tco_4;
                    continue;
                }
            }
        }
    }

    public static IRPat lower_pattern(APat p)
    {
        return ((Func<APat, IRPat>)((_scrutinee34_) => (_scrutinee34_ is AVarPat _mAVarPat34_ ? ((Func<Name, IRPat>)((name) => new IrVarPat(name.value, new ErrorTy())))((Name)_mAVarPat34_.Field0) : (_scrutinee34_ is ALitPat _mALitPat34_ ? ((Func<LiteralKind, IRPat>)((kind) => ((Func<string, IRPat>)((text) => new IrLitPat(text, new ErrorTy())))((string)_mALitPat34_.Field0)))((LiteralKind)_mALitPat34_.Field1) : (_scrutinee34_ is ACtorPat _mACtorPat34_ ? ((Func<List<APat>, IRPat>)((subs) => ((Func<Name, IRPat>)((name) => new IrCtorPat(name.value, map_list(new Func<APat, IRPat>(lower_pattern), subs), new ErrorTy())))((Name)_mACtorPat34_.Field0)))((List<APat>)_mACtorPat34_.Field1) : (_scrutinee34_ is AWildPat _mAWildPat34_ ? new IrWildPat() : throw new InvalidOperationException("Non-exhaustive match")))))))(p);
    }

    public static IRExpr lower_list(List<AExpr> elems, CodexType ty, LowerCtx ctx)
    {
        return ((Func<CodexType, IRExpr>)((elem_ty) => new IrList(lower_list_elems_loop(elems, elem_ty, ctx, 0L, ((long)elems.Count)), elem_ty)))((ty is ListTy _mListTy35_ ? ((Func<CodexType, CodexType>)((e) => e))((CodexType)_mListTy35_.Field0) : ((Func<CodexType, CodexType>)((_) => ((((long)elems.Count) == 0L) ? new ErrorTy() : ir_expr_type(lower_expr(elems[(int)0L], new ErrorTy(), ctx)))))(ty)));
    }

    public static List<IRExpr> lower_list_elems_loop(List<AExpr> elems, CodexType elem_ty, LowerCtx ctx, long i, long len)
    {
        return ((i == len) ? new List<IRExpr>() : Enumerable.Concat(new List<IRExpr>() { lower_expr(elems[(int)i], elem_ty, ctx) }, lower_list_elems_loop(elems, elem_ty, ctx, (i + 1L), len)).ToList());
    }

    public static IRExpr lower_record(Name name, List<AFieldExpr> fields, CodexType ty, LowerCtx ctx)
    {
        return ((Func<CodexType, IRExpr>)((ctor_raw) => ((Func<CodexType, IRExpr>)((record_ty) => ((Func<CodexType, IRExpr>)((actual_ty) => new IrRecord(name.value, lower_record_fields_typed(fields, actual_ty, ctx, 0L, ((long)fields.Count)), actual_ty)))((record_ty is ErrorTy _mErrorTy36_ ? ty : ((Func<CodexType, CodexType>)((_) => record_ty))(record_ty)))))((ctor_raw is ErrorTy _mErrorTy37_ ? ty : ((Func<CodexType, CodexType>)((_) => strip_fun_args_lower(deep_resolve(ctx.ust, ctor_raw))))(ctor_raw)))))(lookup_type(ctx.types, name.value));
    }

    public static List<IRFieldVal> lower_record_fields_typed(List<AFieldExpr> fields, CodexType record_ty, LowerCtx ctx, long i, long len)
    {
        return ((i == len) ? new List<IRFieldVal>() : ((Func<AFieldExpr, List<IRFieldVal>>)((f) => ((Func<CodexType, List<IRFieldVal>>)((field_expected) => Enumerable.Concat(new List<IRFieldVal>() { new IRFieldVal(f.name.value, lower_expr(f.value, field_expected, ctx)) }, lower_record_fields_typed(fields, record_ty, ctx, (i + 1L), len)).ToList()))((record_ty is RecordTy _mRecordTy38_ ? ((Func<List<RecordField>, CodexType>)((rfields) => ((Func<Name, CodexType>)((rname) => lookup_record_field(rfields, f.name.value)))((Name)_mRecordTy38_.Field0)))((List<RecordField>)_mRecordTy38_.Field1) : ((Func<CodexType, CodexType>)((_) => new ErrorTy()))(record_ty)))))(fields[(int)i]));
    }

    public static IRExpr lower_do(List<ADoStmt> stmts, CodexType ty, LowerCtx ctx)
    {
        return new IrDo(lower_do_stmts_loop(stmts, ty, ctx, 0L, ((long)stmts.Count)), ty);
    }

    public static List<IRDoStmt> lower_do_stmts_loop(List<ADoStmt> stmts, CodexType ty, LowerCtx ctx, long i, long len)
    {
        return ((i == len) ? new List<IRDoStmt>() : ((Func<ADoStmt, List<IRDoStmt>>)((s) => ((Func<ADoStmt, List<IRDoStmt>>)((_scrutinee39_) => (_scrutinee39_ is ADoBindStmt _mADoBindStmt39_ ? ((Func<AExpr, List<IRDoStmt>>)((val) => ((Func<Name, List<IRDoStmt>>)((name) => ((Func<IRExpr, List<IRDoStmt>>)((val_ir) => ((Func<CodexType, List<IRDoStmt>>)((val_ty) => ((Func<LowerCtx, List<IRDoStmt>>)((ctx2) => Enumerable.Concat(new List<IRDoStmt>() { new IrDoBind(name.value, val_ty, val_ir) }, lower_do_stmts_loop(stmts, ty, ctx2, (i + 1L), len)).ToList()))(new LowerCtx(Enumerable.Concat(new List<TypeBinding>() { new TypeBinding(name.value, val_ty) }, ctx.types).ToList(), ctx.ust))))(ir_expr_type(val_ir))))(lower_expr(val, ty, ctx))))((Name)_mADoBindStmt39_.Field0)))((AExpr)_mADoBindStmt39_.Field1) : (_scrutinee39_ is ADoExprStmt _mADoExprStmt39_ ? ((Func<AExpr, List<IRDoStmt>>)((e) => Enumerable.Concat(new List<IRDoStmt>() { new IrDoExec(lower_expr(e, ty, ctx)) }, lower_do_stmts_loop(stmts, ty, ctx, (i + 1L), len)).ToList()))((AExpr)_mADoExprStmt39_.Field0) : throw new InvalidOperationException("Non-exhaustive match")))))(s)))(stmts[(int)i]));
    }

    public static IRExpr lower_handle(Name eff, AExpr body, List<AHandleClause> clauses, CodexType ty, LowerCtx ctx)
    {
        return ((Func<IRExpr, IRExpr>)((body_ir) => new IrHandle(eff.value, body_ir, lower_handle_clauses(clauses, ty, ctx), ty)))(lower_expr(body, ty, ctx));
    }

    public static List<IRHandleClause> lower_handle_clauses(List<AHandleClause> clauses, CodexType ty, LowerCtx ctx)
    {
        return lower_handle_clauses_loop(clauses, ty, ctx, 0L);
    }

    public static List<IRHandleClause> lower_handle_clauses_loop(List<AHandleClause> clauses, CodexType ty, LowerCtx ctx, long i)
    {
        return ((i == ((long)clauses.Count)) ? new List<IRHandleClause>() : ((Func<AHandleClause, List<IRHandleClause>>)((c) => ((Func<IRExpr, List<IRHandleClause>>)((body_ir) => Enumerable.Concat(new List<IRHandleClause>() { new IRHandleClause(c.op_name.value, c.resume_name.value, body_ir) }, lower_handle_clauses_loop(clauses, ty, ctx, (i + 1L))).ToList()))(lower_expr(c.body, ty, ctx))))(clauses[(int)i]));
    }

    public static IRDef lower_def(ADef d, List<TypeBinding> types, UnificationState ust)
    {
        return ((Func<CodexType, IRDef>)((raw_type) => ((Func<CodexType, IRDef>)((full_type) => ((Func<CodexType, IRDef>)((stripped) => ((Func<List<IRParam>, IRDef>)((@params) => ((Func<CodexType, IRDef>)((ret_type) => ((Func<LowerCtx, IRDef>)((ctx) => new IRDef(d.name.value, @params, full_type, lower_expr(d.body, ret_type, ctx))))(build_def_ctx(types, ust, d.@params, stripped))))(get_return_type_n(stripped, ((long)d.@params.Count)))))(lower_def_params(d.@params, stripped, 0L))))(strip_forall_ty(full_type))))(deep_resolve(ust, raw_type))))(lookup_type(types, d.name.value));
    }

    public static LowerCtx build_def_ctx(List<TypeBinding> types, UnificationState ust, List<AParam> @params, CodexType ty)
    {
        return ((Func<LowerCtx, LowerCtx>)((base_ctx) => bind_params_to_ctx(base_ctx, @params, ty, 0L)))(new LowerCtx(types, ust));
    }

    public static LowerCtx bind_params_to_ctx(LowerCtx ctx, List<AParam> @params, CodexType ty, long i)
    {
        while (true)
        {
            if ((i == ((long)@params.Count)))
            {
                return ctx;
            }
            else
            {
                var p = @params[(int)i];
                var param_ty = peel_fun_param(ty);
                var rest_ty = peel_fun_return(ty);
                var ctx2 = new LowerCtx(Enumerable.Concat(new List<TypeBinding>() { new TypeBinding(p.name.value, param_ty) }, ctx.types).ToList(), ctx.ust);
                var _tco_0 = ctx2;
                var _tco_1 = @params;
                var _tco_2 = rest_ty;
                var _tco_3 = (i + 1L);
                ctx = _tco_0;
                @params = _tco_1;
                ty = _tco_2;
                i = _tco_3;
                continue;
            }
        }
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
        return ((Func<List<TypeBinding>, IRModule>)((ctor_types) => ((Func<List<TypeBinding>, IRModule>)((all_types) => new IRModule(m.name, lower_defs(m.defs, all_types, ust, 0L))))(Enumerable.Concat(ctor_types, Enumerable.Concat(types, builtin_type_env().bindings).ToList()).ToList())))(collect_ctor_bindings(m.type_defs, 0L, ((long)m.type_defs.Count), new List<TypeBinding>()));
    }

    public static List<IRDef> lower_defs(List<ADef> defs, List<TypeBinding> types, UnificationState ust, long i)
    {
        return ((i == ((long)defs.Count)) ? new List<IRDef>() : Enumerable.Concat(new List<IRDef>() { lower_def(defs[(int)i], types, ust) }, lower_defs(defs, types, ust, (i + 1L))).ToList());
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

    public static CodexType subst_type_vars_from_arg(CodexType param_ty, CodexType arg_ty, CodexType target)
    {
        return ((Func<CodexType, CodexType>)((_scrutinee40_) => (_scrutinee40_ is TypeVar _mTypeVar40_ ? ((Func<long, CodexType>)((id) => subst_type_var_in_target(target, id, arg_ty)))((long)_mTypeVar40_.Field0) : (_scrutinee40_ is ListTy _mListTy40_ ? ((Func<CodexType, CodexType>)((pe) => subst_from_list(pe, arg_ty, target)))((CodexType)_mListTy40_.Field0) : (_scrutinee40_ is FunTy _mFunTy40_ ? ((Func<CodexType, CodexType>)((pr) => ((Func<CodexType, CodexType>)((pp) => subst_from_fun(pp, pr, arg_ty, target)))((CodexType)_mFunTy40_.Field0)))((CodexType)_mFunTy40_.Field1) : ((Func<CodexType, CodexType>)((_) => target))(_scrutinee40_))))))(param_ty);
    }

    public static CodexType subst_from_list(CodexType pe, CodexType arg_ty, CodexType target)
    {
        return (arg_ty is ListTy _mListTy41_ ? ((Func<CodexType, CodexType>)((ae) => subst_type_vars_from_arg(pe, ae, target)))((CodexType)_mListTy41_.Field0) : ((Func<CodexType, CodexType>)((_) => target))(arg_ty));
    }

    public static CodexType subst_from_fun(CodexType pp, CodexType pr, CodexType arg_ty, CodexType target)
    {
        return (arg_ty is FunTy _mFunTy42_ ? ((Func<CodexType, CodexType>)((ar) => ((Func<CodexType, CodexType>)((ap) => ((Func<CodexType, CodexType>)((t2) => subst_type_vars_from_arg(pr, ar, t2)))(subst_type_vars_from_arg(pp, ap, target))))((CodexType)_mFunTy42_.Field0)))((CodexType)_mFunTy42_.Field1) : ((Func<CodexType, CodexType>)((_) => target))(arg_ty));
    }

    public static CodexType subst_type_var_in_target(CodexType ty, long var_id, CodexType replacement)
    {
        return ((Func<CodexType, CodexType>)((_scrutinee43_) => (_scrutinee43_ is TypeVar _mTypeVar43_ ? ((Func<long, CodexType>)((id) => ((id == var_id) ? replacement : ty)))((long)_mTypeVar43_.Field0) : (_scrutinee43_ is FunTy _mFunTy43_ ? ((Func<CodexType, CodexType>)((r) => ((Func<CodexType, CodexType>)((p) => new FunTy(subst_type_var_in_target(p, var_id, replacement), subst_type_var_in_target(r, var_id, replacement))))((CodexType)_mFunTy43_.Field0)))((CodexType)_mFunTy43_.Field1) : (_scrutinee43_ is ListTy _mListTy43_ ? ((Func<CodexType, CodexType>)((elem) => new ListTy(subst_type_var_in_target(elem, var_id, replacement))))((CodexType)_mListTy43_.Field0) : (_scrutinee43_ is ForAllTy _mForAllTy43_ ? ((Func<CodexType, CodexType>)((body) => ((Func<long, CodexType>)((fid) => ((fid == var_id) ? ty : new ForAllTy(fid, subst_type_var_in_target(body, var_id, replacement)))))((long)_mForAllTy43_.Field0)))((CodexType)_mForAllTy43_.Field1) : ((Func<CodexType, CodexType>)((_) => ty))(_scrutinee43_)))))))(ty);
    }

    public static CodexType strip_fun_args_lower(CodexType ty)
    {
        while (true)
        {
            var _tco_s = ty;
            if (_tco_s is FunTy _tco_m0)
            {
                var p = _tco_m0.Field0;
                var r = _tco_m0.Field1;
                var _tco_0 = r;
                ty = _tco_0;
                continue;
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
                return ty;
            }
        }
    }

    public static bool is_text_type(CodexType ty)
    {
        return (ty is TextTy _mTextTy44_ ? true : ((Func<CodexType, bool>)((_) => false))(ty));
    }

    public static List<TypeBinding> collect_ctor_bindings(List<ATypeDef> tdefs, long i, long len, List<TypeBinding> acc)
    {
        while (true)
        {
            if ((i == len))
            {
                return acc;
            }
            else
            {
                var td = tdefs[(int)i];
                var bindings = ctor_bindings_for_typedef(td);
                var _tco_0 = tdefs;
                var _tco_1 = (i + 1L);
                var _tco_2 = len;
                var _tco_3 = Enumerable.Concat(acc, bindings).ToList();
                tdefs = _tco_0;
                i = _tco_1;
                len = _tco_2;
                acc = _tco_3;
                continue;
            }
        }
    }

    public static List<TypeBinding> ctor_bindings_for_typedef(ATypeDef td)
    {
        return ((Func<ATypeDef, List<TypeBinding>>)((_scrutinee45_) => (_scrutinee45_ is AVariantTypeDef _mAVariantTypeDef45_ ? ((Func<List<AVariantCtorDef>, List<TypeBinding>>)((ctors) => ((Func<List<Name>, List<TypeBinding>>)((type_params) => ((Func<Name, List<TypeBinding>>)((name) => ((Func<CodexType, List<TypeBinding>>)((result_ty) => collect_variant_ctor_bindings(ctors, result_ty, 0L, ((long)ctors.Count), new List<TypeBinding>())))(new ConstructedTy(name, new List<CodexType>()))))((Name)_mAVariantTypeDef45_.Field0)))((List<Name>)_mAVariantTypeDef45_.Field1)))((List<AVariantCtorDef>)_mAVariantTypeDef45_.Field2) : (_scrutinee45_ is ARecordTypeDef _mARecordTypeDef45_ ? ((Func<List<ARecordFieldDef>, List<TypeBinding>>)((fields) => ((Func<List<Name>, List<TypeBinding>>)((type_params) => ((Func<Name, List<TypeBinding>>)((name) => ((Func<List<RecordField>, List<TypeBinding>>)((resolved_fields) => ((Func<CodexType, List<TypeBinding>>)((result_ty) => ((Func<CodexType, List<TypeBinding>>)((ctor_ty) => new List<TypeBinding>() { new TypeBinding(name.value, ctor_ty) }))(build_record_ctor_type_for_lower(fields, result_ty, 0L, ((long)fields.Count)))))(new RecordTy(name, resolved_fields))))(build_record_fields_for_lower(fields, 0L, ((long)fields.Count), new List<RecordField>()))))((Name)_mARecordTypeDef45_.Field0)))((List<Name>)_mARecordTypeDef45_.Field1)))((List<ARecordFieldDef>)_mARecordTypeDef45_.Field2) : throw new InvalidOperationException("Non-exhaustive match")))))(td);
    }

    public static List<TypeBinding> collect_variant_ctor_bindings(List<AVariantCtorDef> ctors, CodexType result_ty, long i, long len, List<TypeBinding> acc)
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
                var ctor_ty = build_ctor_type_for_lower(ctor.fields, result_ty, 0L, ((long)ctor.fields.Count));
                var _tco_0 = ctors;
                var _tco_1 = result_ty;
                var _tco_2 = (i + 1L);
                var _tco_3 = len;
                var _tco_4 = Enumerable.Concat(acc, new List<TypeBinding>() { new TypeBinding(ctor.name.value, ctor_ty) }).ToList();
                ctors = _tco_0;
                result_ty = _tco_1;
                i = _tco_2;
                len = _tco_3;
                acc = _tco_4;
                continue;
            }
        }
    }

    public static CodexType build_ctor_type_for_lower(List<ATypeExpr> fields, CodexType result, long i, long len)
    {
        return ((i == len) ? result : ((Func<CodexType, CodexType>)((rest) => new FunTy(resolve_type_expr_for_lower(fields[(int)i]), rest)))(build_ctor_type_for_lower(fields, result, (i + 1L), len)));
    }

    public static List<RecordField> build_record_fields_for_lower(List<ARecordFieldDef> fields, long i, long len, List<RecordField> acc)
    {
        while (true)
        {
            if ((i == len))
            {
                return acc;
            }
            else
            {
                var f = fields[(int)i];
                var rfield = new RecordField(f.name, resolve_type_expr_for_lower(f.type_expr));
                var _tco_0 = fields;
                var _tco_1 = (i + 1L);
                var _tco_2 = len;
                var _tco_3 = Enumerable.Concat(acc, new List<RecordField>() { rfield }).ToList();
                fields = _tco_0;
                i = _tco_1;
                len = _tco_2;
                acc = _tco_3;
                continue;
            }
        }
    }

    public static CodexType build_record_ctor_type_for_lower(List<ARecordFieldDef> fields, CodexType result, long i, long len)
    {
        return ((i == len) ? result : ((Func<ARecordFieldDef, CodexType>)((f) => ((Func<CodexType, CodexType>)((rest) => new FunTy(resolve_type_expr_for_lower(f.type_expr), rest)))(build_record_ctor_type_for_lower(fields, result, (i + 1L), len))))(fields[(int)i]));
    }

    public static CodexType resolve_type_expr_for_lower(ATypeExpr texpr)
    {
        return ((Func<ATypeExpr, CodexType>)((_scrutinee46_) => (_scrutinee46_ is ANamedType _mANamedType46_ ? ((Func<Name, CodexType>)((name) => ((name.value == "Integer") ? new IntegerTy() : ((name.value == "Number") ? new NumberTy() : ((name.value == "Text") ? new TextTy() : ((name.value == "Boolean") ? new BooleanTy() : ((name.value == "Nothing") ? new NothingTy() : new ConstructedTy(name, new List<CodexType>()))))))))((Name)_mANamedType46_.Field0) : (_scrutinee46_ is AFunType _mAFunType46_ ? ((Func<ATypeExpr, CodexType>)((ret) => ((Func<ATypeExpr, CodexType>)((param) => new FunTy(resolve_type_expr_for_lower(param), resolve_type_expr_for_lower(ret))))((ATypeExpr)_mAFunType46_.Field0)))((ATypeExpr)_mAFunType46_.Field1) : (_scrutinee46_ is AAppType _mAAppType46_ ? ((Func<List<ATypeExpr>, CodexType>)((args) => ((Func<ATypeExpr, CodexType>)((ctor) => (ctor is ANamedType _mANamedType47_ ? ((Func<Name, CodexType>)((cname) => ((cname.value == "List") ? ((((long)args.Count) == 1L) ? new ListTy(resolve_type_expr_for_lower(args[(int)0L])) : new ListTy(new ErrorTy())) : new ConstructedTy(cname, new List<CodexType>()))))((Name)_mANamedType47_.Field0) : ((Func<ATypeExpr, CodexType>)((_) => new ErrorTy()))(ctor))))((ATypeExpr)_mAAppType46_.Field0)))((List<ATypeExpr>)_mAAppType46_.Field1) : throw new InvalidOperationException("Non-exhaustive match"))))))(texpr);
    }

    public static IRBinaryOp lower_bin_op(BinaryOp op, CodexType ty)
    {
        return ((Func<BinaryOp, IRBinaryOp>)((_scrutinee48_) => (_scrutinee48_ is OpAdd _mOpAdd48_ ? new IrAddInt() : (_scrutinee48_ is OpSub _mOpSub48_ ? new IrSubInt() : (_scrutinee48_ is OpMul _mOpMul48_ ? new IrMulInt() : (_scrutinee48_ is OpDiv _mOpDiv48_ ? new IrDivInt() : (_scrutinee48_ is OpPow _mOpPow48_ ? new IrPowInt() : (_scrutinee48_ is OpEq _mOpEq48_ ? new IrEq() : (_scrutinee48_ is OpNotEq _mOpNotEq48_ ? new IrNotEq() : (_scrutinee48_ is OpLt _mOpLt48_ ? new IrLt() : (_scrutinee48_ is OpGt _mOpGt48_ ? new IrGt() : (_scrutinee48_ is OpLtEq _mOpLtEq48_ ? new IrLtEq() : (_scrutinee48_ is OpGtEq _mOpGtEq48_ ? new IrGtEq() : (_scrutinee48_ is OpDefEq _mOpDefEq48_ ? new IrEq() : (_scrutinee48_ is OpAppend _mOpAppend48_ ? (is_text_type(ty) ? new IrAppendText() : new IrAppendList()) : (_scrutinee48_ is OpCons _mOpCons48_ ? new IrConsList() : (_scrutinee48_ is OpAnd _mOpAnd48_ ? new IrAnd() : (_scrutinee48_ is OpOr _mOpOr48_ ? new IrOr() : throw new InvalidOperationException("Non-exhaustive match")))))))))))))))))))(op);
    }

    public static CodexType binary_result_type(BinaryOp op, CodexType left_ty, CodexType expected_ty)
    {
        return ((Func<BinaryOp, CodexType>)((_scrutinee49_) => (_scrutinee49_ is OpEq _mOpEq49_ ? new BooleanTy() : (_scrutinee49_ is OpNotEq _mOpNotEq49_ ? new BooleanTy() : (_scrutinee49_ is OpLt _mOpLt49_ ? new BooleanTy() : (_scrutinee49_ is OpGt _mOpGt49_ ? new BooleanTy() : (_scrutinee49_ is OpLtEq _mOpLtEq49_ ? new BooleanTy() : (_scrutinee49_ is OpGtEq _mOpGtEq49_ ? new BooleanTy() : (_scrutinee49_ is OpDefEq _mOpDefEq49_ ? new BooleanTy() : (_scrutinee49_ is OpAnd _mOpAnd49_ ? new BooleanTy() : (_scrutinee49_ is OpOr _mOpOr49_ ? new BooleanTy() : (_scrutinee49_ is OpAppend _mOpAppend49_ ? (is_text_type(left_ty) ? new TextTy() : (is_text_type(expected_ty) ? new TextTy() : left_ty)) : ((Func<BinaryOp, CodexType>)((_) => left_ty))(_scrutinee49_)))))))))))))(op);
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
        return new List<string>() { "show", "negate", "True", "False", "Nothing", "print-line", "read-line", "read-file", "open-file", "read-all", "close-file", "char-at", "text-length", "substring", "is-letter", "is-digit", "is-whitespace", "text-to-integer", "integer-to-text", "text-replace", "char-code", "char-code-at", "code-to-char", "list-length", "list-at", "map", "filter", "fold" };
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
        return ((Func<APat, Scope>)((_scrutinee50_) => (_scrutinee50_ is AVarPat _mAVarPat50_ ? ((Func<Name, Scope>)((name) => scope_add(sc, name.value)))((Name)_mAVarPat50_.Field0) : (_scrutinee50_ is ACtorPat _mACtorPat50_ ? ((Func<List<APat>, Scope>)((subs) => ((Func<Name, Scope>)((name) => collect_ctor_pat_names(sc, subs, 0L, ((long)subs.Count))))((Name)_mACtorPat50_.Field0)))((List<APat>)_mACtorPat50_.Field1) : (_scrutinee50_ is ALitPat _mALitPat50_ ? ((Func<LiteralKind, Scope>)((kind) => ((Func<string, Scope>)((val) => sc))((string)_mALitPat50_.Field0)))((LiteralKind)_mALitPat50_.Field1) : (_scrutinee50_ is AWildPat _mAWildPat50_ ? sc : throw new InvalidOperationException("Non-exhaustive match")))))))(pat);
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
        return resolve_module_with_imports(mod, new List<ResolveResult>());
    }

    public static ResolveResult resolve_module_with_imports(AModule mod, List<ResolveResult> imported)
    {
        return ((Func<CollectResult, ResolveResult>)((top) => ((Func<CtorCollectResult, ResolveResult>)((ctors) => ((Func<List<string>, ResolveResult>)((imported_names) => ((Func<List<string>, ResolveResult>)((all_top) => ((Func<Scope, ResolveResult>)((sc) => ((Func<List<Diagnostic>, ResolveResult>)((expr_errs) => new ResolveResult(Enumerable.Concat(top.errors, expr_errs).ToList(), top.names, ctors.type_names, ctors.ctor_names)))(resolve_all_defs(sc, mod.defs, 0L, ((long)mod.defs.Count), new List<Diagnostic>()))))(build_all_names_scope(all_top, ctors.ctor_names, builtin_names()))))(Enumerable.Concat(top.names, imported_names).ToList())))(collect_imported_names(imported, 0L, ((long)imported.Count), new List<string>()))))(collect_ctor_names(mod.type_defs, 0L, ((long)mod.type_defs.Count), new List<string>(), new List<string>()))))(collect_top_level_names(mod.defs, 0L, ((long)mod.defs.Count), new List<string>(), new List<Diagnostic>()));
    }

    public static List<string> collect_imported_names(List<ResolveResult> results, long i, long len, List<string> acc)
    {
        while (true)
        {
            if ((i == len))
            {
                return acc;
            }
            else
            {
                var r = results[(int)i];
                var names = Enumerable.Concat(r.top_level_names, r.ctor_names).ToList();
                var _tco_0 = results;
                var _tco_1 = (i + 1L);
                var _tco_2 = len;
                var _tco_3 = Enumerable.Concat(acc, names).ToList();
                results = _tco_0;
                i = _tco_1;
                len = _tco_2;
                acc = _tco_3;
                continue;
            }
        }
    }

    public static long cc_newline()
    {
        return 10L;
    }

    public static long cc_carriage_return()
    {
        return 13L;
    }

    public static long cc_space()
    {
        return 32L;
    }

    public static long cc_double_quote()
    {
        return 34L;
    }

    public static long cc_ampersand()
    {
        return 38L;
    }

    public static long cc_left_paren()
    {
        return 40L;
    }

    public static long cc_right_paren()
    {
        return 41L;
    }

    public static long cc_star()
    {
        return 42L;
    }

    public static long cc_plus()
    {
        return 43L;
    }

    public static long cc_comma()
    {
        return 44L;
    }

    public static long cc_minus()
    {
        return 45L;
    }

    public static long cc_dot()
    {
        return 46L;
    }

    public static long cc_slash()
    {
        return 47L;
    }

    public static long cc_zero()
    {
        return 48L;
    }

    public static long cc_nine()
    {
        return 57L;
    }

    public static long cc_colon()
    {
        return 58L;
    }

    public static long cc_less()
    {
        return 60L;
    }

    public static long cc_equals()
    {
        return 61L;
    }

    public static long cc_greater()
    {
        return 62L;
    }

    public static long cc_upper_a()
    {
        return 65L;
    }

    public static long cc_upper_z()
    {
        return 90L;
    }

    public static long cc_left_bracket()
    {
        return 91L;
    }

    public static long cc_backslash()
    {
        return 92L;
    }

    public static long cc_right_bracket()
    {
        return 93L;
    }

    public static long cc_caret()
    {
        return 94L;
    }

    public static long cc_underscore()
    {
        return 95L;
    }

    public static long cc_lower_a()
    {
        return 97L;
    }

    public static long cc_lower_n()
    {
        return 110L;
    }

    public static long cc_lower_r()
    {
        return 114L;
    }

    public static long cc_lower_t()
    {
        return 116L;
    }

    public static long cc_lower_z()
    {
        return 122L;
    }

    public static long cc_left_brace()
    {
        return 123L;
    }

    public static long cc_pipe()
    {
        return 124L;
    }

    public static long cc_right_brace()
    {
        return 125L;
    }

    public static bool is_letter_code(long c)
    {
        return ((c >= cc_upper_a()) ? ((c <= cc_upper_z()) ? true : ((c >= cc_lower_a()) ? (c <= cc_lower_z()) : false)) : false);
    }

    public static bool is_digit_code(long c)
    {
        return ((c >= cc_zero()) ? (c <= cc_nine()) : false);
    }

    public static LexState make_lex_state(string src)
    {
        return new LexState(src, 0L, 1L, 1L);
    }

    public static bool is_at_end(LexState st)
    {
        return (st.offset >= ((long)st.source.Length));
    }

    public static long peek_code(LexState st)
    {
        return (is_at_end(st) ? 0L : ((long)st.source[(int)st.offset]));
    }

    public static LexState advance_char(LexState st)
    {
        return ((peek_code(st) == cc_newline()) ? new LexState(st.source, (st.offset + 1L), (st.line + 1L), 1L) : new LexState(st.source, (st.offset + 1L), st.line, (st.column + 1L)));
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
                var c = peek_code(st);
                if ((c == cc_space()))
                {
                    var _tco_0 = advance_char(st);
                    st = _tco_0;
                    continue;
                }
                else
                {
                    if ((c == cc_carriage_return()))
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
                var c = peek_code(st);
                if (is_letter_code(c))
                {
                    var _tco_0 = advance_char(st);
                    st = _tco_0;
                    continue;
                }
                else
                {
                    if (is_digit_code(c))
                    {
                        var _tco_0 = advance_char(st);
                        st = _tco_0;
                        continue;
                    }
                    else
                    {
                        if ((c == cc_underscore()))
                        {
                            var _tco_0 = advance_char(st);
                            st = _tco_0;
                            continue;
                        }
                        else
                        {
                            if ((c == cc_minus()))
                            {
                                var next = advance_char(st);
                                if (is_at_end(next))
                                {
                                    return st;
                                }
                                else
                                {
                                    if (is_letter_code(peek_code(next)))
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
                var c = peek_code(st);
                if (is_digit_code(c))
                {
                    var _tco_0 = advance_char(st);
                    st = _tco_0;
                    continue;
                }
                else
                {
                    if ((c == cc_underscore()))
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
                var c = peek_code(st);
                if ((c == cc_double_quote()))
                {
                    return advance_char(st);
                }
                else
                {
                    if ((c == cc_newline()))
                    {
                        return st;
                    }
                    else
                    {
                        if ((c == cc_backslash()))
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

    public static string process_escapes(string s, long i, long len, string acc)
    {
        while (true)
        {
            if ((i >= len))
            {
                return acc;
            }
            else
            {
                var c = ((long)s[(int)i]);
                if ((c == cc_backslash()))
                {
                    if (((i + 1L) < len))
                    {
                        var nc = ((long)s[(int)(i + 1L)]);
                        if ((nc == cc_lower_n()))
                        {
                            var _tco_0 = s;
                            var _tco_1 = (i + 2L);
                            var _tco_2 = len;
                            var _tco_3 = string.Concat(acc, ((char)10L).ToString());
                            s = _tco_0;
                            i = _tco_1;
                            len = _tco_2;
                            acc = _tco_3;
                            continue;
                        }
                        else
                        {
                            if ((nc == cc_lower_t()))
                            {
                                var _tco_0 = s;
                                var _tco_1 = (i + 2L);
                                var _tco_2 = len;
                                var _tco_3 = string.Concat(acc, ((char)9L).ToString());
                                s = _tco_0;
                                i = _tco_1;
                                len = _tco_2;
                                acc = _tco_3;
                                continue;
                            }
                            else
                            {
                                if ((nc == cc_lower_r()))
                                {
                                    var _tco_0 = s;
                                    var _tco_1 = (i + 2L);
                                    var _tco_2 = len;
                                    var _tco_3 = string.Concat(acc, ((char)13L).ToString());
                                    s = _tco_0;
                                    i = _tco_1;
                                    len = _tco_2;
                                    acc = _tco_3;
                                    continue;
                                }
                                else
                                {
                                    if ((nc == cc_backslash()))
                                    {
                                        var _tco_0 = s;
                                        var _tco_1 = (i + 2L);
                                        var _tco_2 = len;
                                        var _tco_3 = string.Concat(acc, "\\");
                                        s = _tco_0;
                                        i = _tco_1;
                                        len = _tco_2;
                                        acc = _tco_3;
                                        continue;
                                    }
                                    else
                                    {
                                        if ((nc == cc_double_quote()))
                                        {
                                            var _tco_0 = s;
                                            var _tco_1 = (i + 2L);
                                            var _tco_2 = len;
                                            var _tco_3 = string.Concat(acc, "\"");
                                            s = _tco_0;
                                            i = _tco_1;
                                            len = _tco_2;
                                            acc = _tco_3;
                                            continue;
                                        }
                                        else
                                        {
                                            var _tco_0 = s;
                                            var _tco_1 = (i + 2L);
                                            var _tco_2 = len;
                                            var _tco_3 = string.Concat(acc, s[(int)(i + 1L)].ToString());
                                            s = _tco_0;
                                            i = _tco_1;
                                            len = _tco_2;
                                            acc = _tco_3;
                                            continue;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        return string.Concat(acc, s[(int)i].ToString());
                    }
                }
                else
                {
                    var _tco_0 = s;
                    var _tco_1 = (i + 1L);
                    var _tco_2 = len;
                    var _tco_3 = string.Concat(acc, s[(int)i].ToString());
                    s = _tco_0;
                    i = _tco_1;
                    len = _tco_2;
                    acc = _tco_3;
                    continue;
                }
            }
        }
    }

    public static TokenKind classify_word(string w)
    {
        return ((w == "let") ? new LetKeyword() : ((w == "in") ? new InKeyword() : ((w == "if") ? new IfKeyword() : ((w == "then") ? new ThenKeyword() : ((w == "else") ? new ElseKeyword() : ((w == "when") ? new WhenKeyword() : ((w == "where") ? new WhereKeyword() : ((w == "do") ? new DoKeyword() : ((w == "record") ? new RecordKeyword() : ((w == "import") ? new ImportKeyword() : ((w == "export") ? new ExportKeyword() : ((w == "claim") ? new ClaimKeyword() : ((w == "proof") ? new ProofKeyword() : ((w == "forall") ? new ForAllKeyword() : ((w == "exists") ? new ThereExistsKeyword() : ((w == "linear") ? new LinearKeyword() : ((w == "effect") ? new EffectKeyword() : ((w == "with") ? new WithKeyword() : ((w == "True") ? new TrueKeyword() : ((w == "False") ? new FalseKeyword() : ((Func<long, TokenKind>)((first_code) => ((first_code >= cc_upper_a()) ? ((first_code <= cc_upper_z()) ? new TypeIdentifier() : new Identifier()) : new Identifier())))(((long)w[(int)0L]))))))))))))))))))))));
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
        return ((Func<LexState, LexResult>)((s) => (is_at_end(s) ? new LexEnd() : ((Func<long, LexResult>)((c) => ((c == cc_newline()) ? new LexToken(make_token(new Newline(), "\n", s), advance_char(s)) : ((c == cc_double_quote()) ? ((Func<long, LexResult>)((start) => ((Func<LexState, LexResult>)((after) => ((Func<long, LexResult>)((text_len) => ((Func<string, LexResult>)((raw) => new LexToken(make_token(new TextLiteral(), process_escapes(raw, 0L, ((long)raw.Length), ""), s), after)))(s.source.Substring((int)start, (int)text_len))))(((after.offset - start) - 1L))))(scan_string_body(advance_char(s)))))((s.offset + 1L)) : (is_letter_code(c) ? ((Func<long, LexResult>)((start) => ((Func<LexState, LexResult>)((after) => ((Func<string, LexResult>)((word) => new LexToken(make_token(classify_word(word), word, s), after)))(extract_text(s, start, after))))(scan_ident_rest(advance_char(s)))))(s.offset) : ((c == cc_underscore()) ? ((Func<long, LexResult>)((start) => ((Func<LexState, LexResult>)((after) => ((Func<string, LexResult>)((word) => ((((long)word.Length) == 1L) ? new LexToken(make_token(new Underscore(), "_", s), after) : new LexToken(make_token(classify_word(word), word, s), after))))(extract_text(s, start, after))))(scan_ident_rest(advance_char(s)))))(s.offset) : (is_digit_code(c) ? ((Func<long, LexResult>)((start) => ((Func<LexState, LexResult>)((after) => (is_at_end(after) ? new LexToken(make_token(new IntegerLiteral(), extract_text(s, start, after), s), after) : ((peek_code(after) == cc_dot()) ? ((Func<LexState, LexResult>)((after2) => new LexToken(make_token(new NumberLiteral(), extract_text(s, start, after2), s), after2)))(scan_digits(advance_char(after))) : new LexToken(make_token(new IntegerLiteral(), extract_text(s, start, after), s), after)))))(scan_digits(advance_char(s)))))(s.offset) : scan_operator(s))))))))(peek_code(s)))))(skip_spaces(st));
    }

    public static LexResult scan_operator(LexState s)
    {
        return ((Func<long, LexResult>)((c) => ((Func<LexState, LexResult>)((next) => ((c == cc_left_paren()) ? new LexToken(make_token(new LeftParen(), "(", s), next) : ((c == cc_right_paren()) ? new LexToken(make_token(new RightParen(), ")", s), next) : ((c == cc_left_bracket()) ? new LexToken(make_token(new LeftBracket(), "[", s), next) : ((c == cc_right_bracket()) ? new LexToken(make_token(new RightBracket(), "]", s), next) : ((c == cc_left_brace()) ? new LexToken(make_token(new LeftBrace(), "{", s), next) : ((c == cc_right_brace()) ? new LexToken(make_token(new RightBrace(), "}", s), next) : ((c == cc_comma()) ? new LexToken(make_token(new Comma(), ",", s), next) : ((c == cc_dot()) ? new LexToken(make_token(new Dot(), ".", s), next) : ((c == cc_caret()) ? new LexToken(make_token(new Caret(), "^", s), next) : ((c == cc_ampersand()) ? new LexToken(make_token(new Ampersand(), "&", s), next) : scan_multi_char_operator(s)))))))))))))(advance_char(s))))(peek_code(s));
    }

    public static LexResult scan_multi_char_operator(LexState s)
    {
        return ((Func<long, LexResult>)((c) => ((Func<LexState, LexResult>)((next) => ((Func<long, LexResult>)((nc) => ((c == cc_plus()) ? ((nc == cc_plus()) ? new LexToken(make_token(new PlusPlus(), "++", s), advance_char(next)) : new LexToken(make_token(new Plus(), "+", s), next)) : ((c == cc_minus()) ? ((nc == cc_greater()) ? new LexToken(make_token(new Arrow(), "->", s), advance_char(next)) : new LexToken(make_token(new Minus(), "-", s), next)) : ((c == cc_star()) ? new LexToken(make_token(new Star(), "*", s), next) : ((c == cc_slash()) ? ((nc == cc_equals()) ? new LexToken(make_token(new NotEquals(), "/=", s), advance_char(next)) : new LexToken(make_token(new Slash(), "/", s), next)) : ((c == cc_equals()) ? ((nc == cc_equals()) ? ((Func<LexState, LexResult>)((next2) => ((Func<long, LexResult>)((nc2) => ((nc2 == cc_equals()) ? new LexToken(make_token(new TripleEquals(), "===", s), advance_char(next2)) : new LexToken(make_token(new DoubleEquals(), "==", s), next2))))((is_at_end(next2) ? 0L : peek_code(next2)))))(advance_char(next)) : new LexToken(make_token(new Equals_(), "=", s), next)) : ((c == cc_colon()) ? ((nc == cc_colon()) ? new LexToken(make_token(new ColonColon(), "::", s), advance_char(next)) : new LexToken(make_token(new Colon(), ":", s), next)) : ((c == cc_pipe()) ? ((nc == cc_minus()) ? new LexToken(make_token(new Turnstile(), "|-", s), advance_char(next)) : new LexToken(make_token(new Pipe(), "|", s), next)) : ((c == cc_less()) ? ((nc == cc_equals()) ? new LexToken(make_token(new LessOrEqual(), "<=", s), advance_char(next)) : ((nc == cc_minus()) ? new LexToken(make_token(new LeftArrow(), "<-", s), advance_char(next)) : new LexToken(make_token(new LessThan(), "<", s), next))) : ((c == cc_greater()) ? ((nc == cc_equals()) ? new LexToken(make_token(new GreaterOrEqual(), ">=", s), advance_char(next)) : new LexToken(make_token(new GreaterThan(), ">", s), next)) : new LexToken(make_token(new ErrorToken(), s.source[(int)s.offset].ToString(), s), next))))))))))))((is_at_end(next) ? 0L : peek_code(next)))))(advance_char(s))))(peek_code(s));
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

    public static ParseTypeResult parse_type(ParseState st)
    {
        return ((Func<ParseTypeResult, ParseTypeResult>)((result) => unwrap_type_ok(result, (left) => (st) => parse_type_continue(left, st))))(parse_type_atom(st));
    }

    public static ParseTypeResult parse_type_continue(TypeExpr left, ParseState st)
    {
        return (is_arrow(current_kind(st)) ? ((Func<ParseState, ParseTypeResult>)((st2) => ((Func<ParseTypeResult, ParseTypeResult>)((right_result) => unwrap_type_ok(right_result, (right) => (st) => make_fun_type(left, right, st))))(parse_type(st2))))(advance(st)) : new TypeOk(left, st));
    }

    public static ParseTypeResult make_fun_type(TypeExpr left, TypeExpr right, ParseState st)
    {
        return new TypeOk(new FunType(left, right), st);
    }

    public static ParseTypeResult unwrap_type_ok(ParseTypeResult r, Func<TypeExpr, Func<ParseState, ParseTypeResult>> f)
    {
        return (r is TypeOk _mTypeOk51_ ? ((Func<ParseState, ParseTypeResult>)((st) => ((Func<TypeExpr, ParseTypeResult>)((t) => f(t)(st)))((TypeExpr)_mTypeOk51_.Field0)))((ParseState)_mTypeOk51_.Field1) : throw new InvalidOperationException("Non-exhaustive match"));
    }

    public static ParseTypeResult parse_type_atom(ParseState st)
    {
        return (is_ident(current_kind(st)) ? ((Func<Token, ParseTypeResult>)((tok) => parse_type_args(new NamedType(tok), advance(st))))(current(st)) : (is_type_ident(current_kind(st)) ? ((Func<Token, ParseTypeResult>)((tok) => parse_type_args(new NamedType(tok), advance(st))))(current(st)) : (is_left_paren(current_kind(st)) ? parse_paren_type(advance(st)) : (is_left_bracket(current_kind(st)) ? parse_effect_type(advance(st)) : ((Func<Token, ParseTypeResult>)((tok) => new TypeOk(new NamedType(tok), advance(st))))(current(st))))));
    }

    public static ParseTypeResult parse_paren_type(ParseState st)
    {
        return ((Func<ParseTypeResult, ParseTypeResult>)((inner) => unwrap_type_ok(inner, (t) => (st) => finish_paren_type(t, st))))(parse_type(st));
    }

    public static ParseTypeResult finish_paren_type(TypeExpr t, ParseState st)
    {
        return ((Func<ParseState, ParseTypeResult>)((st2) => new TypeOk(new ParenType(t), st2)))(expect(new RightParen(), st));
    }

    public static ParseTypeResult parse_effect_type(ParseState st)
    {
        return ((Func<ParseState, ParseTypeResult>)((st2) => parse_type(st2)))(skip_effect_contents(st));
    }

    public static ParseState skip_effect_contents(ParseState st)
    {
        while (true)
        {
            if (is_done(st))
            {
                return st;
            }
            else
            {
                if (is_right_bracket(current_kind(st)))
                {
                    return advance(st);
                }
                else
                {
                    var _tco_0 = advance(st);
                    st = _tco_0;
                    continue;
                }
            }
        }
    }

    public static ParseTypeResult parse_type_args(TypeExpr base_type, ParseState st)
    {
        return (is_done(st) ? new TypeOk(base_type, st) : (is_type_arg_start(current_kind(st)) ? parse_type_arg_next(base_type, st) : new TypeOk(base_type, st)));
    }

    public static ParseTypeResult parse_type_arg_next(TypeExpr base_type, ParseState st)
    {
        return ((Func<ParseTypeResult, ParseTypeResult>)((arg_result) => unwrap_type_ok(arg_result, (arg) => (st) => continue_type_args(base_type, arg, st))))(parse_type_atom(st));
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
        return (is_left_paren(current_kind(st)) ? ((Func<ParseState, ParsePatResult>)((st2) => ((Func<ParsePatResult, ParsePatResult>)((sub) => unwrap_pat_ok(sub, (p) => (st) => continue_ctor_fields(ctor, acc, p, st))))(parse_pattern(st2))))(advance(st)) : new PatOk(new CtorPat(ctor, acc), st));
    }

    public static ParsePatResult continue_ctor_fields(Token ctor, List<Pat> acc, Pat p, ParseState st)
    {
        return ((Func<ParseState, ParsePatResult>)((st2) => parse_ctor_pattern_fields(ctor, Enumerable.Concat(acc, new List<Pat>() { p }).ToList(), st2)))(expect(new RightParen(), st));
    }

    public static ParsePatResult unwrap_pat_ok(ParsePatResult r, Func<Pat, Func<ParseState, ParsePatResult>> f)
    {
        return (r is PatOk _mPatOk52_ ? ((Func<ParseState, ParsePatResult>)((st) => ((Func<Pat, ParsePatResult>)((p) => f(p)(st)))((Pat)_mPatOk52_.Field0)))((ParseState)_mPatOk52_.Field1) : throw new InvalidOperationException("Non-exhaustive match"));
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
        return (r is TypeOk _mTypeOk53_ ? ((Func<ParseState, ParseDefResult>)((st) => ((Func<TypeExpr, ParseDefResult>)((ann_type) => ((Func<Token, ParseDefResult>)((name_tok) => ((Func<List<TypeAnn>, ParseDefResult>)((ann) => ((Func<ParseState, ParseDefResult>)((st2) => parse_def_body_with_ann(ann, st2)))(skip_newlines(st))))(new List<TypeAnn>() { new TypeAnn(name_tok, ann_type) })))(new Token(new Identifier(), "", 0L, 0L, 0L))))((TypeExpr)_mTypeOk53_.Field0)))((ParseState)_mTypeOk53_.Field1) : throw new InvalidOperationException("Non-exhaustive match"));
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
        return (r is ExprOk _mExprOk54_ ? ((Func<ParseState, ParseDefResult>)((st) => ((Func<Expr, ParseDefResult>)((b) => new DefOk(new Def(name_tok, @params, ann, b), st)))((Expr)_mExprOk54_.Field0)))((ParseState)_mExprOk54_.Field1) : throw new InvalidOperationException("Non-exhaustive match"));
    }

    public static bool is_paren_type_param(ParseState st)
    {
        return (is_left_paren(current_kind(st)) ? ((Func<TokenKind, bool>)((k1) => (is_ident(k1) ? is_right_paren(peek_kind(st, 2L)) : (is_type_ident(k1) ? is_right_paren(peek_kind(st, 2L)) : false))))(peek_kind(st, 1L)) : false);
    }

    public static bool is_type_param_pattern(ParseState st)
    {
        return (is_paren_type_param(st) ? true : is_ident(current_kind(st)));
    }

    public static ParseState parse_type_params(ParseState st, List<Token> acc)
    {
        while (true)
        {
            if (is_paren_type_param(st))
            {
                var _tco_0 = advance(advance(advance(st)));
                var _tco_1 = Enumerable.Concat(acc, new List<Token>() { st.tokens[(int)(st.pos + 1L)] }).ToList();
                st = _tco_0;
                acc = _tco_1;
                continue;
            }
            else
            {
                if (is_ident(current_kind(st)))
                {
                    var _tco_0 = advance(st);
                    var _tco_1 = Enumerable.Concat(acc, new List<Token>() { current(st) }).ToList();
                    st = _tco_0;
                    acc = _tco_1;
                    continue;
                }
                else
                {
                    return st;
                }
            }
        }
    }

    public static List<Token> collect_type_params(ParseState st, List<Token> acc)
    {
        while (true)
        {
            if (is_paren_type_param(st))
            {
                var _tco_0 = advance(advance(advance(st)));
                var _tco_1 = Enumerable.Concat(acc, new List<Token>() { st.tokens[(int)(st.pos + 1L)] }).ToList();
                st = _tco_0;
                acc = _tco_1;
                continue;
            }
            else
            {
                if (is_ident(current_kind(st)))
                {
                    var _tco_0 = advance(st);
                    var _tco_1 = Enumerable.Concat(acc, new List<Token>() { current(st) }).ToList();
                    st = _tco_0;
                    acc = _tco_1;
                    continue;
                }
                else
                {
                    return acc;
                }
            }
        }
    }

    public static ParseTypeDefResult parse_type_def(ParseState st)
    {
        return (is_type_ident(current_kind(st)) ? ((Func<Token, ParseTypeDefResult>)((name_tok) => ((Func<ParseState, ParseTypeDefResult>)((st2) => ((Func<List<Token>, ParseTypeDefResult>)((tparams) => ((Func<ParseState, ParseTypeDefResult>)((st3) => (is_equals(current_kind(st3)) ? ((Func<ParseState, ParseTypeDefResult>)((st4) => (is_record_keyword(current_kind(st4)) ? parse_record_type(name_tok, tparams, st4) : (is_pipe(current_kind(st4)) ? parse_variant_type(name_tok, tparams, st4) : new TypeDefNone(st)))))(skip_newlines(advance(st3))) : new TypeDefNone(st))))(parse_type_params(st2, new List<Token>()))))(collect_type_params(st2, new List<Token>()))))(advance(st))))(current(st)) : new TypeDefNone(st));
    }

    public static ParseTypeDefResult parse_record_type(Token name_tok, List<Token> tparams, ParseState st)
    {
        return ((Func<ParseState, ParseTypeDefResult>)((st2) => ((Func<ParseState, ParseTypeDefResult>)((st3) => ((Func<ParseState, ParseTypeDefResult>)((st4) => parse_record_fields_loop(name_tok, tparams, new List<RecordFieldDef>(), st4)))(skip_newlines(st3))))(expect(new LeftBrace(), st2))))(advance(st));
    }

    public static ParseTypeDefResult parse_record_fields_loop(Token name_tok, List<Token> tparams, List<RecordFieldDef> acc, ParseState st)
    {
        return (is_right_brace(current_kind(st)) ? new TypeDefOk(new TypeDef(name_tok, tparams, new RecordBody(acc)), advance(st)) : (is_ident(current_kind(st)) ? parse_one_record_field(name_tok, tparams, acc, st) : new TypeDefOk(new TypeDef(name_tok, tparams, new RecordBody(acc)), st)));
    }

    public static ParseTypeDefResult parse_one_record_field(Token name_tok, List<Token> tparams, List<RecordFieldDef> acc, ParseState st)
    {
        return ((Func<Token, ParseTypeDefResult>)((field_name) => ((Func<ParseState, ParseTypeDefResult>)((st2) => ((Func<ParseState, ParseTypeDefResult>)((st3) => ((Func<ParseTypeResult, ParseTypeDefResult>)((field_type_result) => unwrap_record_field_type(name_tok, tparams, acc, field_name, field_type_result)))(parse_type(st3))))(expect(new Colon(), st2))))(advance(st))))(current(st));
    }

    public static ParseTypeDefResult unwrap_record_field_type(Token name_tok, List<Token> tparams, List<RecordFieldDef> acc, Token field_name, ParseTypeResult r)
    {
        return (r is TypeOk _mTypeOk55_ ? ((Func<ParseState, ParseTypeDefResult>)((st) => ((Func<TypeExpr, ParseTypeDefResult>)((ft) => ((Func<RecordFieldDef, ParseTypeDefResult>)((field) => ((Func<ParseState, ParseTypeDefResult>)((st2) => (is_comma(current_kind(st2)) ? parse_record_fields_loop(name_tok, tparams, Enumerable.Concat(acc, new List<RecordFieldDef>() { field }).ToList(), skip_newlines(advance(st2))) : parse_record_fields_loop(name_tok, tparams, Enumerable.Concat(acc, new List<RecordFieldDef>() { field }).ToList(), st2))))(skip_newlines(st))))(new RecordFieldDef(field_name, ft))))((TypeExpr)_mTypeOk55_.Field0)))((ParseState)_mTypeOk55_.Field1) : throw new InvalidOperationException("Non-exhaustive match"));
    }

    public static ParseTypeDefResult parse_variant_type(Token name_tok, List<Token> tparams, ParseState st)
    {
        return parse_variant_ctors(name_tok, tparams, new List<VariantCtorDef>(), st);
    }

    public static ParseTypeDefResult parse_variant_ctors(Token name_tok, List<Token> tparams, List<VariantCtorDef> acc, ParseState st)
    {
        return (is_pipe(current_kind(st)) ? ((Func<ParseState, ParseTypeDefResult>)((st2) => ((Func<Token, ParseTypeDefResult>)((ctor_name) => ((Func<ParseState, ParseTypeDefResult>)((st3) => parse_ctor_fields(ctor_name, new List<TypeExpr>(), st3, name_tok, tparams, acc)))(advance(st2))))(current(st2))))(skip_newlines(advance(st))) : new TypeDefOk(new TypeDef(name_tok, tparams, new VariantBody(acc)), st));
    }

    public static ParseTypeDefResult parse_ctor_fields(Token ctor_name, List<TypeExpr> fields, ParseState st, Token name_tok, List<Token> tparams, List<VariantCtorDef> acc)
    {
        return (is_left_paren(current_kind(st)) ? ((Func<ParseTypeResult, ParseTypeDefResult>)((field_result) => unwrap_ctor_field(field_result, ctor_name, fields, name_tok, tparams, acc)))(parse_type(advance(st))) : ((Func<ParseState, ParseTypeDefResult>)((st2) => ((Func<VariantCtorDef, ParseTypeDefResult>)((ctor) => parse_variant_ctors(name_tok, tparams, Enumerable.Concat(acc, new List<VariantCtorDef>() { ctor }).ToList(), st2)))(new VariantCtorDef(ctor_name, fields))))(skip_newlines(st)));
    }

    public static ParseTypeDefResult unwrap_ctor_field(ParseTypeResult r, Token ctor_name, List<TypeExpr> fields, Token name_tok, List<Token> tparams, List<VariantCtorDef> acc)
    {
        return (r is TypeOk _mTypeOk56_ ? ((Func<ParseState, ParseTypeDefResult>)((st) => ((Func<TypeExpr, ParseTypeDefResult>)((ty) => ((Func<ParseState, ParseTypeDefResult>)((st2) => parse_ctor_fields(ctor_name, Enumerable.Concat(fields, new List<TypeExpr>() { ty }).ToList(), st2, name_tok, tparams, acc)))(expect(new RightParen(), st))))((TypeExpr)_mTypeOk56_.Field0)))((ParseState)_mTypeOk56_.Field1) : throw new InvalidOperationException("Non-exhaustive match"));
    }

    public static Document parse_document(ParseState st)
    {
        return ((Func<ParseState, Document>)((st2) => ((Func<ImportParseResult, Document>)((imp_result) => parse_top_level(new List<Def>(), new List<TypeDef>(), new List<EffectDef>(), imp_result.imports, imp_result.state)))(parse_imports(st2, new List<ImportDecl>()))))(skip_newlines(st));
    }

    public static ImportParseResult parse_imports(ParseState st, List<ImportDecl> acc)
    {
        while (true)
        {
            if (is_import_keyword(current_kind(st)))
            {
                var st2 = advance(st);
                var name_tok = current(st2);
                var st3 = skip_newlines(advance(st2));
                var _tco_0 = st3;
                var _tco_1 = Enumerable.Concat(acc, new List<ImportDecl>() { new ImportDecl(name_tok) }).ToList();
                st = _tco_0;
                acc = _tco_1;
                continue;
            }
            else
            {
                return new ImportParseResult(acc, st);
            }
        }
    }

    public static Document parse_top_level(List<Def> defs, List<TypeDef> type_defs, List<EffectDef> effect_defs, List<ImportDecl> imports, ParseState st)
    {
        return (is_done(st) ? new Document(defs, type_defs, effect_defs, imports) : (is_effect_keyword(current_kind(st)) ? parse_top_level_effect(defs, type_defs, effect_defs, imports, st) : try_top_level_type_def(defs, type_defs, effect_defs, imports, st)));
    }

    public static Document parse_top_level_effect(List<Def> defs, List<TypeDef> type_defs, List<EffectDef> effect_defs, List<ImportDecl> imports, ParseState st)
    {
        return ((Func<ParseState, Document>)((st1) => ((Func<Token, Document>)((name_tok) => ((Func<ParseState, Document>)((st2) => ((Func<ParseState, Document>)((st3) => ((Func<EffectOpsResult, Document>)((ops) => ((Func<EffectDef, Document>)((ed) => parse_top_level(defs, type_defs, Enumerable.Concat(effect_defs, new List<EffectDef>() { ed }).ToList(), imports, skip_newlines(ops.state))))(new EffectDef(name_tok, ops.ops))))(parse_effect_ops(st3, new List<EffectOpDef>()))))((is_where_keyword(current_kind(st2)) ? skip_newlines(advance(st2)) : st2))))(advance(st1))))(current(st1))))(advance(st));
    }

    public static EffectOpsResult parse_effect_ops(ParseState st, List<EffectOpDef> acc)
    {
        while (true)
        {
            if (is_ident(current_kind(st)))
            {
                if (is_colon(peek_kind(st, 1L)))
                {
                    var op_tok = current(st);
                    var st2 = advance(advance(st));
                    var type_result = parse_type(st2);
                    var _tco_s = type_result;
                    if (_tco_s is TypeOk _tco_m0)
                    {
                        var ty = _tco_m0.Field0;
                        var st3 = _tco_m0.Field1;
                        var op = new EffectOpDef(op_tok, ty);
                        var _tco_0 = skip_newlines(st3);
                        var _tco_1 = Enumerable.Concat(acc, new List<EffectOpDef>() { op }).ToList();
                        st = _tco_0;
                        acc = _tco_1;
                        continue;
                    }
                }
                else
                {
                    return new EffectOpsResult(acc, st);
                }
            }
            else
            {
                return new EffectOpsResult(acc, st);
            }
        }
    }

    public static Document try_top_level_type_def(List<Def> defs, List<TypeDef> type_defs, List<EffectDef> effect_defs, List<ImportDecl> imports, ParseState st)
    {
        return ((Func<ParseTypeDefResult, Document>)((td_result) => ((Func<ParseTypeDefResult, Document>)((_scrutinee57_) => (_scrutinee57_ is TypeDefOk _mTypeDefOk57_ ? ((Func<ParseState, Document>)((st2) => ((Func<TypeDef, Document>)((td) => parse_top_level(defs, Enumerable.Concat(type_defs, new List<TypeDef>() { td }).ToList(), effect_defs, imports, skip_newlines(st2))))((TypeDef)_mTypeDefOk57_.Field0)))((ParseState)_mTypeDefOk57_.Field1) : (_scrutinee57_ is TypeDefNone _mTypeDefNone57_ ? ((Func<ParseState, Document>)((st2) => try_top_level_def(defs, type_defs, effect_defs, imports, st)))((ParseState)_mTypeDefNone57_.Field0) : throw new InvalidOperationException("Non-exhaustive match")))))(td_result)))(parse_type_def(st));
    }

    public static Document try_top_level_def(List<Def> defs, List<TypeDef> type_defs, List<EffectDef> effect_defs, List<ImportDecl> imports, ParseState st)
    {
        return ((Func<ParseDefResult, Document>)((def_result) => ((Func<ParseDefResult, Document>)((_scrutinee58_) => (_scrutinee58_ is DefOk _mDefOk58_ ? ((Func<ParseState, Document>)((st2) => ((Func<Def, Document>)((d) => parse_top_level(Enumerable.Concat(defs, new List<Def>() { d }).ToList(), type_defs, effect_defs, imports, skip_newlines(st2))))((Def)_mDefOk58_.Field0)))((ParseState)_mDefOk58_.Field1) : (_scrutinee58_ is DefNone _mDefNone58_ ? ((Func<ParseState, Document>)((st2) => parse_top_level(defs, type_defs, effect_defs, imports, skip_newlines(advance(st2)))))((ParseState)_mDefNone58_.Field0) : throw new InvalidOperationException("Non-exhaustive match")))))(def_result)))(parse_definition(st));
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
        return ((st.pos >= (((long)st.tokens.Count) - 1L)) ? st : new ParseState(st.tokens, (st.pos + 1L)));
    }

    public static bool is_done(ParseState st)
    {
        return (current_kind(st) is EndOfFile _mEndOfFile59_ ? true : ((Func<TokenKind, bool>)((_) => false))(current_kind(st)));
    }

    public static TokenKind peek_kind(ParseState st, long offset)
    {
        return ((Func<long, TokenKind>)((idx) => ((idx >= ((long)st.tokens.Count)) ? new EndOfFile() : st.tokens[(int)idx].kind)))((st.pos + offset));
    }

    public static bool is_ident(TokenKind k)
    {
        return (k is Identifier _mIdentifier60_ ? true : ((Func<TokenKind, bool>)((_) => false))(k));
    }

    public static bool is_type_ident(TokenKind k)
    {
        return (k is TypeIdentifier _mTypeIdentifier61_ ? true : ((Func<TokenKind, bool>)((_) => false))(k));
    }

    public static bool is_arrow(TokenKind k)
    {
        return (k is Arrow _mArrow62_ ? true : ((Func<TokenKind, bool>)((_) => false))(k));
    }

    public static bool is_equals(TokenKind k)
    {
        return (k is Equals_ _mEquals_63_ ? true : ((Func<TokenKind, bool>)((_) => false))(k));
    }

    public static bool is_colon(TokenKind k)
    {
        return (k is Colon _mColon64_ ? true : ((Func<TokenKind, bool>)((_) => false))(k));
    }

    public static bool is_comma(TokenKind k)
    {
        return (k is Comma _mComma65_ ? true : ((Func<TokenKind, bool>)((_) => false))(k));
    }

    public static bool is_pipe(TokenKind k)
    {
        return (k is Pipe _mPipe66_ ? true : ((Func<TokenKind, bool>)((_) => false))(k));
    }

    public static bool is_dot(TokenKind k)
    {
        return (k is Dot _mDot67_ ? true : ((Func<TokenKind, bool>)((_) => false))(k));
    }

    public static bool is_left_paren(TokenKind k)
    {
        return (k is LeftParen _mLeftParen68_ ? true : ((Func<TokenKind, bool>)((_) => false))(k));
    }

    public static bool is_left_brace(TokenKind k)
    {
        return (k is LeftBrace _mLeftBrace69_ ? true : ((Func<TokenKind, bool>)((_) => false))(k));
    }

    public static bool is_left_bracket(TokenKind k)
    {
        return (k is LeftBracket _mLeftBracket70_ ? true : ((Func<TokenKind, bool>)((_) => false))(k));
    }

    public static bool is_right_brace(TokenKind k)
    {
        return (k is RightBrace _mRightBrace71_ ? true : ((Func<TokenKind, bool>)((_) => false))(k));
    }

    public static bool is_right_bracket(TokenKind k)
    {
        return (k is RightBracket _mRightBracket72_ ? true : ((Func<TokenKind, bool>)((_) => false))(k));
    }

    public static bool is_right_paren(TokenKind k)
    {
        return (k is RightParen _mRightParen73_ ? true : ((Func<TokenKind, bool>)((_) => false))(k));
    }

    public static bool is_if_keyword(TokenKind k)
    {
        return (k is IfKeyword _mIfKeyword74_ ? true : ((Func<TokenKind, bool>)((_) => false))(k));
    }

    public static bool is_let_keyword(TokenKind k)
    {
        return (k is LetKeyword _mLetKeyword75_ ? true : ((Func<TokenKind, bool>)((_) => false))(k));
    }

    public static bool is_when_keyword(TokenKind k)
    {
        return (k is WhenKeyword _mWhenKeyword76_ ? true : ((Func<TokenKind, bool>)((_) => false))(k));
    }

    public static bool is_do_keyword(TokenKind k)
    {
        return (k is DoKeyword _mDoKeyword77_ ? true : ((Func<TokenKind, bool>)((_) => false))(k));
    }

    public static bool is_with_keyword(TokenKind k)
    {
        return (k is WithKeyword _mWithKeyword78_ ? true : ((Func<TokenKind, bool>)((_) => false))(k));
    }

    public static bool is_effect_keyword(TokenKind k)
    {
        return (k is EffectKeyword _mEffectKeyword79_ ? true : ((Func<TokenKind, bool>)((_) => false))(k));
    }

    public static bool is_import_keyword(TokenKind k)
    {
        return (k is ImportKeyword _mImportKeyword80_ ? true : ((Func<TokenKind, bool>)((_) => false))(k));
    }

    public static bool is_where_keyword(TokenKind k)
    {
        return (k is WhereKeyword _mWhereKeyword81_ ? true : ((Func<TokenKind, bool>)((_) => false))(k));
    }

    public static bool is_in_keyword(TokenKind k)
    {
        return (k is InKeyword _mInKeyword82_ ? true : ((Func<TokenKind, bool>)((_) => false))(k));
    }

    public static bool is_minus(TokenKind k)
    {
        return (k is Minus _mMinus83_ ? true : ((Func<TokenKind, bool>)((_) => false))(k));
    }

    public static bool is_dedent(TokenKind k)
    {
        return (k is Dedent _mDedent84_ ? true : ((Func<TokenKind, bool>)((_) => false))(k));
    }

    public static bool is_left_arrow(TokenKind k)
    {
        return (k is LeftArrow _mLeftArrow85_ ? true : ((Func<TokenKind, bool>)((_) => false))(k));
    }

    public static bool is_record_keyword(TokenKind k)
    {
        return (k is RecordKeyword _mRecordKeyword86_ ? true : ((Func<TokenKind, bool>)((_) => false))(k));
    }

    public static bool is_underscore(TokenKind k)
    {
        return (k is Underscore _mUnderscore87_ ? true : ((Func<TokenKind, bool>)((_) => false))(k));
    }

    public static bool is_literal(TokenKind k)
    {
        return ((Func<TokenKind, bool>)((_scrutinee88_) => (_scrutinee88_ is IntegerLiteral _mIntegerLiteral88_ ? true : (_scrutinee88_ is NumberLiteral _mNumberLiteral88_ ? true : (_scrutinee88_ is TextLiteral _mTextLiteral88_ ? true : (_scrutinee88_ is TrueKeyword _mTrueKeyword88_ ? true : (_scrutinee88_ is FalseKeyword _mFalseKeyword88_ ? true : ((Func<TokenKind, bool>)((_) => false))(_scrutinee88_))))))))(k);
    }

    public static bool is_app_start(TokenKind k)
    {
        return ((Func<TokenKind, bool>)((_scrutinee89_) => (_scrutinee89_ is Identifier _mIdentifier89_ ? true : (_scrutinee89_ is TypeIdentifier _mTypeIdentifier89_ ? true : (_scrutinee89_ is IntegerLiteral _mIntegerLiteral89_ ? true : (_scrutinee89_ is NumberLiteral _mNumberLiteral89_ ? true : (_scrutinee89_ is TextLiteral _mTextLiteral89_ ? true : (_scrutinee89_ is TrueKeyword _mTrueKeyword89_ ? true : (_scrutinee89_ is FalseKeyword _mFalseKeyword89_ ? true : (_scrutinee89_ is LeftParen _mLeftParen89_ ? true : (_scrutinee89_ is LeftBracket _mLeftBracket89_ ? true : ((Func<TokenKind, bool>)((_) => false))(_scrutinee89_))))))))))))(k);
    }

    public static bool is_compound(Expr e)
    {
        return ((Func<Expr, bool>)((_scrutinee90_) => (_scrutinee90_ is MatchExpr _mMatchExpr90_ ? ((Func<List<MatchArm>, bool>)((arms) => ((Func<Expr, bool>)((s) => true))((Expr)_mMatchExpr90_.Field0)))((List<MatchArm>)_mMatchExpr90_.Field1) : (_scrutinee90_ is IfExpr _mIfExpr90_ ? ((Func<Expr, bool>)((el) => ((Func<Expr, bool>)((t) => ((Func<Expr, bool>)((c) => true))((Expr)_mIfExpr90_.Field0)))((Expr)_mIfExpr90_.Field1)))((Expr)_mIfExpr90_.Field2) : (_scrutinee90_ is LetExpr _mLetExpr90_ ? ((Func<Expr, bool>)((body) => ((Func<List<LetBind>, bool>)((binds) => true))((List<LetBind>)_mLetExpr90_.Field0)))((Expr)_mLetExpr90_.Field1) : (_scrutinee90_ is DoExpr _mDoExpr90_ ? ((Func<List<DoStmt>, bool>)((stmts) => true))((List<DoStmt>)_mDoExpr90_.Field0) : ((Func<Expr, bool>)((_) => false))(_scrutinee90_)))))))(e);
    }

    public static bool is_type_arg_start(TokenKind k)
    {
        return ((Func<TokenKind, bool>)((_scrutinee91_) => (_scrutinee91_ is TypeIdentifier _mTypeIdentifier91_ ? true : (_scrutinee91_ is Identifier _mIdentifier91_ ? true : (_scrutinee91_ is LeftParen _mLeftParen91_ ? true : ((Func<TokenKind, bool>)((_) => false))(_scrutinee91_))))))(k);
    }

    public static long operator_precedence(TokenKind k)
    {
        return ((Func<TokenKind, long>)((_scrutinee92_) => (_scrutinee92_ is PlusPlus _mPlusPlus92_ ? 5L : (_scrutinee92_ is ColonColon _mColonColon92_ ? 5L : (_scrutinee92_ is Plus _mPlus92_ ? 6L : (_scrutinee92_ is Minus _mMinus92_ ? 6L : (_scrutinee92_ is Star _mStar92_ ? 7L : (_scrutinee92_ is Slash _mSlash92_ ? 7L : (_scrutinee92_ is Caret _mCaret92_ ? 8L : (_scrutinee92_ is DoubleEquals _mDoubleEquals92_ ? 4L : (_scrutinee92_ is NotEquals _mNotEquals92_ ? 4L : (_scrutinee92_ is LessThan _mLessThan92_ ? 4L : (_scrutinee92_ is GreaterThan _mGreaterThan92_ ? 4L : (_scrutinee92_ is LessOrEqual _mLessOrEqual92_ ? 4L : (_scrutinee92_ is GreaterOrEqual _mGreaterOrEqual92_ ? 4L : (_scrutinee92_ is TripleEquals _mTripleEquals92_ ? 4L : (_scrutinee92_ is Ampersand _mAmpersand92_ ? 3L : (_scrutinee92_ is Pipe _mPipe92_ ? 2L : ((Func<TokenKind, long>)((_) => (0L - 1L)))(_scrutinee92_)))))))))))))))))))(k);
    }

    public static bool is_right_assoc(TokenKind k)
    {
        return ((Func<TokenKind, bool>)((_scrutinee93_) => (_scrutinee93_ is PlusPlus _mPlusPlus93_ ? true : (_scrutinee93_ is ColonColon _mColonColon93_ ? true : (_scrutinee93_ is Caret _mCaret93_ ? true : (_scrutinee93_ is Arrow _mArrow93_ ? true : ((Func<TokenKind, bool>)((_) => false))(_scrutinee93_)))))))(k);
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

    public static ParseExprResult parse_expr(ParseState st)
    {
        return parse_binary(st, 0L);
    }

    public static ParseExprResult unwrap_expr_ok(ParseExprResult r, Func<Expr, Func<ParseState, ParseExprResult>> f)
    {
        return (r is ExprOk _mExprOk94_ ? ((Func<ParseState, ParseExprResult>)((st) => ((Func<Expr, ParseExprResult>)((e) => f(e)(st)))((Expr)_mExprOk94_.Field0)))((ParseState)_mExprOk94_.Field1) : throw new InvalidOperationException("Non-exhaustive match"));
    }

    public static ParseExprResult parse_binary(ParseState st, long min_prec)
    {
        return ((Func<ParseExprResult, ParseExprResult>)((left_result) => unwrap_expr_ok(left_result, (left) => (st) => start_binary_loop(min_prec, left, st))))(parse_unary(st));
    }

    public static ParseExprResult start_binary_loop(long min_prec, Expr left, ParseState st)
    {
        return parse_binary_loop(left, st, min_prec);
    }

    public static ParseExprResult parse_binary_loop(Expr left, ParseState st, long min_prec)
    {
        return (is_done(st) ? new ExprOk(left, st) : ((Func<long, ParseExprResult>)((prec) => ((prec < min_prec) ? new ExprOk(left, st) : ((Func<Token, ParseExprResult>)((op) => ((Func<ParseState, ParseExprResult>)((st2) => ((Func<long, ParseExprResult>)((next_min) => ((Func<ParseExprResult, ParseExprResult>)((right_result) => unwrap_expr_ok(right_result, (right) => (st) => continue_binary(left, op, min_prec, right, st))))(parse_binary(st2, next_min))))((is_right_assoc(op.kind) ? prec : (prec + 1L)))))(skip_newlines(advance(st)))))(current(st)))))(operator_precedence(current_kind(st))));
    }

    public static ParseExprResult continue_binary(Expr left, Token op, long min_prec, Expr right, ParseState st)
    {
        return parse_binary_loop(new BinExpr(left, op, right), st, min_prec);
    }

    public static ParseExprResult parse_unary(ParseState st)
    {
        return (is_minus(current_kind(st)) ? ((Func<Token, ParseExprResult>)((op) => ((Func<ParseExprResult, ParseExprResult>)((result) => unwrap_expr_ok(result, (operand) => (st) => finish_unary(op, operand, st))))(parse_unary(advance(st)))))(current(st)) : parse_application(st));
    }

    public static ParseExprResult finish_unary(Token op, Expr operand, ParseState st)
    {
        return new ExprOk(new UnaryExpr(op, operand), st);
    }

    public static ParseExprResult parse_application(ParseState st)
    {
        return ((Func<ParseExprResult, ParseExprResult>)((func_result) => unwrap_expr_ok(func_result, (func) => (st) => parse_app_loop(func, st))))(parse_atom(st));
    }

    public static ParseExprResult parse_app_loop(Expr func, ParseState st)
    {
        return (is_compound(func) ? parse_dot_only(func, st) : (is_done(st) ? new ExprOk(func, st) : (is_app_start(current_kind(st)) ? ((Func<ParseExprResult, ParseExprResult>)((arg_result) => unwrap_expr_ok(arg_result, (arg) => (st) => continue_app(func, arg, st))))(parse_atom(st)) : parse_field_access(func, st))));
    }

    public static ParseExprResult continue_app(Expr func, Expr arg, ParseState st)
    {
        return parse_app_loop(new AppExpr(func, arg), st);
    }

    public static ParseExprResult parse_atom(ParseState st)
    {
        return (is_literal(current_kind(st)) ? new ExprOk(new LitExpr(current(st)), advance(st)) : (is_ident(current_kind(st)) ? parse_field_access(new NameExpr(current(st)), advance(st)) : (is_type_ident(current_kind(st)) ? parse_atom_type_ident(st) : (is_left_paren(current_kind(st)) ? parse_paren_expr(advance(st)) : (is_left_bracket(current_kind(st)) ? parse_list_expr(st) : (is_if_keyword(current_kind(st)) ? parse_if_expr(st) : (is_let_keyword(current_kind(st)) ? parse_let_expr(st) : (is_when_keyword(current_kind(st)) ? parse_match_expr(st) : (is_do_keyword(current_kind(st)) ? parse_do_expr(st) : (is_with_keyword(current_kind(st)) ? parse_handle_expr(st) : new ExprOk(new ErrExpr(current(st)), advance(st))))))))))));
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
                return new ExprOk(node, st);
            }
        }
    }

    public static ParseExprResult parse_dot_only(Expr node, ParseState st)
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
                return new ExprOk(node, st);
            }
        }
    }

    public static ParseExprResult parse_atom_type_ident(ParseState st)
    {
        return ((Func<Token, ParseExprResult>)((tok) => ((Func<ParseState, ParseExprResult>)((st2) => (is_left_brace(current_kind(st2)) ? parse_record_expr(tok, st2) : new ExprOk(new NameExpr(tok), st2))))(advance(st))))(current(st));
    }

    public static ParseExprResult parse_paren_expr(ParseState st)
    {
        return ((Func<ParseState, ParseExprResult>)((st2) => ((Func<ParseExprResult, ParseExprResult>)((inner) => unwrap_expr_ok(inner, (e) => (st) => finish_paren_expr(e, st))))(parse_expr(st2))))(skip_newlines(st));
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
        return ((Func<Token, ParseExprResult>)((field_name) => ((Func<ParseState, ParseExprResult>)((st2) => ((Func<ParseState, ParseExprResult>)((st3) => ((Func<ParseExprResult, ParseExprResult>)((val_result) => unwrap_expr_ok(val_result, (v) => (st) => finish_record_field(type_name, acc, field_name, v, st))))(parse_expr(st3))))(expect(new Equals_(), st2))))(advance(st))))(current(st));
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
        return (is_right_bracket(current_kind(st)) ? new ExprOk(new ListExpr(acc), advance(st)) : ((Func<ParseExprResult, ParseExprResult>)((elem) => unwrap_expr_ok(elem, (e) => (st) => finish_list_element(acc, e, st))))(parse_expr(st)));
    }

    public static ParseExprResult finish_list_element(List<Expr> acc, Expr e, ParseState st)
    {
        return ((Func<ParseState, ParseExprResult>)((st2) => (is_comma(current_kind(st2)) ? parse_list_elements(Enumerable.Concat(acc, new List<Expr>() { e }).ToList(), skip_newlines(advance(st2))) : parse_list_elements(Enumerable.Concat(acc, new List<Expr>() { e }).ToList(), st2))))(skip_newlines(st));
    }

    public static ParseExprResult parse_if_expr(ParseState st)
    {
        return ((Func<ParseState, ParseExprResult>)((st2) => ((Func<ParseExprResult, ParseExprResult>)((cond) => unwrap_expr_ok(cond, (c) => (st) => parse_if_then(c, st))))(parse_expr(st2))))(skip_newlines(advance(st)));
    }

    public static ParseExprResult parse_if_then(Expr c, ParseState st)
    {
        return ((Func<ParseState, ParseExprResult>)((st2) => ((Func<ParseState, ParseExprResult>)((st3) => ((Func<ParseState, ParseExprResult>)((st4) => ((Func<ParseExprResult, ParseExprResult>)((then_result) => unwrap_expr_ok(then_result, (t) => (st) => parse_if_else(c, t, st))))(parse_expr(st4))))(skip_newlines(st3))))(expect(new ThenKeyword(), st2))))(skip_newlines(st));
    }

    public static ParseExprResult parse_if_else(Expr c, Expr t, ParseState st)
    {
        return ((Func<ParseState, ParseExprResult>)((st2) => ((Func<ParseState, ParseExprResult>)((st3) => ((Func<ParseState, ParseExprResult>)((st4) => ((Func<ParseExprResult, ParseExprResult>)((else_result) => unwrap_expr_ok(else_result, (e) => (st) => finish_if(c, t, e, st))))(parse_expr(st4))))(skip_newlines(st3))))(expect(new ElseKeyword(), st2))))(skip_newlines(st));
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
        return (is_ident(current_kind(st)) ? parse_let_binding(acc, st) : (is_in_keyword(current_kind(st)) ? ((Func<ParseState, ParseExprResult>)((st2) => ((Func<ParseExprResult, ParseExprResult>)((body) => unwrap_expr_ok(body, (b) => (st) => finish_let(acc, b, st))))(parse_expr(st2))))(skip_newlines(advance(st))) : ((Func<ParseExprResult, ParseExprResult>)((body) => unwrap_expr_ok(body, (b) => (st) => finish_let(acc, b, st))))(parse_expr(st))));
    }

    public static ParseExprResult finish_let(List<LetBind> acc, Expr b, ParseState st)
    {
        return new ExprOk(new LetExpr(acc, b), st);
    }

    public static ParseExprResult parse_let_binding(List<LetBind> acc, ParseState st)
    {
        return ((Func<Token, ParseExprResult>)((name_tok) => ((Func<ParseState, ParseExprResult>)((st2) => ((Func<ParseState, ParseExprResult>)((st3) => ((Func<ParseExprResult, ParseExprResult>)((val_result) => unwrap_expr_ok(val_result, (v) => (st) => finish_let_binding(acc, name_tok, v, st))))(parse_expr(st3))))(expect(new Equals_(), st2))))(advance(st))))(current(st));
    }

    public static ParseExprResult finish_let_binding(List<LetBind> acc, Token name_tok, Expr v, ParseState st)
    {
        return ((Func<LetBind, ParseExprResult>)((binding) => ((Func<ParseState, ParseExprResult>)((st2) => (is_comma(current_kind(st2)) ? parse_let_bindings(Enumerable.Concat(acc, new List<LetBind>() { binding }).ToList(), skip_newlines(advance(st2))) : parse_let_bindings(Enumerable.Concat(acc, new List<LetBind>() { binding }).ToList(), st2))))(skip_newlines(st))))(new LetBind(name_tok, v));
    }

    public static ParseExprResult parse_match_expr(ParseState st)
    {
        return ((Func<ParseState, ParseExprResult>)((st2) => ((Func<ParseExprResult, ParseExprResult>)((scrut) => unwrap_expr_ok(scrut, (s) => (st) => start_match_branches(s, st))))(parse_expr(st2))))(advance(st));
    }

    public static ParseExprResult start_match_branches(Expr s, ParseState st)
    {
        return ((Func<ParseState, ParseExprResult>)((st2) => ((Func<Token, ParseExprResult>)((tok) => parse_match_branches(s, new List<MatchArm>(), tok.column, tok.line, st2)))(current(st2))))(skip_newlines(st));
    }

    public static ParseExprResult parse_match_branches(Expr scrut, List<MatchArm> acc, long col, long ln, ParseState st)
    {
        return (is_if_keyword(current_kind(st)) ? ((Func<Token, ParseExprResult>)((tok) => ((tok.line == ln) ? parse_one_match_branch(scrut, acc, col, ln, st) : ((tok.column == col) ? parse_one_match_branch(scrut, acc, col, ln, st) : new ExprOk(new MatchExpr(scrut, acc), st)))))(current(st)) : new ExprOk(new MatchExpr(scrut, acc), st));
    }

    public static ParseExprResult unwrap_pat_for_expr(ParsePatResult r, Func<Pat, Func<ParseState, ParseExprResult>> f)
    {
        return (r is PatOk _mPatOk95_ ? ((Func<ParseState, ParseExprResult>)((st) => ((Func<Pat, ParseExprResult>)((p) => f(p)(st)))((Pat)_mPatOk95_.Field0)))((ParseState)_mPatOk95_.Field1) : throw new InvalidOperationException("Non-exhaustive match"));
    }

    public static ParseExprResult parse_one_match_branch(Expr scrut, List<MatchArm> acc, long col, long ln, ParseState st)
    {
        return ((Func<ParseState, ParseExprResult>)((st2) => ((Func<ParsePatResult, ParseExprResult>)((pat) => unwrap_pat_for_expr(pat, (p) => (st) => parse_match_branch_body(scrut, acc, col, ln, p, st))))(parse_pattern(st2))))(advance(st));
    }

    public static ParseExprResult parse_match_branch_body(Expr scrut, List<MatchArm> acc, long col, long ln, Pat p, ParseState st)
    {
        return ((Func<ParseState, ParseExprResult>)((st2) => ((Func<ParseState, ParseExprResult>)((st3) => ((Func<ParseExprResult, ParseExprResult>)((body) => unwrap_expr_ok(body, (b) => (st) => finish_match_branch(scrut, acc, col, ln, p, b, st))))(parse_expr(st3))))(skip_newlines(st2))))(expect(new Arrow(), st));
    }

    public static ParseExprResult finish_match_branch(Expr scrut, List<MatchArm> acc, long col, long ln, Pat p, Expr b, ParseState st)
    {
        return ((Func<MatchArm, ParseExprResult>)((arm) => ((Func<ParseState, ParseExprResult>)((st2) => parse_match_branches(scrut, Enumerable.Concat(acc, new List<MatchArm>() { arm }).ToList(), col, ln, st2)))(skip_newlines(st))))(new MatchArm(p, b));
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
        return ((Func<Token, ParseExprResult>)((name_tok) => ((Func<ParseState, ParseExprResult>)((st2) => ((Func<ParseExprResult, ParseExprResult>)((val_result) => unwrap_expr_ok(val_result, (v) => (st) => finish_do_bind(acc, name_tok, v, st))))(parse_expr(st2))))(advance(advance(st)))))(current(st));
    }

    public static ParseExprResult finish_do_bind(List<DoStmt> acc, Token name_tok, Expr v, ParseState st)
    {
        return ((Func<ParseState, ParseExprResult>)((st2) => parse_do_stmts(Enumerable.Concat(acc, new List<DoStmt>() { new DoBindStmt(name_tok, v) }).ToList(), st2)))(skip_newlines(st));
    }

    public static ParseExprResult parse_do_expr_stmt(List<DoStmt> acc, ParseState st)
    {
        return ((Func<ParseExprResult, ParseExprResult>)((expr_result) => unwrap_expr_ok(expr_result, (e) => (st) => finish_do_expr(acc, e, st))))(parse_expr(st));
    }

    public static ParseExprResult finish_do_expr(List<DoStmt> acc, Expr e, ParseState st)
    {
        return ((Func<ParseState, ParseExprResult>)((st2) => parse_do_stmts(Enumerable.Concat(acc, new List<DoStmt>() { new DoExprStmt(e) }).ToList(), st2)))(skip_newlines(st));
    }

    public static ParseExprResult parse_handle_expr(ParseState st)
    {
        return ((Func<ParseState, ParseExprResult>)((st1) => ((Func<Token, ParseExprResult>)((eff_tok) => ((Func<ParseState, ParseExprResult>)((st2) => ((Func<ParseExprResult, ParseExprResult>)((body_result) => unwrap_expr_ok(body_result, (body) => (st) => finish_handle_body(eff_tok, body, st))))(parse_expr(st2))))(advance(st1))))(current(st1))))(advance(st));
    }

    public static ParseExprResult finish_handle_body(Token eff_tok, Expr body, ParseState st)
    {
        return ((Func<ParseState, ParseExprResult>)((st2) => ((Func<HandleParseResult, ParseExprResult>)((clauses) => new ExprOk(new HandleExpr(eff_tok, body, clauses.clauses), clauses.state)))(parse_handle_clauses(st2, new List<HandleClause>()))))(skip_newlines(st));
    }

    public static HandleParseResult parse_handle_clauses(ParseState st, List<HandleClause> acc)
    {
        return (is_ident(current_kind(st)) ? ((Func<Token, HandleParseResult>)((op_tok) => ((Func<ParseState, HandleParseResult>)((st1) => ((Func<HandleParamsResult, HandleParseResult>)((@params) => ((((long)@params.toks.Count) > 0L) ? ((Func<Token, HandleParseResult>)((resume_tok) => ((Func<ParseState, HandleParseResult>)((st5) => ((Func<ParseState, HandleParseResult>)((st6) => ((Func<ParseExprResult, HandleParseResult>)((body_result) => unwrap_handle_clause_body(op_tok, resume_tok, body_result, acc)))(parse_expr(st6))))(skip_newlines(st5))))(expect(new Equals_(), @params.state))))(@params.toks[(int)(((long)@params.toks.Count) - 1L)]) : new HandleParseResult(acc, st))))(parse_handle_params(st1, new List<Token>()))))(advance(st))))(current(st)) : new HandleParseResult(acc, st));
    }

    public static HandleParamsResult parse_handle_params(ParseState st, List<Token> acc)
    {
        while (true)
        {
            if (is_left_paren(current_kind(st)))
            {
                var st1 = advance(st);
                var tok = current(st1);
                var st2 = advance(st1);
                var st3 = expect(new RightParen(), st2);
                var _tco_0 = st3;
                var _tco_1 = Enumerable.Concat(acc, new List<Token>() { tok }).ToList();
                st = _tco_0;
                acc = _tco_1;
                continue;
            }
            else
            {
                return new HandleParamsResult(acc, st);
            }
        }
    }

    public static HandleParseResult unwrap_handle_clause_body(Token op_tok, Token resume_tok, ParseExprResult result, List<HandleClause> acc)
    {
        return (result is ExprOk _mExprOk96_ ? ((Func<ParseState, HandleParseResult>)((st) => ((Func<Expr, HandleParseResult>)((body) => ((Func<HandleClause, HandleParseResult>)((clause) => ((Func<ParseState, HandleParseResult>)((st2) => parse_handle_clauses(st2, Enumerable.Concat(acc, new List<HandleClause>() { clause }).ToList())))(skip_newlines(st))))(new HandleClause(op_tok, resume_tok, body))))((Expr)_mExprOk96_.Field0)))((ParseState)_mExprOk96_.Field1) : throw new InvalidOperationException("Non-exhaustive match"));
    }

    public static long token_length(Token t)
    {
        return ((long)t.text.Length);
    }

    public static CodexType resolve_type_expr(List<TypeBinding> tdm, ATypeExpr texpr)
    {
        return ((Func<ATypeExpr, CodexType>)((_scrutinee97_) => (_scrutinee97_ is ANamedType _mANamedType97_ ? ((Func<Name, CodexType>)((name) => resolve_type_name(tdm, name.value)))((Name)_mANamedType97_.Field0) : (_scrutinee97_ is AFunType _mAFunType97_ ? ((Func<ATypeExpr, CodexType>)((ret) => ((Func<ATypeExpr, CodexType>)((param) => new FunTy(resolve_type_expr(tdm, param), resolve_type_expr(tdm, ret))))((ATypeExpr)_mAFunType97_.Field0)))((ATypeExpr)_mAFunType97_.Field1) : (_scrutinee97_ is AAppType _mAAppType97_ ? ((Func<List<ATypeExpr>, CodexType>)((args) => ((Func<ATypeExpr, CodexType>)((ctor) => resolve_applied_type(tdm, ctor, args)))((ATypeExpr)_mAAppType97_.Field0)))((List<ATypeExpr>)_mAAppType97_.Field1) : throw new InvalidOperationException("Non-exhaustive match"))))))(texpr);
    }

    public static CodexType resolve_applied_type(List<TypeBinding> tdm, ATypeExpr ctor, List<ATypeExpr> args)
    {
        return (ctor is ANamedType _mANamedType98_ ? ((Func<Name, CodexType>)((name) => ((name.value == "List") ? ((((long)args.Count) == 1L) ? new ListTy(resolve_type_expr(tdm, args[(int)0L])) : new ListTy(new ErrorTy())) : new ConstructedTy(name, resolve_type_expr_list(tdm, args, 0L, ((long)args.Count), new List<CodexType>())))))((Name)_mANamedType98_.Field0) : ((Func<ATypeExpr, CodexType>)((_) => resolve_type_expr(tdm, ctor)))(ctor));
    }

    public static List<CodexType> resolve_type_expr_list(List<TypeBinding> tdm, List<ATypeExpr> args, long i, long len, List<CodexType> acc)
    {
        while (true)
        {
            if ((i == len))
            {
                return acc;
            }
            else
            {
                var _tco_0 = tdm;
                var _tco_1 = args;
                var _tco_2 = (i + 1L);
                var _tco_3 = len;
                var _tco_4 = Enumerable.Concat(acc, new List<CodexType>() { resolve_type_expr(tdm, args[(int)i]) }).ToList();
                tdm = _tco_0;
                args = _tco_1;
                i = _tco_2;
                len = _tco_3;
                acc = _tco_4;
                continue;
            }
        }
    }

    public static CodexType resolve_type_name(List<TypeBinding> tdm, string name)
    {
        return ((name == "Integer") ? new IntegerTy() : ((name == "Number") ? new NumberTy() : ((name == "Text") ? new TextTy() : ((name == "Boolean") ? new BooleanTy() : ((name == "Nothing") ? new NothingTy() : lookup_type_def(tdm, name))))));
    }

    public static CodexType lookup_type_def(List<TypeBinding> tdm, string name)
    {
        return lookup_type_def_loop(tdm, name, 0L, ((long)tdm.Count));
    }

    public static CodexType lookup_type_def_loop(List<TypeBinding> tdm, string name, long i, long len)
    {
        while (true)
        {
            if ((i == len))
            {
                return new ConstructedTy(new Name(name), new List<CodexType>());
            }
            else
            {
                var b = tdm[(int)i];
                if ((b.name == name))
                {
                    return b.bound_type;
                }
                else
                {
                    var _tco_0 = tdm;
                    var _tco_1 = name;
                    var _tco_2 = (i + 1L);
                    var _tco_3 = len;
                    tdm = _tco_0;
                    name = _tco_1;
                    i = _tco_2;
                    len = _tco_3;
                    continue;
                }
            }
        }
    }

    public static bool is_value_name(string name)
    {
        return ((((long)name.Length) == 0L) ? false : ((Func<long, bool>)((code) => ((code >= 97L) && (code <= 122L))))(((long)name[(int)0L].ToString()[0])));
    }

    public static ParamResult parameterize_type(UnificationState st, CodexType ty)
    {
        return ((Func<WalkResult, ParamResult>)((r) => ((Func<CodexType, ParamResult>)((wrapped) => new ParamResult(wrapped, r.entries, r.state)))(wrap_forall_from_entries(r.walked, r.entries, 0L, ((long)r.entries.Count)))))(parameterize_walk(st, new List<ParamEntry>(), ty));
    }

    public static CodexType wrap_forall_from_entries(CodexType ty, List<ParamEntry> entries, long i, long len)
    {
        return ((i == len) ? ty : ((Func<ParamEntry, CodexType>)((e) => new ForAllTy(e.var_id, wrap_forall_from_entries(ty, entries, (i + 1L), len))))(entries[(int)i]));
    }

    public static WalkResult parameterize_walk(UnificationState st, List<ParamEntry> entries, CodexType ty)
    {
        return ((Func<CodexType, WalkResult>)((_scrutinee99_) => (_scrutinee99_ is ConstructedTy _mConstructedTy99_ ? ((Func<List<CodexType>, WalkResult>)((args) => ((Func<Name, WalkResult>)((name) => (((((long)args.Count) == 0L) && is_value_name(name.value)) ? ((Func<long, WalkResult>)((looked) => ((looked >= 0L) ? new WalkResult(new TypeVar(looked), entries, st) : ((Func<FreshResult, WalkResult>)((fr) => (fr.var_type is TypeVar _mTypeVar100_ ? ((Func<long, WalkResult>)((new_id) => ((Func<ParamEntry, WalkResult>)((new_entry) => new WalkResult(fr.var_type, Enumerable.Concat(entries, new List<ParamEntry>() { new_entry }).ToList(), fr.state)))(new ParamEntry(name.value, new_id))))((long)_mTypeVar100_.Field0) : ((Func<CodexType, WalkResult>)((_) => new WalkResult(ty, entries, fr.state)))(fr.var_type))))(fresh_and_advance(st)))))(find_param_entry(entries, name.value, 0L, ((long)entries.Count))) : ((Func<WalkListResult, WalkResult>)((args_r) => new WalkResult(new ConstructedTy(name, args_r.walked_list), args_r.entries, args_r.state)))(parameterize_walk_list(st, entries, args, 0L, ((long)args.Count), new List<CodexType>())))))((Name)_mConstructedTy99_.Field0)))((List<CodexType>)_mConstructedTy99_.Field1) : (_scrutinee99_ is FunTy _mFunTy99_ ? ((Func<CodexType, WalkResult>)((ret) => ((Func<CodexType, WalkResult>)((param) => ((Func<WalkResult, WalkResult>)((pr) => ((Func<WalkResult, WalkResult>)((rr) => new WalkResult(new FunTy(pr.walked, rr.walked), rr.entries, rr.state)))(parameterize_walk(pr.state, pr.entries, ret))))(parameterize_walk(st, entries, param))))((CodexType)_mFunTy99_.Field0)))((CodexType)_mFunTy99_.Field1) : (_scrutinee99_ is ListTy _mListTy99_ ? ((Func<CodexType, WalkResult>)((elem) => ((Func<WalkResult, WalkResult>)((er) => new WalkResult(new ListTy(er.walked), er.entries, er.state)))(parameterize_walk(st, entries, elem))))((CodexType)_mListTy99_.Field0) : (_scrutinee99_ is ForAllTy _mForAllTy99_ ? ((Func<CodexType, WalkResult>)((body) => ((Func<long, WalkResult>)((id) => ((Func<WalkResult, WalkResult>)((br) => new WalkResult(new ForAllTy(id, br.walked), br.entries, br.state)))(parameterize_walk(st, entries, body))))((long)_mForAllTy99_.Field0)))((CodexType)_mForAllTy99_.Field1) : ((Func<CodexType, WalkResult>)((_) => new WalkResult(ty, entries, st)))(_scrutinee99_)))))))(ty);
    }

    public static long find_param_entry(List<ParamEntry> entries, string name, long i, long len)
    {
        while (true)
        {
            if ((i == len))
            {
                return (0L - 1L);
            }
            else
            {
                var e = entries[(int)i];
                if ((e.param_name == name))
                {
                    return e.var_id;
                }
                else
                {
                    var _tco_0 = entries;
                    var _tco_1 = name;
                    var _tco_2 = (i + 1L);
                    var _tco_3 = len;
                    entries = _tco_0;
                    name = _tco_1;
                    i = _tco_2;
                    len = _tco_3;
                    continue;
                }
            }
        }
    }

    public static WalkListResult parameterize_walk_list(UnificationState st, List<ParamEntry> entries, List<CodexType> args, long i, long len, List<CodexType> acc)
    {
        while (true)
        {
            if ((i == len))
            {
                return new WalkListResult(acc, entries, st);
            }
            else
            {
                var r = parameterize_walk(st, entries, args[(int)i]);
                var _tco_0 = r.state;
                var _tco_1 = r.entries;
                var _tco_2 = args;
                var _tco_3 = (i + 1L);
                var _tco_4 = len;
                var _tco_5 = Enumerable.Concat(acc, new List<CodexType>() { r.walked }).ToList();
                st = _tco_0;
                entries = _tco_1;
                args = _tco_2;
                i = _tco_3;
                len = _tco_4;
                acc = _tco_5;
                continue;
            }
        }
    }

    public static CheckResult check_def(UnificationState st, TypeEnv env, ADef def)
    {
        return ((Func<DefSetup, CheckResult>)((declared) => ((Func<DefParamResult, CheckResult>)((env2) => ((Func<CheckResult, CheckResult>)((body_r) => ((Func<UnifyResult, CheckResult>)((u) => new CheckResult(declared.expected_type, u.state)))(unify(body_r.state, env2.remaining_type, body_r.inferred_type))))(infer_expr(env2.state, env2.env, def.body))))(bind_def_params(declared.state, declared.env, def.@params, declared.expected_type, 0L, ((long)def.@params.Count)))))(resolve_declared_type(st, env, def));
    }

    public static DefSetup resolve_declared_type(UnificationState st, TypeEnv env, ADef def)
    {
        return ((((long)def.declared_type.Count) == 0L) ? ((Func<FreshResult, DefSetup>)((fr) => new DefSetup(fr.var_type, fr.var_type, fr.state, env)))(fresh_and_advance(st)) : ((Func<CodexType, DefSetup>)((env_type) => ((Func<FreshResult, DefSetup>)((inst) => new DefSetup(inst.var_type, inst.var_type, inst.state, env)))(instantiate_type(st, env_type))))(env_lookup(env, def.name.value)));
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
        return ((Func<List<TypeBinding>, ModuleResult>)((tdm) => ((Func<LetBindResult, ModuleResult>)((tenv) => ((Func<LetBindResult, ModuleResult>)((env) => check_all_defs(env.state, env.env, mod.defs, 0L, ((long)mod.defs.Count), new List<TypeBinding>())))(register_all_defs(tenv.state, tenv.env, tdm, mod.defs, 0L, ((long)mod.defs.Count)))))(register_type_defs(empty_unification_state(), builtin_type_env(), tdm, mod.type_defs, 0L, ((long)mod.type_defs.Count)))))(build_type_def_map(mod.type_defs, 0L, ((long)mod.type_defs.Count), new List<TypeBinding>()));
    }

    public static LetBindResult register_all_defs(UnificationState st, TypeEnv env, List<TypeBinding> tdm, List<ADef> defs, long i, long len)
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
                var ty = ((((long)def.declared_type.Count) == 0L) ? ((Func<FreshResult, LetBindResult>)((fr) => ((Func<TypeEnv, LetBindResult>)((env2) => new LetBindResult(fr.state, env2)))(env_bind(env, def.name.value, fr.var_type))))(fresh_and_advance(st)) : ((Func<CodexType, LetBindResult>)((resolved) => ((Func<ParamResult, LetBindResult>)((pr) => new LetBindResult(pr.state, env_bind(env, def.name.value, pr.parameterized))))(parameterize_type(st, resolved))))(resolve_type_expr(tdm, def.declared_type[(int)0L])));
                var _tco_0 = ty.state;
                var _tco_1 = ty.env;
                var _tco_2 = tdm;
                var _tco_3 = defs;
                var _tco_4 = (i + 1L);
                var _tco_5 = len;
                st = _tco_0;
                env = _tco_1;
                tdm = _tco_2;
                defs = _tco_3;
                i = _tco_4;
                len = _tco_5;
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

    public static List<TypeBinding> build_type_def_map(List<ATypeDef> tdefs, long i, long len, List<TypeBinding> acc)
    {
        while (true)
        {
            if ((i == len))
            {
                return acc;
            }
            else
            {
                var td = tdefs[(int)i];
                var entry = ((Func<ATypeDef, TypeBinding>)((_scrutinee101_) => (_scrutinee101_ is AVariantTypeDef _mAVariantTypeDef101_ ? ((Func<List<AVariantCtorDef>, TypeBinding>)((ctors) => ((Func<List<Name>, TypeBinding>)((type_params) => ((Func<Name, TypeBinding>)((name) => ((Func<List<SumCtor>, TypeBinding>)((sum_ctors) => new TypeBinding(name.value, new SumTy(name, sum_ctors))))(build_sum_ctors(tdefs, ctors, 0L, ((long)ctors.Count), new List<SumCtor>(), acc))))((Name)_mAVariantTypeDef101_.Field0)))((List<Name>)_mAVariantTypeDef101_.Field1)))((List<AVariantCtorDef>)_mAVariantTypeDef101_.Field2) : (_scrutinee101_ is ARecordTypeDef _mARecordTypeDef101_ ? ((Func<List<ARecordFieldDef>, TypeBinding>)((fields) => ((Func<List<Name>, TypeBinding>)((type_params) => ((Func<Name, TypeBinding>)((name) => ((Func<List<RecordField>, TypeBinding>)((rec_fields) => new TypeBinding(name.value, new RecordTy(name, rec_fields))))(build_record_fields_for_map(tdefs, fields, 0L, ((long)fields.Count), new List<RecordField>(), acc))))((Name)_mARecordTypeDef101_.Field0)))((List<Name>)_mARecordTypeDef101_.Field1)))((List<ARecordFieldDef>)_mARecordTypeDef101_.Field2) : throw new InvalidOperationException("Non-exhaustive match")))))(td);
                var _tco_0 = tdefs;
                var _tco_1 = (i + 1L);
                var _tco_2 = len;
                var _tco_3 = Enumerable.Concat(acc, new List<TypeBinding>() { entry }).ToList();
                tdefs = _tco_0;
                i = _tco_1;
                len = _tco_2;
                acc = _tco_3;
                continue;
            }
        }
    }

    public static List<SumCtor> build_sum_ctors(List<ATypeDef> tdefs, List<AVariantCtorDef> ctors, long i, long len, List<SumCtor> acc, List<TypeBinding> partial_tdm)
    {
        while (true)
        {
            if ((i == len))
            {
                return acc;
            }
            else
            {
                var c = ctors[(int)i];
                var field_types = resolve_type_expr_list_for_map(tdefs, c.fields, 0L, ((long)c.fields.Count), new List<CodexType>(), partial_tdm);
                var sc = new SumCtor(c.name, field_types);
                var _tco_0 = tdefs;
                var _tco_1 = ctors;
                var _tco_2 = (i + 1L);
                var _tco_3 = len;
                var _tco_4 = Enumerable.Concat(acc, new List<SumCtor>() { sc }).ToList();
                var _tco_5 = partial_tdm;
                tdefs = _tco_0;
                ctors = _tco_1;
                i = _tco_2;
                len = _tco_3;
                acc = _tco_4;
                partial_tdm = _tco_5;
                continue;
            }
        }
    }

    public static List<RecordField> build_record_fields_for_map(List<ATypeDef> tdefs, List<ARecordFieldDef> fields, long i, long len, List<RecordField> acc, List<TypeBinding> partial_tdm)
    {
        while (true)
        {
            if ((i == len))
            {
                return acc;
            }
            else
            {
                var f = fields[(int)i];
                var rfield = new RecordField(f.name, resolve_type_expr(partial_tdm, f.type_expr));
                var _tco_0 = tdefs;
                var _tco_1 = fields;
                var _tco_2 = (i + 1L);
                var _tco_3 = len;
                var _tco_4 = Enumerable.Concat(acc, new List<RecordField>() { rfield }).ToList();
                var _tco_5 = partial_tdm;
                tdefs = _tco_0;
                fields = _tco_1;
                i = _tco_2;
                len = _tco_3;
                acc = _tco_4;
                partial_tdm = _tco_5;
                continue;
            }
        }
    }

    public static List<CodexType> resolve_type_expr_list_for_map(List<ATypeDef> tdefs, List<ATypeExpr> args, long i, long len, List<CodexType> acc, List<TypeBinding> partial_tdm)
    {
        while (true)
        {
            if ((i == len))
            {
                return acc;
            }
            else
            {
                var _tco_0 = tdefs;
                var _tco_1 = args;
                var _tco_2 = (i + 1L);
                var _tco_3 = len;
                var _tco_4 = Enumerable.Concat(acc, new List<CodexType>() { resolve_type_expr(partial_tdm, args[(int)i]) }).ToList();
                var _tco_5 = partial_tdm;
                tdefs = _tco_0;
                args = _tco_1;
                i = _tco_2;
                len = _tco_3;
                acc = _tco_4;
                partial_tdm = _tco_5;
                continue;
            }
        }
    }

    public static LetBindResult register_type_defs(UnificationState st, TypeEnv env, List<TypeBinding> tdm, List<ATypeDef> tdefs, long i, long len)
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
                var r = register_one_type_def(st, env, tdm, td);
                var _tco_0 = r.state;
                var _tco_1 = r.env;
                var _tco_2 = tdm;
                var _tco_3 = tdefs;
                var _tco_4 = (i + 1L);
                var _tco_5 = len;
                st = _tco_0;
                env = _tco_1;
                tdm = _tco_2;
                tdefs = _tco_3;
                i = _tco_4;
                len = _tco_5;
                continue;
            }
        }
    }

    public static LetBindResult register_one_type_def(UnificationState st, TypeEnv env, List<TypeBinding> tdm, ATypeDef td)
    {
        return ((Func<ATypeDef, LetBindResult>)((_scrutinee102_) => (_scrutinee102_ is AVariantTypeDef _mAVariantTypeDef102_ ? ((Func<List<AVariantCtorDef>, LetBindResult>)((ctors) => ((Func<List<Name>, LetBindResult>)((type_params) => ((Func<Name, LetBindResult>)((name) => ((Func<CodexType, LetBindResult>)((result_ty) => register_variant_ctors(st, env, tdm, ctors, result_ty, 0L, ((long)ctors.Count))))(lookup_type_def(tdm, name.value))))((Name)_mAVariantTypeDef102_.Field0)))((List<Name>)_mAVariantTypeDef102_.Field1)))((List<AVariantCtorDef>)_mAVariantTypeDef102_.Field2) : (_scrutinee102_ is ARecordTypeDef _mARecordTypeDef102_ ? ((Func<List<ARecordFieldDef>, LetBindResult>)((fields) => ((Func<List<Name>, LetBindResult>)((type_params) => ((Func<Name, LetBindResult>)((name) => ((Func<List<RecordField>, LetBindResult>)((resolved_fields) => ((Func<CodexType, LetBindResult>)((result_ty) => ((Func<CodexType, LetBindResult>)((ctor_ty) => new LetBindResult(st, env_bind(env, name.value, ctor_ty))))(build_record_ctor_type(tdm, fields, result_ty, 0L, ((long)fields.Count)))))(new RecordTy(name, resolved_fields))))(build_record_fields(tdm, fields, 0L, ((long)fields.Count), new List<RecordField>()))))((Name)_mARecordTypeDef102_.Field0)))((List<Name>)_mARecordTypeDef102_.Field1)))((List<ARecordFieldDef>)_mARecordTypeDef102_.Field2) : throw new InvalidOperationException("Non-exhaustive match")))))(td);
    }

    public static List<RecordField> build_record_fields(List<TypeBinding> tdm, List<ARecordFieldDef> fields, long i, long len, List<RecordField> acc)
    {
        while (true)
        {
            if ((i == len))
            {
                return acc;
            }
            else
            {
                var f = fields[(int)i];
                var rfield = new RecordField(f.name, resolve_type_expr(tdm, f.type_expr));
                var _tco_0 = tdm;
                var _tco_1 = fields;
                var _tco_2 = (i + 1L);
                var _tco_3 = len;
                var _tco_4 = Enumerable.Concat(acc, new List<RecordField>() { rfield }).ToList();
                tdm = _tco_0;
                fields = _tco_1;
                i = _tco_2;
                len = _tco_3;
                acc = _tco_4;
                continue;
            }
        }
    }

    public static CodexType lookup_record_field(List<RecordField> fields, string name)
    {
        return ((((long)fields.Count) == 0L) ? new ErrorTy() : ((Func<RecordField, CodexType>)((f) => ((f.name.value == name) ? f.type_val : lookup_record_field_loop(fields, name, 1L, ((long)fields.Count)))))(fields[(int)0L]));
    }

    public static CodexType lookup_record_field_loop(List<RecordField> fields, string name, long i, long len)
    {
        while (true)
        {
            if ((i == len))
            {
                return new ErrorTy();
            }
            else
            {
                var f = fields[(int)i];
                if ((f.name.value == name))
                {
                    return f.type_val;
                }
                else
                {
                    var _tco_0 = fields;
                    var _tco_1 = name;
                    var _tco_2 = (i + 1L);
                    var _tco_3 = len;
                    fields = _tco_0;
                    name = _tco_1;
                    i = _tco_2;
                    len = _tco_3;
                    continue;
                }
            }
        }
    }

    public static LetBindResult register_variant_ctors(UnificationState st, TypeEnv env, List<TypeBinding> tdm, List<AVariantCtorDef> ctors, CodexType result_ty, long i, long len)
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
                var ctor_ty = build_ctor_type(tdm, ctor.fields, result_ty, 0L, ((long)ctor.fields.Count));
                var env2 = env_bind(env, ctor.name.value, ctor_ty);
                var _tco_0 = st;
                var _tco_1 = env2;
                var _tco_2 = tdm;
                var _tco_3 = ctors;
                var _tco_4 = result_ty;
                var _tco_5 = (i + 1L);
                var _tco_6 = len;
                st = _tco_0;
                env = _tco_1;
                tdm = _tco_2;
                ctors = _tco_3;
                result_ty = _tco_4;
                i = _tco_5;
                len = _tco_6;
                continue;
            }
        }
    }

    public static CodexType build_ctor_type(List<TypeBinding> tdm, List<ATypeExpr> fields, CodexType result, long i, long len)
    {
        return ((i == len) ? result : ((Func<CodexType, CodexType>)((rest) => new FunTy(resolve_type_expr(tdm, fields[(int)i]), rest)))(build_ctor_type(tdm, fields, result, (i + 1L), len)));
    }

    public static CodexType build_record_ctor_type(List<TypeBinding> tdm, List<ARecordFieldDef> fields, CodexType result, long i, long len)
    {
        return ((i == len) ? result : ((Func<ARecordFieldDef, CodexType>)((f) => ((Func<CodexType, CodexType>)((rest) => new FunTy(resolve_type_expr(tdm, f.type_expr), rest)))(build_record_ctor_type(tdm, fields, result, (i + 1L), len))))(fields[(int)i]));
    }

    public static CheckResult infer_literal(UnificationState st, LiteralKind kind)
    {
        return ((Func<LiteralKind, CheckResult>)((_scrutinee103_) => (_scrutinee103_ is IntLit _mIntLit103_ ? new CheckResult(new IntegerTy(), st) : (_scrutinee103_ is NumLit _mNumLit103_ ? new CheckResult(new NumberTy(), st) : (_scrutinee103_ is TextLit _mTextLit103_ ? new CheckResult(new TextTy(), st) : (_scrutinee103_ is BoolLit _mBoolLit103_ ? new CheckResult(new BooleanTy(), st) : throw new InvalidOperationException("Non-exhaustive match")))))))(kind);
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
        return ((Func<CodexType, CodexType>)((_scrutinee104_) => (_scrutinee104_ is TypeVar _mTypeVar104_ ? ((Func<long, CodexType>)((id) => ((id == var_id) ? replacement : ty)))((long)_mTypeVar104_.Field0) : (_scrutinee104_ is FunTy _mFunTy104_ ? ((Func<CodexType, CodexType>)((ret) => ((Func<CodexType, CodexType>)((param) => new FunTy(subst_type_var(param, var_id, replacement), subst_type_var(ret, var_id, replacement))))((CodexType)_mFunTy104_.Field0)))((CodexType)_mFunTy104_.Field1) : (_scrutinee104_ is ListTy _mListTy104_ ? ((Func<CodexType, CodexType>)((elem) => new ListTy(subst_type_var(elem, var_id, replacement))))((CodexType)_mListTy104_.Field0) : (_scrutinee104_ is ForAllTy _mForAllTy104_ ? ((Func<CodexType, CodexType>)((body) => ((Func<long, CodexType>)((inner_id) => ((inner_id == var_id) ? ty : new ForAllTy(inner_id, subst_type_var(body, var_id, replacement)))))((long)_mForAllTy104_.Field0)))((CodexType)_mForAllTy104_.Field1) : (_scrutinee104_ is ConstructedTy _mConstructedTy104_ ? ((Func<List<CodexType>, CodexType>)((args) => ((Func<Name, CodexType>)((name) => new ConstructedTy(name, map_subst_type_var(args, var_id, replacement, 0L, ((long)args.Count), new List<CodexType>()))))((Name)_mConstructedTy104_.Field0)))((List<CodexType>)_mConstructedTy104_.Field1) : (_scrutinee104_ is SumTy _mSumTy104_ ? ((Func<List<SumCtor>, CodexType>)((ctors) => ((Func<Name, CodexType>)((name) => ty))((Name)_mSumTy104_.Field0)))((List<SumCtor>)_mSumTy104_.Field1) : (_scrutinee104_ is RecordTy _mRecordTy104_ ? ((Func<List<RecordField>, CodexType>)((fields) => ((Func<Name, CodexType>)((name) => ty))((Name)_mRecordTy104_.Field0)))((List<RecordField>)_mRecordTy104_.Field1) : ((Func<CodexType, CodexType>)((_) => ty))(_scrutinee104_))))))))))(ty);
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
        return ((Func<BinaryOp, CheckResult>)((_scrutinee105_) => (_scrutinee105_ is OpAdd _mOpAdd105_ ? infer_arithmetic(st, lt, rt) : (_scrutinee105_ is OpSub _mOpSub105_ ? infer_arithmetic(st, lt, rt) : (_scrutinee105_ is OpMul _mOpMul105_ ? infer_arithmetic(st, lt, rt) : (_scrutinee105_ is OpDiv _mOpDiv105_ ? infer_arithmetic(st, lt, rt) : (_scrutinee105_ is OpPow _mOpPow105_ ? infer_arithmetic(st, lt, rt) : (_scrutinee105_ is OpEq _mOpEq105_ ? infer_comparison(st, lt, rt) : (_scrutinee105_ is OpNotEq _mOpNotEq105_ ? infer_comparison(st, lt, rt) : (_scrutinee105_ is OpLt _mOpLt105_ ? infer_comparison(st, lt, rt) : (_scrutinee105_ is OpGt _mOpGt105_ ? infer_comparison(st, lt, rt) : (_scrutinee105_ is OpLtEq _mOpLtEq105_ ? infer_comparison(st, lt, rt) : (_scrutinee105_ is OpGtEq _mOpGtEq105_ ? infer_comparison(st, lt, rt) : (_scrutinee105_ is OpAnd _mOpAnd105_ ? infer_logical(st, lt, rt) : (_scrutinee105_ is OpOr _mOpOr105_ ? infer_logical(st, lt, rt) : (_scrutinee105_ is OpAppend _mOpAppend105_ ? infer_append(st, lt, rt) : (_scrutinee105_ is OpCons _mOpCons105_ ? infer_cons(st, lt, rt) : (_scrutinee105_ is OpDefEq _mOpDefEq105_ ? infer_comparison(st, lt, rt) : throw new InvalidOperationException("Non-exhaustive match")))))))))))))))))))(op);
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
        return ((Func<CodexType, CheckResult>)((resolved) => (resolved is TextTy _mTextTy106_ ? ((Func<UnifyResult, CheckResult>)((r) => new CheckResult(new TextTy(), r.state)))(unify(st, rt, new TextTy())) : ((Func<CodexType, CheckResult>)((_) => ((Func<UnifyResult, CheckResult>)((r) => new CheckResult(lt, r.state)))(unify(st, lt, rt))))(resolved))))(resolve(st, lt));
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
        return ((Func<APat, PatBindResult>)((_scrutinee107_) => (_scrutinee107_ is AVarPat _mAVarPat107_ ? ((Func<Name, PatBindResult>)((name) => new PatBindResult(st, env_bind(env, name.value, ty))))((Name)_mAVarPat107_.Field0) : (_scrutinee107_ is AWildPat _mAWildPat107_ ? new PatBindResult(st, env) : (_scrutinee107_ is ALitPat _mALitPat107_ ? ((Func<LiteralKind, PatBindResult>)((kind) => ((Func<string, PatBindResult>)((val) => new PatBindResult(st, env)))((string)_mALitPat107_.Field0)))((LiteralKind)_mALitPat107_.Field1) : (_scrutinee107_ is ACtorPat _mACtorPat107_ ? ((Func<List<APat>, PatBindResult>)((sub_pats) => ((Func<Name, PatBindResult>)((ctor_name) => ((Func<FreshResult, PatBindResult>)((ctor_lookup) => bind_ctor_sub_patterns(ctor_lookup.state, env, sub_pats, ctor_lookup.var_type, 0L, ((long)sub_pats.Count))))(instantiate_type(st, env_lookup(env, ctor_name.value)))))((Name)_mACtorPat107_.Field0)))((List<APat>)_mACtorPat107_.Field1) : throw new InvalidOperationException("Non-exhaustive match")))))))(pat);
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
        return ((Func<AExpr, CheckResult>)((_scrutinee108_) => (_scrutinee108_ is ALitExpr _mALitExpr108_ ? ((Func<LiteralKind, CheckResult>)((kind) => ((Func<string, CheckResult>)((val) => infer_literal(st, kind)))((string)_mALitExpr108_.Field0)))((LiteralKind)_mALitExpr108_.Field1) : (_scrutinee108_ is ANameExpr _mANameExpr108_ ? ((Func<Name, CheckResult>)((name) => infer_name(st, env, name.value)))((Name)_mANameExpr108_.Field0) : (_scrutinee108_ is ABinaryExpr _mABinaryExpr108_ ? ((Func<AExpr, CheckResult>)((right) => ((Func<BinaryOp, CheckResult>)((op) => ((Func<AExpr, CheckResult>)((left) => infer_binary(st, env, left, op, right)))((AExpr)_mABinaryExpr108_.Field0)))((BinaryOp)_mABinaryExpr108_.Field1)))((AExpr)_mABinaryExpr108_.Field2) : (_scrutinee108_ is AUnaryExpr _mAUnaryExpr108_ ? ((Func<AExpr, CheckResult>)((operand) => ((Func<CheckResult, CheckResult>)((r) => ((Func<UnifyResult, CheckResult>)((u) => new CheckResult(new IntegerTy(), u.state)))(unify(r.state, r.inferred_type, new IntegerTy()))))(infer_expr(st, env, operand))))((AExpr)_mAUnaryExpr108_.Field0) : (_scrutinee108_ is AApplyExpr _mAApplyExpr108_ ? ((Func<AExpr, CheckResult>)((arg) => ((Func<AExpr, CheckResult>)((func) => infer_application(st, env, func, arg)))((AExpr)_mAApplyExpr108_.Field0)))((AExpr)_mAApplyExpr108_.Field1) : (_scrutinee108_ is AIfExpr _mAIfExpr108_ ? ((Func<AExpr, CheckResult>)((else_e) => ((Func<AExpr, CheckResult>)((then_e) => ((Func<AExpr, CheckResult>)((cond) => infer_if(st, env, cond, then_e, else_e)))((AExpr)_mAIfExpr108_.Field0)))((AExpr)_mAIfExpr108_.Field1)))((AExpr)_mAIfExpr108_.Field2) : (_scrutinee108_ is ALetExpr _mALetExpr108_ ? ((Func<AExpr, CheckResult>)((body) => ((Func<List<ALetBind>, CheckResult>)((bindings) => infer_let(st, env, bindings, body)))((List<ALetBind>)_mALetExpr108_.Field0)))((AExpr)_mALetExpr108_.Field1) : (_scrutinee108_ is ALambdaExpr _mALambdaExpr108_ ? ((Func<AExpr, CheckResult>)((body) => ((Func<List<Name>, CheckResult>)((@params) => infer_lambda(st, env, @params, body)))((List<Name>)_mALambdaExpr108_.Field0)))((AExpr)_mALambdaExpr108_.Field1) : (_scrutinee108_ is AMatchExpr _mAMatchExpr108_ ? ((Func<List<AMatchArm>, CheckResult>)((arms) => ((Func<AExpr, CheckResult>)((scrutinee) => infer_match(st, env, scrutinee, arms)))((AExpr)_mAMatchExpr108_.Field0)))((List<AMatchArm>)_mAMatchExpr108_.Field1) : (_scrutinee108_ is AListExpr _mAListExpr108_ ? ((Func<List<AExpr>, CheckResult>)((elems) => infer_list(st, env, elems)))((List<AExpr>)_mAListExpr108_.Field0) : (_scrutinee108_ is ADoExpr _mADoExpr108_ ? ((Func<List<ADoStmt>, CheckResult>)((stmts) => infer_do(st, env, stmts)))((List<ADoStmt>)_mADoExpr108_.Field0) : (_scrutinee108_ is AFieldAccess _mAFieldAccess108_ ? ((Func<Name, CheckResult>)((field) => ((Func<AExpr, CheckResult>)((obj) => ((Func<CheckResult, CheckResult>)((r) => ((Func<CodexType, CheckResult>)((resolved) => ((Func<CodexType, CheckResult>)((_scrutinee109_) => (_scrutinee109_ is RecordTy _mRecordTy109_ ? ((Func<List<RecordField>, CheckResult>)((rfields) => ((Func<Name, CheckResult>)((rname) => ((Func<CodexType, CheckResult>)((ftype) => new CheckResult(ftype, r.state)))(lookup_record_field(rfields, field.value))))((Name)_mRecordTy109_.Field0)))((List<RecordField>)_mRecordTy109_.Field1) : (_scrutinee109_ is ConstructedTy _mConstructedTy109_ ? ((Func<List<CodexType>, CheckResult>)((cargs) => ((Func<Name, CheckResult>)((cname) => ((Func<CodexType, CheckResult>)((record_type) => (record_type is RecordTy _mRecordTy110_ ? ((Func<List<RecordField>, CheckResult>)((rfields) => ((Func<Name, CheckResult>)((rname) => ((Func<CodexType, CheckResult>)((ftype) => new CheckResult(ftype, r.state)))(lookup_record_field(rfields, field.value))))((Name)_mRecordTy110_.Field0)))((List<RecordField>)_mRecordTy110_.Field1) : ((Func<CodexType, CheckResult>)((_) => ((Func<FreshResult, CheckResult>)((fr) => new CheckResult(fr.var_type, fr.state)))(fresh_and_advance(r.state))))(record_type))))(resolve_constructed_to_record(env, cname.value))))((Name)_mConstructedTy109_.Field0)))((List<CodexType>)_mConstructedTy109_.Field1) : ((Func<CodexType, CheckResult>)((_) => ((Func<FreshResult, CheckResult>)((fr) => new CheckResult(fr.var_type, fr.state)))(fresh_and_advance(r.state))))(_scrutinee109_)))))(resolved)))(deep_resolve(r.state, r.inferred_type))))(infer_expr(st, env, obj))))((AExpr)_mAFieldAccess108_.Field0)))((Name)_mAFieldAccess108_.Field1) : (_scrutinee108_ is ARecordExpr _mARecordExpr108_ ? ((Func<List<AFieldExpr>, CheckResult>)((fields) => ((Func<Name, CheckResult>)((name) => ((Func<UnificationState, CheckResult>)((st2) => ((Func<CodexType, CheckResult>)((ctor_type) => ((Func<CodexType, CheckResult>)((result_type) => new CheckResult(result_type, st2)))(strip_fun_args(ctor_type))))((env_has(env, name.value) ? env_lookup(env, name.value) : new ErrorTy()))))(infer_record_fields(st, env, fields, 0L, ((long)fields.Count)))))((Name)_mARecordExpr108_.Field0)))((List<AFieldExpr>)_mARecordExpr108_.Field1) : (_scrutinee108_ is AErrorExpr _mAErrorExpr108_ ? ((Func<string, CheckResult>)((msg) => new CheckResult(new ErrorTy(), st)))((string)_mAErrorExpr108_.Field0) : throw new InvalidOperationException("Non-exhaustive match")))))))))))))))))(expr);
    }

    public static CodexType resolve_constructed_to_record(TypeEnv env, string name)
    {
        return (env_has(env, name) ? strip_fun_args(env_lookup(env, name)) : new ErrorTy());
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

    public static CodexType strip_fun_args(CodexType ty)
    {
        while (true)
        {
            var _tco_s = ty;
            if (_tco_s is FunTy _tco_m0)
            {
                var p = _tco_m0.Field0;
                var r = _tco_m0.Field1;
                var _tco_0 = r;
                ty = _tco_0;
                continue;
            }
            {
                var _ = _tco_s;
                return ty;
            }
        }
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
        return ((Func<TypeEnv, TypeEnv>)((e) => ((Func<TypeEnv, TypeEnv>)((e2) => ((Func<TypeEnv, TypeEnv>)((e3) => ((Func<TypeEnv, TypeEnv>)((e4) => ((Func<TypeEnv, TypeEnv>)((e5) => ((Func<TypeEnv, TypeEnv>)((e6) => ((Func<TypeEnv, TypeEnv>)((e7) => ((Func<TypeEnv, TypeEnv>)((e8) => ((Func<TypeEnv, TypeEnv>)((e9) => ((Func<TypeEnv, TypeEnv>)((e10) => ((Func<TypeEnv, TypeEnv>)((e10b) => ((Func<TypeEnv, TypeEnv>)((e11) => ((Func<TypeEnv, TypeEnv>)((e12) => ((Func<TypeEnv, TypeEnv>)((e13) => ((Func<TypeEnv, TypeEnv>)((e14) => ((Func<TypeEnv, TypeEnv>)((e15) => ((Func<TypeEnv, TypeEnv>)((e16) => ((Func<TypeEnv, TypeEnv>)((e17) => ((Func<TypeEnv, TypeEnv>)((e18) => ((Func<TypeEnv, TypeEnv>)((e19) => ((Func<TypeEnv, TypeEnv>)((e20) => ((Func<TypeEnv, TypeEnv>)((e21) => ((Func<TypeEnv, TypeEnv>)((e22) => e22))(env_bind(e21, "read-file", new FunTy(new TextTy(), new TextTy())))))(env_bind(e20, "read-line", new TextTy()))))(env_bind(e19, "fold", new ForAllTy(0L, new ForAllTy(1L, new FunTy(new FunTy(new TypeVar(1L), new FunTy(new TypeVar(0L), new TypeVar(1L))), new FunTy(new TypeVar(1L), new FunTy(new ListTy(new TypeVar(0L)), new TypeVar(1L))))))))))(env_bind(e18, "filter", new ForAllTy(0L, new FunTy(new FunTy(new TypeVar(0L), new BooleanTy()), new FunTy(new ListTy(new TypeVar(0L)), new ListTy(new TypeVar(0L)))))))))(env_bind(e17, "map", new ForAllTy(0L, new ForAllTy(1L, new FunTy(new FunTy(new TypeVar(0L), new TypeVar(1L)), new FunTy(new ListTy(new TypeVar(0L)), new ListTy(new TypeVar(1L))))))))))(env_bind(e16, "list-at", new ForAllTy(0L, new FunTy(new ListTy(new TypeVar(0L)), new FunTy(new IntegerTy(), new TypeVar(0L))))))))(env_bind(e15, "list-length", new ForAllTy(0L, new FunTy(new ListTy(new TypeVar(0L)), new IntegerTy()))))))(env_bind(e14, "print-line", new FunTy(new TextTy(), new NothingTy())))))(env_bind(e13, "show", new ForAllTy(0L, new FunTy(new TypeVar(0L), new TextTy()))))))(env_bind(e12, "text-to-integer", new FunTy(new TextTy(), new IntegerTy())))))(env_bind(e11, "text-replace", new FunTy(new TextTy(), new FunTy(new TextTy(), new FunTy(new TextTy(), new TextTy())))))))(env_bind(e10b, "code-to-char", new FunTy(new IntegerTy(), new TextTy())))))(env_bind(e10, "char-code-at", new FunTy(new TextTy(), new FunTy(new IntegerTy(), new IntegerTy()))))))(env_bind(e9, "char-code", new FunTy(new TextTy(), new IntegerTy())))))(env_bind(e8, "is-whitespace", new FunTy(new TextTy(), new BooleanTy())))))(env_bind(e7, "is-digit", new FunTy(new TextTy(), new BooleanTy())))))(env_bind(e6, "is-letter", new FunTy(new TextTy(), new BooleanTy())))))(env_bind(e5, "substring", new FunTy(new TextTy(), new FunTy(new IntegerTy(), new FunTy(new IntegerTy(), new TextTy())))))))(env_bind(e4, "char-at", new FunTy(new TextTy(), new FunTy(new IntegerTy(), new TextTy()))))))(env_bind(e3, "integer-to-text", new FunTy(new IntegerTy(), new TextTy())))))(env_bind(e2, "text-length", new FunTy(new TextTy(), new IntegerTy())))))(env_bind(e, "negate", new FunTy(new IntegerTy(), new IntegerTy())))))(empty_type_env());
    }

    public static UnificationState empty_unification_state()
    {
        return new UnificationState(new List<SubstEntry>(), 2L, new List<Diagnostic>());
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
        return (types_equal(a, b) ? new UnifyResult(true, st) : (a is TypeVar _mTypeVar111_ ? ((Func<long, UnifyResult>)((id_a) => (occurs_in(st, id_a, b) ? new UnifyResult(false, add_unify_error(st, "CDX2010", "Infinite type")) : new UnifyResult(true, add_subst(st, id_a, b)))))((long)_mTypeVar111_.Field0) : ((Func<CodexType, UnifyResult>)((_) => unify_rhs(st, a, b)))(a)));
    }

    public static bool types_equal(CodexType a, CodexType b)
    {
        return ((Func<CodexType, bool>)((_scrutinee112_) => (_scrutinee112_ is TypeVar _mTypeVar112_ ? ((Func<long, bool>)((id_a) => (b is TypeVar _mTypeVar113_ ? ((Func<long, bool>)((id_b) => (id_a == id_b)))((long)_mTypeVar113_.Field0) : ((Func<CodexType, bool>)((_) => false))(b))))((long)_mTypeVar112_.Field0) : (_scrutinee112_ is IntegerTy _mIntegerTy112_ ? (b is IntegerTy _mIntegerTy114_ ? true : ((Func<CodexType, bool>)((_) => false))(b)) : (_scrutinee112_ is NumberTy _mNumberTy112_ ? (b is NumberTy _mNumberTy115_ ? true : ((Func<CodexType, bool>)((_) => false))(b)) : (_scrutinee112_ is TextTy _mTextTy112_ ? (b is TextTy _mTextTy116_ ? true : ((Func<CodexType, bool>)((_) => false))(b)) : (_scrutinee112_ is BooleanTy _mBooleanTy112_ ? (b is BooleanTy _mBooleanTy117_ ? true : ((Func<CodexType, bool>)((_) => false))(b)) : (_scrutinee112_ is NothingTy _mNothingTy112_ ? (b is NothingTy _mNothingTy118_ ? true : ((Func<CodexType, bool>)((_) => false))(b)) : (_scrutinee112_ is VoidTy _mVoidTy112_ ? (b is VoidTy _mVoidTy119_ ? true : ((Func<CodexType, bool>)((_) => false))(b)) : (_scrutinee112_ is ErrorTy _mErrorTy112_ ? (b is ErrorTy _mErrorTy120_ ? true : ((Func<CodexType, bool>)((_) => false))(b)) : ((Func<CodexType, bool>)((_) => false))(_scrutinee112_)))))))))))(a);
    }

    public static UnifyResult unify_rhs(UnificationState st, CodexType a, CodexType b)
    {
        return (b is TypeVar _mTypeVar121_ ? ((Func<long, UnifyResult>)((id_b) => (occurs_in(st, id_b, a) ? new UnifyResult(false, add_unify_error(st, "CDX2010", "Infinite type")) : new UnifyResult(true, add_subst(st, id_b, a)))))((long)_mTypeVar121_.Field0) : ((Func<CodexType, UnifyResult>)((_) => unify_structural(st, a, b)))(b));
    }

    public static UnifyResult unify_structural(UnificationState st, CodexType a, CodexType b)
    {
        return ((Func<CodexType, UnifyResult>)((_scrutinee122_) => (_scrutinee122_ is IntegerTy _mIntegerTy122_ ? ((Func<CodexType, UnifyResult>)((_scrutinee123_) => (_scrutinee123_ is IntegerTy _mIntegerTy123_ ? new UnifyResult(true, st) : (_scrutinee123_ is ErrorTy _mErrorTy123_ ? new UnifyResult(true, st) : ((Func<CodexType, UnifyResult>)((_) => unify_mismatch(st, a, b)))(_scrutinee123_)))))(b) : (_scrutinee122_ is NumberTy _mNumberTy122_ ? ((Func<CodexType, UnifyResult>)((_scrutinee124_) => (_scrutinee124_ is NumberTy _mNumberTy124_ ? new UnifyResult(true, st) : (_scrutinee124_ is ErrorTy _mErrorTy124_ ? new UnifyResult(true, st) : ((Func<CodexType, UnifyResult>)((_) => unify_mismatch(st, a, b)))(_scrutinee124_)))))(b) : (_scrutinee122_ is TextTy _mTextTy122_ ? ((Func<CodexType, UnifyResult>)((_scrutinee125_) => (_scrutinee125_ is TextTy _mTextTy125_ ? new UnifyResult(true, st) : (_scrutinee125_ is ErrorTy _mErrorTy125_ ? new UnifyResult(true, st) : ((Func<CodexType, UnifyResult>)((_) => unify_mismatch(st, a, b)))(_scrutinee125_)))))(b) : (_scrutinee122_ is BooleanTy _mBooleanTy122_ ? ((Func<CodexType, UnifyResult>)((_scrutinee126_) => (_scrutinee126_ is BooleanTy _mBooleanTy126_ ? new UnifyResult(true, st) : (_scrutinee126_ is ErrorTy _mErrorTy126_ ? new UnifyResult(true, st) : ((Func<CodexType, UnifyResult>)((_) => unify_mismatch(st, a, b)))(_scrutinee126_)))))(b) : (_scrutinee122_ is NothingTy _mNothingTy122_ ? ((Func<CodexType, UnifyResult>)((_scrutinee127_) => (_scrutinee127_ is NothingTy _mNothingTy127_ ? new UnifyResult(true, st) : (_scrutinee127_ is ErrorTy _mErrorTy127_ ? new UnifyResult(true, st) : ((Func<CodexType, UnifyResult>)((_) => unify_mismatch(st, a, b)))(_scrutinee127_)))))(b) : (_scrutinee122_ is VoidTy _mVoidTy122_ ? ((Func<CodexType, UnifyResult>)((_scrutinee128_) => (_scrutinee128_ is VoidTy _mVoidTy128_ ? new UnifyResult(true, st) : (_scrutinee128_ is ErrorTy _mErrorTy128_ ? new UnifyResult(true, st) : ((Func<CodexType, UnifyResult>)((_) => unify_mismatch(st, a, b)))(_scrutinee128_)))))(b) : (_scrutinee122_ is ErrorTy _mErrorTy122_ ? new UnifyResult(true, st) : (_scrutinee122_ is FunTy _mFunTy122_ ? ((Func<CodexType, UnifyResult>)((ra) => ((Func<CodexType, UnifyResult>)((pa) => ((Func<CodexType, UnifyResult>)((_scrutinee129_) => (_scrutinee129_ is FunTy _mFunTy129_ ? ((Func<CodexType, UnifyResult>)((rb) => ((Func<CodexType, UnifyResult>)((pb) => unify_fun(st, pa, ra, pb, rb)))((CodexType)_mFunTy129_.Field0)))((CodexType)_mFunTy129_.Field1) : (_scrutinee129_ is ErrorTy _mErrorTy129_ ? new UnifyResult(true, st) : ((Func<CodexType, UnifyResult>)((_) => unify_mismatch(st, a, b)))(_scrutinee129_)))))(b)))((CodexType)_mFunTy122_.Field0)))((CodexType)_mFunTy122_.Field1) : (_scrutinee122_ is ListTy _mListTy122_ ? ((Func<CodexType, UnifyResult>)((ea) => ((Func<CodexType, UnifyResult>)((_scrutinee130_) => (_scrutinee130_ is ListTy _mListTy130_ ? ((Func<CodexType, UnifyResult>)((eb) => unify(st, ea, eb)))((CodexType)_mListTy130_.Field0) : (_scrutinee130_ is ErrorTy _mErrorTy130_ ? new UnifyResult(true, st) : ((Func<CodexType, UnifyResult>)((_) => unify_mismatch(st, a, b)))(_scrutinee130_)))))(b)))((CodexType)_mListTy122_.Field0) : (_scrutinee122_ is ConstructedTy _mConstructedTy122_ ? ((Func<List<CodexType>, UnifyResult>)((args_a) => ((Func<Name, UnifyResult>)((na) => ((Func<CodexType, UnifyResult>)((_scrutinee131_) => (_scrutinee131_ is ConstructedTy _mConstructedTy131_ ? ((Func<List<CodexType>, UnifyResult>)((args_b) => ((Func<Name, UnifyResult>)((nb) => ((na.value == nb.value) ? unify_constructed_args(st, args_a, args_b, 0L, ((long)args_a.Count)) : unify_mismatch(st, a, b))))((Name)_mConstructedTy131_.Field0)))((List<CodexType>)_mConstructedTy131_.Field1) : (_scrutinee131_ is SumTy _mSumTy131_ ? ((Func<List<SumCtor>, UnifyResult>)((sb_ctors) => ((Func<Name, UnifyResult>)((sb_name) => ((na.value == sb_name.value) ? new UnifyResult(true, st) : unify_mismatch(st, a, b))))((Name)_mSumTy131_.Field0)))((List<SumCtor>)_mSumTy131_.Field1) : (_scrutinee131_ is RecordTy _mRecordTy131_ ? ((Func<List<RecordField>, UnifyResult>)((rb_fields) => ((Func<Name, UnifyResult>)((rb_name) => ((na.value == rb_name.value) ? new UnifyResult(true, st) : unify_mismatch(st, a, b))))((Name)_mRecordTy131_.Field0)))((List<RecordField>)_mRecordTy131_.Field1) : (_scrutinee131_ is ErrorTy _mErrorTy131_ ? new UnifyResult(true, st) : ((Func<CodexType, UnifyResult>)((_) => unify_mismatch(st, a, b)))(_scrutinee131_)))))))(b)))((Name)_mConstructedTy122_.Field0)))((List<CodexType>)_mConstructedTy122_.Field1) : (_scrutinee122_ is SumTy _mSumTy122_ ? ((Func<List<SumCtor>, UnifyResult>)((sa_ctors) => ((Func<Name, UnifyResult>)((sa_name) => ((Func<CodexType, UnifyResult>)((_scrutinee132_) => (_scrutinee132_ is SumTy _mSumTy132_ ? ((Func<List<SumCtor>, UnifyResult>)((sb_ctors) => ((Func<Name, UnifyResult>)((sb_name) => ((sa_name.value == sb_name.value) ? new UnifyResult(true, st) : unify_mismatch(st, a, b))))((Name)_mSumTy132_.Field0)))((List<SumCtor>)_mSumTy132_.Field1) : (_scrutinee132_ is ConstructedTy _mConstructedTy132_ ? ((Func<List<CodexType>, UnifyResult>)((args_b) => ((Func<Name, UnifyResult>)((nb) => ((sa_name.value == nb.value) ? new UnifyResult(true, st) : unify_mismatch(st, a, b))))((Name)_mConstructedTy132_.Field0)))((List<CodexType>)_mConstructedTy132_.Field1) : (_scrutinee132_ is ErrorTy _mErrorTy132_ ? new UnifyResult(true, st) : ((Func<CodexType, UnifyResult>)((_) => unify_mismatch(st, a, b)))(_scrutinee132_))))))(b)))((Name)_mSumTy122_.Field0)))((List<SumCtor>)_mSumTy122_.Field1) : (_scrutinee122_ is RecordTy _mRecordTy122_ ? ((Func<List<RecordField>, UnifyResult>)((ra_fields) => ((Func<Name, UnifyResult>)((ra_name) => ((Func<CodexType, UnifyResult>)((_scrutinee133_) => (_scrutinee133_ is RecordTy _mRecordTy133_ ? ((Func<List<RecordField>, UnifyResult>)((rb_fields) => ((Func<Name, UnifyResult>)((rb_name) => ((ra_name.value == rb_name.value) ? new UnifyResult(true, st) : unify_mismatch(st, a, b))))((Name)_mRecordTy133_.Field0)))((List<RecordField>)_mRecordTy133_.Field1) : (_scrutinee133_ is ConstructedTy _mConstructedTy133_ ? ((Func<List<CodexType>, UnifyResult>)((args_b) => ((Func<Name, UnifyResult>)((nb) => ((ra_name.value == nb.value) ? new UnifyResult(true, st) : unify_mismatch(st, a, b))))((Name)_mConstructedTy133_.Field0)))((List<CodexType>)_mConstructedTy133_.Field1) : (_scrutinee133_ is ErrorTy _mErrorTy133_ ? new UnifyResult(true, st) : ((Func<CodexType, UnifyResult>)((_) => unify_mismatch(st, a, b)))(_scrutinee133_))))))(b)))((Name)_mRecordTy122_.Field0)))((List<RecordField>)_mRecordTy122_.Field1) : (_scrutinee122_ is ForAllTy _mForAllTy122_ ? ((Func<CodexType, UnifyResult>)((body) => ((Func<long, UnifyResult>)((id) => unify(st, body, b)))((long)_mForAllTy122_.Field0)))((CodexType)_mForAllTy122_.Field1) : ((Func<CodexType, UnifyResult>)((_) => (b is ErrorTy _mErrorTy134_ ? new UnifyResult(true, st) : ((Func<CodexType, UnifyResult>)((_) => unify_mismatch(st, a, b)))(b))))(_scrutinee122_))))))))))))))))(a);
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
        return new UnifyResult(false, add_unify_error(st, "CDX2001", string.Concat("Type mismatch: ", string.Concat(type_tag(a), string.Concat(" vs ", type_tag(b))))));
    }

    public static string type_tag(CodexType ty)
    {
        return ((Func<CodexType, string>)((_scrutinee135_) => (_scrutinee135_ is IntegerTy _mIntegerTy135_ ? "Integer" : (_scrutinee135_ is NumberTy _mNumberTy135_ ? "Number" : (_scrutinee135_ is TextTy _mTextTy135_ ? "Text" : (_scrutinee135_ is BooleanTy _mBooleanTy135_ ? "Boolean" : (_scrutinee135_ is VoidTy _mVoidTy135_ ? "Void" : (_scrutinee135_ is NothingTy _mNothingTy135_ ? "Nothing" : (_scrutinee135_ is ErrorTy _mErrorTy135_ ? "Error" : (_scrutinee135_ is FunTy _mFunTy135_ ? ((Func<CodexType, string>)((r) => ((Func<CodexType, string>)((p) => "Fun"))((CodexType)_mFunTy135_.Field0)))((CodexType)_mFunTy135_.Field1) : (_scrutinee135_ is ListTy _mListTy135_ ? ((Func<CodexType, string>)((e) => "List"))((CodexType)_mListTy135_.Field0) : (_scrutinee135_ is TypeVar _mTypeVar135_ ? ((Func<long, string>)((id) => string.Concat("T", (id).ToString())))((long)_mTypeVar135_.Field0) : (_scrutinee135_ is ForAllTy _mForAllTy135_ ? ((Func<CodexType, string>)((body) => ((Func<long, string>)((id) => "ForAll"))((long)_mForAllTy135_.Field0)))((CodexType)_mForAllTy135_.Field1) : (_scrutinee135_ is SumTy _mSumTy135_ ? ((Func<List<SumCtor>, string>)((ctors) => ((Func<Name, string>)((name) => string.Concat("Sum:", name.value)))((Name)_mSumTy135_.Field0)))((List<SumCtor>)_mSumTy135_.Field1) : (_scrutinee135_ is RecordTy _mRecordTy135_ ? ((Func<List<RecordField>, string>)((fields) => ((Func<Name, string>)((name) => string.Concat("Rec:", name.value)))((Name)_mRecordTy135_.Field0)))((List<RecordField>)_mRecordTy135_.Field1) : (_scrutinee135_ is ConstructedTy _mConstructedTy135_ ? ((Func<List<CodexType>, string>)((args) => ((Func<Name, string>)((name) => string.Concat("Con:", name.value)))((Name)_mConstructedTy135_.Field0)))((List<CodexType>)_mConstructedTy135_.Field1) : throw new InvalidOperationException("Non-exhaustive match")))))))))))))))))(ty);
    }

    public static CodexType deep_resolve(UnificationState st, CodexType ty)
    {
        return ((Func<CodexType, CodexType>)((resolved) => ((Func<CodexType, CodexType>)((_scrutinee136_) => (_scrutinee136_ is FunTy _mFunTy136_ ? ((Func<CodexType, CodexType>)((ret) => ((Func<CodexType, CodexType>)((param) => new FunTy(deep_resolve(st, param), deep_resolve(st, ret))))((CodexType)_mFunTy136_.Field0)))((CodexType)_mFunTy136_.Field1) : (_scrutinee136_ is ListTy _mListTy136_ ? ((Func<CodexType, CodexType>)((elem) => new ListTy(deep_resolve(st, elem))))((CodexType)_mListTy136_.Field0) : (_scrutinee136_ is ConstructedTy _mConstructedTy136_ ? ((Func<List<CodexType>, CodexType>)((args) => ((Func<Name, CodexType>)((name) => new ConstructedTy(name, deep_resolve_list(st, args, 0L, ((long)args.Count), new List<CodexType>()))))((Name)_mConstructedTy136_.Field0)))((List<CodexType>)_mConstructedTy136_.Field1) : (_scrutinee136_ is ForAllTy _mForAllTy136_ ? ((Func<CodexType, CodexType>)((body) => ((Func<long, CodexType>)((id) => new ForAllTy(id, deep_resolve(st, body))))((long)_mForAllTy136_.Field0)))((CodexType)_mForAllTy136_.Field1) : (_scrutinee136_ is SumTy _mSumTy136_ ? ((Func<List<SumCtor>, CodexType>)((ctors) => ((Func<Name, CodexType>)((name) => resolved))((Name)_mSumTy136_.Field0)))((List<SumCtor>)_mSumTy136_.Field1) : (_scrutinee136_ is RecordTy _mRecordTy136_ ? ((Func<List<RecordField>, CodexType>)((fields) => ((Func<Name, CodexType>)((name) => resolved))((Name)_mRecordTy136_.Field0)))((List<RecordField>)_mRecordTy136_.Field1) : ((Func<CodexType, CodexType>)((_) => resolved))(_scrutinee136_)))))))))(resolved)))(resolve(st, ty));
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
        return compile_with_imports(source, module_name, new List<ResolveResult>());
    }

    public static CompileResult compile_with_imports(string source, string module_name, List<ResolveResult> imported)
    {
        return ((Func<List<Token>, CompileResult>)((tokens) => ((Func<ParseState, CompileResult>)((st) => ((Func<Document, CompileResult>)((doc) => ((Func<AModule, CompileResult>)((ast) => ((Func<ResolveResult, CompileResult>)((resolve_result) => ((((long)resolve_result.errors.Count) > 0L) ? new CompileError(resolve_result.errors) : ((Func<ModuleResult, CompileResult>)((check_result) => ((Func<IRModule, CompileResult>)((ir) => new CompileOk(emit_full_module(ir, ast.type_defs), check_result)))(lower_module(ast, check_result.types, check_result.state))))(check_module(ast)))))(resolve_module_with_imports(ast, imported))))(desugar_document(doc, module_name))))(parse_document(st))))(make_parse_state(tokens))))(tokenize(source));
    }

    public static object main()
    {
        ((Func<object>)(() => {
                var path = Console.ReadLine();
                var source = File.ReadAllText(path);
                Console.WriteLine(compile(source, "Program"));
                return null;
            }))();
        return null;
    }

}
