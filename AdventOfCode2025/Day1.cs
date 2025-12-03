using System;

namespace AdventOfCode2025;

public class Day1 : ISolvable<long>
{
    public long SolvePart1(string[] input)
    {
        var cur = 50;
        var res = 0;
        foreach (var line in input)
        {
            var direction = line[..1];
            var turns = int.Parse(line[1..]);
            var sign = direction == "L" ? -1 : 1;
            cur = (sign * turns + cur) % 100;
            if (cur == 0)
                res++;
        }

        return res;
    }

    public long SolvePart2(string[] input)
    {
        var cur = 50;
        var res = 0;
        foreach (var line in input)
        {
            var direction = line[..1];
            var turns = int.Parse(line[1..]);
            var sign = direction == "L" ? -1 : 1;
            var i = sign * turns + cur;
            var i1 = cur * i >= 0 ? 0 : 1;
            
            cur = (i % 100 + 100) % 100;
            res += Math.Abs(i/100) + i1;
            if (i == 0)
                res++;
        }

        return res;
    }
}