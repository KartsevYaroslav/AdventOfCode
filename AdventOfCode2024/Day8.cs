using Shared;

namespace AdventOfCode2024;

public class Day8 : ISolvable<string>
{
    public string SolvePart1(string[] input)
    {
        var dict = ParseInput(input);
        var antinodes = GetAntinodes(dict, Handler);

        return antinodes.Count.ToString();

        void Handler(Func<(int, int), (int, int)> act, (int, int) pos)
        {
            if (!input.OutOfBorders(pos))
                act(pos);
        }
    }

    public string SolvePart2(string[] input)
    {
        var dict = ParseInput(input);
        var antinodes = GetAntinodes(dict, Handler);

        return antinodes.Concat(dict.SelectMany(x => x.Value)).Distinct().Count().ToString();

        void Handler(Func<(int, int), (int, int)> act, (int, int) pos)
        {
            while (!input.OutOfBorders(pos))
                pos = act(pos);
        }
    }

    private static HashSet<(int, int)> GetAntinodes(Dictionary<char, List<(int i, int j)>> dict, Action<Func<(int, int), (int, int)>, (int, int)> handler)
    {
        var antinodes = new HashSet<(int, int)>();
        foreach (var (_, positions) in dict)
        {
            for (var i = 0; i < positions.Count; i++)
            for (var j = i + 1; j < positions.Count; j++)
            {
                var left = positions[i];
                var right = positions[j];
                var diff = (i: left.i - right.i, j: left.j - right.j);
                var newPos1 = (i: left.i + diff.i, j: left.j + diff.j);
                var newPos2 = (i: right.i - diff.i, j: right.j - diff.j);
                handler(x => Add(x, diff, (l, r) => l + r), newPos1);
                handler(x => Add(x, diff, (l, r) => l - r), newPos2);
            }
        }

        return antinodes;

        (int, int) Add((int i, int j) pos, (int i, int j) diff, Func<int, int, int> func)
        {
            antinodes.Add(pos);
            return (func(pos.i, diff.i), func(pos.j, diff.j));
        }
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