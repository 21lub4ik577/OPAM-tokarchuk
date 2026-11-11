int[] values = Task1Data.CreateValues(7);
Console.WriteLine($"Перші 7 значень масиву: {string.Join(", ", values)}");
Console.WriteLine("Прохід | Стан масиву");

for (int pass = 1; pass < values.Length; pass++)
{
    bool swapped = false;
    for (int i = 0; i < values.Length - pass; i++)
    {
        if (values[i] <= values[i + 1])
        {
            continue;
        }

        (values[i], values[i + 1]) = (values[i + 1], values[i]);
        swapped = true;
    }

    Console.WriteLine($"{pass,6} | {string.Join(", ", values)}");
    if (!swapped)
    {
        break;
    }
}

int[] sample = Task1Data.CreateValues(7);
BubbleSort.Sort(sample, Comparer<int>.Default.Compare, out int comparisons, out int moves);
Console.WriteLine($"Підсумок: {string.Join(", ", sample)}");
Console.WriteLine($"Порівняння: {comparisons}; записи в масив: {moves}");
