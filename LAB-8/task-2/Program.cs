using System;

internal static class Program
{
    private static int BinarySearch<T>(
        T[] array,
        T target,
        Func<T, T, int> compare,
        out int comparisons)
    {
        int low = 0;
        int high = array.Length - 1;
        comparisons = 0;

        while (low <= high)
        {
            int middle = low + (high - low) / 2;
            comparisons++;
            int result = compare(array[middle], target);
            if (result == 0)
            {
                return middle;
            }

            if (result < 0)
            {
                low = middle + 1;
            }
            else
            {
                high = middle - 1;
            }
        }

        return -1;
    }

    private static int BrokenWhileLessThan<T>(
        T[] array,
        T target,
        Func<T, T, int> compare,
        out int comparisons)
    {
        int low = 0;
        int high = array.Length - 1;
        comparisons = 0;

        while (low < high)
        {
            int middle = low + (high - low) / 2;
            comparisons++;
            int result = compare(array[middle], target);
            if (result == 0)
            {
                return middle;
            }

            if (result < 0)
            {
                low = middle + 1;
            }
            else
            {
                high = middle - 1;
            }
        }

        return -1;
    }

    private static (int Result, int Low, int High, int Iterations, bool Stalled) BrokenLowEqualsMiddle<T>(
        T[] array,
        T target,
        Func<T, T, int> compare,
        int iterationLimit,
        out int comparisons)
    {
        int low = 0;
        int high = array.Length - 1;
        int iterations = 0;
        comparisons = 0;

        while (low <= high && iterations < iterationLimit)
        {
            iterations++;
            int middle = low + (high - low) / 2;
            comparisons++;
            int result = compare(array[middle], target);
            if (result == 0)
            {
                return (middle, low, high, iterations, false);
            }

            if (result < 0)
            {
                low = middle;
            }
            else
            {
                high = middle - 1;
            }
        }

        return (-1, low, high, iterations, low <= high);
    }

    private static void Main()
    {
        Student[] sorted = (Student[])Common.students.Clone();
        Array.Sort(sorted, Common.CompareStudents);
        Student targetX = new("X", "КН-3/1", Common.X);
        Student first = sorted[0];
        Student missing = new("Missing", "КН-3/1", Common.NOT_IN_RANGE);

        Console.WriteLine("Правильний бінарний пошук:");
        PrintResult("X = 66", sorted, targetX);
        PrintResult($"Перший елемент (бал {first.Score})", sorted, first);
        PrintResult("Відсутній бал = 101", sorted, missing);

        Console.WriteLine();
        Console.WriteLine("Поломка 1 — умова low < high:");
        Student[] twoSorted = { new("A", "КН-3/1", 1), new("B", "КН-3/1", 2) };
        Student secondValue = new("B", "КН-3/1", 2);
        int brokenIndex = BrokenWhileLessThan(
            twoSorted,
            secondValue,
            Common.CompareStudents,
            out int brokenComparisons);
        Console.WriteLine($"У масиві [1, 2] бал 2 є, але результат пошуку = {brokenIndex} " +
                          $"({brokenComparisons} порівняння).");

        Console.WriteLine();
        Console.WriteLine("Поломка 2 — low = middle (ліміт 20 ітерацій):");
        Student[] twoElements = { new("A", "КН-3/1", 1), new("B", "КН-3/1", 2) };
        var broken = BrokenLowEqualsMiddle(
            twoElements,
            new Student("B", "КН-3/1", 2),
            Common.CompareStudents,
            20,
            out int stalledComparisons);
        Console.WriteLine($"Шукаємо другий із двох елементів: ітерацій = {broken.Iterations}, " +
                          $"порівнянь = {stalledComparisons}, межі = [{broken.Low}, {broken.High}], " +
                          $"межі не рухаються = {broken.Stalled}");

        Console.WriteLine();
        Console.WriteLine("Поломка 3 — mid = (low + high) / 2:");
        const long firstFailingLength = 1_073_741_825;
        int overflowedMiddle = unchecked((int)((firstFailingLength - 1) + (firstFailingLength - 1)));
        Console.WriteLine($"Переповнення можливе від довжини {firstFailingLength}: " +
                          $"для останнього індексу low + high стає {overflowedMiddle}; " +
                          "для 100 000 елементів найбільша сума індексів менша за Int32.MaxValue.");

        Console.WriteLine();
        Console.WriteLine("Поломка 4 — пошук у невідсортованому масиві:");
        Student[] unsorted = (Student[])sorted.Clone();
        Shuffle(unsorted, new Random(26));
        Random queryRandom = new(2026);
        int failures = 0;
        for (int i = 0; i < 100; i++)
        {
            Student query = unsorted[queryRandom.Next(unsorted.Length)];
            if (BinarySearch(unsorted, query, Common.CompareStudents, out _) == -1)
            {
                failures++;
            }
        }

        Console.WriteLine($"Наявні значення, що повернули -1: {failures} із 100.");
    }

    private static void PrintResult(string label, Student[] array, Student target)
    {
        int index = BinarySearch(array, target, Common.CompareStudents, out int comparisons);
        Console.WriteLine($"{label}: індекс = {index}, порівнянь = {comparisons}");
    }

    private static void Shuffle<T>(T[] array, Random random)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (array[i], array[j]) = (array[j], array[i]);
        }
    }
}
