using Shared;

namespace AdventOfCode2024;

public class Day10 : ISolvable<string>
{
    public string SolvePart1(string[] input)
    {
        var map = ParseInput(input);
        var starts = CreateDictionary(map, () => new HashSet<(int, int)>());

        Traverse(starts, map, (x, y) =>
        {
            x.Add(y);
            return x;
        });

        return starts.Values.Sum(x => x.Count).ToString();
    }

    public string SolvePart2(string[] input)
    {
        var map = ParseInput(input);
        var starts = CreateDictionary(map, () => 0);

        Traverse(starts, map, (x, _) => x + 1);

        return starts.Values.Sum().ToString();
    }

    private static void Traverse<T>(Dictionary<(int i, int j), T> starts, int[][] map, Func<T, (int, int), T> act)
    {
        foreach (var start in starts.Keys)
        {
            var stack = new Stack<((int i, int j) curPos, (int, int) start)>();
            stack.Push((start, start));
            while (stack.Count != 0)
            {
                var (curPos, initStart) = stack.Pop();

                if (map[curPos.i][curPos.j] == 9)
                {
                    starts[initStart] = act(starts[initStart], curPos);
                    continue;
                }

                foreach (var (i1, j1) in map.GetDirectNeighbours(curPos))
                {
                    if (map[i1][j1] - map[curPos.i][curPos.j] != 1)
                        continue;

                    stack.Push(((i1, j1), initStart));
                }
            }
        }
    }

    private static Dictionary<(int i, int j), T> CreateDictionary<T>(int[][] map, Func<T> value) =>
        map.SelectMany((x, i) => x.Select((_, j) => (i, j)))
           .Where(x => map[x.i][x.j] == 0)
           .ToDictionary(x => x, _ => value());

    private static int[][] ParseInput(string[] input) => input.Select(x => x.Select(ch => ch - '0').ToArray()).ToArray();
}