using UnityEngine;

namespace CatAnnaDev.Pooling
{
    public static class Pool
    {
        public static GameObject Spawn(GameObject prefab)
        {
            return PoolManager.Instance.Spawn(prefab);
        }

        public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            return PoolManager.Instance.Spawn(prefab, position, rotation, parent);
        }

        public static T Spawn<T>(T prefab) where T : Component
        {
            if (prefab == null) return null;
            GameObject go = PoolManager.Instance.Spawn(prefab.gameObject);
            return go != null ? go.GetComponent<T>() : null;
        }

        public static T Spawn<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent = null) where T : Component
        {
            if (prefab == null) return null;
            GameObject go = PoolManager.Instance.Spawn(prefab.gameObject, position, rotation, parent);
            return go != null ? go.GetComponent<T>() : null;
        }

        public static void Despawn(GameObject instance)
        {
            if (instance == null) return;
            PoolManager.Instance.Despawn(instance);
        }

        public static void Despawn(GameObject instance, float delay)
        {
            if (instance == null) return;
            PoolManager.Instance.Despawn(instance, delay);
        }

        public static void Despawn<T>(T instance) where T : Component
        {
            if (instance != null) Despawn(instance.gameObject);
        }

        public static void Prewarm(GameObject prefab, int count)
        {
            if (prefab == null || count <= 0) return;
            PoolManager.Instance.Prewarm(prefab, count);
        }

        public static void ClearAll()
        {
            if (PoolManager.HasInstance) PoolManager.Instance.ClearAll();
        }
    }
}
