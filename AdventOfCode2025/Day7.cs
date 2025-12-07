using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2025;

public class Day7 : ISolvable<long>
{
    public long SolvePart1(string[] input)
    {
        var posJ = input[0].Select((x, i) => (x, i)).First(x => x.x == 'S').i;

        return GetResult(input, posJ);
    }

    private static long GetResult(string[] input, int posJ)
    {
        var beams = new HashSet<(int i, int j)> { (0, posJ) };

        var res = 0L;
        while (true)
        {
            var newBeams = new HashSet<(int, int)>();
            foreach (var beam in beams)
            {
                if (beam.i == input.Length - 1)
                    return res;
                if (input[beam.i + 1][beam.j] == '^')
                {
                    newBeams.Add((beam.i + 1, beam.j - 1));
                    newBeams.Add((beam.i + 1, beam.j + 1));
                    res++;
                    continue;
                }

                newBeams.Add((beam.i + 1, beam.j));
            }

            beams = newBeams;
        }
    }

    public long SolvePart2(string[] input)
    {
        var posJ = input[0].Select((x, i) => (x, i)).First(x => x.x == 'S').i;
        var dict = Enumerable.Range(0, input.Length - 1).ToDictionary(x => x, _ => 0L);
        dict[posJ] = 1;
        foreach (var line in input)
        {
            var delimiters = line.Select((x, i) => (x, i)).Where(x => x.x == '^').Select(x => x.i).ToHashSet();
            if (delimiters.Count == 0)
                continue;

            foreach (var key in dict.Where(pair => delimiters.Contains(pair.Key)).Select(x => x.Key))
            {
                dict[key - 1] += dict[key];
                dict[key + 1] += dict[key];
                dict[key] = 0;
            }
        }

        return dict.Select(x => x.Value).Sum();
    }
}