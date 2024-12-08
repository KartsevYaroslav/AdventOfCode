namespace AdventOfCode2024;

public class Day7 : ISolvable
{
    private readonly List<Func<long, long, long>> _funcs =
    [
        (x, y) => x * y,
        (x, y) => x + y
    ];

    public string SolvePart1(string[] input)
    {
        var map = ParseInput(input);

        return map.Where(pair => Permutation(pair.Value, pair.Key, 1, pair.Value[0]))
                  .Sum(x => x.Key)
                  .ToString();
    }

    public string SolvePart2(string[] input)
    {
        _funcs.Add((left, right) => long.Parse($"{left.ToString()}{right.ToString()}"));

        return SolvePart1(input);
    }

    private bool Permutation(List<long> nums, long key, int curIndex, long curSum)
    {
        if (curIndex == nums.Count)
            return curSum == key;
        
        if (curSum > key)
            return false;

        return _funcs.Select(func => func(curSum, nums[curIndex]))
                    .Any(newSum => Permutation(nums, key, curIndex + 1, newSum));
    }

    private static Dictionary<long, List<long>> ParseInput(string[] input)
    {
        return input.Select(x => x.Split(':'))
                    .ToDictionary(x => long.Parse(x[0]), ParseNums);

        List<long> ParseNums(string[] x) => x[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList();
    }
}