using Codex.Core;
using Codex.Syntax;
using Codex.Ast;
using Codex.Semantics;
using Codex.Types;
using Codex.Repository;

namespace Codex.Cli;

sealed class ViewConsistencyChecker : IViewConsistencyChecker
{
    public ViewConsistencyResult Check(IReadOnlyList<ViewDefinition> definitions)
    {
        DiagnosticBag diagnostics = new();
        Desugarer desugarer = new(diagnostics);

        List<Definition> allDefinitions = [];
        List<TypeDef> allTypeDefinitions = [];
        List<ClaimDef> allClaims = [];
        List<ProofDef> allProofs = [];
        List<ImportDecl> allImports = [];
        List<ExportDecl> allExports = [];
        List<EffectDef> allEffectDefs = [];

        foreach (ViewDefinition viewDef in definitions)
        {
            SourceText source = new(viewDef.Name + ".codex", viewDef.Source);
            DocumentNode document = ParseSource(source, viewDef.Source, diagnostics);
            Module module = desugarer.Desugar(document, viewDef.Name);

            allDefinitions.AddRange(module.Definitions);
            allTypeDefinitions.AddRange(module.TypeDefinitions);
            allClaims.AddRange(module.Claims);
            allProofs.AddRange(module.Proofs);
            allImports.AddRange(module.Imports);
            allExports.AddRange(module.Exports);
            allEffectDefs.AddRange(module.EffectDefs);
        }

        if (diagnostics.HasErrors)
            return ToResult(diagnostics);

        SourceSpan combinedSpan = allDefinitions.Count > 0
            ? allDefinitions[0].Span
            : SourceSpan.Single(0, 1, 1, "<view>");
        Module combined = new(
            QualifiedName.Simple("view"),
            allDefinitions,
            allTypeDefinitions,
            allClaims,
            allProofs,
            combinedSpan)
        {
            Imports = allImports,
            Exports = allExports,
            EffectDefs = allEffectDefs
        };

        NameResolver resolver = new(diagnostics, new CompositeModuleLoader([]));
        ResolvedModule resolved = resolver.Resolve(combined);

        if (diagnostics.HasErrors)
            return ToResult(diagnostics);

        TypeChecker checker = new(diagnostics);

        foreach (ResolvedModule imported in resolved.ImportedModules)
            checker.ImportModule(imported.Module, imported.ExportedNames);

        Map<string, CodexType> types = checker.CheckModule(resolved.Module);

        if (diagnostics.HasErrors)
            return ToResult(diagnostics);

        LinearityChecker linearityChecker = new(diagnostics, types);
        linearityChecker.CheckModule(resolved.Module);

        if (diagnostics.HasErrors)
            return ToResult(diagnostics);

        return new ViewConsistencyResult(true, []);
    }

    static DocumentNode ParseSource(SourceText source, string content, DiagnosticBag diagnostics)
    {
        if (ProseParser.IsProseDocument(content))
        {
            ProseParser proseParser = new(source, diagnostics);
            return proseParser.ParseDocument();
        }
        Lexer lexer = new(source, diagnostics);
        IReadOnlyList<Token> tokens = lexer.TokenizeAll();
        Parser parser = new(tokens, diagnostics);
        return parser.ParseDocument();
    }

    static ViewConsistencyResult ToResult(DiagnosticBag diagnostics)
    {
        List<string> errors = [];
        foreach (Diagnostic diag in diagnostics.ToImmutable())
        {
            if (diag.Severity == DiagnosticSeverity.Error)
                errors.Add($"{diag.Code}: {diag.Message} {diag.Span}");
        }
        return new ViewConsistencyResult(false, errors);
    }
}
