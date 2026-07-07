using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CatAnnaDev.Services;

namespace CatAnnaDev.Pooling
{
    [AddComponentMenu("CatAnnaDev/Pooling/Pool Manager")]
    public sealed class PoolManager : PersistentSingleton<PoolManager>
    {
        [SerializeField] PoolRegistry bootRegistry;
        [SerializeField] bool prewarmRegistryOnStart = true;
        [SerializeField, Min(1)] int registryPrewarmPerFrame = 8;
        [SerializeField, Min(0f)] float cullCheckInterval = 5f;

        readonly Dictionary<GameObject, GameObjectPool> _pools = new Dictionary<GameObject, GameObjectPool>(64);
        readonly Dictionary<string, GameObjectPool> _poolsById = new Dictionary<string, GameObjectPool>(64);
        readonly List<GameObjectPool> _all = new List<GameObjectPool>(64);

        Coroutine _cullRoutine;

        public int PoolCount => _all.Count;

        void Start()
        {
            if (prewarmRegistryOnStart && bootRegistry != null && bootRegistry.PrewarmOnBoot)
            {
                PrewarmRegistry(bootRegistry);
            }
            EnsureCullLoop();
        }

        void EnsureCullLoop()
        {
            if (_cullRoutine == null && cullCheckInterval > 0f && isActiveAndEnabled)
            {
                _cullRoutine = StartCoroutine(CullLoop());
            }
        }

        public GameObject Spawn(GameObject prefab)
        {
            if (prefab == null)
            {
                CatLog.Error("PoolManager: Spawn called with null prefab.");
                return null;
            }
            Transform pt = prefab.transform;
            return Spawn(prefab, pt.position, pt.rotation, null);
        }

        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            if (prefab == null)
            {
                CatLog.Error("PoolManager: Spawn called with null prefab.");
                return null;
            }

            GameObjectPool pool = GetOrCreatePool(prefab);
            GameObject go = pool.Get(position, rotation, parent);
            if (go == null)
            {
                CatLog.Warn("PoolManager: pool for '" + prefab.name + "' is at its fixed ceiling, spawn refused.", prefab);
            }
            return go;
        }

        public void Despawn(GameObject instance)
        {
            if (instance == null) return;

            if (instance.TryGetComponent(out PooledObject po) && po.OwningPool != null)
            {
                po.OwningPool.Release(po);
            }
            else
            {
                CatLog.Warn("PoolManager: Despawn called on non-pooled '" + instance.name + "', destroying.", instance);
                Destroy(instance);
            }
        }

        public void Despawn(GameObject instance, float delay)
        {
            if (instance == null) return;

            if (delay <= 0f)
            {
                Despawn(instance);
                return;
            }

            if (instance.TryGetComponent(out PooledObject po) && po.OwningPool != null)
            {
                StartCoroutine(DespawnAfter(po, po.Generation, delay));
            }
            else
            {
                StartCoroutine(DestroyAfter(instance, delay));
            }
        }

        IEnumerator DespawnAfter(PooledObject po, int generation, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (po != null && po.IsActive && po.Generation == generation && po.OwningPool != null)
            {
                po.OwningPool.Release(po);
            }
        }

        IEnumerator DestroyAfter(GameObject go, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (go != null) Destroy(go);
        }

        public void Prewarm(GameObject prefab, int count)
        {
            if (prefab == null || count <= 0) return;
            GetOrCreatePool(prefab).Prewarm(count);
        }

        public GameObjectPool GetOrCreatePool(GameObject prefab)
        {
            if (prefab == null) return null;
            if (_pools.TryGetValue(prefab, out GameObjectPool pool)) return pool;
            return CreatePoolInternal(prefab, ExpandPolicy.Default, 0, false, 0, null, null);
        }

        public GameObjectPool CreatePool(PoolConfig config, bool prewarmNow = true)
        {
            if (config == null || !config.IsValid)
            {
                CatLog.Warn("PoolManager: CreatePool called with an invalid PoolConfig.");
                return null;
            }

            GameObject prefab = config.Prefab;
            if (_pools.TryGetValue(prefab, out GameObjectPool existing)) return existing;

            GameObjectPool pool = CreatePoolInternal(
                prefab,
                config.ExpandPolicy,
                config.MaxSize,
                config.CullExcess,
                config.PrewarmCount,
                config.Id,
                config.Category);

            if (prewarmNow && config.PrewarmCount > 0)
            {
                pool.Prewarm(config.PrewarmCount);
            }

            return pool;
        }

        GameObjectPool CreatePoolInternal(GameObject prefab, ExpandPolicy policy, int maxSize, bool cullExcess, int cullRetain, string id, string category)
        {
            GameObject containerGo = new GameObject("Pool [" + prefab.name + "]");
            containerGo.transform.SetParent(transform, false);

            GameObjectPool pool = new GameObjectPool(prefab, containerGo.transform, policy, maxSize, cullExcess)
            {
                Id = id,
                Category = category,
                CullRetain = cullRetain
            };

            _pools[prefab] = pool;
            _all.Add(pool);
            if (!string.IsNullOrEmpty(id)) _poolsById[id] = pool;

            EnsureCullLoop();
            return pool;
        }

        public bool TryGetPool(GameObject prefab, out GameObjectPool pool)
        {
            if (prefab != null) return _pools.TryGetValue(prefab, out pool);
            pool = null;
            return false;
        }

        public GameObjectPool GetPoolById(string id)
        {
            if (!string.IsNullOrEmpty(id) && _poolsById.TryGetValue(id, out GameObjectPool pool)) return pool;
            return null;
        }

        public void PrewarmRegistry(PoolRegistry registry)
        {
            if (registry == null || registry.Pools == null) return;
            StartCoroutine(PrewarmRegistryRoutine(registry));
        }

        IEnumerator PrewarmRegistryRoutine(PoolRegistry registry)
        {
            PoolConfig[] configs = registry.Pools;
            for (int i = 0; i < configs.Length; i++)
            {
                PoolConfig config = configs[i];
                if (config == null || !config.IsValid) continue;

                GameObjectPool pool = CreatePool(config, false);
                if (pool == null) continue;

                int remaining = config.PrewarmCount;
                while (remaining > 0)
                {
                    int batch = remaining < registryPrewarmPerFrame ? remaining : registryPrewarmPerFrame;
                    pool.Prewarm(batch);
                    remaining -= batch;
                    yield return null;
                }
            }
        }

        IEnumerator CullLoop()
        {
            WaitForSeconds wait = new WaitForSeconds(cullCheckInterval);
            while (true)
            {
                yield return wait;
                for (int i = 0; i < _all.Count; i++)
                {
                    GameObjectPool pool = _all[i];
                    if (pool.CullExcess) pool.CullExcessNow();
                }
            }
        }

        public bool TryGetStats(GameObject prefab, out PoolStats stats)
        {
            if (prefab != null && _pools.TryGetValue(prefab, out GameObjectPool pool))
            {
                stats = pool.GetStats();
                return true;
            }
            stats = default;
            return false;
        }

        public void CollectStats(List<PoolStats> buffer)
        {
            if (buffer == null) return;
            buffer.Clear();
            for (int i = 0; i < _all.Count; i++) buffer.Add(_all[i].GetStats());
        }

        public GameObjectPool PoolAt(int index)
        {
            if (index < 0 || index >= _all.Count) return null;
            return _all[index];
        }

        public void ClearPool(GameObject prefab)
        {
            if (prefab != null && _pools.TryGetValue(prefab, out GameObjectPool pool)) pool.Clear();
        }

        public void ClearAll()
        {
            for (int i = 0; i < _all.Count; i++) _all[i].DestroyPool();
            _all.Clear();
            _pools.Clear();
            _poolsById.Clear();
        }
    }
}
