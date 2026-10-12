using System;

class Program
{
    static bool SolveRecursive(int[] a, int lo, int hi)
    {
        if (lo == hi)
        {
            return true;
        }

        int mid = lo + (hi - lo) / 2;

        bool leftResult = SolveRecursive(a, lo, mid);
        bool rightResult = SolveRecursive(a, mid + 1, hi);

        return leftResult && rightResult && a[lo] == a[hi];
    }

    static bool SolveIterative(int[] a)
    {
        if (a.Length == 0)
        {
            return true;
        }

        for (int i = 1; i < a.Length; i++)
        {
            if (a[i] != a[0])
            {
                return false;
            }
        }

        return true;
    }

    static void Main()
    {
        const int N = 26;
        const int K = 16;

        Random random = new Random(N);
        int[] numbers = new int[K];

        for (int i = 0; i < numbers.Length; i++)
        {
            numbers[i] = random.Next(-20, 21);
        }

        Console.WriteLine("Array:");
        Console.WriteLine(string.Join(", ", numbers));

        bool recursiveResult = SolveRecursive(numbers, 0, numbers.Length - 1);

        bool iterativeResult = SolveIterative(numbers);

        Console.WriteLine();
        Console.WriteLine($"Recursive result: {recursiveResult}");
        Console.WriteLine($"Iterative result: {iterativeResult}");
        Console.WriteLine($"Results are equal: {recursiveResult == iterativeResult}");
    }
}