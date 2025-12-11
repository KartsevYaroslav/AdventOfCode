using System.Collections.Generic;
using System.Linq;
using Shared;

namespace AdventOfCode2025;

public class Day5 : ISolvable<long>
{
    public long SolvePart1(string[] input)
    {
        var ranges = ParseRanges(input);

        return input.SkipWhile(x => !string.IsNullOrWhiteSpace(x))
            .Skip(1)
            .Select(long.Parse)
            .Count(num => ranges.Any(r => r.IsInRange(num)));
    }

    private static List<Range> ParseRanges(string[] input)
        => input.TakeWhile(x => !string.IsNullOrWhiteSpace(x))
            .Select(line => line.Split('-').Select(long.Parse).ToList())
            .Select(parsed => new Range(parsed[0], parsed[1])).ToList();

    public long SolvePart2(string[] input)
    {
        var ranges = ParseRanges(input).OrderBy(x => x.Start).ToList();

        var resultRanges = new List<Range>();
        for (var i = 0; i < ranges.Count - 1; i++)
        {
            var curRange = ranges[i];
            var nextRange = ranges[i + 1];
            
            if (nextRange.Start < curRange.Start)
                ranges[i + 1] = nextRange with { Start = curRange.Start };

            var end = nextRange.Start - 1;
            while (curRange.IsInRange(nextRange.Start))
            {
                var newRange = curRange with { End = end };
                resultRanges.Add(newRange);
                end = curRange.End;
                curRange = curRange with { Start = newRange.End + 1 };
                ranges[i + 1] = nextRange with { Start = newRange.End + 1 };
            }

            if (curRange.Start <= curRange.End)
                resultRanges.Add(curRange);
        }

        resultRanges.Add(ranges.Last());

        return resultRanges.Sum(range => range.End + 1 - range.Start);
    }
}
