using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2025;

public class Day11 : ISolvable<long>
{
    public long SolvePart1(string[] input)
    {
        var graph = ParseGraph(input);
        var queue = new Queue<string>();
        queue.Enqueue("you");
        var res = 0L;
        while (queue.Count > 0)
        {
            var curNode = queue.Dequeue();
            if (curNode == "out")
            {
                res++;
                continue;
            }

            if (curNode == "out")
                continue;

            foreach (var neighbour in graph[curNode])
                queue.Enqueue(neighbour);
        }

        return res;
    }

    public long SolvePart2(string[] input)
    {
        var graph = ParseGraph(input);
        var reversedGraph = ReverseGraph(graph);
        return GetPathsCount(graph, "svr", "fft", reversedGraph)
               * GetPathsCount(graph, "fft", "dac", reversedGraph)
               * GetPathsCount(graph, "dac", "out", reversedGraph)
               + GetPathsCount(graph, "svr", "dac", reversedGraph)
               * GetPathsCount(graph, "dac", "fft", reversedGraph)
               * GetPathsCount(graph, "fft", "out", reversedGraph);
    }

    private static Dictionary<string, List<string>> ReverseGraph(Dictionary<string, List<string>> graph)
    {
        var newGraph = new Dictionary<string, List<string>>();
        foreach (var (key, value) in graph)
        {
            foreach (var neighbour in value)
            {
                if (!newGraph.ContainsKey(neighbour))
                    newGraph[neighbour] = [];
                newGraph[neighbour].Add(key);
            }
        }

        return newGraph;
    }

    private static long GetPathsCount(Dictionary<string, List<string>> graph, string start, string end,
        Dictionary<string, List<string>> reversedGraph)
    {
        graph = graph.ToDictionary(x => x.Key, y => y.Value);
        reversedGraph = reversedGraph.ToDictionary(x => x.Key, y => y.Value);
        reversedGraph[start] = [];
        var memo = graph.Keys.Concat(graph.Values.SelectMany(x => x)).Distinct().ToDictionary(x => x, _ => 0L);
        memo[start]++;
        while (true)
        {
            start = reversedGraph.FirstOrDefault(x => x.Value.Count == 0).Key;

            if (start == end)
                break;

            foreach (var neighbour in graph[start].ToList())
            {
                memo[neighbour] += memo[start];
                reversedGraph[neighbour].Remove(start);
                graph[start].Remove(neighbour);
            }

            reversedGraph.Remove(start);
            graph.Remove(start);
        }

        return memo.GetValueOrDefault(end, 0);
    }

    private static Dictionary<string, List<string>> ParseGraph(string[] input)
    {
        var graph = new Dictionary<string, List<string>>();
        foreach (var line in input)
        {
            var nodes = line.Split();
            var key = nodes[0][..^1];
            graph[key] = [];
            foreach (var node in nodes[1..])
            {
                graph[key].Add(node);
            }
        }

        graph["out"] = [];

        return graph;
    }
}