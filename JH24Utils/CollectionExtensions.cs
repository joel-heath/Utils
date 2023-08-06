using System.Collections.Generic;
using System.Diagnostics.Metrics;

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

    public static IEnumerable<T> QuickSort<T>(this IReadOnlyList<T> input) where T : IComparable<T> => RecursiveQuickSort(input, input.Count); 
    private static IEnumerable<T> RecursiveQuickSort<T>(IReadOnlyList<T> input, int count) where T : IComparable<T>
    {
        if (count <= 1)
        {
            foreach (var item in input.Take(count)) yield return item;
            yield break;
        }

        T[] smaller = new T[count], larger = new T[count];
        int smallerIndex = 0, largerIndex = 0;

        for (int i = 1; i < count; i++)
        {
            if (input[i].CompareTo(input[0]) < 0) smaller[smallerIndex++] = input[i];
            else larger[largerIndex++] = input[i];
        }

        foreach (var item in RecursiveQuickSort(smaller, smallerIndex)) yield return item;
        yield return input[0];
        foreach (var item in RecursiveQuickSort(larger, largerIndex)) yield return item;
    }

    public static IEnumerable<T> QuickSortArrays<T>(this IReadOnlyList<T> input) where T : IComparable<T>
    {
        if (input.Count <= 1)
        {
            foreach (var item in input) yield return item;
            yield break;
        }

        T[] smaller = new T[input.Count], larger = new T[input.Count];
        int smallerIndex = 0, largerIndex = 0;

        for (int i = 1; i < input.Count; i++)
        {
            if (input[i].CompareTo(input[0]) < 0) smaller[smallerIndex++] = input[i];
            else larger[largerIndex++] = input[i];
        }

        foreach (var item in QuickSortArrays(smaller.Take(smallerIndex).ToArray())) yield return item;
        yield return input[0];
        foreach (var item in QuickSortArrays(larger.Take(largerIndex).ToArray())) yield return item;
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