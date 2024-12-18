using Shared;

namespace AdventOfCode2024;

public class Day14 : ISolvable<int>
{
    private const int Width = 101;
    private const int Height = 103;

    public int SolvePart1(string[] input)
    {
        var robots = ParseInput(input);

        var points = robots.Select(robot => (robot, newX: (robot.Pos.X + Width * 100 + robot.Velocity.x * 100) % Width))
                           .Select(x => (x.newX, newY: (x.robot.Pos.Y + Height * 100 + x.robot.Velocity.y * 100) % Height))
                           .Select(x => new Point(x.newX, x.newY))
                           .ToList();

        var leftUp = 0;
        var rightUp = 0;
        var leftDown = 0;
        var rightDown = 0;

        foreach (var point in points)
        {
            _ = point switch
            {
                {X: < Width / 2, Y: < Height / 2} => leftUp++,
                {X: > Width / 2, Y: < Height / 2} => rightUp++,
                {X: < Width / 2, Y: > Height / 2} => leftDown++,
                {X: > Width / 2, Y: > Height / 2} => rightDown++,
                _ => 0
            };
        }

        return leftUp * rightUp * leftDown * rightDown;
    }

    public int SolvePart2(string[] input)
    {
        var robots = ParseInput(input).ToArray();
        var field = Enumerable.Range(0, Height).Select(_ => new string('.', Width).ToCharArray()).ToArray();

        var seconds = 0;
        while (true)
        {
            foreach (var robot in robots)
                field[robot.Pos.Y][robot.Pos.X] = '#';

            if ((seconds - 95) % 101 == 0)
            {
                Thread.Sleep(200);
                field.Print();
                Console.WriteLine($"{seconds} seconds");
                Console.WriteLine();
            }

            for (var i = 0; i < robots.Length; i++)
            {
                field[robots[i].Pos.Y][robots[i].Pos.X] = '.';
                var newX = (robots[i].Pos.X + robots[i].Velocity.x + Width) % Width;
                var newY = (robots[i].Pos.Y + robots[i].Velocity.y + Height) % Height;

                robots[i] = new Robot((newX, newY), robots[i].Velocity);
            }

            seconds++;
        }

        return 0;
    }

    private IEnumerable<Robot> ParseInput(string[] input)
    {
        return input.Select(line => line.Split())
                    .Select(parts => (parts, pos: parts.First().Split('=').Last().Split(',').Select(int.Parse).ToList()))
                    .Select(x => (x.pos, velocity: x.parts.Last().Split('=').Last().Split(',').Select(int.Parse).ToList()))
                    .Select(x => new Robot((x.pos[0], x.pos[1]), (x.velocity[0], x.velocity[1])));
    }
}

internal record Robot(Point Pos, (int x, int y) Velocity);