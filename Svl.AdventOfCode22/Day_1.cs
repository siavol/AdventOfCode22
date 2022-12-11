namespace Svl.AdventOfCode22;

public class Day_1
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
}