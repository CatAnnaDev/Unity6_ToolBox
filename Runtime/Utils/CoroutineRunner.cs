using System;
using System.Collections;
using UnityEngine;
using CatAnnaDev.Services;

namespace CatAnnaDev.Utils
{
    [DefaultExecutionOrder(-10000)]
    public sealed class CoroutineRunner : PersistentSingleton<CoroutineRunner>
    {
        public static Coroutine Run(IEnumerator routine)
        {
            if (routine == null)
            {
                return null;
            }
            return Instance.StartCoroutine(routine);
        }

        public static void Stop(Coroutine coroutine)
        {
            if (coroutine != null && HasInstance)
            {
                Instance.StopCoroutine(coroutine);
            }
        }

        public static void StopAll()
        {
            if (HasInstance)
            {
                Instance.StopAllCoroutines();
            }
        }

        public static Coroutine RunDelayed(float seconds, Action action)
        {
            if (action == null)
            {
                return null;
            }
            return Instance.StartCoroutine(DelayedRoutine(seconds, action));
        }

        public static Coroutine RunNextFrame(Action action)
        {
            if (action == null)
            {
                return null;
            }
            return Instance.StartCoroutine(NextFrameRoutine(action));
        }

        public static Coroutine RunWhen(Func<bool> condition, Action action)
        {
            if (condition == null || action == null)
            {
                return null;
            }
            return Instance.StartCoroutine(WhenRoutine(condition, action));
        }

        private static IEnumerator DelayedRoutine(float seconds, Action action)
        {
            if (seconds > 0f)
            {
                yield return WaitCache.ForSeconds(seconds);
            }
            action();
        }

        private static IEnumerator NextFrameRoutine(Action action)
        {
            yield return null;
            action();
        }

        private static IEnumerator WhenRoutine(Func<bool> condition, Action action)
        {
            while (!condition())
            {
                yield return null;
            }
            action();
        }
    }
}
