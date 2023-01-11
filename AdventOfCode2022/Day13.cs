// Copyright (c) Veeam Software Group GmbH

namespace AdventOfCode2022;

public class Day13 : ISolvable
{
    public void Solve(String[] input)
    {
        var arrayComparer = new ArrayComparer();

        var res = 0;
        for (int i = 0; i < input.Length; i++)
        {
            var (arr1, _) = ParseArray(input[i++][1..]);
            var (arr2, _) = ParseArray(input[i++][1..]);

            if (arrayComparer.Compare(arr1, arr2) == -1)
                res += (i + 1) / 3;
        }

        Console.WriteLine($"Part 1: {res}");

        var (divider1, _) = ParseArray("[2]]");
        var (divider2, _) = ParseArray("[6]]");

        var arrays = input.Where(x => !string.IsNullOrEmpty(x))
                          .Select(x => ParseArray(x[1..]).arr)
                          .Concat(new[] {divider1, divider2})
                          .ToList();

        var orderedArrays = arrays.OrderBy(x => x, arrayComparer).ToList();

        var index1 = orderedArrays.FindIndex(x => arrayComparer.Compare(divider1, x) == 0) + 1;
        var index2 = orderedArrays.FindIndex(x => arrayComparer.Compare(divider2, x) == 0) + 1;

        Console.WriteLine($"Part 2: {index1 * index2}");
    }

    private (object[] arr, int length) ParseArray(String line)
    {
        var buff = new List<object>();

        for (var i = 0; i < line.Length; i++)
        {
            switch (line[i])
            {
                case ',': continue;
                case '[':
                    var (arr, length) = ParseArray(line[++i..]);
                    buff.Add(arr);
                    i += length;
                    continue;
                case ']': return (buff.ToArray(), i);
            }

            var num = line[i] - '0';
            while (char.IsDigit(line[i + 1]))
                num = num * 10 + (line[++i] - '0');

            buff.Add(num);
        }

        throw new Exception();
    }

    private void PrintArray(object[] arr)
    {
        Console.Write('[');

        for (var i = 0; i < arr.Length; i++)
        {
            var item = arr[i];
            if (item is object[] innerArr)
                PrintArray(innerArr);

            else
                Console.Write(item);

            if (i < arr.Length - 1)
                Console.Write(',');
        }

        Console.Write(']');
    }
}

public class ArrayComparer : IComparer<Object[]>
{
    public Int32 Compare(Object[]? arr1, Object[]? arr2)
    {
        var maxLength = Math.Max(arr1!.Length, arr2!.Length);
        for (int i = 0; i < maxLength; i++)
        {
            if (i == arr1.Length || i == arr2.Length)
                return arr1.Length < arr2.Length ? -1 : 1;

            var compareRes = CheckObjects(arr1[i], arr2[i]);

            if (compareRes != 0)
                return compareRes;
        }

        return 0;
    }

    private int CheckObjects(Object left, Object right) =>
        left switch
        {
            int ln when right is int rn && rn == ln => 0,
            int ln when right is int rn => ln < rn ? -1 : 1,
            int ln when right is object[] rArr => Compare(new object[] {ln}, rArr),
            object[] lArr when right is int rn => Compare(lArr, new object[] {rn}),
            object[] lArr when right is object[] rArr => Compare(lArr, rArr),
            _ => 0
        };
}