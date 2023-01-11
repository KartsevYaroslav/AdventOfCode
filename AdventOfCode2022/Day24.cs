// Copyright (c) Veeam Software Group GmbH

namespace AdventOfCode2022;

public class Day24 : ISolvable
{
    private int _minTurns = int.MaxValue;
    private readonly Dictionary<(Point, int), int> _dict = new();
    private IReadOnlyCollection<Blizzard> _minBlizzards = new List<Blizzard>();
    private Point _finish;
    private Point _start;

    public void Solve(String[] input)
    {
        var map = input.ParseToArray();
        var blizzards = GetBlizzards(map).ToList();
        _start = new Point(1, 0);
        _finish = new Point(map.Length - 2, map[0].Length - 1);
        var portals = GetPortals(map);

        var res = Dfs(blizzards, portals, map, _start, 0);
        Console.WriteLine($"Part 1: {res}");

        (_finish, _start) = (_start, _finish);
        _dict.Clear();
        _minTurns = int.MaxValue;
        res += Dfs(_minBlizzards, portals, map, _start, 0);

        (_finish, _start) = (_start, _finish);
        _dict.Clear();
        _minTurns = int.MaxValue;
        res += Dfs(_minBlizzards, portals, map, _start, 0);
        Console.WriteLine($"Part 2: {res}");
    }

    private Int32 Dfs(IReadOnlyCollection<Blizzard> blizzards, Dictionary<Point, Point> portals, Char[][] map, Point curPoint, int curTurns)
    {
        if (curTurns >= _minTurns - curPoint.DistanceTo(_finish) || curTurns > 250)
            return int.MaxValue;

        if (_dict.TryGetValue((curPoint, curTurns), out var remainTurns))
            return remainTurns;

        if (curPoint == _finish)
        {
            _minBlizzards = blizzards.ToList();
            _dict[(curPoint, curTurns)] = 0;
            _minTurns = Math.Min(_minTurns, curTurns);
            return 0;
        }

        var newBlizzards = blizzards.Select(x => Move(x, portals)).ToList();

        _dict[(curPoint, curTurns)] = GetPossibleTurns(curPoint, map, newBlizzards.Select(x => x.Point).ToHashSet())
                                      .Select(x => Dfs(newBlizzards, portals, map, x, curTurns + 1))
                                      .DefaultIfEmpty(Int32.MaxValue)
                                      .Min();

        if (_dict[(curPoint, curTurns)] != int.MaxValue)
            _dict[(curPoint, curTurns)]++;

        return _dict[(curPoint, curTurns)];
    }

    private IEnumerable<Point> GetPossibleTurns(Point point, Char[][] map, HashSet<Point> newBlizzards)
    {
        if (point.X < map.Length - 2 && !newBlizzards.Contains(point.Move(1, 0)) && point != _start)
            yield return point.Move(1, 0);

        if (point.Y < map[0].Length - 2 && !newBlizzards.Contains(point.Move(0, 1)) ||
            point.Move(0, 1) == _finish)
            yield return point.Move(0, 1);

        if (!newBlizzards.Contains(point))
            yield return point;

        if (point.X > 1 && !newBlizzards.Contains(point.Move(-1, 0)) && point != _start)
            yield return point.Move(-1, 0);

        if (point.Y > 1 && !newBlizzards.Contains(point.Move(0, -1))
            || point.Move(0, -1) == _finish)
            yield return point.Move(0, -1);
    }

    private Blizzard Move(Blizzard blizzard, Dictionary<Point, Point> portals)
    {
        var newPoint = blizzard.Direction switch
        {
            Direction.Up => blizzard.Point with {Y = blizzard.Point.Y - 1},
            Direction.Down => blizzard.Point with {Y = blizzard.Point.Y + 1},
            Direction.Left => blizzard.Point with {X = blizzard.Point.X - 1},
            Direction.Right => blizzard.Point with {X = blizzard.Point.X + 1},
            _ => throw new ArgumentOutOfRangeException()
        };

        if (portals.TryGetValue(newPoint, out var p))
            newPoint = p;

        return blizzard with {Point = newPoint};
    }

    private Dictionary<Point, Point> GetPortals(char[][] map)
    {
        var dict = new Dictionary<Point, Point>();
        for (int i = 0; i < map.Length; i++)
        {
            dict[new Point(i, 0)] = new Point(i, map[0].Length - 2);
            dict[new Point(i, map[0].Length - 1)] = new Point(i, 1);
        }

        for (int j = 0; j < map[0].Length; j++)
        {
            dict[new Point(0, j)] = new Point(map.Length - 2, j);
            dict[new Point(map.Length - 1, j)] = new Point(1, j);
        }

        return dict;
    }

    private IEnumerable<Blizzard> GetBlizzards(char[][] map)
    {
        var chars = new[] {'<', '^', '>', 'v'};
        return map.SelectMany((col, i) => col.Select((ch, j) => (ch, i, j)))
                  .Where(x => chars.Contains(x.ch))
                  .Select(x => new Blizzard(new Point(x.i, x.j), GetDirection(x.ch)));
    }

    private Direction GetDirection(Char ch)
    {
        return ch switch
        {
            '>' => Direction.Right,
            'v' => Direction.Down,
            '<' => Direction.Left,
            '^' => Direction.Up,
            _ => throw new Exception("Wrong direction")
        };
    }
}

public record Blizzard(Point Point, Direction Direction);