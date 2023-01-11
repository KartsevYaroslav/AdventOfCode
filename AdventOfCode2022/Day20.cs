// Copyright (c) Veeam Software Group GmbH

namespace AdventOfCode2022;

public class Day20 : ISolvable
{
    public void Solve(String[] input)
    {
        var nums = input.Select(long.Parse).ToList();

        var tmp = nums.ToList();
        new MessageMixer(nums.ToList()).MixMessage();
        Console.WriteLine($"Part 1: {GetResult(tmp)}");

        
        tmp = nums.Select(x => x * 811589153).ToList();
        var messageMixer = new MessageMixer(tmp);
        for (int j = 0; j < 10; j++)
            messageMixer.MixMessage();

        Console.WriteLine($"Part 2: {GetResult(tmp)}");
    }

    private static Int64 GetResult(List<Int64> tmp)
    {
        var index = tmp.Select((x, i) => (x, i)).Where(x => x.x == 0).Select(x => x.i).Single();
        return tmp[(index + 1000) % tmp.Count] + tmp[(index + 2000) % tmp.Count] + tmp[(index + 3000) % tmp.Count];
    }
}

internal class MessageMixer
{
    private readonly List<Int64> _message;
    private readonly Dictionary<int, int> _curIndexByStartedIndex = new();
    private readonly Dictionary<int, int> _startIndexByCurIndex = new();

    public MessageMixer(List<Int64> message)
    {
        _message = message;
        
        for (int i = 0; i < _message.Count; i++)
        {
            _curIndexByStartedIndex[i] = i;
            _startIndexByCurIndex[i] = i;
        }
    }
    public void MixMessage()
    {
        for (int i = 0; i < _message.Count; i++)
        {
            var curI = _curIndexByStartedIndex[i];
            var movingCount = GetMovingCount(_message, curI);
            if (_message[curI] > 0)
                MoveRight(_message, curI, movingCount);
            else
                MoveLeft(_message, curI, movingCount);
        }
    }

    private static Int64 GetMovingCount(List<Int64> tmp, Int32 curI)
    {
        var cnt = Math.Abs(tmp[curI]);
        while (cnt > tmp.Count)
            cnt = cnt % tmp.Count + cnt / tmp.Count;

        return cnt;
    }

    private void MoveRight(List<long> buff, Int32 curI, long count)
    {
        var l = curI;
        var r = curI + 1;
        while (count-- > 0)
        {
            l %= buff.Count;
            r %= buff.Count;

            Swap(buff, l, r);
            UpdateIndexes(l, r);

            l++;
            r++;
        }
    }

    private void MoveLeft(List<long> buff, Int32 curI, long count)
    {
        var l = curI;
        var r = curI - 1;
        while (count-- > 0)
        {
            l = l < 0 ? buff.Count - 1 : l;
            r = r < 0 ? buff.Count - 1 : r;
            Swap(buff, l, r);
            UpdateIndexes(l, r);

            l--;
            r--;
        }
    }

    private void UpdateIndexes(Int32 l, Int32 r)
    {
        var startedL = _startIndexByCurIndex[l];
        var startedR = _startIndexByCurIndex[r];
        _curIndexByStartedIndex[startedR] = l;
        _curIndexByStartedIndex[startedL] = r;
        _startIndexByCurIndex[l] = startedR;
        _startIndexByCurIndex[r] = startedL;
    }

    private static void Swap(List<long> buff, Int32 curI, Int32 newI) => (buff[curI], buff[newI]) = (buff[newI], buff[curI]);
}