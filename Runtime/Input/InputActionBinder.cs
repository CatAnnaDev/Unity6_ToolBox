#if ENABLE_INPUT_SYSTEM
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using CatAnnaDev.Lifecycle;

namespace CatAnnaDev.Input
{
    public sealed class InputActionBinder : IDisposable
    {
        private readonly InputAction action;
        private readonly List<Action<InputAction.CallbackContext>> started = new List<Action<InputAction.CallbackContext>>();
        private readonly List<Action<InputAction.CallbackContext>> performed = new List<Action<InputAction.CallbackContext>>();
        private readonly List<Action<InputAction.CallbackContext>> canceled = new List<Action<InputAction.CallbackContext>>();
        private bool disposed;

        public InputActionBinder(InputAction action)
        {
            this.action = action;
        }

        public InputAction Action => action;

        public InputActionBinder OnStarted(Action<InputAction.CallbackContext> callback)
        {
            if (callback != null && action != null)
            {
                action.started += callback;
                started.Add(callback);
            }
            return this;
        }

        public InputActionBinder OnPerformed(Action<InputAction.CallbackContext> callback)
        {
            if (callback != null && action != null)
            {
                action.performed += callback;
                performed.Add(callback);
            }
            return this;
        }

        public InputActionBinder OnCanceled(Action<InputAction.CallbackContext> callback)
        {
            if (callback != null && action != null)
            {
                action.canceled += callback;
                canceled.Add(callback);
            }
            return this;
        }

        public InputActionBinder OnStarted(Action callback) => OnStarted(_ => callback());
        public InputActionBinder OnPerformed(Action callback) => OnPerformed(_ => callback());
        public InputActionBinder OnCanceled(Action callback) => OnCanceled(_ => callback());

        public InputActionBinder Enable()
        {
            action?.Enable();
            return this;
        }

        public InputActionBinder Disable()
        {
            action?.Disable();
            return this;
        }

        public InputActionBinder DisposeWith(GameObject owner)
        {
            owner.Lifecycle().OnDestroyed(Dispose);
            return this;
        }

        public InputActionBinder DisposeWith(Component owner) => DisposeWith(owner.gameObject);

        public void Dispose()
        {
            if (disposed || action == null) return;
            disposed = true;

            foreach (Action<InputAction.CallbackContext> handler in started) action.started -= handler;
            foreach (Action<InputAction.CallbackContext> handler in performed) action.performed -= handler;
            foreach (Action<InputAction.CallbackContext> handler in canceled) action.canceled -= handler;

            started.Clear();
            performed.Clear();
            canceled.Clear();
        }
    }
}
#endif
