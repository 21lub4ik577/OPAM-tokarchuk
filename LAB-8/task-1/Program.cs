using System;

internal static class Program
{
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

    private static void Main()
    {
        Student[] students = (Student[])Common.students.Clone();
        Array.Sort(students, Common.CompareStudents);

        Student targetX = new("X", "КН-3/1", Common.X);
        Student first = students[0];
        Student missing = new("Missing", "КН-3/1", Common.NOT_IN_RANGE);

        PrintResult("X = 66", students, targetX);
        PrintResult($"Перший елемент (бал {first.Score})", students, first);
        PrintResult("Відсутній бал = 101", students, missing);
    }

    private static void PrintResult(string label, Student[] students, Student target)
    {
        int index = LinearSearch(students, target, Common.CompareStudents, out int comparisons);
        Console.WriteLine($"{label}: індекс = {index}, порівнянь = {comparisons}");
    }
}
