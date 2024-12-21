namespace Shared;

public class Direction
{
    public static (int, int)[] AllDirect => [(1, 0), (-1, 0), (0, 1), (0, -1)];
    public static readonly (int, int) North = (-1, 0);

    public static readonly (int, int) East = (0, 1);
}