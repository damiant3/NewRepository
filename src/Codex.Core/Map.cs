using System.Collections;
using System.Collections.Immutable;

namespace Codex.Core;

public sealed class Map<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    where TKey : notnull
    where TValue : class
{
    private readonly ImmutableDictionary<TKey, TValue> m_inner;

    private Map(ImmutableDictionary<TKey, TValue> inner)
    {
        m_inner = inner;
    }

    public static readonly Map<TKey, TValue> s_empty = new(ImmutableDictionary<TKey, TValue>.Empty);

    public TValue? this[TKey key] =>
        m_inner.TryGetValue(key, out TValue? value) ? value : null;

    public Map<TKey, TValue> Set(TKey key, TValue value) =>
        new(m_inner.SetItem(key, value));

    public bool ContainsKey(TKey key) => m_inner.ContainsKey(key);

    public int Count => m_inner.Count;

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => m_inner.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
