using System.Diagnostics;
using System.Globalization;
using System.Text;

internal static class Program
{
    private const string Alphabet = "абвгґдеєжзиіїйклмнопрстуфхцчшщьюя";

    private static void Main()
    {
        string root = FindLabRoot();
        string prose = File.ReadAllText(Path.Combine(root, "data", "text.txt"));
        string[] words = prose.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
        var sizes = new[] { 1_000, 10_000, 50_000 };
        var naiveTimes = new double[sizes.Length];
        var fastTimes = new double[sizes.Length];

        for (int i = 0; i < sizes.Length; i++)
        {
            string input = prose.Substring(0, sizes[i]);
            string naive = NormalizeNaive(input);
            string fast = NormalizeFast(input);
            EnsureSame(naive, fast, $"контроль нормалізації на {sizes[i]} символах");
            naiveTimes[i] = MeasureMilliseconds(() => NormalizeNaive(input).Length);
            fastTimes[i] = MeasureMilliseconds(() => NormalizeFast(input).Length);
        }

        int naiveLines = CountMethodLines(Path.Combine(root, "task-2", "Program.cs"), "private static string NormalizeNaive(");
        int fastLines = CountMethodLines(Path.Combine(root, "task-2", "Program.cs"), "private static string NormalizeFast(");
        Console.WriteLine("Завдання 5. Вибір алгоритму для нормалізації");
        Console.WriteLine($"1. Дані: {prose.Length:N0} символів і {words.Length:N0} слів у data/text.txt; порівнювалася проза довжиною 1 000–50 000 символів.");
        Console.WriteLine("2. Потрібно нормалізувати один вхідний текст за один запит; у вимірюваннях кожен розмір подано окремо.");
        Console.WriteLine("3. Файл незмінний між запитами; повторні прогони — лише вимірювання, а не потік нових даних.");
        Console.WriteLine($"4. Для заданого файлу важливі час і пам'ять: наївний метод займає {naiveLines} рядків, швидкий — {fastLines}; на 50 000 символах отримано {Format(naiveTimes[2])} мс проти {Format(fastTimes[2])} мс.");
        Console.WriteLine("5. StringBuilder і методи рядків є у стандартній бібліотеці, але готового методу для потрібної нормалізації немає.");
        Console.WriteLine($"6. На 1 000 / 10 000 / 50 000 символах наївно: {Format(naiveTimes[0])} / {Format(naiveTimes[1])} / {Format(naiveTimes[2])} мс; швидко: {Format(fastTimes[0])} / {Format(fastTimes[1])} / {Format(fastTimes[2])} мс. Для 50 000 символів обираю швидкий варіант.");

        Console.WriteLine();
        Console.WriteLine("Додаткове завдання, таблиця C (c = 1): масив із 33 частот проти Dictionary<char, int>");
        VerifyFrequencies(prose);
        double arrayMs = MeasureMilliseconds(() => CountArray(prose));
        double dictionaryMs = MeasureMilliseconds(() => CountDictionary(prose));
        Console.WriteLine($"Масив: {Format(arrayMs)} мс; Dictionary: {Format(dictionaryMs)} мс; масив швидший у {Format(dictionaryMs / Math.Max(arrayMs, 0.000001))}x.");
        Console.WriteLine($"Небезпека прямого індексу ch - 'а': для ґ індекс дорівнює {(int)'ґ' - (int)'а'} (поза межами 0..32); великі літери й пунктуація також потребують мапінгу або фільтрації.");
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

    private static long CountArray(string text)
    {
        var counts = new int[Alphabet.Length];
        foreach (char character in text)
        {
            int index = Alphabet.IndexOf(char.ToLowerInvariant(character));
            if (index >= 0)
                counts[index]++;
        }
        long total = 0;
        foreach (int count in counts)
            total += count;
        return total;
    }

    private static long CountDictionary(string text)
    {
        var counts = new Dictionary<char, int>(Alphabet.Length);
        foreach (char character in text)
        {
            char letter = char.ToLowerInvariant(character);
            if (Alphabet.IndexOf(letter) < 0)
                continue;
            if (counts.TryGetValue(letter, out int count))
                counts[letter] = count + 1;
            else
                counts.Add(letter, 1);
        }
        long total = 0;
        foreach (KeyValuePair<char, int> pair in counts)
            total += pair.Value;
        return total;
    }

    private static void VerifyFrequencies(string text)
    {
        var arrayCounts = new int[Alphabet.Length];
        var dictionaryCounts = new Dictionary<char, int>(Alphabet.Length);
        foreach (char character in text)
        {
            char letter = char.ToLowerInvariant(character);
            int index = Alphabet.IndexOf(letter);
            if (index < 0)
                continue;
            arrayCounts[index]++;
            if (dictionaryCounts.TryGetValue(letter, out int count))
                dictionaryCounts[letter] = count + 1;
            else
                dictionaryCounts.Add(letter, 1);
        }
        for (int i = 0; i < Alphabet.Length; i++)
        {
            if (!dictionaryCounts.TryGetValue(Alphabet[i], out int count) || count != arrayCounts[i])
                throw new InvalidOperationException("Частоти масиву й словника не збігаються.");
        }
    }

    private static int CountMethodLines(string path, string signature)
    {
        string[] lines = File.ReadAllLines(path);
        int start = Array.FindIndex(lines, line => line.Contains(signature, StringComparison.Ordinal));
        if (start < 0)
            throw new InvalidOperationException($"Не знайдено метод {signature} у {path}.");

        int depth = 0;
        int count = 0;
        bool opened = false;
        for (int i = start; i < lines.Length; i++)
        {
            string line = lines[i];
            if (line.Trim().Length > 0)
                count++;
            foreach (char character in line)
            {
                if (character == '{')
                {
                    depth++;
                    opened = true;
                }
                else if (character == '}')
                {
                    depth--;
                }
            }
            if (opened && depth == 0)
                return count;
        }
        throw new InvalidOperationException($"Не вдалося порахувати рядки методу {signature}.");
    }

    private static void EnsureSame(string left, string right, string label)
    {
        if (!string.Equals(left, right, StringComparison.Ordinal))
            throw new InvalidOperationException($"Результати не збігаються: {label}.");
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

    private static readonly char[] WhitespaceCharacters =
    {
        '\t', '\n', '\v', '\f', '\r', '\u0085', '\u00a0', '\u1680',
        '\u2000', '\u2001', '\u2002', '\u2003', '\u2004', '\u2005',
        '\u2006', '\u2007', '\u2008', '\u2009', '\u200a', '\u2028',
        '\u2029', '\u202f', '\u205f', '\u3000'
    };

    private static string FindLabRoot()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (Directory.Exists(Path.Combine(directory.FullName, "data")))
                return directory.FullName;
            directory = directory.Parent;
        }
        throw new DirectoryNotFoundException("Не знайдено папку LAB-10/data.");
    }

    private static string Format(double value) => value.ToString("F3", CultureInfo.InvariantCulture);
}
