using UnityEngine;
using CatAnnaDev.Reactive;

namespace CatAnnaDev.Samples
{
    public sealed class ReactiveDemo : MonoBehaviour
    {
        private readonly Observable<int> score = new Observable<int>(0);
        private readonly Observable<float> health = new Observable<float>(100f);

        private string scoreLabel = "";
        private string healthLabel = "";
        private string log = "subscribed";

        private GUIStyle boxStyle;
        private GUIStyle titleStyle;
        private GUIStyle bodyStyle;

        private void Start()
        {
            score.SubscribeAndInvoke(v => scoreLabel = "score = " + v).DisposeWith(gameObject);
            health.SubscribeAndInvoke(v => healthLabel = "health = " + v.ToString("0")).DisposeWith(gameObject);

            score.Subscribe(v => log = "score changed to " + v).DisposeWith(gameObject);
            health.Subscribe(v =>
            {
                if (v <= 0f) log = "player died";
                else if (v < 30f) log = "low health warning";
            }).DisposeWith(gameObject);
        }

        private void OnGUI()
        {
            EnsureStyles();

            GUILayout.BeginArea(new Rect(12f, 12f, 460f, 260f), boxStyle);
            GUILayout.Label("CatAnnaDev - Observable demo", titleStyle);
            GUILayout.Space(4f);
            GUILayout.Label(
                "Bindable values push changes to subscribers, no polling. Observers only\n" +
                "fire when the value actually changes. Subscriptions auto-dispose with the\n" +
                "GameObject via DisposeWith(gameObject).",
                bodyStyle);

            GUILayout.Space(6f);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("+10 score")) score.Value += 10;
            if (GUILayout.Button("-15 health")) health.Value = Mathf.Max(0f, health.Value - 15f);
            if (GUILayout.Button("Reset")) { score.Value = 0; health.Value = 100f; }
            GUILayout.EndHorizontal();

            GUILayout.Space(8f);
            GUILayout.Label(scoreLabel, titleStyle);
            GUILayout.Label(healthLabel, titleStyle);
            GUILayout.Space(4f);
            GUILayout.Label("log: " + log, bodyStyle);
            GUILayout.EndArea();
        }

        private void EnsureStyles()
        {
            if (boxStyle != null) return;
            boxStyle = new GUIStyle(GUI.skin.box) { padding = new RectOffset(12, 12, 12, 12) };
            titleStyle = new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold };
            bodyStyle = new GUIStyle(GUI.skin.label) { wordWrap = true };
        }
    }
}
