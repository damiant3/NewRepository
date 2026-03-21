using Xunit;

namespace Codex.AgentToolkit.Tests;

[Collection("AgentToolkit")]
public class SdiffExeTests : IDisposable
{
    readonly AgentExeRunner m_runner = new();

    public void Dispose() => m_runner.CleanupTestDir();

    [Fact]
    public void No_args_prints_usage()
    {
        (int exit, string stdout, string stderr) = m_runner.Run("sdiff.exe");
        Assert.Equal(0, exit);
        Assert.Contains("Usage", stdout);
        Assert.Empty(stderr);
    }

    [Fact]
    public void Snap_missing_file()
    {
        (int exit, string stdout, string stderr) = m_runner.Run("sdiff.exe", "snap", "nonexistent-42.txt");
        Assert.Equal(0, exit);
        Assert.Contains("not found", stdout);
        Assert.Empty(stderr);
    }

    [Fact]
    public void Diff_without_snapshot()
    {
        string path = m_runner.CreateTempFile("sdiff-nodiff.txt", "content\n");
        string rel = m_runner.RelativePath(path);
        (int exit, string stdout, string stderr) = m_runner.Run("sdiff.exe", "diff", rel);
        Assert.Equal(0, exit);
        Assert.Contains("No snapshot", stdout);
        Assert.Empty(stderr);
    }

    [Fact]
    public void Restore_without_snapshot()
    {
        (int exit, string stdout, string stderr) = m_runner.Run("sdiff.exe", "restore", "no-snap.txt");
        Assert.Equal(0, exit);
        Assert.Contains("No snapshot", stdout);
        Assert.Empty(stderr);
    }

    [Fact]
    public void Unknown_action()
    {
        (int exit, string stdout, string stderr) = m_runner.Run("sdiff.exe", "bogus");
        Assert.Equal(0, exit);
        Assert.Contains("Unknown action", stdout);
        Assert.Empty(stderr);
    }

    [Fact]
    public void Snap_no_file_arg()
    {
        (int exit, string stdout, string stderr) = m_runner.Run("sdiff.exe", "snap");
        Assert.Equal(0, exit);
        Assert.Contains("Usage", stdout);
        Assert.Empty(stderr);
    }

    [Fact]
    public void Full_roundtrip()
    {
        string path = m_runner.CreateTempFile("sdiff-rt.txt", "original\n");
        string rel = m_runner.RelativePath(path);

        (int e1, string o1, _) = m_runner.Run("sdiff.exe", "snap", rel);
        Assert.Equal(0, e1);
        Assert.Contains("Snapshot saved", o1);

        (int e2, string o2, _) = m_runner.Run("sdiff.exe", "diff", rel);
        Assert.Equal(0, e2);
        Assert.Contains("No changes", o2);

        File.WriteAllText(path, "modified\nextra\n");

        (int e3, string o3, _) = m_runner.Run("sdiff.exe", "diff", rel);
        Assert.Equal(0, e3);
        Assert.Contains("Diff", o3);

        (int e4, string o4, _) = m_runner.Run("sdiff.exe", "restore", rel);
        Assert.Equal(0, e4);
        Assert.Contains("Restored", o4);
        Assert.Equal("original\n", File.ReadAllText(path));

        File.Delete(path + ".snap");
    }

    [Fact]
    public void Snap_then_delete_original_then_diff()
    {
        string path = m_runner.CreateTempFile("sdiff-del.txt", "data\n");
        string rel = m_runner.RelativePath(path);

        m_runner.Run("sdiff.exe", "snap", rel);
        File.Delete(path);

        (int exit, string stdout, string stderr) = m_runner.Run("sdiff.exe", "diff", rel);
        Assert.Equal(0, exit);
        Assert.Contains("File not found", stdout);
        Assert.Empty(stderr);

        File.Delete(path + ".snap");
    }
}
