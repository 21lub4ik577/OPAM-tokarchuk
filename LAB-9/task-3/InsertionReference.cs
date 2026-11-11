public static class InsertionReference
{
    public static void Sort<T>(
        T[] array,
        Func<T, T, int> compare,
        out int comparisons,
        out int moves)
    {
        comparisons = 0;
        moves = 0;

        for (int i = 1; i < array.Length; i++)
        {
            T value = array[i];
            int position = i;
            while (position > 0)
            {
                comparisons++;
                if (compare(array[position - 1], value) <= 0)
                {
                    break;
                }

                array[position] = array[position - 1];
                moves++;
                position--;
            }

            if (position != i)
            {
                array[position] = value;
                moves++;
            }
        }
    }
}
