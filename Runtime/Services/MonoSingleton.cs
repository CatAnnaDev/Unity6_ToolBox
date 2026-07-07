using UnityEngine;

namespace CatAnnaDev.Services
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T instance;
        private static bool isQuitting;
        private static readonly object initLock = new object();
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

        private static void SyncSession()
        {
            if (sessionId == SingletonSession.Id)
            {
                return;
            }

            sessionId = SingletonSession.Id;
            instance = null;
            isQuitting = false;
        }

        protected virtual void Awake()
        {
            SyncSession();
            if (instance != null && instance != this)
            {
                CatLog.Warn("Duplicate MonoSingleton of type " + typeof(T).Name + " destroyed.", this);
                Destroy(gameObject);
                return;
            }

            instance = (T)this;
            isQuitting = false;
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
