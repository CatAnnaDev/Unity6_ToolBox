using System;
using UnityEngine;
using UnityEngine.Events;

namespace CatAnnaDev.Events
{
    public abstract class GameEventListener<T> : MonoBehaviour, IGameEventListener<T>
    {
        protected abstract GameEvent<T> Channel { get; }

        protected abstract void Respond(T value);

        private void OnEnable()
        {
            GameEvent<T> channel = Channel;
            if (channel != null)
            {
                channel.Register(this);
            }
        }

        private void OnDisable()
        {
            GameEvent<T> channel = Channel;
            if (channel != null)
            {
                channel.Unregister(this);
            }
        }

        public void OnEventRaised(T value)
        {
            Respond(value);
        }
    }

    [Serializable]
    public sealed class IntEvent : UnityEvent<int>
    {
    }

    [Serializable]
    public sealed class FloatEvent : UnityEvent<float>
    {
    }

    [Serializable]
    public sealed class BoolEvent : UnityEvent<bool>
    {
    }

    [Serializable]
    public sealed class StringEvent : UnityEvent<string>
    {
    }

    [Serializable]
    public sealed class Vector3Event : UnityEvent<Vector3>
    {
    }

    [Serializable]
    public sealed class GameObjectEvent : UnityEvent<GameObject>
    {
    }

    [AddComponentMenu("CatAnnaDev/Events/Int Game Event Listener")]
    public sealed class IntGameEventListener : GameEventListener<int>
    {
        [SerializeField]
        private IntGameEvent gameEvent;

        [SerializeField]
        private IntEvent response;

        protected override GameEvent<int> Channel
        {
            get { return gameEvent; }
        }

        protected override void Respond(int value)
        {
            response?.Invoke(value);
        }
    }

    [AddComponentMenu("CatAnnaDev/Events/Float Game Event Listener")]
    public sealed class FloatGameEventListener : GameEventListener<float>
    {
        [SerializeField]
        private FloatGameEvent gameEvent;

        [SerializeField]
        private FloatEvent response;

        protected override GameEvent<float> Channel
        {
            get { return gameEvent; }
        }

        protected override void Respond(float value)
        {
            response?.Invoke(value);
        }
    }

    [AddComponentMenu("CatAnnaDev/Events/Bool Game Event Listener")]
    public sealed class BoolGameEventListener : GameEventListener<bool>
    {
        [SerializeField]
        private BoolGameEvent gameEvent;

        [SerializeField]
        private BoolEvent response;

        protected override GameEvent<bool> Channel
        {
            get { return gameEvent; }
        }

        protected override void Respond(bool value)
        {
            response?.Invoke(value);
        }
    }

    [AddComponentMenu("CatAnnaDev/Events/String Game Event Listener")]
    public sealed class StringGameEventListener : GameEventListener<string>
    {
        [SerializeField]
        private StringGameEvent gameEvent;

        [SerializeField]
        private StringEvent response;

        protected override GameEvent<string> Channel
        {
            get { return gameEvent; }
        }

        protected override void Respond(string value)
        {
            response?.Invoke(value);
        }
    }

    [AddComponentMenu("CatAnnaDev/Events/Vector3 Game Event Listener")]
    public sealed class Vector3GameEventListener : GameEventListener<Vector3>
    {
        [SerializeField]
        private Vector3GameEvent gameEvent;

        [SerializeField]
        private Vector3Event response;

        protected override GameEvent<Vector3> Channel
        {
            get { return gameEvent; }
        }

        protected override void Respond(Vector3 value)
        {
            response?.Invoke(value);
        }
    }

    [AddComponentMenu("CatAnnaDev/Events/GameObject Game Event Listener")]
    public sealed class GameObjectGameEventListener : GameEventListener<GameObject>
    {
        [SerializeField]
        private GameObjectGameEvent gameEvent;

        [SerializeField]
        private GameObjectEvent response;

        protected override GameEvent<GameObject> Channel
        {
            get { return gameEvent; }
        }

        protected override void Respond(GameObject value)
        {
            response?.Invoke(value);
        }
    }
}
