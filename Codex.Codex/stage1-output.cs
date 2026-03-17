using System;
using System.Collections.Generic;
using System.Linq;

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

public sealed record ArityEntry(string name, long arity);

public sealed record ApplyChain(IRExpr root, List<IRExpr> args);

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

public sealed record LowerCtx(List<TypeBinding> types, UnificationState ust);

public sealed record Scope(List<string> names);

public sealed record ResolveResult(List<Diagnostic> errors, List<string> top_level_names, List<string> type_names, List<string> ctor_names);

public sealed record CollectResult(List<string> names, List<Diagnostic> errors);

public sealed record CtorCollectResult(List<string> type_names, List<string> ctor_names);

public sealed record LexState(string source, long offset, long line, long column);

public abstract record LexResult;
public sealed record LexToken(Token Field0, LexState Field1) : LexResult;
public sealed record LexEnd : LexResult;


public sealed record ParseState(List<Token> tokens, long pos);

public abstract record ParseExprResult;
public sealed record ExprOk(Expr Field0, ParseState Field1) : ParseExprResult;


public abstract record ParsePatResult;
public sealed record PatOk(Pat Field0, ParseState Field1) : ParsePatResult;


public abstract record ParseTypeResult;
public sealed record TypeOk(TypeExpr Field0, ParseState Field1) : ParseTypeResult;


public abstract record ParseDefResult;
public sealed record DefOk(Def Field0, ParseState Field1) : ParseDefResult;
public sealed record DefNone(ParseState Field0) : ParseDefResult;


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
public sealed record ErrExpr(Token Field0) : Expr;


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

public sealed record ModuleResult(List<TypeBinding> types, UnificationState state);

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
    public static AExpr desugar_expr(Expr node) => node switch { LitExpr(var tok) => desugar_literal(tok), NameExpr(var tok) => new ANameExpr(make_name(tok.text)), AppExpr(var f, var a) => new AApplyExpr(desugar_expr(f), desugar_expr(a)), BinExpr(var l, var op, var r) => new ABinaryExpr(desugar_expr(l), desugar_bin_op(op.kind), desugar_expr(r)), UnaryExpr(var op, var operand) => new AUnaryExpr(desugar_expr(operand)), IfExpr(var c, var t, var e) => new AIfExpr(desugar_expr(c), desugar_expr(t), desugar_expr(e)), LetExpr(var bindings, var body) => new ALetExpr((_p0_) => map_list(desugar_let_bind(bindings), _p0_), desugar_expr(body)), MatchExpr(var scrut, var arms) => new AMatchExpr(desugar_expr(scrut), (_p0_) => map_list(desugar_match_arm(arms), _p0_)), ListExpr(var elems) => new AListExpr((_p0_) => map_list(desugar_expr(elems), _p0_)), RecordExpr(var type_tok, var fields) => new ARecordExpr(make_name(type_tok.text), (_p0_) => map_list(desugar_field_expr(fields), _p0_)), FieldExpr(var rec, var field_tok) => new AFieldAccess(desugar_expr(rec), make_name(field_tok.text)), ParenExpr(var inner) => desugar_expr(inner), DoExpr(var stmts) => new ADoExpr((_p0_) => map_list(desugar_do_stmt(stmts), _p0_)), ErrExpr(var tok) => new AErrorExpr(tok.text),  };

    public static AExpr desugar_literal(Token tok) => (is_literal(tok.kind) ? new ALitExpr(tok.text(classify_literal(tok.kind))) : new AErrorExpr(tok.text));

    public static LiteralKind classify_literal(TokenKind k) => k switch { IntegerLiteral { } => IntLit, NumberLiteral { } => NumLit, TextLiteral { } => TextLit, TrueKeyword { } => BoolLit, FalseKeyword { } => BoolLit, _ => TextLit,  };

    public static ALetBind desugar_let_bind(LetBind b) => new ALetBind(name: make_name(b.name.text), value: desugar_expr(b.value));

    public static AMatchArm desugar_match_arm(MatchArm arm) => new AMatchArm(pattern: desugar_pattern(arm.pattern), body: desugar_expr(arm.body));

    public static AFieldExpr desugar_field_expr(RecordFieldExpr f) => new AFieldExpr(name: make_name(f.name.text), value: desugar_expr(f.value));

    public static ADoStmt desugar_do_stmt(DoStmt s) => s switch { DoBindStmt(var tok, var val) => new ADoBindStmt(make_name(tok.text), desugar_expr(val)), DoExprStmt(var e) => new ADoExprStmt(desugar_expr(e)),  };

    public static BinaryOp desugar_bin_op(TokenKind k) => k switch { Plus { } => OpAdd, Minus { } => OpSub, Star { } => OpMul, Slash { } => OpDiv, Caret { } => OpPow, DoubleEquals { } => OpEq, NotEquals { } => OpNotEq, LessThan { } => OpLt, GreaterThan { } => OpGt, LessOrEqual { } => OpLtEq, GreaterOrEqual { } => OpGtEq, TripleEquals { } => OpDefEq, PlusPlus { } => OpAppend, ColonColon { } => OpCons, Ampersand { } => OpAnd, Pipe { } => OpOr, _ => OpAdd,  };

    public static APat desugar_pattern(Pat p) => p switch { VarPat(var tok) => new AVarPat(make_name(tok.text)), LitPat(var tok) => new ALitPat(tok.text(classify_literal(tok.kind))), CtorPat(var tok, var subs) => new ACtorPat(make_name(tok.text), (_p0_) => map_list(desugar_pattern(subs), _p0_)), WildPat(var tok) => AWildPat,  };

    public static ATypeExpr desugar_type_expr(TypeExpr t) => t switch { NamedType(var tok) => new ANamedType(make_name(tok.text)), FunType(var param, var ret) => new AFunType(desugar_type_expr(param), desugar_type_expr(ret)), AppType(var ctor, var args) => new AAppType(desugar_type_expr(ctor), (_p0_) => map_list(desugar_type_expr(args), _p0_)), ParenType(var inner) => desugar_type_expr(inner), ListType(var elem) => new AAppType(new ANamedType(make_name("List")), new List<ATypeExpr> { desugar_type_expr(elem) }), LinearTypeExpr(var inner) => desugar_type_expr(inner),  };

    public static ADef desugar_def(Def d) => ((List<ATypeExpr> ann_types = desugar_annotations(d.ann)) is var _ ? new ADef(name: make_name(d.name.text), @params: (_p0_) => map_list(desugar_param(d.@params), _p0_), declared_type: ann_types, body: desugar_expr(d.body)) : default);

    public static List<ATypeExpr> desugar_annotations(List<TypeAnn> anns) => ((list_length(anns) == 0) ? new List<ATypeExpr>() : ((object a = list_at(anns(0))) is var _ ? new List<ATypeExpr> { desugar_type_expr(a.type_expr) } : default));

    public static AParam desugar_param(Token tok) => new AParam(name: make_name(tok.text));

    public static ATypeDef desugar_type_def(TypeDef td) => td.body switch { RecordBody(var fields) => new ARecordTypeDef(make_name(td.name.text), (_p0_) => map_list(make_type_param_name(td.type_params), _p0_), (_p0_) => map_list(desugar_record_field_def(fields), _p0_)), VariantBody(var ctors) => new AVariantTypeDef(make_name(td.name.text), (_p0_) => map_list(make_type_param_name(td.type_params), _p0_), (_p0_) => map_list(desugar_variant_ctor_def(ctors), _p0_)),  };

    public static Name make_type_param_name(Token tok) => make_name(tok.text);

    public static ARecordFieldDef desugar_record_field_def(RecordFieldDef f) => new ARecordFieldDef(name: make_name(f.name.text), type_expr: desugar_type_expr(f.type_expr));

    public static AVariantCtorDef desugar_variant_ctor_def(VariantCtorDef c) => new AVariantCtorDef(name: make_name(c.name.text), fields: (_p0_) => map_list(desugar_type_expr(c.fields), _p0_));

    public static AModule desugar_document(Document doc, string module_name) => new AModule(name: make_name(module_name), defs: (_p0_) => map_list(desugar_def(doc.defs), _p0_), type_defs: (_p0_) => map_list(desugar_type_def(doc.type_defs), _p0_));

    public static List<b> map_list(Func<a, b> f, List<a> xs) => (_p0_) => (_p1_) => (_p2_) => (_p3_) => map_list_loop(f(xs(0)(list_length(xs))(new List<object>())), _p0_, _p1_, _p2_, _p3_);

    public static List<b> map_list_loop(Func<a, b> f, List<a> xs, long i, long len, List<b> acc) => ((i == len) ? acc : (_p0_) => (_p1_) => (_p2_) => (_p3_) => map_list_loop(f(xs((i + 1))(len((acc + new List<object> { f(list_at(xs(i))) })))), _p0_, _p1_, _p2_, _p3_));

    public static b fold_list(Func<b, Func<a, b>> f, b z, List<a> xs) => (_p0_) => (_p1_) => (_p2_) => (_p3_) => fold_list_loop(f(z(xs(0)(list_length(xs)))), _p0_, _p1_, _p2_, _p3_);

    public static b fold_list_loop(Func<b, Func<a, b>> f, b z, List<a> xs, long i, long len) => ((i == len) ? z : (_p0_) => (_p1_) => (_p2_) => (_p3_) => fold_list_loop(f(f(z(list_at(xs(i)))))(xs((i + 1))(len)), _p0_, _p1_, _p2_, _p3_));

    public static Diagnostic make_error(string code, string msg) => new Diagnostic(code: code, message: msg, severity: Error);

    public static Diagnostic make_warning(string code, string msg) => new Diagnostic(code: code, message: msg, severity: Warning);

    public static string severity_label(DiagnosticSeverity s) => s switch { Error { } => "error", Warning { } => "warning", Info { } => "info",  };

    public static string diagnostic_display(Diagnostic d) => (severity_label(d.severity) + (" " + (d.code + (": " + d.message))));

    public static Name make_name(string s) => new Name(value: s);

    public static string name_value(Name n) => n.value;

    public static SourcePosition make_position(long line, long col, long offset) => new SourcePosition(line: line, column: col, offset: offset);

    public static SourceSpan make_span(SourcePosition s, SourcePosition e, string f) => new SourceSpan(start: s, end: e, file: f);

    public static long span_length(SourceSpan span) => (span.end.offset - span.start.offset);

    public static bool is_cs_keyword(string n) => ((n == "class") ? true : ((n == "static") ? true : ((n == "void") ? true : ((n == "return") ? true : ((n == "if") ? true : ((n == "else") ? true : ((n == "for") ? true : ((n == "while") ? true : ((n == "do") ? true : ((n == "switch") ? true : ((n == "case") ? true : ((n == "break") ? true : ((n == "continue") ? true : ((n == "new") ? true : ((n == "this") ? true : ((n == "base") ? true : ((n == "null") ? true : ((n == "true") ? true : ((n == "false") ? true : ((n == "int") ? true : ((n == "long") ? true : ((n == "string") ? true : ((n == "bool") ? true : ((n == "double") ? true : ((n == "decimal") ? true : ((n == "object") ? true : ((n == "in") ? true : ((n == "is") ? true : ((n == "as") ? true : ((n == "typeof") ? true : ((n == "default") ? true : ((n == "throw") ? true : ((n == "try") ? true : ((n == "catch") ? true : ((n == "finally") ? true : ((n == "using") ? true : ((n == "namespace") ? true : ((n == "public") ? true : ((n == "private") ? true : ((n == "protected") ? true : ((n == "internal") ? true : ((n == "abstract") ? true : ((n == "sealed") ? true : ((n == "override") ? true : ((n == "virtual") ? true : ((n == "event") ? true : ((n == "delegate") ? true : ((n == "out") ? true : ((n == "ref") ? true : ((n == "params") ? true : false))))))))))))))))))))))))))))))))))))))))))))))))));

    public static string sanitize(string name) => ((object s = text_replace(name("-")("_"))) is var _ ? (is_cs_keyword(s) ? ("@" + s) : s) : default);

    public static string cs_type(CodexType ty) => ty switch { IntegerTy { } => "long", NumberTy { } => "decimal", TextTy { } => "string", BooleanTy { } => "bool", VoidTy { } => "void", NothingTy { } => "object", ErrorTy { } => "object", FunTy(var p, var r) => ("Func<" + (cs_type(p) + (", " + (cs_type(r) + ">")))), ListTy(var elem) => ("List<" + (cs_type(elem) + ">")), TypeVar(var id) => ("T" + integer_to_text(id)), ForAllTy(var id, var body) => cs_type(body), SumTy(var name, var ctors) => sanitize(name.value), RecordTy(var name, var fields) => sanitize(name.value), ConstructedTy(var name, var args) => sanitize(name.value),  };

    public static List<ArityEntry> build_arity_map(List<IRDef> defs, long i) => ((i == list_length(defs)) ? new List<ArityEntry>() : ((object d = list_at(defs(i))) is var _ ? (new List<ArityEntry> { new ArityEntry(name: d.name, arity: list_length(d.@params)) } + (_p0_) => build_arity_map(defs((i + 1)), _p0_)) : default));

    public static long lookup_arity(List<ArityEntry> entries, string name) => (_p0_) => (_p1_) => (_p2_) => lookup_arity_loop(entries(name(0)(list_length(entries))), _p0_, _p1_, _p2_);

    public static long lookup_arity_loop(List<ArityEntry> entries, string name, long i, long len) => ((i == len) ? (0 - 1) : ((object e = list_at(entries(i))) is var _ ? ((e.name == name) ? e.arity : (_p0_) => (_p1_) => (_p2_) => lookup_arity_loop(entries(name((i + 1))(len)), _p0_, _p1_, _p2_)) : default));

    public static ApplyChain collect_apply_chain(IRExpr e, List<IRExpr> acc) => e switch { IrApply(var f, var a, var ty) => (_p0_) => collect_apply_chain(f((new List<object> { a } + acc)), _p0_), _ => new ApplyChain(root: e, args: acc),  };

    public static bool is_upper_letter(string c) => ((object code = char_code(c)) is var _ ? ((code >= 65) && (code <= 90)) : default);

    public static string emit_apply_args(List<IRExpr> args, List<ArityEntry> arities, long i) => ((i == list_length(args)) ? "" : ((i == (list_length(args) - 1)) ? emit_expr(list_at(args(i)), arities) : (emit_expr(list_at(args(i)), arities) + (", " + (_p0_) => (_p1_) => emit_apply_args(args(arities((i + 1))), _p0_, _p1_)))));

    public static string emit_partial_params(long i, long count) => ((i == count) ? "" : ((i == (count - 1)) ? ("_p" + (integer_to_text(i) + "_")) : ("_p" + (integer_to_text(i) + ("_" + (", " + emit_partial_params((i + 1), count)))))));

    public static string emit_partial_wrappers(long i, long count) => ((i == count) ? "" : ("(_p" + (integer_to_text(i) + ("_) => " + emit_partial_wrappers((i + 1), count)))));

    public static string emit_apply(IRExpr e, List<ArityEntry> arities) => ((Func<List<IRExpr>, ApplyChain> chain = (_p0_) => collect_apply_chain(e(new List<object>()), _p0_)) is var _ ? ((object root = chain.root) is var _ ? ((object args = chain.args) is var _ ? root switch { IrName(var n, var ty) => (((text_length(n) > 0) && is_upper_letter(char_at(n(0)))) ? ("new " + (sanitize(n) + ("(" + ((_p0_) => (_p1_) => emit_apply_args(args(arities(0)), _p0_, _p1_) + ")")))) : ((Func<string, long> ar = (_p0_) => lookup_arity(arities(n), _p0_)) is var _ ? (((ar > 1) && (list_length(args) == ar)) ? (sanitize(n) + ("(" + ((_p0_) => (_p1_) => emit_apply_args(args(arities(0)), _p0_, _p1_) + ")"))) : (((ar > 1) && (list_length(args) < ar)) ? ((object remaining = (ar - list_length(args))) is var _ ? (emit_partial_wrappers(0, remaining) + (sanitize(n) + ("(" + ((_p0_) => (_p1_) => emit_apply_args(args(arities(0)), _p0_, _p1_) + (", " + (emit_partial_params(0, remaining) + ")")))))) : default) : (_p0_) => emit_expr_curried(e(arities), _p0_))) : default)), _ => (_p0_) => emit_expr_curried(e(arities), _p0_),  } : default) : default) : default);

    public static string emit_expr_curried(IRExpr e, List<ArityEntry> arities) => e switch { IrApply(var f, var a, var ty) => ((_p0_) => emit_expr(f(arities), _p0_) + ("(" + ((_p0_) => emit_expr(a(arities), _p0_) + ")"))), _ => (_p0_) => emit_expr(e(arities), _p0_),  };

    public static string emit_expr(IRExpr e, List<ArityEntry> arities) => e switch { IrIntLit(var n) => integer_to_text(n), IrNumLit(var n) => integer_to_text(n), IrTextLit(var s) => ("\\\"" + (escape_text(s) + "\\\"")), IrBoolLit(var b) => (b ? "true" : "false"), IrName(var n, var ty) => sanitize(n), IrBinary(var op, var l, var r, var ty) => ("(" + ((_p0_) => emit_expr(l(arities), _p0_) + (" " + (emit_bin_op(op) + (" " + ((_p0_) => emit_expr(r(arities), _p0_) + ")")))))), IrNegate(var operand) => ("(-" + ((_p0_) => emit_expr(operand(arities), _p0_) + ")")), IrIf(var c, var t, var el, var ty) => ("(" + ((_p0_) => emit_expr(c(arities), _p0_) + (" ? " + ((_p0_) => emit_expr(t(arities), _p0_) + (" : " + ((_p0_) => emit_expr(el(arities), _p0_) + ")")))))), IrLet(var name, var ty, var val, var body) => (_p0_) => (_p1_) => (_p2_) => (_p3_) => emit_let(name(ty(val(body(arities)))), _p0_, _p1_, _p2_, _p3_), IrApply(var f, var a, var ty) => (_p0_) => emit_apply(e(arities), _p0_), IrLambda(var @params, var body, var ty) => (_p0_) => (_p1_) => emit_lambda(@params(body(arities)), _p0_, _p1_), IrList(var elems, var ty) => (_p0_) => (_p1_) => emit_list(elems(ty(arities)), _p0_, _p1_), IrMatch(var scrut, var branches, var ty) => (_p0_) => (_p1_) => (_p2_) => emit_match(scrut(branches(ty(arities))), _p0_, _p1_, _p2_), IrDo(var stmts, var ty) => (_p0_) => emit_do(stmts(arities), _p0_), IrRecord(var name, var fields, var ty) => (_p0_) => (_p1_) => emit_record(name(fields(arities)), _p0_, _p1_), IrFieldAccess(var rec, var field, var ty) => ((_p0_) => emit_expr(rec(arities), _p0_) + ("." + sanitize(field))), IrError(var msg, var ty) => ("/* error: " + (msg + " */ default")),  };

    public static string escape_text(string s) => text_replace(text_replace(s("\\\\")("\\\\\\\\")))("\\\"")("\\\\\\\"");

    public static string emit_bin_op(IRBinaryOp op) => op switch { IrAddInt { } => "+", IrSubInt { } => "-", IrMulInt { } => "*", IrDivInt { } => "/", IrPowInt { } => "^", IrAddNum { } => "+", IrSubNum { } => "-", IrMulNum { } => "*", IrDivNum { } => "/", IrEq { } => "==", IrNotEq { } => "!=", IrLt { } => "<", IrGt { } => ">", IrLtEq { } => "<=", IrGtEq { } => ">=", IrAnd { } => "&&", IrOr { } => "||", IrAppendText { } => "+", IrAppendList { } => "+", IrConsList { } => "+",  };

    public static string emit_let(string name, CodexType ty, IRExpr val, IRExpr body, List<ArityEntry> arities) => ("((" + (cs_type(ty) + (" " + (sanitize(name) + (" = " + ((_p0_) => emit_expr(val(arities), _p0_) + (") is var _ ? " + ((_p0_) => emit_expr(body(arities), _p0_) + " : default)"))))))));

    public static string emit_lambda(List<IRParam> @params, IRExpr body, List<ArityEntry> arities) => ((list_length(@params) == 0) ? ("(() => " + ((_p0_) => emit_expr(body(arities), _p0_) + ")")) : ((list_length(@params) == 1) ? ((object p = list_at(@params(0))) is var _ ? ("((" + (cs_type(p.type_val) + (" " + (sanitize(p.name) + (") => " + ((_p0_) => emit_expr(body(arities), _p0_) + ")")))))) : default) : ("(() => " + ((_p0_) => emit_expr(body(arities), _p0_) + ")"))));

    public static string emit_list(List<IRExpr> elems, CodexType ty, List<ArityEntry> arities) => ((list_length(elems) == 0) ? ("new List<" + (cs_type(ty) + ">()")) : ("new List<" + (cs_type(ty) + ("> { " + ((_p0_) => (_p1_) => emit_list_elems(elems(0)(arities), _p0_, _p1_) + " }")))));

    public static string emit_list_elems(List<IRExpr> elems, long i, List<ArityEntry> arities) => ((i == list_length(elems)) ? "" : ((i == (list_length(elems) - 1)) ? emit_expr(list_at(elems(i)), arities) : (emit_expr(list_at(elems(i)), arities) + (", " + (_p0_) => (_p1_) => emit_list_elems(elems((i + 1))(arities), _p0_, _p1_)))));

    public static string emit_match(IRExpr scrut, List<IRBranch> branches, CodexType ty, List<ArityEntry> arities) => ((_p0_) => emit_expr(scrut(arities), _p0_) + (" switch { " + ((_p0_) => (_p1_) => emit_match_arms(branches(0)(arities), _p0_, _p1_) + " }")));

    public static string emit_match_arms(List<IRBranch> branches, long i, List<ArityEntry> arities) => ((i == list_length(branches)) ? "" : ((object arm = list_at(branches(i))) is var _ ? (emit_pattern(arm.pattern) + (" => " + ((_p0_) => emit_expr(arm.body(arities), _p0_) + (", " + (_p0_) => (_p1_) => emit_match_arms(branches((i + 1))(arities), _p0_, _p1_))))) : default));

    public static string emit_pattern(IRPat p) => p switch { IrVarPat(var name, var ty) => (cs_type(ty) + (" " + sanitize(name))), IrLitPat(var text, var ty) => text, IrCtorPat(var name, var subs, var ty) => ((list_length(subs) == 0) ? (sanitize(name) + " { }") : (sanitize(name) + ("(" + ((_p0_) => emit_sub_patterns(subs(0), _p0_) + ")")))), IrWildPat { } => "_",  };

    public static string emit_sub_patterns(List<IRPat> subs, long i) => ((i == list_length(subs)) ? "" : ((object sub = list_at(subs(i))) is var _ ? (emit_sub_pattern(sub) + (((i < (list_length(subs) - 1)) ? ", " : "") + (_p0_) => emit_sub_patterns(subs((i + 1)), _p0_))) : default));

    public static string emit_sub_pattern(IRPat p) => p switch { IrVarPat(var name, var ty) => ("var " + sanitize(name)), IrCtorPat(var name, var subs, var ty) => emit_pattern(p), IrWildPat { } => "_", IrLitPat(var text, var ty) => text,  };

    public static string emit_do(List<IRDoStmt> stmts, List<ArityEntry> arities) => ("{ " + ((_p0_) => (_p1_) => emit_do_stmts(stmts(0)(arities), _p0_, _p1_) + " }"));

    public static string emit_do_stmts(List<IRDoStmt> stmts, long i, List<ArityEntry> arities) => ((i == list_length(stmts)) ? "" : ((object s = list_at(stmts(i))) is var _ ? ((_p0_) => emit_do_stmt(s(arities), _p0_) + (" " + (_p0_) => (_p1_) => emit_do_stmts(stmts((i + 1))(arities), _p0_, _p1_))) : default));

    public static string emit_do_stmt(IRDoStmt s, List<ArityEntry> arities) => s switch { IrDoBind(var name, var ty, var val) => ("var " + (sanitize(name) + (" = " + ((_p0_) => emit_expr(val(arities), _p0_) + ";")))), IrDoExec(var e) => ((_p0_) => emit_expr(e(arities), _p0_) + ";"),  };

    public static string emit_record(string name, List<IRFieldVal> fields, List<ArityEntry> arities) => ("new " + (sanitize(name) + ("(" + ((_p0_) => (_p1_) => emit_record_fields(fields(0)(arities), _p0_, _p1_) + ")"))));

    public static string emit_record_fields(List<IRFieldVal> fields, long i, List<ArityEntry> arities) => ((i == list_length(fields)) ? "" : ((object f = list_at(fields(i))) is var _ ? (sanitize(f.name) + (": " + ((_p0_) => emit_expr(f.value(arities), _p0_) + (((i < (list_length(fields) - 1)) ? ", " : "") + (_p0_) => (_p1_) => emit_record_fields(fields((i + 1))(arities), _p0_, _p1_))))) : default));

    public static string emit_type_defs(List<ATypeDef> tds, long i) => ((i == list_length(tds)) ? "" : (emit_type_def(list_at(tds(i))) + ("\\n" + (_p0_) => emit_type_defs(tds((i + 1)), _p0_))));

    public static string emit_type_def(ATypeDef td) => td switch { ARecordTypeDef(var name, var tparams, var fields) => ((string gen = emit_tparam_suffix(tparams)) is var _ ? ("public sealed record " + (sanitize(name.value) + (gen + ("(" + ((_p0_) => (_p1_) => emit_record_field_defs(fields(tparams(0)), _p0_, _p1_) + ");\\n"))))) : default), AVariantTypeDef(var name, var tparams, var ctors) => ((string gen = emit_tparam_suffix(tparams)) is var _ ? ("public abstract record " + (sanitize(name.value) + (gen + (";\\n" + ((_p0_) => (_p1_) => (_p2_) => emit_variant_ctors(ctors(name(tparams(0))), _p0_, _p1_, _p2_) + "\\n"))))) : default),  };

    public static string emit_tparam_suffix(List<Name> tparams) => ((list_length(tparams) == 0) ? "" : ("<" + ((_p0_) => emit_tparam_names(tparams(0), _p0_) + ">")));

    public static string emit_tparam_names(List<Name> tparams, long i) => ((i == list_length(tparams)) ? "" : ((i == (list_length(tparams) - 1)) ? ("T" + integer_to_text(i)) : ("T" + (integer_to_text(i) + (", " + (_p0_) => emit_tparam_names(tparams((i + 1)), _p0_))))));

    public static string emit_record_field_defs(List<ARecordFieldDef> fields, List<Name> tparams, long i) => ((i == list_length(fields)) ? "" : ((object f = list_at(fields(i))) is var _ ? ((_p0_) => emit_type_expr_tp(f.type_expr(tparams), _p0_) + (" " + (sanitize(f.name.value) + (((i < (list_length(fields) - 1)) ? ", " : "") + (_p0_) => (_p1_) => emit_record_field_defs(fields(tparams((i + 1))), _p0_, _p1_))))) : default));

    public static string emit_variant_ctors(List<AVariantCtorDef> ctors, Name base_name, List<Name> tparams, long i) => ((i == list_length(ctors)) ? "" : ((object c = list_at(ctors(i))) is var _ ? ((_p0_) => (_p1_) => emit_variant_ctor(c(base_name(tparams)), _p0_, _p1_) + (_p0_) => (_p1_) => (_p2_) => emit_variant_ctors(ctors(base_name(tparams((i + 1)))), _p0_, _p1_, _p2_)) : default));

    public static string emit_variant_ctor(AVariantCtorDef c, Name base_name, List<Name> tparams) => ((string gen = emit_tparam_suffix(tparams)) is var _ ? ((list_length(c.fields) == 0) ? ("public sealed record " + (sanitize(c.name.value) + (gen + (" : " + (sanitize(base_name.value) + (gen + ";\\n")))))) : ("public sealed record " + (sanitize(c.name.value) + (gen + ("(" + ((_p0_) => (_p1_) => emit_ctor_fields(c.fields(tparams(0)), _p0_, _p1_) + (") : " + (sanitize(base_name.value) + (gen + ";\\n"))))))))) : default);

    public static string emit_ctor_fields(List<ATypeExpr> fields, List<Name> tparams, long i) => ((i == list_length(fields)) ? "" : (emit_type_expr_tp(list_at(fields(i)), tparams) + (" Field" + (integer_to_text(i) + (((i < (list_length(fields) - 1)) ? ", " : "") + (_p0_) => (_p1_) => emit_ctor_fields(fields(tparams((i + 1))), _p0_, _p1_))))));

    public static string emit_type_expr(ATypeExpr te) => (_p0_) => emit_type_expr_tp(te(new List<object>()), _p0_);

    public static string emit_type_expr_tp(ATypeExpr te, List<Name> tparams) => te switch { ANamedType(var name) => ((Func<string, Func<long, long>> idx = (_p0_) => (_p1_) => find_tparam_index(tparams(name.value(0)), _p0_, _p1_)) is var _ ? ((idx >= 0) ? ("T" + integer_to_text(idx)) : when_type_name(name.value)) : default), AFunType(var p, var r) => ("Func<" + ((_p0_) => emit_type_expr_tp(p(tparams), _p0_) + (", " + ((_p0_) => emit_type_expr_tp(r(tparams), _p0_) + ">")))), AAppType(var @base, var args) => ((_p0_) => emit_type_expr_tp(@base(tparams), _p0_) + ("<" + ((_p0_) => (_p1_) => emit_type_expr_list_tp(args(tparams(0)), _p0_, _p1_) + ">"))),  };

    public static long find_tparam_index(List<Name> tparams, string name, long i) => ((i == list_length(tparams)) ? (0 - 1) : ((list_at(tparams(i)).value == name) ? i : (_p0_) => (_p1_) => find_tparam_index(tparams(name((i + 1))), _p0_, _p1_)));

    public static string when_type_name(string n) => ((n == "Integer") ? "long" : ((n == "Number") ? "decimal" : ((n == "Text") ? "string" : ((n == "Boolean") ? "bool" : ((n == "List") ? "List" : sanitize(n))))));

    public static string emit_type_expr_list(List<ATypeExpr> args, long i) => ((i == list_length(args)) ? "" : (emit_type_expr(list_at(args(i))) + (((i < (list_length(args) - 1)) ? ", " : "") + (_p0_) => emit_type_expr_list(args((i + 1)), _p0_))));

    public static string emit_type_expr_list_tp(List<ATypeExpr> args, List<Name> tparams, long i) => ((i == list_length(args)) ? "" : (emit_type_expr_tp(list_at(args(i)), tparams) + (((i < (list_length(args) - 1)) ? ", " : "") + (_p0_) => (_p1_) => emit_type_expr_list_tp(args(tparams((i + 1))), _p0_, _p1_))));

    public static List<long> collect_type_var_ids(CodexType ty, List<long> acc) => ty switch { TypeVar(var id) => ((_p0_) => list_contains_int(acc(id), _p0_) ? acc : (_p0_) => list_append_int(acc(id), _p0_)), FunTy(var p, var r) => (_p0_) => collect_type_var_ids(r((_p0_) => collect_type_var_ids(p(acc), _p0_)), _p0_), ListTy(var elem) => (_p0_) => collect_type_var_ids(elem(acc), _p0_), ForAllTy(var id, var body) => (_p0_) => collect_type_var_ids(body(acc), _p0_), ConstructedTy(var name, var args) => (_p0_) => collect_type_var_ids_list(args(acc), _p0_), _ => acc,  };

    public static List<long> collect_type_var_ids_list(List<CodexType> types, List<long> acc) => (_p0_) => (_p1_) => (_p2_) => collect_type_var_ids_list_loop(types(acc(0)(list_length(types))), _p0_, _p1_, _p2_);

    public static List<long> collect_type_var_ids_list_loop(List<CodexType> types, List<long> acc, long i, long len) => ((i == len) ? acc : (_p0_) => (_p1_) => (_p2_) => collect_type_var_ids_list_loop(types(collect_type_var_ids(list_at(types(i)), acc))((i + 1))(len), _p0_, _p1_, _p2_));

    public static bool list_contains_int(List<long> xs, long n) => (_p0_) => (_p1_) => (_p2_) => list_contains_int_loop(xs(n(0)(list_length(xs))), _p0_, _p1_, _p2_);

    public static bool list_contains_int_loop(List<long> xs, long n, long i, long len) => ((i == len) ? false : ((list_at(xs(i)) == n) ? true : (_p0_) => (_p1_) => (_p2_) => list_contains_int_loop(xs(n((i + 1))(len)), _p0_, _p1_, _p2_)));

    public static List<long> list_append_int(List<long> xs, long n) => (xs + new List<long> { n });

    public static string generic_suffix(CodexType ty) => ((Func<List<long>, List<long>> ids = (_p0_) => collect_type_var_ids(ty(new List<object>()), _p0_)) is var _ ? ((list_length(ids) == 0) ? "" : ("<" + ((_p0_) => emit_type_params(ids(0), _p0_) + ">"))) : default);

    public static string emit_type_params(List<long> ids, long i) => ((i == list_length(ids)) ? "" : ((i == (list_length(ids) - 1)) ? ("T" + integer_to_text(list_at(ids(i)))) : ("T" + (integer_to_text(list_at(ids(i))) + (", " + (_p0_) => emit_type_params(ids((i + 1)), _p0_))))));

    public static string emit_def(IRDef d, List<ArityEntry> arities) => ((Func<long, CodexType> ret = (_p0_) => get_return_type(d.type_val(list_length(d.@params)), _p0_)) is var _ ? ((string gen = generic_suffix(d.type_val)) is var _ ? ("    public static " + (cs_type(ret) + (" " + (sanitize(d.name) + (gen + ("(" + ((_p0_) => emit_def_params(d.@params(0), _p0_) + (") => " + ((_p0_) => emit_expr(d.body(arities), _p0_) + ";\\n"))))))))) : default) : default);

    public static CodexType get_return_type(CodexType ty, long n) => ((n == 0) ? strip_forall(ty) : strip_forall(ty) switch { FunTy(var p, var r) => (_p0_) => get_return_type(r((n - 1)), _p0_), _ => ty,  });

    public static CodexType strip_forall(CodexType ty) => ty switch { ForAllTy(var id, var body) => strip_forall(body), _ => ty,  };

    public static string emit_def_params(List<IRParam> @params, long i) => ((i == list_length(@params)) ? "" : ((object p = list_at(@params(i))) is var _ ? (cs_type(p.type_val) + (" " + (sanitize(p.name) + (((i < (list_length(@params) - 1)) ? ", " : "") + (_p0_) => emit_def_params(@params((i + 1)), _p0_))))) : default));

    public static string emit_full_module(IRModule m, List<ATypeDef> type_defs) => ((Func<long, List<ArityEntry>> arities = (_p0_) => build_arity_map(m.defs(0), _p0_)) is var _ ? ("using System;\\nusing System.Collections.Generic;\\nusing System.Linq;\\n\\n" + ((_p0_) => emit_type_defs(type_defs(0), _p0_) + (emit_class_header(m.name.value) + ((_p0_) => (_p1_) => emit_defs(m.defs(0)(arities), _p0_, _p1_) + "}\\n")))) : default);

    public static string emit_module(IRModule m) => ((Func<long, List<ArityEntry>> arities = (_p0_) => build_arity_map(m.defs(0), _p0_)) is var _ ? ("using System;\\nusing System.Collections.Generic;\\nusing System.Linq;\\n\\n" + (emit_class_header(m.name.value) + ((_p0_) => (_p1_) => emit_defs(m.defs(0)(arities), _p0_, _p1_) + "}\\n"))) : default);

    public static string emit_class_header(string name) => ("public static class Codex_" + (sanitize(name) + "\\n{\\n"));

    public static string emit_defs(List<IRDef> defs, long i, List<ArityEntry> arities) => ((i == list_length(defs)) ? "" : (emit_def(list_at(defs(i)), arities) + ("\\n" + (_p0_) => (_p1_) => emit_defs(defs((i + 1))(arities), _p0_, _p1_))));

    public static CodexType ir_expr_type(IRExpr e) => e switch { IrIntLit(var v) => IntegerTy, IrNumLit(var v) => IntegerTy, IrTextLit(var v) => TextTy, IrBoolLit(var v) => BooleanTy, IrName(var n, var t) => t, IrBinary(var op, var l, var r, var t) => t, IrNegate(var x) => IntegerTy, IrIf(var c, var th, var el, var t) => t, IrLet(var n, var t, var v, var b) => ir_expr_type(b), IrApply(var f, var a, var t) => t, IrLambda(var ps, var b, var t) => t, IrList(var es, var t) => new ListTy(t), IrMatch(var s, var bs, var t) => t, IrDo(var ss, var t) => t, IrRecord(var n, var fs, var t) => t, IrFieldAccess(var r, var f, var t) => t, IrError(var m, var t) => t,  };

    public static CodexType lookup_type(List<TypeBinding> bindings, string name) => (_p0_) => (_p1_) => (_p2_) => lookup_type_loop(bindings(name(0)(list_length(bindings))), _p0_, _p1_, _p2_);

    public static CodexType lookup_type_loop(List<TypeBinding> bindings, string name, long i, long len) => ((i == len) ? ErrorTy : ((object b = list_at(bindings(i))) is var _ ? ((b.name == name) ? b.bound_type : (_p0_) => (_p1_) => (_p2_) => lookup_type_loop(bindings(name((i + 1))(len)), _p0_, _p1_, _p2_)) : default));

    public static CodexType peel_fun_param(CodexType ty) => ty switch { FunTy(var p, var r) => p, ForAllTy(var id, var body) => peel_fun_param(body), _ => ErrorTy,  };

    public static CodexType peel_fun_return(CodexType ty) => ty switch { FunTy(var p, var r) => r, ForAllTy(var id, var body) => peel_fun_return(body), _ => ErrorTy,  };

    public static CodexType strip_forall_ty(CodexType ty) => ty switch { ForAllTy(var id, var body) => strip_forall_ty(body), _ => ty,  };

    public static IRBinaryOp lower_bin_op(BinaryOp op, CodexType ty) => op switch { OpAdd { } => IrAddInt, OpSub { } => IrSubInt, OpMul { } => IrMulInt, OpDiv { } => IrDivInt, OpPow { } => IrPowInt, OpEq { } => IrEq, OpNotEq { } => IrNotEq, OpLt { } => IrLt, OpGt { } => IrGt, OpLtEq { } => IrLtEq, OpGtEq { } => IrGtEq, OpDefEq { } => IrEq, OpAppend { } => (is_text_type(ty) ? IrAppendText : IrAppendList), OpCons { } => IrConsList, OpAnd { } => IrAnd, OpOr { } => IrOr,  };

    public static bool is_text_type(CodexType ty) => ty switch { TextTy { } => true, _ => false,  };

    public static IRExpr lower_expr(AExpr e, CodexType ty, LowerCtx ctx) => e switch { ALitExpr(var text, var kind) => (_p0_) => lower_literal(text(kind), _p0_), ANameExpr(var name) => (_p0_) => (_p1_) => lower_name(name.value(ty(ctx)), _p0_, _p1_), AApplyExpr(var f, var a) => (_p0_) => (_p1_) => (_p2_) => lower_apply(f(a(ty(ctx))), _p0_, _p1_, _p2_), ABinaryExpr(var l, var op, var r) => ((Func<CodexType, Func<LowerCtx, IRExpr>> left_ir = (_p0_) => (_p1_) => lower_expr(l(ty(ctx)), _p0_, _p1_)) is var _ ? ((Func<CodexType, Func<LowerCtx, IRExpr>> right_ir = (_p0_) => (_p1_) => lower_expr(r(ty(ctx)), _p0_, _p1_)) is var _ ? new IrBinary((_p0_) => lower_bin_op(op(ir_expr_type(left_ir)), _p0_), left_ir(right_ir(ty))) : default) : default), AUnaryExpr(var operand) => new IrNegate((_p0_) => (_p1_) => lower_expr(operand(IntegerTy)(ctx), _p0_, _p1_)), AIfExpr(var c, var t, var e2) => new IrIf((_p0_) => (_p1_) => lower_expr(c(BooleanTy)(ctx), _p0_, _p1_), (_p0_) => (_p1_) => lower_expr(t(ty(ctx)), _p0_, _p1_), (_p0_) => (_p1_) => lower_expr(e2(ty(ctx)), _p0_, _p1_), ty), ALetExpr(var binds, var body) => (_p0_) => (_p1_) => (_p2_) => lower_let(binds(body(ty(ctx))), _p0_, _p1_, _p2_), ALambdaExpr(var @params, var body) => (_p0_) => (_p1_) => (_p2_) => lower_lambda(@params(body(ty(ctx))), _p0_, _p1_, _p2_), AMatchExpr(var scrut, var arms) => (_p0_) => (_p1_) => (_p2_) => lower_match(scrut(arms(ty(ctx))), _p0_, _p1_, _p2_), AListExpr(var elems) => (_p0_) => (_p1_) => lower_list(elems(ty(ctx)), _p0_, _p1_), ARecordExpr(var name, var fields) => (_p0_) => (_p1_) => (_p2_) => lower_record(name(fields(ty(ctx))), _p0_, _p1_, _p2_), AFieldAccess(var rec, var field) => ((Func<CodexType, Func<LowerCtx, IRExpr>> rec_ir = (_p0_) => (_p1_) => lower_expr(rec(ErrorTy)(ctx), _p0_, _p1_)) is var _ ? new IrFieldAccess(rec_ir(field.value(ty))) : default), ADoExpr(var stmts) => (_p0_) => (_p1_) => lower_do(stmts(ty(ctx)), _p0_, _p1_), AErrorExpr(var msg) => new IrError(msg(ty)),  };

    public static IRExpr lower_name(string name, CodexType ty, LowerCtx ctx) => ((Func<string, CodexType> raw = (_p0_) => lookup_type(ctx.types(name), _p0_)) is var _ ? raw switch { ErrorTy { } => new IrName(name(ty)), _ => ((Func<CodexType, CodexType> resolved = (_p0_) => deep_resolve(ctx.ust(raw), _p0_)) is var _ ? ((CodexType stripped = strip_forall_ty(resolved)) is var _ ? new IrName(name(stripped)) : default) : default),  } : default);

    public static IRExpr lower_literal(string text, LiteralKind kind) => kind switch { IntLit { } => new IrIntLit(text_to_integer(text)), NumLit { } => new IrIntLit(text_to_integer(text)), TextLit { } => new IrTextLit(text), BoolLit { } => new IrBoolLit((text == "True")),  };

    public static IRExpr lower_apply(AExpr f, AExpr a, CodexType ty, LowerCtx ctx) => ((Func<CodexType, Func<LowerCtx, IRExpr>> func_ir = (_p0_) => (_p1_) => lower_expr(f(ErrorTy)(ctx), _p0_, _p1_)) is var _ ? ((CodexType func_ty = ir_expr_type(func_ir)) is var _ ? ((CodexType arg_ty = peel_fun_param(func_ty)) is var _ ? ((CodexType ret_ty = peel_fun_return(func_ty)) is var _ ? ((object actual_ret = ret_ty switch { ErrorTy { } => ty, _ => ret_ty,  }) is var _ ? ((Func<CodexType, Func<LowerCtx, IRExpr>> arg_ir = (_p0_) => (_p1_) => lower_expr(a(arg_ty(ctx)), _p0_, _p1_)) is var _ ? new IrApply(func_ir(arg_ir(actual_ret))) : default) : default) : default) : default) : default) : default);

    public static IRExpr lower_let(List<ALetBind> binds, AExpr body, CodexType ty, LowerCtx ctx) => ((list_length(binds) == 0) ? (_p0_) => (_p1_) => lower_expr(body(ty(ctx)), _p0_, _p1_) : ((object b = list_at(binds(0))) is var _ ? ((Func<CodexType, Func<LowerCtx, IRExpr>> val_ir = (_p0_) => (_p1_) => lower_expr(b.value(ErrorTy)(ctx), _p0_, _p1_)) is var _ ? ((CodexType val_ty = ir_expr_type(val_ir)) is var _ ? ((object ctx2 = new LowerCtx(types: (new List<object> { new TypeBinding(name: b.name.value, bound_type: val_ty) } + ctx.types), ust: ctx.ust)) is var _ ? new IrLet(b.name.value(val_ty(val_ir((_p0_) => (_p1_) => (_p2_) => (_p3_) => lower_let_rest(binds(body(ty(ctx2(1)))), _p0_, _p1_, _p2_, _p3_))))) : default) : default) : default) : default));

    public static IRExpr lower_let_rest(List<ALetBind> binds, AExpr body, CodexType ty, LowerCtx ctx, long i) => ((i == list_length(binds)) ? (_p0_) => (_p1_) => lower_expr(body(ty(ctx)), _p0_, _p1_) : ((object b = list_at(binds(i))) is var _ ? ((Func<CodexType, Func<LowerCtx, IRExpr>> val_ir = (_p0_) => (_p1_) => lower_expr(b.value(ErrorTy)(ctx), _p0_, _p1_)) is var _ ? ((CodexType val_ty = ir_expr_type(val_ir)) is var _ ? ((object ctx2 = new LowerCtx(types: (new List<object> { new TypeBinding(name: b.name.value, bound_type: val_ty) } + ctx.types), ust: ctx.ust)) is var _ ? new IrLet(b.name.value(val_ty(val_ir((_p0_) => (_p1_) => (_p2_) => (_p3_) => lower_let_rest(binds(body(ty(ctx2((i + 1))))), _p0_, _p1_, _p2_, _p3_))))) : default) : default) : default) : default));

    public static IRExpr lower_lambda(List<Name> @params, AExpr body, CodexType ty, LowerCtx ctx) => ((CodexType stripped = strip_forall_ty(ty)) is var _ ? ((Func<CodexType, Func<long, List<IRParam>>> lparams = (_p0_) => (_p1_) => lower_lambda_params(@params(stripped(0)), _p0_, _p1_)) is var _ ? ((Func<List<Name>, Func<CodexType, Func<long, LowerCtx>>> lctx = (_p0_) => (_p1_) => (_p2_) => bind_lambda_to_ctx(ctx(@params(stripped(0))), _p0_, _p1_, _p2_)) is var _ ? new IrLambda(lparams((_p0_) => (_p1_) => lower_expr(body((_p0_) => get_lambda_return(stripped(list_length(@params)), _p0_))(lctx), _p0_, _p1_))(ty)) : default) : default) : default);

    public static LowerCtx bind_lambda_to_ctx(LowerCtx ctx, List<Name> @params, CodexType ty, long i) => ((i == list_length(@params)) ? ctx : ((object p = list_at(@params(i))) is var _ ? ((CodexType param_ty = peel_fun_param(ty)) is var _ ? ((CodexType rest_ty = peel_fun_return(ty)) is var _ ? ((object ctx2 = new LowerCtx(types: (new List<object> { new TypeBinding(name: p.value, bound_type: param_ty) } + ctx.types), ust: ctx.ust)) is var _ ? (_p0_) => (_p1_) => (_p2_) => bind_lambda_to_ctx(ctx2(@params(rest_ty((i + 1)))), _p0_, _p1_, _p2_) : default) : default) : default) : default));

    public static List<IRParam> lower_lambda_params(List<Name> @params, CodexType ty, long i) => ((i == list_length(@params)) ? new List<IRParam>() : ((object p = list_at(@params(i))) is var _ ? ((CodexType param_ty = peel_fun_param(ty)) is var _ ? ((CodexType rest_ty = peel_fun_return(ty)) is var _ ? (new List<IRParam> { new IRParam(name: p.value, type_val: param_ty) } + (_p0_) => (_p1_) => lower_lambda_params(@params(rest_ty((i + 1))), _p0_, _p1_)) : default) : default) : default));

    public static CodexType get_lambda_return(CodexType ty, long n) => ((n == 0) ? ty : ty switch { FunTy(var p, var r) => (_p0_) => get_lambda_return(r((n - 1)), _p0_), _ => ErrorTy,  });

    public static IRExpr lower_match(AExpr scrut, List<AMatchArm> arms, CodexType ty, LowerCtx ctx) => ((Func<CodexType, Func<LowerCtx, IRExpr>> scrut_ir = (_p0_) => (_p1_) => lower_expr(scrut(ErrorTy)(ctx), _p0_, _p1_)) is var _ ? ((CodexType scrut_ty = ir_expr_type(scrut_ir)) is var _ ? new IrMatch(scrut_ir((_p0_) => (_p1_) => (_p2_) => (_p3_) => (_p4_) => lower_match_arms_loop(arms(ty(scrut_ty(ctx(0)(list_length(arms))))), _p0_, _p1_, _p2_, _p3_, _p4_))(ty)) : default) : default);

    public static List<IRBranch> lower_match_arms_loop(List<AMatchArm> arms, CodexType ty, CodexType scrut_ty, LowerCtx ctx, long i, long len) => ((i == len) ? new List<IRBranch>() : ((object arm = list_at(arms(i))) is var _ ? ((Func<APat, Func<CodexType, LowerCtx>> arm_ctx = (_p0_) => (_p1_) => bind_pattern_to_ctx(ctx(arm.pattern(scrut_ty)), _p0_, _p1_)) is var _ ? (new List<IRBranch> { new IRBranch(pattern: lower_pattern(arm.pattern), body: (_p0_) => (_p1_) => lower_expr(arm.body(ty(arm_ctx)), _p0_, _p1_)) } + (_p0_) => (_p1_) => (_p2_) => (_p3_) => (_p4_) => lower_match_arms_loop(arms(ty(scrut_ty(ctx((i + 1))(len)))), _p0_, _p1_, _p2_, _p3_, _p4_)) : default) : default));

    public static LowerCtx bind_pattern_to_ctx(LowerCtx ctx, APat pat, CodexType ty) => pat switch { AVarPat(var name) => new LowerCtx(types: (new List<object> { new TypeBinding(name: name.value, bound_type: ty) } + ctx.types), ust: ctx.ust), ACtorPat(var ctor_name, var sub_pats) => ((Func<string, CodexType> ctor_raw = (_p0_) => lookup_type(ctx.types(ctor_name.value), _p0_)) is var _ ? ((Func<CodexType, CodexType> ctor_ty = (_p0_) => deep_resolve(ctx.ust(ctor_raw), _p0_)) is var _ ? ((CodexType ctor_stripped = strip_forall_ty(ctor_ty)) is var _ ? (_p0_) => (_p1_) => (_p2_) => (_p3_) => bind_ctor_pattern_fields(ctx(sub_pats(ctor_stripped(0)(list_length(sub_pats)))), _p0_, _p1_, _p2_, _p3_) : default) : default) : default), AWildPat { } => ctx, ALitPat(var text, var kind) => ctx,  };

    public static LowerCtx bind_ctor_pattern_fields(LowerCtx ctx, List<APat> sub_pats, CodexType ctor_ty, long i, long len) => ((i == len) ? ctx : ctor_ty switch { FunTy(var param_ty, var ret_ty) => ((Func<APat, Func<CodexType, LowerCtx>> ctx2 = (_p0_) => (_p1_) => bind_pattern_to_ctx(ctx(list_at(sub_pats(i)))(param_ty), _p0_, _p1_)) is var _ ? (_p0_) => (_p1_) => (_p2_) => (_p3_) => bind_ctor_pattern_fields(ctx2(sub_pats(ret_ty((i + 1))(len))), _p0_, _p1_, _p2_, _p3_) : default), _ => ((Func<APat, Func<CodexType, LowerCtx>> ctx2 = (_p0_) => (_p1_) => bind_pattern_to_ctx(ctx(list_at(sub_pats(i)))(ErrorTy), _p0_, _p1_)) is var _ ? (_p0_) => (_p1_) => (_p2_) => (_p3_) => bind_ctor_pattern_fields(ctx2(sub_pats(ctor_ty((i + 1))(len))), _p0_, _p1_, _p2_, _p3_) : default),  });

    public static IRPat lower_pattern(APat p) => p switch { AVarPat(var name) => new IrVarPat(name.value(ErrorTy)), ALitPat(var text, var kind) => new IrLitPat(text(ErrorTy)), ACtorPat(var name, var subs) => new IrCtorPat(name.value((_p0_) => map_list(lower_pattern(subs), _p0_))(ErrorTy)), AWildPat { } => IrWildPat,  };

    public static IRExpr lower_list(List<AExpr> elems, CodexType ty, LowerCtx ctx) => ((object elem_ty = ty switch { ListTy(var e) => e, _ => ErrorTy,  }) is var _ ? new IrList((_p0_) => (_p1_) => (_p2_) => (_p3_) => lower_list_elems_loop(elems(elem_ty(ctx(0)(list_length(elems)))), _p0_, _p1_, _p2_, _p3_), elem_ty) : default);

    public static List<IRExpr> lower_list_elems_loop(List<AExpr> elems, CodexType elem_ty, LowerCtx ctx, long i, long len) => ((i == len) ? new List<IRExpr>() : (new List<IRExpr> { (_p0_) => lower_expr(list_at(elems(i)), elem_ty(ctx), _p0_) } + (_p0_) => (_p1_) => (_p2_) => (_p3_) => lower_list_elems_loop(elems(elem_ty(ctx((i + 1))(len))), _p0_, _p1_, _p2_, _p3_)));

    public static IRExpr lower_record(Name name, List<AFieldExpr> fields, CodexType ty, LowerCtx ctx) => new IrRecord(name.value((_p0_) => (_p1_) => (_p2_) => (_p3_) => lower_record_fields_loop(fields(ty(ctx(0)(list_length(fields)))), _p0_, _p1_, _p2_, _p3_))(ty));

    public static List<IRFieldVal> lower_record_fields_loop(List<AFieldExpr> fields, CodexType ty, LowerCtx ctx, long i, long len) => ((i == len) ? new List<IRFieldVal>() : ((object f = list_at(fields(i))) is var _ ? (new List<IRFieldVal> { new IRFieldVal(name: f.name.value, value: (_p0_) => (_p1_) => lower_expr(f.value(ty(ctx)), _p0_, _p1_)) } + (_p0_) => (_p1_) => (_p2_) => (_p3_) => lower_record_fields_loop(fields(ty(ctx((i + 1))(len))), _p0_, _p1_, _p2_, _p3_)) : default));

    public static IRExpr lower_do(List<ADoStmt> stmts, CodexType ty, LowerCtx ctx) => new IrDo((_p0_) => (_p1_) => (_p2_) => (_p3_) => lower_do_stmts_loop(stmts(ty(ctx(0)(list_length(stmts)))), _p0_, _p1_, _p2_, _p3_), ty);

    public static List<IRDoStmt> lower_do_stmts_loop(List<ADoStmt> stmts, CodexType ty, LowerCtx ctx, long i, long len) => ((i == len) ? new List<IRDoStmt>() : ((object s = list_at(stmts(i))) is var _ ? s switch { ADoBindStmt(var name, var val) => ((Func<CodexType, Func<LowerCtx, IRExpr>> val_ir = (_p0_) => (_p1_) => lower_expr(val(ty(ctx)), _p0_, _p1_)) is var _ ? ((CodexType val_ty = ir_expr_type(val_ir)) is var _ ? ((object ctx2 = new LowerCtx(types: (new List<object> { new TypeBinding(name: name.value, bound_type: val_ty) } + ctx.types), ust: ctx.ust)) is var _ ? (new List<IRDoStmt> { new IrDoBind(name.value(val_ty(val_ir))) } + (_p0_) => (_p1_) => (_p2_) => (_p3_) => lower_do_stmts_loop(stmts(ty(ctx2((i + 1))(len))), _p0_, _p1_, _p2_, _p3_)) : default) : default) : default), ADoExprStmt(var e) => (new List<IRDoStmt> { new IrDoExec((_p0_) => (_p1_) => lower_expr(e(ty(ctx)), _p0_, _p1_)) } + (_p0_) => (_p1_) => (_p2_) => (_p3_) => lower_do_stmts_loop(stmts(ty(ctx((i + 1))(len))), _p0_, _p1_, _p2_, _p3_)),  } : default));

    public static IRDef lower_def(ADef d, List<TypeBinding> types, UnificationState ust) => ((Func<string, CodexType> raw_type = (_p0_) => lookup_type(types(d.name.value), _p0_)) is var _ ? ((Func<CodexType, CodexType> full_type = (_p0_) => deep_resolve(ust(raw_type), _p0_)) is var _ ? ((CodexType stripped = strip_forall_ty(full_type)) is var _ ? ((Func<CodexType, Func<long, List<IRParam>>> @params = (_p0_) => (_p1_) => lower_def_params(d.@params(stripped(0)), _p0_, _p1_)) is var _ ? ((Func<long, CodexType> ret_type = (_p0_) => get_return_type_n(stripped(list_length(d.@params)), _p0_)) is var _ ? ((Func<UnificationState, Func<List<AParam>, Func<CodexType, LowerCtx>>> ctx = (_p0_) => (_p1_) => (_p2_) => build_def_ctx(types(ust(d.@params(stripped))), _p0_, _p1_, _p2_)) is var _ ? new IRDef(name: d.name.value, @params: @params, type_val: full_type, body: (_p0_) => (_p1_) => lower_expr(d.body(ret_type(ctx)), _p0_, _p1_)) : default) : default) : default) : default) : default) : default);

    public static LowerCtx build_def_ctx(List<TypeBinding> types, UnificationState ust, List<AParam> @params, CodexType ty) => ((object base_ctx = new LowerCtx(types: types, ust: ust)) is var _ ? (_p0_) => (_p1_) => (_p2_) => bind_params_to_ctx(base_ctx(@params(ty(0))), _p0_, _p1_, _p2_) : default);

    public static LowerCtx bind_params_to_ctx(LowerCtx ctx, List<AParam> @params, CodexType ty, long i) => ((i == list_length(@params)) ? ctx : ((object p = list_at(@params(i))) is var _ ? ((CodexType param_ty = peel_fun_param(ty)) is var _ ? ((CodexType rest_ty = peel_fun_return(ty)) is var _ ? ((object ctx2 = new LowerCtx(types: (new List<object> { new TypeBinding(name: p.name.value, bound_type: param_ty) } + ctx.types), ust: ctx.ust)) is var _ ? (_p0_) => (_p1_) => (_p2_) => bind_params_to_ctx(ctx2(@params(rest_ty((i + 1)))), _p0_, _p1_, _p2_) : default) : default) : default) : default));

    public static List<IRParam> lower_def_params(List<AParam> @params, CodexType ty, long i) => ((i == list_length(@params)) ? new List<IRParam>() : ((object p = list_at(@params(i))) is var _ ? ((CodexType param_ty = peel_fun_param(ty)) is var _ ? ((CodexType rest_ty = peel_fun_return(ty)) is var _ ? (new List<IRParam> { new IRParam(name: p.name.value, type_val: param_ty) } + (_p0_) => (_p1_) => lower_def_params(@params(rest_ty((i + 1))), _p0_, _p1_)) : default) : default) : default));

    public static CodexType get_return_type_n(CodexType ty, long n) => ((n == 0) ? ty : ty switch { FunTy(var p, var r) => (_p0_) => get_return_type_n(r((n - 1)), _p0_), _ => ErrorTy,  });

    public static IRModule lower_module(AModule m, List<TypeBinding> types, UnificationState ust) => ((Func<long, Func<long, Func<List<TypeBinding>, List<TypeBinding>>>> ctor_types = (_p0_) => (_p1_) => (_p2_) => collect_ctor_bindings(m.type_defs(0)(list_length(m.type_defs))(new List<object>()), _p0_, _p1_, _p2_)) is var _ ? ((object all_types = (ctor_types + types)) is var _ ? new IRModule(name: m.name, defs: (_p0_) => (_p1_) => (_p2_) => lower_defs(m.defs(all_types(ust(0))), _p0_, _p1_, _p2_)) : default) : default);

    public static List<IRDef> lower_defs(List<ADef> defs, List<TypeBinding> types, UnificationState ust, long i) => ((i == list_length(defs)) ? new List<IRDef>() : (new List<IRDef> { (_p0_) => lower_def(list_at(defs(i)), types(ust), _p0_) } + (_p0_) => (_p1_) => (_p2_) => lower_defs(defs(types(ust((i + 1)))), _p0_, _p1_, _p2_)));

    public static List<TypeBinding> collect_ctor_bindings(List<ATypeDef> tdefs, long i, long len, List<TypeBinding> acc) => ((i == len) ? acc : ((object td = list_at(tdefs(i))) is var _ ? ((List<TypeBinding> bindings = ctor_bindings_for_typedef(td)) is var _ ? (_p0_) => (_p1_) => (_p2_) => collect_ctor_bindings(tdefs((i + 1))(len((acc + bindings))), _p0_, _p1_, _p2_) : default) : default));

    public static List<TypeBinding> ctor_bindings_for_typedef(ATypeDef td) => td switch { AVariantTypeDef(var name, var type_params, var ctors) => ((Func<List<CodexType>, CodexType> result_ty = new ConstructedTy(name(new List<object>()))) is var _ ? (_p0_) => (_p1_) => (_p2_) => (_p3_) => collect_variant_ctor_bindings(ctors(result_ty(0)(list_length(ctors))(new List<object>())), _p0_, _p1_, _p2_, _p3_) : default), ARecordTypeDef(var name, var type_params, var fields) => ((Func<long, Func<long, Func<List<RecordField>, List<RecordField>>>> resolved_fields = (_p0_) => (_p1_) => (_p2_) => build_record_fields_for_lower(fields(0)(list_length(fields))(new List<object>()), _p0_, _p1_, _p2_)) is var _ ? ((Func<List<RecordField>, CodexType> result_ty = new RecordTy(name(resolved_fields))) is var _ ? ((Func<CodexType, Func<long, Func<long, CodexType>>> ctor_ty = (_p0_) => (_p1_) => (_p2_) => build_record_ctor_type_for_lower(fields(result_ty(0)(list_length(fields))), _p0_, _p1_, _p2_)) is var _ ? new List<TypeBinding> { new TypeBinding(name: name.value, bound_type: ctor_ty) } : default) : default) : default),  };

    public static List<TypeBinding> collect_variant_ctor_bindings(List<AVariantCtorDef> ctors, CodexType result_ty, long i, long len, List<TypeBinding> acc) => ((i == len) ? acc : ((object ctor = list_at(ctors(i))) is var _ ? ((Func<CodexType, Func<long, Func<long, CodexType>>> ctor_ty = (_p0_) => (_p1_) => (_p2_) => build_ctor_type_for_lower(ctor.fields(result_ty(0)(list_length(ctor.fields))), _p0_, _p1_, _p2_)) is var _ ? (_p0_) => (_p1_) => (_p2_) => (_p3_) => collect_variant_ctor_bindings(ctors(result_ty((i + 1))(len((acc + new List<object> { new TypeBinding(name: ctor.name.value, bound_type: ctor_ty) })))), _p0_, _p1_, _p2_, _p3_) : default) : default));

    public static CodexType build_ctor_type_for_lower(List<ATypeExpr> fields, CodexType result, long i, long len) => ((i == len) ? result : ((Func<CodexType, Func<long, Func<long, CodexType>>> rest = (_p0_) => (_p1_) => (_p2_) => build_ctor_type_for_lower(fields(result((i + 1))(len)), _p0_, _p1_, _p2_)) is var _ ? new FunTy(resolve_type_expr(list_at(fields(i))), rest) : default));

    public static List<RecordField> build_record_fields_for_lower(List<ARecordFieldDef> fields, long i, long len, List<RecordField> acc) => ((i == len) ? acc : ((object f = list_at(fields(i))) is var _ ? ((object rfield = new RecordField(name: f.name, type_val: resolve_type_expr(f.type_expr))) is var _ ? (_p0_) => (_p1_) => (_p2_) => build_record_fields_for_lower(fields((i + 1))(len((acc + new List<object> { rfield }))), _p0_, _p1_, _p2_) : default) : default));

    public static CodexType build_record_ctor_type_for_lower(List<ARecordFieldDef> fields, CodexType result, long i, long len) => ((i == len) ? result : ((object f = list_at(fields(i))) is var _ ? ((Func<CodexType, Func<long, Func<long, CodexType>>> rest = (_p0_) => (_p1_) => (_p2_) => build_record_ctor_type_for_lower(fields(result((i + 1))(len)), _p0_, _p1_, _p2_)) is var _ ? new FunTy(resolve_type_expr(f.type_expr), rest) : default) : default));

    public static Scope empty_scope() => new Scope(names: new List<object>());

    public static bool scope_has(Scope sc, string name) => (_p0_) => (_p1_) => (_p2_) => scope_has_loop(sc.names(name(0)(list_length(sc.names))), _p0_, _p1_, _p2_);

    public static bool scope_has_loop(List<string> names, string name, long i, long len) => ((i == len) ? false : ((list_at(names(i)) == name) ? true : (_p0_) => (_p1_) => (_p2_) => scope_has_loop(names(name((i + 1))(len)), _p0_, _p1_, _p2_)));

    public static Scope scope_add(Scope sc, string name) => new Scope(names: (new List<object> { name } + sc.names));

    public static List<string> builtin_names() => new List<string> { "show", "negate", "True", "False", "Nothing", "print-line", "read-line", "open-file", "read-all", "close-file", "char-at", "text-length", "substring", "is-letter", "is-digit", "is-whitespace", "text-to-integer", "integer-to-text", "text-replace", "char-code", "code-to-char", "list-length", "list-at", "map", "filter", "fold" };

    public static bool is_type_name(string name) => ((text_length(name) == 0) ? false : (is_letter(char_at(name(0))) && is_upper_char(char_at(name(0)))));

    public static bool is_upper_char(string c) => ((object code = char_code(c)) is var _ ? ((code >= 65) && (code <= 90)) : default);

    public static CollectResult collect_top_level_names(List<ADef> defs, long i, long len, List<string> acc, List<Diagnostic> errs) => ((i == len) ? new CollectResult(names: acc, errors: errs) : ((object def = list_at(defs(i))) is var _ ? ((object name = def.name.value) is var _ ? ((_p0_) => list_contains(acc(name), _p0_) ? (_p0_) => (_p1_) => (_p2_) => (_p3_) => collect_top_level_names(defs((i + 1))(len(acc((errs + new List<object> { make_error("CDX3001", ("Duplicate definition: " + name)) })))), _p0_, _p1_, _p2_, _p3_) : (_p0_) => (_p1_) => (_p2_) => (_p3_) => collect_top_level_names(defs((i + 1))(len((acc + new List<object> { name }))(errs)), _p0_, _p1_, _p2_, _p3_)) : default) : default));

    public static bool list_contains(List<string> xs, string name) => (_p0_) => (_p1_) => (_p2_) => list_contains_loop(xs(name(0)(list_length(xs))), _p0_, _p1_, _p2_);

    public static bool list_contains_loop(List<string> xs, string name, long i, long len) => ((i == len) ? false : ((list_at(xs(i)) == name) ? true : (_p0_) => (_p1_) => (_p2_) => list_contains_loop(xs(name((i + 1))(len)), _p0_, _p1_, _p2_)));

    public static CtorCollectResult collect_ctor_names(List<ATypeDef> type_defs, long i, long len, List<string> type_acc, List<string> ctor_acc) => ((i == len) ? new CtorCollectResult(type_names: type_acc, ctor_names: ctor_acc) : ((object td = list_at(type_defs(i))) is var _ ? td switch { AVariantTypeDef(var name, var @params, var ctors) => ((object new_type_acc = (type_acc + new List<object> { name.value })) is var _ ? ((Func<long, Func<long, Func<List<string>, List<string>>>> new_ctor_acc = (_p0_) => (_p1_) => (_p2_) => collect_variant_ctors(ctors(0)(list_length(ctors))(ctor_acc), _p0_, _p1_, _p2_)) is var _ ? (_p0_) => (_p1_) => (_p2_) => (_p3_) => collect_ctor_names(type_defs((i + 1))(len(new_type_acc(new_ctor_acc))), _p0_, _p1_, _p2_, _p3_) : default) : default), ARecordTypeDef(var name, var @params, var fields) => (_p0_) => (_p1_) => (_p2_) => (_p3_) => collect_ctor_names(type_defs((i + 1))(len((type_acc + new List<object> { name.value }))(ctor_acc)), _p0_, _p1_, _p2_, _p3_),  } : default));

    public static List<string> collect_variant_ctors(List<AVariantCtorDef> ctors, long i, long len, List<string> acc) => ((i == len) ? acc : ((object ctor = list_at(ctors(i))) is var _ ? (_p0_) => (_p1_) => (_p2_) => collect_variant_ctors(ctors((i + 1))(len((acc + new List<object> { ctor.name.value }))), _p0_, _p1_, _p2_) : default));

    public static Scope build_all_names_scope(List<string> top_names, List<string> ctor_names, List<string> builtins) => ((Func<List<string>, Func<long, Func<long, Scope>>> sc = (_p0_) => (_p1_) => (_p2_) => add_names_to_scope(empty_scope(top_names(0)(list_length(top_names))), _p0_, _p1_, _p2_)) is var _ ? ((Func<List<string>, Func<long, Func<long, Scope>>> sc2 = (_p0_) => (_p1_) => (_p2_) => add_names_to_scope(sc(ctor_names(0)(list_length(ctor_names))), _p0_, _p1_, _p2_)) is var _ ? (_p0_) => (_p1_) => (_p2_) => add_names_to_scope(sc2(builtins(0)(list_length(builtins))), _p0_, _p1_, _p2_) : default) : default);

    public static Scope add_names_to_scope(Scope sc, List<string> names, long i, long len) => ((i == len) ? sc : (_p0_) => (_p1_) => add_names_to_scope((_p0_) => scope_add(sc(list_at(names(i))), _p0_), names((i + 1))(len), _p0_, _p1_));

    public static List<Diagnostic> resolve_expr(Scope sc, AExpr expr) => expr switch { ALitExpr(var val, var kind) => new List<Diagnostic>(), ANameExpr(var name) => (((_p0_) => scope_has(sc(name.value), _p0_) || is_type_name(name.value)) ? new List<Diagnostic>() : new List<Diagnostic> { make_error("CDX3002", ("Undefined name: " + name.value)) }), ABinaryExpr(var left, var op, var right) => ((_p0_) => resolve_expr(sc(left), _p0_) + (_p0_) => resolve_expr(sc(right), _p0_)), AUnaryExpr(var operand) => (_p0_) => resolve_expr(sc(operand), _p0_), AApplyExpr(var func, var arg) => ((_p0_) => resolve_expr(sc(func), _p0_) + (_p0_) => resolve_expr(sc(arg), _p0_)), AIfExpr(var cond, var then_e, var else_e) => ((_p0_) => resolve_expr(sc(cond), _p0_) + ((_p0_) => resolve_expr(sc(then_e), _p0_) + (_p0_) => resolve_expr(sc(else_e), _p0_))), ALetExpr(var bindings, var body) => (_p0_) => (_p1_) => (_p2_) => (_p3_) => (_p4_) => resolve_let(sc(bindings(body(0)(list_length(bindings))(new List<object>()))), _p0_, _p1_, _p2_, _p3_, _p4_), ALambdaExpr(var @params, var body) => ((Func<List<Name>, Func<long, Func<long, Scope>>> sc2 = (_p0_) => (_p1_) => (_p2_) => add_lambda_params(sc(@params(0)(list_length(@params))), _p0_, _p1_, _p2_)) is var _ ? (_p0_) => resolve_expr(sc2(body), _p0_) : default), AMatchExpr(var scrutinee, var arms) => ((_p0_) => resolve_expr(sc(scrutinee), _p0_) + (_p0_) => (_p1_) => (_p2_) => (_p3_) => resolve_match_arms(sc(arms(0)(list_length(arms))(new List<object>())), _p0_, _p1_, _p2_, _p3_)), AListExpr(var elems) => (_p0_) => (_p1_) => (_p2_) => (_p3_) => resolve_list_elems(sc(elems(0)(list_length(elems))(new List<object>())), _p0_, _p1_, _p2_, _p3_), ARecordExpr(var name, var fields) => (_p0_) => (_p1_) => (_p2_) => (_p3_) => resolve_record_fields(sc(fields(0)(list_length(fields))(new List<object>())), _p0_, _p1_, _p2_, _p3_), AFieldAccess(var obj, var field) => (_p0_) => resolve_expr(sc(obj), _p0_), ADoExpr(var stmts) => (_p0_) => (_p1_) => (_p2_) => (_p3_) => resolve_do_stmts(sc(stmts(0)(list_length(stmts))(new List<object>())), _p0_, _p1_, _p2_, _p3_), AErrorExpr(var msg) => new List<Diagnostic>(),  };

    public static List<Diagnostic> resolve_let(Scope sc, List<ALetBind> bindings, AExpr body, long i, long len, List<Diagnostic> errs) => ((i == len) ? (errs + (_p0_) => resolve_expr(sc(body), _p0_)) : ((object b = list_at(bindings(i))) is var _ ? ((Func<AExpr, List<Diagnostic>> bind_errs = (_p0_) => resolve_expr(sc(b.value), _p0_)) is var _ ? ((Func<string, Scope> sc2 = (_p0_) => scope_add(sc(b.name.value), _p0_)) is var _ ? (_p0_) => (_p1_) => (_p2_) => (_p3_) => (_p4_) => resolve_let(sc2(bindings(body((i + 1))(len((errs + bind_errs))))), _p0_, _p1_, _p2_, _p3_, _p4_) : default) : default) : default));

    public static Scope add_lambda_params(Scope sc, List<Name> @params, long i, long len) => ((i == len) ? sc : ((object p = list_at(@params(i))) is var _ ? (_p0_) => (_p1_) => add_lambda_params((_p0_) => scope_add(sc(p.value), _p0_), @params((i + 1))(len), _p0_, _p1_) : default));

    public static List<Diagnostic> resolve_match_arms(Scope sc, List<AMatchArm> arms, long i, long len, List<Diagnostic> errs) => ((i == len) ? errs : ((object arm = list_at(arms(i))) is var _ ? ((Func<APat, Scope> sc2 = (_p0_) => collect_pattern_names(sc(arm.pattern), _p0_)) is var _ ? ((Func<AExpr, List<Diagnostic>> arm_errs = (_p0_) => resolve_expr(sc2(arm.body), _p0_)) is var _ ? (_p0_) => (_p1_) => (_p2_) => (_p3_) => resolve_match_arms(sc(arms((i + 1))(len((errs + arm_errs)))), _p0_, _p1_, _p2_, _p3_) : default) : default) : default));

    public static Scope collect_pattern_names(Scope sc, APat pat) => pat switch { AVarPat(var name) => (_p0_) => scope_add(sc(name.value), _p0_), ACtorPat(var name, var subs) => (_p0_) => (_p1_) => (_p2_) => collect_ctor_pat_names(sc(subs(0)(list_length(subs))), _p0_, _p1_, _p2_), ALitPat(var val, var kind) => sc, AWildPat { } => sc,  };

    public static Scope collect_ctor_pat_names(Scope sc, List<APat> subs, long i, long len) => ((i == len) ? sc : ((object sub = list_at(subs(i))) is var _ ? (_p0_) => (_p1_) => collect_ctor_pat_names((_p0_) => collect_pattern_names(sc(sub), _p0_), subs((i + 1))(len), _p0_, _p1_) : default));

    public static List<Diagnostic> resolve_list_elems(Scope sc, List<AExpr> elems, long i, long len, List<Diagnostic> errs) => ((i == len) ? errs : ((Func<AExpr, List<Diagnostic>> errs2 = (_p0_) => resolve_expr(sc(list_at(elems(i))), _p0_)) is var _ ? (_p0_) => (_p1_) => (_p2_) => (_p3_) => resolve_list_elems(sc(elems((i + 1))(len((errs + errs2)))), _p0_, _p1_, _p2_, _p3_) : default));

    public static List<Diagnostic> resolve_record_fields(Scope sc, List<AFieldExpr> fields, long i, long len, List<Diagnostic> errs) => ((i == len) ? errs : ((object f = list_at(fields(i))) is var _ ? ((Func<AExpr, List<Diagnostic>> errs2 = (_p0_) => resolve_expr(sc(f.value), _p0_)) is var _ ? (_p0_) => (_p1_) => (_p2_) => (_p3_) => resolve_record_fields(sc(fields((i + 1))(len((errs + errs2)))), _p0_, _p1_, _p2_, _p3_) : default) : default));

    public static List<Diagnostic> resolve_do_stmts(Scope sc, List<ADoStmt> stmts, long i, long len, List<Diagnostic> errs) => ((i == len) ? errs : ((object stmt = list_at(stmts(i))) is var _ ? stmt switch { ADoExprStmt(var e) => ((Func<AExpr, List<Diagnostic>> errs2 = (_p0_) => resolve_expr(sc(e), _p0_)) is var _ ? (_p0_) => (_p1_) => (_p2_) => (_p3_) => resolve_do_stmts(sc(stmts((i + 1))(len((errs + errs2)))), _p0_, _p1_, _p2_, _p3_) : default), ADoBindStmt(var name, var e) => ((Func<AExpr, List<Diagnostic>> errs2 = (_p0_) => resolve_expr(sc(e), _p0_)) is var _ ? ((Func<string, Scope> sc2 = (_p0_) => scope_add(sc(name.value), _p0_)) is var _ ? (_p0_) => (_p1_) => (_p2_) => (_p3_) => resolve_do_stmts(sc2(stmts((i + 1))(len((errs + errs2)))), _p0_, _p1_, _p2_, _p3_) : default) : default),  } : default));

    public static List<Diagnostic> resolve_all_defs(Scope sc, List<ADef> defs, long i, long len, List<Diagnostic> errs) => ((i == len) ? errs : ((object def = list_at(defs(i))) is var _ ? ((Func<List<AParam>, Func<long, Func<long, Scope>>> def_scope = (_p0_) => (_p1_) => (_p2_) => add_def_params(sc(def.@params(0)(list_length(def.@params))), _p0_, _p1_, _p2_)) is var _ ? ((Func<AExpr, List<Diagnostic>> errs2 = (_p0_) => resolve_expr(def_scope(def.body), _p0_)) is var _ ? (_p0_) => (_p1_) => (_p2_) => (_p3_) => resolve_all_defs(sc(defs((i + 1))(len((errs + errs2)))), _p0_, _p1_, _p2_, _p3_) : default) : default) : default));

    public static Scope add_def_params(Scope sc, List<AParam> @params, long i, long len) => ((i == len) ? sc : ((object p = list_at(@params(i))) is var _ ? (_p0_) => (_p1_) => add_def_params((_p0_) => scope_add(sc(p.name.value), _p0_), @params((i + 1))(len), _p0_, _p1_) : default));

    public static ResolveResult resolve_module(AModule mod) => ((Func<long, Func<long, Func<List<string>, Func<List<Diagnostic>, CollectResult>>>> top = (_p0_) => (_p1_) => (_p2_) => (_p3_) => collect_top_level_names(mod.defs(0)(list_length(mod.defs))(new List<object>())(new List<object>()), _p0_, _p1_, _p2_, _p3_)) is var _ ? ((Func<long, Func<long, Func<List<string>, Func<List<string>, CtorCollectResult>>>> ctors = (_p0_) => (_p1_) => (_p2_) => (_p3_) => collect_ctor_names(mod.type_defs(0)(list_length(mod.type_defs))(new List<object>())(new List<object>()), _p0_, _p1_, _p2_, _p3_)) is var _ ? ((Func<List<string>, Func<List<string>, Scope>> sc = (_p0_) => (_p1_) => build_all_names_scope(top.names(ctors.ctor_names(builtin_names)), _p0_, _p1_)) is var _ ? ((Func<List<ADef>, Func<long, Func<long, Func<List<Diagnostic>, List<Diagnostic>>>>> expr_errs = (_p0_) => (_p1_) => (_p2_) => (_p3_) => resolve_all_defs(sc(mod.defs(0)(list_length(mod.defs))(new List<object>())), _p0_, _p1_, _p2_, _p3_)) is var _ ? new ResolveResult(errors: (top.errors + expr_errs), top_level_names: top.names, type_names: ctors.type_names, ctor_names: ctors.ctor_names) : default) : default) : default) : default);

    public static LexState make_lex_state(string src) => new LexState(source: src, offset: 0, line: 1, column: 1);

    public static bool is_at_end(LexState st) => (st.offset >= text_length(st.source));

    public static string peek_char(LexState st) => (is_at_end(st) ? "" : char_at(st.source(st.offset)));

    public static LexState advance_char(LexState st) => ((peek_char(st) == "\\n") ? new LexState(source: st.source, offset: (st.offset + 1), line: (st.line + 1), column: 1) : new LexState(source: st.source, offset: (st.offset + 1), line: st.line, column: (st.column + 1)));

    public static LexState skip_spaces(LexState st) => (is_at_end(st) ? st : ((peek_char(st) == " ") ? skip_spaces(advance_char(st)) : st));

    public static LexState scan_ident_rest(LexState st) => (is_at_end(st) ? st : ((string ch = peek_char(st)) is var _ ? (is_letter(ch) ? scan_ident_rest(advance_char(st)) : (is_digit(ch) ? scan_ident_rest(advance_char(st)) : ((ch == "_") ? scan_ident_rest(advance_char(st)) : ((ch == "-") ? ((LexState next = advance_char(st)) is var _ ? (is_at_end(next) ? st : (is_letter(peek_char(next)) ? scan_ident_rest(next) : st)) : default) : st)))) : default));

    public static LexState scan_digits(LexState st) => (is_at_end(st) ? st : ((string ch = peek_char(st)) is var _ ? (is_digit(ch) ? scan_digits(advance_char(st)) : ((ch == "_") ? scan_digits(advance_char(st)) : st)) : default));

    public static LexState scan_string_body(LexState st) => (is_at_end(st) ? st : ((string ch = peek_char(st)) is var _ ? ((ch == "\\\"") ? advance_char(st) : ((ch == "\\n") ? st : ((ch == "\\\\") ? scan_string_body(advance_char(advance_char(st))) : scan_string_body(advance_char(st))))) : default));

    public static TokenKind classify_word(string w) => ((w == "let") ? LetKeyword : ((w == "in") ? InKeyword : ((w == "if") ? IfKeyword : ((w == "then") ? ThenKeyword : ((w == "else") ? ElseKeyword : ((w == "when") ? WhenKeyword : ((w == "where") ? WhereKeyword : ((w == "do") ? DoKeyword : ((w == "record") ? RecordKeyword : ((w == "import") ? ImportKeyword : ((w == "export") ? ExportKeyword : ((w == "claim") ? ClaimKeyword : ((w == "proof") ? ProofKeyword : ((w == "forall") ? ForAllKeyword : ((w == "exists") ? ThereExistsKeyword : ((w == "linear") ? LinearKeyword : ((w == "True") ? TrueKeyword : ((w == "False") ? FalseKeyword : ((object first_code = char_code(char_at(w(0)))) is var _ ? ((first_code >= 65) ? ((first_code <= 90) ? TypeIdentifier : Identifier) : Identifier) : default)))))))))))))))))));

    public static Token make_token(TokenKind kind, string text, LexState st) => new Token(kind: kind, text: text, offset: st.offset, line: st.line, column: st.column);

    public static string extract_text(LexState st, long start, LexState end_st) => substring(st.source(start((end_st.offset - start))));

    public static LexResult scan_token(LexState st) => ((LexState s = skip_spaces(st)) is var _ ? (is_at_end(s) ? LexEnd : ((string ch = peek_char(s)) is var _ ? ((ch == "\\n") ? new LexToken(make_token(Newline, "\\n", s), advance_char(s)) : ((ch == "\\\"") ? ((object start = (s.offset + 1)) is var _ ? ((LexState after = scan_string_body(advance_char(s))) is var _ ? ((object text_len = ((after.offset - start) - 1)) is var _ ? new LexToken(make_token(TextLiteral, substring(s.source(start(text_len))), s), after) : default) : default) : default) : (is_letter(ch) ? ((object start = s.offset) is var _ ? ((LexState after = scan_ident_rest(advance_char(s))) is var _ ? ((Func<long, Func<LexState, string>> word = (_p0_) => (_p1_) => extract_text(s(start(after)), _p0_, _p1_)) is var _ ? new LexToken((_p0_) => make_token(classify_word(word), word(s), _p0_), after) : default) : default) : default) : ((ch == "_") ? ((object start = s.offset) is var _ ? ((LexState after = scan_ident_rest(advance_char(s))) is var _ ? ((Func<long, Func<LexState, string>> word = (_p0_) => (_p1_) => extract_text(s(start(after)), _p0_, _p1_)) is var _ ? ((text_length(word) == 1) ? new LexToken(make_token(Underscore, "_", s), after) : new LexToken((_p0_) => make_token(classify_word(word), word(s), _p0_), after)) : default) : default) : default) : (is_digit(ch) ? ((object start = s.offset) is var _ ? ((LexState after = scan_digits(advance_char(s))) is var _ ? (is_at_end(after) ? new LexToken(make_token(IntegerLiteral, (_p0_) => (_p1_) => extract_text(s(start(after)), _p0_, _p1_), s), after) : ((peek_char(after) == ".") ? ((LexState after2 = scan_digits(advance_char(after))) is var _ ? new LexToken(make_token(NumberLiteral, (_p0_) => (_p1_) => extract_text(s(start(after2)), _p0_, _p1_), s), after2) : default) : new LexToken(make_token(IntegerLiteral, (_p0_) => (_p1_) => extract_text(s(start(after)), _p0_, _p1_), s), after))) : default) : default) : scan_operator(s)))))) : default)) : default);

    public static LexResult scan_operator(LexState s) => ((string ch = peek_char(s)) is var _ ? ((LexState next = advance_char(s)) is var _ ? ((ch == "(") ? new LexToken(make_token(LeftParen, "(", s), next) : ((ch == ")") ? new LexToken(make_token(RightParen, ")", s), next) : ((ch == "[") ? new LexToken(make_token(LeftBracket, "[", s), next) : ((ch == "]") ? new LexToken(make_token(RightBracket, "]", s), next) : ((ch == "{") ? new LexToken(make_token(LeftBrace, "{", s), next) : ((ch == "}") ? new LexToken(make_token(RightBrace, "}", s), next) : ((ch == ",") ? new LexToken(make_token(Comma, ",", s), next) : ((ch == ".") ? new LexToken(make_token(Dot, ".", s), next) : ((ch == "^") ? new LexToken(make_token(Caret, "^", s), next) : ((ch == "&") ? new LexToken(make_token(Ampersand, "&", s), next) : scan_multi_char_operator(s))))))))))) : default) : default);

    public static LexResult scan_multi_char_operator(LexState s) => ((string ch = peek_char(s)) is var _ ? ((LexState next = advance_char(s)) is var _ ? ((object next_ch = (is_at_end(next) ? "" : peek_char(next))) is var _ ? ((ch == "+") ? ((next_ch == "+") ? new LexToken(make_token(PlusPlus, "++", s), advance_char(next)) : new LexToken(make_token(Plus, "+", s), next)) : ((ch == "-") ? ((next_ch == ">") ? new LexToken(make_token(Arrow, "->", s), advance_char(next)) : new LexToken(make_token(Minus, "-", s), next)) : ((ch == "*") ? new LexToken(make_token(Star, "*", s), next) : ((ch == "/") ? ((next_ch == "=") ? new LexToken(make_token(NotEquals, "/=", s), advance_char(next)) : new LexToken(make_token(Slash, "/", s), next)) : ((ch == "=") ? ((next_ch == "=") ? ((LexState next2 = advance_char(next)) is var _ ? ((object next2_ch = (is_at_end(next2) ? "" : peek_char(next2))) is var _ ? ((next2_ch == "=") ? new LexToken(make_token(TripleEquals, "===", s), advance_char(next2)) : new LexToken(make_token(DoubleEquals, "==", s), next2)) : default) : default) : new LexToken(make_token(Equals, "=", s), next)) : ((ch == ":") ? ((next_ch == ":") ? new LexToken(make_token(ColonColon, "::", s), advance_char(next)) : new LexToken(make_token(Colon, ":", s), next)) : ((ch == "|") ? ((next_ch == "-") ? new LexToken(make_token(Turnstile, "|-", s), advance_char(next)) : new LexToken(make_token(Pipe, "|", s), next)) : ((ch == "<") ? ((next_ch == "=") ? new LexToken(make_token(LessOrEqual, "<=", s), advance_char(next)) : ((next_ch == "-") ? new LexToken(make_token(LeftArrow, "<-", s), advance_char(next)) : new LexToken(make_token(LessThan, "<", s), next))) : ((ch == ">") ? ((next_ch == "=") ? new LexToken(make_token(GreaterOrEqual, ">=", s), advance_char(next)) : new LexToken(make_token(GreaterThan, ">", s), next)) : new LexToken(make_token(ErrorToken, char_at(s.source(s.offset)), s), next)))))))))) : default) : default) : default);

    public static List<Token> tokenize_loop(LexState st, List<Token> acc) => scan_token(st) switch { LexToken(var tok, var next) => ((tok.kind == EndOfFile) ? (acc + new List<Token> { tok }) : (_p0_) => tokenize_loop(next((acc + new List<object> { tok })), _p0_)), LexEnd { } => (acc + new List<Token> { make_token(EndOfFile, "", st) }),  };

    public static List<Token> tokenize(string src) => tokenize_loop(make_lex_state(src), new List<Token>());

    public static ParseState make_parse_state(List<Token> toks) => new ParseState(tokens: toks, pos: 0);

    public static Token current(ParseState st) => list_at(st.tokens(st.pos));

    public static TokenKind current_kind(ParseState st) => current(st).kind;

    public static ParseState advance(ParseState st) => new ParseState(tokens: st.tokens, pos: (st.pos + 1));

    public static bool is_done(ParseState st) => current_kind(st) switch { EndOfFile { } => true, _ => false,  };

    public static TokenKind peek_kind(ParseState st, long offset) => list_at(st.tokens((st.pos + offset))).kind;

    public static bool is_ident(TokenKind k) => k switch { Identifier { } => true, _ => false,  };

    public static bool is_type_ident(TokenKind k) => k switch { TypeIdentifier { } => true, _ => false,  };

    public static bool is_arrow(TokenKind k) => k switch { Arrow { } => true, _ => false,  };

    public static bool is_equals(TokenKind k) => k switch { Equals { } => true, _ => false,  };

    public static bool is_colon(TokenKind k) => k switch { Colon { } => true, _ => false,  };

    public static bool is_comma(TokenKind k) => k switch { Comma { } => true, _ => false,  };

    public static bool is_pipe(TokenKind k) => k switch { Pipe { } => true, _ => false,  };

    public static bool is_dot(TokenKind k) => k switch { Dot { } => true, _ => false,  };

    public static bool is_left_paren(TokenKind k) => k switch { LeftParen { } => true, _ => false,  };

    public static bool is_left_brace(TokenKind k) => k switch { LeftBrace { } => true, _ => false,  };

    public static bool is_left_bracket(TokenKind k) => k switch { LeftBracket { } => true, _ => false,  };

    public static bool is_right_brace(TokenKind k) => k switch { RightBrace { } => true, _ => false,  };

    public static bool is_right_bracket(TokenKind k) => k switch { RightBracket { } => true, _ => false,  };

    public static bool is_if_keyword(TokenKind k) => k switch { IfKeyword { } => true, _ => false,  };

    public static bool is_let_keyword(TokenKind k) => k switch { LetKeyword { } => true, _ => false,  };

    public static bool is_when_keyword(TokenKind k) => k switch { WhenKeyword { } => true, _ => false,  };

    public static bool is_do_keyword(TokenKind k) => k switch { DoKeyword { } => true, _ => false,  };

    public static bool is_in_keyword(TokenKind k) => k switch { InKeyword { } => true, _ => false,  };

    public static bool is_minus(TokenKind k) => k switch { Minus { } => true, _ => false,  };

    public static bool is_dedent(TokenKind k) => k switch { Dedent { } => true, _ => false,  };

    public static bool is_left_arrow(TokenKind k) => k switch { LeftArrow { } => true, _ => false,  };

    public static bool is_record_keyword(TokenKind k) => k switch { RecordKeyword { } => true, _ => false,  };

    public static bool is_underscore(TokenKind k) => k switch { Underscore { } => true, _ => false,  };

    public static bool is_literal(TokenKind k) => k switch { IntegerLiteral { } => true, NumberLiteral { } => true, TextLiteral { } => true, TrueKeyword { } => true, FalseKeyword { } => true, _ => false,  };

    public static bool is_app_start(TokenKind k) => k switch { Identifier { } => true, TypeIdentifier { } => true, IntegerLiteral { } => true, NumberLiteral { } => true, TextLiteral { } => true, TrueKeyword { } => true, FalseKeyword { } => true, LeftParen { } => true, LeftBracket { } => true, _ => false,  };

    public static bool is_compound(Expr e) => e switch { MatchExpr(var s, var arms) => true, IfExpr(var c, var t, var el) => true, LetExpr(var binds, var body) => true, DoExpr(var stmts) => true, _ => false,  };

    public static bool is_type_arg_start(TokenKind k) => k switch { TypeIdentifier { } => true, Identifier { } => true, LeftParen { } => true, _ => false,  };

    public static long operator_precedence(TokenKind k) => k switch { PlusPlus { } => 5, ColonColon { } => 5, Plus { } => 6, Minus { } => 6, Star { } => 7, Slash { } => 7, Caret { } => 8, DoubleEquals { } => 4, NotEquals { } => 4, LessThan { } => 4, GreaterThan { } => 4, LessOrEqual { } => 4, GreaterOrEqual { } => 4, TripleEquals { } => 4, Ampersand { } => 3, Pipe { } => 2, _ => (0 - 1),  };

    public static bool is_right_assoc(TokenKind k) => k switch { PlusPlus { } => true, ColonColon { } => true, Caret { } => true, Arrow { } => true, _ => false,  };

    public static ParseState expect(TokenKind kind, ParseState st) => (is_done(st) ? st : advance(st));

    public static ParseState skip_newlines(ParseState st) => (is_done(st) ? st : current_kind(st) switch { Newline { } => skip_newlines(advance(st)), Indent { } => skip_newlines(advance(st)), Dedent { } => skip_newlines(advance(st)), _ => st,  });

    public static ParseTypeResult parse_type(ParseState st) => ((ParseTypeResult result = parse_type_atom(st)) is var _ ? (_p0_) => unwrap_type_ok(result(parse_type_continue), _p0_) : default);

    public static ParseTypeResult parse_type_continue(TypeExpr left, ParseState st) => (is_arrow(current_kind(st)) ? ((ParseState st2 = advance(st)) is var _ ? ((ParseTypeResult right_result = parse_type(st2)) is var _ ? (_p0_) => unwrap_type_ok(right_result((_p0_) => (_p1_) => make_fun_type(left, _p0_, _p1_)), _p0_) : default) : default) : new TypeOk(left(st)));

    public static ParseTypeResult make_fun_type(TypeExpr left, TypeExpr right, ParseState st) => new TypeOk(new FunType(left(right)), st);

    public static ParseTypeResult unwrap_type_ok(ParseTypeResult r, Func<TypeExpr, Func<ParseState, ParseTypeResult>> f) => r switch { TypeOk(var t, var st) => f(t(st)),  };

    public static ParseTypeResult parse_type_atom(ParseState st) => (is_ident(current_kind(st)) ? ((Token tok = current(st)) is var _ ? parse_type_args(new NamedType(tok), advance(st)) : default) : (is_type_ident(current_kind(st)) ? ((Token tok = current(st)) is var _ ? parse_type_args(new NamedType(tok), advance(st)) : default) : (is_left_paren(current_kind(st)) ? parse_paren_type(advance(st)) : ((Token tok = current(st)) is var _ ? new TypeOk(new NamedType(tok), advance(st)) : default))));

    public static ParseTypeResult parse_paren_type(ParseState st) => ((ParseTypeResult inner = parse_type(st)) is var _ ? (_p0_) => unwrap_type_ok(inner(finish_paren_type), _p0_) : default);

    public static ParseTypeResult finish_paren_type(TypeExpr t, ParseState st) => ((ParseState st2 = expect(RightParen, st)) is var _ ? new TypeOk(new ParenType(t), st2) : default);

    public static ParseTypeResult parse_type_args(TypeExpr base_type, ParseState st) => (is_done(st) ? new TypeOk(base_type(st)) : (is_type_arg_start(current_kind(st)) ? (_p0_) => parse_type_arg_next(base_type(st), _p0_) : new TypeOk(base_type(st))));

    public static ParseTypeResult parse_type_arg_next(TypeExpr base_type, ParseState st) => ((ParseTypeResult arg_result = parse_type_atom(st)) is var _ ? (_p0_) => unwrap_type_ok(arg_result((_p0_) => (_p1_) => continue_type_args(base_type, _p0_, _p1_)), _p0_) : default);

    public static ParseTypeResult continue_type_args(TypeExpr base_type, TypeExpr arg, ParseState st) => parse_type_args(new AppType(base_type(new List<object> { arg })), st);

    public static ParsePatResult parse_pattern(ParseState st) => (is_underscore(current_kind(st)) ? new PatOk(new WildPat(current(st)), advance(st)) : (is_literal(current_kind(st)) ? new PatOk(new LitPat(current(st)), advance(st)) : (is_type_ident(current_kind(st)) ? ((Token tok = current(st)) is var _ ? (_p0_) => (_p1_) => parse_ctor_pattern_fields(tok(new List<object>())(advance(st)), _p0_, _p1_) : default) : (is_ident(current_kind(st)) ? new PatOk(new VarPat(current(st)), advance(st)) : new PatOk(new WildPat(current(st)), advance(st))))));

    public static ParsePatResult parse_ctor_pattern_fields(Token ctor, List<Pat> acc, ParseState st) => (is_left_paren(current_kind(st)) ? ((ParseState st2 = advance(st)) is var _ ? ((ParsePatResult sub = parse_pattern(st2)) is var _ ? (_p0_) => unwrap_pat_ok(sub((_p0_) => (_p1_) => (_p2_) => continue_ctor_fields(ctor(acc), _p0_, _p1_, _p2_)), _p0_) : default) : default) : new PatOk(new CtorPat(ctor(acc)), st));

    public static ParsePatResult continue_ctor_fields(Token ctor, List<Pat> acc, Pat p, ParseState st) => ((ParseState st2 = expect(RightParen, st)) is var _ ? (_p0_) => (_p1_) => parse_ctor_pattern_fields(ctor((acc + new List<object> { p }))(st2), _p0_, _p1_) : default);

    public static ParsePatResult unwrap_pat_ok(ParsePatResult r, Func<Pat, Func<ParseState, ParsePatResult>> f) => r switch { PatOk(var p, var st) => f(p(st)),  };

    public static ParseExprResult parse_expr(ParseState st) => (_p0_) => parse_binary(st(0), _p0_);

    public static ParseExprResult unwrap_expr_ok(ParseExprResult r, Func<Expr, Func<ParseState, ParseExprResult>> f) => r switch { ExprOk(var e, var st) => f(e(st)),  };

    public static ParseExprResult parse_binary(ParseState st, long min_prec) => ((ParseExprResult left_result = parse_unary(st)) is var _ ? (_p0_) => unwrap_expr_ok(left_result((_p0_) => (_p1_) => start_binary_loop(min_prec, _p0_, _p1_)), _p0_) : default);

    public static ParseExprResult start_binary_loop(long min_prec, Expr left, ParseState st) => (_p0_) => (_p1_) => parse_binary_loop(left(st(min_prec)), _p0_, _p1_);

    public static ParseExprResult parse_binary_loop(Expr left, ParseState st, long min_prec) => (is_done(st) ? new ExprOk(left(st)) : ((long prec = operator_precedence(current_kind(st))) is var _ ? ((prec < min_prec) ? new ExprOk(left(st)) : ((Token op = current(st)) is var _ ? ((ParseState st2 = skip_newlines(advance(st))) is var _ ? ((object next_min = (is_right_assoc(op.kind) ? prec : (prec + 1))) is var _ ? ((Func<long, ParseExprResult> right_result = (_p0_) => parse_binary(st2(next_min), _p0_)) is var _ ? (_p0_) => unwrap_expr_ok(right_result((_p0_) => (_p1_) => (_p2_) => (_p3_) => continue_binary(left(op(min_prec)), _p0_, _p1_, _p2_, _p3_)), _p0_) : default) : default) : default) : default)) : default));

    public static ParseExprResult continue_binary(Expr left, Token op, long min_prec, Expr right, ParseState st) => (_p0_) => parse_binary_loop(new BinExpr(left(op(right))), st(min_prec), _p0_);

    public static ParseExprResult parse_unary(ParseState st) => (is_minus(current_kind(st)) ? ((Token op = current(st)) is var _ ? ((ParseExprResult result = parse_unary(advance(st))) is var _ ? (_p0_) => unwrap_expr_ok(result((_p0_) => (_p1_) => finish_unary(op, _p0_, _p1_)), _p0_) : default) : default) : parse_application(st));

    public static ParseExprResult finish_unary(Token op, Expr operand, ParseState st) => new ExprOk(new UnaryExpr(op(operand)), st);

    public static ParseExprResult parse_application(ParseState st) => ((ParseExprResult func_result = parse_atom(st)) is var _ ? (_p0_) => unwrap_expr_ok(func_result(parse_app_loop), _p0_) : default);

    public static ParseExprResult parse_app_loop(Expr func, ParseState st) => (is_compound(func) ? (_p0_) => parse_dot_only(func(st), _p0_) : (is_done(st) ? new ExprOk(func(st)) : (is_app_start(current_kind(st)) ? ((ParseExprResult arg_result = parse_atom(st)) is var _ ? (_p0_) => unwrap_expr_ok(arg_result((_p0_) => (_p1_) => continue_app(func, _p0_, _p1_)), _p0_) : default) : (_p0_) => parse_field_access(func(st), _p0_))));

    public static ParseExprResult continue_app(Expr func, Expr arg, ParseState st) => parse_app_loop(new AppExpr(func(arg)), st);

    public static ParseExprResult parse_atom(ParseState st) => (is_literal(current_kind(st)) ? new ExprOk(new LitExpr(current(st)), advance(st)) : (is_ident(current_kind(st)) ? parse_field_access(new NameExpr(current(st)), advance(st)) : (is_type_ident(current_kind(st)) ? parse_atom_type_ident(st) : (is_left_paren(current_kind(st)) ? parse_paren_expr(advance(st)) : (is_left_bracket(current_kind(st)) ? parse_list_expr(st) : (is_if_keyword(current_kind(st)) ? parse_if_expr(st) : (is_let_keyword(current_kind(st)) ? parse_let_expr(st) : (is_when_keyword(current_kind(st)) ? parse_match_expr(st) : (is_do_keyword(current_kind(st)) ? parse_do_expr(st) : new ExprOk(new ErrExpr(current(st)), advance(st)))))))))));

    public static ParseExprResult parse_field_access(Expr node, ParseState st) => (is_dot(current_kind(st)) ? ((ParseState st2 = advance(st)) is var _ ? ((Token field = current(st2)) is var _ ? ((ParseState st3 = advance(st2)) is var _ ? parse_field_access(new FieldExpr(node(field)), st3) : default) : default) : default) : (is_app_start(current_kind(st)) ? ((ParseExprResult arg_result = parse_atom(st)) is var _ ? (_p0_) => unwrap_expr_ok(arg_result((_p0_) => (_p1_) => continue_app(node, _p0_, _p1_)), _p0_) : default) : new ExprOk(node(st))));

    public static ParseExprResult parse_dot_only(Expr node, ParseState st) => (is_dot(current_kind(st)) ? ((ParseState st2 = advance(st)) is var _ ? ((Token field = current(st2)) is var _ ? ((ParseState st3 = advance(st2)) is var _ ? parse_dot_only(new FieldExpr(node(field)), st3) : default) : default) : default) : new ExprOk(node(st)));

    public static ParseExprResult parse_atom_type_ident(ParseState st) => ((Token tok = current(st)) is var _ ? ((ParseState st2 = advance(st)) is var _ ? (is_left_brace(current_kind(st2)) ? (_p0_) => parse_record_expr(tok(st2), _p0_) : new ExprOk(new NameExpr(tok), st2)) : default) : default);

    public static ParseExprResult parse_paren_expr(ParseState st) => ((ParseState st2 = skip_newlines(st)) is var _ ? ((ParseExprResult inner = parse_expr(st2)) is var _ ? (_p0_) => unwrap_expr_ok(inner(finish_paren_expr), _p0_) : default) : default);

    public static ParseExprResult finish_paren_expr(Expr e, ParseState st) => ((ParseState st2 = skip_newlines(st)) is var _ ? ((ParseState st3 = expect(RightParen, st2)) is var _ ? new ExprOk(new ParenExpr(e), st3) : default) : default);

    public static ParseExprResult parse_record_expr(Token type_name, ParseState st) => ((ParseState st2 = advance(st)) is var _ ? ((ParseState st3 = skip_newlines(st2)) is var _ ? (_p0_) => (_p1_) => parse_record_expr_fields(type_name(new List<object>())(st3), _p0_, _p1_) : default) : default);

    public static ParseExprResult parse_record_expr_fields(Token type_name, List<RecordFieldExpr> acc, ParseState st) => (is_right_brace(current_kind(st)) ? new ExprOk(new RecordExpr(type_name(acc)), advance(st)) : (is_ident(current_kind(st)) ? (_p0_) => (_p1_) => parse_record_field(type_name(acc(st)), _p0_, _p1_) : new ExprOk(new RecordExpr(type_name(acc)), st)));

    public static ParseExprResult parse_record_field(Token type_name, List<RecordFieldExpr> acc, ParseState st) => ((Token field_name = current(st)) is var _ ? ((ParseState st2 = advance(st)) is var _ ? ((ParseState st3 = expect(Equals, st2)) is var _ ? ((ParseExprResult val_result = parse_expr(st3)) is var _ ? (_p0_) => unwrap_expr_ok(val_result((_p0_) => (_p1_) => (_p2_) => (_p3_) => finish_record_field(type_name(acc(field_name)), _p0_, _p1_, _p2_, _p3_)), _p0_) : default) : default) : default) : default);

    public static ParseExprResult finish_record_field(Token type_name, List<RecordFieldExpr> acc, Token field_name, Expr v, ParseState st) => ((object field = new RecordFieldExpr(name: field_name, value: v)) is var _ ? ((ParseState st2 = skip_newlines(st)) is var _ ? (is_comma(current_kind(st2)) ? (_p0_) => (_p1_) => parse_record_expr_fields(type_name((acc + new List<object> { field }))(skip_newlines(advance(st2))), _p0_, _p1_) : (_p0_) => (_p1_) => parse_record_expr_fields(type_name((acc + new List<object> { field }))(st2), _p0_, _p1_)) : default) : default);

    public static ParseExprResult parse_list_expr(ParseState st) => ((ParseState st2 = advance(st)) is var _ ? ((ParseState st3 = skip_newlines(st2)) is var _ ? parse_list_elements(new List<Expr>(), st3) : default) : default);

    public static ParseExprResult parse_list_elements(List<Expr> acc, ParseState st) => (is_right_bracket(current_kind(st)) ? new ExprOk(new ListExpr(acc), advance(st)) : ((ParseExprResult elem = parse_expr(st)) is var _ ? (_p0_) => unwrap_expr_ok(elem((_p0_) => (_p1_) => finish_list_element(acc, _p0_, _p1_)), _p0_) : default));

    public static ParseExprResult finish_list_element(List<Expr> acc, Expr e, ParseState st) => ((ParseState st2 = skip_newlines(st)) is var _ ? (is_comma(current_kind(st2)) ? parse_list_elements((acc + new List<Expr> { e }), skip_newlines(advance(st2))) : parse_list_elements((acc + new List<Expr> { e }), st2)) : default);

    public static ParseExprResult parse_if_expr(ParseState st) => ((ParseState st2 = skip_newlines(advance(st))) is var _ ? ((ParseExprResult cond = parse_expr(st2)) is var _ ? (_p0_) => unwrap_expr_ok(cond(parse_if_then), _p0_) : default) : default);

    public static ParseExprResult parse_if_then(Expr c, ParseState st) => ((ParseState st2 = skip_newlines(st)) is var _ ? ((ParseState st3 = expect(ThenKeyword, st2)) is var _ ? ((ParseState st4 = skip_newlines(st3)) is var _ ? ((ParseExprResult then_result = parse_expr(st4)) is var _ ? (_p0_) => unwrap_expr_ok(then_result((_p0_) => (_p1_) => parse_if_else(c, _p0_, _p1_)), _p0_) : default) : default) : default) : default);

    public static ParseExprResult parse_if_else(Expr c, Expr t, ParseState st) => ((ParseState st2 = skip_newlines(st)) is var _ ? ((ParseState st3 = expect(ElseKeyword, st2)) is var _ ? ((ParseState st4 = skip_newlines(st3)) is var _ ? ((ParseExprResult else_result = parse_expr(st4)) is var _ ? (_p0_) => unwrap_expr_ok(else_result((_p0_) => (_p1_) => (_p2_) => finish_if(c(t), _p0_, _p1_, _p2_)), _p0_) : default) : default) : default) : default);

    public static ParseExprResult finish_if(Expr c, Expr t, Expr e, ParseState st) => new ExprOk(new IfExpr(c(t(e))), st);

    public static ParseExprResult parse_let_expr(ParseState st) => ((ParseState st2 = skip_newlines(advance(st))) is var _ ? parse_let_bindings(new List<LetBind>(), st2) : default);

    public static ParseExprResult parse_let_bindings(List<LetBind> acc, ParseState st) => (is_ident(current_kind(st)) ? (_p0_) => parse_let_binding(acc(st), _p0_) : (is_in_keyword(current_kind(st)) ? ((ParseState st2 = skip_newlines(advance(st))) is var _ ? ((ParseExprResult body = parse_expr(st2)) is var _ ? (_p0_) => unwrap_expr_ok(body((_p0_) => (_p1_) => finish_let(acc, _p0_, _p1_)), _p0_) : default) : default) : ((ParseExprResult body = parse_expr(st)) is var _ ? (_p0_) => unwrap_expr_ok(body((_p0_) => (_p1_) => finish_let(acc, _p0_, _p1_)), _p0_) : default)));

    public static ParseExprResult finish_let(List<LetBind> acc, Expr b, ParseState st) => new ExprOk(new LetExpr(acc(b)), st);

    public static ParseExprResult parse_let_binding(List<LetBind> acc, ParseState st) => ((Token name_tok = current(st)) is var _ ? ((ParseState st2 = advance(st)) is var _ ? ((ParseState st3 = expect(Equals, st2)) is var _ ? ((ParseExprResult val_result = parse_expr(st3)) is var _ ? (_p0_) => unwrap_expr_ok(val_result((_p0_) => (_p1_) => (_p2_) => finish_let_binding(acc(name_tok), _p0_, _p1_, _p2_)), _p0_) : default) : default) : default) : default);

    public static ParseExprResult finish_let_binding(List<LetBind> acc, Token name_tok, Expr v, ParseState st) => ((object binding = new LetBind(name: name_tok, value: v)) is var _ ? ((ParseState st2 = skip_newlines(st)) is var _ ? (is_comma(current_kind(st2)) ? parse_let_bindings((acc + new List<LetBind> { binding }), skip_newlines(advance(st2))) : parse_let_bindings((acc + new List<LetBind> { binding }), st2)) : default) : default);

    public static ParseExprResult parse_match_expr(ParseState st) => ((ParseState st2 = advance(st)) is var _ ? ((ParseExprResult scrut = parse_expr(st2)) is var _ ? (_p0_) => unwrap_expr_ok(scrut(start_match_branches), _p0_) : default) : default);

    public static ParseExprResult start_match_branches(Expr s, ParseState st) => ((ParseState st2 = skip_newlines(st)) is var _ ? (_p0_) => (_p1_) => parse_match_branches(s(new List<object>())(st2), _p0_, _p1_) : default);

    public static ParseExprResult parse_match_branches(Expr scrut, List<MatchArm> acc, ParseState st) => (is_if_keyword(current_kind(st)) ? (_p0_) => (_p1_) => parse_one_match_branch(scrut(acc(st)), _p0_, _p1_) : new ExprOk(new MatchExpr(scrut(acc)), st));

    public static ParseExprResult unwrap_pat_for_expr(ParsePatResult r, Func<Pat, Func<ParseState, ParseExprResult>> f) => r switch { PatOk(var p, var st) => f(p(st)),  };

    public static ParseExprResult parse_one_match_branch(Expr scrut, List<MatchArm> acc, ParseState st) => ((ParseState st2 = advance(st)) is var _ ? ((ParsePatResult pat = parse_pattern(st2)) is var _ ? (_p0_) => unwrap_pat_for_expr(pat((_p0_) => (_p1_) => (_p2_) => parse_match_branch_body(scrut(acc), _p0_, _p1_, _p2_)), _p0_) : default) : default);

    public static ParseExprResult parse_match_branch_body(Expr scrut, List<MatchArm> acc, Pat p, ParseState st) => ((ParseState st2 = expect(Arrow, st)) is var _ ? ((ParseState st3 = skip_newlines(st2)) is var _ ? ((ParseExprResult body = parse_expr(st3)) is var _ ? (_p0_) => unwrap_expr_ok(body((_p0_) => (_p1_) => (_p2_) => (_p3_) => finish_match_branch(scrut(acc(p)), _p0_, _p1_, _p2_, _p3_)), _p0_) : default) : default) : default);

    public static ParseExprResult finish_match_branch(Expr scrut, List<MatchArm> acc, Pat p, Expr b, ParseState st) => ((object arm = new MatchArm(pattern: p, body: b)) is var _ ? ((ParseState st2 = skip_newlines(st)) is var _ ? (_p0_) => (_p1_) => parse_match_branches(scrut((acc + new List<object> { arm }))(st2), _p0_, _p1_) : default) : default);

    public static ParseExprResult parse_do_expr(ParseState st) => ((ParseState st2 = skip_newlines(advance(st))) is var _ ? parse_do_stmts(new List<DoStmt>(), st2) : default);

    public static ParseExprResult parse_do_stmts(List<DoStmt> acc, ParseState st) => (is_done(st) ? new ExprOk(new DoExpr(acc), st) : (is_dedent(current_kind(st)) ? new ExprOk(new DoExpr(acc), st) : (is_do_bind(st) ? (_p0_) => parse_do_bind_stmt(acc(st), _p0_) : (_p0_) => parse_do_expr_stmt(acc(st), _p0_))));

    public static bool is_do_bind(ParseState st) => (is_ident(current_kind(st)) ? is_left_arrow((_p0_) => peek_kind(st(1), _p0_)) : false);

    public static ParseExprResult parse_do_bind_stmt(List<DoStmt> acc, ParseState st) => ((Token name_tok = current(st)) is var _ ? ((ParseState st2 = advance(advance(st))) is var _ ? ((ParseExprResult val_result = parse_expr(st2)) is var _ ? (_p0_) => unwrap_expr_ok(val_result((_p0_) => (_p1_) => (_p2_) => finish_do_bind(acc(name_tok), _p0_, _p1_, _p2_)), _p0_) : default) : default) : default);

    public static ParseExprResult finish_do_bind(List<DoStmt> acc, Token name_tok, Expr v, ParseState st) => ((ParseState st2 = skip_newlines(st)) is var _ ? parse_do_stmts((acc + new List<DoStmt> { new DoBindStmt(name_tok(v)) }), st2) : default);

    public static ParseExprResult parse_do_expr_stmt(List<DoStmt> acc, ParseState st) => ((ParseExprResult expr_result = parse_expr(st)) is var _ ? (_p0_) => unwrap_expr_ok(expr_result((_p0_) => (_p1_) => finish_do_expr(acc, _p0_, _p1_)), _p0_) : default);

    public static ParseExprResult finish_do_expr(List<DoStmt> acc, Expr e, ParseState st) => ((ParseState st2 = skip_newlines(st)) is var _ ? parse_do_stmts((acc + new List<DoStmt> { new DoExprStmt(e) }), st2) : default);

    public static ParseTypeResult parse_type_annotation(ParseState st) => ((ParseState st2 = advance(st)) is var _ ? ((ParseState st3 = expect(Colon, st2)) is var _ ? parse_type(st3) : default) : default);

    public static ParseDefResult parse_definition(ParseState st) => (is_done(st) ? new DefNone(st) : (is_ident(current_kind(st)) ? try_parse_def(st) : (is_type_ident(current_kind(st)) ? try_parse_def(st) : new DefNone(st))));

    public static ParseDefResult try_parse_def(ParseState st) => (is_colon((_p0_) => peek_kind(st(1), _p0_)) ? ((ParseTypeResult ann_result = parse_type_annotation(st)) is var _ ? unwrap_type_for_def(ann_result) : default) : parse_def_body_with_ann(new List<TypeAnn>(), st));

    public static ParseDefResult unwrap_type_for_def(ParseTypeResult r) => r switch { TypeOk(var ann_type, var st) => ((object name_tok = new Token(kind: Identifier, text: "", offset: 0, line: 0, column: 0)) is var _ ? ((List<object> ann = new List<object> { new TypeAnn(name: name_tok, type_expr: ann_type) }) is var _ ? ((ParseState st2 = skip_newlines(st)) is var _ ? (_p0_) => parse_def_body_with_ann(ann(st2), _p0_) : default) : default) : default),  };

    public static ParseDefResult parse_def_body_with_ann(List<TypeAnn> ann, ParseState st) => ((Token name_tok = current(st)) is var _ ? ((ParseState st2 = advance(st)) is var _ ? (_p0_) => (_p1_) => (_p2_) => parse_def_params_then(ann(name_tok(new List<object>())(st2)), _p0_, _p1_, _p2_) : default) : default);

    public static ParseDefResult parse_def_params_then(List<TypeAnn> ann, Token name_tok, List<Token> acc, ParseState st) => (is_left_paren(current_kind(st)) ? ((ParseState st2 = advance(st)) is var _ ? (is_ident(current_kind(st2)) ? ((Token param = current(st2)) is var _ ? ((ParseState st3 = advance(st2)) is var _ ? ((ParseState st4 = expect(RightParen, st3)) is var _ ? (_p0_) => (_p1_) => (_p2_) => parse_def_params_then(ann(name_tok((acc + new List<object> { param }))(st4)), _p0_, _p1_, _p2_) : default) : default) : default) : (_p0_) => (_p1_) => (_p2_) => finish_def(ann(name_tok(acc(st))), _p0_, _p1_, _p2_)) : default) : (_p0_) => (_p1_) => (_p2_) => finish_def(ann(name_tok(acc(st))), _p0_, _p1_, _p2_));

    public static ParseDefResult finish_def(List<TypeAnn> ann, Token name_tok, List<Token> @params, ParseState st) => ((ParseState st2 = expect(Equals, st)) is var _ ? ((ParseState st3 = skip_newlines(st2)) is var _ ? ((ParseExprResult body_result = parse_expr(st3)) is var _ ? (_p0_) => (_p1_) => (_p2_) => unwrap_def_body(body_result(ann(name_tok(@params))), _p0_, _p1_, _p2_) : default) : default) : default);

    public static ParseDefResult unwrap_def_body(ParseExprResult r, List<TypeAnn> ann, Token name_tok, List<Token> @params) => r switch { ExprOk(var b, var st) => new DefOk(new Def(name: name_tok, @params: @params, ann: ann, body: b), st),  };

    public static ParseTypeDefResult parse_type_def(ParseState st) => (is_type_ident(current_kind(st)) ? ((Token name_tok = current(st)) is var _ ? ((ParseState st2 = advance(st)) is var _ ? (is_equals(current_kind(st2)) ? ((ParseState st3 = skip_newlines(advance(st2))) is var _ ? (is_record_keyword(current_kind(st3)) ? (_p0_) => parse_record_type(name_tok(st3), _p0_) : (is_pipe(current_kind(st3)) ? (_p0_) => parse_variant_type(name_tok(st3), _p0_) : new TypeDefNone(st))) : default) : new TypeDefNone(st)) : default) : default) : new TypeDefNone(st));

    public static ParseTypeDefResult parse_record_type(Token name_tok, ParseState st) => ((ParseState st2 = advance(st)) is var _ ? ((ParseState st3 = expect(LeftBrace, st2)) is var _ ? ((ParseState st4 = skip_newlines(st3)) is var _ ? (_p0_) => (_p1_) => parse_record_fields_loop(name_tok(new List<object>())(st4), _p0_, _p1_) : default) : default) : default);

    public static ParseTypeDefResult parse_record_fields_loop(Token name_tok, List<RecordFieldDef> acc, ParseState st) => (is_right_brace(current_kind(st)) ? new TypeDefOk(new TypeDef(name: name_tok, type_params: new List<object>(), body: new RecordBody(acc)), advance(st)) : (is_ident(current_kind(st)) ? (_p0_) => (_p1_) => parse_one_record_field(name_tok(acc(st)), _p0_, _p1_) : new TypeDefOk(new TypeDef(name: name_tok, type_params: new List<object>(), body: new RecordBody(acc)), st)));

    public static ParseTypeDefResult parse_one_record_field(Token name_tok, List<RecordFieldDef> acc, ParseState st) => ((Token field_name = current(st)) is var _ ? ((ParseState st2 = advance(st)) is var _ ? ((ParseState st3 = expect(Colon, st2)) is var _ ? ((ParseTypeResult field_type_result = parse_type(st3)) is var _ ? (_p0_) => (_p1_) => (_p2_) => unwrap_record_field_type(name_tok(acc(field_name(field_type_result))), _p0_, _p1_, _p2_) : default) : default) : default) : default);

    public static ParseTypeDefResult unwrap_record_field_type(Token name_tok, List<RecordFieldDef> acc, Token field_name, ParseTypeResult r) => r switch { TypeOk(var ft, var st) => ((object field = new RecordFieldDef(name: field_name, type_expr: ft)) is var _ ? ((ParseState st2 = skip_newlines(st)) is var _ ? (is_comma(current_kind(st2)) ? (_p0_) => (_p1_) => parse_record_fields_loop(name_tok((acc + new List<object> { field }))(skip_newlines(advance(st2))), _p0_, _p1_) : (_p0_) => (_p1_) => parse_record_fields_loop(name_tok((acc + new List<object> { field }))(st2), _p0_, _p1_)) : default) : default),  };

    public static ParseTypeDefResult parse_variant_type(Token name_tok, ParseState st) => (_p0_) => (_p1_) => parse_variant_ctors(name_tok(new List<object>())(st), _p0_, _p1_);

    public static ParseTypeDefResult parse_variant_ctors(Token name_tok, List<VariantCtorDef> acc, ParseState st) => (is_pipe(current_kind(st)) ? ((ParseState st2 = skip_newlines(advance(st))) is var _ ? ((Token ctor_name = current(st2)) is var _ ? ((ParseState st3 = advance(st2)) is var _ ? (_p0_) => (_p1_) => (_p2_) => (_p3_) => parse_ctor_fields(ctor_name(new List<object>())(st3(name_tok(acc))), _p0_, _p1_, _p2_, _p3_) : default) : default) : default) : new TypeDefOk(new TypeDef(name: name_tok, type_params: new List<object>(), body: new VariantBody(acc)), st));

    public static ParseTypeDefResult parse_ctor_fields(Token ctor_name, List<TypeExpr> fields, ParseState st, Token name_tok, List<VariantCtorDef> acc) => (is_left_paren(current_kind(st)) ? ((ParseTypeResult field_result = parse_type(advance(st))) is var _ ? (_p0_) => (_p1_) => (_p2_) => (_p3_) => unwrap_ctor_field(field_result(ctor_name(fields(name_tok(acc)))), _p0_, _p1_, _p2_, _p3_) : default) : ((ParseState st2 = skip_newlines(st)) is var _ ? ((object ctor = new VariantCtorDef(name: ctor_name, fields: fields)) is var _ ? (_p0_) => (_p1_) => parse_variant_ctors(name_tok((acc + new List<object> { ctor }))(st2), _p0_, _p1_) : default) : default));

    public static ParseTypeDefResult unwrap_ctor_field(ParseTypeResult r, Token ctor_name, List<TypeExpr> fields, Token name_tok, List<VariantCtorDef> acc) => r switch { TypeOk(var ty, var st) => ((ParseState st2 = expect(RightParen, st)) is var _ ? (_p0_) => (_p1_) => (_p2_) => (_p3_) => parse_ctor_fields(ctor_name((fields + new List<object> { ty }))(st2(name_tok(acc))), _p0_, _p1_, _p2_, _p3_) : default),  };

    public static Document parse_document(ParseState st) => ((ParseState st2 = skip_newlines(st)) is var _ ? parse_top_level(new List<Def>(), new List<TypeDef>(), st2) : default);

    public static Document parse_top_level(List<Def> defs, List<TypeDef> type_defs, ParseState st) => (is_done(st) ? new Document(defs: defs, type_defs: type_defs) : (_p0_) => (_p1_) => try_top_level_type_def(defs(type_defs(st)), _p0_, _p1_));

    public static Document try_top_level_type_def(List<Def> defs, List<TypeDef> type_defs, ParseState st) => ((ParseTypeDefResult td_result = parse_type_def(st)) is var _ ? td_result switch { TypeDefOk(var td, var st2) => (_p0_) => (_p1_) => parse_top_level(defs((type_defs + new List<object> { td }))(skip_newlines(st2)), _p0_, _p1_), TypeDefNone(var st2) => (_p0_) => (_p1_) => try_top_level_def(defs(type_defs(st)), _p0_, _p1_),  } : default);

    public static Document try_top_level_def(List<Def> defs, List<TypeDef> type_defs, ParseState st) => ((ParseDefResult def_result = parse_definition(st)) is var _ ? def_result switch { DefOk(var d, var st2) => (_p0_) => parse_top_level((defs + new List<Def> { d }), type_defs(skip_newlines(st2)), _p0_), DefNone(var st2) => (_p0_) => (_p1_) => parse_top_level(defs(type_defs(skip_newlines(advance(st2)))), _p0_, _p1_),  } : default);

    public static long token_length(Token t) => text_length(t.text);

    public static CheckResult infer_literal(UnificationState st, LiteralKind kind) => kind switch { IntLit { } => new CheckResult(inferred_type: IntegerTy, state: st), NumLit { } => new CheckResult(inferred_type: NumberTy, state: st), TextLit { } => new CheckResult(inferred_type: TextTy, state: st), BoolLit { } => new CheckResult(inferred_type: BooleanTy, state: st),  };

    public static CheckResult infer_name(UnificationState st, TypeEnv env, string name) => ((_p0_) => env_has(env(name), _p0_) ? ((Func<string, CodexType> raw = (_p0_) => env_lookup(env(name), _p0_)) is var _ ? ((Func<CodexType, FreshResult> inst = (_p0_) => instantiate_type(st(raw), _p0_)) is var _ ? new CheckResult(inferred_type: inst.var_type, state: inst.state) : default) : default) : new CheckResult(inferred_type: ErrorTy, state: (_p0_) => (_p1_) => add_unify_error(st("CDX2002")(("Unknown name: " + name)), _p0_, _p1_)));

    public static FreshResult instantiate_type(UnificationState st, CodexType ty) => ty switch { ForAllTy(var var_id, var body) => ((FreshResult fr = fresh_and_advance(st)) is var _ ? ((Func<long, Func<CodexType, CodexType>> substituted = (_p0_) => (_p1_) => subst_type_var(body(var_id(fr.var_type)), _p0_, _p1_)) is var _ ? (_p0_) => instantiate_type(fr.state(substituted), _p0_) : default) : default), _ => new FreshResult(var_type: ty, state: st),  };

    public static CodexType subst_type_var(CodexType ty, long var_id, CodexType replacement) => ty switch { TypeVar(var id) => ((id == var_id) ? replacement : ty), FunTy(var param, var ret) => new FunTy((_p0_) => (_p1_) => subst_type_var(param(var_id(replacement)), _p0_, _p1_), (_p0_) => (_p1_) => subst_type_var(ret(var_id(replacement)), _p0_, _p1_)), ListTy(var elem) => new ListTy((_p0_) => (_p1_) => subst_type_var(elem(var_id(replacement)), _p0_, _p1_)), ForAllTy(var inner_id, var body) => ((inner_id == var_id) ? ty : new ForAllTy(inner_id((_p0_) => (_p1_) => subst_type_var(body(var_id(replacement)), _p0_, _p1_)))), ConstructedTy(var name, var args) => new ConstructedTy(name((_p0_) => (_p1_) => (_p2_) => (_p3_) => (_p4_) => map_subst_type_var(args(var_id(replacement(0)(list_length(args))(new List<object>()))), _p0_, _p1_, _p2_, _p3_, _p4_))), SumTy(var name, var ctors) => ty, RecordTy(var name, var fields) => ty, _ => ty,  };

    public static List<CodexType> map_subst_type_var(List<CodexType> args, long var_id, CodexType replacement, long i, long len, List<CodexType> acc) => ((i == len) ? acc : (_p0_) => (_p1_) => (_p2_) => (_p3_) => (_p4_) => map_subst_type_var(args(var_id(replacement((i + 1))(len((acc + new List<object> { (_p0_) => subst_type_var(list_at(args(i)), var_id(replacement), _p0_) }))))), _p0_, _p1_, _p2_, _p3_, _p4_));

    public static CheckResult infer_binary(UnificationState st, TypeEnv env, AExpr left, BinaryOp op, AExpr right) => ((Func<TypeEnv, Func<AExpr, CheckResult>> lr = (_p0_) => (_p1_) => infer_expr(st(env(left)), _p0_, _p1_)) is var _ ? ((Func<TypeEnv, Func<AExpr, CheckResult>> rr = (_p0_) => (_p1_) => infer_expr(lr.state(env(right)), _p0_, _p1_)) is var _ ? (_p0_) => (_p1_) => (_p2_) => infer_binary_op(rr.state(lr.inferred_type(rr.inferred_type(op))), _p0_, _p1_, _p2_) : default) : default);

    public static CheckResult infer_binary_op(UnificationState st, CodexType lt, CodexType rt, BinaryOp op) => op switch { OpAdd { } => (_p0_) => (_p1_) => infer_arithmetic(st(lt(rt)), _p0_, _p1_), OpSub { } => (_p0_) => (_p1_) => infer_arithmetic(st(lt(rt)), _p0_, _p1_), OpMul { } => (_p0_) => (_p1_) => infer_arithmetic(st(lt(rt)), _p0_, _p1_), OpDiv { } => (_p0_) => (_p1_) => infer_arithmetic(st(lt(rt)), _p0_, _p1_), OpPow { } => (_p0_) => (_p1_) => infer_arithmetic(st(lt(rt)), _p0_, _p1_), OpEq { } => (_p0_) => (_p1_) => infer_comparison(st(lt(rt)), _p0_, _p1_), OpNotEq { } => (_p0_) => (_p1_) => infer_comparison(st(lt(rt)), _p0_, _p1_), OpLt { } => (_p0_) => (_p1_) => infer_comparison(st(lt(rt)), _p0_, _p1_), OpGt { } => (_p0_) => (_p1_) => infer_comparison(st(lt(rt)), _p0_, _p1_), OpLtEq { } => (_p0_) => (_p1_) => infer_comparison(st(lt(rt)), _p0_, _p1_), OpGtEq { } => (_p0_) => (_p1_) => infer_comparison(st(lt(rt)), _p0_, _p1_), OpAnd { } => (_p0_) => (_p1_) => infer_logical(st(lt(rt)), _p0_, _p1_), OpOr { } => (_p0_) => (_p1_) => infer_logical(st(lt(rt)), _p0_, _p1_), OpAppend { } => (_p0_) => (_p1_) => infer_append(st(lt(rt)), _p0_, _p1_), OpCons { } => (_p0_) => (_p1_) => infer_cons(st(lt(rt)), _p0_, _p1_), OpDefEq { } => (_p0_) => (_p1_) => infer_comparison(st(lt(rt)), _p0_, _p1_),  };

    public static CheckResult infer_arithmetic(UnificationState st, CodexType lt, CodexType rt) => ((Func<CodexType, Func<CodexType, UnifyResult>> r = (_p0_) => (_p1_) => unify(st(lt(rt)), _p0_, _p1_)) is var _ ? new CheckResult(inferred_type: lt, state: r.state) : default);

    public static CheckResult infer_comparison(UnificationState st, CodexType lt, CodexType rt) => ((Func<CodexType, Func<CodexType, UnifyResult>> r = (_p0_) => (_p1_) => unify(st(lt(rt)), _p0_, _p1_)) is var _ ? new CheckResult(inferred_type: BooleanTy, state: r.state) : default);

    public static CheckResult infer_logical(UnificationState st, CodexType lt, CodexType rt) => ((Func<CodexType, Func<CodexType, UnifyResult>> r1 = (_p0_) => (_p1_) => unify(st(lt(BooleanTy)), _p0_, _p1_)) is var _ ? ((Func<CodexType, Func<CodexType, UnifyResult>> r2 = (_p0_) => (_p1_) => unify(r1.state(rt(BooleanTy)), _p0_, _p1_)) is var _ ? new CheckResult(inferred_type: BooleanTy, state: r2.state) : default) : default);

    public static CheckResult infer_append(UnificationState st, CodexType lt, CodexType rt) => ((Func<CodexType, CodexType> resolved = (_p0_) => resolve(st(lt), _p0_)) is var _ ? resolved switch { TextTy { } => ((Func<CodexType, Func<CodexType, UnifyResult>> r = (_p0_) => (_p1_) => unify(st(rt(TextTy)), _p0_, _p1_)) is var _ ? new CheckResult(inferred_type: TextTy, state: r.state) : default), _ => ((Func<CodexType, Func<CodexType, UnifyResult>> r = (_p0_) => (_p1_) => unify(st(lt(rt)), _p0_, _p1_)) is var _ ? new CheckResult(inferred_type: lt, state: r.state) : default),  } : default);

    public static CheckResult infer_cons(UnificationState st, CodexType lt, CodexType rt) => ((CodexType list_ty = new ListTy(lt)) is var _ ? ((Func<CodexType, Func<CodexType, UnifyResult>> r = (_p0_) => (_p1_) => unify(st(rt(list_ty)), _p0_, _p1_)) is var _ ? new CheckResult(inferred_type: list_ty, state: r.state) : default) : default);

    public static CheckResult infer_if(UnificationState st, TypeEnv env, AExpr cond, AExpr then_e, AExpr else_e) => ((Func<TypeEnv, Func<AExpr, CheckResult>> cr = (_p0_) => (_p1_) => infer_expr(st(env(cond)), _p0_, _p1_)) is var _ ? ((Func<CodexType, Func<CodexType, UnifyResult>> r1 = (_p0_) => (_p1_) => unify(cr.state(cr.inferred_type(BooleanTy)), _p0_, _p1_)) is var _ ? ((Func<TypeEnv, Func<AExpr, CheckResult>> tr = (_p0_) => (_p1_) => infer_expr(r1.state(env(then_e)), _p0_, _p1_)) is var _ ? ((Func<TypeEnv, Func<AExpr, CheckResult>> er = (_p0_) => (_p1_) => infer_expr(tr.state(env(else_e)), _p0_, _p1_)) is var _ ? ((Func<CodexType, Func<CodexType, UnifyResult>> r2 = (_p0_) => (_p1_) => unify(er.state(tr.inferred_type(er.inferred_type)), _p0_, _p1_)) is var _ ? new CheckResult(inferred_type: tr.inferred_type, state: r2.state) : default) : default) : default) : default) : default);

    public static CheckResult infer_let(UnificationState st, TypeEnv env, List<ALetBind> bindings, AExpr body) => ((Func<TypeEnv, Func<List<ALetBind>, Func<long, Func<long, LetBindResult>>>> env2 = (_p0_) => (_p1_) => (_p2_) => (_p3_) => infer_let_bindings(st(env(bindings(0)(list_length(bindings)))), _p0_, _p1_, _p2_, _p3_)) is var _ ? (_p0_) => (_p1_) => infer_expr(env2.state(env2.env(body)), _p0_, _p1_) : default);

    public static LetBindResult infer_let_bindings(UnificationState st, TypeEnv env, List<ALetBind> bindings, long i, long len) => ((i == len) ? new LetBindResult(state: st, env: env) : ((object b = list_at(bindings(i))) is var _ ? ((Func<TypeEnv, Func<AExpr, CheckResult>> vr = (_p0_) => (_p1_) => infer_expr(st(env(b.value)), _p0_, _p1_)) is var _ ? ((Func<string, Func<CodexType, TypeEnv>> env2 = (_p0_) => (_p1_) => env_bind(env(b.name.value(vr.inferred_type)), _p0_, _p1_)) is var _ ? (_p0_) => (_p1_) => (_p2_) => (_p3_) => infer_let_bindings(vr.state(env2(bindings((i + 1))(len))), _p0_, _p1_, _p2_, _p3_) : default) : default) : default));

    public static CheckResult infer_lambda(UnificationState st, TypeEnv env, List<Name> @params, AExpr body) => ((Func<TypeEnv, Func<List<Name>, Func<long, Func<long, Func<List<CodexType>, LambdaBindResult>>>>> pr = (_p0_) => (_p1_) => (_p2_) => (_p3_) => (_p4_) => bind_lambda_params(st(env(@params(0)(list_length(@params))(new List<object>()))), _p0_, _p1_, _p2_, _p3_, _p4_)) is var _ ? ((Func<TypeEnv, Func<AExpr, CheckResult>> br = (_p0_) => (_p1_) => infer_expr(pr.state(pr.env(body)), _p0_, _p1_)) is var _ ? ((Func<CodexType, CodexType> fun_ty = (_p0_) => wrap_fun_type(pr.param_types(br.inferred_type), _p0_)) is var _ ? new CheckResult(inferred_type: fun_ty, state: br.state) : default) : default) : default);

    public static LambdaBindResult bind_lambda_params(UnificationState st, TypeEnv env, List<Name> @params, long i, long len, List<CodexType> acc) => ((i == len) ? new LambdaBindResult(state: st, env: env, param_types: acc) : ((object p = list_at(@params(i))) is var _ ? ((FreshResult fr = fresh_and_advance(st)) is var _ ? ((Func<string, Func<CodexType, TypeEnv>> env2 = (_p0_) => (_p1_) => env_bind(env(p.value(fr.var_type)), _p0_, _p1_)) is var _ ? (_p0_) => (_p1_) => (_p2_) => (_p3_) => (_p4_) => bind_lambda_params(fr.state(env2(@params((i + 1))(len((acc + new List<object> { fr.var_type }))))), _p0_, _p1_, _p2_, _p3_, _p4_) : default) : default) : default));

    public static CodexType wrap_fun_type(List<CodexType> param_types, CodexType result) => (_p0_) => (_p1_) => wrap_fun_type_loop(param_types(result((list_length(param_types) - 1))), _p0_, _p1_);

    public static CodexType wrap_fun_type_loop(List<CodexType> param_types, CodexType result, long i) => ((i < 0) ? result : (_p0_) => (_p1_) => wrap_fun_type_loop(param_types(new FunTy(list_at(param_types(i)), result))((i - 1)), _p0_, _p1_));

    public static CheckResult infer_application(UnificationState st, TypeEnv env, AExpr func, AExpr arg) => ((Func<TypeEnv, Func<AExpr, CheckResult>> fr = (_p0_) => (_p1_) => infer_expr(st(env(func)), _p0_, _p1_)) is var _ ? ((Func<TypeEnv, Func<AExpr, CheckResult>> ar = (_p0_) => (_p1_) => infer_expr(fr.state(env(arg)), _p0_, _p1_)) is var _ ? ((FreshResult ret = fresh_and_advance(ar.state)) is var _ ? ((Func<CodexType, Func<CodexType, UnifyResult>> r = (_p0_) => (_p1_) => unify(ret.state(fr.inferred_type(new FunTy(ar.inferred_type(ret.var_type)))), _p0_, _p1_)) is var _ ? new CheckResult(inferred_type: ret.var_type, state: r.state) : default) : default) : default) : default);

    public static CheckResult infer_list(UnificationState st, TypeEnv env, List<AExpr> elems) => ((list_length(elems) == 0) ? ((FreshResult fr = fresh_and_advance(st)) is var _ ? new CheckResult(inferred_type: new ListTy(fr.var_type), state: fr.state) : default) : ((Func<TypeEnv, Func<AExpr, CheckResult>> first = (_p0_) => (_p1_) => infer_expr(st(env(list_at(elems(0)))), _p0_, _p1_)) is var _ ? ((Func<TypeEnv, Func<List<AExpr>, Func<CodexType, Func<long, Func<long, UnificationState>>>>> st2 = (_p0_) => (_p1_) => (_p2_) => (_p3_) => (_p4_) => unify_list_elems(first.state(env(elems(first.inferred_type(1)(list_length(elems))))), _p0_, _p1_, _p2_, _p3_, _p4_)) is var _ ? new CheckResult(inferred_type: new ListTy(first.inferred_type), state: st2) : default) : default));

    public static UnificationState unify_list_elems(UnificationState st, TypeEnv env, List<AExpr> elems, CodexType elem_ty, long i, long len) => ((i == len) ? st : ((Func<TypeEnv, Func<AExpr, CheckResult>> er = (_p0_) => (_p1_) => infer_expr(st(env(list_at(elems(i)))), _p0_, _p1_)) is var _ ? ((Func<CodexType, Func<CodexType, UnifyResult>> r = (_p0_) => (_p1_) => unify(er.state(er.inferred_type(elem_ty)), _p0_, _p1_)) is var _ ? (_p0_) => (_p1_) => (_p2_) => (_p3_) => (_p4_) => unify_list_elems(r.state(env(elems(elem_ty((i + 1))(len)))), _p0_, _p1_, _p2_, _p3_, _p4_) : default) : default));

    public static CheckResult infer_match(UnificationState st, TypeEnv env, AExpr scrutinee, List<AMatchArm> arms) => ((Func<TypeEnv, Func<AExpr, CheckResult>> sr = (_p0_) => (_p1_) => infer_expr(st(env(scrutinee)), _p0_, _p1_)) is var _ ? ((FreshResult fr = fresh_and_advance(sr.state)) is var _ ? ((Func<TypeEnv, Func<CodexType, Func<CodexType, Func<List<AMatchArm>, Func<long, Func<long, UnificationState>>>>>> st2 = (_p0_) => (_p1_) => (_p2_) => (_p3_) => (_p4_) => (_p5_) => infer_match_arms(fr.state(env(sr.inferred_type(fr.var_type(arms(0)(list_length(arms)))))), _p0_, _p1_, _p2_, _p3_, _p4_, _p5_)) is var _ ? new CheckResult(inferred_type: fr.var_type, state: st2) : default) : default) : default);

    public static UnificationState infer_match_arms(UnificationState st, TypeEnv env, CodexType scrut_ty, CodexType result_ty, List<AMatchArm> arms, long i, long len) => ((i == len) ? st : ((object arm = list_at(arms(i))) is var _ ? ((Func<TypeEnv, Func<APat, Func<CodexType, PatBindResult>>> pr = (_p0_) => (_p1_) => (_p2_) => bind_pattern(st(env(arm.pattern(scrut_ty))), _p0_, _p1_, _p2_)) is var _ ? ((Func<TypeEnv, Func<AExpr, CheckResult>> br = (_p0_) => (_p1_) => infer_expr(pr.state(pr.env(arm.body)), _p0_, _p1_)) is var _ ? ((Func<CodexType, Func<CodexType, UnifyResult>> r = (_p0_) => (_p1_) => unify(br.state(br.inferred_type(result_ty)), _p0_, _p1_)) is var _ ? (_p0_) => (_p1_) => (_p2_) => (_p3_) => (_p4_) => (_p5_) => infer_match_arms(r.state(env(scrut_ty(result_ty(arms((i + 1))(len))))), _p0_, _p1_, _p2_, _p3_, _p4_, _p5_) : default) : default) : default) : default));

    public static PatBindResult bind_pattern(UnificationState st, TypeEnv env, APat pat, CodexType ty) => pat switch { AVarPat(var name) => new PatBindResult(state: st, env: (_p0_) => (_p1_) => env_bind(env(name.value(ty)), _p0_, _p1_)), AWildPat { } => new PatBindResult(state: st, env: env), ALitPat(var val, var kind) => new PatBindResult(state: st, env: env), ACtorPat(var ctor_name, var sub_pats) => ((Func<CodexType, FreshResult> ctor_lookup = (_p0_) => instantiate_type(st((_p0_) => env_lookup(env(ctor_name.value), _p0_)), _p0_)) is var _ ? (_p0_) => (_p1_) => (_p2_) => (_p3_) => (_p4_) => bind_ctor_sub_patterns(ctor_lookup.state(env(sub_pats(ctor_lookup.var_type(0)(list_length(sub_pats))))), _p0_, _p1_, _p2_, _p3_, _p4_) : default),  };

    public static PatBindResult bind_ctor_sub_patterns(UnificationState st, TypeEnv env, List<APat> sub_pats, CodexType ctor_ty, long i, long len) => ((i == len) ? new PatBindResult(state: st, env: env) : ctor_ty switch { FunTy(var param_ty, var ret_ty) => ((Func<TypeEnv, Func<APat, Func<CodexType, PatBindResult>>> pr = (_p0_) => (_p1_) => (_p2_) => bind_pattern(st(env(list_at(sub_pats(i)))(param_ty)), _p0_, _p1_, _p2_)) is var _ ? (_p0_) => (_p1_) => (_p2_) => (_p3_) => (_p4_) => bind_ctor_sub_patterns(pr.state(pr.env(sub_pats(ret_ty((i + 1))(len)))), _p0_, _p1_, _p2_, _p3_, _p4_) : default), _ => ((FreshResult fr = fresh_and_advance(st)) is var _ ? ((Func<TypeEnv, Func<APat, Func<CodexType, PatBindResult>>> pr = (_p0_) => (_p1_) => (_p2_) => bind_pattern(fr.state(env(list_at(sub_pats(i)))(fr.var_type)), _p0_, _p1_, _p2_)) is var _ ? (_p0_) => (_p1_) => (_p2_) => (_p3_) => (_p4_) => bind_ctor_sub_patterns(pr.state(pr.env(sub_pats(ctor_ty((i + 1))(len)))), _p0_, _p1_, _p2_, _p3_, _p4_) : default) : default),  });

    public static CheckResult infer_do(UnificationState st, TypeEnv env, List<ADoStmt> stmts) => (_p0_) => (_p1_) => (_p2_) => (_p3_) => (_p4_) => infer_do_loop(st(env(stmts(0)(list_length(stmts))(NothingTy))), _p0_, _p1_, _p2_, _p3_, _p4_);

    public static CheckResult infer_do_loop(UnificationState st, TypeEnv env, List<ADoStmt> stmts, long i, long len, CodexType last_ty) => ((i == len) ? new CheckResult(inferred_type: last_ty, state: st) : ((object stmt = list_at(stmts(i))) is var _ ? stmt switch { ADoExprStmt(var e) => ((Func<TypeEnv, Func<AExpr, CheckResult>> er = (_p0_) => (_p1_) => infer_expr(st(env(e)), _p0_, _p1_)) is var _ ? (_p0_) => (_p1_) => (_p2_) => (_p3_) => (_p4_) => infer_do_loop(er.state(env(stmts((i + 1))(len(er.inferred_type)))), _p0_, _p1_, _p2_, _p3_, _p4_) : default), ADoBindStmt(var name, var e) => ((Func<TypeEnv, Func<AExpr, CheckResult>> er = (_p0_) => (_p1_) => infer_expr(st(env(e)), _p0_, _p1_)) is var _ ? ((Func<string, Func<CodexType, TypeEnv>> env2 = (_p0_) => (_p1_) => env_bind(env(name.value(er.inferred_type)), _p0_, _p1_)) is var _ ? (_p0_) => (_p1_) => (_p2_) => (_p3_) => (_p4_) => infer_do_loop(er.state(env2(stmts((i + 1))(len(er.inferred_type)))), _p0_, _p1_, _p2_, _p3_, _p4_) : default) : default),  } : default));

    public static CheckResult infer_expr(UnificationState st, TypeEnv env, AExpr expr) => expr switch { ALitExpr(var val, var kind) => (_p0_) => infer_literal(st(kind), _p0_), ANameExpr(var name) => (_p0_) => (_p1_) => infer_name(st(env(name.value)), _p0_, _p1_), ABinaryExpr(var left, var op, var right) => (_p0_) => (_p1_) => (_p2_) => (_p3_) => infer_binary(st(env(left(op(right)))), _p0_, _p1_, _p2_, _p3_), AUnaryExpr(var operand) => ((Func<TypeEnv, Func<AExpr, CheckResult>> r = (_p0_) => (_p1_) => infer_expr(st(env(operand)), _p0_, _p1_)) is var _ ? ((Func<CodexType, Func<CodexType, UnifyResult>> u = (_p0_) => (_p1_) => unify(r.state(r.inferred_type(IntegerTy)), _p0_, _p1_)) is var _ ? new CheckResult(inferred_type: IntegerTy, state: u.state) : default) : default), AApplyExpr(var func, var arg) => (_p0_) => (_p1_) => (_p2_) => infer_application(st(env(func(arg))), _p0_, _p1_, _p2_), AIfExpr(var cond, var then_e, var else_e) => (_p0_) => (_p1_) => (_p2_) => (_p3_) => infer_if(st(env(cond(then_e(else_e)))), _p0_, _p1_, _p2_, _p3_), ALetExpr(var bindings, var body) => (_p0_) => (_p1_) => (_p2_) => infer_let(st(env(bindings(body))), _p0_, _p1_, _p2_), ALambdaExpr(var @params, var body) => (_p0_) => (_p1_) => (_p2_) => infer_lambda(st(env(@params(body))), _p0_, _p1_, _p2_), AMatchExpr(var scrutinee, var arms) => (_p0_) => (_p1_) => (_p2_) => infer_match(st(env(scrutinee(arms))), _p0_, _p1_, _p2_), AListExpr(var elems) => (_p0_) => (_p1_) => infer_list(st(env(elems)), _p0_, _p1_), ADoExpr(var stmts) => (_p0_) => (_p1_) => infer_do(st(env(stmts)), _p0_, _p1_), AFieldAccess(var obj, var field) => ((Func<TypeEnv, Func<AExpr, CheckResult>> r = (_p0_) => (_p1_) => infer_expr(st(env(obj)), _p0_, _p1_)) is var _ ? ((Func<CodexType, CodexType> resolved = (_p0_) => deep_resolve(r.state(r.inferred_type), _p0_)) is var _ ? resolved switch { RecordTy(var rname, var rfields) => ((Func<string, CodexType> ftype = (_p0_) => lookup_record_field(rfields(field.value), _p0_)) is var _ ? new CheckResult(inferred_type: ftype, state: r.state) : default), _ => ((FreshResult fr = fresh_and_advance(r.state)) is var _ ? new CheckResult(inferred_type: fr.var_type, state: fr.state) : default), ARecordExpr(var name, var fields) => ((Func<TypeEnv, Func<List<AFieldExpr>, Func<long, Func<long, UnificationState>>>> st2 = (_p0_) => (_p1_) => (_p2_) => (_p3_) => infer_record_fields(st(env(fields(0)(list_length(fields)))), _p0_, _p1_, _p2_, _p3_)) is var _ ? ((object ctor_type = ((_p0_) => env_has(env(name.value), _p0_) ? (_p0_) => env_lookup(env(name.value), _p0_) : ErrorTy)) is var _ ? ((CodexType result_type = strip_fun_args(ctor_type)) is var _ ? new CheckResult(inferred_type: result_type, state: st2) : default) : default) : default), AErrorExpr(var msg) => new CheckResult(inferred_type: ErrorTy, state: st),  } : default) : default),  };

    public static UnificationState infer_record_fields(UnificationState st, TypeEnv env, List<AFieldExpr> fields, long i, long len) => ((i == len) ? st : ((object f = list_at(fields(i))) is var _ ? ((Func<TypeEnv, Func<AExpr, CheckResult>> r = (_p0_) => (_p1_) => infer_expr(st(env(f.value)), _p0_, _p1_)) is var _ ? (_p0_) => (_p1_) => (_p2_) => (_p3_) => infer_record_fields(r.state(env(fields((i + 1))(len))), _p0_, _p1_, _p2_, _p3_) : default) : default));

    public static CodexType strip_fun_args(CodexType ty) => ty switch { FunTy(var p, var r) => strip_fun_args(r), _ => ty,  };

    public static CodexType resolve_type_expr(ATypeExpr texpr) => texpr switch { ANamedType(var name) => resolve_type_name(name.value), AFunType(var param, var ret) => new FunTy(resolve_type_expr(param), resolve_type_expr(ret)), AAppType(var ctor, var args) => (_p0_) => resolve_applied_type(ctor(args), _p0_),  };

    public static CodexType resolve_applied_type(ATypeExpr ctor, List<ATypeExpr> args) => ctor switch { ANamedType(var name) => ((name.value == "List") ? ((list_length(args) == 1) ? new ListTy(resolve_type_expr(list_at(args(0)))) : new ListTy(ErrorTy)) : new ConstructedTy(name((_p0_) => (_p1_) => (_p2_) => resolve_type_expr_list(args(0)(list_length(args))(new List<object>()), _p0_, _p1_, _p2_)))), _ => resolve_type_expr(ctor),  };

    public static List<CodexType> resolve_type_expr_list(List<ATypeExpr> args, long i, long len, List<CodexType> acc) => ((i == len) ? acc : (_p0_) => (_p1_) => (_p2_) => resolve_type_expr_list(args((i + 1))(len((acc + new List<object> { resolve_type_expr(list_at(args(i))) }))), _p0_, _p1_, _p2_));

    public static CodexType resolve_type_name(string name) => ((name == "Integer") ? IntegerTy : ((name == "Number") ? NumberTy : ((name == "Text") ? TextTy : ((name == "Boolean") ? BooleanTy : ((name == "Nothing") ? NothingTy : new ConstructedTy(new Name(value: name), new List<CodexType>()))))));

    public static CheckResult check_def(UnificationState st, TypeEnv env, ADef def) => ((Func<TypeEnv, Func<ADef, DefSetup>> declared = (_p0_) => (_p1_) => resolve_declared_type(st(env(def)), _p0_, _p1_)) is var _ ? ((Func<TypeEnv, Func<List<AParam>, Func<CodexType, Func<long, Func<long, DefParamResult>>>>> env2 = (_p0_) => (_p1_) => (_p2_) => (_p3_) => (_p4_) => bind_def_params(declared.state(declared.env(def.@params(declared.expected_type(0)(list_length(def.@params))))), _p0_, _p1_, _p2_, _p3_, _p4_)) is var _ ? ((Func<TypeEnv, Func<AExpr, CheckResult>> body_r = (_p0_) => (_p1_) => infer_expr(env2.state(env2.env(def.body)), _p0_, _p1_)) is var _ ? ((Func<CodexType, Func<CodexType, UnifyResult>> u = (_p0_) => (_p1_) => unify(body_r.state(env2.remaining_type(body_r.inferred_type)), _p0_, _p1_)) is var _ ? new CheckResult(inferred_type: declared.expected_type, state: u.state) : default) : default) : default) : default);

    public static DefSetup resolve_declared_type(UnificationState st, TypeEnv env, ADef def) => ((list_length(def.declared_type) == 0) ? ((FreshResult fr = fresh_and_advance(st)) is var _ ? new DefSetup(expected_type: fr.var_type, remaining_type: fr.var_type, state: fr.state, env: env) : default) : ((CodexType ty = resolve_type_expr(list_at(def.declared_type(0)))) is var _ ? new DefSetup(expected_type: ty, remaining_type: ty, state: st, env: env) : default));

    public static DefParamResult bind_def_params(UnificationState st, TypeEnv env, List<AParam> @params, CodexType remaining, long i, long len) => ((i == len) ? new DefParamResult(state: st, env: env, remaining_type: remaining) : ((object p = list_at(@params(i))) is var _ ? remaining switch { FunTy(var param_ty, var ret_ty) => ((Func<string, Func<CodexType, TypeEnv>> env2 = (_p0_) => (_p1_) => env_bind(env(p.name.value(param_ty)), _p0_, _p1_)) is var _ ? (_p0_) => (_p1_) => (_p2_) => (_p3_) => (_p4_) => bind_def_params(st(env2(@params(ret_ty((i + 1))(len)))), _p0_, _p1_, _p2_, _p3_, _p4_) : default), _ => ((FreshResult fr = fresh_and_advance(st)) is var _ ? ((Func<string, Func<CodexType, TypeEnv>> env2 = (_p0_) => (_p1_) => env_bind(env(p.name.value(fr.var_type)), _p0_, _p1_)) is var _ ? (_p0_) => (_p1_) => (_p2_) => (_p3_) => (_p4_) => bind_def_params(fr.state(env2(@params(remaining((i + 1))(len)))), _p0_, _p1_, _p2_, _p3_, _p4_) : default) : default),  } : default));

    public static ModuleResult check_module(AModule mod) => ((Func<TypeEnv, Func<List<ATypeDef>, Func<long, Func<long, LetBindResult>>>> tenv = (_p0_) => (_p1_) => (_p2_) => (_p3_) => register_type_defs(empty_unification_state(builtin_type_env(mod.type_defs(0)(list_length(mod.type_defs)))), _p0_, _p1_, _p2_, _p3_)) is var _ ? ((Func<TypeEnv, Func<List<ADef>, Func<long, Func<long, LetBindResult>>>> env = (_p0_) => (_p1_) => (_p2_) => (_p3_) => register_all_defs(tenv.state(tenv.env(mod.defs(0)(list_length(mod.defs)))), _p0_, _p1_, _p2_, _p3_)) is var _ ? (_p0_) => (_p1_) => (_p2_) => (_p3_) => (_p4_) => check_all_defs(env.state(env.env(mod.defs(0)(list_length(mod.defs))(new List<object>()))), _p0_, _p1_, _p2_, _p3_, _p4_) : default) : default);

    public static LetBindResult register_all_defs(UnificationState st, TypeEnv env, List<ADef> defs, long i, long len) => ((i == len) ? new LetBindResult(state: st, env: env) : ((object def = list_at(defs(i))) is var _ ? ((object ty = ((list_length(def.declared_type) == 0) ? ((FreshResult fr = fresh_and_advance(st)) is var _ ? ((Func<string, Func<CodexType, TypeEnv>> env2 = (_p0_) => (_p1_) => env_bind(env(def.name.value(fr.var_type)), _p0_, _p1_)) is var _ ? new LetBindResult(state: fr.state, env: env2) : default) : default) : ((CodexType resolved = resolve_type_expr(list_at(def.declared_type(0)))) is var _ ? new LetBindResult(state: st, env: (_p0_) => (_p1_) => env_bind(env(def.name.value(resolved)), _p0_, _p1_)) : default))) is var _ ? (_p0_) => (_p1_) => (_p2_) => (_p3_) => register_all_defs(ty.state(ty.env(defs((i + 1))(len))), _p0_, _p1_, _p2_, _p3_) : default) : default));

    public static ModuleResult check_all_defs(UnificationState st, TypeEnv env, List<ADef> defs, long i, long len, List<TypeBinding> acc) => ((i == len) ? new ModuleResult(types: acc, state: st) : ((object def = list_at(defs(i))) is var _ ? ((Func<TypeEnv, Func<ADef, CheckResult>> r = (_p0_) => (_p1_) => check_def(st(env(def)), _p0_, _p1_)) is var _ ? ((Func<CodexType, CodexType> resolved = (_p0_) => deep_resolve(r.state(r.inferred_type), _p0_)) is var _ ? ((object entry = new TypeBinding(name: def.name.value, bound_type: resolved)) is var _ ? (_p0_) => (_p1_) => (_p2_) => (_p3_) => (_p4_) => check_all_defs(r.state(env(defs((i + 1))(len((acc + new List<object> { entry }))))), _p0_, _p1_, _p2_, _p3_, _p4_) : default) : default) : default) : default));

    public static LetBindResult register_type_defs(UnificationState st, TypeEnv env, List<ATypeDef> tdefs, long i, long len) => ((i == len) ? new LetBindResult(state: st, env: env) : ((object td = list_at(tdefs(i))) is var _ ? ((Func<TypeEnv, Func<ATypeDef, LetBindResult>> r = (_p0_) => (_p1_) => register_one_type_def(st(env(td)), _p0_, _p1_)) is var _ ? (_p0_) => (_p1_) => (_p2_) => (_p3_) => register_type_defs(r.state(r.env(tdefs((i + 1))(len))), _p0_, _p1_, _p2_, _p3_) : default) : default));

    public static LetBindResult register_one_type_def(UnificationState st, TypeEnv env, ATypeDef td) => td switch { AVariantTypeDef(var name, var type_params, var ctors) => ((Func<List<CodexType>, CodexType> result_ty = new ConstructedTy(name(new List<object>()))) is var _ ? (_p0_) => (_p1_) => (_p2_) => (_p3_) => (_p4_) => register_variant_ctors(st(env(ctors(result_ty(0)(list_length(ctors))))), _p0_, _p1_, _p2_, _p3_, _p4_) : default), ARecordTypeDef(var name, var type_params, var fields) => ((Func<long, Func<long, Func<List<RecordField>, List<RecordField>>>> resolved_fields = (_p0_) => (_p1_) => (_p2_) => build_record_fields(fields(0)(list_length(fields))(new List<object>()), _p0_, _p1_, _p2_)) is var _ ? ((Func<List<RecordField>, CodexType> result_ty = new RecordTy(name(resolved_fields))) is var _ ? ((Func<CodexType, Func<long, Func<long, CodexType>>> ctor_ty = (_p0_) => (_p1_) => (_p2_) => build_record_ctor_type(fields(result_ty(0)(list_length(fields))), _p0_, _p1_, _p2_)) is var _ ? new LetBindResult(state: st, env: (_p0_) => (_p1_) => env_bind(env(name.value(ctor_ty)), _p0_, _p1_)) : default) : default) : default),  };

    public static List<RecordField> build_record_fields(List<ARecordFieldDef> fields, long i, long len, List<RecordField> acc) => ((i == len) ? acc : ((object f = list_at(fields(i))) is var _ ? ((object rfield = new RecordField(name: f.name, type_val: resolve_type_expr(f.type_expr))) is var _ ? (_p0_) => (_p1_) => (_p2_) => build_record_fields(fields((i + 1))(len((acc + new List<object> { rfield }))), _p0_, _p1_, _p2_) : default) : default));

    public static CodexType lookup_record_field(List<RecordField> fields, string name) => ((list_length(fields) == 0) ? ErrorTy : ((object f = list_at(fields(0))) is var _ ? ((f.name.value == name) ? f.type_val : (_p0_) => (_p1_) => (_p2_) => lookup_record_field_loop(fields(name(1)(list_length(fields))), _p0_, _p1_, _p2_)) : default));

    public static CodexType lookup_record_field_loop(List<RecordField> fields, string name, long i, long len) => ((i == len) ? ErrorTy : ((object f = list_at(fields(i))) is var _ ? ((f.name.value == name) ? f.type_val : (_p0_) => (_p1_) => (_p2_) => lookup_record_field_loop(fields(name((i + 1))(len)), _p0_, _p1_, _p2_)) : default));

    public static LetBindResult register_variant_ctors(UnificationState st, TypeEnv env, List<AVariantCtorDef> ctors, CodexType result_ty, long i, long len) => ((i == len) ? new LetBindResult(state: st, env: env) : ((object ctor = list_at(ctors(i))) is var _ ? ((Func<CodexType, Func<long, Func<long, CodexType>>> ctor_ty = (_p0_) => (_p1_) => (_p2_) => build_ctor_type(ctor.fields(result_ty(0)(list_length(ctor.fields))), _p0_, _p1_, _p2_)) is var _ ? ((Func<string, Func<CodexType, TypeEnv>> env2 = (_p0_) => (_p1_) => env_bind(env(ctor.name.value(ctor_ty)), _p0_, _p1_)) is var _ ? (_p0_) => (_p1_) => (_p2_) => (_p3_) => (_p4_) => register_variant_ctors(st(env2(ctors(result_ty((i + 1))(len)))), _p0_, _p1_, _p2_, _p3_, _p4_) : default) : default) : default));

    public static CodexType build_ctor_type(List<ATypeExpr> fields, CodexType result, long i, long len) => ((i == len) ? result : ((Func<CodexType, Func<long, Func<long, CodexType>>> rest = (_p0_) => (_p1_) => (_p2_) => build_ctor_type(fields(result((i + 1))(len)), _p0_, _p1_, _p2_)) is var _ ? new FunTy(resolve_type_expr(list_at(fields(i))), rest) : default));

    public static CodexType build_record_ctor_type(List<ARecordFieldDef> fields, CodexType result, long i, long len) => ((i == len) ? result : ((object f = list_at(fields(i))) is var _ ? ((Func<CodexType, Func<long, Func<long, CodexType>>> rest = (_p0_) => (_p1_) => (_p2_) => build_record_ctor_type(fields(result((i + 1))(len)), _p0_, _p1_, _p2_)) is var _ ? new FunTy(resolve_type_expr(f.type_expr), rest) : default) : default));

    public static TypeEnv empty_type_env() => new TypeEnv(bindings: new List<object>());

    public static CodexType env_lookup(TypeEnv env, string name) => (_p0_) => (_p1_) => (_p2_) => env_lookup_loop(env.bindings(name(0)(list_length(env.bindings))), _p0_, _p1_, _p2_);

    public static CodexType env_lookup_loop(List<TypeBinding> bindings, string name, long i, long len) => ((i == len) ? ErrorTy : ((object b = list_at(bindings(i))) is var _ ? ((b.name == name) ? b.bound_type : (_p0_) => (_p1_) => (_p2_) => env_lookup_loop(bindings(name((i + 1))(len)), _p0_, _p1_, _p2_)) : default));

    public static bool env_has(TypeEnv env, string name) => (_p0_) => (_p1_) => (_p2_) => env_has_loop(env.bindings(name(0)(list_length(env.bindings))), _p0_, _p1_, _p2_);

    public static bool env_has_loop(List<TypeBinding> bindings, string name, long i, long len) => ((i == len) ? false : ((object b = list_at(bindings(i))) is var _ ? ((b.name == name) ? true : (_p0_) => (_p1_) => (_p2_) => env_has_loop(bindings(name((i + 1))(len)), _p0_, _p1_, _p2_)) : default));

    public static TypeEnv env_bind(TypeEnv env, string name, CodexType ty) => new TypeEnv(bindings: (new List<object> { new TypeBinding(name: name, bound_type: ty) } + env.bindings));

    public static TypeEnv builtin_type_env() => ((TypeEnv e = empty_type_env) is var _ ? ((Func<string, Func<CodexType, TypeEnv>> e2 = (_p0_) => (_p1_) => env_bind(e("negate")(new FunTy(IntegerTy, IntegerTy)), _p0_, _p1_)) is var _ ? ((Func<string, Func<CodexType, TypeEnv>> e3 = (_p0_) => (_p1_) => env_bind(e2("text-length")(new FunTy(TextTy, IntegerTy)), _p0_, _p1_)) is var _ ? ((Func<string, Func<CodexType, TypeEnv>> e4 = (_p0_) => (_p1_) => env_bind(e3("integer-to-text")(new FunTy(IntegerTy, TextTy)), _p0_, _p1_)) is var _ ? ((Func<string, Func<CodexType, TypeEnv>> e5 = (_p0_) => (_p1_) => env_bind(e4("char-at")(new FunTy(TextTy, new FunTy(IntegerTy, TextTy))), _p0_, _p1_)) is var _ ? ((Func<string, Func<CodexType, TypeEnv>> e6 = (_p0_) => (_p1_) => env_bind(e5("substring")(new FunTy(TextTy, new FunTy(IntegerTy, new FunTy(IntegerTy, TextTy)))), _p0_, _p1_)) is var _ ? ((Func<string, Func<CodexType, TypeEnv>> e7 = (_p0_) => (_p1_) => env_bind(e6("is-letter")(new FunTy(TextTy, BooleanTy)), _p0_, _p1_)) is var _ ? ((Func<string, Func<CodexType, TypeEnv>> e8 = (_p0_) => (_p1_) => env_bind(e7("is-digit")(new FunTy(TextTy, BooleanTy)), _p0_, _p1_)) is var _ ? ((Func<string, Func<CodexType, TypeEnv>> e9 = (_p0_) => (_p1_) => env_bind(e8("is-whitespace")(new FunTy(TextTy, BooleanTy)), _p0_, _p1_)) is var _ ? ((Func<string, Func<CodexType, TypeEnv>> e10 = (_p0_) => (_p1_) => env_bind(e9("char-code")(new FunTy(TextTy, IntegerTy)), _p0_, _p1_)) is var _ ? ((Func<string, Func<CodexType, TypeEnv>> e11 = (_p0_) => (_p1_) => env_bind(e10("code-to-char")(new FunTy(IntegerTy, TextTy)), _p0_, _p1_)) is var _ ? ((Func<string, Func<CodexType, TypeEnv>> e12 = (_p0_) => (_p1_) => env_bind(e11("text-replace")(new FunTy(TextTy, new FunTy(TextTy, new FunTy(TextTy, TextTy)))), _p0_, _p1_)) is var _ ? ((Func<string, Func<CodexType, TypeEnv>> e13 = (_p0_) => (_p1_) => env_bind(e12("text-to-integer")(new FunTy(TextTy, IntegerTy)), _p0_, _p1_)) is var _ ? ((Func<string, Func<CodexType, TypeEnv>> e14 = (_p0_) => (_p1_) => env_bind(e13("show")(new ForAllTy(0, new FunTy(new TypeVar(0), TextTy))), _p0_, _p1_)) is var _ ? ((Func<string, Func<CodexType, TypeEnv>> e15 = (_p0_) => (_p1_) => env_bind(e14("print-line")(new FunTy(TextTy, NothingTy)), _p0_, _p1_)) is var _ ? ((Func<string, Func<CodexType, TypeEnv>> e16 = (_p0_) => (_p1_) => env_bind(e15("list-length")(new ForAllTy(0, new FunTy(new ListTy(new TypeVar(0)), IntegerTy))), _p0_, _p1_)) is var _ ? ((Func<string, Func<CodexType, TypeEnv>> e17 = (_p0_) => (_p1_) => env_bind(e16("list-at")(new ForAllTy(0, new FunTy(new ListTy(new TypeVar(0)), new FunTy(IntegerTy, new TypeVar(0))))), _p0_, _p1_)) is var _ ? ((Func<string, Func<CodexType, TypeEnv>> e18 = (_p0_) => (_p1_) => env_bind(e17("map")(new ForAllTy(0, new ForAllTy(1, new FunTy(new FunTy(new TypeVar(0), new TypeVar(1)), new FunTy(new ListTy(new TypeVar(0)), new ListTy(new TypeVar(1))))))), _p0_, _p1_)) is var _ ? ((Func<string, Func<CodexType, TypeEnv>> e19 = (_p0_) => (_p1_) => env_bind(e18("filter")(new ForAllTy(0, new FunTy(new FunTy(new TypeVar(0), BooleanTy), new FunTy(new ListTy(new TypeVar(0)), new ListTy(new TypeVar(0)))))), _p0_, _p1_)) is var _ ? ((Func<string, Func<CodexType, TypeEnv>> e20 = (_p0_) => (_p1_) => env_bind(e19("fold")(new ForAllTy(0, new ForAllTy(1, new FunTy(new FunTy(new TypeVar(1), new FunTy(new TypeVar(0), new TypeVar(1))), new FunTy(new TypeVar(1), new FunTy(new ListTy(new TypeVar(0)), new TypeVar(1))))))), _p0_, _p1_)) is var _ ? ((Func<string, Func<CodexType, TypeEnv>> e21 = (_p0_) => (_p1_) => env_bind(e20("read-line")(TextTy), _p0_, _p1_)) is var _ ? e21 : default) : default) : default) : default) : default) : default) : default) : default) : default) : default) : default) : default) : default) : default) : default) : default) : default) : default) : default) : default) : default);

    public static UnificationState empty_unification_state() => new UnificationState(substitutions: new List<object>(), next_id: 0, errors: new List<object>());

    public static CodexType fresh_var(UnificationState st) => new TypeVar(st.next_id);

    public static UnificationState advance_id(UnificationState st) => new UnificationState(substitutions: st.substitutions, next_id: (st.next_id + 1), errors: st.errors);

    public static FreshResult fresh_and_advance(UnificationState st) => new FreshResult(var_type: new TypeVar(st.next_id), state: advance_id(st));

    public static CodexType subst_lookup(long var_id, List<SubstEntry> entries) => (_p0_) => (_p1_) => (_p2_) => subst_lookup_loop(var_id(entries(0)(list_length(entries))), _p0_, _p1_, _p2_);

    public static CodexType subst_lookup_loop(long var_id, List<SubstEntry> entries, long i, long len) => ((i == len) ? ErrorTy : ((object entry = list_at(entries(i))) is var _ ? ((entry.var_id == var_id) ? entry.resolved_type : (_p0_) => (_p1_) => (_p2_) => subst_lookup_loop(var_id(entries((i + 1))(len)), _p0_, _p1_, _p2_)) : default));

    public static bool has_subst(long var_id, List<SubstEntry> entries) => (_p0_) => (_p1_) => (_p2_) => has_subst_loop(var_id(entries(0)(list_length(entries))), _p0_, _p1_, _p2_);

    public static bool has_subst_loop(long var_id, List<SubstEntry> entries, long i, long len) => ((i == len) ? false : ((object entry = list_at(entries(i))) is var _ ? ((entry.var_id == var_id) ? true : (_p0_) => (_p1_) => (_p2_) => has_subst_loop(var_id(entries((i + 1))(len)), _p0_, _p1_, _p2_)) : default));

    public static CodexType resolve(UnificationState st, CodexType ty) => ty switch { TypeVar(var id) => ((_p0_) => has_subst(id(st.substitutions), _p0_) ? (_p0_) => resolve(st((_p0_) => subst_lookup(id(st.substitutions), _p0_)), _p0_) : ty), _ => ty,  };

    public static UnificationState add_subst(UnificationState st, long var_id, CodexType ty) => new UnificationState(substitutions: (st.substitutions + new List<object> { new SubstEntry(var_id: var_id, resolved_type: ty) }), next_id: st.next_id, errors: st.errors);

    public static UnificationState add_unify_error(UnificationState st, string code, string msg) => new UnificationState(substitutions: st.substitutions, next_id: st.next_id, errors: (st.errors + new List<object> { (_p0_) => make_error(code(msg), _p0_) }));

    public static bool occurs_in(UnificationState st, long var_id, CodexType ty) => ((Func<CodexType, CodexType> resolved = (_p0_) => resolve(st(ty), _p0_)) is var _ ? resolved switch { TypeVar(var id) => (id == var_id), FunTy(var param, var ret) => ((_p0_) => (_p1_) => occurs_in(st(var_id(param)), _p0_, _p1_) || (_p0_) => (_p1_) => occurs_in(st(var_id(ret)), _p0_, _p1_)), ListTy(var elem) => (_p0_) => (_p1_) => occurs_in(st(var_id(elem)), _p0_, _p1_), _ => false,  } : default);

    public static UnifyResult unify(UnificationState st, CodexType a, CodexType b) => ((Func<CodexType, CodexType> ra = (_p0_) => resolve(st(a), _p0_)) is var _ ? ((Func<CodexType, CodexType> rb = (_p0_) => resolve(st(b), _p0_)) is var _ ? (_p0_) => (_p1_) => unify_resolved(st(ra(rb)), _p0_, _p1_) : default) : default);

    public static UnifyResult unify_resolved(UnificationState st, CodexType a, CodexType b) => a switch { TypeVar(var id_a) => ((_p0_) => (_p1_) => occurs_in(st(id_a(b)), _p0_, _p1_) ? new UnifyResult(success: false, state: (_p0_) => (_p1_) => add_unify_error(st("CDX2010")("Infinite type"), _p0_, _p1_)) : new UnifyResult(success: true, state: (_p0_) => (_p1_) => add_subst(st(id_a(b)), _p0_, _p1_))), _ => (_p0_) => (_p1_) => unify_rhs(st(a(b)), _p0_, _p1_),  };

    public static UnifyResult unify_rhs(UnificationState st, CodexType a, CodexType b) => b switch { TypeVar(var id_b) => ((_p0_) => (_p1_) => occurs_in(st(id_b(a)), _p0_, _p1_) ? new UnifyResult(success: false, state: (_p0_) => (_p1_) => add_unify_error(st("CDX2010")("Infinite type"), _p0_, _p1_)) : new UnifyResult(success: true, state: (_p0_) => (_p1_) => add_subst(st(id_b(a)), _p0_, _p1_))), _ => (_p0_) => (_p1_) => unify_structural(st(a(b)), _p0_, _p1_),  };

    public static UnifyResult unify_structural(UnificationState st, CodexType a, CodexType b) => a switch { IntegerTy { } => b switch { IntegerTy { } => new UnifyResult(success: true, state: st), ErrorTy { } => new UnifyResult(success: true, state: st), _ => (_p0_) => (_p1_) => unify_mismatch(st(a(b)), _p0_, _p1_), NumberTy { } => b switch { NumberTy { } => new UnifyResult(success: true, state: st), ErrorTy { } => new UnifyResult(success: true, state: st), _ => (_p0_) => (_p1_) => unify_mismatch(st(a(b)), _p0_, _p1_), TextTy { } => b switch { TextTy { } => new UnifyResult(success: true, state: st), ErrorTy { } => new UnifyResult(success: true, state: st), _ => (_p0_) => (_p1_) => unify_mismatch(st(a(b)), _p0_, _p1_), BooleanTy { } => b switch { BooleanTy { } => new UnifyResult(success: true, state: st), ErrorTy { } => new UnifyResult(success: true, state: st), _ => (_p0_) => (_p1_) => unify_mismatch(st(a(b)), _p0_, _p1_), NothingTy { } => b switch { NothingTy { } => new UnifyResult(success: true, state: st), ErrorTy { } => new UnifyResult(success: true, state: st), _ => (_p0_) => (_p1_) => unify_mismatch(st(a(b)), _p0_, _p1_), VoidTy { } => b switch { VoidTy { } => new UnifyResult(success: true, state: st), ErrorTy { } => new UnifyResult(success: true, state: st), _ => (_p0_) => (_p1_) => unify_mismatch(st(a(b)), _p0_, _p1_), ErrorTy { } => new UnifyResult(success: true, state: st), FunTy(var pa, var ra) => b switch { FunTy(var pb, var rb) => (_p0_) => (_p1_) => (_p2_) => (_p3_) => unify_fun(st(pa(ra(pb(rb)))), _p0_, _p1_, _p2_, _p3_), ErrorTy { } => new UnifyResult(success: true, state: st), _ => (_p0_) => (_p1_) => unify_mismatch(st(a(b)), _p0_, _p1_), ListTy(var ea) => b switch { ListTy(var eb) => (_p0_) => (_p1_) => unify(st(ea(eb)), _p0_, _p1_), ErrorTy { } => new UnifyResult(success: true, state: st), _ => (_p0_) => (_p1_) => unify_mismatch(st(a(b)), _p0_, _p1_), ConstructedTy(var na, var args_a) => b switch { ConstructedTy(var nb, var args_b) => ((na.value == nb.value) ? (_p0_) => (_p1_) => (_p2_) => (_p3_) => unify_constructed_args(st(args_a(args_b(0)(list_length(args_a)))), _p0_, _p1_, _p2_, _p3_) : (_p0_) => (_p1_) => unify_mismatch(st(a(b)), _p0_, _p1_)), SumTy(var sb_name, var sb_ctors) => ((na.value == sb_name.value) ? new UnifyResult(success: true, state: st) : (_p0_) => (_p1_) => unify_mismatch(st(a(b)), _p0_, _p1_)), RecordTy(var rb_name, var rb_fields) => ((na.value == rb_name.value) ? new UnifyResult(success: true, state: st) : (_p0_) => (_p1_) => unify_mismatch(st(a(b)), _p0_, _p1_)), ErrorTy { } => new UnifyResult(success: true, state: st), _ => (_p0_) => (_p1_) => unify_mismatch(st(a(b)), _p0_, _p1_), SumTy(var sa_name, var sa_ctors) => b switch { SumTy(var sb_name, var sb_ctors) => ((sa_name.value == sb_name.value) ? new UnifyResult(success: true, state: st) : (_p0_) => (_p1_) => unify_mismatch(st(a(b)), _p0_, _p1_)), ConstructedTy(var nb, var args_b) => ((sa_name.value == nb.value) ? new UnifyResult(success: true, state: st) : (_p0_) => (_p1_) => unify_mismatch(st(a(b)), _p0_, _p1_)), ErrorTy { } => new UnifyResult(success: true, state: st), _ => (_p0_) => (_p1_) => unify_mismatch(st(a(b)), _p0_, _p1_), RecordTy(var ra_name, var ra_fields) => b switch { RecordTy(var rb_name, var rb_fields) => ((ra_name.value == rb_name.value) ? new UnifyResult(success: true, state: st) : (_p0_) => (_p1_) => unify_mismatch(st(a(b)), _p0_, _p1_)), ConstructedTy(var nb, var args_b) => ((ra_name.value == nb.value) ? new UnifyResult(success: true, state: st) : (_p0_) => (_p1_) => unify_mismatch(st(a(b)), _p0_, _p1_)), ErrorTy { } => new UnifyResult(success: true, state: st), _ => (_p0_) => (_p1_) => unify_mismatch(st(a(b)), _p0_, _p1_), ForAllTy(var id, var body) => (_p0_) => (_p1_) => unify(st(body(b)), _p0_, _p1_), _ => b switch { ErrorTy { } => new UnifyResult(success: true, state: st), _ => (_p0_) => (_p1_) => unify_mismatch(st(a(b)), _p0_, _p1_),  },  },  },  },  },  },  },  },  },  },  },  },  };

    public static UnifyResult unify_constructed_args(UnificationState st, List<CodexType> args_a, List<CodexType> args_b, long i, long len) => ((i == len) ? new UnifyResult(success: true, state: st) : ((i >= list_length(args_b)) ? new UnifyResult(success: true, state: st) : ((Func<CodexType, Func<CodexType, UnifyResult>> r = (_p0_) => (_p1_) => unify(st(list_at(args_a(i)))(list_at(args_b(i))), _p0_, _p1_)) is var _ ? (r.success ? (_p0_) => (_p1_) => (_p2_) => (_p3_) => unify_constructed_args(r.state(args_a(args_b((i + 1))(len))), _p0_, _p1_, _p2_, _p3_) : r) : default)));

    public static UnifyResult unify_fun(UnificationState st, CodexType pa, CodexType ra, CodexType pb, CodexType rb) => ((Func<CodexType, Func<CodexType, UnifyResult>> r1 = (_p0_) => (_p1_) => unify(st(pa(pb)), _p0_, _p1_)) is var _ ? (r1.success ? (_p0_) => (_p1_) => unify(r1.state(ra(rb)), _p0_, _p1_) : r1) : default);

    public static UnifyResult unify_mismatch(UnificationState st, CodexType a, CodexType b) => new UnifyResult(success: false, state: (_p0_) => (_p1_) => add_unify_error(st("CDX2001")("Type mismatch"), _p0_, _p1_));

    public static CodexType deep_resolve(UnificationState st, CodexType ty) => ((Func<CodexType, CodexType> resolved = (_p0_) => resolve(st(ty), _p0_)) is var _ ? resolved switch { FunTy(var param, var ret) => new FunTy((_p0_) => deep_resolve(st(param), _p0_), (_p0_) => deep_resolve(st(ret), _p0_)), ListTy(var elem) => new ListTy((_p0_) => deep_resolve(st(elem), _p0_)), ConstructedTy(var name, var args) => new ConstructedTy(name((_p0_) => (_p1_) => (_p2_) => (_p3_) => deep_resolve_list(st(args(0)(list_length(args))(new List<object>())), _p0_, _p1_, _p2_, _p3_))), ForAllTy(var id, var body) => new ForAllTy(id((_p0_) => deep_resolve(st(body), _p0_))), SumTy(var name, var ctors) => resolved, RecordTy(var name, var fields) => resolved, _ => resolved,  } : default);

    public static List<CodexType> deep_resolve_list(UnificationState st, List<CodexType> args, long i, long len, List<CodexType> acc) => ((i == len) ? acc : (_p0_) => (_p1_) => (_p2_) => (_p3_) => deep_resolve_list(st(args((i + 1))(len((acc + new List<object> { (_p0_) => deep_resolve(st(list_at(args(i))), _p0_) })))), _p0_, _p1_, _p2_, _p3_));

    public static string compile(string source, string module_name) => ((List<Token> tokens = tokenize(source)) is var _ ? ((ParseState st = make_parse_state(tokens)) is var _ ? ((Document doc = parse_document(st)) is var _ ? ((Func<string, AModule> ast = (_p0_) => desugar_document(doc(module_name), _p0_)) is var _ ? ((ModuleResult check_result = check_module(ast)) is var _ ? ((Func<List<TypeBinding>, Func<UnificationState, IRModule>> ir = (_p0_) => (_p1_) => lower_module(ast(check_result.types(check_result.state)), _p0_, _p1_)) is var _ ? (_p0_) => emit_full_module(ir(ast.type_defs), _p0_) : default) : default) : default) : default) : default) : default);

    public static CompileResult compile_checked(string source, string module_name) => ((List<Token> tokens = tokenize(source)) is var _ ? ((ParseState st = make_parse_state(tokens)) is var _ ? ((Document doc = parse_document(st)) is var _ ? ((Func<string, AModule> ast = (_p0_) => desugar_document(doc(module_name), _p0_)) is var _ ? ((ResolveResult resolve_result = resolve_module(ast)) is var _ ? ((list_length(resolve_result.errors) > 0) ? new CompileError(resolve_result.errors) : ((ModuleResult check_result = check_module(ast)) is var _ ? ((Func<List<TypeBinding>, Func<UnificationState, IRModule>> ir = (_p0_) => (_p1_) => lower_module(ast(check_result.types(check_result.state)), _p0_, _p1_)) is var _ ? new CompileOk((_p0_) => emit_full_module(ir(ast.type_defs), _p0_), check_result) : default) : default)) : default) : default) : default) : default) : default);

    public static string test_source() => "square : Integer -> Integer\\nsquare (x) = x * x\\nmain = square 5";

    public static [ Console() => Nothing;

    public static T4223 main<T4223>() => { print_line((_p0_) => compile(test_source("test"), _p0_));  };

}
