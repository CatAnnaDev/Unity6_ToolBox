using System;
using UnityEngine;

namespace CatAnnaDev.Timers
{
    public sealed class FrequencyTimer : Timer
    {
        private const int MaxTicksPerUpdate = 512;

        private float ticksPerSecond;
        private float interval;
        private float accumulator;

        public float TicksPerSecond
        {
            get { return ticksPerSecond; }
            set { SetFrequency(value); }
        }

        public float Interval
        {
            get { return interval; }
        }

        public int TickCount { get; private set; }

        public override float Progress
        {
            get
            {
                if (interval <= 0f)
                {
                    return 0f;
                }

                return Mathf.Clamp01(accumulator / interval);
            }
        }

        public event Action OnTick;

        public FrequencyTimer(float ticksPerSecond, bool useUnscaledTime = false)
            : base(useUnscaledTime)
        {
            SetFrequency(ticksPerSecond);
        }

        public void SetFrequency(float value)
        {
            if (value <= 0f)
            {
                CatLog.Warn("FrequencyTimer requires a positive ticks-per-second value; clamping to 1.");
                value = 1f;
            }

            ticksPerSecond = value;
            interval = 1f / value;
        }

        public override void Reset()
        {
            base.Reset();
            accumulator = 0f;
            TickCount = 0;
        }

        protected override void Tick(float delta)
        {
            CurrentTime += delta;
            accumulator += delta;

            int guard = 0;
            while (accumulator >= interval)
            {
                accumulator -= interval;
                TickCount++;
                OnTick?.Invoke();

                if (++guard >= MaxTicksPerUpdate)
                {
                    accumulator = 0f;
                    break;
                }
            }
        }
    }
}
