public class Day8 : ISolvable
{
    public string SolvePart1(string[] input)
    {
        var instructions = input.First();
        var graph = ParseGraph(input.Skip(2));

        var curNode = "AAA";
        var turns = 0;
        for (var i = 0;; i++)
        {
            if (curNode == "ZZZ")
            {
                turns = i;
                break;
            }

            var i1 = i % instructions.Length;
            curNode = instructions[i1] == 'R' ? graph[curNode].right : graph[curNode].left;
        }

        return turns.ToString();
    }

    private Dictionary<string, (string left, string right)> ParseGraph(IEnumerable<string> input)
    {
        var dict = new Dictionary<string, (string, string)>();
        foreach (var line in input)
        {
            var parts = line.Split(" = ");
            var nodes = parts[1].Replace("(", "").Replace(")", "").Split(", ");
            dict[parts[0]] = (nodes[0], nodes[1]);
        }

        return dict;
    }


    public string SolvePart2(string[] input)
    {
        var instructions = input.First();
        var graph = ParseGraph(input.Skip(2));

        var startNodes = graph.Keys.Where(x => x.EndsWith("A")).ToList();

        var dict = new Dictionary<(string start, string end), long>();
        foreach (var node in startNodes)
        {
            var curNode = node;
            for (var i = 0L;; i++)
            {
                if (curNode.EndsWith("Z"))
                {
                    dict[(node, curNode)] = i;
                    break;
                }

                var i1 = (int) (i % instructions.Length);
                curNode = instructions[i1] == 'R' ? graph[curNode].right : graph[curNode].left;
            }
        }

        var nod = GetNod(dict.Values.ToArray());
        var res = dict.Values.Select(x=>x/nod).Aggregate((x,y) => x * y) * nod;
        
        return res.ToString();
    }

    private static long GetNod(long[] nums)
    {
        var nod = nums.Min();
        for (; nod > 1; nod--)
        {
            if (nums.All(x => x % nod == 0))
                break;
        }

        return nod;
    }
}