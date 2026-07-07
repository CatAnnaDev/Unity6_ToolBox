using System;
using UnityEngine;

namespace CatAnnaDev.Timers
{
    public enum TimerState
    {
        Stopped,
        Running,
        Paused
    }

    public abstract class Timer : IDisposable
    {
        public float CurrentTime { get; protected set; }

        public bool UseUnscaledTime { get; set; }

        public TimerState State { get; private set; }

        public bool IsRunning
        {
            get { return State == TimerState.Running; }
        }

        public bool IsPaused
        {
            get { return State == TimerState.Paused; }
        }

        public bool IsStopped
        {
            get { return State == TimerState.Stopped; }
        }

        public virtual float Progress
        {
            get { return 0f; }
        }

        public event Action OnStart;
        public event Action OnStop;

        protected Timer(bool useUnscaledTime = false)
        {
            UseUnscaledTime = useUnscaledTime;
            State = TimerState.Stopped;
            Reset();
        }

        public void Start()
        {
            if (State == TimerState.Running)
            {
                return;
            }

            Reset();
            State = TimerState.Running;
            TimerManager.Register(this);
            OnStart?.Invoke();
        }

        public void Stop()
        {
            if (State == TimerState.Stopped)
            {
                return;
            }

            State = TimerState.Stopped;
            TimerManager.Unregister(this);
            OnStop?.Invoke();
        }

        public void Restart()
        {
            Stop();
            Start();
        }

        public void Pause()
        {
            if (State != TimerState.Running)
            {
                return;
            }

            State = TimerState.Paused;
        }

        public void Resume()
        {
            if (State != TimerState.Paused)
            {
                return;
            }

            State = TimerState.Running;
        }

        public void Toggle()
        {
            if (State == TimerState.Running)
            {
                Pause();
            }
            else if (State == TimerState.Paused)
            {
                Resume();
            }
            else
            {
                Start();
            }
        }

        public virtual void Reset()
        {
            CurrentTime = 0f;
        }

        internal void Advance(float scaledDelta, float unscaledDelta)
        {
            if (State != TimerState.Running)
            {
                return;
            }

            Tick(UseUnscaledTime ? unscaledDelta : scaledDelta);
        }

        protected abstract void Tick(float delta);

        public void Dispose()
        {
            Stop();
        }
    }
}
