// Copyright (c) Veeam Software Group GmbH

namespace AdventOfCode2022;

public class Day23 : ISolvable
{
    public void Solve(String[] input)
    {
        var map = input.ParseToArray();

        StartTurns(map, 10, 10);
        var res = GetFreePointsCount(map);

        Console.WriteLine($"Part 1: {res}");

        var round = StartTurns(map, int.MaxValue, 100);

        Console.WriteLine($"Part 2: {round}");
    }

    private Int32 StartTurns(Char[][] map, Int32 rounds, Int32 extendCount)
    {
        map = map.ExtendBy(extendCount, '.');
        var elvesPositions = GetElvesPositions(map).ToList();
        var compassDirections = new[] {CompassDirection.North, CompassDirection.South, CompassDirection.West, CompassDirection.East};

        for (int i = 0; i < rounds; i++)
        {
            var possiblePositions = PlanTurn(elvesPositions, map, compassDirections);

            var newPositions = possiblePositions.GroupBy(x => x.newP).Where(x => x.Count() == 1).SelectMany(x => x).ToList();

            if (newPositions.Count == 0)
                return i + 1;

            Move(newPositions, map);
            elvesPositions = elvesPositions.Except(newPositions.Select(x => x.oldP)).Concat(newPositions.Select(x => x.newP)).ToList();
            compassDirections = compassDirections.Append(compassDirections[0]).Skip(1).ToArray();

            if ((i + 1) % extendCount != 0)
                continue;

            map = map.ExtendBy(extendCount, '.');
            elvesPositions = elvesPositions.Select(x => x.Move(extendCount, extendCount)).ToList();
        }

        return rounds;
    }

    private Int32 GetFreePointsCount(Char[][] map)
    {
        var count = 0;
        var elvesPositions = map.SelectMany((x, X) => x.Select((p, Y) => (p, X, Y))).ToList();
        var minX = elvesPositions.Select(x => x.X).Min();
        var maxX = elvesPositions.Select(x => x.X).Max();
        var minY = elvesPositions.Select(x => x.Y).Min();
        var maxY = elvesPositions.Select(x => x.Y).Max();

        for (int x = minX; x <= maxX; x++)
        for (int y = minY; y <= maxY; y++)
        {
            if (map[x][y] == '.')
                count++;
        }

        return count;
    }

    private void Move(IReadOnlyCollection<(Point oldP, Point newP)> newPositions, char[][] map)
    {
        foreach (var (oldP, newP) in newPositions)
        {
            map[oldP.X][oldP.Y] = '.';
            map[newP.X][newP.Y] = '#';
        }
    }

    private IEnumerable<(Point oldP, Point newP)> PlanTurn(IReadOnlyCollection<Point> elvesPositions, Char[][] map,
        IReadOnlyCollection<CompassDirection> directionsOrder) =>
        elvesPositions.Where(position => NeedToGo(position, map))
                      .Select(position => (position, ChoosePosition(position, map, directionsOrder)))
                      .Where(x => x.Item2 is not null)
                      .Select(x => (x.position, x.Item2!.Value));

    private Point? ChoosePosition(Point position, char[][] map, IReadOnlyCollection<CompassDirection> directionsOrder)
        => directionsOrder.Select(x => ChoosePosition(position, map, x)).FirstOrDefault(x => x is not null);

    private Point? ChoosePosition(Point position, char[][] map, CompassDirection direction)
    {
        return direction switch
        {
            CompassDirection.North when TryMoveNorth(position, map, out var newPos) => newPos,
            CompassDirection.South when TryMoveSouth(position, map, out var newPos) => newPos,
            CompassDirection.West when TryMoveWest(position, map, out var newPos) => newPos,
            CompassDirection.East when TryMoveEast(position, map, out var newPos) => newPos,
            _ => null
        };
    }

    private Boolean TryMoveNorth(Point position, Char[][] map, out Point choosePosition)
    {
        choosePosition = default;
        var newPos = new[] {(-1, -1), (0, -1), (1, -1)}.Select(position.Move);
        if (!newPos.All(x => PositionsIsFree(x, map) && position.Y > 0))
            return false;

        choosePosition = position with {Y = position.Y - 1};
        return true;
    }

    private Boolean TryMoveEast(Point position, Char[][] map, out Point point)
    {
        point = default;
        IEnumerable<Point> newPos = new[] {(1, -1), (1, 0), (1, 1)}.Select(position.Move);
        if (!newPos.All(x => PositionsIsFree(x, map) && position.X < map.Length - 1))
            return false;

        point = position with {X = position.X + 1};
        return true;
    }

    private Boolean TryMoveWest(Point position, Char[][] map, out Point point)
    {
        point = default;
        IEnumerable<Point> newPos = new[] {(-1, -1), (-1, 0), (-1, 1)}.Select(position.Move);
        if (!newPos.All(x => PositionsIsFree(x, map) && position.X > 0))
            return false;

        point = position with {X = position.X - 1};
        return true;
    }

    private Boolean TryMoveSouth(Point position, Char[][] map, out Point point)
    {
        point = default;
        IEnumerable<Point> newPos = new[] {(-1, 1), (0, 1), (1, 1)}.Select(position.Move);
        if (!newPos.All(x => PositionsIsFree(x, map) && position.Y < map[0].Length - 1))
            return false;

        point = position with {Y = position.Y + 1};
        return true;
    }

    private Boolean PositionsIsFree(Point newPos, char[][] map) => newPos.X < 0 || newPos.X >= map.Length ||
                                                                   newPos.Y < 0 || newPos.Y >= map[0].Length ||
                                                                   map[newPos.X][newPos.Y] != '#';

    private Boolean NeedToGo(Point position, Char[][] map)
    {
        var diffs = new[] {(-1, -1), (0, -1), (1, -1), (-1, 0), (1, 0), (-1, 1), (0, 1), (1, 1)};

        foreach (var (dx, dy) in diffs)
        {
            var newPos = new Point(position.X + dx, position.Y + dy);
            if (!PositionsIsFree(newPos, map))
                return true;
        }

        return false;
    }

    private IEnumerable<Point> GetElvesPositions(char[][] arr)
        => arr.SelectMany((x, i) => x.Select((_, j) => new Point(i, j))).Where(x => arr[x.X][x.Y] == '#');
}

public enum CompassDirection
{
    North,
    South,
    West,
    East
}