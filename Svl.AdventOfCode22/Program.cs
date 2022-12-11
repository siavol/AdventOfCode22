// See https://aka.ms/new-console-template for more information

using Svl.AdventOfCode22;

Console.WriteLine("Advent Of Code 2022!");

var taskName = args[0];
switch (taskName)
{
    case "1":
    case "1p1":
    {
        Console.WriteLine("Solving Day 1 task, Part 1.");
        await using var stream = new FileStream("Day_1_Input.txt", FileMode.Open);
        var maxCalories = Day_1.SolveTask(stream);
        Console.WriteLine($"Day 1 task solution: {maxCalories}");
        break;
    }
    case "1p2":
    {
        Console.WriteLine("Solving Day 1 task, Part 2.");
        await using var stream = new FileStream("Day_1_Input.txt", FileMode.Open);
        var max3Calories = Day_1.SolveSecondPartTask(stream);
        Console.WriteLine($"Day 1 task part 2 solution: {max3Calories}");
        break;
    }
    default:
        throw new ApplicationException($"Unknown task {taskName}");
}

