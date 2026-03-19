using System.Text.Json;

namespace Codex.Cli;

public static partial class Program
{
    internal sealed class CodexProject
    {
        public string Name { get; set; } = "";
        public string Version { get; set; } = "0.1.0";
        public string Description { get; set; } = "";
        public string[] Sources { get; set; } = ["**/*.codex"];
        public string[] Exclude { get; set; } = [];
        public string[] Dependencies { get; set; } = [];
        public string Target { get; set; } = "cs";
        public string[] Targets { get; set; } = [];
        public string Output { get; set; } = "out/";
    }

    internal static CodexProject? LoadProjectFile(string directory)
    {
        string projectPath = Path.Combine(directory, "codex.project.json");
        if (!File.Exists(projectPath)) return null;

        string json = File.ReadAllText(projectPath);
        JsonSerializerOptions options = new()
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true
        };
        return JsonSerializer.Deserialize<CodexProject>(json, options);
    }

    internal static string[] ResolveProjectSources(string directory, CodexProject project)
    {
        List<string> files = [];
        foreach (string pattern in project.Sources)
        {
            if (pattern.Contains('*'))
            {
                string searchDir = directory;
                SearchOption searchOpt = pattern.Contains("**")
                    ? SearchOption.AllDirectories
                    : SearchOption.TopDirectoryOnly;
                string filePattern = Path.GetFileName(pattern);
                foreach (string file in Directory.GetFiles(searchDir, filePattern, searchOpt))
                {
                    if (!files.Contains(file))
                        files.Add(file);
                }
            }
            else
            {
                string fullPath = Path.Combine(directory, pattern);
                if (File.Exists(fullPath) && !files.Contains(fullPath))
                    files.Add(fullPath);
            }
        }

        // Apply exclude patterns
        foreach (string exclude in project.Exclude)
        {
            string excludePattern = Path.GetFileName(exclude);
            bool recursive = exclude.Contains("**");
            SearchOption excludeOpt = recursive
                ? SearchOption.AllDirectories
                : SearchOption.TopDirectoryOnly;
            string excludeDir = recursive ? directory
                : Path.Combine(directory, Path.GetDirectoryName(exclude) ?? "");
            if (Directory.Exists(excludeDir))
            {
                HashSet<string> excluded = new(
                    Directory.GetFiles(excludeDir, excludePattern, excludeOpt),
                    StringComparer.OrdinalIgnoreCase);
                files.RemoveAll(f => excluded.Contains(f));
            }
        }

        files.Sort(StringComparer.Ordinal);
        return files.ToArray();
    }
}
