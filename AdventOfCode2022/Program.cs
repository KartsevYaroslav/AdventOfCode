namespace AdventOfCode2022;

public class Program
{
    public static void Main()
    {
        var lines = File.ReadAllLines("input.txt");
        new Day22().Solve(lines);
    }
}