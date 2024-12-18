using Shared;

namespace AdventOfCode2024;

public class Day4 : ISolvable<string>
{
    public string SolvePart1(string[] input)
    {
        var res = 0;
        for (var i = 0; i < input.Length; i++)
        for (var j = 0; j < input[0].Length; j++)
        {
            res += FindXmas(input, i, j);
        }

        return res.ToString();
    }

    public string SolvePart2(string[] input)
    {
        var res = 0;
        for (var i = 0; i < input.Length; i++)
        for (var j = 0; j < input[0].Length; j++)
        {
            if (FindMasAsX(input, i, j))
                res++;
        }

        return res.ToString();
    }

    private static int FindXmas(string[] input, int i, int j)
    {
        var diffs = new[] {(0, 1), (1, 0), (0, -1), (-1, 0), (-1, -1), (1, 1), (-1, 1), (1, -1)};

        return diffs.Count(diff => FindWord(input, (i, j), diff, "XMAS"));
    }

    private static bool FindMasAsX(string[] input, int i, int j)
    {
        var foundLeft = false;
        var foundRight = false;
        foreach (var parts in new[] {("AS", "AM"), ("AM", "AS")})
        {
            if (FindWord(input, (i, j), (-1, -1), parts.Item1) &&
                FindWord(input, (i, j), (1, 1), parts.Item2))
                foundLeft = true;

            if (FindWord(input, (i, j), (-1, 1), parts.Item1) &&
                FindWord(input, (i, j), (1, -1), parts.Item2))
                foundRight = true;
        }

        return foundLeft && foundRight;
    }

    private static bool FindWord(string[] input, (int i, int j) pos, (int i, int j) diff, string word)
    {
        var curLet = 0;
        while (true)
        {
            if (curLet == word.Length)
                return true;

            if (input.OutOfBorders(pos) || input[pos.i][pos.j] != word[curLet++])
                return false;

            pos = (i: pos.i + diff.i, j: pos.j + diff.j);
        }

    }
}