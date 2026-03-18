using System;

var source = "square : Integer -> Integer\nsquare (x) = x * x\n\ndouble : Integer -> Integer\ndouble (x) = x + x\n\nmain : Integer\nmain = square 5\n";
var result = Codex_Codex_Codex.compile(source, "hello");
Console.WriteLine(result);