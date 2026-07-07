using System;
using System.Collections.Generic;
using UnityEngine;

namespace CatAnnaDev.Tweening
{
    [AddComponentMenu("")]
    public sealed class TweenManager : MonoBehaviour
    {
        static TweenManager instance;
        static bool quitting;

        readonly List<Tween> active = new List<Tween>(64);
        readonly Stack<Tween> pool = new Stack<Tween>(64);

        public static bool HasInstance => instance != null;

        public static int ActiveCount => instance != null ? instance.active.Count : 0;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            instance = null;
            quitting = false;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Bootstrap()
        {
            EnsureInstance();
        }

        internal static TweenManager EnsureInstance()
        {
            if (instance != null) return instance;
            if (quitting) return null;

            var go = new GameObject("CatAnnaDev.TweenManager");
            go.hideFlags = HideFlags.HideAndDontSave;
            DontDestroyOnLoad(go);
            instance = go.AddComponent<TweenManager>();
            return instance;
        }

        void OnApplicationQuit()
        {
            quitting = true;
        }

        void Update()
        {
            float dt = Time.deltaTime;
            float udt = Time.unscaledDeltaTime;

            int count = active.Count;
            for (int i = 0; i < count; i++)
            {
                Tween t = active[i];
                if (!t.alive) continue;
                t.Step(t.ignoreTimeScale ? udt : dt);
            }

            int write = 0;
            int total = active.Count;
            for (int i = 0; i < total; i++)
            {
                Tween t = active[i];
                if (t.alive)
                {
                    active[write++] = t;
                }
                else
                {
                    Recycle(t);
                }
            }
            if (write < total)
                active.RemoveRange(write, total - write);
        }

        Tween RentInternal()
        {
            Tween t = pool.Count > 0 ? pool.Pop() : new Tween();
            t.ResetState();
            active.Add(t);
            return t;
        }

        void Recycle(Tween t)
        {
            t.ClearReferences();
            pool.Push(t);
        }

        internal TweenHandle CreateFloat(UnityEngine.Object owner, float from, float to, float duration, Action<float> setter)
        {
            Tween t = RentInternal();
            t.kind = TweenValueKind.Float;
            t.fromV4 = new Vector4(from, 0f, 0f, 0f);
            t.toV4 = new Vector4(to, 0f, 0f, 0f);
            t.duration = Mathf.Max(0f, duration);
            t.setter = setter;
            t.SetTarget(owner);
            return new TweenHandle(t);
        }

        internal TweenHandle CreateVector2(UnityEngine.Object owner, Vector2 from, Vector2 to, float duration, Action<Vector2> setter)
        {
            Tween t = RentInternal();
            t.kind = TweenValueKind.Vector2;
            t.fromV4 = from;
            t.toV4 = to;
            t.duration = Mathf.Max(0f, duration);
            t.setter = setter;
            t.SetTarget(owner);
            return new TweenHandle(t);
        }

        internal TweenHandle CreateVector3(UnityEngine.Object owner, Vector3 from, Vector3 to, float duration, Action<Vector3> setter)
        {
            Tween t = RentInternal();
            t.kind = TweenValueKind.Vector3;
            t.fromV4 = from;
            t.toV4 = to;
            t.duration = Mathf.Max(0f, duration);
            t.setter = setter;
            t.SetTarget(owner);
            return new TweenHandle(t);
        }

        internal TweenHandle CreateQuaternion(UnityEngine.Object owner, Quaternion from, Quaternion to, float duration, Action<Quaternion> setter)
        {
            Tween t = RentInternal();
            t.kind = TweenValueKind.Quaternion;
            t.fromQ = from;
            t.toQ = to;
            t.duration = Mathf.Max(0f, duration);
            t.setter = setter;
            t.SetTarget(owner);
            return new TweenHandle(t);
        }

        internal TweenHandle CreateColor(UnityEngine.Object owner, Color from, Color to, float duration, Action<Color> setter)
        {
            Tween t = RentInternal();
            t.kind = TweenValueKind.Color;
            t.fromV4 = from;
            t.toV4 = to;
            t.duration = Mathf.Max(0f, duration);
            t.setter = setter;
            t.SetTarget(owner);
            return new TweenHandle(t);
        }

        internal TweenHandle CreateCallback(UnityEngine.Object owner, float duration, Action<float> onEval)
        {
            Tween t = RentInternal();
            t.kind = TweenValueKind.Callback;
            t.duration = Mathf.Max(0f, duration);
            t.eval = onEval;
            t.SetTarget(owner);
            return new TweenHandle(t);
        }

        public static void KillAll(bool complete = false)
        {
            if (instance == null) return;
            List<Tween> list = instance.active;
            for (int i = 0; i < list.Count; i++)
                list[i].Kill(complete);
        }

        public static void KillTweensOf(UnityEngine.Object owner, bool complete = false)
        {
            if (instance == null || owner == null) return;
            List<Tween> list = instance.active;
            for (int i = 0; i < list.Count; i++)
            {
                Tween t = list[i];
                if (t.target == owner)
                    t.Kill(complete);
            }
        }

        public static void PauseAll()
        {
            if (instance == null) return;
            List<Tween> list = instance.active;
            for (int i = 0; i < list.Count; i++)
                list[i].isPaused = true;
        }

        public static void ResumeAll()
        {
            if (instance == null) return;
            List<Tween> list = instance.active;
            for (int i = 0; i < list.Count; i++)
                list[i].isPaused = false;
        }
    }
}
