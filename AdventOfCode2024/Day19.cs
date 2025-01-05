namespace AdventOfCode2024;

public class Day19 : ISolvable<long>
{
    public long SolvePart1(string[] input)
    {
        var towels = input.First().Split(',').Select(x => x.Trim()).ToHashSet();
        var patterns = input.Skip(2).ToList();
        var cache = new Dictionary<string, long>();

        return patterns.Select(TrySolve)
                       .Count(x => x.Any(isPossible => isPossible));

        IEnumerable<bool> TrySolve(string pattern)
            => Enumerable.Range(0, pattern.Length).Select(i => CountSolutions(pattern, 0, i, towels, cache) > 0);
    }

    public long SolvePart2(string[] input)
    {
        var towels = input.First().Split(',').Select(x => x.Trim()).ToHashSet();
        var patterns = input.Skip(2).ToList();
        var cache = new Dictionary<string, long>();

        return patterns.Select(PatternSolutions)
                       .Sum(x => x.Sum());

        IEnumerable<long> PatternSolutions(string pattern)
            => Enumerable.Range(0, pattern.Length).Select(i => CountSolutions(pattern, 0, i, towels, cache));
    }

    private static long CountSolutions(string pattern, int start, int end, HashSet<string> towels, Dictionary<string, long> cache)
    {
        var containsTowel = towels.Contains(pattern.Substring(start, end - start + 1));
        if (end == pattern.Length - 1 && containsTowel)
            return 1;

        if (!containsTowel)
            return 0;

        var newStart = end + 1;
        var remainPattern = pattern.Substring(newStart, pattern.Length - newStart);
        if (cache.TryGetValue(remainPattern, out var solutions))
            return solutions;

        var solutionsCount = Enumerable.Range(newStart, pattern.Length - newStart).Sum(i => CountSolutions(pattern, newStart, i, towels, cache));
        cache.Add(remainPattern, solutionsCount);

        return solutionsCount;
    }
}