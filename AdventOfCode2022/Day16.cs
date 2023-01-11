// Copyright (c) Veeam Software Group GmbH

using System.Text.RegularExpressions;

namespace AdventOfCode2022;

public class Day16 : ISolvable
{
    static Dictionary<string, int> weights = new();
    static Dictionary<string, List<string>> graph = new();

    private static Dictionary<(string, int), int> maxWeightByValveAtMin = new();
    private static Dictionary<(Position, int), int> maxWeightByValveAtMin2 = new();
    private int maxWeight = 0;

    public void Solve(String[] input)
    {
        foreach (var line in input)
        {
            var parts = line.Split(';');
            var match = Regex.Match(parts[0], @"Valve (.*) has flow rate=(\d*)");
            var curValve = match.Groups[1].Value;
            weights[curValve] = int.Parse(match.Groups[2].Value);

            var valves = parts[1].Replace(" tunnels lead to valves ", "").Replace(" tunnel leads to valve ", "").Split(',', StringSplitOptions.TrimEntries);
            graph[curValve] = new List<String>();
            foreach (var valve in valves)
            {
                graph[curValve].Add(valve);
            }
        }

        Traverse1("AA", 0, 0, 29, new HashSet<String>());
        var max1 = maxWeightByValveAtMin.Where(x => x.Key.Item2 == 0).Select(x => x.Value).Max();
        Console.WriteLine($"Part1: {max1}");

        Traverse2(new Position("AA", "AA"), 0, 0, 0, 25, new HashSet<String>());

        var max2 = maxWeightByValveAtMin2.Where(x => x.Key.Item2 == 0).Select(x => x.Value).Max();
        

        Console.WriteLine($"Part1: {max2}");
    }

    private void Traverse1(String curNode, Int32 curWeight, int curPressure, Int32 remainMinutes, HashSet<String> opened)
    {
        curWeight += curPressure;
        if (remainMinutes < 0 ||
            maxWeightByValveAtMin.TryGetValue((curNode, remainMinutes), out var weight) && weight > curWeight)
            return;
    
        maxWeightByValveAtMin[(curNode, remainMinutes)] = curWeight;
    
        if (remainMinutes <= 0)
        {
            maxWeight = Math.Max(curWeight, maxWeight);
            return;
        }
    
        foreach (var neighbour in graph[curNode])
        {
            Traverse1(neighbour, curWeight, curPressure, remainMinutes - 1, opened);
            if (opened.Contains(curNode) || weights[curNode] == 0)
                continue;
    
            var newWeight = curWeight + curPressure + weights[curNode];
            opened.Add(curNode);
            Traverse1(neighbour, newWeight, curPressure + weights[curNode], remainMinutes - 2, opened);
            opened.Remove(curNode);
        }
    }

    private void Traverse2(Position curNode, Int32 curWeight, int curPressure, int newPressure, Int32 remainMinutes, HashSet<String> opened)
    {
        curWeight += curPressure + newPressure;
        if (maxWeightByValveAtMin2.TryGetValue((curNode, remainMinutes), out var weight) && weight >= curWeight)
            return;

        maxWeightByValveAtMin2[(curNode, remainMinutes)] = curWeight;

        if (remainMinutes == 0)
            return;

        curPressure += newPressure;

        var neighbours = graph[curNode.A].SelectMany(a => graph[curNode.B].Select(b => new Position(a, b))).Distinct().ToList();
        foreach (var neighbour in neighbours)
            Traverse2(neighbour, curWeight, curPressure, 0, remainMinutes - 1, opened);

        if (weights[curNode.A] != 0 && !opened.Contains(curNode.A))
        {
            foreach (var p in graph[curNode.B])
            {
                opened.Add(curNode.A);
                var position = curNode with {B = p};
                Traverse2(position, curWeight, curPressure, weights[curNode.A], remainMinutes - 1, opened);
                opened.Remove(curNode.A);
            }
        }

        if (weights[curNode.B] != 0 && !opened.Contains(curNode.B))
        {
            foreach (var p in graph[curNode.A])
            {
                opened.Add(curNode.B);
                var position = curNode with {A = p};
                Traverse2(position, curWeight, curPressure, weights[curNode.B], remainMinutes - 1, opened);
                opened.Remove(curNode.B);
            }
        }

        if (curNode.A != curNode.B && weights[curNode.B] != 0 && weights[curNode.A] != 0 && !opened.Contains(curNode.A) && !opened.Contains(curNode.B))
        {
            opened.Add(curNode.A);
            opened.Add(curNode.B);
            Traverse2(curNode, curWeight, curPressure, weights[curNode.A] + weights[curNode.B], remainMinutes - 1, opened);
            opened.Remove(curNode.A);
            opened.Remove(curNode.B);
        }
    }
}

public record Position(string A, string B)
{
    public override Int32 GetHashCode()
    {
        return A.GetHashCode() + B.GetHashCode();
    }

    public virtual Boolean Equals(Position? other)
    {
        return other != null && (other.A == A && other.B == B || other.A == B && other.B == A);
    }
}