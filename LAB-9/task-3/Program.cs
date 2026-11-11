using System.Globalization;
using System.Linq;

static int CompareInts(int first, int second) => first.CompareTo(second);

var sorts = new (string Name, SortWithCounts<int> Sort)[]
{
    ("Бульбашка", BubbleSort.Sort),
    ("QuickSort", QuickSort.Sort),
    ("Бінарні вставки", BinaryInsertionSort.Sort)
};

Console.WriteLine("Параметри варіанта: N=26, K=16; числа 0..320; Random(26).");
PrintTrace();
PrintCountTables(sorts);
PrintInsertionComparison();
PrintAllEqualQuickSort();
PrintStability();
PrintEdgeCases(sorts);
Benchmark.PrintTimingTable();

void PrintTrace()
{
    int[] values = LabData.CreateValues(7);
    Console.WriteLine($"TRACE|input|{string.Join(",", values)}");
    for (int pass = 1; pass < values.Length; pass++)
    {
        bool swapped = false;
        for (int i = 0; i < values.Length - pass; i++)
        {
            if (CompareInts(values[i], values[i + 1]) <= 0)
            {
                continue;
            }

            (values[i], values[i + 1]) = (values[i + 1], values[i]);
            swapped = true;
        }

        Console.WriteLine($"TRACE|pass-{pass}|{string.Join(",", values)}");
        if (!swapped)
        {
            break;
        }
    }
}

void PrintCountTables((string Name, SortWithCounts<int> Sort)[] algorithms)
{
    foreach (int length in new[] { 100, 1_000, 10_000 })
    {
        foreach ((string inputName, int[] input) in LabData.CreateInputs(length))
        {
            foreach ((string algorithmName, SortWithCounts<int> sort) in algorithms)
            {
                int[] values = (int[])input.Clone();
                sort(values, CompareInts, out int comparisons, out int moves);
                bool sorted = Validation.IsSorted(values, CompareInts);
                bool permutation = Validation.IsPermutationOf(input, values);
                if (!sorted || !permutation)
                {
                    throw new InvalidOperationException($"{algorithmName} не пройшов перевірку {inputName}, n={length}.");
                }

                Console.WriteLine($"COUNT|{length}|{inputName}|{algorithmName}|{comparisons}|{moves}|OK");
            }
        }
    }
}

void PrintAllEqualQuickSort()
{
    int[] values = new int[10_000];
    QuickSort.Sort(values, CompareInts, out int comparisons, out int moves);
    Console.WriteLine($"EQUAL|QuickSort|10000|{comparisons}|{moves}|"
        + $"{Validation.IsSorted(values, CompareInts)}|{Validation.IsPermutationOf(new int[10_000], values)}");
}

void PrintInsertionComparison()
{
    int[] input = LabData.CreateValues(10_000);
    int[] linear = (int[])input.Clone();
    int[] binary = (int[])input.Clone();
    InsertionReference.Sort(linear, CompareInts, out int linearComparisons, out int linearMoves);
    BinaryInsertionSort.Sort(binary, CompareInts, out int binaryComparisons, out int binaryMoves);

    if (!Validation.IsSorted(linear, CompareInts)
        || !Validation.IsPermutationOf(input, linear)
        || !Validation.IsSorted(binary, CompareInts)
        || !Validation.IsPermutationOf(input, binary)
        || linearMoves != binaryMoves)
    {
        throw new InvalidOperationException("Порівняння звичайних і бінарних вставок не пройшло перевірку.");
    }

    double linearMilliseconds = MeasureInsertion(input, false);
    double binaryMilliseconds = MeasureInsertion(input, true);
    Console.WriteLine($"INSERTION-REF|10000|звичайні|{linearComparisons}|{linearMoves}|{linearMilliseconds:F3} ms");
    Console.WriteLine($"INSERTION-REF|10000|бінарні|{binaryComparisons}|{binaryMoves}|{binaryMilliseconds:F3} ms");
}

double MeasureInsertion(int[] input, bool binary)
{
    void Sort(int[] values)
    {
        if (binary)
        {
            BinaryInsertionSort.Sort(values, CompareInts, out _, out _);
        }
        else
        {
            InsertionReference.Sort(values, CompareInts, out _, out _);
        }
    }

    Sort((int[])input.Clone());
    double best = double.MaxValue;
    for (int trial = 0; trial < 3; trial++)
    {
        int[] values = (int[])input.Clone();
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        Sort(values);
        stopwatch.Stop();
        best = Math.Min(best, stopwatch.Elapsed.TotalMilliseconds);
    }

    return best;
}

void PrintStability()
{
    Student[] students = CreateStudents();
    var comparer = Comparer<Student>.Create(CompareStudents);
    var stableSorts = new (string Name, Func<Student[], Student[]> Sort, bool Expected)[]
    {
        ("Бульбашка", SortWithBubble, true),
        ("QuickSort", SortWithQuickSort, false),
        ("Бінарні вставки", SortWithBinaryInsertion, true),
        ("Array.Sort", SortWithArraySort, false),
        ("OrderBy", SortWithOrderBy, true)
    };

    foreach ((string name, Func<Student[], Student[]> sort, bool expected) in stableSorts)
    {
        Student[] result = sort(students);
        if (!Validation.IsSorted(result, comparer.Compare)
            || !Validation.IsPermutationOf(students, result))
        {
            throw new InvalidOperationException($"{name} не пройшов перевірку стабільності.");
        }

        string counterexample = FirstStabilityCounterexample(result);
        bool actual = counterexample.Length == 0;
        Console.WriteLine($"STABILITY|{name}|{(expected ? "стабільне" : "нестабільне")}|"
            + $"{(actual ? "стабільне" : "нестабільне")}|{(actual ? "—" : counterexample)}");
    }
}

void PrintEdgeCases((string Name, SortWithCounts<int> Sort)[] algorithms)
{
    var cases = new (string Name, int[] Values)[]
    {
        ("порожній", Array.Empty<int>()),
        ("один елемент", new[] { 5 }),
        ("два за порядком", new[] { 1, 2 }),
        ("два навпаки", new[] { 2, 1 }),
        ("усі однакові", new[] { 4, 4, 4, 4, 4, 4 }),
        ("два значення порівну", new[] { 1, 2, 1, 2, 1, 2, 1, 2 })
    };

    foreach ((string caseName, int[] input) in cases)
    {
        foreach ((string algorithmName, SortWithCounts<int> sort) in algorithms)
        {
            int[] result = (int[])input.Clone();
            sort(result, CompareInts, out _, out _);
            bool okay = Validation.IsSorted(result, CompareInts)
                && Validation.IsPermutationOf(input, result);
            Console.WriteLine($"EDGE|{caseName}|{algorithmName}|{(okay ? "OK" : "FAIL")}");
            if (!okay)
            {
                throw new InvalidOperationException($"{algorithmName}: крайовий випадок «{caseName}».");
            }
        }
    }
}

Student[] CreateStudents()
{
    var students = new Student[64];
    for (int i = 0; i < students.Length; i++)
    {
        students[i] = new Student($"Прізвище{i:00}", $"КН-{i % 3 + 1}", (i % 4) switch
        {
            0 or 2 => 70,
            1 => 82,
            _ => 91
        });
    }

    return students;
}

int CompareStudents(Student first, Student second) => first.Score.CompareTo(second.Score);

Student[] SortWithBubble(Student[] input)
{
    Student[] result = (Student[])input.Clone();
    BubbleSort.Sort(result, CompareStudents, out _, out _);
    return result;
}

Student[] SortWithQuickSort(Student[] input)
{
    Student[] result = (Student[])input.Clone();
    QuickSort.Sort(result, CompareStudents, out _, out _);
    return result;
}

Student[] SortWithBinaryInsertion(Student[] input)
{
    Student[] result = (Student[])input.Clone();
    BinaryInsertionSort.Sort(result, CompareStudents, out _, out _);
    return result;
}

Student[] SortWithArraySort(Student[] input)
{
    Student[] result = (Student[])input.Clone();
    Array.Sort(result, Comparer<Student>.Create(CompareStudents));
    return result;
}

Student[] SortWithOrderBy(Student[] input) =>
    Enumerable.OrderBy(input, student => student.Score).ToArray();

string FirstStabilityCounterexample(Student[] values)
{
    for (int i = 1; i < values.Length; i++)
    {
        if (values[i - 1].Score == values[i].Score
            && string.CompareOrdinal(values[i - 1].Name, values[i].Name) > 0)
        {
            return $"{values[i - 1].Score}: {values[i - 1].Name} перед {values[i].Name}";
        }
    }

    return string.Empty;
}

sealed record Student(string Name, string Group, int Score);
