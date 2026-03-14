using System.Collections.Immutable;
using Codex.Core;
using Codex.Ast;

namespace Codex.Semantics;

public sealed record ResolvedModule(
    Module Module,
    ImmutableHashSet<string> TopLevelNames,
    ImmutableHashSet<string> TypeNames,
    ImmutableHashSet<string> ConstructorNames);

public sealed class NameResolver
{
    private readonly DiagnosticBag m_diagnostics;

    private static readonly ImmutableHashSet<string> s_builtins = ImmutableHashSet.Create(
        "show", "negate", "True", "False", "Nothing"
    );

    public NameResolver(DiagnosticBag diagnostics)
    {
        m_diagnostics = diagnostics;
    }

    public ResolvedModule Resolve(Module module)
    {
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

        ImmutableHashSet<string>.Builder typeNames = ImmutableHashSet.CreateBuilder<string>();
        ImmutableHashSet<string>.Builder ctorNames = ImmutableHashSet.CreateBuilder<string>();

        foreach (TypeDef td in module.TypeDefinitions)
        {
            if (!typeNames.Add(td.Name.Value))
            {
                m_diagnostics.Error("CDX3001",
                    $"Duplicate type definition: '{td.Name.Value}' is already defined",
                    td.Span);
            }

            if (td is VariantTypeDef variant)
            {
                foreach (VariantCtorDef ctor in variant.Constructors)
                {
                    if (!ctorNames.Add(ctor.Name.Value))
                    {
                        m_diagnostics.Error("CDX3001",
                            $"Duplicate constructor: '{ctor.Name.Value}' is already defined",
                            ctor.Span);
                    }
                }
            }
        }

        ImmutableHashSet<string> topLevelNames = topLevel.ToImmutable();
        ImmutableHashSet<string> allCtors = ctorNames.ToImmutable();
        ImmutableHashSet<string> allTypeNames = typeNames.ToImmutable();

        ImmutableHashSet<string> allKnownNames = topLevelNames
            .Union(s_builtins)
            .Union(allCtors);

        foreach (Definition def in module.Definitions)
        {
            ImmutableHashSet<string> scope = allKnownNames
                .Union(def.Parameters.Select(p => p.Name.Value));
            ResolveExpr(def.Body, scope);
        }

        return new ResolvedModule(module, topLevelNames, allTypeNames, allCtors);
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

    private static bool IsTypeName(Name name) => name.IsTypeName;
}
