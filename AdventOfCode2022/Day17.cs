// Copyright (c) Veeam Software Group GmbH

namespace AdventOfCode2022;

public class Day17 : ISolvable
{
    public void Solve(String[] input)
    {
        var commands = input.First();

        var simulation = new RockFallingSimulation();
        var res1 = simulation.GetHeightByRocksCount(commands, 2022);
        Console.WriteLine($"Part 1: {res1}");

        var res2 = simulation.GetHeightByRocksCount(commands, 1000000000000L);
        Console.WriteLine($"Part 2: {res2}");
    }
}

public class RockFallingSimulation
{
    readonly IReadOnlyList<IShape> _shapes =
        new IShape[] {HorLineShape.Default, PlusShape.Default, CornerShape.Default, VertLineShape.Default, SquareShape.Default}
            .Select(s => s.Move(2, 0))
            .ToList();

    public Int64 GetHeightByRocksCount(String commands, Int64 targetRocks)
    {
        var linkedList = new LinkedList<byte>();
        var memo = new FieldMemo(targetRocks);

        var hShift = 0L;
        var curCmd = 0;
        var curRockNum = 0L;

        while (true)
        {
            var curRockIndex = (int) (curRockNum % _shapes.Count);
            var curRock = _shapes[curRockIndex];
            while (linkedList.Count != 0 && linkedList.First.Value == 0)
                linkedList.RemoveFirst();

            if (curRockNum == targetRocks)
                return linkedList.Count + hShift;

            (curRockNum, hShift) = memo.ShiftStateIfPossible(curCmd, curRockIndex, curRockNum, hShift, linkedList.Count);

            for (int i = 0; i < 3 + curRock.Height; i++)
                linkedList.AddFirst(0);

            AddRockToField(curRock, linkedList);
            curCmd = LetRockFall(linkedList.First, commands, curCmd, curRock);
            curRockNum++;
        }
    }

    private static void AddRockToField(IShape curRock, LinkedList<Byte> linkedList)
    {
        foreach (var p in curRock.Points)
        {
            var curNode = linkedList.First;
            for (int i = 0; i < p.Y; i++)
                curNode = curNode.Next;
            curNode.Value = (byte) ((1 << p.X) | curNode.Value);
        }
    }

    private Int32 LetRockFall(LinkedListNode<Byte>? curNode, String commands, Int32 curCmd, IShape curRock)
    {
        while (true)
        {
            var diff = commands[curCmd] == '>' ? (1, 0) : (-1, 0);
            if (MoveIsPossible(curNode, diff, curRock))
            {
                Move(curNode, curRock, diff);
                curRock = curRock.Move(diff.Item1, diff.Item2);
            }

            curCmd = (curCmd + 1) % commands.Length;

            if (!MoveIsPossible(curNode, (0, 1), curRock))
                break;

            Move(curNode, curRock, (0, 1));
            curNode = curNode.Next;
        }

        return curCmd;
    }

    private Boolean MoveIsPossible(LinkedListNode<Byte>? node, (int dx, int dy) diff, IShape curShape)
    {
        return curShape.Points
                       .Select(p => p.Move(diff.dx, diff.dy))
                       .Except(curShape.Points)
                       .All(point => point is {X: >= 0 and < 7, Y: >= 0} && IsEmpty(point));

        Boolean IsEmpty(Point p)
        {
            var cur = node;
            for (int i = 0; i < p.Y; i++)
                cur = cur.Next;

            if (cur == null)
                return false;

            var checkPosition = 1 << p.X;

            return (cur.Value & checkPosition) != checkPosition;
        }
    }

    private void Move(LinkedListNode<Byte>? node, IShape curShape, (Int32 dx, Int32 dy) diff)
    {
        foreach (var point in curShape.Points)
        {
            var cur = node;
            for (int i = 0; i < point.Y; i++)
                cur = cur.Next;

            var place = (byte) ~(1 << point.X);
            cur.Value = (byte) (cur.Value & place);
        }

        foreach (var point in curShape.Points)
        {
            var cur = node;
            for (int i = 0; i < point.Y + diff.dy; i++)
                cur = cur.Next;

            var place = (byte) (1 << (point.X + diff.dx));
            cur.Value = (byte) (cur.Value | place);
        }
    }
}

public class FieldMemo
{
    private readonly Dictionary<(int rockI, int cmdI), (int height, long rCount)> _memo = new();
    private int _matchCount;
    private bool _isFound;
    private readonly Int64 _targetRocks;

    public FieldMemo(Int64 targetRocks)
    {
        _targetRocks = targetRocks;
    }

    public (long newRockNum, long newShift) ShiftStateIfPossible(Int32 curCmd, int curRockIndex, Int64 curRockNum, long curShift, Int32 curH)
    {
        if (_isFound)
            return (curRockNum, curShift);

        if (_memo.TryGetValue((curRockIndex, curCmd), out var val))
        {
            _matchCount++;
            if (!_isFound && _matchCount == 10)
            {
                var rDiff = curRockNum - val.rCount;
                var hDiff = curH - val.height;
                var cyclesCount = (_targetRocks - val.rCount) / rDiff;

                curRockNum = rDiff * cyclesCount + val.rCount;
                curShift = hDiff * cyclesCount + val.height;
                curShift -= curH;

                _isFound = true;
            }
        }
        else
        {
            _matchCount = 0;
        }

        if (!_memo.ContainsKey((curRockIndex, curCmd)))
            _memo[(curRockIndex, curCmd)] = (curH, curRockNum);

        return (curRockNum, curShift);
    }
}

public interface IShape
{
    IEnumerable<Point> Points { get; }
    int Height { get; }

    public IShape Move(int dx, int dy);
}

public class HorLineShape : IShape
{
    private HorLineShape(IEnumerable<Point> points)
    {
        Points = points;
    }

    public static IShape Default => new HorLineShape(new[]
    {
        new Point(0, 0),
        new Point(1, 0),
        new Point(2, 0),
        new Point(3, 0)
    });

    public IEnumerable<Point> Points { get; }
    public Int32 Height => 1;

    public IShape Move(Int32 dx, Int32 dy) => new HorLineShape(Points.Select(p => p.Move(dx, dy)));
}

public class PlusShape : IShape
{
    private PlusShape(IEnumerable<Point> points)
    {
        Points = points;
    }

    public static IShape Default => new PlusShape(new[]
    {
        new Point(1, 0),
        new Point(1, 1),
        new Point(1, 2),
        new Point(0, 1),
        new Point(2, 1)
    });

    public IEnumerable<Point> Points { get; }
    public Int32 Height => 3;

    public IShape Move(Int32 dx, Int32 dy) => new PlusShape(Points.Select(p => p.Move(dx, dy)));
}

public class CornerShape : IShape
{
    private CornerShape(IEnumerable<Point> points)
    {
        Points = points;
    }

    public static IShape Default => new CornerShape(new[]
    {
        new Point(2, 0),
        new Point(2, 1),
        new Point(2, 2),
        new Point(1, 2),
        new Point(0, 2)
    });

    public IEnumerable<Point> Points { get; }

    public Int32 Height => 3;

    public IShape Move(Int32 dx, Int32 dy) => new CornerShape(Points.Select(p => p.Move(dx, dy)));
}

public class VertLineShape : IShape
{
    private VertLineShape(IEnumerable<Point> points)
    {
        Points = points;
    }

    public static IShape Default => new VertLineShape(new[]
    {
        new Point(0, 0),
        new Point(0, 1),
        new Point(0, 2),
        new Point(0, 3)
    });

    public IEnumerable<Point> Points { get; }
    public Int32 Height => 4;

    public IShape Move(Int32 dx, Int32 dy) => new VertLineShape(Points.Select(p => p.Move(dx, dy)));
}

public class SquareShape : IShape
{
    private SquareShape(IEnumerable<Point> points)
    {
        Points = points;
    }

    public static IShape Default => new SquareShape(new[]
    {
        new Point(0, 0),
        new Point(0, 1),
        new Point(1, 0),
        new Point(1, 1)
    });
    public IEnumerable<Point> Points { get; }
    public Int32 Height => 2;

    public IShape Move(Int32 dx, Int32 dy) => new SquareShape(Points.Select(p => p.Move(dx, dy)));
}