using Shared;

namespace AdventOfCode2024;

public class Day21 : ISolvable<long>
{
    public long SolvePart1(string[] input) => SolveInternal(input, 2);
    public long SolvePart2(string[] input) => SolveInternal(input, 25);

    private long SolveInternal(string[] input, int depth)
    {
        var res = 0L;
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
            var commands = GetCommands(line, keypad);

            keypad = """
                      ^A
                     <v>
                     """
                     .Split("\r\n")
                     .ToCharArray();
            var memo = new Dictionary<(string, int), long>();
            var minLength = commands.Select(command => command.Split('A'))
                                    .Select(parts => parts.Sum(part => GetMin(part, 0, depth, keypad, memo)) - 1)
                                    .Prepend(long.MaxValue)
                                    .Min();

            var index = int.Parse(new string(line.Where(char.IsDigit).ToArray()));
            res += index * minLength;
        }

        return res;
    }

    private List<string> GetCommands(string line, char[][] map)
    {
        var startCh = 'A';
        var prevParts = new List<string> {""};

        foreach (var ch in line)
        {
            prevParts = GetNextParts(startCh, ch, map)
                        .Select(x => x + 'A')
                        .SelectMany(next => prevParts.Select(prev => prev + next))
                        .ToList();
            startCh = ch;
        }

        return prevParts;
    }

    private List<string> GetNextParts(char startCh, char endCh, char[][] map)
    {
        var start = map.GetAllPoints().First(x => map.At(x) == startCh);
        var end = map.GetAllPoints().First(x => map.At(x) == endCh);
        var path = GetPath(map, start, end);
        return GetCommands(path, end, start).ToList();
    }

    private static IEnumerable<string> GetCommands(Dictionary<Point, HashSet<Point>> paths, Point curPoint, Point start)
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

    private long GetMin(string part, int depth, int maxDepth, char[][] map, Dictionary<(string part, int depth), long> dict)
    {
        if (depth == maxDepth)
            return part.Length + 1;

        if (dict.TryGetValue((part, depth), out var minPath))
            return minPath;

        var startCh = 'A';
        var res = 0L;
        foreach (var endCh in part + 'A')
        {
            res += GetNextParts(startCh, endCh, map).Select(x => GetMin(x, depth + 1, maxDepth, map, dict)).Min();
            startCh = endCh;
        }

        dict[(part, depth)] = res;
        return res;
    }
}