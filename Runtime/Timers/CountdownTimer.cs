using System;
using UnityEngine;

namespace CatAnnaDev.Timers
{
    public sealed class CountdownTimer : Timer
    {
        private float duration;

        public float Duration
        {
            get { return duration; }
            set { duration = Mathf.Max(0f, value); }
        }

        public bool Loop { get; set; }

        public bool IsFinished { get; private set; }

        public int LoopCount { get; private set; }

        public float Remaining
        {
            get { return CurrentTime; }
        }

        public float Elapsed
        {
            get { return duration - CurrentTime; }
        }

        public override float Progress
        {
            get
            {
                if (duration <= 0f)
                {
                    return 1f;
                }

                return Mathf.Clamp01(1f - CurrentTime / duration);
            }
        }

        public event Action OnComplete;

        public CountdownTimer(float duration, bool useUnscaledTime = false)
            : base(useUnscaledTime)
        {
            this.duration = Mathf.Max(0f, duration);
            Reset();
        }

        public override void Reset()
        {
            CurrentTime = duration;
            IsFinished = false;
            LoopCount = 0;
        }

        protected override void Tick(float delta)
        {
            CurrentTime -= delta;

            while (CurrentTime <= 0f)
            {
                LoopCount++;
                OnComplete?.Invoke();

                if (Loop)
                {
                    if (duration <= 0f)
                    {
                        CurrentTime = 0f;
                        break;
                    }

                    CurrentTime += duration;
                }
                else
                {
                    CurrentTime = 0f;
                    IsFinished = true;
                    Stop();
                    return;
                }
            }
        }
    }
}
