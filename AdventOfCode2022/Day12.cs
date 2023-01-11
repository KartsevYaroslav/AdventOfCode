public class Day12 : ISolvable
{
    public void Solve(string[] lines)
    {
        var replacedLines = lines.Select(line => line.Replace('S', 'a').Replace('E', 'z')).ToArray();

        var graph1 = ParseGraph(replacedLines, x => x <= 1);
        var res1 = GetMinTurns(lines, GetStartPoint('S'), 'E', graph1);
        Console.WriteLine($"Part 1: {res1}");

        var graph2 = ParseGraph(replacedLines, x => x >= -1);
        var res2 = GetMinTurns(lines, GetStartPoint('E'), 'a', graph2);
        Console.WriteLine($"Part 2: {res2}");

        Point GetStartPoint(char ch) => lines.SelectMany((x, i) => x.Select((ch1, j) => (ch: ch1, i, j)))
                                             .Where(x => x.ch == ch)
                                             .Select(x => new Point(x.j, x.i))
                                             .First();
    }

    private static Int32 GetMinTurns(IReadOnlyList<String> lines, Point start, Char endChar, IReadOnlyDictionary<Point, List<Point>> graph)
    {
        var queue = new Queue<(Point p, Int32 turns)>();
        queue.Enqueue((start, 0));

        var visited = new HashSet<Point>();
        while (queue.Count != 0)
        {
            var (p, turns) = queue.Dequeue();
            if (lines[p.Y][p.X] == endChar)
                return turns;

            foreach (var newP in graph[p].Where(x => !visited.Contains(x)))
            {
                visited.Add(newP);
                queue.Enqueue((newP, turns + 1));
            }
        }

        throw new Exception();
    }

    private static Dictionary<Point, List<Point>> ParseGraph(String[] lines, Func<int, bool> diffSelector)
    {
        var graph = new Dictionary<Point, List<Point>>();

        for (var i = 0; i < lines.Length; i++)
        for (var j = 0; j < lines[0].Length; j++)
        {
            var point = new Point(j, i);

            if (!graph.ContainsKey(point))
                graph[point] = new List<Point>();

            var points = GetNeighbours(lines, point).Where(IsAccessible);
            graph[point].AddRange(points);

            Boolean IsAccessible(Point newP) => diffSelector(lines[newP.Y][newP.X] - lines[point.Y][point.X]);
        }

        return graph;
    }

    private static IEnumerable<Point> GetNeighbours(String[] lines, Point point)
    {
        if (point.X > 0)
            yield return point with {X = point.X - 1};

        if (point.X < lines[0].Length - 1)
            yield return point with {X = point.X + 1};

        if (point.Y > 0)
            yield return point with {Y = point.Y - 1};

        if (point.Y < lines.Length - 1)
            yield return point with {Y = point.Y + 1};
    }
}

public interface ISolvable
{
    void Solve(string[] input);
}

public record struct Point(Int32 X, Int32 Y)
{
    public Point Move(int dx, int dy) => new(X + dx, Y + dy);
    public Point Move((int dx, int dy) diff) => new(X + diff.dx, Y + diff.dy);
}