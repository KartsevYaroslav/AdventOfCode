namespace AdventOfCode2024;

public class Day2 : ISolvable<string>
{
    public string SolvePart1(string[] input) => ParseInput(input).Count(IsSafe).ToString();

    public string SolvePart2(string[] input)
    {
        return ParseInput(input)
               .Count(line => IsSafe(line) || AnySafe(line))
               .ToString();

        bool AnySafe(int[] line) => Enumerable.Range(0, line.Length)
                                              .Select(i => line.Take(i).Concat(line.Skip(i + 1)).ToArray())
                                              .Any(IsSafe);
    }

    private static bool IsSafe(int[] numLine)
    {
        var diffs = numLine.Zip(numLine.Skip(1), (x, y) => x - y).ToList();
        var isLinear = Enumerable.Range(1, diffs.Count - 1).All(i => diffs[i] * diffs[i - 1] > 0);

        return isLinear && diffs.All(x => Math.Abs(x) <= 3);
    }

    private static IEnumerable<int[]> ParseInput(string[] input) => input.Select(x => x.Split().Select(int.Parse).ToArray());
}