using Codex.Ast;
using Codex.Core;
using Codex.Emit.CSharp;
using Codex.Emit.IL;
using Codex.IR;
using Codex.Semantics;
using Codex.Syntax;

namespace Codex.Types.Tests
{
    public static class Helpers
    {
        public static DiagnosticBag CheckWithProofs(string source, string moduleName = "test")
        {
            SourceText src = new("test.codex", source);
            DiagnosticBag diagnostics = new();

            Lexer lexer = new(src, diagnostics);
            IReadOnlyList<Token> tokens = lexer.TokenizeAll();
            Parser parser = new(tokens, diagnostics);
            DocumentNode document = parser.ParseDocument();

            Desugarer desugarer = new(diagnostics);
            Module module = desugarer.Desugar(document, moduleName);
            if (diagnostics.HasErrors) return diagnostics;

            NameResolver resolver = new(diagnostics);
            ResolvedModule resolved = resolver.Resolve(module);
            if (diagnostics.HasErrors) return diagnostics;

            TypeChecker checker = new(diagnostics);
            Map<string, CodexType> types = checker.CheckModule(resolved.Module);
            if (diagnostics.HasErrors) return diagnostics;

            Codex.Proofs.ProofChecker proofChecker = new(diagnostics);
            proofChecker.CheckModule(resolved.Module, types);
            return diagnostics;
        }

        public static DiagnosticBag CheckWithLinearity(string source, string moduleName = "test")
        {
            SourceText src = new("test.codex", source);
            DiagnosticBag diagnostics = new();

            Lexer lexer = new(src, diagnostics);
            IReadOnlyList<Token> tokens = lexer.TokenizeAll();
            Parser parser = new(tokens, diagnostics);
            DocumentNode document = parser.ParseDocument();

            Desugarer desugarer = new(diagnostics);
            Module module = desugarer.Desugar(document, moduleName);

            NameResolver resolver = new(diagnostics);
            ResolvedModule resolved = resolver.Resolve(module);

            TypeChecker checker = new(diagnostics);
            Map<string, CodexType> types = checker.CheckModule(resolved.Module);

            LinearityChecker linearityChecker = new(diagnostics, types);
            linearityChecker.CheckModule(resolved.Module);

            return diagnostics;
        }

        public static DiagnosticBag TypeCheckWithDiagnostics(string source, string moduleName = "test")
        {
            SourceText src = new("test.codex", source);
            DiagnosticBag diagnostics = new();

            Lexer lexer = new(src, diagnostics);
            IReadOnlyList<Token> tokens = lexer.TokenizeAll();
            Parser parser = new(tokens, diagnostics);
            DocumentNode document = parser.ParseDocument();

            Desugarer desugarer = new(diagnostics);
            Module module = desugarer.Desugar(document, moduleName);

            NameResolver resolver = new(diagnostics);
            ResolvedModule resolved = resolver.Resolve(module);

            TypeChecker checker = new(diagnostics);
            checker.CheckModule(resolved.Module);
            return diagnostics;
        }


        public static string? CompileToCS(string source, string moduleName = "test")
        {
            return CompileToTarget(source, moduleName, new CSharpEmitter());
        }

#if LEGACY_EMITTERS
        public static string? CompileToJS(string source, string moduleName = "test")
        {
            return CompileToTarget(source, moduleName, new Codex.Emit.JavaScript.JavaScriptEmitter());
        }

        public static string? CompileToPython(string source, string moduleName = "test")
        {
            return CompileToTarget(source, moduleName, new Codex.Emit.Python.PythonEmitter());
        }

        public static string? CompileToRust(string source, string moduleName = "test")
        {
            return CompileToTarget(source, moduleName, new Codex.Emit.Rust.RustEmitter());
        }

        public static string? CompileToCpp(string source, string moduleName = "test")
        {
            return CompileToTarget(source, moduleName, new Codex.Emit.Cpp.CppEmitter());
        }

        public static string? CompileToGo(string source, string moduleName = "test")
        {
            return CompileToTarget(source, moduleName, new Codex.Emit.Go.GoEmitter());
        }

        public static string? CompileToJava(string source, string moduleName = "test")
        {
            return CompileToTarget(source, moduleName, new Codex.Emit.Java.JavaEmitter());
        }

        public static string? CompileToAda(string source, string moduleName = "test")
        {
            return CompileToTarget(source, moduleName, new Codex.Emit.Ada.AdaEmitter());
        }

        public static string? CompileToBabbage(string source, string moduleName = "test")
        {
            return CompileToTarget(source, moduleName, new Codex.Emit.Babbage.BabbageEmitter());
        }

        public static string? CompileToFortran(string source, string moduleName = "test")
        {
            return CompileToTarget(source, moduleName, new Codex.Emit.Fortran.FortranEmitter());
        }

        public static string? CompileToCobol(string source, string moduleName = "test")
        {
            return CompileToTarget(source, moduleName, new Codex.Emit.Cobol.CobolEmitter());
        }
#else
        public static string? CompileToJS(string source, string moduleName = "test") => null;
        public static string? CompileToPython(string source, string moduleName = "test") => null;
        public static string? CompileToRust(string source, string moduleName = "test") => null;
        public static string? CompileToCpp(string source, string moduleName = "test") => null;
        public static string? CompileToGo(string source, string moduleName = "test") => null;
        public static string? CompileToJava(string source, string moduleName = "test") => null;
        public static string? CompileToAda(string source, string moduleName = "test") => null;
        public static string? CompileToBabbage(string source, string moduleName = "test") => null;
        public static string? CompileToFortran(string source, string moduleName = "test") => null;
        public static string? CompileToCobol(string source, string moduleName = "test") => null;
#endif

        public static byte[]? CompileToIL(string source, string moduleName = "test")
        {
            SourceText src = new("test.codex", source);
            DiagnosticBag diagnostics = new();

            DocumentNode document;
            if (ProseParser.IsProseDocument(source))
            {
                ProseParser proseParser = new(src, diagnostics);
                document = proseParser.ParseDocument();
            }
            else
            {
                Lexer lexer = new(src, diagnostics);
                IReadOnlyList<Token> tokens = lexer.TokenizeAll();
                Parser parser = new(tokens, diagnostics);
                document = parser.ParseDocument();
            }

            Desugarer desugarer = new(diagnostics);
            Module module = desugarer.Desugar(document, moduleName);
            if (diagnostics.HasErrors) return null;

            NameResolver resolver = new(diagnostics);
            ResolvedModule resolved = resolver.Resolve(module);
            if (diagnostics.HasErrors) return null;

            TypeChecker checker = new(diagnostics);
            Map<string, CodexType> types = checker.CheckModule(resolved.Module);
            if (diagnostics.HasErrors) return null;

            LinearityChecker linearityChecker = new(diagnostics, types);
            linearityChecker.CheckModule(resolved.Module);
            if (diagnostics.HasErrors) return null;

            Lowering lowering = new(types, checker.ConstructorMap, checker.TypeDefMap, diagnostics);
            IRModule irModule = lowering.Lower(resolved.Module);
            if (diagnostics.HasErrors) return null;

            ILEmitter emitter = new();
            return emitter.EmitAssembly(irModule, moduleName);
        }

        public static byte[]? CompileToRiscV(string source, string moduleName = "test")
        {
            SourceText src = new("test.codex", source);
            DiagnosticBag diagnostics = new();

            DocumentNode document;
            if (ProseParser.IsProseDocument(source))
            {
                ProseParser proseParser = new(src, diagnostics);
                document = proseParser.ParseDocument();
            }
            else
            {
                Lexer lexer = new(src, diagnostics);
                IReadOnlyList<Token> tokens = lexer.TokenizeAll();
                Parser parser = new(tokens, diagnostics);
                document = parser.ParseDocument();
            }

            Desugarer desugarer = new(diagnostics);
            Module module = desugarer.Desugar(document, moduleName);
            if (diagnostics.HasErrors) return null;

            NameResolver resolver = new(diagnostics);
            ResolvedModule resolved = resolver.Resolve(module);
            if (diagnostics.HasErrors) return null;

            TypeChecker checker = new(diagnostics);
            Map<string, CodexType> types = checker.CheckModule(resolved.Module);
            if (diagnostics.HasErrors) return null;

            LinearityChecker linearityChecker = new(diagnostics, types);
            linearityChecker.CheckModule(resolved.Module);
            if (diagnostics.HasErrors) return null;

            Lowering lowering = new(types, checker.ConstructorMap, checker.TypeDefMap, diagnostics);
            IRModule irModule = lowering.Lower(resolved.Module);
            if (diagnostics.HasErrors) return null;

            Codex.Emit.RiscV.RiscVEmitter riscvEmitter = new();
            return riscvEmitter.EmitAssembly(irModule, moduleName);
        }

        public static string? CompileToTarget(string source, string moduleName, Codex.Emit.ICodeEmitter emitter)
        {
            SourceText src = new("test.codex", source);
            DiagnosticBag diagnostics = new();

            DocumentNode document;
            if (ProseParser.IsProseDocument(source))
            {
                ProseParser proseParser = new(src, diagnostics);
                document = proseParser.ParseDocument();
            }
            else
            {
                Lexer lexer = new(src, diagnostics);
                IReadOnlyList<Token> tokens = lexer.TokenizeAll();
                Parser parser = new(tokens, diagnostics);
                document = parser.ParseDocument();
            }

            Desugarer desugarer = new(diagnostics);
            Module module = desugarer.Desugar(document, moduleName);
            if (diagnostics.HasErrors) return null;

            NameResolver resolver = new(diagnostics);
            ResolvedModule resolved = resolver.Resolve(module);
            if (diagnostics.HasErrors) return null;

            TypeChecker checker = new(diagnostics);
            Map<string, CodexType> types = checker.CheckModule(resolved.Module);
            if (diagnostics.HasErrors) return null;

            LinearityChecker linearityChecker = new(diagnostics, types);
            linearityChecker.CheckModule(resolved.Module);
            if (diagnostics.HasErrors) return null;

            Lowering lowering = new(types, checker.ConstructorMap, checker.TypeDefMap, diagnostics);
            IRModule irModule = lowering.Lower(resolved.Module);
            if (diagnostics.HasErrors) return null;

            return emitter.Emit(irModule);
        }


        public static Map<string, CodexType>? TypeCheck(
            string source, string moduleName = "test")
        {
            SourceText src = new("test.codex", source);
            DiagnosticBag diagnostics = new();

            DocumentNode document;
            if (ProseParser.IsProseDocument(source))
            {
                ProseParser proseParser = new(src, diagnostics);
                document = proseParser.ParseDocument();
            }
            else
            {
                Lexer lexer = new(src, diagnostics);
                IReadOnlyList<Token> tokens = lexer.TokenizeAll();
                Parser parser = new(tokens, diagnostics);
                document = parser.ParseDocument();
            }

            Desugarer desugarer = new(diagnostics);
            Module module = desugarer.Desugar(document, moduleName);
            if (diagnostics.HasErrors) return null;

            NameResolver resolver = new(diagnostics);
            ResolvedModule resolved = resolver.Resolve(module);
            if (diagnostics.HasErrors) return null;

            TypeChecker checker = new(diagnostics);
            Map<string, CodexType> types = checker.CheckModule(resolved.Module);
            return diagnostics.HasErrors ? null : types;
        }
    }
}
