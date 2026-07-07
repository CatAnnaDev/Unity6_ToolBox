using System;
using System.Collections.Generic;
using UnityEngine;

namespace CatAnnaDev.ScriptableArchitecture
{
    public abstract class RuntimeSet<T> : ScriptableObject
    {
        [SerializeField] private bool clearOnEnable = true;

        private readonly List<T> items = new List<T>();

        public event Action<T> OnItemAdded;
        public event Action<T> OnItemRemoved;
        public event Action OnChanged;

        public int Count
        {
            get { return items.Count; }
        }

        public IReadOnlyList<T> Items
        {
            get { return items; }
        }

        public T this[int index]
        {
            get { return items[index]; }
        }

        public bool Add(T item)
        {
            if (item == null || items.Contains(item))
            {
                return false;
            }

            items.Add(item);

            Action<T> added = OnItemAdded;
            if (added != null)
            {
                added.Invoke(item);
            }

            RaiseChanged();
            return true;
        }

        public bool Remove(T item)
        {
            if (item == null)
            {
                return false;
            }

            int index = items.IndexOf(item);
            if (index < 0)
            {
                return false;
            }

            items.RemoveAt(index);

            Action<T> removed = OnItemRemoved;
            if (removed != null)
            {
                removed.Invoke(item);
            }

            RaiseChanged();
            return true;
        }

        public bool Contains(T item)
        {
            return items.Contains(item);
        }

        public void Clear()
        {
            if (items.Count == 0)
            {
                return;
            }

            items.Clear();
            RaiseChanged();
        }

        public T GetRandom()
        {
            if (items.Count == 0)
            {
                return default;
            }

            return items[UnityEngine.Random.Range(0, items.Count)];
        }

        public void CopyTo(List<T> destination)
        {
            if (destination == null)
            {
                return;
            }

            destination.Clear();
            for (int i = 0; i < items.Count; i++)
            {
                destination.Add(items[i]);
            }
        }

        private void RaiseChanged()
        {
            Action changed = OnChanged;
            if (changed != null)
            {
                changed.Invoke();
            }
        }

        protected virtual void OnEnable()
        {
            if (clearOnEnable)
            {
                items.Clear();
            }
        }

        protected virtual void OnDisable()
        {
            items.Clear();
        }
    }
}
