using UnityEngine;

namespace CatAnnaDev.Services
{
    public abstract class SingletonScriptableObject<T> : ScriptableObject where T : SingletonScriptableObject<T>
    {
        private static T instance;

        public static bool HasInstance
        {
            get { return instance != null; }
        }

        public static T Instance
        {
            get
            {
                if (instance != null)
                {
                    return instance;
                }

                instance = LoadFromResources();
                if (instance == null)
                {
                    CatLog.Error("SingletonScriptableObject of type " + typeof(T).Name +
                        " not found in Resources. Expected a single asset at Resources root or Resources/" + typeof(T).Name + ".");
                    return null;
                }

                instance.OnLoaded();
                return instance;
            }
        }

        private static T LoadFromResources()
        {
            T direct = Resources.Load<T>(typeof(T).Name);
            if (direct != null)
            {
                return direct;
            }

            T[] all = Resources.LoadAll<T>(string.Empty);
            if (all != null && all.Length > 0)
            {
                if (all.Length > 1)
                {
                    CatLog.Warn("Multiple SingletonScriptableObject assets of type " + typeof(T).Name +
                        " found in Resources. Using the first one.");
                }

                return all[0];
            }

            return null;
        }

        protected virtual void OnLoaded()
        {
        }
    }
}
