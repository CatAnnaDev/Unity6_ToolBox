using System;
using System.Collections.Concurrent;
using System.Threading;
using UnityEngine;
using UnityEngine.PlayerLoop;
using CatAnnaDev;

namespace CatAnnaDev.Scheduling
{
    public static class MainThreadDispatcher
    {
        struct MainThreadDispatcherUpdate { }

        static readonly ConcurrentQueue<Action> Queue = new ConcurrentQueue<Action>();
        static int _mainThreadId = -1;
        static bool _installed;

        public static bool IsMainThread
        {
            get { return Thread.CurrentThread.ManagedThreadId == _mainThreadId; }
        }

        public static int PendingCount
        {
            get { return Queue.Count; }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetState()
        {
            while (Queue.TryDequeue(out _))
            {
            }
            _installed = false;
            _mainThreadId = Thread.CurrentThread.ManagedThreadId;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Install()
        {
            _mainThreadId = Thread.CurrentThread.ManagedThreadId;
            if (_installed)
            {
                return;
            }
            _installed = PlayerLoopBinder.Insert(typeof(Update), typeof(MainThreadDispatcherUpdate), Pump);
            if (!_installed)
            {
                CatLog.Warn("MainThreadDispatcher failed to install into the PlayerLoop.");
            }
        }

        public static void Enqueue(Action action)
        {
            if (action == null)
            {
                return;
            }
            Queue.Enqueue(action);
        }

        public static void RunOnMainThread(Action action)
        {
            if (action == null)
            {
                return;
            }
            if (IsMainThread)
            {
                Invoke(action);
                return;
            }
            Queue.Enqueue(action);
        }

        public static void Clear()
        {
            while (Queue.TryDequeue(out _))
            {
            }
        }

        static void Pump()
        {
            while (Queue.TryDequeue(out var action))
            {
                Invoke(action);
            }
        }

        static void Invoke(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                CatLog.Error(e);
            }
        }
    }
}
