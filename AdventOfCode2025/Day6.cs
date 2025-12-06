using System;
using System.Collections.Generic;
using System.Linq;
using Shared;

namespace AdventOfCode2025;

public class Day6 : ISolvable<long>
{
    public long SolvePart1(string[] input)
    {
        var list = new List<List<long>>();
        foreach (var line in input[..^1])
        {
            var longs = line
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(long.Parse)
                .ToList();

            if (list.Count < longs.Count)
                list.AddRange(longs.Select(_ => new List<long>()).ToList());

            for (var i = 0; i < longs.Count; i++)
                list[i].Add(longs[i]);
        }

        return Calculate(input.Last(), list);
    }

    public long SolvePart2(string[] input)
    {
        var problems = new List<List<long>>();
        var rotated = input[..^1].ToCharArray().Rotate();
        while (rotated.Length != 0)
        {
            var array = rotated.TakeWhile(x => x.Any(y => y != ' ')).ToArray();
            var problem = array.Select(x => long.Parse(new string(x))).ToList();
            problems.Add(problem);
            rotated = rotated.SkipWhile(x => x.Any(y => y != ' ')).Skip(1).ToArray();
        }

        return Calculate(input.Last(), problems);
    }

    private static long Calculate(string input, List<List<long>> list)
    {
        var operators = input.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
        var res = 0L;
        for (var i = 0; i < operators.Count; i++)
        {
            if (operators[i] == "+")
                res += list[i].Sum();
            if (operators[i] == "*")
                res += list[i].Aggregate((x, y) => x * y);
        }

        return res;
    }
}