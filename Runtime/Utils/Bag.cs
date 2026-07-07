using System;
using System.Collections;
using System.Collections.Generic;

namespace CatAnnaDev.Utils
{
    public sealed class Bag<T> : IEnumerable<T>
    {
        private T[] items;
        private int count;

        public Bag(int capacity = 16)
        {
            items = new T[Math.Max(1, capacity)];
            count = 0;
        }

        public int Count
        {
            get { return count; }
        }

        public T this[int index]
        {
            get { return items[index]; }
            set { items[index] = value; }
        }

        public void Add(T item)
        {
            if (count == items.Length)
            {
                Array.Resize(ref items, items.Length * 2);
            }
            items[count++] = item;
        }

        public void RemoveAt(int index)
        {
            count--;
            items[index] = items[count];
            items[count] = default;
        }

        public bool Remove(T item)
        {
            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < count; i++)
            {
                if (comparer.Equals(items[i], item))
                {
                    RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        public bool Contains(T item)
        {
            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < count; i++)
            {
                if (comparer.Equals(items[i], item))
                {
                    return true;
                }
            }
            return false;
        }

        public void Clear()
        {
            Array.Clear(items, 0, count);
            count = 0;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < count; i++)
            {
                yield return items[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
