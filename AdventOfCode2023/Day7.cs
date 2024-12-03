public class Day7 : ISolvable
{
    public string SolvePart1(string[] input)
    {
        var parsed = ParseInput(input);
        var res = Calculate(parsed);

        return res.ToString();
    }

    public string SolvePart2(string[] input)
    {
        var parsed = ParseInput(input);
        var res = Calculate2(parsed);

        return res.ToString();
    }

    private long Calculate(IEnumerable<(string hand, int bid)> parsed)
    {
        return parsed.OrderBy(x => GetWinPoints(x.hand))
                     .ThenBy(x => x.hand, new MyComparer(new Dictionary<char, int>
                     {
                         ['T'] = 10,
                         ['J'] = 11,
                         ['Q'] = 12,
                         ['K'] = 13,
                         ['A'] = 14
                     }))
                     .Select((x, i) => (i + 1) * (long) x.bid)
                     .Sum();
    }

    private int GetWinPoints(string hand)
    {
        var funcs = new List<Func<string, bool>>
        {
            x => x.Distinct().Count() == 1,
            x => x.GroupBy(ch => ch).Any(g => g.Count() == 4),
            IsFullHouse,
            x => x.GroupBy(ch => ch).Any(g => g.Count() == 3),
            x => x.GroupBy(ch => ch).Count(p => p.Count() == 2) == 2,
            x => x.GroupBy(ch => ch).Any(p => p.Count() == 2),
            x => x.Distinct().Count() == 5
        };

        for (var i = 0; i < funcs.Count; i++)
        {
            if (funcs[i](hand))
                return funcs.Count - i;
        }

        return 0;

        bool IsFullHouse(string x)
        {
            var groupBy = x.GroupBy(ch => ch).ToList();
            return groupBy.Any(g => g.Count() == 3) && groupBy.Any(g => g.Count() == 2);
        }
    }

    private IEnumerable<(string hand, int bid)> ParseInput(string[] input) =>
        input.Select(line => line.Split()).Select(parts => (parts[0], int.Parse(parts[1])));

    private long Calculate2(IEnumerable<(string hand, int bid)> parsed)
    {
        var orderedEnumerable = parsed.Select(x => (orig: x, transformed: TransformToMax(x.hand)))
                                      .OrderBy(x => GetWinPoints(x.transformed));
        return orderedEnumerable
               .ThenBy(x => x.orig.hand, new MyComparer(new Dictionary<char, int>
               {
                   ['T'] = 10,
                   ['J'] = 1,
                   ['Q'] = 12,
                   ['K'] = 13,
                   ['A'] = 14
               }))
               .Select((x, i) => (i + 1) * (long) x.orig.bid)
               .Sum();
    }

    private string TransformToMax(string hand)
    {
        var permutations = GetPermutations(hand, 0).ToList();

        return permutations
               .Select(x => (str: x, points: GetWinPoints(x)))
               .MaxBy(x => x.points)
               .str;
    }

    private IEnumerable<string> GetPermutations(string hand, int skip)
    {
        var valueTuples = hand.Select((x, i) => (x, i)).Where(x => x.x == 'J').Skip(skip).ToList();

        if (!valueTuples.Any())
            yield return hand;

        foreach (var (_, i) in valueTuples)
        {
            foreach (var perm in GetPermutations(hand, skip + 1))
                yield return perm;

            var notIncluded = new[] {'2', '3', '4', '5', '6', '7', '8', '9', 'T', 'Q', 'K', 'A'}.Except(hand).First();
            foreach (var newCh in hand.Distinct().Except(new[] {'J'}).Append(notIncluded))
            {
                var newStr = hand.ToArray();
                newStr[i] = newCh;
                foreach (var perm in GetPermutations(new string(newStr), skip))
                    yield return perm;
            }
        }
    }
}

internal class MyComparer : IComparer<string>
{
    private readonly Dictionary<char, int> dict;

    public MyComparer(Dictionary<char, int> dict)
    {
        this.dict = dict;
    }

    public int Compare(string? x, string? y)
    {
        for (int i = 0; i < x.Length; i++)
        {
            var left = char.IsDigit(x[i]) ? x[i] - '0' : dict[x[i]];
            var right = char.IsDigit(y[i]) ? y[i] - '0' : dict[y[i]];

            if (left == right)
                continue;

            return left < right ? -1 : 1;
        }

        return 0;
    }
}