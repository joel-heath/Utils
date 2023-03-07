namespace JH24Utils;
public static class CollectionExtensions
{ 
    public static IEnumerable<T> BubbleSort<T>(this IEnumerable<T> input) where T : IComparable<T>
    {
        T[] output = input.ToArray();
        bool sorted = false;
        while (!sorted)
        {
            sorted = true;
            for (int i = 0; i < output.Length - 1; i++)
            {
                if (output[i].CompareTo(output[i + 1]) > 0)
                {
                    (output[i], output[i + 1]) = (output[i + 1], output[i]);
                    sorted = false;
                }
            }
        }
        return output;
    }

    public static IEnumerable<T> QuickSort<T>(this IReadOnlyList<T> input) where T : IComparable<T>
    {
        if (input.Count <= 1) return input;

        List<T> smaller = new(input.Count), larger = new(input.Count);

        for (int i = 1; i < input.Count; i++)
        {
            if (input[i].CompareTo(input[0]) < 0) smaller.Add(input[i]);
            else larger.Add(input[i]);
        }

        return QuickSort(smaller)
              .Append(input[0])
              .Concat(QuickSort(larger));
    }

    public static T[] Merge<T>(T[] left, T[] right) where T : IComparable<T>
    {
        T[] merged = new T[left.Length + right.Length];
        int li = 0, ri = 0, mi = 0;

        while (li < left.Length && ri < right.Length)
            merged[mi++] = left[li].CompareTo(right[ri]) < 0 ? left[li++] : right[ri++];

        while (li < left.Length) merged[mi++] = left[li++];
        while (ri < right.Length) merged[mi++] = right[ri++];

        return merged;
    }

    public static IEnumerable<T> MergeSort<T>(this IEnumerable<T> input) where T : IComparable<T>
    {
        T[][] subarrays = input.Select(n => new T[] { n }).ToArray();

        while (subarrays.Length > 1)
        {
            T[][] temp = new T[(subarrays.Length + 1) / 2][];

            for (int i = 0; i < subarrays.Length / 2; i++)
            {
                temp[i] = Merge(subarrays[i * 2], subarrays[i * 2 + 1]);
            }

            if (subarrays.Length % 2 == 1) temp[^1] = subarrays[^1];

            subarrays = temp;
        }

        return subarrays[0];
    }
}