using System;

namespace CatAnnaDev.Tweening
{
    public readonly struct TweenHandle
    {
        readonly Tween tween;
        readonly int generation;

        internal TweenHandle(Tween t)
        {
            tween = t;
            generation = t != null ? t.generation : 0;
        }

        bool Valid => tween != null && tween.alive && tween.generation == generation;

        public bool IsActive => Valid;

        public bool IsPaused => Valid && tween.isPaused;

        public float Progress => Valid ? tween.NormalizedProgress : 0f;

        public TweenHandle SetEase(Ease ease)
        {
            if (Valid) tween.ease = ease;
            return this;
        }

        public TweenHandle SetDelay(float seconds)
        {
            if (Valid)
            {
                tween.delay = seconds;
                if (!tween.started)
                    tween.delayRemaining = seconds;
            }
            return this;
        }

        public TweenHandle SetLoops(int count, LoopType type = LoopType.Restart)
        {
            if (Valid)
            {
                tween.loops = count;
                tween.loop = count == 1 ? LoopType.None : type;
            }
            return this;
        }

        public TweenHandle SetLoopType(LoopType type)
        {
            if (Valid) tween.loop = type;
            return this;
        }

        public TweenHandle SetTimeScaleIndependent(bool independent = true)
        {
            if (Valid) tween.ignoreTimeScale = independent;
            return this;
        }

        public TweenHandle OnComplete(Action callback)
        {
            if (Valid) tween.onComplete = callback;
            return this;
        }

        public TweenHandle OnUpdate(Action callback)
        {
            if (Valid) tween.onUpdate = callback;
            return this;
        }

        public TweenHandle Pause()
        {
            if (Valid) tween.isPaused = true;
            return this;
        }

        public TweenHandle Resume()
        {
            if (Valid) tween.isPaused = false;
            return this;
        }

        public void Complete()
        {
            if (Valid) tween.ForceComplete();
        }

        public void Kill(bool complete = false)
        {
            if (Valid) tween.Kill(complete);
        }
    }
}
