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
}