using System.Text;
using Xunit;

namespace Svl.AdventOfCode22;

public static class Day3
{
    public static int SolveTaskPart1(Stream stream)
    {
        var acc = 0;
        using var streamReader = new StreamReader(stream);
        while (!streamReader.EndOfStream)
        {
            var line = streamReader.ReadLine();
            var priority = GetRucksackPriority(line);
            acc += priority;
        }

        return acc;
    }

    internal static int GetItemPriority(char item) => item switch
    {
        >= 'a' and <= 'z' => item - 'a' + 1,
        >= 'A' and <= 'Z' => item - 'A' + 27,
        _ => throw new NotSupportedException()
    };

    internal static int GetRucksackPriority(string rucksack)
    {
        var len = rucksack.Length;
        var compartment1 = rucksack.Substring(0, len / 2);
        var compartment2 = rucksack.Substring(len / 2, len / 2);

        var commonItem = GetCommonItem(compartment1, compartment2);
        return GetItemPriority(commonItem);
    }

    private static char GetCommonItem(string compartment1, string compartment2)
    {
        var itemsMap = new HashSet<char>(compartment1);
        return compartment2.First(item => itemsMap.Contains(item));
    }
}

public class Day3Tests
{
    [Theory]
    [InlineData('a', 1)]
    [InlineData('b', 2)]
    [InlineData('z', 26)]
    [InlineData('A', 27)]
    [InlineData('Z', 52)]
    public void TestItemPriority(char item, int expectedPriority)
    {
        var priority = Day3.GetItemPriority(item);
        Assert.Equal(expectedPriority, priority);
    }

    [Theory]
    [InlineData("vJrwpWtwJgWrhcsFMMfFFhFp", 16)]
    [InlineData("jqHRNqRjqzjGDLGLrsFMfFZSrLrFZsSL", 38)]
    public void TestGetRucksackPriority(string rucksack, int expectedPriority)
    {
        var priority = Day3.GetRucksackPriority(rucksack);
        Assert.Equal(expectedPriority, priority);
    }
    
    [Fact]
    public void TestTaskPart1()
    {
        const string input = """
vJrwpWtwJgWrhcsFMMfFFhFp
jqHRNqRjqzjGDLGLrsFMfFZSrLrFZsSL
PmmdzqPrVvPwwTWBwg
wMqvLMZHhHMvwLHjbvcjnnSBnvTQFn
ttgJtRGJQctTZtZT
CrZsJsPPZsGzwwsLwLmpwMDw
""";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));
        var result = Day3.SolveTaskPart1(stream);
        Assert.Equal(157, result);
    }
}