using System;
using System.Collections.Generic;
using System.Linq;

Codex_MiniTest.main();

public abstract record Color;
public sealed record Red : Color;
public sealed record Green : Color;
public sealed record Blue(long Field0) : Color;


public sealed record Point(long x, long y);

public static class Codex_MiniTest
{
    public static string show_color(Color c) => c switch { Red { } => "red", Green { } => "green", Blue(var n) => "blue", _ => throw new InvalidOperationException("Non-exhaustive match"), };

    public static long get_x(Point p) => p.x;

    public static Point add_points(Point a, Point b) => new Point(x: (a.x + b.x), y: (a.y + b.y));

    public static List<T18> map_list<T8, T18>(Func<T8, T18> f, List<T8> xs) => map_list_loop(f, xs, 0, list_length(xs), new List<T18>());

    public static List<T31> map_list_loop<T30, T31>(Func<T30, T31> f, List<T30> xs, long i, long len, List<T31> acc)
    {
        while (true)
        {
            if ((i == len))
            {
            return acc;
            }
            else
            {
            var _tco_0 = f;
            var _tco_1 = xs;
            var _tco_2 = (i + 1);
            var _tco_3 = len;
            var _tco_4 = Enumerable.Concat(acc, new List<T31> { f(list_at(xs)(i)) }).ToList();
            f = _tco_0;
            xs = _tco_1;
            i = _tco_2;
            len = _tco_3;
            acc = _tco_4;
            continue;
            }
        }
    }

    public static List<long> use_map(List<Point> pts) => map_list(get_x, pts);

    public static List<long> main() => use_map(new List<Point> { new Point(x: 1, y: 2) });

}
