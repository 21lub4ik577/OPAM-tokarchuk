using System;
using System.Collections.Generic;
using System.Diagnostics;

class Program
{
    static void Main()
    {
        const int size = 100000;

        List<int> warmup = new List<int>();
        for (int i = 0; i < 1000; i++)
            warmup.Add(i);

        List<int> list1 = new List<int>();
        Stopwatch stopwatch = Stopwatch.StartNew();

        for (int i = 0; i < size; i++)
            list1.Add(i);

        stopwatch.Stop();
        Console.WriteLine($"Add без Capacity: {stopwatch.Elapsed.TotalMilliseconds:F3} мс");

        List<int> list2 = new List<int>(size);
        stopwatch.Restart();

        for (int i = 0; i < size; i++)
            list2.Add(i);

        stopwatch.Stop();
        Console.WriteLine($"Add з Capacity: {stopwatch.Elapsed.TotalMilliseconds:F3} мс");

        List<int> list3 = new List<int>(size);
        for (int i = 0; i < size; i++)
            list3.Add(i);

        stopwatch.Restart();

        for (int i = 0; i < size; i++)
            list3.Insert(0, i);

        stopwatch.Stop();
        Console.WriteLine($"Insert(0, x): {stopwatch.Elapsed.TotalMilliseconds:F3} мс");

        List<int> list4 = new List<int>(size);
        for (int i = 0; i < size; i++)
            list4.Add(i);

        int missingValue = -1;
        stopwatch.Restart();

        for (int i = 0; i < 1000; i++)
            list4.Contains(missingValue);

        stopwatch.Stop();
        Console.WriteLine($"1000 викликів Contains: {stopwatch.Elapsed.TotalMilliseconds:F3} мс");
    }
}