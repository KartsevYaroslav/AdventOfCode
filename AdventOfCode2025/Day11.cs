using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2025;

public class Day11 : ISolvable<long>
{
    public long SolvePart1(string[] input)
    {
        var graph = ParseGraph(input);
        var reversedGraph = ReverseGraph(graph);
        
        return GetPathsCount(graph, "you", "out", reversedGraph);
    }

    public long SolvePart2(string[] input)
    {
        var graph = ParseGraph(input);
        var reversedGraph = ReverseGraph(graph);
        var reversedGraph2 = reversedGraph.ToDictionary(x => x.Key, y => y.Value.ToList());
        
        return GetPathsCount(graph, "svr", "fft", reversedGraph)
               * GetPathsCount(graph, "fft", "dac", reversedGraph)
               * GetPathsCount(graph, "dac", "out", reversedGraph)
               + GetPathsCount(graph, "svr", "dac", reversedGraph2)
               * GetPathsCount(graph, "dac", "fft", reversedGraph2)
               * GetPathsCount(graph, "fft", "out", reversedGraph2);
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
        newGraph["svr"] = [];

        return newGraph;
    }

    private static long GetPathsCount(Dictionary<string, List<string>> graph, string start, string end,
        Dictionary<string, List<string>> reversedGraph)
    {
        var memo = graph.Keys.Concat(graph.Values.SelectMany(x => x)).Distinct().ToDictionary(x => x, _ => 0L);
        memo[start]++;
        while (true)
        {
            start = reversedGraph.FirstOrDefault(x => x.Value.Count == 0).Key;

            if (start == end || start == null)
                break;

            foreach (var neighbour in graph[start])
            {
                memo[neighbour] += memo[start];
                reversedGraph[neighbour].Remove(start);
            }

            reversedGraph.Remove(start);
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