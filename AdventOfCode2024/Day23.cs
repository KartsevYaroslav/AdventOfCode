namespace AdventOfCode2024;

public class Day23 : ISolvable<string>
{
    public string SolvePart1(string[] input)
    {
        var hashSet = new HashSet<string>();
        SolveInternal(input, path =>
        {
            if (path.Count == 3 && path.Any(x => x.StartsWith('t')))
                hashSet.Add(string.Concat(path.OrderBy(x => x)));
        }, 3);

        return hashSet.Count.ToString();
    }

    public string SolvePart2(string[] input)
    {
        var maxStr = "";

        SolveInternal(input, path =>
        {
            var pathStr = string.Join(",", path.OrderBy(x => x));
            maxStr = pathStr.Length > maxStr.Length ? pathStr : maxStr;
        }, int.MaxValue);

        return maxStr;
    }

    private static void SolveInternal(string[] input, Action<List<string>> act, int maxLength)
    {
        var dict = ParseInput(input);
        var unprocessed = dict.Keys.ToHashSet();

        while (unprocessed.Count != 0)
        {
            var start = unprocessed.First();
            var visited = new HashSet<string>();
            var queue = new Stack<(string, List<string>)>();
            queue.Push((start, []));

            while (queue.Count != 0)
            {
                var (node, curPath) = queue.Pop();
                if (maxLength == int.MaxValue && node != start)
                    visited.Add(node);

                if (node == start && curPath.Count > 0)
                    act(curPath);

                if (curPath.Count >= maxLength || !curPath.All(x => dict[node].Contains(x)))
                    continue;

                foreach (var newNode in dict[node].Where(x => !visited.Contains(x)))
                    queue.Push((newNode, curPath.Append(node).ToList()));
            }

            unprocessed.Remove(start);
        }
    }

    private static Dictionary<string, HashSet<string>> ParseInput(string[] input)
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

        return dict;
    }
}