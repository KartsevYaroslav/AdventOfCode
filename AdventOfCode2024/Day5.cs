namespace AdventOfCode2024;

public class Day5 : ISolvable<string>
{
    public string SolvePart1(string[] input)
    {
        var map = ParseInput(input);

        var res = input.SkipWhile(x => !string.IsNullOrEmpty(x))
                       .Skip(1)
                       .Select(line => line.Split(',').Select(int.Parse).ToList())
                       .Where(nums => IsValid(nums, map))
                       .Sum(nums => nums[nums.Count / 2]);

        return res.ToString();
    }

    public string SolvePart2(string[] input)
    {
        var map = ParseInput(input);
        var myComparer = new MyComparer(map);

        var res = input.SkipWhile(x => !string.IsNullOrEmpty(x))
                       .Skip(1)
                       .Select(line => line.Split(',').Select(int.Parse).ToList())
                       .Where(nums => !IsValid(nums, map))
                       .Select(x => x.Order(myComparer).ToList())
                       .Sum(nums => nums[nums.Count / 2]);

        return res.ToString();
    }

    private static bool IsValid(List<int> nums, Dictionary<int, HashSet<int>> map)
    {
        for (var i = 0; i < nums.Count; i++)
        for (var j = i + 1; j < nums.Count; j++)
        {
            if (map.TryGetValue(nums[i], out var before) && before.Contains(nums[j]))
                return false;
        }

        return true;
    }

    private static Dictionary<int, HashSet<int>> ParseInput(string[] input)
    {
        var dict = new Dictionary<int, HashSet<int>>();
        foreach (var line in input.TakeWhile(x => !string.IsNullOrEmpty(x)))
        {
            var list = line.Split('|').Select(int.Parse).ToList();

            if (!dict.ContainsKey(list[1]))
                dict[list[1]] = [];

            dict[list[1]].Add(list[0]);
        }

        return dict;
    }
}

public class MyComparer(Dictionary<int, HashSet<int>> dictionary) : IComparer<int>
{
    public int Compare(int x, int y)
    {
        if (dictionary.TryGetValue(y, out var val) && val.Contains(x))
            return -1;
        if (dictionary.TryGetValue(x, out val) && val.Contains(y))
            return 1;

        return 0;
    }
}