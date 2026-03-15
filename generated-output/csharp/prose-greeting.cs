using System;
using System.Collections.Generic;
using System.Linq;

Console.WriteLine(Codex_prose_greeting.main());

public static class Codex_prose_greeting
{
    public static string greet(string name)
    {
        return string.Concat("Hello, ", string.Concat(name, "!"));
    }

    public static string main()
    {
        return greet("World");
    }

}
