public static class Validation
{
    public static bool IsSorted<T>(T[] values, Func<T, T, int> compare)
    {
        for (int i = 1; i < values.Length; i++)
        {
            if (compare(values[i - 1], values[i]) > 0)
            {
                return false;
            }
        }

        return true;
    }

    public static bool IsPermutationOf<T>(T[] original, T[] result) where T : notnull
    {
        if (original.Length != result.Length)
        {
            return false;
        }

        var counts = new Dictionary<T, int>();
        foreach (T value in original)
        {
            counts.TryGetValue(value, out int count);
            counts[value] = count + 1;
        }

        foreach (T value in result)
        {
            if (!counts.TryGetValue(value, out int count))
            {
                return false;
            }

            if (count == 1)
            {
                counts.Remove(value);
            }
            else
            {
                counts[value] = count - 1;
            }
        }

        return counts.Count == 0;
    }
}
