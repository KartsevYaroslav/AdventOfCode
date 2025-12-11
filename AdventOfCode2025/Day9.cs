using System;
using System.Collections.Generic;
using System.Linq;
using Shared;

namespace AdventOfCode2025;

public class Day9 : ISolvable<long>
{
    public long SolvePart1(string[] input)
        => GetRectangles(ParsePoints(input)).Select(x => x.GetAreaIncluded()).Max();

    public long SolvePart2(string[] input)
    {
        var points = ParsePoints(input);

        var pointsByX = new Dictionary<int, HashSet<Point>>();
        var pointsByY = new Dictionary<int, HashSet<Point>>();
        foreach (var point in points)
        {
            if (!pointsByX.ContainsKey(point.X))
                pointsByX[point.X] = [];

            if (!pointsByY.ContainsKey(point.Y))
                pointsByY[point.Y] = [];
            pointsByY[point.Y].Add(point);
            pointsByX[point.X].Add(point);
        }

        var start = points.OrderBy(x => x.X).ThenBy(x => x.Y).First();

        var cur = start;
        var pointsByX1 = Enumerable.Range(start.X, pointsByX.Keys.Max() - start.X + 1)
            .ToDictionary(x => x, _ => new HashSet<int>());
        var minSide = int.MaxValue;
        while (true)
        {
            var next = NextByY(cur);
            minSide = Math.Min(minSide, Math.Abs(next.X - cur.X));
            for (var i = Math.Min(cur.X, next.X); i <= Math.Max(cur.X, next.X); i++)
                pointsByX1[i].Add(cur.Y);

            cur = next;
            if (cur == start)
                break;

            next = NextByX(cur);
            for (var i = Math.Min(cur.Y, next.Y); i <= Math.Max(cur.Y, next.Y); i++)
                pointsByX1[cur.X].Add(i);

            cur = next;
            if (cur == start)
                break;
        }

        var areas = GetRectangles(points);

        var pointsLine = pointsByX1.SelectMany(x => x.Value.Select(y => new Point(x.Key, y)))
            .OrderBy(x => x.X)
            .ToList();

        foreach (var rectangle in areas.OrderByDescending(x => x.GetAreaIncluded()))
        {
            var maxArea = CheckRectangle(rectangle, pointsLine, minSide);
            if (maxArea != 0)
                return maxArea;
        }

        return 0;

        Point NextByX(Point p) => pointsByX[p.X].OrderBy(x => Math.Abs(x.Y - p.Y)).Skip(1).First();

        Point NextByY(Point p) => pointsByY[p.Y].OrderBy(x => Math.Abs(x.X - p.X)).Skip(1).First();
    }

    private static List<Point> ParsePoints(string[] input)
        => input.Select(x => x.Split(',')).Select(x => new Point(int.Parse(x[0]), int.Parse(x[1]))).ToList();

    private static List<Rectangle> GetRectangles(List<Point> points)
    {
        var areas = new List<Rectangle>();
        for (var i = 0; i < points.Count - 1; i++)
        for (var j = i + 1; j < points.Count; j++)
        {
            var minX = Math.Min(points[i].X, points[j].X);
            var maxX = Math.Max(points[i].X, points[j].X);
            var minY = Math.Min(points[i].Y, points[j].Y);
            var maxY = Math.Max(points[i].Y, points[j].Y);
            var rectangle = new Rectangle(new Point(minX, minY), new Point(maxX, maxY));
            areas.Add(rectangle);
        }

        return areas;
    }

    private static long CheckRectangle(Rectangle rectangle, List<Point> pointsByX, int step)
    {
        var left = GetIndex(pointsByX, rectangle.LeftTop.X);
        var right = GetIndex(pointsByX, rectangle.RightTop.X);

        var leftBottomY = rectangle.LeftBottom.Y;
        var leftTopY = rectangle.LeftTop.Y;
        for (var i = left; i <= right; i += step)
        {
            var point = pointsByX[i];
            if (point.Y > leftTopY &&
                point.Y < leftBottomY)
                return 0;
        }

        return rectangle.GetAreaIncluded();
    }

    private static int GetIndex(List<Point> pointsByX, int x)
    {
        var left = 0;
        var right = pointsByX.Count - 1;
        while (left < right)
        {
            var mid = (left + right) / 2;
            if (pointsByX[mid].X > x)
                right = mid;

            if (pointsByX[mid].X < x)
                left = mid;

            if (pointsByX[mid].X == x)
                return mid;
        }

        return left;
    }
}