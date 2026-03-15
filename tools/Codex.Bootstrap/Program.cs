namespace Codex.Bootstrap;

class Program
{
    static int Main(string[] args)
    {
        int result = 1;
        Thread thread = new(() => result = Run(args), 256 * 1024 * 1024);
        thread.Start();
        thread.Join();
        return result;
    }

    static int Run(string[] args)
    {
        if (args.Length < 1)
        {
            Console.Error.WriteLine("Usage: stage1 build <directory>");
            return 1;
        }

        if (args[0] == "build" && args.Length >= 2)
        {
            string dir = args[1];
            string[] files = Directory.GetFiles(dir, "*.codex", SearchOption.AllDirectories);
            Array.Sort(files, StringComparer.Ordinal);

            string combined = "";
            foreach (string file in files)
            {
                string raw = File.ReadAllText(file);
                string extracted = ExtractNotation(raw);
                combined += extracted + "\n";
            }

            string moduleName = Path.GetFileName(dir);
            string output = Codex_codex_src.compile(combined, moduleName);
            string outputPath = Path.Combine(dir, "stage1-output.cs");
            File.WriteAllText(outputPath, output);
            Console.WriteLine($"Stage 1 compiled to {outputPath}");
            return 0;
        }

        Console.Error.WriteLine($"Unknown command: {args[0]}");
        return 1;
    }

    static string ExtractNotation(string content)
    {
        bool hasProse = false;
        foreach (string line in content.Split('\n'))
        {
            string t = line.Trim();
            if (t.Length == 0) continue;
            if (t.StartsWith("Chapter:", StringComparison.Ordinal))
            {
                hasProse = true;
                break;
            }
            break;
        }

        if (!hasProse)
            return content;

        string[] lines = content.Split('\n');
        List<string> result = [];
        int i = 0;

        while (i < lines.Length)
        {
            string raw = lines[i].TrimEnd('\r');
            string trimmed = raw.TrimStart();

            if (trimmed.StartsWith("Chapter:", StringComparison.Ordinal) ||
                trimmed.StartsWith("Section:", StringComparison.Ordinal))
            {
                i++;
                continue;
            }

            int indent = raw.Length - trimmed.Length;
            if (trimmed.Length == 0 || indent < 4 || !LooksLikeNotation(trimmed))
            {
                i++;
                continue;
            }

            int baseIndent = indent;

            while (i < lines.Length)
            {
                string line = lines[i].TrimEnd('\r');
                string lt = line.Trim();

                if (lt.Length == 0)
                {
                    int peek = i + 1;
                    while (peek < lines.Length && lines[peek].Trim().Length == 0) peek++;
                    if (peek < lines.Length)
                    {
                        string peekLine = lines[peek].TrimEnd('\r');
                        int peekIndent = peekLine.Length - peekLine.TrimStart().Length;
                        if (peekIndent >= baseIndent &&
                            !peekLine.TrimStart().StartsWith("Chapter:", StringComparison.Ordinal) &&
                            !peekLine.TrimStart().StartsWith("Section:", StringComparison.Ordinal))
                        {
                            result.Add("");
                            i++;
                            continue;
                        }
                    }
                    break;
                }

                int lineIndent = line.Length - line.TrimStart().Length;
                if (lineIndent < baseIndent) break;
                if (lt.StartsWith("Chapter:", StringComparison.Ordinal) ||
                    lt.StartsWith("Section:", StringComparison.Ordinal))
                    break;

                string dedented = lineIndent >= baseIndent ? line[baseIndent..] : lt;
                result.Add(dedented);
                i++;
            }

            result.Add("");
        }

        return string.Join("\n", result);
    }

    static bool LooksLikeNotation(string trimmed)
    {
        if (trimmed.Length == 0) return false;
        if (trimmed[0] == '|') return true;
        if (char.IsLetter(trimmed[0]) || trimmed[0] == '_')
        {
            if (trimmed.Contains(" : ")) return true;
            if (trimmed.Contains(" = ")) return true;
            if (trimmed.EndsWith(" =") || trimmed.EndsWith("=")) return true;
            if (trimmed.Contains('(')) return true;
        }
        return false;
    }
}
