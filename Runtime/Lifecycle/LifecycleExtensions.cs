using System;
using UnityEngine;

namespace CatAnnaDev.Lifecycle
{
    public static class LifecycleExtensions
    {
        public static LifecycleHooks Lifecycle(this GameObject go)
        {
            LifecycleHooks hooks = go.GetComponent<LifecycleHooks>();
            return hooks != null ? hooks : go.AddComponent<LifecycleHooks>();
        }

        public static LifecycleHooks Lifecycle(this Component component) => component.gameObject.Lifecycle();

        public static LifecycleHooks OnBecameVisible(this LifecycleHooks hooks, Action callback) { if (callback != null) hooks.BecameVisible += callback; return hooks; }
        public static LifecycleHooks OnBecameInvisible(this LifecycleHooks hooks, Action callback) { if (callback != null) hooks.BecameInvisible += callback; return hooks; }

        public static LifecycleHooks OnDestroyed(this GameObject go, Action callback) => go.Lifecycle().OnDestroyed(callback);
        public static LifecycleHooks OnDisabled(this GameObject go, Action callback) => go.Lifecycle().OnDisabled(callback);
        public static LifecycleHooks OnEnabled(this GameObject go, Action callback) => go.Lifecycle().OnEnabled(callback);
        public static LifecycleHooks OnUpdate(this GameObject go, Action callback) => go.Lifecycle().OnUpdate(callback);
        public static LifecycleHooks OnBecameVisible(this GameObject go, Action callback) => go.Lifecycle().OnBecameVisible(callback);
        public static LifecycleHooks OnBecameInvisible(this GameObject go, Action callback) => go.Lifecycle().OnBecameInvisible(callback);

        public static LifecycleHooks OnDestroyed(this Component component, Action callback) => component.gameObject.Lifecycle().OnDestroyed(callback);
        public static LifecycleHooks OnDisabled(this Component component, Action callback) => component.gameObject.Lifecycle().OnDisabled(callback);
    }
}
