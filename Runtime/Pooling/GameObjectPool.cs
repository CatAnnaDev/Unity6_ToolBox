using System.Collections.Generic;
using UnityEngine;

namespace CatAnnaDev.Pooling
{
    public sealed class GameObjectPool
    {
        readonly GameObject _prefab;
        readonly Transform _container;
        readonly Stack<PooledObject> _inactive;
        readonly Vector3 _prefabScale;

        ExpandPolicy _expandPolicy;
        int _maxSize;

        int _countActive;
        int _peakActive;
        int _totalSpawned;
        int _totalReleased;
        int _misses;

        public GameObjectPool(GameObject prefab, Transform container, ExpandPolicy expandPolicy, int maxSize, bool cullExcess)
        {
            _prefab = prefab;
            _container = container;
            _expandPolicy = expandPolicy;
            _maxSize = maxSize < 0 ? 0 : maxSize;
            _inactive = new Stack<PooledObject>(32);
            _prefabScale = prefab != null ? prefab.transform.localScale : Vector3.one;

            CullExcess = cullExcess;
            CullRetain = 0;
        }

        public GameObject Prefab => _prefab;
        public Transform Container => _container;
        public string Id { get; set; }
        public string Category { get; set; }

        public bool CullExcess { get; set; }
        public int CullRetain { get; set; }

        public int CountActive => _countActive;
        public int CountInactive => _inactive.Count;
        public int CountAll => _countActive + _inactive.Count;

        public ExpandPolicy ExpandPolicy
        {
            get => _expandPolicy;
            set => _expandPolicy = value;
        }

        public int MaxSize
        {
            get => _maxSize;
            set => _maxSize = value < 0 ? 0 : value;
        }

        public GameObject Get(Vector3 position, Quaternion rotation, Transform parent)
        {
            PooledObject po = TakeOrCreate();
            if (po == null) return null;

            Transform t = po.transform;
            t.SetParent(parent != null ? parent : _container, false);
            t.SetPositionAndRotation(position, rotation);
            t.localScale = _prefabScale;

            GameObject go = po.gameObject;
            if (!go.activeSelf) go.SetActive(true);

            _countActive++;
            if (_countActive > _peakActive) _peakActive = _countActive;
            _totalSpawned++;

            po.HandleSpawn();
            return go;
        }

        public void Release(GameObject instance)
        {
            if (instance == null) return;
            if (instance.TryGetComponent(out PooledObject po))
            {
                Release(po);
            }
            else
            {
                CatLog.Warn("GameObjectPool: released instance without PooledObject, destroying.", instance);
                Object.Destroy(instance);
            }
        }

        public void Release(PooledObject po)
        {
            if (po == null) return;

            if (!po.IsActive)
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                CatLog.Warn("GameObjectPool: double release of '" + po.name + "' ignored.", po);
#endif
                return;
            }

            po.HandleDespawn();

            GameObject go = po.gameObject;
            if (go.activeSelf) go.SetActive(false);
            po.transform.SetParent(_container, false);

            if (_countActive > 0) _countActive--;
            _totalReleased++;

            if (_maxSize > 0 && _inactive.Count >= _maxSize)
            {
                DestroyInstance(po);
            }
            else
            {
                _inactive.Push(po);
            }
        }

        public void Prewarm(int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (AtCeiling()) break;
                PooledObject po = CreateInstance(true);
                _inactive.Push(po);
            }
        }

        public int CullExcessNow()
        {
            int keep = CullRetain < 0 ? 0 : CullRetain;
            int destroyed = 0;
            while (_inactive.Count > keep)
            {
                DestroyInstance(_inactive.Pop());
                destroyed++;
            }
            return destroyed;
        }

        public void Clear()
        {
            while (_inactive.Count > 0) DestroyInstance(_inactive.Pop());
        }

        public void DestroyPool()
        {
            Clear();
            if (_container != null) Object.Destroy(_container.gameObject);
        }

        public PoolStats GetStats()
        {
            PoolStats s;
            s.active = _countActive;
            s.inactive = _inactive.Count;
            s.total = _countActive + _inactive.Count;
            s.peakActive = _peakActive;
            s.totalSpawned = _totalSpawned;
            s.totalReleased = _totalReleased;
            s.misses = _misses;
            return s;
        }

        PooledObject TakeOrCreate()
        {
            while (_inactive.Count > 0)
            {
                PooledObject po = _inactive.Pop();
                if (po != null) return po;
            }
            return Expand();
        }

        PooledObject Expand()
        {
            _misses++;
            int ceiling = Ceiling();

            switch (_expandPolicy.mode)
            {
                case ExpandMode.Fixed:
                    if (ceiling > 0 && (_countActive + _inactive.Count) >= ceiling) return null;
                    return CreateInstance(false);

                case ExpandMode.GrowByChunk:
                {
                    int chunk = _expandPolicy.chunkSize < 1 ? 1 : _expandPolicy.chunkSize;
                    PooledObject first = CreateInstance(false);
                    for (int i = 1; i < chunk; i++)
                    {
                        if (ceiling > 0 && (_countActive + _inactive.Count + 1) >= ceiling) break;
                        _inactive.Push(CreateInstance(true));
                    }
                    return first;
                }

                default:
                    return CreateInstance(false);
            }
        }

        PooledObject CreateInstance(bool startInactive)
        {
            GameObject go = Object.Instantiate(_prefab, _container);
            go.name = _prefab.name;

            if (!go.TryGetComponent(out PooledObject po)) po = go.AddComponent<PooledObject>();
            po.Bind(this);

            if (startInactive && go.activeSelf) go.SetActive(false);
            return po;
        }

        void DestroyInstance(PooledObject po)
        {
            if (po == null) return;
            Object.Destroy(po.gameObject);
        }

        bool AtCeiling()
        {
            int ceiling = Ceiling();
            return ceiling > 0 && (_countActive + _inactive.Count) >= ceiling;
        }

        int Ceiling()
        {
            if (_expandPolicy.hardCeiling > 0) return _expandPolicy.hardCeiling;
            return _maxSize;
        }
    }
}
