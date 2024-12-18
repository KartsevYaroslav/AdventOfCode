namespace AdventOfCode2024;

public class Day9 : ISolvable<string>
{
    public string SolvePart1(string[] input)
    {
        var line = input[0].Select(x => x - '0').ToArray();
        var right = line.Length % 2 == 0 ? line.Length - 2 : line.Length - 1;
        var left = 0;

        var files = new List<(int, int)>();

        while (left <= right)
        {
            if (left % 2 == 0)
            {
                files.Add((left / 2, line[left]));
                left++;
                continue;
            }

            var id = right / 2;
            var freeSpace = line[left];
            var fileSize = line[right];

            if (freeSpace < fileSize)
            {
                line[right] -= freeSpace;
                line[left] = 0;
                files.Add((id, freeSpace));
            }
            else if (freeSpace != 0)
            {
                line[left] -= fileSize;
                line[right] = -1;
                right -= 2;
                files.Add((id, fileSize));
            }

            if (line[left] <= 0)
                left++;
        }

        var res = GetResult(files);
        return res.ToString();
    }

    public string SolvePart2(string[] input)
    {
        var line = input[0].Select(x => x - '0').ToArray();
        var right = line.Length % 2 == 0 ? line.Length - 2 : line.Length - 1;
        var left = 0;

        var files = new List<(int id, int size, int index)>();

        while (left < line.Length)
        {
            if (left % 2 == 0 && line[left] > 0)
            {
                files.Add((left / 2, line[left], left));
                left++;
                continue;
            }

            if (left >= right)
            {
                left++;
                continue;
            }

            var id = right / 2;

            var leftTpm = left;
            var rightTpm = right;
            while (leftTpm < rightTpm && line[leftTpm] < line[rightTpm])
                leftTpm += 2;

            if (leftTpm < rightTpm)
            {
                line[leftTpm] -= line[rightTpm];
                files.Add((id, line[rightTpm], leftTpm));
                files.Add((0, line[rightTpm], rightTpm));
                line[rightTpm] = 0;
            }

            right -= 2;

            if (line[left] <= 0)
                left++;
        }

        for (var j = 0; j < line.Length; j++)
        {
            if (j % 2 != 0 && line[j] != 0)
                files.Add((0, line[j], j));
        }

        var valueTuples = files.OrderBy(x => x.Item3).Select(x => (x.Item1, x.Item2)).ToList();

        return GetResult(valueTuples).ToString();

    }

    private static long GetResult(List<(int, int)> files)
    {
        var i = 0L;
        var res = 0L;
        foreach (var (id, length) in files)
        {
            for (int j = 0; j < length; j++)
            {
                res += id * i;
                i++;
            }
        }

        return res;
    }
}