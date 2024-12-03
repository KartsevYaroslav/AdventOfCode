public class Day4 : ISolvable
{
    public void SolvePart1(string[] input)
    {
        var games = input.Select(x => x.Split(':')[1])
                         .Select(x => x.Split('|'))
                         .Select(x => x.Select(y => y.Split(' ', StringSplitOptions.RemoveEmptyEntries)).ToList())
                         .Select(x => (win: x[0].ToHashSet(), have: x[1]));

        var res = 0L;
        foreach (var (win, have) in games)
        {
            var wins = have.Where(win.Contains).ToList();
            if (!wins.Any())
                continue;

            res += (long) Math.Pow(2, wins.Count - 1);
        }

        Console.WriteLine(res);
    }

    public void SolvePart2(string[] input)
    {
        var games = input.Select(x => x.Split(':')[1])
                         .Select(x => x.Split('|'))
                         .Select(x => x.Select(y => y.Split(' ', StringSplitOptions.RemoveEmptyEntries)).ToList())
                         .Select(x => x[1].Where(x[0].ToHashSet().Contains).Count())
                         .ToList();

        var dict = new Dictionary<int, int>();

        for (var i = 1; i <= games.Count; i++)
            dict[i] = 1;

        for (var i = 1; i <= games.Count; i++)
        for (var j = 0; j < dict[i]; j++)
        for (var k = 1; k <= games[i - 1]; k++)
            dict[i + k]++;

        var sum = dict.Values.Sum();

        Console.WriteLine(sum);
    }
}