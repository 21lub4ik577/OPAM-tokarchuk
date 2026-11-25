using System.Diagnostics;
using System.Globalization;

internal static class Program
{
    private const int Capacity = 26;

    private static void Main()
    {
        var random = new Random(26);
        int optimalCount = 0;
        double worstGapPercent = 0;
        string worstExample = "у 100 тестах відставання не було";

        for (int test = 0; test < 100; test++)
        {
            int count = random.Next(0, 16);
            Item[] items = CreateItems(random, count);
            int greedy = Greedy(items, Capacity);
            int optimal = Exhaustive(items, Capacity);
            if (greedy == optimal)
                optimalCount++;

            double gap = optimal == 0 ? 0 : (optimal - greedy) * 100.0 / optimal;
            if (gap > worstGapPercent)
            {
                worstGapPercent = gap;
                worstExample = FormatItems(items, greedy, optimal);
            }
        }

        Console.WriteLine("Завдання 4. Рюкзак цілими предметами, місткість 26 кг");
        Console.WriteLine($"Тести з оптимумом: {optimalCount}/100");
        Console.WriteLine($"Найгірше відставання у 100 тестах: {worstGapPercent.ToString("F2", CultureInfo.InvariantCulture)}%");
        Console.WriteLine($"Відповідний тест: {worstExample}");

        Item[] counterexample =
        {
            new(1, 1),
            new(26, 20)
        };
        Console.WriteLine($"Найменший контрприклад (2 предмети): місткість 26; {FormatItems(counterexample, Greedy(counterexample, Capacity), Exhaustive(counterexample, Capacity))}");

        CheckEdge("нуль елементів", Array.Empty<Item>(), 0);
        CheckEdge("один елемент", new[] { new Item(4, 9) }, 9);

        foreach (int count in new[] { 15, 20 })
        {
            Item[] items = CreateItems(new Random(26 + count), count);
            double milliseconds = MeasureMilliseconds(() => Exhaustive(items, Capacity));
            Console.WriteLine($"Повний перебір, {count} предметів: {milliseconds.ToString("F3", CultureInfo.InvariantCulture)} мс");
        }
    }

    private static int Greedy(Item[] items, int capacity)
    {
        Item[] ordered = (Item[])items.Clone();
        Array.Sort(ordered, (left, right) =>
        {
            int ratioOrder = (right.Value / (double)right.Weight).CompareTo(left.Value / (double)left.Weight);
            return ratioOrder != 0 ? ratioOrder : left.Weight.CompareTo(right.Weight);
        });

        int weight = 0;
        int value = 0;
        foreach (Item item in ordered)
        {
            if (weight + item.Weight <= capacity)
            {
                weight += item.Weight;
                value += item.Value;
            }
        }
        return value;
    }

    private static int Exhaustive(Item[] items, int capacity)
    {
        int best = 0;
        Search(items, 0, 0, 0, capacity, ref best);
        return best;
    }

    private static void Search(Item[] items, int index, int weight, int value, int capacity, ref int best)
    {
        if (index == items.Length)
        {
            if (value > best)
                best = value;
            return;
        }

        Search(items, index + 1, weight, value, capacity, ref best);
        Item item = items[index];
        if (weight + item.Weight <= capacity)
            Search(items, index + 1, weight + item.Weight, value + item.Value, capacity, ref best);
    }

    private static Item[] CreateItems(Random random, int count)
    {
        var items = new Item[count];
        for (int i = 0; i < count; i++)
            items[i] = new Item(random.Next(1, 16), random.Next(1, 101));
        return items;
    }

    private static void CheckEdge(string label, Item[] items, int expected)
    {
        int actual = Exhaustive(items, Capacity);
        if (actual != expected)
            throw new InvalidOperationException($"Помилка крайового випадку «{label}».");
        Console.WriteLine($"Крайовий випадок «{label}»: {actual} — PASS");
    }

    private static string FormatItems(Item[] items, int greedy, int optimal)
    {
        var formattedItems = new string[items.Length];
        for (int i = 0; i < items.Length; i++)
            formattedItems[i] = $"({items[i].Weight} кг, {items[i].Value})";
        string list = string.Join(", ", formattedItems);
        return $"[{list}], жадібне {greedy}, оптимум {optimal}";
    }

    private static double MeasureMilliseconds(Func<int> action)
    {
        _ = action();
        double best = double.MaxValue;
        for (int i = 0; i < 3; i++)
        {
            var stopwatch = Stopwatch.StartNew();
            _ = action();
            stopwatch.Stop();
            best = Math.Min(best, stopwatch.Elapsed.TotalMilliseconds);
        }
        return best;
    }

    private static string FindDataFile()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);
        while (directory is not null)
        {
            string candidate = Path.Combine(directory.FullName, "data", "text.txt");
            if (File.Exists(candidate))
                return candidate;
            directory = directory.Parent;
        }
        throw new FileNotFoundException("Не знайдено data/text.txt у LAB-10.");
    }

    private readonly record struct Item(int Weight, int Value);
}
