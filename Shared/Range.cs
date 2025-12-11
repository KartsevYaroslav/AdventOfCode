namespace Shared;

public record Range(long Start, long End)
{
    public bool IsInRange(long i) => i >= Start && i <= End || i >= End && i <= Start;
    public static implicit operator Range((int, int) range) => new Range(range.Item1, range.Item2);
}