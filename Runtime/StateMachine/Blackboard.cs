using System;
using System.Collections.Generic;

namespace CatAnnaDev.StateMachine
{
    public readonly struct BlackboardKey : IEquatable<BlackboardKey>
    {
        public readonly string Name;
        private readonly int _hash;

        public BlackboardKey(string name)
        {
            Name = name ?? string.Empty;
            _hash = Name.GetHashCode();
        }

        public bool Equals(BlackboardKey other)
        {
            return _hash == other._hash && string.Equals(Name, other.Name, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return obj is BlackboardKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _hash;
        }

        public override string ToString()
        {
            return Name;
        }

        public static implicit operator BlackboardKey(string name)
        {
            return new BlackboardKey(name);
        }
    }

    public sealed class Blackboard
    {
        private readonly Dictionary<BlackboardKey, object> _values = new Dictionary<BlackboardKey, object>();

        public int Count
        {
            get { return _values.Count; }
        }

        public void Set<T>(BlackboardKey key, T value)
        {
            _values[key] = value;
        }

        public bool TryGet<T>(BlackboardKey key, out T value)
        {
            if (_values.TryGetValue(key, out object raw) && raw is T typed)
            {
                value = typed;
                return true;
            }

            value = default;
            return false;
        }

        public T Get<T>(BlackboardKey key)
        {
            if (TryGet(key, out T value))
            {
                return value;
            }

            throw new KeyNotFoundException("Blackboard key not found or type mismatch: " + key);
        }

        public T GetOrDefault<T>(BlackboardKey key, T fallback = default)
        {
            return TryGet(key, out T value) ? value : fallback;
        }

        public bool Has(BlackboardKey key)
        {
            return _values.ContainsKey(key);
        }

        public bool Remove(BlackboardKey key)
        {
            return _values.Remove(key);
        }

        public void Clear()
        {
            _values.Clear();
        }
    }
}
