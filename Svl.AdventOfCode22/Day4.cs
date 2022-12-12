using System.Text;
using Xunit;

namespace Svl.AdventOfCode22;

public static class Day4
{
    public static int SolveTaskPart1(Stream stream)
    {
        var count = 0;
        using var streamReader = new StreamReader(stream);
        while (!streamReader.EndOfStream)
        {
            var line = streamReader.ReadLine();
            var pair = ParseAssignmentPairs(line ?? throw new InvalidOperationException());
            if (FullyContains(pair))
            {
                count++;
            }
        }

        return count;
    }

    private static bool FullyContains(Tuple<Range,Range> pair)
    {
        return (pair.Item1.From >= pair.Item2.From && pair.Item1.To <= pair.Item2.To)
               || (pair.Item1.From <= pair.Item2.From && pair.Item1.To >= pair.Item2.To);
    }

    private static Tuple<Range, Range> ParseAssignmentPairs(string line)
    {
        var parts = line.Split(',', 2);
        return Tuple.Create(
            ParseRange(parts[0]),
            ParseRange(parts[1]));
    }

    private static Range ParseRange(string rangeStr)
    {
        var parts = rangeStr.Split('-', 2);
        var from = Int32.Parse(parts[0]);
        var to = Int32.Parse(parts[1]);
        return new Range(from, to);
    }

    private record Range(int From, int To);
}

public class Day4Tests
{
    [Fact]
    public void TestSolveTaskPart1()
    {
        const string input = """
2-4,6-8
2-3,4-5
5-7,7-9
2-8,3-7
6-6,4-6
2-6,4-8
""";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));
        var result = Day4.SolveTaskPart1(stream);
        Assert.Equal(2, result);
    }
}