using UnityEngine;

namespace CatAnnaDev.Services
{
    public abstract class PersistentSingleton<T> : MonoBehaviour where T : PersistentSingleton<T>
    {
        private const string PersistentRootName = "CatAnnaDev.Persistent";

        private static T instance;
        private static bool isQuitting;
        private static readonly object initLock = new object();
        private static Transform persistentRoot;
        private static int sessionId = -1;

        public static bool HasInstance
        {
            get
            {
                SyncSession();
                return instance != null && !isQuitting;
            }
        }

        public static T InstanceOrNull
        {
            get
            {
                SyncSession();
                return isQuitting ? null : instance;
            }
        }

        public static T Instance
        {
            get
            {
                SyncSession();
                if (isQuitting)
                {
                    return null;
                }

                if (instance != null)
                {
                    return instance;
                }

                lock (initLock)
                {
                    if (instance != null)
                    {
                        return instance;
                    }

                    instance = FindExisting();
                    if (instance == null)
                    {
                        instance = CreateInstance();
                    }

                    return instance;
                }
            }
        }

        private static void SyncSession()
        {
            if (sessionId == SingletonSession.Id)
            {
                return;
            }

            sessionId = SingletonSession.Id;
            instance = null;
            isQuitting = false;
            persistentRoot = null;
        }

        private static T FindExisting()
        {
#if UNITY_2023_1_OR_NEWER
            return Object.FindFirstObjectByType<T>();
#else
            return Object.FindObjectOfType<T>();
#endif
        }

        private static T CreateInstance()
        {
            GameObject holder = new GameObject(typeof(T).Name);
            T created = holder.AddComponent<T>();
            return created;
        }

        private static Transform GetPersistentRoot()
        {
            if (persistentRoot != null)
            {
                return persistentRoot;
            }

            GameObject existing = GameObject.Find(PersistentRootName);
            if (existing == null)
            {
                existing = new GameObject(PersistentRootName);
                DontDestroyOnLoad(existing);
            }

            persistentRoot = existing.transform;
            return persistentRoot;
        }

        protected virtual void Awake()
        {
            SyncSession();
            if (instance != null && instance != this)
            {
                CatLog.Warn("Duplicate PersistentSingleton of type " + typeof(T).Name + " destroyed.", this);
                Destroy(gameObject);
                return;
            }

            instance = (T)this;
            isQuitting = false;

            Transform root = GetPersistentRoot();
            if (transform.parent != root)
            {
                transform.SetParent(root, true);
            }

            DontDestroyOnLoad(gameObject);
            OnSingletonAwake();
        }

        protected virtual void OnSingletonAwake()
        {
        }

        protected virtual void OnApplicationQuit()
        {
            isQuitting = true;
        }

        protected virtual void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }
    }
}
