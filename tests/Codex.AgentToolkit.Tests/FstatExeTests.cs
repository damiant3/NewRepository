using Xunit;

namespace Codex.AgentToolkit.Tests;

[Collection("AgentToolkit")]
public class FstatExeTests : IDisposable
{
    readonly AgentExeRunner m_runner = new();

    public void Dispose() => m_runner.CleanupTestDir();

    [Fact]
    public void No_args_prints_usage()
    {
        (int exit, string stdout, string stderr) = m_runner.Run("fstat.exe");
        Assert.Equal(0, exit);
        Assert.Contains("Usage", stdout);
        Assert.Empty(stderr);
    }

    [Fact]
    public void Valid_single_file()
    {
        string path = m_runner.CreateTempFile("fstat-test.txt", "one\ntwo\nthree\n");
        string rel = m_runner.RelativePath(path);
        (int exit, string stdout, string stderr) = m_runner.Run("fstat.exe", rel);
        Assert.Equal(0, exit);
        Assert.Contains("fstat-test.txt", stdout);
        Assert.Empty(stderr);
    }

    [Fact]
    public void Missing_file()
    {
        (int exit, string stdout, string stderr) = m_runner.Run("fstat.exe", "nonexistent-42.txt");
        Assert.Equal(0, exit);
        Assert.Contains("not found", stdout);
        Assert.Empty(stderr);
    }

    [Fact]
    public void Multiple_files_shows_total()
    {
        string p1 = m_runner.CreateTempFile("fstat-a.txt", "aaa\n");
        string p2 = m_runner.CreateTempFile("fstat-b.txt", "bbb\nccc\n");
        (int exit, string stdout, string stderr) = m_runner.Run("fstat.exe",
            m_runner.RelativePath(p1), m_runner.RelativePath(p2));
        Assert.Equal(0, exit);
        Assert.Contains("TOTAL", stdout);
        Assert.Empty(stderr);
    }

    [Fact]
    public void Empty_file()
    {
        string path = m_runner.CreateTempFile("fstat-empty.txt", "");
        string rel = m_runner.RelativePath(path);
        (int exit, string stdout, string stderr) = m_runner.Run("fstat.exe", rel);
        Assert.Equal(0, exit);
        Assert.Contains("fstat-empty.txt", stdout);
        Assert.Empty(stderr);
    }

    [Fact]
    public void Mixed_existing_and_missing()
    {
        string path = m_runner.CreateTempFile("fstat-exists.txt", "data\n");
        (int exit, string stdout, string stderr) = m_runner.Run("fstat.exe",
            m_runner.RelativePath(path), "missing-file-xyz.txt");
        Assert.Equal(0, exit);
        Assert.Contains("not found", stdout);
        Assert.Empty(stderr);
    }
}
