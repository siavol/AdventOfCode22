using System.Text;
using Xunit;

namespace Svl.AdventOfCode22;

public class Day_1_Tests
{
    [Fact]
    public void Test_Part1()
    {
        var input = """
1000
2000
3000

4000

5000
6000

7000
8000
9000

10000
""";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));
        var result = Day_1.SolveTask(stream);
        Assert.Equal(24000, result);
    }

    [Fact]
    public void Test_Part2()
    {
        var input = """
1000
2000
3000

4000

5000
6000

7000
8000
9000

10000
""";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));
        var result = Day_1.SolveSecondPartTask(stream);
        Assert.Equal(24000 + 11000 + 10000, result);
    }
}