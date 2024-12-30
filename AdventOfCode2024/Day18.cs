using Shared;

namespace AdventOfCode2024;

public class Day18 : ISolvable<string>
{
    private const int Size = 70;

    public string SolvePart1(string[] input)
    {
        var fallingPoints = ParsePoints(input);
        var map = GenerateMap();

        foreach (var point in fallingPoints.Take(1024))
            map.Set(point, '#');

        return GetDistance(map).ToString();
    }

    public string SolvePart2(string[] input)
    {
        var fallingPoints = ParsePoints(input).ToList();
        var map = GenerateMap();

        var left = 0;
        var right = fallingPoints.Count - 1;

        while (left < right)
        {
            var mid = (left + right) / 2;
            for (var i = 0; i <= mid; i++)
                map.Set(fallingPoints[i], '#');

            if (GetDistance(map) == -1)
                right = mid;
            else
                left = mid + 1;

            for (var i = 0; i <= mid; i++)
                map.Set(fallingPoints[i], '.');
        }

        return $"{fallingPoints[left].Y},{fallingPoints[left].X}";
    }

    private static char[][] GenerateMap() => Enumerable.Range(0, Size + 1).Select(_ => Enumerable.Range(0, Size + 1).Select(_ => '.').ToArray()).ToArray();

    private static IEnumerable<Point> ParsePoints(string[] input)
        => input.Select(x => x.Split(',').Select(int.Parse).ToList()).Select(x => new Point(x[1], x[0]));

    private static int GetDistance(char[][] map)
    {
        var start = (0, 0);
        var end = (Size, Size);
        var distances = map.SelectMany((x, i) => x.Select((_, j) => new Point(i, j))).ToDictionary(x => x, _ => int.MaxValue);

        var queue = new Queue<Point>();
        queue.Enqueue(start);
        distances[start] = 0;

        while (queue.Count != 0)
        {
            var pos = queue.Dequeue();
            var distance = distances[pos];
            if (pos == end)
                return distance;

            foreach (var newPos in map.GetDirectNeighbours(pos).Where(x => map.At(x) != '#'))
            {
                var newDist = distance + 1;
                if (distances[newPos] <= newDist)
                    continue;

                distances[newPos] = newDist;
                queue.Enqueue(newPos);
            }
        }

        return -1;
    }
}