
using AdventOfCode2023;

var lines = File.ReadAllLines("input.txt");

var res1 = new Day1().SolvePart1(lines);
Console.WriteLine($"Part 1: {res1}");

var res2 = new Day1().SolvePart2(lines);
Console.WriteLine($"Part 2: {res2}");
