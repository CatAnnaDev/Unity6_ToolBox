using System;
using UnityEngine;

namespace CatAnnaDev.Utils
{
    [Serializable]
    public struct SerializableGuid : IEquatable<SerializableGuid>
    {
        [SerializeField]
        private uint part0;

        [SerializeField]
        private uint part1;

        [SerializeField]
        private uint part2;

        [SerializeField]
        private uint part3;

        public SerializableGuid(uint p0, uint p1, uint p2, uint p3)
        {
            part0 = p0;
            part1 = p1;
            part2 = p2;
            part3 = p3;
        }

        public static SerializableGuid Empty
        {
            get { return new SerializableGuid(0u, 0u, 0u, 0u); }
        }

        public bool IsEmpty
        {
            get { return part0 == 0u && part1 == 0u && part2 == 0u && part3 == 0u; }
        }

        public static SerializableGuid New()
        {
            return FromGuid(Guid.NewGuid());
        }

        public static SerializableGuid FromGuid(Guid guid)
        {
            byte[] bytes = guid.ToByteArray();
            return new SerializableGuid(
                BitConverter.ToUInt32(bytes, 0),
                BitConverter.ToUInt32(bytes, 4),
                BitConverter.ToUInt32(bytes, 8),
                BitConverter.ToUInt32(bytes, 12));
        }

        public Guid ToGuid()
        {
            byte[] bytes = new byte[16];
            BitConverter.GetBytes(part0).CopyTo(bytes, 0);
            BitConverter.GetBytes(part1).CopyTo(bytes, 4);
            BitConverter.GetBytes(part2).CopyTo(bytes, 8);
            BitConverter.GetBytes(part3).CopyTo(bytes, 12);
            return new Guid(bytes);
        }

        public bool Equals(SerializableGuid other)
        {
            return part0 == other.part0 && part1 == other.part1 && part2 == other.part2 && part3 == other.part3;
        }

        public override bool Equals(object obj)
        {
            return obj is SerializableGuid other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = (int)part0;
                hash = (hash * 397) ^ (int)part1;
                hash = (hash * 397) ^ (int)part2;
                hash = (hash * 397) ^ (int)part3;
                return hash;
            }
        }

        public override string ToString()
        {
            return ToGuid().ToString("N");
        }

        public static bool operator ==(SerializableGuid a, SerializableGuid b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(SerializableGuid a, SerializableGuid b)
        {
            return !a.Equals(b);
        }
    }
}
