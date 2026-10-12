using System;

class Program
{
    static int maxDepth = 0;

    static bool Solve(int[] a, int lo, int hi, int depth = 1)
    {
        if (depth > maxDepth)
        {
            maxDepth = depth;
        }

        if (lo == hi)
        {
            return true;
        }

        int mid = lo + (hi - lo) / 2;

        bool leftResult = Solve(a, lo, mid, depth + 1);
        bool rightResult = Solve(a, mid + 1, hi, depth + 1);

        return leftResult && rightResult && a[lo] == a[hi];
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

        bool result = Solve(numbers, 0, numbers.Length - 1);

        Console.WriteLine();
        Console.WriteLine($"All elements are equal: {result}");
        Console.WriteLine($"Maximum recursion depth: {maxDepth}");
        Console.WriteLine($"log2({K}) = {Math.Log2(K)}");
    }
}