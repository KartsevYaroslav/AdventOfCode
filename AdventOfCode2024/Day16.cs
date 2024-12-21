using Shared;

namespace AdventOfCode2024;

public class Day16 : ISolvable<int>
{
    public int SolvePart1(string[] input)
    {
        var map = input.ToCharArray();

        var start = map.GetAllPoints().First(x => map.At(x) == 'S');
        var end = map.GetAllPoints().First(x => map.At(x) == 'E');
        var distances = map.GetAllPoints().Where(x => map.At(x) == '.').ToDictionary(x => x, _ => int.MaxValue);
        distances[end] = int.MaxValue;
        distances[start] = 0;
        var queue = new Queue<(Point, int, (int dx, int dy) dir)>();
        queue.Enqueue((start, 0, Direction.East));

        while (queue.Count != 0)
        {
            var (point, distance, dir) = queue.Dequeue();

            foreach (var neighbour in map.GetDirectNeighbours(point).Where(x => distances.ContainsKey(x)))
            {
                var newDist = 0;
                var newDir = dir;
                if (point.Move(dir) == neighbour)
                    newDist = distance + 1;

                if (point.Move(dir.TurnLeft()) == neighbour || point.Move(dir.TurnRight()) == neighbour)
                {
                    newDist = distance + 1001;
                    newDir = point.Move(dir.TurnLeft()) == neighbour ? dir.TurnLeft() : dir.TurnRight();
                }

                if (point.Move(dir.TurnLeft().TurnLeft()) == neighbour)
                    continue;

                if (distances[neighbour] <= newDist)
                    continue;

                distances[neighbour] = newDist;
                queue.Enqueue((neighbour, newDist, newDir));
            }
        }

        return distances[end];
    }

    public int SolvePart2(string[] input)
    {
        var map = input.ToCharArray();

        var start = map.GetAllPoints().First(x => map.At(x) == 'S');
        var end = map.GetAllPoints().First(x => map.At(x) == 'E');

        var distances = ParseGraph(map, end, start);
        var paths = TraverseGraph(start, end, distances, map);
        var successPaths = GetSuccessPaths(end, start, paths);

        return successPaths.Select(x => x.Point).Distinct().Count();
    }

    private static HashSet<Position> GetSuccessPaths(Point end, Point start, Dictionary<Position, HashSet<Position>> paths)
    {
        var points = new Queue<Position>();
        points.Enqueue(new Position(end, Direction.North));
        var visited = new HashSet<Position> {new(end, Direction.North)};
        while (points.Count != 0)
        {
            var position = points.Dequeue();
            if (position.Point == start)
                break;

            var newPoints = paths[position];
            foreach (var newPoint in newPoints.Where(x => !visited.Contains(x)))
            {
                visited.Add(newPoint);
                points.Enqueue(newPoint);
            }
        }

        return visited;
    }

    private static Dictionary<Position, int> ParseGraph(char[][] map, Point end, Point start)
    {
        var distances = map.GetAllPoints()
                           .Where(x => map.At(x) == '.')
                           .SelectMany(x => Direction.AllDirect.Select(y => new Position(x, y)))
                           .ToDictionary(x => x, _ => int.MaxValue);

        foreach (var dir in Direction.AllDirect)
        {
            distances[new Position(end, dir)] = int.MaxValue;
            distances[new Position(start, dir)] = int.MaxValue;
        }

        distances[new Position(start, Direction.East)] = 0;

        return distances;
    }

    private static Dictionary<Position, HashSet<Position>> TraverseGraph(Point start, Point end, Dictionary<Position, int> distances, char[][] map)
    {
        var queue = new Queue<Position>();
        queue.Enqueue(new Position(start, Direction.East));
        var paths = new Dictionary<Position, HashSet<Position>>();
        while (queue.Count != 0)
        {
            var position = queue.Dequeue();
            var distance = distances[position];
            if (end == position.Point)
                continue;

            foreach (var neighbour in map.GetDirectNeighbours(position.Point).Where(x => map.At(x) is '.' or 'E'))
            {
                var (newPos, newDist) = neighbour switch
                {
                    _ when neighbour == position.TurnBack().Move().Point  => (position.TurnBack(), distance + 2000),
                    _ when neighbour == position.TurnLeft().Move().Point  => (position.TurnLeft(), distance + 1000),
                    _ when neighbour == position.TurnRight().Move().Point  => (position.TurnRight(), distance + 1000),
                    _ when neighbour == position.Move().Point  => (position.Move(), distance + 1),
                    _ => throw new ArgumentOutOfRangeException()
                };

                if (distances[newPos] < newDist)
                    continue;

                if (!paths.ContainsKey(newPos))
                    paths[newPos] = [];

                if (distances[newPos] > newDist)
                    paths[newPos].Clear();

                paths[newPos].Add(position);

                distances[newPos] = newDist;
                queue.Enqueue(newPos);
            }
        }

        return paths;
    }
}

public readonly record struct Position(Point Point, (int dx, int dy) Direction)
{
    public Position Move() => this with {Point = Point.Move(Direction)};
    public Position TurnBack() => this with {Direction = Direction.TurnBack()};
    public Position TurnRight() => this with {Direction = Direction.TurnRight()};
    public Position TurnLeft() => this with {Direction = Direction.TurnLeft()};
}