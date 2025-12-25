using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2025;

public class Day12 : ISolvable<long>
{
    public long SolvePart1(string[] input)
    {
        var shapes = new List<int>();
        while (!input[0].Contains('x'))
        {
            var shapeLines = input.Skip(1).Take(3);
            shapes.Add(shapeLines.SelectMany(x => x).Count(x => x == '#'));
            input = input.Skip(5).ToArray();
        }

        var res = 0L;
        foreach (var line in input)
        {
            var strings = line.Split(':');
            var size = strings[0].Split('x').Select(int.Parse).ToList();
            var counts = strings[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();

            var sum = counts.Select((x, i) => shapes[i] * x).Sum();
            var area = size[0] * size[1];
            if (area > sum)
                res++;
        }

        return res;
    }
}