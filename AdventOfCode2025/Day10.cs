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
                .Select(x => x[1..^1].Split(',').Select(int.Parse).Select(y => (long)Math.Pow(2, y)).Sum())
                .ToList();
            var goalNum = Convert.ToInt64(string.Join("", goalLights.Select(x => x == '#' ? "1" : "0").Reverse()), 2);
            res += GetMinPresses(buttons, goalNum);
        }

        return res;
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
            var buttonsIByNumI = GetIndexes(buttons, goalNums).Select(x => (x.Key, x.Value)).ToList();

            var minPresses2 = GetMinPresses(goalNums, buttonsIByNumI);
            res += minPresses2;
            Console.WriteLine($"Line {c}, result: {res}");
            c++;
        }

        return res;
    }

    private long GetMinPresses(int[] goalNums, List<(int i, List<int[]> buttons)> buttonsByNumI)
    {
        if (buttonsByNumI.Count == 0)
            return 0;

        var (numI, validButtons) = buttonsByNumI
            .Select(x => x with { buttons = x.buttons.Where(y => y.All(i => goalNums[i] > 0)).ToList() })
            .Where(x => x.buttons.Count != 0)
            .OrderBy(x => x.buttons.Count)
            .FirstOrDefault();

        if (validButtons == null)
            return -1;

        var combinations = GetPossibleCombinations(validButtons.Count, goalNums[numI]);
        // .Where(CheckCombination);

        var min = long.MaxValue;
        foreach (var combination in combinations)
        {
            FillArray(goalNums, validButtons, combination);

            if (goalNums.Any(x => x < 0))
            {
                FillArray(goalNums, validButtons, combination, true);
                continue;
            }

            var newDict = buttonsByNumI.Where(x => goalNums[x.i] > 0).ToList();
            var minPresses = GetMinPresses(goalNums, newDict);
            FillArray(goalNums, validButtons, combination, true);

            if (minPresses != -1)
                min = Math.Min(min, goalNums[numI] + minPresses);
        }


        return min == long.MaxValue ? -1 : min;

        bool CheckCombination(List<int> ints)
        {
            return validButtons.Zip(ints, (button, count) => (button, count))
                .SelectMany(x => x.button.Select(y => (numIndex: y, x.count)))
                .GroupBy(x => x.numIndex)
                .All(group => goalNums[group.Key] >= group.Sum(x => x.count));
        }
    }

    private static void FillArray(int[] goalNums, List<int[]> buttons, List<int> combination, bool backward = false)
    {
        for (var i = 0; i < buttons.Count; i++)
            foreach (var numsI in buttons[i])
            {
                goalNums[numsI] = backward
                    ? goalNums[numsI] + combination[i]
                    : goalNums[numsI] - combination[i];
            }
    }

    private IEnumerable<List<int>> GetPossibleCombinations(int buttonsCount, int remainNum)
    {
        if (buttonsCount == 0)
        {
            yield break;
        }

        if (buttonsCount == 1)
        {
            var list = new List<int> { remainNum };
            _dict[(buttonsCount, remainNum)] = [list];
            yield return list;
            yield break;
        }

        if (_dict.TryGetValue((buttonsCount, remainNum), out var combinations))
        {
            foreach (var combination in combinations)
                yield return combination;

            yield break;
        }

        _dict[(buttonsCount, remainNum)] = [];
        for (var newNum = 0; newNum <= remainNum; newNum++)
        {
            var possibleCombinations = GetPossibleCombinations(buttonsCount - 1, remainNum - newNum);

            foreach (var possibleCombination in possibleCombinations)
            {
                var newCombination = possibleCombination.Prepend(newNum).ToList();
                _dict[(buttonsCount, remainNum)].Add(newCombination);
                yield return newCombination;
            }
        }
    }

    private static Dictionary<int, List<int[]>> GetIndexes(List<int[]> buttons, int[] goalNums)
    {
        var buttonsByNumI = goalNums.Select((_, i) => i).ToDictionary(x => x, _ => new List<int[]>());
        for (var buttonI = 0; buttonI < buttons.Count; buttonI++)
        {
            foreach (var numI in buttons[buttonI])
            {
                buttonsByNumI[numI].Add(buttons[buttonI]);
            }
        }

        return buttonsByNumI;
    }

    private static long GetMinPresses(List<long> buttons, long goalNum)
    {
        var queue = new Queue<(int buttonIndex, long curNum, long presses)>();
        for (var i = 0; i < buttons.Count; i++)
        {
            queue.Enqueue((i, 0, 0));
        }

        while (true)
        {
            var (buttonIndex, curNum, presses) = queue.Dequeue();
            if (curNum == goalNum)
                return presses;

            curNum ^= buttons[buttonIndex];

            for (var i = buttonIndex; i < buttons.Count; i++)
            {
                queue.Enqueue((i, curNum, presses + 1));
            }
        }
    }
}