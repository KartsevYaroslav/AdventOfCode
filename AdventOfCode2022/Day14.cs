// Copyright (c) Veeam Software Group GmbH

namespace AdventOfCode2022;

public class Day14 : ISolvable
{
    public void Solve(String[] input)
    {
        const Int32 MaxX = 1000;

        var rockLines = ParseRockLines(input).ToList();

        var maxY = rockLines.Select(line => Math.Max(line.start.Y, line.end.Y)).Max();
        var map1 = GetMap(rockLines, MaxX, maxY);
        var result1 = SolveInternal(map1, pos => pos.Y != map1[0].Length - 1) - 1;

        Console.WriteLine($"Part 1: {result1}");

        maxY += 2;
        rockLines = rockLines.Append(new Line(new Point(0, maxY), new Point(MaxX, maxY))).ToList();

        var map2 = GetMap(rockLines, MaxX, maxY);
        var result2 = SolveInternal(map2, _ => map2[500][0] != 'o');

        Console.WriteLine($"Part 2: {result2}");
    }

    private Int32 SolveInternal(Char[][] map, Func<Point, bool> stopCondition)
    {
        var sandCount = 0;
        var sandPos = new Point(500, 0);

        while (stopCondition(sandPos))
        {
            sandPos = new Point(500, 0);
            map[sandPos.X][sandPos.Y] = 'o';
            while (true)
            {
                var newPos = MoveSand(map, sandPos);

                if (sandPos == newPos)
                    break;

                map[sandPos.X][sandPos.Y] = '.';
                map[newPos.X][newPos.Y] = 'o';

                sandPos = newPos;
            }

            sandCount++;
        }

        return sandCount;
    }

    private static Point MoveSand(Char[][] map, Point pos)
    {
        if (pos.Y == map[0].Length - 1)
            return pos;

        if (map[pos.X][pos.Y + 1] == '.')
            return pos with {Y = pos.Y + 1};

        if (map[pos.X - 1][pos.Y + 1] == '.')
            return new Point(pos.X - 1, pos.Y + 1);

        if (map[pos.X + 1][pos.Y + 1] == '.')
            return new Point(pos.X + 1, pos.Y + 1);

        return pos;
    }


    private static Char[][] GetMap(List<Line> rockLines, Int32 maxX, Int32 maxY)
    {
        var map = new char[maxX + 1].Select(_ => new char[maxY + 1].Select(_ => '.').ToArray()).ToArray();

        foreach (var rockLine in rockLines)
        {
            for (int i = Math.Min(rockLine.start.X, rockLine.end.X); i <= Math.Max(rockLine.start.X, rockLine.end.X); i++)
            for (int j = Math.Min(rockLine.start.Y, rockLine.end.Y); j <= Math.Max(rockLine.start.Y, rockLine.end.Y); j++)
                map[i][j] = 'x';
        }

        return map;
    }

    private IEnumerable<Line> ParseRockLines(string[] input) =>
        input.SelectMany(str =>
                         {
                             var parts = str.Split(" -> ").Select(x => x.Split(',').ToArray()).ToArray();
                             return parts[..^1].Zip(parts[1..], (x, y) => new Line(ParsePoint(x), ParsePoint(y))).ToList();
                         });

    private static Point ParsePoint(string[] x) => new(Int32.Parse(x[0]), Int32.Parse(x[1]));

    public record Line(Point start, Point end);
}