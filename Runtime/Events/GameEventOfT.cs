using System;
using System.Collections.Generic;
using UnityEngine;

namespace CatAnnaDev.Events
{
    public interface IGameEventListener<T>
    {
        void OnEventRaised(T value);
    }

    public abstract class GameEvent<T> : ScriptableObject
    {
        [SerializeField]
        private bool logOnRaise;

        private readonly List<IGameEventListener<T>> listeners = new List<IGameEventListener<T>>(4);
        private readonly List<Action<T>> actions = new List<Action<T>>(4);
        private T lastValue;
        private bool hasLastValue;

        public T LastValue
        {
            get { return lastValue; }
        }

        public bool HasLastValue
        {
            get { return hasLastValue; }
        }

        public int ListenerCount
        {
            get { return listeners.Count + actions.Count; }
        }

        public void Raise(T value)
        {
            lastValue = value;
            hasLastValue = true;

            if (logOnRaise)
            {
                CatLog.Info("GameEvent<" + typeof(T).Name + "> raised: " + name + " = " + value, this);
            }

            for (int i = listeners.Count - 1; i >= 0; i--)
            {
                IGameEventListener<T> listener = listeners[i];
                if (listener == null)
                {
                    continue;
                }

                try
                {
                    listener.OnEventRaised(value);
                }
                catch (Exception ex)
                {
                    CatLog.Error("GameEvent '" + name + "' listener threw: " + ex, this);
                }
            }

            for (int i = actions.Count - 1; i >= 0; i--)
            {
                Action<T> action = actions[i];
                if (action == null)
                {
                    continue;
                }

                try
                {
                    action.Invoke(value);
                }
                catch (Exception ex)
                {
                    CatLog.Error("GameEvent '" + name + "' action threw: " + ex, this);
                }
            }
        }

        public void Register(IGameEventListener<T> listener)
        {
            if (listener == null || listeners.Contains(listener))
            {
                return;
            }

            listeners.Add(listener);
        }

        public void Unregister(IGameEventListener<T> listener)
        {
            if (listener == null)
            {
                return;
            }

            listeners.Remove(listener);
        }

        public void Register(Action<T> action)
        {
            if (action == null || actions.Contains(action))
            {
                return;
            }

            actions.Add(action);
        }

        public void Unregister(Action<T> action)
        {
            if (action == null)
            {
                return;
            }

            actions.Remove(action);
        }

        public void Clear()
        {
            listeners.Clear();
            actions.Clear();
            hasLastValue = false;
            lastValue = default;
        }

        private void OnDisable()
        {
            Clear();
        }
    }
}
