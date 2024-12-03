public class Day2 : ISolvable
{
    public void SolvePart1(string[] input)
    {
        var maxValues = new Dictionary<string, int>
        {
            ["red"] = 12,
            ["blue"] = 14,
            ["green"] = 13
        };
        var sum = 0;
        for (int i = 0; i < input.Length; i++)
        {
            var parts = input[i].Split(':');
            var sets = parts[1].Split(';');
            var possible = true;
            foreach (var set in sets)
            foreach (var cube in set.Split(','))
            {
                var cubeParts = cube.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (maxValues[cubeParts[1]] < int.Parse(cubeParts[0]))
                    possible = false;
            }

            if (possible)
                sum += i + 1;
        }

        Console.WriteLine(sum);
    }

    public void SolvePart2(string[] input)
    {
        
        var sum = 0;
        foreach (var line in input)
        {
            var maxValues = new Dictionary<string, int>
            {
                ["red"] = 0,
                ["blue"] = 0,
                ["green"] = 0
            };
            var sets = line.Split(':')[1].Split(';');
            foreach (var set in sets)
            foreach (var cube in set.Split(','))
            {
                var cubeParts = cube.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                maxValues[cubeParts[1]] = Math.Max(int.Parse(cubeParts[0]), maxValues[cubeParts[1]]);
            }

            sum += maxValues.Values.Aggregate((x, y) => x * y);
        }

        Console.WriteLine(sum);
    }
}