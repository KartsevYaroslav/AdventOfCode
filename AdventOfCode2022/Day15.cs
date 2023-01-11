// Copyright (c) Veeam Software Group GmbH

namespace AdventOfCode2022;

public class Day15 : ISolvable
{
    public void Solve(String[] input)
    {
        var sensorsData = ParseInput(input).ToList();

        // var count = CreateMap(sensorsData);

        var count = GetBeacon(sensorsData);
        Console.WriteLine(count);

        // map.PrintArray();
    }

    private long CreateMap(IReadOnlyCollection<SensorData> data)
    {
        var res = 0L;

        var minMax = data.Select(x =>
                                 {
                                     var sPoint = x.Sensor.Point;
                                     var bPoint = x.Beacon.Point;
                                     var d = Math.Abs(sPoint.X - bPoint.X) + Math.Abs(sPoint.Y - bPoint.Y);

                                     return (min: Math.Min(sPoint.X, bPoint.X) - d, max: Math.Max(sPoint.X, bPoint.X) + d);
                                 })
                         .ToList();
        var minX = minMax.Select(y => y.min).Min();
        var maxX = minMax.Select(y => y.max).Max();
        for (int x = minX; x <= maxX; x++)
        {
            if (!IsBeaconPossible(data, x, 2000000))
                res++;
        }

        return res;
    }

    private long GetBeacon(IReadOnlyCollection<SensorData> data)
    {
        var possiblePoints = FillPossiblePoints(data);

        var notPossiblePoints = possiblePoints.Where(x => IsBeaconPossible(data, x.x, x.y)).ToList();
        var res = 0L;
        
        return res;
    }

    private static Boolean IsBeaconPossible(IReadOnlyCollection<SensorData> data, Int32 x, Int32 y)
    {
        foreach (var sData in data)
        {
            var sPoint = sData.Sensor.Point;
            var bPoint = sData.Beacon.Point;
            var distance = Math.Abs(sPoint.X - bPoint.X) + Math.Abs(sPoint.Y - bPoint.Y);
            // if (x == bPoint.X && y == bPoint.Y)
            //     return true;
            if (Math.Abs(x - sPoint.X) + Math.Abs(y - sPoint.Y) <= distance)
                return false;
        }

        return true;
    }

    private static HashSet<(Int32 x, Int32 y)> FillPossiblePoints(IReadOnlyCollection<SensorData> data)
    {
        var visited = new HashSet<(int x, int y)>();
        foreach (var sData in data)
        {
            var sPoint = sData.Sensor.Point;
            var bPoint = sData.Beacon.Point;
            var distance = Math.Abs(sPoint.X - bPoint.X) + Math.Abs(sPoint.Y - bPoint.Y);

            if (InArea(sPoint.X - distance - 1) && InArea(sPoint.Y))
                visited.Add((sPoint.X - distance - 1, sPoint.Y));
            if (InArea(sPoint.X + distance + 1) && InArea(sPoint.Y))
                visited.Add((sPoint.X + distance + 1, sPoint.Y));
            if (InArea(sPoint.Y + distance + 1) && InArea(sPoint.X))
                visited.Add((sPoint.X, sPoint.Y + distance + 1));
            if (InArea(sPoint.Y - distance - 1) && InArea(sPoint.X))
                visited.Add((sPoint.X, sPoint.Y - distance - 1));
            for (int x = Math.Max(sPoint.X - distance, 0); x <= Math.Min(sPoint.X + distance, 4000000); x++)
            {
                var y1 = distance - Math.Abs(sPoint.X - x) + sPoint.Y;
                var y2 = distance - Math.Abs(sPoint.X - x) - sPoint.Y;
                y2 = y1 >= 0 && y2 < 0 ? -y2 : y1;

                var dX = x > sPoint.X ? 1 : -1;
                if (InArea(y1))
                {
                    visited.Add((x, y1 + 1));
                    visited.Add((x + dX, y1));
                }

                if (InArea(y2))
                {
                    visited.Add((x + dX, y2));
                    visited.Add((x, y2 - 1));
                }
            }
        }

        return visited;

        Boolean InArea(int coordinate) => Math.Abs(coordinate) is >= 0 and <= 4000000;
    }

    private static IEnumerable<SensorData> ParseInput(String[] input)
    {
        foreach (var line in input)
        {
            var points = line.Replace("Sensor at ", "")
                             .Replace(" closest beacon is at ", "")
                             .Replace("x=", "")
                             .Replace("y=", "")
                             .Split(':');

            var sP = points[0].Split(", ").Select(int.Parse).ToList();
            var sensor = new Sensor(new Point(sP[0], sP[1]));

            var bP = points[1].Split(", ").Select(int.Parse).ToList();
            var beacon = new Beacon(new Point(bP[0], bP[1]));

            yield return new SensorData(sensor, beacon);
        }
    }
}

public record SensorData(Sensor Sensor, Beacon Beacon);

public record Sensor(Point Point);

public record Beacon(Point Point);