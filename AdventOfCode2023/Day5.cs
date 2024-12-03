public class Day5 : ISolvable
{
    public string SolvePart1(string[] input)
    {
        var seeds = input.First()
                         .Split(':')
                         .Last()
                         .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                         .Select(long.Parse);

        var dict = new Dictionary<string, (string, List<Map>)>();
        for (var i = 2; i < input.Length; i++)
        {
            var names = input[i].Split().First().Split("-to-");
            var map = new List<Map>();
            while (i < input.Length - 1 && input[++i] != string.Empty)
            {
                var nums = input[i].Split().Select(long.Parse).ToList();
                map.Add(new Map(nums[0], nums[1], nums[2]));
            }

            dict[names[0]] = (names[1], map);
        }

        var min = long.MaxValue;
        foreach (var seed in seeds)
        {
            var curLoc = "seed";
            var curNum = seed;

            while (curLoc != "location")
            {
                foreach (var map in dict[curLoc].Item2)
                {
                    if (curNum >= map.Source && curNum <= map.Source + map.Length)
                    {
                        curNum = map.Dest + (curNum - map.Source);
                        break;
                    }
                }

                curLoc = dict[curLoc].Item1;
            }

            min = Math.Min(curNum, min);
        }

        return min.ToString();
    }

    public string SolvePart2(string[] input)
    {
        var seeds = input.First()
                         .Split(':')
                         .Last()
                         .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                         .Select(long.Parse)
                         .ToList();

        var seedRanges = new List<Range>();
        for (int i = 0; i < seeds.Count; i += 2)
            seedRanges.Add(new Range(seeds[i], seeds[i + 1]));

        var dict = new Dictionary<string, (string, List<Map>)>();
        for (var i = 2; i < input.Length; i++)
        {
            var names = input[i].Split().First().Split("-to-");
            var map = new List<Map>();
            while (i < input.Length - 1 && input[++i] != string.Empty)
            {
                var nums = input[i].Split().Select(long.Parse).ToList();
                map.Add(new Map(nums[0], nums[1], nums[2]));
            }

            dict[names[0]] = (names[1], map);
        }

        var min = long.MaxValue;
        foreach (var range in seedRanges)
        {
            var curLoc = "seed";
            var curRanges = new List<Range> {range};

            while (true)
            {
                curRanges = SplitRanges(curRanges, dict[curLoc].Item2);

                curLoc = dict[curLoc].Item1;
                if (curLoc == "location")
                    break;
            }


            min = Math.Min(curRanges.OrderBy(x => x.Start).First().Start, min);
        }

        return min.ToString();
    }

    private List<Range> SplitRanges(List<Range> ranges, List<Map> map)
    {
        var res = new List<Range>();

        foreach (var range in ranges.OrderBy(x => x.Start))
        {
            var curStart = range.Start;
            foreach (var curMap in map.OrderBy(x => x.Source))
            {
                if (curStart > curMap.SourceEnd)
                    continue;

                if (range.End < curMap.Source)
                {
                    res.Add(new Range(curStart, range.End - curStart + 1));
                    break;
                }

                if (range.End <= curMap.SourceEnd && curStart >= curMap.Source)
                {
                    res.Add(new Range(curMap.Dest + (curStart - curMap.Source), range.End - curStart + 1));
                    break;
                }

                if (range.End >= curMap.Source && range.End <= curMap.SourceEnd && curStart < curMap.Source)
                {
                    res.Add(new Range(curStart, curMap.Source - curStart + 1));
                    res.Add(new Range(curMap.Dest, range.End - curMap.Source + 1));
                    break;
                }

                if (range.End >= curMap.SourceEnd && curStart <= curMap.Source)
                {
                    res.Add(new Range(curStart, curMap.Source - curStart + 1));
                    res.Add(new Range(curMap.Dest, curMap.Length));
                    curStart = curMap.SourceEnd + 1;
                    continue;
                }

                if (range.End > curMap.SourceEnd && curStart > curMap.Source && curStart < curMap.SourceEnd)
                {
                    res.Add(new Range(curMap.Dest + (curStart - curMap.Source), curMap.SourceEnd - curStart + 1));
                    curStart = curMap.SourceEnd + 1;
                }
            }
        }

        if (!res.Any())
            res.AddRange(ranges);

        return res;
    }
}

public record Map(long Dest, long Source, long Length)
{
    public long SourceEnd => Source + Length - 1;
}

public record Range(long Start, long Length)
{
    public long End => Start + Length - 1;
}