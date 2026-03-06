namespace AdventOfCode2023
{
    public class Day9 : ISolvable<long>
    {
        public long SolvePart1(string[] input)
        => input.Select(x => x.Split().Select(long.Parse).ToArray()).Sum(x => SolveLine(x, (x, y) => x.Last() + y));

        public long SolvePart2(string[] input)
        => input.Select(x => x.Split().Select(long.Parse).ToArray()).Sum(x => SolveLine(x, (x, y) => x.First() - y));

        private static long SolveLine(long[] nums, Func<long[], long, long> selector)
        {
            if (nums.All(x => x == 0))
                return 0;

            var newLine = nums.Skip(1).Zip(nums, (x, y) => x - y).ToArray();
            var next = SolveLine(newLine, selector);

            return selector(nums, next);
        }
    }
}