using System.Text;
using Xunit;

namespace Svl.AdventOfCode22;

public static class Day6
{
    public static int SolveTaskPart1(Stream stream)
    {
        return SolveGeneralTask(stream, 4);
    }

    public static int SolveTaskPart2(Stream stream)
    {
        return SolveGeneralTask(stream, 14);
    }

    private static int SolveGeneralTask(Stream stream, int markerLength)
    {
        var index = 0;

        var windowQueue = new Queue<char>();

        using var streamReader = new StreamReader(stream, Encoding.UTF8);
        while (!streamReader.EndOfStream)
        {
            index++;
            var signal = (char)streamReader.Read();
            windowQueue.Enqueue(signal);
            if (windowQueue.Count < markerLength) continue;

            // We can have window dictionary in count characters in the window.
            // It will provide better performance, but I am lazy to do it for such small dataset.
            if (windowQueue.ToHashSet().Count >= markerLength)
            {
                return index;
            }

            windowQueue.Dequeue();
        }

        return -1;
    }
}

public class Day6Tests
{
    [Theory]
    [InlineData("mjqjpqmgbljsphdztnvjfqwrcgsmlb", 7)]
    [InlineData("bvwbjplbgvbhsrlpgdmjqwftvncz", 5)]
    [InlineData("nppdvjthqldpwncqszvftbrmjlhg", 6)]
    [InlineData("nznrnfrfntjfmvfwmzdfjlvtqnbhcprsg", 10)]
    [InlineData("zcfzfwzzqfrljwzlrfnpqdbhtmscgvjw", 11)]
    public void TestSolveTaskPart1(string input, int expectedMarkerStart)
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));
        var result = Day6.SolveTaskPart1(stream);
        Assert.Equal(expectedMarkerStart, result);
    }
    
    [Theory]
    [InlineData("mjqjpqmgbljsphdztnvjfqwrcgsmlb", 19)]
    [InlineData("bvwbjplbgvbhsrlpgdmjqwftvncz", 23)]
    [InlineData("nppdvjthqldpwncqszvftbrmjlhg", 23)]
    [InlineData("nznrnfrfntjfmvfwmzdfjlvtqnbhcprsg", 29)]
    [InlineData("zcfzfwzzqfrljwzlrfnpqdbhtmscgvjw", 26)]
    public void TestSolveTaskPart2(string input, int expectedMarkerStart)
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));
        var result = Day6.SolveTaskPart2(stream);
        Assert.Equal(expectedMarkerStart, result);
    }
}