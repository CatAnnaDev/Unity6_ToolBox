using System.Collections.Generic;
using UnityEngine;

namespace CatAnnaDev.Tweening
{
    [AddComponentMenu("")]
    public sealed class SequenceRunner : MonoBehaviour
    {
        private static SequenceRunner instance;
        private static bool quitting;

        private readonly List<Sequence> active = new List<Sequence>(32);

        public static bool HasInstance => instance != null;
        public static int ActiveCount => instance != null ? instance.active.Count : 0;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            instance = null;
            quitting = false;
        }

        internal static SequenceRunner EnsureInstance()
        {
            if (instance != null) return instance;
            if (quitting) return null;

            GameObject go = new GameObject("CatAnnaDev.SequenceRunner");
            go.hideFlags = HideFlags.HideAndDontSave;
            DontDestroyOnLoad(go);
            instance = go.AddComponent<SequenceRunner>();
            return instance;
        }

        internal void Add(Sequence sequence)
        {
            if (!active.Contains(sequence)) active.Add(sequence);
        }

        private void OnApplicationQuit()
        {
            quitting = true;
        }

        private void Update()
        {
            float dt = Time.deltaTime;
            float udt = Time.unscaledDeltaTime;

            int total = active.Count;
            int write = 0;
            for (int i = 0; i < total; i++)
            {
                Sequence s = active[i];
                bool alive = s.Tick(s.IgnoreTimeScale ? udt : dt);
                if (alive) active[write++] = s;
            }

            if (write < total)
                active.RemoveRange(write, total - write);
        }
    }
}
