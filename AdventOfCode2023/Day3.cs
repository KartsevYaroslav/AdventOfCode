using System.Collections;
using System.Text;

public class Day3 : ISolvable<string>
{
    public string SolvePart1(string[] input)
    {
        var sum = 0L;
        for (int i = 0; i < input.Length; i++)
        {
            var builder = new StringBuilder();
            var found = false;
            for (int j = 0; j < input[0].Length; j++)
            {
                var ch = input[i][j];
                if (char.IsDigit(ch))
                {
                    builder.Append(ch);
                    if (found)
                        continue;
                    var foundSymbols = GetNeighbours(input, i, j).Where(x => input[x.i1][x.j1] != '.' && !char.IsDigit(input[x.i1][x.j1]));
                    if (foundSymbols.Any())
                        found = true;
                }
                else
                {
                    sum += found && builder.Length != 0 ? long.Parse(builder.ToString()) : 0;
                    found = false;
                    builder.Clear();
                }
            }

            sum += found && builder.Length != 0 ? long.Parse(builder.ToString()) : 0;
        }

        return sum.ToString();
    }

    private IEnumerable<(int i1, int j1)> GetNeighbours(string[] input, int i, int j)
    {
        if (i < input.Length - 1)
            yield return (i + 1, j);
        if (i > 0)
            yield return (i - 1, j);
        if (j < input[0].Length - 1)
            yield return (i, j + 1);
        if (j > 0)
            yield return (i, j - 1);
        if (i < input.Length - 1 && j < input[0].Length - 1)
            yield return (i + 1, j + 1);
        if (i > 0 && j > 0)
            yield return (i - 1, j - 1);
        if (i < input.Length - 1 && j > 0)
            yield return (i + 1, j - 1);
        if (i > 0 && j < input[0].Length - 1)
            yield return (i - 1, j + 1);
    }

    public string SolvePart2(string[] input)
    {
        var sum = 0L;
        for (int i = 0; i < input.Length; i++)
        {
            for (int j = 0; j < input[0].Length; j++)
            {
                var ch = input[i][j];
                if (ch != '*')
                    continue;
                var foundSymbols = GetNeighbours2(input, i, j).ToList();
                if (foundSymbols.Count < 2)
                    continue;

                if (foundSymbols.Count != 2)
                    throw new Exception();

                long num1 = GetNum(input, foundSymbols.First());
                long num2 = GetNum(input, foundSymbols.Last());

                sum += num1 * num2;
            }
        }

        return sum.ToString();
    }

    private int GetNum(string[] input, (int i, int j) pos)
    {
        var (i1, j1) = pos;
        while (j1 > 0 && char.IsDigit(input[i1][j1 - 1]))
            j1--;

        var builder = new StringBuilder();
        while (j1 < input[0].Length - 1 && char.IsDigit(input[i1][j1 + 1]))
        {
            builder.Append(input[i1][j1]);
            j1++;
        }

        if (char.IsDigit(input[i1][j1]))
            builder.Append(input[i1][j1]);

        return int.Parse(builder.ToString());
    }

    private IEnumerable<(int i1, int j1)> GetNeighbours2(string[] input, int i, int j)
    {
        if (i > 0)
            foreach (var pos in FindNumsPos(input, i - 1, j))
                yield return pos;

        if (i < input.Length - 1)
            foreach (var pos in FindNumsPos(input, i + 1, j))
                yield return pos;

        foreach (var pos in FindNumsPos(input, i, j))
            yield return pos;
    }

    private IEnumerable<(int i1, int j1)> FindNumsPos(string[] input, int i, int j)
    {
        if (char.IsDigit(input[i][j]))
        {
            yield return (i, j);
            yield break;
        }

        if (j > 0 && char.IsDigit(input[i][j - 1]))
            yield return (i, j - 1);

        if (j < input[0].Length - 1 && char.IsDigit(input[i][j + 1]))
            yield return (i, j + 1);
    }
}