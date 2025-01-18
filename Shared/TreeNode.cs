namespace Shared;

public class TreeNode<T>
{
    public T? Value { get; set; }
    public TreeNode<T>? Parent { get; private set; }
    public int Depth { get; private set; } = 1;
    private readonly List<TreeNode<T>> _children = [];
    public IReadOnlyCollection<TreeNode<T>> Children => _children;

    private void IncreaseDepth(int depth)
    {
        Depth+=depth;
        Parent?.IncreaseDepth(depth);
    }

    public void AddChild(TreeNode<T> node)
    {
        node.Parent = this;
        if (_children.Count == 0)
            IncreaseDepth(node.Depth);
        _children.Add(node);
    }

    public void AddChildren(IEnumerable<TreeNode<T>> nodes)
    {
        foreach (var node in nodes)
            AddChild(node);
    }

    public IEnumerable<TreeNode<T>> GetLeafs()
    {
        if (Children.Count == 0)
        {
            yield return this;
            yield break;
        }

        foreach (var leaf in Children.SelectMany(child => child.GetLeafs()))
            yield return leaf;
    }

    public TreeNode<T> GetRoot() => Parent is null ? this : Parent.GetRoot();

    public int GetMaxDepth()
    {
        if (Children.Count == 0)
            return 1;

        return 1 + Children.Max(x => x.GetMaxDepth());
    }

    public int GetMinDepth()
    {
        if (Children.Count == 0)
            return 1;

        return 1 + Children.Min(x => x.GetMinDepth());
    }
}