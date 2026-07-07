using System;
using System.Collections.Generic;

namespace CatAnnaDev.Pooling
{
    public class LinkedPool<T> : IObjectPool<T>, IDisposable where T : class
    {
        sealed class LinkedNode
        {
            public LinkedNode next;
            public T value;
        }

        readonly Func<T> _createFunc;
        readonly Action<T> _onGet;
        readonly Action<T> _onRelease;
        readonly Action<T> _onDestroy;
        readonly int _maxSize;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        readonly HashSet<T> _pooledSet;
#endif

        LinkedNode _freeList;
        LinkedNode _nodePool;

        int _countActive;
        int _countInactive;
        int _peakActive;
        int _totalSpawned;
        int _totalReleased;
        int _misses;

        public LinkedPool(
            Func<T> createFunc,
            Action<T> onGet = null,
            Action<T> onRelease = null,
            Action<T> onDestroy = null,
            int maxSize = 10000,
            bool collectionCheck = true)
        {
            if (createFunc == null) throw new ArgumentNullException(nameof(createFunc));
            if (maxSize < 0) maxSize = 0;

            _createFunc = createFunc;
            _onGet = onGet;
            _onRelease = onRelease;
            _onDestroy = onDestroy;
            _maxSize = maxSize;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (collectionCheck) _pooledSet = new HashSet<T>();
#endif
        }

        public int CountActive => _countActive;
        public int CountInactive => _countInactive;
        public int CountAll => _countActive + _countInactive;
        public int MaxSize => _maxSize;

        public T Get()
        {
            T item;
            if (_freeList == null)
            {
                item = _createFunc();
                _misses++;
            }
            else
            {
                LinkedNode node = _freeList;
                item = node.value;
                _freeList = node.next;
                _countInactive--;

                node.value = null;
                node.next = _nodePool;
                _nodePool = node;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
                _pooledSet?.Remove(item);
#endif
            }

            _countActive++;
            if (_countActive > _peakActive) _peakActive = _countActive;
            _totalSpawned++;
            _onGet?.Invoke(item);
            return item;
        }

        public void Release(T item)
        {
            if (item == null) return;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (_pooledSet != null && _pooledSet.Contains(item))
            {
                CatLog.Error("LinkedPool<" + typeof(T).Name + ">: element released more than once.");
                return;
            }
#endif

            _onRelease?.Invoke(item);
            if (_countActive > 0) _countActive--;
            _totalReleased++;

            if (_maxSize > 0 && _countInactive >= _maxSize)
            {
                _onDestroy?.Invoke(item);
                return;
            }

            LinkedNode node = _nodePool;
            if (node == null)
            {
                node = new LinkedNode();
            }
            else
            {
                _nodePool = node.next;
            }

            node.value = item;
            node.next = _freeList;
            _freeList = node;
            _countInactive++;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            _pooledSet?.Add(item);
#endif
        }

        public void Prewarm(int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (_maxSize > 0 && _countInactive >= _maxSize) break;

                T item = _createFunc();
                LinkedNode node = _nodePool;
                if (node == null) node = new LinkedNode();
                else _nodePool = node.next;

                node.value = item;
                node.next = _freeList;
                _freeList = node;
                _countInactive++;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
                _pooledSet?.Add(item);
#endif
            }
        }

        public void Clear()
        {
            LinkedNode node = _freeList;
            while (node != null)
            {
                LinkedNode next = node.next;
                _onDestroy?.Invoke(node.value);
                node.value = null;
                node.next = _nodePool;
                _nodePool = node;
                node = next;
            }

            _freeList = null;
            _countInactive = 0;
            _countActive = 0;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            _pooledSet?.Clear();
#endif
        }

        public PoolStats GetStats()
        {
            PoolStats s;
            s.active = _countActive;
            s.inactive = _countInactive;
            s.total = _countActive + _countInactive;
            s.peakActive = _peakActive;
            s.totalSpawned = _totalSpawned;
            s.totalReleased = _totalReleased;
            s.misses = _misses;
            return s;
        }

        public void Dispose()
        {
            Clear();
        }
    }
}
