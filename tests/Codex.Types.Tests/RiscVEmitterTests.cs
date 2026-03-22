using System.Diagnostics;
using Xunit;

namespace Codex.Types.Tests;

public class RiscVEmitterTests
{
    [Fact]
    public void Simple_integer_emits_elf_bytes()
    {
        string source = """
            main : Integer
            main = 42
            """;
        byte[]? bytes = Helpers.CompileToRiscV(source, "simple_rv");
        Assert.NotNull(bytes);
        Assert.True(bytes.Length > 0);
        // Verify ELF magic
        Assert.Equal(0x7F, bytes[0]);
        Assert.Equal((byte)'E', bytes[1]);
        Assert.Equal((byte)'L', bytes[2]);
        Assert.Equal((byte)'F', bytes[3]);
        // ELF64
        Assert.Equal(2, bytes[4]);
        // Little-endian
        Assert.Equal(1, bytes[5]);
    }

    [Fact]
    public void Square_emits_elf_bytes()
    {
        string source = """
            square : Integer -> Integer
            square (x) = x * x

            main : Integer
            main = square 5
            """;
        byte[]? bytes = Helpers.CompileToRiscV(source, "square_rv");
        Assert.NotNull(bytes);
        Assert.True(bytes.Length > 0);
        AssertValidElf(bytes);
    }

    [Fact]
    public void Factorial_emits_elf_bytes()
    {
        string source = """
            factorial : Integer -> Integer
            factorial (n) = if n == 0 then 1 else n * factorial (n - 1)

            main : Integer
            main = factorial 5
            """;
        byte[]? bytes = Helpers.CompileToRiscV(source, "factorial_rv");
        Assert.NotNull(bytes);
        Assert.True(bytes.Length > 0);
        AssertValidElf(bytes);
    }

    [Fact]
    public void Arithmetic_emits_elf_bytes()
    {
        string source = """
            add : Integer -> Integer -> Integer
            add (x) (y) = x + y

            main : Integer
            main = add 3 4
            """;
        byte[]? bytes = Helpers.CompileToRiscV(source, "arith_rv");
        Assert.NotNull(bytes);
        Assert.True(bytes.Length > 0);
        AssertValidElf(bytes);
    }

    [Fact]
    public void Let_binding_emits_elf_bytes()
    {
        string source = """
            main : Integer
            main = let x = 10 in let y = 20 in x + y
            """;
        byte[]? bytes = Helpers.CompileToRiscV(source, "let_rv");
        Assert.NotNull(bytes);
        Assert.True(bytes.Length > 0);
        AssertValidElf(bytes);
    }

    [Fact]
    public void If_else_emits_elf_bytes()
    {
        string source = """
            main : Integer
            main = if 1 == 1 then 42 else 0
            """;
        byte[]? bytes = Helpers.CompileToRiscV(source, "ifelse_rv");
        Assert.NotNull(bytes);
        Assert.True(bytes.Length > 0);
        AssertValidElf(bytes);
    }

    [Fact]
    public void Elf_has_riscv_machine_type()
    {
        string source = """
            main : Integer
            main = 1
            """;
        byte[]? bytes = Helpers.CompileToRiscV(source, "machine_rv");
        Assert.NotNull(bytes);
        // e_machine at offset 18 (2 bytes, little-endian)
        ushort machine = (ushort)(bytes[18] | (bytes[19] << 8));
        Assert.Equal(243, machine); // EM_RISCV
    }

    [Fact]
    public void Elf_has_valid_entry_point()
    {
        string source = """
            main : Integer
            main = 1
            """;
        byte[]? bytes = Helpers.CompileToRiscV(source, "entry_rv");
        Assert.NotNull(bytes);
        // e_entry at offset 24 (8 bytes, little-endian)
        ulong entry = BitConverter.ToUInt64(bytes, 24);
        Assert.True(entry >= 0x10000, "Entry point should be at or above base address 0x10000");
    }

    static void AssertValidElf(byte[] bytes)
    {
        Assert.True(bytes.Length >= 64, "ELF must be at least 64 bytes (header size)");
        Assert.Equal(0x7F, bytes[0]);
        Assert.Equal((byte)'E', bytes[1]);
        Assert.Equal((byte)'L', bytes[2]);
        Assert.Equal((byte)'F', bytes[3]);
    }

    // ── Integration tests (require qemu-riscv64 via WSL) ────────

    [Fact]
    public void Integer_42_runs_under_qemu()
    {
        string source = """
            main : Integer
            main = 42
            """;
        string? output = CompileAndRun(source, "int42_run_rv");
        if (output is null) return;
        Assert.Equal("42", output.Trim());
    }

    [Fact]
    public void Arithmetic_add_runs_under_qemu()
    {
        string source = """
            add : Integer -> Integer -> Integer
            add (x) (y) = x + y

            main : Integer
            main = add 3 4
            """;
        string? output = CompileAndRun(source, "add_run_rv");
        if (output is null) return;
        Assert.Equal("7", output.Trim());
    }

    [Fact]
    public void Square_runs_under_qemu()
    {
        string source = """
            square : Integer -> Integer
            square (x) = x * x

            main : Integer
            main = square 5
            """;
        string? output = CompileAndRun(source, "square_run_rv");
        if (output is null) return;
        Assert.Equal("25", output.Trim());
    }

    [Fact]
    public void Let_binding_runs_under_qemu()
    {
        string source = """
            main : Integer
            main = let x = 10 in let y = 20 in x + y
            """;
        string? output = CompileAndRun(source, "let_run_rv");
        if (output is null) return;
        Assert.Equal("30", output.Trim());
    }

    [Fact]
    public void Factorial_runs_under_qemu()
    {
        string source = """
            factorial : Integer -> Integer
            factorial (n) = if n == 0 then 1 else n * factorial (n - 1)

            main : Integer
            main = factorial 5
            """;
        string? output = CompileAndRun(source, "fact_run_rv");
        if (output is null) return;
        Assert.Equal("120", output.Trim());
    }

    // ── Helpers ──────────────────────────────────────────────────

    static string? CompileAndRun(string source, string moduleName)
    {
        byte[]? bytes = Helpers.CompileToRiscV(source, moduleName);
        if (bytes is null) return null;

        if (!IsQemuAvailable()) return null;

        string tempDir = Path.Combine(Path.GetTempPath(),
            "codex_rv_test_" + moduleName + "_" + Guid.NewGuid().ToString("N")[..8]);
        Directory.CreateDirectory(tempDir);
        try
        {
            string elfPath = Path.Combine(tempDir, moduleName);
            File.WriteAllBytes(elfPath, bytes);

            // Copy into WSL filesystem (NTFS doesn't support chmod),
            // make executable, then run under qemu-riscv64
            string wslPath = ToWslPath(elfPath);
            string wslTmp = $"/tmp/codex_rv_{moduleName}_{Guid.NewGuid().ToString("N")[..8]}";

            ProcessStartInfo psi = new("bash",
                $"-c \"cp '{wslPath}' '{wslTmp}' && chmod +x '{wslTmp}' && qemu-riscv64 '{wslTmp}'; EXIT=$?; rm -f '{wslTmp}'; exit $EXIT\"")
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
                    $"qemu-riscv64 exited with code {proc.ExitCode}.\nstdout: {stdout}\nstderr: {stderr}");

            return stdout;
        }
        finally
        {
            try { Directory.Delete(tempDir, true); } catch { }
        }
    }

    static bool IsQemuAvailable()
    {
        try
        {
            ProcessStartInfo psi = new("bash", "-c \"qemu-riscv64 --version\"")
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

    static string ToWslPath(string windowsPath)
    {
        string full = Path.GetFullPath(windowsPath);
        // D:\foo\bar → /mnt/d/foo/bar
        if (full.Length >= 2 && full[1] == ':')
        {
            char drive = char.ToLowerInvariant(full[0]);
            string rest = full[2..].Replace('\\', '/');
            return $"/mnt/{drive}{rest}";
        }
        return full.Replace('\\', '/');
    }
}
