using System.Diagnostics;
using System.Text;
using Xunit;

namespace Svl.AdventOfCode22;

public static class Day9
{
    public record Coordinate(int X, int Y);

    public enum Direction
    {
        Up, Down, Left, Right
    }

    public class Rope
    {
        private readonly Coordinate[] _knots;

        public Coordinate Head
        {
            get => _knots[0];
            private set => _knots[0] = value;
        }

        public Coordinate Tail => _knots[^1];

        public Rope(int length = 2)
        {
            _knots = new Coordinate[length];
            for (var i = 0; i < length; i++)
            {
                _knots[i] = new Coordinate(0, 0);
            }
        }

        public void MoveHeadTo(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    Head = Head with { Y = Head.Y - 1 };
                    break;
                case Direction.Down:
                    Head = Head with { Y = Head.Y + 1 };
                    break;
                case Direction.Left:
                    Head = Head with { X = Head.X - 1 };
                    break;
                case Direction.Right:
                    Head = Head with { X = Head.X + 1 };
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }

            // Move next knots
            for (var i = 1; i < _knots.Length; i++)
            {
                var firstKnot = _knots[i - 1];
                var nextKnot = _knots[i];
                
                var dx = firstKnot.X - nextKnot.X;
                var dy = firstKnot.Y - nextKnot.Y;
                if (Math.Abs(dx) > 1 || Math.Abs(dy) > 1)
                {
                    var x = dx != 0 ? nextKnot.X + dx / Math.Abs(dx) : nextKnot.X;
                    var y = dy != 0 ? nextKnot.Y + dy / Math.Abs(dy) : nextKnot.Y;
                    _knots[i] = new Coordinate(x, y);
                }
            }
        }
    }

    public static int FindTailPositionsCount(Stream stream)
    {
        return FindTailPositionsCount(stream, 2);
    }

    private static int FindTailPositionsCount(Stream stream, int ropeLength)
    {
        var rope = new Rope(ropeLength);
        var tailPositions = new HashSet<Coordinate>();
        using var streamReader = new StreamReader(stream);
        while (!streamReader.EndOfStream)
        {
            var line = streamReader.ReadLine();
            Debug.Assert(line != null, nameof(line) + " != null");
            var (direction, movesCount) = ParseMoveInstruction(line);
            for (var i = 0; i < movesCount; i++)
            {
                rope.MoveHeadTo(direction);
                tailPositions.Add(rope.Tail);
            }
        }

        return tailPositions.Count;
    }

    public static int FindLongRopeTailPositionsCount(Stream stream)
    {
        return FindTailPositionsCount(stream, 10);
    }

    private static (Direction, int) ParseMoveInstruction(string line)
    {
        var parts = line.Split(' ', 2, 
            StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var direction = parts[0] switch
        {
            "U" => Direction.Up,
            "D" => Direction.Down,
            "L" => Direction.Left,
            "R" => Direction.Right,
            _ => throw new InvalidOperationException()
        };
        var movesCount = Int32.Parse(parts[1]);
        return (direction, movesCount);
    }
}

public class Day9Tests
{
    [Fact]
    public void TestCoordinateEquality()
    {
        var c1 = new Day9.Coordinate(1, 3);
        var c2 = new Day9.Coordinate(1, 3);
        Assert.True(c1 == c2);
    }

    [Fact]
    public void TestRopeMoveHeadTo()
    {
        var rope = new Day9.Rope();
        
        rope.MoveHeadTo(Day9.Direction.Up);
        Assert.Equal(new Day9.Coordinate(0, -1), rope.Head);
        Assert.Equal(new Day9.Coordinate(0, 0), rope.Tail);

        rope.MoveHeadTo(Day9.Direction.Up);
        Assert.Equal(new Day9.Coordinate(0, -2), rope.Head);
        Assert.Equal(new Day9.Coordinate(0, -1), rope.Tail);

        rope.MoveHeadTo(Day9.Direction.Right);
        Assert.Equal(new Day9.Coordinate(1, -2), rope.Head);
        Assert.Equal(new Day9.Coordinate(0, -1), rope.Tail);

        rope.MoveHeadTo(Day9.Direction.Right);
        Assert.Equal(new Day9.Coordinate(2, -2), rope.Head);
        Assert.Equal(new Day9.Coordinate(1, -2), rope.Tail);
        
        rope.MoveHeadTo(Day9.Direction.Down);
        Assert.Equal(new Day9.Coordinate(2, -1), rope.Head);
        Assert.Equal(new Day9.Coordinate(1, -2), rope.Tail);
        
        rope.MoveHeadTo(Day9.Direction.Left);
        Assert.Equal(new Day9.Coordinate(1, -1), rope.Head);
        Assert.Equal(new Day9.Coordinate(1, -2), rope.Tail);
    }

    [Fact]
    public void TestFindTailPositions()
    {
        const string input = """
R 4
U 4
L 3
D 1
R 4
D 1
L 5
R 2
""";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));
        var result = Day9.FindTailPositionsCount(stream);
        Assert.Equal(13, result);
    }
    
    [Theory]
    [InlineData("""
R 4
U 4
L 3
D 1
R 4
D 1
L 5
R 2
""", 1)]
    [InlineData("""
R 5
U 8
L 8
D 3
R 17
D 10
L 25
U 20
""", 36)]
    public void TestFindLongRopeTailPositions(string input, int expectedResult)
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));
        var result = Day9.FindLongRopeTailPositionsCount(stream);
        Assert.Equal(expectedResult, result);
    }
}