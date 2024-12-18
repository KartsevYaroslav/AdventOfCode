namespace AdventOfCode2024;

public class Day13 : ISolvable<long>
{
    public long SolvePart1(string[] input)
    {
        var cases = ParseInput(input);

        return SolveInternal(cases);
    }

    public long SolvePart2(string[] input)
    {
        var cases = ParseInput(input, 10000000000000);

        return SolveInternal(cases);
    }

    private static long SolveInternal(List<Case> cases)
    {
        var res = 0L;
        foreach (var case1 in cases)
        {
            var right = case1.Prize.x * case1.A.y - case1.Prize.y * case1.A.x;
            var left = case1.A.y * case1.B.x - case1.B.y * case1.A.x;
            if (right % left != 0)
                continue;

            //y - how many times push B, x - the same for A
            var y = right / left;
            var rightX = case1.Prize.x - case1.B.x * y;
            var x = rightX / case1.A.x;
            res += x * 3 + y;
        }

        return res;
    }

    private static List<Case> ParseInput(string[] input, long add = 0)
    {
        var cases = new List<Case>();
        input = input.Append(string.Empty).ToArray();
        while (input.Length != 0)
        {
            var list = input.Take(3).ToList();
            var aLine = list[0].Split(": ").Last().Split(", ").Select(x => x.Split('+').Last()).Select(int.Parse).ToList();
            var bLine = list[1].Split(": ").Last().Split(", ").Select(x => x.Split('+').Last()).Select(int.Parse).ToList();
            var prize = list[2].Split(": ").Last().Split(", ").Select(x => x.Split('=').Last()).Select(int.Parse).ToList();
            cases.Add(new Case((aLine[0], aLine[1]), (bLine[0], bLine[1]), (prize[0] + add, prize[1] + add)));

            input = input.Skip(4).ToArray();
        }

        return cases;
    }
}

public record Case((long x, long y) A, (long x, long y) B, (long x, long y) Prize);