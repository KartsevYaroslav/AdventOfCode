public class Day6 : ISolvable
{
    public void SolvePart1(string[] input)
    {
        var times = input.First().Split(':').Last().Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
        var distances = input.Last().Split(':').Last().Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();

        var res = Enumerable.Range(0, times.Count)
                            .Select(i => GetWinCounts(times[i], distances[i]))
                            .Aggregate((x, y) => x * y);

        Console.WriteLine(res);
    }

    public void SolvePart2(string[] input)
    {
        var time = Parse(input.First());
        var distance = Parse(input.Last());

        var res = GetWinCounts(time, distance);

        Console.WriteLine(res);
    }

    private static long Parse(string input)
    {
        var parsed = input.Split(':').Last().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var str = string.Join("", parsed);
        return long.Parse(str);
    }

    private static long GetWinCounts(long time, long distance)
    {
        var left = FindBorder(time, x => IsWin(time, x, distance));
        var right = FindBorder(time, x => !IsWin(time, x, distance));

        return right - left;
    }

    private static long FindBorder(long time, Func<long, bool> predicate)
    {
        var left = 0L;
        var right = time;
        while (left < right)
        {
            var mid = (left + right) / 2;
            if (predicate(mid))
                right = mid;
            else
                left = mid + 1;
        }

        return left;
    }

    private static bool IsWin(long fullTime, long holdTime, long distance) => (fullTime - holdTime) * holdTime > distance;
}