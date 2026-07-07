using System;
using System.Collections.Generic;
using UnityEngine;

namespace CatAnnaDev.Events
{
    [CreateAssetMenu(menuName = "CatAnnaDev/Events/Game Event", fileName = "GameEvent")]
    public class GameEvent : ScriptableObject
    {
        [SerializeField]
        private bool logOnRaise;

        private readonly List<GameEventListener> listeners = new List<GameEventListener>(4);
        private readonly List<Action> actions = new List<Action>(4);

        public int ListenerCount
        {
            get { return listeners.Count + actions.Count; }
        }

        public void Raise()
        {
            if (logOnRaise)
            {
                CatLog.Info("GameEvent raised: " + name, this);
            }

            for (int i = listeners.Count - 1; i >= 0; i--)
            {
                GameEventListener listener = listeners[i];
                if (listener == null)
                {
                    continue;
                }

                try
                {
                    listener.OnEventRaised();
                }
                catch (Exception ex)
                {
                    CatLog.Error("GameEvent '" + name + "' listener threw: " + ex, this);
                }
            }

            for (int i = actions.Count - 1; i >= 0; i--)
            {
                Action action = actions[i];
                if (action == null)
                {
                    continue;
                }

                try
                {
                    action.Invoke();
                }
                catch (Exception ex)
                {
                    CatLog.Error("GameEvent '" + name + "' action threw: " + ex, this);
                }
            }
        }

        public void Register(GameEventListener listener)
        {
            if (listener == null || listeners.Contains(listener))
            {
                return;
            }

            listeners.Add(listener);
        }

        public void Unregister(GameEventListener listener)
        {
            if (listener == null)
            {
                return;
            }

            listeners.Remove(listener);
        }

        public void Register(Action action)
        {
            if (action == null || actions.Contains(action))
            {
                return;
            }

            actions.Add(action);
        }

        public void Unregister(Action action)
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
        }

        private void OnDisable()
        {
            Clear();
        }
    }
}
