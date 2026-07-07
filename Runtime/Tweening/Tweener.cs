using System;
using UnityEngine;

namespace CatAnnaDev.Tweening
{
    public static class Tweener
    {
        public static TweenHandle To(float from, float to, float duration, Action<float> setter)
            => TweenManager.EnsureInstance().CreateFloat(null, from, to, duration, setter);

        public static TweenHandle To(Func<float> getter, Action<float> setter, float to, float duration)
            => TweenManager.EnsureInstance().CreateFloat(null, getter != null ? getter() : 0f, to, duration, setter);

        public static TweenHandle To(Vector2 from, Vector2 to, float duration, Action<Vector2> setter)
            => TweenManager.EnsureInstance().CreateVector2(null, from, to, duration, setter);

        public static TweenHandle To(Vector3 from, Vector3 to, float duration, Action<Vector3> setter)
            => TweenManager.EnsureInstance().CreateVector3(null, from, to, duration, setter);

        public static TweenHandle To(Quaternion from, Quaternion to, float duration, Action<Quaternion> setter)
            => TweenManager.EnsureInstance().CreateQuaternion(null, from, to, duration, setter);

        public static TweenHandle To(Color from, Color to, float duration, Action<Color> setter)
            => TweenManager.EnsureInstance().CreateColor(null, from, to, duration, setter);

        public static TweenHandle Callback(float duration, Action<float> onEval)
            => TweenManager.EnsureInstance().CreateCallback(null, duration, onEval);

        public static void Kill(UnityEngine.Object owner, bool complete = false)
            => TweenManager.KillTweensOf(owner, complete);

        public static void KillAll(bool complete = false)
            => TweenManager.KillAll(complete);
    }
}
