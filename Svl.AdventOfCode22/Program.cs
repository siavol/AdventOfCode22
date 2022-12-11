// See https://aka.ms/new-console-template for more information

using Svl.AdventOfCode22;

Console.WriteLine("Advent Of Code 2022!");

var taskName = args[0];
switch (taskName)
{
    case "1":
    case "1p1":
    {
        RunTaskForTheDay(1, 1, Day1.SolveTask);
        break;
    }
    case "1p2":
    {
        RunTaskForTheDay(1, 2, Day1.SolveSecondPartTask);
        break;
    }
    case "2":
    case "2p1":
    {
        RunTaskForTheDay(2, 1, Day2.SolveTaskPart1);
        break;
    }
    case "2p2":
    {
        RunTaskForTheDay(2, 2, Day2.SolveTaskPart2);
        break;
    }
    case "3":
    case "3p1":
    {
        RunTaskForTheDay(3, 1, Day3.SolveTaskPart1);
        break;
    }
    case "3p2":
    {
        RunTaskForTheDay(3, 1, Day3.SolveTaskPart2);
        break;
    }
    default:
        throw new ApplicationException($"Unknown task {taskName}");
}

async Task RunTaskForTheDay(int day, int part, Func<Stream, int> task)
{
    Console.WriteLine($"Solving Day {day} task, Part {part}.");
    await using var stream = new FileStream($"Day{day}Input.txt", FileMode.Open);
    var result = task(stream);
    Console.WriteLine($"Day {day} task part {part} solution: {result}");
}