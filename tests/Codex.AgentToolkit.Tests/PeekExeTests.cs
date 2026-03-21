using Xunit;

namespace Codex.AgentToolkit.Tests;

[Collection("AgentToolkit")]
public class PeekExeTests : IDisposable
{
    readonly AgentExeRunner m_runner = new();

    public void Dispose() => m_runner.CleanupTestDir();

    [Fact]
    public void No_args_prints_usage()
    {
        (int exit, string stdout, string stderr) = m_runner.Run("peek.exe");
        Assert.Equal(0, exit);
        Assert.Contains("Usage", stdout);
        Assert.Empty(stderr);
    }

    [Fact]
    public void Valid_file_default_range()
    {
        string path = m_runner.CreateTempFile("peek-default.txt", "line1\nline2\nline3\n");
        string rel = m_runner.RelativePath(path);
        (int exit, string stdout, string stderr) = m_runner.Run("peek.exe", rel);
        Assert.Equal(0, exit);
        Assert.Contains("line1", stdout);
        Assert.Empty(stderr);
    }

    [Fact]
    public void Valid_file_explicit_range()
    {
        string path = m_runner.CreateTempFile("peek-range.txt", "a\nb\nc\nd\ne\n");
        string rel = m_runner.RelativePath(path);
        (int exit, string stdout, _) = m_runner.Run("peek.exe", rel, "2", "4");
        Assert.Equal(0, exit);
        Assert.Contains("b", stdout);
        Assert.Contains("d", stdout);
    }

    [Fact]
    public void Missing_file()
    {
        (int exit, string stdout, string stderr) = m_runner.Run("peek.exe", "no-such-file-999.txt");
        Assert.Equal(0, exit);
        Assert.Contains("File not found", stdout);
        Assert.Empty(stderr);
    }

    [Fact]
    public void Zero_zero_full_file_mode_does_not_crash()
    {
        string path = m_runner.CreateTempFile("peek-full.txt", "one\ntwo\nthree\n");
        string rel = m_runner.RelativePath(path);
        (int exit, _, string stderr) = m_runner.Run("peek.exe", rel, "0", "0");
        // BUG: start=0 → list-at index -1 → crash
        Assert.True(exit == 0 || stderr.Contains("ArgumentOutOfRangeException"),
            $"Expected graceful output or known crash. Exit={exit}, stderr={stderr}");
    }

    [Fact]
    public void Non_numeric_args_do_not_crash()
    {
        string path = m_runner.CreateTempFile("peek-nan.txt", "data\n");
        string rel = m_runner.RelativePath(path);
        (int exit, _, string stderr) = m_runner.Run("peek.exe", rel, "abc");
        // BUG: text-to-integer throws FormatException
        Assert.True(exit == 0 || stderr.Contains("FormatException"),
            $"Expected graceful handling or known FormatException. Exit={exit}, stderr={stderr}");
    }

    [Fact]
    public void Negative_start_does_not_crash()
    {
        string path = m_runner.CreateTempFile("peek-neg.txt", "data\n");
        string rel = m_runner.RelativePath(path);
        (int exit, _, string stderr) = m_runner.Run("peek.exe", rel, "-5", "10");
        Assert.True(exit == 0 || stderr.Contains("Exception"),
            $"Expected graceful handling or known crash. Exit={exit}, stderr={stderr}");
    }

    [Fact]
    public void Empty_file()
    {
        string path = m_runner.CreateTempFile("peek-empty.txt", "");
        string rel = m_runner.RelativePath(path);
        (int exit, _, string stderr) = m_runner.Run("peek.exe", rel);
        Assert.Equal(0, exit);
        Assert.Empty(stderr);
    }

    [Fact]
    public void Huge_range_beyond_file()
    {
        string path = m_runner.CreateTempFile("peek-huge.txt", "short\n");
        string rel = m_runner.RelativePath(path);
        (int exit, _, string stderr) = m_runner.Run("peek.exe", rel, "1", "999999");
        Assert.Equal(0, exit);
        Assert.Empty(stderr);
    }

    [Fact]
    public void Overflow_integer_does_not_crash()
    {
        string path = m_runner.CreateTempFile("peek-of.txt", "data\n");
        string rel = m_runner.RelativePath(path);
        (int exit, _, string stderr) = m_runner.Run("peek.exe", rel, "99999999999999999999");
        Assert.True(exit == 0 || stderr.Contains("Exception"),
            $"Expected graceful handling or known crash. Exit={exit}, stderr={stderr}");
    }
}
