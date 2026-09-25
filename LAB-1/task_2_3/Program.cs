using System;
class Program
{
    static void Main()
    {
        Console.Write("Введіть натуральне число: ");
        int number;

        if (!int.TryParse(Console.ReadLine(), out number) || number <= 0)
        {
            Console.WriteLine("Помилка введення");
            return;
        }

        int sum = 0;
        int zeros = 0;
        int selectedSum = 0;

        while (number > 0)
        {
            int digit = number % 10;
            sum += digit;

            if (digit == 0)
            {
                zeros++;
            }

            if (digit % 2 != 0 && digit < 16)
            {
                selectedSum += digit;
            }

            number = number / 10;
        }

        Console.WriteLine("Сума всіх цифр: " + sum);
        Console.WriteLine("Кількість нулів: " + zeros);
        Console.WriteLine("Сума непарних цифр менших за 16: " + selectedSum);
    }
}