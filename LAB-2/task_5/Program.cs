using System;

class Program
{
    static void Task5(int[,] matrix)
    {
        int rows = matrix.GetLength(0);
        int columns = matrix.GetLength(1);

        Console.WriteLine("Матриця:");

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                Console.Write($"{matrix[i, j],4}");
            }

            Console.WriteLine();
        }

        Console.WriteLine();

        int maxRowSum = 0;
        int maxRowIndex = 0;

        Console.WriteLine("Сума кожного рядка:");

        for (int i = 0; i < rows; i++)
        {
            int rowSum = 0;

            for (int j = 0; j < columns; j++)
            {
                rowSum += matrix[i, j];
            }

            Console.WriteLine($"Рядок {i}: {rowSum}");

            if (i == 0 || rowSum > maxRowSum)
            {
                maxRowSum = rowSum;
                maxRowIndex = i;
            }
        }

        Console.WriteLine();
        Console.WriteLine("Максимум кожного стовпця:");

        for (int j = 0; j < columns; j++)
        {
            int max = matrix[0, j];

            for (int i = 1; i < rows; i++)
            {
                if (matrix[i, j] > max)
                {
                    max = matrix[i, j];
                }
            }

            Console.WriteLine($"Стовпець {j}: {max}");
        }

        Console.WriteLine();
        Console.WriteLine("Номер рядка з найбільшою сумою: " + maxRowIndex);
    }

    static void Main()
    {
        int[,] matrix = new int[3, 6];

        Random random = new Random(26);

        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                matrix[i, j] = random.Next(1, 5);
            }
        }

        Task5(matrix);
    }
}