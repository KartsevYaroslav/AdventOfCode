using System.Drawing;
using Shared;

namespace AdventOfCode2024;

public class Day12 : ISolvable
{
    public string SolvePart1(string[] input)
    {
        var perimeters = input.SelectMany((x, i) => x.Select((_, j) => (i, j))).ToDictionary(x => x, _ => 4);
        var regions = new List<HashSet<(int, int)>>();
        var visited = new HashSet<(int, int)>();
        var array = input.Select(x => x.ToCharArray()).ToArray();

        for (var i = 0; i < array.Length; i++)
        {
            for (var j = 0; j < array[0].Length; j++)
            {
                if (visited.Contains((i, j)))
                    continue;

                var stack = new Stack<(int i, int j)>();
                stack.Push((i, j));
                visited.Add((i, j));

                var region = new HashSet<(int, int)>();
                while (stack.Count != 0)
                {
                    var pos = stack.Pop();

                    region.Add(pos);
                    var neighbours = array.GetDirectNeighbours(pos).Where(x => !region.Contains(x) && array.At(pos) == array.At(x));
                    foreach (var neighbour in neighbours)
                    {
                        perimeters[pos]--;
                        perimeters[neighbour]--;
                        if (!visited.Contains(neighbour))
                            stack.Push(neighbour);

                        visited.Add(neighbour);
                    }
                }

                regions.Add(region);
            }
        }

        var res = 0;

        foreach (var region in regions)
        {
            res += region.Sum(x => perimeters[x]) * region.Count;
        }

        return res.ToString();
    }

    public string SolvePart2(string[] input)
    {
        var regions = new List<HashSet<(int, int)>>();
        var visited = new HashSet<(int, int)>();
        var array = input.Select(x => x.ToCharArray()).ToArray();

        for (var i = 0; i < array.Length; i++)
        {
            for (var j = 0; j < array[0].Length; j++)
            {
                if (visited.Contains((i, j)))
                    continue;

                var stack = new Stack<(int i, int j)>();
                stack.Push((i, j));
                visited.Add((i, j));

                var region = new HashSet<(int, int)>();
                while (stack.Count != 0)
                {
                    var pos = stack.Pop();

                    region.Add(pos);
                    var neighbours = array.GetDirectNeighbours(pos).Where(x => !region.Contains(x) && array.At(pos) == array.At(x));
                    foreach (var neighbour in neighbours)
                    {
                        if (!visited.Contains(neighbour))
                            stack.Push(neighbour);

                        visited.Add(neighbour);
                    }
                }

                regions.Add(region);
            }
        }

        var sides = regions.Select((x, i) => (x, i)).ToDictionary(x => x.i, _ => 0L);
        for (var i = 0; i < regions.Count; i++)
        {
            var region = regions[i];
            var start = region.OrderBy(x => x.Item1).ThenBy(x => x.Item2).First();
            var ch = array.At(start);
            start = (start.Item1 - 1, start.Item2 - 1);

            var cur = start;
            var dir = (1, 0);
            var visited2 = new HashSet<(int, int)>();
            do
            {
                (cur, dir) = GoForward(cur, dir, region, visited2);
                sides[i]++;
            } while (cur != start);

            while (true)
            {
                start = region.SelectMany(x => x.GetAllNeighbours())
                                  .Where(x => !visited2.Contains(x) && !region.Contains(x))
                                  .OrderBy(x => x.Item1)
                                  .ThenBy(x => x.Item2)
                                  .FirstOrDefault();
                if (start == default)
                    break;
                cur = start;
                dir = (1, 0);
                do
                {
                    (cur, dir) = GoForward2(cur, dir, region, visited2);
                    sides[i]++;
                } while ((cur, dir) != (start, (1,0)));
            }
        }

        var res = regions.Select((t, i) => t.Count * sides[i]).Sum();

        return res.ToString();
    }

    private static ((int i, int j), (int i, int j) newDirection) GoForward((int i, int j) cur, (int i, int j) direction, HashSet<(int, int)> region,
        HashSet<(int, int)> visited)
    {
        while (true)
        {
            visited.Add(cur);
            var forward = (cur.i + direction.i, cur.j + direction.j);

            var left = (forward.Item1 - direction.j, forward.Item2 + direction.i);
            if (!region.Contains(left) && !region.Contains(forward))
                return (forward, direction.TurnLeft());

            if (region.Contains(forward))
                return (cur, direction.TurnRight());

            cur = forward;
        }
        
        
    }
    
    private static ((int i, int j), (int i, int j) newDirection) GoForward2((int i, int j) cur, (int i, int j) direction, HashSet<(int, int)> region,
        HashSet<(int, int)> visited)
    {
        while (true)
        {
            visited.Add(cur);
            var forward = (cur.i + direction.i, cur.j + direction.j);

            var right = (forward.Item1 + direction.j, forward.Item2 - direction.i);
            if (!region.Contains(right) && !region.Contains(forward))
                return (forward, direction.TurnRight());

            if (region.Contains(forward))
                return (cur, direction.TurnLeft());

            cur = forward;
        }
        
        
    }
}