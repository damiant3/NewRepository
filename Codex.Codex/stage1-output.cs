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

public sealed record ParamEntry(string param_name, long var_id);

public sealed record ParamResult(CodexType parameterized, List<ParamEntry> entries, UnificationState state);

public sealed record WalkResult(CodexType walked, List<ParamEntry> entries, UnificationState state);

public sealed record WalkListResult(List<CodexType> walked_list, List<ParamEntry> entries, UnificationState state);

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
    public static AExpr desugar_expr(Expr node) => node switch { LitExpr(var tok) => desugar_literal(tok), NameExpr(var tok) => new ANameExpr(make_name(tok.text)), AppExpr(var f, var a) => new AApplyExpr(desugar_expr(f), desugar_expr(a)), BinExpr(var l, var op, var r) => new ABinaryExpr(desugar_expr(l), desugar_bin_op(op.kind), desugar_expr(r)), UnaryExpr(var op, var operand) => new AUnaryExpr(desugar_expr(operand)), IfExpr(var c, var t, var e) => new AIfExpr(desugar_expr(c), desugar_expr(t), desugar_expr(e)), LetExpr(var bindings, var body) => new ALetExpr(map_list(desugar_let_bind, bindings), desugar_expr(body)), MatchExpr(var scrut, var arms) => new AMatchExpr(desugar_expr(scrut), map_list(desugar_match_arm, arms)), ListExpr(var elems) => new AListExpr(map_list(desugar_expr, elems)), RecordExpr(var type_tok, var fields) => new ARecordExpr(make_name(type_tok.text), map_list(desugar_field_expr, fields)), FieldExpr(var rec, var field_tok) => new AFieldAccess(desugar_expr(rec), make_name(field_tok.text)), ParenExpr(var inner) => desugar_expr(inner), DoExpr(var stmts) => new ADoExpr(map_list(desugar_do_stmt, stmts)), ErrExpr(var tok) => new AErrorExpr(tok.text),  };

    public static AExpr desugar_literal(Token tok) => (is_literal(tok.kind) ? new ALitExpr(tok.text, classify_literal(tok.kind)) : new AErrorExpr(tok.text));

    public static LiteralKind classify_literal(TokenKind k) => k switch { IntegerLiteral { } => IntLit, NumberLiteral { } => NumLit, TextLiteral { } => TextLit, TrueKeyword { } => BoolLit, FalseKeyword { } => BoolLit, _ => TextLit,  };

    public static ALetBind desugar_let_bind(LetBind b) => new ALetBind(name: make_name(b.name.text), value: desugar_expr(b.value));

    public static AMatchArm desugar_match_arm(MatchArm arm) => new AMatchArm(pattern: desugar_pattern(arm.pattern), body: desugar_expr(arm.body));

    public static AFieldExpr desugar_field_expr(RecordFieldExpr f) => new AFieldExpr(name: make_name(f.name.text), value: desugar_expr(f.value));

    public static ADoStmt desugar_do_stmt(DoStmt s) => s switch { DoBindStmt(var tok, var val) => new ADoBindStmt(make_name(tok.text), desugar_expr(val)), DoExprStmt(var e) => new ADoExprStmt(desugar_expr(e)),  };

    public static BinaryOp desugar_bin_op(TokenKind k) => k switch { Plus { } => OpAdd, Minus { } => OpSub, Star { } => OpMul, Slash { } => OpDiv, Caret { } => OpPow, DoubleEquals { } => OpEq, NotEquals { } => OpNotEq, LessThan { } => OpLt, GreaterThan { } => OpGt, LessOrEqual { } => OpLtEq, GreaterOrEqual { } => OpGtEq, TripleEquals { } => OpDefEq, PlusPlus { } => OpAppend, ColonColon { } => OpCons, Ampersand { } => OpAnd, Pipe { } => OpOr, _ => OpAdd,  };

    public static APat desugar_pattern(Pat p) => p switch { VarPat(var tok) => new AVarPat(make_name(tok.text)), LitPat(var tok) => new ALitPat(tok.text, classify_literal(tok.kind)), CtorPat(var tok, var subs) => new ACtorPat(make_name(tok.text), map_list(desugar_pattern, subs)), WildPat(var tok) => AWildPat,  };

    public static ATypeExpr desugar_type_expr(TypeExpr t) => t switch { NamedType(var tok) => new ANamedType(make_name(tok.text)), FunType(var param, var ret) => new AFunType(desugar_type_expr(param), desugar_type_expr(ret)), AppType(var ctor, var args) => new AAppType(desugar_type_expr(ctor), map_list(desugar_type_expr, args)), ParenType(var inner) => desugar_type_expr(inner), ListType(var elem) => new AAppType(new ANamedType(make_name("List")), new List<ATypeExpr> { desugar_type_expr(elem) }), LinearTypeExpr(var inner) => desugar_type_expr(inner),  };

    public static ADef desugar_def(Def d) => ((List<ATypeExpr> ann_types = desugar_annotations(d.ann)) is var _ ? new ADef(name: make_name(d.name.text), @params: map_list(desugar_param, d.@params), declared_type: ann_types, body: desugar_expr(d.body)) : default);

    public static List<ATypeExpr> desugar_annotations(List<TypeAnn> anns) => ((list_length(anns) == 0) ? new List<ATypeExpr>() : ((TypeAnn a = list_at(anns)(0)) is var _ ? new List<ATypeExpr> { desugar_type_expr(a.type_expr) } : default));

    public static AParam desugar_param(Token tok) => new AParam(name: make_name(tok.text));

    public static ATypeDef desugar_type_def(TypeDef td) => td.body switch { RecordBody(var fields) => new ARecordTypeDef(make_name(td.name.text), map_list(make_type_param_name, td.type_params), map_list(desugar_record_field_def, fields)), VariantBody(var ctors) => new AVariantTypeDef(make_name(td.name.text), map_list(make_type_param_name, td.type_params), map_list(desugar_variant_ctor_def, ctors)),  };

    public static Name make_type_param_name(Token tok) => make_name(tok.text);

    public static ARecordFieldDef desugar_record_field_def(RecordFieldDef f) => new ARecordFieldDef(name: make_name(f.name.text), type_expr: desugar_type_expr(f.type_expr));

    public static AVariantCtorDef desugar_variant_ctor_def(VariantCtorDef c) => new AVariantCtorDef(name: make_name(c.name.text), fields: map_list(desugar_type_expr, c.fields));

    public static AModule desugar_document(Document doc, string module_name) => new AModule(name: make_name(module_name), defs: map_list(desugar_def, doc.defs), type_defs: map_list(desugar_type_def, doc.type_defs));

    public static List<T191> map_list<T181, T191>(Func<T181, T191> f, List<T181> xs) => map_list_loop(f, xs, 0, list_length(xs), new List<T191>());

    public static List<T204> map_list_loop<T203, T204>(Func<T203, T204> f, List<T203> xs, long i, long len, List<T204> acc) => ((i == len) ? acc : map_list_loop(f, xs, (i + 1), len, (acc + new List<T204> { f(list_at(xs)(i)) })));

    public static T216 fold_list<T216, T207>(Func<T216, Func<T207, T216>> f, T216 z, List<T207> xs) => fold_list_loop(f, z, xs, 0, list_length(xs));

    public static T230 fold_list_loop<T230, T225>(Func<T230, Func<T225, T230>> f, T230 z, List<T225> xs, long i, long len) => ((i == len) ? z : fold_list_loop(f, f(z)(list_at(xs)(i)), xs, (i + 1), len));

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

    public static string sanitize(string name) => ((string s = text_replace(name)("-")("_")) is var _ ? (is_cs_keyword(s) ? ("@" + s) : s) : default);

    public static string cs_type(CodexType ty) => ty switch { IntegerTy { } => "long", NumberTy { } => "decimal", TextTy { } => "string", BooleanTy { } => "bool", VoidTy { } => "void", NothingTy { } => "object", ErrorTy { } => "object", FunTy(var p, var r) => ("Func<" + (cs_type(p) + (", " + (cs_type(r) + ">")))), ListTy(var elem) => ("List<" + (cs_type(elem) + ">")), TypeVar(var id) => ("T" + integer_to_text(id)), ForAllTy(var id, var body) => cs_type(body), SumTy(var name, var ctors) => sanitize(name.value), RecordTy(var name, var fields) => sanitize(name.value), ConstructedTy(var name, var args) => sanitize(name.value),  };

    public static List<ArityEntry> build_arity_map(List<IRDef> defs, long i) => ((i == list_length(defs)) ? new List<ArityEntry>() : ((IRDef d = list_at(defs)(i)) is var _ ? (new List<ArityEntry> { new ArityEntry(name: d.name, arity: list_length(d.@params)) } + build_arity_map(defs, (i + 1))) : default));

    public static long lookup_arity(List<ArityEntry> entries, string name) => lookup_arity_loop(entries, name, 0, list_length(entries));

    public static long lookup_arity_loop(List<ArityEntry> entries, string name, long i, long len) => ((i == len) ? (0 - 1) : ((ArityEntry e = list_at(entries)(i)) is var _ ? ((e.name == name) ? e.arity : lookup_arity_loop(entries, name, (i + 1), len)) : default));

    public static ApplyChain collect_apply_chain(IRExpr e, List<IRExpr> acc) => e switch { IrApply(var f, var a, var ty) => collect_apply_chain(f, (new List<IRExpr> { a } + acc)), _ => new ApplyChain(root: e, args: acc),  };

    public static bool is_upper_letter(string c) => ((long code = char_code(c)) is var _ ? ((code >= 65) && (code <= 90)) : default);

    public static string emit_apply_args(List<IRExpr> args, List<ArityEntry> arities, long i) => ((i == list_length(args)) ? "" : ((i == (list_length(args) - 1)) ? emit_expr(list_at(args)(i), arities) : (emit_expr(list_at(args)(i), arities) + (", " + emit_apply_args(args, arities, (i + 1))))));

    public static string emit_partial_params(long i, long count) => ((i == count) ? "" : ((i == (count - 1)) ? ("_p" + (integer_to_text(i) + "_")) : ("_p" + (integer_to_text(i) + ("_" + (", " + emit_partial_params((i + 1), count)))))));

    public static string emit_partial_wrappers(long i, long count) => ((i == count) ? "" : ("(_p" + (integer_to_text(i) + ("_) => " + emit_partial_wrappers((i + 1), count)))));

    public static string emit_apply(IRExpr e, List<ArityEntry> arities) => ((ApplyChain chain = collect_apply_chain(e, new List<IRExpr>())) is var _ ? ((IRExpr root = chain.root) is var _ ? ((List<IRExpr> args = chain.args) is var _ ? root switch { IrName(var n, var ty) => (((text_length(n) > 0) && is_upper_letter(char_at(n)(0))) ? ("new " + (sanitize(n) + ("(" + (emit_apply_args(args, arities, 0) + ")")))) : ((long ar = lookup_arity(arities, n)) is var _ ? (((ar > 1) && (list_length(args) == ar)) ? (sanitize(n) + ("(" + (emit_apply_args(args, arities, 0) + ")"))) : (((ar > 1) && (list_length(args) < ar)) ? ((object remaining = (ar - list_length(args))) is var _ ? (emit_partial_wrappers(0, remaining) + (sanitize(n) + ("(" + (emit_apply_args(args, arities, 0) + (", " + (emit_partial_params(0, remaining) + ")")))))) : default) : emit_expr_curried(e, arities))) : default)), _ => emit_expr_curried(e, arities),  } : default) : default) : default);

    public static string emit_expr_curried(IRExpr e, List<ArityEntry> arities) => e switch { IrApply(var f, var a, var ty) => (emit_expr(f, arities) + ("(" + (emit_expr(a, arities) + ")"))), _ => emit_expr(e, arities),  };

    public static string emit_expr(IRExpr e, List<ArityEntry> arities) => e switch { IrIntLit(var n) => integer_to_text(n), IrNumLit(var n) => integer_to_text(n), IrTextLit(var s) => ("\\\"" + (escape_text(s) + "\\\"")), IrBoolLit(var b) => (b ? "true" : "false"), IrName(var n, var ty) => sanitize(n), IrBinary(var op, var l, var r, var ty) => ("(" + (emit_expr(l, arities) + (" " + (emit_bin_op(op) + (" " + (emit_expr(r, arities) + ")")))))), IrNegate(var operand) => ("(-" + (emit_expr(operand, arities) + ")")), IrIf(var c, var t, var el, var ty) => ("(" + (emit_expr(c, arities) + (" ? " + (emit_expr(t, arities) + (" : " + (emit_expr(el, arities) + ")")))))), IrLet(var name, var ty, var val, var body) => emit_let(name, ty, val, body, arities), IrApply(var f, var a, var ty) => emit_apply(e, arities), IrLambda(var @params, var body, var ty) => emit_lambda(@params, body, arities), IrList(var elems, var ty) => emit_list(elems, ty, arities), IrMatch(var scrut, var branches, var ty) => emit_match(scrut, branches, ty, arities), IrDo(var stmts, var ty) => emit_do(stmts, arities), IrRecord(var name, var fields, var ty) => emit_record(name, fields, arities), IrFieldAccess(var rec, var field, var ty) => (emit_expr(rec, arities) + ("." + sanitize(field))), IrError(var msg, var ty) => ("/* error: " + (msg + " */ default")),  };

    public static string escape_text(string s) => text_replace(text_replace(s)("\\\\")("\\\\\\\\"))("\\\"")("\\\\\\\"");

    public static string emit_bin_op(IRBinaryOp op) => op switch { IrAddInt { } => "+", IrSubInt { } => "-", IrMulInt { } => "*", IrDivInt { } => "/", IrPowInt { } => "^", IrAddNum { } => "+", IrSubNum { } => "-", IrMulNum { } => "*", IrDivNum { } => "/", IrEq { } => "==", IrNotEq { } => "!=", IrLt { } => "<", IrGt { } => ">", IrLtEq { } => "<=", IrGtEq { } => ">=", IrAnd { } => "&&", IrOr { } => "||", IrAppendText { } => "+", IrAppendList { } => "+", IrConsList { } => "+",  };

    public static string emit_let(string name, CodexType ty, IRExpr val, IRExpr body, List<ArityEntry> arities) => ("((" + (cs_type(ty) + (" " + (sanitize(name) + (" = " + (emit_expr(val, arities) + (") is var _ ? " + (emit_expr(body, arities) + " : default)"))))))));

    public static string emit_lambda(List<IRParam> @params, IRExpr body, List<ArityEntry> arities) => ((list_length(@params) == 0) ? ("(() => " + (emit_expr(body, arities) + ")")) : ((list_length(@params) == 1) ? ((IRParam p = list_at(@params)(0)) is var _ ? ("((" + (cs_type(p.type_val) + (" " + (sanitize(p.name) + (") => " + (emit_expr(body, arities) + ")")))))) : default) : ("(() => " + (emit_expr(body, arities) + ")"))));

    public static string emit_list(List<IRExpr> elems, CodexType ty, List<ArityEntry> arities) => ((list_length(elems) == 0) ? ("new List<" + (cs_type(ty) + ">()")) : ("new List<" + (cs_type(ty) + ("> { " + (emit_list_elems(elems, 0, arities) + " }")))));

    public static string emit_list_elems(List<IRExpr> elems, long i, List<ArityEntry> arities) => ((i == list_length(elems)) ? "" : ((i == (list_length(elems) - 1)) ? emit_expr(list_at(elems)(i), arities) : (emit_expr(list_at(elems)(i), arities) + (", " + emit_list_elems(elems, (i + 1), arities)))));

    public static string emit_match(IRExpr scrut, List<IRBranch> branches, CodexType ty, List<ArityEntry> arities) => (emit_expr(scrut, arities) + (" switch { " + (emit_match_arms(branches, 0, arities) + " }")));

    public static string emit_match_arms(List<IRBranch> branches, long i, List<ArityEntry> arities) => ((i == list_length(branches)) ? "" : ((IRBranch arm = list_at(branches)(i)) is var _ ? (emit_pattern(arm.pattern) + (" => " + (emit_expr(arm.body, arities) + (", " + emit_match_arms(branches, (i + 1), arities))))) : default));

    public static string emit_pattern(IRPat p) => p switch { IrVarPat(var name, var ty) => (cs_type(ty) + (" " + sanitize(name))), IrLitPat(var text, var ty) => text, IrCtorPat(var name, var subs, var ty) => ((list_length(subs) == 0) ? (sanitize(name) + " { }") : (sanitize(name) + ("(" + (emit_sub_patterns(subs, 0) + ")")))), IrWildPat { } => "_",  };

    public static string emit_sub_patterns(List<IRPat> subs, long i) => ((i == list_length(subs)) ? "" : ((IRPat sub = list_at(subs)(i)) is var _ ? (emit_sub_pattern(sub) + (((i < (list_length(subs) - 1)) ? ", " : "") + emit_sub_patterns(subs, (i + 1)))) : default));

    public static string emit_sub_pattern(IRPat p) => p switch { IrVarPat(var name, var ty) => ("var " + sanitize(name)), IrCtorPat(var name, var subs, var ty) => emit_pattern(p), IrWildPat { } => "_", IrLitPat(var text, var ty) => text,  };

    public static string emit_do(List<IRDoStmt> stmts, List<ArityEntry> arities) => ("{ " + (emit_do_stmts(stmts, 0, arities) + " }"));

    public static string emit_do_stmts(List<IRDoStmt> stmts, long i, List<ArityEntry> arities) => ((i == list_length(stmts)) ? "" : ((IRDoStmt s = list_at(stmts)(i)) is var _ ? (emit_do_stmt(s, arities) + (" " + emit_do_stmts(stmts, (i + 1), arities))) : default));

    public static string emit_do_stmt(IRDoStmt s, List<ArityEntry> arities) => s switch { IrDoBind(var name, var ty, var val) => ("var " + (sanitize(name) + (" = " + (emit_expr(val, arities) + ";")))), IrDoExec(var e) => (emit_expr(e, arities) + ";"),  };

    public static string emit_record(string name, List<IRFieldVal> fields, List<ArityEntry> arities) => ("new " + (sanitize(name) + ("(" + (emit_record_fields(fields, 0, arities) + ")"))));

    public static string emit_record_fields(List<IRFieldVal> fields, long i, List<ArityEntry> arities) => ((i == list_length(fields)) ? "" : ((IRFieldVal f = list_at(fields)(i)) is var _ ? (sanitize(f.name) + (": " + (emit_expr(f.value, arities) + (((i < (list_length(fields) - 1)) ? ", " : "") + emit_record_fields(fields, (i + 1), arities))))) : default));

    public static string emit_type_defs(List<ATypeDef> tds, long i) => ((i == list_length(tds)) ? "" : (emit_type_def(list_at(tds)(i)) + ("\\n" + emit_type_defs(tds, (i + 1)))));

    public static string emit_type_def(ATypeDef td) => td switch { ARecordTypeDef(var name, var tparams, var fields) => ((string gen = emit_tparam_suffix(tparams)) is var _ ? ("public sealed record " + (sanitize(name.value) + (gen + ("(" + (emit_record_field_defs(fields, tparams, 0) + ");\\n"))))) : default), AVariantTypeDef(var name, var tparams, var ctors) => ((string gen = emit_tparam_suffix(tparams)) is var _ ? ("public abstract record " + (sanitize(name.value) + (gen + (";\\n" + (emit_variant_ctors(ctors, name, tparams, 0) + "\\n"))))) : default),  };

    public static string emit_tparam_suffix(List<Name> tparams) => ((list_length(tparams) == 0) ? "" : ("<" + (emit_tparam_names(tparams, 0) + ">")));

    public static string emit_tparam_names(List<Name> tparams, long i) => ((i == list_length(tparams)) ? "" : ((i == (list_length(tparams) - 1)) ? ("T" + integer_to_text(i)) : ("T" + (integer_to_text(i) + (", " + emit_tparam_names(tparams, (i + 1)))))));

    public static string emit_record_field_defs(List<ARecordFieldDef> fields, List<Name> tparams, long i) => ((i == list_length(fields)) ? "" : ((ARecordFieldDef f = list_at(fields)(i)) is var _ ? (emit_type_expr_tp(f.type_expr, tparams) + (" " + (sanitize(f.name.value) + (((i < (list_length(fields) - 1)) ? ", " : "") + emit_record_field_defs(fields, tparams, (i + 1)))))) : default));

    public static string emit_variant_ctors(List<AVariantCtorDef> ctors, Name base_name, List<Name> tparams, long i) => ((i == list_length(ctors)) ? "" : ((AVariantCtorDef c = list_at(ctors)(i)) is var _ ? (emit_variant_ctor(c, base_name, tparams) + emit_variant_ctors(ctors, base_name, tparams, (i + 1))) : default));

    public static string emit_variant_ctor(AVariantCtorDef c, Name base_name, List<Name> tparams) => ((string gen = emit_tparam_suffix(tparams)) is var _ ? ((list_length(c.fields) == 0) ? ("public sealed record " + (sanitize(c.name.value) + (gen + (" : " + (sanitize(base_name.value) + (gen + ";\\n")))))) : ("public sealed record " + (sanitize(c.name.value) + (gen + ("(" + (emit_ctor_fields(c.fields, tparams, 0) + (") : " + (sanitize(base_name.value) + (gen + ";\\n"))))))))) : default);

    public static string emit_ctor_fields(List<ATypeExpr> fields, List<Name> tparams, long i) => ((i == list_length(fields)) ? "" : (emit_type_expr_tp(list_at(fields)(i), tparams) + (" Field" + (integer_to_text(i) + (((i < (list_length(fields) - 1)) ? ", " : "") + emit_ctor_fields(fields, tparams, (i + 1)))))));

    public static string emit_type_expr(ATypeExpr te) => emit_type_expr_tp(te, new List<Name>());

    public static string emit_type_expr_tp(ATypeExpr te, List<Name> tparams) => te switch { ANamedType(var name) => ((long idx = find_tparam_index(tparams, name.value, 0)) is var _ ? ((idx >= 0) ? ("T" + integer_to_text(idx)) : when_type_name(name.value)) : default), AFunType(var p, var r) => ("Func<" + (emit_type_expr_tp(p, tparams) + (", " + (emit_type_expr_tp(r, tparams) + ">")))), AAppType(var @base, var args) => (emit_type_expr_tp(@base, tparams) + ("<" + (emit_type_expr_list_tp(args, tparams, 0) + ">"))),  };

    public static long find_tparam_index(List<Name> tparams, string name, long i) => ((i == list_length(tparams)) ? (0 - 1) : ((list_at(tparams)(i).value == name) ? i : find_tparam_index(tparams, name, (i + 1))));

    public static string when_type_name(string n) => ((n == "Integer") ? "long" : ((n == "Number") ? "decimal" : ((n == "Text") ? "string" : ((n == "Boolean") ? "bool" : ((n == "List") ? "List" : sanitize(n))))));

    public static string emit_type_expr_list(List<ATypeExpr> args, long i) => ((i == list_length(args)) ? "" : (emit_type_expr(list_at(args)(i)) + (((i < (list_length(args) - 1)) ? ", " : "") + emit_type_expr_list(args, (i + 1)))));

    public static string emit_type_expr_list_tp(List<ATypeExpr> args, List<Name> tparams, long i) => ((i == list_length(args)) ? "" : (emit_type_expr_tp(list_at(args)(i), tparams) + (((i < (list_length(args) - 1)) ? ", " : "") + emit_type_expr_list_tp(args, tparams, (i + 1)))));

    public static List<long> collect_type_var_ids(CodexType ty, List<long> acc) => ty switch { TypeVar(var id) => (list_contains_int(acc, id) ? acc : list_append_int(acc, id)), FunTy(var p, var r) => collect_type_var_ids(r, collect_type_var_ids(p, acc)), ListTy(var elem) => collect_type_var_ids(elem, acc), ForAllTy(var id, var body) => collect_type_var_ids(body, acc), ConstructedTy(var name, var args) => collect_type_var_ids_list(args, acc), _ => acc,  };

    public static List<long> collect_type_var_ids_list(List<CodexType> types, List<long> acc) => collect_type_var_ids_list_loop(types, acc, 0, list_length(types));

    public static List<long> collect_type_var_ids_list_loop(List<CodexType> types, List<long> acc, long i, long len) => ((i == len) ? acc : collect_type_var_ids_list_loop(types, collect_type_var_ids(list_at(types)(i), acc), (i + 1), len));

    public static bool list_contains_int(List<long> xs, long n) => list_contains_int_loop(xs, n, 0, list_length(xs));

    public static bool list_contains_int_loop(List<long> xs, long n, long i, long len) => ((i == len) ? false : ((list_at(xs)(i) == n) ? true : list_contains_int_loop(xs, n, (i + 1), len)));

    public static List<long> list_append_int(List<long> xs, long n) => (xs + new List<long> { n });

    public static string generic_suffix(CodexType ty) => ((List<long> ids = collect_type_var_ids(ty, new List<long>())) is var _ ? ((list_length(ids) == 0) ? "" : ("<" + (emit_type_params(ids, 0) + ">"))) : default);

    public static string emit_type_params(List<long> ids, long i) => ((i == list_length(ids)) ? "" : ((i == (list_length(ids) - 1)) ? ("T" + integer_to_text(list_at(ids)(i))) : ("T" + (integer_to_text(list_at(ids)(i)) + (", " + emit_type_params(ids, (i + 1)))))));

    public static string emit_def(IRDef d, List<ArityEntry> arities) => ((CodexType ret = get_return_type(d.type_val, list_length(d.@params))) is var _ ? ((string gen = generic_suffix(d.type_val)) is var _ ? ("    public static " + (cs_type(ret) + (" " + (sanitize(d.name) + (gen + ("(" + (emit_def_params(d.@params, 0) + (") => " + (emit_expr(d.body, arities) + ";\\n"))))))))) : default) : default);

    public static CodexType get_return_type(CodexType ty, long n) => ((n == 0) ? strip_forall(ty) : strip_forall(ty) switch { FunTy(var p, var r) => get_return_type(r, (n - 1)), _ => ty,  });

    public static CodexType strip_forall(CodexType ty) => ty switch { ForAllTy(var id, var body) => strip_forall(body), _ => ty,  };

    public static string emit_def_params(List<IRParam> @params, long i) => ((i == list_length(@params)) ? "" : ((IRParam p = list_at(@params)(i)) is var _ ? (cs_type(p.type_val) + (" " + (sanitize(p.name) + (((i < (list_length(@params) - 1)) ? ", " : "") + emit_def_params(@params, (i + 1)))))) : default));

    public static string emit_full_module(IRModule m, List<ATypeDef> type_defs) => ((List<ArityEntry> arities = build_arity_map(m.defs, 0)) is var _ ? ("using System;\\nusing System.Collections.Generic;\\nusing System.Linq;\\n\\n" + (emit_type_defs(type_defs, 0) + (emit_class_header(m.name.value) + (emit_defs(m.defs, 0, arities) + "}\\n")))) : default);

    public static string emit_module(IRModule m) => ((List<ArityEntry> arities = build_arity_map(m.defs, 0)) is var _ ? ("using System;\\nusing System.Collections.Generic;\\nusing System.Linq;\\n\\n" + (emit_class_header(m.name.value) + (emit_defs(m.defs, 0, arities) + "}\\n"))) : default);

    public static string emit_class_header(string name) => ("public static class Codex_" + (sanitize(name) + "\\n{\\n"));

    public static string emit_defs(List<IRDef> defs, long i, List<ArityEntry> arities) => ((i == list_length(defs)) ? "" : (emit_def(list_at(defs)(i), arities) + ("\\n" + emit_defs(defs, (i + 1), arities))));

    public static CodexType ir_expr_type(IRExpr e) => e switch { IrIntLit(var v) => IntegerTy, IrNumLit(var v) => IntegerTy, IrTextLit(var v) => TextTy, IrBoolLit(var v) => BooleanTy, IrName(var n, var t) => t, IrBinary(var op, var l, var r, var t) => t, IrNegate(var x) => IntegerTy, IrIf(var c, var th, var el, var t) => t, IrLet(var n, var t, var v, var b) => ir_expr_type(b), IrApply(var f, var a, var t) => t, IrLambda(var ps, var b, var t) => t, IrList(var es, var t) => new ListTy(t), IrMatch(var s, var bs, var t) => t, IrDo(var ss, var t) => t, IrRecord(var n, var fs, var t) => t, IrFieldAccess(var r, var f, var t) => t, IrError(var m, var t) => t,  };

    public static CodexType lookup_type(List<TypeBinding> bindings, string name) => lookup_type_loop(bindings, name, 0, list_length(bindings));

    public static CodexType lookup_type_loop(List<TypeBinding> bindings, string name, long i, long len) => ((i == len) ? ErrorTy : ((TypeBinding b = list_at(bindings)(i)) is var _ ? ((b.name == name) ? b.bound_type : lookup_type_loop(bindings, name, (i + 1), len)) : default));

    public static CodexType peel_fun_param(CodexType ty) => ty switch { FunTy(var p, var r) => p, ForAllTy(var id, var body) => peel_fun_param(body), _ => ErrorTy,  };

    public static CodexType peel_fun_return(CodexType ty) => ty switch { FunTy(var p, var r) => r, ForAllTy(var id, var body) => peel_fun_return(body), _ => ErrorTy,  };

    public static CodexType strip_forall_ty(CodexType ty) => ty switch { ForAllTy(var id, var body) => strip_forall_ty(body), _ => ty,  };

    public static CodexType subst_type_vars_from_arg(CodexType param_ty, CodexType arg_ty, CodexType target) => param_ty switch { TypeVar(var id) => subst_type_var_in_target(target, id, arg_ty), ListTy(var pe) => subst_from_list(pe, arg_ty, target), FunTy(var pp, var pr) => subst_from_fun(pp, pr, arg_ty, target), _ => target,  };

    public static CodexType subst_from_list(CodexType pe, CodexType arg_ty, CodexType target) => arg_ty switch { ListTy(var ae) => subst_type_vars_from_arg(pe, ae, target), _ => target,  };

    public static CodexType subst_from_fun(CodexType pp, CodexType pr, CodexType arg_ty, CodexType target) => arg_ty switch { FunTy(var ap, var ar) => ((CodexType t2 = subst_type_vars_from_arg(pp, ap, target)) is var _ ? subst_type_vars_from_arg(pr, ar, t2) : default), _ => target,  };

    public static CodexType subst_type_var_in_target(CodexType ty, long var_id, CodexType replacement) => ty switch { TypeVar(var id) => ((id == var_id) ? replacement : ty), FunTy(var p, var r) => new FunTy(subst_type_var_in_target(p, var_id, replacement), subst_type_var_in_target(r, var_id, replacement)), ListTy(var elem) => new ListTy(subst_type_var_in_target(elem, var_id, replacement)), ForAllTy(var fid, var body) => ((fid == var_id) ? ty : new ForAllTy(fid, subst_type_var_in_target(body, var_id, replacement))), _ => ty,  };

    public static IRBinaryOp lower_bin_op(BinaryOp op, CodexType ty) => op switch { OpAdd { } => IrAddInt, OpSub { } => IrSubInt, OpMul { } => IrMulInt, OpDiv { } => IrDivInt, OpPow { } => IrPowInt, OpEq { } => IrEq, OpNotEq { } => IrNotEq, OpLt { } => IrLt, OpGt { } => IrGt, OpLtEq { } => IrLtEq, OpGtEq { } => IrGtEq, OpDefEq { } => IrEq, OpAppend { } => (is_text_type(ty) ? IrAppendText : IrAppendList), OpCons { } => IrConsList, OpAnd { } => IrAnd, OpOr { } => IrOr,  };

    public static bool is_text_type(CodexType ty) => ty switch { TextTy { } => true, _ => false,  };

    public static IRExpr lower_expr(AExpr e, CodexType ty, LowerCtx ctx) => e switch { ALitExpr(var text, var kind) => lower_literal(text, kind), ANameExpr(var name) => lower_name(name.value, ty, ctx), AApplyExpr(var f, var a) => lower_apply(f, a, ty, ctx), ABinaryExpr(var l, var op, var r) => ((IRExpr left_ir = lower_expr(l, ty, ctx)) is var _ ? ((IRExpr right_ir = lower_expr(r, ty, ctx)) is var _ ? new IrBinary(lower_bin_op(op, ir_expr_type(left_ir)), left_ir, right_ir, ty) : default) : default), AUnaryExpr(var operand) => new IrNegate(lower_expr(operand, IntegerTy, ctx)), AIfExpr(var c, var t, var e2) => new IrIf(lower_expr(c, BooleanTy, ctx), lower_expr(t, ty, ctx), lower_expr(e2, ty, ctx), ty), ALetExpr(var binds, var body) => lower_let(binds, body, ty, ctx), ALambdaExpr(var @params, var body) => lower_lambda(@params, body, ty, ctx), AMatchExpr(var scrut, var arms) => lower_match(scrut, arms, ty, ctx), AListExpr(var elems) => lower_list(elems, ty, ctx), ARecordExpr(var name, var fields) => lower_record(name, fields, ty, ctx), AFieldAccess(var rec, var field) => ((IRExpr rec_ir = lower_expr(rec, ErrorTy, ctx)) is var _ ? ((CodexType rec_ty = deep_resolve(ctx.ust, ir_expr_type(rec_ir))) is var _ ? ((object field_ty = rec_ty switch { RecordTy(var rname, var rfields) => lookup_record_field(rfields, field.value), _ => ty,  }) is var _ ? ((object actual_field_ty = field_ty switch { ErrorTy { } => ty, _ => field_ty,  }) is var _ ? new IrFieldAccess(rec_ir, field.value, actual_field_ty) : default) : default) : default) : default), ADoExpr(var stmts) => lower_do(stmts, ty, ctx), AErrorExpr(var msg) => new IrError(msg, ty),  };

    public static IRExpr lower_name(string name, CodexType ty, LowerCtx ctx) => ((CodexType raw = lookup_type(ctx.types, name)) is var _ ? raw switch { ErrorTy { } => new IrName(name, ty), _ => ((CodexType resolved = deep_resolve(ctx.ust, raw)) is var _ ? ((CodexType stripped = strip_forall_ty(resolved)) is var _ ? new IrName(name, stripped) : default) : default),  } : default);

    public static IRExpr lower_literal(string text, LiteralKind kind) => kind switch { IntLit { } => new IrIntLit(text_to_integer(text)), NumLit { } => new IrIntLit(text_to_integer(text)), TextLit { } => new IrTextLit(text), BoolLit { } => new IrBoolLit((text == "True")),  };

    public static IRExpr lower_apply(AExpr f, AExpr a, CodexType ty, LowerCtx ctx) => ((IRExpr func_ir = lower_expr(f, ErrorTy, ctx)) is var _ ? ((CodexType func_ty = deep_resolve(ctx.ust, ir_expr_type(func_ir))) is var _ ? ((CodexType arg_ty = peel_fun_param(func_ty)) is var _ ? ((CodexType ret_ty = peel_fun_return(func_ty)) is var _ ? ((IRExpr arg_ir = lower_expr(a, arg_ty, ctx)) is var _ ? ((CodexType resolved_ret = subst_type_vars_from_arg(arg_ty, ir_expr_type(arg_ir), ret_ty)) is var _ ? ((object actual_ret = resolved_ret switch { ErrorTy { } => ty, _ => resolved_ret,  }) is var _ ? new IrApply(func_ir, arg_ir, actual_ret) : default) : default) : default) : default) : default) : default) : default);

    public static IRExpr lower_let(List<ALetBind> binds, AExpr body, CodexType ty, LowerCtx ctx) => ((list_length(binds) == 0) ? lower_expr(body, ty, ctx) : ((ALetBind b = list_at(binds)(0)) is var _ ? ((IRExpr val_ir = lower_expr(b.value, ErrorTy, ctx)) is var _ ? ((CodexType val_ty = deep_resolve(ctx.ust, ir_expr_type(val_ir))) is var _ ? ((LowerCtx ctx2 = new LowerCtx(types: (new List<TypeBinding> { new TypeBinding(name: b.name.value, bound_type: val_ty) } + ctx.types), ust: ctx.ust)) is var _ ? new IrLet(b.name.value, val_ty, val_ir, lower_let_rest(binds, body, ty, ctx2, 1)) : default) : default) : default) : default));

    public static IRExpr lower_let_rest(List<ALetBind> binds, AExpr body, CodexType ty, LowerCtx ctx, long i) => ((i == list_length(binds)) ? lower_expr(body, ty, ctx) : ((ALetBind b = list_at(binds)(i)) is var _ ? ((IRExpr val_ir = lower_expr(b.value, ErrorTy, ctx)) is var _ ? ((CodexType val_ty = deep_resolve(ctx.ust, ir_expr_type(val_ir))) is var _ ? ((LowerCtx ctx2 = new LowerCtx(types: (new List<TypeBinding> { new TypeBinding(name: b.name.value, bound_type: val_ty) } + ctx.types), ust: ctx.ust)) is var _ ? new IrLet(b.name.value, val_ty, val_ir, lower_let_rest(binds, body, ty, ctx2, (i + 1))) : default) : default) : default) : default));

    public static IRExpr lower_lambda(List<Name> @params, AExpr body, CodexType ty, LowerCtx ctx) => ((CodexType stripped = strip_forall_ty(ty)) is var _ ? ((List<IRParam> lparams = lower_lambda_params(@params, stripped, 0)) is var _ ? ((LowerCtx lctx = bind_lambda_to_ctx(ctx, @params, stripped, 0)) is var _ ? new IrLambda(lparams, lower_expr(body, get_lambda_return(stripped, list_length(@params)), lctx), ty) : default) : default) : default);

    public static LowerCtx bind_lambda_to_ctx(LowerCtx ctx, List<Name> @params, CodexType ty, long i) => ((i == list_length(@params)) ? ctx : ((Name p = list_at(@params)(i)) is var _ ? ((CodexType param_ty = peel_fun_param(ty)) is var _ ? ((CodexType rest_ty = peel_fun_return(ty)) is var _ ? ((LowerCtx ctx2 = new LowerCtx(types: (new List<TypeBinding> { new TypeBinding(name: p.value, bound_type: param_ty) } + ctx.types), ust: ctx.ust)) is var _ ? bind_lambda_to_ctx(ctx2, @params, rest_ty, (i + 1)) : default) : default) : default) : default));

    public static List<IRParam> lower_lambda_params(List<Name> @params, CodexType ty, long i) => ((i == list_length(@params)) ? new List<IRParam>() : ((Name p = list_at(@params)(i)) is var _ ? ((CodexType param_ty = peel_fun_param(ty)) is var _ ? ((CodexType rest_ty = peel_fun_return(ty)) is var _ ? (new List<IRParam> { new IRParam(name: p.value, type_val: param_ty) } + lower_lambda_params(@params, rest_ty, (i + 1))) : default) : default) : default));

    public static CodexType get_lambda_return(CodexType ty, long n) => ((n == 0) ? ty : ty switch { FunTy(var p, var r) => get_lambda_return(r, (n - 1)), _ => ErrorTy,  });

    public static IRExpr lower_match(AExpr scrut, List<AMatchArm> arms, CodexType ty, LowerCtx ctx) => ((IRExpr scrut_ir = lower_expr(scrut, ErrorTy, ctx)) is var _ ? ((CodexType scrut_ty = ir_expr_type(scrut_ir)) is var _ ? new IrMatch(scrut_ir, lower_match_arms_loop(arms, ty, scrut_ty, ctx, 0, list_length(arms)), ty) : default) : default);

    public static List<IRBranch> lower_match_arms_loop(List<AMatchArm> arms, CodexType ty, CodexType scrut_ty, LowerCtx ctx, long i, long len) => ((i == len) ? new List<IRBranch>() : ((AMatchArm arm = list_at(arms)(i)) is var _ ? ((LowerCtx arm_ctx = bind_pattern_to_ctx(ctx, arm.pattern, scrut_ty)) is var _ ? (new List<IRBranch> { new IRBranch(pattern: lower_pattern(arm.pattern), body: lower_expr(arm.body, ty, arm_ctx)) } + lower_match_arms_loop(arms, ty, scrut_ty, ctx, (i + 1), len)) : default) : default));

    public static LowerCtx bind_pattern_to_ctx(LowerCtx ctx, APat pat, CodexType ty) => pat switch { AVarPat(var name) => new LowerCtx(types: (new List<TypeBinding> { new TypeBinding(name: name.value, bound_type: ty) } + ctx.types), ust: ctx.ust), ACtorPat(var ctor_name, var sub_pats) => ((CodexType ctor_raw = lookup_type(ctx.types, ctor_name.value)) is var _ ? ((CodexType ctor_ty = deep_resolve(ctx.ust, ctor_raw)) is var _ ? ((CodexType ctor_stripped = strip_forall_ty(ctor_ty)) is var _ ? bind_ctor_pattern_fields(ctx, sub_pats, ctor_stripped, 0, list_length(sub_pats)) : default) : default) : default), AWildPat { } => ctx, ALitPat(var text, var kind) => ctx,  };

    public static LowerCtx bind_ctor_pattern_fields(LowerCtx ctx, List<APat> sub_pats, CodexType ctor_ty, long i, long len) => ((i == len) ? ctx : ctor_ty switch { FunTy(var param_ty, var ret_ty) => ((LowerCtx ctx2 = bind_pattern_to_ctx(ctx, list_at(sub_pats)(i), param_ty)) is var _ ? bind_ctor_pattern_fields(ctx2, sub_pats, ret_ty, (i + 1), len) : default), _ => ((LowerCtx ctx2 = bind_pattern_to_ctx(ctx, list_at(sub_pats)(i), ErrorTy)) is var _ ? bind_ctor_pattern_fields(ctx2, sub_pats, ctor_ty, (i + 1), len) : default),  });

    public static IRPat lower_pattern(APat p) => p switch { AVarPat(var name) => new IrVarPat(name.value, ErrorTy), ALitPat(var text, var kind) => new IrLitPat(text, ErrorTy), ACtorPat(var name, var subs) => new IrCtorPat(name.value, map_list(lower_pattern, subs), ErrorTy), AWildPat { } => IrWildPat,  };

    public static IRExpr lower_list(List<AExpr> elems, CodexType ty, LowerCtx ctx) => ((object elem_ty = ty switch { ListTy(var e) => e, _ => ((list_length(elems) == 0) ? ErrorTy : ir_expr_type(lower_expr(list_at(elems)(0), ErrorTy, ctx))),  }) is var _ ? new IrList(lower_list_elems_loop(elems, elem_ty, ctx, 0, list_length(elems)), elem_ty) : default);

    public static List<IRExpr> lower_list_elems_loop(List<AExpr> elems, CodexType elem_ty, LowerCtx ctx, long i, long len) => ((i == len) ? new List<IRExpr>() : (new List<IRExpr> { lower_expr(list_at(elems)(i), elem_ty, ctx) } + lower_list_elems_loop(elems, elem_ty, ctx, (i + 1), len)));

    public static IRExpr lower_record(Name name, List<AFieldExpr> fields, CodexType ty, LowerCtx ctx) => ((CodexType ctor_raw = lookup_type(ctx.types, name.value)) is var _ ? ((object record_ty = ctor_raw switch { ErrorTy { } => ty, _ => strip_fun_args_lower(deep_resolve(ctx.ust, ctor_raw)),  }) is var _ ? ((object actual_ty = record_ty switch { ErrorTy { } => ty, _ => record_ty,  }) is var _ ? new IrRecord(name.value, lower_record_fields_loop(fields, actual_ty, ctx, 0, list_length(fields)), actual_ty) : default) : default) : default);

    public static List<IRFieldVal> lower_record_fields_loop(List<AFieldExpr> fields, CodexType ty, LowerCtx ctx, long i, long len) => ((i == len) ? new List<IRFieldVal>() : ((AFieldExpr f = list_at(fields)(i)) is var _ ? (new List<IRFieldVal> { new IRFieldVal(name: f.name.value, value: lower_expr(f.value, ty, ctx)) } + lower_record_fields_loop(fields, ty, ctx, (i + 1), len)) : default));

    public static CodexType strip_fun_args_lower(CodexType ty) => ty switch { FunTy(var p, var r) => strip_fun_args_lower(r), ForAllTy(var id, var body) => strip_fun_args_lower(body), _ => ty,  };

    public static IRExpr lower_do(List<ADoStmt> stmts, CodexType ty, LowerCtx ctx) => new IrDo(lower_do_stmts_loop(stmts, ty, ctx, 0, list_length(stmts)), ty);

    public static List<IRDoStmt> lower_do_stmts_loop(List<ADoStmt> stmts, CodexType ty, LowerCtx ctx, long i, long len) => ((i == len) ? new List<IRDoStmt>() : ((ADoStmt s = list_at(stmts)(i)) is var _ ? s switch { ADoBindStmt(var name, var val) => ((IRExpr val_ir = lower_expr(val, ty, ctx)) is var _ ? ((CodexType val_ty = ir_expr_type(val_ir)) is var _ ? ((LowerCtx ctx2 = new LowerCtx(types: (new List<TypeBinding> { new TypeBinding(name: name.value, bound_type: val_ty) } + ctx.types), ust: ctx.ust)) is var _ ? (new List<IRDoStmt> { new IrDoBind(name.value, val_ty, val_ir) } + lower_do_stmts_loop(stmts, ty, ctx2, (i + 1), len)) : default) : default) : default), ADoExprStmt(var e) => (new List<IRDoStmt> { new IrDoExec(lower_expr(e, ty, ctx)) } + lower_do_stmts_loop(stmts, ty, ctx, (i + 1), len)),  } : default));

    public static IRDef lower_def(ADef d, List<TypeBinding> types, UnificationState ust) => ((CodexType raw_type = lookup_type(types, d.name.value)) is var _ ? ((CodexType full_type = deep_resolve(ust, raw_type)) is var _ ? ((CodexType stripped = strip_forall_ty(full_type)) is var _ ? ((List<IRParam> @params = lower_def_params(d.@params, stripped, 0)) is var _ ? ((CodexType ret_type = get_return_type_n(stripped, list_length(d.@params))) is var _ ? ((LowerCtx ctx = build_def_ctx(types, ust, d.@params, stripped)) is var _ ? new IRDef(name: d.name.value, @params: @params, type_val: full_type, body: lower_expr(d.body, ret_type, ctx)) : default) : default) : default) : default) : default) : default);

    public static LowerCtx build_def_ctx(List<TypeBinding> types, UnificationState ust, List<AParam> @params, CodexType ty) => ((LowerCtx base_ctx = new LowerCtx(types: types, ust: ust)) is var _ ? bind_params_to_ctx(base_ctx, @params, ty, 0) : default);

    public static LowerCtx bind_params_to_ctx(LowerCtx ctx, List<AParam> @params, CodexType ty, long i) => ((i == list_length(@params)) ? ctx : ((AParam p = list_at(@params)(i)) is var _ ? ((CodexType param_ty = peel_fun_param(ty)) is var _ ? ((CodexType rest_ty = peel_fun_return(ty)) is var _ ? ((LowerCtx ctx2 = new LowerCtx(types: (new List<TypeBinding> { new TypeBinding(name: p.name.value, bound_type: param_ty) } + ctx.types), ust: ctx.ust)) is var _ ? bind_params_to_ctx(ctx2, @params, rest_ty, (i + 1)) : default) : default) : default) : default));

    public static List<IRParam> lower_def_params(List<AParam> @params, CodexType ty, long i) => ((i == list_length(@params)) ? new List<IRParam>() : ((AParam p = list_at(@params)(i)) is var _ ? ((CodexType param_ty = peel_fun_param(ty)) is var _ ? ((CodexType rest_ty = peel_fun_return(ty)) is var _ ? (new List<IRParam> { new IRParam(name: p.name.value, type_val: param_ty) } + lower_def_params(@params, rest_ty, (i + 1))) : default) : default) : default));

    public static CodexType get_return_type_n(CodexType ty, long n) => ((n == 0) ? ty : ty switch { FunTy(var p, var r) => get_return_type_n(r, (n - 1)), _ => ErrorTy,  });

    public static IRModule lower_module(AModule m, List<TypeBinding> types, UnificationState ust) => ((List<TypeBinding> ctor_types = collect_ctor_bindings(m.type_defs, 0, list_length(m.type_defs), new List<TypeBinding>())) is var _ ? ((object all_types = (ctor_types + (types + builtin_type_env.bindings))) is var _ ? new IRModule(name: m.name, defs: lower_defs(m.defs, all_types, ust, 0)) : default) : default);

    public static List<IRDef> lower_defs(List<ADef> defs, List<TypeBinding> types, UnificationState ust, long i) => ((i == list_length(defs)) ? new List<IRDef>() : (new List<IRDef> { lower_def(list_at(defs)(i), types, ust) } + lower_defs(defs, types, ust, (i + 1))));

    public static List<TypeBinding> collect_ctor_bindings(List<ATypeDef> tdefs, long i, long len, List<TypeBinding> acc) => ((i == len) ? acc : ((ATypeDef td = list_at(tdefs)(i)) is var _ ? ((List<TypeBinding> bindings = ctor_bindings_for_typedef(td)) is var _ ? collect_ctor_bindings(tdefs, (i + 1), len, (acc + bindings)) : default) : default));

    public static List<TypeBinding> ctor_bindings_for_typedef(ATypeDef td) => td switch { AVariantTypeDef(var name, var type_params, var ctors) => ((CodexType result_ty = new ConstructedTy(name, new List<CodexType>())) is var _ ? collect_variant_ctor_bindings(ctors, result_ty, 0, list_length(ctors), new List<TypeBinding>()) : default), ARecordTypeDef(var name, var type_params, var fields) => ((List<RecordField> resolved_fields = build_record_fields_for_lower(fields, 0, list_length(fields), new List<RecordField>())) is var _ ? ((CodexType result_ty = new RecordTy(name, resolved_fields)) is var _ ? ((CodexType ctor_ty = build_record_ctor_type_for_lower(fields, result_ty, 0, list_length(fields))) is var _ ? new List<TypeBinding> { new TypeBinding(name: name.value, bound_type: ctor_ty) } : default) : default) : default),  };

    public static List<TypeBinding> collect_variant_ctor_bindings(List<AVariantCtorDef> ctors, CodexType result_ty, long i, long len, List<TypeBinding> acc) => ((i == len) ? acc : ((AVariantCtorDef ctor = list_at(ctors)(i)) is var _ ? ((CodexType ctor_ty = build_ctor_type_for_lower(ctor.fields, result_ty, 0, list_length(ctor.fields))) is var _ ? collect_variant_ctor_bindings(ctors, result_ty, (i + 1), len, (acc + new List<TypeBinding> { new TypeBinding(name: ctor.name.value, bound_type: ctor_ty) })) : default) : default));

    public static CodexType build_ctor_type_for_lower(List<ATypeExpr> fields, CodexType result, long i, long len) => ((i == len) ? result : ((CodexType rest = build_ctor_type_for_lower(fields, result, (i + 1), len)) is var _ ? new FunTy(resolve_type_expr_for_lower(list_at(fields)(i)), rest) : default));

    public static List<RecordField> build_record_fields_for_lower(List<ARecordFieldDef> fields, long i, long len, List<RecordField> acc) => ((i == len) ? acc : ((ARecordFieldDef f = list_at(fields)(i)) is var _ ? ((RecordField rfield = new RecordField(name: f.name, type_val: resolve_type_expr_for_lower(f.type_expr))) is var _ ? build_record_fields_for_lower(fields, (i + 1), len, (acc + new List<RecordField> { rfield })) : default) : default));

    public static CodexType build_record_ctor_type_for_lower(List<ARecordFieldDef> fields, CodexType result, long i, long len) => ((i == len) ? result : ((ARecordFieldDef f = list_at(fields)(i)) is var _ ? ((CodexType rest = build_record_ctor_type_for_lower(fields, result, (i + 1), len)) is var _ ? new FunTy(resolve_type_expr_for_lower(f.type_expr), rest) : default) : default));

    public static CodexType resolve_type_expr_for_lower(ATypeExpr texpr) => texpr switch { ANamedType(var name) => ((name.value == "Integer") ? IntegerTy : ((name.value == "Number") ? NumberTy : ((name.value == "Text") ? TextTy : ((name.value == "Boolean") ? BooleanTy : ((name.value == "Nothing") ? NothingTy : new ConstructedTy(name, new List<CodexType>())))))), AFunType(var param, var ret) => new FunTy(resolve_type_expr_for_lower(param), resolve_type_expr_for_lower(ret)), AAppType(var ctor, var args) => ctor switch { ANamedType(var cname) => ((cname.value == "List") ? ((list_length(args) == 1) ? new ListTy(resolve_type_expr_for_lower(list_at(args)(0))) : new ListTy(ErrorTy)) : new ConstructedTy(cname, new List<CodexType>())), _ => ErrorTy,  },  };

    public static Scope empty_scope() => new Scope(names: new List<object>());

    public static bool scope_has(Scope sc, string name) => scope_has_loop(sc.names, name, 0, list_length(sc.names));

    public static bool scope_has_loop(List<string> names, string name, long i, long len) => ((i == len) ? false : ((list_at(names)(i) == name) ? true : scope_has_loop(names, name, (i + 1), len)));

    public static Scope scope_add(Scope sc, string name) => new Scope(names: (new List<string> { name } + sc.names));

    public static List<string> builtin_names() => new List<string> { "show", "negate", "True", "False", "Nothing", "print-line", "read-line", "open-file", "read-all", "close-file", "char-at", "text-length", "substring", "is-letter", "is-digit", "is-whitespace", "text-to-integer", "integer-to-text", "text-replace", "char-code", "code-to-char", "list-length", "list-at", "map", "filter", "fold" };

    public static bool is_type_name(string name) => ((text_length(name) == 0) ? false : (is_letter(char_at(name)(0)) && is_upper_char(char_at(name)(0))));

    public static bool is_upper_char(string c) => ((long code = char_code(c)) is var _ ? ((code >= 65) && (code <= 90)) : default);

    public static CollectResult collect_top_level_names(List<ADef> defs, long i, long len, List<string> acc, List<Diagnostic> errs) => ((i == len) ? new CollectResult(names: acc, errors: errs) : ((ADef def = list_at(defs)(i)) is var _ ? ((object name = def.name.value) is var _ ? (list_contains(acc, name) ? collect_top_level_names(defs, (i + 1), len, acc, (errs + new List<Diagnostic> { make_error("CDX3001", ("Duplicate definition: " + name)) })) : collect_top_level_names(defs, (i + 1), len, (acc + new List<string> { name }), errs)) : default) : default));

    public static bool list_contains(List<string> xs, string name) => list_contains_loop(xs, name, 0, list_length(xs));

    public static bool list_contains_loop(List<string> xs, string name, long i, long len) => ((i == len) ? false : ((list_at(xs)(i) == name) ? true : list_contains_loop(xs, name, (i + 1), len)));

    public static CtorCollectResult collect_ctor_names(List<ATypeDef> type_defs, long i, long len, List<string> type_acc, List<string> ctor_acc) => ((i == len) ? new CtorCollectResult(type_names: type_acc, ctor_names: ctor_acc) : ((ATypeDef td = list_at(type_defs)(i)) is var _ ? td switch { AVariantTypeDef(var name, var @params, var ctors) => ((object new_type_acc = (type_acc + new List<object> { name.value })) is var _ ? ((List<string> new_ctor_acc = collect_variant_ctors(ctors, 0, list_length(ctors), ctor_acc)) is var _ ? collect_ctor_names(type_defs, (i + 1), len, new_type_acc, new_ctor_acc) : default) : default), ARecordTypeDef(var name, var @params, var fields) => collect_ctor_names(type_defs, (i + 1), len, (type_acc + new List<string> { name.value }), ctor_acc),  } : default));

    public static List<string> collect_variant_ctors(List<AVariantCtorDef> ctors, long i, long len, List<string> acc) => ((i == len) ? acc : ((AVariantCtorDef ctor = list_at(ctors)(i)) is var _ ? collect_variant_ctors(ctors, (i + 1), len, (acc + new List<string> { ctor.name.value })) : default));

    public static Scope build_all_names_scope(List<string> top_names, List<string> ctor_names, List<string> builtins) => ((Scope sc = add_names_to_scope(empty_scope, top_names, 0, list_length(top_names))) is var _ ? ((Scope sc2 = add_names_to_scope(sc, ctor_names, 0, list_length(ctor_names))) is var _ ? add_names_to_scope(sc2, builtins, 0, list_length(builtins)) : default) : default);

    public static Scope add_names_to_scope(Scope sc, List<string> names, long i, long len) => ((i == len) ? sc : add_names_to_scope(scope_add(sc, list_at(names)(i)), names, (i + 1), len));

    public static List<Diagnostic> resolve_expr(Scope sc, AExpr expr) => expr switch { ALitExpr(var val, var kind) => new List<Diagnostic>(), ANameExpr(var name) => ((scope_has(sc, name.value) || is_type_name(name.value)) ? new List<Diagnostic>() : new List<Diagnostic> { make_error("CDX3002", ("Undefined name: " + name.value)) }), ABinaryExpr(var left, var op, var right) => (resolve_expr(sc, left) + resolve_expr(sc, right)), AUnaryExpr(var operand) => resolve_expr(sc, operand), AApplyExpr(var func, var arg) => (resolve_expr(sc, func) + resolve_expr(sc, arg)), AIfExpr(var cond, var then_e, var else_e) => (resolve_expr(sc, cond) + (resolve_expr(sc, then_e) + resolve_expr(sc, else_e))), ALetExpr(var bindings, var body) => resolve_let(sc, bindings, body, 0, list_length(bindings), new List<Diagnostic>()), ALambdaExpr(var @params, var body) => ((Scope sc2 = add_lambda_params(sc, @params, 0, list_length(@params))) is var _ ? resolve_expr(sc2, body) : default), AMatchExpr(var scrutinee, var arms) => (resolve_expr(sc, scrutinee) + resolve_match_arms(sc, arms, 0, list_length(arms), new List<Diagnostic>())), AListExpr(var elems) => resolve_list_elems(sc, elems, 0, list_length(elems), new List<Diagnostic>()), ARecordExpr(var name, var fields) => resolve_record_fields(sc, fields, 0, list_length(fields), new List<Diagnostic>()), AFieldAccess(var obj, var field) => resolve_expr(sc, obj), ADoExpr(var stmts) => resolve_do_stmts(sc, stmts, 0, list_length(stmts), new List<Diagnostic>()), AErrorExpr(var msg) => new List<Diagnostic>(),  };

    public static List<Diagnostic> resolve_let(Scope sc, List<ALetBind> bindings, AExpr body, long i, long len, List<Diagnostic> errs) => ((i == len) ? (errs + resolve_expr(sc, body)) : ((ALetBind b = list_at(bindings)(i)) is var _ ? ((List<Diagnostic> bind_errs = resolve_expr(sc, b.value)) is var _ ? ((Scope sc2 = scope_add(sc, b.name.value)) is var _ ? resolve_let(sc2, bindings, body, (i + 1), len, (errs + bind_errs)) : default) : default) : default));

    public static Scope add_lambda_params(Scope sc, List<Name> @params, long i, long len) => ((i == len) ? sc : ((Name p = list_at(@params)(i)) is var _ ? add_lambda_params(scope_add(sc, p.value), @params, (i + 1), len) : default));

    public static List<Diagnostic> resolve_match_arms(Scope sc, List<AMatchArm> arms, long i, long len, List<Diagnostic> errs) => ((i == len) ? errs : ((AMatchArm arm = list_at(arms)(i)) is var _ ? ((Scope sc2 = collect_pattern_names(sc, arm.pattern)) is var _ ? ((List<Diagnostic> arm_errs = resolve_expr(sc2, arm.body)) is var _ ? resolve_match_arms(sc, arms, (i + 1), len, (errs + arm_errs)) : default) : default) : default));

    public static Scope collect_pattern_names(Scope sc, APat pat) => pat switch { AVarPat(var name) => scope_add(sc, name.value), ACtorPat(var name, var subs) => collect_ctor_pat_names(sc, subs, 0, list_length(subs)), ALitPat(var val, var kind) => sc, AWildPat { } => sc,  };

    public static Scope collect_ctor_pat_names(Scope sc, List<APat> subs, long i, long len) => ((i == len) ? sc : ((APat sub = list_at(subs)(i)) is var _ ? collect_ctor_pat_names(collect_pattern_names(sc, sub), subs, (i + 1), len) : default));

    public static List<Diagnostic> resolve_list_elems(Scope sc, List<AExpr> elems, long i, long len, List<Diagnostic> errs) => ((i == len) ? errs : ((List<Diagnostic> errs2 = resolve_expr(sc, list_at(elems)(i))) is var _ ? resolve_list_elems(sc, elems, (i + 1), len, (errs + errs2)) : default));

    public static List<Diagnostic> resolve_record_fields(Scope sc, List<AFieldExpr> fields, long i, long len, List<Diagnostic> errs) => ((i == len) ? errs : ((AFieldExpr f = list_at(fields)(i)) is var _ ? ((List<Diagnostic> errs2 = resolve_expr(sc, f.value)) is var _ ? resolve_record_fields(sc, fields, (i + 1), len, (errs + errs2)) : default) : default));

    public static List<Diagnostic> resolve_do_stmts(Scope sc, List<ADoStmt> stmts, long i, long len, List<Diagnostic> errs) => ((i == len) ? errs : ((ADoStmt stmt = list_at(stmts)(i)) is var _ ? stmt switch { ADoExprStmt(var e) => ((List<Diagnostic> errs2 = resolve_expr(sc, e)) is var _ ? resolve_do_stmts(sc, stmts, (i + 1), len, (errs + errs2)) : default), ADoBindStmt(var name, var e) => ((List<Diagnostic> errs2 = resolve_expr(sc, e)) is var _ ? ((Scope sc2 = scope_add(sc, name.value)) is var _ ? resolve_do_stmts(sc2, stmts, (i + 1), len, (errs + errs2)) : default) : default),  } : default));

    public static List<Diagnostic> resolve_all_defs(Scope sc, List<ADef> defs, long i, long len, List<Diagnostic> errs) => ((i == len) ? errs : ((ADef def = list_at(defs)(i)) is var _ ? ((Scope def_scope = add_def_params(sc, def.@params, 0, list_length(def.@params))) is var _ ? ((List<Diagnostic> errs2 = resolve_expr(def_scope, def.body)) is var _ ? resolve_all_defs(sc, defs, (i + 1), len, (errs + errs2)) : default) : default) : default));

    public static Scope add_def_params(Scope sc, List<AParam> @params, long i, long len) => ((i == len) ? sc : ((AParam p = list_at(@params)(i)) is var _ ? add_def_params(scope_add(sc, p.name.value), @params, (i + 1), len) : default));

    public static ResolveResult resolve_module(AModule mod) => ((CollectResult top = collect_top_level_names(mod.defs, 0, list_length(mod.defs), new List<string>(), new List<Diagnostic>())) is var _ ? ((CtorCollectResult ctors = collect_ctor_names(mod.type_defs, 0, list_length(mod.type_defs), new List<string>(), new List<string>())) is var _ ? ((Scope sc = build_all_names_scope(top.names, ctors.ctor_names, builtin_names)) is var _ ? ((List<Diagnostic> expr_errs = resolve_all_defs(sc, mod.defs, 0, list_length(mod.defs), new List<Diagnostic>())) is var _ ? new ResolveResult(errors: (top.errors + expr_errs), top_level_names: top.names, type_names: ctors.type_names, ctor_names: ctors.ctor_names) : default) : default) : default) : default);

    public static LexState make_lex_state(string src) => new LexState(source: src, offset: 0, line: 1, column: 1);

    public static bool is_at_end(LexState st) => (st.offset >= text_length(st.source));

    public static string peek_char(LexState st) => (is_at_end(st) ? "" : char_at(st.source)(st.offset));

    public static LexState advance_char(LexState st) => ((peek_char(st) == "\\n") ? new LexState(source: st.source, offset: (st.offset + 1), line: (st.line + 1), column: 1) : new LexState(source: st.source, offset: (st.offset + 1), line: st.line, column: (st.column + 1)));

    public static LexState skip_spaces(LexState st) => (is_at_end(st) ? st : ((peek_char(st) == " ") ? skip_spaces(advance_char(st)) : st));

    public static LexState scan_ident_rest(LexState st) => (is_at_end(st) ? st : ((string ch = peek_char(st)) is var _ ? (is_letter(ch) ? scan_ident_rest(advance_char(st)) : (is_digit(ch) ? scan_ident_rest(advance_char(st)) : ((ch == "_") ? scan_ident_rest(advance_char(st)) : ((ch == "-") ? ((LexState next = advance_char(st)) is var _ ? (is_at_end(next) ? st : (is_letter(peek_char(next)) ? scan_ident_rest(next) : st)) : default) : st)))) : default));

    public static LexState scan_digits(LexState st) => (is_at_end(st) ? st : ((string ch = peek_char(st)) is var _ ? (is_digit(ch) ? scan_digits(advance_char(st)) : ((ch == "_") ? scan_digits(advance_char(st)) : st)) : default));

    public static LexState scan_string_body(LexState st) => (is_at_end(st) ? st : ((string ch = peek_char(st)) is var _ ? ((ch == "\\\"") ? advance_char(st) : ((ch == "\\n") ? st : ((ch == "\\\\") ? scan_string_body(advance_char(advance_char(st))) : scan_string_body(advance_char(st))))) : default));

    public static TokenKind classify_word(string w) => ((w == "let") ? LetKeyword : ((w == "in") ? InKeyword : ((w == "if") ? IfKeyword : ((w == "then") ? ThenKeyword : ((w == "else") ? ElseKeyword : ((w == "when") ? WhenKeyword : ((w == "where") ? WhereKeyword : ((w == "do") ? DoKeyword : ((w == "record") ? RecordKeyword : ((w == "import") ? ImportKeyword : ((w == "export") ? ExportKeyword : ((w == "claim") ? ClaimKeyword : ((w == "proof") ? ProofKeyword : ((w == "forall") ? ForAllKeyword : ((w == "exists") ? ThereExistsKeyword : ((w == "linear") ? LinearKeyword : ((w == "True") ? TrueKeyword : ((w == "False") ? FalseKeyword : ((long first_code = char_code(char_at(w)(0))) is var _ ? ((first_code >= 65) ? ((first_code <= 90) ? TypeIdentifier : Identifier) : Identifier) : default)))))))))))))))))));

    public static Token make_token(TokenKind kind, string text, LexState st) => new Token(kind: kind, text: text, offset: st.offset, line: st.line, column: st.column);

    public static string extract_text(LexState st, long start, LexState end_st) => substring(st.source)(start)((end_st.offset - start));

    public static LexResult scan_token(LexState st) => ((LexState s = skip_spaces(st)) is var _ ? (is_at_end(s) ? LexEnd : ((string ch = peek_char(s)) is var _ ? ((ch == "\\n") ? new LexToken(make_token(Newline, "\\n", s), advance_char(s)) : ((ch == "\\\"") ? ((object start = (s.offset + 1)) is var _ ? ((LexState after = scan_string_body(advance_char(s))) is var _ ? ((object text_len = ((after.offset - start) - 1)) is var _ ? new LexToken(make_token(TextLiteral, substring(s.source)(start)(text_len), s), after) : default) : default) : default) : (is_letter(ch) ? ((long start = s.offset) is var _ ? ((LexState after = scan_ident_rest(advance_char(s))) is var _ ? ((string word = extract_text(s, start, after)) is var _ ? new LexToken(make_token(classify_word(word), word, s), after) : default) : default) : default) : ((ch == "_") ? ((long start = s.offset) is var _ ? ((LexState after = scan_ident_rest(advance_char(s))) is var _ ? ((string word = extract_text(s, start, after)) is var _ ? ((text_length(word) == 1) ? new LexToken(make_token(Underscore, "_", s), after) : new LexToken(make_token(classify_word(word), word, s), after)) : default) : default) : default) : (is_digit(ch) ? ((long start = s.offset) is var _ ? ((LexState after = scan_digits(advance_char(s))) is var _ ? (is_at_end(after) ? new LexToken(make_token(IntegerLiteral, extract_text(s, start, after), s), after) : ((peek_char(after) == ".") ? ((LexState after2 = scan_digits(advance_char(after))) is var _ ? new LexToken(make_token(NumberLiteral, extract_text(s, start, after2), s), after2) : default) : new LexToken(make_token(IntegerLiteral, extract_text(s, start, after), s), after))) : default) : default) : scan_operator(s)))))) : default)) : default);

    public static LexResult scan_operator(LexState s) => ((string ch = peek_char(s)) is var _ ? ((LexState next = advance_char(s)) is var _ ? ((ch == "(") ? new LexToken(make_token(LeftParen, "(", s), next) : ((ch == ")") ? new LexToken(make_token(RightParen, ")", s), next) : ((ch == "[") ? new LexToken(make_token(LeftBracket, "[", s), next) : ((ch == "]") ? new LexToken(make_token(RightBracket, "]", s), next) : ((ch == "{") ? new LexToken(make_token(LeftBrace, "{", s), next) : ((ch == "}") ? new LexToken(make_token(RightBrace, "}", s), next) : ((ch == ",") ? new LexToken(make_token(Comma, ",", s), next) : ((ch == ".") ? new LexToken(make_token(Dot, ".", s), next) : ((ch == "^") ? new LexToken(make_token(Caret, "^", s), next) : ((ch == "&") ? new LexToken(make_token(Ampersand, "&", s), next) : scan_multi_char_operator(s))))))))))) : default) : default);

    public static LexResult scan_multi_char_operator(LexState s) => ((string ch = peek_char(s)) is var _ ? ((LexState next = advance_char(s)) is var _ ? ((object next_ch = (is_at_end(next) ? "" : peek_char(next))) is var _ ? ((ch == "+") ? ((next_ch == "+") ? new LexToken(make_token(PlusPlus, "++", s), advance_char(next)) : new LexToken(make_token(Plus, "+", s), next)) : ((ch == "-") ? ((next_ch == ">") ? new LexToken(make_token(Arrow, "->", s), advance_char(next)) : new LexToken(make_token(Minus, "-", s), next)) : ((ch == "*") ? new LexToken(make_token(Star, "*", s), next) : ((ch == "/") ? ((next_ch == "=") ? new LexToken(make_token(NotEquals, "/=", s), advance_char(next)) : new LexToken(make_token(Slash, "/", s), next)) : ((ch == "=") ? ((next_ch == "=") ? ((LexState next2 = advance_char(next)) is var _ ? ((object next2_ch = (is_at_end(next2) ? "" : peek_char(next2))) is var _ ? ((next2_ch == "=") ? new LexToken(make_token(TripleEquals, "===", s), advance_char(next2)) : new LexToken(make_token(DoubleEquals, "==", s), next2)) : default) : default) : new LexToken(make_token(Equals, "=", s), next)) : ((ch == ":") ? ((next_ch == ":") ? new LexToken(make_token(ColonColon, "::", s), advance_char(next)) : new LexToken(make_token(Colon, ":", s), next)) : ((ch == "|") ? ((next_ch == "-") ? new LexToken(make_token(Turnstile, "|-", s), advance_char(next)) : new LexToken(make_token(Pipe, "|", s), next)) : ((ch == "<") ? ((next_ch == "=") ? new LexToken(make_token(LessOrEqual, "<=", s), advance_char(next)) : ((next_ch == "-") ? new LexToken(make_token(LeftArrow, "<-", s), advance_char(next)) : new LexToken(make_token(LessThan, "<", s), next))) : ((ch == ">") ? ((next_ch == "=") ? new LexToken(make_token(GreaterOrEqual, ">=", s), advance_char(next)) : new LexToken(make_token(GreaterThan, ">", s), next)) : new LexToken(make_token(ErrorToken, char_at(s.source)(s.offset), s), next)))))))))) : default) : default) : default);

    public static List<Token> tokenize_loop(LexState st, List<Token> acc) => scan_token(st) switch { LexToken(var tok, var next) => ((tok.kind == EndOfFile) ? (acc + new List<Token> { tok }) : tokenize_loop(next, (acc + new List<Token> { tok }))), LexEnd { } => (acc + new List<Token> { make_token(EndOfFile, "", st) }),  };

    public static List<Token> tokenize(string src) => tokenize_loop(make_lex_state(src), new List<Token>());

    public static ParseState make_parse_state(List<Token> toks) => new ParseState(tokens: toks, pos: 0);

    public static Token current(ParseState st) => list_at(st.tokens)(st.pos);

    public static TokenKind current_kind(ParseState st) => current(st).kind;

    public static ParseState advance(ParseState st) => new ParseState(tokens: st.tokens, pos: (st.pos + 1));

    public static bool is_done(ParseState st) => current_kind(st) switch { EndOfFile { } => true, _ => false,  };

    public static TokenKind peek_kind(ParseState st, long offset) => list_at(st.tokens)((st.pos + offset)).kind;

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

    public static ParseTypeResult parse_type(ParseState st) => ((ParseTypeResult result = parse_type_atom(st)) is var _ ? unwrap_type_ok(result, parse_type_continue) : default);

    public static ParseTypeResult parse_type_continue(TypeExpr left, ParseState st) => (is_arrow(current_kind(st)) ? ((ParseState st2 = advance(st)) is var _ ? ((ParseTypeResult right_result = parse_type(st2)) is var _ ? unwrap_type_ok(right_result, (_p0_) => (_p1_) => make_fun_type(left, _p0_, _p1_)) : default) : default) : new TypeOk(left, st));

    public static ParseTypeResult make_fun_type(TypeExpr left, TypeExpr right, ParseState st) => new TypeOk(new FunType(left, right), st);

    public static ParseTypeResult unwrap_type_ok(ParseTypeResult r, Func<TypeExpr, Func<ParseState, ParseTypeResult>> f) => r switch { TypeOk(var t, var st) => f(t)(st),  };

    public static ParseTypeResult parse_type_atom(ParseState st) => (is_ident(current_kind(st)) ? ((Token tok = current(st)) is var _ ? parse_type_args(new NamedType(tok), advance(st)) : default) : (is_type_ident(current_kind(st)) ? ((Token tok = current(st)) is var _ ? parse_type_args(new NamedType(tok), advance(st)) : default) : (is_left_paren(current_kind(st)) ? parse_paren_type(advance(st)) : ((Token tok = current(st)) is var _ ? new TypeOk(new NamedType(tok), advance(st)) : default))));

    public static ParseTypeResult parse_paren_type(ParseState st) => ((ParseTypeResult inner = parse_type(st)) is var _ ? unwrap_type_ok(inner, finish_paren_type) : default);

    public static ParseTypeResult finish_paren_type(TypeExpr t, ParseState st) => ((ParseState st2 = expect(RightParen, st)) is var _ ? new TypeOk(new ParenType(t), st2) : default);

    public static ParseTypeResult parse_type_args(TypeExpr base_type, ParseState st) => (is_done(st) ? new TypeOk(base_type, st) : (is_type_arg_start(current_kind(st)) ? parse_type_arg_next(base_type, st) : new TypeOk(base_type, st)));

    public static ParseTypeResult parse_type_arg_next(TypeExpr base_type, ParseState st) => ((ParseTypeResult arg_result = parse_type_atom(st)) is var _ ? unwrap_type_ok(arg_result, (_p0_) => (_p1_) => continue_type_args(base_type, _p0_, _p1_)) : default);

    public static ParseTypeResult continue_type_args(TypeExpr base_type, TypeExpr arg, ParseState st) => parse_type_args(new AppType(base_type, new List<TypeExpr> { arg }), st);

    public static ParsePatResult parse_pattern(ParseState st) => (is_underscore(current_kind(st)) ? new PatOk(new WildPat(current(st)), advance(st)) : (is_literal(current_kind(st)) ? new PatOk(new LitPat(current(st)), advance(st)) : (is_type_ident(current_kind(st)) ? ((Token tok = current(st)) is var _ ? parse_ctor_pattern_fields(tok, new List<Pat>(), advance(st)) : default) : (is_ident(current_kind(st)) ? new PatOk(new VarPat(current(st)), advance(st)) : new PatOk(new WildPat(current(st)), advance(st))))));

    public static ParsePatResult parse_ctor_pattern_fields(Token ctor, List<Pat> acc, ParseState st) => (is_left_paren(current_kind(st)) ? ((ParseState st2 = advance(st)) is var _ ? ((ParsePatResult sub = parse_pattern(st2)) is var _ ? unwrap_pat_ok(sub, (_p0_) => (_p1_) => continue_ctor_fields(ctor, acc, _p0_, _p1_)) : default) : default) : new PatOk(new CtorPat(ctor, acc), st));

    public static ParsePatResult continue_ctor_fields(Token ctor, List<Pat> acc, Pat p, ParseState st) => ((ParseState st2 = expect(RightParen, st)) is var _ ? parse_ctor_pattern_fields(ctor, (acc + new List<Pat> { p }), st2) : default);

    public static ParsePatResult unwrap_pat_ok(ParsePatResult r, Func<Pat, Func<ParseState, ParsePatResult>> f) => r switch { PatOk(var p, var st) => f(p)(st),  };

    public static ParseExprResult parse_expr(ParseState st) => parse_binary(st, 0);

    public static ParseExprResult unwrap_expr_ok(ParseExprResult r, Func<Expr, Func<ParseState, ParseExprResult>> f) => r switch { ExprOk(var e, var st) => f(e)(st),  };

    public static ParseExprResult parse_binary(ParseState st, long min_prec) => ((ParseExprResult left_result = parse_unary(st)) is var _ ? unwrap_expr_ok(left_result, (_p0_) => (_p1_) => start_binary_loop(min_prec, _p0_, _p1_)) : default);

    public static ParseExprResult start_binary_loop(long min_prec, Expr left, ParseState st) => parse_binary_loop(left, st, min_prec);

    public static ParseExprResult parse_binary_loop(Expr left, ParseState st, long min_prec) => (is_done(st) ? new ExprOk(left, st) : ((long prec = operator_precedence(current_kind(st))) is var _ ? ((prec < min_prec) ? new ExprOk(left, st) : ((Token op = current(st)) is var _ ? ((ParseState st2 = skip_newlines(advance(st))) is var _ ? ((object next_min = (is_right_assoc(op.kind) ? prec : (prec + 1))) is var _ ? ((ParseExprResult right_result = parse_binary(st2, next_min)) is var _ ? unwrap_expr_ok(right_result, (_p0_) => (_p1_) => continue_binary(left, op, min_prec, _p0_, _p1_)) : default) : default) : default) : default)) : default));

    public static ParseExprResult continue_binary(Expr left, Token op, long min_prec, Expr right, ParseState st) => parse_binary_loop(new BinExpr(left, op, right), st, min_prec);

    public static ParseExprResult parse_unary(ParseState st) => (is_minus(current_kind(st)) ? ((Token op = current(st)) is var _ ? ((ParseExprResult result = parse_unary(advance(st))) is var _ ? unwrap_expr_ok(result, (_p0_) => (_p1_) => finish_unary(op, _p0_, _p1_)) : default) : default) : parse_application(st));

    public static ParseExprResult finish_unary(Token op, Expr operand, ParseState st) => new ExprOk(new UnaryExpr(op, operand), st);

    public static ParseExprResult parse_application(ParseState st) => ((ParseExprResult func_result = parse_atom(st)) is var _ ? unwrap_expr_ok(func_result, parse_app_loop) : default);

    public static ParseExprResult parse_app_loop(Expr func, ParseState st) => (is_compound(func) ? parse_dot_only(func, st) : (is_done(st) ? new ExprOk(func, st) : (is_app_start(current_kind(st)) ? ((ParseExprResult arg_result = parse_atom(st)) is var _ ? unwrap_expr_ok(arg_result, (_p0_) => (_p1_) => continue_app(func, _p0_, _p1_)) : default) : parse_field_access(func, st))));

    public static ParseExprResult continue_app(Expr func, Expr arg, ParseState st) => parse_app_loop(new AppExpr(func, arg), st);

    public static ParseExprResult parse_atom(ParseState st) => (is_literal(current_kind(st)) ? new ExprOk(new LitExpr(current(st)), advance(st)) : (is_ident(current_kind(st)) ? parse_field_access(new NameExpr(current(st)), advance(st)) : (is_type_ident(current_kind(st)) ? parse_atom_type_ident(st) : (is_left_paren(current_kind(st)) ? parse_paren_expr(advance(st)) : (is_left_bracket(current_kind(st)) ? parse_list_expr(st) : (is_if_keyword(current_kind(st)) ? parse_if_expr(st) : (is_let_keyword(current_kind(st)) ? parse_let_expr(st) : (is_when_keyword(current_kind(st)) ? parse_match_expr(st) : (is_do_keyword(current_kind(st)) ? parse_do_expr(st) : new ExprOk(new ErrExpr(current(st)), advance(st)))))))))));

    public static ParseExprResult parse_field_access(Expr node, ParseState st) => (is_dot(current_kind(st)) ? ((ParseState st2 = advance(st)) is var _ ? ((Token field = current(st2)) is var _ ? ((ParseState st3 = advance(st2)) is var _ ? parse_field_access(new FieldExpr(node, field), st3) : default) : default) : default) : new ExprOk(node, st));

    public static ParseExprResult parse_dot_only(Expr node, ParseState st) => (is_dot(current_kind(st)) ? ((ParseState st2 = advance(st)) is var _ ? ((Token field = current(st2)) is var _ ? ((ParseState st3 = advance(st2)) is var _ ? parse_dot_only(new FieldExpr(node, field), st3) : default) : default) : default) : new ExprOk(node, st));

    public static ParseExprResult parse_atom_type_ident(ParseState st) => ((Token tok = current(st)) is var _ ? ((ParseState st2 = advance(st)) is var _ ? (is_left_brace(current_kind(st2)) ? parse_record_expr(tok, st2) : new ExprOk(new NameExpr(tok), st2)) : default) : default);

    public static ParseExprResult parse_paren_expr(ParseState st) => ((ParseState st2 = skip_newlines(st)) is var _ ? ((ParseExprResult inner = parse_expr(st2)) is var _ ? unwrap_expr_ok(inner, finish_paren_expr) : default) : default);

    public static ParseExprResult finish_paren_expr(Expr e, ParseState st) => ((ParseState st2 = skip_newlines(st)) is var _ ? ((ParseState st3 = expect(RightParen, st2)) is var _ ? new ExprOk(new ParenExpr(e), st3) : default) : default);

    public static ParseExprResult parse_record_expr(Token type_name, ParseState st) => ((ParseState st2 = advance(st)) is var _ ? ((ParseState st3 = skip_newlines(st2)) is var _ ? parse_record_expr_fields(type_name, new List<RecordFieldExpr>(), st3) : default) : default);

    public static ParseExprResult parse_record_expr_fields(Token type_name, List<RecordFieldExpr> acc, ParseState st) => (is_right_brace(current_kind(st)) ? new ExprOk(new RecordExpr(type_name, acc), advance(st)) : (is_ident(current_kind(st)) ? parse_record_field(type_name, acc, st) : new ExprOk(new RecordExpr(type_name, acc), st)));

    public static ParseExprResult parse_record_field(Token type_name, List<RecordFieldExpr> acc, ParseState st) => ((Token field_name = current(st)) is var _ ? ((ParseState st2 = advance(st)) is var _ ? ((ParseState st3 = expect(Equals, st2)) is var _ ? ((ParseExprResult val_result = parse_expr(st3)) is var _ ? unwrap_expr_ok(val_result, (_p0_) => (_p1_) => finish_record_field(type_name, acc, field_name, _p0_, _p1_)) : default) : default) : default) : default);

    public static ParseExprResult finish_record_field(Token type_name, List<RecordFieldExpr> acc, Token field_name, Expr v, ParseState st) => ((RecordFieldExpr field = new RecordFieldExpr(name: field_name, value: v)) is var _ ? ((ParseState st2 = skip_newlines(st)) is var _ ? (is_comma(current_kind(st2)) ? parse_record_expr_fields(type_name, (acc + new List<RecordFieldExpr> { field }), skip_newlines(advance(st2))) : parse_record_expr_fields(type_name, (acc + new List<RecordFieldExpr> { field }), st2)) : default) : default);

    public static ParseExprResult parse_list_expr(ParseState st) => ((ParseState st2 = advance(st)) is var _ ? ((ParseState st3 = skip_newlines(st2)) is var _ ? parse_list_elements(new List<Expr>(), st3) : default) : default);

    public static ParseExprResult parse_list_elements(List<Expr> acc, ParseState st) => (is_right_bracket(current_kind(st)) ? new ExprOk(new ListExpr(acc), advance(st)) : ((ParseExprResult elem = parse_expr(st)) is var _ ? unwrap_expr_ok(elem, (_p0_) => (_p1_) => finish_list_element(acc, _p0_, _p1_)) : default));

    public static ParseExprResult finish_list_element(List<Expr> acc, Expr e, ParseState st) => ((ParseState st2 = skip_newlines(st)) is var _ ? (is_comma(current_kind(st2)) ? parse_list_elements((acc + new List<Expr> { e }), skip_newlines(advance(st2))) : parse_list_elements((acc + new List<Expr> { e }), st2)) : default);

    public static ParseExprResult parse_if_expr(ParseState st) => ((ParseState st2 = skip_newlines(advance(st))) is var _ ? ((ParseExprResult cond = parse_expr(st2)) is var _ ? unwrap_expr_ok(cond, parse_if_then) : default) : default);

    public static ParseExprResult parse_if_then(Expr c, ParseState st) => ((ParseState st2 = skip_newlines(st)) is var _ ? ((ParseState st3 = expect(ThenKeyword, st2)) is var _ ? ((ParseState st4 = skip_newlines(st3)) is var _ ? ((ParseExprResult then_result = parse_expr(st4)) is var _ ? unwrap_expr_ok(then_result, (_p0_) => (_p1_) => parse_if_else(c, _p0_, _p1_)) : default) : default) : default) : default);

    public static ParseExprResult parse_if_else(Expr c, Expr t, ParseState st) => ((ParseState st2 = skip_newlines(st)) is var _ ? ((ParseState st3 = expect(ElseKeyword, st2)) is var _ ? ((ParseState st4 = skip_newlines(st3)) is var _ ? ((ParseExprResult else_result = parse_expr(st4)) is var _ ? unwrap_expr_ok(else_result, (_p0_) => (_p1_) => finish_if(c, t, _p0_, _p1_)) : default) : default) : default) : default);

    public static ParseExprResult finish_if(Expr c, Expr t, Expr e, ParseState st) => new ExprOk(new IfExpr(c, t, e), st);

    public static ParseExprResult parse_let_expr(ParseState st) => ((ParseState st2 = skip_newlines(advance(st))) is var _ ? parse_let_bindings(new List<LetBind>(), st2) : default);

    public static ParseExprResult parse_let_bindings(List<LetBind> acc, ParseState st) => (is_ident(current_kind(st)) ? parse_let_binding(acc, st) : (is_in_keyword(current_kind(st)) ? ((ParseState st2 = skip_newlines(advance(st))) is var _ ? ((ParseExprResult body = parse_expr(st2)) is var _ ? unwrap_expr_ok(body, (_p0_) => (_p1_) => finish_let(acc, _p0_, _p1_)) : default) : default) : ((ParseExprResult body = parse_expr(st)) is var _ ? unwrap_expr_ok(body, (_p0_) => (_p1_) => finish_let(acc, _p0_, _p1_)) : default)));

    public static ParseExprResult finish_let(List<LetBind> acc, Expr b, ParseState st) => new ExprOk(new LetExpr(acc, b), st);

    public static ParseExprResult parse_let_binding(List<LetBind> acc, ParseState st) => ((Token name_tok = current(st)) is var _ ? ((ParseState st2 = advance(st)) is var _ ? ((ParseState st3 = expect(Equals, st2)) is var _ ? ((ParseExprResult val_result = parse_expr(st3)) is var _ ? unwrap_expr_ok(val_result, (_p0_) => (_p1_) => finish_let_binding(acc, name_tok, _p0_, _p1_)) : default) : default) : default) : default);

    public static ParseExprResult finish_let_binding(List<LetBind> acc, Token name_tok, Expr v, ParseState st) => ((LetBind binding = new LetBind(name: name_tok, value: v)) is var _ ? ((ParseState st2 = skip_newlines(st)) is var _ ? (is_comma(current_kind(st2)) ? parse_let_bindings((acc + new List<LetBind> { binding }), skip_newlines(advance(st2))) : parse_let_bindings((acc + new List<LetBind> { binding }), st2)) : default) : default);

    public static ParseExprResult parse_match_expr(ParseState st) => ((ParseState st2 = advance(st)) is var _ ? ((ParseExprResult scrut = parse_expr(st2)) is var _ ? unwrap_expr_ok(scrut, start_match_branches) : default) : default);

    public static ParseExprResult start_match_branches(Expr s, ParseState st) => ((ParseState st2 = skip_newlines(st)) is var _ ? parse_match_branches(s, new List<MatchArm>(), st2) : default);

    public static ParseExprResult parse_match_branches(Expr scrut, List<MatchArm> acc, ParseState st) => (is_if_keyword(current_kind(st)) ? parse_one_match_branch(scrut, acc, st) : new ExprOk(new MatchExpr(scrut, acc), st));

    public static ParseExprResult unwrap_pat_for_expr(ParsePatResult r, Func<Pat, Func<ParseState, ParseExprResult>> f) => r switch { PatOk(var p, var st) => f(p)(st),  };

    public static ParseExprResult parse_one_match_branch(Expr scrut, List<MatchArm> acc, ParseState st) => ((ParseState st2 = advance(st)) is var _ ? ((ParsePatResult pat = parse_pattern(st2)) is var _ ? unwrap_pat_for_expr(pat, (_p0_) => (_p1_) => parse_match_branch_body(scrut, acc, _p0_, _p1_)) : default) : default);

    public static ParseExprResult parse_match_branch_body(Expr scrut, List<MatchArm> acc, Pat p, ParseState st) => ((ParseState st2 = expect(Arrow, st)) is var _ ? ((ParseState st3 = skip_newlines(st2)) is var _ ? ((ParseExprResult body = parse_expr(st3)) is var _ ? unwrap_expr_ok(body, (_p0_) => (_p1_) => finish_match_branch(scrut, acc, p, _p0_, _p1_)) : default) : default) : default);

    public static ParseExprResult finish_match_branch(Expr scrut, List<MatchArm> acc, Pat p, Expr b, ParseState st) => ((MatchArm arm = new MatchArm(pattern: p, body: b)) is var _ ? ((ParseState st2 = skip_newlines(st)) is var _ ? parse_match_branches(scrut, (acc + new List<MatchArm> { arm }), st2) : default) : default);

    public static ParseExprResult parse_do_expr(ParseState st) => ((ParseState st2 = skip_newlines(advance(st))) is var _ ? parse_do_stmts(new List<DoStmt>(), st2) : default);

    public static ParseExprResult parse_do_stmts(List<DoStmt> acc, ParseState st) => (is_done(st) ? new ExprOk(new DoExpr(acc), st) : (is_dedent(current_kind(st)) ? new ExprOk(new DoExpr(acc), st) : (is_do_bind(st) ? parse_do_bind_stmt(acc, st) : parse_do_expr_stmt(acc, st))));

    public static bool is_do_bind(ParseState st) => (is_ident(current_kind(st)) ? is_left_arrow(peek_kind(st, 1)) : false);

    public static ParseExprResult parse_do_bind_stmt(List<DoStmt> acc, ParseState st) => ((Token name_tok = current(st)) is var _ ? ((ParseState st2 = advance(advance(st))) is var _ ? ((ParseExprResult val_result = parse_expr(st2)) is var _ ? unwrap_expr_ok(val_result, (_p0_) => (_p1_) => finish_do_bind(acc, name_tok, _p0_, _p1_)) : default) : default) : default);

    public static ParseExprResult finish_do_bind(List<DoStmt> acc, Token name_tok, Expr v, ParseState st) => ((ParseState st2 = skip_newlines(st)) is var _ ? parse_do_stmts((acc + new List<DoStmt> { new DoBindStmt(name_tok, v) }), st2) : default);

    public static ParseExprResult parse_do_expr_stmt(List<DoStmt> acc, ParseState st) => ((ParseExprResult expr_result = parse_expr(st)) is var _ ? unwrap_expr_ok(expr_result, (_p0_) => (_p1_) => finish_do_expr(acc, _p0_, _p1_)) : default);

    public static ParseExprResult finish_do_expr(List<DoStmt> acc, Expr e, ParseState st) => ((ParseState st2 = skip_newlines(st)) is var _ ? parse_do_stmts((acc + new List<DoStmt> { new DoExprStmt(e) }), st2) : default);

    public static ParseTypeResult parse_type_annotation(ParseState st) => ((ParseState st2 = advance(st)) is var _ ? ((ParseState st3 = expect(Colon, st2)) is var _ ? parse_type(st3) : default) : default);

    public static ParseDefResult parse_definition(ParseState st) => (is_done(st) ? new DefNone(st) : (is_ident(current_kind(st)) ? try_parse_def(st) : (is_type_ident(current_kind(st)) ? try_parse_def(st) : new DefNone(st))));

    public static ParseDefResult try_parse_def(ParseState st) => (is_colon(peek_kind(st, 1)) ? ((ParseTypeResult ann_result = parse_type_annotation(st)) is var _ ? unwrap_type_for_def(ann_result) : default) : parse_def_body_with_ann(new List<TypeAnn>(), st));

    public static ParseDefResult unwrap_type_for_def(ParseTypeResult r) => r switch { TypeOk(var ann_type, var st) => ((Token name_tok = new Token(kind: Identifier, text: "", offset: 0, line: 0, column: 0)) is var _ ? ((List<TypeAnn> ann = new List<TypeAnn> { new TypeAnn(name: name_tok, type_expr: ann_type) }) is var _ ? ((ParseState st2 = skip_newlines(st)) is var _ ? parse_def_body_with_ann(ann, st2) : default) : default) : default),  };

    public static ParseDefResult parse_def_body_with_ann(List<TypeAnn> ann, ParseState st) => ((Token name_tok = current(st)) is var _ ? ((ParseState st2 = advance(st)) is var _ ? parse_def_params_then(ann, name_tok, new List<Token>(), st2) : default) : default);

    public static ParseDefResult parse_def_params_then(List<TypeAnn> ann, Token name_tok, List<Token> acc, ParseState st) => (is_left_paren(current_kind(st)) ? ((ParseState st2 = advance(st)) is var _ ? (is_ident(current_kind(st2)) ? ((Token param = current(st2)) is var _ ? ((ParseState st3 = advance(st2)) is var _ ? ((ParseState st4 = expect(RightParen, st3)) is var _ ? parse_def_params_then(ann, name_tok, (acc + new List<Token> { param }), st4) : default) : default) : default) : finish_def(ann, name_tok, acc, st)) : default) : finish_def(ann, name_tok, acc, st));

    public static ParseDefResult finish_def(List<TypeAnn> ann, Token name_tok, List<Token> @params, ParseState st) => ((ParseState st2 = expect(Equals, st)) is var _ ? ((ParseState st3 = skip_newlines(st2)) is var _ ? ((ParseExprResult body_result = parse_expr(st3)) is var _ ? unwrap_def_body(body_result, ann, name_tok, @params) : default) : default) : default);

    public static ParseDefResult unwrap_def_body(ParseExprResult r, List<TypeAnn> ann, Token name_tok, List<Token> @params) => r switch { ExprOk(var b, var st) => new DefOk(new Def(name: name_tok, @params: @params, ann: ann, body: b), st),  };

    public static ParseTypeDefResult parse_type_def(ParseState st) => (is_type_ident(current_kind(st)) ? ((Token name_tok = current(st)) is var _ ? ((ParseState st2 = advance(st)) is var _ ? (is_equals(current_kind(st2)) ? ((ParseState st3 = skip_newlines(advance(st2))) is var _ ? (is_record_keyword(current_kind(st3)) ? parse_record_type(name_tok, st3) : (is_pipe(current_kind(st3)) ? parse_variant_type(name_tok, st3) : new TypeDefNone(st))) : default) : new TypeDefNone(st)) : default) : default) : new TypeDefNone(st));

    public static ParseTypeDefResult parse_record_type(Token name_tok, ParseState st) => ((ParseState st2 = advance(st)) is var _ ? ((ParseState st3 = expect(LeftBrace, st2)) is var _ ? ((ParseState st4 = skip_newlines(st3)) is var _ ? parse_record_fields_loop(name_tok, new List<RecordFieldDef>(), st4) : default) : default) : default);

    public static ParseTypeDefResult parse_record_fields_loop(Token name_tok, List<RecordFieldDef> acc, ParseState st) => (is_right_brace(current_kind(st)) ? new TypeDefOk(new TypeDef(name: name_tok, type_params: new List<object>(), body: new RecordBody(acc)), advance(st)) : (is_ident(current_kind(st)) ? parse_one_record_field(name_tok, acc, st) : new TypeDefOk(new TypeDef(name: name_tok, type_params: new List<object>(), body: new RecordBody(acc)), st)));

    public static ParseTypeDefResult parse_one_record_field(Token name_tok, List<RecordFieldDef> acc, ParseState st) => ((Token field_name = current(st)) is var _ ? ((ParseState st2 = advance(st)) is var _ ? ((ParseState st3 = expect(Colon, st2)) is var _ ? ((ParseTypeResult field_type_result = parse_type(st3)) is var _ ? unwrap_record_field_type(name_tok, acc, field_name, field_type_result) : default) : default) : default) : default);

    public static ParseTypeDefResult unwrap_record_field_type(Token name_tok, List<RecordFieldDef> acc, Token field_name, ParseTypeResult r) => r switch { TypeOk(var ft, var st) => ((RecordFieldDef field = new RecordFieldDef(name: field_name, type_expr: ft)) is var _ ? ((ParseState st2 = skip_newlines(st)) is var _ ? (is_comma(current_kind(st2)) ? parse_record_fields_loop(name_tok, (acc + new List<RecordFieldDef> { field }), skip_newlines(advance(st2))) : parse_record_fields_loop(name_tok, (acc + new List<RecordFieldDef> { field }), st2)) : default) : default),  };

    public static ParseTypeDefResult parse_variant_type(Token name_tok, ParseState st) => parse_variant_ctors(name_tok, new List<VariantCtorDef>(), st);

    public static ParseTypeDefResult parse_variant_ctors(Token name_tok, List<VariantCtorDef> acc, ParseState st) => (is_pipe(current_kind(st)) ? ((ParseState st2 = skip_newlines(advance(st))) is var _ ? ((Token ctor_name = current(st2)) is var _ ? ((ParseState st3 = advance(st2)) is var _ ? parse_ctor_fields(ctor_name, new List<TypeExpr>(), st3, name_tok, acc) : default) : default) : default) : new TypeDefOk(new TypeDef(name: name_tok, type_params: new List<object>(), body: new VariantBody(acc)), st));

    public static ParseTypeDefResult parse_ctor_fields(Token ctor_name, List<TypeExpr> fields, ParseState st, Token name_tok, List<VariantCtorDef> acc) => (is_left_paren(current_kind(st)) ? ((ParseTypeResult field_result = parse_type(advance(st))) is var _ ? unwrap_ctor_field(field_result, ctor_name, fields, name_tok, acc) : default) : ((ParseState st2 = skip_newlines(st)) is var _ ? ((VariantCtorDef ctor = new VariantCtorDef(name: ctor_name, fields: fields)) is var _ ? parse_variant_ctors(name_tok, (acc + new List<VariantCtorDef> { ctor }), st2) : default) : default));

    public static ParseTypeDefResult unwrap_ctor_field(ParseTypeResult r, Token ctor_name, List<TypeExpr> fields, Token name_tok, List<VariantCtorDef> acc) => r switch { TypeOk(var ty, var st) => ((ParseState st2 = expect(RightParen, st)) is var _ ? parse_ctor_fields(ctor_name, (fields + new List<TypeExpr> { ty }), st2, name_tok, acc) : default),  };

    public static Document parse_document(ParseState st) => ((ParseState st2 = skip_newlines(st)) is var _ ? parse_top_level(new List<Def>(), new List<TypeDef>(), st2) : default);

    public static Document parse_top_level(List<Def> defs, List<TypeDef> type_defs, ParseState st) => (is_done(st) ? new Document(defs: defs, type_defs: type_defs) : try_top_level_type_def(defs, type_defs, st));

    public static Document try_top_level_type_def(List<Def> defs, List<TypeDef> type_defs, ParseState st) => ((ParseTypeDefResult td_result = parse_type_def(st)) is var _ ? td_result switch { TypeDefOk(var td, var st2) => parse_top_level(defs, (type_defs + new List<TypeDef> { td }), skip_newlines(st2)), TypeDefNone(var st2) => try_top_level_def(defs, type_defs, st),  } : default);

    public static Document try_top_level_def(List<Def> defs, List<TypeDef> type_defs, ParseState st) => ((ParseDefResult def_result = parse_definition(st)) is var _ ? def_result switch { DefOk(var d, var st2) => parse_top_level((defs + new List<Def> { d }), type_defs, skip_newlines(st2)), DefNone(var st2) => parse_top_level(defs, type_defs, skip_newlines(advance(st2))),  } : default);

    public static long token_length(Token t) => text_length(t.text);

    public static CheckResult infer_literal(UnificationState st, LiteralKind kind) => kind switch { IntLit { } => new CheckResult(inferred_type: IntegerTy, state: st), NumLit { } => new CheckResult(inferred_type: NumberTy, state: st), TextLit { } => new CheckResult(inferred_type: TextTy, state: st), BoolLit { } => new CheckResult(inferred_type: BooleanTy, state: st),  };

    public static CheckResult infer_name(UnificationState st, TypeEnv env, string name) => (env_has(env, name) ? ((CodexType raw = env_lookup(env, name)) is var _ ? ((FreshResult inst = instantiate_type(st, raw)) is var _ ? new CheckResult(inferred_type: inst.var_type, state: inst.state) : default) : default) : new CheckResult(inferred_type: ErrorTy, state: add_unify_error(st, "CDX2002", ("Unknown name: " + name))));

    public static FreshResult instantiate_type(UnificationState st, CodexType ty) => ty switch { ForAllTy(var var_id, var body) => ((FreshResult fr = fresh_and_advance(st)) is var _ ? ((CodexType substituted = subst_type_var(body, var_id, fr.var_type)) is var _ ? instantiate_type(fr.state, substituted) : default) : default), _ => new FreshResult(var_type: ty, state: st),  };

    public static CodexType subst_type_var(CodexType ty, long var_id, CodexType replacement) => ty switch { TypeVar(var id) => ((id == var_id) ? replacement : ty), FunTy(var param, var ret) => new FunTy(subst_type_var(param, var_id, replacement), subst_type_var(ret, var_id, replacement)), ListTy(var elem) => new ListTy(subst_type_var(elem, var_id, replacement)), ForAllTy(var inner_id, var body) => ((inner_id == var_id) ? ty : new ForAllTy(inner_id, subst_type_var(body, var_id, replacement))), ConstructedTy(var name, var args) => new ConstructedTy(name, map_subst_type_var(args, var_id, replacement, 0, list_length(args), new List<CodexType>())), SumTy(var name, var ctors) => ty, RecordTy(var name, var fields) => ty, _ => ty,  };

    public static List<CodexType> map_subst_type_var(List<CodexType> args, long var_id, CodexType replacement, long i, long len, List<CodexType> acc) => ((i == len) ? acc : map_subst_type_var(args, var_id, replacement, (i + 1), len, (acc + new List<CodexType> { subst_type_var(list_at(args)(i), var_id, replacement) })));

    public static CheckResult infer_binary(UnificationState st, TypeEnv env, AExpr left, BinaryOp op, AExpr right) => ((CheckResult lr = infer_expr(st, env, left)) is var _ ? ((CheckResult rr = infer_expr(lr.state, env, right)) is var _ ? infer_binary_op(rr.state, lr.inferred_type, rr.inferred_type, op) : default) : default);

    public static CheckResult infer_binary_op(UnificationState st, CodexType lt, CodexType rt, BinaryOp op) => op switch { OpAdd { } => infer_arithmetic(st, lt, rt), OpSub { } => infer_arithmetic(st, lt, rt), OpMul { } => infer_arithmetic(st, lt, rt), OpDiv { } => infer_arithmetic(st, lt, rt), OpPow { } => infer_arithmetic(st, lt, rt), OpEq { } => infer_comparison(st, lt, rt), OpNotEq { } => infer_comparison(st, lt, rt), OpLt { } => infer_comparison(st, lt, rt), OpGt { } => infer_comparison(st, lt, rt), OpLtEq { } => infer_comparison(st, lt, rt), OpGtEq { } => infer_comparison(st, lt, rt), OpAnd { } => infer_logical(st, lt, rt), OpOr { } => infer_logical(st, lt, rt), OpAppend { } => infer_append(st, lt, rt), OpCons { } => infer_cons(st, lt, rt), OpDefEq { } => infer_comparison(st, lt, rt),  };

    public static CheckResult infer_arithmetic(UnificationState st, CodexType lt, CodexType rt) => ((UnifyResult r = unify(st, lt, rt)) is var _ ? new CheckResult(inferred_type: lt, state: r.state) : default);

    public static CheckResult infer_comparison(UnificationState st, CodexType lt, CodexType rt) => ((UnifyResult r = unify(st, lt, rt)) is var _ ? new CheckResult(inferred_type: BooleanTy, state: r.state) : default);

    public static CheckResult infer_logical(UnificationState st, CodexType lt, CodexType rt) => ((UnifyResult r1 = unify(st, lt, BooleanTy)) is var _ ? ((UnifyResult r2 = unify(r1.state, rt, BooleanTy)) is var _ ? new CheckResult(inferred_type: BooleanTy, state: r2.state) : default) : default);

    public static CheckResult infer_append(UnificationState st, CodexType lt, CodexType rt) => ((CodexType resolved = resolve(st, lt)) is var _ ? resolved switch { TextTy { } => ((UnifyResult r = unify(st, rt, TextTy)) is var _ ? new CheckResult(inferred_type: TextTy, state: r.state) : default), _ => ((UnifyResult r = unify(st, lt, rt)) is var _ ? new CheckResult(inferred_type: lt, state: r.state) : default),  } : default);

    public static CheckResult infer_cons(UnificationState st, CodexType lt, CodexType rt) => ((CodexType list_ty = new ListTy(lt)) is var _ ? ((UnifyResult r = unify(st, rt, list_ty)) is var _ ? new CheckResult(inferred_type: list_ty, state: r.state) : default) : default);

    public static CheckResult infer_if(UnificationState st, TypeEnv env, AExpr cond, AExpr then_e, AExpr else_e) => ((CheckResult cr = infer_expr(st, env, cond)) is var _ ? ((UnifyResult r1 = unify(cr.state, cr.inferred_type, BooleanTy)) is var _ ? ((CheckResult tr = infer_expr(r1.state, env, then_e)) is var _ ? ((CheckResult er = infer_expr(tr.state, env, else_e)) is var _ ? ((UnifyResult r2 = unify(er.state, tr.inferred_type, er.inferred_type)) is var _ ? new CheckResult(inferred_type: tr.inferred_type, state: r2.state) : default) : default) : default) : default) : default);

    public static CheckResult infer_let(UnificationState st, TypeEnv env, List<ALetBind> bindings, AExpr body) => ((LetBindResult env2 = infer_let_bindings(st, env, bindings, 0, list_length(bindings))) is var _ ? infer_expr(env2.state, env2.env, body) : default);

    public static LetBindResult infer_let_bindings(UnificationState st, TypeEnv env, List<ALetBind> bindings, long i, long len) => ((i == len) ? new LetBindResult(state: st, env: env) : ((ALetBind b = list_at(bindings)(i)) is var _ ? ((CheckResult vr = infer_expr(st, env, b.value)) is var _ ? ((TypeEnv env2 = env_bind(env, b.name.value, vr.inferred_type)) is var _ ? infer_let_bindings(vr.state, env2, bindings, (i + 1), len) : default) : default) : default));

    public static CheckResult infer_lambda(UnificationState st, TypeEnv env, List<Name> @params, AExpr body) => ((LambdaBindResult pr = bind_lambda_params(st, env, @params, 0, list_length(@params), new List<CodexType>())) is var _ ? ((CheckResult br = infer_expr(pr.state, pr.env, body)) is var _ ? ((CodexType fun_ty = wrap_fun_type(pr.param_types, br.inferred_type)) is var _ ? new CheckResult(inferred_type: fun_ty, state: br.state) : default) : default) : default);

    public static LambdaBindResult bind_lambda_params(UnificationState st, TypeEnv env, List<Name> @params, long i, long len, List<CodexType> acc) => ((i == len) ? new LambdaBindResult(state: st, env: env, param_types: acc) : ((Name p = list_at(@params)(i)) is var _ ? ((FreshResult fr = fresh_and_advance(st)) is var _ ? ((TypeEnv env2 = env_bind(env, p.value, fr.var_type)) is var _ ? bind_lambda_params(fr.state, env2, @params, (i + 1), len, (acc + new List<CodexType> { fr.var_type })) : default) : default) : default));

    public static CodexType wrap_fun_type(List<CodexType> param_types, CodexType result) => wrap_fun_type_loop(param_types, result, (list_length(param_types) - 1));

    public static CodexType wrap_fun_type_loop(List<CodexType> param_types, CodexType result, long i) => ((i < 0) ? result : wrap_fun_type_loop(param_types, new FunTy(list_at(param_types)(i), result), (i - 1)));

    public static CheckResult infer_application(UnificationState st, TypeEnv env, AExpr func, AExpr arg) => ((CheckResult fr = infer_expr(st, env, func)) is var _ ? ((CheckResult ar = infer_expr(fr.state, env, arg)) is var _ ? ((FreshResult ret = fresh_and_advance(ar.state)) is var _ ? ((UnifyResult r = unify(ret.state, fr.inferred_type, new FunTy(ar.inferred_type, ret.var_type))) is var _ ? new CheckResult(inferred_type: ret.var_type, state: r.state) : default) : default) : default) : default);

    public static CheckResult infer_list(UnificationState st, TypeEnv env, List<AExpr> elems) => ((list_length(elems) == 0) ? ((FreshResult fr = fresh_and_advance(st)) is var _ ? new CheckResult(inferred_type: new ListTy(fr.var_type), state: fr.state) : default) : ((CheckResult first = infer_expr(st, env, list_at(elems)(0))) is var _ ? ((UnificationState st2 = unify_list_elems(first.state, env, elems, first.inferred_type, 1, list_length(elems))) is var _ ? new CheckResult(inferred_type: new ListTy(first.inferred_type), state: st2) : default) : default));

    public static UnificationState unify_list_elems(UnificationState st, TypeEnv env, List<AExpr> elems, CodexType elem_ty, long i, long len) => ((i == len) ? st : ((CheckResult er = infer_expr(st, env, list_at(elems)(i))) is var _ ? ((UnifyResult r = unify(er.state, er.inferred_type, elem_ty)) is var _ ? unify_list_elems(r.state, env, elems, elem_ty, (i + 1), len) : default) : default));

    public static CheckResult infer_match(UnificationState st, TypeEnv env, AExpr scrutinee, List<AMatchArm> arms) => ((CheckResult sr = infer_expr(st, env, scrutinee)) is var _ ? ((FreshResult fr = fresh_and_advance(sr.state)) is var _ ? ((UnificationState st2 = infer_match_arms(fr.state, env, sr.inferred_type, fr.var_type, arms, 0, list_length(arms))) is var _ ? new CheckResult(inferred_type: fr.var_type, state: st2) : default) : default) : default);

    public static UnificationState infer_match_arms(UnificationState st, TypeEnv env, CodexType scrut_ty, CodexType result_ty, List<AMatchArm> arms, long i, long len) => ((i == len) ? st : ((AMatchArm arm = list_at(arms)(i)) is var _ ? ((PatBindResult pr = bind_pattern(st, env, arm.pattern, scrut_ty)) is var _ ? ((CheckResult br = infer_expr(pr.state, pr.env, arm.body)) is var _ ? ((UnifyResult r = unify(br.state, br.inferred_type, result_ty)) is var _ ? infer_match_arms(r.state, env, scrut_ty, result_ty, arms, (i + 1), len) : default) : default) : default) : default));

    public static PatBindResult bind_pattern(UnificationState st, TypeEnv env, APat pat, CodexType ty) => pat switch { AVarPat(var name) => new PatBindResult(state: st, env: env_bind(env, name.value, ty)), AWildPat { } => new PatBindResult(state: st, env: env), ALitPat(var val, var kind) => new PatBindResult(state: st, env: env), ACtorPat(var ctor_name, var sub_pats) => ((FreshResult ctor_lookup = instantiate_type(st, env_lookup(env, ctor_name.value))) is var _ ? bind_ctor_sub_patterns(ctor_lookup.state, env, sub_pats, ctor_lookup.var_type, 0, list_length(sub_pats)) : default),  };

    public static PatBindResult bind_ctor_sub_patterns(UnificationState st, TypeEnv env, List<APat> sub_pats, CodexType ctor_ty, long i, long len) => ((i == len) ? new PatBindResult(state: st, env: env) : ctor_ty switch { FunTy(var param_ty, var ret_ty) => ((PatBindResult pr = bind_pattern(st, env, list_at(sub_pats)(i), param_ty)) is var _ ? bind_ctor_sub_patterns(pr.state, pr.env, sub_pats, ret_ty, (i + 1), len) : default), _ => ((FreshResult fr = fresh_and_advance(st)) is var _ ? ((PatBindResult pr = bind_pattern(fr.state, env, list_at(sub_pats)(i), fr.var_type)) is var _ ? bind_ctor_sub_patterns(pr.state, pr.env, sub_pats, ctor_ty, (i + 1), len) : default) : default),  });

    public static CheckResult infer_do(UnificationState st, TypeEnv env, List<ADoStmt> stmts) => infer_do_loop(st, env, stmts, 0, list_length(stmts), NothingTy);

    public static CheckResult infer_do_loop(UnificationState st, TypeEnv env, List<ADoStmt> stmts, long i, long len, CodexType last_ty) => ((i == len) ? new CheckResult(inferred_type: last_ty, state: st) : ((ADoStmt stmt = list_at(stmts)(i)) is var _ ? stmt switch { ADoExprStmt(var e) => ((CheckResult er = infer_expr(st, env, e)) is var _ ? infer_do_loop(er.state, env, stmts, (i + 1), len, er.inferred_type) : default), ADoBindStmt(var name, var e) => ((CheckResult er = infer_expr(st, env, e)) is var _ ? ((TypeEnv env2 = env_bind(env, name.value, er.inferred_type)) is var _ ? infer_do_loop(er.state, env2, stmts, (i + 1), len, er.inferred_type) : default) : default),  } : default));

    public static CheckResult infer_expr(UnificationState st, TypeEnv env, AExpr expr) => expr switch { ALitExpr(var val, var kind) => infer_literal(st, kind), ANameExpr(var name) => infer_name(st, env, name.value), ABinaryExpr(var left, var op, var right) => infer_binary(st, env, left, op, right), AUnaryExpr(var operand) => ((CheckResult r = infer_expr(st, env, operand)) is var _ ? ((UnifyResult u = unify(r.state, r.inferred_type, IntegerTy)) is var _ ? new CheckResult(inferred_type: IntegerTy, state: u.state) : default) : default), AApplyExpr(var func, var arg) => infer_application(st, env, func, arg), AIfExpr(var cond, var then_e, var else_e) => infer_if(st, env, cond, then_e, else_e), ALetExpr(var bindings, var body) => infer_let(st, env, bindings, body), ALambdaExpr(var @params, var body) => infer_lambda(st, env, @params, body), AMatchExpr(var scrutinee, var arms) => infer_match(st, env, scrutinee, arms), AListExpr(var elems) => infer_list(st, env, elems), ADoExpr(var stmts) => infer_do(st, env, stmts), AFieldAccess(var obj, var field) => ((CheckResult r = infer_expr(st, env, obj)) is var _ ? ((CodexType resolved = deep_resolve(r.state, r.inferred_type)) is var _ ? resolved switch { RecordTy(var rname, var rfields) => ((CodexType ftype = lookup_record_field(rfields, field.value)) is var _ ? new CheckResult(inferred_type: ftype, state: r.state) : default), ConstructedTy(var cname, var cargs) => ((CodexType record_type = resolve_constructed_to_record(env, cname.value)) is var _ ? record_type switch { RecordTy(var rname, var rfields) => ((CodexType ftype = lookup_record_field(rfields, field.value)) is var _ ? new CheckResult(inferred_type: ftype, state: r.state) : default), _ => ((FreshResult fr = fresh_and_advance(r.state)) is var _ ? new CheckResult(inferred_type: fr.var_type, state: fr.state) : default), _ => ((FreshResult fr = fresh_and_advance(r.state)) is var _ ? new CheckResult(inferred_type: fr.var_type, state: fr.state) : default), ARecordExpr(var name, var fields) => ((UnificationState st2 = infer_record_fields(st, env, fields, 0, list_length(fields))) is var _ ? ((object ctor_type = (env_has(env, name.value) ? env_lookup(env, name.value) : ErrorTy)) is var _ ? ((CodexType result_type = strip_fun_args(ctor_type)) is var _ ? new CheckResult(inferred_type: result_type, state: st2) : default) : default) : default), AErrorExpr(var msg) => new CheckResult(inferred_type: ErrorTy, state: st),  } : default),  } : default) : default),  };

    public static CodexType resolve_constructed_to_record(TypeEnv env, string name) => (env_has(env, name) ? strip_fun_args(env_lookup(env, name)) : ErrorTy);

    public static UnificationState infer_record_fields(UnificationState st, TypeEnv env, List<AFieldExpr> fields, long i, long len) => ((i == len) ? st : ((AFieldExpr f = list_at(fields)(i)) is var _ ? ((CheckResult r = infer_expr(st, env, f.value)) is var _ ? infer_record_fields(r.state, env, fields, (i + 1), len) : default) : default));

    public static CodexType strip_fun_args(CodexType ty) => ty switch { FunTy(var p, var r) => strip_fun_args(r), _ => ty,  };

    public static CodexType resolve_type_expr(List<TypeBinding> tdm, ATypeExpr texpr) => texpr switch { ANamedType(var name) => resolve_type_name(tdm, name.value), AFunType(var param, var ret) => new FunTy(resolve_type_expr(tdm, param), resolve_type_expr(tdm, ret)), AAppType(var ctor, var args) => resolve_applied_type(tdm, ctor, args),  };

    public static CodexType resolve_applied_type(List<TypeBinding> tdm, ATypeExpr ctor, List<ATypeExpr> args) => ctor switch { ANamedType(var name) => ((name.value == "List") ? ((list_length(args) == 1) ? new ListTy(resolve_type_expr(tdm, list_at(args)(0))) : new ListTy(ErrorTy)) : new ConstructedTy(name, resolve_type_expr_list(tdm, args, 0, list_length(args), new List<CodexType>()))), _ => resolve_type_expr(tdm, ctor),  };

    public static List<CodexType> resolve_type_expr_list(List<TypeBinding> tdm, List<ATypeExpr> args, long i, long len, List<CodexType> acc) => ((i == len) ? acc : resolve_type_expr_list(tdm, args, (i + 1), len, (acc + new List<CodexType> { resolve_type_expr(tdm, list_at(args)(i)) })));

    public static CodexType resolve_type_name(List<TypeBinding> tdm, string name) => ((name == "Integer") ? IntegerTy : ((name == "Number") ? NumberTy : ((name == "Text") ? TextTy : ((name == "Boolean") ? BooleanTy : ((name == "Nothing") ? NothingTy : lookup_type_def(tdm, name))))));

    public static CodexType lookup_type_def(List<TypeBinding> tdm, string name) => lookup_type_def_loop(tdm, name, 0, list_length(tdm));

    public static CodexType lookup_type_def_loop(List<TypeBinding> tdm, string name, long i, long len) => ((i == len) ? new ConstructedTy(new Name(value: name), new List<CodexType>()) : ((TypeBinding b = list_at(tdm)(i)) is var _ ? ((b.name == name) ? b.bound_type : lookup_type_def_loop(tdm, name, (i + 1), len)) : default));

    public static bool is_value_name(string name) => ((text_length(name) == 0) ? false : ((long code = char_code(char_at(name)(0))) is var _ ? ((code >= 97) && (code <= 122)) : default));

    public static ParamResult parameterize_type(UnificationState st, CodexType ty) => ((WalkResult r = parameterize_walk(st, new List<ParamEntry>(), ty)) is var _ ? ((CodexType wrapped = wrap_forall_from_entries(r.walked, r.entries, 0, list_length(r.entries))) is var _ ? new ParamResult(parameterized: wrapped, entries: r.entries, state: r.state) : default) : default);

    public static CodexType wrap_forall_from_entries(CodexType ty, List<ParamEntry> entries, long i, long len) => ((i == len) ? ty : ((ParamEntry e = list_at(entries)(i)) is var _ ? new ForAllTy(e.var_id, wrap_forall_from_entries(ty, entries, (i + 1), len)) : default));

    public static WalkResult parameterize_walk(UnificationState st, List<ParamEntry> entries, CodexType ty) => ty switch { ConstructedTy(var name, var args) => (((list_length(args) == 0) && is_value_name(name.value)) ? ((long looked = find_param_entry(entries, name.value, 0, list_length(entries))) is var _ ? ((looked >= 0) ? new WalkResult(walked: new TypeVar(looked), entries: entries, state: st) : ((FreshResult fr = fresh_and_advance(st)) is var _ ? fr.var_type switch { TypeVar(var new_id) => ((ParamEntry new_entry = new ParamEntry(param_name: name.value, var_id: new_id)) is var _ ? new WalkResult(walked: fr.var_type, entries: (entries + new List<ParamEntry> { new_entry }), state: fr.state) : default), _ => new WalkResult(walked: ty, entries: entries, state: fr.state),  } : default)) : default) : ((WalkListResult args_r = parameterize_walk_list(st, entries, args, 0, list_length(args), new List<CodexType>())) is var _ ? new WalkResult(walked: new ConstructedTy(name, args_r.walked_list), entries: args_r.entries, state: args_r.state) : default)), FunTy(var param, var ret) => ((WalkResult pr = parameterize_walk(st, entries, param)) is var _ ? ((WalkResult rr = parameterize_walk(pr.state, pr.entries, ret)) is var _ ? new WalkResult(walked: new FunTy(pr.walked, rr.walked), entries: rr.entries, state: rr.state) : default) : default), ListTy(var elem) => ((WalkResult er = parameterize_walk(st, entries, elem)) is var _ ? new WalkResult(walked: new ListTy(er.walked), entries: er.entries, state: er.state) : default), ForAllTy(var id, var body) => ((WalkResult br = parameterize_walk(st, entries, body)) is var _ ? new WalkResult(walked: new ForAllTy(id, br.walked), entries: br.entries, state: br.state) : default), _ => new WalkResult(walked: ty, entries: entries, state: st),  };

    public static long find_param_entry(List<ParamEntry> entries, string name, long i, long len) => ((i == len) ? (0 - 1) : ((ParamEntry e = list_at(entries)(i)) is var _ ? ((e.param_name == name) ? e.var_id : find_param_entry(entries, name, (i + 1), len)) : default));

    public static WalkListResult parameterize_walk_list(UnificationState st, List<ParamEntry> entries, List<CodexType> args, long i, long len, List<CodexType> acc) => ((i == len) ? new WalkListResult(walked_list: acc, entries: entries, state: st) : ((WalkResult r = parameterize_walk(st, entries, list_at(args)(i))) is var _ ? parameterize_walk_list(r.state, r.entries, args, (i + 1), len, (acc + new List<CodexType> { r.walked })) : default));

    public static CheckResult check_def(UnificationState st, TypeEnv env, ADef def) => ((DefSetup declared = resolve_declared_type(st, env, def)) is var _ ? ((DefParamResult env2 = bind_def_params(declared.state, declared.env, def.@params, declared.expected_type, 0, list_length(def.@params))) is var _ ? ((CheckResult body_r = infer_expr(env2.state, env2.env, def.body)) is var _ ? ((UnifyResult u = unify(body_r.state, env2.remaining_type, body_r.inferred_type)) is var _ ? new CheckResult(inferred_type: declared.expected_type, state: u.state) : default) : default) : default) : default);

    public static DefSetup resolve_declared_type(UnificationState st, TypeEnv env, ADef def) => ((list_length(def.declared_type) == 0) ? ((FreshResult fr = fresh_and_advance(st)) is var _ ? new DefSetup(expected_type: fr.var_type, remaining_type: fr.var_type, state: fr.state, env: env) : default) : ((CodexType env_type = env_lookup(env, def.name.value)) is var _ ? ((FreshResult inst = instantiate_type(st, env_type)) is var _ ? new DefSetup(expected_type: inst.var_type, remaining_type: inst.var_type, state: inst.state, env: env) : default) : default));

    public static DefParamResult bind_def_params(UnificationState st, TypeEnv env, List<AParam> @params, CodexType remaining, long i, long len) => ((i == len) ? new DefParamResult(state: st, env: env, remaining_type: remaining) : ((AParam p = list_at(@params)(i)) is var _ ? remaining switch { FunTy(var param_ty, var ret_ty) => ((TypeEnv env2 = env_bind(env, p.name.value, param_ty)) is var _ ? bind_def_params(st, env2, @params, ret_ty, (i + 1), len) : default), _ => ((FreshResult fr = fresh_and_advance(st)) is var _ ? ((TypeEnv env2 = env_bind(env, p.name.value, fr.var_type)) is var _ ? bind_def_params(fr.state, env2, @params, remaining, (i + 1), len) : default) : default),  } : default));

    public static ModuleResult check_module(AModule mod) => ((List<TypeBinding> tdm = build_type_def_map(mod.type_defs, 0, list_length(mod.type_defs), new List<TypeBinding>())) is var _ ? ((LetBindResult tenv = register_type_defs(empty_unification_state, builtin_type_env, tdm, mod.type_defs, 0, list_length(mod.type_defs))) is var _ ? ((LetBindResult env = register_all_defs(tenv.state, tenv.env, tdm, mod.defs, 0, list_length(mod.defs))) is var _ ? check_all_defs(env.state, env.env, mod.defs, 0, list_length(mod.defs), new List<TypeBinding>()) : default) : default) : default);

    public static LetBindResult register_all_defs(UnificationState st, TypeEnv env, List<TypeBinding> tdm, List<ADef> defs, long i, long len) => ((i == len) ? new LetBindResult(state: st, env: env) : ((ADef def = list_at(defs)(i)) is var _ ? ((object ty = ((list_length(def.declared_type) == 0) ? ((FreshResult fr = fresh_and_advance(st)) is var _ ? ((TypeEnv env2 = env_bind(env, def.name.value, fr.var_type)) is var _ ? new LetBindResult(state: fr.state, env: env2) : default) : default) : ((CodexType resolved = resolve_type_expr(tdm, list_at(def.declared_type)(0))) is var _ ? ((ParamResult pr = parameterize_type(st, resolved)) is var _ ? new LetBindResult(state: pr.state, env: env_bind(env, def.name.value, pr.parameterized)) : default) : default))) is var _ ? register_all_defs(ty.state, ty.env, tdm, defs, (i + 1), len) : default) : default));

    public static ModuleResult check_all_defs(UnificationState st, TypeEnv env, List<ADef> defs, long i, long len, List<TypeBinding> acc) => ((i == len) ? new ModuleResult(types: acc, state: st) : ((ADef def = list_at(defs)(i)) is var _ ? ((CheckResult r = check_def(st, env, def)) is var _ ? ((CodexType resolved = deep_resolve(r.state, r.inferred_type)) is var _ ? ((TypeBinding entry = new TypeBinding(name: def.name.value, bound_type: resolved)) is var _ ? check_all_defs(r.state, env, defs, (i + 1), len, (acc + new List<TypeBinding> { entry })) : default) : default) : default) : default));

    public static List<TypeBinding> build_type_def_map(List<ATypeDef> tdefs, long i, long len, List<TypeBinding> acc) => ((i == len) ? acc : ((ATypeDef td = list_at(tdefs)(i)) is var _ ? ((object entry = td switch { AVariantTypeDef(var name, var type_params, var ctors) => ((List<SumCtor> sum_ctors = build_sum_ctors(tdefs, ctors, 0, list_length(ctors), new List<SumCtor>(), acc)) is var _ ? new TypeBinding(name: name.value, bound_type: new SumTy(name, sum_ctors)) : default), ARecordTypeDef(var name, var type_params, var fields) => ((List<RecordField> rec_fields = build_record_fields_for_map(tdefs, fields, 0, list_length(fields), new List<RecordField>(), acc)) is var _ ? new TypeBinding(name: name.value, bound_type: new RecordTy(name, rec_fields)) : default),  }) is var _ ? build_type_def_map(tdefs, (i + 1), len, (acc + new List<TypeBinding> { entry })) : default) : default));

    public static List<SumCtor> build_sum_ctors(List<ATypeDef> tdefs, List<AVariantCtorDef> ctors, long i, long len, List<SumCtor> acc, List<TypeBinding> partial_tdm) => ((i == len) ? acc : ((AVariantCtorDef c = list_at(ctors)(i)) is var _ ? ((List<CodexType> field_types = resolve_type_expr_list_for_map(tdefs, c.fields, 0, list_length(c.fields), new List<CodexType>(), partial_tdm)) is var _ ? ((SumCtor sc = new SumCtor(name: c.name, fields: field_types)) is var _ ? build_sum_ctors(tdefs, ctors, (i + 1), len, (acc + new List<SumCtor> { sc }), partial_tdm) : default) : default) : default));

    public static List<RecordField> build_record_fields_for_map(List<ATypeDef> tdefs, List<ARecordFieldDef> fields, long i, long len, List<RecordField> acc, List<TypeBinding> partial_tdm) => ((i == len) ? acc : ((ARecordFieldDef f = list_at(fields)(i)) is var _ ? ((RecordField rfield = new RecordField(name: f.name, type_val: resolve_type_expr(partial_tdm, f.type_expr))) is var _ ? build_record_fields_for_map(tdefs, fields, (i + 1), len, (acc + new List<RecordField> { rfield }), partial_tdm) : default) : default));

    public static List<CodexType> resolve_type_expr_list_for_map(List<ATypeDef> tdefs, List<ATypeExpr> args, long i, long len, List<CodexType> acc, List<TypeBinding> partial_tdm) => ((i == len) ? acc : resolve_type_expr_list_for_map(tdefs, args, (i + 1), len, (acc + new List<CodexType> { resolve_type_expr(partial_tdm, list_at(args)(i)) }), partial_tdm));

    public static LetBindResult register_type_defs(UnificationState st, TypeEnv env, List<TypeBinding> tdm, List<ATypeDef> tdefs, long i, long len) => ((i == len) ? new LetBindResult(state: st, env: env) : ((ATypeDef td = list_at(tdefs)(i)) is var _ ? ((LetBindResult r = register_one_type_def(st, env, tdm, td)) is var _ ? register_type_defs(r.state, r.env, tdm, tdefs, (i + 1), len) : default) : default));

    public static LetBindResult register_one_type_def(UnificationState st, TypeEnv env, List<TypeBinding> tdm, ATypeDef td) => td switch { AVariantTypeDef(var name, var type_params, var ctors) => ((CodexType result_ty = lookup_type_def(tdm, name.value)) is var _ ? register_variant_ctors(st, env, tdm, ctors, result_ty, 0, list_length(ctors)) : default), ARecordTypeDef(var name, var type_params, var fields) => ((List<RecordField> resolved_fields = build_record_fields(tdm, fields, 0, list_length(fields), new List<RecordField>())) is var _ ? ((CodexType result_ty = new RecordTy(name, resolved_fields)) is var _ ? ((CodexType ctor_ty = build_record_ctor_type(tdm, fields, result_ty, 0, list_length(fields))) is var _ ? new LetBindResult(state: st, env: env_bind(env, name.value, ctor_ty)) : default) : default) : default),  };

    public static List<RecordField> build_record_fields(List<TypeBinding> tdm, List<ARecordFieldDef> fields, long i, long len, List<RecordField> acc) => ((i == len) ? acc : ((ARecordFieldDef f = list_at(fields)(i)) is var _ ? ((RecordField rfield = new RecordField(name: f.name, type_val: resolve_type_expr(tdm, f.type_expr))) is var _ ? build_record_fields(tdm, fields, (i + 1), len, (acc + new List<RecordField> { rfield })) : default) : default));

    public static CodexType lookup_record_field(List<RecordField> fields, string name) => ((list_length(fields) == 0) ? ErrorTy : ((RecordField f = list_at(fields)(0)) is var _ ? ((f.name.value == name) ? f.type_val : lookup_record_field_loop(fields, name, 1, list_length(fields))) : default));

    public static CodexType lookup_record_field_loop(List<RecordField> fields, string name, long i, long len) => ((i == len) ? ErrorTy : ((RecordField f = list_at(fields)(i)) is var _ ? ((f.name.value == name) ? f.type_val : lookup_record_field_loop(fields, name, (i + 1), len)) : default));

    public static LetBindResult register_variant_ctors(UnificationState st, TypeEnv env, List<TypeBinding> tdm, List<AVariantCtorDef> ctors, CodexType result_ty, long i, long len) => ((i == len) ? new LetBindResult(state: st, env: env) : ((AVariantCtorDef ctor = list_at(ctors)(i)) is var _ ? ((CodexType ctor_ty = build_ctor_type(tdm, ctor.fields, result_ty, 0, list_length(ctor.fields))) is var _ ? ((TypeEnv env2 = env_bind(env, ctor.name.value, ctor_ty)) is var _ ? register_variant_ctors(st, env2, tdm, ctors, result_ty, (i + 1), len) : default) : default) : default));

    public static CodexType build_ctor_type(List<TypeBinding> tdm, List<ATypeExpr> fields, CodexType result, long i, long len) => ((i == len) ? result : ((CodexType rest = build_ctor_type(tdm, fields, result, (i + 1), len)) is var _ ? new FunTy(resolve_type_expr(tdm, list_at(fields)(i)), rest) : default));

    public static CodexType build_record_ctor_type(List<TypeBinding> tdm, List<ARecordFieldDef> fields, CodexType result, long i, long len) => ((i == len) ? result : ((ARecordFieldDef f = list_at(fields)(i)) is var _ ? ((CodexType rest = build_record_ctor_type(tdm, fields, result, (i + 1), len)) is var _ ? new FunTy(resolve_type_expr(tdm, f.type_expr), rest) : default) : default));

    public static TypeEnv empty_type_env() => new TypeEnv(bindings: new List<object>());

    public static CodexType env_lookup(TypeEnv env, string name) => env_lookup_loop(env.bindings, name, 0, list_length(env.bindings));

    public static CodexType env_lookup_loop(List<TypeBinding> bindings, string name, long i, long len) => ((i == len) ? ErrorTy : ((TypeBinding b = list_at(bindings)(i)) is var _ ? ((b.name == name) ? b.bound_type : env_lookup_loop(bindings, name, (i + 1), len)) : default));

    public static bool env_has(TypeEnv env, string name) => env_has_loop(env.bindings, name, 0, list_length(env.bindings));

    public static bool env_has_loop(List<TypeBinding> bindings, string name, long i, long len) => ((i == len) ? false : ((TypeBinding b = list_at(bindings)(i)) is var _ ? ((b.name == name) ? true : env_has_loop(bindings, name, (i + 1), len)) : default));

    public static TypeEnv env_bind(TypeEnv env, string name, CodexType ty) => new TypeEnv(bindings: (new List<TypeBinding> { new TypeBinding(name: name, bound_type: ty) } + env.bindings));

    public static TypeEnv builtin_type_env() => ((TypeEnv e = empty_type_env) is var _ ? ((TypeEnv e2 = env_bind(e, "negate", new FunTy(IntegerTy, IntegerTy))) is var _ ? ((TypeEnv e3 = env_bind(e2, "text-length", new FunTy(TextTy, IntegerTy))) is var _ ? ((TypeEnv e4 = env_bind(e3, "integer-to-text", new FunTy(IntegerTy, TextTy))) is var _ ? ((TypeEnv e5 = env_bind(e4, "char-at", new FunTy(TextTy, new FunTy(IntegerTy, TextTy)))) is var _ ? ((TypeEnv e6 = env_bind(e5, "substring", new FunTy(TextTy, new FunTy(IntegerTy, new FunTy(IntegerTy, TextTy))))) is var _ ? ((TypeEnv e7 = env_bind(e6, "is-letter", new FunTy(TextTy, BooleanTy))) is var _ ? ((TypeEnv e8 = env_bind(e7, "is-digit", new FunTy(TextTy, BooleanTy))) is var _ ? ((TypeEnv e9 = env_bind(e8, "is-whitespace", new FunTy(TextTy, BooleanTy))) is var _ ? ((TypeEnv e10 = env_bind(e9, "char-code", new FunTy(TextTy, IntegerTy))) is var _ ? ((TypeEnv e11 = env_bind(e10, "code-to-char", new FunTy(IntegerTy, TextTy))) is var _ ? ((TypeEnv e12 = env_bind(e11, "text-replace", new FunTy(TextTy, new FunTy(TextTy, new FunTy(TextTy, TextTy))))) is var _ ? ((TypeEnv e13 = env_bind(e12, "text-to-integer", new FunTy(TextTy, IntegerTy))) is var _ ? ((TypeEnv e14 = env_bind(e13, "show", new ForAllTy(0, new FunTy(new TypeVar(0), TextTy)))) is var _ ? ((TypeEnv e15 = env_bind(e14, "print-line", new FunTy(TextTy, NothingTy))) is var _ ? ((TypeEnv e16 = env_bind(e15, "list-length", new ForAllTy(0, new FunTy(new ListTy(new TypeVar(0)), IntegerTy)))) is var _ ? ((TypeEnv e17 = env_bind(e16, "list-at", new ForAllTy(0, new FunTy(new ListTy(new TypeVar(0)), new FunTy(IntegerTy, new TypeVar(0)))))) is var _ ? ((TypeEnv e18 = env_bind(e17, "map", new ForAllTy(0, new ForAllTy(1, new FunTy(new FunTy(new TypeVar(0), new TypeVar(1)), new FunTy(new ListTy(new TypeVar(0)), new ListTy(new TypeVar(1)))))))) is var _ ? ((TypeEnv e19 = env_bind(e18, "filter", new ForAllTy(0, new FunTy(new FunTy(new TypeVar(0), BooleanTy), new FunTy(new ListTy(new TypeVar(0)), new ListTy(new TypeVar(0))))))) is var _ ? ((TypeEnv e20 = env_bind(e19, "fold", new ForAllTy(0, new ForAllTy(1, new FunTy(new FunTy(new TypeVar(1), new FunTy(new TypeVar(0), new TypeVar(1))), new FunTy(new TypeVar(1), new FunTy(new ListTy(new TypeVar(0)), new TypeVar(1)))))))) is var _ ? ((TypeEnv e21 = env_bind(e20, "read-line", TextTy)) is var _ ? e21 : default) : default) : default) : default) : default) : default) : default) : default) : default) : default) : default) : default) : default) : default) : default) : default) : default) : default) : default) : default) : default);

    public static UnificationState empty_unification_state() => new UnificationState(substitutions: new List<object>(), next_id: 2, errors: new List<object>());

    public static CodexType fresh_var(UnificationState st) => new TypeVar(st.next_id);

    public static UnificationState advance_id(UnificationState st) => new UnificationState(substitutions: st.substitutions, next_id: (st.next_id + 1), errors: st.errors);

    public static FreshResult fresh_and_advance(UnificationState st) => new FreshResult(var_type: new TypeVar(st.next_id), state: advance_id(st));

    public static CodexType subst_lookup(long var_id, List<SubstEntry> entries) => subst_lookup_loop(var_id, entries, 0, list_length(entries));

    public static CodexType subst_lookup_loop(long var_id, List<SubstEntry> entries, long i, long len) => ((i == len) ? ErrorTy : ((SubstEntry entry = list_at(entries)(i)) is var _ ? ((entry.var_id == var_id) ? entry.resolved_type : subst_lookup_loop(var_id, entries, (i + 1), len)) : default));

    public static bool has_subst(long var_id, List<SubstEntry> entries) => has_subst_loop(var_id, entries, 0, list_length(entries));

    public static bool has_subst_loop(long var_id, List<SubstEntry> entries, long i, long len) => ((i == len) ? false : ((SubstEntry entry = list_at(entries)(i)) is var _ ? ((entry.var_id == var_id) ? true : has_subst_loop(var_id, entries, (i + 1), len)) : default));

    public static CodexType resolve(UnificationState st, CodexType ty) => ty switch { TypeVar(var id) => (has_subst(id, st.substitutions) ? resolve(st, subst_lookup(id, st.substitutions)) : ty), _ => ty,  };

    public static UnificationState add_subst(UnificationState st, long var_id, CodexType ty) => new UnificationState(substitutions: (st.substitutions + new List<SubstEntry> { new SubstEntry(var_id: var_id, resolved_type: ty) }), next_id: st.next_id, errors: st.errors);

    public static UnificationState add_unify_error(UnificationState st, string code, string msg) => new UnificationState(substitutions: st.substitutions, next_id: st.next_id, errors: (st.errors + new List<Diagnostic> { make_error(code, msg) }));

    public static bool occurs_in(UnificationState st, long var_id, CodexType ty) => ((CodexType resolved = resolve(st, ty)) is var _ ? resolved switch { TypeVar(var id) => (id == var_id), FunTy(var param, var ret) => (occurs_in(st, var_id, param) || occurs_in(st, var_id, ret)), ListTy(var elem) => occurs_in(st, var_id, elem), _ => false,  } : default);

    public static UnifyResult unify(UnificationState st, CodexType a, CodexType b) => ((CodexType ra = resolve(st, a)) is var _ ? ((CodexType rb = resolve(st, b)) is var _ ? unify_resolved(st, ra, rb) : default) : default);

    public static UnifyResult unify_resolved(UnificationState st, CodexType a, CodexType b) => (types_equal(a, b) ? new UnifyResult(success: true, state: st) : a switch { TypeVar(var id_a) => (occurs_in(st, id_a, b) ? new UnifyResult(success: false, state: add_unify_error(st, "CDX2010", "Infinite type")) : new UnifyResult(success: true, state: add_subst(st, id_a, b))), _ => unify_rhs(st, a, b),  });

    public static bool types_equal(CodexType a, CodexType b) => a switch { TypeVar(var id_a) => b switch { TypeVar(var id_b) => (id_a == id_b), _ => false, IntegerTy { } => b switch { IntegerTy { } => true, _ => false, NumberTy { } => b switch { NumberTy { } => true, _ => false, TextTy { } => b switch { TextTy { } => true, _ => false, BooleanTy { } => b switch { BooleanTy { } => true, _ => false, NothingTy { } => b switch { NothingTy { } => true, _ => false, VoidTy { } => b switch { VoidTy { } => true, _ => false, ErrorTy { } => b switch { ErrorTy { } => true, _ => false, _ => false,  },  },  },  },  },  },  },  },  };

    public static UnifyResult unify_rhs(UnificationState st, CodexType a, CodexType b) => b switch { TypeVar(var id_b) => (occurs_in(st, id_b, a) ? new UnifyResult(success: false, state: add_unify_error(st, "CDX2010", "Infinite type")) : new UnifyResult(success: true, state: add_subst(st, id_b, a))), _ => unify_structural(st, a, b),  };

    public static UnifyResult unify_structural(UnificationState st, CodexType a, CodexType b) => a switch { IntegerTy { } => b switch { IntegerTy { } => new UnifyResult(success: true, state: st), ErrorTy { } => new UnifyResult(success: true, state: st), _ => unify_mismatch(st, a, b), NumberTy { } => b switch { NumberTy { } => new UnifyResult(success: true, state: st), ErrorTy { } => new UnifyResult(success: true, state: st), _ => unify_mismatch(st, a, b), TextTy { } => b switch { TextTy { } => new UnifyResult(success: true, state: st), ErrorTy { } => new UnifyResult(success: true, state: st), _ => unify_mismatch(st, a, b), BooleanTy { } => b switch { BooleanTy { } => new UnifyResult(success: true, state: st), ErrorTy { } => new UnifyResult(success: true, state: st), _ => unify_mismatch(st, a, b), NothingTy { } => b switch { NothingTy { } => new UnifyResult(success: true, state: st), ErrorTy { } => new UnifyResult(success: true, state: st), _ => unify_mismatch(st, a, b), VoidTy { } => b switch { VoidTy { } => new UnifyResult(success: true, state: st), ErrorTy { } => new UnifyResult(success: true, state: st), _ => unify_mismatch(st, a, b), ErrorTy { } => new UnifyResult(success: true, state: st), FunTy(var pa, var ra) => b switch { FunTy(var pb, var rb) => unify_fun(st, pa, ra, pb, rb), ErrorTy { } => new UnifyResult(success: true, state: st), _ => unify_mismatch(st, a, b), ListTy(var ea) => b switch { ListTy(var eb) => unify(st, ea, eb), ErrorTy { } => new UnifyResult(success: true, state: st), _ => unify_mismatch(st, a, b), ConstructedTy(var na, var args_a) => b switch { ConstructedTy(var nb, var args_b) => ((na.value == nb.value) ? unify_constructed_args(st, args_a, args_b, 0, list_length(args_a)) : unify_mismatch(st, a, b)), SumTy(var sb_name, var sb_ctors) => ((na.value == sb_name.value) ? new UnifyResult(success: true, state: st) : unify_mismatch(st, a, b)), RecordTy(var rb_name, var rb_fields) => ((na.value == rb_name.value) ? new UnifyResult(success: true, state: st) : unify_mismatch(st, a, b)), ErrorTy { } => new UnifyResult(success: true, state: st), _ => unify_mismatch(st, a, b), SumTy(var sa_name, var sa_ctors) => b switch { SumTy(var sb_name, var sb_ctors) => ((sa_name.value == sb_name.value) ? new UnifyResult(success: true, state: st) : unify_mismatch(st, a, b)), ConstructedTy(var nb, var args_b) => ((sa_name.value == nb.value) ? new UnifyResult(success: true, state: st) : unify_mismatch(st, a, b)), ErrorTy { } => new UnifyResult(success: true, state: st), _ => unify_mismatch(st, a, b), RecordTy(var ra_name, var ra_fields) => b switch { RecordTy(var rb_name, var rb_fields) => ((ra_name.value == rb_name.value) ? new UnifyResult(success: true, state: st) : unify_mismatch(st, a, b)), ConstructedTy(var nb, var args_b) => ((ra_name.value == nb.value) ? new UnifyResult(success: true, state: st) : unify_mismatch(st, a, b)), ErrorTy { } => new UnifyResult(success: true, state: st), _ => unify_mismatch(st, a, b), ForAllTy(var id, var body) => unify(st, body, b), _ => b switch { ErrorTy { } => new UnifyResult(success: true, state: st), _ => unify_mismatch(st, a, b),  },  },  },  },  },  },  },  },  },  },  },  },  };

    public static UnifyResult unify_constructed_args(UnificationState st, List<CodexType> args_a, List<CodexType> args_b, long i, long len) => ((i == len) ? new UnifyResult(success: true, state: st) : ((i >= list_length(args_b)) ? new UnifyResult(success: true, state: st) : ((UnifyResult r = unify(st, list_at(args_a)(i), list_at(args_b)(i))) is var _ ? (r.success ? unify_constructed_args(r.state, args_a, args_b, (i + 1), len) : r) : default)));

    public static UnifyResult unify_fun(UnificationState st, CodexType pa, CodexType ra, CodexType pb, CodexType rb) => ((UnifyResult r1 = unify(st, pa, pb)) is var _ ? (r1.success ? unify(r1.state, ra, rb) : r1) : default);

    public static UnifyResult unify_mismatch(UnificationState st, CodexType a, CodexType b) => new UnifyResult(success: false, state: add_unify_error(st, "CDX2001", ("Type mismatch: " + (type_tag(a) + (" vs " + type_tag(b))))));

    public static string type_tag(CodexType ty) => ty switch { IntegerTy { } => "Integer", NumberTy { } => "Number", TextTy { } => "Text", BooleanTy { } => "Boolean", VoidTy { } => "Void", NothingTy { } => "Nothing", ErrorTy { } => "Error", FunTy(var p, var r) => "Fun", ListTy(var e) => "List", TypeVar(var id) => ("T" + integer_to_text(id)), ForAllTy(var id, var body) => "ForAll", SumTy(var name, var ctors) => ("Sum:" + name.value), RecordTy(var name, var fields) => ("Rec:" + name.value), ConstructedTy(var name, var args) => ("Con:" + name.value),  };

    public static CodexType deep_resolve(UnificationState st, CodexType ty) => ((CodexType resolved = resolve(st, ty)) is var _ ? resolved switch { FunTy(var param, var ret) => new FunTy(deep_resolve(st, param), deep_resolve(st, ret)), ListTy(var elem) => new ListTy(deep_resolve(st, elem)), ConstructedTy(var name, var args) => new ConstructedTy(name, deep_resolve_list(st, args, 0, list_length(args), new List<CodexType>())), ForAllTy(var id, var body) => new ForAllTy(id, deep_resolve(st, body)), SumTy(var name, var ctors) => resolved, RecordTy(var name, var fields) => resolved, _ => resolved,  } : default);

    public static List<CodexType> deep_resolve_list(UnificationState st, List<CodexType> args, long i, long len, List<CodexType> acc) => ((i == len) ? acc : deep_resolve_list(st, args, (i + 1), len, (acc + new List<CodexType> { deep_resolve(st, list_at(args)(i)) })));

    public static string compile(string source, string module_name) => ((List<Token> tokens = tokenize(source)) is var _ ? ((ParseState st = make_parse_state(tokens)) is var _ ? ((Document doc = parse_document(st)) is var _ ? ((AModule ast = desugar_document(doc, module_name)) is var _ ? ((ModuleResult check_result = check_module(ast)) is var _ ? ((IRModule ir = lower_module(ast, check_result.types, check_result.state)) is var _ ? emit_full_module(ir, ast.type_defs) : default) : default) : default) : default) : default) : default);

    public static CompileResult compile_checked(string source, string module_name) => ((List<Token> tokens = tokenize(source)) is var _ ? ((ParseState st = make_parse_state(tokens)) is var _ ? ((Document doc = parse_document(st)) is var _ ? ((AModule ast = desugar_document(doc, module_name)) is var _ ? ((ResolveResult resolve_result = resolve_module(ast)) is var _ ? ((list_length(resolve_result.errors) > 0) ? new CompileError(resolve_result.errors) : ((ModuleResult check_result = check_module(ast)) is var _ ? ((IRModule ir = lower_module(ast, check_result.types, check_result.state)) is var _ ? new CompileOk(emit_full_module(ir, ast.type_defs), check_result) : default) : default)) : default) : default) : default) : default) : default);

    public static string test_source() => "square : Integer -> Integer\\nsquare (x) = x * x\\nmain = square 5";

    public static [ Console() => Nothing;

    public static object main() => { print_line(compile(test_source, "test"));  };

}
