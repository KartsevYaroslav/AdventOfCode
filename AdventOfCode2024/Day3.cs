using System.Text.RegularExpressions;

namespace AdventOfCode2024;

public class Day3 : ISolvable<string>
{
    public string SolvePart1(string[] input)
    {
        var pattern = @"mul\((\d+,\d+)\)";

        return input.SelectMany(x => Regex.Matches(x, pattern).Select(y => y.Groups.Values.Last()))
                    .Select(x => x.Value.Split(',').Select(long.Parse).ToList())
                    .Select(x => x[0] * x[1])
                    .Sum()
                    .ToString();
    }

    public string SolvePart2(string[] input)
    {
        var list = $"do(){string.Join("", input)}".Split("don't()")
                                                  .SelectMany(x => x.Split("do()")[1..])
                                                  .ToArray();

        return SolvePart1(list);
    }
}