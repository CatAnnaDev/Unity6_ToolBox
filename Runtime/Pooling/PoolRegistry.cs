using System;
using UnityEngine;

namespace CatAnnaDev.Pooling
{
    [CreateAssetMenu(menuName = "CatAnnaDev/Pooling/Pool Registry", fileName = "PoolRegistry")]
    public sealed class PoolRegistry : ScriptableObject
    {
        [SerializeField] PoolConfig[] pools = Array.Empty<PoolConfig>();
        [SerializeField] bool prewarmOnBoot = true;

        public PoolConfig[] Pools => pools;
        public bool PrewarmOnBoot => prewarmOnBoot;

        public int Count => pools != null ? pools.Length : 0;
    }
}
