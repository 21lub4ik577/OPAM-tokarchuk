public static class QuickSort
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
        SortRange(array, 0, array.Length - 1, compare, null, ref comparisons, ref moves, ref operations);
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
        return SortRange(array, 0, array.Length - 1, compare, shouldStop, ref comparisons, ref moves, ref operations);
    }

    private static bool SortRange<T>(
        T[] array,
        int left,
        int right,
        Func<T, T, int> compare,
        Func<bool>? shouldStop,
        ref int comparisons,
        ref int moves,
        ref int operations)
    {
        while (left < right)
        {
            if (shouldStop is not null && (operations++ & 1023) == 0 && shouldStop())
            {
                return false;
            }

            int i = left;
            int j = right;
            T pivot = array[left + ((right - left) / 2)];

            while (i <= j)
            {
                while (i <= right)
                {
                    if (shouldStop is not null && (operations++ & 1023) == 0 && shouldStop())
                    {
                        return false;
                    }

                    comparisons++;
                    if (compare(array[i], pivot) >= 0)
                    {
                        break;
                    }

                    i++;
                }

                while (j >= left)
                {
                    if (shouldStop is not null && (operations++ & 1023) == 0 && shouldStop())
                    {
                        return false;
                    }

                    comparisons++;
                    if (compare(array[j], pivot) <= 0)
                    {
                        break;
                    }

                    j--;
                }

                if (i <= j)
                {
                    if (i < j)
                    {
                        (array[i], array[j]) = (array[j], array[i]);
                        moves += 2;
                    }

                    i++;
                    j--;
                }
            }

            if (j - left < right - i)
            {
                if (left < j && !SortRange(array, left, j, compare, shouldStop, ref comparisons, ref moves, ref operations))
                {
                    return false;
                }

                left = i;
            }
            else
            {
                if (i < right && !SortRange(array, i, right, compare, shouldStop, ref comparisons, ref moves, ref operations))
                {
                    return false;
                }

                right = j;
            }
        }

        return true;
    }
}
