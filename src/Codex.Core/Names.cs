namespace Codex.Core;

public readonly record struct Name(string Value) : IComparable<Name>
{
    public bool IsTypeName => Value.Length > 0 && char.IsUpper(Value[0]);

    public bool IsValueName => Value.Length > 0 && char.IsLower(Value[0]);

    public int CompareTo(Name other) =>
        string.Compare(Value, other.Value, StringComparison.Ordinal);

    public override string ToString() => Value;
}

public readonly record struct QualifiedName(IReadOnlyList<Name> Parts)
{
    public static QualifiedName Simple(string name) =>
        new([new Name(name)]);

    public static QualifiedName Parse(string dotted) =>
        new(dotted.Split('.').Select(p => new Name(p)).ToArray());

    public Name Leaf => Parts[^1];

    public QualifiedName Qualifier =>
        Parts.Count > 1
            ? new(Parts.Take(Parts.Count - 1).ToArray())
            : new(Array.Empty<Name>());

    public QualifiedName Append(Name name) =>
        new(Parts.Append(name).ToArray());

    public override string ToString() =>
        string.Join(".", Parts.Select(p => p.Value));
}
