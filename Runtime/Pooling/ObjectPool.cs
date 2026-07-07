using System;
using System.Collections.Generic;

namespace CatAnnaDev.Pooling
{
    public class ObjectPool<T> : IObjectPool<T>, IDisposable where T : class
    {
        readonly Stack<T> _stack;
        readonly Func<T> _createFunc;
        readonly Action<T> _onGet;
        readonly Action<T> _onRelease;
        readonly Action<T> _onDestroy;
        readonly int _maxSize;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        readonly HashSet<T> _pooledSet;
#endif

        int _countActive;
        int _peakActive;
        int _totalSpawned;
        int _totalReleased;
        int _misses;

        public ObjectPool(
            Func<T> createFunc,
            Action<T> onGet = null,
            Action<T> onRelease = null,
            Action<T> onDestroy = null,
            int defaultCapacity = 16,
            int maxSize = 10000,
            bool collectionCheck = true)
        {
            if (createFunc == null) throw new ArgumentNullException(nameof(createFunc));
            if (defaultCapacity < 0) defaultCapacity = 0;
            if (maxSize < 0) maxSize = 0;

            _createFunc = createFunc;
            _onGet = onGet;
            _onRelease = onRelease;
            _onDestroy = onDestroy;
            _maxSize = maxSize;
            _stack = new Stack<T>(defaultCapacity);

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (collectionCheck) _pooledSet = new HashSet<T>();
#endif
        }

        public int CountActive => _countActive;
        public int CountInactive => _stack.Count;
        public int CountAll => _countActive + _stack.Count;
        public int MaxSize => _maxSize;

        public T Get()
        {
            T element;
            if (_stack.Count == 0)
            {
                element = _createFunc();
                _misses++;
            }
            else
            {
                element = _stack.Pop();
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                _pooledSet?.Remove(element);
#endif
            }

            _countActive++;
            if (_countActive > _peakActive) _peakActive = _countActive;
            _totalSpawned++;
            _onGet?.Invoke(element);
            return element;
        }

        public PooledHandle<T> GetScoped(out T element)
        {
            element = Get();
            return new PooledHandle<T>(this, element);
        }

        public void Release(T element)
        {
            if (element == null) return;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (_pooledSet != null && _pooledSet.Contains(element))
            {
                CatLog.Error("ObjectPool<" + typeof(T).Name + ">: element released more than once.");
                return;
            }
#endif

            _onRelease?.Invoke(element);
            if (_countActive > 0) _countActive--;
            _totalReleased++;

            if (_maxSize > 0 && _stack.Count >= _maxSize)
            {
                _onDestroy?.Invoke(element);
            }
            else
            {
                _stack.Push(element);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                _pooledSet?.Add(element);
#endif
            }
        }

        public void Prewarm(int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (_maxSize > 0 && _stack.Count >= _maxSize) break;
                T element = _createFunc();
                _stack.Push(element);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                _pooledSet?.Add(element);
#endif
            }
        }

        public void Clear()
        {
            if (_onDestroy != null)
            {
                while (_stack.Count > 0) _onDestroy(_stack.Pop());
            }

            _stack.Clear();
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            _pooledSet?.Clear();
#endif
            _countActive = 0;
        }

        public PoolStats GetStats()
        {
            PoolStats s;
            s.active = _countActive;
            s.inactive = _stack.Count;
            s.total = _countActive + _stack.Count;
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

    public readonly struct PooledHandle<T> : IDisposable where T : class
    {
        readonly ObjectPool<T> _pool;
        readonly T _value;

        public PooledHandle(ObjectPool<T> pool, T value)
        {
            _pool = pool;
            _value = value;
        }

        public T Value => _value;

        public void Dispose()
        {
            if (_pool != null && _value != null) _pool.Release(_value);
        }
    }
}
