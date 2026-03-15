using System;
using System.Collections.Generic;
using System.Linq;

Console.WriteLine(Codex_safe_divide.main());

public abstract record Result;

public sealed record Success(T0 Field0) : Result;
public sealed record Failure(string Field0) : Result;

public static class Codex_safe_divide
{
    public static Result safe_divide(long x, long y)
    {
        return ((y == 0L) ? new Failure("division by zero") : new Success((x / y)));
    }

    public static string describe(Result result)
    {
        return ((Func<Result, string>)((_scrutinee_) => (_scrutinee_ is Success _mSuccess_ ? ((Func<long, string>)((n) => string.Concat("got ", Convert.ToString(n))))((long)_mSuccess_.Field0) : (_scrutinee_ is Failure _mFailure_ ? ((Func<string, string>)((msg) => string.Concat("error: ", msg)))((string)_mFailure_.Field0) : throw new InvalidOperationException("Non-exhaustive match")))))(result);
    }

    public static string main()
    {
        return describe(safe_divide(42L, 7L));
    }

}
