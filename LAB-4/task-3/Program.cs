using System;
using System.Collections.Generic;

class Program
{
    static List<int> CreateSortedList(int length, Random random)
    {
        List<int> numbers = new List<int>(length);

        if (length == 0)
        {
            return numbers;
        }

        numbers.Add(random.Next(1, 10));

        for (int i = 1; i < length; i++)
        {
            numbers.Add(numbers[i - 1] + random.Next(1, 10));
        }

        return numbers;
    }

    static List<int> MergeSortedLists(List<int> first, List<int> second)
    {
        List<int> result = new List<int>(first.Count + second.Count);

        int i = 0;
        int j = 0;

        while (i < first.Count && j < second.Count)
        {
            if (first[i] <= second[j])
            {
                result.Add(first[i]);
                i++;
            }
            else
            {
                result.Add(second[j]);
                j++;
            }
        }

        while (i < first.Count)
        {
            result.Add(first[i]);
            i++;
        }

        while (j < second.Count)
        {
            result.Add(second[j]);
            j++;
        }

        return result;
    }

    static bool IsSorted(List<int> numbers)
    {
        for (int i = 1; i < numbers.Count; i++)
        {
            if (numbers[i] < numbers[i - 1])
            {
                return false;
            }
        }

        return true;
    }

    static void Main()
    {
        Random random1 = new Random(26);
        Random random2 = new Random(126);

        List<int> first = CreateSortedList(11, random1);
        List<int> second = CreateSortedList(6, random2);

        Console.WriteLine($"Перший список: [{string.Join(", ", first)}]");
        Console.WriteLine($"Другий список: [{string.Join(", ", second)}]");

        List<int> merged = MergeSortedLists(first, second);

        Console.WriteLine($"Об'єднаний список: [{string.Join(", ", merged)}]");
        Console.WriteLine($"Список упорядкований: {IsSorted(merged)}");
    }
}