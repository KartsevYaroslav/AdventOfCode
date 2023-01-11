// Copyright (c) Veeam Software Group GmbH

namespace AdventOfCode2022;

public class Day22 : ISolvable
{
    public void Solve(String[] input)
    {
        var mapInput = input.TakeWhile(v => !string.IsNullOrEmpty(v)).ToList();
        var cubeHeight = mapInput.Select(v => v.SkipWhile(p => p == ' ').TakeWhile(p => p != ' ')).Min(v => v.Count());
        var map = ParseMap(mapInput);
        var commands = input.SkipWhile(v => !string.IsNullOrEmpty(v)).Skip(1).First();

        var faces = GetFaceMap(map, cubeHeight).ToList();
        var (y, x) = input.SelectMany((arr, i) => arr.Select((_, j) => (i, j))).First(p => input[p.i][p.j] == '.');

        var transitions = GetFaceTransitions(faces, map, cubeHeight);
        var portals = GetPortals(transitions, cubeHeight);

        var res = SolveInner(commands, x, y, map, portals);

        Console.WriteLine($"Part 2: {res}");
    }

    private int SolveInner(String commands, Int32 x, Int32 y, Char[][] map, Dictionary<(Int32 x, Int32 y, Direction dir), (Int32 x, Int32 y, Direction dir)> portals)
    {
        var i = 0;
        var curDir = Direction.Right;

        while (i < commands.Length)
        {
            var steps = 0;
            while (i < commands.Length && char.IsDigit(commands[i]))
                steps = steps * 10 + (commands[i++] - '0');

            for (int j = 0; j < steps; j++)
            {
                (x, y, curDir) = curDir switch
                {
                    Direction.Right => Move(map, portals, (x, y, curDir), (1, 0)),
                    Direction.Left => Move(map, portals, (x, y, curDir), (-1, 0)),
                    Direction.Down => Move(map, portals, (x, y, curDir), (0, 1)),
                    Direction.Up => Move(map, portals, (x, y, curDir), (0, -1)),
                    _ => throw new Exception()
                };
            }

            if (i >= commands.Length)
                break;

            curDir = GetDirection(commands[i], curDir);
            i++;
        }

        x++;
        y++;
        return 1000 * y + 4 * x + (int)curDir;
    }

    private Dictionary<(int x, int y, Direction dir), (int x, int y, Direction dir)> GetPortals(
        Dictionary<(Point face, Direction dir), (Point face, Direction dir)> transitions, Int32 cubeHeight)
    {
        var dict = new Dictionary<(int x, int y, Direction dir), (int x, int y, Direction dir)>();

        foreach (var (k, v) in transitions)
        {
            var diff = (v.dir - k.dir).Mod(4);
            var (startX, startY) = k.dir switch
            {
                Direction.Right => ((k.face.X + 1) * cubeHeight, k.face.Y * cubeHeight),
                Direction.Left => (k.face.X * cubeHeight - 1, k.face.Y * cubeHeight),
                Direction.Up => (k.face.X * cubeHeight, k.face.Y * cubeHeight - 1),
                Direction.Down => (k.face.X * cubeHeight, (k.face.Y + 1) * cubeHeight),
            };

            var (targetX, targetY) = v.dir switch
            {
                Direction.Right => (v.face.X * cubeHeight, v.face.Y * cubeHeight),
                Direction.Left => ((v.face.X + 1) * cubeHeight - 1, v.face.Y * cubeHeight),
                Direction.Up => (v.face.X * cubeHeight, (v.face.Y + 1) * cubeHeight - 1),
                Direction.Down => (v.face.X * cubeHeight, v.face.Y * cubeHeight),
            };

            for (int i = 0; i < cubeHeight; i++)
            {
                var (startDx, startDy, targetDx, targetDy) = GetDiffs(k, diff, i);

                dict.Add((startX + startDx, startY + startDy, k.dir), (targetX + targetDx, targetY + targetDy, v.dir));
            }
        }

        return dict;

        (Int32 startDx, Int32 startDy, Int32 targetDx, Int32 targetDy) GetDiffs((Point face, Direction dir) k, Int32 diff, Int32 i) => k.dir switch
        {
            Direction.Up or Direction.Down when diff == 1 => (i, 0, 0, i),
            Direction.Up or Direction.Down when diff == 2 => (i, 0, cubeHeight - 1 - i, 0),
            Direction.Up or Direction.Down when diff == 3 => (i, 0, 0, cubeHeight - 1 - i),
            Direction.Up or Direction.Down => (i, 0, i, 0),
            Direction.Left or Direction.Right when diff == 1 => (0, i, cubeHeight - 1 - i, 0),
            Direction.Left or Direction.Right when diff == 2 => (0, i, 0, cubeHeight - 1 - i),
            Direction.Left or Direction.Right when diff == 3 => (0, i, i, 0),
            Direction.Left or Direction.Right => (0, i, 0, i),
        };
    }

    private Dictionary<(Point face, Direction dir), (Point face, Direction dir)> GetFaceTransitions(List<Point> faces, char[][] map, Int32 cubeHeight)
    {
        var dict = new Dictionary<(Point face, Direction dir), (Point face, Direction dir)>();
        foreach (var face in faces)
        {
            var point = new Point(face.X * cubeHeight, face.Y * cubeHeight);
            if (map.InRange(point.Move(cubeHeight, 0)))
                dict.Add((face, Direction.Right), (face.Move(1, 0), Direction.Right));

            if (map.InRange(point.Move(-1, 0)))
                dict.Add((face, Direction.Left), (face.Move(-1, 0), Direction.Left));

            if (map.InRange(point.Move(0, cubeHeight)))
                dict.Add((face, Direction.Down), (face.Move(0, 1), Direction.Down));

            if (map.InRange(point.Move(0, -1)))
                dict.Add((face, Direction.Up), (face.Move(0, -1), Direction.Up));
        }

        while (dict.Count < 24)
        {
            foreach (var face in faces)
            {
                for (int dir = 0; dir < 4; dir++)
                {
                    if (dict.ContainsKey((face, (Direction) dir)))
                        continue;

                    if (dict.TryGetValue((face, (dir + 1).Mod(4).ToDir()), out var tr1) &&
                        dict.TryGetValue((tr1.face, ((int) tr1.dir - 1).Mod(4).ToDir()), out var tr2))
                    {
                        dict.Add((face, dir.ToDir()), (tr2.face, ((int) tr2.dir + 1).Mod(4).ToDir()));
                        dict.Add((tr2.face, ((int) tr2.dir - 1).Mod(4).ToDir()), (face, (dir + 2).Mod(4).ToDir()));
                    }

                    if (dict.Count == 24)
                        break;

                    if (dict.TryGetValue((face, (dir - 1).Mod(4).ToDir()), out var tr3) &&
                        dict.TryGetValue((tr3.face, ((int) tr3.dir + 1).Mod(4).ToDir()), out var tr4))
                    {
                        dict.Add((face, dir.ToDir()), (tr4.face, ((int) tr4.dir - 1).Mod(4).ToDir()));
                        dict.Add((tr4.face, ((int) tr4.dir + 1).Mod(4).ToDir()), (face, (dir + 2).Mod(4).ToDir()));
                    }
                }
            }
        }

        return dict;
    }

    private static IEnumerable<Point> GetFaceMap(char[][] map, Int32 cubeHeight)
    {
        var faces = new List<Point>();
        for (int x = 0; x < map.Length; x += cubeHeight)
        for (int y = 0; y < map[0].Length; y += cubeHeight)
        {
            if (map[x][y] != ' ')
                faces.Add(new Point(x / cubeHeight, y / cubeHeight));
        }

        return faces;
    }

    private Direction GetDirection(Char command, Direction curDir)
    {
        return command switch
        {
            'R' => ((int) curDir + 1).Mod(4).ToDir(),
            'L' =>  ((int) curDir - 1).Mod(4).ToDir(),
        };
    }

    private (Int32 x, Int32 y, Direction dir) Move(Char[][] map, Dictionary<(Int32 x, Int32 y, Direction dir), (Int32 x, Int32 y, Direction dir)> portals,
        (Int32 x, Int32 y, Direction dir) point, (Int32 dx, Int32 dy) diff)
    {
        var (newX, newY, newDir) = (point.x + diff.dx, point.y + diff.dy, point.dir);

        if (portals.TryGetValue((newX, newY, newDir), out var val))
            (newX, newY, newDir) = val;

        if (map[newX][newY] == '#')
            return point;

        map[newX][newY] = '@';
        map[point.x][point.y] = '.';

        return (newX, newY, newDir);
    }

    private char[][] ParseMap(List<String> input)
    {
        var array = new char[input.MaxBy(x => x.Length).Length].Select(_ => new char[input.Count].Select(_ => ' ').ToArray()).ToArray();
        for (int y = 0; y < input.Count; y++)
        for (int x = 0; x < input[y].Length; x++)
            array[x][y] = input[y][x];

        return array;
    }
}

public enum Direction
{
    Right,
    Down,
    Left,
    Up,
}