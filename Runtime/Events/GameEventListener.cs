using UnityEngine;
using UnityEngine.Events;

namespace CatAnnaDev.Events
{
    [AddComponentMenu("CatAnnaDev/Events/Game Event Listener")]
    public class GameEventListener : MonoBehaviour
    {
        [SerializeField]
        private GameEvent gameEvent;

        [SerializeField]
        private UnityEvent response;

        public GameEvent Event
        {
            get { return gameEvent; }
            set
            {
                if (gameEvent == value)
                {
                    return;
                }

                bool wasActive = isActiveAndEnabled;
                if (wasActive && gameEvent != null)
                {
                    gameEvent.Unregister(this);
                }

                gameEvent = value;

                if (wasActive && gameEvent != null)
                {
                    gameEvent.Register(this);
                }
            }
        }

        public UnityEvent Response
        {
            get { return response; }
        }

        private void OnEnable()
        {
            if (gameEvent != null)
            {
                gameEvent.Register(this);
            }
        }

        private void OnDisable()
        {
            if (gameEvent != null)
            {
                gameEvent.Unregister(this);
            }
        }

        public void OnEventRaised()
        {
            response?.Invoke();
        }
    }
}
