using Shared;

namespace AdventOfCode2024;

public class Day6 : ISolvable<string>
{
    private readonly Dictionary<char, (int i1, int j1)> _directions =
        new()
        {
            {'>', (0, 1)},
            {'v', (1, 0)},
            {'<', (0, -1)},
            {'^', (-1, 0)}
        };

    private readonly Dictionary<char, char> _nextDirections =
        new()
        {
            {'>', 'v'},
            {'v', '<'},
            {'<', '^'},
            {'^', '>'}
        };

    public string SolvePart1(string[] input)
    {
        var (ch, i, j) = GetStartPoint(input);

        return GetVisitedCount(input.Select(x => x.ToCharArray()).ToArray(), (ch, i, j)).ToString();
    }

    public string SolvePart2(string[] input)
    {
        var input1 = input.Select(x => x.ToCharArray()).ToArray();
        var pos = GetStartPoint(input);

        var res = 0;
        for (var k = 0; k < input1.Length; k++)
        for (var l = 0; l < input1[0].Length; l++)
        {
            if (input1[k][l] != '.')
                continue;

            input1[k][l] = '#';
            if (GetVisitedCount(input1, pos) == -1)
                res++;

            input1[k][l] = '.';
        }

        return res.ToString();
    }

    private (char c, int i, int j) GetStartPoint(string[] input) =>
        input.SelectMany((x, i) => x.Select((c, j) => (c, i, j))).First(x => _directions.ContainsKey(x.c));

    private int GetVisitedCount(char[][] input, (char ch, int i, int j) pos)
    {
        var visited = new HashSet<(int, int)>();
        var corners = new HashSet<(char, int, int)>();

        while (true)
        {
            var newPos = pos;
            while (input[newPos.i][newPos.j] != '#')
            {
                pos = newPos;
                visited.Add((pos.i, pos.j));
                newPos = (pos.ch, i: pos.i + _directions[pos.ch].i1, j: pos.j + _directions[pos.ch].j1);
                if (input.OutOfBorders((newPos.i, newPos.j)))
                    return visited.Count;
            }

            if (!corners.Add(pos))
                return -1;

            pos.ch = _nextDirections[pos.ch];
        }
    }
}