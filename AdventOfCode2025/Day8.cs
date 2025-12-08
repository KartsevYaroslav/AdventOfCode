using System;
using System.Collections.Generic;
using System.Linq;
using Shared;

namespace AdventOfCode2025;

public class Day8 : ISolvable<long>
{
    public long SolvePart1(string[] input)
    {
        var distances = ParseDistances(input);
        var graph = new Dictionary<Point3d, List<Point3d>>();
        foreach (var (pointL, pointR) in distances.OrderBy(x => x.Value).Take(1000).Select(x => x.Key))
        {
            if (!graph.ContainsKey(pointL))
                graph[pointL] = [];
            if (!graph.ContainsKey(pointR))
                graph[pointR] = [];
            graph[pointL].Add(pointR);
            graph[pointR].Add(pointL);
            distances.Remove((pointL, pointR));
            distances.Remove((pointR, pointL));
        }

        var sizes = new List<long>();
        while (graph.Count > 0)
        {
            var visited = new HashSet<Point3d>();
            var queue = new Queue<Point3d>();
            queue.Enqueue(graph.First().Key);
            var size = 0L;
            while (queue.Any())
            {
                var point = queue.Dequeue();
                visited.Add(point);
                if (!graph.TryGetValue(point, out var neighbors))
                    continue;

                size++;

                foreach (var neighbor in neighbors.Where(x => !visited.Contains(x)))
                {
                    queue.Enqueue(neighbor);
                }

                graph.Remove(point);
            }

            sizes.Add(size);
        }

        return sizes.OrderByDescending(size => size).Take(3).Aggregate((x, y) => x * y);
    }

    public long SolvePart2(string[] input)
    {
        var distances = ParseDistances(input);
        var graph = new Dictionary<Point3d, HashSet<Point3d>>();
        foreach (var (lP, rP) in distances.Keys)
        {
            graph[lP] = [];
            graph[rP] = [];
        }

        foreach (var (l, r) in distances.OrderBy(x => x.Value).Select(x => x.Key))
        {
            distances.Remove((l, r));
            foreach (var neighbor in graph[r])
            {
                graph[l].Add(neighbor);
                graph[neighbor] = graph[l];
            }

            graph[l].Add(r);
            graph[l].Add(l);
            graph[r] = graph[l];
            if (graph[l].Count == graph.Count)
                return (long)l.X * r.X;
        }

        return 0;
    }

    private static Dictionary<(Point3d, Point3d), double> ParseDistances(string[] input)
    {
        var dict = new Dictionary<(Point3d, Point3d), double>();
        for (var i = 0; i < input.Length - 1; i++)
        {
            var coordinatesL = input[i].Split(',').Select(int.Parse).ToArray();
            var pointL = new Point3d(coordinatesL[0], coordinatesL[1], coordinatesL[2]);
            for (var j = i + 1; j < input.Length; j++)
            {
                var coordinatesR = input[j].Split(',').Select(int.Parse).ToArray();
                var pointR = new Point3d(coordinatesR[0], coordinatesR[1], coordinatesR[2]);
                var pow = Math.Pow(pointR.X - pointL.X, 2) +
                          Math.Pow(pointR.Y - pointL.Y, 2) +
                          Math.Pow(pointR.Z - pointL.Z, 2);
                dict[(pointL, pointR)] = Math.Sqrt(pow);
            }
        }

        return dict;
    }
}