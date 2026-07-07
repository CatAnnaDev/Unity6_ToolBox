using System;

namespace CatAnnaDev.Pooling
{
    public enum ExpandMode
    {
        Fixed,
        Grow,
        GrowByChunk
    }

    [Serializable]
    public struct ExpandPolicy
    {
        public ExpandMode mode;
        public int chunkSize;
        public int hardCeiling;

        public ExpandPolicy(ExpandMode mode, int chunkSize, int hardCeiling)
        {
            this.mode = mode;
            this.chunkSize = chunkSize < 1 ? 1 : chunkSize;
            this.hardCeiling = hardCeiling < 0 ? 0 : hardCeiling;
        }

        public bool HasCeiling => hardCeiling > 0;

        public static ExpandPolicy Default => new ExpandPolicy(ExpandMode.Grow, 8, 0);
        public static ExpandPolicy Grow => new ExpandPolicy(ExpandMode.Grow, 8, 0);

        public static ExpandPolicy FixedSize(int ceiling)
        {
            return new ExpandPolicy(ExpandMode.Fixed, 1, ceiling);
        }

        public static ExpandPolicy Chunk(int chunkSize, int ceiling = 0)
        {
            return new ExpandPolicy(ExpandMode.GrowByChunk, chunkSize, ceiling);
        }
    }
}
