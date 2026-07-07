using System.Collections.Generic;
using UnityEngine;

namespace CatAnnaDev.Timers
{
    internal sealed class TimerLifecycle : MonoBehaviour
    {
        private readonly List<Timer> tracked = new List<Timer>(4);

        internal static void Bind(MonoBehaviour owner, Timer timer)
        {
            if (owner == null || timer == null)
            {
                return;
            }

            TimerLifecycle lifecycle = owner.GetComponent<TimerLifecycle>();
            if (lifecycle == null)
            {
                lifecycle = owner.gameObject.AddComponent<TimerLifecycle>();
                lifecycle.hideFlags = HideFlags.HideInInspector;
            }

            lifecycle.tracked.Add(timer);
        }

        private void OnDestroy()
        {
            for (int i = 0; i < tracked.Count; i++)
            {
                Timer timer = tracked[i];
                if (timer != null)
                {
                    timer.Stop();
                }
            }

            tracked.Clear();
        }
    }
}
