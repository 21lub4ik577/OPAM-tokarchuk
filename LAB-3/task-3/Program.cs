using System;
using System.Collections.Generic;

class Program
{
    static int targetSum = 26;
    static int solutionCount = 0;

    static void FindSubsets(
        int[] numbers,
        int index,
        int currentSum,
        List<int> currentSubset)
    {
        if (currentSum == targetSum)
        {
            solutionCount++;

            Console.WriteLine(
                $"{solutionCount}. [{string.Join(", ", currentSubset)}]");

            return;
        }

        if (index >= numbers.Length || currentSum > targetSum)
        {
            return;
        }

        currentSubset.Add(numbers[index]);

        FindSubsets(
            numbers,
            index + 1,
            currentSum + numbers[index],
            currentSubset);

        currentSubset.RemoveAt(currentSubset.Count - 1);

        FindSubsets(
            numbers,
            index + 1,
            currentSum,
            currentSubset);
    }

    static void Main()
    {
        const int N = 26;
        const int K = 16;
        const int ArraySize = 10;

        const int S = K % 20 + 10;

        Random random = new Random(N);

        int[] numbers = new int[ArraySize];

        for (int i = 0; i < numbers.Length; i++)
        {
            numbers[i] = random.Next(1, 10);
        }

        Console.WriteLine("Array:");
        Console.WriteLine(string.Join(", ", numbers));

        Console.WriteLine($"\nTarget sum: {S}");
        Console.WriteLine("\nSubsets:");

        FindSubsets(numbers, 0, 0, new List<int>());

        Console.WriteLine($"\nNumber of subsets found: {solutionCount}");
    }
}