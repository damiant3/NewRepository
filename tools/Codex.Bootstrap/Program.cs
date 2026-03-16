using System;
using System.IO;
using System.Linq;
using System.Threading;

class Program
{
    static int Main(string[] args)
    {
        int exitCode = 1;
        var thread = new Thread(() => exitCode = Run(args), 256 * 1024 * 1024);
        thread.Start();
        thread.Join();
        return exitCode;
    }

    static int Run(string[] args)
    {
        string codexDir = args.Length > 0 ? args[0] : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "Codex.Codex"));

        if (!Directory.Exists(codexDir))
        {
            Console.Error.WriteLine($"Codex.Codex directory not found: {codexDir}");
            return 1;
        }

        Console.WriteLine($"Reading .codex sources from: {codexDir}");

        string[] files = Directory.GetFiles(codexDir, "*.codex", SearchOption.AllDirectories)
            .OrderBy(f => f, StringComparer.Ordinal)
            .ToArray();

        Console.WriteLine($"Found {files.Length} .codex files");

        string combined = string.Join("\n\n", files.Select(f =>
        {
            Console.WriteLine($"  {Path.GetRelativePath(codexDir, f)}");
            return File.ReadAllText(f);
        }));

        Console.WriteLine($"Total source: {combined.Length} chars");
        Console.WriteLine("Compiling with Codex.Codex (Stage 1)...");

        try
        {
            string output = Codex_Codex_Codex.compile(combined, "Codex_Codex");
            string outputPath = Path.Combine(codexDir, "stage1-output.cs");
            File.WriteAllText(outputPath, output);
            Console.WriteLine($"Stage 1 output written to: {outputPath}");
            Console.WriteLine($"Output size: {output.Length} chars");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Compilation failed: {ex.GetType().Name}: {ex.Message}");
            Console.Error.WriteLine(ex.StackTrace);
            return 1;
        }
    }
}
