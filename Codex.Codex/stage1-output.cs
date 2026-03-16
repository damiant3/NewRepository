using System;
using System.Collections.Generic;
using System.Linq;

public sealed record ALetBind();

public sealed record AMatchArm();

public sealed record AFieldExpr();

public sealed record AParam();

public sealed record ADef();

public sealed record ARecordFieldDef();

public sealed record AVariantCtorDef();

public sealed record AModule();

public sealed record Diagnostic();

public sealed record Name();

public sealed record SourcePosition();

public sealed record SourceSpan();

public sealed record IRParam();

public sealed record IRBranch();

public sealed record IRFieldVal();

public sealed record IRDef();

public sealed record IRModule();

public sealed record Scope();

public sealed record ResolveResult();

public sealed record CollectResult();

public sealed record CtorCollectResult();

public sealed record LexState();

public sealed record ParseState();

public sealed record LetBind();

public sealed record MatchArm();

public sealed record RecordFieldExpr();

public sealed record TypeAnn();

public sealed record Def();

public sealed record RecordFieldDef();

public sealed record VariantCtorDef();

public sealed record TypeDef();

public sealed record Document();

public sealed record Token();

public sealed record SumCtor();

public sealed record RecordField();

public sealed record CheckResult();

public sealed record LetBindResult();

public sealed record LambdaBindResult();

public sealed record DefSetup();

public sealed record DefParamResult();

public sealed record ModuleResult();

public sealed record TypeEnv();

public sealed record TypeBinding();

public sealed record UnificationState();

public sealed record SubstEntry();

public sealed record UnifyResult();

public sealed record FreshResult();

public static class Codex_Codex_Codex
{
    public static object () => /* error:  */ default;

    public static T2283 The<T2283>() => syntax(tree)(is)(the)(desugared)(representation)(used);

    public static T2291 by<T2291>() => type(checker)(and)(later)(stages.CST)(nodes)(carry)(tokens);

    public static T2297 AST<T2297>() => carry(Names)(and)(resolved)(literal)(values.);

    public static object () => /* error:  */ default;

    public static object LiteralKind() => /* error:  */ default;

    public static T2301 IntLit<T2301>() => /* error: | */ default(NumLit);

    public static T2303 TextLit<T2303>() => /* error: | */ default(BoolLit);

    public static object BinaryOp() => /* error:  */ default;

    public static T2306 OpAdd<T2306>() => /* error: | */ default(OpSub);

    public static T2308 OpMul<T2308>() => /* error: | */ default(OpDiv);

    public static T2310 OpPow<T2310>() => /* error: | */ default(OpEq);

    public static T2312 OpNotEq<T2312>() => /* error: | */ default(OpLt);

    public static T2314 OpGt<T2314>() => /* error: | */ default(OpLtEq);

    public static T2316 OpGtEq<T2316>() => /* error: | */ default(OpDefEq);

    public static T2318 OpAppend<T2318>() => /* error: | */ default(OpCons);

    public static T2320 OpAnd<T2320>() => /* error: | */ default(OpOr);

    public static object () => /* error:  */ default;

    public static object AExpr() => /* error:  */ default;

    public static object ALitExpr() => Text;

    public static object LiteralKind() => /* error:  */ default;

    public static object ANameExpr() => Name;

    public static object AApplyExpr() => AExpr;

    public static object AExpr() => /* error:  */ default;

    public static object ABinaryExpr() => AExpr;

    public static object BinaryOp() => AExpr;

    public static object AUnaryExpr() => AExpr;

    public static object AIfExpr() => AExpr;

    public static object AExpr() => AExpr;

    public static T2334 ALetExpr<T2334>() => List(ALetBind);

    public static object AExpr() => /* error:  */ default;

    public static T2337 ALambdaExpr<T2337>() => List(Name);

    public static object AExpr() => /* error:  */ default;

    public static object AMatchExpr() => AExpr;

    public static object List() => /* error: ) */ default;

    public static T2342 AListExpr<T2342>() => List(AExpr);

    public static object ARecordExpr() => Name;

    public static object List() => /* error: ) */ default;

    public static object AFieldAccess() => AExpr;

    public static object Name() => /* error:  */ default;

    public static T2348 ADoExpr<T2348>() => List(ADoStmt);

    public static object AErrorExpr() => Text;

    public static object ,() => value;

    public static object AExpr() => /* error: } */ default;

    public static object ,() => body;

    public static object AExpr() => /* error: } */ default;

    public static object ,() => value;

    public static object AExpr() => /* error: } */ default;

    public static object ADoStmt() => /* error:  */ default;

    public static object ADoBindStmt() => Name;

    public static object AExpr() => /* error:  */ default;

    public static object ADoExprStmt() => AExpr;

    public static object () => /* error:  */ default;

    public static object APat() => /* error:  */ default;

    public static object AVarPat() => Name;

    public static object ALitPat() => Text;

    public static object LiteralKind() => /* error:  */ default;

    public static object ACtorPat() => Name;

    public static object List() => /* error: ) */ default;

    public static object AWildPat() => /* error:  */ default;

    public static object () => /* error:  */ default;

    public static object ATypeExpr() => /* error:  */ default;

    public static object ANamedType() => Name;

    public static object AFunType() => ATypeExpr;

    public static object ATypeExpr() => /* error:  */ default;

    public static object AAppType() => ATypeExpr;

    public static object List() => /* error: ) */ default;

    public static object () => /* error:  */ default;

    public static object () => /* error: } */ default;

    public static object ,() => params;

    public static object List() => /* error: , */ default;

    public static object ,() => body;

    public static object AExpr() => /* error: } */ default;

    public static object ,() => type_expr;

    public static object ATypeExpr() => /* error: } */ default;

    public static object ,() => fields;

    public static object List() => /* error:  */ default;

    public static object ATypeDef() => /* error:  */ default;

    public static object ARecordTypeDef() => Name;

    public static object List() => /* error: ) */ default(List(ARecordFieldDef));

    public static object AVariantTypeDef() => Name;

    public static object List() => /* error: ) */ default(List(AVariantCtorDef));

    public static object () => /* error:  */ default;

    public static object ,() => defs;

    public static object List() => /* error: , */ default;

    public static object () => /* error: } */ default;

    public static object () => /* error:  */ default;

    public static T2406 Transforms<T2406>() => concrete(syntax)(tree)(into)(the)(abstract)(syntax)(tree.);

    public static object Tokens() => Names;

    public static T2411 literal<T2411>() => text(becomes)(literal)(values);

    public static T2413 parenthesized<T2413>() => are(removed);

    public static T2416 and<T2416>() => operators(are)(resolved.);

    public static object () => /* error:  */ default;

    public static object () => desugar_expr(node);

    public static object node() => (LitExpr(tok) ? desugar_literal(tok) : (NameExpr(tok) ? ANameExpr(make_name(tok.text)) : (AppExpr(f)(a) ? AApplyExpr(desugar_expr(f))(desugar_expr(a)) : (BinExpr(l)(op)(r) ? ABinaryExpr(desugar_expr(l))(desugar_bin_op(op.kind))(desugar_expr(r)) : (UnaryExpr(op)(operand) ? AUnaryExpr(desugar_expr(operand)) : (IfExpr(c)(t)(e) ? AIfExpr(desugar_expr(c))(desugar_expr(t))(desugar_expr(e)) : (LetExpr(bindings)(body) ? ALetExpr(map_list(desugar_let_bind)(bindings))(desugar_expr(body)) : (MatchExpr(scrut)(arms) ? AMatchExpr(desugar_expr(scrut))(map_list(desugar_match_arm)(arms)) : (ListExpr(elems) ? AListExpr(map_list(desugar_expr)(elems)) : (RecordExpr(type_tok)(fields) ? ARecordExpr(make_name(type_tok.text))(map_list(desugar_field_expr)(fields)) : (FieldExpr(rec)(field_tok) ? AFieldAccess(desugar_expr(rec))(make_name(field_tok.text)) : (ParenExpr(inner) ? desugar_expr(inner) : (DoExpr(stmts) ? ADoExpr(map_list(desugar_do_stmt)(stmts)) : (ErrExpr(tok) ? AErrorExpr(tok.text) : /* error:  */ default))))))))))))));

    public static object () => desugar_literal(tok);

    public static object is_literal(object tok) => /* error: ) */ default;

    public static object ALitExpr() => /* error: . */ default(text)(classify_literal(tok.kind));

    public static object AErrorExpr() => /* error: . */ default(text);

    public static object () => classify_literal(k);

    public static object k() => (IntegerLiteral ? IntLit : (NumberLiteral ? NumLit : (TextLiteral ? TextLit : (TrueKeyword ? BoolLit : (FalseKeyword ? BoolLit : (/* error: _ */ default ? TextLit : /* error:  */ default))))));

    public static object () => desugar_let_bind(b);

    public static object ALetBind() => name;

    public static T2510 make_name<T2510>() => /* error: . */ default(name.text);

    public static T2512 value<T2512>() => desugar_expr(b.value);

    public static object () => desugar_match_arm(arm);

    public static object AMatchArm() => pattern;

    public static T2517 desugar_pattern<T2517>() => /* error: . */ default(pattern);

    public static T2519 body<T2519>() => desugar_expr(arm.body);

    public static object () => desugar_field_expr(f);

    public static object AFieldExpr() => name;

    public static T2510 make_name<T2510>() => /* error: . */ default(name.text);

    public static T2512 value<T2512>() => desugar_expr(f.value);

    public static object () => desugar_do_stmt(s);

    public static object s() => (DoBindStmt(tok)(val) ? ADoBindStmt(make_name(tok.text))(desugar_expr(val)) : (DoExprStmt(e) ? ADoExprStmt(desugar_expr(e)) : /* error:  */ default));

    public static object () => /* error:  */ default;

    public static object () => desugar_bin_op(k);

    public static object k() => (Plus ? OpAdd : (Minus ? OpSub : (Star ? OpMul : (Slash ? OpDiv : (Caret ? OpPow : (DoubleEquals ? OpEq : (NotEquals ? OpNotEq : (LessThan ? OpLt : (GreaterThan ? OpGt : (LessOrEqual ? OpLtEq : (GreaterOrEqual ? OpGtEq : (TripleEquals ? OpDefEq : (PlusPlus ? OpAppend : (ColonColon ? OpCons : (Ampersand ? OpAnd : (Pipe ? OpOr : (/* error: _ */ default ? OpAdd : /* error:  */ default)))))))))))))))));

    public static object () => /* error:  */ default;

    public static object () => desugar_pattern(p);

    public static object p() => (VarPat(tok) ? AVarPat(make_name(tok.text)) : (LitPat(tok) ? ALitPat(tok.text)(classify_literal(tok.kind)) : (CtorPat(tok)(subs) ? ACtorPat(make_name(tok.text))(map_list(desugar_pattern)(subs)) : (WildPat(tok) ? AWildPat : /* error:  */ default))));

    public static object () => /* error:  */ default;

    public static object () => desugar_type_expr(t);

    public static object t() => (NamedType(tok) ? ANamedType(make_name(tok.text)) : (FunType(param)(ret) ? AFunType(desugar_type_expr(param))(desugar_type_expr(ret)) : (AppType(ctor)(args) ? AAppType(desugar_type_expr(ctor))(map_list(desugar_type_expr)(args)) : (ParenType(inner) ? desugar_type_expr(inner) : (ListType(elem) ? AAppType(ANamedType(make_name("List")))(new List<object> { desugar_type_expr(elem) }) : (LinearTypeExpr(inner) ? desugar_type_expr(inner) : /* error:  */ default))))));

    public static object () => /* error:  */ default;

    public static object () => desugar_def(d);

    public static object ADef() => name;

    public static T2510 make_name<T2510>() => /* error: . */ default(name.text);

    public static T2600 params<T2600>() => map_list(desugar_param)(d.params);

    public static List<T2602> declared_type<T2602>() => new List<T2602>();

    public static T2519 body<T2519>() => desugar_expr(d.body);

    public static object () => desugar_param(tok);

    public static object AParam() => name;

    public static T2510 make_name<T2510>() => /* error: . */ default(text);

    public static object () => desugar_type_def(td);

    public static object td() => body;

    public static T2622 RecordBody<T2622>(object fields) => ARecordTypeDef(make_name(td.name.text))(map_list(make_type_param_name)(td.type_params))(map_list(desugar_record_field_def)(fields));

    public static T2632 VariantBody<T2632>(object ctors) => AVariantTypeDef(make_name(td.name.text))(map_list(make_type_param_name)(td.type_params))(map_list(desugar_variant_ctor_def)(ctors));

    public static object () => make_type_param_name(tok);

    public static T2510 make_name<T2510>() => /* error: . */ default(text);

    public static object () => desugar_record_field_def(f);

    public static object ARecordFieldDef() => name;

    public static T2510 make_name<T2510>() => /* error: . */ default(name.text);

    public static T2643 type_expr<T2643>() => desugar_type_expr(f.type_expr);

    public static object () => desugar_variant_ctor_def(c);

    public static object AVariantCtorDef() => name;

    public static T2510 make_name<T2510>() => /* error: . */ default(name.text);

    public static T2651 fields<T2651>() => map_list(desugar_type_expr)(c.fields);

    public static object () => /* error:  */ default;

    public static object () => desugar_document(doc)(module_name);

    public static object AModule() => name;

    public static T2510 make_name<T2510>() => /* error: , */ default(defs);

    public static object map_list() => doc.defs;

    public static T2662 type_defs<T2662>() => map_list(desugar_type_def)(doc.type_defs);

    public static object () => /* error:  */ default;

    public static T2668 Functional<T2668>() => operations(used)(throughout)(the)(compiler.);

    public static object () => /* error:  */ default;

    public static object () => map_list(f)(xs);

    public static T2678 map_list_loop<T2678>() => xs(0)(list_length(xs))(new List<object>());

    public static object () => map_list_loop(f)(xs)(i)(len)(acc);

    public static object i() => len;

    public static T2695 acc<T2695>() => /* error: else */ default(map_list_loop)(f)(xs)((i + 1))(len)((acc + new List<object> { f(list_at(xs)(i)) }));

    public static object () => fold_list(f)(z)(xs);

    public static T2704 fold_list_loop<T2704>() => z(xs)(0)(list_length(xs));

    public static object () => fold_list_loop(f)(z)(xs)(i)(len);

    public static object i() => len;

    public static T2722 z<T2722>() => /* error: else */ default(fold_list_loop)(f)(f(z)(list_at(xs)(i)))(xs)((i + 1))(len);

    public static object () => /* error:  */ default;

    public static object Compiler() => types.;

    public static object () => /* error:  */ default;

    public static object DiagnosticSeverity() => /* error:  */ default;

    public static T2728 Error<T2728>() => /* error: | */ default(Warning);

    public static object Info() => /* error:  */ default;

    public static object ,() => message;

    public static object Text() => /* error:  */ default;

    public static object () => /* error: } */ default;

    public static object () => /* error:  */ default;

    public static object () => make_error(code)(msg);

    public static object Diagnostic() => code;

    public static object code() => message;

    public static object msg() => severity;

    public static T2728 Error<T2728>() => /* error:  */ default;

    public static object () => make_warning(code)(msg);

    public static object Diagnostic() => code;

    public static object code() => message;

    public static object msg() => severity;

    public static object Warning() => /* error:  */ default;

    public static object () => /* error:  */ default;

    public static object () => severity_label(s);

    public static object s() => Error;

    public static object Warning() => "warning";

    public static object Info() => "info";

    public static object () => diagnostic_display(d);

    public static string severity_label() => ((((/* error: . */ default(severity) + " ") + d.code) + ": ") + d.message);

    public static object () => /* error:  */ default;

    public static T2761 A<T2761>() => for(identifier)(names.);

    public static object () => /* error:  */ default;

    public static object () => /* error: } */ default;

    public static object () => /* error:  */ default;

    public static object () => make_name(s);

    public static object Name() => value;

    public static object s() => /* error:  */ default;

    public static object () => name_value(n);

    public static object n() => value;

    public static object () => /* error:  */ default;

    public static T2778 Source<T2778>() => and(span)(types)(for)(error)(reporting.);

    public static object () => /* error:  */ default;

    public static object ,() => column;

    public static object Integer() => /* error:  */ default;

    public static object () => /* error: } */ default;

    public static object ,() => end;

    public static object SourcePosition() => /* error:  */ default;

    public static object () => /* error: } */ default;

    public static object () => /* error:  */ default;

    public static object () => make_position(line)(col)(offset);

    public static object SourcePosition() => line;

    public static object line() => column;

    public static object col() => offset;

    public static object offset() => /* error:  */ default;

    public static object () => make_span(s)(e)(f);

    public static object SourceSpan() => start;

    public static object s() => end;

    public static object e() => file;

    public static object f() => /* error:  */ default;

    public static object () => span_length(span);

    public static object span() => (end.offset - span.start.offset);

    public static object () => /* error:  */ default;

    public static T2813 Generates<T2813>() => /* error: # */ default(source)(code)(from)(the)(IR)(module.);

    public static T2818 Uses<T2818>() => concatenation(to)(build)(the)(output.);

    public static object () => /* error:  */ default;

    public static object () => sanitize(name);

    public static T2823 text_replace<T2823>() => "-"("_");

    public static object () => cs_type(ty);

    public static string ty() => (IntegerTy ? "long" : (NumberTy ? "decimal" : (TextTy ? "string" : (BooleanTy ? "bool" : (VoidTy ? "void" : (NothingTy ? "object" : (ErrorTy ? "object" : (FunTy(p)(r) ? (((("Func<" + cs_type(p)) + ", ") + cs_type(r)) + ">") : /* error:  */ default))))))));

    public static string ListTy(object elem) => (("List<" + cs_type(elem)) + ">");

    public static string TypeVar(object id) => ("T" + integer_to_text(id));

    public static T2840 ForAllTy<T2840>(object id, object body) => cs_type(body);

    public static T2844 SumTy<T2844>(object name, object ctors) => sanitize(name.value);

    public static T2848 RecordTy<T2848>(object name, object fields) => sanitize(name.value);

    public static T2852 ConstructedTy<T2852>(object name, object args) => sanitize(name.value);

    public static object () => /* error:  */ default;

    public static object () => emit_expr(e);

    public static object e() => (IrIntLit(n) ? integer_to_text(n) : (IrNumLit(n) ? integer_to_text(n) : (IrTextLit(s) ? (("\\\"" + escape_text(s)) + "\\\"") : (IrBoolLit(b) ? (b ? "true" : "false") : (IrName(n)(ty) ? sanitize(n) : (IrBinary(op)(l)(r)(ty) ? (((((("(" + emit_expr(l)) + " ") + emit_bin_op(op)) + " ") + emit_expr(r)) + ")") : (IrNegate(operand) ? (("(-" + emit_expr(operand)) + ")") : (IrIf(c)(t)(el)(ty) ? (((((("(" + emit_expr(c)) + " ? ") + emit_expr(t)) + " : ") + emit_expr(el)) + ")") : (IrLet(name)(ty)(val)(body) ? emit_let(name)(ty)(val)(body) : (IrApply(f)(a)(ty) ? (((emit_expr(f) + "(") + emit_expr(a)) + ")") : (IrLambda(params)(body)(ty) ? emit_lambda(params)(body) : (IrList(elems)(ty) ? emit_list(elems)(ty) : (IrMatch(scrut)(branches)(ty) ? emit_match(scrut)(branches)(ty) : (IrDo(stmts)(ty) ? emit_do(stmts) : (IrRecord(name)(fields)(ty) ? emit_record(name)(fields) : (IrFieldAccess(rec)(field)(ty) ? ((emit_expr(rec) + ".") + sanitize(field)) : (IrError(msg)(ty) ? (("/* error: " + msg) + " */ default") : /* error:  */ default)))))))))))))))));

    public static object () => escape_text(s);

    public static T2823 text_replace<T2823>(object text_replace) => "\\\\\\\\";

    public static object () => emit_bin_op(op);

    public static string op() => (IrAddInt ? "+" : (IrSubInt ? "-" : /* error:  */ default));

    public static string IrMulInt() => "*";

    public static string IrDivInt() => "/";

    public static string IrPowInt() => "^";

    public static string IrAddNum() => "+";

    public static string IrSubNum() => "-";

    public static string IrMulNum() => "*";

    public static string IrDivNum() => "/";

    public static string IrEq() => "==";

    public static string IrNotEq() => "!=";

    public static string IrLt() => "<";

    public static string IrGt() => ">";

    public static string IrLtEq() => "<=";

    public static string IrGtEq() => ">=";

    public static string IrAnd() => "&&";

    public static string IrOr() => "||";

    public static string IrAppendText() => "+";

    public static string IrAppendList() => "+";

    public static string IrConsList() => "+";

    public static object () => emit_let(name)(ty)(val)(body);

    public static string cs_type() => ((((((/* error: ++ */ default(" ") + sanitize(name)) + " = ") + emit_expr(val)) + ") is var _ ? ") + emit_expr(body)) + " : default)");

    public static object () => emit_lambda(params)(body);

    public static T2965 list_length<T2965>() => /* error: == */ default(0);

    public static T2967 emit_expr<T2967>() => /* error: ++ */ default(")");

    public static T2965 list_length<T2965>() => /* error: == */ default(1);

    public static object p() => list_at(params)(0);

    public static string cs_type() => (((((/* error: . */ default(type_val) + " ") + sanitize(p.name)) + ") => ") + emit_expr(body)) + ")");

    public static T2967 emit_expr<T2967>() => /* error: ++ */ default(")");

    public static object () => emit_list(elems)(ty);

    public static T2965 list_length<T2965>() => /* error: == */ default(0);

    public static string cs_type() => /* error: ++ */ default(">()");

    public static string cs_type() => ((/* error: ++ */ default("> { ") + emit_list_elems(elems)(0)) + " }");

    public static object () => emit_list_elems(elems)(i);

    public static object i() => list_length(elems);

    public static object i() => (list_length(elems) - 1);

    public static T2967 emit_expr<T2967>(object list_at) => /* error: ) */ default;

    public static T2967 emit_expr<T2967>(object list_at) => ((/* error: ) */ default + ", ") + emit_list_elems(elems)((i + 1)));

    public static object () => emit_match(scrut)(branches)(ty);

    public static T2967 emit_expr<T2967>() => ((/* error: ++ */ default(" switch { ") + emit_match_arms(branches)(0)) + " }");

    public static object () => emit_match_arms(branches)(i);

    public static object i() => list_length(branches);

    public static T3018 arm<T3018>() => list_at(branches)(i);

    public static string emit_pattern() => ((((/* error: . */ default(pattern) + " => ") + emit_expr(arm.body)) + ", ") + emit_match_arms(branches)((i + 1)));

    public static object () => emit_pattern(p);

    public static object p() => (IrVarPat(name)(ty) ? ((cs_type(ty) + " ") + sanitize(name)) : (IrLitPat(text)(ty) ? text : (IrCtorPat(name)(subs)(ty) ? /* error:  */ default : (list_length(subs) == 0))));

    public static T3038 sanitize<T3038>() => /* error: ++ */ default(" { }");

    public static T3038 sanitize<T3038>() => ((/* error: ++ */ default("(") + emit_sub_patterns(subs)(0)) + ")");

    public static string IrWildPat() => "_";

    public static object () => emit_sub_patterns(subs)(i);

    public static object i() => list_length(subs);

    public static T3051 sub<T3051>() => list_at(subs)(i);

    public static T3056 emit_sub_pattern<T3056>() => (/* error: ++ */ default(((i < (list_length(subs) - 1)) ? ", " : "")) + emit_sub_patterns(subs)((i + 1)));

    public static object () => emit_sub_pattern(p);

    public static object p() => (IrVarPat(name)(ty) ? ("var " + sanitize(name)) : (IrCtorPat(name)(subs)(ty) ? emit_pattern(p) : (IrWildPat ? "_" : (IrLitPat(text)(ty) ? text : /* error:  */ default))));

    public static object () => emit_do(stmts);

    public static long emit_do_stmts() => (0 + " }");

    public static object () => emit_do_stmts(stmts)(i);

    public static object i() => list_length(stmts);

    public static object s() => list_at(stmts)(i);

    public static T3083 emit_do_stmt<T3083>() => (/* error: ++ */ default(" ") + emit_do_stmts(stmts)((i + 1)));

    public static object () => emit_do_stmt(s);

    public static object s() => (IrDoBind(name)(ty)(val) ? (((("var " + sanitize(name)) + " = ") + emit_expr(val)) + ";") : (IrDoExec(e) ? (emit_expr(e) + ";") : /* error:  */ default));

    public static object () => emit_record(name)(fields);

    public static T3038 sanitize<T3038>() => ((/* error: ++ */ default("(") + emit_record_fields(fields)(0)) + ")");

    public static object () => emit_record_fields(fields)(i);

    public static object i() => list_length(fields);

    public static object f() => list_at(fields)(i);

    public static T3038 sanitize<T3038>() => ((((/* error: . */ default(name) + ": ") + emit_expr(f.value)) + ((i < (list_length(fields) - 1)) ? ", " : "")) + emit_record_fields(fields)((i + 1)));

    public static object () => /* error:  */ default;

    public static object () => emit_type_defs(tds)(i);

    public static object i() => list_length(tds);

    public static object emit_type_def(object list_at) => ((/* error: ) */ default + "\\n") + emit_type_defs(tds)((i + 1)));

    public static object () => emit_type_def(td);

    public static object td() => (ARecordTypeDef(name)(tparams)(fields) ? /* error:  */ default : gen);

    public static object emit_tparam_suffix() => /* error:  */ default;

    public static T3038 sanitize<T3038>() => ((((/* error: . */ default(value) + gen) + "(") + emit_record_field_defs(fields)(tparams)(0)) + ");\\n");

    public static object AVariantTypeDef(object name, object tparams, object ctors) => /* error:  */ default;

    public static T3142 gen<T3142>() => emit_tparam_suffix(tparams);

    public static T3038 sanitize<T3038>() => ((((/* error: . */ default(value) + gen) + ";\\n") + emit_variant_ctors(ctors)(name)(tparams)(0)) + "\\n");

    public static object () => emit_tparam_suffix(tparams);

    public static T2965 list_length<T2965>() => /* error: == */ default(0);

    public static long emit_tparam_names() => (0 + ">");

    public static object () => emit_tparam_names(tparams)(i);

    public static object i() => list_length(tparams);

    public static object i() => (list_length(tparams) - 1);

    public static object integer_to_text() => /* error:  */ default;

    public static object integer_to_text() => (/* error: ++ */ default(", ") + emit_tparam_names(tparams)((i + 1)));

    public static object () => emit_record_field_defs(fields)(tparams)(i);

    public static object i() => list_length(fields);

    public static object f() => list_at(fields)(i);

    public static string emit_type_expr_tp() => ((((/* error: . */ default(type_expr)(tparams) + " ") + sanitize(f.name.value)) + ((i < (list_length(fields) - 1)) ? ", " : "")) + emit_record_field_defs(fields)(tparams)((i + 1)));

    public static object () => emit_variant_ctors(ctors)(base_name)(tparams)(i);

    public static object i() => list_length(ctors);

    public static T3192 c<T3192>() => list_at(ctors)(i);

    public static T3198 emit_variant_ctor<T3198>() => (base_name(tparams) + emit_variant_ctors(ctors)(base_name)(tparams)((i + 1)));

    public static object () => emit_variant_ctor(c)(base_name)(tparams);

    public static T3142 gen<T3142>() => emit_tparam_suffix(tparams);

    public static T2965 list_length<T2965>() => (/* error: . */ default(fields) == 0);

    public static T3038 sanitize<T3038>() => (((((/* error: . */ default(name.value) + gen) + " : ") + sanitize(base_name.value)) + gen) + ";\\n");

    public static T3038 sanitize<T3038>() => (((((((/* error: . */ default(name.value) + gen) + "(") + emit_ctor_fields(c.fields)(tparams)(0)) + ") : ") + sanitize(base_name.value)) + gen) + ";\\n");

    public static object () => emit_ctor_fields(fields)(tparams)(i);

    public static object i() => list_length(fields);

    public static string emit_type_expr_tp(object list_at) => ((((/* error: ) */ default(tparams) + " Field") + integer_to_text(i)) + ((i < (list_length(fields) - 1)) ? ", " : "")) + emit_ctor_fields(fields)(tparams)((i + 1)));

    public static object () => emit_type_expr(te);

    public static string emit_type_expr_tp() => new List<object>();

    public static object () => emit_type_expr_tp(te)(tparams);

    public static object te() => (ANamedType(name) ? /* error:  */ default : ((object idx = find_tparam_index(tparams)(name.value)(0)) is var _ ? /* error:  */ default : default));

    public static long idx() => 0;

    public static object integer_to_text() => /* error:  */ default;

    public static T3245 when_type_name<T3245>() => /* error: . */ default(value);

    public static object AFunType(object p, object r) => (((("Func<" + emit_type_expr_tp(p)(tparams)) + ", ") + emit_type_expr_tp(r)(tparams)) + ">");

    public static object AAppType(object base, object args) => (((emit_type_expr_tp(base)(tparams) + "<") + emit_type_expr_list_tp(args)(tparams)(0)) + ">");

    public static object () => find_tparam_index(tparams)(name)(i);

    public static object i() => list_length(tparams);

    public static object list_at() => i;

    public static T2512 value<T2512>() => name;

    public static object i() => /* error: else */ default(find_tparam_index)(tparams)(name)((i + 1));

    public static object () => when_type_name(n);

    public static object n() => "Integer";

    public static object n() => "Number";

    public static object n() => "Text";

    public static object n() => "Boolean";

    public static object n() => "List";

    public static T3038 sanitize<T3038>() => /* error:  */ default;

    public static object () => emit_type_expr_list(args)(i);

    public static object i() => list_length(args);

    public static object emit_type_expr(object list_at) => ((/* error: ) */ default + ((i < (list_length(args) - 1)) ? ", " : "")) + emit_type_expr_list(args)((i + 1)));

    public static object () => emit_type_expr_list_tp(args)(tparams)(i);

    public static object i() => list_length(args);

    public static string emit_type_expr_tp(object list_at) => ((/* error: ) */ default(tparams) + ((i < (list_length(args) - 1)) ? ", " : "")) + emit_type_expr_list_tp(args)(tparams)((i + 1)));

    public static object () => /* error:  */ default;

    public static object () => collect_type_var_ids(ty)(acc);

    public static string ty() => (TypeVar(id) ? (list_contains_int(acc)(id) ? acc : list_append_int(acc)(id)) : (FunTy(p)(r) ? collect_type_var_ids(r)(collect_type_var_ids(p)(acc)) : (ListTy(elem) ? collect_type_var_ids(elem)(acc) : (ForAllTy(id)(body) ? collect_type_var_ids(body)(acc) : (ConstructedTy(name)(args) ? collect_type_var_ids_list(args)(acc) : (/* error: _ */ default ? acc : /* error:  */ default))))));

    public static object () => collect_type_var_ids_list(types)(acc);

    public static T3338 collect_type_var_ids_list_loop<T3338>() => acc(0)(list_length(types));

    public static object () => collect_type_var_ids_list_loop(types)(acc)(i)(len);

    public static object i() => len;

    public static T2695 acc<T2695>() => /* error: else */ default(collect_type_var_ids_list_loop)(types)(collect_type_var_ids(list_at(types)(i))(acc))((i + 1))(len);

    public static object () => list_contains_int(xs)(n);

    public static T3361 list_contains_int_loop<T3361>() => n(0)(list_length(xs));

    public static object () => list_contains_int_loop(xs)(n)(i)(len);

    public static object i() => len;

    public static object list_at() => (i == n);

    public static T3361 list_contains_int_loop<T3361>() => n((i + 1))(len);

    public static object () => list_append_int(xs)(n);

    public static List<object> xs() => new List<object> { n };

    public static object () => generic_suffix(ty);

    public static T3381 ids<T3381>() => collect_type_var_ids(ty)(new List<object>());

    public static T2965 list_length<T2965>() => /* error: == */ default(0);

    public static long emit_type_params() => (0 + ">");

    public static object () => emit_type_params(ids)(i);

    public static object i() => list_length(ids);

    public static object i() => (list_length(ids) - 1);

    public static object integer_to_text(object list_at) => /* error: ) */ default;

    public static object integer_to_text(object list_at) => ((/* error: ) */ default + ", ") + emit_type_params(ids)((i + 1)));

    public static object () => /* error:  */ default;

    public static object () => emit_def(d);

    public static T3404 ret<T3404>() => get_return_type(d.type_val)(list_length(d.params));

    public static T3142 gen<T3142>() => generic_suffix(d.type_val);

    public static string cs_type() => (((((((/* error: ++ */ default(" ") + sanitize(d.name)) + gen) + "(") + emit_def_params(d.params)(0)) + ") => ") + emit_expr(d.body)) + ";\\n");

    public static object () => get_return_type(ty)(n);

    public static object n() => 0;

    public static object strip_forall() => /* error:  */ default;

    public static object strip_forall() => /* error: ) */ default;

    public static T3423 FunTy<T3423>(object p, object r) => get_return_type(r)((n - 1));

    public static string ty() => /* error:  */ default;

    public static object () => strip_forall(ty);

    public static string ty() => (ForAllTy(id)(body) ? strip_forall(body) : (/* error: _ */ default ? ty : /* error:  */ default));

    public static object () => emit_def_params(params)(i);

    public static object i() => list_length(params);

    public static object p() => list_at(params)(i);

    public static string cs_type() => ((((/* error: . */ default(type_val) + " ") + sanitize(p.name)) + ((i < (list_length(params) - 1)) ? ", " : "")) + emit_def_params(params)((i + 1)));

    public static object () => /* error:  */ default;

    public static object () => emit_full_module(m)(type_defs);

    public static long emit_type_defs() => (((0 + emit_class_header(m.name.value)) + emit_defs(m.defs)(0)) + "}\\n");

    public static object () => emit_module(m);

    public static string emit_class_header() => ((/* error: . */ default(name.value) + emit_defs(m.defs)(0)) + "}\\n");

    public static object () => emit_class_header(name);

    public static T3038 sanitize<T3038>() => /* error: ++ */ default("\\n{\\n");

    public static object () => emit_defs(defs)(i);

    public static object i() => list_length(defs);

    public static object emit_def(object list_at) => ((/* error: ) */ default + "\\n") + emit_defs(defs)((i + 1)));

    public static object () => /* error:  */ default;

    public static T2283 The<T2283>() => representation(sits)(between)(the)(typed)(AST)(and)(code);

    public static T3486 emission<T3486>() => Every(node)(carries)(its)(resolved)(type.);

    public static object () => /* error:  */ default;

    public static object IRBinaryOp() => /* error:  */ default;

    public static bool IrAddInt() => (((IrSubInt || IrMulInt) || IrDivInt) || IrPowInt);

    public static string IrAddNum() => ((IrSubNum || IrMulNum) || IrDivNum);

    public static string IrEq() => ((((IrNotEq || IrLt) || IrGt) || IrLtEq) || IrGtEq);

    public static string IrAnd() => IrOr;

    public static string IrAppendText() => IrAppendList;

    public static string IrConsList() => /* error:  */ default;

    public static object IRExpr() => /* error:  */ default;

    public static object IrIntLit() => Integer;

    public static object IrNumLit() => Integer;

    public static object IrTextLit() => Text;

    public static object IrBoolLit() => Boolean;

    public static object IrName() => Text;

    public static object CodexType() => /* error:  */ default;

    public static object IrBinary() => IRBinaryOp;

    public static object IRExpr() => IRExpr(CodexType);

    public static object IrNegate() => IRExpr;

    public static object IrIf() => IRExpr;

    public static object IRExpr() => IRExpr(CodexType);

    public static object IrLet() => Text;

    public static object CodexType() => IRExpr(IRExpr);

    public static object IrApply() => IRExpr;

    public static object IRExpr() => CodexType;

    public static T3515 IrLambda<T3515>() => List(IRParam);

    public static object IRExpr() => CodexType;

    public static T3518 IrList<T3518>() => List(IRExpr);

    public static object CodexType() => /* error:  */ default;

    public static object IrMatch() => IRExpr;

    public static object List() => /* error: ) */ default(CodexType);

    public static T3524 IrDo<T3524>() => List(IRDoStmt);

    public static object CodexType() => /* error:  */ default;

    public static object IrRecord() => Text;

    public static object List() => /* error: ) */ default(CodexType);

    public static object IrFieldAccess() => IRExpr;

    public static object Text() => CodexType;

    public static object IrError() => Text;

    public static object CodexType() => /* error:  */ default;

    public static object ,() => type_val;

    public static object CodexType() => /* error: } */ default;

    public static object ,() => body;

    public static object IRExpr() => /* error: } */ default;

    public static object IRPat() => /* error:  */ default;

    public static object IrVarPat() => Text;

    public static object CodexType() => /* error:  */ default;

    public static object IrLitPat() => Text;

    public static object CodexType() => /* error:  */ default;

    public static object IrCtorPat() => Text;

    public static object List() => /* error: ) */ default(CodexType);

    public static string IrWildPat() => /* error:  */ default;

    public static object IRDoStmt() => /* error:  */ default;

    public static object IrDoBind() => Text;

    public static object CodexType() => IRExpr;

    public static object IrDoExec() => IRExpr;

    public static object ,() => value;

    public static object IRExpr() => /* error: } */ default;

    public static object () => /* error:  */ default;

    public static object ,() => params;

    public static object List() => /* error: , */ default;

    public static object ,() => body;

    public static object IRExpr() => /* error: } */ default;

    public static object ,() => defs;

    public static object List() => /* error:  */ default;

    public static object () => /* error:  */ default;

    public static T2406 Transforms<T2406>() => typed(AST)(into)(the)(intermediate)(representation.);

    public static T3574 Each<T3574>() => expression(is)(lowered)(to)(an)(IR)(expression)(that)(carries);

    public static object its() => type.;

    public static object () => /* error:  */ default;

    public static object () => lookup_type(bindings)(name);

    public static T3583 lookup_type_loop<T3583>() => name(0)(list_length(bindings));

    public static object () => lookup_type_loop(bindings)(name)(i)(len);

    public static object i() => len;

    public static object ErrorTy() => /* error: else */ default;

    public static T3593 b<T3593>() => list_at(bindings)(i);

    public static T3593 b<T3593>() => (name == name);

    public static T3593 b<T3593>() => bound_type;

    public static T3583 lookup_type_loop<T3583>() => name((i + 1))(len);

    public static object () => peel_fun_param(ty);

    public static string ty() => (FunTy(p)(r) ? p : (ForAllTy(id)(body) ? peel_fun_param(body) : (/* error: _ */ default ? ErrorTy : /* error:  */ default)));

    public static object () => peel_fun_return(ty);

    public static string ty() => (FunTy(p)(r) ? r : (ForAllTy(id)(body) ? peel_fun_return(body) : (/* error: _ */ default ? ErrorTy : /* error:  */ default)));

    public static object () => strip_forall_ty(ty);

    public static string ty() => (ForAllTy(id)(body) ? strip_forall_ty(body) : (/* error: _ */ default ? ty : /* error:  */ default));

    public static object () => /* error:  */ default;

    public static object () => lower_bin_op(op)(ty);

    public static string op() => (OpAdd ? IrAddInt : (OpSub ? IrSubInt : (OpMul ? IrMulInt : (OpDiv ? IrDivInt : (OpPow ? IrPowInt : (OpEq ? IrEq : (OpNotEq ? IrNotEq : (OpLt ? IrLt : (OpGt ? IrGt : (OpLtEq ? IrLtEq : (OpGtEq ? IrGtEq : (OpDefEq ? IrEq : (OpAppend ? IrAppendList : (OpCons ? IrConsList : (OpAnd ? IrAnd : (OpOr ? IrOr : /* error:  */ default))))))))))))))));

    public static object () => /* error:  */ default;

    public static object () => lower_expr(e)(ty);

    public static object e() => (ALitExpr(text)(kind) ? lower_literal(text)(kind) : (ANameExpr(name) ? IrName(name.value)(ty) : (AApplyExpr(f)(a) ? lower_apply(f)(a)(ty) : (ABinaryExpr(l)(op)(r) ? IrBinary(lower_bin_op(op)(ty))(lower_expr(l)(ty))(lower_expr(r)(ty))(ty) : (AUnaryExpr(operand) ? IrNegate(lower_expr(operand)(IntegerTy)) : (AIfExpr(c)(t)(e2) ? IrIf(lower_expr(c)(BooleanTy))(lower_expr(t)(ty))(lower_expr(e2)(ty))(ty) : (ALetExpr(binds)(body) ? lower_let(binds)(body)(ty) : (ALambdaExpr(params)(body) ? lower_lambda(params)(body)(ty) : (AMatchExpr(scrut)(arms) ? lower_match(scrut)(arms)(ty) : (AListExpr(elems) ? lower_list(elems)(ty) : (ARecordExpr(name)(fields) ? lower_record(name)(fields)(ty) : (AFieldAccess(rec)(field) ? IrFieldAccess(lower_expr(rec)(ty))(field.value)(ty) : (ADoExpr(stmts) ? lower_do(stmts)(ty) : (AErrorExpr(msg) ? IrError(msg)(ty) : /* error:  */ default))))))))))))));

    public static object () => lower_literal(text)(kind);

    public static object kind() => (IntLit ? IrIntLit(text_to_integer(text)) : (NumLit ? IrIntLit(text_to_integer(text)) : (TextLit ? IrTextLit(text) : (BoolLit ? IrBoolLit((text == "True")) : /* error:  */ default))));

    public static object () => lower_apply(f)(a)(ty);

    public static object IrApply(object lower_expr) => /* error: ) */ default(lower_expr(a)(ty))(ty);

    public static object () => lower_let(binds)(body)(ty);

    public static T2965 list_length<T2965>() => /* error: == */ default(0);

    public static object lower_expr() => ty;

    public static T3593 b<T3593>() => list_at(binds)(0);

    public static object IrLet() => /* error: . */ default(name.value)(ty)(lower_expr(b.value)(ErrorTy))(lower_let_rest(binds)(body)(ty)(1));

    public static object () => lower_let_rest(binds)(body)(ty)(i);

    public static object i() => list_length(binds);

    public static object lower_expr() => ty;

    public static T3593 b<T3593>() => list_at(binds)(i);

    public static object IrLet() => /* error: . */ default(name.value)(ty)(lower_expr(b.value)(ErrorTy))(lower_let_rest(binds)(body)(ty)((i + 1)));

    public static object () => lower_lambda(params)(body)(ty);

    public static T3777 stripped<T3777>() => strip_forall_ty(ty);

    public static T3515 IrLambda<T3515>(object lower_lambda_params) => 0;

    public static object lower_expr() => get_lambda_return(stripped)(list_length(params));

    public static string ty() => /* error:  */ default;

    public static object () => lower_lambda_params(params)(ty)(i);

    public static object i() => list_length(params);

    public static object p() => list_at(params)(i);

    public static T3795 param_ty<T3795>() => peel_fun_param(ty);

    public static T3797 rest_ty<T3797>() => peel_fun_return(ty);

    public static object IRParam() => name;

    public static object p() => value;

    public static object type_val() => param_ty;

    public static T3802 lower_lambda_params<T3802>() => rest_ty((i + 1));

    public static object () => get_lambda_return(ty)(n);

    public static object n() => 0;

    public static string ty() => /* error: else */ default;

    public static string ty() => (FunTy(p)(r) ? get_lambda_return(r)((n - 1)) : (/* error: _ */ default ? ErrorTy : /* error:  */ default));

    public static object () => lower_match(scrut)(arms)(ty);

    public static object IrMatch(object lower_expr) => /* error: ) */ default(map_list(lower_arm(ty))(arms))(ty);

    public static object () => lower_arm(ty)(arm);

    public static object IRBranch() => pattern;

    public static T3829 lower_pattern<T3829>() => /* error: . */ default(pattern);

    public static T2519 body<T2519>() => lower_expr(arm.body)(ty);

    public static object () => lower_pattern(p);

    public static object p() => (AVarPat(name) ? IrVarPat(name.value)(ErrorTy) : (ALitPat(text)(kind) ? IrLitPat(text)(ErrorTy) : (ACtorPat(name)(subs) ? IrCtorPat(name.value)(map_list(lower_pattern)(subs))(ErrorTy) : (AWildPat ? IrWildPat : /* error:  */ default))));

    public static object () => lower_list(elems)(ty);

    public static T3854 elem_ty<T3854>() => ty switch {  };

    public static string ListTy(object e) => e;

    public static object ErrorTy() => /* error: in */ default(IrList)(map_list(lower_elem(elem_ty))(elems))(elem_ty);

    public static object () => lower_elem(ty)(e);

    public static object lower_expr() => ty;

    public static object () => lower_record(name)(fields)(ty);

    public static object IrRecord() => /* error: . */ default(value)(map_list(lower_field_val(ty))(fields))(ty);

    public static object () => lower_field_val(ty)(f);

    public static object IRFieldVal() => name;

    public static object f() => name.value;

    public static T2512 value<T2512>() => lower_expr(f.value)(ty);

    public static object () => lower_do(stmts)(ty);

    public static T3524 IrDo<T3524>(object map_list) => ty;

    public static object stmts() => ty;

    public static object () => lower_do_stmt(ty)(s);

    public static object s() => (ADoBindStmt(name)(val) ? IrDoBind(name.value)(ty)(lower_expr(val)(ty)) : (ADoExprStmt(e) ? IrDoExec(lower_expr(e)(ty)) : /* error:  */ default));

    public static object () => /* error:  */ default;

    public static object () => lower_def(d)(types)(ust);

    public static T3915 raw_type<T3915>() => lookup_type(types)(d.name.value);

    public static T3918 full_type<T3918>() => deep_resolve(ust)(raw_type);

    public static T3777 stripped<T3777>() => strip_forall_ty(full_type);

    public static T2600 params<T2600>() => lower_def_params(d.params)(stripped)(0);

    public static T3928 ret_type<T3928>() => get_return_type_n(stripped)(list_length(d.params));

    public static object IRDef() => name;

    public static object d() => name.value;

    public static T2600 params<T2600>() => params;

    public static object type_val() => full_type;

    public static T2519 body<T2519>() => lower_expr(d.body)(ret_type);

    public static object () => lower_def_params(params)(ty)(i);

    public static object i() => list_length(params);

    public static object p() => list_at(params)(i);

    public static T3795 param_ty<T3795>() => peel_fun_param(ty);

    public static T3797 rest_ty<T3797>() => peel_fun_return(ty);

    public static object IRParam() => name;

    public static object p() => name.value;

    public static object type_val() => param_ty;

    public static T3953 lower_def_params<T3953>() => rest_ty((i + 1));

    public static object () => get_return_type_n(ty)(n);

    public static object n() => 0;

    public static string ty() => /* error: else */ default;

    public static string ty() => (FunTy(p)(r) ? get_return_type_n(r)((n - 1)) : (/* error: _ */ default ? ErrorTy : /* error:  */ default));

    public static object () => lower_module(m)(types)(ust);

    public static object IRModule() => name;

    public static object m() => name;

    public static T3974 defs<T3974>() => lower_defs(m.defs)(types)(ust)(0);

    public static object () => lower_defs(defs)(types)(ust)(i);

    public static object i() => list_length(defs);

    public static T3985 lower_def<T3985>(object list_at) => /* error: ) */ default(types)(ust);

    public static T3988 lower_defs<T3988>() => types(ust)((i + 1));

    public static object () => /* error:  */ default;

    public static T3992 Checks<T3992>() => all(names)(referenced);

    public static T3994 expressions<T3994>() => either(defined);

    public static T3996 at<T3996>() => top(level);

    public static object are() => functions;

    public static object are() => names;

    public static object are() => names(capitalized);

    public static T4004 or<T4004>() => /* error: in */ default(local)(scope)(parameters);

    public static T4006 bindings<T4006>() => match(patterns);

    public static T4006 bindings<T4006>() => /* error: . */ default;

    public static T2283 The<T2283>() => uses(a)(Scope)(which)(is)(a)(list)(of)(known)(names.It)(walks);

    public static T4024 every<T4024>() => and(reports)(errors)(for)(unknown)(names.);

    public static object () => /* error:  */ default;

    public static object () => /* error: } */ default;

    public static object () => empty_scope;

    public static object Scope() => names;

    public static object () => scope_has(sc)(name);

    public static T4037 scope_has_loop<T4037>() => /* error: . */ default(names)(name)(0)(list_length(sc.names));

    public static object () => scope_has_loop(names)(name)(i)(len);

    public static object i() => len;

    public static object list_at() => (i == name);

    public static T4037 scope_has_loop<T4037>() => name((i + 1))(len);

    public static object () => scope_add(sc)(name);

    public static object Scope() => names;

    public static T4053 name<T4053>() => /* error: ++ */ default(sc.names);

    public static object () => /* error:  */ default;

    public static object () => builtin_names;

    public static object () => /* error:  */ default;

    public static object () => is_type_name(name);

    public static T4060 text_length<T4060>() => /* error: == */ default(0);

    public static bool is_letter(object char_at) => (/* error: ) */ default && is_upper_char(char_at(name)(0)));

    public static object () => is_upper_char(c);

    public static object code() => char_code(c);

    public static object code() => (65 && (code <= 90));

    public static object () => /* error:  */ default;

    public static object ,() => top_level_names;

    public static object List() => /* error: , */ default;

    public static object ,() => ctor_names;

    public static object List() => /* error:  */ default;

    public static object () => /* error:  */ default;

    public static object () => collect_top_level_names(defs)(i)(len)(acc)(errs);

    public static object i() => len;

    public static object CollectResult() => names;

    public static T2695 acc<T2695>() => errors;

    public static object errs() => /* error:  */ default;

    public static T4089 def<T4089>() => list_at(defs)(i);

    public static T4053 name<T4053>() => def.name.value;

    public static object list_contains() => name;

    public static T4097 collect_top_level_names<T4097>() => (i + 1)(len)(acc)((errs + new List<object> { make_error("CDX3001")(("Duplicate definition: " + name)) }));

    public static T4097 collect_top_level_names<T4097>() => (i + 1)(len)((acc + new List<object> { name }))(errs);

    public static object ,() => errors;

    public static object List() => /* error:  */ default;

    public static object () => list_contains(xs)(name);

    public static T4110 list_contains_loop<T4110>() => name(0)(list_length(xs));

    public static object () => list_contains_loop(xs)(name)(i)(len);

    public static object i() => len;

    public static object list_at() => (i == name);

    public static T4110 list_contains_loop<T4110>() => name((i + 1))(len);

    public static object () => collect_ctor_names(type_defs)(i)(len)(type_acc)(ctor_acc);

    public static object i() => len;

    public static object CtorCollectResult() => type_names;

    public static object type_acc() => ctor_names;

    public static object ctor_acc() => /* error:  */ default;

    public static object td() => list_at(type_defs)(i);

    public static object td() => (AVariantTypeDef(name)(params)(ctors) ? /* error:  */ default : new_type_acc);

    public static object type_acc() => new List<object> { name.value };

    public static T4144 new_ctor_acc<T4144>() => collect_variant_ctors(ctors)(0)(list_length(ctors))(ctor_acc);

    public static T4148 collect_ctor_names<T4148>() => (i + 1)(len)(new_type_acc)(new_ctor_acc);

    public static object ARecordTypeDef(object name, object params, object fields) => /* error:  */ default;

    public static T4148 collect_ctor_names<T4148>() => (i + 1)(len)((type_acc + new List<object> { name.value }))(ctor_acc);

    public static object ,() => ctor_names;

    public static object List() => /* error:  */ default;

    public static object () => collect_variant_ctors(ctors)(i)(len)(acc);

    public static object i() => len;

    public static T2695 acc<T2695>() => /* error: else */ default;

    public static T4168 ctor<T4168>() => list_at(ctors)(i);

    public static T4171 collect_variant_ctors<T4171>() => (i + 1)(len)((acc + new List<object> { ctor.name.value }));

    public static object () => /* error:  */ default;

    public static object () => build_all_names_scope(top_names)(ctor_names)(builtins);

    public static T4182 sc<T4182>() => add_names_to_scope(empty_scope)(top_names)(0)(list_length(top_names));

    public static T4188 sc2<T4188>() => add_names_to_scope(sc)(ctor_names)(0)(list_length(ctor_names));

    public static T4192 add_names_to_scope<T4192>() => builtins(0)(list_length(builtins));

    public static object () => add_names_to_scope(sc)(names)(i)(len);

    public static object i() => len;

    public static T4182 sc<T4182>() => /* error: else */ default(add_names_to_scope)(scope_add(sc)(list_at(names)(i)))(names)((i + 1))(len);

    public static object () => /* error:  */ default;

    public static object () => resolve_expr(sc)(expr);

    public static List<T4216> expr<T4216>() => (ALitExpr(val)(kind) ? new List<T4216>() : (ANameExpr(name) ? /* error:  */ default : (scope_has(sc)(name.value) || is_type_name(name.value))));

    public static string make_error() => ("Undefined name: " + name.value);

    public static object ABinaryExpr(object left, object op, object right) => /* error:  */ default;

    public static object resolve_expr() => (left + resolve_expr(sc)(right));

    public static object AUnaryExpr(object operand) => resolve_expr(sc)(operand);

    public static object AApplyExpr(object func, object arg) => /* error:  */ default;

    public static object resolve_expr() => (func + resolve_expr(sc)(arg));

    public static object AIfExpr(object cond, object then_e, object else_e) => /* error:  */ default;

    public static object resolve_expr() => ((cond + resolve_expr(sc)(then_e)) + resolve_expr(sc)(else_e));

    public static T2334 ALetExpr<T2334>(object bindings, object body) => /* error:  */ default;

    public static T4257 resolve_let<T4257>() => bindings(body)(0)(list_length(bindings))(new List<object>());

    public static T2337 ALambdaExpr<T2337>(object params, object body) => /* error:  */ default;

    public static T4188 sc2<T4188>() => add_lambda_params(sc)(params)(0)(list_length(params));

    public static object resolve_expr() => body;

    public static object AMatchExpr(object scrutinee, object arms) => /* error:  */ default;

    public static object resolve_expr() => (scrutinee + resolve_match_arms(sc)(arms)(0)(list_length(arms))(new List<object>()));

    public static T2342 AListExpr<T2342>(object elems) => /* error:  */ default;

    public static T4286 resolve_list_elems<T4286>() => elems(0)(list_length(elems))(new List<object>());

    public static object ARecordExpr(object name, object fields) => /* error:  */ default;

    public static T4295 resolve_record_fields<T4295>() => fields(0)(list_length(fields))(new List<object>());

    public static object AFieldAccess(object obj, object field) => resolve_expr(sc)(obj);

    public static T2348 ADoExpr<T2348>(object stmts) => resolve_do_stmts(sc)(stmts)(0)(list_length(stmts))(new List<object>());

    public static object AErrorExpr(object msg) => new List<object>();

    public static object () => /* error:  */ default;

    public static object () => resolve_let(sc)(bindings)(body)(i)(len)(errs);

    public static object i() => len;

    public static object errs() => resolve_expr(sc)(body);

    public static T3593 b<T3593>() => list_at(bindings)(i);

    public static T4330 bind_errs<T4330>() => resolve_expr(sc)(b.value);

    public static T4188 sc2<T4188>() => scope_add(sc)(b.name.value);

    public static T4257 resolve_let<T4257>() => bindings(body)((i + 1))(len)((errs + bind_errs));

    public static object () => /* error:  */ default;

    public static object () => add_lambda_params(sc)(params)(i)(len);

    public static object i() => len;

    public static T4182 sc<T4182>() => /* error: else */ default;

    public static object p() => list_at(params)(i);

    public static T4352 add_lambda_params<T4352>(object scope_add) => /* error: . */ default(value);

    public static T2600 params<T2600>(object i) => /* error: ) */ default(len);

    public static object () => /* error:  */ default;

    public static object () => resolve_match_arms(sc)(arms)(i)(len)(errs);

    public static object i() => len;

    public static object errs() => /* error: else */ default;

    public static T3018 arm<T3018>() => list_at(arms)(i);

    public static T4188 sc2<T4188>() => collect_pattern_names(sc)(arm.pattern);

    public static T4373 arm_errs<T4373>() => resolve_expr(sc2)(arm.body);

    public static T4377 resolve_match_arms<T4377>() => arms((i + 1))(len)((errs + arm_errs));

    public static object () => collect_pattern_names(sc)(pat);

    public static object pat() => (AVarPat(name) ? scope_add(sc)(name.value) : (ACtorPat(name)(subs) ? collect_ctor_pat_names(sc)(subs)(0)(list_length(subs)) : (ALitPat(val)(kind) ? sc : (AWildPat ? sc : /* error:  */ default))));

    public static object () => collect_ctor_pat_names(sc)(subs)(i)(len);

    public static object i() => len;

    public static T4182 sc<T4182>() => /* error: else */ default;

    public static T3051 sub<T3051>() => list_at(subs)(i);

    public static T4408 collect_ctor_pat_names<T4408>(object collect_pattern_names) => /* error: ) */ default(subs)((i + 1))(len);

    public static object () => /* error:  */ default;

    public static object () => resolve_list_elems(sc)(elems)(i)(len)(errs);

    public static object i() => len;

    public static object errs() => /* error: else */ default;

    public static T4422 errs2<T4422>() => resolve_expr(sc)(list_at(elems)(i));

    public static T4286 resolve_list_elems<T4286>() => elems((i + 1))(len)((errs + errs2));

    public static object () => /* error:  */ default;

    public static object () => resolve_record_fields(sc)(fields)(i)(len)(errs);

    public static object i() => len;

    public static object errs() => /* error: else */ default;

    public static object f() => list_at(fields)(i);

    public static T4422 errs2<T4422>() => resolve_expr(sc)(f.value);

    public static T4295 resolve_record_fields<T4295>() => fields((i + 1))(len)((errs + errs2));

    public static object () => /* error:  */ default;

    public static object () => resolve_do_stmts(sc)(stmts)(i)(len)(errs);

    public static object i() => len;

    public static object errs() => /* error: else */ default;

    public static T4457 stmt<T4457>() => list_at(stmts)(i);

    public static T4457 stmt<T4457>() => (ADoExprStmt(e) ? /* error:  */ default : errs2);

    public static object resolve_expr() => e;

    public static T4464 resolve_do_stmts<T4464>() => stmts((i + 1))(len)((errs + errs2));

    public static object ADoBindStmt(object name, object e) => /* error:  */ default;

    public static T4422 errs2<T4422>() => resolve_expr(sc)(e);

    public static T4188 sc2<T4188>() => scope_add(sc)(name.value);

    public static T4464 resolve_do_stmts<T4464>() => stmts((i + 1))(len)((errs + errs2));

    public static object () => /* error:  */ default;

    public static object () => resolve_all_defs(sc)(defs)(i)(len)(errs);

    public static object i() => len;

    public static object errs() => /* error: else */ default;

    public static T4089 def<T4089>() => list_at(defs)(i);

    public static T4495 def_scope<T4495>() => add_def_params(sc)(def.params)(0)(list_length(def.params));

    public static T4422 errs2<T4422>() => resolve_expr(def_scope)(def.body);

    public static T4502 resolve_all_defs<T4502>() => defs((i + 1))(len)((errs + errs2));

    public static object () => add_def_params(sc)(params)(i)(len);

    public static object i() => len;

    public static T4182 sc<T4182>() => /* error: else */ default;

    public static object p() => list_at(params)(i);

    public static T4515 add_def_params<T4515>(object scope_add) => /* error: . */ default(name.value);

    public static T2600 params<T2600>(object i) => /* error: ) */ default(len);

    public static object () => /* error:  */ default;

    public static object () => resolve_module(mod);

    public static T4530 top<T4530>() => collect_top_level_names(mod.defs)(0)(list_length(mod.defs))(new List<object>())(new List<object>());

    public static T4539 ctors<T4539>() => collect_ctor_names(mod.type_defs)(0)(list_length(mod.type_defs))(new List<object>())(new List<object>());

    public static T4182 sc<T4182>() => build_all_names_scope(top.names)(ctors.ctor_names)(builtin_names);

    public static T4551 expr_errs<T4551>() => resolve_all_defs(sc)(mod.defs)(0)(list_length(mod.defs))(new List<object>());

    public static object ResolveResult() => /* error:  */ default;

    public static object errors() => (top.errors + expr_errs);

    public static object top_level_names() => top.names;

    public static object type_names() => ctors.type_names;

    public static object ctor_names() => ctors.ctor_names;

    public static object () => /* error:  */ default;

    public static T2283 The<T2283>() => converts(source)(text)(into)(a)(flat)(list)(of)(tokens.It)(operates);

    public static T4576 as<T4576>() => series(of)(pure)(functions)(threading)(state)(through.The)(input)(is);

    public static object well_formed() => source;

    public static long UTF() => 8;

    public static object newline_delimited() => space_indented.;

    public static object () => /* error:  */ default;

    public static object ,() => offset;

    public static object Integer() => /* error:  */ default;

    public static object ,() => column;

    public static object Integer() => /* error: } */ default;

    public static object () => make_lex_state(src);

    public static object LexState() => source;

    public static object src() => offset;

    public static object line() => 1;

    public static long column() => 1;

    public static object () => is_at_end(st);

    public static bool st() => (offset >= text_length(st.source));

    public static object () => peek_char(st);

    public static object is_at_end() => /* error:  */ default;

    public static T4600 char_at<T4600>() => /* error: . */ default(source)(st.offset);

    public static object () => advance_char(st);

    public static T4604 peek_char<T4604>() => /* error: == */ default("\\n");

    public static object LexState() => /* error:  */ default;

    public static object source() => st.source;

    public static object offset() => (st.offset + 1);

    public static object line() => (st.line + 1);

    public static long column() => 1;

    public static object LexState() => /* error:  */ default;

    public static object source() => st.source;

    public static object offset() => (st.offset + 1);

    public static object line() => st.line;

    public static long column() => (st.column + 1);

    public static object () => /* error:  */ default;

    public static object () => skip_spaces(st);

    public static object is_at_end() => /* error:  */ default;

    public static bool st() => /* error: else */ default;

    public static T4604 peek_char<T4604>() => /* error: == */ default(" ");

    public static object skip_spaces(object advance_char) => /* error:  */ default;

    public static bool st() => /* error:  */ default;

    public static object () => /* error:  */ default;

    public static object () => scan_ident_rest(st);

    public static object is_at_end() => /* error:  */ default;

    public static bool st() => /* error: else */ default;

    public static T4631 ch<T4631>() => peek_char(st);

    public static bool is_letter() => /* error:  */ default;

    public static object scan_ident_rest(object advance_char) => /* error:  */ default;

    public static object is_digit() => /* error:  */ default;

    public static object scan_ident_rest(object advance_char) => /* error:  */ default;

    public static T4631 ch<T4631>() => "_";

    public static object scan_ident_rest(object advance_char) => /* error:  */ default;

    public static T4631 ch<T4631>() => "-";

    public static T4643 next<T4643>() => advance_char(st);

    public static object is_at_end() => /* error:  */ default;

    public static bool st() => /* error: else */ default;

    public static bool is_letter(object peek_char) => /* error:  */ default;

    public static object scan_ident_rest() => /* error:  */ default;

    public static bool st() => /* error: else */ default(st);

    public static object () => scan_digits(st);

    public static object is_at_end() => /* error:  */ default;

    public static bool st() => /* error: else */ default;

    public static T4631 ch<T4631>() => peek_char(st);

    public static object is_digit() => /* error:  */ default;

    public static object scan_digits(object advance_char) => /* error:  */ default;

    public static T4631 ch<T4631>() => "_";

    public static object scan_digits(object advance_char) => /* error:  */ default;

    public static bool st() => /* error:  */ default;

    public static object () => scan_string_body(st);

    public static object is_at_end() => /* error:  */ default;

    public static bool st() => /* error: else */ default;

    public static T4631 ch<T4631>() => peek_char(st);

    public static T4631 ch<T4631>() => "\\\"";

    public static object advance_char() => /* error:  */ default;

    public static T4631 ch<T4631>() => "\\n";

    public static bool st() => /* error: else */ default;

    public static T4631 ch<T4631>() => "\\\\";

    public static object scan_string_body(object advance_char) => st;

    public static object scan_string_body(object advance_char) => /* error:  */ default;

    public static object () => /* error:  */ default;

    public static object () => classify_word(w);

    public static string w() => "let";

    public static object LetKeyword() => /* error: else */ default;

    public static string w() => "in";

    public static object InKeyword() => /* error: else */ default;

    public static string w() => "if";

    public static object IfKeyword() => /* error: else */ default;

    public static string w() => "then";

    public static object ThenKeyword() => /* error: else */ default;

    public static string w() => "else";

    public static object ElseKeyword() => /* error: else */ default;

    public static string w() => "when";

    public static object WhenKeyword() => /* error: else */ default;

    public static string w() => "where";

    public static object WhereKeyword() => /* error: else */ default;

    public static string w() => "do";

    public static object DoKeyword() => /* error: else */ default;

    public static string w() => "record";

    public static object RecordKeyword() => /* error: else */ default;

    public static string w() => "import";

    public static object ImportKeyword() => /* error: else */ default;

    public static string w() => "export";

    public static object ExportKeyword() => /* error: else */ default;

    public static string w() => "claim";

    public static object ClaimKeyword() => /* error: else */ default;

    public static string w() => "proof";

    public static object ProofKeyword() => /* error: else */ default;

    public static string w() => "forall";

    public static object ForAllKeyword() => /* error: else */ default;

    public static string w() => "exists";

    public static object ThereExistsKeyword() => /* error: else */ default;

    public static string w() => "linear";

    public static object LinearKeyword() => /* error: else */ default;

    public static string w() => "True";

    public static object TrueKeyword() => /* error: else */ default;

    public static string w() => "False";

    public static object FalseKeyword() => /* error: else */ default;

    public static long first_code() => char_code(char_at(w)(0));

    public static long first_code() => 65;

    public static long first_code() => 90;

    public static T4725 TypeIdentifier<T4725>() => /* error: else */ default(Identifier);

    public static object Identifier() => /* error:  */ default;

    public static object () => /* error:  */ default;

    public static object LexResult() => /* error:  */ default;

    public static object LexToken() => Token;

    public static object LexState() => /* error:  */ default;

    public static object LexEnd() => /* error:  */ default;

    public static object () => make_token(kind)(text)(st);

    public static object Token() => /* error:  */ default;

    public static object kind() => kind;

    public static object text() => text;

    public static object offset() => st.offset;

    public static object line() => st.line;

    public static long column() => st.column;

    public static object () => extract_text(st)(start)(end_st);

    public static T4749 substring<T4749>() => /* error: . */ default(source)(start)((end_st.offset - start));

    public static object () => scan_token(st);

    public static object s() => skip_spaces(st);

    public static object is_at_end() => /* error:  */ default;

    public static object LexEnd() => /* error: else */ default;

    public static T4631 ch<T4631>() => peek_char(s);

    public static T4631 ch<T4631>() => "\\n";

    public static object LexToken(object make_token) => s;

    public static object advance_char() => /* error: ) */ default;

    public static T4631 ch<T4631>() => "\\\"";

    public static object start() => (s.offset + 1);

    public static T4766 after<T4766>() => scan_string_body(advance_char(s));

    public static object text_len() => ((after.offset - start) - 1);

    public static object LexToken(object make_token, object substring) => source(start)(text_len);

    public static object s() => after;

    public static bool is_letter() => /* error:  */ default;

    public static object start() => s.offset;

    public static T4766 after<T4766>() => scan_ident_rest(advance_char(s));

    public static T4782 word<T4782>() => extract_text(s)(start)(after);

    public static object LexToken(object make_token) => word;

    public static T4782 word<T4782>() => /* error: ) */ default(after);

    public static T4631 ch<T4631>() => "_";

    public static object start() => s.offset;

    public static T4766 after<T4766>() => scan_ident_rest(advance_char(s));

    public static T4782 word<T4782>() => extract_text(s)(start)(after);

    public static T4060 text_length<T4060>() => /* error: == */ default(1);

    public static object LexToken(object make_token) => s;

    public static T4766 after<T4766>() => /* error: else */ default(LexToken)(make_token(classify_word(word))(word)(s))(after);

    public static object is_digit() => /* error:  */ default;

    public static object start() => s.offset;

    public static T4766 after<T4766>() => scan_digits(advance_char(s));

    public static object is_at_end() => /* error:  */ default;

    public static object LexToken(object make_token, object extract_text) => after;

    public static object s() => after;

    public static T4604 peek_char<T4604>() => /* error: == */ default(".");

    public static T4822 after2<T4822>() => scan_digits(advance_char(after));

    public static object LexToken(object make_token, object extract_text) => after2;

    public static object s() => after2;

    public static object LexToken(object make_token, object extract_text) => after;

    public static object s() => after;

    public static object scan_operator() => /* error:  */ default;

    public static object () => scan_operator(s);

    public static T4631 ch<T4631>() => peek_char(s);

    public static T4643 next<T4643>() => advance_char(s);

    public static T4631 ch<T4631>() => "(";

    public static object LexToken(object make_token) => s;

    public static T4643 next<T4643>() => /* error: else */ default;

    public static T4631 ch<T4631>() => ")";

    public static object LexToken(object make_token) => s;

    public static T4643 next<T4643>() => /* error: else */ default;

    public static T4631 ch<T4631>() => "[";

    public static object LexToken(object make_token) => s;

    public static T4643 next<T4643>() => /* error: else */ default;

    public static T4631 ch<T4631>() => "]";

    public static object LexToken(object make_token) => s;

    public static T4643 next<T4643>() => /* error: else */ default;

    public static T4631 ch<T4631>() => "{";

    public static object LexToken(object make_token) => s;

    public static T4643 next<T4643>() => /* error: else */ default;

    public static T4631 ch<T4631>() => "}";

    public static object LexToken(object make_token) => s;

    public static T4643 next<T4643>() => /* error: else */ default;

    public static T4631 ch<T4631>() => ",";

    public static object LexToken(object make_token) => s;

    public static T4643 next<T4643>() => /* error: else */ default;

    public static T4631 ch<T4631>() => ".";

    public static object LexToken(object make_token) => s;

    public static T4643 next<T4643>() => /* error: else */ default;

    public static T4631 ch<T4631>() => "^";

    public static object LexToken(object make_token) => s;

    public static T4643 next<T4643>() => /* error: else */ default;

    public static T4631 ch<T4631>() => "&";

    public static object LexToken(object make_token) => s;

    public static T4643 next<T4643>() => /* error: else */ default(scan_multi_char_operator)(s);

    public static object () => scan_multi_char_operator(s);

    public static T4631 ch<T4631>() => peek_char(s);

    public static T4643 next<T4643>() => advance_char(s);

    public static string next_ch() => (is_at_end(next) ? "" : peek_char(next));

    public static T4631 ch<T4631>() => "+";

    public static string next_ch() => "+";

    public static object LexToken(object make_token) => s;

    public static object advance_char() => /* error: ) */ default;

    public static object LexToken(object make_token) => s;

    public static T4643 next<T4643>() => /* error: else */ default;

    public static T4631 ch<T4631>() => "-";

    public static string next_ch() => ">";

    public static object LexToken(object make_token) => s;

    public static object advance_char() => /* error: ) */ default;

    public static object LexToken(object make_token) => s;

    public static T4643 next<T4643>() => /* error: else */ default;

    public static T4631 ch<T4631>() => "*";

    public static object LexToken(object make_token) => s;

    public static T4643 next<T4643>() => /* error: else */ default;

    public static T4631 ch<T4631>() => "/";

    public static string next_ch() => "=";

    public static object LexToken(object make_token) => s;

    public static object advance_char() => /* error: ) */ default;

    public static object LexToken(object make_token) => s;

    public static T4643 next<T4643>() => /* error: else */ default;

    public static T4631 ch<T4631>() => "=";

    public static string next_ch() => "=";

    public static T4920 next2<T4920>() => advance_char(next);

    public static string next2_ch() => (is_at_end(next2) ? "" : peek_char(next2));

    public static string next2_ch() => "=";

    public static object LexToken(object make_token) => s;

    public static object advance_char() => /* error: ) */ default;

    public static object LexToken(object make_token) => s;

    public static T4920 next2<T4920>() => /* error: else */ default(LexToken)(make_token(Equals)("=")(s))(next);

    public static T4631 ch<T4631>() => ":";

    public static string next_ch() => ":";

    public static object LexToken(object make_token) => s;

    public static object advance_char() => /* error: ) */ default;

    public static object LexToken(object make_token) => s;

    public static T4643 next<T4643>() => /* error: else */ default;

    public static T4631 ch<T4631>() => "|";

    public static string next_ch() => "-";

    public static object LexToken(object make_token) => s;

    public static object advance_char() => /* error: ) */ default;

    public static object LexToken(object make_token) => s;

    public static T4643 next<T4643>() => /* error: else */ default;

    public static T4631 ch<T4631>() => "<";

    public static string next_ch() => "=";

    public static object LexToken(object make_token) => s;

    public static object advance_char() => /* error: ) */ default;

    public static string next_ch() => "-";

    public static object LexToken(object make_token) => s;

    public static object advance_char() => /* error: ) */ default;

    public static object LexToken(object make_token) => s;

    public static T4643 next<T4643>() => /* error: else */ default;

    public static T4631 ch<T4631>() => ">";

    public static string next_ch() => "=";

    public static object LexToken(object make_token) => s;

    public static object advance_char() => /* error: ) */ default;

    public static object LexToken(object make_token) => s;

    public static T4643 next<T4643>() => /* error: else */ default(LexToken)(make_token(ErrorToken)(char_at(s.source)(s.offset))(s))(next);

    public static object () => /* error:  */ default;

    public static T4987 Collect<T4987>() => tokens(from)(source)(into)(a)(list.);

    public static object () => tokenize_loop(st)(acc);

    public static object scan_token() => /* error:  */ default;

    public static object LexToken(object tok, object next) => /* error:  */ default;

    public static bool tok() => (kind == EndOfFile);

    public static T2695 acc<T2695>() => new List<object> { tok };

    public static object tokenize_loop() => (acc + new List<object> { tok });

    public static object LexEnd() => /* error:  */ default;

    public static T2695 acc<T2695>() => new List<object> { make_token(EndOfFile)("")(st) };

    public static object () => tokenize(src);

    public static object tokenize_loop(object make_lex_state) => new List<object>();

    public static object () => /* error:  */ default;

    public static T2283 The<T2283>() => transforms(a)(flat)(token)(list)(into)(a)(concrete)(syntax)(tree.);

    public static T5027 It<T5027>() => a(recursive)(descent)(parser)(threading)(state)(through)(pure)(functions.);

    public static object in() => /* error:  */ default;

    public static object non_final() => body;

    public static T5034 because<T5034>() => Stage(0)(parser)(greedily)(consumes);

    public static T5036 if_branches<T5036>() => checking(indentation.);

    public static object () => /* error:  */ default;

    public static object ,() => pos;

    public static object Integer() => /* error: } */ default;

    public static object () => make_parse_state(toks);

    public static object ParseState() => tokens;

    public static object toks() => pos;

    public static object () => current(st);

    public static object list_at() => /* error: . */ default(tokens)(st.pos);

    public static object () => current_kind(st);

    public static object current() => /* error: ) */ default.kind;

    public static object () => advance(st);

    public static object ParseState() => tokens;

    public static bool st() => tokens;

    public static object pos() => (st.pos + 1);

    public static object () => is_done(st);

    public static object current_kind() => /* error:  */ default;

    public static bool EndOfFile() => true;

    public static object () => peek_kind(st)(offset);

    public static object list_at() => /* error: . */ default(tokens)((st.pos + offset));

    public static object kind() => /* error:  */ default;

    public static object () => /* error:  */ default;

    public static T3574 Each<T3574>() => function(returns)(the)(parsed)(node)(and)(the)(updated)(state.);

    public static object ParseExprResult() => /* error:  */ default;

    public static object ExprOk() => Expr;

    public static object ParseState() => /* error:  */ default;

    public static object ParsePatResult() => /* error:  */ default;

    public static object PatOk() => Pat;

    public static object ParseState() => /* error:  */ default;

    public static object ParseTypeResult() => /* error:  */ default;

    public static object TypeOk() => TypeExpr;

    public static object ParseState() => /* error:  */ default;

    public static object ParseDefResult() => /* error:  */ default;

    public static object DefOk() => Def;

    public static object ParseState() => /* error:  */ default;

    public static object DefNone() => ParseState;

    public static object ParseTypeDefResult() => /* error:  */ default;

    public static object TypeDefOk() => TypeDef;

    public static object ParseState() => /* error:  */ default;

    public static object TypeDefNone() => ParseState;

    public static object () => /* error:  */ default;

    public static object () => is_ident(k);

    public static object k() => (Identifier ? true : (/* error: _ */ default ? false : /* error:  */ default));

    public static object () => is_type_ident(k);

    public static object k() => (TypeIdentifier ? true : (/* error: _ */ default ? false : /* error:  */ default));

    public static object () => is_arrow(k);

    public static object k() => (Arrow ? true : (/* error: _ */ default ? false : /* error:  */ default));

    public static object () => is_equals(k);

    public static object k() => (Equals ? true : (/* error: _ */ default ? false : /* error:  */ default));

    public static object () => is_colon(k);

    public static object k() => (Colon ? true : (/* error: _ */ default ? false : /* error:  */ default));

    public static object () => is_comma(k);

    public static object k() => (Comma ? true : (/* error: _ */ default ? false : /* error:  */ default));

    public static object () => is_pipe(k);

    public static object k() => (Pipe ? true : (/* error: _ */ default ? false : /* error:  */ default));

    public static object () => is_dot(k);

    public static object k() => (Dot ? true : (/* error: _ */ default ? false : /* error:  */ default));

    public static object () => is_left_paren(k);

    public static object k() => (LeftParen ? true : (/* error: _ */ default ? false : /* error:  */ default));

    public static object () => is_left_brace(k);

    public static object k() => (LeftBrace ? true : (/* error: _ */ default ? false : /* error:  */ default));

    public static object () => is_left_bracket(k);

    public static object k() => (LeftBracket ? true : (/* error: _ */ default ? false : /* error:  */ default));

    public static object () => is_right_brace(k);

    public static object k() => (RightBrace ? true : (/* error: _ */ default ? false : /* error:  */ default));

    public static object () => is_right_bracket(k);

    public static object k() => (RightBracket ? true : (/* error: _ */ default ? false : /* error:  */ default));

    public static object () => is_if_keyword(k);

    public static object k() => (IfKeyword ? true : (/* error: _ */ default ? false : /* error:  */ default));

    public static object () => is_let_keyword(k);

    public static object k() => (LetKeyword ? true : (/* error: _ */ default ? false : /* error:  */ default));

    public static object () => is_when_keyword(k);

    public static object k() => (WhenKeyword ? true : (/* error: _ */ default ? false : /* error:  */ default));

    public static object () => is_do_keyword(k);

    public static object k() => (DoKeyword ? true : (/* error: _ */ default ? false : /* error:  */ default));

    public static object () => is_in_keyword(k);

    public static object k() => (InKeyword ? true : (/* error: _ */ default ? false : /* error:  */ default));

    public static object () => is_minus(k);

    public static object k() => (Minus ? true : (/* error: _ */ default ? false : /* error:  */ default));

    public static object () => is_dedent(k);

    public static object k() => (Dedent ? true : (/* error: _ */ default ? false : /* error:  */ default));

    public static object () => is_left_arrow(k);

    public static object k() => (LeftArrow ? true : (/* error: _ */ default ? false : /* error:  */ default));

    public static object () => is_record_keyword(k);

    public static object k() => (RecordKeyword ? true : (/* error: _ */ default ? false : /* error:  */ default));

    public static object () => is_underscore(k);

    public static object k() => (Underscore ? true : (/* error: _ */ default ? false : /* error:  */ default));

    public static object () => is_literal(k);

    public static object k() => (IntegerLiteral ? true : (NumberLiteral ? true : (TextLiteral ? true : (TrueKeyword ? true : (FalseKeyword ? true : (/* error: _ */ default ? false : /* error:  */ default))))));

    public static object () => is_app_start(k);

    public static object k() => (Identifier ? true : (TypeIdentifier ? true : (IntegerLiteral ? true : (NumberLiteral ? true : (TextLiteral ? true : (TrueKeyword ? true : (FalseKeyword ? true : (LeftParen ? true : (LeftBracket ? true : (/* error: _ */ default ? false : /* error:  */ default))))))))));

    public static object () => is_compound(e);

    public static object e() => (MatchExpr(s)(arms) ? true : (IfExpr(c)(t)(el) ? true : (LetExpr(binds)(body) ? true : (DoExpr(stmts) ? true : (/* error: _ */ default ? false : /* error:  */ default)))));

    public static object () => is_type_arg_start(k);

    public static object k() => (TypeIdentifier ? true : (Identifier ? true : (LeftParen ? true : (/* error: _ */ default ? false : /* error:  */ default))));

    public static object () => operator_precedence(k);

    public static object k() => (PlusPlus ? 5 : (ColonColon ? 5 : (Plus ? 6 : (Minus ? 6 : (Star ? 7 : (Slash ? 7 : (Caret ? 8 : (DoubleEquals ? 4 : (NotEquals ? 4 : (LessThan ? 4 : (GreaterThan ? 4 : (LessOrEqual ? 4 : (GreaterOrEqual ? 4 : (TripleEquals ? 4 : (Ampersand ? 3 : (Pipe ? 2 : (/* error: _ */ default ? (0 - 1) : /* error:  */ default)))))))))))))))));

    public static object () => /* error:  */ default;

    public static object () => expect(kind)(st);

    public static T5193 is_done<T5193>() => /* error: then */ default(st);

    public static object advance() => /* error:  */ default;

    public static object () => skip_newlines(st);

    public static T5193 is_done<T5193>() => /* error: then */ default(st);

    public static object current_kind() => /* error:  */ default;

    public static T5202 Newline<T5202>() => skip_newlines(advance(st));

    public static T5205 Indent<T5205>() => skip_newlines(advance(st));

    public static T5208 Dedent<T5208>() => skip_newlines(advance(st));

    public static bool st() => /* error:  */ default;

    public static object () => /* error:  */ default;

    public static object () => parse_type(st);

    public static T5214 result<T5214>() => parse_type_atom(st);

    public static object unwrap_type_ok() => parse_type_continue;

    public static object () => parse_type_continue(left)(st);

    public static object is_arrow(object current_kind) => /* error:  */ default;

    public static T5222 st2<T5222>() => advance(st);

    public static T5224 right_result<T5224>() => parse_type(st2);

    public static object unwrap_type_ok() => make_fun_type(left);

    public static object TypeOk() => st;

    public static object () => make_fun_type(left)(right)(st);

    public static object TypeOk() => FunType(left)(right);

    public static bool st() => /* error:  */ default;

    public static object () => unwrap_type_ok(r)(f);

    public static object r() => (TypeOk(t)(st) ? f(t)(st) : /* error:  */ default);

    public static object () => parse_type_atom(st);

    public static object is_ident(object current_kind) => /* error:  */ default;

    public static bool tok() => current(st);

    public static T5251 parse_type_args<T5251>() => NamedType(tok);

    public static object advance() => /* error: ) */ default;

    public static object is_type_ident(object current_kind) => /* error:  */ default;

    public static bool tok() => current(st);

    public static T5251 parse_type_args<T5251>() => NamedType(tok);

    public static object advance() => /* error: ) */ default;

    public static object is_left_paren(object current_kind) => /* error:  */ default;

    public static object parse_paren_type(object advance) => /* error:  */ default;

    public static bool tok() => current(st);

    public static object TypeOk() => NamedType(tok);

    public static object advance() => /* error: ) */ default;

    public static object () => parse_paren_type(st);

    public static T5272 inner<T5272>() => parse_type(st);

    public static object unwrap_type_ok() => finish_paren_type;

    public static object () => finish_paren_type(t)(st);

    public static T5222 st2<T5222>() => expect(RightParen)(st);

    public static object TypeOk() => ParenType(t);

    public static T5222 st2<T5222>() => /* error:  */ default;

    public static object () => parse_type_args(base_type)(st);

    public static T5193 is_done<T5193>() => /* error: then */ default(TypeOk)(base_type)(st);

    public static object is_type_arg_start(object current_kind) => /* error:  */ default;

    public static object parse_type_arg_next() => st;

    public static object TypeOk() => st;

    public static object () => parse_type_arg_next(base_type)(st);

    public static T5298 arg_result<T5298>() => parse_type_atom(st);

    public static object unwrap_type_ok() => continue_type_args(base_type);

    public static object () => continue_type_args(base_type)(arg)(st);

    public static T5251 parse_type_args<T5251>() => AppType(base_type)(new List<object> { arg });

    public static bool st() => /* error:  */ default;

    public static object () => /* error:  */ default;

    public static object () => parse_pattern(st);

    public static object is_underscore(object current_kind) => /* error:  */ default;

    public static object PatOk() => WildPat(current(st));

    public static object advance() => /* error: ) */ default;

    public static object is_literal(object current_kind) => /* error:  */ default;

    public static object PatOk() => LitPat(current(st));

    public static object advance() => /* error: ) */ default;

    public static object is_type_ident(object current_kind) => /* error:  */ default;

    public static bool tok() => current(st);

    public static T5331 parse_ctor_pattern_fields<T5331>() => new List<object>()(advance(st));

    public static object is_ident(object current_kind) => /* error:  */ default;

    public static object PatOk() => VarPat(current(st));

    public static object advance() => /* error: ) */ default;

    public static object PatOk() => WildPat(current(st));

    public static object advance() => /* error: ) */ default;

    public static object () => parse_ctor_pattern_fields(ctor)(acc)(st);

    public static object is_left_paren(object current_kind) => /* error:  */ default;

    public static T5222 st2<T5222>() => advance(st);

    public static T3051 sub<T3051>() => parse_pattern(st2);

    public static T5354 unwrap_pat_ok<T5354>() => continue_ctor_fields(ctor)(acc);

    public static object PatOk() => CtorPat(ctor)(acc);

    public static bool st() => /* error:  */ default;

    public static object () => continue_ctor_fields(ctor)(acc)(p)(st);

    public static T5222 st2<T5222>() => expect(RightParen)(st);

    public static T5331 parse_ctor_pattern_fields<T5331>() => (acc + new List<object> { p })(st2);

    public static object () => unwrap_pat_ok(r)(f);

    public static object r() => (PatOk(p)(st) ? f(p)(st) : /* error:  */ default);

    public static object () => /* error:  */ default;

    public static object () => parse_expr(st);

    public static long parse_binary() => 0;

    public static object () => unwrap_expr_ok(r)(f);

    public static object r() => (ExprOk(e)(st) ? f(e)(st) : /* error:  */ default);

    public static object () => parse_binary(st)(min_prec);

    public static T5393 left_result<T5393>() => parse_unary(st);

    public static T5395 unwrap_expr_ok<T5395>() => start_binary_loop(min_prec);

    public static object () => start_binary_loop(min_prec)(left)(st);

    public static T5401 parse_binary_loop<T5401>() => st(min_prec);

    public static object () => parse_binary_loop(left)(st)(min_prec);

    public static T5193 is_done<T5193>() => /* error: then */ default(ExprOk)(left)(st);

    public static T5412 prec<T5412>() => operator_precedence(current_kind(st));

    public static T5412 prec<T5412>() => min_prec;

    public static object ExprOk() => st;

    public static string op() => current(st);

    public static T5222 st2<T5222>() => skip_newlines(advance(st));

    public static T5224 right_result<T5224>() => parse_binary(st2)((prec + 1));

    public static T5395 unwrap_expr_ok<T5395>() => continue_binary(left)(op)(min_prec);

    public static object () => continue_binary(left)(op)(min_prec)(right)(st);

    public static T5401 parse_binary_loop<T5401>() => BinExpr(left)(op)(right);

    public static bool st() => /* error:  */ default;

    public static object () => parse_unary(st);

    public static object is_minus(object current_kind) => /* error:  */ default;

    public static string op() => current(st);

    public static T5214 result<T5214>() => parse_unary(advance(st));

    public static T5395 unwrap_expr_ok<T5395>() => finish_unary(op);

    public static object parse_application() => /* error:  */ default;

    public static object () => finish_unary(op)(operand)(st);

    public static object ExprOk() => UnaryExpr(op)(operand);

    public static bool st() => /* error:  */ default;

    public static object () => parse_application(st);

    public static T5461 func_result<T5461>() => parse_atom(st);

    public static T5395 unwrap_expr_ok<T5395>() => parse_app_loop;

    public static object () => parse_app_loop(func)(st);

    public static T5469 is_compound<T5469>() => /* error: then */ default(parse_field_access)(func)(st);

    public static T5193 is_done<T5193>() => /* error: then */ default(ExprOk)(func)(st);

    public static object is_app_start(object current_kind) => /* error:  */ default;

    public static T5298 arg_result<T5298>() => parse_atom(st);

    public static T5395 unwrap_expr_ok<T5395>() => continue_app(func);

    public static object parse_field_access() => st;

    public static object () => continue_app(func)(arg)(st);

    public static T5487 parse_app_loop<T5487>() => AppExpr(func)(arg);

    public static bool st() => /* error:  */ default;

    public static object () => parse_atom(st);

    public static object is_literal(object current_kind) => /* error:  */ default;

    public static object ExprOk() => LitExpr(current(st));

    public static object advance() => /* error: ) */ default;

    public static object is_ident(object current_kind) => /* error:  */ default;

    public static object parse_field_access() => NameExpr(current(st));

    public static object advance() => /* error: ) */ default;

    public static object is_type_ident(object current_kind) => /* error:  */ default;

    public static object parse_atom_type_ident() => /* error:  */ default;

    public static object is_left_paren(object current_kind) => /* error:  */ default;

    public static object parse_paren_expr(object advance) => /* error:  */ default;

    public static object is_left_bracket(object current_kind) => /* error:  */ default;

    public static object parse_list_expr() => /* error:  */ default;

    public static object is_if_keyword(object current_kind) => /* error:  */ default;

    public static object parse_if_expr() => /* error:  */ default;

    public static object is_let_keyword(object current_kind) => /* error:  */ default;

    public static object parse_let_expr() => /* error:  */ default;

    public static object is_when_keyword(object current_kind) => /* error:  */ default;

    public static object parse_match_expr() => /* error:  */ default;

    public static object is_do_keyword(object current_kind) => /* error:  */ default;

    public static object parse_do_expr() => /* error:  */ default;

    public static object ExprOk() => ErrExpr(current(st));

    public static object advance() => /* error: ) */ default;

    public static object () => parse_field_access(node)(st);

    public static object is_dot(object current_kind) => /* error:  */ default;

    public static T5222 st2<T5222>() => advance(st);

    public static T5537 field<T5537>() => current(st2);

    public static T5539 st3<T5539>() => advance(st2);

    public static object parse_field_access() => FieldExpr(node)(field);

    public static T5539 st3<T5539>() => /* error: else */ default(ExprOk)(node)(st);

    public static object () => parse_atom_type_ident(st);

    public static bool tok() => current(st);

    public static T5222 st2<T5222>() => advance(st);

    public static object is_left_brace(object current_kind) => /* error:  */ default;

    public static object parse_record_expr() => st2;

    public static object ExprOk() => NameExpr(tok);

    public static T5222 st2<T5222>() => /* error:  */ default;

    public static object () => parse_paren_expr(st);

    public static T5222 st2<T5222>() => skip_newlines(st);

    public static T5272 inner<T5272>() => parse_expr(st2);

    public static T5395 unwrap_expr_ok<T5395>() => finish_paren_expr;

    public static object () => finish_paren_expr(e)(st);

    public static T5222 st2<T5222>() => skip_newlines(st);

    public static T5539 st3<T5539>() => expect(RightParen)(st2);

    public static object ExprOk() => ParenExpr(e);

    public static T5539 st3<T5539>() => /* error:  */ default;

    public static object () => /* error:  */ default;

    public static object () => parse_record_expr(type_name)(st);

    public static T5222 st2<T5222>() => advance(st);

    public static T5539 st3<T5539>() => skip_newlines(st2);

    public static T5587 parse_record_expr_fields<T5587>() => new List<object>()(st3);

    public static object () => parse_record_expr_fields(type_name)(acc)(st);

    public static object is_right_brace(object current_kind) => /* error:  */ default;

    public static object ExprOk() => RecordExpr(type_name)(acc);

    public static object advance() => /* error: ) */ default;

    public static object is_ident(object current_kind) => /* error:  */ default;

    public static T5601 parse_record_field<T5601>() => acc(st);

    public static object ExprOk() => RecordExpr(type_name)(acc);

    public static bool st() => /* error:  */ default;

    public static object () => parse_record_field(type_name)(acc)(st);

    public static T5611 field_name<T5611>() => current(st);

    public static T5222 st2<T5222>() => advance(st);

    public static T5539 st3<T5539>() => expect(Equals)(st2);

    public static T5618 val_result<T5618>() => parse_expr(st3);

    public static T5395 unwrap_expr_ok<T5395>() => finish_record_field(type_name)(acc)(field_name);

    public static object () => finish_record_field(type_name)(acc)(field_name)(v)(st);

    public static T5537 field<T5537>() => new RecordFieldExpr(name: field_name, value: v);

    public static T5222 st2<T5222>() => skip_newlines(st);

    public static object is_comma(object current_kind) => /* error:  */ default;

    public static T5587 parse_record_expr_fields<T5587>() => (acc + new List<object> { field })(skip_newlines(advance(st2)));

    public static T5587 parse_record_expr_fields<T5587>() => (acc + new List<object> { field })(st2);

    public static object () => parse_list_expr(st);

    public static T5222 st2<T5222>() => advance(st);

    public static T5539 st3<T5539>() => skip_newlines(st2);

    public static T5647 parse_list_elements<T5647>() => /* error: ] */ default(st3);

    public static object () => parse_list_elements(acc)(st);

    public static object is_right_bracket(object current_kind) => /* error:  */ default;

    public static object ExprOk() => ListExpr(acc);

    public static object advance() => /* error: ) */ default;

    public static T5657 elem<T5657>() => parse_expr(st);

    public static T5395 unwrap_expr_ok<T5395>() => finish_list_element(acc);

    public static object () => finish_list_element(acc)(e)(st);

    public static T5222 st2<T5222>() => skip_newlines(st);

    public static object is_comma(object current_kind) => /* error:  */ default;

    public static T5647 parse_list_elements<T5647>(object acc) => e;

    public static object skip_newlines(object advance) => /* error: ) */ default;

    public static T5647 parse_list_elements<T5647>(object acc) => e;

    public static T5222 st2<T5222>() => /* error:  */ default;

    public static object () => parse_if_expr(st);

    public static T5222 st2<T5222>() => skip_newlines(advance(st));

    public static T5681 cond<T5681>() => parse_expr(st2);

    public static T5395 unwrap_expr_ok<T5395>() => parse_if_then;

    public static object () => parse_if_then(c)(st);

    public static T5222 st2<T5222>() => skip_newlines(st);

    public static T5539 st3<T5539>() => expect(ThenKeyword)(st2);

    public static T5692 st4<T5692>() => skip_newlines(st3);

    public static T5694 then_result<T5694>() => parse_expr(st4);

    public static T5395 unwrap_expr_ok<T5395>() => parse_if_else(c);

    public static object () => parse_if_else(c)(t)(st);

    public static T5222 st2<T5222>() => skip_newlines(st);

    public static T5539 st3<T5539>() => expect(ElseKeyword)(st2);

    public static T5692 st4<T5692>() => skip_newlines(st3);

    public static T5709 else_result<T5709>() => parse_expr(st4);

    public static T5395 unwrap_expr_ok<T5395>() => finish_if(c)(t);

    public static object () => finish_if(c)(t)(e)(st);

    public static object ExprOk() => IfExpr(c)(t)(e);

    public static bool st() => /* error:  */ default;

    public static object () => parse_let_expr(st);

    public static T5222 st2<T5222>() => skip_newlines(advance(st));

    public static T5729 parse_let_bindings<T5729>() => /* error: ] */ default(st2);

    public static object () => parse_let_bindings(acc)(st);

    public static object is_ident(object current_kind) => /* error:  */ default;

    public static object parse_let_binding() => st;

    public static object is_in_keyword(object current_kind) => /* error:  */ default;

    public static T5222 st2<T5222>() => skip_newlines(advance(st));

    public static T2519 body<T2519>() => parse_expr(st2);

    public static T5395 unwrap_expr_ok<T5395>() => finish_let(acc);

    public static T2519 body<T2519>() => parse_expr(st);

    public static T5395 unwrap_expr_ok<T5395>() => finish_let(acc);

    public static object () => finish_let(acc)(b)(st);

    public static object ExprOk() => LetExpr(acc)(b);

    public static bool st() => /* error:  */ default;

    public static object () => parse_let_binding(acc)(st);

    public static T5761 name_tok<T5761>() => current(st);

    public static T5222 st2<T5222>() => advance(st);

    public static T5539 st3<T5539>() => expect(Equals)(st2);

    public static T5618 val_result<T5618>() => parse_expr(st3);

    public static T5395 unwrap_expr_ok<T5395>() => finish_let_binding(acc)(name_tok);

    public static object () => finish_let_binding(acc)(name_tok)(v)(st);

    public static object binding() => new LetBind(name: name_tok, value: v);

    public static T5222 st2<T5222>() => skip_newlines(st);

    public static object is_comma(object current_kind) => /* error:  */ default;

    public static T5729 parse_let_bindings<T5729>(object acc) => binding;

    public static object skip_newlines(object advance) => /* error: ) */ default;

    public static T5729 parse_let_bindings<T5729>(object acc) => binding;

    public static T5222 st2<T5222>() => /* error:  */ default;

    public static object () => parse_match_expr(st);

    public static T5222 st2<T5222>() => advance(st);

    public static T5794 scrut<T5794>() => parse_expr(st2);

    public static T5395 unwrap_expr_ok<T5395>() => start_match_branches;

    public static object () => start_match_branches(s)(st);

    public static T5222 st2<T5222>() => skip_newlines(st);

    public static T5803 parse_match_branches<T5803>() => new List<object>()(st2);

    public static object () => parse_match_branches(scrut)(acc)(st);

    public static object is_if_keyword(object current_kind) => /* error:  */ default;

    public static T5811 parse_one_match_branch<T5811>() => acc(st);

    public static object ExprOk() => MatchExpr(scrut)(acc);

    public static bool st() => /* error:  */ default;

    public static object () => unwrap_pat_for_expr(r)(f);

    public static object r() => (PatOk(p)(st) ? f(p)(st) : /* error:  */ default);

    public static object () => parse_one_match_branch(scrut)(acc)(st);

    public static T5222 st2<T5222>() => advance(st);

    public static object pat() => parse_pattern(st2);

    public static T5834 unwrap_pat_for_expr<T5834>() => parse_match_branch_body(scrut)(acc);

    public static object () => parse_match_branch_body(scrut)(acc)(p)(st);

    public static T5222 st2<T5222>() => expect(Arrow)(st);

    public static T5539 st3<T5539>() => skip_newlines(st2);

    public static T2519 body<T2519>() => parse_expr(st3);

    public static T5395 unwrap_expr_ok<T5395>() => finish_match_branch(scrut)(acc)(p);

    public static object () => finish_match_branch(scrut)(acc)(p)(b)(st);

    public static T3018 arm<T3018>() => new MatchArm(pattern: p, body: b);

    public static T5222 st2<T5222>() => skip_newlines(st);

    public static T5803 parse_match_branches<T5803>() => (acc + new List<object> { arm })(st2);

    public static object () => parse_do_expr(st);

    public static T5222 st2<T5222>() => skip_newlines(advance(st));

    public static T5868 parse_do_stmts<T5868>() => /* error: ] */ default(st2);

    public static object () => parse_do_stmts(acc)(st);

    public static T5193 is_done<T5193>() => /* error: then */ default(ExprOk)(DoExpr(acc))(st);

    public static T5882 is_dedent<T5882>(object current_kind) => /* error: then */ default(ExprOk)(DoExpr(acc))(st);

    public static object is_do_bind() => /* error:  */ default;

    public static object parse_do_bind_stmt() => st;

    public static object parse_do_expr_stmt() => st;

    public static object () => is_do_bind(st);

    public static object is_ident(object current_kind) => /* error:  */ default;

    public static object is_left_arrow(object peek_kind) => /* error: ) */ default;

    public static object () => parse_do_bind_stmt(acc)(st);

    public static T5761 name_tok<T5761>() => current(st);

    public static T5222 st2<T5222>() => advance(advance(st));

    public static T5618 val_result<T5618>() => parse_expr(st2);

    public static T5395 unwrap_expr_ok<T5395>() => finish_do_bind(acc)(name_tok);

    public static object () => finish_do_bind(acc)(name_tok)(v)(st);

    public static T5222 st2<T5222>() => skip_newlines(st);

    public static T5868 parse_do_stmts<T5868>(object acc) => DoBindStmt(name_tok)(v);

    public static T5222 st2<T5222>() => /* error:  */ default;

    public static object () => parse_do_expr_stmt(acc)(st);

    public static T5921 expr_result<T5921>() => parse_expr(st);

    public static T5395 unwrap_expr_ok<T5395>() => finish_do_expr(acc);

    public static object () => finish_do_expr(acc)(e)(st);

    public static T5222 st2<T5222>() => skip_newlines(st);

    public static T5868 parse_do_stmts<T5868>(object acc) => DoExprStmt(e);

    public static T5222 st2<T5222>() => /* error:  */ default;

    public static object () => /* error:  */ default;

    public static object () => parse_type_annotation(st);

    public static T5222 st2<T5222>() => advance(st);

    public static T5539 st3<T5539>() => expect(Colon)(st2);

    public static object parse_type() => /* error:  */ default;

    public static object () => parse_definition(st);

    public static T5193 is_done<T5193>() => /* error: then */ default(DefNone)(st);

    public static object is_ident(object current_kind) => /* error: then */ default(try_parse_def)(st);

    public static object is_type_ident(object current_kind) => /* error: then */ default(try_parse_def)(st);

    public static object DefNone() => /* error:  */ default;

    public static object () => try_parse_def(st);

    public static object is_colon(object peek_kind) => /* error: ) */ default;

    public static T5962 ann_result<T5962>() => parse_type_annotation(st);

    public static object unwrap_type_for_def() => /* error:  */ default;

    public static T5965 parse_def_body_with_ann<T5965>() => /* error: ] */ default(st);

    public static object () => unwrap_type_for_def(r);

    public static object r() => (TypeOk(ann_type)(st) ? /* error:  */ default : name_tok);

    public static object Token() => kind;

    public static object Identifier() => text;

    public static object offset() => 0;

    public static object line() => 0;

    public static long column() => 0;

    public static List<object> ann() => new List<object> { new TypeAnn(name: name_tok, type_expr: ann_type) };

    public static T5222 st2<T5222>() => skip_newlines(st);

    public static T5965 parse_def_body_with_ann<T5965>() => st2;

    public static object () => parse_def_body_with_ann(ann)(st);

    public static T5761 name_tok<T5761>() => current(st);

    public static T5222 st2<T5222>() => advance(st);

    public static T5990 parse_def_params_then<T5990>() => name_tok(new List<object>())(st2);

    public static object () => parse_def_params_then(ann)(name_tok)(acc)(st);

    public static object is_left_paren(object current_kind) => /* error:  */ default;

    public static T5222 st2<T5222>() => advance(st);

    public static object is_ident(object current_kind) => /* error:  */ default;

    public static T6003 param<T6003>() => current(st2);

    public static T5539 st3<T5539>() => advance(st2);

    public static T5692 st4<T5692>() => expect(RightParen)(st3);

    public static T5990 parse_def_params_then<T5990>() => name_tok((acc + new List<object> { param }))(st4);

    public static T6014 finish_def<T6014>() => name_tok(acc)(st);

    public static T6014 finish_def<T6014>() => name_tok(acc)(st);

    public static object () => finish_def(ann)(name_tok)(params)(st);

    public static T5222 st2<T5222>() => expect(Equals)(st);

    public static T5539 st3<T5539>() => skip_newlines(st2);

    public static T6029 body_result<T6029>() => parse_expr(st3);

    public static T6032 unwrap_def_body<T6032>() => ann(name_tok)(params);

    public static object () => unwrap_def_body(r)(ann)(name_tok)(params);

    public static object r() => (ExprOk(b)(st) ? /* error:  */ default : new Def(name: name_tok, params: params, ann: ann, body: b)(st));

    public static object () => /* error:  */ default;

    public static object () => parse_type_def(st);

    public static object is_type_ident(object current_kind) => /* error:  */ default;

    public static T5761 name_tok<T5761>() => current(st);

    public static T5222 st2<T5222>() => advance(st);

    public static object is_equals(object current_kind) => /* error:  */ default;

    public static T5539 st3<T5539>() => skip_newlines(advance(st2));

    public static object is_record_keyword(object current_kind) => /* error:  */ default;

    public static object parse_record_type() => st3;

    public static object is_pipe(object current_kind) => /* error:  */ default;

    public static object parse_variant_type() => st3;

    public static object TypeDefNone() => /* error:  */ default;

    public static object TypeDefNone() => /* error:  */ default;

    public static object TypeDefNone() => /* error:  */ default;

    public static object () => parse_record_type(name_tok)(st);

    public static T5222 st2<T5222>() => advance(st);

    public static T5539 st3<T5539>() => expect(LeftBrace)(st2);

    public static T5692 st4<T5692>() => skip_newlines(st3);

    public static T6077 parse_record_fields_loop<T6077>() => new List<object>()(st4);

    public static object () => parse_record_fields_loop(name_tok)(acc)(st);

    public static object is_right_brace(object current_kind) => /* error:  */ default;

    public static object TypeDefOk() => new TypeDef(name: name_tok, type_params: new List<object>(), body: RecordBody(acc));

    public static object advance() => /* error: ) */ default;

    public static object is_ident(object current_kind) => /* error:  */ default;

    public static T6089 parse_one_record_field<T6089>() => acc(st);

    public static object TypeDefOk() => new TypeDef(name: name_tok, type_params: new List<object>(), body: RecordBody(acc));

    public static bool st() => /* error:  */ default;

    public static object () => parse_one_record_field(name_tok)(acc)(st);

    public static T5611 field_name<T5611>() => current(st);

    public static T5222 st2<T5222>() => advance(st);

    public static T5539 st3<T5539>() => expect(Colon)(st2);

    public static T6104 field_type_result<T6104>() => parse_type(st3);

    public static T6107 unwrap_record_field_type<T6107>() => acc(field_name)(field_type_result);

    public static object () => unwrap_record_field_type(name_tok)(acc)(field_name)(r);

    public static object r() => (TypeOk(ft)(st) ? /* error:  */ default : field);

    public static object RecordFieldDef() => name;

    public static T5611 field_name<T5611>() => type_expr;

    public static object ft() => /* error:  */ default;

    public static T5222 st2<T5222>() => skip_newlines(st);

    public static object is_comma(object current_kind) => /* error:  */ default;

    public static T6077 parse_record_fields_loop<T6077>() => (acc + new List<object> { field })(skip_newlines(advance(st2)));

    public static T6077 parse_record_fields_loop<T6077>() => (acc + new List<object> { field })(st2);

    public static object () => parse_variant_type(name_tok)(st);

    public static T6134 parse_variant_ctors<T6134>() => new List<object>()(st);

    public static object () => parse_variant_ctors(name_tok)(acc)(st);

    public static object is_pipe(object current_kind) => /* error:  */ default;

    public static T5222 st2<T5222>() => skip_newlines(advance(st));

    public static T6145 ctor_name<T6145>() => current(st2);

    public static T5539 st3<T5539>() => advance(st2);

    public static T6152 parse_ctor_fields<T6152>() => new List<object>()(st3)(name_tok)(acc);

    public static object TypeDefOk() => new TypeDef(name: name_tok, type_params: new List<object>(), body: VariantBody(acc));

    public static bool st() => /* error:  */ default;

    public static object () => parse_ctor_fields(ctor_name)(fields)(st)(name_tok)(acc);

    public static object is_left_paren(object current_kind) => /* error:  */ default;

    public static T6165 field_result<T6165>() => parse_type(advance(st));

    public static T6169 unwrap_ctor_field<T6169>() => ctor_name(fields)(name_tok)(acc);

    public static T5222 st2<T5222>() => skip_newlines(st);

    public static T4168 ctor<T4168>() => new VariantCtorDef(name: ctor_name, fields: fields);

    public static T6134 parse_variant_ctors<T6134>() => (acc + new List<object> { ctor })(st2);

    public static object () => unwrap_ctor_field(r)(ctor_name)(fields)(name_tok)(acc);

    public static object r() => (TypeOk(ty)(st) ? /* error:  */ default : st2);

    public static object expect() => st;

    public static T6152 parse_ctor_fields<T6152>() => (fields + new List<object> { ty })(st2)(name_tok)(acc);

    public static object () => /* error:  */ default;

    public static object () => parse_document(st);

    public static T5222 st2<T5222>() => skip_newlines(st);

    public static T6197 parse_top_level<T6197>() => /* error: ] */ default(new List<object>())(st2);

    public static object () => parse_top_level(defs)(type_defs)(st);

    public static T5193 is_done<T5193>() => /* error: then */ default(new Document(defs: defs, type_defs: type_defs));

    public static T6205 try_top_level_type_def<T6205>() => type_defs(st);

    public static object () => try_top_level_type_def(defs)(type_defs)(st);

    public static T6211 td_result<T6211>() => parse_type_def(st);

    public static T6211 td_result<T6211>() => (TypeDefOk(td)(st2) ? /* error:  */ default : defs((type_defs + new List<object> { td }))(skip_newlines(st2)));

    public static object TypeDefNone(object st2) => /* error:  */ default;

    public static T6221 try_top_level_def<T6221>() => type_defs(st);

    public static object () => try_top_level_def(defs)(type_defs)(st);

    public static T6227 def_result<T6227>() => parse_definition(st);

    public static T6227 def_result<T6227>() => (DefOk(d)(st2) ? /* error:  */ default : (defs + new List<object> { d })(type_defs)(skip_newlines(st2)));

    public static object DefNone(object st2) => /* error:  */ default;

    public static T6197 parse_top_level<T6197>() => type_defs(skip_newlines(advance(st2)));

    public static object () => /* error:  */ default;

    public static T2283 The<T2283>() => syntax(tree)(represents)(the)(structure)(of)(a)(parsed)(Codex);

    public static object source() => /* error: . */ default(Each)(node)(carries)(position)(information)(for)(diagnostics.);

    public static object () => /* error:  */ default;

    public static object Expr() => /* error:  */ default;

    public static object LitExpr() => Token;

    public static object NameExpr() => Token;

    public static object AppExpr() => Expr;

    public static object Expr() => /* error:  */ default;

    public static object BinExpr() => Expr;

    public static object Token() => Expr;

    public static object UnaryExpr() => Token;

    public static object Expr() => /* error:  */ default;

    public static object IfExpr() => Expr;

    public static object Expr() => Expr;

    public static T6271 LetExpr<T6271>() => List(LetBind);

    public static object Expr() => /* error:  */ default;

    public static object MatchExpr() => Expr;

    public static object List() => /* error: ) */ default;

    public static T6276 ListExpr<T6276>() => List(Expr);

    public static object RecordExpr() => Token;

    public static object List() => /* error: ) */ default;

    public static object FieldExpr() => Expr;

    public static object Token() => /* error:  */ default;

    public static object ParenExpr() => Expr;

    public static T6283 DoExpr<T6283>() => List(DoStmt);

    public static object ErrExpr() => Token;

    public static object ,() => value;

    public static object Expr() => /* error: } */ default;

    public static object ,() => body;

    public static object Expr() => /* error: } */ default;

    public static object ,() => value;

    public static object Expr() => /* error: } */ default;

    public static object DoStmt() => /* error:  */ default;

    public static object DoBindStmt() => Token;

    public static object Expr() => /* error:  */ default;

    public static object DoExprStmt() => Expr;

    public static object () => /* error:  */ default;

    public static object Pat() => /* error:  */ default;

    public static object VarPat() => Token;

    public static object LitPat() => Token;

    public static object CtorPat() => Token;

    public static object List() => /* error: ) */ default;

    public static object WildPat() => Token;

    public static object () => /* error:  */ default;

    public static object TypeExpr() => /* error:  */ default;

    public static object NamedType() => Token;

    public static object FunType() => TypeExpr;

    public static object TypeExpr() => /* error:  */ default;

    public static object AppType() => TypeExpr;

    public static object List() => /* error: ) */ default;

    public static object ParenType() => TypeExpr;

    public static object ListType() => TypeExpr;

    public static object LinearTypeExpr() => TypeExpr;

    public static object () => /* error:  */ default;

    public static object ,() => type_expr;

    public static object TypeExpr() => /* error: } */ default;

    public static object ,() => params;

    public static object List() => /* error: , */ default;

    public static object ,() => body;

    public static object Expr() => /* error: } */ default;

    public static object ,() => type_expr;

    public static object TypeExpr() => /* error: } */ default;

    public static object ,() => fields;

    public static object List() => /* error:  */ default;

    public static object TypeBody() => /* error:  */ default;

    public static T2622 RecordBody<T2622>() => List(RecordFieldDef);

    public static T2632 VariantBody<T2632>() => List(VariantCtorDef);

    public static object ,() => type_params;

    public static object List() => /* error: , */ default;

    public static object () => /* error: } */ default;

    public static object () => /* error:  */ default;

    public static object ,() => type_defs;

    public static object List() => /* error:  */ default;

    public static object () => /* error:  */ default;

    public static T2761 A<T2761>() => is(a)(lexical)(element)(produced)(by)(the)(lexer.It)(carries)(a)(kind);

    public static T6347 the<T6347>() => source(text);

    public static T2416 and<T2416>() => information.;

    public static object ,() => text;

    public static object Text() => /* error:  */ default;

    public static object ,() => line;

    public static object Integer() => /* error:  */ default;

    public static object () => /* error: } */ default;

    public static T6360 We<T6360>() => ask(for)(the)(length)(of)(a)(token);

    public static object s() => /* error: : */ default;

    public static object () => token_length(t);

    public static T4060 text_length<T4060>() => /* error: . */ default(text);

    public static object () => /* error:  */ default;

    public static T2761 A<T2761>() => kind(describes)(what)(kind)(of)(lexical)(element)(a)(token)(represents.);

    public static T6385 This<T6385>() => the(fundamental)(enumeration)(that)(the)(lexer)(produces)(and)(the);

    public static object parser() => /* error: . */ default;

    public static T6360 We<T6360>() => /* error: : */ default;

    public static object TokenKind() => /* error:  */ default;

    public static bool EndOfFile() => /* error: | */ default(Newline);

    public static T5205 Indent<T5205>() => /* error: | */ default(Dedent);

    public static T6394 IntegerLiteral<T6394>() => /* error: | */ default(NumberLiteral);

    public static T6396 TextLiteral<T6396>() => /* error: | */ default(TrueKeyword);

    public static object FalseKeyword() => /* error: | */ default(Identifier);

    public static T4725 TypeIdentifier<T4725>() => /* error: | */ default(ProseText);

    public static T6402 ChapterHeader<T6402>() => /* error: | */ default(SectionHeader);

    public static object LetKeyword() => /* error: | */ default(InKeyword);

    public static object IfKeyword() => /* error: | */ default(ThenKeyword);

    public static object ElseKeyword() => /* error: | */ default(WhenKeyword);

    public static object WhereKeyword() => /* error: | */ default(SuchThatKeyword);

    public static object DoKeyword() => /* error: | */ default(RecordKeyword);

    public static object ImportKeyword() => /* error: | */ default(ExportKeyword);

    public static object ClaimKeyword() => /* error: | */ default(ProofKeyword);

    public static object ForAllKeyword() => /* error: | */ default(ThereExistsKeyword);

    public static object LinearKeyword() => /* error: | */ default(Equals);

    public static T6422 Colon<T6422>() => /* error: | */ default(Arrow);

    public static T6424 LeftArrow<T6424>() => /* error: | */ default(Pipe);

    public static T6426 Ampersand<T6426>() => /* error: | */ default(Plus);

    public static T6428 Minus<T6428>() => /* error: | */ default(Star);

    public static T6430 Slash<T6430>() => /* error: | */ default(Caret);

    public static T6432 PlusPlus<T6432>() => /* error: | */ default(ColonColon);

    public static T6434 DoubleEquals<T6434>() => /* error: | */ default(NotEquals);

    public static T6436 LessThan<T6436>() => /* error: | */ default(GreaterThan);

    public static T6438 LessOrEqual<T6438>() => /* error: | */ default(GreaterOrEqual);

    public static T6440 TripleEquals<T6440>() => /* error: | */ default(Turnstile);

    public static T6442 LinearProduct<T6442>() => /* error: | */ default(ForAllSymbol);

    public static T6444 ExistsSymbol<T6444>() => /* error: | */ default(LeftParen);

    public static T6446 RightParen<T6446>() => /* error: | */ default(LeftBracket);

    public static T6448 RightBracket<T6448>() => /* error: | */ default(LeftBrace);

    public static T6450 RightBrace<T6450>() => /* error: | */ default(Comma);

    public static T6452 Dot<T6452>() => /* error: | */ default(DashGreater);

    public static T6454 Underscore<T6454>() => /* error: | */ default(ErrorToken);

    public static object () => /* error:  */ default;

    public static T6463 Type<T6463>() => used(by)(the)(type)(checker)(and)(later)(stages.);

    public static object () => /* error:  */ default;

    public static object CodexType() => /* error:  */ default;

    public static T6467 IntegerTy<T6467>() => /* error: | */ default(NumberTy);

    public static T6469 TextTy<T6469>() => /* error: | */ default(BooleanTy);

    public static T6471 VoidTy<T6471>() => /* error: | */ default(NothingTy);

    public static object ErrorTy() => /* error: | */ default(FunTy)(CodexType)(CodexType);

    public static string ListTy() => CodexType;

    public static string TypeVar() => Integer;

    public static T2840 ForAllTy<T2840>() => Integer;

    public static object CodexType() => /* error:  */ default;

    public static T2844 SumTy<T2844>() => Name;

    public static object List() => /* error: ) */ default;

    public static T2848 RecordTy<T2848>() => Name;

    public static object List() => /* error: ) */ default;

    public static T2852 ConstructedTy<T2852>() => Name;

    public static object List() => /* error: ) */ default;

    public static object ,() => fields;

    public static object List() => /* error:  */ default;

    public static object ,() => type_val;

    public static object CodexType() => /* error: } */ default;

    public static object () => /* error:  */ default;

    public static object Bidirectional() => checker(for)(Codex).Every;

    public static object function() => the;

    public static object UnificationState() => explicitly;

    public static T6347 the<T6347>() => tracks(all)(type);

    public static T6501 variable<T6501>() => accumulated(during)(inference.);

    public static T2283 The<T2283>() => works;

    public static object two() => /* error: : */ default;

    public static T6506 Register<T6506>() => top_level(type)(annotations);

    public static object creating() => variables;

    public static object for() => definitions.;

    public static T6518 Infer<T6518>() => body(of)(each)(definition)(and)(unify)(with)(the)(declared)(type.);

    public static object () => /* error:  */ default;

    public static object ,() => state;

    public static object UnificationState() => /* error: } */ default;

    public static object () => /* error:  */ default;

    public static object () => infer_literal(st)(kind);

    public static object kind() => (IntLit ? new CheckResult(inferred_type: IntegerTy, state: st) : (NumLit ? new CheckResult(inferred_type: NumberTy, state: st) : (TextLit ? new CheckResult(inferred_type: TextTy, state: st) : (BoolLit ? new CheckResult(inferred_type: BooleanTy, state: st) : /* error:  */ default))));

    public static object () => /* error:  */ default;

    public static object () => infer_name(st)(env)(name);

    public static object env_has() => name;

    public static object CheckResult() => inferred_type;

    public static object env_lookup() => name;

    public static object state() => st;

    public static object CheckResult() => inferred_type;

    public static object ErrorTy() => state;

    public static T6539 add_unify_error<T6539>() => "CDX2002"(("Unknown name: " + name));

    public static object () => /* error:  */ default;

    public static object () => infer_binary(st)(env)(left)(op)(right);

    public static T6550 lr<T6550>() => infer_expr(st)(env)(left);

    public static T6554 rr<T6554>() => infer_expr(lr.state)(env)(right);

    public static T6559 infer_binary_op<T6559>() => /* error: . */ default(state)(lr.inferred_type)(rr.inferred_type)(op);

    public static object () => infer_binary_op(st)(lt)(rt)(op);

    public static string op() => (OpAdd ? infer_arithmetic(st)(lt)(rt) : (OpSub ? infer_arithmetic(st)(lt)(rt) : (OpMul ? infer_arithmetic(st)(lt)(rt) : (OpDiv ? infer_arithmetic(st)(lt)(rt) : (OpPow ? infer_arithmetic(st)(lt)(rt) : (OpEq ? infer_comparison(st)(lt)(rt) : (OpNotEq ? infer_comparison(st)(lt)(rt) : (OpLt ? infer_comparison(st)(lt)(rt) : (OpGt ? infer_comparison(st)(lt)(rt) : (OpLtEq ? infer_comparison(st)(lt)(rt) : (OpGtEq ? infer_comparison(st)(lt)(rt) : (OpAnd ? infer_logical(st)(lt)(rt) : (OpOr ? infer_logical(st)(lt)(rt) : (OpAppend ? infer_append(st)(lt)(rt) : (OpCons ? infer_cons(st)(lt)(rt) : (OpDefEq ? infer_comparison(st)(lt)(rt) : /* error:  */ default))))))))))))))));

    public static object () => infer_arithmetic(st)(lt)(rt);

    public static object r() => unify(st)(lt)(rt);

    public static object CheckResult() => inferred_type;

    public static object lt() => state;

    public static object r() => state;

    public static object () => infer_comparison(st)(lt)(rt);

    public static object r() => unify(st)(lt)(rt);

    public static object CheckResult() => inferred_type;

    public static object BooleanTy() => state;

    public static object r() => state;

    public static object () => infer_logical(st)(lt)(rt);

    public static T6643 r1<T6643>() => unify(st)(lt)(BooleanTy);

    public static T6647 r2<T6647>() => unify(r1.state)(rt)(BooleanTy);

    public static object CheckResult() => inferred_type;

    public static object BooleanTy() => state;

    public static T6647 r2<T6647>() => state;

    public static object () => infer_append(st)(lt)(rt);

    public static T6657 resolved<T6657>() => resolve(st)(lt);

    public static T6657 resolved<T6657>() => (TextTy ? /* error:  */ default : r);

    public static T6660 unify<T6660>() => rt(TextTy);

    public static object CheckResult() => inferred_type;

    public static T6469 TextTy<T6469>() => state;

    public static object r() => state;

    public static object r() => unify(st)(lt)(rt);

    public static object CheckResult() => inferred_type;

    public static object lt() => state;

    public static object r() => state;

    public static object () => infer_cons(st)(lt)(rt);

    public static T6676 list_ty<T6676>() => ListTy(lt);

    public static object r() => unify(st)(rt)(list_ty);

    public static object CheckResult() => inferred_type;

    public static T6676 list_ty<T6676>() => state;

    public static object r() => state;

    public static object () => /* error:  */ default;

    public static object () => infer_if(st)(env)(cond)(then_e)(else_e);

    public static T6694 cr<T6694>() => infer_expr(st)(env)(cond);

    public static T6643 r1<T6643>() => unify(cr.state)(cr.inferred_type)(BooleanTy);

    public static T6702 tr<T6702>() => infer_expr(r1.state)(env)(then_e);

    public static T6706 er<T6706>() => infer_expr(tr.state)(env)(else_e);

    public static T6647 r2<T6647>() => unify(er.state)(tr.inferred_type)(er.inferred_type);

    public static object CheckResult() => inferred_type;

    public static T6702 tr<T6702>() => inferred_type;

    public static object state() => r2.state;

    public static object () => /* error:  */ default;

    public static object () => infer_let(st)(env)(bindings)(body);

    public static T6726 env2<T6726>() => infer_let_bindings(st)(env)(bindings)(0)(list_length(bindings));

    public static T6730 infer_expr<T6730>() => /* error: . */ default(state)(env2.env)(body);

    public static object ,() => env;

    public static object TypeEnv() => /* error: } */ default;

    public static object () => infer_let_bindings(st)(env)(bindings)(i)(len);

    public static object i() => len;

    public static object LetBindResult() => state;

    public static bool st() => env;

    public static object env() => /* error:  */ default;

    public static T3593 b<T3593>() => list_at(bindings)(i);

    public static T6749 vr<T6749>() => infer_expr(st)(env)(b.value);

    public static T6726 env2<T6726>() => env_bind(env)(b.name.value)(vr.inferred_type);

    public static T6759 infer_let_bindings<T6759>() => /* error: . */ default(state)(env2)(bindings)((i + 1))(len);

    public static object () => /* error:  */ default;

    public static object () => infer_lambda(st)(env)(params)(body);

    public static T6774 pr<T6774>() => bind_lambda_params(st)(env)(params)(0)(list_length(params))(new List<object>());

    public static T6778 br<T6778>() => infer_expr(pr.state)(pr.env)(body);

    public static T6781 fun_ty<T6781>() => wrap_fun_type(pr.param_types)(br.inferred_type);

    public static object CheckResult() => inferred_type;

    public static T6781 fun_ty<T6781>() => state;

    public static T6778 br<T6778>() => state;

    public static object ,() => env;

    public static object TypeEnv() => /* error:  */ default;

    public static object () => /* error: } */ default;

    public static object () => bind_lambda_params(st)(env)(params)(i)(len)(acc);

    public static object i() => len;

    public static object LambdaBindResult() => state;

    public static bool st() => env;

    public static object env() => param_types;

    public static T2695 acc<T2695>() => /* error:  */ default;

    public static object p() => list_at(params)(i);

    public static T6804 fr<T6804>() => fresh_and_advance(st);

    public static T6726 env2<T6726>() => env_bind(env)(p.value)(fr.var_type);

    public static T6815 bind_lambda_params<T6815>() => /* error: . */ default(state)(env2)(params)((i + 1))(len)((acc + new List<object> { fr.var_type }));

    public static object () => wrap_fun_type(param_types)(result);

    public static T6821 wrap_fun_type_loop<T6821>() => result((list_length(param_types) - 1));

    public static object () => wrap_fun_type_loop(param_types)(result)(i);

    public static object i() => 0;

    public static T5214 result<T5214>() => /* error: else */ default(wrap_fun_type_loop)(param_types)(FunTy(list_at(param_types)(i))(result))((i - 1));

    public static object () => /* error:  */ default;

    public static object () => infer_application(st)(env)(func)(arg);

    public static T6804 fr<T6804>() => infer_expr(st)(env)(func);

    public static T6849 ar<T6849>() => infer_expr(fr.state)(env)(arg);

    public static T3404 ret<T3404>() => fresh_and_advance(ar.state);

    public static object r() => unify(ret.state)(fr.inferred_type)(FunTy(ar.inferred_type)(ret.var_type));

    public static object CheckResult() => inferred_type;

    public static T3404 ret<T3404>() => var_type;

    public static object state() => r.state;

    public static object () => /* error:  */ default;

    public static object () => infer_list(st)(env)(elems);

    public static T2965 list_length<T2965>() => /* error: == */ default(0);

    public static T6804 fr<T6804>() => fresh_and_advance(st);

    public static object CheckResult() => inferred_type;

    public static string ListTy() => /* error: . */ default(var_type);

    public static object state() => fr.state;

    public static T6879 first<T6879>() => infer_expr(st)(env)(list_at(elems)(0));

    public static T5222 st2<T5222>() => unify_list_elems(first.state)(env)(elems)(first.inferred_type)(1)(list_length(elems));

    public static object CheckResult() => inferred_type;

    public static string ListTy() => /* error: . */ default(inferred_type);

    public static object state() => st2;

    public static object () => unify_list_elems(st)(env)(elems)(elem_ty)(i)(len);

    public static object i() => len;

    public static bool st() => /* error: else */ default;

    public static T6706 er<T6706>() => infer_expr(st)(env)(list_at(elems)(i));

    public static object r() => unify(er.state)(er.inferred_type)(elem_ty);

    public static T6917 unify_list_elems<T6917>() => /* error: . */ default(state)(env)(elems)(elem_ty)((i + 1))(len);

    public static object () => /* error:  */ default;

    public static object () => infer_match(st)(env)(scrutinee)(arms);

    public static T6927 sr<T6927>() => infer_expr(st)(env)(scrutinee);

    public static T6804 fr<T6804>() => fresh_and_advance(sr.state);

    public static T5222 st2<T5222>() => infer_match_arms(fr.state)(env)(sr.inferred_type)(fr.var_type)(arms)(0)(list_length(arms));

    public static object CheckResult() => inferred_type;

    public static T6804 fr<T6804>() => var_type;

    public static object state() => st2;

    public static object () => infer_match_arms(st)(env)(scrut_ty)(result_ty)(arms)(i)(len);

    public static object i() => len;

    public static bool st() => /* error: else */ default;

    public static T3018 arm<T3018>() => list_at(arms)(i);

    public static T6726 env2<T6726>() => bind_pattern(env)(arm.pattern)(scrut_ty);

    public static T6778 br<T6778>() => infer_expr(st)(env2)(arm.body);

    public static object r() => unify(br.state)(br.inferred_type)(result_ty);

    public static T6974 infer_match_arms<T6974>() => /* error: . */ default(state)(env)(scrut_ty)(result_ty)(arms)((i + 1))(len);

    public static object () => bind_pattern(env)(pat)(ty);

    public static object pat() => (AVarPat(name) ? env_bind(env)(name.value)(ty) : (AWildPat ? env : (/* error: _ */ default ? env : /* error:  */ default)));

    public static object () => /* error:  */ default;

    public static object () => infer_do(st)(env)(stmts);

    public static T6994 infer_do_loop<T6994>() => env(stmts)(0)(list_length(stmts))(NothingTy);

    public static object () => infer_do_loop(st)(env)(stmts)(i)(len)(last_ty);

    public static object i() => len;

    public static object CheckResult() => inferred_type;

    public static object last_ty() => state;

    public static bool st() => /* error:  */ default;

    public static T4457 stmt<T4457>() => list_at(stmts)(i);

    public static T4457 stmt<T4457>() => (ADoExprStmt(e) ? /* error:  */ default : er);

    public static T6730 infer_expr<T6730>() => env(e);

    public static T6994 infer_do_loop<T6994>() => /* error: . */ default(state)(env)(stmts)((i + 1))(len)(er.inferred_type);

    public static object ADoBindStmt(object name, object e) => /* error:  */ default;

    public static T6706 er<T6706>() => infer_expr(st)(env)(e);

    public static T6726 env2<T6726>() => env_bind(env)(name.value)(er.inferred_type);

    public static T6994 infer_do_loop<T6994>() => /* error: . */ default(state)(env2)(stmts)((i + 1))(len)(er.inferred_type);

    public static object () => /* error:  */ default;

    public static object () => infer_expr(st)(env)(expr);

    public static List<T4216> expr<T4216>() => (ALitExpr(val)(kind) ? infer_literal(st)(kind) : (ANameExpr(name) ? infer_name(st)(env)(name.value) : (ABinaryExpr(left)(op)(right) ? infer_binary(st)(env)(left)(op)(right) : (AUnaryExpr(operand) ? /* error:  */ default : r))));

    public static T6730 infer_expr<T6730>() => env(operand);

    public static T7066 u<T7066>() => unify(r.state)(r.inferred_type)(IntegerTy);

    public static object CheckResult() => inferred_type;

    public static T6467 IntegerTy<T6467>() => state;

    public static T7066 u<T7066>() => state;

    public static object AApplyExpr(object func, object arg) => infer_application(st)(env)(func)(arg);

    public static object AIfExpr(object cond, object then_e, object else_e) => infer_if(st)(env)(cond)(then_e)(else_e);

    public static T2334 ALetExpr<T2334>(object bindings, object body) => infer_let(st)(env)(bindings)(body);

    public static T2337 ALambdaExpr<T2337>(object params, object body) => infer_lambda(st)(env)(params)(body);

    public static object AMatchExpr(object scrutinee, object arms) => infer_match(st)(env)(scrutinee)(arms);

    public static T2342 AListExpr<T2342>(object elems) => infer_list(st)(env)(elems);

    public static T2348 ADoExpr<T2348>(object stmts) => infer_do(st)(env)(stmts);

    public static object AFieldAccess(object obj, object field) => /* error:  */ default;

    public static object r() => infer_expr(st)(env)(obj);

    public static object CheckResult() => inferred_type;

    public static object ErrorTy() => state;

    public static object r() => state;

    public static object ARecordExpr(object name, object fields) => new CheckResult(inferred_type: ErrorTy, state: st);

    public static object AErrorExpr(object msg) => new CheckResult(inferred_type: ErrorTy, state: st);

    public static object () => /* error:  */ default;

    public static object () => resolve_type_expr(texpr);

    public static object texpr() => (ANamedType(name) ? resolve_type_name(name.value) : (AFunType(param)(ret) ? FunTy(resolve_type_expr(param))(resolve_type_expr(ret)) : (AAppType(ctor)(args) ? resolve_type_expr(ctor) : /* error:  */ default)));

    public static object () => resolve_type_name(name);

    public static T4053 name<T4053>() => "Integer";

    public static T6467 IntegerTy<T6467>() => /* error: else */ default;

    public static T4053 name<T4053>() => "Number";

    public static object NumberTy() => /* error: else */ default;

    public static T4053 name<T4053>() => "Text";

    public static T6469 TextTy<T6469>() => /* error: else */ default;

    public static T4053 name<T4053>() => "Boolean";

    public static object BooleanTy() => /* error: else */ default;

    public static T4053 name<T4053>() => "Nothing";

    public static T7162 NothingTy<T7162>() => /* error: else */ default(ConstructedTy)(new Name(value: name))(new List<object>());

    public static object () => /* error:  */ default;

    public static object () => check_def(st)(env)(def);

    public static T7170 declared<T7170>() => resolve_declared_type(st)(def);

    public static T6726 env2<T6726>() => bind_def_params(declared.state)(declared.env)(def.params)(declared.expected_type)(0)(list_length(def.params));

    public static T7182 body_r<T7182>() => infer_expr(env2.state)(env2.env)(def.body);

    public static T7066 u<T7066>() => unify(body_r.state)(env2.remaining_type)(body_r.inferred_type);

    public static object CheckResult() => inferred_type;

    public static T7170 declared<T7170>() => expected_type;

    public static object state() => u.state;

    public static object ,() => remaining_type;

    public static object CodexType() => /* error:  */ default;

    public static object ,() => env;

    public static object TypeEnv() => /* error: } */ default;

    public static object () => resolve_declared_type(st)(def);

    public static T2965 list_length<T2965>() => (/* error: . */ default(declared_type) == 0);

    public static T6804 fr<T6804>() => fresh_and_advance(st);

    public static object DefSetup() => expected_type;

    public static T6804 fr<T6804>() => var_type;

    public static object remaining_type() => fr.var_type;

    public static object state() => fr.state;

    public static object env() => builtin_type_env;

    public static string ty() => resolve_type_expr(list_at(def.declared_type)(0));

    public static object DefSetup() => expected_type;

    public static string ty() => remaining_type;

    public static string ty() => state;

    public static bool st() => env;

    public static object builtin_type_env() => /* error:  */ default;

    public static object ,() => env;

    public static object TypeEnv() => /* error:  */ default;

    public static object () => /* error: } */ default;

    public static object () => bind_def_params(st)(env)(params)(remaining)(i)(len);

    public static object i() => len;

    public static object DefParamResult() => state;

    public static bool st() => env;

    public static object env() => remaining_type;

    public static object remaining() => /* error:  */ default;

    public static object p() => list_at(params)(i);

    public static object remaining() => (FunTy(param_ty)(ret_ty) ? /* error:  */ default : env2);

    public static T7237 env_bind<T7237>() => p.name.value(param_ty);

    public static T7242 bind_def_params<T7242>() => env2(params)(ret_ty)((i + 1))(len);

    public static T6804 fr<T6804>() => fresh_and_advance(st);

    public static T6726 env2<T6726>() => env_bind(env)(p.name.value)(fr.var_type);

    public static T7242 bind_def_params<T7242>() => /* error: . */ default(state)(env2)(params)(remaining)((i + 1))(len);

    public static object () => /* error:  */ default;

    public static object ,() => state;

    public static object UnificationState() => /* error: } */ default;

    public static object () => check_module(mod);

    public static object env() => register_all_defs(empty_unification_state)(builtin_type_env)(mod.defs)(0)(list_length(mod.defs));

    public static T7276 check_all_defs<T7276>() => /* error: . */ default(state)(env.env)(mod.defs)(0)(list_length(mod.defs))(new List<object>());

    public static object () => register_all_defs(st)(env)(defs)(i)(len);

    public static object i() => len;

    public static object LetBindResult() => state;

    public static bool st() => env;

    public static object env() => /* error:  */ default;

    public static T4089 def<T4089>() => list_at(defs)(i);

    public static string ty() => ((list_length(def.declared_type) == 0) ? /* error: then */ default : fr);

    public static object fresh_and_advance() => /* error:  */ default;

    public static T6726 env2<T6726>() => env_bind(env)(def.name.value)(fr.var_type);

    public static object LetBindResult() => state;

    public static T6804 fr<T6804>() => state;

    public static object env() => env2;

    public static T6657 resolved<T6657>() => resolve_type_expr(list_at(def.declared_type)(0));

    public static object LetBindResult() => state;

    public static bool st() => env;

    public static T7237 env_bind<T7237>() => def.name.value(resolved);

    public static T7313 register_all_defs<T7313>() => /* error: . */ default(state)(ty.env)(defs)((i + 1))(len);

    public static object () => check_all_defs(st)(env)(defs)(i)(len)(acc);

    public static object i() => len;

    public static object ModuleResult() => types;

    public static T2695 acc<T2695>() => state;

    public static bool st() => /* error:  */ default;

    public static T4089 def<T4089>() => list_at(defs)(i);

    public static object r() => check_def(st)(env)(def);

    public static T6657 resolved<T6657>() => deep_resolve(r.state)(r.inferred_type);

    public static object entry() => new TypeBinding(name: def.name.value, bound_type: resolved);

    public static T7276 check_all_defs<T7276>() => /* error: . */ default(state)(env)(defs)((i + 1))(len)((acc + new List<object> { entry }));

    public static object () => /* error:  */ default;

    public static T7348 An<T7348>() => mapping(from)(names)(to)(types);

    public static T7350 used<T7350>() => type(checking.);

    public static T2283 The<T2283>() => is(extended);

    public static object entering() => bindings;

    public static object lambda() => parameters;

    public static T2416 and<T2416>() => branches;

    public static object restored() => exit.;

    public static object () => /* error:  */ default;

    public static object () => /* error: } */ default;

    public static object ,() => bound_type;

    public static object CodexType() => /* error: } */ default;

    public static object () => /* error:  */ default;

    public static object () => empty_type_env;

    public static object TypeEnv() => bindings;

    public static object () => /* error:  */ default;

    public static object () => env_lookup(env)(name);

    public static T7373 env_lookup_loop<T7373>() => /* error: . */ default(bindings)(name)(0)(list_length(env.bindings));

    public static object () => env_lookup_loop(bindings)(name)(i)(len);

    public static object i() => len;

    public static object ErrorTy() => /* error: else */ default;

    public static T3593 b<T3593>() => list_at(bindings)(i);

    public static T3593 b<T3593>() => (name == name);

    public static T3593 b<T3593>() => bound_type;

    public static T7373 env_lookup_loop<T7373>() => name((i + 1))(len);

    public static object () => env_has(env)(name);

    public static T7397 env_has_loop<T7397>() => /* error: . */ default(bindings)(name)(0)(list_length(env.bindings));

    public static object () => env_has_loop(bindings)(name)(i)(len);

    public static object i() => len;

    public static T3593 b<T3593>() => list_at(bindings)(i);

    public static T3593 b<T3593>() => (name == name);

    public static T7397 env_has_loop<T7397>() => name((i + 1))(len);

    public static object () => /* error:  */ default;

    public static object () => env_bind(env)(name)(ty);

    public static object TypeEnv() => bindings;

    public static object TypeBinding() => name;

    public static T4053 name<T4053>() => bound_type;

    public static string ty() => (/* error: ] */ default + env.bindings);

    public static object () => /* error:  */ default;

    public static object () => builtin_type_env;

    public static object e() => empty_type_env;

    public static T7428 e2<T7428>() => env_bind(e)("negate")(FunTy(IntegerTy)(IntegerTy));

    public static T7434 e3<T7434>() => env_bind(e2)("text-length")(FunTy(TextTy)(IntegerTy));

    public static T7440 e4<T7440>() => env_bind(e3)("integer-to-text")(FunTy(IntegerTy)(TextTy));

    public static T7448 e5<T7448>() => env_bind(e4)("char-at")(FunTy(TextTy)(FunTy(IntegerTy)(TextTy)));

    public static T7458 e6<T7458>() => env_bind(e5)("substring")(FunTy(TextTy)(FunTy(IntegerTy)(FunTy(IntegerTy)(TextTy))));

    public static T7464 e7<T7464>() => env_bind(e6)("is-letter")(FunTy(TextTy)(BooleanTy));

    public static T7470 e8<T7470>() => env_bind(e7)("is-digit")(FunTy(TextTy)(BooleanTy));

    public static T7476 e9<T7476>() => env_bind(e8)("is-whitespace")(FunTy(TextTy)(BooleanTy));

    public static T7482 e10<T7482>() => env_bind(e9)("char-code")(FunTy(TextTy)(IntegerTy));

    public static T7488 e11<T7488>() => env_bind(e10)("code-to-char")(FunTy(IntegerTy)(TextTy));

    public static T7498 e12<T7498>() => env_bind(e11)("text-replace")(FunTy(TextTy)(FunTy(TextTy)(FunTy(TextTy)(TextTy))));

    public static T7504 e13<T7504>() => env_bind(e12)("text-to-integer")(FunTy(TextTy)(IntegerTy));

    public static T7513 e14<T7513>() => env_bind(e13)("show")(ForAllTy(0)(FunTy(TypeVar(0))(TextTy)));

    public static T7519 e15<T7519>() => env_bind(e14)("print-line")(FunTy(TextTy)(NothingTy));

    public static T7529 e16<T7529>() => env_bind(e15)("list-length")(ForAllTy(0)(FunTy(ListTy(TypeVar(0)))(IntegerTy)));

    public static T7542 e17<T7542>() => env_bind(e16)("list-at")(ForAllTy(0)(FunTy(ListTy(TypeVar(0)))(FunTy(IntegerTy)(TypeVar(0)))));

    public static T7562 e18<T7562>() => env_bind(e17)("map")(ForAllTy(0)(ForAllTy(1)(FunTy(FunTy(TypeVar(0))(TypeVar(1)))(FunTy(ListTy(TypeVar(0)))(ListTy(TypeVar(1)))))));

    public static T7579 e19<T7579>() => env_bind(e18)("filter")(ForAllTy(0)(FunTy(FunTy(TypeVar(0))(BooleanTy))(FunTy(ListTy(TypeVar(0)))(ListTy(TypeVar(0))))));

    public static T7604 e20<T7604>() => env_bind(e19)("fold")(ForAllTy(0)(ForAllTy(1)(FunTy(FunTy(TypeVar(1))(FunTy(TypeVar(0))(TypeVar(1))))(FunTy(TypeVar(1))(FunTy(ListTy(TypeVar(0)))(TypeVar(1)))))));

    public static T7608 e21<T7608>() => env_bind(e20)("read-line")(TextTy);

    public static T7608 e21<T7608>() => Chapter;

    public static object Unification() => /* error:  */ default;

    public static T6463 Type<T6463>() => for(the)(Codex)(type)(checker.The)(UnificationState);

    public static T7619 is<T7619>() => through(all)(operations);

    public static T7621 it<T7621>() => type(variable);

    public static T7627 substitutions<T7627>() => a(counter)(for)(generating)(fresh)(variables);

    public static T2416 and<T2416>() => accumulated(diagnostics.);

    public static T7637 Naming<T7637>() => /* error: : */ default(the)(state)(is)(called)(a)(UnificationState)(because);

    public static object its() => serve(a)(specific)(purpose);

    public static object unifying() => variables;

    public static T7650 with<T7650>() => types(during)(type)(inference.It)(is)(not)(a)(general);

    public static object mutable() => /* error: . */ default;

    public static object () => /* error:  */ default;

    public static object ,() => next_id;

    public static object Integer() => /* error:  */ default;

    public static object () => /* error: } */ default;

    public static object ,() => resolved_type;

    public static object CodexType() => /* error: } */ default;

    public static object ,() => state;

    public static object UnificationState() => /* error: } */ default;

    public static object () => /* error:  */ default;

    public static object () => empty_unification_state;

    public static object UnificationState() => substitutions;

    public static long next_id() => 0;

    public static object errors() => new List<object>();

    public static object () => /* error:  */ default;

    public static object () => fresh_var(st);

    public static string TypeVar() => /* error: . */ default(next_id);

    public static object () => advance_id(st);

    public static object UnificationState() => substitutions;

    public static bool st() => substitutions;

    public static long next_id() => (st.next_id + 1);

    public static object errors() => st.errors;

    public static object () => fresh_and_advance(st);

    public static object FreshResult() => var_type;

    public static string TypeVar() => /* error: . */ default(next_id);

    public static object state() => advance_id(st);

    public static object ,() => state;

    public static object UnificationState() => /* error: } */ default;

    public static object () => /* error:  */ default;

    public static object () => subst_lookup(var_id)(entries);

    public static T7693 subst_lookup_loop<T7693>() => entries(0)(list_length(entries));

    public static object () => subst_lookup_loop(var_id)(entries)(i)(len);

    public static object i() => len;

    public static object ErrorTy() => /* error: else */ default;

    public static object entry() => list_at(entries)(i);

    public static object entry() => (var_id == var_id);

    public static object entry() => resolved_type;

    public static T7693 subst_lookup_loop<T7693>() => entries((i + 1))(len);

    public static object () => has_subst(var_id)(entries);

    public static T7715 has_subst_loop<T7715>() => entries(0)(list_length(entries));

    public static object () => has_subst_loop(var_id)(entries)(i)(len);

    public static object i() => len;

    public static object entry() => list_at(entries)(i);

    public static object entry() => (var_id == var_id);

    public static T7715 has_subst_loop<T7715>() => entries((i + 1))(len);

    public static object () => /* error:  */ default;

    public static object () => resolve(st)(ty);

    public static string ty() => (TypeVar(id) ? /* error:  */ default : has_subst(id)(st.substitutions));

    public static T7739 resolve<T7739>() => subst_lookup(id)(st.substitutions);

    public static string ty() => (/* error: _ */ default ? ty : /* error:  */ default);

    public static object () => /* error:  */ default;

    public static object () => add_subst(st)(var_id)(ty);

    public static object UnificationState() => /* error:  */ default;

    public static T7627 substitutions<T7627>() => (st.substitutions + new List<object> { new SubstEntry(var_id: var_id, resolved_type: ty) });

    public static long next_id() => st.next_id;

    public static object errors() => st.errors;

    public static object () => /* error:  */ default;

    public static object () => add_unify_error(st)(code)(msg);

    public static object UnificationState() => /* error:  */ default;

    public static T7627 substitutions<T7627>() => st.substitutions;

    public static long next_id() => st.next_id;

    public static object errors() => (st.errors + new List<object> { make_error(code)(msg) });

    public static object () => /* error:  */ default;

    public static object () => occurs_in(st)(var_id)(ty);

    public static T6657 resolved<T6657>() => resolve(st)(ty);

    public static T6657 resolved<T6657>() => (TypeVar(id) ? (id == var_id) : (FunTy(param)(ret) ? (occurs_in(st)(var_id)(param) || occurs_in(st)(var_id)(ret)) : (ListTy(elem) ? occurs_in(st)(var_id)(elem) : (/* error: _ */ default ? false : /* error:  */ default))));

    public static object () => /* error:  */ default;

    public static object () => unify(st)(a)(b);

    public static T7790 ra<T7790>() => resolve(st)(a);

    public static T7793 rb<T7793>() => resolve(st)(b);

    public static T7795 unify_resolved<T7795>() => ra(rb);

    public static object () => unify_resolved(st)(a)(b);

    public static object a() => (TypeVar(id_a) ? /* error:  */ default : occurs_in(st)(id_a)(b));

    public static object UnifyResult() => success;

    public static object state() => add_unify_error(st)("CDX2010")("Infinite type");

    public static object UnifyResult() => success;

    public static object state() => add_subst(st)(id_a)(b);

    public static T7816 unify_rhs<T7816>() => a(b);

    public static object () => unify_rhs(st)(a)(b);

    public static T3593 b<T3593>() => (TypeVar(id_b) ? /* error:  */ default : occurs_in(st)(id_b)(a));

    public static object UnifyResult() => success;

    public static object state() => add_unify_error(st)("CDX2010")("Infinite type");

    public static object UnifyResult() => success;

    public static object state() => add_subst(st)(id_b)(a);

    public static T7837 unify_structural<T7837>() => a(b);

    public static object () => unify_structural(st)(a)(b);

    public static object a() => (IntegerTy ? b switch {  } : (IntegerTy ? new UnifyResult(success: true, state: st) : (/* error: _ */ default ? unify_mismatch(st)(a)(b) : (NumberTy ? b switch {  } : (NumberTy ? new UnifyResult(success: true, state: st) : (/* error: _ */ default ? unify_mismatch(st)(a)(b) : (TextTy ? b switch {  } : (TextTy ? new UnifyResult(success: true, state: st) : (/* error: _ */ default ? unify_mismatch(st)(a)(b) : (BooleanTy ? b switch {  } : (BooleanTy ? new UnifyResult(success: true, state: st) : (/* error: _ */ default ? unify_mismatch(st)(a)(b) : (NothingTy ? b switch {  } : (NothingTy ? new UnifyResult(success: true, state: st) : (/* error: _ */ default ? unify_mismatch(st)(a)(b) : (VoidTy ? b switch {  } : (VoidTy ? new UnifyResult(success: true, state: st) : (/* error: _ */ default ? unify_mismatch(st)(a)(b) : (ErrorTy ? new UnifyResult(success: true, state: st) : (FunTy(pa)(ra) ? b switch {  } : (FunTy(pb)(rb) ? unify_fun(st)(pa)(ra)(pb)(rb) : (/* error: _ */ default ? unify_mismatch(st)(a)(b) : (ListTy(ea) ? b switch {  } : (ListTy(eb) ? unify(st)(ea)(eb) : (/* error: _ */ default ? unify_mismatch(st)(a)(b) : (/* error: _ */ default ? unify_mismatch(st)(a)(b) : /* error:  */ default))))))))))))))))))))))))));

    public static object () => unify_fun(st)(pa)(ra)(pb)(rb);

    public static T6643 r1<T6643>() => unify(st)(pa)(pb);

    public static T6643 r1<T6643>() => success;

    public static T6660 unify<T6660>() => /* error: . */ default(state)(ra)(rb);

    public static T6643 r1<T6643>() => /* error:  */ default;

    public static object () => unify_mismatch(st)(a)(b);

    public static object UnifyResult() => success;

    public static object state() => add_unify_error(st)("CDX2001")("Type mismatch");

    public static object () => /* error:  */ default;

    public static object () => deep_resolve(st)(ty);

    public static T6657 resolved<T6657>() => resolve(st)(ty);

    public static T6657 resolved<T6657>() => (FunTy(param)(ret) ? FunTy(deep_resolve(st)(param))(deep_resolve(st)(ret)) : (ListTy(elem) ? ListTy(deep_resolve(st)(elem)) : (/* error: _ */ default ? resolved : Chapter)));

    public static object Main() => /* error:  */ default;

    public static T7942 Entry<T7942>() => for(the)(Codex)(compiler.Reads)(source);

    public static object runs() => pipeline;

    public static T2416 and<T2416>() => the(C);

    public static object output() => /* error:  */ default;

    public static object () => /* error:  */ default;

    public static object () => compile(source)(module_name);

    public static T7952 tokens<T7952>() => tokenize(source);

    public static bool st() => make_parse_state(tokens);

    public static T7956 doc<T7956>() => parse_document(st);

    public static T7959 ast<T7959>() => desugar_document(doc)(module_name);

    public static T7961 check_result<T7961>() => check_module(ast);

    public static T7965 ir<T7965>() => lower_module(ast)(check_result.types)(check_result.state);

    public static object emit_full_module() => ast.type_defs;

    public static object () => /* error:  */ default;

    public static object () => compile_checked(source)(module_name);

    public static T7952 tokens<T7952>() => tokenize(source);

    public static bool st() => make_parse_state(tokens);

    public static T7956 doc<T7956>() => parse_document(st);

    public static T7959 ast<T7959>() => desugar_document(doc)(module_name);

    public static T7981 resolve_result<T7981>() => resolve_module(ast);

    public static T2965 list_length<T2965>() => (/* error: . */ default(errors) > 0);

    public static T7985 CompileError<T7985>() => /* error: . */ default(errors);

    public static T7961 check_result<T7961>() => check_module(ast);

    public static T7965 ir<T7965>() => lower_module(ast)(check_result.types)(check_result.state);

    public static T7994 CompileOk<T7994>(object emit_full_module) => /* error: . */ default(type_defs);

    public static T7961 check_result<T7961>() => /* error:  */ default;

    public static object CompileResult() => /* error:  */ default;

    public static T7994 CompileOk<T7994>() => Text;

    public static object ModuleResult() => /* error:  */ default;

    public static T7985 CompileError<T7985>() => List(Diagnostic);

    public static object () => /* error:  */ default;

    public static object () => test_source;

    public static object Console() => Nothing;

    public static object main() => { /* error:  */ default; print_line(compile(test_source)("test")); /* error:  */ default;  };

}
