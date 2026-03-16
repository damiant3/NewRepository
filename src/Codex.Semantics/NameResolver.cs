using Codex.Core;
using Codex.Ast;

namespace Codex.Semantics;

public sealed record ResolvedModule(
    Module Module,
    Set<string> TopLevelNames,
    Set<string> TypeNames,
    Set<string> ConstructorNames)
{
    public IReadOnlyList<ResolvedModule> ImportedModules { get; init; } = [];
}

public sealed class NameResolver(DiagnosticBag diagnostics)
{
    readonly DiagnosticBag m_diagnostics = diagnostics;
    IModuleLoader? m_loader;

    static readonly Set<string> s_builtins = Set<string>.Of(
        "show", "negate", "True", "False", "Nothing",
        "print-line", "read-line",
        "open-file", "read-all", "close-file",
        "char-at", "text-length", "substring",
        "is-letter", "is-digit", "is-whitespace",
        "text-to-integer", "integer-to-text", "text-replace",
        "char-code", "code-to-char",
        "list-length", "list-at",
        "map",
        "get-state", "set-state", "run-state"
    );

    public NameResolver(DiagnosticBag diagnostics, IModuleLoader? loader)
        : this(diagnostics)
    {
        m_loader = loader;
    }

    public ResolvedModule Resolve(Module module)
    {
        Set<string> topLevel = Set<string>.s_empty;
        foreach (Definition def in module.Definitions)
        {
            if (topLevel.Contains(def.Name.Value))
            {
                m_diagnostics.Error("CDX3001",
                    $"Duplicate definition: '{def.Name.Value}' is already defined",
                    def.Span);
            }
            topLevel = topLevel.Add(def.Name.Value);
        }

        Set<string> typeNames = Set<string>.s_empty;
        Set<string> ctorNames = Set<string>.s_empty;

        foreach (TypeDef td in module.TypeDefinitions)
        {
            if (typeNames.Contains(td.Name.Value))
            {
                m_diagnostics.Error("CDX3001",
                    $"Duplicate type definition: '{td.Name.Value}' is already defined",
                    td.Span);
            }
            typeNames = typeNames.Add(td.Name.Value);

            if (td is VariantTypeDef variant)
                foreach (VariantCtorDef ctor in variant.Constructors)
                {
                    if (ctorNames.Contains(ctor.Name.Value))
                    {
                        m_diagnostics.Error("CDX3001",
                            $"Duplicate constructor: '{ctor.Name.Value}' is already defined",
                            ctor.Span);
                    }
                    ctorNames = ctorNames.Add(ctor.Name.Value);
                }
        }

        List<ResolvedModule> importedModules = [];
        foreach (ImportDecl imp in module.Imports)
        {
            ResolvedModule? imported = m_loader?.Load(imp.ModuleName.Value);
            if (imported is null)
            {
                m_diagnostics.Error("CDX3010",
                    $"Cannot resolve import '{imp.ModuleName.Value}'",
                    imp.Span);
                continue;
            }
            importedModules.Add(imported);
            topLevel = topLevel.Union(imported.TopLevelNames);
            typeNames = typeNames.Union(imported.TypeNames);
            ctorNames = ctorNames.Union(imported.ConstructorNames);
        }

        Set<string> allKnownNames = topLevel
            .Union(s_builtins)
            .Union(ctorNames);

        foreach (Definition def in module.Definitions)
        {
            Set<string> scope = allKnownNames;
            foreach (Parameter p in def.Parameters)
                scope = scope.Add(p.Name.Value);
            ResolveExpr(def.Body, scope);
        }

        return new ResolvedModule(module, topLevel, typeNames, ctorNames)
            { ImportedModules = importedModules };
    }

    void ResolveExpr(Expr expr, Set<string> scope)
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
                Set<string> letScope = scope;
                foreach (LetBinding binding in let.Bindings)
                {
                    ResolveExpr(binding.Value, letScope);
                    letScope = letScope.Add(binding.Name.Value);
                }
                ResolveExpr(let.Body, letScope);
                break;

            case LambdaExpr lam:
                Set<string> lamScope = scope;
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
                    Set<string> branchScope = scope;
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

            case DoExpr doExpr:
            {
                Set<string> doScope = scope;
                foreach (DoStatement stmt in doExpr.Statements)
                {
                    switch (stmt)
                    {
                        case DoBindStatement bind:
                            ResolveExpr(bind.Value, doScope);
                            doScope = doScope.Add(bind.Name.Value);
                            break;
                        case DoExprStatement exprStmt:
                            ResolveExpr(exprStmt.Expression, doScope);
                            break;
                    }
                }
                break;
            }

            case ErrorExpr:
                break;
        }
    }

    static void CollectPatternBindings(Pattern pattern, ref Set<string> scope)
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

    static bool IsTypeName(Name name) => name.IsTypeName;
}
