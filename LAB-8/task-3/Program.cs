using System;

internal static class Program
{
    private static int FindBoundary<T>(T[] array, Func<T, int> condition, out int comparisons)
    {
        int low = 0;
        int high = array.Length;
        comparisons = 0;

        while (low < high)
        {
            int middle = low + (high - low) / 2;
            comparisons++;
            if (condition(array[middle]) >= 0)
            {
                high = middle;
            }
            else
            {
                low = middle + 1;
            }
        }

        return low;
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
        Student[] students = (Student[])Common.students.Clone();
        Array.Sort(students, Common.CompareStudents);
        Student targetX = new("X", "КН-3/1", Common.X);

        int lower = FindBoundary(
            students,
            student => Common.CompareStudents(student, targetX),
            out int lowerComparisons);
        int upper = FindBoundary(
            students,
            student => Common.CompareStudents(student, targetX) > 0 ? 1 : -1,
            out int upperComparisons);

        Student targetY = new("Y", "КН-3/1", Common.Y);
        int afterY = FindBoundary(
            students,
            student => Common.CompareStudents(student, targetY) > 0 ? 1 : -1,
            out int afterYComparisons);

        Console.WriteLine($"LowerBound(66): індекс = {lower}, порівнянь = {lowerComparisons}");
        Console.WriteLine($"UpperBound(66): індекс = {upper}, порівнянь = {upperComparisons}");
        Console.WriteLine($"Кількість оцінок 66: {upper - lower} (без повного проходу)");
        Console.WriteLine($"Діапазон [66; 76]: {afterY - lower}, порівнянь меж = " +
                          $"{lowerComparisons + afterYComparisons}");
        Console.WriteLine($"Запит варіанта b = 2 — перший бал не менший за 66: " +
                          $"індекс = {lower}, порівнянь = {lowerComparisons}");

        Student[] belowX = (Student[])students.Clone();
        Array.Sort(belowX, Common.CompareStudents);
        Student lowerTarget = new("low", "КН-3/1", -1);
        int belowIndex = FindBoundary(
            belowX,
            student => Common.CompareStudents(student, lowerTarget),
            out int belowComparisons);
        Student aboveX = new("high", "КН-3/1", 101);
        int aboveIndex = FindBoundary(
            students,
            student => Common.CompareStudents(student, aboveX),
            out int aboveComparisons);
        Console.WriteLine($"Крайові межі: менше за всі = {belowIndex} ({belowComparisons} порівнянь), " +
                          $"більше за всі = {aboveIndex} ({aboveComparisons} порівнянь)");

        int actualMultipleIndex = -1;
        for (int i = 0; i < students.Length; i++)
        {
            if (students[i].Score % 7 == 0)
            {
                actualMultipleIndex = i;
                break;
            }
        }

        int invalidConditionIndex = FindBoundary(
            students,
            student => student.Score % 7 == 0 ? 1 : -1,
            out int invalidComparisons);
        Console.WriteLine($"Некоректна для межі умова (бал кратний 7): отримано індекс " +
                          $"{invalidConditionIndex} за {invalidComparisons} перевірок; " +
                          $"перший справжній індекс = {actualMultipleIndex}; " +
                          $"відповідь правильна = {invalidConditionIndex == actualMultipleIndex}");

        Console.WriteLine();
        Console.WriteLine("Крайові випадки:");
        PrintEdgeCase("порожній масив", Array.Empty<Student>(), targetX, Common.CompareStudents);
        PrintEdgeCase("один елемент, він же шуканий",
            new[] { new Student("A", "КН-3/1", 66) }, targetX, Common.CompareStudents);
        PrintEdgeCase("два елементи, шуканий — другий",
            new[] { new Student("A", "КН-3/1", 65), new Student("B", "КН-3/1", 66) },
            targetX,
            Common.CompareStudents);

        Student[] allEqual =
        {
            new("A", "КН-3/1", 66),
            new("B", "КН-3/1", 66),
            new("C", "КН-3/1", 66)
        };
        int equalLow = FindBoundary(
            allEqual,
            student => Common.CompareStudents(student, targetX),
            out int equalLowComparisons);
        int equalHigh = FindBoundary(
            allEqual,
            student => Common.CompareStudents(student, targetX) > 0 ? 1 : -1,
            out int equalHighComparisons);
        Console.WriteLine($"усі елементи дорівнюють X: перша = {equalLow}, остання = {equalHigh - 1}; " +
                          $"порівнянь = {equalLowComparisons + equalHighComparisons}");
    }

    private static void PrintEdgeCase(
        string label,
        Student[] array,
        Student target,
        Func<Student, Student, int> compare)
    {
        int index = BinarySearch(array, target, compare, out int searchComparisons);
        int boundary = FindBoundary(
            array,
            item => compare(item, target),
            out int boundaryComparisons);
        Console.WriteLine($"{label}: пошук = {index} ({searchComparisons}), межа = " +
                          $"{boundary} ({boundaryComparisons})");
    }
}
