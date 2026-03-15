using Codex.Core;

namespace Codex.Types;

public sealed class TypeEnvironment
{
    readonly Map<string, CodexType> m_bindings;

    public TypeEnvironment() : this(Map<string, CodexType>.s_empty)
    {
    }

    TypeEnvironment(Map<string, CodexType> bindings)
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

        env = env.Bind("char-at", new FunctionType(TextType.s_instance,
            new FunctionType(IntegerType.s_instance, TextType.s_instance)));
        env = env.Bind("text-length", new FunctionType(TextType.s_instance, IntegerType.s_instance));
        env = env.Bind("substring", new FunctionType(TextType.s_instance,
            new FunctionType(IntegerType.s_instance,
                new FunctionType(IntegerType.s_instance, TextType.s_instance))));
        env = env.Bind("is-letter", new FunctionType(TextType.s_instance, BooleanType.s_instance));
        env = env.Bind("is-digit", new FunctionType(TextType.s_instance, BooleanType.s_instance));
        env = env.Bind("is-whitespace", new FunctionType(TextType.s_instance, BooleanType.s_instance));
        env = env.Bind("text-to-integer", new FunctionType(TextType.s_instance, IntegerType.s_instance));
        env = env.Bind("char-code", new FunctionType(TextType.s_instance, IntegerType.s_instance));
        env = env.Bind("code-to-char", new FunctionType(IntegerType.s_instance, TextType.s_instance));

        env = env.Bind("list-length", new ForAllType(0,
            new FunctionType(new ListType(new TypeVariable(0)), IntegerType.s_instance)));
        env = env.Bind("list-at", new ForAllType(0,
            new FunctionType(new ListType(new TypeVariable(0)),
                new FunctionType(IntegerType.s_instance, new TypeVariable(0)))));
        
        return env;
    }
}
