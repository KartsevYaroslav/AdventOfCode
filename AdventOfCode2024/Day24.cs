using System.Globalization;

namespace AdventOfCode2024;

public class Day24 : ISolvable<string>
{
    public string SolvePart1(string[] input)
    {
        var wires = input.TakeWhile(x => !string.IsNullOrEmpty(x))
                         .Select(x => x.Split(": "))
                         .ToDictionary(x => x[0], x => int.Parse(x[1]));

        var instructions = input.SkipWhile(x => !string.IsNullOrEmpty(x)).Skip(1).ToArray();
        wires = Solve(instructions, wires);

        return GetNum(wires, 'z').ToString();
    }

    public string SolvePart2(string[] input)
    {
        var instructions = input.SkipWhile(x => !string.IsNullOrEmpty(x)).Skip(1).ToArray();

        var gateByOutput = instructions.ToDictionary(x => Gate.Parse(x).Output, Gate.Parse);
        var gateByInput = new Dictionary<string, List<Gate>>();
        foreach (var gate in gateByOutput.Values)
        {
            gateByInput.TryAdd(gate.Left, []);
            gateByInput.TryAdd(gate.Right, []);

            gateByInput[gate.Left].Add(gate);
            gateByInput[gate.Right].Add(gate);
        }

        var incorrect = new HashSet<string>();
        foreach (var gate in gateByOutput.Values)
        {
            if (gate.IsInitial() && gate.Operand == "OR")
                incorrect.Add(gate.Output);

            if (gate.Operand == "AND" && (gate.IsFinal() || NotFirstPair(gate) && gateByInput[gate.Output].Any(x => x.Operand != "OR")))
                incorrect.Add(gate.Output);

            if (gate.Operand == "XOR" && !gate.IsFinal() && gateByInput[gate.Output].Any(x => x.Operand == "OR"))
                incorrect.Add(gate.Output);

            if (gate.Operand == "OR" && gate.Output != "z45" && (gate.IsFinal() || gateByInput[gate.Output].Any(x => x.Operand == "OR")))
                incorrect.Add(gate.Output);

            if (gate.Operand == "XOR" && !gate.IsInitial() && !gate.IsFinal() && (CorrectXor(gate.Left, gate.Right) || CorrectXor(gate.Right, gate.Left)))
                incorrect.Add(gate.Output);

            if (gate.IsFinal() && gate.IsInitial() && gate.Output != "z00")
                incorrect.Add(gate.Output);
        }

        return string.Join(",", incorrect.OrderBy(x => x));

        bool CorrectXor(string l, string r) => gateByOutput[l].Operand == "XOR" && gateByOutput[l].IsInitial() && gateByOutput[r].Operand == "OR";
        bool NotFirstPair(Gate gate) => gate.Left != "x00" && gate.Right != "x00";
    }

    private static long GetNum(Dictionary<string, int> wires, char ch)
    {
        var binaryNums = wires
                         .Where(x => x.Key.StartsWith(ch))
                         .OrderBy(x => int.Parse(x.Key[1..]))
                         .Select(x => x.Value)
                         .Reverse();

        return long.Parse(string.Concat(binaryNums), NumberStyles.BinaryNumber);
    }

    private static Dictionary<string, int> Solve(string[] instructions, Dictionary<string, int> wires)
    {
        wires = wires.ToDictionary(x => x.Key, x => x.Value);
        var funcs = new Dictionary<string, Action>();
        var unprocessed = new HashSet<string>();
        foreach (var line in instructions)
        {
            var parts = line.Split(" -> ");
            var instr = parts[0].Split();
            unprocessed.Add(line);
            Func<int, int, int> func = instr[1] switch
            {
                "XOR" => (x, y) => x ^ y,
                "OR" => (x, y) => x | y,
                "AND" => (x, y) => x & y,
                _ => throw new ArgumentOutOfRangeException()
            };
            var act = () =>
            {
                wires[parts[1]] = func(wires[instr[0]], wires[instr[2]]);
                unprocessed.Remove(line);
            };
            funcs[line] = act;
        }

        while (unprocessed.Count != 0)
        {
            var prev = unprocessed.Count;
            foreach (var line in unprocessed)
            {
                var strings = line.Split(" -> ")[0].Split();
                if (wires.ContainsKey(strings[0]) && wires.ContainsKey(strings[2]))
                    funcs[line]();
            }

            if (prev == unprocessed.Count)
                throw new Exception();
        }

        return wires;
    }
}

internal record Gate(string Left, string Right, string Operand, string Output)
{
    public bool IsInitial() => Left.StartsWith('x') || Right.StartsWith('x');
    public bool IsFinal() => Output.StartsWith('z');

    public static Gate Parse(string input)
    {
        var outByIn = input.Split(" -> ");
        var inParts = outByIn[0].Split();

        return new Gate(inParts[0], inParts[2], inParts[1], outByIn[1]);
    }
}