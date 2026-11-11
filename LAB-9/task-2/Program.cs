int[] values = Task2Data.CreateValues(20);
QuickSort.Sort(values, Comparer<int>.Default.Compare, out int comparisons, out int moves);
Console.WriteLine($"Відсортований масив: {string.Join(", ", values)}");
Console.WriteLine($"Порівняння: {comparisons}; записи в масив: {moves}");

int[] equalValues = new int[100];
QuickSort.Sort(equalValues, Comparer<int>.Default.Compare, out comparisons, out moves);
Console.WriteLine($"100 однакових елементів: {comparisons} порівнянь, {moves} записів.");
