using System.Text;
using Xunit;

namespace Svl.AdventOfCode22;

public class Day_1_Tests
{
    [Fact]
    public void Test()
    {
        var input = """
2
5

3

1
""";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));
        var result = Day_1.SolveTask(stream);
        Assert.Equal(7, result);
    }
}