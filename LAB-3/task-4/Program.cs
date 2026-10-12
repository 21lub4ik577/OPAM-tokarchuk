using System;

class Program
{
    static bool Solve(int[] a, int lo, int hi, int depth = 0)
    {
        string indent = new string(' ', depth * 2);

        Console.WriteLine($"{indent}enter Solve({lo}..{hi})");

        bool result;

        if (lo == hi)
        {
            result = true;
        }
        else
        {
            int mid = lo + (hi - lo) / 2;

            bool leftResult = Solve(a, lo, mid, depth + 1);
            bool rightResult = Solve(a, mid + 1, hi, depth + 1);

            result = leftResult && rightResult && a[lo] == a[hi];
        }

        Console.WriteLine(
            $"{indent}exit  Solve({lo}..{hi}) -> {result}");

        return result;
    }

    static void Main()
    {
        int[] numbers = { 5, 5, 5, 5 };

        Console.WriteLine("Array: " + string.Join(", ", numbers));
        Console.WriteLine();

        bool result = Solve(numbers, 0, numbers.Length - 1);

        Console.WriteLine();
        Console.WriteLine($"All elements are equal: {result}");
    }
}