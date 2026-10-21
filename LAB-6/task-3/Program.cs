const int numberCount = 25;
const int seed = 26;
const int maximumValue = 12;

Random random = new(seed);
List<int> numbers = new(numberCount);
for (int index = 0; index < numberCount; index++)
{
    numbers.Add(random.Next(1, maximumValue + 1));
}

HashSet<int> uniqueSet = RemoveDuplicatesWithSet(numbers);
List<int> orderPreserved = RemoveDuplicatesPreservingOrder(numbers);
List<int> listOnly = RemoveDuplicatesWithList(numbers);

Console.WriteLine("Завдання 3. Вилучення дублікатів");
Console.WriteLine($"Початковий список: {Format(numbers)}");
Console.WriteLine($"HashSet (порядок не гарантовано): {Format(uniqueSet)}");
Console.WriteLine($"HashSet зі збереженням порядку: {Format(orderPreserved)}");
Console.WriteLine($"Лише списком: {Format(listOnly)}");
Console.WriteLine($"Усі результати містять однакові значення: {SameValues(uniqueSet, orderPreserved, listOnly)}");

List<int> noDuplicates = new() { 1, 2, 3, 4 };
if (!SameValues(
        RemoveDuplicatesWithSet(noDuplicates),
        RemoveDuplicatesPreservingOrder(noDuplicates),
        RemoveDuplicatesWithList(noDuplicates)))
{
    throw new InvalidOperationException("Список без дублікатів має залишитися незмінним.");
}

static HashSet<int> RemoveDuplicatesWithSet(List<int> numbers)
{
    return new HashSet<int>(numbers);
}

static List<int> RemoveDuplicatesPreservingOrder(List<int> numbers)
{
    HashSet<int> seen = new();
    List<int> result = new();

    foreach (int number in numbers)
    {
        if (seen.Add(number))
        {
            result.Add(number);
        }
    }

    return result;
}

static List<int> RemoveDuplicatesWithList(List<int> numbers)
{
    List<int> result = new();

    foreach (int number in numbers)
    {
        if (!result.Contains(number))
        {
            result.Add(number);
        }
    }

    return result;
}

static bool SameValues(HashSet<int> expected, List<int> first, List<int> second)
{
    if (expected.Count != first.Count || expected.Count != second.Count)
    {
        return false;
    }

    foreach (int value in expected)
    {
        if (!first.Contains(value) || !second.Contains(value))
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
