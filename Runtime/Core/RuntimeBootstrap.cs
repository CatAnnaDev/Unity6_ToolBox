using UnityEngine;

namespace CatAnnaDev
{
    internal static class RuntimeBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            CatLog.Verbose(PackVersion.Display + " initialized");
        }
    }
}
