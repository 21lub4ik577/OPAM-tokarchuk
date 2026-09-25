using System;
class Program
{
    static void Main()
    {
        Console.Write("Введіть перше число: ");
        int a;
        if (!int.TryParse(Console.ReadLine(), out a))
        {
            Console.WriteLine("Помилка введення");
            return;
        }

        Console.Write("Введіть друге число: ");
        int b;
        if (!int.TryParse(Console.ReadLine(), out b))
        {
            Console.WriteLine("Помилка введення");
            return;
        }

        Console.Write("Введіть третє число: ");
        int c;
        if (!int.TryParse(Console.ReadLine(), out c))
        {
            Console.WriteLine("Помилка введення");
            return;
        }


        //MAX
        int max = a;

        if ( a >= b && a >= c )
        {
            max = a;
        }
        
        else if ( b >= a && b >= c)
        {
            max = b;
        }

        else if ( c >= a && c >= b)
        {
            max = c;
        }

        //MIN
        int min = a;

        if ( a <= b && a <= c )
        {
            min = a;
        }
        
        else if ( b <= a && b <= c)
        {
            min= b;
        }

        else if ( c <= a && c <= b)
        {
            min = c;
        }

        //MIDDLE
        int middle = a + b + c - max - min;
        
        Console.WriteLine(max);
        Console.WriteLine(middle);        
        Console.WriteLine(min);

        if ( a == b && b == c)
        {
            Console.WriteLine("різних значень - 1");
        }

        else if ( a == b || b == c || a == c)
        {
            Console.WriteLine("різних значень - 2");
        }

        else if ( a != b && b != c && a != c)
        {
            Console.WriteLine("різних значень - 3");
        }

    }
}