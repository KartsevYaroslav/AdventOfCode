// Copyright (c) Veeam Software Group GmbH

namespace AdventOfCode2022;

public static class Extensions
{
    public static char At(this char[][] arr, Point p)
    {
        return arr[p.X][p.Y];
    }
    
    public static bool InRange(this char[][] arr, Point p)
    {
        return p.X >= 0 && p.X < arr.Length &&
               p.Y >= 0 && p.Y < arr[0].Length &&
               arr.At(p) != ' ';
    }

    public static Direction ToDir(this int dir) => (Direction) dir;

    public static int Mod(this int val, int modulo) => (val % modulo + modulo) % modulo;

    public static int DistanceTo(this Point point, Point other) => Math.Abs(other.X - point.X) + Math.Abs(other.Y - point.Y);

    public static void FillWith(this char[][] arr, IEnumerable<Point> points, char ch)
    {
        foreach (Point point in points)
            arr[point.X][point.Y] = ch;
    }

    public static void FillWith(this char[][] arr, IEnumerable<(Point, char ch)> points)
    {
        foreach (var (point, ch) in points)
            arr[point.X][point.Y] = ch;
    }

    public static char[][] Rotate(this char[][] arr)
    {
        return new char[arr[0].Length].Select((_, i) => arr.Select(x => x[i]).ToArray()).ToArray();
    }

    public static char[][] ParseToArray(this string[] input)
    {
        var array = new char[input[0].Length].Select(_ => new char[input.Length]).ToArray();

        for (int i = 0; i < array.Length; i++)
        for (int j = 0; j < array[0].Length; j++)
            array[i][j] = input[j][i];

        return array;
    }

    public static char[][] ExtendBy(this char[][] input, int count, char emptyPoint)
    {
        var array = new char[input.Length + count * 2].Select(_ => new char[input[0].Length + count * 2].Select(_ => emptyPoint).ToArray()).ToArray();

        for (int i = 0; i < input.Length; i++)
        for (int j = 0; j < input[0].Length; j++)
            array[i + count][j + count] = input[i][j];

        return array;
    }

    public static void PrintToFile(this char[][] map)
    {
        using var fileStream = File.OpenWrite("output.txt");
        using var streamWriter = new StreamWriter(fileStream);
        Console.SetOut(streamWriter);

        map.Print();

        var standardOutput = new StreamWriter(Console.OpenStandardOutput());
        standardOutput.AutoFlush = true;
        Console.SetOut(standardOutput);
    }

    public static void Print(this char[][] map)
    {
        for (int y = 0; y < map[0].Length; y++)
        {
            for (int x = 0; x < map.Length; x++)
            {
                Console.Write(map[x][y]);
            }

            Console.WriteLine();
        }
    }
}