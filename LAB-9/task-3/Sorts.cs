public delegate void SortWithCounts<T>(
    T[] array,
    Func<T, T, int> compare,
    out int comparisons,
    out int moves);

public static class BinaryInsertionSort
{
    public static void Sort<T>(
        T[] array,
        Func<T, T, int> compare,
        out int comparisons,
        out int moves)
    {
        ArgumentNullException.ThrowIfNull(array);
        ArgumentNullException.ThrowIfNull(compare);

        comparisons = 0;
        moves = 0;
        int operations = 0;
        SortCore(array, compare, null, ref comparisons, ref moves, ref operations);
    }

    public static bool TrySort<T>(
        T[] array,
        Func<T, T, int> compare,
        Func<bool> shouldStop,
        out int comparisons,
        out int moves)
    {
        ArgumentNullException.ThrowIfNull(array);
        ArgumentNullException.ThrowIfNull(compare);
        ArgumentNullException.ThrowIfNull(shouldStop);

        comparisons = 0;
        moves = 0;
        int operations = 0;
        return SortCore(array, compare, shouldStop, ref comparisons, ref moves, ref operations);
    }

    private static bool SortCore<T>(
        T[] array,
        Func<T, T, int> compare,
        Func<bool>? shouldStop,
        ref int comparisons,
        ref int moves,
        ref int operations)
    {
        for (int i = 1; i < array.Length; i++)
        {
            if (shouldStop is not null && (operations++ & 1023) == 0 && shouldStop())
            {
                return false;
            }

            T value = array[i];
            int low = 0;
            int high = i;
            while (low < high)
            {
                if (shouldStop is not null && (operations++ & 1023) == 0 && shouldStop())
                {
                    return false;
                }

                int middle = low + ((high - low) / 2);
                comparisons++;
                if (compare(value, array[middle]) < 0)
                {
                    high = middle;
                }
                else
                {
                    low = middle + 1;
                }
            }

            if (low == i)
            {
                continue;
            }

            for (int j = i; j > low; j--)
            {
                if (shouldStop is not null && (operations++ & 1023) == 0 && shouldStop())
                {
                    return false;
                }

                array[j] = array[j - 1];
                moves++;
            }

            array[low] = value;
            moves++;
        }

        return true;
    }
}
