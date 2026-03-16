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

public sealed record ADef(Name name, List<AParam> params, List<ATypeExpr> declared_type, AExpr body);

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

public sealed record IRDef(string name, List<IRParam> params, CodexType type_val, IRExpr body);

public sealed record IRModule(Name name, List<IRDef> defs);

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

public sealed record Def(Token name, List<Token> params, List<TypeAnn> ann, Expr body);

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
    public static T333 desugar_expr<T333>(object node) => node switch { LitExpr(var tok) => desugar_literal(tok), NameExpr(var tok) => ANameExpr(make_name(tok.text)), AppExpr(var f, var a) => AApplyExpr(desugar_expr(f))(desugar_expr(a)), BinExpr(var l, var op, var r) => ABinaryExpr(desugar_expr(l))(desugar_bin_op(op.kind))(desugar_expr(r)), UnaryExpr(var op, var operand) => AUnaryExpr(desugar_expr(operand)), IfExpr(var c, var t, var e) => AIfExpr(desugar_expr(c))(desugar_expr(t))(desugar_expr(e)), LetExpr(var bindings, var body) => ALetExpr(map_list(desugar_let_bind)(bindings))(desugar_expr(body)), MatchExpr(var scrut, var arms) => AMatchExpr(desugar_expr(scrut))(map_list(desugar_match_arm)(arms)), ListExpr(var elems) => AListExpr(map_list(desugar_expr)(elems)), RecordExpr(var type_tok, var fields) => ARecordExpr(make_name(type_tok.text))(map_list(desugar_field_expr)(fields)), FieldExpr(var rec, var field_tok) => AFieldAccess(desugar_expr(rec))(make_name(field_tok.text)), ParenExpr(var inner) => desugar_expr(inner), DoExpr(var stmts) => ADoExpr(map_list(desugar_do_stmt)(stmts)), ErrExpr(var tok) => AErrorExpr(tok.text),  };

    public static T388 desugar_literal<T388>(object tok) => (is_literal(tok.kind) ? ALitExpr(tok.text)(classify_literal(tok.kind)) : AErrorExpr(tok.text));

    public static object classify_literal(object k) => k switch { IntegerLiteral { } => IntLit, NumberLiteral { } => NumLit, TextLiteral { } => TextLit, TrueKeyword { } => BoolLit, FalseKeyword { } => BoolLit, _ => TextLit,  };

    public static object desugar_let_bind(object b) => new ALetBind(name: make_name(b.name.text), value: desugar_expr(b.value));

    public static object desugar_match_arm(object arm) => new AMatchArm(pattern: desugar_pattern(arm.pattern), body: desugar_expr(arm.body));

    public static object desugar_field_expr(object f) => new AFieldExpr(name: make_name(f.name.text), value: desugar_expr(f.value));

    public static T400 desugar_do_stmt<T400>(object s) => s switch { DoBindStmt(var tok, var val) => ADoBindStmt(make_name(tok.text))(desugar_expr(val)), DoExprStmt(var e) => ADoExprStmt(desugar_expr(e)),  };

    public static object desugar_bin_op(object k) => k switch { Plus { } => OpAdd, Minus { } => OpSub, Star { } => OpMul, Slash { } => OpDiv, Caret { } => OpPow, DoubleEquals { } => OpEq, NotEquals { } => OpNotEq, LessThan { } => OpLt, GreaterThan { } => OpGt, LessOrEqual { } => OpLtEq, GreaterOrEqual { } => OpGtEq, TripleEquals { } => OpDefEq, PlusPlus { } => OpAppend, ColonColon { } => OpCons, Ampersand { } => OpAnd, Pipe { } => OpOr, _ => OpAdd,  };

    public static object desugar_pattern(object p) => p switch { VarPat(var tok) => AVarPat(make_name(tok.text)), LitPat(var tok) => ALitPat(tok.text)(classify_literal(tok.kind)), CtorPat(var tok, var subs) => ACtorPat(make_name(tok.text))(map_list(desugar_pattern)(subs)), WildPat(var tok) => AWildPat,  };

    public static T425 desugar_type_expr<T425>(object t) => t switch { NamedType(var tok) => ANamedType(make_name(tok.text)), FunType(var param, var ret) => AFunType(desugar_type_expr(param))(desugar_type_expr(ret)), AppType(var ctor, var args) => AAppType(desugar_type_expr(ctor))(map_list(desugar_type_expr)(args)), ParenType(var inner) => desugar_type_expr(inner), ListType(var elem) => AAppType(ANamedType(make_name("List")))(new List<object> { desugar_type_expr(elem) }), LinearTypeExpr(var inner) => desugar_type_expr(inner),  };

    public static object desugar_def(object d) => new ADef(name: make_name(d.name.text), params: map_list(desugar_param)(d.params), declared_type: new List<object>(), body: desugar_expr(d.body));

    public static object desugar_param(object tok) => new AParam(name: make_name(tok.text));

    public static T450 desugar_type_def<T450>(object td) => td.body switch { RecordBody(var fields) => ARecordTypeDef(make_name(td.name.text))(map_list(make_type_param_name)(td.type_params))(map_list(desugar_record_field_def)(fields)), VariantBody(var ctors) => AVariantTypeDef(make_name(td.name.text))(map_list(make_type_param_name)(td.type_params))(map_list(desugar_variant_ctor_def)(ctors)),  };

    public static T469 make_type_param_name<T469>(object tok) => make_name(tok.text);

    public static object desugar_record_field_def(object f) => new ARecordFieldDef(name: make_name(f.name.text), type_expr: desugar_type_expr(f.type_expr));

    public static object desugar_variant_ctor_def(object c) => new AVariantCtorDef(name: make_name(c.name.text), fields: map_list(desugar_type_expr)(c.fields));

    public static object desugar_document(object doc, object module_name) => new AModule(name: make_name(module_name), defs: map_list(desugar_def)(doc.defs), type_defs: map_list(desugar_type_def)(doc.type_defs));

    public static T486 map_list<T486>(object f, object xs) => map_list_loop(f)(xs)(0)(list_length(xs))(new List<object>());

    public static List<T499> map_list_loop<T499>(object f, object xs, object i, object len, object acc) => ((i == len) ? acc : map_list_loop(f)(xs)((i + 1))(len)((acc + new List<object> { f(list_at(xs)(i)) })));

    public static T510 fold_list<T510>(object f, object z, object xs) => fold_list_loop(f)(z)(xs)(0)(list_length(xs));

    public static T525 fold_list_loop<T525>(object f, object z, object xs, object i, object len) => ((i == len) ? z : fold_list_loop(f)(f(z)(list_at(xs)(i)))(xs)((i + 1))(len));

    public static object make_error(object code, object msg) => new Diagnostic(code: code, message: msg, severity: Error);

    public static object make_warning(object code, object msg) => new Diagnostic(code: code, message: msg, severity: Warning);

    public static string severity_label(object s) => s switch { Error { } => "error", Warning { } => "warning", Info { } => "info",  };

    public static string diagnostic_display(object d) => ((((severity_label(d.severity) + " ") + d.code) + ": ") + d.message);

    public static object make_name(object s) => new Name(value: s);

    public static object name_value(object n) => n.value;

    public static object make_position(object line, object col, object offset) => new SourcePosition(line: line, column: col, offset: offset);

    public static object make_span(object s, object e, object f) => new SourceSpan(start: s, end: e, file: f);

    public static object span_length(object span) => (span.end.offset - span.start.offset);

    public static string sanitize(object name) => text_replace(name)("-")("_");

    public static string cs_type(object ty) => ty switch { IntegerTy { } => "long", NumberTy { } => "decimal", TextTy { } => "string", BooleanTy { } => "bool", VoidTy { } => "void", NothingTy { } => "object", ErrorTy { } => "object", FunTy(var p, var r) => (((("Func<" + cs_type(p)) + ", ") + cs_type(r)) + ">"), ListTy(var elem) => (("List<" + cs_type(elem)) + ">"), TypeVar(var id) => ("T" + integer_to_text(id)), ForAllTy(var id, var body) => cs_type(body), SumTy(var name, var ctors) => sanitize(name.value), RecordTy(var name, var fields) => sanitize(name.value), ConstructedTy(var name, var args) => sanitize(name.value),  };

    public static string emit_expr(object e) => e switch { IrIntLit(var n) => integer_to_text(n), IrNumLit(var n) => integer_to_text(n), IrTextLit(var s) => (("\\\"" + escape_text(s)) + "\\\""), IrBoolLit(var b) => (b ? "true" : "false"), IrName(var n, var ty) => sanitize(n), IrBinary(var op, var l, var r, var ty) => (((((("(" + emit_expr(l)) + " ") + emit_bin_op(op)) + " ") + emit_expr(r)) + ")"), IrNegate(var operand) => (("(-" + emit_expr(operand)) + ")"), IrIf(var c, var t, var el, var ty) => (((((("(" + emit_expr(c)) + " ? ") + emit_expr(t)) + " : ") + emit_expr(el)) + ")"), IrLet(var name, var ty, var val, var body) => emit_let(name)(ty)(val)(body), IrApply(var f, var a, var ty) => (((emit_expr(f) + "(") + emit_expr(a)) + ")"), IrLambda(var params, var body, var ty) => emit_lambda(params)(body), IrList(var elems, var ty) => emit_list(elems)(ty), IrMatch(var scrut, var branches, var ty) => emit_match(scrut)(branches)(ty), IrDo(var stmts, var ty) => emit_do(stmts), IrRecord(var name, var fields, var ty) => emit_record(name)(fields), IrFieldAccess(var rec, var field, var ty) => ((emit_expr(rec) + ".") + sanitize(field)), IrError(var msg, var ty) => (("/* error: " + msg) + " */ default"),  };

    public static string escape_text(object s) => text_replace(text_replace(s)("\\\\")("\\\\\\\\"))("\\\"")("\\\\\\\"");

    public static string emit_bin_op(object op) => op switch { IrAddInt { } => "+", IrSubInt { } => "-", IrMulInt { } => "*", IrDivInt { } => "/", IrPowInt { } => "^", IrAddNum { } => "+", IrSubNum { } => "-", IrMulNum { } => "*", IrDivNum { } => "/", IrEq { } => "==", IrNotEq { } => "!=", IrLt { } => "<", IrGt { } => ">", IrLtEq { } => "<=", IrGtEq { } => ">=", IrAnd { } => "&&", IrOr { } => "||", IrAppendText { } => "+", IrAppendList { } => "+", IrConsList { } => "+",  };

    public static string emit_let(object name, object ty, object val, object body) => (((((((("((" + cs_type(ty)) + " ") + sanitize(name)) + " = ") + emit_expr(val)) + ") is var _ ? ") + emit_expr(body)) + " : default)");

    public static string emit_lambda(object params, object body) => ((list_length(params) == 0) ? (("(() => " + emit_expr(body)) + ")") : ((list_length(params) == 1) ? ((object p = list_at(params)(0)) is var _ ? (((((("((" + cs_type(p.type_val)) + " ") + sanitize(p.name)) + ") => ") + emit_expr(body)) + ")") : default) : (("(() => " + emit_expr(body)) + ")")));

    public static string emit_list(object elems, object ty) => ((list_length(elems) == 0) ? (("new List<" + cs_type(ty)) + ">()") : (((("new List<" + cs_type(ty)) + "> { ") + emit_list_elems(elems)(0)) + " }"));

    public static string emit_list_elems(object elems, object i) => ((i == list_length(elems)) ? "" : ((i == (list_length(elems) - 1)) ? emit_expr(list_at(elems)(i)) : ((emit_expr(list_at(elems)(i)) + ", ") + emit_list_elems(elems)((i + 1)))));

    public static string emit_match(object scrut, object branches, object ty) => (((emit_expr(scrut) + " switch { ") + emit_match_arms(branches)(0)) + " }");

    public static string emit_match_arms(object branches, object i) => ((i == list_length(branches)) ? "" : ((object arm = list_at(branches)(i)) is var _ ? ((((emit_pattern(arm.pattern) + " => ") + emit_expr(arm.body)) + ", ") + emit_match_arms(branches)((i + 1))) : default));

    public static string emit_pattern(object p) => p switch { IrVarPat(var name, var ty) => ((cs_type(ty) + " ") + sanitize(name)), IrLitPat(var text, var ty) => text, IrCtorPat(var name, var subs, var ty) => ((list_length(subs) == 0) ? (sanitize(name) + " { }") : (((sanitize(name) + "(") + emit_sub_patterns(subs)(0)) + ")")), IrWildPat { } => "_",  };

    public static string emit_sub_patterns(object subs, object i) => ((i == list_length(subs)) ? "" : ((object sub = list_at(subs)(i)) is var _ ? ((emit_sub_pattern(sub) + ((i < (list_length(subs) - 1)) ? ", " : "")) + emit_sub_patterns(subs)((i + 1))) : default));

    public static string emit_sub_pattern(object p) => p switch { IrVarPat(var name, var ty) => ("var " + sanitize(name)), IrCtorPat(var name, var subs, var ty) => emit_pattern(p), IrWildPat { } => "_", IrLitPat(var text, var ty) => text,  };

    public static string emit_do(object stmts) => (("{ " + emit_do_stmts(stmts)(0)) + " }");

    public static string emit_do_stmts(object stmts, object i) => ((i == list_length(stmts)) ? "" : ((object s = list_at(stmts)(i)) is var _ ? ((emit_do_stmt(s) + " ") + emit_do_stmts(stmts)((i + 1))) : default));

    public static string emit_do_stmt(object s) => s switch { IrDoBind(var name, var ty, var val) => (((("var " + sanitize(name)) + " = ") + emit_expr(val)) + ";"), IrDoExec(var e) => (emit_expr(e) + ";"),  };

    public static string emit_record(object name, object fields) => (((("new " + sanitize(name)) + "(") + emit_record_fields(fields)(0)) + ")");

    public static string emit_record_fields(object fields, object i) => ((i == list_length(fields)) ? "" : ((object f = list_at(fields)(i)) is var _ ? ((((sanitize(f.name) + ": ") + emit_expr(f.value)) + ((i < (list_length(fields) - 1)) ? ", " : "")) + emit_record_fields(fields)((i + 1))) : default));

    public static string emit_type_defs(object tds, object i) => ((i == list_length(tds)) ? "" : ((emit_type_def(list_at(tds)(i)) + "\\n") + emit_type_defs(tds)((i + 1))));

    public static string emit_type_def(object td) => td switch { ARecordTypeDef(var name, var tparams, var fields) => ((object gen = emit_tparam_suffix(tparams)) is var _ ? ((((("public sealed record " + sanitize(name.value)) + gen) + "(") + emit_record_field_defs(fields)(tparams)(0)) + ");\\n") : default), AVariantTypeDef(var name, var tparams, var ctors) => ((object gen = emit_tparam_suffix(tparams)) is var _ ? ((((("public abstract record " + sanitize(name.value)) + gen) + ";\\n") + emit_variant_ctors(ctors)(name)(tparams)(0)) + "\\n") : default),  };

    public static string emit_tparam_suffix(object tparams) => ((list_length(tparams) == 0) ? "" : (("<" + emit_tparam_names(tparams)(0)) + ">"));

    public static string emit_tparam_names(object tparams, object i) => ((i == list_length(tparams)) ? "" : ((i == (list_length(tparams) - 1)) ? ("T" + integer_to_text(i)) : ((("T" + integer_to_text(i)) + ", ") + emit_tparam_names(tparams)((i + 1)))));

    public static string emit_record_field_defs(object fields, object tparams, object i) => ((i == list_length(fields)) ? "" : ((object f = list_at(fields)(i)) is var _ ? ((((emit_type_expr_tp(f.type_expr)(tparams) + " ") + sanitize(f.name.value)) + ((i < (list_length(fields) - 1)) ? ", " : "")) + emit_record_field_defs(fields)(tparams)((i + 1))) : default));

    public static string emit_variant_ctors(object ctors, object base_name, object tparams, object i) => ((i == list_length(ctors)) ? "" : ((object c = list_at(ctors)(i)) is var _ ? (emit_variant_ctor(c)(base_name)(tparams) + emit_variant_ctors(ctors)(base_name)(tparams)((i + 1))) : default));

    public static string emit_variant_ctor(object c, object base_name, object tparams) => ((object gen = emit_tparam_suffix(tparams)) is var _ ? ((list_length(c.fields) == 0) ? (((((("public sealed record " + sanitize(c.name.value)) + gen) + " : ") + sanitize(base_name.value)) + gen) + ";\\n") : (((((((("public sealed record " + sanitize(c.name.value)) + gen) + "(") + emit_ctor_fields(c.fields)(tparams)(0)) + ") : ") + sanitize(base_name.value)) + gen) + ";\\n")) : default);

    public static string emit_ctor_fields(object fields, object tparams, object i) => ((i == list_length(fields)) ? "" : ((((emit_type_expr_tp(list_at(fields)(i))(tparams) + " Field") + integer_to_text(i)) + ((i < (list_length(fields) - 1)) ? ", " : "")) + emit_ctor_fields(fields)(tparams)((i + 1))));

    public static T828 emit_type_expr<T828>(object te) => emit_type_expr_tp(te)(new List<object>());

    public static string emit_type_expr_tp(object te, object tparams) => te switch { ANamedType(var name) => ((object idx = find_tparam_index(tparams)(name.value)(0)) is var _ ? ((idx >= 0) ? ("T" + integer_to_text(idx)) : when_type_name(name.value)) : default), AFunType(var p, var r) => (((("Func<" + emit_type_expr_tp(p)(tparams)) + ", ") + emit_type_expr_tp(r)(tparams)) + ">"), AAppType(var base, var args) => (((emit_type_expr_tp(base)(tparams) + "<") + emit_type_expr_list_tp(args)(tparams)(0)) + ">"),  };

    public static long find_tparam_index(object tparams, object name, object i) => ((i == list_length(tparams)) ? (0 - 1) : ((list_at(tparams)(i).value == name) ? i : find_tparam_index(tparams)(name)((i + 1))));

    public static string when_type_name(object n) => ((n == "Integer") ? "long" : ((n == "Number") ? "decimal" : ((n == "Text") ? "string" : ((n == "Boolean") ? "bool" : ((n == "List") ? "List" : sanitize(n))))));

    public static string emit_type_expr_list(object args, object i) => ((i == list_length(args)) ? "" : ((emit_type_expr(list_at(args)(i)) + ((i < (list_length(args) - 1)) ? ", " : "")) + emit_type_expr_list(args)((i + 1))));

    public static string emit_type_expr_list_tp(object args, object tparams, object i) => ((i == list_length(args)) ? "" : ((emit_type_expr_tp(list_at(args)(i))(tparams) + ((i < (list_length(args) - 1)) ? ", " : "")) + emit_type_expr_list_tp(args)(tparams)((i + 1))));

    public static T886 collect_type_var_ids<T886>(object ty, object acc) => ty switch { TypeVar(var id) => (list_contains_int(acc)(id) ? acc : list_append_int(acc)(id)), FunTy(var p, var r) => collect_type_var_ids(r)(collect_type_var_ids(p)(acc)), ListTy(var elem) => collect_type_var_ids(elem)(acc), ForAllTy(var id, var body) => collect_type_var_ids(body)(acc), ConstructedTy(var name, var args) => collect_type_var_ids_list(args)(acc), _ => acc,  };

    public static T908 collect_type_var_ids_list<T908>(object types, object acc) => collect_type_var_ids_list_loop(types)(acc)(0)(list_length(types));

    public static T921 collect_type_var_ids_list_loop<T921>(object types, object acc, object i, object len) => ((i == len) ? acc : collect_type_var_ids_list_loop(types)(collect_type_var_ids(list_at(types)(i))(acc))((i + 1))(len));

    public static T929 list_contains_int<T929>(object xs, object n) => list_contains_int_loop(xs)(n)(0)(list_length(xs));

    public static bool list_contains_int_loop(object xs, object n, object i, object len) => ((i == len) ? false : ((list_at(xs)(i) == n) ? true : list_contains_int_loop(xs)(n)((i + 1))(len)));

    public static List<T943> list_append_int<T943>(object xs, object n) => (xs + new List<object> { n });

    public static string generic_suffix(object ty) => ((object ids = collect_type_var_ids(ty)(new List<object>())) is var _ ? ((list_length(ids) == 0) ? "" : (("<" + emit_type_params(ids)(0)) + ">")) : default);

    public static string emit_type_params(object ids, object i) => ((i == list_length(ids)) ? "" : ((i == (list_length(ids) - 1)) ? ("T" + integer_to_text(list_at(ids)(i))) : ((("T" + integer_to_text(list_at(ids)(i))) + ", ") + emit_type_params(ids)((i + 1)))));

    public static string emit_def(object d) => ((object ret = get_return_type(d.type_val)(list_length(d.params))) is var _ ? ((object gen = generic_suffix(d.type_val)) is var _ ? ((((((((("    public static " + cs_type(ret)) + " ") + sanitize(d.name)) + gen) + "(") + emit_def_params(d.params)(0)) + ") => ") + emit_expr(d.body)) + ";\\n") : default) : default);

    public static T981 get_return_type<T981>(object ty, object n) => ((n == 0) ? strip_forall(ty) : strip_forall(ty) switch { FunTy(var p, var r) => get_return_type(r)((n - 1)), _ => ty,  });

    public static T986 strip_forall<T986>(object ty) => ty switch { ForAllTy(var id, var body) => strip_forall(body), _ => ty,  };

    public static string emit_def_params(object params, object i) => ((i == list_length(params)) ? "" : ((object p = list_at(params)(i)) is var _ ? ((((cs_type(p.type_val) + " ") + sanitize(p.name)) + ((i < (list_length(params) - 1)) ? ", " : "")) + emit_def_params(params)((i + 1))) : default));

    public static string emit_full_module(object m, object type_defs) => (((("using System;\\nusing System.Collections.Generic;\\nusing System.Linq;\\n\\n" + emit_type_defs(type_defs)(0)) + emit_class_header(m.name.value)) + emit_defs(m.defs)(0)) + "}\\n");

    public static string emit_module(object m) => ((("using System;\\nusing System.Collections.Generic;\\nusing System.Linq;\\n\\n" + emit_class_header(m.name.value)) + emit_defs(m.defs)(0)) + "}\\n");

    public static string emit_class_header(object name) => (("public static class Codex_" + sanitize(name)) + "\\n{\\n");

    public static string emit_defs(object defs, object i) => ((i == list_length(defs)) ? "" : ((emit_def(list_at(defs)(i)) + "\\n") + emit_defs(defs)((i + 1))));

    public static T1031 lookup_type<T1031>(object bindings, object name) => lookup_type_loop(bindings)(name)(0)(list_length(bindings));

    public static object lookup_type_loop(object bindings, object name, object i, object len) => ((i == len) ? ErrorTy : ((object b = list_at(bindings)(i)) is var _ ? ((b.name == name) ? b.bound_type : lookup_type_loop(bindings)(name)((i + 1))(len)) : default));

    public static object peel_fun_param(object ty) => ty switch { FunTy(var p, var r) => p, ForAllTy(var id, var body) => peel_fun_param(body), _ => ErrorTy,  };

    public static object peel_fun_return(object ty) => ty switch { FunTy(var p, var r) => r, ForAllTy(var id, var body) => peel_fun_return(body), _ => ErrorTy,  };

    public static T1053 strip_forall_ty<T1053>(object ty) => ty switch { ForAllTy(var id, var body) => strip_forall_ty(body), _ => ty,  };

    public static object lower_bin_op(object op, object ty) => op switch { OpAdd { } => IrAddInt, OpSub { } => IrSubInt, OpMul { } => IrMulInt, OpDiv { } => IrDivInt, OpPow { } => IrPowInt, OpEq { } => IrEq, OpNotEq { } => IrNotEq, OpLt { } => IrLt, OpGt { } => IrGt, OpLtEq { } => IrLtEq, OpGtEq { } => IrGtEq, OpDefEq { } => IrEq, OpAppend { } => IrAppendList, OpCons { } => IrConsList, OpAnd { } => IrAnd, OpOr { } => IrOr,  };

    public static T1062 lower_expr<T1062>(object e, object ty) => e switch { ALitExpr(var text, var kind) => lower_literal(text)(kind), ANameExpr(var name) => IrName(name.value)(ty), AApplyExpr(var f, var a) => lower_apply(f)(a)(ty), ABinaryExpr(var l, var op, var r) => IrBinary(lower_bin_op(op)(ty))(lower_expr(l)(ty))(lower_expr(r)(ty))(ty), AUnaryExpr(var operand) => IrNegate(lower_expr(operand)(IntegerTy)), AIfExpr(var c, var t, var e2) => IrIf(lower_expr(c)(BooleanTy))(lower_expr(t)(ty))(lower_expr(e2)(ty))(ty), ALetExpr(var binds, var body) => lower_let(binds)(body)(ty), ALambdaExpr(var params, var body) => lower_lambda(params)(body)(ty), AMatchExpr(var scrut, var arms) => lower_match(scrut)(arms)(ty), AListExpr(var elems) => lower_list(elems)(ty), ARecordExpr(var name, var fields) => lower_record(name)(fields)(ty), AFieldAccess(var rec, var field) => IrFieldAccess(lower_expr(rec)(ty))(field.value)(ty), ADoExpr(var stmts) => lower_do(stmts)(ty), AErrorExpr(var msg) => IrError(msg)(ty),  };

    public static T1119 lower_literal<T1119>(object text, object kind) => kind switch { IntLit { } => IrIntLit(text_to_integer(text)), NumLit { } => IrIntLit(text_to_integer(text)), TextLit { } => IrTextLit(text), BoolLit { } => IrBoolLit((text == "True")),  };

    public static T1136 lower_apply<T1136>(object f, object a, object ty) => IrApply(lower_expr(f)(ty))(lower_expr(a)(ty))(ty);

    public static T1155 lower_let<T1155>(object binds, object body, object ty) => ((list_length(binds) == 0) ? lower_expr(body)(ty) : ((object b = list_at(binds)(0)) is var _ ? IrLet(b.name.value)(ty)(lower_expr(b.value)(ErrorTy))(lower_let_rest(binds)(body)(ty)(1)) : default));

    public static T1175 lower_let_rest<T1175>(object binds, object body, object ty, object i) => ((i == list_length(binds)) ? lower_expr(body)(ty) : ((object b = list_at(binds)(i)) is var _ ? IrLet(b.name.value)(ty)(lower_expr(b.value)(ErrorTy))(lower_let_rest(binds)(body)(ty)((i + 1))) : default));

    public static T1191 lower_lambda<T1191>(object params, object body, object ty) => ((object stripped = strip_forall_ty(ty)) is var _ ? IrLambda(lower_lambda_params(params)(stripped)(0))(lower_expr(body)(get_lambda_return(stripped)(list_length(params))))(ty) : default);

    public static List<object> lower_lambda_params(object params, object ty, object i) => ((i == list_length(params)) ? new List<object>() : ((object p = list_at(params)(i)) is var _ ? ((object param_ty = peel_fun_param(ty)) is var _ ? ((object rest_ty = peel_fun_return(ty)) is var _ ? (new List<object> { new IRParam(name: p.value, type_val: param_ty) } + lower_lambda_params(params)(rest_ty)((i + 1))) : default) : default) : default));

    public static object get_lambda_return(object ty, object n) => ((n == 0) ? ty : ty switch { FunTy(var p, var r) => get_lambda_return(r)((n - 1)), _ => ErrorTy,  });

    public static T1222 lower_match<T1222>(object scrut, object arms, object ty) => IrMatch(lower_expr(scrut)(ty))(map_list(lower_arm(ty))(arms))(ty);

    public static object lower_arm(object ty, object arm) => new IRBranch(pattern: lower_pattern(arm.pattern), body: lower_expr(arm.body)(ty));

    public static object lower_pattern(object p) => p switch { AVarPat(var name) => IrVarPat(name.value)(ErrorTy), ALitPat(var text, var kind) => IrLitPat(text)(ErrorTy), ACtorPat(var name, var subs) => IrCtorPat(name.value)(map_list(lower_pattern)(subs))(ErrorTy), AWildPat { } => IrWildPat,  };

    public static T1246 lower_list<T1246>(object elems, object ty) => ((object elem_ty = ty switch { ListTy(var e) => e, _ => ErrorTy,  }) is var _ ? IrList(map_list(lower_elem(elem_ty))(elems))(elem_ty) : default);

    public static T1251 lower_elem<T1251>(object ty, object e) => lower_expr(e)(ty);

    public static T1261 lower_record<T1261>(object name, object fields, object ty) => IrRecord(name.value)(map_list(lower_field_val(ty))(fields))(ty);

    public static object lower_field_val(object ty, object f) => new IRFieldVal(name: f.name.value, value: lower_expr(f.value)(ty));

    public static T1272 lower_do<T1272>(object stmts, object ty) => IrDo(map_list(lower_do_stmt(ty))(stmts))(ty);

    public static T1276 lower_do_stmt<T1276>(object ty, object s) => s switch { ADoBindStmt(var name, var val) => IrDoBind(name.value)(ty)(lower_expr(val)(ty)), ADoExprStmt(var e) => IrDoExec(lower_expr(e)(ty)),  };

    public static object lower_def(object d, object types, object ust) => ((object raw_type = lookup_type(types)(d.name.value)) is var _ ? ((object full_type = deep_resolve(ust)(raw_type)) is var _ ? ((object stripped = strip_forall_ty(full_type)) is var _ ? ((object params = lower_def_params(d.params)(stripped)(0)) is var _ ? ((object ret_type = get_return_type_n(stripped)(list_length(d.params))) is var _ ? new IRDef(name: d.name.value, params: params, type_val: full_type, body: lower_expr(d.body)(ret_type)) : default) : default) : default) : default) : default);

    public static List<object> lower_def_params(object params, object ty, object i) => ((i == list_length(params)) ? new List<object>() : ((object p = list_at(params)(i)) is var _ ? ((object param_ty = peel_fun_param(ty)) is var _ ? ((object rest_ty = peel_fun_return(ty)) is var _ ? (new List<object> { new IRParam(name: p.name.value, type_val: param_ty) } + lower_def_params(params)(rest_ty)((i + 1))) : default) : default) : default));

    public static object get_return_type_n(object ty, object n) => ((n == 0) ? ty : ty switch { FunTy(var p, var r) => get_return_type_n(r)((n - 1)), _ => ErrorTy,  });

    public static object lower_module(object m, object types, object ust) => new IRModule(name: m.name, defs: lower_defs(m.defs)(types)(ust)(0));

    public static List<T1334> lower_defs<T1334>(object defs, object types, object ust, object i) => ((i == list_length(defs)) ? new List<object>() : (new List<object> { lower_def(list_at(defs)(i))(types)(ust) } + lower_defs(defs)(types)(ust)((i + 1))));

    public static object empty_scope() => new Scope(names: new List<object>());

    public static T1347 scope_has<T1347>(object sc, object name) => scope_has_loop(sc.names)(name)(0)(list_length(sc.names));

    public static bool scope_has_loop(object names, object name, object i, object len) => ((i == len) ? false : ((list_at(names)(i) == name) ? true : scope_has_loop(names)(name)((i + 1))(len)));

    public static object scope_add(object sc, object name) => new Scope(names: (new List<object> { name } + sc.names));

    public static List<string> builtin_names() => new List<string> { "show", "negate", "True", "False", "Nothing", "print-line", "read-line", "open-file", "read-all", "close-file", "char-at", "text-length", "substring", "is-letter", "is-digit", "is-whitespace", "text-to-integer", "integer-to-text", "text-replace", "char-code", "code-to-char", "list-length", "list-at", "map", "filter", "fold" };

    public static bool is_type_name(object name) => ((text_length(name) == 0) ? false : (is_letter(char_at(name)(0)) && is_upper_char(char_at(name)(0))));

    public static bool is_upper_char(object c) => ((object code = char_code(c)) is var _ ? ((code >= 65) && (code <= 90)) : default);

    public static object collect_top_level_names(object defs, object i, object len, object acc, object errs) => ((i == len) ? new CollectResult(names: acc, errors: errs) : ((object def = list_at(defs)(i)) is var _ ? ((object name = def.name.value) is var _ ? (list_contains(acc)(name) ? collect_top_level_names(defs)((i + 1))(len)(acc)((errs + new List<object> { make_error("CDX3001")(("Duplicate definition: " + name)) })) : collect_top_level_names(defs)((i + 1))(len)((acc + new List<object> { name }))(errs)) : default) : default));

    public static T1404 list_contains<T1404>(object xs, object name) => list_contains_loop(xs)(name)(0)(list_length(xs));

    public static bool list_contains_loop(object xs, object name, object i, object len) => ((i == len) ? false : ((list_at(xs)(i) == name) ? true : list_contains_loop(xs)(name)((i + 1))(len)));

    public static object collect_ctor_names(object type_defs, object i, object len, object type_acc, object ctor_acc) => ((i == len) ? new CtorCollectResult(type_names: type_acc, ctor_names: ctor_acc) : ((object td = list_at(type_defs)(i)) is var _ ? td switch { AVariantTypeDef(var name, var params, var ctors) => ((object new_type_acc = (type_acc + new List<object> { name.value })) is var _ ? ((object new_ctor_acc = collect_variant_ctors(ctors)(0)(list_length(ctors))(ctor_acc)) is var _ ? collect_ctor_names(type_defs)((i + 1))(len)(new_type_acc)(new_ctor_acc) : default) : default), ARecordTypeDef(var name, var params, var fields) => collect_ctor_names(type_defs)((i + 1))(len)((type_acc + new List<object> { name.value }))(ctor_acc),  } : default));

    public static List<object> collect_variant_ctors(object ctors, object i, object len, object acc) => ((i == len) ? acc : ((object ctor = list_at(ctors)(i)) is var _ ? collect_variant_ctors(ctors)((i + 1))(len)((acc + new List<object> { ctor.name.value })) : default));

    public static T1469 build_all_names_scope<T1469>(object top_names, object ctor_names, object builtins) => ((object sc = add_names_to_scope(empty_scope)(top_names)(0)(list_length(top_names))) is var _ ? ((object sc2 = add_names_to_scope(sc)(ctor_names)(0)(list_length(ctor_names))) is var _ ? add_names_to_scope(sc2)(builtins)(0)(list_length(builtins)) : default) : default);

    public static T1482 add_names_to_scope<T1482>(object sc, object names, object i, object len) => ((i == len) ? sc : add_names_to_scope(scope_add(sc)(list_at(names)(i)))(names)((i + 1))(len));

    public static List<T1487> resolve_expr<T1487>(object sc, object expr) => expr switch { ALitExpr(var val, var kind) => new List<object>(), ANameExpr(var name) => ((scope_has(sc)(name.value) || is_type_name(name.value)) ? new List<object>() : new List<object> { make_error("CDX3002")(("Undefined name: " + name.value)) }), ABinaryExpr(var left, var op, var right) => (resolve_expr(sc)(left) + resolve_expr(sc)(right)), AUnaryExpr(var operand) => resolve_expr(sc)(operand), AApplyExpr(var func, var arg) => (resolve_expr(sc)(func) + resolve_expr(sc)(arg)), AIfExpr(var cond, var then_e, var else_e) => ((resolve_expr(sc)(cond) + resolve_expr(sc)(then_e)) + resolve_expr(sc)(else_e)), ALetExpr(var bindings, var body) => resolve_let(sc)(bindings)(body)(0)(list_length(bindings))(new List<object>()), ALambdaExpr(var params, var body) => ((object sc2 = add_lambda_params(sc)(params)(0)(list_length(params))) is var _ ? resolve_expr(sc2)(body) : default), AMatchExpr(var scrutinee, var arms) => (resolve_expr(sc)(scrutinee) + resolve_match_arms(sc)(arms)(0)(list_length(arms))(new List<object>())), AListExpr(var elems) => resolve_list_elems(sc)(elems)(0)(list_length(elems))(new List<object>()), ARecordExpr(var name, var fields) => resolve_record_fields(sc)(fields)(0)(list_length(fields))(new List<object>()), AFieldAccess(var obj, var field) => resolve_expr(sc)(obj), ADoExpr(var stmts) => resolve_do_stmts(sc)(stmts)(0)(list_length(stmts))(new List<object>()), AErrorExpr(var msg) => new List<object>(),  };

    public static T1578 resolve_let<T1578>(object sc, object bindings, object body, object i, object len, object errs) => ((i == len) ? (errs + resolve_expr(sc)(body)) : ((object b = list_at(bindings)(i)) is var _ ? ((object bind_errs = resolve_expr(sc)(b.value)) is var _ ? ((object sc2 = scope_add(sc)(b.name.value)) is var _ ? resolve_let(sc2)(bindings)(body)((i + 1))(len)((errs + bind_errs)) : default) : default) : default));

    public static T1591 add_lambda_params<T1591>(object sc, object params, object i, object len) => ((i == len) ? sc : ((object p = list_at(params)(i)) is var _ ? add_lambda_params(scope_add(sc)(p.value))(params)((i + 1))(len) : default));

    public static T1608 resolve_match_arms<T1608>(object sc, object arms, object i, object len, object errs) => ((i == len) ? errs : ((object arm = list_at(arms)(i)) is var _ ? ((object sc2 = collect_pattern_names(sc)(arm.pattern)) is var _ ? ((object arm_errs = resolve_expr(sc2)(arm.body)) is var _ ? resolve_match_arms(sc)(arms)((i + 1))(len)((errs + arm_errs)) : default) : default) : default));

    public static T1612 collect_pattern_names<T1612>(object sc, object pat) => pat switch { AVarPat(var name) => scope_add(sc)(name.value), ACtorPat(var name, var subs) => collect_ctor_pat_names(sc)(subs)(0)(list_length(subs)), ALitPat(var val, var kind) => sc, AWildPat { } => sc,  };

    public static T1632 collect_ctor_pat_names<T1632>(object sc, object subs, object i, object len) => ((i == len) ? sc : ((object sub = list_at(subs)(i)) is var _ ? collect_ctor_pat_names(collect_pattern_names(sc)(sub))(subs)((i + 1))(len) : default));

    public static T1647 resolve_list_elems<T1647>(object sc, object elems, object i, object len, object errs) => ((i == len) ? errs : ((object errs2 = resolve_expr(sc)(list_at(elems)(i))) is var _ ? resolve_list_elems(sc)(elems)((i + 1))(len)((errs + errs2)) : default));

    public static T1662 resolve_record_fields<T1662>(object sc, object fields, object i, object len, object errs) => ((i == len) ? errs : ((object f = list_at(fields)(i)) is var _ ? ((object errs2 = resolve_expr(sc)(f.value)) is var _ ? resolve_record_fields(sc)(fields)((i + 1))(len)((errs + errs2)) : default) : default));

    public static T1671 resolve_do_stmts<T1671>(object sc, object stmts, object i, object len, object errs) => ((i == len) ? errs : ((object stmt = list_at(stmts)(i)) is var _ ? stmt switch { ADoExprStmt(var e) => ((object errs2 = resolve_expr(sc)(e)) is var _ ? resolve_do_stmts(sc)(stmts)((i + 1))(len)((errs + errs2)) : default), ADoBindStmt(var name, var e) => ((object errs2 = resolve_expr(sc)(e)) is var _ ? ((object sc2 = scope_add(sc)(name.value)) is var _ ? resolve_do_stmts(sc2)(stmts)((i + 1))(len)((errs + errs2)) : default) : default),  } : default));

    public static T1707 resolve_all_defs<T1707>(object sc, object defs, object i, object len, object errs) => ((i == len) ? errs : ((object def = list_at(defs)(i)) is var _ ? ((object def_scope = add_def_params(sc)(def.params)(0)(list_length(def.params))) is var _ ? ((object errs2 = resolve_expr(def_scope)(def.body)) is var _ ? resolve_all_defs(sc)(defs)((i + 1))(len)((errs + errs2)) : default) : default) : default));

    public static T1720 add_def_params<T1720>(object sc, object params, object i, object len) => ((i == len) ? sc : ((object p = list_at(params)(i)) is var _ ? add_def_params(scope_add(sc)(p.name.value))(params)((i + 1))(len) : default));

    public static object resolve_module(object mod) => ((object top = collect_top_level_names(mod.defs)(0)(list_length(mod.defs))(new List<object>())(new List<object>())) is var _ ? ((object ctors = collect_ctor_names(mod.type_defs)(0)(list_length(mod.type_defs))(new List<object>())(new List<object>())) is var _ ? ((object sc = build_all_names_scope(top.names)(ctors.ctor_names)(builtin_names)) is var _ ? ((object expr_errs = resolve_all_defs(sc)(mod.defs)(0)(list_length(mod.defs))(new List<object>())) is var _ ? new ResolveResult(errors: (top.errors + expr_errs), top_level_names: top.names, type_names: ctors.type_names, ctor_names: ctors.ctor_names) : default) : default) : default) : default);

    public static object make_lex_state(object src) => new LexState(source: src, offset: 0, line: 1, column: 1);

    public static bool is_at_end(object st) => (st.offset >= text_length(st.source));

    public static string peek_char(object st) => (is_at_end(st) ? "" : char_at(st.source)(st.offset));

    public static object advance_char(object st) => ((peek_char(st) == "\\n") ? new LexState(source: st.source, offset: (st.offset + 1), line: (st.line + 1), column: 1) : new LexState(source: st.source, offset: (st.offset + 1), line: st.line, column: (st.column + 1)));

    public static T1763 skip_spaces<T1763>(object st) => (is_at_end(st) ? st : ((peek_char(st) == " ") ? skip_spaces(advance_char(st)) : st));

    public static T1769 scan_ident_rest<T1769>(object st) => (is_at_end(st) ? st : ((object ch = peek_char(st)) is var _ ? (is_letter(ch) ? scan_ident_rest(advance_char(st)) : (is_digit(ch) ? scan_ident_rest(advance_char(st)) : ((ch == "_") ? scan_ident_rest(advance_char(st)) : ((ch == "-") ? ((object next = advance_char(st)) is var _ ? (is_at_end(next) ? st : (is_letter(peek_char(next)) ? scan_ident_rest(next) : st)) : default) : st)))) : default));

    public static T1786 scan_digits<T1786>(object st) => (is_at_end(st) ? st : ((object ch = peek_char(st)) is var _ ? (is_digit(ch) ? scan_digits(advance_char(st)) : ((ch == "_") ? scan_digits(advance_char(st)) : st)) : default));

    public static T1803 scan_string_body<T1803>(object st) => (is_at_end(st) ? st : ((object ch = peek_char(st)) is var _ ? ((ch == "\\\"") ? advance_char(st) : ((ch == "\\n") ? st : ((ch == "\\\\") ? scan_string_body(advance_char(advance_char(st))) : scan_string_body(advance_char(st))))) : default));

    public static object classify_word(object w) => ((w == "let") ? LetKeyword : ((w == "in") ? InKeyword : ((w == "if") ? IfKeyword : ((w == "then") ? ThenKeyword : ((w == "else") ? ElseKeyword : ((w == "when") ? WhenKeyword : ((w == "where") ? WhereKeyword : ((w == "do") ? DoKeyword : ((w == "record") ? RecordKeyword : ((w == "import") ? ImportKeyword : ((w == "export") ? ExportKeyword : ((w == "claim") ? ClaimKeyword : ((w == "proof") ? ProofKeyword : ((w == "forall") ? ForAllKeyword : ((w == "exists") ? ThereExistsKeyword : ((w == "linear") ? LinearKeyword : ((w == "True") ? TrueKeyword : ((w == "False") ? FalseKeyword : ((object first_code = char_code(char_at(w)(0))) is var _ ? ((first_code >= 65) ? ((first_code <= 90) ? TypeIdentifier : Identifier) : Identifier) : default)))))))))))))))))));

    public static object make_token(object kind, object text, object st) => new Token(kind: kind, text: text, offset: st.offset, line: st.line, column: st.column);

    public static T1819 extract_text<T1819>(object st, object start, object end_st) => substring(st.source)(start)((end_st.offset - start));

    public static object scan_token(object st) => ((object s = skip_spaces(st)) is var _ ? (is_at_end(s) ? LexEnd : ((object ch = peek_char(s)) is var _ ? ((ch == "\\n") ? LexToken(make_token(Newline)("\\n")(s))(advance_char(s)) : ((ch == "\\\"") ? ((object start = (s.offset + 1)) is var _ ? ((object after = scan_string_body(advance_char(s))) is var _ ? ((object text_len = ((after.offset - start) - 1)) is var _ ? LexToken(make_token(TextLiteral)(substring(s.source)(start)(text_len))(s))(after) : default) : default) : default) : (is_letter(ch) ? ((object start = s.offset) is var _ ? ((object after = scan_ident_rest(advance_char(s))) is var _ ? ((object word = extract_text(s)(start)(after)) is var _ ? LexToken(make_token(classify_word(word))(word)(s))(after) : default) : default) : default) : ((ch == "_") ? ((object start = s.offset) is var _ ? ((object after = scan_ident_rest(advance_char(s))) is var _ ? ((object word = extract_text(s)(start)(after)) is var _ ? ((text_length(word) == 1) ? LexToken(make_token(Underscore)("_")(s))(after) : LexToken(make_token(classify_word(word))(word)(s))(after)) : default) : default) : default) : (is_digit(ch) ? ((object start = s.offset) is var _ ? ((object after = scan_digits(advance_char(s))) is var _ ? (is_at_end(after) ? LexToken(make_token(IntegerLiteral)(extract_text(s)(start)(after))(s))(after) : ((peek_char(after) == ".") ? ((object after2 = scan_digits(advance_char(after))) is var _ ? LexToken(make_token(NumberLiteral)(extract_text(s)(start)(after2))(s))(after2) : default) : LexToken(make_token(IntegerLiteral)(extract_text(s)(start)(after))(s))(after))) : default) : default) : scan_operator(s)))))) : default)) : default);

    public static T1956 scan_operator<T1956>(object s) => ((object ch = peek_char(s)) is var _ ? ((object next = advance_char(s)) is var _ ? ((ch == "(") ? LexToken(make_token(LeftParen)("(")(s))(next) : ((ch == ")") ? LexToken(make_token(RightParen)(")")(s))(next) : ((ch == "[") ? LexToken(make_token(LeftBracket)("[")(s))(next) : ((ch == "]") ? LexToken(make_token(RightBracket)("]")(s))(next) : ((ch == "{") ? LexToken(make_token(LeftBrace)("{")(s))(next) : ((ch == "}") ? LexToken(make_token(RightBrace)("}")(s))(next) : ((ch == ",") ? LexToken(make_token(Comma)(",")(s))(next) : ((ch == ".") ? LexToken(make_token(Dot)(".")(s))(next) : ((ch == "^") ? LexToken(make_token(Caret)("^")(s))(next) : ((ch == "&") ? LexToken(make_token(Ampersand)("&")(s))(next) : scan_multi_char_operator(s))))))))))) : default) : default);

    public static T2076 scan_multi_char_operator<T2076>(object s) => ((object ch = peek_char(s)) is var _ ? ((object next = advance_char(s)) is var _ ? ((object next_ch = (is_at_end(next) ? "" : peek_char(next))) is var _ ? ((ch == "+") ? ((next_ch == "+") ? LexToken(make_token(PlusPlus)("++")(s))(advance_char(next)) : LexToken(make_token(Plus)("+")(s))(next)) : ((ch == "-") ? ((next_ch == ">") ? LexToken(make_token(Arrow)("->")(s))(advance_char(next)) : LexToken(make_token(Minus)("-")(s))(next)) : ((ch == "*") ? LexToken(make_token(Star)("*")(s))(next) : ((ch == "/") ? ((next_ch == "=") ? LexToken(make_token(NotEquals)("/=")(s))(advance_char(next)) : LexToken(make_token(Slash)("/")(s))(next)) : ((ch == "=") ? ((next_ch == "=") ? ((object next2 = advance_char(next)) is var _ ? ((object next2_ch = (is_at_end(next2) ? "" : peek_char(next2))) is var _ ? ((next2_ch == "=") ? LexToken(make_token(TripleEquals)("===")(s))(advance_char(next2)) : LexToken(make_token(DoubleEquals)("==")(s))(next2)) : default) : default) : LexToken(make_token(Equals)("=")(s))(next)) : ((ch == ":") ? ((next_ch == ":") ? LexToken(make_token(ColonColon)("::")(s))(advance_char(next)) : LexToken(make_token(Colon)(":")(s))(next)) : ((ch == "|") ? ((next_ch == "-") ? LexToken(make_token(Turnstile)("|-")(s))(advance_char(next)) : LexToken(make_token(Pipe)("|")(s))(next)) : ((ch == "<") ? ((next_ch == "=") ? LexToken(make_token(LessOrEqual)("<=")(s))(advance_char(next)) : ((next_ch == "-") ? LexToken(make_token(LeftArrow)("<-")(s))(advance_char(next)) : LexToken(make_token(LessThan)("<")(s))(next))) : ((ch == ">") ? ((next_ch == "=") ? LexToken(make_token(GreaterOrEqual)(">=")(s))(advance_char(next)) : LexToken(make_token(GreaterThan)(">")(s))(next)) : LexToken(make_token(ErrorToken)(char_at(s.source)(s.offset))(s))(next)))))))))) : default) : default) : default);

    public static List<object> tokenize_loop(object st, object acc) => scan_token(st) switch { LexToken(var tok, var next) => ((tok.kind == EndOfFile) ? (acc + new List<object> { tok }) : tokenize_loop(next)((acc + new List<object> { tok }))), LexEnd { } => (acc + new List<object> { make_token(EndOfFile)("")(st) }),  };

    public static T2092 tokenize<T2092>(object src) => tokenize_loop(make_lex_state(src))(new List<object>());

    public static object make_parse_state(object toks) => new ParseState(tokens: toks, pos: 0);

    public static T2098 current<T2098>(object st) => list_at(st.tokens)(st.pos);

    public static object current_kind(object st) => current(st).kind;

    public static object advance(object st) => new ParseState(tokens: st.tokens, pos: (st.pos + 1));

    public static bool is_done(object st) => current_kind(st) switch { EndOfFile { } => true, _ => false,  };

    public static object peek_kind(object st, object offset) => list_at(st.tokens)((st.pos + offset)).kind;

    public static bool is_ident(object k) => k switch { Identifier { } => true, _ => false,  };

    public static bool is_type_ident(object k) => k switch { TypeIdentifier { } => true, _ => false,  };

    public static bool is_arrow(object k) => k switch { Arrow { } => true, _ => false,  };

    public static bool is_equals(object k) => k switch { Equals { } => true, _ => false,  };

    public static bool is_colon(object k) => k switch { Colon { } => true, _ => false,  };

    public static bool is_comma(object k) => k switch { Comma { } => true, _ => false,  };

    public static bool is_pipe(object k) => k switch { Pipe { } => true, _ => false,  };

    public static bool is_dot(object k) => k switch { Dot { } => true, _ => false,  };

    public static bool is_left_paren(object k) => k switch { LeftParen { } => true, _ => false,  };

    public static bool is_left_brace(object k) => k switch { LeftBrace { } => true, _ => false,  };

    public static bool is_left_bracket(object k) => k switch { LeftBracket { } => true, _ => false,  };

    public static bool is_right_brace(object k) => k switch { RightBrace { } => true, _ => false,  };

    public static bool is_right_bracket(object k) => k switch { RightBracket { } => true, _ => false,  };

    public static bool is_if_keyword(object k) => k switch { IfKeyword { } => true, _ => false,  };

    public static bool is_let_keyword(object k) => k switch { LetKeyword { } => true, _ => false,  };

    public static bool is_when_keyword(object k) => k switch { WhenKeyword { } => true, _ => false,  };

    public static bool is_do_keyword(object k) => k switch { DoKeyword { } => true, _ => false,  };

    public static bool is_in_keyword(object k) => k switch { InKeyword { } => true, _ => false,  };

    public static bool is_minus(object k) => k switch { Minus { } => true, _ => false,  };

    public static bool is_dedent(object k) => k switch { Dedent { } => true, _ => false,  };

    public static bool is_left_arrow(object k) => k switch { LeftArrow { } => true, _ => false,  };

    public static bool is_record_keyword(object k) => k switch { RecordKeyword { } => true, _ => false,  };

    public static bool is_underscore(object k) => k switch { Underscore { } => true, _ => false,  };

    public static bool is_literal(object k) => k switch { IntegerLiteral { } => true, NumberLiteral { } => true, TextLiteral { } => true, TrueKeyword { } => true, FalseKeyword { } => true, _ => false,  };

    public static bool is_app_start(object k) => k switch { Identifier { } => true, TypeIdentifier { } => true, IntegerLiteral { } => true, NumberLiteral { } => true, TextLiteral { } => true, TrueKeyword { } => true, FalseKeyword { } => true, LeftParen { } => true, LeftBracket { } => true, _ => false,  };

    public static bool is_compound(object e) => e switch { MatchExpr(var s, var arms) => true, IfExpr(var c, var t, var el) => true, LetExpr(var binds, var body) => true, DoExpr(var stmts) => true, _ => false,  };

    public static bool is_type_arg_start(object k) => k switch { TypeIdentifier { } => true, Identifier { } => true, LeftParen { } => true, _ => false,  };

    public static long operator_precedence(object k) => k switch { PlusPlus { } => 5, ColonColon { } => 5, Plus { } => 6, Minus { } => 6, Star { } => 7, Slash { } => 7, Caret { } => 8, DoubleEquals { } => 4, NotEquals { } => 4, LessThan { } => 4, GreaterThan { } => 4, LessOrEqual { } => 4, GreaterOrEqual { } => 4, TripleEquals { } => 4, Ampersand { } => 3, Pipe { } => 2, _ => (0 - 1),  };

    public static T2201 expect<T2201>(object kind, object st) => (is_done(st) ? st : advance(st));

    public static T2206 skip_newlines<T2206>(object st) => (is_done(st) ? st : current_kind(st) switch { Newline { } => skip_newlines(advance(st)), Indent { } => skip_newlines(advance(st)), Dedent { } => skip_newlines(advance(st)), _ => st,  });

    public static T2217 parse_type<T2217>(object st) => ((object result = parse_type_atom(st)) is var _ ? unwrap_type_ok(result)(parse_type_continue) : default);

    public static T2229 parse_type_continue<T2229>(object left, object st) => (is_arrow(current_kind(st)) ? ((object st2 = advance(st)) is var _ ? ((object right_result = parse_type(st2)) is var _ ? unwrap_type_ok(right_result)(make_fun_type(left)) : default) : default) : TypeOk(left)(st));

    public static T2237 make_fun_type<T2237>(object left, object right, object st) => TypeOk(FunType(left)(right))(st);

    public static T2241 unwrap_type_ok<T2241>(object r, object f) => r switch { TypeOk(var t, var st) => f(t)(st),  };

    public static T2268 parse_type_atom<T2268>(object st) => (is_ident(current_kind(st)) ? ((object tok = current(st)) is var _ ? parse_type_args(NamedType(tok))(advance(st)) : default) : (is_type_ident(current_kind(st)) ? ((object tok = current(st)) is var _ ? parse_type_args(NamedType(tok))(advance(st)) : default) : (is_left_paren(current_kind(st)) ? parse_paren_type(advance(st)) : ((object tok = current(st)) is var _ ? TypeOk(NamedType(tok))(advance(st)) : default))));

    public static T2273 parse_paren_type<T2273>(object st) => ((object inner = parse_type(st)) is var _ ? unwrap_type_ok(inner)(finish_paren_type) : default);

    public static T2281 finish_paren_type<T2281>(object t, object st) => ((object st2 = expect(RightParen)(st)) is var _ ? TypeOk(ParenType(t))(st2) : default);

    public static T2293 parse_type_args<T2293>(object base_type, object st) => (is_done(st) ? TypeOk(base_type)(st) : (is_type_arg_start(current_kind(st)) ? parse_type_arg_next(base_type)(st) : TypeOk(base_type)(st)));

    public static T2300 parse_type_arg_next<T2300>(object base_type, object st) => ((object arg_result = parse_type_atom(st)) is var _ ? unwrap_type_ok(arg_result)(continue_type_args(base_type)) : default);

    public static T2308 continue_type_args<T2308>(object base_type, object arg, object st) => parse_type_args(AppType(base_type)(new List<object> { arg }))(st);

    public static T2344 parse_pattern<T2344>(object st) => (is_underscore(current_kind(st)) ? PatOk(WildPat(current(st)))(advance(st)) : (is_literal(current_kind(st)) ? PatOk(LitPat(current(st)))(advance(st)) : (is_type_ident(current_kind(st)) ? ((object tok = current(st)) is var _ ? parse_ctor_pattern_fields(tok)(new List<object>())(advance(st)) : default) : (is_ident(current_kind(st)) ? PatOk(VarPat(current(st)))(advance(st)) : PatOk(WildPat(current(st)))(advance(st))))));

    public static T2360 parse_ctor_pattern_fields<T2360>(object ctor, object acc, object st) => (is_left_paren(current_kind(st)) ? ((object st2 = advance(st)) is var _ ? ((object sub = parse_pattern(st2)) is var _ ? unwrap_pat_ok(sub)(continue_ctor_fields(ctor)(acc)) : default) : default) : PatOk(CtorPat(ctor)(acc))(st));

    public static T2370 continue_ctor_fields<T2370>(object ctor, object acc, object p, object st) => ((object st2 = expect(RightParen)(st)) is var _ ? parse_ctor_pattern_fields(ctor)((acc + new List<object> { p }))(st2) : default);

    public static T2374 unwrap_pat_ok<T2374>(object r, object f) => r switch { PatOk(var p, var st) => f(p)(st),  };

    public static T2380 parse_expr<T2380>(object st) => parse_binary(st)(0);

    public static T2384 unwrap_expr_ok<T2384>(object r, object f) => r switch { ExprOk(var e, var st) => f(e)(st),  };

    public static T2393 parse_binary<T2393>(object st, object min_prec) => ((object left_result = parse_unary(st)) is var _ ? unwrap_expr_ok(left_result)(start_binary_loop(min_prec)) : default);

    public static T2400 start_binary_loop<T2400>(object min_prec, object left, object st) => parse_binary_loop(left)(st)(min_prec);

    public static T2421 parse_binary_loop<T2421>(object left, object st, object min_prec) => (is_done(st) ? ExprOk(left)(st) : ((object prec = operator_precedence(current_kind(st))) is var _ ? ((prec < min_prec) ? ExprOk(left)(st) : ((object op = current(st)) is var _ ? ((object st2 = skip_newlines(advance(st))) is var _ ? ((object right_result = parse_binary(st2)((prec + 1))) is var _ ? unwrap_expr_ok(right_result)(continue_binary(left)(op)(min_prec)) : default) : default) : default)) : default));

    public static T2433 continue_binary<T2433>(object left, object op, object min_prec, object right, object st) => parse_binary_loop(BinExpr(left)(op)(right))(st)(min_prec);

    public static T2444 parse_unary<T2444>(object st) => (is_minus(current_kind(st)) ? ((object op = current(st)) is var _ ? ((object result = parse_unary(advance(st))) is var _ ? unwrap_expr_ok(result)(finish_unary(op)) : default) : default) : parse_application(st));

    public static T2452 finish_unary<T2452>(object op, object operand, object st) => ExprOk(UnaryExpr(op)(operand))(st);

    public static T2457 parse_application<T2457>(object st) => ((object func_result = parse_atom(st)) is var _ ? unwrap_expr_ok(func_result)(parse_app_loop) : default);

    public static T2474 parse_app_loop<T2474>(object func, object st) => (is_compound(func) ? parse_field_access(func)(st) : (is_done(st) ? ExprOk(func)(st) : (is_app_start(current_kind(st)) ? ((object arg_result = parse_atom(st)) is var _ ? unwrap_expr_ok(arg_result)(continue_app(func)) : default) : parse_field_access(func)(st))));

    public static T2482 continue_app<T2482>(object func, object arg, object st) => parse_app_loop(AppExpr(func)(arg))(st);

    public static T2525 parse_atom<T2525>(object st) => (is_literal(current_kind(st)) ? ExprOk(LitExpr(current(st)))(advance(st)) : (is_ident(current_kind(st)) ? parse_field_access(NameExpr(current(st)))(advance(st)) : (is_type_ident(current_kind(st)) ? parse_atom_type_ident(st) : (is_left_paren(current_kind(st)) ? parse_paren_expr(advance(st)) : (is_left_bracket(current_kind(st)) ? parse_list_expr(st) : (is_if_keyword(current_kind(st)) ? parse_if_expr(st) : (is_let_keyword(current_kind(st)) ? parse_let_expr(st) : (is_when_keyword(current_kind(st)) ? parse_match_expr(st) : (is_do_keyword(current_kind(st)) ? parse_do_expr(st) : ExprOk(ErrExpr(current(st)))(advance(st)))))))))));

    public static T2539 parse_field_access<T2539>(object node, object st) => (is_dot(current_kind(st)) ? ((object st2 = advance(st)) is var _ ? ((object field = current(st2)) is var _ ? ((object st3 = advance(st2)) is var _ ? parse_field_access(FieldExpr(node)(field))(st3) : default) : default) : default) : ExprOk(node)(st));

    public static T2550 parse_atom_type_ident<T2550>(object st) => ((object tok = current(st)) is var _ ? ((object st2 = advance(st)) is var _ ? (is_left_brace(current_kind(st2)) ? parse_record_expr(tok)(st2) : ExprOk(NameExpr(tok))(st2)) : default) : default);

    public static T2556 parse_paren_expr<T2556>(object st) => ((object st2 = skip_newlines(st)) is var _ ? ((object inner = parse_expr(st2)) is var _ ? unwrap_expr_ok(inner)(finish_paren_expr) : default) : default);

    public static T2565 finish_paren_expr<T2565>(object e, object st) => ((object st2 = skip_newlines(st)) is var _ ? ((object st3 = expect(RightParen)(st2)) is var _ ? ExprOk(ParenExpr(e))(st3) : default) : default);

    public static T2574 parse_record_expr<T2574>(object type_name, object st) => ((object st2 = advance(st)) is var _ ? ((object st3 = skip_newlines(st2)) is var _ ? parse_record_expr_fields(type_name)(new List<object>())(st3) : default) : default);

    public static T2594 parse_record_expr_fields<T2594>(object type_name, object acc, object st) => (is_right_brace(current_kind(st)) ? ExprOk(RecordExpr(type_name)(acc))(advance(st)) : (is_ident(current_kind(st)) ? parse_record_field(type_name)(acc)(st) : ExprOk(RecordExpr(type_name)(acc))(st)));

    public static T2608 parse_record_field<T2608>(object type_name, object acc, object st) => ((object field_name = current(st)) is var _ ? ((object st2 = advance(st)) is var _ ? ((object st3 = expect(Equals)(st2)) is var _ ? ((object val_result = parse_expr(st3)) is var _ ? unwrap_expr_ok(val_result)(finish_record_field(type_name)(acc)(field_name)) : default) : default) : default) : default);

    public static T2625 finish_record_field<T2625>(object type_name, object acc, object field_name, object v, object st) => ((object field = new RecordFieldExpr(name: field_name, value: v)) is var _ ? ((object st2 = skip_newlines(st)) is var _ ? (is_comma(current_kind(st2)) ? parse_record_expr_fields(type_name)((acc + new List<object> { field }))(skip_newlines(advance(st2))) : parse_record_expr_fields(type_name)((acc + new List<object> { field }))(st2)) : default) : default);

    public static T2632 parse_list_expr<T2632>(object st) => ((object st2 = advance(st)) is var _ ? ((object st3 = skip_newlines(st2)) is var _ ? parse_list_elements(new List<object>())(st3) : default) : default);

    public static T2645 parse_list_elements<T2645>(object acc, object st) => (is_right_bracket(current_kind(st)) ? ExprOk(ListExpr(acc))(advance(st)) : ((object elem = parse_expr(st)) is var _ ? unwrap_expr_ok(elem)(finish_list_element(acc)) : default));

    public static T2658 finish_list_element<T2658>(object acc, object e, object st) => ((object st2 = skip_newlines(st)) is var _ ? (is_comma(current_kind(st2)) ? parse_list_elements((acc + new List<object> { e }))(skip_newlines(advance(st2))) : parse_list_elements((acc + new List<object> { e }))(st2)) : default);

    public static T2665 parse_if_expr<T2665>(object st) => ((object st2 = skip_newlines(advance(st))) is var _ ? ((object cond = parse_expr(st2)) is var _ ? unwrap_expr_ok(cond)(parse_if_then) : default) : default);

    public static T2676 parse_if_then<T2676>(object c, object st) => ((object st2 = skip_newlines(st)) is var _ ? ((object st3 = expect(ThenKeyword)(st2)) is var _ ? ((object st4 = skip_newlines(st3)) is var _ ? ((object then_result = parse_expr(st4)) is var _ ? unwrap_expr_ok(then_result)(parse_if_else(c)) : default) : default) : default) : default);

    public static T2689 parse_if_else<T2689>(object c, object t, object st) => ((object st2 = skip_newlines(st)) is var _ ? ((object st3 = expect(ElseKeyword)(st2)) is var _ ? ((object st4 = skip_newlines(st3)) is var _ ? ((object else_result = parse_expr(st4)) is var _ ? unwrap_expr_ok(else_result)(finish_if(c)(t)) : default) : default) : default) : default);

    public static T2699 finish_if<T2699>(object c, object t, object e, object st) => ExprOk(IfExpr(c)(t)(e))(st);

    public static T2706 parse_let_expr<T2706>(object st) => ((object st2 = skip_newlines(advance(st))) is var _ ? parse_let_bindings(new List<object>())(st2) : default);

    public static T2725 parse_let_bindings<T2725>(object acc, object st) => (is_ident(current_kind(st)) ? parse_let_binding(acc)(st) : (is_in_keyword(current_kind(st)) ? ((object st2 = skip_newlines(advance(st))) is var _ ? ((object body = parse_expr(st2)) is var _ ? unwrap_expr_ok(body)(finish_let(acc)) : default) : default) : ((object body = parse_expr(st)) is var _ ? unwrap_expr_ok(body)(finish_let(acc)) : default)));

    public static T2733 finish_let<T2733>(object acc, object b, object st) => ExprOk(LetExpr(acc)(b))(st);

    public static T2745 parse_let_binding<T2745>(object acc, object st) => ((object name_tok = current(st)) is var _ ? ((object st2 = advance(st)) is var _ ? ((object st3 = expect(Equals)(st2)) is var _ ? ((object val_result = parse_expr(st3)) is var _ ? unwrap_expr_ok(val_result)(finish_let_binding(acc)(name_tok)) : default) : default) : default) : default);

    public static T2759 finish_let_binding<T2759>(object acc, object name_tok, object v, object st) => ((object binding = new LetBind(name: name_tok, value: v)) is var _ ? ((object st2 = skip_newlines(st)) is var _ ? (is_comma(current_kind(st2)) ? parse_let_bindings((acc + new List<object> { binding }))(skip_newlines(advance(st2))) : parse_let_bindings((acc + new List<object> { binding }))(st2)) : default) : default);

    public static T2765 parse_match_expr<T2765>(object st) => ((object st2 = advance(st)) is var _ ? ((object scrut = parse_expr(st2)) is var _ ? unwrap_expr_ok(scrut)(start_match_branches) : default) : default);

    public static T2773 start_match_branches<T2773>(object s, object st) => ((object st2 = skip_newlines(st)) is var _ ? parse_match_branches(s)(new List<object>())(st2) : default);

    public static T2786 parse_match_branches<T2786>(object scrut, object acc, object st) => (is_if_keyword(current_kind(st)) ? parse_one_match_branch(scrut)(acc)(st) : ExprOk(MatchExpr(scrut)(acc))(st));

    public static T2790 unwrap_pat_for_expr<T2790>(object r, object f) => r switch { PatOk(var p, var st) => f(p)(st),  };

    public static T2802 parse_one_match_branch<T2802>(object scrut, object acc, object st) => ((object st2 = advance(st)) is var _ ? ((object pat = parse_pattern(st2)) is var _ ? unwrap_pat_for_expr(pat)(parse_match_branch_body(scrut)(acc)) : default) : default);

    public static T2816 parse_match_branch_body<T2816>(object scrut, object acc, object p, object st) => ((object st2 = expect(Arrow)(st)) is var _ ? ((object st3 = skip_newlines(st2)) is var _ ? ((object body = parse_expr(st3)) is var _ ? unwrap_expr_ok(body)(finish_match_branch(scrut)(acc)(p)) : default) : default) : default);

    public static T2826 finish_match_branch<T2826>(object scrut, object acc, object p, object b, object st) => ((object arm = new MatchArm(pattern: p, body: b)) is var _ ? ((object st2 = skip_newlines(st)) is var _ ? parse_match_branches(scrut)((acc + new List<object> { arm }))(st2) : default) : default);

    public static T2833 parse_do_expr<T2833>(object st) => ((object st2 = skip_newlines(advance(st))) is var _ ? parse_do_stmts(new List<object>())(st2) : default);

    public static T2850 parse_do_stmts<T2850>(object acc, object st) => (is_done(st) ? ExprOk(DoExpr(acc))(st) : (is_dedent(current_kind(st)) ? ExprOk(DoExpr(acc))(st) : (is_do_bind(st) ? parse_do_bind_stmt(acc)(st) : parse_do_expr_stmt(acc)(st))));

    public static bool is_do_bind(object st) => (is_ident(current_kind(st)) ? is_left_arrow(peek_kind(st)(1)) : false);

    public static T2868 parse_do_bind_stmt<T2868>(object acc, object st) => ((object name_tok = current(st)) is var _ ? ((object st2 = advance(advance(st))) is var _ ? ((object val_result = parse_expr(st2)) is var _ ? unwrap_expr_ok(val_result)(finish_do_bind(acc)(name_tok)) : default) : default) : default);

    public static T2878 finish_do_bind<T2878>(object acc, object name_tok, object v, object st) => ((object st2 = skip_newlines(st)) is var _ ? parse_do_stmts((acc + new List<object> { DoBindStmt(name_tok)(v) }))(st2) : default);

    public static T2885 parse_do_expr_stmt<T2885>(object acc, object st) => ((object expr_result = parse_expr(st)) is var _ ? unwrap_expr_ok(expr_result)(finish_do_expr(acc)) : default);

    public static T2893 finish_do_expr<T2893>(object acc, object e, object st) => ((object st2 = skip_newlines(st)) is var _ ? parse_do_stmts((acc + new List<object> { DoExprStmt(e) }))(st2) : default);

    public static T2899 parse_type_annotation<T2899>(object st) => ((object st2 = advance(st)) is var _ ? ((object st3 = expect(Colon)(st2)) is var _ ? parse_type(st3) : default) : default);

    public static T2910 parse_definition<T2910>(object st) => (is_done(st) ? DefNone(st) : (is_ident(current_kind(st)) ? try_parse_def(st) : (is_type_ident(current_kind(st)) ? try_parse_def(st) : DefNone(st))));

    public static T2920 try_parse_def<T2920>(object st) => (is_colon(peek_kind(st)(1)) ? ((object ann_result = parse_type_annotation(st)) is var _ ? unwrap_type_for_def(ann_result) : default) : parse_def_body_with_ann(new List<object>())(st));

    public static T2923 unwrap_type_for_def<T2923>(object r) => r switch { TypeOk(var ann_type, var st) => ((object name_tok = new Token(kind: Identifier, text: "", offset: 0, line: 0, column: 0)) is var _ ? ((object ann = new List<object> { new TypeAnn(name: name_tok, type_expr: ann_type) }) is var _ ? ((object st2 = skip_newlines(st)) is var _ ? parse_def_body_with_ann(ann)(st2) : default) : default) : default),  };

    public static T2936 parse_def_body_with_ann<T2936>(object ann, object st) => ((object name_tok = current(st)) is var _ ? ((object st2 = advance(st)) is var _ ? parse_def_params_then(ann)(name_tok)(new List<object>())(st2) : default) : default);

    public static T2962 parse_def_params_then<T2962>(object ann, object name_tok, object acc, object st) => (is_left_paren(current_kind(st)) ? ((object st2 = advance(st)) is var _ ? (is_ident(current_kind(st2)) ? ((object param = current(st2)) is var _ ? ((object st3 = advance(st2)) is var _ ? ((object st4 = expect(RightParen)(st3)) is var _ ? parse_def_params_then(ann)(name_tok)((acc + new List<object> { param }))(st4) : default) : default) : default) : finish_def(ann)(name_tok)(acc)(st)) : default) : finish_def(ann)(name_tok)(acc)(st));

    public static T2975 finish_def<T2975>(object ann, object name_tok, object params, object st) => ((object st2 = expect(Equals)(st)) is var _ ? ((object st3 = skip_newlines(st2)) is var _ ? ((object body_result = parse_expr(st3)) is var _ ? unwrap_def_body(body_result)(ann)(name_tok)(params) : default) : default) : default);

    public static T2981 unwrap_def_body<T2981>(object r, object ann, object name_tok, object params) => r switch { ExprOk(var b, var st) => DefOk(new Def(name: name_tok, params: params, ann: ann, body: b))(st),  };

    public static T3004 parse_type_def<T3004>(object st) => (is_type_ident(current_kind(st)) ? ((object name_tok = current(st)) is var _ ? ((object st2 = advance(st)) is var _ ? (is_equals(current_kind(st2)) ? ((object st3 = skip_newlines(advance(st2))) is var _ ? (is_record_keyword(current_kind(st3)) ? parse_record_type(name_tok)(st3) : (is_pipe(current_kind(st3)) ? parse_variant_type(name_tok)(st3) : TypeDefNone(st))) : default) : TypeDefNone(st)) : default) : default) : TypeDefNone(st));

    public static T3015 parse_record_type<T3015>(object name_tok, object st) => ((object st2 = advance(st)) is var _ ? ((object st3 = expect(LeftBrace)(st2)) is var _ ? ((object st4 = skip_newlines(st3)) is var _ ? parse_record_fields_loop(name_tok)(new List<object>())(st4) : default) : default) : default);

    public static T3031 parse_record_fields_loop<T3031>(object name_tok, object acc, object st) => (is_right_brace(current_kind(st)) ? TypeDefOk(new TypeDef(name: name_tok, type_params: new List<object>(), body: RecordBody(acc)))(advance(st)) : (is_ident(current_kind(st)) ? parse_one_record_field(name_tok)(acc)(st) : TypeDefOk(new TypeDef(name: name_tok, type_params: new List<object>(), body: RecordBody(acc)))(st)));

    public static T3044 parse_one_record_field<T3044>(object name_tok, object acc, object st) => ((object field_name = current(st)) is var _ ? ((object st2 = advance(st)) is var _ ? ((object st3 = expect(Colon)(st2)) is var _ ? ((object field_type_result = parse_type(st3)) is var _ ? unwrap_record_field_type(name_tok)(acc)(field_name)(field_type_result) : default) : default) : default) : default);

    public static T3050 unwrap_record_field_type<T3050>(object name_tok, object acc, object field_name, object r) => r switch { TypeOk(var ft, var st) => ((object field = new RecordFieldDef(name: field_name, type_expr: ft)) is var _ ? ((object st2 = skip_newlines(st)) is var _ ? (is_comma(current_kind(st2)) ? parse_record_fields_loop(name_tok)((acc + new List<object> { field }))(skip_newlines(advance(st2))) : parse_record_fields_loop(name_tok)((acc + new List<object> { field }))(st2)) : default) : default),  };

    public static T3068 parse_variant_type<T3068>(object name_tok, object st) => parse_variant_ctors(name_tok)(new List<object>())(st);

    public static T3086 parse_variant_ctors<T3086>(object name_tok, object acc, object st) => (is_pipe(current_kind(st)) ? ((object st2 = skip_newlines(advance(st))) is var _ ? ((object ctor_name = current(st2)) is var _ ? ((object st3 = advance(st2)) is var _ ? parse_ctor_fields(ctor_name)(new List<object>())(st3)(name_tok)(acc) : default) : default) : default) : TypeDefOk(new TypeDef(name: name_tok, type_params: new List<object>(), body: VariantBody(acc)))(st));

    public static T3105 parse_ctor_fields<T3105>(object ctor_name, object fields, object st, object name_tok, object acc) => (is_left_paren(current_kind(st)) ? ((object field_result = parse_type(advance(st))) is var _ ? unwrap_ctor_field(field_result)(ctor_name)(fields)(name_tok)(acc) : default) : ((object st2 = skip_newlines(st)) is var _ ? ((object ctor = new VariantCtorDef(name: ctor_name, fields: fields)) is var _ ? parse_variant_ctors(name_tok)((acc + new List<object> { ctor }))(st2) : default) : default));

    public static T3112 unwrap_ctor_field<T3112>(object r, object ctor_name, object fields, object name_tok, object acc) => r switch { TypeOk(var ty, var st) => ((object st2 = expect(RightParen)(st)) is var _ ? parse_ctor_fields(ctor_name)((fields + new List<object> { ty }))(st2)(name_tok)(acc) : default),  };

    public static T3127 parse_document<T3127>(object st) => ((object st2 = skip_newlines(st)) is var _ ? parse_top_level(new List<object>())(new List<object>())(st2) : default);

    public static object parse_top_level(object defs, object type_defs, object st) => (is_done(st) ? new Document(defs: defs, type_defs: type_defs) : try_top_level_type_def(defs)(type_defs)(st));

    public static T3141 try_top_level_type_def<T3141>(object defs, object type_defs, object st) => ((object td_result = parse_type_def(st)) is var _ ? td_result switch { TypeDefOk(var td, var st2) => parse_top_level(defs)((type_defs + new List<object> { td }))(skip_newlines(st2)), TypeDefNone(var st2) => try_top_level_def(defs)(type_defs)(st),  } : default);

    public static T3154 try_top_level_def<T3154>(object defs, object type_defs, object st) => ((object def_result = parse_definition(st)) is var _ ? def_result switch { DefOk(var d, var st2) => parse_top_level((defs + new List<object> { d }))(type_defs)(skip_newlines(st2)), DefNone(var st2) => parse_top_level(defs)(type_defs)(skip_newlines(advance(st2))),  } : default);

    public static T3166 token_length<T3166>(object t) => text_length(t.text);

    public static object infer_literal(object st, object kind) => kind switch { IntLit { } => new CheckResult(inferred_type: IntegerTy, state: st), NumLit { } => new CheckResult(inferred_type: NumberTy, state: st), TextLit { } => new CheckResult(inferred_type: TextTy, state: st), BoolLit { } => new CheckResult(inferred_type: BooleanTy, state: st),  };

    public static object infer_name(object st, object env, object name) => (env_has(env)(name) ? new CheckResult(inferred_type: env_lookup(env)(name), state: st) : new CheckResult(inferred_type: ErrorTy, state: add_unify_error(st)("CDX2002")(("Unknown name: " + name))));

    public static T3192 infer_binary<T3192>(object st, object env, object left, object op, object right) => ((object lr = infer_expr(st)(env)(left)) is var _ ? ((object rr = infer_expr(lr.state)(env)(right)) is var _ ? infer_binary_op(rr.state)(lr.inferred_type)(rr.inferred_type)(op) : default) : default);

    public static T3198 infer_binary_op<T3198>(object st, object lt, object rt, object op) => op switch { OpAdd { } => infer_arithmetic(st)(lt)(rt), OpSub { } => infer_arithmetic(st)(lt)(rt), OpMul { } => infer_arithmetic(st)(lt)(rt), OpDiv { } => infer_arithmetic(st)(lt)(rt), OpPow { } => infer_arithmetic(st)(lt)(rt), OpEq { } => infer_comparison(st)(lt)(rt), OpNotEq { } => infer_comparison(st)(lt)(rt), OpLt { } => infer_comparison(st)(lt)(rt), OpGt { } => infer_comparison(st)(lt)(rt), OpLtEq { } => infer_comparison(st)(lt)(rt), OpGtEq { } => infer_comparison(st)(lt)(rt), OpAnd { } => infer_logical(st)(lt)(rt), OpOr { } => infer_logical(st)(lt)(rt), OpAppend { } => infer_append(st)(lt)(rt), OpCons { } => infer_cons(st)(lt)(rt), OpDefEq { } => infer_comparison(st)(lt)(rt),  };

    public static object infer_arithmetic(object st, object lt, object rt) => ((object r = unify(st)(lt)(rt)) is var _ ? new CheckResult(inferred_type: lt, state: r.state) : default);

    public static object infer_comparison(object st, object lt, object rt) => ((object r = unify(st)(lt)(rt)) is var _ ? new CheckResult(inferred_type: BooleanTy, state: r.state) : default);

    public static object infer_logical(object st, object lt, object rt) => ((object r1 = unify(st)(lt)(BooleanTy)) is var _ ? ((object r2 = unify(r1.state)(rt)(BooleanTy)) is var _ ? new CheckResult(inferred_type: BooleanTy, state: r2.state) : default) : default);

    public static object infer_append(object st, object lt, object rt) => ((object resolved = resolve(st)(lt)) is var _ ? resolved switch { TextTy { } => ((object r = unify(st)(rt)(TextTy)) is var _ ? new CheckResult(inferred_type: TextTy, state: r.state) : default), _ => ((object r = unify(st)(lt)(rt)) is var _ ? new CheckResult(inferred_type: lt, state: r.state) : default),  } : default);

    public static object infer_cons(object st, object lt, object rt) => ((object list_ty = ListTy(lt)) is var _ ? ((object r = unify(st)(rt)(list_ty)) is var _ ? new CheckResult(inferred_type: list_ty, state: r.state) : default) : default);

    public static object infer_if(object st, object env, object cond, object then_e, object else_e) => ((object cr = infer_expr(st)(env)(cond)) is var _ ? ((object r1 = unify(cr.state)(cr.inferred_type)(BooleanTy)) is var _ ? ((object tr = infer_expr(r1.state)(env)(then_e)) is var _ ? ((object er = infer_expr(tr.state)(env)(else_e)) is var _ ? ((object r2 = unify(er.state)(tr.inferred_type)(er.inferred_type)) is var _ ? new CheckResult(inferred_type: tr.inferred_type, state: r2.state) : default) : default) : default) : default) : default);

    public static T3326 infer_let<T3326>(object st, object env, object bindings, object body) => ((object env2 = infer_let_bindings(st)(env)(bindings)(0)(list_length(bindings))) is var _ ? infer_expr(env2.state)(env2.env)(body) : default);

    public static object infer_let_bindings(object st, object env, object bindings, object i, object len) => ((i == len) ? new LetBindResult(state: st, env: env) : ((object b = list_at(bindings)(i)) is var _ ? ((object vr = infer_expr(st)(env)(b.value)) is var _ ? ((object env2 = env_bind(env)(b.name.value)(vr.inferred_type)) is var _ ? infer_let_bindings(vr.state)(env2)(bindings)((i + 1))(len) : default) : default) : default));

    public static object infer_lambda(object st, object env, object params, object body) => ((object pr = bind_lambda_params(st)(env)(params)(0)(list_length(params))(new List<object>())) is var _ ? ((object br = infer_expr(pr.state)(pr.env)(body)) is var _ ? ((object fun_ty = wrap_fun_type(pr.param_types)(br.inferred_type)) is var _ ? new CheckResult(inferred_type: fun_ty, state: br.state) : default) : default) : default);

    public static object bind_lambda_params(object st, object env, object params, object i, object len, object acc) => ((i == len) ? new LambdaBindResult(state: st, env: env, param_types: acc) : ((object p = list_at(params)(i)) is var _ ? ((object fr = fresh_and_advance(st)) is var _ ? ((object env2 = env_bind(env)(p.value)(fr.var_type)) is var _ ? bind_lambda_params(fr.state)(env2)(params)((i + 1))(len)((acc + new List<object> { fr.var_type })) : default) : default) : default));

    public static T3389 wrap_fun_type<T3389>(object param_types, object result) => wrap_fun_type_loop(param_types)(result)((list_length(param_types) - 1));

    public static T3400 wrap_fun_type_loop<T3400>(object param_types, object result, object i) => ((i < 0) ? result : wrap_fun_type_loop(param_types)(FunTy(list_at(param_types)(i))(result))((i - 1)));

    public static object infer_application(object st, object env, object func, object arg) => ((object fr = infer_expr(st)(env)(func)) is var _ ? ((object ar = infer_expr(fr.state)(env)(arg)) is var _ ? ((object ret = fresh_and_advance(ar.state)) is var _ ? ((object r = unify(ret.state)(fr.inferred_type)(FunTy(ar.inferred_type)(ret.var_type))) is var _ ? new CheckResult(inferred_type: ret.var_type, state: r.state) : default) : default) : default) : default);

    public static object infer_list(object st, object env, object elems) => ((list_length(elems) == 0) ? ((object fr = fresh_and_advance(st)) is var _ ? new CheckResult(inferred_type: ListTy(fr.var_type), state: fr.state) : default) : ((object first = infer_expr(st)(env)(list_at(elems)(0))) is var _ ? ((object st2 = unify_list_elems(first.state)(env)(elems)(first.inferred_type)(1)(list_length(elems))) is var _ ? new CheckResult(inferred_type: ListTy(first.inferred_type), state: st2) : default) : default));

    public static T3456 unify_list_elems<T3456>(object st, object env, object elems, object elem_ty, object i, object len) => ((i == len) ? st : ((object er = infer_expr(st)(env)(list_at(elems)(i))) is var _ ? ((object r = unify(er.state)(er.inferred_type)(elem_ty)) is var _ ? unify_list_elems(r.state)(env)(elems)(elem_ty)((i + 1))(len) : default) : default));

    public static object infer_match(object st, object env, object scrutinee, object arms) => ((object sr = infer_expr(st)(env)(scrutinee)) is var _ ? ((object fr = fresh_and_advance(sr.state)) is var _ ? ((object st2 = infer_match_arms(fr.state)(env)(sr.inferred_type)(fr.var_type)(arms)(0)(list_length(arms))) is var _ ? new CheckResult(inferred_type: fr.var_type, state: st2) : default) : default) : default);

    public static T3499 infer_match_arms<T3499>(object st, object env, object scrut_ty, object result_ty, object arms, object i, object len) => ((i == len) ? st : ((object arm = list_at(arms)(i)) is var _ ? ((object env2 = bind_pattern(env)(arm.pattern)(scrut_ty)) is var _ ? ((object br = infer_expr(st)(env2)(arm.body)) is var _ ? ((object r = unify(br.state)(br.inferred_type)(result_ty)) is var _ ? infer_match_arms(r.state)(env)(scrut_ty)(result_ty)(arms)((i + 1))(len) : default) : default) : default) : default));

    public static T3504 bind_pattern<T3504>(object env, object pat, object ty) => pat switch { AVarPat(var name) => env_bind(env)(name.value)(ty), AWildPat { } => env, _ => env,  };

    public static T3518 infer_do<T3518>(object st, object env, object stmts) => infer_do_loop(st)(env)(stmts)(0)(list_length(stmts))(NothingTy);

    public static object infer_do_loop(object st, object env, object stmts, object i, object len, object last_ty) => ((i == len) ? new CheckResult(inferred_type: last_ty, state: st) : ((object stmt = list_at(stmts)(i)) is var _ ? stmt switch { ADoExprStmt(var e) => ((object er = infer_expr(st)(env)(e)) is var _ ? infer_do_loop(er.state)(env)(stmts)((i + 1))(len)(er.inferred_type) : default), ADoBindStmt(var name, var e) => ((object er = infer_expr(st)(env)(e)) is var _ ? ((object env2 = env_bind(env)(name.value)(er.inferred_type)) is var _ ? infer_do_loop(er.state)(env2)(stmts)((i + 1))(len)(er.inferred_type) : default) : default),  } : default));

    public static object infer_expr(object st, object env, object expr) => expr switch { ALitExpr(var val, var kind) => infer_literal(st)(kind), ANameExpr(var name) => infer_name(st)(env)(name.value), ABinaryExpr(var left, var op, var right) => infer_binary(st)(env)(left)(op)(right), AUnaryExpr(var operand) => ((object r = infer_expr(st)(env)(operand)) is var _ ? ((object u = unify(r.state)(r.inferred_type)(IntegerTy)) is var _ ? new CheckResult(inferred_type: IntegerTy, state: u.state) : default) : default), AApplyExpr(var func, var arg) => infer_application(st)(env)(func)(arg), AIfExpr(var cond, var then_e, var else_e) => infer_if(st)(env)(cond)(then_e)(else_e), ALetExpr(var bindings, var body) => infer_let(st)(env)(bindings)(body), ALambdaExpr(var params, var body) => infer_lambda(st)(env)(params)(body), AMatchExpr(var scrutinee, var arms) => infer_match(st)(env)(scrutinee)(arms), AListExpr(var elems) => infer_list(st)(env)(elems), ADoExpr(var stmts) => infer_do(st)(env)(stmts), AFieldAccess(var obj, var field) => ((object r = infer_expr(st)(env)(obj)) is var _ ? new CheckResult(inferred_type: ErrorTy, state: r.state) : default), ARecordExpr(var name, var fields) => new CheckResult(inferred_type: ErrorTy, state: st), AErrorExpr(var msg) => new CheckResult(inferred_type: ErrorTy, state: st),  };

    public static T3603 resolve_type_expr<T3603>(object texpr) => texpr switch { ANamedType(var name) => resolve_type_name(name.value), AFunType(var param, var ret) => FunTy(resolve_type_expr(param))(resolve_type_expr(ret)), AAppType(var ctor, var args) => resolve_type_expr(ctor),  };

    public static object resolve_type_name(object name) => ((name == "Integer") ? IntegerTy : ((name == "Number") ? NumberTy : ((name == "Text") ? TextTy : ((name == "Boolean") ? BooleanTy : ((name == "Nothing") ? NothingTy : ConstructedTy(new Name(value: name))(new List<object>()))))));

    public static object check_def(object st, object env, object def) => ((object declared = resolve_declared_type(st)(def)) is var _ ? ((object env2 = bind_def_params(declared.state)(declared.env)(def.params)(declared.expected_type)(0)(list_length(def.params))) is var _ ? ((object body_r = infer_expr(env2.state)(env2.env)(def.body)) is var _ ? ((object u = unify(body_r.state)(env2.remaining_type)(body_r.inferred_type)) is var _ ? new CheckResult(inferred_type: declared.expected_type, state: u.state) : default) : default) : default) : default);

    public static object resolve_declared_type(object st, object def) => ((list_length(def.declared_type) == 0) ? ((object fr = fresh_and_advance(st)) is var _ ? new DefSetup(expected_type: fr.var_type, remaining_type: fr.var_type, state: fr.state, env: builtin_type_env) : default) : ((object ty = resolve_type_expr(list_at(def.declared_type)(0))) is var _ ? new DefSetup(expected_type: ty, remaining_type: ty, state: st, env: builtin_type_env) : default));

    public static object bind_def_params(object st, object env, object params, object remaining, object i, object len) => ((i == len) ? new DefParamResult(state: st, env: env, remaining_type: remaining) : ((object p = list_at(params)(i)) is var _ ? remaining switch { FunTy(var param_ty, var ret_ty) => ((object env2 = env_bind(env)(p.name.value)(param_ty)) is var _ ? bind_def_params(st)(env2)(params)(ret_ty)((i + 1))(len) : default), _ => ((object fr = fresh_and_advance(st)) is var _ ? ((object env2 = env_bind(env)(p.name.value)(fr.var_type)) is var _ ? bind_def_params(fr.state)(env2)(params)(remaining)((i + 1))(len) : default) : default),  } : default));

    public static T3686 check_module<T3686>(object mod) => ((object env = register_all_defs(empty_unification_state)(builtin_type_env)(mod.defs)(0)(list_length(mod.defs))) is var _ ? check_all_defs(env.state)(env.env)(mod.defs)(0)(list_length(mod.defs))(new List<object>()) : default);

    public static object register_all_defs(object st, object env, object defs, object i, object len) => ((i == len) ? new LetBindResult(state: st, env: env) : ((object def = list_at(defs)(i)) is var _ ? ((object ty = ((list_length(def.declared_type) == 0) ? ((object fr = fresh_and_advance(st)) is var _ ? ((object env2 = env_bind(env)(def.name.value)(fr.var_type)) is var _ ? new LetBindResult(state: fr.state, env: env2) : default) : default) : ((object resolved = resolve_type_expr(list_at(def.declared_type)(0))) is var _ ? new LetBindResult(state: st, env: env_bind(env)(def.name.value)(resolved)) : default))) is var _ ? register_all_defs(ty.state)(ty.env)(defs)((i + 1))(len) : default) : default));

    public static object check_all_defs(object st, object env, object defs, object i, object len, object acc) => ((i == len) ? new ModuleResult(types: acc, state: st) : ((object def = list_at(defs)(i)) is var _ ? ((object r = check_def(st)(env)(def)) is var _ ? ((object resolved = deep_resolve(r.state)(r.inferred_type)) is var _ ? ((object entry = new TypeBinding(name: def.name.value, bound_type: resolved)) is var _ ? check_all_defs(r.state)(env)(defs)((i + 1))(len)((acc + new List<object> { entry })) : default) : default) : default) : default));

    public static object empty_type_env() => new TypeEnv(bindings: new List<object>());

    public static T3736 env_lookup<T3736>(object env, object name) => env_lookup_loop(env.bindings)(name)(0)(list_length(env.bindings));

    public static object env_lookup_loop(object bindings, object name, object i, object len) => ((i == len) ? ErrorTy : ((object b = list_at(bindings)(i)) is var _ ? ((b.name == name) ? b.bound_type : env_lookup_loop(bindings)(name)((i + 1))(len)) : default));

    public static T3755 env_has<T3755>(object env, object name) => env_has_loop(env.bindings)(name)(0)(list_length(env.bindings));

    public static bool env_has_loop(object bindings, object name, object i, object len) => ((i == len) ? false : ((object b = list_at(bindings)(i)) is var _ ? ((b.name == name) ? true : env_has_loop(bindings)(name)((i + 1))(len)) : default));

    public static object env_bind(object env, object name, object ty) => new TypeEnv(bindings: (new List<object> { new TypeBinding(name: name, bound_type: ty) } + env.bindings));

    public static T3937 builtin_type_env<T3937>() => ((T3937 e = empty_type_env) is var _ ? ((T3937 e2 = env_bind(e)("negate")(FunTy(IntegerTy)(IntegerTy))) is var _ ? ((T3937 e3 = env_bind(e2)("text-length")(FunTy(TextTy)(IntegerTy))) is var _ ? ((T3937 e4 = env_bind(e3)("integer-to-text")(FunTy(IntegerTy)(TextTy))) is var _ ? ((T3937 e5 = env_bind(e4)("char-at")(FunTy(TextTy)(FunTy(IntegerTy)(TextTy)))) is var _ ? ((T3937 e6 = env_bind(e5)("substring")(FunTy(TextTy)(FunTy(IntegerTy)(FunTy(IntegerTy)(TextTy))))) is var _ ? ((T3937 e7 = env_bind(e6)("is-letter")(FunTy(TextTy)(BooleanTy))) is var _ ? ((T3937 e8 = env_bind(e7)("is-digit")(FunTy(TextTy)(BooleanTy))) is var _ ? ((T3937 e9 = env_bind(e8)("is-whitespace")(FunTy(TextTy)(BooleanTy))) is var _ ? ((T3937 e10 = env_bind(e9)("char-code")(FunTy(TextTy)(IntegerTy))) is var _ ? ((T3937 e11 = env_bind(e10)("code-to-char")(FunTy(IntegerTy)(TextTy))) is var _ ? ((T3937 e12 = env_bind(e11)("text-replace")(FunTy(TextTy)(FunTy(TextTy)(FunTy(TextTy)(TextTy))))) is var _ ? ((T3937 e13 = env_bind(e12)("text-to-integer")(FunTy(TextTy)(IntegerTy))) is var _ ? ((T3937 e14 = env_bind(e13)("show")(ForAllTy(0)(FunTy(TypeVar(0))(TextTy)))) is var _ ? ((T3937 e15 = env_bind(e14)("print-line")(FunTy(TextTy)(NothingTy))) is var _ ? ((T3937 e16 = env_bind(e15)("list-length")(ForAllTy(0)(FunTy(ListTy(TypeVar(0)))(IntegerTy)))) is var _ ? ((T3937 e17 = env_bind(e16)("list-at")(ForAllTy(0)(FunTy(ListTy(TypeVar(0)))(FunTy(IntegerTy)(TypeVar(0)))))) is var _ ? ((T3937 e18 = env_bind(e17)("map")(ForAllTy(0)(ForAllTy(1)(FunTy(FunTy(TypeVar(0))(TypeVar(1)))(FunTy(ListTy(TypeVar(0)))(ListTy(TypeVar(1)))))))) is var _ ? ((T3937 e19 = env_bind(e18)("filter")(ForAllTy(0)(FunTy(FunTy(TypeVar(0))(BooleanTy))(FunTy(ListTy(TypeVar(0)))(ListTy(TypeVar(0))))))) is var _ ? ((T3937 e20 = env_bind(e19)("fold")(ForAllTy(0)(ForAllTy(1)(FunTy(FunTy(TypeVar(1))(FunTy(TypeVar(0))(TypeVar(1))))(FunTy(TypeVar(1))(FunTy(ListTy(TypeVar(0)))(TypeVar(1)))))))) is var _ ? ((T3937 e21 = env_bind(e20)("read-line")(TextTy)) is var _ ? e21 : default) : default) : default) : default) : default) : default) : default) : default) : default) : default) : default) : default) : default) : default) : default) : default) : default) : default) : default) : default) : default);

    public static object empty_unification_state() => new UnificationState(substitutions: new List<object>(), next_id: 0, errors: new List<object>());

    public static T3941 fresh_var<T3941>(object st) => TypeVar(st.next_id);

    public static object advance_id(object st) => new UnificationState(substitutions: st.substitutions, next_id: (st.next_id + 1), errors: st.errors);

    public static object fresh_and_advance(object st) => new FreshResult(var_type: TypeVar(st.next_id), state: advance_id(st));

    public static T3953 subst_lookup<T3953>(object var_id, object entries) => subst_lookup_loop(var_id)(entries)(0)(list_length(entries));

    public static object subst_lookup_loop(object var_id, object entries, object i, object len) => ((i == len) ? ErrorTy : ((object entry = list_at(entries)(i)) is var _ ? ((entry.var_id == var_id) ? entry.resolved_type : subst_lookup_loop(var_id)(entries)((i + 1))(len)) : default));

    public static T3972 has_subst<T3972>(object var_id, object entries) => has_subst_loop(var_id)(entries)(0)(list_length(entries));

    public static bool has_subst_loop(object var_id, object entries, object i, object len) => ((i == len) ? false : ((object entry = list_at(entries)(i)) is var _ ? ((entry.var_id == var_id) ? true : has_subst_loop(var_id)(entries)((i + 1))(len)) : default));

    public static T3987 resolve<T3987>(object st, object ty) => ty switch { TypeVar(var id) => (has_subst(id)(st.substitutions) ? resolve(st)(subst_lookup(id)(st.substitutions)) : ty), _ => ty,  };

    public static object add_subst(object st, object var_id, object ty) => new UnificationState(substitutions: (st.substitutions + new List<object> { new SubstEntry(var_id: var_id, resolved_type: ty) }), next_id: st.next_id, errors: st.errors);

    public static object add_unify_error(object st, object code, object msg) => new UnificationState(substitutions: st.substitutions, next_id: st.next_id, errors: (st.errors + new List<object> { make_error(code)(msg) }));

    public static bool occurs_in(object st, object var_id, object ty) => ((object resolved = resolve(st)(ty)) is var _ ? resolved switch { TypeVar(var id) => (id == var_id), FunTy(var param, var ret) => (occurs_in(st)(var_id)(param) || occurs_in(st)(var_id)(ret)), ListTy(var elem) => occurs_in(st)(var_id)(elem), _ => false,  } : default);

    public static T4028 unify<T4028>(object st, object a, object b) => ((object ra = resolve(st)(a)) is var _ ? ((object rb = resolve(st)(b)) is var _ ? unify_resolved(st)(ra)(rb) : default) : default);

    public static object unify_resolved(object st, object a, object b) => a switch { TypeVar(var id_a) => (occurs_in(st)(id_a)(b) ? new UnifyResult(success: false, state: add_unify_error(st)("CDX2010")("Infinite type")) : new UnifyResult(success: true, state: add_subst(st)(id_a)(b))), _ => unify_rhs(st)(a)(b),  };

    public static object unify_rhs(object st, object a, object b) => b switch { TypeVar(var id_b) => (occurs_in(st)(id_b)(a) ? new UnifyResult(success: false, state: add_unify_error(st)("CDX2010")("Infinite type")) : new UnifyResult(success: true, state: add_subst(st)(id_b)(a))), _ => unify_structural(st)(a)(b),  };

    public static object unify_structural(object st, object a, object b) => a switch { IntegerTy { } => b switch { IntegerTy { } => new UnifyResult(success: true, state: st), _ => unify_mismatch(st)(a)(b), NumberTy { } => b switch { NumberTy { } => new UnifyResult(success: true, state: st), _ => unify_mismatch(st)(a)(b), TextTy { } => b switch { TextTy { } => new UnifyResult(success: true, state: st), _ => unify_mismatch(st)(a)(b), BooleanTy { } => b switch { BooleanTy { } => new UnifyResult(success: true, state: st), _ => unify_mismatch(st)(a)(b), NothingTy { } => b switch { NothingTy { } => new UnifyResult(success: true, state: st), _ => unify_mismatch(st)(a)(b), VoidTy { } => b switch { VoidTy { } => new UnifyResult(success: true, state: st), _ => unify_mismatch(st)(a)(b), ErrorTy { } => new UnifyResult(success: true, state: st), FunTy(var pa, var ra) => b switch { FunTy(var pb, var rb) => unify_fun(st)(pa)(ra)(pb)(rb), _ => unify_mismatch(st)(a)(b), ListTy(var ea) => b switch { ListTy(var eb) => unify(st)(ea)(eb), _ => unify_mismatch(st)(a)(b), _ => unify_mismatch(st)(a)(b),  },  },  },  },  },  },  },  },  };

    public static T4107 unify_fun<T4107>(object st, object pa, object ra, object pb, object rb) => ((object r1 = unify(st)(pa)(pb)) is var _ ? (r1.success ? unify(r1.state)(ra)(rb) : r1) : default);

    public static object unify_mismatch(object st, object a, object b) => new UnifyResult(success: false, state: add_unify_error(st)("CDX2001")("Type mismatch"));

    public static T4120 deep_resolve<T4120>(object st, object ty) => ((object resolved = resolve(st)(ty)) is var _ ? resolved switch { FunTy(var param, var ret) => FunTy(deep_resolve(st)(param))(deep_resolve(st)(ret)), ListTy(var elem) => ListTy(deep_resolve(st)(elem)), _ => resolved,  } : default);

    public static T4143 compile<T4143>(object source, object module_name) => ((object tokens = tokenize(source)) is var _ ? ((object st = make_parse_state(tokens)) is var _ ? ((object doc = parse_document(st)) is var _ ? ((object ast = desugar_document(doc)(module_name)) is var _ ? ((object check_result = check_module(ast)) is var _ ? ((object ir = lower_module(ast)(check_result.types)(check_result.state)) is var _ ? emit_full_module(ir)(ast.type_defs) : default) : default) : default) : default) : default) : default);

    public static T4162 compile_checked<T4162>(object source, object module_name) => ((object tokens = tokenize(source)) is var _ ? ((object st = make_parse_state(tokens)) is var _ ? ((object doc = parse_document(st)) is var _ ? ((object ast = desugar_document(doc)(module_name)) is var _ ? ((object resolve_result = resolve_module(ast)) is var _ ? ((list_length(resolve_result.errors) > 0) ? CompileError(resolve_result.errors) : ((object check_result = check_module(ast)) is var _ ? ((object ir = lower_module(ast)(check_result.types)(check_result.state)) is var _ ? CompileOk(emit_full_module(ir)(ast.type_defs))(check_result) : default) : default)) : default) : default) : default) : default) : default);

    public static string test_source() => "square : Integer -> Integer\\nsquare (x) = x * x\\nmain = square 5";

    public static object Console() => Nothing;

    public static object main() => { print_line(compile(test_source)("test"));  };

}
