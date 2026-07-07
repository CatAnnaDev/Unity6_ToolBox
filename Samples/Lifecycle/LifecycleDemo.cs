using UnityEngine;
using CatAnnaDev.Lifecycle;

namespace CatAnnaDev.Samples
{
    public sealed class LifecycleDemo : MonoBehaviour
    {
        private GameObject tracked;
        private int enabledCount;
        private int disabledCount;
        private int updateTicks;
        private bool destroyed;
        private string lastEvent = "spawn a tracked object";

        private GUIStyle boxStyle;
        private GUIStyle titleStyle;
        private GUIStyle bodyStyle;

        private void Spawn()
        {
            if (tracked != null) Destroy(tracked);

            tracked = new GameObject("Tracked Object");
            destroyed = false;
            enabledCount = 0;
            disabledCount = 0;
            updateTicks = 0;

            tracked.Lifecycle()
                .OnEnabled(() => { enabledCount++; lastEvent = "OnEnabled"; })
                .OnDisabled(() => { disabledCount++; lastEvent = "OnDisabled"; })
                .OnUpdate(() => updateTicks++)
                .OnDestroyed(() => { destroyed = true; lastEvent = "OnDestroyed"; });

            lastEvent = "spawned (OnEnabled fired once)";
        }

        private void ToggleActive()
        {
            if (tracked == null) return;
            tracked.SetActive(!tracked.activeSelf);
        }

        private void DestroyTracked()
        {
            if (tracked == null) return;
            Destroy(tracked);
            tracked = null;
        }

        private void OnGUI()
        {
            EnsureStyles();

            GUILayout.BeginArea(new Rect(12f, 12f, 460f, 320f), boxStyle);
            GUILayout.Label("CatAnnaDev - Lifecycle hooks demo", titleStyle);
            GUILayout.Space(4f);
            GUILayout.Label(
                "Subscribe to a GameObject's lifecycle from the outside, no script on it:\n" +
                "go.Lifecycle().OnEnabled(...).OnDisabled(...).OnDestroyed(...).OnUpdate(...).",
                bodyStyle);

            GUILayout.Space(6f);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Spawn tracked")) Spawn();
            if (GUILayout.Button("Toggle active")) ToggleActive();
            if (GUILayout.Button("Destroy")) DestroyTracked();
            GUILayout.EndHorizontal();

            GUILayout.Space(8f);
            bool alive = tracked != null;
            GUILayout.Label("tracked object : " + (alive ? (tracked.activeSelf ? "active" : "inactive") : (destroyed ? "destroyed" : "none")), titleStyle);
            GUILayout.Label(
                "OnEnabled  : " + enabledCount +
                "\nOnDisabled : " + disabledCount +
                "\nOnUpdate ticks : " + updateTicks,
                bodyStyle);

            GUILayout.Space(4f);
            GUILayout.Label("last: " + lastEvent, bodyStyle);
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
