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
            if (compare(array[middle], target) == 0)
            {
                return middle;
            }

            comparisons++;
            if (compare(array[middle], target) < 0)
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

    private static int BinarySearchWithoutLoopEquality<T>(
        T[] array,
        T target,
        Func<T, T, int> compare,
        out int comparisons)
    {
        int low = 0;
        int high = array.Length;
        comparisons = 0;

        while (low < high)
        {
            int middle = low + (high - low) / 2;
            comparisons++;
            if (compare(array[middle], target) < 0)
            {
                low = middle + 1;
            }
            else
            {
                high = middle;
            }
        }

        if (low == array.Length)
        {
            return -1;
        }

        comparisons++;
        return compare(array[low], target) == 0 ? low : -1;
    }

    private static void Main()
    {
        Student[] students = (Student[])Common.students.Clone();
        Array.Sort(students, Common.CompareStudents);
        Student targetX = new("X", "КН-3/1", Common.X);
        Student first = students[0];
        Student missing = new("Missing", "КН-3/1", Common.NOT_IN_RANGE);

        Console.WriteLine("Варіант c = 1 — бінарний пошук без перевірки рівності в циклі:");
        PrintResult("X = 66", students, targetX);
        PrintResult($"Перший елемент (бал {first.Score})", students, first);
        PrintResult("Відсутній бал = 101", students, missing);
        Console.WriteLine();
        Console.WriteLine("У варіанті в циклі одне порівняння визначає напрямок, а після звуження " +
                          "меж додається одна перевірка рівності. Класичний варіант перевіряє " +
                          "рівність і напрямок окремо (до двох порівнянь за крок); натомість " +
                          "варіант доходить до кандидата й тому може зробити на один крок більше.");
    }

    private static void PrintResult(string label, Student[] array, Student target)
    {
        int classicIndex = BinarySearch(array, target, Common.CompareStudents, out int classicComparisons);
        int variantIndex = BinarySearchWithoutLoopEquality(
            array,
            target,
            Common.CompareStudents,
            out int variantComparisons);
        Console.WriteLine($"{label}: класичний індекс = {classicIndex}, порівнянь = {classicComparisons}; " +
                          $"варіант індекс = {variantIndex}, порівнянь = {variantComparisons}");
    }
}
