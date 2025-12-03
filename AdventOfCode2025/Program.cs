using System.Diagnostics;

var lines = File.ReadAllLines("input.txt");

var stopwatch = new Stopwatch();
ISolvable<long> day = null;
stopwatch.Start();
var res1 = day.SolvePart1(lines);
stopwatch.Stop();
Console.WriteLine($"Part 1: '{res1}', Elapsed: {stopwatch.ElapsedMilliseconds} ms");

stopwatch.Restart();
var res2 = day.SolvePart2(lines);
stopwatch.Stop();
Console.WriteLine($"Part 2: '{res2}', Elapsed: {stopwatch.ElapsedMilliseconds} ms");