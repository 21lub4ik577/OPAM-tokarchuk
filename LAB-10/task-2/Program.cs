using System.Diagnostics;
using System.Globalization;
using System.Text;

internal static class Program
{
    private static void Main()
    {
        string prose = File.ReadAllText(FindDataFile());
        Console.WriteLine("Завдання 2. Нормалізація: нижній регістр, слова через один пробіл");
        Console.WriteLine("| Символів | Наївно, мс | Швидко, мс | У скільки разів |");
        Console.WriteLine("|---:|---:|---:|---:|");

        foreach (int size in new[] { 1_000, 10_000, 50_000 })
        {
            string input = prose.Substring(0, size);
            string naiveResult = NormalizeNaive(input);
            string fastResult = NormalizeFast(input);
            EnsureSame(naiveResult, fastResult, $"текст довжини {size}");
            double naiveMs = MeasureMilliseconds(() => NormalizeNaive(input));
            double fastMs = MeasureMilliseconds(() => NormalizeFast(input));
            Console.WriteLine($"| {size:N0} | {Format(naiveMs)} | {Format(fastMs)} | {Format(naiveMs / Math.Max(fastMs, 0.000001))}x |");
        }

        Console.WriteLine();
        Console.WriteLine("Крайові випадки:");
        CheckEdge("порожній рядок", string.Empty);
        CheckEdge("один символ", "А");
        CheckEdge("усі символи однакові", "лллллллллл");
        CheckEdge("пробіли з країв і подвійний усередині", "   ДІМ   біля  річки  ");
        Console.WriteLine("Алгоритми щоразу повертають однаковий рядок (Ordinal).");
    }

    private static string NormalizeNaive(string input)
    {
        string text = input;
        foreach (char whitespace in WhitespaceCharacters)
            text = text.Replace(whitespace, ' ');
        while (text.Contains("  ", StringComparison.Ordinal))
            text = text.Replace("  ", " ", StringComparison.Ordinal);

        text = text.Trim();
        var words = new List<string>();
        while (text.Length > 0)
        {
            int separator = text.IndexOf(' ');
            if (separator < 0)
            {
                words.Add(text.Substring(0).ToLowerInvariant());
                break;
            }

            words.Add(text.Substring(0, separator).ToLowerInvariant());
            text = text.Substring(separator + 1);
        }
        return string.Join(" ", words);
    }

    private static string NormalizeFast(string input)
    {
        var result = new StringBuilder(input.Length);
        int read = 0;
        while (read < input.Length)
        {
            while (read < input.Length && char.IsWhiteSpace(input[read]))
                read++;
            int wordStart = read;
            while (read < input.Length && !char.IsWhiteSpace(input[read]))
                read++;
            if (wordStart == read)
                continue;

            if (result.Length > 0)
                result.Append(' ');
            for (int write = wordStart; write < read; write++)
                result.Append(char.ToLowerInvariant(input[write]));
        }
        return result.ToString();
    }

    private static void CheckEdge(string label, string input)
    {
        string naive = NormalizeNaive(input);
        string fast = NormalizeFast(input);
        EnsureSame(naive, fast, label);
        Console.WriteLine($"  {label}: «{fast}» — PASS");
    }

    private static void EnsureSame(string left, string right, string label)
    {
        if (!string.Equals(left, right, StringComparison.Ordinal))
            throw new InvalidOperationException($"Результати не збігаються для: {label}.");
    }

    private static double MeasureMilliseconds(Func<string> action)
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

    private static readonly char[] WhitespaceCharacters =
    {
        '\t', '\n', '\v', '\f', '\r', '\u0085', '\u00a0', '\u1680',
        '\u2000', '\u2001', '\u2002', '\u2003', '\u2004', '\u2005',
        '\u2006', '\u2007', '\u2008', '\u2009', '\u200a', '\u2028',
        '\u2029', '\u202f', '\u205f', '\u3000'
    };

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
