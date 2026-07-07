using System.Collections.Generic;
using UnityEngine;

namespace CatAnnaDev.Utils
{
    public static class WaitCache
    {
        private static readonly Dictionary<float, WaitForSeconds> Seconds = new Dictionary<float, WaitForSeconds>();
        private static readonly Dictionary<float, WaitForSecondsRealtime> RealtimeSeconds = new Dictionary<float, WaitForSecondsRealtime>();

        private static readonly WaitForFixedUpdate FixedUpdateInstance = new WaitForFixedUpdate();
        private static readonly WaitForEndOfFrame EndOfFrameInstance = new WaitForEndOfFrame();

        public static WaitForSeconds ForSeconds(float seconds)
        {
            if (!Seconds.TryGetValue(seconds, out WaitForSeconds wait))
            {
                wait = new WaitForSeconds(seconds);
                Seconds[seconds] = wait;
            }
            return wait;
        }

        public static WaitForSecondsRealtime ForSecondsRealtime(float seconds)
        {
            if (!RealtimeSeconds.TryGetValue(seconds, out WaitForSecondsRealtime wait))
            {
                wait = new WaitForSecondsRealtime(seconds);
                RealtimeSeconds[seconds] = wait;
            }
            return wait;
        }

        public static WaitForFixedUpdate FixedUpdate
        {
            get { return FixedUpdateInstance; }
        }

        public static WaitForEndOfFrame EndOfFrame
        {
            get { return EndOfFrameInstance; }
        }

        public static void Clear()
        {
            Seconds.Clear();
            RealtimeSeconds.Clear();
        }
    }
}
