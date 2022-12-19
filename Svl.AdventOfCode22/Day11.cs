using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Xunit;

namespace Svl.AdventOfCode22;

public partial class Day11
{
    public static int GetLevelOfMonkeyBusiness(Stream stream)
    {
        const int roundsCount = 20;
        const int businessMonkeysCount = 2;
        
        var monkeys = MonkeyParser.ParseMonkeys(stream);
        var game = new MonkeysGame(monkeys);
        for (var i = 0; i < roundsCount; i++)
        {
            game.PlayRound();
        }

        return game.GetMonkeys()
            .Select(m => m.InspectedItemsCount)
            .OrderDescending()
            .Take(businessMonkeysCount)
            .Aggregate(1, (acc, val) => acc * val);
    }

    public class MonkeysGame
    {
        private const int WorryLevelDecreaseFactor = 3;
        private readonly Dictionary<int, Monkey> _monkeys = new();
        
        public MonkeysGame(IEnumerable<Monkey> monkeys)
        {
            foreach (var monkey in monkeys)
            {
                _monkeys.Add(monkey.Id, monkey);
            }
        }

        public Monkey GetMonkey(int id)
        {
            return _monkeys[id];
        }

        public void PlayRound()
        {
            var monkeysCount = _monkeys.Count;
            for (var id = 0; id < monkeysCount; id++)
            {
                var curMonkey = GetMonkey(id);

                while (curMonkey.HasItems)
                {
                    var itemWorryLevel = curMonkey.InspectItem();
                    itemWorryLevel = itemWorryLevel / WorryLevelDecreaseFactor;

                    var toMonkeyId = curMonkey.GetMonkeyToThrowTheItem(itemWorryLevel);
                    _monkeys[toMonkeyId].EnqueueItem(itemWorryLevel);
                }
            }
        }

        public IEnumerable<Monkey> GetMonkeys()
        {
            return _monkeys.Values;
        }
    }
    
    public partial class MonkeyParser
    {
        public static IEnumerable<Monkey> ParseMonkeys(Stream stream)
        {
            using var streamReader = new StreamReader(stream);
            while (!streamReader.EndOfStream)
            {
                yield return Parse(streamReader);
                streamReader.ReadLine();
            }
        }

        public static Monkey Parse(StreamReader streamReader)
        {
            var id = ParseTitle(streamReader);
            var startingItems = ParseStartingItems(streamReader);
            var operation = ParseOperation(streamReader);

            var testStringBuilder = new StringBuilder();
            testStringBuilder.AppendLine(streamReader.ReadLine());
            testStringBuilder.AppendLine(streamReader.ReadLine());
            testStringBuilder.AppendLine(streamReader.ReadLine());
            var testStr = testStringBuilder.ToString();
            var testMatch = TestRegex().Match(testStr);
            Debug.Assert(testMatch.Success, "Test string does not match");
            var testFunc = GetTestFunc(
                Convert.ToInt32(testMatch.Groups["div"].Value),
                Convert.ToInt32(testMatch.Groups["mTrue"].Value),
                Convert.ToInt32(testMatch.Groups["mFalse"].Value));

            var monkey = new Monkey(id, startingItems, operation, testFunc);
            return monkey;
        }

        private static int ParseTitle(StreamReader streamReader)
        {
            var title = streamReader.ReadLine();
            Debug.Assert(title != null, nameof(title) + " != null");
            var titleMatch = TitleRegex().Match(title);
            Debug.Assert(titleMatch.Success, "Title string does not match");
            var idStr = titleMatch.Groups["id"].Value;
            var id = Int32.Parse(idStr);
            return id;
        }

        private static IEnumerable<int> ParseStartingItems(StreamReader streamReader)
        {
            var startingItemsLine = streamReader.ReadLine();
            Debug.Assert(startingItemsLine != null, nameof(startingItemsLine) + " != null");
            var startingItemsMatch = StartingItemsRegex().Match(startingItemsLine);
            Debug.Assert(startingItemsMatch.Success, "Starting items string does not match");
            var itemsStr = startingItemsMatch.Groups["items"].Value;
            return itemsStr.Split(',', StringSplitOptions.TrimEntries)
                .Select(Int32.Parse);
        }

        private static Func<int, int> ParseOperation(StreamReader streamReader)
        {
            var operationLine = streamReader.ReadLine();
            Debug.Assert(operationLine != null, nameof(operationLine) + " != null");
            var operationMatch = OperationRegex().Match(operationLine);
            Debug.Assert(operationMatch.Success, "Operation string does not match");
            var operation = GetOperationFunc(
                operationMatch.Groups["arg1"].Value,
                operationMatch.Groups["op"].Value,
                    operationMatch.Groups["arg2"].Value);
            return operation;
        }

        private static Func<int, int> GetOperationFunc(string arg1, string op, string arg2)
        {
            return old =>
            {
                var val1 = arg1 switch
                {
                    "old" => old,
                    var str => Int32.Parse(str)
                };
                var val2 = arg2 switch
                {
                    "old" => old,
                    var str => Int32.Parse(str)
                };
                
                return op switch
                {
                    "+" => val1 + val2,
                    "*" => val1 * val2,
                    _ => throw new ApplicationException()
                };
            };
        }

        private static Func<int, int> GetTestFunc(int divider, int monkeyIdTrue, int monkeyIdFalse)
        {
            return val => (val % divider) == 0 ? monkeyIdTrue : monkeyIdFalse;
        }

        [GeneratedRegex(
            "Monkey\\s+(?<id>\\d+)",
            RegexOptions.Singleline)]
        public static partial Regex TitleRegex();

        [GeneratedRegex(
            "\\s*Starting items:(?<items>(\\s*\\d+,?)+)",
            RegexOptions.Singleline)]
        public static partial Regex StartingItemsRegex();

        [GeneratedRegex(
            "\\s*Operation: new = (?<arg1>\\d+|old) (?<op>\\*|\\+) (?<arg2>\\d+|old)",
            RegexOptions.Singleline)]
        public static partial Regex OperationRegex();

        [GeneratedRegex(
            """
\s*Test: divisible by (?<div>\d+)
\s*If true: throw to monkey (?<mTrue>\d+)
\s*If false: throw to monkey (?<mFalse>\d+)
""",
            RegexOptions.Singleline)]
        public static partial Regex TestRegex();
    }
    
    public class Monkey
    {
        private readonly Queue<int> _items = new();
        private readonly Func<int, int> _operation;
        private readonly Func<int, int> _testFunc;


        public int Id { get; }

        public bool HasItems => _items.Count > 0;

        public int InspectedItemsCount { get; private set; }

        public Monkey(int id, IEnumerable<int> startingItems, Func<int, int> operation, Func<int, int> testFunc)
        {
            Id = id;
            _operation = operation;
            _testFunc = testFunc;

            foreach (var item in startingItems)
            {
                EnqueueItem(item);
            }
        }

        public void EnqueueItem(int item)
        {
            _items.Enqueue(item);
        }

        public int InspectItem()
        {
            var item = _items.Dequeue();
            InspectedItemsCount++;
            return _operation(item);
        }

        public int GetMonkeyToThrowTheItem(int item)
        {
            return _testFunc(item);
        }

        public IEnumerable<int> GetItems()
        {
            return _items.AsEnumerable();
        }
    }
}

public class Day11Tests
{
    [Fact]
    public void TestMonkey()
    {
        const string input = """
Monkey 0:
  Starting items: 79, 98
  Operation: new = old * 19
  Test: divisible by 23
    If true: throw to monkey 2
    If false: throw to monkey 3
""";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));
        using var streamReader = new StreamReader(stream);
        var monkey = Day11.MonkeyParser.Parse(streamReader);
        
        Assert.Equal(0, monkey.Id);
        Assert.True(monkey.HasItems);
        Assert.Equal(0, monkey.InspectedItemsCount);

        var inspectedItem = monkey.InspectItem();
        Assert.Equal(1501, inspectedItem);
        Assert.Equal(1, monkey.InspectedItemsCount);

        var monkeyToThrow = monkey.GetMonkeyToThrowTheItem(inspectedItem);
        Assert.Equal(3, monkeyToThrow);
    }

    private const string TestInput = """
Monkey 0:
  Starting items: 79, 98
  Operation: new = old * 19
  Test: divisible by 23
    If true: throw to monkey 2
    If false: throw to monkey 3

Monkey 1:
  Starting items: 54, 65, 75, 74
  Operation: new = old + 6
  Test: divisible by 19
    If true: throw to monkey 2
    If false: throw to monkey 0

Monkey 2:
  Starting items: 79, 60, 97
  Operation: new = old * old
  Test: divisible by 13
    If true: throw to monkey 1
    If false: throw to monkey 3

Monkey 3:
  Starting items: 74
  Operation: new = old + 3
  Test: divisible by 17
    If true: throw to monkey 0
    If false: throw to monkey 1
""";

    [Fact]
    public void TestMonkeysInputParse()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(TestInput));
        var monkeys = Day11.MonkeyParser.ParseMonkeys(stream);
        Assert.Equal(4, monkeys.Count());
    }

    [Fact]
    public void TestPlayRound()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(TestInput));
        var monkeys = Day11.MonkeyParser.ParseMonkeys(stream);
        var game = new Day11.MonkeysGame(monkeys);
        game.PlayRound();

        var monkey0 = game.GetMonkey(0);
        Assert.Equal(new[] { 20, 23, 27, 26 }, monkey0.GetItems());

        var monkey1 = game.GetMonkey(1);
        Assert.Equal(new[] { 2080, 25, 167, 207, 401, 1046 }, monkey1.GetItems());
        
        Assert.False(game.GetMonkey(2).HasItems);
        Assert.False(game.GetMonkey(3).HasItems);
    }

    [Fact]
    public void TestGetTotalInspectedItemsCount()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(TestInput));

        var result = Day11.GetLevelOfMonkeyBusiness(stream);
        Assert.Equal(10605, result);
    }
}