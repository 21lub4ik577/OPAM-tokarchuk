public static class LabData
{
    public static int[] CreateValues(int length)
    {
        var random = new Random(26);
        var values = new int[length];
        for (int i = 0; i < values.Length; i++)
        {
            values[i] = random.Next(321);
        }

        return values;
    }

    public static (string Name, int[] Values)[] CreateInputs(int length)
    {
        int[] random = CreateValues(length);
        int[] sorted = (int[])random.Clone();
        QuickSort.Sort(sorted, Comparer<int>.Default.Compare, out _, out _);

        int[] reversed = (int[])sorted.Clone();
        Reverse(reversed);

        int[] nearlySorted = (int[])sorted.Clone();
        var swapRandom = new Random(26);
        for (int i = 0; i < 16 && length > 1; i++)
        {
            int first = swapRandom.Next(length);
            int second = swapRandom.Next(length);
            (nearlySorted[first], nearlySorted[second]) = (nearlySorted[second], nearlySorted[first]);
        }

        return new[]
        {
            ("Випадковий", random),
            ("Відсортований", sorted),
            ("Навпаки", reversed),
            ("Майже відсортований", nearlySorted)
        };
    }

    private static void Reverse(int[] values)
    {
        for (int left = 0, right = values.Length - 1; left < right; left++, right--)
        {
            (values[left], values[right]) = (values[right], values[left]);
        }
    }
}
