using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace CatAnnaDev.Timers
{
    public static class TimerManager
    {
        private struct CatAnnaDevTimerLoop
        {
        }

        private static readonly List<Timer> Active = new List<Timer>(64);
        private static readonly List<Timer> PendingAdd = new List<Timer>(16);
        private static readonly List<Timer> PendingRemove = new List<Timer>(16);

        private static bool installed;
        private static bool iterating;
        private static TimerUpdater fallback;

        public static int ActiveCount
        {
            get { return Active.Count; }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            Active.Clear();
            PendingAdd.Clear();
            PendingRemove.Clear();
            installed = false;
            iterating = false;
            fallback = null;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Bootstrap()
        {
            Install();
        }

        internal static void Register(Timer timer)
        {
            if (timer == null)
            {
                return;
            }

            EnsureInitialized();

            if (iterating)
            {
                PendingRemove.Remove(timer);
                if (!PendingAdd.Contains(timer))
                {
                    PendingAdd.Add(timer);
                }
                return;
            }

            if (!Active.Contains(timer))
            {
                Active.Add(timer);
            }
        }

        internal static void Unregister(Timer timer)
        {
            if (timer == null)
            {
                return;
            }

            if (iterating)
            {
                PendingAdd.Remove(timer);
                if (!PendingRemove.Contains(timer))
                {
                    PendingRemove.Add(timer);
                }
                return;
            }

            Active.Remove(timer);
        }

        public static void StopAll()
        {
            for (int i = Active.Count - 1; i >= 0; i--)
            {
                Timer timer = Active[i];
                if (timer != null)
                {
                    timer.Stop();
                }
            }

            FlushPending();
        }

        public static void ClearAll()
        {
            Active.Clear();
            PendingAdd.Clear();
            PendingRemove.Clear();
        }

        internal static void Tick()
        {
            float scaled = Time.deltaTime;
            float unscaled = Time.unscaledDeltaTime;

            iterating = true;
            for (int i = 0; i < Active.Count; i++)
            {
                Timer timer = Active[i];
                if (timer != null)
                {
                    timer.Advance(scaled, unscaled);
                }
            }
            iterating = false;

            FlushPending();
        }

        private static void FlushPending()
        {
            if (PendingRemove.Count > 0)
            {
                for (int i = 0; i < PendingRemove.Count; i++)
                {
                    Active.Remove(PendingRemove[i]);
                }
                PendingRemove.Clear();
            }

            if (PendingAdd.Count > 0)
            {
                for (int i = 0; i < PendingAdd.Count; i++)
                {
                    Timer timer = PendingAdd[i];
                    if (!Active.Contains(timer))
                    {
                        Active.Add(timer);
                    }
                }
                PendingAdd.Clear();
            }
        }

        private static void EnsureInitialized()
        {
            if (installed)
            {
                return;
            }

            Install();
        }

        private static void Install()
        {
            if (installed)
            {
                return;
            }

            try
            {
                PlayerLoopSystem root = PlayerLoop.GetCurrentPlayerLoop();
                if (InsertInto(ref root, typeof(Update), OnPlayerLoop))
                {
                    PlayerLoop.SetPlayerLoop(root);
                    installed = true;
                    return;
                }
            }
            catch (Exception exception)
            {
                CatLog.Warn("TimerManager could not insert into the PlayerLoop, using fallback updater. " + exception.Message);
            }

            InstallFallback();
        }

        private static void OnPlayerLoop()
        {
            Tick();
        }

        private static bool InsertInto(ref PlayerLoopSystem loop, Type phaseType, PlayerLoopSystem.UpdateFunction fn)
        {
            if (loop.subSystemList == null)
            {
                return false;
            }

            for (int i = 0; i < loop.subSystemList.Length; i++)
            {
                if (loop.subSystemList[i].type != phaseType)
                {
                    continue;
                }

                PlayerLoopSystem phase = loop.subSystemList[i];
                PlayerLoopSystem[] subs = phase.subSystemList;
                int length = subs != null ? subs.Length : 0;

                PlayerLoopSystem[] extended = new PlayerLoopSystem[length + 1];
                if (subs != null)
                {
                    Array.Copy(subs, extended, length);
                }

                extended[length] = new PlayerLoopSystem
                {
                    type = typeof(CatAnnaDevTimerLoop),
                    updateDelegate = fn
                };

                phase.subSystemList = extended;
                loop.subSystemList[i] = phase;
                return true;
            }

            return false;
        }

        private static void InstallFallback()
        {
            if (installed)
            {
                return;
            }

            if (!Application.isPlaying)
            {
                return;
            }

            if (fallback == null)
            {
                GameObject host = new GameObject("CatAnnaDev.TimerUpdater");
                host.hideFlags = HideFlags.HideAndDontSave;
                UnityEngine.Object.DontDestroyOnLoad(host);
                fallback = host.AddComponent<TimerUpdater>();
            }

            installed = true;
        }

        private sealed class TimerUpdater : MonoBehaviour
        {
            private void Update()
            {
                Tick();
            }
        }
    }
}
