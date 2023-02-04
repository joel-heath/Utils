namespace JH24Utils;
public static class CollectionExtensions
{
    static int[] BubbleSort(this IEnumerable<int> input)
    {
        int[] output = input.ToArray();
        bool sorted = false;
        while (!sorted)
        {
            sorted = true;
            for (int i = 0; i < output.Length - 1; i++)
            {
                if (output[i] > output[i + 1])
                {
                    (output[i], output[i + 1]) = (output[i + 1], output[i]);
                    sorted = false;
                }
            }
        }
        return output;
    }

    static int[] QuickSort(this IReadOnlyList<int> input)
    {
        if (input.Count <= 1) return input.ToArray();

        List<int> smaller = new(input.Count), larger = new(input.Count);

        for (int i = 1; i < input.Count; i++)
        {
            if (input[i] < input[0]) smaller.Add(input[i]);
            else larger.Add(input[i]);
        }

        return QuickSort(smaller)
              .Append(input[0])
              .Concat(QuickSort(larger))
              .ToArray();
    }

    static int[] Merge(int[] left, int[] right)
    {
        int[] merged = new int[left.Length + right.Length];
        int li = 0, ri = 0, mi = 0;

        while (li < left.Length && ri < right.Length)
            merged[mi++] = left[li] < right[ri] ? left[li++] : right[ri++];

        while (li < left.Length) merged[mi++] = left[li++];
        while (ri < right.Length) merged[mi++] = right[ri++];

        return merged;
    }

    static int[] MergeSort(this IEnumerable<int> input)
    {
        int[][] subarrays = input.Select(n => new int[] { n }).ToArray();

        while (subarrays.Length > 1)
        {
            int[][] temp = new int[(subarrays.Length + 1) / 2][];

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