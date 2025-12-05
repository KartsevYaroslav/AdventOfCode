using System.Linq;
using Shared;

namespace AdventOfCode2025;

public class Day4 : ISolvable<long>
{
    public long SolvePart1(string[] input)
    {
        var array = input.ToCharArray();
        var res = 0;
        for (var i = 0; i < array.Length; i++)
        for (var j = 0; j < array[0].Length; j++)
        {
            if (array[i][j] != '@')
                continue;

            var count = array.GetAllNeighbours((i, j)).Count(x => array.At(x) == '@');
            if (count < 4)
                res++;
        }

        return res;
    }

    public long SolvePart2(string[] input)
    {
        var array = input.ToCharArray();
        var res = 0;
        while (true)
        {
            var before = array.SelectMany(x => x).Count(x => x == '@');


            for (var i = 0; i < array.Length; i++)
            for (var j = 0; j < array[0].Length; j++)
            {
                if (array[i][j] != '@')
                    continue;

                var count = array.GetAllNeighbours((i, j)).Count(x => array.At(x) == '@');
                if (count >= 4)
                    continue;

                array[i][j] = '.';
                res++;
            }

            var after = array.SelectMany(x => x).Count(x => x == '@');
            if (before == after)
                break;
        }

        return res;
    }
}