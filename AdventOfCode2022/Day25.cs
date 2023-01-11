// Copyright (c) Veeam Software Group GmbH

namespace AdventOfCode2022;

public class Day25 : ISolvable
{
    public void Solve(String[] input)
    {
        var sum = input.Select(x => x.Select(ToNum).ToArray()).Select(FromSnafu).Sum();

        var res = string.Join("", ToSnafu(sum).Select(ToChar));
        Console.WriteLine(res);
    }

    public long FromSnafu(int[] snafu)
    {
        var res = 0L;
        for (int i = 0; i < snafu.Length; i++)
        {
            var pow = snafu.Length - i - 1;
            var num = (long) Math.Pow(5L, pow) * snafu[i];
            res += num;
        }

        return res;
    }

    public int[] ToSnafu(long num)
    {
        var prev = 1L;
        var cur = prev;
        while (cur < num)
        {
            prev = cur;
            cur *= 5;
        }

        var list = new List<int>();
        var tmp = num;
        while (cur != 0)
        {
            var reminder = GetReminder(prev);
            if (Math.Abs(tmp - cur * 2) <= reminder)
            {
                list.Add(2);
                tmp -= cur * 2;
            }
            else if (Math.Abs(tmp - cur) <= reminder)
            {
                list.Add(1);
                tmp -= cur;
            }
            else if (-tmp >= cur * 2 - reminder)
            {
                list.Add(-2);
                tmp += cur * 2;
            }
            else if (-tmp >= cur - reminder)
            {
                list.Add(-1);
                tmp += cur;
            }
            else
            {
                list.Add(0);
            }

            cur = prev;
            prev /= 5;
        }

        return list.SkipWhile(x => x == 0).ToArray();
    }

    private char ToChar(Int32 arg)
    {
        return arg switch
        {
            0 => '0',
            1 => '1',
            2 => '2',
            -1 => '-',
            -2 => '=',
            _ => throw new ArgumentOutOfRangeException(nameof(arg), arg, null)
        };
    }

    private int ToNum(Char arg)
    {
        return arg switch
        {
            '0' => 0,
            '1' => 1,
            '2' => 2,
            '=' => -2,
            '-' => -1,
            _ => throw new ArgumentOutOfRangeException(nameof(arg), arg, null)
        };
    }

    private static long GetReminder(long prev)
    {
        var c = prev;
        var t = 0L;
        while (c > 0)
        {
            t += c * 2;
            c /= 5;
        }

        return t;
    }
}