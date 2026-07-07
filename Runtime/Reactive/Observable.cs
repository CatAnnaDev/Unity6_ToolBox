using System;
using System.Collections.Generic;

namespace CatAnnaDev.Reactive
{
    public interface IReadOnlyObservable<T>
    {
        T Value { get; }
        IDisposable Subscribe(Action<T> observer);
        IDisposable SubscribeAndInvoke(Action<T> observer);
    }

    public sealed class Observable<T> : IReadOnlyObservable<T>
    {
        private readonly List<Action<T>> observers = new List<Action<T>>();
        private readonly IEqualityComparer<T> comparer;
        private T value;

        public Observable(T initial = default)
        {
            value = initial;
            comparer = EqualityComparer<T>.Default;
        }

        public Observable(T initial, IEqualityComparer<T> comparer)
        {
            value = initial;
            this.comparer = comparer ?? EqualityComparer<T>.Default;
        }

        public T Value
        {
            get => value;
            set
            {
                if (comparer.Equals(this.value, value)) return;
                this.value = value;
                Notify();
            }
        }

        public void SetForce(T newValue)
        {
            value = newValue;
            Notify();
        }

        public IDisposable Subscribe(Action<T> observer)
        {
            if (observer == null) return EmptySubscription.Instance;
            observers.Add(observer);
            return new Subscription(this, observer);
        }

        public IDisposable SubscribeAndInvoke(Action<T> observer)
        {
            IDisposable subscription = Subscribe(observer);
            observer?.Invoke(value);
            return subscription;
        }

        public void ClearObservers() => observers.Clear();

        public static implicit operator T(Observable<T> observable) => observable.value;

        private void Notify()
        {
            for (int i = observers.Count - 1; i >= 0; i--)
            {
                if (i < observers.Count) observers[i]?.Invoke(value);
            }
        }

        private void Remove(Action<T> observer) => observers.Remove(observer);

        private sealed class Subscription : IDisposable
        {
            private Observable<T> owner;
            private Action<T> observer;

            public Subscription(Observable<T> owner, Action<T> observer)
            {
                this.owner = owner;
                this.observer = observer;
            }

            public void Dispose()
            {
                if (owner == null) return;
                owner.Remove(observer);
                owner = null;
                observer = null;
            }
        }

        private sealed class EmptySubscription : IDisposable
        {
            public static readonly EmptySubscription Instance = new EmptySubscription();
            public void Dispose() { }
        }
    }
}
