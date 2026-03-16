using System;
using System.Collections.Generic;
using System.Linq;

Console.WriteLine(Codex_Codex.Codex.main());

public static class Codex_Codex.Codex
{
    public static string greeting(string name)
    {
        return string.Concat("Hello, ", string.Concat(name, "!"));
    }

    public static string main()
    {
        return greeting("World");
    }

}
