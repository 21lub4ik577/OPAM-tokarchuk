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

List<int> selectedNumbers = Algorithms.Filter(numbers, number => number <= 0);
List<string> selectedWords = Algorithms.Filter(words, word => word.Contains('-'));
List<Student> selectedStudents =
    Algorithms.Filter(students, student => student.Score >= 60 && student.Score <= 89);

if (Algorithms.Filter(new List<int>(), number => number <= 0).Count != 0 ||
    Algorithms.Filter(numbers, _ => false).Count != 0)
{
    throw new InvalidOperationException("Перевірка крайових випадків Filter не пройдена.");
}

Console.WriteLine("Завдання 1. Filter");
Console.WriteLine($"Числа (від'ємні або нуль), {selectedNumbers.Count}: {string.Join(", ", selectedNumbers)}");
Console.WriteLine($"Рядки (містять дефіс), {selectedWords.Count}: {string.Join(", ", selectedWords)}");
Console.WriteLine($"Студенти (бал [60; 89]), {selectedStudents.Count}:");
foreach (Student student in selectedStudents)
{
    Console.WriteLine($"  {student.Name}, {student.Group}, {student.Score}");
}

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