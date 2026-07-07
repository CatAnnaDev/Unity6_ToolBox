using System;
using UnityEngine;

namespace CatAnnaDev.Tweening
{
    internal enum TweenValueKind
    {
        Float,
        Vector2,
        Vector3,
        Quaternion,
        Color,
        Callback
    }

    public sealed class Tween
    {
        internal int generation;
        internal bool alive;
        internal bool isPaused;
        internal bool started;
        internal bool reversed;
        internal bool ignoreTimeScale;

        internal TweenValueKind kind;
        internal UnityEngine.Object target;
        internal bool hasTarget;

        internal Vector4 fromV4;
        internal Vector4 toV4;
        internal Quaternion fromQ;
        internal Quaternion toQ;

        internal Delegate setter;
        internal Action<float> eval;
        internal Action onComplete;
        internal Action onUpdate;

        internal float duration;
        internal float delay;
        internal float delayRemaining;
        internal float elapsed;

        internal Ease ease;
        internal LoopType loop;
        internal int loops;
        internal int completedCycles;

        internal void ResetState()
        {
            generation++;
            alive = true;
            isPaused = false;
            started = false;
            reversed = false;
            ignoreTimeScale = false;
            target = null;
            hasTarget = false;
            fromV4 = Vector4.zero;
            toV4 = Vector4.zero;
            fromQ = Quaternion.identity;
            toQ = Quaternion.identity;
            setter = null;
            eval = null;
            onComplete = null;
            onUpdate = null;
            duration = 0f;
            delay = 0f;
            delayRemaining = 0f;
            elapsed = 0f;
            ease = Ease.Linear;
            loop = LoopType.None;
            loops = 1;
            completedCycles = 0;
        }

        internal void ClearReferences()
        {
            setter = null;
            eval = null;
            onComplete = null;
            onUpdate = null;
            target = null;
            hasTarget = false;
        }

        internal void SetTarget(UnityEngine.Object owner)
        {
            target = owner;
            hasTarget = owner != null;
        }

        internal float NormalizedProgress
        {
            get
            {
                if (duration <= 0f) return 1f;
                float p = elapsed / duration;
                if (p < 0f) p = 0f;
                else if (p > 1f) p = 1f;
                return reversed ? 1f - p : p;
            }
        }

        internal void Step(float dt)
        {
            if (!alive || isPaused) return;

            if (hasTarget && target == null)
            {
                alive = false;
                return;
            }

            if (delayRemaining > 0f)
            {
                delayRemaining -= dt;
                if (delayRemaining > 0f) return;
                dt = -delayRemaining;
                delayRemaining = 0f;
            }

            if (!started)
            {
                started = true;
                Apply(reversed ? 1f : 0f);
            }

            elapsed += dt;
            float p = duration <= 0f ? 1f : elapsed / duration;
            bool cycleDone = p >= 1f;
            if (cycleDone) p = 1f;

            Apply(reversed ? 1f - p : p);
            onUpdate?.Invoke();

            if (!cycleDone) return;

            completedCycles++;
            if (loops >= 0 && completedCycles >= loops)
            {
                Finish();
                return;
            }

            elapsed -= duration;
            if (elapsed < 0f) elapsed = 0f;

            switch (loop)
            {
                case LoopType.Restart:
                    reversed = false;
                    break;
                case LoopType.PingPong:
                    reversed = !reversed;
                    break;
                case LoopType.Incremental:
                    ApplyIncrement();
                    reversed = false;
                    break;
                default:
                    Finish();
                    break;
            }
        }

        internal void Apply(float sample)
        {
            float eased = Easing.Evaluate(ease, sample);
            switch (kind)
            {
                case TweenValueKind.Float:
                    ((Action<float>)setter)(Mathf.LerpUnclamped(fromV4.x, toV4.x, eased));
                    break;
                case TweenValueKind.Vector2:
                    ((Action<Vector2>)setter)(Vector2.LerpUnclamped(fromV4, toV4, eased));
                    break;
                case TweenValueKind.Vector3:
                    ((Action<Vector3>)setter)(Vector3.LerpUnclamped(fromV4, toV4, eased));
                    break;
                case TweenValueKind.Quaternion:
                    ((Action<Quaternion>)setter)(Quaternion.SlerpUnclamped(fromQ, toQ, eased));
                    break;
                case TweenValueKind.Color:
                    ((Action<Color>)setter)(Color.LerpUnclamped(fromV4, toV4, eased));
                    break;
                case TweenValueKind.Callback:
                    eval?.Invoke(eased);
                    break;
            }
        }

        void ApplyIncrement()
        {
            switch (kind)
            {
                case TweenValueKind.Float:
                {
                    float d = toV4.x - fromV4.x;
                    fromV4.x = toV4.x;
                    toV4.x += d;
                    break;
                }
                case TweenValueKind.Vector2:
                case TweenValueKind.Vector3:
                case TweenValueKind.Color:
                {
                    Vector4 d = toV4 - fromV4;
                    fromV4 = toV4;
                    toV4 += d;
                    break;
                }
                case TweenValueKind.Quaternion:
                {
                    Quaternion d = Quaternion.Inverse(fromQ) * toQ;
                    fromQ = toQ;
                    toQ = toQ * d;
                    break;
                }
            }
        }

        void Finish()
        {
            Action cb = onComplete;
            alive = false;
            cb?.Invoke();
        }

        internal void ForceComplete()
        {
            if (!alive) return;
            reversed = false;
            Apply(1f);
            Action cb = onComplete;
            alive = false;
            cb?.Invoke();
        }

        internal void Kill(bool complete)
        {
            if (!alive) return;
            if (complete) ForceComplete();
            else alive = false;
        }
    }
}
