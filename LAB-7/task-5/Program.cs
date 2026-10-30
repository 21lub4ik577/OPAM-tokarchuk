using System;
using System.Collections.Generic;

const int k = 16;

int threshold = k;
Func<int, bool> greaterThanThreshold = number => number > threshold;
threshold = 1000;
Console.WriteLine("Завдання 5. Захоплення змінних");
Console.WriteLine($"Поріг після зміни: {threshold}; перевірка числа 20: {greaterThanThreshold(20)}");

List<Func<int>> forLambdas = new();
for (int index = 0; index < 3; index++)
{
    forLambdas.Add(() => index);
}

Console.WriteLine($"Лямбди у for: {string.Join(", ", InvokeAll(forLambdas))}");

int[] values = { 0, 1, 2 };
List<Func<int>> foreachLambdas = new();
foreach (int value in values)
{
    foreachLambdas.Add(() => value);
}

Console.WriteLine($"Лямбди у foreach: {string.Join(", ", InvokeAll(foreachLambdas))}");

static List<int> InvokeAll(List<Func<int>> functions)
{
    List<int> results = new();
    foreach (Func<int> function in functions)
    {
        results.Add(function());
    }

    return results;
}
