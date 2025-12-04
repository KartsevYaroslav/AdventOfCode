using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2025;

public class Day3 : ISolvable<long>
{
    public long SolvePart1(string[] input) => SolveInternal(input, 2);

    public long SolvePart2(string[] input) => SolveInternal(input, 12);

    private static long SolveInternal(string[] input, int digitsNum)
    {
        var res = 0L;
        foreach (var line in input)
        {
            var nums = line.Select(x => x - '0').ToArray();

            var indexes = new List<int>();
            var maxI = -1;
            for (var i = digitsNum; i > 0; i--)
            {
                maxI = GetMaxIndex(nums, maxI + 1, i);
                indexes.Add(maxI);
            }

            res += indexes.Aggregate(0L, (current, i) => current * 10 + nums[i]);
        }

        return res;
    }

    private static int GetMaxIndex(int[] nums, int startI, int remainDigits)
    {
        var max = nums[startI];
        var maxI = startI;
        for (var i = startI; i < nums.Length - remainDigits + 1; i++)
        {
            if (nums[i] <= max)
                continue;

            max = nums[i];
            maxI = i;
        }

        return maxI;
    }
}