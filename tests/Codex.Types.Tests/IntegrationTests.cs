using System.Collections.Immutable;
using Codex.Core;
using Codex.Syntax;
using Codex.Ast;
using Codex.Semantics;
using Codex.Types;
using Codex.IR;
using Codex.Emit.CSharp;
using Xunit;

namespace Codex.Types.Tests;

public class IntegrationTests
{
    private static string? CompileToCS(string source, string moduleName = "test")
    {
        SourceText src = new SourceText("test.codex", source);
        DiagnosticBag diagnostics = new DiagnosticBag();

        DocumentNode document;
        if (ProseParser.IsProseDocument(source))
        {
            ProseParser proseParser = new ProseParser(src, diagnostics);
            document = proseParser.ParseDocument();
        }
        else
        {
            Lexer lexer = new Lexer(src, diagnostics);
            IReadOnlyList<Token> tokens = lexer.TokenizeAll();
            Parser parser = new Parser(tokens, diagnostics);
            document = parser.ParseDocument();
        }

        Desugarer desugarer = new Desugarer(diagnostics);
        Module module = desugarer.Desugar(document, moduleName);
        if (diagnostics.HasErrors) return null;

        NameResolver resolver = new NameResolver(diagnostics);
        ResolvedModule resolved = resolver.Resolve(module);
        if (diagnostics.HasErrors) return null;

        TypeChecker checker = new TypeChecker(diagnostics);
        ImmutableDictionary<string, CodexType> types = checker.CheckModule(resolved.Module);
        if (diagnostics.HasErrors) return null;

        LinearityChecker linearityChecker = new LinearityChecker(diagnostics, types);
        linearityChecker.CheckModule(resolved.Module);
        if (diagnostics.HasErrors) return null;

        Lowering lowering = new Lowering(types, checker.ConstructorMap, checker.TypeDefMap, diagnostics);
        IRModule irModule = lowering.Lower(resolved.Module);
        if (diagnostics.HasErrors) return null;

        CSharpEmitter emitter = new CSharpEmitter();
        return emitter.Emit(irModule);
    }

    private static ImmutableDictionary<string, CodexType>? TypeCheck(
        string source, string moduleName = "test")
    {
        SourceText src = new SourceText("test.codex", source);
        DiagnosticBag diagnostics = new DiagnosticBag();

        DocumentNode document;
        if (ProseParser.IsProseDocument(source))
        {
            ProseParser proseParser = new ProseParser(src, diagnostics);
            document = proseParser.ParseDocument();
        }
        else
        {
            Lexer lexer = new Lexer(src, diagnostics);
            IReadOnlyList<Token> tokens = lexer.TokenizeAll();
            Parser parser = new Parser(tokens, diagnostics);
            document = parser.ParseDocument();
        }

        Desugarer desugarer = new Desugarer(diagnostics);
        Module module = desugarer.Desugar(document, moduleName);
        if (diagnostics.HasErrors) return null;

        NameResolver resolver = new NameResolver(diagnostics);
        ResolvedModule resolved = resolver.Resolve(module);
        if (diagnostics.HasErrors) return null;

        TypeChecker checker = new TypeChecker(diagnostics);
        ImmutableDictionary<string, CodexType> types = checker.CheckModule(resolved.Module);
        return diagnostics.HasErrors ? null : types;
    }

    // --- Notation-only programs ---

    [Fact]
    public void Hello_compiles_to_csharp()
    {
        string source = "square : Integer -> Integer\nsquare (x) = x * x\n\nmain : Integer\nmain = square 5";
        string? cs = CompileToCS(source, "hello");
        Assert.NotNull(cs);
        Assert.Contains("square", cs);
        Assert.Contains("main", cs);
        Assert.Contains("Console.WriteLine", cs);
    }

    [Fact]
    public void Factorial_compiles_to_csharp()
    {
        string source =
            "factorial : Integer -> Integer\n" +
            "factorial (n) = if n == 0 then 1 else n * factorial (n - 1)\n\n" +
            "main : Integer\nmain = factorial 10";
        string? cs = CompileToCS(source, "factorial");
        Assert.NotNull(cs);
        Assert.Contains("factorial", cs);
    }

    [Fact]
    public void Greeting_compiles_to_csharp()
    {
        string source =
            "greeting : Text -> Text\n" +
            "greeting (name) = \"Hello, \" ++ name ++ \"!\"\n\n" +
            "main : Text\nmain = greeting \"World\"";
        string? cs = CompileToCS(source, "greeting");
        Assert.NotNull(cs);
        Assert.Contains("string.Concat", cs);
    }

    [Fact]
    public void Let_binding_compiles()
    {
        string source =
            "clamp : Integer -> Integer -> Integer -> Integer\n" +
            "clamp (lo) (hi) (x) =\n" +
            "  let clamped = if x < lo then lo else if x > hi then hi else x\n" +
            "  in clamped";
        string? cs = CompileToCS(source, "clamp");
        Assert.NotNull(cs);
        Assert.Contains("clamped", cs);
    }

    [Fact]
    public void Fibonacci_type_checks()
    {
        string source =
            "fib (n) = if n == 0 then 0 else if n == 1 then 1 else fib (n - 1) + fib (n - 2)\n\n" +
            "main : Integer\nmain = fib 20";
        ImmutableDictionary<string, CodexType>? types = TypeCheck(source);
        Assert.NotNull(types);
        Assert.True(types!.ContainsKey("fib"));
        Assert.True(types.ContainsKey("main"));
    }

    // --- Prose-mode programs ---

    [Fact]
    public void Prose_greeting_compiles_to_csharp()
    {
        string source =
            "Chapter: Greeting\n\n" +
            "  This module greets people.\n\n" +
            "  We say:\n\n" +
            "    greet : Text -> Text\n" +
            "    greet (name) = \"Hello, \" ++ name ++ \"!\"\n\n" +
            "    main : Text\n" +
            "    main = greet \"World\"\n";
        string? cs = CompileToCS(source, "greeting");
        Assert.NotNull(cs);
        Assert.Contains("greet", cs);
        Assert.Contains("Console.WriteLine", cs);
    }

    [Fact]
    public void Prose_document_type_checks()
    {
        string source =
            "Chapter: Math\n\n" +
            "    double : Integer -> Integer\n" +
            "    double (x) = x + x\n\n" +
            "    main : Integer\n" +
            "    main = double 21\n";
        ImmutableDictionary<string, CodexType>? types = TypeCheck(source);
        Assert.NotNull(types);
        Assert.True(types!.ContainsKey("double"));
        Assert.Equal("Integer → Integer", types["double"].ToString());
    }

    // --- Generated C# structure ---

    [Fact]
    public void Emitted_csharp_has_class_prefix()
    {
        string source = "main : Integer\nmain = 42";
        string? cs = CompileToCS(source, "mymodule");
        Assert.NotNull(cs);
        Assert.Contains("Codex_mymodule", cs);
    }

    [Fact]
    public void Emitted_csharp_has_top_level_console_writeline()
    {
        string source = "main : Integer\nmain = 42";
        string? cs = CompileToCS(source);
        Assert.NotNull(cs);
        string beforeClass = cs!.Substring(0, cs.IndexOf("public static class"));
        Assert.Contains("Console.WriteLine", beforeClass);
    }

    [Fact]
    public void Sum_type_parses_and_type_checks()
    {
        string source =
            "Color =\n" +
            "  | Red\n" +
            "  | Green\n" +
            "  | Blue\n\n" +
            "name : Color -> Text\n" +
            "name (c) =\n" +
            "  when c\n" +
            "    if Red -> \"red\"\n" +
            "    if Green -> \"green\"\n" +
            "    if Blue -> \"blue\"\n";
        ImmutableDictionary<string, CodexType>? types = TypeCheck(source);
        Assert.NotNull(types);
        Assert.True(types!.ContainsKey("name"));
    }

    [Fact]
    public void Sum_type_with_fields_type_checks()
    {
        string source =
            "Shape =\n" +
            "  | Circle (radius : Number)\n" +
            "  | Rect (width : Number) (height : Number)\n\n" +
            "describe : Shape -> Text\n" +
            "describe (s) =\n" +
            "  when s\n" +
            "    if Circle (r) -> \"circle\"\n" +
            "    if Rect (w) (h) -> \"rect\"\n";
        ImmutableDictionary<string, CodexType>? types = TypeCheck(source);
        Assert.NotNull(types);
        Assert.True(types!.ContainsKey("describe"));
    }

    [Fact]
    public void Sum_type_compiles_to_csharp()
    {
        string source =
            "Color =\n" +
            "  | Red\n" +
            "  | Green\n" +
            "  | Blue\n\n" +
            "name : Color -> Text\n" +
            "name (c) =\n" +
            "  when c\n" +
            "    if Red -> \"red\"\n" +
            "    if Green -> \"green\"\n" +
            "    if Blue -> \"blue\"\n";
        string? cs = CompileToCS(source, "colors");
        Assert.NotNull(cs);
        Assert.Contains("name", cs);
        Assert.Contains("Color", cs);
    }

    [Fact]
    public void Record_type_parses_and_type_checks()
    {
        string source =
            "Point = record {\n" +
            "  x : Number,\n" +
            "  y : Number\n" +
            "}\n\n" +
            "origin : Point\n" +
            "origin = Point { x = 0.0, y = 0.0 }\n";
        ImmutableDictionary<string, CodexType>? types = TypeCheck(source);
        Assert.NotNull(types);
        Assert.True(types!.ContainsKey("origin"));
    }

    [Fact]
    public void Record_field_access_type_checks()
    {
        string source =
            "Point = record {\n" +
            "  x : Number,\n" +
            "  y : Number\n" +
            "}\n\n" +
            "get-x : Point -> Number\n" +
            "get-x (p) = p.x\n";
        ImmutableDictionary<string, CodexType>? types = TypeCheck(source);
        Assert.NotNull(types);
        Assert.True(types!.ContainsKey("get-x"));
        Assert.Contains("Number", types["get-x"].ToString());
    }

    [Fact]
    public void Constructor_as_function_type_checks()
    {
        string source =
            "Maybe =\n" +
            "  | Just (value : Integer)\n" +
            "  | None\n\n" +
            "wrap : Integer -> Maybe\n" +
            "wrap (x) = Just x\n";
        ImmutableDictionary<string, CodexType>? types = TypeCheck(source);
        Assert.NotNull(types);
        Assert.True(types!.ContainsKey("wrap"));
    }

    [Fact]
    public void Exhaustive_match_produces_no_warning()
    {
        string source =
            "Color =\n" +
            "  | Red\n" +
            "  | Green\n" +
            "  | Blue\n\n" +
            "name : Color -> Text\n" +
            "name (c) =\n" +
            "  when c\n" +
            "    if Red -> \"red\"\n" +
            "    if Green -> \"green\"\n" +
            "    if Blue -> \"blue\"\n";
        DiagnosticBag diag = TypeCheckWithDiagnostics(source);
        Assert.False(diag.HasErrors);
        Assert.DoesNotContain(diag.ToImmutable(), d => d.Code == "CDX2020");
    }

    [Fact]
    public void NonExhaustive_match_produces_warning()
    {
        string source =
            "Color =\n" +
            "  | Red\n" +
            "  | Green\n" +
            "  | Blue\n\n" +
            "name : Color -> Text\n" +
            "name (c) =\n" +
            "  when c\n" +
            "    if Red -> \"red\"\n" +
            "    if Green -> \"green\"\n";
        DiagnosticBag diag = TypeCheckWithDiagnostics(source);
        Assert.False(diag.HasErrors);
        Assert.Contains(diag.ToImmutable(), d => d.Code == "CDX2020" && d.Message.Contains("Blue"));
    }

    [Fact]
    public void Wildcard_match_is_exhaustive()
    {
        string source =
            "Color =\n" +
            "  | Red\n" +
            "  | Green\n" +
            "  | Blue\n\n" +
            "is-red : Color -> Boolean\n" +
            "is-red (c) =\n" +
            "  when c\n" +
            "    if Red -> True\n" +
            "    if _ -> False\n";
        DiagnosticBag diag = TypeCheckWithDiagnostics(source);
        Assert.False(diag.HasErrors);
        Assert.DoesNotContain(diag.ToImmutable(), d => d.Code == "CDX2020");
    }

    [Fact]
    public void Parametric_sum_type_type_checks()
    {
        string source =
            "Maybe (a) =\n" +
            "  | Just (a)\n" +
            "  | None\n\n" +
            "wrap : Integer -> Maybe Integer\n" +
            "wrap (x) = Just x\n";
        ImmutableDictionary<string, CodexType>? types = TypeCheck(source);
        Assert.NotNull(types);
        Assert.True(types!.ContainsKey("wrap"));
    }

    [Fact]
    public void Parametric_sum_type_pattern_match_type_checks()
    {
        string source =
            "Maybe (a) =\n" +
            "  | Just (a)\n" +
            "  | None\n\n" +
            "unwrap : Maybe Integer -> Integer\n" +
            "unwrap (m) =\n" +
            "  when m\n" +
            "    if Just (n) -> n\n" +
            "    if None -> 0\n";
        ImmutableDictionary<string, CodexType>? types = TypeCheck(source);
        Assert.NotNull(types);
        Assert.True(types!.ContainsKey("unwrap"));
    }

    [Fact]
    public void Parametric_sum_type_compiles_to_csharp()
    {
        string source =
            "Maybe (a) =\n" +
            "  | Just (a)\n" +
            "  | None\n\n" +
            "wrap : Integer -> Maybe Integer\n" +
            "wrap (x) = Just x\n\n" +
            "main : Integer\n" +
            "main =\n" +
            "  when wrap 42\n" +
            "    if Just (n) -> n\n" +
            "    if None -> 0\n";
        string? cs = CompileToCS(source, "maybe_test");
        Assert.NotNull(cs);
        Assert.Contains("Just", cs);
        Assert.Contains("Maybe", cs);
    }

    private static DiagnosticBag TypeCheckWithDiagnostics(string source, string moduleName = "test")
    {
        SourceText src = new SourceText("test.codex", source);
        DiagnosticBag diagnostics = new DiagnosticBag();

        Lexer lexer = new Lexer(src, diagnostics);
        IReadOnlyList<Token> tokens = lexer.TokenizeAll();
        Parser parser = new Parser(tokens, diagnostics);
        DocumentNode document = parser.ParseDocument();

        Desugarer desugarer = new Desugarer(diagnostics);
        Module module = desugarer.Desugar(document, moduleName);

        NameResolver resolver = new NameResolver(diagnostics);
        ResolvedModule resolved = resolver.Resolve(module);

        TypeChecker checker = new TypeChecker(diagnostics);
        checker.CheckModule(resolved.Module);
        return diagnostics;
    }

    private static DiagnosticBag CheckWithLinearity(string source, string moduleName = "test")
    {
        SourceText src = new SourceText("test.codex", source);
        DiagnosticBag diagnostics = new DiagnosticBag();

        Lexer lexer = new Lexer(src, diagnostics);
        IReadOnlyList<Token> tokens = lexer.TokenizeAll();
        Parser parser = new Parser(tokens, diagnostics);
        DocumentNode document = parser.ParseDocument();

        Desugarer desugarer = new Desugarer(diagnostics);
        Module module = desugarer.Desugar(document, moduleName);

        NameResolver resolver = new NameResolver(diagnostics);
        ResolvedModule resolved = resolver.Resolve(module);

        TypeChecker checker = new TypeChecker(diagnostics);
        ImmutableDictionary<string, CodexType> types = checker.CheckModule(resolved.Module);

        LinearityChecker linearityChecker = new LinearityChecker(diagnostics, types);
        linearityChecker.CheckModule(resolved.Module);

        return diagnostics;
    }

    // --- Effectful programs ---

    [Fact]
    public void Effectful_function_type_checks()
    {
        string source =
            "main : [Console] Nothing\n" +
            "main = do\n" +
            "  print-line \"hello\"\n";
        ImmutableDictionary<string, CodexType>? types = TypeCheck(source);
        Assert.NotNull(types);
        Assert.True(types!.ContainsKey("main"));
        Assert.Contains("Console", types["main"].ToString());
    }

    [Fact]
    public void Effectful_function_compiles_to_csharp()
    {
        string source =
            "main : [Console] Nothing\n" +
            "main = do\n" +
            "  print-line \"hello\"\n";
        string? cs = CompileToCS(source, "eftest");
        Assert.NotNull(cs);
        Assert.Contains("Console.WriteLine", cs!);
        Assert.DoesNotContain("Console.WriteLine(Codex_eftest.main())", cs);
    }

    [Fact]
    public void Do_bind_type_checks()
    {
        string source =
            "main : [Console] Nothing\n" +
            "main = do\n" +
            "  name <- read-line\n" +
            "  print-line name\n";
        ImmutableDictionary<string, CodexType>? types = TypeCheck(source);
        Assert.NotNull(types);
        Assert.True(types!.ContainsKey("main"));
    }

    [Fact]
    public void Do_bind_compiles_to_csharp()
    {
        string source =
            "main : [Console] Nothing\n" +
            "main = do\n" +
            "  name <- read-line\n" +
            "  print-line (\"Hello, \" ++ name)\n";
        string? cs = CompileToCS(source, "dobind");
        Assert.NotNull(cs);
        Assert.Contains("Console.ReadLine()", cs!);
        Assert.Contains("Console.WriteLine", cs);
    }

    [Fact]
    public void Effectful_helper_function_compiles()
    {
        string source =
            "greet : Text -> [Console] Nothing\n" +
            "greet (name) = print-line (\"Hello, \" ++ name)\n\n" +
            "main : [Console] Nothing\n" +
            "main = do\n" +
            "  greet \"World\"\n";
        string? cs = CompileToCS(source, "efhelper");
        Assert.NotNull(cs);
        Assert.Contains("greet", cs!);
        Assert.Contains("Console.WriteLine", cs);
    }

    [Fact]
    public void Pure_function_calling_effectful_produces_error()
    {
        string source =
            "bad : Nothing\n" +
            "bad = print-line \"oops\"\n";
        DiagnosticBag diag = TypeCheckWithDiagnostics(source);
        Assert.Contains(diag.ToImmutable(), d => d.Code == "CDX2031");
    }

    [Fact]
    public void Effectful_function_calling_effectful_is_allowed()
    {
        string source =
            "say-hello : [Console] Nothing\n" +
            "say-hello = print-line \"hello\"\n";
        DiagnosticBag diag = TypeCheckWithDiagnostics(source);
        Assert.DoesNotContain(diag.ToImmutable(), d => d.Code == "CDX2031");
    }

    [Fact]
    public void Multiple_do_statements_compile()
    {
        string source =
            "main : [Console] Nothing\n" +
            "main = do\n" +
            "  print-line \"one\"\n" +
            "  print-line \"two\"\n" +
            "  print-line \"three\"\n";
        string? cs = CompileToCS(source, "multi");
        Assert.NotNull(cs);
        int count = 0;
        int idx = 0;
        while ((idx = cs!.IndexOf("Console.WriteLine", idx)) >= 0)
        {
            count++;
            idx++;
        }
        Assert.Equal(3, count);
    }

    // --- M2 gap fixes ---

    [Fact]
    public void Nested_ctor_pattern_compiles()
    {
        string source =
            "Maybe (a) =\n" +
            "  | Just (a)\n" +
            "  | None\n\n" +
            "unwrap-nested : Maybe (Maybe Integer) -> Integer\n" +
            "unwrap-nested (m) =\n" +
            "  when m\n" +
            "    if Just (Just (n)) -> n\n" +
            "    if _ -> 0\n";
        string? cs = CompileToCS(source, "nested");
        Assert.NotNull(cs);
        Assert.Contains("Just", cs!);
    }

    [Fact]
    public void Type_param_arity_too_many_args_produces_error()
    {
        string source =
            "Maybe (a) =\n" +
            "  | Just (a)\n" +
            "  | None\n\n" +
            "bad : Maybe (Integer) (Text) -> Integer\n" +
            "bad (x) = 0\n";
        DiagnosticBag diag = TypeCheckWithDiagnostics(source);
        Assert.Contains(diag.ToImmutable(), d => d.Code == "CDX2032");
    }

    [Fact]
    public void Type_param_arity_correct_no_error()
    {
        string source =
            "Maybe (a) =\n" +
            "  | Just (a)\n" +
            "  | None\n\n" +
            "wrap : Integer -> Maybe Integer\n" +
            "wrap (x) = Just x\n";
        DiagnosticBag diag = TypeCheckWithDiagnostics(source);
        Assert.DoesNotContain(diag.ToImmutable(), d => d.Code == "CDX2032");
    }

    [Fact]
    public void Show_as_first_class_value_compiles()
    {
        string source =
            "apply-to : (Integer -> Text) -> Integer -> Text\n" +
            "apply-to (f) (x) = f x\n\n" +
            "main : Text\n" +
            "main = apply-to show 42\n";
        string? cs = CompileToCS(source, "showval");
        Assert.NotNull(cs);
        Assert.Contains("Convert.ToString", cs!);
    }

    // --- Linear types (Milestone 6) ---

    [Fact]
    public void Linear_type_parses()
    {
        string source =
            "consume : linear FileHandle -> [FileSystem] Nothing\n" +
            "consume (h) = close-file h\n";
        ImmutableDictionary<string, CodexType>? types = TypeCheck(source);
        Assert.NotNull(types);
        Assert.True(types!.ContainsKey("consume"));
        CodexType consumeType = types["consume"];
        Assert.IsType<FunctionType>(consumeType);
        FunctionType ft = (FunctionType)consumeType;
        Assert.IsType<LinearType>(ft.Parameter);
    }

    [Fact]
    public void Linear_variable_used_once_is_ok()
    {
        string source =
            "consume : linear FileHandle -> [FileSystem] Nothing\n" +
            "consume (h) = close-file h\n";
        DiagnosticBag diag = CheckWithLinearity(source);
        Assert.DoesNotContain(diag.ToImmutable(), d =>
            d.Code == "CDX2040" || d.Code == "CDX2041");
    }

    [Fact]
    public void Linear_variable_used_twice_produces_error()
    {
        string source =
            "use-twice : linear FileHandle -> [FileSystem] Nothing\n" +
            "use-twice (h) = do\n" +
            "  close-file h\n" +
            "  close-file h\n";
        DiagnosticBag diag = CheckWithLinearity(source);
        Assert.Contains(diag.ToImmutable(), d => d.Code == "CDX2041");
    }

    [Fact]
    public void Linear_variable_unused_produces_error()
    {
        string source =
            "leak : linear FileHandle -> Integer\n" +
            "leak (h) = 42\n";
        DiagnosticBag diag = CheckWithLinearity(source);
        Assert.Contains(diag.ToImmutable(), d => d.Code == "CDX2040");
    }

    [Fact]
    public void Linear_file_handle_round_trip_type_checks()
    {
        string source =
            "open-and-close : Text -> [FileSystem] Nothing\n" +
            "open-and-close (path) = do\n" +
            "  handle <- open-file path\n" +
            "  close-file handle\n";
        ImmutableDictionary<string, CodexType>? types = TypeCheck(source);
        Assert.NotNull(types);
    }

    [Fact]
    public void Linear_file_handle_compiles_to_csharp()
    {
        string source =
            "consume : linear FileHandle -> [FileSystem] Nothing\n" +
            "consume (h) = close-file h\n";
        string? cs = CompileToCS(source, "linfile");
        Assert.NotNull(cs);
        Assert.Contains(".Dispose()", cs!);
    }

    [Fact]
    public void Open_file_compiles_to_csharp()
    {
        string source =
            "open-and-close : Text -> [FileSystem] Nothing\n" +
            "open-and-close (path) = do\n" +
            "  h <- open-file path\n" +
            "  close-file h\n";
        string? cs = CompileToCS(source, "openclose");
        Assert.NotNull(cs);
        Assert.Contains("File.OpenRead", cs!);
        Assert.Contains(".Dispose()", cs!);
    }

    [Fact]
    public void Linear_type_in_format_type()
    {
        string source =
            "consume : linear FileHandle -> [FileSystem] Nothing\n" +
            "consume (h) = close-file h\n";
        DiagnosticBag diag = CheckWithLinearity(source);
        Assert.DoesNotContain(diag.ToImmutable(), d => d.Severity == DiagnosticSeverity.Error);
    }
}
