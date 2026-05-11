// Copyright (c) Veeam Software Group GmbH

using Shared;

namespace AdventOfCode2023;

public class Day10 : ISolvable<long>
{
    public long SolvePart1(string[] input)
    {
        var graph = GetGraph(input);
        var start = input.SelectMany((x, i) => x.Select((y, j) => (i, j))).Single(x => input[x.i][x.j] == 'S');

        graph[start] = [(start.i + 1, start.j), (start.i - 1, start.j)];
        var turns = new Dictionary<Point, long>();
        var queue = new Queue<(Point, long)>();
        queue.Enqueue((start, 0));
        var visited = new HashSet<Point>{start};
        while (queue.Count > 0)
        {
            var (point, curTurns) = queue.Dequeue();
            turns.Add(point, curTurns);
            foreach (var neighbour in graph[point].Where(x => !input.OutOfBorders(x) && !visited.Contains(x)))
            {
                queue.Enqueue((neighbour, curTurns + 1));
                visited.Add(neighbour);
            }
        }

        return turns.Max(x => x.Value);
    }

    private Dictionary<Point, List<Point>> GetGraph(string[] input)
    {
        var graph = new Dictionary<Point, List<Point>>();
        for (int i = 0; i < input.Length; i++)
        for (int j = 0; j < input[0].Length; j++)
        {
            graph[(i, j)] = input[i][j] switch
            {
                // 'S' => [(i + 1, j), (i - 1, j), (i, j + 1), (i, j - 1)],
                'F' => [(i + 1, j), (i, j + 1)],
                '7' => [(i + 1, j), (i, j - 1)],
                'L' => [(i - 1, j), (i, j + 1)],
                'J' => [(i - 1, j), (i, j - 1)],
                '|' => [(i - 1, j), (i + 1, j)],
                '-' => [(i, j - 1), (i, j + 1)],
                _ => []
            };
        }

        return graph;
    }
}
