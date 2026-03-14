namespace Codex.Core;

/// <summary>
/// A Codex identifier. Names in Codex are hyphenated-lowercase for values (e.g., compute-monthly-payment)
/// and Capitalized for types (e.g., Account, List, Result).
/// </summary>
public readonly record struct Name(string Value) : IComparable<Name>
{
    /// <summary>Is this a type name (starts with uppercase)?</summary>
    public bool IsTypeName => Value.Length > 0 && char.IsUpper(Value[0]);

    /// <summary>Is this a value name (starts with lowercase)?</summary>
    public bool IsValueName => Value.Length > 0 && char.IsLower(Value[0]);

    public int CompareTo(Name other) =>
        string.Compare(Value, other.Value, StringComparison.Ordinal);

    public override string ToString() => Value;
}

/// <summary>
/// A qualified name: a sequence of names representing a path through the module hierarchy.
/// Example: Sorting.Merge-Sort.merge-sort
/// </summary>
public readonly record struct QualifiedName(IReadOnlyList<Name> Parts)
{
    /// <summary>Create a qualified name from a single unqualified name.</summary>
    public static QualifiedName Simple(string name) =>
        new([new Name(name)]);

    /// <summary>Create from dot-separated string.</summary>
    public static QualifiedName Parse(string dotted) =>
        new(dotted.Split('.').Select(p => new Name(p)).ToArray());

    /// <summary>The final component (the unqualified name).</summary>
    public Name Leaf => Parts[^1];

    /// <summary>The qualifier (all components except the last), or empty.</summary>
    public QualifiedName Qualifier =>
        Parts.Count > 1
            ? new(Parts.Take(Parts.Count - 1).ToArray())
            : new(Array.Empty<Name>());

    /// <summary>Append a name to this qualified name.</summary>
    public QualifiedName Append(Name name) =>
        new(Parts.Append(name).ToArray());

    public override string ToString() =>
        string.Join(".", Parts.Select(p => p.Value));
}
