using System;
using System.Collections.Generic;
using System.Linq;

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
            var buttons = list[1..^1]
                .Select(x => x[1..^1].Split(',').Select(int.Parse).ToArray())
                .ToList();
            res += GetCombinations(goalLights, buttons).First().Count;
        }

        return res;
    }

    private static IEnumerable<List<int>> GetCombinations(string goalLights, List<int[]> buttons)
    {
        var longs = buttons.Select(x => x.Select(y => (long)Math.Pow(2, y)).Sum()).ToList();
        var goalNum = Convert.ToInt64(string.Join("", goalLights.Select(x => x == '#' ? "1" : "0").Reverse()), 2);
        return GetMinCombinations(longs, goalNum);
    }

    public long SolvePart2(string[] input)
    {
        var res = 0L;
        var c = 1;
        foreach (var line in input)
        {
            var list = line.Split().ToList();
            var goalNums = list.Last()[1..^1].Split(',').Select(int.Parse).ToArray();
            var buttons = list[1..^1]
                .Select(x => x[1..^1].Split(',').Select(int.Parse).ToArray())
                .ToList();

            res += SolveFinal(goalNums, buttons, new Dictionary<string, long>());

            Console.WriteLine($"Line {c}, result: {res}");
            c++;
        }

        return res;
    }

    private static long SolveFinal(int[] goalNums, List<int[]> buttons, Dictionary<string, long> dictionary)
    {
        if (goalNums.All(y => y == 0))
            return 0;

        if (goalNums.Any(x => x < 0))
            return int.MaxValue;
        
        var key = string.Join("", goalNums);
        if (dictionary.TryGetValue(key, out var res))
            return res;
        
        dictionary[key] = int.MaxValue;
        if (goalNums.All(y => y % 2 == 0))
            dictionary[key] = Math.Min(DivideTwice(goalNums), dictionary[key]);

        var lights = string.Join("", goalNums.Select(x => x % 2 == 0 ? '.' : '#'));
        foreach (var combination in GetCombinations(lights, buttons))
        {
            var indexes = combination.SelectMany(i => buttons[i]).ToList();
            foreach (var i in indexes)
                goalNums[i]--;

            var solveFinal = DivideTwice(goalNums);
            foreach (var i in indexes)
                goalNums[i]++;

            dictionary[key] =  Math.Min(solveFinal + combination.Count, dictionary[key]);
        }

        return dictionary[key];

        long DivideTwice(int[] array)
        {
            for (var i = 0; i < array.Length; i++)
                array[i] /= 2;

            var res1 = 2 * SolveFinal(array, buttons, dictionary);

            for (var i = 0; i < array.Length; i++)
                array[i] *= 2;

            return res1;
        }
    }

    private static IEnumerable<List<int>> GetMinCombinations(List<long> buttons, long goalNum)
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