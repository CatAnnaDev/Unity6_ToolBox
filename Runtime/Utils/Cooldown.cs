using System;
using UnityEngine;

namespace CatAnnaDev.Utils
{
    [Serializable]
    public struct Cooldown
    {
        [SerializeField]
        private float duration;

        private float readyTime;

        public Cooldown(float duration)
        {
            this.duration = duration;
            readyTime = float.NegativeInfinity;
        }

        public float Duration
        {
            get { return duration; }
            set { duration = value; }
        }

        public bool IsReady
        {
            get { return Time.time >= readyTime; }
        }

        public float Remaining
        {
            get
            {
                float remaining = readyTime - Time.time;
                return remaining > 0f ? remaining : 0f;
            }
        }

        public float Progress
        {
            get
            {
                if (duration <= 0f)
                {
                    return 1f;
                }
                float elapsed = duration - Remaining;
                return Mathf.Clamp01(elapsed / duration);
            }
        }

        public void Trigger()
        {
            readyTime = Time.time + duration;
        }

        public void Trigger(float customDuration)
        {
            readyTime = Time.time + customDuration;
        }

        public bool TryTrigger()
        {
            if (!IsReady)
            {
                return false;
            }
            Trigger();
            return true;
        }

        public void Reset()
        {
            readyTime = float.NegativeInfinity;
        }
    }
}
