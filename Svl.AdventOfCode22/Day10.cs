using System.Diagnostics;
using System.Text;
using Xunit;

namespace Svl.AdventOfCode22;

public class Day10
{
    public class Machine
    {
        private readonly List<int> _history = new();
        
        public int Register { get; private set; } = 1;

        public int Cycle => _history.Count;

        public Machine Noop()
        {
            HandleCycles(1);
            return this;
        }

        public Machine Addx(int x)
        {
            HandleCycles(2);
            Register += x;
            return this;
        }

        private void HandleCycles(int count)
        {
            for (int i = 0; i < count; i++)
            {
                _history.Add(Register);
            }
        }

        public int GetRegisterDuringTheCycle(int cycle)
        {
            return _history[cycle-1];
        }

        public Machine ExecuteCommand(string command)
        {
            var parts = command.Split(' ', 2, StringSplitOptions.TrimEntries);
            switch (parts[0])
            {
                case "noop":
                    return Noop();
                case "addx":
                    var x = Int32.Parse(parts[1]);
                    return Addx(x);
                default:
                    throw new ApplicationException();
            }
        }
    }

    public static Machine ExecuteMachine(Stream stream)
    {
        var machine = new Machine();
        using var streamReader = new StreamReader(stream);
        while (!streamReader.EndOfStream)
        {
            var command = streamReader.ReadLine();
            Debug.Assert(command != null, nameof(command) + " != null");
            machine.ExecuteCommand(command);
        }

        return machine;
    }

    public static int GetSignalStrengthSum(Stream stream)
    {
        var machine = ExecuteMachine(stream);

        var signalCycles = new[] { 20, 60, 100, 140, 180, 220 };
        return signalCycles
            .Select(cycle => machine.GetRegisterDuringTheCycle(cycle) * cycle)
            .Sum();
    }
}

public class Day10Tests
{
    [Fact]
    public void TestMachine()
    {
        var machine = new Day10.Machine();
        machine
            .Noop()
            .Addx(3)
            .Addx(-5);
        Assert.Equal(5, machine.Cycle);
        Assert.Equal(-1, machine.Register);
        
        // noop
        Assert.Equal(1, machine.GetRegisterDuringTheCycle(1));
        
        // addx 3
        Assert.Equal(1, machine.GetRegisterDuringTheCycle(2));
        Assert.Equal(1, machine.GetRegisterDuringTheCycle(3));
        
        // addx -5
        Assert.Equal(4, machine.GetRegisterDuringTheCycle(4));
        Assert.Equal(4, machine.GetRegisterDuringTheCycle(5));
    }

    [Fact]
    public void TestMachineForStringInput()
    {
        const string input = """
noop
addx 3
addx -5
""";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));
        var machine = Day10.ExecuteMachine(stream);

        Assert.Equal(5, machine.Cycle);
        Assert.Equal(-1, machine.Register);
        
        // noop
        Assert.Equal(1, machine.GetRegisterDuringTheCycle(1));
        
        // addx 3
        Assert.Equal(1, machine.GetRegisterDuringTheCycle(2));
        Assert.Equal(1, machine.GetRegisterDuringTheCycle(3));
        
        // addx -5
        Assert.Equal(4, machine.GetRegisterDuringTheCycle(4));
        Assert.Equal(4, machine.GetRegisterDuringTheCycle(5));
    }

    #region Test input

    private const string TestInput = """
addx 15
addx -11
addx 6
addx -3
addx 5
addx -1
addx -8
addx 13
addx 4
noop
addx -1
addx 5
addx -1
addx 5
addx -1
addx 5
addx -1
addx 5
addx -1
addx -35
addx 1
addx 24
addx -19
addx 1
addx 16
addx -11
noop
noop
addx 21
addx -15
noop
noop
addx -3
addx 9
addx 1
addx -3
addx 8
addx 1
addx 5
noop
noop
noop
noop
noop
addx -36
noop
addx 1
addx 7
noop
noop
noop
addx 2
addx 6
noop
noop
noop
noop
noop
addx 1
noop
noop
addx 7
addx 1
noop
addx -13
addx 13
addx 7
noop
addx 1
addx -33
noop
noop
noop
addx 2
noop
noop
noop
addx 8
noop
addx -1
addx 2
addx 1
noop
addx 17
addx -9
addx 1
addx 1
addx -3
addx 11
noop
noop
addx 1
noop
addx 1
noop
noop
addx -13
addx -19
addx 1
addx 3
addx 26
addx -30
addx 12
addx -1
addx 3
addx 1
noop
noop
noop
addx -9
addx 18
addx 1
addx 2
noop
noop
addx 9
noop
noop
noop
addx -1
addx 2
addx -37
addx 1
addx 3
noop
addx 15
addx -21
addx 22
addx -6
addx 1
noop
addx 2
addx 1
noop
addx -10
noop
noop
addx 20
addx 1
addx 2
addx 2
addx -6
addx -11
noop
noop
noop
""";

    #endregion

    [Fact]
    public void TestMachineOnTheBiggerInput()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(TestInput));
        var machine = Day10.ExecuteMachine(stream);
        
        Assert.Equal(21, machine.GetRegisterDuringTheCycle(20));
        Assert.Equal(19, machine.GetRegisterDuringTheCycle(60));
        Assert.Equal(18, machine.GetRegisterDuringTheCycle(100));
        Assert.Equal(21, machine.GetRegisterDuringTheCycle(140));
        Assert.Equal(16, machine.GetRegisterDuringTheCycle(180));
        Assert.Equal(18, machine.GetRegisterDuringTheCycle(220));
    }
    
    [Fact]
    public void TestGetSignalStrengthSum()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(TestInput));
        int result = Day10.GetSignalStrengthSum(stream);
        
        Assert.Equal(13140, result);
    }
}