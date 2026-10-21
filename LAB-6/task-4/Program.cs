const int numberCount = 30;
const int seed = 26;
const int secondSeed = 126;
const int maximumValue = 36;

Random firstRandom = new(seed);
Random secondRandom = new(secondSeed);
List<int> firstValues = GenerateValues(firstRandom, numberCount, maximumValue);
List<int> secondValues = GenerateValues(secondRandom, numberCount, maximumValue);
HashSet<int> firstSet = new(firstValues);
HashSet<int> secondSet = new(secondValues);

HashSet<int> setDifference = DifferenceWithSets(firstSet, secondSet);
List<int> listDifference = DifferenceWithNestedLoops(firstValues, secondValues);
bool resultsMatch = SameValues(setDifference, listDifference);

Console.WriteLine("Завдання 4. Елементи першого набору, яких немає в другому");
Console.WriteLine($"Перший список: {Format(firstValues)}");
Console.WriteLine($"Другий список: {Format(secondValues)}");
Console.WriteLine($"Перший набір: {Format(firstSet)}");
Console.WriteLine($"Другий набір: {Format(secondSet)}");
Console.WriteLine($"Різниця через HashSet: {Format(setDifference)}");
Console.WriteLine($"Різниця вкладеними циклами: {Format(listDifference)}");
Console.WriteLine($"Результати збігаються: {resultsMatch}");

if (!resultsMatch)
{
    throw new InvalidOperationException("Реалізації різниці множин повернули різні результати.");
}

VerifyEdgeCases();

static void VerifyEdgeCases()
{
    List<int> first = new() { 1, 2, 3 };
    List<int> disjoint = new() { 4, 5, 6 };
    List<int> identical = new() { 1, 2, 3 };
    HashSet<int> firstSet = new(first);
    HashSet<int> disjointDifference = DifferenceWithSets(firstSet, new HashSet<int>(disjoint));
    List<int> disjointListDifference = DifferenceWithNestedLoops(first, disjoint);
    HashSet<int> identicalDifference = DifferenceWithSets(firstSet, new HashSet<int>(identical));
    List<int> identicalListDifference = DifferenceWithNestedLoops(first, identical);

    if (disjointDifference.Count != firstSet.Count ||
        !SameValues(disjointDifference, first) ||
        !SameValues(disjointDifference, disjointListDifference) ||
        identicalDifference.Count != 0 ||
        identicalListDifference.Count != 0)
    {
        throw new InvalidOperationException("Перевірку крайових випадків різниці множин не пройдено.");
    }
}

static List<int> GenerateValues(Random random, int count, int maximumValue)
{
    List<int> values = new(count);
    for (int index = 0; index < count; index++)
    {
        values.Add(random.Next(1, maximumValue + 1));
    }

    return values;
}

static HashSet<int> DifferenceWithSets(HashSet<int> first, HashSet<int> second)
{
    HashSet<int> result = new();

    foreach (int value in first)
    {
        if (!second.Contains(value))
        {
            result.Add(value);
        }
    }

    return result;
}

static List<int> DifferenceWithNestedLoops(List<int> first, List<int> second)
{
    List<int> result = new();

    foreach (int candidate in first)
    {
        if (result.Contains(candidate))
        {
            continue;
        }

        bool foundInSecond = false;
        foreach (int value in second)
        {
            if (candidate == value)
            {
                foundInSecond = true;
                break;
            }
        }

        if (!foundInSecond)
        {
            result.Add(candidate);
        }
    }

    return result;
}

static bool SameValues(HashSet<int> expected, List<int> actual)
{
    if (expected.Count != actual.Count)
    {
        return false;
    }

    foreach (int value in expected)
    {
        if (!actual.Contains(value))
        {
            return false;
        }
    }

    return true;
}

static string Format(IEnumerable<int> values)
{
    return string.Join(", ", values);
}
