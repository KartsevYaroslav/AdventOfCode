namespace AdventOfCode2024;

public class Day22 : ISolvable<long>
{
    private const int GenerationsCount = 2000;

    private readonly List<Func<long, long>> _steps =
    [
        x => ((x * 64) ^ x) % 16777216,
        x => ((x / 32) ^ x) % 16777216,
        x => ((x * 2048) ^ x) % 16777216
    ];

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
        var memo = new Dictionary<int, int>();
        foreach (var num in input.Select(long.Parse))
        {
            var newNum = num;
            var diffs = new List<int>();
            var lastReminder = (int) (num % 10);
            for (var j = 0; j < GenerationsCount - 1; j++)
            {
                newNum = _steps.Aggregate(newNum, (current, step) => step(current));
                var reminder = (int) (newNum % 10);
                diffs.Add(reminder - lastReminder);
                lastReminder = reminder;
            }

            Index((int) (num % 10), diffs, memo);
        }

        return memo.Max(x => x.Value);
    }

    private static void Index(int truncated, List<int> diffs, Dictionary<int, int> dict)
    {
        var visited = new HashSet<int>();
        for (var i = 0; i < diffs.Count - 4; i++)
        {
            var i1 = i;
            var (d1, d2, d3, d4) = (diffs[i1++], diffs[i1++], diffs[i1++], diffs[i1]);
            var newNum = d1 + d2 + d3 + d4 + truncated;
            truncated += diffs[i];

            var hash = HashCode.Combine(d1, d2, d3, d4);
            if (!visited.Add(hash))
                continue;

            dict.TryAdd(hash, 0);
            dict[hash] += newNum;
        }
    }
}