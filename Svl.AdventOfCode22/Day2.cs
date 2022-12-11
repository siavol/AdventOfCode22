using System.Text;
using Xunit;

namespace Svl.AdventOfCode22;

public static class Day2
{
    private const int OpponentIndex = 0;
    private const int YourIndex = 2;

    private enum HandShape
    {
        Rock = 1,
        Paper = 2,
        Scissors = 3
    }

    private enum RoundResult
    {
        Lost = 0,
        Draw = 3,
        Won = 6
    }

    private static readonly Dictionary<Char, HandShape> handShapeMap = new()
    {
        { 'A', HandShape.Rock },
        { 'B', HandShape.Paper },
        { 'C', HandShape.Scissors },
        { 'X', HandShape.Rock },
        { 'Y', HandShape.Paper },
        { 'Z', HandShape.Scissors }
    };

    private class Accumulator
    {
        public int Score { get; private set; } = 0;

        public void CountRound(HandShape opponent, HandShape your)
        {
            var result = (opponent, your) switch
            {
                (HandShape.Paper, HandShape.Paper) => RoundResult.Draw,
                (HandShape.Paper, HandShape.Rock) => RoundResult.Lost,
                (HandShape.Paper, HandShape.Scissors) => RoundResult.Won,

                (HandShape.Rock, HandShape.Paper) => RoundResult.Won,
                (HandShape.Rock, HandShape.Rock) => RoundResult.Draw,
                (HandShape.Rock, HandShape.Scissors) => RoundResult.Lost,

                (HandShape.Scissors, HandShape.Paper) => RoundResult.Lost,
                (HandShape.Scissors, HandShape.Rock) => RoundResult.Won,
                (HandShape.Scissors, HandShape.Scissors) => RoundResult.Draw,

                _ => throw new NotSupportedException($"Unknown combination {opponent} vs {your}")
            };

            var roundScore = (int)your + (int)result;
            Score += roundScore;
        }
    }
    
    public static int SolveTask(Stream stream)
    {
        var accumulator = new Accumulator();
        
        using var reader = new StreamReader(stream);
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            var opponent = handShapeMap[line[OpponentIndex]];
            var your = handShapeMap[line[YourIndex]];
            accumulator.CountRound(opponent, your);
        }

        return accumulator.Score;
    }
}

public class Day2Tests
{
    [Fact]
    public void TestTask()
    {
        var input = """
A Y
B X
C Z
""";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));
        var result = Day2.SolveTask(stream);
        Assert.Equal(15, result);

    }
}