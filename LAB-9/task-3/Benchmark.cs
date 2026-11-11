using System.Diagnostics;

public static class Benchmark
{
    private const double LimitMilliseconds = 1000;

    private delegate void SortAction(int[] values);
    private delegate bool CancellableSort(int[] values, Func<bool> shouldStop);

    public static void PrintTimingTable()
    {
#if DEBUG
        throw new InvalidOperationException("Вимірювання необхідно запускати в конфігурації Release.");
#endif
        var algorithms = new (string Name, SortAction Sort, CancellableSort? TrySort)[]
        {
            ("Бульбашка", BubbleSortAction, BubbleSortTimed),
            ("QuickSort", QuickSortAction, QuickSortTimed),
            ("Бінарні вставки", BinaryInsertionAction, BinaryInsertionTimed),
            ("Array.Sort", ArraySortAction, null)
        };

        Console.WriteLine("TIME|n|algorithm|best-ms-or-limit");
        foreach (int length in new[] { 1_000, 10_000, 100_000, 1_000_000 })
        {
            int[] input = LabData.CreateValues(length);
            foreach ((string name, SortAction sort, CancellableSort? trySort) in algorithms)
            {
                string result = Measure(input, sort, trySort);
                Console.WriteLine($"TIME|{length}|{name}|{result}");
            }
        }

        Console.WriteLine($"ACCEPTANCE|{FindBubbleBoundary()}");
    }

    public static int FindBubbleBoundary()
    {
        BubbleSortAction(LabData.CreateValues(1_000));
        int low = 0;
        int high = 256;
        while (CompletesWithinOneSecond(high))
        {
            low = high;
            high *= 2;
        }

        while (high - low > 1)
        {
            int middle = low + ((high - low) / 2);
            if (CompletesWithinOneSecond(middle))
            {
                low = middle;
            }
            else
            {
                high = middle;
            }
        }

        return high;
    }

    private static string Measure(int[] input, SortAction sort, CancellableSort? cancellableSort)
    {
        int[] warmup = (int[])input.Clone();
        RunOnce(warmup, sort, cancellableSort);

        double bestMilliseconds = double.MaxValue;
        bool exceededLimit = false;
        for (int trial = 0; trial < 3; trial++)
        {
            int[] values = (int[])input.Clone();
            var stopwatch = Stopwatch.StartNew();
            bool completed = cancellableSort is null
                ? RunStandard(values, sort)
                : cancellableSort(values, () => stopwatch.Elapsed.TotalMilliseconds >= LimitMilliseconds);
            stopwatch.Stop();

            if (!completed || stopwatch.Elapsed.TotalMilliseconds >= LimitMilliseconds)
            {
                exceededLimit = true;
                continue;
            }

            if (!Validation.IsSorted(values, Comparer<int>.Default.Compare)
                || !Validation.IsPermutationOf(input, values))
            {
                throw new InvalidOperationException("Алгоритм не пройшов перевірку після вимірювання.");
            }

            bestMilliseconds = Math.Min(bestMilliseconds, stopwatch.Elapsed.TotalMilliseconds);
        }

        return exceededLimit
            ? ">1000 (one-second limit)"
            : bestMilliseconds.ToString("F3", System.Globalization.CultureInfo.InvariantCulture);
    }

    private static void RunOnce(int[] values, SortAction sort, CancellableSort? cancellableSort)
    {
        if (cancellableSort is null)
        {
            sort(values);
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        cancellableSort(values, () => stopwatch.Elapsed.TotalMilliseconds >= LimitMilliseconds);
    }

    private static bool RunStandard(int[] values, SortAction sort)
    {
        sort(values);
        return true;
    }

    private static bool CompletesWithinOneSecond(int length)
    {
        int[] values = LabData.CreateValues(length);
        var warmupWatch = Stopwatch.StartNew();
        BubbleSortTimed(values, () => warmupWatch.Elapsed.TotalMilliseconds >= LimitMilliseconds);

        double bestMilliseconds = double.MaxValue;
        for (int trial = 0; trial < 3; trial++)
        {
            values = LabData.CreateValues(length);
            var stopwatch = Stopwatch.StartNew();
            bool completed = BubbleSortTimed(
                values,
                () => stopwatch.Elapsed.TotalMilliseconds >= LimitMilliseconds);
            stopwatch.Stop();

            if (completed)
            {
                bestMilliseconds = Math.Min(bestMilliseconds, stopwatch.Elapsed.TotalMilliseconds);
            }
        }

        return bestMilliseconds < LimitMilliseconds;
    }

    private static void BubbleSortAction(int[] values) =>
        BubbleSort.Sort(values, Comparer<int>.Default.Compare, out _, out _);

    private static bool BubbleSortTimed(int[] values, Func<bool> shouldStop) =>
        BubbleSort.TrySort(values, Comparer<int>.Default.Compare, shouldStop, out _, out _);

    private static void QuickSortAction(int[] values) =>
        QuickSort.Sort(values, Comparer<int>.Default.Compare, out _, out _);

    private static bool QuickSortTimed(int[] values, Func<bool> shouldStop) =>
        QuickSort.TrySort(values, Comparer<int>.Default.Compare, shouldStop, out _, out _);

    private static void BinaryInsertionAction(int[] values) =>
        BinaryInsertionSort.Sort(values, Comparer<int>.Default.Compare, out _, out _);

    private static bool BinaryInsertionTimed(int[] values, Func<bool> shouldStop) =>
        BinaryInsertionSort.TrySort(values, Comparer<int>.Default.Compare, shouldStop, out _, out _);

    private static void ArraySortAction(int[] values) => Array.Sort(values);
}
