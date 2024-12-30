namespace AdventOfCode2024;

public class Day17 : ISolvable<string>
{
    public string SolvePart1(string[] input)
    {
        var regs = new[] {input[0], input[1], input[2]}.Select(x => x.Split().Last()).Select(long.Parse).ToArray();
        var commands = input.Last().Split().Last().Split(',').Select(int.Parse).ToArray();

        var output = Run(regs, commands);

        return string.Join(',', output);
    }


    public string SolvePart2(string[] input)
    {
        var commandStr = input.Last().Split().Last();
        var commands = commandStr.Split(',').Select(int.Parse).ToArray();

        return GetResult(commands).ToString();
    }

    private static List<long> Run(long[] regs, int[] commands)
    {
        var operands = new[] {0, 1, 2, 3, regs[0], regs[1], regs[2]};
        var pointer = 0;
        var output = new List<long>();

        var instructions = new Dictionary<int, Action<int>>
        {
            [0] = x => operands[4] = (long) (operands[4] / Math.Pow(2, operands[x])),
            [1] = x => operands[5] ^= x,
            [2] = x => operands[5] = operands[x] % 8,
            [3] = x =>
            {
                if (operands[4] == 0)
                    return;

                pointer = x;
            },
            [4] = _ => operands[5] ^= operands[6],
            [5] = x => output.Add(operands[x] % 8),
            [6] = x => operands[5] = (long) (operands[4] / Math.Pow(2, operands[x])),
            [7] = x => operands[6] = (long) (operands[4] / Math.Pow(2, operands[x]))
        };


        while (pointer < commands.Length)
        {
            var command = commands[pointer++];
            var operand = commands[pointer++];
            instructions[command](operand);
        }

        regs[0] = operands[4];
        regs[1] = operands[5];
        regs[2] = operands[6];

        return output;
    }

    private static long GetResult(int[] commands)
    {
        var queue = new Queue<(long num, int index)>();
        queue.Enqueue((0, commands.Length - 1));
        while (queue.Count != 0)
        {
            var (curAReg, index) = queue.Dequeue();
            if (index == -1)
                return curAReg;
            // for each digit in output only 3 last bits matter
            for (var possibleReminder = 0; possibleReminder < 8; possibleReminder++)
            {
                var newAReg = (curAReg << 3) + possibleReminder;
                var output = Run([newAReg, 0, 0], commands);
                if (output.First() == commands[index])
                    queue.Enqueue((newAReg, index - 1));
            }
        }

        return -1;
    }
}