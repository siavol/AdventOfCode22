// See https://aka.ms/new-console-template for more information

using Svl.AdventOfCode22;

Console.WriteLine("Advent Of Code 2022!");

var taskName = args[0];
switch (taskName)
{
    case "1":
    case "1p1":
    {
        await RunTaskForTheDay(1, 1, Wrap(Day1.SolveTask));
        break;
    }
    case "1p2":
    {
        await RunTaskForTheDay(1, 2, Wrap(Day1.SolveSecondPartTask));
        break;
    }
    case "2":
    case "2p1":
    {
        await RunTaskForTheDay(2, 1, Wrap(Day2.SolveTaskPart1));
        break;
    }
    case "2p2":
    {
        await RunTaskForTheDay(2, 2, Wrap(Day2.SolveTaskPart2));
        break;
    }
    case "3":
    case "3p1":
    {
        await RunTaskForTheDay(3, 1, Wrap(Day3.SolveTaskPart1));
        break;
    }
    case "3p2":
    {
        await RunTaskForTheDay(3, 1, Wrap(Day3.SolveTaskPart2));
        break;
    }
    case "4":
    case "4p1":
    {
        await RunTaskForTheDay(4, 1, Wrap(Day4.SolveTaskPart1));
        break;
    }
    case "4p2":
    {
        await RunTaskForTheDay(4, 1, Wrap(Day4.SolveTaskPart2));
        break;
    }
    case "5":
    case "5p1":
    {
        await RunTaskForTheDay(5, 1, Day5.SolveTaskPart1);
        break;
    }
    case "5p2":
    {
        await RunTaskForTheDay(5, 2, Day5.SolveTaskPart2);
        break;
    }
    case "6":
    case "6p1":
    {
        await RunTaskForTheDay(6, 1, Wrap(Day6.SolveTaskPart1));
        break;
    }
    case "6p2":
    {
        await RunTaskForTheDay(6, 2, Wrap(Day6.SolveTaskPart2));
        break;
    }
    case "7":
    case "7p1":
    {
        await RunTaskForTheDay(7, 1, Wrap(Day7.SolveTaskPart1));
        break;
    }
    case "7p2":
    {
        await RunTaskForTheDay(7, 2, Wrap(Day7.SolveTaskPart2));
        break;
    }
    case "8":
    case "8p1":
    {
        await RunTaskForTheDay(8, 1, Wrap(Day8.GetVisibleTreeCount));
        break;
    }
    case "8p2":
    {
        await RunTaskForTheDay(8, 2, Wrap(Day8.GetMaxScenicScore));
        break;
    }
    case "9":
    case "9p1":
    {
        await RunTaskForTheDay(9, 1, Wrap(Day9.FindTailPositionsCount));
        break;
    }
    case "9p2":
    {
        await RunTaskForTheDay(9, 2, Wrap(Day9.FindLongRopeTailPositionsCount));
        break;
    }
    default:
        throw new ApplicationException($"Unknown task {taskName}");
}

async Task RunTaskForTheDay(int day, int part, Func<Stream, string> task)
{
    Console.WriteLine($"Solving Day {day} task, Part {part}.");
    await using var stream = new FileStream($"Day{day}Input.txt", FileMode.Open);
    var result = task(stream);
    Console.WriteLine($"Day {day} task part {part} solution: {result}");
}

Func<Stream, string> Wrap(Func<Stream, int> task) => stream => task(stream).ToString();
