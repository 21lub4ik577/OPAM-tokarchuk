using System.Diagnostics;
using System.Globalization;

internal static class Program
{
    private static void Main()
    {
        string prose = File.ReadAllText(FindDataFile());
        string[] words = prose.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);

        Console.WriteLine("Завдання 1. Пастки рядків");
        Console.WriteLine($"Вхід: {prose.Length:N0} символів, {words.Length:N0} слів");
        foreach (int count in new[] { 10_000, 100_000 })
        {
            double concatenation = MeasureMilliseconds(() => BuildWithConcatenation(count));
            double builder = MeasureMilliseconds(() => BuildWithBuilder(count));
            Console.WriteLine($"{count:N0} додавань: += {Format(concatenation)} мс; StringBuilder {Format(builder)} мс; різниця {Format(concatenation / Math.Max(builder, 0.000001))}x");
        }

        const int comparisons = 100_000;
        if (CompareWithLower(words, comparisons) != CompareOrdinalIgnoreCase(words, comparisons))
            throw new InvalidOperationException("Порівняння без урахування регістру мають давати однаковий результат.");
        double lowerTime = MeasureMilliseconds(() => CompareWithLower(words, comparisons));
        double ordinalIgnoreCaseTime = MeasureMilliseconds(() => CompareOrdinalIgnoreCase(words, comparisons));
        Console.WriteLine($"{comparisons:N0} порівнянь: ToLower {Format(lowerTime)} мс; OrdinalIgnoreCase {Format(ordinalIgnoreCaseTime)} мс");

        string[] sample =
        {
            "глина", "ґава", "дім", "ґрунт", "гілка", "дорога", "ґудзик", "гніздо",
            "дощ", "ґрати", "гарний", "долина", "ґатунок", "гілля", "дуб", "ґердан",
            "ґніт", "гомін", "диво", "ґуральня"
        };
        string[] ordinal = (string[])sample.Clone();
        string[] ukrainian = (string[])sample.Clone();
        Array.Sort(ordinal, StringComparer.Ordinal);
        Array.Sort(ukrainian, StringComparer.Create(CultureInfo.GetCultureInfo("uk-UA"), ignoreCase: false));
        int differentAt = -1;
        for (int i = 0; i < ordinal.Length; i++)
        {
            if (!string.Equals(ordinal[i], ukrainian[i], StringComparison.Ordinal))
            {
                differentAt = i;
                break;
            }
        }
        Console.WriteLine("Ординальний порядок: " + string.Join(", ", ordinal));
        Console.WriteLine("Порядок uk-UA:        " + string.Join(", ", ukrainian));
        Console.WriteLine(differentAt >= 0
            ? $"Порядки різняться з позиції {differentAt + 1}: «{ordinal[differentAt]}» проти «{ukrainian[differentAt]}»."
            : "ПОМИЛКА: порядки збіглися.");
        if (differentAt < 0)
            throw new InvalidOperationException("Сортування має показати різний порядок.");
    }

    private static long BuildWithConcatenation(int count)
    {
        string result = string.Empty;
        for (int i = 0; i < count; i++)
            result += 'Л';
        return result.Length;
    }

    private static long BuildWithBuilder(int count)
    {
        var result = new System.Text.StringBuilder(count);
        for (int i = 0; i < count; i++)
            result.Append('Л');
        return result.Length;
    }

    private static long CompareWithLower(string[] words, int count)
    {
        long equal = 0;
        for (int i = 0; i < count; i++)
        {
            string left = words[i % words.Length];
            string right = left.ToUpper(CultureInfo.CurrentCulture);
            if (left.ToLower(CultureInfo.CurrentCulture) == right.ToLower(CultureInfo.CurrentCulture))
                equal++;
        }
        return equal;
    }

    private static long CompareOrdinalIgnoreCase(string[] words, int count)
    {
        long equal = 0;
        for (int i = 0; i < count; i++)
        {
            string left = words[i % words.Length];
            string right = left.ToUpperInvariant();
            if (string.Equals(left, right, StringComparison.OrdinalIgnoreCase))
                equal++;
        }
        return equal;
    }

    private static double MeasureMilliseconds(Func<long> action)
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

    private static string Format(double value) => value.ToString("F3", CultureInfo.InvariantCulture);
}
