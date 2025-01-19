namespace AdventOfCode2024;

public class Day22 : ISolvable<long>
{
    public long SolvePart1(string[] input)
    {
        const int generationsCount = 2000;
        var steps = new List<Func<long, long>>
        {
            x => ((x * 64) ^ x) % 16777216,
            x => ((x / 32) ^ x) % 16777216,
            x => ((x * 2048) ^ x) % 16777216
        };
        var res = 0L;
        foreach (var num in input.Select(long.Parse))
        {
            var newNum = num;
            for (var j = 0; j < generationsCount; j++)
                newNum = steps.Aggregate(newNum, (current, step) => step(current));

            res += newNum;
        }

        return res;
    }

    public long SolvePart2(string[] input)
    {
        const int generationsCount = 1999;
        var steps = new List<Func<long, long>>
        {
            x => ((x * 64) ^ x) % 16777216,
            x => ((x / 32) ^ x) % 16777216,
            x => ((x * 2048) ^ x) % 16777216
        };
        var diffsList = new List<List<long>>();

        var sequences = new HashSet<(long, long, long, long)>();
        var nums = input.Select(long.Parse).ToList();
        foreach (var num in nums)
        {
            var newNum = num;
            var diffs = new List<long>();
            var last = num % 10;
            for (var j = 0; j < generationsCount; j++)
            {
                newNum = steps.Aggregate(newNum, (current, step) => step(current));

                var reminder = newNum % 10;
                diffs.Add(reminder - last);
                last = reminder;
            }

            for (var i = 0; i < diffs.Count - 4; i++)
            {
                var sequence = (diffs[i], diffs[i + 1], diffs[i + 2], diffs[i + 3]);
                sequences.Add(sequence);
            }

            diffsList.Add(diffs);
        }

        var maxSum = long.MinValue;
        var sequencesList = sequences.ToList();
        
        var memo = new Dictionary<(long, (long, long, long, long)), long>();
        for (var i = 0; i < nums.Count; i++)
        {
            var diffs = diffsList[i];
            Index(nums[i], diffs, memo);
        }
        
        for (var j = 0; j < sequences.Count; j++)
        {
            var sum = nums.Sum(num => memo.GetValueOrDefault((num, sequencesList[j]), 0));
            maxSum = Math.Max(maxSum, sum);
        }

        return maxSum;
    }
    private static void Index(long num, List<long> diffs, Dictionary<(long, (long, long, long, long)), long> dict)
    {
        var truncated = num % 10;
        for (var i = 0; i < diffs.Count - 4; i++)
        {
            var i1 = i;
            var (d1, d2, d3, d4) = (diffs[i1++], diffs[i1++], diffs[i1++], diffs[i1]);
            var newNum = d1 + d2 + d3 + d4 + truncated;
            if (!dict.ContainsKey((num, (d1, d2, d3, d4))))
                dict[(num, (d1, d2, d3, d4))] = newNum;

            truncated += diffs[i];
        }
    }
}