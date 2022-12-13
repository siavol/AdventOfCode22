using System.Text;
using Xunit;

namespace Svl.AdventOfCode22;

public static class Day6
{
    private const int MarkerLength = 4;
        
    public static int SolveTaskPart1(Stream stream)
    {
        var index = 0;

        var windowQueue = new Queue<char>();
        
        using var streamReader = new StreamReader(stream, Encoding.UTF8);
        while (!streamReader.EndOfStream)
        {
            index++;
            var signal = (char) streamReader.Read();
            windowQueue.Enqueue(signal);
            if (windowQueue.Count < MarkerLength) continue;
            
            if (windowQueue.ToHashSet().Count >= MarkerLength)
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
}