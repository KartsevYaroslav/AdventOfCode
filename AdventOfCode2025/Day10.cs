using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdventOfCode2025;

public class Day10 : ISolvable<long>
{
    private readonly Dictionary<(int, int), List<List<int>>> _dict = new();

    public long SolvePart1(string[] input)
    {
        var res = 0L;
        foreach (var line in input)
        {
            var list = line.Split().ToList();
            var goalLights = list[0][1..^1];
            var buttons = ParseButtons(list);
            res += GetCombinations(goalLights, buttons).First().Count;
        }

        return res;
    }

    public async Task<long> SolvePart2Async(string[] input)
    {
        var tasks = new List<Task<long>>();
        foreach (var line in input)
        {
            var task = Task.Run(() =>
            {
                var list = line.Split().ToList();
                var goalNums = list.Last()[1..^1].Split(',').Select(int.Parse).ToArray();
                var buttons = ParseButtons(list);

                return SolveFinal(goalNums, buttons, new Dictionary<string, long>(), new Dictionary<string, List<List<int>>>());
            });
            tasks.Add(task);
        }

        var all = await Task.WhenAll(tasks);
        return all.Sum();
    }

    private static List<int[]> ParseButtons(List<string> list)
    {
        return list[1..^1]
            .Select(x => x[1..^1].Split(',').Select(int.Parse).ToArray())
            .ToList();
    }

    private static IEnumerable<List<int>> GetCombinations(string goalLights, List<int[]> buttons)
    {
        var longs = buttons.Select(x => x.Select(y => (long)Math.Pow(2, y)).Sum()).ToList();
        var goalNum = Convert.ToInt64(string.Join("", goalLights.Select(x => x == '#' ? "1" : "0").Reverse()), 2);
        return GetCombinations(longs, goalNum);
    }

    private static long SolveFinal(int[] goalNums, List<int[]> buttons, Dictionary<string, long> memo, Dictionary<string, List<List<int>>> memo2)
    {
        if (goalNums.All(y => y == 0))
            return 0;

        if (goalNums.Any(x => x < 0))
            return int.MaxValue;

        var key = string.Join("", goalNums);
        if (memo.TryGetValue(key, out var res))
            return res;

        memo[key] = int.MaxValue;
        if (goalNums.All(y => y % 2 == 0))
            memo[key] = Math.Min(DivideTwice(goalNums), memo[key]);

        var lights = string.Join("", goalNums.Select(x => x % 2 == 0 ? '.' : '#'));
        List<List<int>> combinations;
        if (!memo2.TryGetValue(lights, out combinations))
        {
            combinations = GetCombinations(lights, buttons).ToList();
            memo2.Add(lights, combinations);
        }
        
        foreach (var combination in combinations)
        {
            var indexes = combination.SelectMany(i => buttons[i]).ToList();
            foreach (var i in indexes)
                goalNums[i]--;

            var result = DivideTwice(goalNums);
            foreach (var i in indexes)
                goalNums[i]++;

            memo[key] = Math.Min(result + combination.Count, memo[key]);
        }

        return memo[key];

        long DivideTwice(int[] array)
        {
            for (var i = 0; i < array.Length; i++)
                array[i] /= 2;

            var res1 = 2 * SolveFinal(array, buttons, memo, memo2);

            for (var i = 0; i < array.Length; i++)
                array[i] *= 2;

            return res1;
        }
    }

    private static IEnumerable<List<int>> GetCombinations(List<long> buttons, long goalNum)
    {
        var queue = new Queue<(int buttonIndex, long curNum, List<int> buttons)>();
        for (var i = 0; i < buttons.Count; i++)
        {
            queue.Enqueue((i, 0, []));
        }

        while (queue.Count > 0)
        {
            var (buttonIndex, curNum, presses) = queue.Dequeue();

            curNum ^= buttons[buttonIndex];
            presses.Add(buttonIndex);

            if (presses.Count > buttons.Count)
                continue;

            if (curNum == goalNum)
                yield return presses;

            for (var i = buttonIndex + 1; i < buttons.Count; i++)
                queue.Enqueue((i, curNum, presses.ToList()));
        }
    }
}