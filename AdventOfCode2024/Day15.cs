using Shared;

namespace AdventOfCode2024;

public class Day15 : ISolvable<int>
{
    public int SolvePart1(string[] input)
    {
        var map = input.TakeWhile(x => !string.IsNullOrEmpty(x)).Select(x => x.ToCharArray()).ToArray();
        var commands = ParseCommands(input);
        var cur = GetRobotPosition(map);

        foreach (var command in commands)
        {
            cur = command switch
            {
                '>' => Move1Part(cur, (0, 1), map),
                'v' => Move1Part(cur, (1, 0), map),
                '<' => Move1Part(cur, (0, -1), map),
                '^' => Move1Part(cur, (-1, 0), map),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        return Calculate(map, 'O');
    }

    public int SolvePart2(string[] input)
    {
        var map = input.TakeWhile(x => !string.IsNullOrEmpty(x)).Select(x => ConvertLine(x).ToArray()).ToArray();
        var commands = ParseCommands(input);
        var cur = GetRobotPosition(map);

        foreach (var command in commands)
        {
            cur = command switch
            {
                '>' => Move2Part(cur, (0, 1), map),
                'v' => Move2Part(cur, (1, 0), map),
                '<' => Move2Part(cur, (0, -1), map),
                '^' => Move2Part(cur, (-1, 0), map),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        return Calculate(map, '[');
    }

    private static int Calculate(char[][] map, char ch)
        => map.SelectMany((x, i) => x.Select((_, j) => (i, j))).Where(x => map[x.i][x.j] == ch).Sum(x => x.i * 100 + x.j);

    private static string ParseCommands(string[] input) => input.SkipWhile(x => !string.IsNullOrEmpty(x)).Skip(1).Aggregate((x, y) => $"{x}{y}");

    private static (int i, int j) GetRobotPosition(char[][] map) => map.SelectMany((x, i) => x.Select((_, j) => (i, j))).First(x => map[x.i][x.j] == '@');


    private Point Move1Part(Point cur, (int di, int dj) diff, char[][] map)
    {
        var newPoint = cur.Move(diff);

        return map.At(newPoint) switch
        {
            '#' => cur,
            '.' => MoveRobot(),
            'O' => MoveBox(),
            _ => throw new ArgumentOutOfRangeException()
        };

        Point MoveBox()
        {
            var freePoint = newPoint;
            while (map.At(freePoint) != '.' && map.At(freePoint) != '#')
                freePoint = freePoint.Move(diff);

            if (map.At(freePoint) == '#')
                return cur;

            if (map.At(freePoint) != '.')
                throw new Exception();

            map.Set(freePoint, 'O');

            return MoveRobot();
        }

        Point MoveRobot()
        {
            map.Set(cur, '.');
            map.Set(newPoint, '@');

            return newPoint;
        }
    }

    private static IEnumerable<char> ConvertLine(string s)
    {
        foreach (var ch in s)
        {
            switch (ch)
            {
                case '#':
                    yield return '#';
                    yield return '#';
                    break;
                case '.':
                    yield return '.';
                    yield return '.';
                    break;
                case 'O':
                    yield return '[';
                    yield return ']';
                    break;
                case '@':
                    yield return '@';
                    yield return '.';
                    break;
            }
        }
    }

    private static Point Move2Part(Point cur, (int di, int dj) diff, char[][] map)
    {
        var newPoint = cur.Move(diff);

        return map.At(newPoint) switch
        {
            '#' => cur,
            '.' => Move(cur, diff, map),
            '[' or ']' when diff.dj != 0 => CanMove(cur, diff, map) ? Move(cur, diff, map) : cur,
            '[' or ']' => CanMove(cur, diff, map) ? Move(cur, diff, map) : cur,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private static bool CanMove(Point cur, (int di, int dj) direction, char[][] map)
    {
        var destPoint = cur.Move(direction);

        if (map.At(destPoint) == '.')
            return true;

        if (map.At(destPoint) == '#')
            return false;

        if (direction.dj != 0)
            return CanMove(destPoint, direction, map);

        var boxPart = map.At(destPoint) == '[' ? destPoint.Move((0, 1)) : destPoint.Move((0, -1));

        return CanMove(destPoint, direction, map) && CanMove(boxPart, direction, map);
    }

    private static Point Move(Point cur, (int di, int dj) direction, char[][] map)
    {
        var destPoint = cur.Move(direction);
        var obj = map.At(destPoint);
        switch (obj)
        {
            case '#':
                throw new Exception("Can't move to wall");
            case '[' or ']' when direction.dj != 0:
                Move(destPoint, direction, map);
                break;
            case '[' or ']' when direction.dj == 0:
                var boxPart = obj == '[' ? destPoint.Move((0, 1)) : destPoint.Move((0, -1));
                Move(destPoint, direction, map);
                Move(boxPart, direction, map);
                break;
        }

        map.Set(destPoint, map.At(cur));
        map.Set(cur, '.');

        return destPoint;
    }
}