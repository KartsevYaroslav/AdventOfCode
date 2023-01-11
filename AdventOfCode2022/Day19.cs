// Copyright (c) Veeam Software Group GmbH

using System.Text.RegularExpressions;

namespace AdventOfCode2022;

public class Day19 : ISolvable
{
    public void Solve(String[] input)
    {
        var blueprints = input.Select(ParseBlueprint).ToList();

        var solver1 = new RobotFactorySimulation(24);
        var res1 = blueprints.Select(blueprint => solver1.GetMaxPossibleGeode(blueprint))
                             .Select((maxGeode, i) => maxGeode * (i + 1))
                             .Sum();

        Console.WriteLine($"Part 1: {res1}");

        var solver2 = new RobotFactorySimulation(32);
        var res2 = blueprints.Take(3)
                             .Select(solver2.GetMaxPossibleGeode)
                             .Aggregate((x, y) => x * y);

        Console.WriteLine($"Part 2: {res2}");
    }

    private Blueprint ParseBlueprint(String input)
    {
        var pattern =
            @".*: Each ore robot costs (\d+) ore. Each clay robot costs (\d+) ore. Each obsidian robot costs (\d+) ore and (\d+) clay. Each geode robot costs (\d+) ore and (\d+) obsidian.";

        var match = Regex.Match(input, pattern);
        var costs = match.Groups.Values.Skip(1).Select(x => int.Parse(x.Value)).ToList();

        var oreRobotCost = new Resources(costs[0], 0, 0, 0);
        var clayRobotCost = new Resources(costs[1], 0, 0, 0);
        var obsidianRobotCost = new Resources(costs[2], costs[3], 0, 0);
        var geodeRobotCost = new Resources(costs[4], 0, costs[5], 0);

        return new Blueprint(oreRobotCost, clayRobotCost, obsidianRobotCost, geodeRobotCost);
    }
}

public class RobotFactorySimulation
{
    private readonly Dictionary<(Int32, Int32), Resources> maxGeodeByMin = new();
    private readonly Int32 _totalMinutes;
    private int _maxGeode = 0;

    public RobotFactorySimulation(Int32 totalMinutes)
    {
        _totalMinutes = totalMinutes;
    }

    public int GetMaxPossibleGeode(Blueprint blueprint)
    {
        var robots = new Robots();
        robots.CreateOreRobot();
        var maxGeode = GetMaxPossibleGeode(blueprint, new Resources(0, 0, 0, 0), 0, robots);
        _maxGeode = 0;
        maxGeodeByMin.Clear();

        return maxGeode;
    }

    private int GetMaxPossibleGeode(Blueprint blueprint, Resources curResources, int curMinute, IRobots robots)
    {
        if (curMinute == _totalMinutes)
        {
            _maxGeode = Math.Max(_maxGeode, curResources.Geode);
            return _maxGeode;
        }

        if (maxGeodeByMin.TryGetValue((curMinute, robots.StateHash), out var mGeode) && mGeode >= curResources)
            return _maxGeode;

        maxGeodeByMin[(curMinute, robots.StateHash)] = curResources;

        var newResources = curResources + robots.CollectResources();

        if (curMinute < _totalMinutes - 1)
        {
            if (curResources >= blueprint.GeodeRobotCost)
            {
                robots.CreateGeodeRobot();
                GetMaxPossibleGeode(blueprint, newResources - blueprint.GeodeRobotCost, curMinute + 1, robots);
                robots.RemoveLast();
            }

            if (curResources >= blueprint.ObsidianRobotCost &&
                robots.ObsidianRobotsCount < blueprint.MaxNeededObsidianRobots)
            {
                robots.CreateObsidianRobot();
                GetMaxPossibleGeode(blueprint, newResources - blueprint.ObsidianRobotCost, curMinute + 1, robots);
                robots.RemoveLast();
            }

            if (curResources >= blueprint.ClayRobotCost &&
                robots.ClayRobotsCount < blueprint.MaxNeededClayRobots)
            {
                robots.CreateClayRobot();
                GetMaxPossibleGeode(blueprint, newResources - blueprint.ClayRobotCost, curMinute + 1, robots);
                robots.RemoveLast();
            }

            if (curResources >= blueprint.OreRobotCost &&
                robots.OreRobotsCount < blueprint.MaxNeededOreRobots)
            {
                robots.CreateOreRobot();
                GetMaxPossibleGeode(blueprint, newResources - blueprint.OreRobotCost, curMinute + 1, robots);
                robots.RemoveLast();
            }
        }

        GetMaxPossibleGeode(blueprint, newResources, curMinute + 1, robots);

        return _maxGeode;
    }
}

public interface IRobots
{
    int OreRobotsCount { get; }
    int ClayRobotsCount { get; }
    int ObsidianRobotsCount { get; }
    int GeodeRobotsCount { get; }
    int StateHash { get; }

    Resources CollectResources();
    void CreateOreRobot();
    void CreateClayRobot();
    void CreateObsidianRobot();
    void CreateGeodeRobot();
    void RemoveLast();
}

class Robots : IRobots
{
    private readonly List<IRobot> _robots = new();
    public Int32 OreRobotsCount { get; private set; }
    public Int32 ClayRobotsCount { get; private set; }
    public Int32 ObsidianRobotsCount { get; private set; }
    public Int32 GeodeRobotsCount { get; private set; }
    public Int32 StateHash { get; private set; }
    public Resources CollectResources() => _robots.Aggregate(new Resources(0, 0, 0, 0), (current, robot) => current + robot.CollectResources());

    public void CreateOreRobot()
    {
        _robots.Add(new OreRobot());
        StateHash++;
        OreRobotsCount++;
    }

    public void CreateClayRobot()
    {
        _robots.Add(new ClayRobot());
        StateHash = StateHash + 32 + 1;
        ClayRobotsCount++;
    }

    public void CreateObsidianRobot()
    {
        _robots.Add(new ObsidianRobot());
        StateHash = StateHash + 32 * 32 + 1;
        ObsidianRobotsCount++;
    }

    public void CreateGeodeRobot()
    {
        _robots.Add(new GeodeRobot());
        StateHash = StateHash + 32 * 32 * 32 + 1;
        GeodeRobotsCount++;
    }

    public void RemoveLast()
    {
        var robot = _robots[^1];
        if (robot is OreRobot)
        {
            OreRobotsCount--;
            StateHash--;
        }

        if (robot is ClayRobot)
        {
            ClayRobotsCount--;
            StateHash = StateHash - 32 - 1;
        }

        if (robot is ObsidianRobot)
        {
            ObsidianRobotsCount--;
            StateHash = StateHash - 32 * 32 - 1;
        }

        if (robot is GeodeRobot)
        {
            GeodeRobotsCount--;
            StateHash = StateHash - 32 * 32 * 32 - 1;
        }

        _robots.RemoveAt(_robots.Count - 1);
    }
}

internal class GeodeRobot : IRobot
{
    public Int32 Type => 4;
    public Resources CollectResources() => new(0, 0, 0, 1);
}

internal class ObsidianRobot : IRobot
{
    public Int32 Type => 3;
    public Resources CollectResources() => new(0, 0, 1, 0);
}

internal class ClayRobot : IRobot
{
    public Int32 Type => 2;
    public Resources CollectResources() => new(0, 1, 0, 0);
}

internal interface IRobot
{
    int Type { get; }
    Resources CollectResources();
}

class OreRobot : IRobot
{
    public Int32 Type => 1;
    public Resources CollectResources() => new(1, 0, 0, 0);
}

public record Resources(int Ore, int Clay, int Obsidian, int Geode)
{
    public static bool operator >=(Resources l, Resources r) => l.Ore >= r.Ore && l.Clay >= r.Clay && l.Obsidian >= r.Obsidian && l.Geode >= r.Geode;
    public static Resources operator +(Resources l, Resources r) => new(l.Ore + r.Ore, l.Clay + r.Clay, l.Obsidian + r.Obsidian, l.Geode + r.Geode);
    public static Resources operator -(Resources l, Resources r) => new(l.Ore - r.Ore, l.Clay - r.Clay, l.Obsidian - r.Obsidian, l.Geode - r.Geode);

    public static Boolean operator <=(Resources l, Resources r)=> l.Ore <= r.Ore && l.Clay <= r.Clay && l.Obsidian <= r.Obsidian && l.Geode <= r.Geode;
}

public record Blueprint(Resources OreRobotCost, Resources ClayRobotCost, Resources ObsidianRobotCost, Resources GeodeRobotCost)
{
    public int MaxNeededOreRobots => Math.Max(Math.Max(ClayRobotCost.Ore, ObsidianRobotCost.Ore), GeodeRobotCost.Ore);
    public int MaxNeededClayRobots => ObsidianRobotCost.Clay;
    public int MaxNeededObsidianRobots => GeodeRobotCost.Obsidian;
}