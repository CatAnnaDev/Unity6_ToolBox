using System;
using System.Collections.Generic;

namespace CatAnnaDev.Events
{
    public static class EventBus
    {
        private static readonly List<Action> ClearActions = new List<Action>(32);

        internal static void RegisterClearCallback(Action clear)
        {
            if (clear == null || ClearActions.Contains(clear))
            {
                return;
            }

            ClearActions.Add(clear);
        }

        public static void Register<T>(Action<T> handler) where T : IEvent
        {
            EventBus<T>.Add(handler);
        }

        public static void RegisterOnce<T>(Action<T> handler) where T : IEvent
        {
            EventBus<T>.AddOnce(handler);
        }

        public static void Unregister<T>(Action<T> handler) where T : IEvent
        {
            EventBus<T>.Remove(handler);
        }

        public static void Raise<T>(T evt) where T : IEvent
        {
            EventBus<T>.Raise(evt);
        }

        public static EventBinding<T> Bind<T>(Action<T> handler) where T : IEvent
        {
            EventBus<T>.Add(handler);
            return new EventBinding<T>(handler);
        }

        public static EventBinding<T> BindOnce<T>(Action<T> handler) where T : IEvent
        {
            EventBus<T>.AddOnce(handler);
            return new EventBinding<T>(handler);
        }

        public static bool IsRegistered<T>(Action<T> handler) where T : IEvent
        {
            return EventBus<T>.Contains(handler);
        }

        public static int ListenerCount<T>() where T : IEvent
        {
            return EventBus<T>.Count;
        }

        public static void Clear<T>() where T : IEvent
        {
            EventBus<T>.Clear();
        }

        public static void ClearAll()
        {
            for (int i = 0; i < ClearActions.Count; i++)
            {
                Action clear = ClearActions[i];
                if (clear == null)
                {
                    continue;
                }

                try
                {
                    clear.Invoke();
                }
                catch (Exception ex)
                {
                    CatLog.Error("EventBus.ClearAll callback threw: " + ex);
                }
            }
        }
    }

    internal static class EventBus<T> where T : IEvent
    {
        private static readonly List<Action<T>> Handlers = new List<Action<T>>(8);
        private static readonly List<Action<T>> OnceHandlers = new List<Action<T>>(0);
        private static readonly List<Action<T>> DeferredAdds = new List<Action<T>>(0);
        private static readonly List<Action<T>> DeferredRemoves = new List<Action<T>>(0);
        private static int raiseDepth;

        static EventBus()
        {
            EventBus.RegisterClearCallback(Clear);
        }

        internal static int Count
        {
            get { return Handlers.Count; }
        }

        internal static bool Contains(Action<T> handler)
        {
            return handler != null && Handlers.Contains(handler);
        }

        internal static void Add(Action<T> handler)
        {
            if (handler == null)
            {
                return;
            }

            if (raiseDepth > 0)
            {
                DeferredRemoves.Remove(handler);
                if (!Handlers.Contains(handler) && !DeferredAdds.Contains(handler))
                {
                    DeferredAdds.Add(handler);
                }

                return;
            }

            if (!Handlers.Contains(handler))
            {
                Handlers.Add(handler);
            }
        }

        internal static void AddOnce(Action<T> handler)
        {
            if (handler == null)
            {
                return;
            }

            Add(handler);
            if (!OnceHandlers.Contains(handler))
            {
                OnceHandlers.Add(handler);
            }
        }

        internal static void Remove(Action<T> handler)
        {
            if (handler == null)
            {
                return;
            }

            OnceHandlers.Remove(handler);

            if (raiseDepth > 0)
            {
                DeferredAdds.Remove(handler);
                if (Handlers.Contains(handler) && !DeferredRemoves.Contains(handler))
                {
                    DeferredRemoves.Add(handler);
                }

                return;
            }

            Handlers.Remove(handler);
        }

        internal static void Raise(T evt)
        {
            raiseDepth++;
            try
            {
                for (int i = 0; i < Handlers.Count; i++)
                {
                    Action<T> handler = Handlers[i];
                    if (handler == null)
                    {
                        continue;
                    }

                    if (DeferredRemoves.Count > 0 && DeferredRemoves.Contains(handler))
                    {
                        continue;
                    }

                    try
                    {
                        handler.Invoke(evt);
                    }
                    catch (Exception ex)
                    {
                        CatLog.Error("EventBus handler for " + typeof(T).Name + " threw: " + ex);
                    }

                    if (OnceHandlers.Count > 0 && OnceHandlers.Remove(handler))
                    {
                        Remove(handler);
                    }
                }
            }
            finally
            {
                raiseDepth--;
                if (raiseDepth == 0)
                {
                    ApplyDeferred();
                }
            }
        }

        private static void ApplyDeferred()
        {
            if (DeferredRemoves.Count > 0)
            {
                for (int i = 0; i < DeferredRemoves.Count; i++)
                {
                    Handlers.Remove(DeferredRemoves[i]);
                }

                DeferredRemoves.Clear();
            }

            if (DeferredAdds.Count > 0)
            {
                for (int i = 0; i < DeferredAdds.Count; i++)
                {
                    Action<T> handler = DeferredAdds[i];
                    if (!Handlers.Contains(handler))
                    {
                        Handlers.Add(handler);
                    }
                }

                DeferredAdds.Clear();
            }
        }

        internal static void Clear()
        {
            Handlers.Clear();
            OnceHandlers.Clear();
            DeferredAdds.Clear();
            DeferredRemoves.Clear();
            raiseDepth = 0;
        }
    }
}
