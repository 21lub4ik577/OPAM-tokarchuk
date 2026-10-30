using System;
using System.Collections.Generic;

public static class Algorithms
{
    public static List<T> Filter<T>(List<T> src, Func<T, bool> keep)
    {
        ArgumentNullException.ThrowIfNull(src);
        ArgumentNullException.ThrowIfNull(keep);

        List<T> result = new();
        foreach (T item in src)
        {
            if (keep(item))
            {
                result.Add(item);
            }
        }

        return result;
    }

    public static List<TOut> Map<TIn, TOut>(List<TIn> src, Func<TIn, TOut> convert)
    {
        ArgumentNullException.ThrowIfNull(src);
        ArgumentNullException.ThrowIfNull(convert);

        List<TOut> result = new();
        foreach (TIn item in src)
        {
            result.Add(convert(item));
        }

        return result;
    }

    public static TAcc Aggregate<T, TAcc>(
        List<T> src,
        TAcc start,
        Func<TAcc, T, TAcc> step)
    {
        ArgumentNullException.ThrowIfNull(src);
        ArgumentNullException.ThrowIfNull(step);

        TAcc state = start;
        foreach (T item in src)
        {
            state = step(state, item);
        }

        return state;
    }

    public static Dictionary<TKey, List<T>> GroupBy<T, TKey>(
        List<T> src,
        Func<T, TKey> keyOf)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(src);
        ArgumentNullException.ThrowIfNull(keyOf);

        Dictionary<TKey, List<T>> groups = new();
        foreach (T item in src)
        {
            TKey key = keyOf(item);
            if (!groups.TryGetValue(key, out List<T>? group))
            {
                group = new List<T>();
                groups.Add(key, group);
            }

            group.Add(item);
        }

        return groups;
    }
}
