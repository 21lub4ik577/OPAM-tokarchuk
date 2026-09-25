using System;

class Program
{
    static int[] Task2(int[] a)
    {
        if (a.Length == 0)
        {
            Console.WriteLine("Для порожнього масиву результат не визначений.");
            return new int[0];
        }

        int count = 0;

        for (int i = 0; i < a.Length; i++)
        {
            if (a[i] < 1)
            {
                count++;
            }
        }

        int[] result = new int[count];

        int index = 0;

        for (int i = 0; i < a.Length; i++)
        {
            if (a[i] < 1)
            {
                result[index] = a[i];
                index++;
            }
        }

        return result;
    }

    static void Main()
    {
        int[] a = new int[24];

        Random random = new Random(26);

        for (int i = 0; i < a.Length; i++)
        {
            a[i] = random.Next(1, 5);
        }

        Console.WriteLine("Початковий масив:");

        for (int i = 0; i < a.Length; i++)
        {
            Console.Write(a[i] + " ");
        }

        Console.WriteLine();

        int[] result = Task2(a);

        Console.WriteLine("Відібрані елементи:");

        for (int i = 0; i < result.Length; i++)
        {
            Console.Write(result[i] + " ");
        }

        Console.WriteLine();
        Console.WriteLine("Кількість відібраних елементів: " + result.Length);
    }
}