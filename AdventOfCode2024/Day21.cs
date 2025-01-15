using Shared;

namespace AdventOfCode2024;

public class Day21 : ISolvable<long>
{
    public long SolvePart1(string[] input)
    {
        var res = 0;
        foreach (var line in input)
        {
            var keypad = """
                         789
                         456
                         123
                          0A
                         """
                         .Split("\r\n")
                         .ToCharArray();
            int minLength = int.MaxValue;
            var commands = GetCommands(line, keypad, ref minLength, new Dictionary<string, List<string>>());

            keypad = """
                      ^A
                     <v>
                     """
                     .Split("\r\n")
                     .ToCharArray();

            var dict = new Dictionary<string, List<string>>();
            for (var j = 0; j < 2; j++)
            {
                minLength = int.MaxValue;
                commands = commands.SelectMany(x => GetCommands(x, keypad, ref minLength, dict))
                                   .ToList()
                                   .Where(x => x.Length == minLength)
                                   .ToList();
            }

            var index = int.Parse(new string(line.Where(char.IsDigit).ToArray()));
            res += index * minLength;
        }

        return res;
    }

    private List<string> GetCommands(string line, char[][] map, ref int minLength, Dictionary<string, List<string>> dict)
    {
        var startCh = 'A';
        var prevParts = new List<string> {""};

        for (var i = 0; i < line.Length; i++)
        {
            var ch = line[i];
            var start = map.GetAllPoints().First(x => map.At(x) == startCh);
            var end = map.GetAllPoints().First(x => map.At(x) == ch);
            var path = GetPath(map, start, end);
            var nextParts = GetCommands(path, end, start).ToList();

            prevParts = nextParts.SelectMany(x => prevParts.Select(y => y + x + 'A')).ToList();
            startCh = ch;

            if (prevParts.First().Length > minLength)
                return [];
        }

        minLength = prevParts.First().Length;
        return prevParts;
    }

    private IEnumerable<string> GetCommands(Dictionary<Point, HashSet<Point>> paths, Point curPoint, Point start)
    {
        if (curPoint == start)
        {
            yield return "";
            yield break;
        }

        foreach (var prevPoint in paths[curPoint])
        {
            var dx = curPoint.X - prevPoint.X;
            var dy = curPoint.Y - prevPoint.Y;
            var command = dx switch
            {
                1 => 'v',
                -1 => '^',
                0 when dy == 1 => '>',
                0 when dy == -1 => '<',
                _ => throw new ArgumentOutOfRangeException()
            };

            foreach (var next in GetCommands(paths, prevPoint, start))
                yield return next + command;
        }
    }

    private Dictionary<Point, HashSet<Point>> GetPath(char[][] map, Point start, Point end)
    {
        var paths = new Dictionary<Point, HashSet<Point>>();
        var queue = new Queue<(Point, int)>();
        var visited = new HashSet<Point>();
        queue.Enqueue((start, 0));
        var minDistance = int.MaxValue;
        while (queue.Count != 0)
        {
            var (pos, dist) = queue.Dequeue();
            visited.Add(pos);

            if (pos == end)
            {
                if (minDistance < dist)
                    continue;

                minDistance = dist;
            }

            dist++;
            foreach (var newPos in map.GetDirectNeighbours(pos).Where(x => !visited.Contains(x) && map.At(x) != ' '))
            {
                if (!paths.ContainsKey(newPos))
                    paths[newPos] = [];

                paths[newPos].Add(pos);

                queue.Enqueue((newPos, dist));
            }
        }

        return paths;
    }
}