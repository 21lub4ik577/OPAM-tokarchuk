using System;
using System.Diagnostics;

internal static class Program
{
    private static readonly int[] Sizes = { 1_000, 10_000, 100_000, 1_000_000 };
    private static readonly int[] QueryCounts = { 1, 5, 10, 20, 30, 50, 100 };

    private static int LinearSearch<T>(
        T[] array,
        T target,
        Func<T, T, int> compare,
        out int comparisons)
    {
        comparisons = 0;
        for (int i = 0; i < array.Length; i++)
        {
            comparisons++;
            if (compare(array[i], target) == 0)
            {
                return i;
            }
        }

        return -1;
    }

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

    private static void Main()
    {
        PrintComparisonTable();
        PrintTimingTable();
    }

    private static void PrintComparisonTable()
    {
        Console.WriteLine("Середня кількість порівнянь на 100 запитів:");
        Console.WriteLine("n\tЛінійний\tБінарний\tВідношення\tТеорія n/2 і log2(n)");

        foreach (int size in Sizes)
        {
            Student[] data = GenerateStudents(size, new Random(26));
            Array.Sort(data, Common.CompareStudents);
            Student[] queries = GenerateQueries(data, 100, new Random(1000 + size));
            long linearTotal = 0;
            long binaryTotal = 0;

            foreach (Student query in queries)
            {
                LinearSearch(data, query, Common.CompareStudents, out int linearComparisons);
                BinarySearch(data, query, Common.CompareStudents, out int binaryComparisons);
                linearTotal += linearComparisons;
                binaryTotal += binaryComparisons;
            }

            double linearAverage = linearTotal / 100.0;
            double binaryAverage = binaryTotal / 100.0;
            Console.WriteLine($"{size}\t{linearAverage:F2}\t\t{binaryAverage:F2}\t\t" +
                              $"{linearAverage / binaryAverage:F2}\t\t{size / 2.0:F0} і " +
                              $"{Math.Log2(size):F2}");
        }
    }

    private static void PrintTimingTable()
    {
        const int size = 100_000;
        Student[] unsorted = GenerateStudents(size, new Random(26));
        Student[] queries = GenerateQueries(unsorted, 50_000, new Random(8026));

        MeasureLinear(unsorted, queries, 1);
        MeasureSortAndBinary(unsorted, queries, 1);

        Console.WriteLine();
        Console.WriteLine("Release Stopwatch, прогрів виконано, кожен замір тричі (найменший час):");
        Console.WriteLine("q\tЛінійно, мс\tСортування + бінарно, мс\tШвидший");

        int firstWinningQueryCount = -1;
        foreach (int queryCount in QueryCounts)
        {
            double linearMilliseconds = BestOfThree(
                () => MeasureLinear(unsorted, queries, queryCount));
            double sortedMilliseconds = BestSortAndBinaryTime(unsorted, queries, queryCount);
            string winner = sortedMilliseconds < linearMilliseconds ? "сортування + бінарно" : "лінійно";
            if (firstWinningQueryCount == -1 && sortedMilliseconds < linearMilliseconds)
            {
                firstWinningQueryCount = queryCount;
            }

            Console.WriteLine($"{queryCount}\t{linearMilliseconds:F3}\t\t{sortedMilliseconds:F3}\t\t\t{winner}");
        }

        Console.WriteLine($"Перша точка окупності серед обов'язкових q: " +
                          $"{(firstWinningQueryCount == -1 ? "не знайдена" : firstWinningQueryCount)}.");

        int[] extendedQueryCounts = { 1_000, 5_000, 10_000, 20_000, 30_000, 35_000, 40_000, 45_000, 50_000 };
        int previousQueryCount = QueryCounts[^1];
        int measuredBreakEven = -1;
        foreach (int queryCount in extendedQueryCounts)
        {
            double linearMilliseconds = BestOfThree(
                () => MeasureLinear(unsorted, queries, queryCount));
            double sortedMilliseconds = BestSortAndBinaryTime(unsorted, queries, queryCount);
            Console.WriteLine($"Додаткова перевірка q={queryCount}: лінійно {linearMilliseconds:F3} мс, " +
                              $"сортування + бінарно {sortedMilliseconds:F3} мс.");
            if (measuredBreakEven == -1 && sortedMilliseconds < linearMilliseconds)
            {
                measuredBreakEven = queryCount;
                Console.WriteLine($"Перший виміряний q, де сортування окупилося: {measuredBreakEven} " +
                                  $"(між {previousQueryCount} та {measuredBreakEven}).");
                break;
            }

            previousQueryCount = queryCount;
        }

        if (measuredBreakEven == -1)
        {
            Console.WriteLine("Точку окупності не знайдено навіть до q=50 000.");
        }
    }

    private static double BestOfThree(Action action)
    {
        double bestMilliseconds = double.PositiveInfinity;
        for (int run = 0; run < 3; run++)
        {
            var stopwatch = Stopwatch.StartNew();
            action();
            stopwatch.Stop();
            bestMilliseconds = Math.Min(bestMilliseconds, stopwatch.Elapsed.TotalMilliseconds);
        }

        return bestMilliseconds;
    }

    private static void MeasureLinear(Student[] data, Student[] queries, int queryCount)
    {
        for (int i = 0; i < queryCount; i++)
        {
            LinearSearch(data, queries[i], Common.CompareStudents, out _);
        }
    }

    private static void MeasureSortAndBinary(Student[] data, Student[] queries, int queryCount)
    {
        Student[] sorted = (Student[])data.Clone();
        Array.Sort(sorted, Common.CompareStudents);
        for (int i = 0; i < queryCount; i++)
        {
            BinarySearch(sorted, queries[i], Common.CompareStudents, out _);
        }
    }

    private static double BestSortAndBinaryTime(Student[] data, Student[] queries, int queryCount)
    {
        double bestMilliseconds = double.PositiveInfinity;
        for (int run = 0; run < 3; run++)
        {
            Student[] sorted = (Student[])data.Clone();
            var stopwatch = Stopwatch.StartNew();
            Array.Sort(sorted, Common.CompareStudents);
            for (int i = 0; i < queryCount; i++)
            {
                BinarySearch(sorted, queries[i], Common.CompareStudents, out _);
            }

            stopwatch.Stop();
            bestMilliseconds = Math.Min(bestMilliseconds, stopwatch.Elapsed.TotalMilliseconds);
        }

        return bestMilliseconds;
    }

    private static Student[] GenerateStudents(int size, Random random)
    {
        Student[] students = new Student[size];
        for (int i = 0; i < students.Length; i++)
        {
            students[i] = new Student($"Student{i}", "КН-3/1", random.Next(0, 101));
        }

        return students;
    }

    private static Student[] GenerateQueries(Student[] data, int count, Random random)
    {
        Student[] queries = new Student[count];
        for (int i = 0; i < queries.Length; i++)
        {
            queries[i] = data[random.Next(data.Length)];
        }

        return queries;
    }
}
