using Shared;

namespace AdventOfCode2024;

public class Day11 : ISolvable
{
    public string SolvePart1(string[] input)
    {
        var nums = input.First().Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(ulong.Parse).ToList();
        var res = Blink(nums, 25);

        return res.ToString();
    }

    public string SolvePart2(string[] input)
    {
        var nums = input.First().Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(ulong.Parse).ToList();
        
        var res = Blink(nums, 75);

        return res.ToString();
    }


    private static ulong Blink(List<ulong> nums, int times)
    {
        var hashSet = new Dictionary<(ulong, int), ulong>();

        return nums.Aggregate(0UL, (current, num) => current + GetCount(num, times, hashSet));
    }

    private static ulong GetCount(ulong curValue, int times, Dictionary<(ulong, int), ulong> cache)
    {
        while (times > 0)
        {
            if (cache.TryGetValue((curValue, times), out var count))
                return count;

            times--;
            
            if (curValue == 0)
            {
                curValue++;
                continue;
            }

            var strVal = curValue.ToString();

            if (strVal.Length % 2 != 0)
            {
                curValue *= 2024;
                continue;
            }

            var mid = strVal.Length / 2;
            var left = ulong.Parse(strVal[..mid]);
            var right = ulong.Parse(strVal[mid..]);
            var res = GetCount(left, times, cache) + GetCount(right, times, cache);
            cache.TryAdd((curValue, times + 1), res);
            
            return res;
        }

        return 1;
    }
}