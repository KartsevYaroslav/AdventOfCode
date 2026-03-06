using System.Diagnostics;
using AdventOfCode2023;

var lines = File.ReadAllLines("input.txt");

var stopwatch = new Stopwatch();
ISolvable<long> day = new Day9();
stopwatch.Start();
var res1 = await day.SolvePart1Async(lines);
stopwatch.Stop();
Console.WriteLine($"Part 1: '{res1}', Elapsed: {stopwatch.ElapsedMilliseconds} ms");

stopwatch.Restart();
var res2 = await day.SolvePart2Async(lines);
stopwatch.Stop();
Console.WriteLine($"Part 2: '{res2}', Elapsed: {stopwatch.ElapsedMilliseconds} ms");