using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CatAnnaDev.Utils
{
    [Serializable]
    public sealed class WeightedList<T> : IEnumerable<T>
    {
        [Serializable]
        public struct Entry
        {
            public T Value;
            public float Weight;

            public Entry(T value, float weight)
            {
                Value = value;
                Weight = weight;
            }
        }

        [SerializeField]
        private List<Entry> entries;

        private float totalWeight;
        private bool dirty;

        public WeightedList()
        {
            entries = new List<Entry>();
            dirty = true;
        }

        public WeightedList(int capacity)
        {
            entries = new List<Entry>(capacity);
            dirty = true;
        }

        public int Count
        {
            get { return entries.Count; }
        }

        public T this[int index]
        {
            get { return entries[index].Value; }
        }

        public void Add(T value, float weight)
        {
            if (weight < 0f)
            {
                weight = 0f;
            }
            entries.Add(new Entry(value, weight));
            dirty = true;
        }

        public bool Remove(T value)
        {
            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < entries.Count; i++)
            {
                if (comparer.Equals(entries[i].Value, value))
                {
                    entries.RemoveAt(i);
                    dirty = true;
                    return true;
                }
            }
            return false;
        }

        public void Clear()
        {
            entries.Clear();
            dirty = true;
        }

        public float GetWeight(int index)
        {
            return entries[index].Weight;
        }

        public void SetWeight(int index, float weight)
        {
            Entry entry = entries[index];
            entry.Weight = weight < 0f ? 0f : weight;
            entries[index] = entry;
            dirty = true;
        }

        private void Recalculate()
        {
            totalWeight = 0f;
            for (int i = 0; i < entries.Count; i++)
            {
                totalWeight += entries[i].Weight;
            }
            dirty = false;
        }

        public T Sample()
        {
            if (entries.Count == 0)
            {
                return default;
            }
            if (dirty)
            {
                Recalculate();
            }
            if (totalWeight <= 0f)
            {
                return entries[UnityEngine.Random.Range(0, entries.Count)].Value;
            }
            float roll = UnityEngine.Random.value * totalWeight;
            float running = 0f;
            for (int i = 0; i < entries.Count; i++)
            {
                running += entries[i].Weight;
                if (roll <= running)
                {
                    return entries[i].Value;
                }
            }
            return entries[entries.Count - 1].Value;
        }

        public T Sample(ref RandomState state)
        {
            if (entries.Count == 0)
            {
                return default;
            }
            if (dirty)
            {
                Recalculate();
            }
            if (totalWeight <= 0f)
            {
                return entries[state.NextInt(0, entries.Count)].Value;
            }
            float roll = state.NextFloat() * totalWeight;
            float running = 0f;
            for (int i = 0; i < entries.Count; i++)
            {
                running += entries[i].Weight;
                if (roll <= running)
                {
                    return entries[i].Value;
                }
            }
            return entries[entries.Count - 1].Value;
        }

        public void MarkDirty()
        {
            dirty = true;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < entries.Count; i++)
            {
                yield return entries[i].Value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
