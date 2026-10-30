using System;
using System.Collections.Generic;
using System.Numerics;

const int n = 26;
const int k = 16;

Random random = new(n);
List<int> numbers = new();
for (int index = 0; index < 20; index++)
{
    numbers.Add(random.Next(-k, k + 1));
}

int sum = Algorithms.Aggregate(numbers, 0, (state, number) => state + number);
BigInteger product = Algorithms.Aggregate(
    numbers,
    BigInteger.One,
    (state, number) => state * number);
int? maximum = Algorithms.Aggregate<int, int?>(
    numbers,
    null,
    (state, number) => state is null || number > state.Value ? number : state);
int? largestByAbsoluteValue = Algorithms.Aggregate<int, int?>(
    numbers,
    null,
    (state, number) =>
        state is null || Math.Abs(number) > Math.Abs(state.Value) ? number : state);

List<int> emptyNumbers = new();
int emptySum = Algorithms.Aggregate(emptyNumbers, 0, (state, number) => state + number);
BigInteger emptyProduct = Algorithms.Aggregate(
    emptyNumbers,
    BigInteger.One,
    (state, number) => state * number);
int? emptyMaximum = Algorithms.Aggregate<int, int?>(emptyNumbers, null, (state, number) => number);
int? emptyLargestByAbsoluteValue =
    Algorithms.Aggregate<int, int?>(emptyNumbers, null, (state, number) => number);
int? singleNumberMaximum =
    Algorithms.Aggregate<int, int?>(new List<int> { 7 }, null, (state, number) => number);
if (emptySum != 0 ||
    emptyProduct != BigInteger.One ||
    emptyMaximum is not null ||
    emptyLargestByAbsoluteValue is not null ||
    singleNumberMaximum != 7)
{
    throw new InvalidOperationException("Перевірка крайових випадків Aggregate не пройдена.");
}

Console.WriteLine("Завдання 3. Aggregate");
Console.WriteLine($"Числа: {string.Join(", ", numbers)}");
Console.WriteLine($"Сума: {sum}");
Console.WriteLine($"Добуток: {product}");
Console.WriteLine($"Максимум: {maximum?.ToString() ?? "немає елементів"}");
Console.WriteLine(
    $"Найбільший за модулем: {largestByAbsoluteValue?.ToString() ?? "немає елементів"}");
Console.WriteLine("Для порожнього списку максимум і найбільший за модулем — null.");
