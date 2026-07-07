using UnityEngine;
using CatAnnaDev.Events;

namespace CatAnnaDev.Samples
{
    public sealed class EventsDemo : MonoBehaviour
    {
        public struct DamageEvent : IEvent
        {
            public int Amount;
            public string Source;
        }

        public struct PlayerDiedEvent : IEvent
        {
        }

        private int damageEventsReceived;
        private int totalDamageTaken;
        private int deathEventsReceived;
        private int onceEventsReceived;

        private int soVoidRaises;
        private int soIntRaises;
        private int soIntLastValue;

        private GameEvent respawnGameEvent;
        private IntGameEvent scoreGameEvent;

        private EventBinding<DamageEvent> boundLogger;

        private string lastLine = "Nothing yet. Press a key.";

        private void OnEnable()
        {
            EventBus.Register<DamageEvent>(OnDamage);
            EventBus.Register<PlayerDiedEvent>(OnPlayerDied);

            boundLogger = EventBus.Bind<DamageEvent>(OnDamageLogged);

            respawnGameEvent = ScriptableObject.CreateInstance<GameEvent>();
            respawnGameEvent.name = "RespawnChannel";
            respawnGameEvent.Register(OnRespawn);

            scoreGameEvent = ScriptableObject.CreateInstance<IntGameEvent>();
            scoreGameEvent.name = "ScoreChannel";
            scoreGameEvent.Register(OnScoreChanged);
        }

        private void OnDisable()
        {
            EventBus.Unregister<DamageEvent>(OnDamage);
            EventBus.Unregister<PlayerDiedEvent>(OnPlayerDied);

            boundLogger?.Dispose();
            boundLogger = null;

            if (respawnGameEvent != null)
            {
                respawnGameEvent.Unregister(OnRespawn);
                Destroy(respawnGameEvent);
                respawnGameEvent = null;
            }

            if (scoreGameEvent != null)
            {
                scoreGameEvent.Unregister(OnScoreChanged);
                Destroy(scoreGameEvent);
                scoreGameEvent = null;
            }
        }

        private void OnDamage(DamageEvent evt)
        {
            damageEventsReceived++;
            totalDamageTaken += evt.Amount;
            lastLine = "DamageEvent handled: " + evt.Amount + " from " + evt.Source;
        }

        private void OnDamageLogged(DamageEvent evt)
        {
            lastLine = "Bound logger also saw " + evt.Amount + " damage";
        }

        private void OnPlayerDied(PlayerDiedEvent evt)
        {
            deathEventsReceived++;
            lastLine = "PlayerDiedEvent handled";
        }

        private void OnDamageOnce(DamageEvent evt)
        {
            onceEventsReceived++;
            lastLine = "RegisterOnce handler fired for " + evt.Amount + " then auto-removed";
        }

        private void OnRespawn()
        {
            soVoidRaises++;
            lastLine = "GameEvent (respawn) action invoked";
        }

        private void OnScoreChanged(int value)
        {
            soIntRaises++;
            soIntLastValue = value;
            lastLine = "IntGameEvent action invoked with value " + value;
        }

        private void Update()
        {
            if (DemoInput.GetKeyDown(KeyCode.Alpha1))
            {
                EventBus.Raise(new DamageEvent { Amount = Random.Range(5, 25), Source = "Spikes" });
            }

            if (DemoInput.GetKeyDown(KeyCode.Alpha2))
            {
                EventBus.Raise(new PlayerDiedEvent());
            }

            if (DemoInput.GetKeyDown(KeyCode.Alpha3))
            {
                EventBus.RegisterOnce<DamageEvent>(OnDamageOnce);
                lastLine = "Armed a RegisterOnce handler. Press 1 to fire it once.";
            }

            if (DemoInput.GetKeyDown(KeyCode.Alpha4))
            {
                if (respawnGameEvent != null)
                {
                    respawnGameEvent.Raise();
                }
            }

            if (DemoInput.GetKeyDown(KeyCode.Alpha5))
            {
                if (scoreGameEvent != null)
                {
                    scoreGameEvent.Raise(soIntRaises * 100 + 100);
                }
            }
        }

        private GUIStyle titleStyle;
        private GUIStyle bodyStyle;
        private GUIStyle boxStyle;

        private void EnsureStyles()
        {
            if (boxStyle != null)
            {
                return;
            }

            boxStyle = new GUIStyle(GUI.skin.box);
            Texture2D bg = new Texture2D(1, 1);
            bg.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.85f));
            bg.Apply();
            boxStyle.normal.background = bg;

            titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold
            };
            titleStyle.normal.textColor = Color.white;

            bodyStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                wordWrap = true
            };
            bodyStyle.normal.textColor = Color.white;
        }

        private void OnGUI()
        {
            EnsureStyles();

            GUILayout.BeginArea(new Rect(10f, 10f, 460f, 520f), boxStyle);
            GUILayout.Space(6f);

            GUILayout.Label("CatAnnaDev - Events Demo", titleStyle);
            GUILayout.Label(
                "Two event systems side by side. The static EventBus is a typed, global " +
                "publish/subscribe channel keyed on struct types that implement IEvent. " +
                "A GameEvent ScriptableObject is an asset-based channel you subscribe Actions to.",
                bodyStyle);

            GUILayout.Space(8f);
            GUILayout.Label("Controls", titleStyle);
            GUILayout.Label("[1] Raise DamageEvent on the EventBus (random amount)", bodyStyle);
            GUILayout.Label("[2] Raise PlayerDiedEvent on the EventBus", bodyStyle);
            GUILayout.Label("[3] Arm a RegisterOnce DamageEvent handler (fires once on next [1])", bodyStyle);
            GUILayout.Label("[4] Raise the void GameEvent ScriptableObject", bodyStyle);
            GUILayout.Label("[5] Raise the IntGameEvent ScriptableObject", bodyStyle);

            GUILayout.Space(8f);
            GUILayout.Label("EventBus status", titleStyle);
            GUILayout.Label("DamageEvent listeners: " + EventBus.ListenerCount<DamageEvent>(), bodyStyle);
            GUILayout.Label("DamageEvents received: " + damageEventsReceived +
                            "   total damage: " + totalDamageTaken, bodyStyle);
            GUILayout.Label("PlayerDiedEvents received: " + deathEventsReceived, bodyStyle);
            GUILayout.Label("RegisterOnce fires: " + onceEventsReceived, bodyStyle);
            GUILayout.Label("Bound logger active: " + (boundLogger != null && boundLogger.IsBound), bodyStyle);

            GUILayout.Space(8f);
            GUILayout.Label("GameEvent (ScriptableObject) status", titleStyle);
            GUILayout.Label("Respawn GameEvent raises: " + soVoidRaises +
                            "   listeners: " + (respawnGameEvent != null ? respawnGameEvent.ListenerCount : 0), bodyStyle);
            GUILayout.Label("Score IntGameEvent raises: " + soIntRaises +
                            "   last value: " + soIntLastValue, bodyStyle);

            GUILayout.Space(8f);
            GUILayout.Label("Last: " + lastLine, bodyStyle);

            GUILayout.EndArea();
        }
    }
}
