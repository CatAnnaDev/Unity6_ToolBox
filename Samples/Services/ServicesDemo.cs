using UnityEngine;
using CatAnnaDev.Services;

namespace CatAnnaDev.Samples
{
    public interface IScoreService
    {
        int Total { get; }
        void Add(int amount);
        void Reset();
    }

    public sealed class ScoreService : IScoreService, IService
    {
        private int total;
        private bool registered;

        public int Total
        {
            get { return total; }
        }

        public bool IsRegistered
        {
            get { return registered; }
        }

        public void Add(int amount)
        {
            total += amount;
        }

        public void Reset()
        {
            total = 0;
        }

        public void OnRegistered()
        {
            registered = true;
        }

        public void OnUnregistered()
        {
            registered = false;
        }
    }

    public sealed class GameClock : MonoSingleton<GameClock>
    {
        private float elapsed;

        public float Elapsed
        {
            get { return elapsed; }
        }

        private void Update()
        {
            elapsed += Time.deltaTime;
        }

        public void ResetClock()
        {
            elapsed = 0f;
        }
    }

    public sealed class ServicesDemo : MonoBehaviour
    {
        private ScoreService ownedService;
        private GUIStyle boxStyle;
        private Rect windowRect = new Rect(16f, 16f, 460f, 360f);
        private string lastAction = "nothing yet";

        private void Awake()
        {
            ownedService = new ScoreService();
        }

        private void OnDisable()
        {
            if (ServiceLocator.IsRegistered<IScoreService>())
            {
                ServiceLocator.Unregister<IScoreService>();
            }

            if (GameClock.HasInstance)
            {
                Destroy(GameClock.Instance.gameObject);
            }

            lastAction = "cleaned up on disable";
        }

        private void Update()
        {
            if (DemoInput.GetKeyDown(KeyCode.R))
            {
                RegisterScoreService();
            }

            if (DemoInput.GetKeyDown(KeyCode.U))
            {
                UnregisterScoreService();
            }

            if (DemoInput.GetKeyDown(KeyCode.Space))
            {
                AddScore(10);
            }

            if (DemoInput.GetKeyDown(KeyCode.C))
            {
                ClearScore();
            }

            if (DemoInput.GetKeyDown(KeyCode.G))
            {
                TouchGameClock();
            }
        }

        private void RegisterScoreService()
        {
            ServiceLocator.Register<IScoreService>(ownedService);
            lastAction = "Registered IScoreService";
        }

        private void UnregisterScoreService()
        {
            bool removed = ServiceLocator.Unregister<IScoreService>();
            lastAction = removed ? "Unregistered IScoreService" : "IScoreService was not registered";
        }

        private void AddScore(int amount)
        {
            IScoreService score;
            if (ServiceLocator.TryGet<IScoreService>(out score))
            {
                score.Add(amount);
                lastAction = "Added " + amount + " via resolved service, total " + score.Total;
            }
            else
            {
                lastAction = "Cannot add, register the service first (R)";
            }
        }

        private void ClearScore()
        {
            IScoreService score;
            if (ServiceLocator.TryGet<IScoreService>(out score))
            {
                score.Reset();
                lastAction = "Reset score to 0";
            }
            else
            {
                lastAction = "Cannot reset, register the service first (R)";
            }
        }

        private void TouchGameClock()
        {
            GameClock clock = GameClock.Instance;
            clock.ResetClock();
            lastAction = "GameClock.Instance auto-created and reset";
        }

        private void OnGUI()
        {
            if (boxStyle == null)
            {
                boxStyle = new GUIStyle(GUI.skin.box);
                boxStyle.fontSize = 13;
                boxStyle.alignment = TextAnchor.UpperLeft;
                boxStyle.wordWrap = true;
            }

            windowRect = GUILayout.Window(GetEntityId().GetHashCode(), windowRect, DrawWindow, "CatAnnaDev - Services Demo");
        }

        private void DrawWindow(int id)
        {
            GUILayout.Label(
                "ServiceLocator is a static registry: Register<T>() an implementation, then " +
                "Get<T>() / TryGet<T>() it from anywhere. MonoSingleton<T> gives one auto-created " +
                "instance via .Instance. This is the pattern real systems (audio, saving, input) use.",
                boxStyle);

            GUILayout.Space(6f);

            bool registered = ServiceLocator.IsRegistered<IScoreService>();
            GUILayout.Label("Services registered total: " + ServiceLocator.Count);
            GUILayout.Label("IScoreService registered: " + registered);

            int total = registered ? ServiceLocator.Get<IScoreService>().Total : ownedService.Total;
            GUILayout.Label("Score total: " + total);

            string clockState = GameClock.HasInstance
                ? GameClock.Instance.Elapsed.ToString("F1") + "s alive"
                : "not created yet";
            GUILayout.Label("GameClock singleton: " + clockState);

            GUILayout.Space(6f);
            GUILayout.Label("Last action: " + lastAction);

            GUILayout.Space(8f);
            GUILayout.Label("Keys:  R register   U unregister   Space +10   C reset   G touch GameClock");

            GUILayout.Space(6f);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Register (R)"))
            {
                RegisterScoreService();
            }
            if (GUILayout.Button("Unregister (U)"))
            {
                UnregisterScoreService();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add +10 (Space)"))
            {
                AddScore(10);
            }
            if (GUILayout.Button("Reset (C)"))
            {
                ClearScore();
            }
            if (GUILayout.Button("GameClock (G)"))
            {
                TouchGameClock();
            }
            GUILayout.EndHorizontal();

            GUI.DragWindow(new Rect(0f, 0f, 10000f, 24f));
        }
    }
}
