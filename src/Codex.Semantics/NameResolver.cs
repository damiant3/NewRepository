using System.Collections.Immutable;
using Codex.Core;
using Codex.Ast;

namespace Codex.Semantics;

// The result of name resolution: the module plus a set of all defined names.
public sealed record ResolvedModule(
    Module Module,
    ImmutableHashSet<string> TopLevelNames);

// Resolves names in a module. Reports errors for undefined references.
// Does not do type checking — just ensures every name refers to something.
public sealed class NameResolver
{
    private readonly DiagnosticBag m_diagnostics;

    // Names that are always available (built-in functions, etc.)
    private static readonly ImmutableHashSet<string> s_builtins = ImmutableHashSet.Create(
        "show", "negate", "True", "False",
        // Built-in type names are valid as constructor expressions too
        "Nothing"
    );

    public NameResolver(DiagnosticBag diagnostics)
    {
        m_diagnostics = diagnostics;
    }

    public ResolvedModule Resolve(Module module)
    {
        // Collect all top-level definition names (forward references allowed)
        ImmutableHashSet<string>.Builder topLevel = ImmutableHashSet.CreateBuilder<string>();
        foreach (Definition def in module.Definitions)
        {
            if (!topLevel.Add(def.Name.Value))
            {
                m_diagnostics.Error("CDX3001",
                    $"Duplicate definition: '{def.Name.Value}' is already defined",
                    def.Span);
            }
        }

        ImmutableHashSet<string> topLevelNames = topLevel.ToImmutable();

        foreach (Definition def in module.Definitions)
        {
            ImmutableHashSet<string> scope = topLevelNames
                .Union(s_builtins)
                .Union(def.Parameters.Select(p => p.Name.Value));
            ResolveExpr(def.Body, scope);
        }

        return new ResolvedModule(module, topLevelNames);
    }

    private void ResolveExpr(Expr expr, ImmutableHashSet<string> scope)
    {
        switch (expr)
        {
            case NameExpr name:
                if (!scope.Contains(name.Name.Value) && !IsTypeName(name.Name))
                {
                    m_diagnostics.Error("CDX3002",
                        $"Undefined name: '{name.Name.Value}'",
                        name.Span);
                }
                break;

            case LiteralExpr:
                break;

            case BinaryExpr bin:
                ResolveExpr(bin.Left, scope);
                ResolveExpr(bin.Right, scope);
                break;

            case UnaryExpr un:
                ResolveExpr(un.Operand, scope);
                break;

            case ApplyExpr app:
                ResolveExpr(app.Function, scope);
                ResolveExpr(app.Argument, scope);
                break;

            case IfExpr iff:
                ResolveExpr(iff.Condition, scope);
                ResolveExpr(iff.Then, scope);
                ResolveExpr(iff.Else, scope);
                break;

            case LetExpr let:
                ImmutableHashSet<string> letScope = scope;
                foreach (LetBinding binding in let.Bindings)
                {
                    ResolveExpr(binding.Value, letScope);
                    letScope = letScope.Add(binding.Name.Value);
                }
                ResolveExpr(let.Body, letScope);
                break;

            case LambdaExpr lam:
                ImmutableHashSet<string> lamScope = scope;
                foreach (Parameter p in lam.Parameters)
                {
                    lamScope = lamScope.Add(p.Name.Value);
                }
                ResolveExpr(lam.Body, lamScope);
                break;

            case MatchExpr match:
                ResolveExpr(match.Scrutinee, scope);
                foreach (MatchBranch branch in match.Branches)
                {
                    ImmutableHashSet<string> branchScope = scope;
                    CollectPatternBindings(branch.Pattern, ref branchScope);
                    ResolveExpr(branch.Body, branchScope);
                }
                break;

            case ListExpr list:
                foreach (Expr element in list.Elements)
                    ResolveExpr(element, scope);
                break;

            case RecordExpr rec:
                foreach (RecordFieldExpr field in rec.Fields)
                    ResolveExpr(field.Value, scope);
                break;

            case FieldAccessExpr fa:
                ResolveExpr(fa.Record, scope);
                break;

            case ErrorExpr:
                break;
        }
    }

    private static void CollectPatternBindings(Pattern pattern, ref ImmutableHashSet<string> scope)
    {
        switch (pattern)
        {
            case VarPattern v:
                scope = scope.Add(v.Name.Value);
                break;
            case CtorPattern ctor:
                foreach (Pattern sub in ctor.SubPatterns)
                    CollectPatternBindings(sub, ref scope);
                break;
            case WildcardPattern:
            case LiteralPattern:
                break;
        }
    }

    // Type names (capitalized) are always allowed as expressions — they may be constructors
    private static bool IsTypeName(Name name) => name.IsTypeName;
}
