public static class Task2Data
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
}
