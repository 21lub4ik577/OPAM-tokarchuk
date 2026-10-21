using System.Diagnostics;

const int lookupDataSize = 100_000;
const int lookupCount = 10_000;
const int wordCount = 50_000;
const int uniqueWordCount = 200;
const int duplicateDataSize = 20_000;
const int duplicateRange = 5_000;
const int setSize = 5_000;
const int setRange = 10_000;

int[] lookupValues = new int[lookupDataSize];
for (int index = 0; index < lookupValues.Length; index++)
{
    lookupValues[index] = index;
}

int[] lookups = new int[lookupCount];
for (int index = 0; index < lookups.Length; index++)
{
    lookups[index] = lookupDataSize - 1 - index;
}

HashSet<int> lookupSet = new(lookupValues);
string[] words = new string[wordCount];
for (int index = 0; index < words.Length; index++)
{
    words[index] = "word" + (index % uniqueWordCount);
}

int[] duplicateValues = GenerateNumbers(duplicateDataSize, duplicateRange, 2601);
int[] firstSetValues = GenerateNumbers(setSize, setRange, 2602);
int[] secondSetValues = GenerateNumbers(setSize, setRange, 2603);
HashSet<int> firstSet = new(firstSetValues);
HashSet<int> secondSet = new(secondSetValues);

VerifyWordFrequencies(words);
RunAndMeasure(
    "10 000 перевірок наявності (100 000 елементів)",
    () => CountListLookups(lookupValues, lookups),
    () => CountSetLookups(lookupSet, lookups));
RunAndMeasure(
    "Частоти 50 000 слів",
    () => CountWordFrequenciesWithLists(words),
    () => CountWordFrequenciesWithDictionary(words));
RunAndMeasure(
    "Унікальні значення з 20 000 чисел",
    () => RemoveDuplicatesWithList(duplicateValues),
    () => RemoveDuplicatesWithSet(duplicateValues));
RunAndMeasure(
    "Спільні елементи двох наборів по 5 000",
    () => FindCommonWithLists(firstSetValues, secondSetValues),
    () => FindCommonWithSets(firstSet, secondSet));
MeasureSmallSetCrossover();

Console.WriteLine();
Console.WriteLine("Усі виміри: Release, кожен варіант прогрітий одним запуском.");

static void RunAndMeasure(string title, Func<int> listOperation, Func<int> setOperation)
{
    int listResult = listOperation();
    int setResult = setOperation();
    if (listResult != setResult)
    {
        throw new InvalidOperationException($"Результати виміру не збігаються: {title}.");
    }

    Stopwatch stopwatch = Stopwatch.StartNew();
    listResult = listOperation();
    stopwatch.Stop();
    double listMilliseconds = stopwatch.Elapsed.TotalMilliseconds;

    stopwatch.Restart();
    setResult = setOperation();
    stopwatch.Stop();
    double setMilliseconds = stopwatch.Elapsed.TotalMilliseconds;

    if (listResult != setResult)
    {
        throw new InvalidOperationException($"Результати виміру не збігаються після заміру: {title}.");
    }

    double ratio = setMilliseconds == 0 ? double.PositiveInfinity : listMilliseconds / setMilliseconds;
    Console.WriteLine($"{title}: список {listMilliseconds:F3} мс; словник/множина {setMilliseconds:F3} мс; прискорення {ratio:F2}x");
}

static int CountListLookups(int[] values, int[] lookups)
{
    int foundCount = 0;
    foreach (int target in lookups)
    {
        if (Contains(values, target))
        {
            foundCount++;
        }
    }

    return foundCount;
}

static int CountSetLookups(HashSet<int> values, int[] lookups)
{
    int foundCount = 0;
    foreach (int target in lookups)
    {
        if (values.Contains(target))
        {
            foundCount++;
        }
    }

    return foundCount;
}

static bool Contains(int[] values, int target)
{
    foreach (int value in values)
    {
        if (value == target)
        {
            return true;
        }
    }

    return false;
}

static int CountWordFrequenciesWithLists(string[] words)
{
    List<string> uniqueWords = new();
    List<int> counts = new();

    foreach (string word in words)
    {
        int index = FindIndex(uniqueWords, word);
        if (index >= 0)
        {
            counts[index]++;
        }
        else
        {
            uniqueWords.Add(word);
            counts.Add(1);
        }
    }

    int total = 0;
    foreach (int count in counts)
    {
        total += count;
    }

    return total;
}

static void VerifyWordFrequencies(string[] words)
{
    List<string> keys = new();
    List<int> listCounts = new();
    Dictionary<string, int> dictionaryCounts = new();

    foreach (string word in words)
    {
        int index = FindIndex(keys, word);
        if (index >= 0)
        {
            listCounts[index]++;
        }
        else
        {
            keys.Add(word);
            listCounts.Add(1);
        }

        if (dictionaryCounts.TryGetValue(word, out int count))
        {
            dictionaryCounts[word] = count + 1;
        }
        else
        {
            dictionaryCounts.Add(word, 1);
        }
    }

    if (keys.Count != dictionaryCounts.Count)
    {
        throw new InvalidOperationException("Список і словник мають різну кількість унікальних слів.");
    }

    for (int index = 0; index < keys.Count; index++)
    {
        if (!dictionaryCounts.TryGetValue(keys[index], out int count) || count != listCounts[index])
        {
            throw new InvalidOperationException($"Частоти слова «{keys[index]}» не збігаються.");
        }
    }
}

static int CountWordFrequenciesWithDictionary(string[] words)
{
    Dictionary<string, int> counts = new();

    foreach (string word in words)
    {
        if (counts.TryGetValue(word, out int count))
        {
            counts[word] = count + 1;
        }
        else
        {
            counts.Add(word, 1);
        }
    }

    int total = 0;
    foreach (KeyValuePair<string, int> item in counts)
    {
        total += item.Value;
    }

    return total;
}

static int FindIndex(List<string> values, string target)
{
    for (int index = 0; index < values.Count; index++)
    {
        if (values[index] == target)
        {
            return index;
        }
    }

    return -1;
}

static int RemoveDuplicatesWithList(int[] values)
{
    List<int> uniqueValues = new();
    foreach (int value in values)
    {
        if (!uniqueValues.Contains(value))
        {
            uniqueValues.Add(value);
        }
    }

    return uniqueValues.Count;
}

static int RemoveDuplicatesWithSet(int[] values)
{
    return new HashSet<int>(values).Count;
}

static int FindCommonWithLists(int[] first, int[] second)
{
    List<int> common = new();
    foreach (int candidate in first)
    {
        if (common.Contains(candidate))
        {
            continue;
        }

        if (Contains(second, candidate))
        {
            common.Add(candidate);
        }
    }

    return common.Count;
}

static int FindCommonWithSets(HashSet<int> first, HashSet<int> second)
{
    int commonCount = 0;
    foreach (int value in first)
    {
        if (second.Contains(value))
        {
            commonCount++;
        }
    }

    return commonCount;
}

static int[] GenerateNumbers(int count, int exclusiveUpperBound, int seed)
{
    Random random = new(seed);
    int[] values = new int[count];
    for (int index = 0; index < values.Length; index++)
    {
        values[index] = random.Next(exclusiveUpperBound);
    }

    return values;
}

static void MeasureSmallSetCrossover()
{
    Console.WriteLine();
    Console.WriteLine("Додаткова проба пошуку для оцінки малих наборів:");

    foreach (int size in new[] { 8, 32, 128, 512 })
    {
        int[] values = new int[size];
        for (int index = 0; index < values.Length; index++)
        {
            values[index] = index;
        }

        int[] lookups = new int[10_000];
        for (int index = 0; index < lookups.Length; index++)
        {
            lookups[index] = size - 1 - index % size;
        }

        HashSet<int> set = new(values);
        RunAndMeasure(
            $"Малий набір {size} елементів, 10 000 перевірок",
            () => CountListLookups(values, lookups),
            () => CountSetLookups(set, lookups));
    }
}
