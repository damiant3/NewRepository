using Codex.Ast;
using Codex.Core;
using Codex.Syntax;

namespace Codex.Types;

sealed class BuiltinEffects
{
    static readonly string[] s_effectSources =
    [
        """
        effect Console where
          print-line : Text -> [Console] Nothing
          read-line  : [Console] Text
        """,

        """
        effect FileSystem where
          open-file  : Text -> [FileSystem] linear FileHandle
          read-all   : linear FileHandle -> [FileSystem] Pair Text (linear FileHandle)
          close-file : linear FileHandle -> [FileSystem] Nothing
          read-file  : Text -> [FileSystem] Text
          write-file : Text -> Text -> [FileSystem] Nothing
        """,

        """
        effect Time where
          now : [Time] Integer
        """,

        """
        effect Random where
          random-integer : Integer -> Integer -> [Random] Integer
        """,

        """
        effect State where
          get-state : [State] s
          set-state : s -> [State] Nothing
        """
    ];

    static IReadOnlyList<EffectDef>? s_cached;

    public static IReadOnlyList<EffectDef> Load()
    {
        if (s_cached is not null)
            return s_cached;

        List<EffectDef> allEffects = [];
        DiagnosticBag diagnostics = new();
        Desugarer desugarer = new(diagnostics);

        foreach (string source in s_effectSources)
        {
            SourceText src = new("<builtin-effects>", source);
            Lexer lexer = new(src, diagnostics);
            IReadOnlyList<Token> tokens = lexer.TokenizeAll();
            Parser parser = new(tokens, diagnostics);
            DocumentNode document = parser.ParseDocument();
            Module module = desugarer.Desugar(document, "builtins");
            allEffects.AddRange(module.EffectDefs);
        }

        s_cached = allEffects;
        return s_cached;
    }
}
