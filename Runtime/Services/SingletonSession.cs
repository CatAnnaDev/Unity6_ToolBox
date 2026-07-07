using UnityEngine;

namespace CatAnnaDev.Services
{
    internal static class SingletonSession
    {
        private static int id;

        public static int Id
        {
            get { return id; }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void BeginSession()
        {
            id++;
        }
    }
}
