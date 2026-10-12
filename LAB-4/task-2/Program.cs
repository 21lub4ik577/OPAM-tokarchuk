
using System;
using System.Collections.Generic;

class Program
{
    static void RemoveMultiplesWrong(List<int> numbers)
    {
        for (int i = 0; i < numbers.Count; i++)
        {
            if (numbers[i] % 3 == 0)
            {
                numbers.RemoveAt(i);
            }
        }
    }
    static void RemoveMultiplesFromEnd(List<int> numbers)
    {
        for (int i = numbers.Count - 1; i >= 0; i--)
        {
            if (numbers[i] % 3 == 0)
            {
                numbers.RemoveAt(i);
            }
        }
    }

    static List<int> BuildWithoutMultiplesOfThree(List<int> numbers)
    {
        List<int> result = new List<int>();

        foreach (int number in numbers)
        {
            if (number % 3 != 0)
            {
                result.Add(number);
            }
        }

        return result;
    }
    static bool AreListsEqual(List<int> first, List<int> second)
    {
        if (first.Count != second.Count)
        {
            return false;
        }

        for (int i = 0; i < first.Count; i++)
        {
            if (first[i] != second[i])
            {
                return false;
            }
        }

        return true;
    }

    static void Main()
    {
        Random random = new Random(26);
        List<int> numbers = new List<int>(20);

        for (int i = 0; i < 20; i++)
        {
            numbers.Add(random.Next(-9, 10));
        }

        Console.WriteLine(
            $"Основний список: [{string.Join(", ", numbers)}]");

        List<int> wrongCopy = new List<int>(numbers);
        RemoveMultiplesWrong(wrongCopy);

        Console.WriteLine(
            $"Неправильний варіант: [{string.Join(", ", wrongCopy)}]");

        List<int> skippedElements = new List<int>();

        foreach (int number in wrongCopy)
        {
            if (number % 3 == 0)
            {
                skippedElements.Add(number);
            }
        }

        Console.WriteLine(
            $"Пропущені елементи: [{string.Join(", ", skippedElements)}]");

        List<int> reverseCopy = new List<int>(numbers);
        RemoveMultiplesFromEnd(reverseCopy);

        Console.WriteLine(
            $"Вилучення з кінця: [{string.Join(", ", reverseCopy)}]");

        List<int> newList = BuildWithoutMultiplesOfThree(numbers);

        Console.WriteLine(
            $"Новий список: [{string.Join(", ", newList)}]");

        Console.WriteLine(
            $"Результати однакові: {AreListsEqual(reverseCopy, newList)}");
    }
}
