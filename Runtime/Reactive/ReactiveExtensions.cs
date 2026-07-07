using System;
using UnityEngine;
using CatAnnaDev.Lifecycle;

namespace CatAnnaDev.Reactive
{
    public static class ReactiveExtensions
    {
        public static IDisposable DisposeWith(this IDisposable disposable, GameObject owner)
        {
            if (disposable == null) return null;
            owner.Lifecycle().OnDestroyed(disposable.Dispose);
            return disposable;
        }

        public static IDisposable DisposeWith(this IDisposable disposable, Component owner)
            => disposable.DisposeWith(owner.gameObject);
    }
}
