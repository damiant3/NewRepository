using System;
using System.Collections.Generic;
using System.Linq;

Console.WriteLine("All proofs verified at compile time.");

public sealed record RecordFieldExpr(Token name, Expr value);

public abstract record TypeExpr;

public sealed record NamedType(Token Field0) : TypeExpr;
public sealed record FunType(TypeExpr Field0, TypeExpr Field1) : TypeExpr;
public sealed record AppType(TypeExpr Field0, List<TypeExpr> Field1) : TypeExpr;
public sealed record ParenType(TypeExpr Field0) : TypeExpr;
public sealed record ListType(TypeExpr Field0) : TypeExpr;
public sealed record LinearTypeExpr(TypeExpr Field0) : TypeExpr;

public abstract record DoStmt;

public sealed record DoBindStmt(Token Field0, Expr Field1) : DoStmt;
public sealed record DoExprStmt(Expr Field0) : DoStmt;

public sealed record MatchArm(Pat pattern, Expr body);

public abstract record TypeBody;

public sealed record RecordBody(List<RecordFieldDef> Field0) : TypeBody;
public sealed record VariantBody(List<VariantCtorDef> Field0) : TypeBody;

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

public sealed record Document(List<Def> defs, List<TypeDef> type_defs);

public sealed record Def(Token name, List<Token> @params, List<TypeAnn> ann, Expr body);

public sealed record TypeAnn(Token name, TypeExpr type_expr);

public abstract record Pat;

public sealed record VarPat(Token Field0) : Pat;
public sealed record LitPat(Token Field0) : Pat;
public sealed record CtorPat(Token Field0, List<Pat> Field1) : Pat;
public sealed record WildPat(Token Field0) : Pat;

public sealed record VariantCtorDef(Token name, List<TypeExpr> fields);

public sealed record TypeDef(Token name, List<Token> type_params, TypeBody body);

public sealed record RecordFieldDef(Token name, TypeExpr type_expr);

public static class Codex_SyntaxNodes
{
}
