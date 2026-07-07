using System;
using System.Collections.Generic;
using UnityEngine;

namespace CatAnnaDev.Pooling
{
    [DisallowMultipleComponent]
    [AddComponentMenu("CatAnnaDev/Pooling/Pooled Object")]
    public sealed class PooledObject : MonoBehaviour
    {
        static readonly List<IPoolable> SharedBuffer = new List<IPoolable>(8);

        IPoolable[] _poolables;
        bool _cached;
        bool _isActive;
        int _generation;

        internal GameObjectPool OwningPool { get; private set; }

        public bool IsPooled => OwningPool != null;
        public bool IsActive => _isActive;
        public int Generation => _generation;
        public int PoolableCount => _poolables != null ? _poolables.Length : 0;

        internal void Bind(GameObjectPool pool)
        {
            OwningPool = pool;
        }

        public void RefreshPoolables()
        {
            _cached = false;
            EnsureCache();
        }

        void EnsureCache()
        {
            if (_cached) return;
            GetComponentsInChildren<IPoolable>(true, SharedBuffer);
            _poolables = SharedBuffer.Count > 0 ? SharedBuffer.ToArray() : Array.Empty<IPoolable>();
            SharedBuffer.Clear();
            _cached = true;
        }

        internal void HandleSpawn()
        {
            EnsureCache();
            _isActive = true;
            _generation++;
            for (int i = 0; i < _poolables.Length; i++) _poolables[i].OnSpawn();
        }

        internal void HandleDespawn()
        {
            if (!_isActive) return;
            _isActive = false;
            if (_poolables == null) return;
            for (int i = 0; i < _poolables.Length; i++) _poolables[i].OnDespawn();
        }

        public void ReturnToPool()
        {
            if (OwningPool != null)
            {
                OwningPool.Release(this);
            }
            else if (PoolManager.HasInstance)
            {
                PoolManager.Instance.Despawn(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
