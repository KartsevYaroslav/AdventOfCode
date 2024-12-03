using System.Text;

namespace AdventOfCode2023;

public class Day1 : ISolvable
{
    public string SolvePart1(string[] input)
    {
        var sum = 0;
        foreach (var line in input)
        {
            var firstNum = 0;
            var curNum = 0;
            for (var i = 0; i < line.Length; i++)
            {
                if (!char.IsDigit(line[i]))
                    continue;

                curNum = line[i] - '0';
                firstNum = firstNum == 0 ? curNum : firstNum;
            }

            sum += firstNum * 10 + curNum;
        }

        return sum.ToString();
    }

    public string SolvePart2(string[] input)
    {
        var map = new Dictionary<string, int>
        {
            ["one"] = 1,
            ["two"] = 2,
            ["three"] = 3,
            ["four"] = 4,
            ["five"] = 5,
            ["six"] = 6,
            ["seven"] = 7,
            ["eight"] = 8,
            ["nine"] = 9,
        };
        var tmpBuilder = new StringBuilder(5);
        var resBuilder = new StringBuilder(5);
        var arr = new List<string>();
        foreach (var line in input)
        {
            for (var i = 0; i < line.Length; i++)
            {
                tmpBuilder.Clear();
                if (char.IsDigit(line[i]))
                {
                    resBuilder.Append(line[i]);
                    continue;
                }
                for (var j = i; j < line.Length; j++)
                {
                    tmpBuilder.Append(line[j]);

                    if (!map.TryGetValue(tmpBuilder.ToString(), out var num))
                        continue;
                    
                    resBuilder.Append(num);
                    break;
                }
            }
            arr.Add(resBuilder.ToString());
            resBuilder.Clear();
        }

        return SolvePart1(arr.ToArray());
    }
}