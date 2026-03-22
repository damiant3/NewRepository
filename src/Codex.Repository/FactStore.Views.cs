using Codex.Core;

namespace Codex.Repository;

public sealed record ViewInfo(string Name, int DefinitionCount, bool IsCurrent);

public sealed record ViewConsistencyResult(
    bool IsConsistent,
    IReadOnlyList<string> Errors);

public interface IViewConsistencyChecker
{
    ViewConsistencyResult Check(IReadOnlyList<ViewDefinition> definitions);
}

public sealed record ViewDefinition(string Name, string Source);

partial class FactStore
{
    readonly string m_viewsPath = Path.Combine(rootPath, ".codex", "views");
    readonly string m_currentViewMarker = Path.Combine(rootPath, ".codex", "current-view");

    public ViewConsistencyResult CheckViewConsistency(string viewName, IViewConsistencyChecker checker)
    {
        ValueMap<string, ContentHash> view = GetNamedView(viewName);
        List<ViewDefinition> definitions = [];

        foreach (KeyValuePair<string, ContentHash> kv in view)
        {
            Fact? fact = Load(kv.Value);
            if (fact is null)
            {
                return new ViewConsistencyResult(false,
                    [$"Definition '{kv.Key}' references missing fact {kv.Value.ToHex()}"]);
            }
            if (fact.Kind != FactKind.Definition)
            {
                return new ViewConsistencyResult(false,
                    [$"View entry '{kv.Key}' references a {fact.Kind} fact, expected Definition"]);
            }
            definitions.Add(new ViewDefinition(kv.Key, fact.Content));
        }

        if (definitions.Count == 0)
            return new ViewConsistencyResult(true, []);

        return checker.Check(definitions);
    }

    public string GetCurrentViewName()
    {
        if (File.Exists(m_currentViewMarker))
        {
            string name = File.ReadAllText(m_currentViewMarker).Trim();
            if (name.Length > 0)
                return name;
        }
        return "canonical";
    }

    public void CreateView(string name, bool copyFromCurrent = false)
    {
        ValidateViewName(name);

        if (name == "canonical" && File.Exists(m_viewPath))
            throw new InvalidOperationException("View 'canonical' already exists.");

        Directory.CreateDirectory(m_viewsPath);
        string viewFile = Path.Combine(m_viewsPath, name + ".json");
        if (File.Exists(viewFile))
            throw new InvalidOperationException($"View '{name}' already exists.");

        if (copyFromCurrent)
        {
            Map<string, string> current = LoadCurrentViewMap();
            SaveViewMapTo(viewFile, current);
        }
        else
        {
            File.WriteAllText(viewFile, "{}");
        }
    }

    public bool ViewExists(string name)
    {
        string viewFile = Path.Combine(m_viewsPath, name + ".json");
        if (File.Exists(viewFile))
            return true;
        if (name == "canonical" && File.Exists(m_viewPath))
            return true;
        return false;
    }

    public IReadOnlyList<ViewInfo> ListViews()
    {
        List<ViewInfo> views = [];
        string currentName = GetCurrentViewName();

        // Legacy view.json → canonical
        if (File.Exists(m_viewPath) && !File.Exists(Path.Combine(m_viewsPath, "canonical.json")))
        {
            Map<string, string> legacy = LoadViewMapFrom(m_viewPath);
            views.Add(new ViewInfo("canonical", legacy.Count, currentName == "canonical"));
        }

        if (!Directory.Exists(m_viewsPath))
            return views;

        foreach (string file in Directory.GetFiles(m_viewsPath, "*.json"))
        {
            string viewName = Path.GetFileNameWithoutExtension(file);
            Map<string, string> map = LoadViewMapFrom(file);
            views.Add(new ViewInfo(viewName, map.Count, viewName == currentName));
        }

        return views;
    }

    public void SwitchView(string name)
    {
        string viewFile = ResolveViewFile(name);
        if (!File.Exists(viewFile))
            throw new InvalidOperationException($"View '{name}' does not exist.");

        Directory.CreateDirectory(Path.GetDirectoryName(m_currentViewMarker)!);
        File.WriteAllText(m_currentViewMarker, name);
    }

    public void DeleteView(string name)
    {
        if (name == "canonical")
            throw new InvalidOperationException("Cannot delete the canonical view.");

        string viewFile = Path.Combine(m_viewsPath, name + ".json");
        if (!File.Exists(viewFile))
            throw new InvalidOperationException($"View '{name}' does not exist.");

        File.Delete(viewFile);

        if (GetCurrentViewName() == name)
            File.WriteAllText(m_currentViewMarker, "canonical");
    }

    public ValueMap<string, ContentHash> GetNamedView(string viewName)
    {
        string viewFile = ResolveViewFile(viewName);
        if (!File.Exists(viewFile))
            throw new InvalidOperationException($"View '{viewName}' does not exist.");
        Map<string, string> raw = LoadViewMapFrom(viewFile);
        ValueMap<string, ContentHash> result = ValueMap<string, ContentHash>.s_empty;
        foreach (KeyValuePair<string, string> kv in raw)
            result = result.Set(kv.Key, ContentHash.FromHex(kv.Value));
        return result;
    }

    public void UpdateNamedView(string viewName, string definitionName, ContentHash hash)
    {
        string viewFile = ResolveViewFile(viewName);
        if (!File.Exists(viewFile))
            throw new InvalidOperationException($"View '{viewName}' does not exist.");
        Map<string, string> map = LoadViewMapFrom(viewFile);
        map = map.Set(definitionName, hash.ToHex());
        SaveViewMapTo(viewFile, map);
    }

    public void RemoveFromView(string viewName, string definitionName)
    {
        string viewFile = ResolveViewFile(viewName);
        if (!File.Exists(viewFile))
            throw new InvalidOperationException($"View '{viewName}' does not exist.");
        Map<string, string> map = LoadViewMapFrom(viewFile);
        map = map.Remove(definitionName);
        SaveViewMapTo(viewFile, map);
    }

    string ResolveViewFile(string name)
    {
        // Check views directory first
        string viewFile = Path.Combine(m_viewsPath, name + ".json");
        if (File.Exists(viewFile))
            return viewFile;

        // Fall back to legacy view.json for "canonical"
        if (name == "canonical" && File.Exists(m_viewPath))
            return m_viewPath;

        return viewFile; // caller checks existence
    }

    Map<string, string> LoadCurrentViewMap()
    {
        string viewFile = ResolveViewFile(GetCurrentViewName());
        return LoadViewMapFrom(viewFile);
    }

    static void ValidateViewName(string name)
    {
        string trimmed = name.Trim();
        if (trimmed.Length == 0)
            throw new ArgumentException("View name cannot be empty or whitespace.", nameof(name));
        if (trimmed.Length != name.Length)
            throw new ArgumentException("View name cannot have leading or trailing whitespace.", nameof(name));
        if (name.Contains('/') || name.Contains('\\') || name.Contains(Path.DirectorySeparatorChar))
            throw new ArgumentException("View name cannot contain path separators.", nameof(name));
        if (name is "." or "..")
            throw new ArgumentException("View name cannot be '.' or '..'.", nameof(name));
    }

    static Map<string, string> LoadViewMapFrom(string path)
    {
        if (!File.Exists(path))
            return Map<string, string>.s_empty;
        string json = File.ReadAllText(path);
        Dictionary<string, string>? raw =
            System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json, s_jsonOptions);
        if (raw is null)
            return Map<string, string>.s_empty;
        Map<string, string> result = Map<string, string>.s_empty;
        foreach (KeyValuePair<string, string> kv in raw)
            result = result.Set(kv.Key, kv.Value);
        return result;
    }

    static void SaveViewMapTo(string path, Map<string, string> view)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        Dictionary<string, string> raw = [];
        foreach (KeyValuePair<string, string> kv in view)
            raw[kv.Key] = kv.Value;
        string json = System.Text.Json.JsonSerializer.Serialize(raw, s_jsonOptions);
        File.WriteAllText(path, json);
    }
}
