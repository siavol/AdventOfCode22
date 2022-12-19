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
        public Coordinate Head { get; private set; } = new(0, 0);
        public Coordinate Tail { get; private set; } = new(0, 0);

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
            
            // Move tail
            var dx = Head.X - Tail.X;
            var dy = Head.Y - Tail.Y;
            if (Math.Abs(dx) > 1 || Math.Abs(dy) > 1)
            {
                var x = dx != 0 ? Tail.X + dx / Math.Abs(dx) : Tail.X;
                var y = dy != 0 ? Tail.Y + dy / Math.Abs(dy) : Tail.Y;
                Tail = new Coordinate(x, y);
            }
        }
    }

    public static int FindTailPositionsCount(Stream stream)
    {
        var rope = new Rope();
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
}