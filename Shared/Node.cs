namespace Shared;

public class Node<T>
{
    public required T Value { get; set; }
    public Node<T>? Prev { get; set; }
    public Node<T>? Next { get; set; }

    public IEnumerable<T> GetAllValues()
    {
        var cur = this;
        while (cur != null)
        {
            yield return cur.Value;
            cur = cur.Next;
        }
    }
}

public static class Node
{
    public static Node<T> Create<T>(T value) => new()
    {
        Value = value
    };
}