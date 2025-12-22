public interface ISolvable<TRes>
{
    TRes SolvePart1(string[] input) => default!;
    TRes SolvePart2(string[] input) => default!;
    Task<TRes> SolvePart1Async(string[] input) => Task.FromResult(SolvePart1(input));
    Task<TRes> SolvePart2Async(string[] input) => Task.FromResult(SolvePart2(input));
}