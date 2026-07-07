using System;
using System.Collections;
using System.Collections.Generic;

namespace CatAnnaDev.Utils
{
    public sealed class CircularBuffer<T> : IEnumerable<T>
    {
        private readonly T[] buffer;
        private int head;
        private int count;

        public CircularBuffer(int capacity)
        {
            if (capacity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity));
            }
            buffer = new T[capacity];
            head = 0;
            count = 0;
        }

        public int Capacity
        {
            get { return buffer.Length; }
        }

        public int Count
        {
            get { return count; }
        }

        public bool IsFull
        {
            get { return count == buffer.Length; }
        }

        public bool IsEmpty
        {
            get { return count == 0; }
        }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= count)
                {
                    throw new IndexOutOfRangeException();
                }
                return buffer[(head + index) % buffer.Length];
            }
            set
            {
                if (index < 0 || index >= count)
                {
                    throw new IndexOutOfRangeException();
                }
                buffer[(head + index) % buffer.Length] = value;
            }
        }

        public T Newest
        {
            get
            {
                if (count == 0)
                {
                    return default;
                }
                return buffer[(head + count - 1) % buffer.Length];
            }
        }

        public T Oldest
        {
            get
            {
                if (count == 0)
                {
                    return default;
                }
                return buffer[head];
            }
        }

        public void Add(T item)
        {
            int tail = (head + count) % buffer.Length;
            buffer[tail] = item;
            if (count == buffer.Length)
            {
                head = (head + 1) % buffer.Length;
            }
            else
            {
                count++;
            }
        }

        public bool TryRemoveOldest(out T item)
        {
            if (count == 0)
            {
                item = default;
                return false;
            }
            item = buffer[head];
            buffer[head] = default;
            head = (head + 1) % buffer.Length;
            count--;
            return true;
        }

        public void Clear()
        {
            Array.Clear(buffer, 0, buffer.Length);
            head = 0;
            count = 0;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < count; i++)
            {
                yield return buffer[(head + i) % buffer.Length];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
