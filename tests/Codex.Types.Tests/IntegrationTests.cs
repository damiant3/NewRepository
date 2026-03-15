using Codex.Core;
using Codex.Syntax;
using Codex.Ast;
using Codex.Semantics;
using Codex.IR;
using Codex.Emit.CSharp;
using Xunit;

namespace Codex.Types.Tests;

public class IntegrationTests
{
    private static string? CompileToCS(string source, string moduleName = "test")
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

        CSharpEmitter emitter = new();
        return emitter.Emit(irModule);
    }

    private static Map<string, CodexType>? TypeCheck(
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
        Map<string, CodexType>? types = TypeCheck(source);
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
        Map<string, CodexType>? types = TypeCheck(source);
        Assert.NotNull(types);
        Assert.True(types!.ContainsKey("double"));
        Assert.Equal("Integer → Integer", types["double"]!.ToString());
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
        Map<string, CodexType>? types = TypeCheck(source);
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
        Map<string, CodexType>? types = TypeCheck(source);
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
        Map<string, CodexType>? types = TypeCheck(source);
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
        Map<string, CodexType>? types = TypeCheck(source);
        Assert.NotNull(types);
        Assert.True(types!.ContainsKey("get-x"));
        Assert.Contains("Number", types["get-x"]!.ToString());
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
        Map<string, CodexType>? types = TypeCheck(source);
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
        Map<string, CodexType>? types = TypeCheck(source);
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
        Map<string, CodexType>? types = TypeCheck(source);
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

    private static DiagnosticBag CheckWithLinearity(string source, string moduleName = "test")
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

    // --- Effectful programs ---

    [Fact]
    public void Effectful_function_type_checks()
    {
        string source =
            "main : [Console] Nothing\n" +
            "main = do\n" +
            "  print-line \"hello\"\n";
        Map<string, CodexType>? types = TypeCheck(source);
        Assert.NotNull(types);
        Assert.True(types!.ContainsKey("main"));
        Assert.Contains("Console", types["main"]!.ToString());
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
        Map<string, CodexType>? types = TypeCheck(source);
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
        Map<string, CodexType>? types = TypeCheck(source);
        Assert.NotNull(types);
        Assert.True(types!.ContainsKey("consume"));
        CodexType consumeType = types["consume"]!;
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
        Map<string, CodexType>? types = TypeCheck(source);
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

    // --- Dependent types (Milestone 8) ---

    [Fact]
    public void Dependent_function_type_checks()
    {
        string source =
            "identity : (n : Integer) -> Integer\n" +
            "identity (x) = x\n";
        Map<string, CodexType>? types = TypeCheck(source);
        Assert.NotNull(types);
        Assert.IsType<DependentFunctionType>(types!["identity"]);
    }

    [Fact]
    public void Dependent_type_application_substitutes_value()
    {
        string source =
            "f : (n : Integer) -> Integer\n" +
            "f (x) = x\n\n" +
            "main : Integer\n" +
            "main = f 42\n";
        Map<string, CodexType>? types = TypeCheck(source);
        Assert.NotNull(types);
        Assert.IsType<IntegerType>(types!["main"]);
    }

    [Fact]
    public void Dependent_type_with_vector_type_checks()
    {
        string source =
            "length : (n : Integer) -> Vector n Integer -> Integer\n" +
            "length (n) (v) = n\n";
        Map<string, CodexType>? types = TypeCheck(source);
        Assert.NotNull(types);
        Assert.IsType<DependentFunctionType>(types!["length"]);
    }

    [Fact]
    public void Nested_dependent_types_type_check()
    {
        string source =
            "add-lengths : (m : Integer) -> (n : Integer) -> Integer\n" +
            "add-lengths (a) (b) = a + b\n";
        Map<string, CodexType>? types = TypeCheck(source);
        Assert.NotNull(types);
        DependentFunctionType outer = Assert.IsType<DependentFunctionType>(types!["add-lengths"]);
        Assert.IsType<DependentFunctionType>(outer.Body);
    }

    [Fact]
    public void Dependent_type_compiles_to_csharp()
    {
        string source =
            "f : (n : Integer) -> Integer\n" +
            "f (x) = x\n\n" +
            "main : Integer\n" +
            "main = f 5\n";
        string? cs = CompileToCS(source, "deptest");
        Assert.NotNull(cs);
        Assert.Contains("f", cs!);
    }

    [Fact]
    public void Type_level_arithmetic_verified_at_compile_time()
    {
        string source =
            "f : (m : Integer) -> (n : Integer) -> Vector (m + n) Integer -> Integer\n" +
            "f (a) (b) (v) = a + b\n\n" +
            "main : Integer\n" +
            "main = f 3 2 [1, 2, 3, 4, 5]\n";
        Map<string, CodexType>? types = TypeCheck(source);
        Assert.NotNull(types);
    }

    [Fact]
    public void Proof_obligation_discharged_for_literal_evidence()
    {
        string source =
            "safe-index : (i : Integer) -> (n : Integer) -> {proof : i < n} -> Integer\n" +
            "safe-index (i) (n) = i\n\n" +
            "main : Integer\n" +
            "main = safe-index 3 5\n";
        DiagnosticBag diag = TypeCheckWithDiagnostics(source);
        Assert.DoesNotContain(diag.ToImmutable(), d => d.Severity == DiagnosticSeverity.Error);
    }

    [Fact]
    public void Proof_obligation_fails_for_invalid_literal_evidence()
    {
        string source =
            "safe-index : (i : Integer) -> (n : Integer) -> {proof : i < n} -> Integer\n" +
            "safe-index (i) (n) = i\n\n" +
            "main : Integer\n" +
            "main = safe-index 5 3\n";
        DiagnosticBag diag = TypeCheckWithDiagnostics(source);
        Assert.Contains(diag.ToImmutable(), d => d.Code == "CDX2040");
    }

    // --- Proofs (Milestone 10) ---

    private static DiagnosticBag CheckWithProofs(string source, string moduleName = "test")
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

    [Fact]
    public void Claim_and_refl_proof_succeeds()
    {
        string source =
            "claim zero-is-zero : 0 === 0\n" +
            "proof zero-is-zero = Refl\n";
        DiagnosticBag diag = CheckWithProofs(source);
        Assert.DoesNotContain(diag.ToImmutable(), d => d.Severity == DiagnosticSeverity.Error);
    }

    [Fact]
    public void Claim_and_refl_proof_with_types_succeeds()
    {
        string source =
            "claim five-is-five : 5 === 5\n" +
            "proof five-is-five = Refl\n";
        DiagnosticBag diag = CheckWithProofs(source);
        Assert.DoesNotContain(diag.ToImmutable(), d => d.Severity == DiagnosticSeverity.Error);
    }

    [Fact]
    public void Refl_proof_with_unequal_sides_fails()
    {
        string source =
            "claim bad : 3 === 5\n" +
            "proof bad = Refl\n";
        DiagnosticBag diag = CheckWithProofs(source);
        Assert.Contains(diag.ToImmutable(), d => d.Code == "CDX4010");
    }

    [Fact]
    public void Sym_proof_succeeds()
    {
        string source =
            "claim a-eq : 5 === 5\n" +
            "proof a-eq = Refl\n\n" +
            "claim b-eq : 5 === 5\n" +
            "proof b-eq = sym a-eq\n";
        DiagnosticBag diag = CheckWithProofs(source);
        Assert.DoesNotContain(diag.ToImmutable(), d => d.Severity == DiagnosticSeverity.Error);
    }

    [Fact]
    public void Proof_without_claim_fails()
    {
        string source =
            "proof orphan = Refl\n";
        DiagnosticBag diag = CheckWithProofs(source);
        Assert.Contains(diag.ToImmutable(), d => d.Code == "CDX4001");
    }

    [Fact]
    public void Type_level_arithmetic_in_claim_normalizes()
    {
        string source =
            "claim add-comm : (3 + 2) === 5\n" +
            "proof add-comm = Refl\n";
        DiagnosticBag diag = CheckWithProofs(source);
        Assert.DoesNotContain(diag.ToImmutable(), d => d.Severity == DiagnosticSeverity.Error);
    }

    [Fact]
    public void Type_level_arithmetic_wrong_value_fails()
    {
        string source =
            "claim bad-add : (3 + 2) === 6\n" +
            "proof bad-add = Refl\n";
        DiagnosticBag diag = CheckWithProofs(source);
        Assert.Contains(diag.ToImmutable(), d => d.Code == "CDX4010");
    }

    [Fact]
    public void Cong_proof_succeeds()
    {
        string source =
            "claim inner-eq : 5 === 5\n" +
            "proof inner-eq = Refl\n\n" +
            "claim outer-eq : List 5 === List 5\n" +
            "proof outer-eq = cong List inner-eq\n";
        DiagnosticBag diag = CheckWithProofs(source);
        Assert.DoesNotContain(diag.ToImmutable(), d => d.Severity == DiagnosticSeverity.Error);
    }
}
