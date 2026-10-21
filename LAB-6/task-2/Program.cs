
Student[] students =
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

Dictionary<string, List<Student>> dictionaryGroups = GroupWithDictionary(students);
List<string> listKeys = new();
List<List<Student>> listGroups = GroupWithLists(students, listKeys);

Console.WriteLine("Завдання 2. Групування за діапазоном балу");
Console.WriteLine("Словник:");
PrintGroups(dictionaryGroups);
Console.WriteLine("Паралельні списки:");
PrintListGroups(listKeys, listGroups);
Console.WriteLine($"Результати збігаються: {ResultsMatch(dictionaryGroups, listKeys, listGroups)}");

Student[] oneGroupStudents =
{
    new("Тестовий1", "ГРШ-3/1", 76),
    new("Тестовий2", "ГРШ-3/2", 80)
};

Dictionary<string, List<Student>> oneGroupDictionary = GroupWithDictionary(oneGroupStudents);
List<string> oneGroupKeys = new();
List<List<Student>> oneGroupLists = GroupWithLists(oneGroupStudents, oneGroupKeys);

if (oneGroupDictionary.Count != 1 ||
    oneGroupKeys.Count != 1 ||
    !ResultsMatch(oneGroupDictionary, oneGroupKeys, oneGroupLists))
{
    throw new InvalidOperationException("Студенти з одного діапазону мають утворювати одну групу.");
}

static Dictionary<string, List<Student>> GroupWithDictionary(Student[] students)
{
    Dictionary<string, List<Student>> groups = new();

    foreach (Student student in students)
    {
        string key = GetScoreRange(student.Score);

        if (!groups.TryGetValue(key, out List<Student>? group))
        {
            group = new List<Student>();
            groups.Add(key, group);
        }

        group.Add(student);
    }

    return groups;
}

static List<List<Student>> GroupWithLists(Student[] students, List<string> keys)
{
    List<List<Student>> groups = new();

    foreach (Student student in students)
    {
        string key = GetScoreRange(student.Score);
        int index = FindIndex(keys, key);

        if (index < 0)
        {
            keys.Add(key);
            groups.Add(new List<Student>());
            index = groups.Count - 1;
        }

        groups[index].Add(student);
    }

    return groups;
}

static string GetScoreRange(int score)
{
    return score switch
    {
        >= 0 and <= 59 => "0–59",
        >= 60 and <= 74 => "60–74",
        >= 75 and <= 89 => "75–89",
        >= 90 and <= 100 => "90–100",
        _ => throw new ArgumentOutOfRangeException(
            nameof(score),
            "Бал має бути в діапазоні 0–100.")
    };
}

static int FindIndex(List<string> keys, string target)
{
    for (int index = 0; index < keys.Count; index++)
    {
        if (keys[index] == target)
        {
            return index;
        }
    }

    return -1;
}

static void PrintGroups(Dictionary<string, List<Student>> groups)
{
    foreach (KeyValuePair<string, List<Student>> item in groups)
    {
        PrintGroup(item.Key, item.Value);
    }
}

static void PrintListGroups(List<string> keys, List<List<Student>> groups)
{
    for (int index = 0; index < keys.Count; index++)
    {
        PrintGroup(keys[index], groups[index]);
    }
}

static void PrintGroup(string key, List<Student> students)
{
    int totalScore = 0;

    foreach (Student student in students)
    {
        totalScore += student.Score;
    }

    Console.WriteLine(
        $"{key}: студентів {students.Count}, середній бал {(double)totalScore / students.Count:F2}");
}

static bool ResultsMatch(
    Dictionary<string, List<Student>> dictionary,
    List<string> keys,
    List<List<Student>> groups)
{
    if (dictionary.Count != keys.Count)
    {
        return false;
    }

    for (int index = 0; index < keys.Count; index++)
    {
        if (!dictionary.TryGetValue(keys[index], out List<Student>? group) ||
            group.Count != groups[index].Count)
        {
            return false;
        }

        for (int studentIndex = 0; studentIndex < group.Count; studentIndex++)
        {
            if (group[studentIndex] != groups[index][studentIndex])
            {
                return false;
            }
        }
    }

    return true;
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
