using System;
using System.Collections.Generic;
using System.Text;

namespace CatAnnaDev.Utils
{
    public static class ListPool<T>
    {
        private static readonly Stack<List<T>> Pool = new Stack<List<T>>();

        public static List<T> Get()
        {
            return Pool.Count > 0 ? Pool.Pop() : new List<T>();
        }

        public static PooledScope Get(out List<T> list)
        {
            list = Get();
            return new PooledScope(list);
        }

        public static void Release(List<T> list)
        {
            if (list == null)
            {
                return;
            }
            list.Clear();
            Pool.Push(list);
        }

        public readonly struct PooledScope : IDisposable
        {
            private readonly List<T> list;

            public PooledScope(List<T> list)
            {
                this.list = list;
            }

            public void Dispose()
            {
                Release(list);
            }
        }
    }

    public static class DictionaryPool<TKey, TValue>
    {
        private static readonly Stack<Dictionary<TKey, TValue>> Pool = new Stack<Dictionary<TKey, TValue>>();

        public static Dictionary<TKey, TValue> Get()
        {
            return Pool.Count > 0 ? Pool.Pop() : new Dictionary<TKey, TValue>();
        }

        public static PooledScope Get(out Dictionary<TKey, TValue> dictionary)
        {
            dictionary = Get();
            return new PooledScope(dictionary);
        }

        public static void Release(Dictionary<TKey, TValue> dictionary)
        {
            if (dictionary == null)
            {
                return;
            }
            dictionary.Clear();
            Pool.Push(dictionary);
        }

        public readonly struct PooledScope : IDisposable
        {
            private readonly Dictionary<TKey, TValue> dictionary;

            public PooledScope(Dictionary<TKey, TValue> dictionary)
            {
                this.dictionary = dictionary;
            }

            public void Dispose()
            {
                Release(dictionary);
            }
        }
    }

    public static class HashSetPool<T>
    {
        private static readonly Stack<HashSet<T>> Pool = new Stack<HashSet<T>>();

        public static HashSet<T> Get()
        {
            return Pool.Count > 0 ? Pool.Pop() : new HashSet<T>();
        }

        public static PooledScope Get(out HashSet<T> set)
        {
            set = Get();
            return new PooledScope(set);
        }

        public static void Release(HashSet<T> set)
        {
            if (set == null)
            {
                return;
            }
            set.Clear();
            Pool.Push(set);
        }

        public readonly struct PooledScope : IDisposable
        {
            private readonly HashSet<T> set;

            public PooledScope(HashSet<T> set)
            {
                this.set = set;
            }

            public void Dispose()
            {
                Release(set);
            }
        }
    }

    public static class StringBuilderPool
    {
        private static readonly Stack<StringBuilder> Pool = new Stack<StringBuilder>();

        public static StringBuilder Get()
        {
            return Pool.Count > 0 ? Pool.Pop() : new StringBuilder(256);
        }

        public static PooledScope Get(out StringBuilder builder)
        {
            builder = Get();
            return new PooledScope(builder);
        }

        public static void Release(StringBuilder builder)
        {
            if (builder == null)
            {
                return;
            }
            builder.Clear();
            Pool.Push(builder);
        }

        public static string ReleaseToString(StringBuilder builder)
        {
            if (builder == null)
            {
                return string.Empty;
            }
            string result = builder.ToString();
            Release(builder);
            return result;
        }

        public readonly struct PooledScope : IDisposable
        {
            private readonly StringBuilder builder;

            public PooledScope(StringBuilder builder)
            {
                this.builder = builder;
            }

            public void Dispose()
            {
                Release(builder);
            }
        }
    }
}
