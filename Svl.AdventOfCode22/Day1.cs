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