using System;

class Program
{
    static void Task1(int[] a)
    {
        if (a.Length == 0)
        {
            Console.WriteLine("Для порожнього масиву результат не визначений.");
            return;
        }

        int sum = 0;
        int min = a[0];
        int max = a[0];

        int minIndex = 0;
        int maxIndex = 0;

        int zeroCount = 0;

        for (int i = 0; i < a.Length; i++)
        {
            sum += a[i];

            if (a[i] < min)
            {
                min = a[i];
                minIndex = i;
            }

            if (a[i] > max)
            {
                max = a[i];
                maxIndex = i;
            }

            if (a[i] == 0)
            {
                zeroCount++;
            }
        }

        double average = (double)sum / a.Length;

        Console.WriteLine("Сума: " + sum);
        Console.WriteLine("Середнє: " + average);
        Console.WriteLine("Мінімум: " + min);
        Console.WriteLine("Індекс мінімуму: " + minIndex);
        Console.WriteLine("Максимум: " + max);
        Console.WriteLine("Індекс максимуму: " + maxIndex);
        Console.WriteLine("Кількість нулів: " + zeroCount);
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

        Task1(a);
    }
}