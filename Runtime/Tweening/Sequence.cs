using System;
using System.Collections.Generic;

namespace CatAnnaDev.Tweening
{
    public sealed class Sequence
    {
        private enum StepKind { Tween, Wait, Callback }

        private sealed class Step
        {
            public StepKind kind;
            public Func<TweenHandle> factory;
            public float wait;
            public Action callback;
        }

        private readonly List<List<Step>> groups = new List<List<Step>>();
        private readonly List<TweenHandle> activeTweens = new List<TweenHandle>();

        private bool playing;
        private bool paused;
        private bool done;

        private int currentGroup;
        private bool groupStarted;
        private float groupWaitRemaining;

        private int loops = 1;
        private int completedLoops;
        private bool ignoreTimeScale;
        private Action onComplete;

        public static Sequence Create() => new Sequence();

        public bool IsActive => playing && !done;
        public bool IsPlaying => playing && !paused && !done;
        public bool IsPaused => paused;

        public Sequence Append(Func<TweenHandle> tween)
        {
            groups.Add(new List<Step> { new Step { kind = StepKind.Tween, factory = tween } });
            return this;
        }

        public Sequence Join(Func<TweenHandle> tween)
        {
            if (groups.Count == 0) return Append(tween);
            groups[groups.Count - 1].Add(new Step { kind = StepKind.Tween, factory = tween });
            return this;
        }

        public Sequence AppendInterval(float seconds)
        {
            groups.Add(new List<Step> { new Step { kind = StepKind.Wait, wait = seconds } });
            return this;
        }

        public Sequence AppendCallback(Action callback)
        {
            groups.Add(new List<Step> { new Step { kind = StepKind.Callback, callback = callback } });
            return this;
        }

        public Sequence OnComplete(Action callback) { onComplete = callback; return this; }
        public Sequence SetLoops(int count) { loops = count; return this; }
        public Sequence SetTimeScaleIndependent(bool independent = true) { ignoreTimeScale = independent; return this; }

        public Sequence Play()
        {
            if (playing) return this;
            playing = true;
            paused = false;
            done = false;
            currentGroup = 0;
            groupStarted = false;
            completedLoops = 0;
            SequenceRunner.EnsureInstance()?.Add(this);
            return this;
        }

        public Sequence Pause()
        {
            if (!playing || done) return this;
            paused = true;
            for (int i = 0; i < activeTweens.Count; i++) activeTweens[i].Pause();
            return this;
        }

        public Sequence Resume()
        {
            if (!playing || done) return this;
            paused = false;
            for (int i = 0; i < activeTweens.Count; i++) activeTweens[i].Resume();
            return this;
        }

        public void Kill(bool complete = false)
        {
            for (int i = 0; i < activeTweens.Count; i++) activeTweens[i].Kill(complete);
            activeTweens.Clear();
            bool wasRunning = playing && !done;
            done = true;
            playing = false;
            if (complete && wasRunning) onComplete?.Invoke();
        }

        internal bool IgnoreTimeScale => ignoreTimeScale;

        internal bool Tick(float deltaTime)
        {
            if (done) return false;
            if (paused) return true;

            if (currentGroup >= groups.Count)
                return CompleteOrLoop();

            if (!groupStarted)
            {
                StartGroup(groups[currentGroup]);
                groupStarted = true;
            }

            if (groupWaitRemaining > 0f)
                groupWaitRemaining -= deltaTime;

            if (GroupFinished())
            {
                activeTweens.Clear();
                currentGroup++;
                groupStarted = false;
            }

            return !done;
        }

        private void StartGroup(List<Step> group)
        {
            activeTweens.Clear();
            groupWaitRemaining = 0f;
            for (int i = 0; i < group.Count; i++)
            {
                Step s = group[i];
                switch (s.kind)
                {
                    case StepKind.Tween:
                        if (s.factory != null)
                        {
                            TweenHandle handle = s.factory();
                            if (handle.IsActive) activeTweens.Add(handle);
                        }
                        break;
                    case StepKind.Wait:
                        if (s.wait > groupWaitRemaining) groupWaitRemaining = s.wait;
                        break;
                    case StepKind.Callback:
                        s.callback?.Invoke();
                        break;
                }
            }
        }

        private bool GroupFinished()
        {
            if (groupWaitRemaining > 0f) return false;
            for (int i = 0; i < activeTweens.Count; i++)
                if (activeTweens[i].IsActive) return false;
            return true;
        }

        private bool CompleteOrLoop()
        {
            completedLoops++;
            if (loops >= 0 && completedLoops >= loops)
            {
                done = true;
                playing = false;
                onComplete?.Invoke();
                return false;
            }

            currentGroup = 0;
            groupStarted = false;
            return true;
        }
    }
}
