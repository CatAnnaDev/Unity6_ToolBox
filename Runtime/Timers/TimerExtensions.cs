using System;
using UnityEngine;

namespace CatAnnaDev.Timers
{
    public static class TimerExtensions
    {
        public static CountdownTimer CountdownTimer(this MonoBehaviour owner, float duration, Action onComplete = null, bool loop = false, bool useUnscaledTime = false, bool autoStopOnDestroy = true)
        {
            CountdownTimer timer = new CountdownTimer(duration, useUnscaledTime)
            {
                Loop = loop
            };

            if (onComplete != null)
            {
                timer.OnComplete += onComplete;
            }

            if (autoStopOnDestroy)
            {
                TimerLifecycle.Bind(owner, timer);
            }

            timer.Start();
            return timer;
        }

        public static CountdownTimer Delay(this MonoBehaviour owner, float seconds, Action action, bool useUnscaledTime = false, bool autoStopOnDestroy = true)
        {
            return owner.CountdownTimer(seconds, action, false, useUnscaledTime, autoStopOnDestroy);
        }

        public static CountdownTimer Repeat(this MonoBehaviour owner, float interval, Action action, bool useUnscaledTime = false, bool autoStopOnDestroy = true)
        {
            return owner.CountdownTimer(interval, action, true, useUnscaledTime, autoStopOnDestroy);
        }

        public static StopwatchTimer Stopwatch(this MonoBehaviour owner, bool useUnscaledTime = false, bool autoStopOnDestroy = true)
        {
            StopwatchTimer timer = new StopwatchTimer(useUnscaledTime);

            if (autoStopOnDestroy)
            {
                TimerLifecycle.Bind(owner, timer);
            }

            timer.Start();
            return timer;
        }

        public static FrequencyTimer Frequency(this MonoBehaviour owner, float ticksPerSecond, Action onTick = null, bool useUnscaledTime = false, bool autoStopOnDestroy = true)
        {
            FrequencyTimer timer = new FrequencyTimer(ticksPerSecond, useUnscaledTime);

            if (onTick != null)
            {
                timer.OnTick += onTick;
            }

            if (autoStopOnDestroy)
            {
                TimerLifecycle.Bind(owner, timer);
            }

            timer.Start();
            return timer;
        }

        public static T BindTo<T>(this T timer, MonoBehaviour owner) where T : Timer
        {
            TimerLifecycle.Bind(owner, timer);
            return timer;
        }

        public static T Started<T>(this T timer) where T : Timer
        {
            timer.Start();
            return timer;
        }
    }
}
