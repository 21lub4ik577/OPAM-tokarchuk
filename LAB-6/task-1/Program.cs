const string text = """
Алгоритм описує послідовність дій. Алгоритм можна записати словами,
а можна записати кодом. Кодом алгоритм стає тоді, коли його
виконує машина, а не людина. Одна задача — багато алгоритмів.
""";
const int minimumFrequency = 3;

List<int> emptyLengths = GetWordLengths(string.Empty);
Dictionary<int, int> emptyDictionary = CountWithDictionary(emptyLengths);
List<int> emptyKeys = new();
List<int> emptyCounts = CountWithLists(emptyLengths, emptyKeys);
if (emptyDictionary.Count != 0 || emptyKeys.Count != 0 || emptyCounts.Count != 0)
{
    throw new InvalidOperationException("Порожній текст має давати порожній результат.");
}

List<int> wordLengths = GetWordLengths(text);
Dictionary<int, int> dictionaryCounts = CountWithDictionary(wordLengths);
List<int> listKeys = new();
List<int> listCounts = CountWithLists(wordLengths, listKeys);

Console.WriteLine("Завдання 1. Частоти довжин слів");
Console.WriteLine($"Порогова частота: {minimumFrequency}");
Console.WriteLine("Словник:");
PrintDictionaryResults(dictionaryCounts, minimumFrequency);
Console.WriteLine("Паралельні списки:");
PrintListResults(listKeys, listCounts, minimumFrequency);
Console.WriteLine($"Різних довжин: {dictionaryCounts.Count}");
Console.WriteLine($"Результати збігаються: {ResultsMatch(dictionaryCounts, listKeys, listCounts)}");

static List<int> GetWordLengths(string text)
{
    List<int> lengths = new();
    int currentLength = 0;

    foreach (char character in text)
    {
        if (char.IsLetter(character))
        {
            currentLength++;
        }
        else if (currentLength > 0)
        {
            lengths.Add(currentLength);
            currentLength = 0;
        }
    }

    if (currentLength > 0)
    {
        lengths.Add(currentLength);
    }

    return lengths;
}

static Dictionary<int, int> CountWithDictionary(List<int> values)
{
    Dictionary<int, int> counts = new();

    foreach (int value in values)
    {
        if (counts.TryGetValue(value, out int currentCount))
        {
            counts[value] = currentCount + 1;
        }
        else
        {
            counts.Add(value, 1);
        }
    }

    return counts;
}

static List<int> CountWithLists(List<int> values, List<int> keys)
{
    List<int> counts = new();

    foreach (int value in values)
    {
        int index = FindIndex(keys, value);
        if (index >= 0)
        {
            counts[index]++;
        }
        else
        {
            keys.Add(value);
            counts.Add(1);
        }
    }

    return counts;
}

static int FindIndex(List<int> values, int target)
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

static void PrintDictionaryResults(Dictionary<int, int> counts, int threshold)
{
    foreach (KeyValuePair<int, int> item in counts)
    {
        if (item.Value >= threshold)
        {
            Console.WriteLine($"{item.Key} літер: {item.Value} слів");
        }
    }
}

static void PrintListResults(List<int> keys, List<int> counts, int threshold)
{
    for (int index = 0; index < keys.Count; index++)
    {
        if (counts[index] >= threshold)
        {
            Console.WriteLine($"{keys[index]} літер: {counts[index]} слів");
        }
    }
}

static bool ResultsMatch(Dictionary<int, int> dictionary, List<int> keys, List<int> counts)
{
    if (dictionary.Count != keys.Count)
    {
        return false;
    }

    for (int index = 0; index < keys.Count; index++)
    {
        if (!dictionary.TryGetValue(keys[index], out int count) || count != counts[index])
        {
            return false;
        }
    }

    return true;
}
