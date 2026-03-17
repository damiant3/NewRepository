using System;
using System.Collections.Generic;
using System.Linq;

Console.WriteLine(Codex_shapes.main());

public abstract record Shape;

public sealed record Circle(decimal Field0) : Shape;
public sealed record Rectangle(decimal Field0, decimal Field1) : Shape;

public static class Codex_shapes
{
    public static decimal main()
    {
        return ((Func<Shape, decimal>)((_scrutinee5_) => (_scrutinee5_ is Circle _mCircle5_ ? ((Func<decimal, decimal>)((r) => ((3.14m * r) * r)))((decimal)_mCircle5_.Field0) : (_scrutinee5_ is Rectangle _mRectangle5_ ? ((Func<decimal, decimal>)((h) => ((Func<decimal, decimal>)((w) => (w * h)))((decimal)_mRectangle5_.Field0)))((decimal)_mRectangle5_.Field1) : throw new InvalidOperationException("Non-exhaustive match")))))(new Circle(5.0m));
    }

}
