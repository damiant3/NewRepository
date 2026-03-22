using Codex.Core;

namespace Codex.Repository;

public sealed record ViewInfo(string Name, int DefinitionCount, bool IsCurrent);

partial class FactStore
{
    readonly string m_viewsPath = Path.Combine(rootPath, ".codex", "views");
    readonly string m_currentViewMarker = Path.Combine(rootPath, ".codex", "current-view");

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

    public IReadOnlyList<ViewInfo> ListViews()
    {
        List<ViewInfo> views = [];
        string currentName = GetCurrentViewName();

        // Legacy view.json → canonical
        if (File.Exists(m_viewPath) && !File.Exists(Path.Combine(m_viewsPath, "canonical.json")))
        {
            Map<string, string> legacy = LoadViewMapFrom(m_viewPath);
            int count = 0;
            foreach (KeyValuePair<string, string> _ in legacy)
                count++;
            views.Add(new ViewInfo("canonical", count, currentName == "canonical"));
        }

        if (!Directory.Exists(m_viewsPath))
            return views;

        foreach (string file in Directory.GetFiles(m_viewsPath, "*.json"))
        {
            string name = Path.GetFileNameWithoutExtension(file);
            Map<string, string> map = LoadViewMapFrom(file);
            int count = 0;
            foreach (KeyValuePair<string, string> _ in map)
                count++;
            views.Add(new ViewInfo(name, count, name == currentName));
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
        Map<string, string> raw = LoadViewMapFrom(viewFile);
        ValueMap<string, ContentHash> result = ValueMap<string, ContentHash>.s_empty;
        foreach (KeyValuePair<string, string> kv in raw)
            result = result.Set(kv.Key, ContentHash.FromHex(kv.Value));
        return result;
    }

    public void UpdateNamedView(string viewName, string definitionName, ContentHash hash)
    {
        string viewFile = ResolveViewFile(viewName);
        Map<string, string> map = LoadViewMapFrom(viewFile);
        map = map.Set(definitionName, hash.ToHex());
        SaveViewMapTo(viewFile, map);
    }

    public void RemoveFromView(string viewName, string definitionName)
    {
        string viewFile = ResolveViewFile(viewName);
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

        return viewFile; // will fail on read if doesn't exist
    }

    Map<string, string> LoadCurrentViewMap()
    {
        string viewFile = ResolveViewFile(GetCurrentViewName());
        return LoadViewMapFrom(viewFile);
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
