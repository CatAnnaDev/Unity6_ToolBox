using System;
using UnityEngine;

namespace CatAnnaDev.Lifecycle
{
    [AddComponentMenu("CatAnnaDev/Lifecycle/Lifecycle Hooks")]
    public class LifecycleHooks : MonoBehaviour
    {
        public event Action Started;
        public event Action Enabled;
        public event Action Disabled;
        public event Action Destroyed;
        public event Action Updated;
        public event Action LateUpdated;
        public event Action FixedUpdated;
        public event Action BecameVisible;
        public event Action BecameInvisible;
        public event Action<bool> ApplicationPaused;
        public event Action<bool> ApplicationFocused;
        public event Action ApplicationQuitting;

        public LifecycleHooks OnStarted(Action callback) => Add(ref Started, callback);
        public LifecycleHooks OnEnabled(Action callback) => Add(ref Enabled, callback);
        public LifecycleHooks OnDisabled(Action callback) => Add(ref Disabled, callback);
        public LifecycleHooks OnDestroyed(Action callback) => Add(ref Destroyed, callback);
        public LifecycleHooks OnUpdate(Action callback) => Add(ref Updated, callback);
        public LifecycleHooks OnLateUpdate(Action callback) => Add(ref LateUpdated, callback);
        public LifecycleHooks OnFixedUpdate(Action callback) => Add(ref FixedUpdated, callback);
        public LifecycleHooks OnApplicationPaused(Action<bool> callback) => Add(ref ApplicationPaused, callback);
        public LifecycleHooks OnApplicationFocused(Action<bool> callback) => Add(ref ApplicationFocused, callback);
        public LifecycleHooks OnApplicationQuitting(Action callback) => Add(ref ApplicationQuitting, callback);

        private void Start() => Started?.Invoke();
        private void OnEnable() => Enabled?.Invoke();
        private void OnDisable() => Disabled?.Invoke();
        private void OnDestroy() => Destroyed?.Invoke();
        private void Update() => Updated?.Invoke();
        private void LateUpdate() => LateUpdated?.Invoke();
        private void FixedUpdate() => FixedUpdated?.Invoke();
        private void OnBecameVisible() => BecameVisible?.Invoke();
        private void OnBecameInvisible() => BecameInvisible?.Invoke();
        private void OnApplicationPause(bool paused) => ApplicationPaused?.Invoke(paused);
        private void OnApplicationFocus(bool focused) => ApplicationFocused?.Invoke(focused);
        private void OnApplicationQuit() => ApplicationQuitting?.Invoke();

        private LifecycleHooks Add(ref Action target, Action callback)
        {
            if (callback != null) target += callback;
            return this;
        }

        private LifecycleHooks Add(ref Action<bool> target, Action<bool> callback)
        {
            if (callback != null) target += callback;
            return this;
        }
    }
}
