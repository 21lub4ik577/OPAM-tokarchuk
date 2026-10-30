using System;
using System.Collections.Generic;

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

Dictionary<string, List<Student>> byGroup =
    Algorithms.GroupBy(students, student => student.Group);
Dictionary<char, List<Student>> bySurnameInitial =
    Algorithms.GroupBy(students, student => student.Name[0]);
Dictionary<string, List<Student>> byScoreRange =
    Algorithms.GroupBy(students, student => GetScoreRange(student.Score));
Dictionary<int, List<string>> wordsByLength =
    Algorithms.GroupBy(words, word => word.Length);
Dictionary<string, List<Student>> emptyGroups =
    Algorithms.GroupBy(new List<Student>(), student => student.Group);
Dictionary<string, List<Student>> oneGroup =
    Algorithms.GroupBy(
        new List<Student>
        {
            new("Тестовий1", "КН-3/1", 76),
            new("Тестовий2", "КН-3/1", 80)
        },
        student => student.Group);
if (emptyGroups.Count != 0 || oneGroup.Count != 1 || oneGroup["КН-3/1"].Count != 2)
{
    throw new InvalidOperationException("Перевірка крайових випадків GroupBy не пройдена.");
}

Console.WriteLine("Завдання 4. GroupBy");
PrintCounts("Студенти за групою", byGroup);
PrintCounts("Студенти за першою літерою прізвища", bySurnameInitial);
PrintCounts("Студенти за діапазоном балу", byScoreRange);
PrintCounts("Рядки за довжиною", wordsByLength);

static string GetScoreRange(int score)
{
    return score switch
    {
        >= 0 and <= 59 => "0–59",
        >= 60 and <= 74 => "60–74",
        >= 75 and <= 89 => "75–89",
        >= 90 and <= 100 => "90–100",
        _ => throw new ArgumentOutOfRangeException(nameof(score), "Бал має бути в діапазоні 0–100.")
    };
}

static void PrintCounts<TKey, TValue>(
    string title,
    Dictionary<TKey, List<TValue>> groups)
    where TKey : notnull
{
    Console.WriteLine($"{title}:");
    foreach (KeyValuePair<TKey, List<TValue>> group in groups)
    {
        Console.WriteLine($"  {group.Key}: {group.Value.Count}");
    }
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
