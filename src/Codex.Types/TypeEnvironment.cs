using System.Collections.Immutable;
using Codex.Core;

namespace Codex.Types;

public sealed class TypeEnvironment
{
    private readonly Map<string, CodexType> m_bindings;

    public TypeEnvironment() : this(Map<string, CodexType>.s_empty)
    {
    }

    private TypeEnvironment(Map<string, CodexType> bindings)
    {
        m_bindings = bindings;
    }

    public TypeEnvironment Bind(string name, CodexType type) => new(m_bindings.Set(name, type));

    public TypeEnvironment Bind(Name name, CodexType type) => Bind(name.Value, type);

    public CodexType? Lookup(string name) => m_bindings[name];

    public CodexType? Lookup(Name name) => Lookup(name.Value);

    public bool Contains(string name) => m_bindings.ContainsKey(name);

    public static TypeEnvironment WithBuiltins()
    {
        TypeEnvironment env = new();
        env = env.Bind("show", new ForAllType(0,
            new FunctionType(new TypeVariable(0), TextType.s_instance)));
        env = env.Bind("negate", new FunctionType(IntegerType.s_instance, IntegerType.s_instance));

        EffectfulType consoleText = new(
            [new EffectType(new Name("Console"))],
            TextType.s_instance);
        env = env.Bind("read-line", consoleText);

        EffectfulType consoleNothing = new(
            [new EffectType(new Name("Console"))],
            NothingType.s_instance);
        env = env.Bind("print-line", new FunctionType(TextType.s_instance, consoleNothing));

        LinearType fileHandle = new(new ConstructedType(new Name("FileHandle"),[]));

        EffectfulType fsFileHandle = new(
            [new EffectType(new Name("FileSystem"))],
            fileHandle);
        env = env.Bind("open-file", new FunctionType(TextType.s_instance, fsFileHandle));

        EffectfulType fsTextAndHandle = new(
            [new EffectType(new Name("FileSystem"))],
            new ConstructedType(new Name("Pair"), [TextType.s_instance, fileHandle]));
        env = env.Bind("read-all", new FunctionType(fileHandle, fsTextAndHandle));

        EffectfulType fsNothing = new(
            [new EffectType(new Name("FileSystem"))],
            NothingType.s_instance);
        env = env.Bind("close-file", new FunctionType(fileHandle, fsNothing));

        return env;
    }
}
