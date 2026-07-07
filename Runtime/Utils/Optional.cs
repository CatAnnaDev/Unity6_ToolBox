using System;
using UnityEngine;

namespace CatAnnaDev.Utils
{
    [Serializable]
    public struct Optional<T>
    {
        [SerializeField]
        private bool enabled;

        [SerializeField]
        private T value;

        public Optional(T value, bool enabled = true)
        {
            this.value = value;
            this.enabled = enabled;
        }

        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        public T Value
        {
            get { return value; }
            set { this.value = value; }
        }

        public bool HasValue
        {
            get { return enabled; }
        }

        public T GetValueOrDefault(T fallback = default)
        {
            return enabled ? value : fallback;
        }

        public bool TryGet(out T result)
        {
            result = value;
            return enabled;
        }

        public static implicit operator bool(Optional<T> optional)
        {
            return optional.enabled;
        }

        public static Optional<T> Some(T value)
        {
            return new Optional<T>(value, true);
        }

        public static Optional<T> None
        {
            get { return new Optional<T>(default, false); }
        }
    }
}
