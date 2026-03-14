using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using Codex.Core;

namespace Codex.Repository;

public enum FactKind
{
    Definition,
    Supersession,
    Deprecation
}

public sealed record Fact(
    ContentHash Hash,
    FactKind Kind,
    string Content,
    string Author,
    DateTime Timestamp,
    string Justification,
    ImmutableArray<ContentHash> References)
{
    public static Fact CreateDefinition(string source, string author, string justification)
    {
        ContentHash hash = ContentHash.Of(source);
        return new Fact(hash, FactKind.Definition, source, author, DateTime.UtcNow,
            justification, []);
    }

    public static Fact CreateSupersession(
        ContentHash newDefinition, ContentHash supersedes, string author, string justification)
    {
        string content = $"supersedes:{supersedes.ToHex()}\nnew:{newDefinition.ToHex()}";
        ContentHash hash = ContentHash.Of(content);
        return new Fact(hash, FactKind.Supersession, content, author, DateTime.UtcNow,
            justification, [newDefinition, supersedes]);
    }
}

public sealed class FactStore(string rootPath)
{
    readonly string m_rootPath = rootPath;
    readonly string m_factsPath = Path.Combine(rootPath, ".codex", "facts");
    readonly string m_viewPath = Path.Combine(rootPath, ".codex", "view.json");
    Map<ContentHash, Fact> m_cache = Map<ContentHash, Fact>.s_empty;

    public static FactStore Init(string rootPath)
    {
        string codexDir = Path.Combine(rootPath, ".codex");
        string factsDir = Path.Combine(codexDir, "facts");

        Directory.CreateDirectory(codexDir);
        Directory.CreateDirectory(factsDir);

        string viewPath = Path.Combine(codexDir, "view.json");
        if (!File.Exists(viewPath))
        {
            File.WriteAllText(viewPath, "{}");
        }

        return new(rootPath);
    }

    public static FactStore? Open(string rootPath)
    {
        string codexDir = Path.Combine(rootPath, ".codex");
        if (!Directory.Exists(codexDir))
            return null;
        return new(rootPath);
    }

    public ContentHash Store(Fact fact)
    {
        string hex = fact.Hash.ToHex();
        string factDir = Path.Combine(m_factsPath, hex[..2]);
        Directory.CreateDirectory(factDir);

        string factFile = Path.Combine(factDir, hex + ".json");
        if (!File.Exists(factFile))
        {
            FactDto dto = new()
            {
                Hash = hex,
                Kind = fact.Kind.ToString(),
                Content = fact.Content,
                Author = fact.Author,
                Timestamp = fact.Timestamp,
                Justification = fact.Justification,
                References = [.. fact.References.Select(r => r.ToHex())]
            };
            string json = JsonSerializer.Serialize(dto, s_jsonOptions);
            File.WriteAllText(factFile, json);
        }

        m_cache = m_cache.Set(fact.Hash, fact);
        return fact.Hash;
    }

    public Fact? Load(ContentHash hash)
    {
        Fact? cached = m_cache[hash];
        if (cached is not null)
            return cached;

        string hex = hash.ToHex();
        string factFile = Path.Combine(m_factsPath, hex[..2], hex + ".json");
        if (!File.Exists(factFile))
            return null;

        string json = File.ReadAllText(factFile);
        FactDto? dto = JsonSerializer.Deserialize<FactDto>(json, s_jsonOptions);
        if (dto is null)
            return null;

        Fact fact = new(
            ContentHash.FromHex(dto.Hash),
            Enum.Parse<FactKind>(dto.Kind),
            dto.Content,
            dto.Author,
            dto.Timestamp,
            dto.Justification,
            [.. dto.References.Select(ContentHash.FromHex)]);

        m_cache = m_cache.Set(hash, fact);
        return fact;
    }

    public void UpdateView(string name, ContentHash hash)
    {
        Map<string, string> view = LoadViewMap();
        view = view.Set(name, hash.ToHex());
        SaveViewMap(view);
    }

    public ContentHash? LookupView(string name)
    {
        Map<string, string> view = LoadViewMap();
        string? hex = view[name];
        return hex is not null ? ContentHash.FromHex(hex) : null;
    }

    public ValueMap<string, ContentHash> GetView()
    {
        Map<string, string> raw = LoadViewMap();
        ValueMap<string, ContentHash> result = ValueMap<string, ContentHash>.s_empty;
        foreach (KeyValuePair<string, string> kv in raw)
        {
            result = result.Set(kv.Key, ContentHash.FromHex(kv.Value));
        }
        return result;
    }

    public IReadOnlyList<Fact> GetHistory(string name)
    {
        List<Fact> history = [];
        ContentHash? current = LookupView(name);
        Set<string> visited = Set<string>.s_empty;

        while (current is not null)
        {
            string hex = current.Value.ToHex();
            if (visited.Contains(hex))
                break;
            visited = visited.Add(hex);

            Fact? fact = Load(current.Value);
            if (fact is null) break;
            history.Add(fact);

            Fact? supersession = FindSupersessionOf(current.Value);
            if (supersession is not null)
            {
                ContentHash oldHash = supersession.References.Length > 1
                    ? supersession.References[1]
                    : default;
                if (oldHash.Bytes.Length > 0)
                {
                    current = oldHash;
                    continue;
                }
            }
            break;
        }

        return history;
    }

    Fact? FindSupersessionOf(ContentHash newHash)
    {
        if (!Directory.Exists(m_factsPath))
            return null;

        foreach (string subDir in Directory.GetDirectories(m_factsPath))
        {
            foreach (string file in Directory.GetFiles(subDir, "*.json"))
            {
                string json = File.ReadAllText(file);
                FactDto? dto = JsonSerializer.Deserialize<FactDto>(json, s_jsonOptions);
                if (dto?.Kind == "Supersession" && dto.References.Contains(newHash.ToHex()))
                {
                    return new Fact(
                        ContentHash.FromHex(dto.Hash),
                        FactKind.Supersession,
                        dto.Content,
                        dto.Author,
                        dto.Timestamp,
                        dto.Justification,
                        [.. dto.References.Select(ContentHash.FromHex)]);
                }
            }
        }
        return null;
    }

    public bool IsInitialized => Directory.Exists(Path.Combine(m_rootPath, ".codex"));

    Map<string, string> LoadViewMap()
    {
        if (!File.Exists(m_viewPath))
            return Map<string, string>.s_empty;
        string json = File.ReadAllText(m_viewPath);
        Dictionary<string, string>? raw =
            JsonSerializer.Deserialize<Dictionary<string, string>>(json, s_jsonOptions);
        if (raw is null)
            return Map<string, string>.s_empty;
        Map<string, string> result = Map<string, string>.s_empty;
        foreach (KeyValuePair<string, string> kv in raw)
            result = result.Set(kv.Key, kv.Value);
        return result;
    }

    void SaveViewMap(Map<string, string> view)
    {
        Dictionary<string, string> raw = [];
        foreach (KeyValuePair<string, string> kv in view)
            raw[kv.Key] = kv.Value;
        string json = JsonSerializer.Serialize(raw, s_jsonOptions);
        File.WriteAllText(m_viewPath, json);
    }

    static readonly JsonSerializerOptions s_jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    sealed class FactDto
    {
        public string Hash { get; set; } = "";
        public string Kind { get; set; } = "";
        public string Content { get; set; } = "";
        public string Author { get; set; } = "";
        public DateTime Timestamp { get; set; }
        public string Justification { get; set; } = "";
        public List<string> References { get; set; } = [];
    }
}
