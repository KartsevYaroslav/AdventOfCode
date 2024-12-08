using Shared;

namespace AdventOfCode2024;

public class Day8 : ISolvable
{
    public string SolvePart1(string[] input)
    {
        var dict = ParseInput(input);
        var antinodes = GetAntinodes(input, dict);

        return antinodes.Count.ToString();
    }

    public string SolvePart2(string[] input)
    {
        var dict = ParseInput(input);
        var antinodes = new HashSet<(int, int)>();
        foreach (var (key, positions) in dict)
        {
            for (var i = 0; i < positions.Count; i++)
            for (var j = i + 1; j < positions.Count; j++)
            {
                var left = positions[i];
                var right = positions[j];
                var diff = (i: left.i - right.i, j: left.j - right.j);
                var newPos1 = (i: left.i + diff.i, j: left.j + diff.j);
                var newPos2 = (i: right.i - diff.i, j: right.j - diff.j);

                while (!input.OutOfBorders(newPos1))
                {
                    antinodes.Add(newPos1);
                    newPos1 = (newPos1.i + diff.i, newPos1.j + diff.j);
                }

                while (!input.OutOfBorders(newPos2))
                {
                    antinodes.Add(newPos2);
                    newPos2 = (newPos2.i - diff.i, newPos2.j - diff.j);
                }
            }
        }

        return antinodes.Concat(dict.SelectMany(x => x.Value)).Distinct().Count().ToString();
    }

    private static HashSet<(int, int)> GetAntinodes(string[] input, Dictionary<char, List<(int i, int j)>> dict)
    {
        var antinodes = new HashSet<(int, int)>();
        foreach (var (key, positions) in dict)
        {
            for (var i = 0; i < positions.Count; i++)
            for (var j = i + 1; j < positions.Count; j++)
            {
                var left = positions[i];
                var right = positions[j];
                var diff = (i: left.i - right.i, j: left.j - right.j);
                var newPos1 = (left.i + diff.i, left.j + diff.j);
                var newPos2 = (right.i - diff.i, right.j - diff.j);
                if (!input.OutOfBorders(newPos1))
                    antinodes.Add(newPos1);

                if (!input.OutOfBorders(newPos2))
                    antinodes.Add(newPos2);
            }
        }

        return antinodes;
    }

    private static Dictionary<char, List<(int i, int j)>> ParseInput(string[] input)
    {
        var dict = new Dictionary<char, List<(int i, int j)>>();
        for (var i = 0; i < input.Length; i++)
        for (var j = 0; j < input[0].Length; j++)
        {
            var ch = input[i][j];
            if (ch == '.')
                continue;

            if (!dict.ContainsKey(ch))
                dict[ch] = [];

            dict[ch].Add((i, j));
        }

        return dict;
    }
}