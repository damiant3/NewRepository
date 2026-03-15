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
        return ((Func<Shape, decimal>)((_scrutinee_) => (_scrutinee_ is Circle _mCircle_ ? ((Func<decimal, decimal>)((r) => ((3.14m * r) * r)))((decimal)_mCircle_.Field0) : (_scrutinee_ is Rectangle _mRectangle_ ? ((Func<decimal, decimal>)((h) => ((Func<decimal, decimal>)((w) => (w * h)))((decimal)_mRectangle_.Field0)))((decimal)_mRectangle_.Field1) : throw new InvalidOperationException("Non-exhaustive match")))))(new Circle(5.0m));
    }

}
