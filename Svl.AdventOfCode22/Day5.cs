using System.Text;
using System.Text.RegularExpressions;
using Xunit;

namespace Svl.AdventOfCode22;

public static partial class Day5
{
    public static string SolveTaskPart1(Stream stream)
    {
        return SolveGeneralTask(stream, (itemsCount, fromStack, toStack) =>
        {
            for (var i = 0; i < itemsCount; i++)
            {
                var item = fromStack.Pop();
                toStack.Push(item);
            }
        });
    }

    public static string SolveTaskPart2(Stream stream)
    {
        return SolveGeneralTask(stream, (itemsCount, fromStack, toStack) =>
        {
            var tmpStack = new Stack<char>();
            for (var i = 0; i < itemsCount; i++)
            {
                var item = fromStack.Pop();
                tmpStack.Push(item);
            }

            foreach (var item in tmpStack)
            {
                toStack.Push(item);
            }
        });
    }

    private static string SolveGeneralTask(Stream stream, Action<int, Stack<char>, Stack<char>> moveItems)
    {
        using var streamReader = new StreamReader(stream);
        var stacks = ParseStacks(streamReader);

        // skip empty line
        streamReader.ReadLine();

        while (!streamReader.EndOfStream)
        {
            var line = streamReader.ReadLine();
            var moveInstruction = ParseMoveExpression(line);
            var fromStack = stacks[moveInstruction.From - 1];
            var toStack = stacks[moveInstruction.To - 1];
            var itemsCount = moveInstruction.ItemsCount;
            moveItems(itemsCount, fromStack, toStack);
        }

        var topItemsStr = String.Join("", stacks.Select(s => s.Pop()));
        return topItemsStr;
    }

    private static MoveInstruction ParseMoveExpression(string line)
    {
        var match = MoveInstructionRegex().Match(line);
        return new MoveInstruction(
            ItemsCount: Int32.Parse(match.Groups["itemsCount"].Value),
            From: Int32.Parse(match.Groups["from"].Value),
            To: Int32.Parse(match.Groups["to"].Value));
    }

    public static Stack<char>[] ParseStacks(StreamReader streamReader)
    {
        var result = new List<List<char>>();

        List<char> GetStackList(int index)
        {
            while (result.Count <= index)
            {
                result.Add(new List<char>());
            }

            return result[index];
        }

        while (true)
        {
            var line = streamReader.ReadLine();

            var startIndex = 0;
            int index;
            while ((index = line.IndexOf('[', startIndex)) >= 0)
            {
                var stackItem = line[index + 1];
                var stackIndex = (int) (index / 4.0);
                var stack = GetStackList(stackIndex);
                stack.Add(stackItem);
                startIndex = index + 1;
            }
            
            // break cycle if there were no stacks in the line
            if (startIndex == 0)
            {
                break;
            }
        }
        return result
            .Select(list => new Stack<char>(Enumerable.Reverse(list)))
            .ToArray();
    }

    private record MoveInstruction(int ItemsCount, int From, int To);

    [GeneratedRegex(
        "move (?<itemsCount>\\d+) from (?<from>\\d+) to (?<to>\\d+)",
        RegexOptions.Singleline)]
    private static partial Regex MoveInstructionRegex();
}

public class Day5Tests
{
    [Fact]
    public void TestParseStacks()
    {
        const string input = """
    [D]    
[N] [C]    
[Z] [M] [P]
 1   2   3 
""";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));
        using var streamReader = new StreamReader(stream);
        var result = Day5.ParseStacks(streamReader);
        Assert.Equal(3, result.Length);

        Assert.Equal(new[] { 'N', 'Z' }, result[0]);
        Assert.Equal(new[] { 'D', 'C', 'M' }, result[1]);
        Assert.Equal(new[] { 'P' }, result[2]);
    }
    
    [Fact]
    public void TestSolveTaskPart1()
    {
        const string input = """
    [D]    
[N] [C]    
[Z] [M] [P]
 1   2   3 

move 1 from 2 to 1
move 3 from 1 to 3
move 2 from 2 to 1
move 1 from 1 to 2
""";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));
        var result = Day5.SolveTaskPart1(stream);
        Assert.Equal("CMZ", result);
    }
    
    [Fact]
    public void TestSolveTaskPart2()
    {
        const string input = """
    [D]    
[N] [C]    
[Z] [M] [P]
 1   2   3 

move 1 from 2 to 1
move 3 from 1 to 3
move 2 from 2 to 1
move 1 from 1 to 2
""";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));
        var result = Day5.SolveTaskPart2(stream);
        Assert.Equal("MCD", result);
    }
}