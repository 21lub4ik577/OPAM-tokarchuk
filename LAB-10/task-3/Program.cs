using System.Diagnostics;
using System.Globalization;

internal static class Program
{
    private static void Main()
    {
        string prose = File.ReadAllText(FindDataFile());
        string[] words = prose.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
        string presentWord = words[500].Trim('.', ',', '!', '?', ':', ';', '«', '»', '"', '(', ')');
        if (presentWord.Length == 0)
            throw new InvalidOperationException("Не вдалося вибрати слово з прози.");

        string repeated = new('Л', 10_000);
        string difficultPattern = new string('Л', 1_059) + "М";

        Console.WriteLine("Завдання 3. Наївний пошук підрядка (порівняння символів)");
        RunCase($"проза, слово «{presentWord}» не на початку", prose.Substring(0, 50_000), presentWord);
        RunCase("проза, слова немає", prose.Substring(0, 50_000), "надзвичайнокосмічнийвигаданийтекст");
        RunCase("10 000 повторів, 1 059 × Л + М", repeated, difficultPattern);

        CheckEdge("порожній текст і шаблон", string.Empty, string.Empty, 0);
        CheckEdge("шаблон довший за текст", "кіт", "котик", -1);
        CheckEdge("порожній шаблон", "кіт", string.Empty, 0);
    }

    private static void RunCase(string label, string text, string pattern)
    {
        int result = NaiveIndexOf(text, pattern, out int comparisons);
        int reference = text.IndexOf(pattern, StringComparison.Ordinal);
        if (result != reference)
            throw new InvalidOperationException($"Наївний пошук не збігається з еталоном: {label}.");

        double naiveMs = MeasureMilliseconds(() => NaiveIndexOf(text, pattern, out _));
        double referenceMs = MeasureMilliseconds(() => text.IndexOf(pattern, StringComparison.Ordinal));
        Console.WriteLine($"{label}: індекс {result}; порівнянь {comparisons:N0}; власний {Format(naiveMs)} мс; string.IndexOf {Format(referenceMs)} мс");
    }

    private static int NaiveIndexOf(string text, string pattern, out int comparisons)
    {
        comparisons = 0;
        if (pattern.Length == 0)
            return 0;
        if (pattern.Length > text.Length)
            return -1;

        for (int start = 0; start <= text.Length - pattern.Length; start++)
        {
            int offset = 0;
            while (offset < pattern.Length)
            {
                comparisons++;
                if (text[start + offset] != pattern[offset])
                    break;
                offset++;
            }
            if (offset == pattern.Length)
                return start;
        }
        return -1;
    }

    private static void CheckEdge(string label, string text, string pattern, int expected)
    {
        int actual = NaiveIndexOf(text, pattern, out _);
        if (actual != expected)
            throw new InvalidOperationException($"Помилка крайового випадку «{label}»: {actual}, очікувалося {expected}.");
        Console.WriteLine($"Крайовий випадок «{label}»: {actual} — PASS");
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

    private static string Format(double value) => value.ToString("F6", CultureInfo.InvariantCulture);
}
