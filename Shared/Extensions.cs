namespace Shared;

public static class Extensions
{
    public static (int i, int j) TurnLeft(this (int i, int j) dir) => (-dir.j, dir.i);
    public static (int i, int j) TurnBack(this (int i, int j) dir) => dir.TurnRight().TurnRight();
    public static (int i, int j) TurnRight(this (int i, int j) dir) => (dir.j, -dir.i);

    public static bool OutOfBorders<T>(this T[][] input, (int i, int j) pos)
        => pos.i < 0 || pos.j < 0 || pos.i >= input.Length || pos.j >= input[0].Length;

    public static bool OutOfBorders(this string[] input, (int i, int j) pos)
        => pos.i < 0 || pos.j < 0 || pos.i >= input.Length || pos.j >= input[0].Length;

    public static T[][] Rotate<T>(this T[][] arr)
    {
        var array = arr[0].Select(_ => new T[arr.Length]).ToArray();
        for (var i = 0; i < arr.Length; i++)
        {
            for (var j = 0; j < arr[0].Length; j++)
            {
                array[j][i] = arr[i][j];
            }
        }

        return array;
    }

    public static int GetIndex<T>(this T[] array, int curIndex, int diff) => (curIndex + diff + array.Length) % array.Length;

    public static char[][] ToCharArray(this string[] input) => input.Select(x => x.ToCharArray()).ToArray();
    public static IEnumerable<Point> GetAllPoints(this char[][] map) => map.SelectMany((x, i) => x.Select((_, j) => new Point(i, j)));

    public static void Print(this char[][] input)
    {
        foreach (var line in input)
            Console.WriteLine(new string(line));
    }

    public static T At<T>(this T[][] input, (int i, int j) pos) => input[pos.i][pos.j];
    public static void Set<T>(this T[][] input, (int i, int j) pos, T val) => input[pos.i][pos.j] = val;

    public static IEnumerable<(int i1, int j1)> GetDirectNeighbours<T>(this T[][] input, (int i, int j) pos)
    {
        if (pos.i < input.Length - 1)
            yield return (pos.i + 1, pos.j);

        if (pos.j < input[0].Length - 1)
            yield return (pos.i, pos.j + 1);

        if (pos.i > 0)
            yield return (pos.i - 1, pos.j);

        if (pos.j > 0)
            yield return (pos.i, pos.j - 1);
    }

    public static IEnumerable<(int i1, int j1)> GetAllNeighbours<T>(this T[][] input, (int i, int j) pos)
    {
        if (pos.i < input.Length - 1)
            yield return (pos.i + 1, pos.j);
        if (pos.i > 0)
            yield return (pos.i - 1, pos.j);
        if (pos.j < input[0].Length - 1)
            yield return (pos.i, pos.j + 1);
        if (pos.j > 0)
            yield return (pos.i, pos.j - 1);
        if (pos.i < input.Length - 1 && pos.j < input[0].Length - 1)
            yield return (pos.i + 1, pos.j + 1);
        if (pos is {i: > 0, j: > 0})
            yield return (pos.i - 1, pos.j - 1);
        if (pos.i < input.Length - 1 && pos.j > 0)
            yield return (pos.i + 1, pos.j - 1);
        if (pos.i > 0 && pos.j < input[0].Length - 1)
            yield return (pos.i - 1, pos.j + 1);
    }

    public static IEnumerable<(int i1, int j1)> GetAllNeighbours(this (int i, int j) pos)
    {
        yield return (pos.i + 1, pos.j);
        yield return (pos.i - 1, pos.j);
        yield return (pos.i, pos.j + 1);
        yield return (pos.i, pos.j - 1);
        yield return (pos.i + 1, pos.j + 1);
        yield return (pos.i - 1, pos.j - 1);
        yield return (pos.i + 1, pos.j - 1);
        yield return (pos.i - 1, pos.j + 1);
    }

    public static Point Move(this Point p, int dx, int dy) => new(p.X + dx, p.Y + dy);
    public static Point Move(this Point p, (int dx, int dy) diff) => new(p.X + diff.dx, p.Y + diff.dy);
}