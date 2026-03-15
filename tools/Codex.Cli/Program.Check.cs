using Codex.Core;
using Codex.Syntax;
using Codex.Ast;
using Codex.Semantics;
using Codex.Types;

namespace Codex.Cli;

public static partial class Program
{
    static int RunCheck(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("Usage: codex check <file.codex>");
            return 1;
        }

        string filePath = args[0];
        if (!File.Exists(filePath))
        {
            Console.Error.WriteLine($"File not found: {filePath}");
            return 1;
        }

        string content = File.ReadAllText(filePath);
        SourceText source = new(filePath, content);
        DiagnosticBag diagnostics = new();

        DocumentNode document = ParseSourceFile(source, content, diagnostics);

        Desugarer desugarer = new(diagnostics);
        string moduleName = Path.GetFileNameWithoutExtension(filePath);
        Module module = desugarer.Desugar(document, moduleName);

        if (diagnostics.HasErrors)
        {
            PrintDiagnostics(diagnostics);
            return 1;
        }

        NameResolver resolver = new(diagnostics);
        ResolvedModule resolved = resolver.Resolve(module);

        if (diagnostics.HasErrors)
        {
            PrintDiagnostics(diagnostics);
            return 1;
        }

        TypeChecker checker = new(diagnostics);
        Map<string, CodexType> types = checker.CheckModule(resolved.Module);

        if (diagnostics.HasErrors) { PrintDiagnostics(diagnostics); return 1; }

        LinearityChecker linearityChecker = new(diagnostics, types);
        linearityChecker.CheckModule(resolved.Module);

        if (diagnostics.HasErrors) { PrintDiagnostics(diagnostics); return 1; }

        Codex.Proofs.ProofChecker proofChecker = new(diagnostics);
        proofChecker.CheckModule(resolved.Module, types);

        if (diagnostics.HasErrors) { PrintDiagnostics(diagnostics); return 1; }

        if (!diagnostics.HasErrors)
        {
            int claimCount = resolved.Module.Claims.Count;
            int proofCount = resolved.Module.Proofs.Count;
            string proofInfo = claimCount > 0 ? $", {claimCount} claim(s), {proofCount} proof(s)" : "";
            Console.WriteLine($"✓ {module.Name}: {module.Definitions.Count} definition(s){proofInfo}, no errors.");
            foreach (KeyValuePair<string, CodexType> kv in types)
            {
                Console.WriteLine($"  {kv.Key} : {kv.Value}");
            }
        }

        PrintDiagnostics(diagnostics);
        return diagnostics.HasErrors ? 1 : 0;
    }
}
