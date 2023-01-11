// Copyright (c) Veeam Software Group GmbH

namespace AdventOfCode2022;

public class Day18 : ISolvable
{
    public void Solve(String[] input)
    {
        var lavaPoints = input.Select(x => x.Split(',').Select(int.Parse).ToArray()).Select(x => new Point3d(x[0], x[1], x[2])).ToHashSet();

        var surfaceArea = GetSurfaceArea(lavaPoints);

        Console.WriteLine($"Part 1: {surfaceArea}");

        var airPoints = new HashSet<Point3d>();
        for (int x = lavaPoints.Min(p => p.X); x <= lavaPoints.Max(p => p.X); x++)
        for (int y = lavaPoints.Min(p => p.Y); y <= lavaPoints.Max(p => p.Y); y++)
        for (int z = lavaPoints.Min(p => p.Z); z <= lavaPoints.Max(p => p.Z); z++)
        {
            var point = new Point3d(x, y, z);
            if (!lavaPoints.Contains(point))
                airPoints.Add(point);
        }

        RemoveNotLockedAirPoints(lavaPoints, airPoints);

        surfaceArea -= GetSurfaceArea(airPoints);

        Console.WriteLine($"Part 2: {surfaceArea}");
    }

    private void RemoveNotLockedAirPoints(HashSet<Point3d> lavaPoints, HashSet<Point3d> airPoints)
    {
        var edgesAir = airPoints.Where(x => x.X == lavaPoints.Min(p => p.X) || x.X == lavaPoints.Max(p => p.X) ||
                                            x.Y == lavaPoints.Min(p => p.Y) || x.Y == lavaPoints.Max(p => p.Y) ||
                                            x.Z == lavaPoints.Min(p => p.Z) || x.Z == lavaPoints.Max(p => p.Z));

        var queue = new Queue<Point3d>();
        foreach (var p in edgesAir)
            queue.Enqueue(p);

        var visited = new HashSet<Point3d>();
        while (queue.Count != 0)
        {
            var point = queue.Dequeue();
            airPoints.Remove(point);

            foreach (var neighbour in GetNeighbours(point).Where(airPoints.Contains).Where(x => !visited.Contains(x)))
            {
                visited.Add(neighbour);
                queue.Enqueue(neighbour);
            }
        }
    }

    private static Int32 GetSurfaceArea(HashSet<Point3d> points)
    {
        var xyLookup = points.ToLookup(x => (x.X, x.Y));
        var yzLookup = points.ToLookup(x => (x.Y, x.Z));
        var xzLookup = points.ToLookup(x => (x.X, x.Z));

        var surfaceArea = 0;
        foreach (var point in points)
        {
            var freeEdges = 6;

            freeEdges -= xyLookup[(point.X, point.Y)].Count(x => Math.Abs(point.Z - x.Z) == 1);
            freeEdges -= yzLookup[(point.Y, point.Z)].Count(x => Math.Abs(point.X - x.X) == 1);
            freeEdges -= xzLookup[(point.X, point.Z)].Count(x => Math.Abs(point.Y - x.Y) == 1);

            surfaceArea += freeEdges;
        }

        return surfaceArea;
    }

    private IEnumerable<Point3d> GetNeighbours(Point3d point)
    {
        yield return point with {X = point.X + 1};
        yield return point with {X = point.X - 1};
        yield return point with {Y = point.Y + 1};
        yield return point with {Y = point.Y - 1};
        yield return point with {Z = point.Z + 1};
        yield return point with {Z = point.Z - 1};
    }
}

public record Point3d(int X, int Y, int Z);