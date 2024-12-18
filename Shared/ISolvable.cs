public interface ISolvable<out TRes>
{
    TRes SolvePart1(string[] input);
    TRes SolvePart2(string[] input) => default!;
}