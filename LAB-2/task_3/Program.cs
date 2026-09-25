using System;

class Program
{
    static void Task3(int[] a)
    {
        if (a.Length == 0)
        {
            Console.WriteLine("Для порожнього масиву результат не визначений.");
            return;
        }

        int bestValue = a[0];
        int bestLength = 1;
        int bestStart = 0;

        int currentLength = 1;
        int currentStart = 0;

        for (int i = 1; i < a.Length; i++)
        {
            if (a[i] == a[i - 1])
            {
                currentLength++;
            }
            else
            {
                if (currentLength > bestLength)
                {
                    bestLength = currentLength;
                    bestValue = a[i - 1];
                    bestStart = currentStart;
                }

                currentLength = 1;
                currentStart = i;
            }
        }

        if (currentLength > bestLength)
        {
            bestLength = currentLength;
            bestValue = a[a.Length - 1];
            bestStart = currentStart;
        }

        Console.WriteLine("Значення: " + bestValue);
        Console.WriteLine("Довжина серії: " + bestLength);
        Console.WriteLine("Індекс початку: " + bestStart);
    }

    static void Main()
    {
        int[] a = new int[24];

        Random random = new Random(26);

        for (int i = 0; i < a.Length; i++)
        {
            a[i] = random.Next(1, 5);
        }

        Console.WriteLine("Масив:");

        for (int i = 0; i < a.Length; i++)
        {
            Console.Write(a[i] + " ");
        }

        Console.WriteLine();
        Console.WriteLine();

        Task3(a);
    }
}