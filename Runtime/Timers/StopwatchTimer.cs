using System;
using UnityEngine;

namespace CatAnnaDev.Timers
{
    public sealed class StopwatchTimer : Timer
    {
        private float targetTime;

        public float TargetTime
        {
            get { return targetTime; }
            set { targetTime = Mathf.Max(0f, value); }
        }

        public float Elapsed
        {
            get { return CurrentTime; }
        }

        public override float Progress
        {
            get
            {
                if (targetTime <= 0f)
                {
                    return 0f;
                }

                return Mathf.Clamp01(CurrentTime / targetTime);
            }
        }

        public event Action OnTargetReached;

        public StopwatchTimer(bool useUnscaledTime = false)
            : base(useUnscaledTime)
        {
        }

        public StopwatchTimer(float targetTime, bool useUnscaledTime = false)
            : base(useUnscaledTime)
        {
            this.targetTime = Mathf.Max(0f, targetTime);
        }

        public float Lap()
        {
            float value = CurrentTime;
            CurrentTime = 0f;
            return value;
        }

        protected override void Tick(float delta)
        {
            CurrentTime += delta;

            if (targetTime > 0f && CurrentTime >= targetTime)
            {
                CurrentTime = targetTime;
                OnTargetReached?.Invoke();
                Stop();
            }
        }
    }
}
