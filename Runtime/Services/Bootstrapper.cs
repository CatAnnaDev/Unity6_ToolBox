using UnityEngine;

namespace CatAnnaDev.Services
{
    [DefaultExecutionOrder(-10000)]
    public abstract class Bootstrapper : MonoBehaviour
    {
        private const string SystemsPrefabResourcePath = "Systems";

        private static bool systemsSpawned;

        [SerializeField] private bool bootstrapOnAwake = true;

        protected virtual void Awake()
        {
            if (bootstrapOnAwake)
            {
                Bootstrap();
            }
        }

        public abstract void Bootstrap();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            systemsSpawned = false;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void SpawnSystems()
        {
            if (systemsSpawned)
            {
                return;
            }

            GameObject prefab = Resources.Load<GameObject>(SystemsPrefabResourcePath);
            if (prefab == null)
            {
                return;
            }

            GameObject instance = Object.Instantiate(prefab);
            instance.name = prefab.name;
            Object.DontDestroyOnLoad(instance);
            systemsSpawned = true;

            CatLog.Info("Bootstrapper spawned Systems prefab from Resources/" + SystemsPrefabResourcePath + ".");
        }
    }
}
