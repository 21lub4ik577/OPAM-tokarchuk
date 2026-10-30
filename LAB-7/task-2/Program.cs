using System;
using System.Collections.Generic;

const int n = 26;
const int k = 16;

Random random = new(n);
List<int> numbers = new();
for (int index = 0; index < 20; index++)
{
    numbers.Add(random.Next(-k, k + 1));
}

List<string> words = new()
{
    "сонце", "будь-що", "алгоритм", "C#", "узагальнений",
    "код", "пів-острів", "делегат", "тип", "рядок",
    "високо-рівневий", "цикл", "програмування", "так", "дані", "об'єкт"
};

List<Student> students = new()
{
    new("Коваленко", "ГРШ-3/1", 88),
    new("Мельник", "ГРШ-3/2", 64),
    new("Іщенко", "ГРШ-3/1", 93),
    new("Бондар", "ГРШ-3/1", 71),
    new("Савченко", "ГРШ-3/2", 55),
    new("Ткачук", "ГРШ-3/2", 90),
    new("Кравець", "ГРШ-3/1", 76),
    new("Іванів", "ГРШ-3/1", 48),
    new("Мороз", "ГРШ-3/2", 82),
    new("Шевчук", "ГРШ-3/1", 67),
    new("Бойко", "ГРШ-3/2", 95),
    new("Кушнір", "ГРШ-3/2", 73)
};

List<int> signs = Algorithms.Map(numbers, number => number < 0 ? -1 : number > 0 ? 1 : 0);
List<char> firstLetters = Algorithms.Map(words, word => word[0]);
List<int> scores = Algorithms.Map(students, student => student.Score);

if (Algorithms.Map(new List<int>(), number => number.ToString()).Count != 0)
{
    throw new InvalidOperationException("Map порожнього списку має повернути порожній список.");
}

Console.WriteLine("Завдання 2. Map");
Console.WriteLine($"Число -> знак: {string.Join(", ", signs)}");
Console.WriteLine($"Рядок -> перша літера: {string.Join(", ", firstLetters)}");
Console.WriteLine($"Студент -> бал: {string.Join(", ", scores)}");

sealed class Student
{
    public string Name;
    public string Group;
    public int Score;

    public Student(string name, string group, int score)
    {
        Name = name;
        Group = group;
        Score = score;
    }
}
