using Shared;

namespace AdventOfCode2024;

public class Day20 : ISolvable<int>
{
    public int SolvePart1(string[] input) => GetCheats(input, 2);

    public int SolvePart2(string[] input) => GetCheats(input, 20);

    private static int GetCheats(string[] input, int cheatSec)
    {
        var map = input.ToCharArray();
        var start = map.GetAllPoints().First(x => map.At(x) == 'S');
        var singlePathDistances = GetDistances(map, start);

        return GetPossibleCheats(singlePathDistances, cheatSec);
    }

    private static int GetPossibleCheats(Dictionary<Point, int> basePath, int cheatSec)
    {
        var res = 0;
        var keys = basePath.Keys.ToList();
        const int savedTurns = 100;
        for (var i = 0; i < keys.Count - savedTurns; i++)
        {
            var (x, y) = keys[i];
            var visited = new HashSet<Point>();

            for (var cheatDist = 2; cheatDist <= cheatSec; cheatDist++)
            {
                for (var d = 0; d <= cheatDist; d++)
                {
                    Check((x + d, y + cheatDist - d));
                    Check((x - d, y + cheatDist - d));
                    Check((x + d, y - cheatDist + d));
                    Check((x - d, y - cheatDist + d));
                }

                void Check(Point otherPos)
                {
                    if (visited.Add(otherPos)
                        && basePath.TryGetValue(otherPos, out var distToStart)
                        && distToStart - basePath[(x, y)] - cheatDist >= savedTurns)
                        res++;
                }
            }
        }

        return res;
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