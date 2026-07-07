using System;

namespace CatAnnaDev.Pooling
{
    [Serializable]
    public struct PoolStats
    {
        public int active;
        public int inactive;
        public int total;
        public int peakActive;
        public int totalSpawned;
        public int totalReleased;
        public int misses;

        public float ReuseRatio
        {
            get
            {
                if (totalSpawned <= 0) return 0f;
                int reused = totalSpawned - misses;
                if (reused < 0) reused = 0;
                return (float)reused / totalSpawned;
            }
        }

        public override string ToString()
        {
            return "active=" + active +
                   " inactive=" + inactive +
                   " total=" + total +
                   " peak=" + peakActive +
                   " spawned=" + totalSpawned +
                   " released=" + totalReleased +
                   " misses=" + misses +
                   " reuse=" + ReuseRatio.ToString("0.00");
        }
    }
}
