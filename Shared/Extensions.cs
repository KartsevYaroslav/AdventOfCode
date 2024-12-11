namespace Shared;

public static class Extensions
{
    public static bool OutOfBorders<T>(this T[][] input, (int i, int j) pos)
        => pos.i < 0 || pos.j < 0 || pos.i >= input.Length || pos.j >= input[0].Length;

    public static bool OutOfBorders(this string[] input, (int i, int j) pos)
        => pos.i < 0 || pos.j < 0 || pos.i >= input.Length || pos.j >= input[0].Length;

    public static void Print(this char[][] input)
    {
        foreach (var line in input)
        {
            Console.WriteLine(new string(line));
        }
    }
    
    public static IEnumerable<(int i1, int j1)> GetDirectNeighbours<T>(this T[][] input, (int i, int j) pos)
    {
        if (pos.i < input.Length - 1)
            yield return (pos.i + 1, pos.j);
        if (pos.i > 0)
            yield return (pos.i - 1, pos.j);
        if (pos.j < input[0].Length - 1)
            yield return (pos.i, pos.j + 1);
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

}