using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2025;

public class Day2 : ISolvable<long>
{
    public long SolvePart1(string[] input)
    {
        var res = 0L;
        foreach (var line in input.First().Split(","))
        {
            var range = line.Split('-').Select(long.Parse).ToList();
            res += CheckRange(range[0], range[1]);
        }

        return res;
    }

    private long CheckRange(long left, long right)
    {
        var res = 0L;
        var num = 10L;
        while (num < right)
        {
            res += CheckRangeInternal(Math.Max(left, num), Math.Min(num * 10 - 1, right));
            num *= 100;
        }

        return res;
    }

    private long CheckRangeInternal(long left, long right)
    {
        var log10 = (long)(Math.Log10(left) / 2) + 1;
        var pow = (long)Math.Pow(10, log10);
        var num = left / pow;
        var res = 0L;

        while (true)
        {
            var d = num + num * pow;
            if (d > right)
                return res;

            if (d >= left)
                res += d;

            num++;
        }
    }

    public long SolvePart2(string[] input)
    {
        var res = 0L;
        foreach (var line in input.First().Split(","))
        {
            var range = line.Split('-').Select(long.Parse).ToList();
            res += CheckRange2(range[0], range[1]);
        }

        return res;
    }

    private long CheckRange2(long left, long right)
    {
        var pow = 0;
        var digitsCountR = (int)Math.Log10(right);
        var set = new HashSet<long>();

        while (true)
        {
            var baseNum = (long)Math.Pow(10, pow);
            if (pow + 1 > (digitsCountR + 1) / 2)
                break;
            var next = baseNum * 10;
            var cur = baseNum;
            while (cur < next)
            {
                var tmp = cur;
                do
                {
                    tmp = tmp * (long)Math.Pow(10, pow + 1) + cur;
                } while (tmp < left);

                if (tmp <= right)
                    set.Add(tmp);

                cur++;
            }

            pow++;
        }

        return set.Sum();
    }
}