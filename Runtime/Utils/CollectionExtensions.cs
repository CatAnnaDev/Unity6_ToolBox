using System.Collections.Generic;

namespace CatAnnaDev.Utils
{
    public static class CollectionExtensions
    {
        public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
        {
            return collection == null || collection.Count == 0;
        }

        public static bool IsNullOrEmpty<T>(this T[] array)
        {
            return array == null || array.Length == 0;
        }

        public static T RandomElement<T>(this IList<T> list)
        {
            if (list == null || list.Count == 0)
            {
                return default;
            }
            return list[UnityEngine.Random.Range(0, list.Count)];
        }

        public static T RandomElement<T>(this IList<T> list, ref RandomState state)
        {
            if (list == null || list.Count == 0)
            {
                return default;
            }
            return list[state.NextInt(0, list.Count)];
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }

        public static void Shuffle<T>(this IList<T> list, ref RandomState state)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = state.NextInt(0, i + 1);
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }

        public static void Swap<T>(this IList<T> list, int a, int b)
        {
            T temp = list[a];
            list[a] = list[b];
            list[b] = temp;
        }

        public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
            where TValue : new()
        {
            if (!dictionary.TryGetValue(key, out TValue value))
            {
                value = new TValue();
                dictionary[key] = value;
            }
            return value;
        }

        public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            if (!dictionary.TryGetValue(key, out TValue value))
            {
                value = defaultValue;
                dictionary[key] = value;
            }
            return value;
        }

        public static TValue GetOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default)
        {
            return dictionary.TryGetValue(key, out TValue value) ? value : defaultValue;
        }

        public static void AddRange<T>(this HashSet<T> set, IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                set.Add(item);
            }
        }

        public static T WeightedRandom<T>(this IList<T> items, IList<float> weights)
        {
            if (items == null || items.Count == 0)
            {
                return default;
            }
            float total = 0f;
            for (int i = 0; i < weights.Count; i++)
            {
                total += weights[i];
            }
            float roll = UnityEngine.Random.value * total;
            float running = 0f;
            for (int i = 0; i < items.Count; i++)
            {
                running += weights[i];
                if (roll <= running)
                {
                    return items[i];
                }
            }
            return items[items.Count - 1];
        }

        public static int IndexOf<T>(this IReadOnlyList<T> list, T item)
        {
            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < list.Count; i++)
            {
                if (comparer.Equals(list[i], item))
                {
                    return i;
                }
            }
            return -1;
        }

        public static bool TryGetRandom<T>(this IList<T> list, out T result)
        {
            if (list == null || list.Count == 0)
            {
                result = default;
                return false;
            }
            result = list[UnityEngine.Random.Range(0, list.Count)];
            return true;
        }

        public static T First<T>(this IList<T> list)
        {
            return list.Count > 0 ? list[0] : default;
        }

        public static T Last<T>(this IList<T> list)
        {
            return list.Count > 0 ? list[list.Count - 1] : default;
        }

        public static void RemoveBySwap<T>(this IList<T> list, int index)
        {
            int last = list.Count - 1;
            list[index] = list[last];
            list.RemoveAt(last);
        }
    }
}
