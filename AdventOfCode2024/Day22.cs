namespace AdventOfCode2024;

public class Day22 : ISolvable<long>
{
    private readonly List<Func<long, long>> _steps =
    [
        x => ((x * 64) ^ x) % 16777216,
        x => ((x / 32) ^ x) % 16777216,
        x => ((x * 2048) ^ x) % 16777216
    ];

    private const int GenerationsCount = 2000;

    public long SolvePart1(string[] input)
    {
        var res = 0L;
        foreach (var num in input.Select(long.Parse))
        {
            var newNum = num;
            for (var j = 0; j < GenerationsCount; j++)
                newNum = _steps.Aggregate(newNum, (current, step) => step(current));

            res += newNum;
        }

        return res;
    }

    public long SolvePart2(string[] input)
    {
        var memo = new Dictionary<(long, long, long, long), long>();
        foreach (var num in input.Select(long.Parse))
        {
            var newNum = num;
            var diffs = new List<long>();
            var last = num % 10;
            for (var j = 0; j < GenerationsCount - 1; j++)
            {
                newNum = _steps.Aggregate(newNum, (current, step) => step(current));
                var reminder = newNum % 10;
                diffs.Add(reminder - last);
                last = reminder;
            }

            Index(num, diffs, memo);
        }

        return memo.Max(x => x.Value);
    }

    private static void Index(long num, List<long> diffs, Dictionary<(long, long, long, long), long> dict)
    {
        var truncated = num % 10;
        var visited = new HashSet<(long, long, long, long)>();
        for (var i = 0; i < diffs.Count - 4; i++)
        {
            var i1 = i;
            var (d1, d2, d3, d4) = (diffs[i1++], diffs[i1++], diffs[i1++], diffs[i1]);
            var newNum = d1 + d2 + d3 + d4 + truncated;
            truncated += diffs[i];

            if (!visited.Add((d1, d2, d3, d4)))
                continue;

            if (!dict.ContainsKey((d1, d2, d3, d4)))
                dict[(d1, d2, d3, d4)] = 0;
            dict[(d1, d2, d3, d4)] += newNum;
        }
    }
}