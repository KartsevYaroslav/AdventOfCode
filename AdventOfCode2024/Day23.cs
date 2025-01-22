using Shared;

namespace AdventOfCode2024;

public class Day23 : ISolvable<string>
{
    public string SolvePart1(string[] input)
    {
        var dict = new Dictionary<string, HashSet<string>>();

        foreach (var line in input)
        {
            var parts = line.Split('-');
            var (key, value) = (parts[0], parts[1]);
            if (!dict.ContainsKey(key))
                dict[key] = [];

            if (!dict.ContainsKey(value))
                dict[value] = [];

            dict[key].Add(value);
            dict[value].Add(key);
        }

        var unprocessed = dict.Keys.ToHashSet();
        var opened = new HashSet<string>();
        while (unprocessed.Count != 0)
        {
            var c1 = unprocessed.First();

            var queue = new Queue<(Node<string>, int)>();
            var node1 = new Node<string> {Value = c1};
            queue.Enqueue((node1, 0));
            while (queue.Count != 0)
            {
                var (node, dist) = queue.Dequeue();

                if (dist == 3)
                {
                    if (node.Value == c1 && (node.Value.StartsWith('t') || node.Next!.Value.StartsWith('t') || node.Next!.Next!.Value.StartsWith('t')))
                    {
                        var open = String.Concat(new[] {node.Value, node.Next!.Value, node.Next!.Next!.Value}.OrderBy(x => x));
                        opened.Add(open);
                    }

                    continue;
                }

                foreach (var newNode in dict[node.Value])
                {
                    var next = new Node<string>
                    {
                        Value = newNode,
                        Next = node
                    };
                    queue.Enqueue((next, dist + 1));
                }
            }

            unprocessed.Remove(c1);
        }

        return opened.Count.ToString();
    }

    public string SolvePart2(string[] input)
    {
        var dict = new Dictionary<string, HashSet<string>>();

        foreach (var line in input)
        {
            var parts = line.Split('-');
            var (key, value) = (parts[0], parts[1]);
            if (!dict.ContainsKey(key))
                dict[key] = [];

            if (!dict.ContainsKey(value))
                dict[value] = [];

            dict[key].Add(value);
            dict[value].Add(key);
        }

        var maxStr = "";
        var unprocessed = dict.Keys.ToHashSet();
        
        while (unprocessed.Count != 0)
        {
            var start = unprocessed.First();
            var visitedPaths = new HashSet<string>();
            var queue = new Queue<(string, List<string>)>();
            queue.Enqueue((start, []));
            
            while (queue.Count != 0)
            {
                var (node, curPath) = queue.Dequeue();

                var pathStr = string.Join(",", curPath.OrderBy(x => x));
                if (node == start && curPath.Count > 0)
                    maxStr = pathStr.Length > maxStr.Length ? pathStr : maxStr;
                
                if (!curPath.All(x => dict[node].Contains(x)) || !visitedPaths.Add(pathStr))
                    continue;
                
                foreach (var newNode in dict[node])
                    queue.Enqueue((newNode, curPath.Append(node).ToList()));
            }

            unprocessed.Remove(start);
        }

        return maxStr[..^1];
    }
}