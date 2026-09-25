using System;

class Program
{
    static void Task4(int[] a)
    {
        if (a.Length == 0)
        {
            Console.WriteLine("Порожній масив.");
            return;
        }

        int first = a[0];

        for (int i = 0; i < a.Length - 1; i++)
        {
            a[i] = a[i + 1];
        }

        a[a.Length - 1] = first;
    }

    static void Main()
    {
        int[] a = new int[24];

        Random random = new Random(26);

        for (int i = 0; i < a.Length; i++)
        {
            a[i] = random.Next(1, 5);
        }

        Console.WriteLine("Масив до перестановки:");

        for (int i = 0; i < a.Length; i++)
        {
            Console.Write(a[i] + " ");
        }

        Console.WriteLine();

        Task4(a);

        Console.WriteLine("Масив після перестановки:");

        for (int i = 0; i < a.Length; i++)
        {
            Console.Write(a[i] + " ");
        }

        Console.WriteLine();
    }
}