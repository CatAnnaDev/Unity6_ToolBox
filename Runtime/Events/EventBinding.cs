using System;

namespace CatAnnaDev.Events
{
    public sealed class EventBinding<T> : IDisposable where T : IEvent
    {
        private Action<T> handler;
        private bool disposed;

        internal EventBinding(Action<T> handler)
        {
            this.handler = handler;
        }

        public bool IsBound
        {
            get { return !disposed && handler != null; }
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;

            if (handler != null)
            {
                EventBus.Unregister(handler);
                handler = null;
            }
        }
    }
}
