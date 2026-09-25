using System;
class Program
{
    static void Main()
    {
        int N = 26;
        int A = 1000000 + N;
        int B = 999999 - N;

        int min;

        if (A < B)
        {
            min = A;
        }
        
        else
        {
            min = B;
        }

        int gcd1 = 1;
        int iterations1 = 0;

        for (int i = min; i >= 1; i--)
        {
            iterations1++;

            if (A % i == 0 && B % i == 0)
            {
                gcd1 = i;
                break;
            }
        }

        int x = A;
        int y = B;
        int iterations2 = 0;

        while (y != 0)
        {
            int remainder = x % y;
            x = y;
            y = remainder;
            iterations2++;
        }

        int gcd2 = x;

        Console.WriteLine("A = " + A);
        Console.WriteLine("B = " + B);

        Console.WriteLine("Перший спосіб:");
        Console.WriteLine("НСД = " + gcd1);
        Console.WriteLine("Кількість ітерацій = " + iterations1);

        Console.WriteLine("Алгоритм Евкліда:");
        Console.WriteLine("НСД = " + gcd2);
        Console.WriteLine("Кількість ітерацій = " + iterations2);
    }
}