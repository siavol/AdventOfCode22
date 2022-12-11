using System.Text;
using Xunit;

namespace Svl.AdventOfCode22;

public static class Day1
{
    public static int SolveTask(Stream stream)
    {
        var maxCalories = 0;
        var curCalories = 0;
        
        using var reader = new StreamReader(stream);
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            if (string.IsNullOrWhiteSpace(line))
            {
                curCalories = 0;
            }
            else
            {
                var calories = Int32.Parse(line);
                curCalories += calories;
                maxCalories = Math.Max(maxCalories, curCalories);
            }
        }
        
        return maxCalories;
    }

    public static int SolveSecondPartTask(Stream stream)
    {
        var caloriesHeap = new PriorityQueue<int, int>(new IntComparer());

        var curCalories = 0;
        using var reader = new StreamReader(stream);
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            if (string.IsNullOrWhiteSpace(line))
            {
                caloriesHeap.Enqueue(curCalories, curCalories);
                curCalories = 0;
            }
            else
            {
                var calories = Int32.Parse(line);
                curCalories += calories;
            }
        }
        caloriesHeap.Enqueue(curCalories, curCalories);

        return caloriesHeap.Dequeue() + caloriesHeap.Dequeue() + caloriesHeap.Dequeue();
    }

    private class IntComparer : IComparer<int>
    {
        public int Compare(int x, int y)
        {
            return y - x;
        }
    }
}

public class Day1Tests
{
    [Fact]
    public void Test_Part1()
    {
        const string input = """
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
        var result = Day1.SolveTask(stream);
        Assert.Equal(24000, result);
    }

    [Fact]
    public void Test_Part2()
    {
        const string input = """
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
        var result = Day1.SolveSecondPartTask(stream);
        Assert.Equal(24000 + 11000 + 10000, result);
    }
}