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
        return ((Func<Shape, decimal>)((_scrutinee3_) => (_scrutinee3_ is Circle _mCircle3_ ? ((Func<decimal, decimal>)((r) => ((3.14m * r) * r)))((decimal)_mCircle3_.Field0) : (_scrutinee3_ is Rectangle _mRectangle3_ ? ((Func<decimal, decimal>)((h) => ((Func<decimal, decimal>)((w) => (w * h)))((decimal)_mRectangle3_.Field0)))((decimal)_mRectangle3_.Field1) : throw new InvalidOperationException("Non-exhaustive match")))))(new Circle(5.0m));
    }

}
