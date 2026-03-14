using System.Collections.Immutable;
using Codex.Core;

namespace Codex.Types;

// Immutable type environment. Each scope extends the parent without mutation.
public sealed class TypeEnvironment
{
    private readonly ImmutableDictionary<string, CodexType> m_bindings;

    public TypeEnvironment()
        : this(ImmutableDictionary<string, CodexType>.Empty)
    {
    }

    private TypeEnvironment(ImmutableDictionary<string, CodexType> bindings)
    {
        m_bindings = bindings;
    }

    public TypeEnvironment Bind(string name, CodexType type)
    {
        return new TypeEnvironment(m_bindings.SetItem(name, type));
    }

    public TypeEnvironment Bind(Name name, CodexType type)
    {
        return Bind(name.Value, type);
    }

    public CodexType? Lookup(string name)
    {
        return m_bindings.TryGetValue(name, out CodexType? type) ? type : null;
    }

    public CodexType? Lookup(Name name) => Lookup(name.Value);

    public bool Contains(string name) => m_bindings.ContainsKey(name);

    public IEnumerable<KeyValuePair<string, CodexType>> AllBindings => m_bindings;

    public static TypeEnvironment WithBuiltins()
    {
        TypeEnvironment env = new TypeEnvironment();

        env = env.Bind("show", new ForAllType(0,
            new FunctionType(new TypeVariable(0), TextType.s_instance)));

        env = env.Bind("negate", new FunctionType(IntegerType.s_instance, IntegerType.s_instance));

        return env;
    }
}
