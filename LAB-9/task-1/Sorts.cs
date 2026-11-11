using System;

public static class BubbleSort
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

        for (int end = array.Length - 1; end > 0; end--)
        {
            bool swapped = false;
            for (int i = 0; i < end; i++)
            {
                comparisons++;
                if (compare(array[i], array[i + 1]) <= 0)
                {
                    continue;
                }

                (array[i], array[i + 1]) = (array[i + 1], array[i]);
                moves += 2;
                swapped = true;
            }

            if (!swapped)
            {
                break;
            }
        }
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

        for (int end = array.Length - 1; end > 0; end--)
        {
            bool swapped = false;
            for (int i = 0; i < end; i++)
            {
                if ((operations++ & 1023) == 0 && shouldStop())
                {
                    return false;
                }

                comparisons++;
                if (compare(array[i], array[i + 1]) <= 0)
                {
                    continue;
                }

                (array[i], array[i + 1]) = (array[i + 1], array[i]);
                moves += 2;
                swapped = true;
            }

            if (!swapped)
            {
                break;
            }
        }

        return true;
    }
}
