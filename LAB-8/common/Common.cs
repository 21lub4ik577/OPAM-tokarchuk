using System;

public static class Common{
    public static Student[] students = GenerateStudents();

    static Student[] GenerateStudents()
    {
        Random random = new Random(26);
        Student[] result = new Student[100000];

        for (int i = 0; i < result.Length; i++)
        {
            result[i] = new Student(
                $"Student{i}",
                "ГРШ-3/1",
                random.Next(0, 101)
            );
        }

        return result;
    }

    public static int CompareStudents(Student first, Student second)
    {
        return first.Score.CompareTo(second.Score);
    }

    public const int X = 66;
    public const int Y = 76;
    public const int NOT_IN_RANGE = 101;
}

public sealed class Student
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
