namespace Shared;

public record struct Point(int X, int Y)
{
    public static implicit operator Point((int x, int y) pos) => new(pos.x, pos.y);
    public static implicit operator (int x, int y)(Point point) => (point.X, point.Y);

}