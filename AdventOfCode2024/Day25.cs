using Shared;

namespace AdventOfCode2024;

public class Day25 : ISolvable<long>
{
    public long SolvePart1(string[] input)
    {
        var schematics = new List<string[]>();

        while (input.Length != 0)
        {
            var schematic = input.TakeWhile(x => !string.IsNullOrEmpty(x)).ToArray();
            schematics.Add(schematic);
            input = input.SkipWhile(x => !string.IsNullOrEmpty(x)).Skip(1).ToArray();
        }

        var locks = schematics.Where(x => x[0].All(y => y == '#')).ToList();
        var keys = schematics.Where(x => x.Last().All(y => y == '#')).ToList();

        var locksSizes = locks.Select(x => x.ToCharArray().Rotate().Select(y => y.Count(ch => ch == '.') - 1).ToArray()).ToList();
        var keysSizes = keys.Select(x => x.ToCharArray().Rotate().Select(y => y.Count(ch => ch == '#') - 1).ToArray()).ToList();

        return locksSizes.Select(x => keysSizes.Count(y => x.Zip(y, (l, r) => l - r).All(c => c >= 0))).Sum();
    }
}