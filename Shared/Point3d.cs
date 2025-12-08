namespace Shared;

public record struct Point3d(int X, int Y, int Z)
{
    public static implicit operator Point3d((int x, int y, int z) pos) => new(pos.x, pos.y, pos.z);
    public static implicit operator (int x, int y, int z)(Point3d point) => (point.X, point.Y, point.Z);

}