#if ENABLE_INPUT_SYSTEM
using System;
using UnityEngine.InputSystem;

namespace CatAnnaDev.Input
{
    public static class InputExtensions
    {
        public static InputActionBinder Bind(this InputAction action) => new InputActionBinder(action);

        public static InputActionBinder Bind(this InputActionReference reference) => new InputActionBinder(reference.action);

        public static InputActionBinder OnStarted(this InputAction action, Action<InputAction.CallbackContext> callback) => action.Bind().OnStarted(callback);
        public static InputActionBinder OnPerformed(this InputAction action, Action<InputAction.CallbackContext> callback) => action.Bind().OnPerformed(callback);
        public static InputActionBinder OnCanceled(this InputAction action, Action<InputAction.CallbackContext> callback) => action.Bind().OnCanceled(callback);

        public static InputActionBinder OnStarted(this InputAction action, Action callback) => action.Bind().OnStarted(callback);
        public static InputActionBinder OnPerformed(this InputAction action, Action callback) => action.Bind().OnPerformed(callback);
        public static InputActionBinder OnCanceled(this InputAction action, Action callback) => action.Bind().OnCanceled(callback);
    }
}
#endif
