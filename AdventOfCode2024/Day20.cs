using System.Diagnostics;
using Shared;

namespace AdventOfCode2024;

public class Day20 : ISolvable<int>
{
    public int SolvePart1(string[] input)
    {
        var map = input.ToCharArray();
        var start = map.GetAllPoints().First(x => map.At(x) == 'S');
        var singlePathDistances = GetDistances(map, start);

        return GetPossibleCheatsBfs(map, singlePathDistances, 2);
    }

    public int SolvePart2(string[] input)
    {
        var map = input.ToCharArray();
        var start = map.GetAllPoints().First(x => map.At(x) == 'S');
        var singlePathDistances = GetDistances(map, start);
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var res = GetPossibleCheats(singlePathDistances, 20);
        stopwatch.Stop();
        Console.WriteLine($"Without BFS: {stopwatch.ElapsedMilliseconds}");
        stopwatch.Restart();
        var res2 = GetPossibleCheatsBfs(map, singlePathDistances, 20);
        stopwatch.Stop();
        Console.WriteLine($"With BFS: {stopwatch.ElapsedMilliseconds}");

        return res;
    }

    private static int GetPossibleCheats(Dictionary<Point, int> distances, int cheatSec)
    {
        var res = 0;
        var keys = distances.Keys.ToList();
        const int savedTurns = 100;
        for (var i = 0; i < keys.Count - savedTurns; i++)
        {
            var start = keys[i];
            for (var j = i + savedTurns; j < keys.Count; j++)
            {
                var end = keys[j];
                var fairDiff = distances[end] - distances[start];

                var dX = Math.Abs(end.X - start.X);
                var dY = Math.Abs(end.Y - start.Y);
                var cheatDiff = dX + dY;

                if (cheatDiff <= cheatSec && fairDiff - cheatDiff >= savedTurns)
                    res++;
            }
        }

        return res;
    }

    private static int GetPossibleCheatsBfs(char[][] map, Dictionary<Point, int> path, int cheatSec)
    {
        var keys = path.Keys.ToList();
        var cheats = new HashSet<(Point, Point)>();
        var start = keys.First();

        var queue = new Queue<(Point pos, int dist)>();
        var visited = new HashSet<Point>();
        queue.Enqueue((start, 0));
        visited.Add(start);
        var startDist = path[start];
        while (queue.Count != 0)
        {
            var (pos, dist) = queue.Dequeue();

            if (dist++ == cheatSec)
                continue;

            foreach (var newPos in map.GetDirectNeighbours(pos).Where(x => !visited.Contains(x)))
            {
                if (path.TryGetValue(newPos, out var fairDist))
                {
                    var cheatDist = startDist + dist;
                    if (fairDist - cheatDist >= 100)
                        cheats.Add((start, newPos));
                }

                visited.Add(newPos);
                queue.Enqueue((newPos, dist));
            }
        }

        return cheats.Count;
    }

    private static Dictionary<Point, int> GetDistances(char[][] map, Point start)
    {
        var curPoint = start;
        var curDist = 0;
        var distances = new Dictionary<Point, int>
        {
            [start] = 0
        };
        while (map.At(curPoint) != 'E')
        {
            var nextPoint = map.GetDirectNeighbours(curPoint).First(x => map.At(x) != '#' && !distances.ContainsKey(x));
            distances[nextPoint] = ++curDist;
            curPoint = nextPoint;
        }

        return distances;
    }
}