using System.Diagnostics;
using Xunit;

namespace Codex.Types.Tests;

public class WasmEmitterTests
{
    [Fact]
    public void Hello_emits_wasm_bytes()
    {
        string source = """
            square : Integer -> Integer
            square (x) = x * x

            main : Integer
            main = square 5
            """;
        byte[]? bytes = Helpers.CompileToWasm(source, "hello_wasm");
        Assert.NotNull(bytes);
        Assert.True(bytes.Length > 0);
        // Verify WASM magic bytes
        Assert.Equal(0x00, bytes[0]);
        Assert.Equal(0x61, bytes[1]);
        Assert.Equal(0x73, bytes[2]);
        Assert.Equal(0x6D, bytes[3]);
        // Verify WASM version 1
        Assert.Equal(0x01, bytes[4]);
        Assert.Equal(0x00, bytes[5]);
        Assert.Equal(0x00, bytes[6]);
        Assert.Equal(0x00, bytes[7]);
    }

    [Fact]
    public void Factorial_emits_wasm_bytes()
    {
        string source = """
            factorial : Integer -> Integer
            factorial (n) = if n == 0 then 1 else n * factorial (n - 1)

            main : Integer
            main = factorial 5
            """;
        byte[]? bytes = Helpers.CompileToWasm(source, "factorial_wasm");
        Assert.NotNull(bytes);
        Assert.True(bytes.Length > 0);
    }

    [Fact]
    public void Text_literal_emits_wasm_bytes()
    {
        string source = """
            greeting : Text -> Text
            greeting (name) = "Hello, " ++ name ++ "!"

            main : Text
            main = greeting "World"
            """;
        byte[]? bytes = Helpers.CompileToWasm(source, "greeting_wasm");
        Assert.NotNull(bytes);
        Assert.True(bytes.Length > 0);
    }

    [Fact]
    public void Arithmetic_emits_wasm_bytes()
    {
        string source = """
            add : Integer -> Integer -> Integer
            add (x) (y) = x + y

            main : Integer
            main = add 3 4
            """;
        byte[]? bytes = Helpers.CompileToWasm(source, "arithmetic_wasm");
        Assert.NotNull(bytes);
        Assert.True(bytes.Length > 0);
    }

    [Fact]
    public void Boolean_emits_wasm_bytes()
    {
        string source = """
            is-even : Integer -> Integer
            is-even (n) = if n == 0 then 1 else 0

            main : Integer
            main = is-even 4
            """;
        byte[]? bytes = Helpers.CompileToWasm(source, "bool_wasm");
        Assert.NotNull(bytes);
        Assert.True(bytes.Length > 0);
    }

    [Fact]
    public void Hello_runs_under_wasmtime()
    {
        string source = """
            square : Integer -> Integer
            square (x) = x * x

            main : Integer
            main = square 5
            """;
        string? output = CompileAndRun(source, "hello_run_wasm");
        if (output is null) return; // wasmtime not available, skip
        Assert.Equal("25", output.Trim());
    }

    [Fact]
    public void Factorial_runs_under_wasmtime()
    {
        string source = """
            factorial : Integer -> Integer
            factorial (n) = if n == 0 then 1 else n * factorial (n - 1)

            main : Integer
            main = factorial 5
            """;
        string? output = CompileAndRun(source, "factorial_run_wasm");
        if (output is null) return;
        Assert.Equal("120", output.Trim());
    }

    [Fact]
    public void Text_greeting_runs_under_wasmtime()
    {
        string source = """
            greeting : Text -> Text
            greeting (name) = "Hello, " ++ name ++ "!"

            main : Text
            main = greeting "World"
            """;
        string? output = CompileAndRun(source, "greeting_run_wasm");
        if (output is null) return;
        Assert.Equal("Hello, World!", output.Trim());
    }

    [Fact]
    public void Let_binding_runs_under_wasmtime()
    {
        string source = """
            main : Integer
            main = let x = 10 in let y = 20 in x + y
            """;
        string? output = CompileAndRun(source, "let_run_wasm");
        if (output is null) return;
        Assert.Equal("30", output.Trim());
    }

    [Fact]
    public void Show_integer_runs_under_wasmtime()
    {
        string source = """
            main : Text
            main = show 42
            """;
        string? output = CompileAndRun(source, "show_int_run_wasm");
        if (output is null) return;
        Assert.Equal("42", output.Trim());
    }

    // ── Helpers ────────────────────────────────────────────────

    static string? CompileAndRun(string source, string moduleName)
    {
        byte[]? bytes = Helpers.CompileToWasm(source, moduleName);
        if (bytes is null) return null;

        // Check if wasmtime is available
        if (!IsWasmtimeAvailable()) return null;

        string tempDir = Path.Combine(Path.GetTempPath(),
            "codex_wasm_test_" + moduleName + "_" + Guid.NewGuid().ToString("N")[..8]);
        Directory.CreateDirectory(tempDir);
        try
        {
            string wasmPath = Path.Combine(tempDir, moduleName + ".wasm");
            File.WriteAllBytes(wasmPath, bytes);

            ProcessStartInfo psi = new("wasmtime", wasmPath)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = tempDir
            };

            using Process? proc = Process.Start(psi);
            if (proc is null) return null;

            string stdout = proc.StandardOutput.ReadToEnd();
            string stderr = proc.StandardError.ReadToEnd();
            proc.WaitForExit(10_000);

            if (proc.ExitCode != 0)
                throw new InvalidOperationException(
                    $"wasmtime exited with code {proc.ExitCode}.\nstdout: {stdout}\nstderr: {stderr}");

            return stdout;
        }
        finally
        {
            try { Directory.Delete(tempDir, true); } catch { }
        }
    }

    static bool IsWasmtimeAvailable()
    {
        try
        {
            ProcessStartInfo psi = new("wasmtime", "--version")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using Process? proc = Process.Start(psi);
            if (proc is null) return false;
            proc.WaitForExit(5_000);
            return proc.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }
}
