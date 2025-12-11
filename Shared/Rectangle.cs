namespace Shared;

public record Rectangle
{
    public Rectangle(Point LeftTop, Point RightBottom)
    {
        this.LeftTop = LeftTop;
        this.RightBottom = RightBottom;
        RightTop = LeftTop with { X = RightBottom.X };
        LeftBottom = LeftTop with { Y = RightBottom.Y };
    }

    public Point RightTop { get; }
    public Point LeftBottom { get; }
    public Point LeftTop { get; }
    public Point RightBottom { get; }
}