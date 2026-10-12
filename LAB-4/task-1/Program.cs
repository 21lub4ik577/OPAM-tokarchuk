
using System;
using System.Collections.Generic;

class Program
{
    static List<int> BuildPositiveOddList(List<int> numbers)
    {
        List<int> result = new List<int>();

        foreach (int number in numbers)
        {
            if (number > 0 && number % 2 != 0)
            {
                result.Add(number);
            }
        }
        return result;
    }

    static void Main()
    {
        Random random = new Random(26);
        List<int> numbers = new List<int>(20);

        for (int i = 0; i < 20; i++)
        {
            numbers.Add(random.Next(-9, 10));
        }

        Console.WriteLine($"Основний список: [{string.Join(", ", numbers)}]");

        List<int> result = BuildPositiveOddList(numbers);

        Console.WriteLine($"Додатні непарні числа: [{string.Join(", ", result)}]");

        Console.WriteLine($"Count: {result.Count}");
    }
}
