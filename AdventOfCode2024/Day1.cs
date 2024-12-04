namespace AdventOfCode2024;

public class Day1 : ISolvable
{
    public string SolvePart1(string[] input)
    {
        var (left, right) = ParseInput(input);

        Array.Sort(left);
        Array.Sort(right);

        var sum = left.Select((x, i) => Math.Abs(x - right[i])).Sum();

        return sum.ToString();
    }

    public string SolvePart2(string[] input)
    {
        var (left, right) = ParseInput(input);

        var rightMap = new Dictionary<int, int>();
        foreach (var num in right)
        {
            rightMap.TryAdd(num, 0);
            rightMap[num]++;
        }

        var sum = left.Where(rightMap.ContainsKey).Sum(x => x * rightMap[x]);

        return sum.ToString();
    }

    private static (int[] left, int[] right) ParseInput(string[] input)
    {
        var left = new List<int>();
        var right = new List<int>();
        foreach (var line in input)
        {
            var strings = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            left.Add(int.Parse(strings[0]));
            right.Add(int.Parse(strings[1]));
        }

        return (left.ToArray(), right.ToArray());
    }
}