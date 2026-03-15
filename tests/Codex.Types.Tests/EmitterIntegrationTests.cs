using Xunit;

namespace Codex.Types.Tests;

public class EmitterIntegrationTests
{
    static string ReadSample(string name)
    {
        string path = Path.Combine(FindSamplesDir(), name);
        return File.ReadAllText(path);
    }

    static string FindSamplesDir()
    {
        string dir = AppContext.BaseDirectory;
        while (dir is not null)
        {
            string candidate = Path.Combine(dir, "samples");
            if (Directory.Exists(candidate))
                return candidate;
            dir = Path.GetDirectoryName(dir)!;
        }
        throw new DirectoryNotFoundException("Cannot find samples/ directory");
    }

    // ── hello.codex ────────────────────────────────────────────

    [Fact]
    public void Hello_emits_csharp()
    {
        string source = ReadSample("hello.codex");
        string? cs = Helpers.CompileToCS(source, "hello");
        Assert.NotNull(cs);
        Assert.Contains("square", cs);
    }

    [Fact]
    public void Hello_emits_javascript()
    {
        string source = ReadSample("hello.codex");
        string? js = Helpers.CompileToJS(source, "hello");
        Assert.NotNull(js);
        Assert.Contains("function square", js);
        Assert.Contains("5n", js);
    }

    [Fact]
    public void Hello_emits_rust()
    {
        string source = ReadSample("hello.codex");
        string? rs = Helpers.CompileToRust(source, "hello");
        Assert.NotNull(rs);
        Assert.Contains("fn square", rs);
        Assert.Contains("i64", rs);
    }

    // ── factorial.codex ────────────────────────────────────────

    [Fact]
    public void Factorial_emits_csharp()
    {
        string source = ReadSample("factorial.codex");
        string? cs = Helpers.CompileToCS(source, "factorial");
        Assert.NotNull(cs);
        Assert.Contains("factorial", cs);
    }

    [Fact]
    public void Factorial_emits_javascript()
    {
        string source = ReadSample("factorial.codex");
        string? js = Helpers.CompileToJS(source, "factorial");
        Assert.NotNull(js);
        Assert.Contains("function factorial", js);
    }

    [Fact]
    public void Factorial_emits_rust()
    {
        string source = ReadSample("factorial.codex");
        string? rs = Helpers.CompileToRust(source, "factorial");
        Assert.NotNull(rs);
        Assert.Contains("fn factorial", rs);
    }

    // ── fibonacci.codex ────────────────────────────────────────

    [Fact]
    public void Fibonacci_emits_csharp()
    {
        string source = ReadSample("fibonacci.codex");
        string? cs = Helpers.CompileToCS(source, "fibonacci");
        Assert.NotNull(cs);
        Assert.Contains("fib", cs);
    }

    [Fact]
    public void Fibonacci_emits_javascript()
    {
        string source = ReadSample("fibonacci.codex");
        string? js = Helpers.CompileToJS(source, "fibonacci");
        Assert.NotNull(js);
        Assert.Contains("function fib", js);
    }

    [Fact]
    public void Fibonacci_emits_rust()
    {
        string source = ReadSample("fibonacci.codex");
        string? rs = Helpers.CompileToRust(source, "fibonacci");
        Assert.NotNull(rs);
        Assert.Contains("fn fib", rs);
    }

    // ── greeting.codex ─────────────────────────────────────────

    [Fact]
    public void Greeting_emits_csharp()
    {
        string source = ReadSample("greeting.codex");
        string? cs = Helpers.CompileToCS(source, "greeting");
        Assert.NotNull(cs);
        Assert.Contains("greeting", cs);
    }

    [Fact]
    public void Greeting_emits_javascript()
    {
        string source = ReadSample("greeting.codex");
        string? js = Helpers.CompileToJS(source, "greeting");
        Assert.NotNull(js);
        Assert.Contains("function greeting", js);
        Assert.Contains("\"Hello, \"", js);
    }

    [Fact]
    public void Greeting_emits_rust()
    {
        string source = ReadSample("greeting.codex");
        string? rs = Helpers.CompileToRust(source, "greeting");
        Assert.NotNull(rs);
        Assert.Contains("fn greeting", rs);
        Assert.Contains("format!", rs);
    }

    // ── shapes.codex ───────────────────────────────────────────

    [Fact]
    public void Shapes_emits_csharp()
    {
        string source = ReadSample("shapes.codex");
        string? cs = Helpers.CompileToCS(source, "shapes");
        Assert.NotNull(cs);
        Assert.Contains("Circle", cs);
        Assert.Contains("Rectangle", cs);
    }

    [Fact]
    public void Shapes_emits_javascript()
    {
        string source = ReadSample("shapes.codex");
        string? js = Helpers.CompileToJS(source, "shapes");
        Assert.NotNull(js);
        Assert.Contains("function Circle", js);
        Assert.Contains("function Rectangle", js);
        Assert.Contains("tag", js);
    }

    [Fact]
    public void Shapes_emits_rust()
    {
        string source = ReadSample("shapes.codex");
        string? rs = Helpers.CompileToRust(source, "shapes");
        Assert.NotNull(rs);
        Assert.Contains("enum Shape", rs);
        Assert.Contains("Circle", rs);
        Assert.Contains("Rectangle", rs);
    }

    // ── person.codex ───────────────────────────────────────────

    [Fact]
    public void Person_emits_csharp()
    {
        string source = ReadSample("person.codex");
        string? cs = Helpers.CompileToCS(source, "person");
        Assert.NotNull(cs);
        Assert.Contains("Person", cs);
        Assert.Contains("name", cs);
    }

    [Fact]
    public void Person_emits_javascript()
    {
        string source = ReadSample("person.codex");
        string? js = Helpers.CompileToJS(source, "person");
        Assert.NotNull(js);
        Assert.Contains("function Person", js);
        Assert.Contains("name", js);
    }

    [Fact]
    public void Person_emits_rust()
    {
        string source = ReadSample("person.codex");
        string? rs = Helpers.CompileToRust(source, "person");
        Assert.NotNull(rs);
        Assert.Contains("struct Person", rs);
        Assert.Contains("name: String", rs);
    }

    // ── safe-divide.codex ──────────────────────────────────────

    [Fact]
    public void SafeDivide_emits_csharp()
    {
        string source = ReadSample("safe-divide.codex");
        string? cs = Helpers.CompileToCS(source, "safe_divide");
        Assert.NotNull(cs);
        Assert.Contains("safe_divide", cs);
        Assert.Contains("Success", cs);
        Assert.Contains("Failure", cs);
    }

    [Fact]
    public void SafeDivide_emits_javascript()
    {
        string source = ReadSample("safe-divide.codex");
        string? js = Helpers.CompileToJS(source, "safe_divide");
        Assert.NotNull(js);
        Assert.Contains("Success", js);
        Assert.Contains("Failure", js);
    }

    [Fact]
    public void SafeDivide_emits_rust()
    {
        string source = ReadSample("safe-divide.codex");
        string? rs = Helpers.CompileToRust(source, "safe_divide");
        Assert.NotNull(rs);
        Assert.Contains("enum Result", rs);
        Assert.Contains("Success", rs);
        Assert.Contains("Failure", rs);
    }

    // ── string-ops.codex ───────────────────────────────────────

    [Fact]
    public void StringOps_emits_csharp()
    {
        string source = ReadSample("string-ops.codex");
        string? cs = Helpers.CompileToCS(source, "string_ops");
        Assert.NotNull(cs);
        Assert.Contains("count_letters", cs);
    }

    [Fact]
    public void StringOps_emits_javascript()
    {
        string source = ReadSample("string-ops.codex");
        string? js = Helpers.CompileToJS(source, "string_ops");
        Assert.NotNull(js);
        Assert.Contains("function count_letters", js);
    }

    [Fact]
    public void StringOps_emits_rust()
    {
        string source = ReadSample("string-ops.codex");
        string? rs = Helpers.CompileToRust(source, "string_ops");
        Assert.NotNull(rs);
        Assert.Contains("fn count_letters", rs);
    }

    // ── prose-greeting.codex ───────────────────────────────────

    [Fact]
    public void ProseGreeting_emits_csharp()
    {
        string source = ReadSample("prose-greeting.codex");
        string? cs = Helpers.CompileToCS(source, "prose_greeting");
        Assert.NotNull(cs);
        Assert.Contains("greet", cs);
    }

    [Fact]
    public void ProseGreeting_emits_javascript()
    {
        string source = ReadSample("prose-greeting.codex");
        string? js = Helpers.CompileToJS(source, "prose_greeting");
        Assert.NotNull(js);
        Assert.Contains("function greet", js);
    }

    [Fact]
    public void ProseGreeting_emits_rust()
    {
        string source = ReadSample("prose-greeting.codex");
        string? rs = Helpers.CompileToRust(source, "prose_greeting");
        Assert.NotNull(rs);
        Assert.Contains("fn greet", rs);
    }

    // ── effectful-hello.codex ──────────────────────────────────

    [Fact]
    public void EffectfulHello_emits_csharp()
    {
        string source = ReadSample("effectful-hello.codex");
        string? cs = Helpers.CompileToCS(source, "effectful_hello");
        Assert.NotNull(cs);
        Assert.Contains("Console.WriteLine", cs);
        Assert.Contains("Console.ReadLine", cs);
    }

    [Fact]
    public void EffectfulHello_emits_javascript()
    {
        string source = ReadSample("effectful-hello.codex");
        string? js = Helpers.CompileToJS(source, "effectful_hello");
        Assert.NotNull(js);
        Assert.Contains("console.log", js);
    }

    [Fact]
    public void EffectfulHello_emits_rust()
    {
        string source = ReadSample("effectful-hello.codex");
        string? rs = Helpers.CompileToRust(source, "effectful_hello");
        Assert.NotNull(rs);
        Assert.Contains("println!", rs);
    }
}
