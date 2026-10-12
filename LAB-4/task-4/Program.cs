using System;
using System.Collections.Generic;

class Program
{
    static void SplitList(List<int> numbers, out List<int> lessThanTwo, out List<int> rest)
    {
        lessThanTwo = new List<int>();
        rest = new List<int>();

        foreach (int number in numbers)
        {
            if (number < 2)
                lessThanTwo.Add(number);
            else
                rest.Add(number);
        }
    }

    static void Main()
    {
        Random random = new Random(26);
        List<int> numbers = new List<int>(20);

        for (int i = 0; i < 20; i++)
            numbers.Add(random.Next(-9, 10));

        Console.WriteLine($"Основний список: [{string.Join(", ", numbers)}]");

        SplitList(numbers, out List<int> lessThanTwo, out List<int> rest);

        Console.WriteLine($"Числа, менші за 2: [{string.Join(", ", lessThanTwo)}]");
        Console.WriteLine($"Решта чисел: [{string.Join(", ", rest)}]");

        Console.WriteLine($"Кількість елементів збігається: {lessThanTwo.Count + rest.Count == numbers.Count}");
        Console.WriteLine($"Основний список не змінено: [{string.Join(", ", numbers)}]");
    }
}