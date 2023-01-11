// Copyright (c) Veeam Software Group GmbH

namespace AdventOfCode2022;

public class Day21 : ISolvable
{
    private readonly Dictionary<string, long> _numByMonkey = new();
    private readonly Dictionary<string, Operation> _opByMonkey = new();
    private readonly Dictionary<string, TreeNode> _treeIndex = new();

    public void Solve(String[] input)
    {
        var tree = ParseInput(input);
        var res1 = Calculate(tree);

        Console.WriteLine($"Part 1: {res1}");

        var neededNode = _treeIndex["humn"];
        neededNode.Op = "x";

        var cur = neededNode;
        var prev = neededNode;
        while (cur.Parent != null)
        {
            prev = cur;
            cur = cur.Parent;
        }

        var curRight = cur.Left.Name == prev.Name ? cur.Right : cur.Left;

        var res = Calculate(curRight);
        _numByMonkey[prev.Name] = res;

        var res2 = FindX(neededNode);

        Console.WriteLine($"Part 2: {res2}");
    }

    private long Calculate(TreeNode neededNode)
    {
        if (neededNode.Op == null)
            return _numByMonkey[neededNode.Name];

        var func = GetOp(neededNode.Op);
        return func(Calculate(neededNode.Left), Calculate(neededNode.Right));
    }

    private long FindX(TreeNode node)
    {
        if (node.Parent.Name == "root")
            return _numByMonkey[node.Name];
        if (node.Op == null)
            return _numByMonkey[node.Name];

        if (node.Parent.Right.Name == node.Name)
        {
            var func = GetOpForRight(node.Parent.Op);
            long l = func(FindX(node.Parent), Calculate(node.Parent.Left));
            _numByMonkey[node.Name] = l;
        }
        else
        {
            var func = GetOpForLeft(node.Parent.Op);
            long l = func(FindX(node.Parent), Calculate(node.Parent.Right));
            _numByMonkey[node.Name] = l;
        }

        return _numByMonkey[node.Name];
    }

    private TreeNode ParseInput(String[] input)
    {
        foreach (var line in input)
        {
            var parts = line.Split(": ").ToList();

            if (long.TryParse(parts[1], out var num))
            {
                _numByMonkey[parts[0]] = num;
                continue;
            }

            var opParts = parts[1].Split();
            _opByMonkey[parts[0]] = new Operation(opParts[0], opParts[2], opParts[1]);
        }

        return GetTreeNode("root", null);
    }

    private TreeNode GetTreeNode(string name, TreeNode parent)
    {
        var node = new TreeNode
        {
            Name = name,
            Parent = parent
        };
        _treeIndex[name] = node;
        if (!_opByMonkey.TryGetValue(name, out var op))
            return node;

        node.Left = GetTreeNode(op.LMonkey, node);
        node.Right = GetTreeNode(op.RMonkey, node);
        node.Op = op.op;

        return node;
    }

    private Func<long, long, long> GetOp(String part) => part switch
    {
        "+" => (l, r) => l + r,
        "-" => (l, r) => l - r,
        "*" => (l, r) => l * r,
        "/" => (l, r) => l / r,
        _ => throw new Exception()
    };

    private Func<long, long, long> GetOpForRight(String op) => op switch
    {
        "+" => (p, l) => p - l,
        "-" => (p, l) => l - p,
        "*" => (p, l) => p / l,
        "/" => (p, l) => l / p,
        _ => throw new Exception()
    };

    private Func<long, long, long> GetOpForLeft(String op) => op switch
    {
        "+" => (p, r) => p - r,
        "-" => (p, r) => r + p,
        "*" => (p, r) => p / r,
        "/" => (p, r) => r * p,
        _ => throw new Exception()
    };
}

public class TreeNode
{
    public string Name { get; set; }
    public string? Op { get; set; }
    public TreeNode? Left { get; set; }
    public TreeNode? Right { get; set; }
    public TreeNode? Parent { get; set; }
}

internal record Operation(string LMonkey, string RMonkey, string op);
// internal record Operation(string LMonkey, string RMonkey, Func<long, long, long> Func);