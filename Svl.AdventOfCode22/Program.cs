// See https://aka.ms/new-console-template for more information

using Svl.AdventOfCode22;

Console.WriteLine("Hello, World!");

await using var stream = new FileStream("Day_1_Input.txt", FileMode.Open);
var maxCalories = Day_1.SolveTask(stream);
Console.WriteLine($"Day 1 task solution: {maxCalories}");