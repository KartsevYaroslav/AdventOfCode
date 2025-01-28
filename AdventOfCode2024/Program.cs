using System.Diagnostics;using AdventOfCode2024;

var lines = File.ReadAllLines("input.txt");

var stopwatch = new Stopwatch();
ISolvable<string> day = new Day24();
stopwatch.Start();
var res1 = day.SolvePart1(lines);
stopwatch.Stop();
Console.WriteLine($"Part 1: '{res1}', Elapsed: {stopwatch.ElapsedMilliseconds} ms");

stopwatch.Restart();
var res2 = day.SolvePart2(lines);
stopwatch.Stop();
Console.WriteLine($"Part 2: '{res2}', Elapsed: {stopwatch.ElapsedMilliseconds} ms");