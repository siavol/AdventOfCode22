using System.Text;
using Xunit;

namespace Svl.AdventOfCode22;

public class Day8
{
    public static int GetVisibleTreeCount(Stream stream)
    {
        using var streamReader = new StreamReader(stream, Encoding.UTF8);
        var forest = new Forest();
        while (!streamReader.EndOfStream)
        {
            var line = streamReader.ReadLine();
            forest.AddRow(line);
        }
        
        return forest.GetVisibleTrees().Count();
    }

    public static int GetMaxScenicScore(Stream stream)
    {
        using var streamReader = new StreamReader(stream, Encoding.UTF8);
        var forest = new Forest();
        while (!streamReader.EndOfStream)
        {
            var line = streamReader.ReadLine();
            forest.AddRow(line);
        }

        return forest.GetMaxScenicScore();
    }

    public class Tree
    {
        private readonly Dictionary<Direction, int> _highestTrees = new();
        
        public int Height { get; }

        public Visibility Visibility { get; set; } = Visibility.Unknown;

        public Tree(int height)
        {
            Height = height;
        }

        public int? GetHighestTreeInDirection(Direction direction)
        {
            if (_highestTrees.TryGetValue(direction, out int value))
            {
                return value;
            }

            return null;
        }

        public void SetHighestTreeInDirection(Direction direction, int high)
        {
            if (_highestTrees.ContainsKey(direction))
            {
                throw new ApplicationException();
            }

            _highestTrees[direction] = high;
        }
    }

    public enum Visibility
    {
        Visible,
        Hidden,
        Unknown
    }

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    public class Forest
    {
        private readonly List<Tree[]> _grid = new();

        public void AddRow(Tree[] row)
        {
            if (row == null) throw new ArgumentNullException(nameof(row));
            if (_grid.Count > 0 && _grid[0].Length != row.Length)
            {
                throw new ArgumentException("Invalid trees count in the row", nameof(row));
            }

            _grid.Add(row);
        }

        public void AddRow(string line)
        {
            var row = line
                .Select(ch => Int32.Parse(ch.ToString()))
                .Select(height => new Tree(height))
                .ToArray();
            AddRow(row);
        }

        public Visibility GetTreeVisibility(int row, int col)
        {
            var tree = _grid[row][col];
            if (tree.Visibility == Visibility.Unknown)
            {
                var directions = Enum.GetValues<Direction>();
                var isVisible = directions
                    .Select(d =>
                    {
                        var highestTreeInDirection = GetHighestTreeInDirection(row, col, d);
                        return tree.Height > highestTreeInDirection ? Visibility.Visible : Visibility.Hidden;
                    })
                    .Any(v => v == Visibility.Visible);
                tree.Visibility = isVisible ? Visibility.Visible : Visibility.Hidden;
            }

            return tree.Visibility;
        }

        public IEnumerable<Tree> GetVisibleTrees()
        {
            for (int row = 0; row < _grid.Count; row++)
            {
                for (int col = 0; col < _grid[0].Length; col++)
                {
                    if (GetTreeVisibility(row, col) == Visibility.Visible)
                    {
                        yield return _grid[row][col];
                    }
                }
            }
        }

        private int GetHighestTreeInDirection(int row, int col, Direction direction)
        {
            var tree = _grid[row][col];
            var highestTreeInDirection = tree.GetHighestTreeInDirection(direction);
            if (highestTreeInDirection.HasValue)
            {
                return highestTreeInDirection.Value;
            }

            var (nextTreeRow, nextTreeCol) = GetNextCoord(row, col, direction);
            if (nextTreeRow >= 0 && nextTreeRow < _grid.Count
                                 && nextTreeCol >= 0 && nextTreeCol < _grid[0].Length)
            {
                var highestTree = GetHighestTreeInDirection(nextTreeRow, nextTreeCol, direction);
                var nextTree = _grid[nextTreeRow][nextTreeCol];
                highestTree = Math.Max(nextTree.Height, highestTree);
                tree.SetHighestTreeInDirection(direction, highestTree);
                return highestTree;
            }
            else
            {
                var noTreesInDirection = -1;
                tree.SetHighestTreeInDirection(direction, noTreesInDirection);
                return noTreesInDirection;
            }
        }

        private static (int, int) GetNextCoord(int row, int col, Direction direction)
        {
            return direction switch
            {
                Direction.Up => (row - 1, col),
                Direction.Down => (row + 1, col),
                Direction.Left => (row, col - 1),
                Direction.Right => (row, col + 1),
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };
        }

        public int GetScenicScore(int row, int col)
        {
            var tree = _grid[row][col];
            var directions = Enum.GetValues<Direction>();
            return directions
                .Select(d => GetScenicScoreInDirection(row, col, d, tree.Height, 0))
                .Aggregate(1, (acc, i) => acc * i);
        }

        private int GetScenicScoreInDirection(int row, int col, Direction direction, int treeHeight, int baseScore)
        {
            var (nextTreeRow, nextTreeCol) = GetNextCoord(row, col, direction);
            if (nextTreeRow >= 0 && nextTreeRow < _grid.Count
                                 && nextTreeCol >= 0 && nextTreeCol < _grid[0].Length)
            {
                var nextTree = _grid[nextTreeRow][nextTreeCol];
                if (nextTree.Height < treeHeight)
                {
                    return GetScenicScoreInDirection(nextTreeRow, nextTreeCol, direction, treeHeight, baseScore + 1);
                }
                else
                {
                    return baseScore + 1;
                }
            }
            else
            {
                return baseScore;
            }
        }

        public int GetMaxScenicScore()
        {
            var maxScore = -1;
            for (int row = 0; row < _grid.Count; row++)
            {
                for (int col = 0; col < _grid[0].Length; col++)
                {
                    var scenicScore = GetScenicScore(row, col);
                    maxScore = Math.Max(scenicScore, maxScore);
                }
            }

            return maxScore;
        }
    }
}

public class Day8Tests
{
    [Fact]
    public void TestSolveTask1()
    {
        const string input = """
30373
25512
65332
33549
35390
""";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));
        var result = Day8.GetVisibleTreeCount(stream);
        Assert.Equal(21, result);
    }

    [Theory]
    [InlineData(0, 0, Day8.Visibility.Visible)]
    [InlineData(1, 2, Day8.Visibility.Visible)]
    [InlineData(1, 3, Day8.Visibility.Hidden)]
    [InlineData(2, 1, Day8.Visibility.Visible)]
    [InlineData(2, 2, Day8.Visibility.Hidden)]
    [InlineData(2, 3, Day8.Visibility.Visible)]
    [InlineData(3, 1, Day8.Visibility.Hidden)]
    [InlineData(3, 2, Day8.Visibility.Visible)]
    [InlineData(3, 3, Day8.Visibility.Hidden)]
    public void TestForestGetTreeVisibility(int row, int col, Day8.Visibility expectedResult)
    {
        var forest = new Day8.Forest();
        forest.AddRow("30373");
        forest.AddRow("25512");
        forest.AddRow("65332");
        forest.AddRow("33549");
        forest.AddRow("35390");
        
        Assert.Equal(expectedResult, forest.GetTreeVisibility(row, col));
    }

    [Theory]
    [InlineData(1, 2, 4)]
    [InlineData(3, 2, 8)]
    public void TestGetScenicScore(int row, int col, int expectedScore)
    {
        var forest = new Day8.Forest();
        forest.AddRow("30373");
        forest.AddRow("25512");
        forest.AddRow("65332");
        forest.AddRow("33549");
        forest.AddRow("35390");
        
        Assert.Equal(expectedScore, forest.GetScenicScore(row, col));
    }
    
    [Fact]
    public void TestSolveTask2()
    {
        const string input = """
30373
25512
65332
33549
35390
""";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));
        var result = Day8.GetMaxScenicScore(stream);
        Assert.Equal(8, result);
    }
}
