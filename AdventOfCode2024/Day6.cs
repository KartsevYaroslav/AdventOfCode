using Shared;

namespace AdventOfCode2024;

public class Day6 : ISolvable
{
    public string SolvePart1(string[] input)
    {
        var nextDirs = GetNextDirs();
        var directions = GetDirections();
        var (ch, i, j) = GetStartPoint(input, directions);

        return GetVisitedCount(input.Select(x => x.ToCharArray()).ToArray(), (ch, i, j), directions, nextDirs).ToString();
    }

    public string SolvePart2(string[] input)
    {
        var input1 = input.Select(x => x.ToCharArray()).ToArray();
        var nextDirs = GetNextDirs();
        var directions = GetDirections();
        var pos = GetStartPoint(input, directions);

        var res = 0;
        for (var k = 0; k < input1.Length; k++)
        for (var l = 0; l < input1[0].Length; l++)
        {
            if (input1[k][l] != '.')
                continue;

            input1[k][l] = '#';
            if (GetVisitedCount(input1, pos, directions, nextDirs) == -1)
                res++;

            input1[k][l] = '.';
        }

        return res.ToString();
    }

    private static (char c, int i, int j) GetStartPoint(string[] input, Dictionary<char, (int i1, int j1)> directions)
    {
        return input.SelectMany((x, i) => x.Select((c, j) => (c, i, j))).First(x => directions.ContainsKey(x.c));
    }

    private static Dictionary<char, (int i1, int j1)> GetDirections() =>
        new()
        {
            {'>', (0, 1)},
            {'v', (1, 0)},
            {'<', (0, -1)},
            {'^', (-1, 0)}
        };

    private static Dictionary<char, char> GetNextDirs() =>
        new()
        {
            {'>', 'v'},
            {'v', '<'},
            {'<', '^'},
            {'^', '>'}
        };

    private static int GetVisitedCount(char[][] input, (char ch, int i, int j) pos, Dictionary<char, (int i1, int j1)> directions,
        Dictionary<char, char> nextDirs)
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
                newPos = (pos.ch, i: pos.i + directions[pos.ch].i1, j: pos.j + directions[pos.ch].j1);
                if (input.OutOfBorders((newPos.i, newPos.j)))
                    return visited.Count;
            }

            if (!corners.Add(pos))
                return -1;

            pos.ch = nextDirs[pos.ch];
        }
    }
}