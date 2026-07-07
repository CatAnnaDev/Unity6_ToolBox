using System;
using UnityEngine;

namespace CatAnnaDev.Saving
{
    [Serializable]
    public struct SerializableGuid : IEquatable<SerializableGuid>
    {
        [SerializeField] private string value;

        public SerializableGuid(string guid)
        {
            value = string.IsNullOrEmpty(guid) ? string.Empty : guid;
        }

        public SerializableGuid(Guid guid)
        {
            value = guid.ToString("N");
        }

        public string Value
        {
            get { return string.IsNullOrEmpty(value) ? string.Empty : value; }
        }

        public bool IsValid
        {
            get { return !string.IsNullOrEmpty(value); }
        }

        public static SerializableGuid NewGuid()
        {
            return new SerializableGuid(Guid.NewGuid());
        }

        public Guid ToGuid()
        {
            Guid parsed;
            return Guid.TryParse(value, out parsed) ? parsed : Guid.Empty;
        }

        public bool Equals(SerializableGuid other)
        {
            return string.Equals(Value, other.Value, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return obj is SerializableGuid other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value;
        }

        public static bool operator ==(SerializableGuid left, SerializableGuid right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SerializableGuid left, SerializableGuid right)
        {
            return !left.Equals(right);
        }

        public static implicit operator string(SerializableGuid guid)
        {
            return guid.Value;
        }
    }
}
