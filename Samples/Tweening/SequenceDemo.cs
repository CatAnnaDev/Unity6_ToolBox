using UnityEngine;
using CatAnnaDev.Tweening;

namespace CatAnnaDev.Samples
{
    public sealed class SequenceDemo : MonoBehaviour
    {
        private Transform cube;
        private Material material;
        private Vector3 basePosition;
        private Vector3 baseScale;
        private Sequence current;
        private string lastEvent = "press Play";

        private GUIStyle boxStyle;
        private GUIStyle titleStyle;
        private GUIStyle bodyStyle;

        private void Awake()
        {
            EnsureCamera();
            EnsureLight();

            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = "Sequence Cube";
            cube = go.transform;
            basePosition = new Vector3(0f, 0.5f, 0f);
            baseScale = Vector3.one;
            cube.position = basePosition;

            material = new Material(Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard"));
            material.color = new Color(0.25f, 0.6f, 1f);
            go.GetComponent<Renderer>().material = material;
        }

        private void PlaySequence()
        {
            current?.Kill();
            cube.position = basePosition;
            cube.localScale = baseScale;
            material.color = new Color(0.25f, 0.6f, 1f);
            lastEvent = "playing...";

            current = Sequence.Create()
                .Append(() => cube.TweenLocalPosition(basePosition + Vector3.up * 2.5f, 0.4f).SetEase(Ease.OutBack))
                .Append(() => cube.TweenLocalScale(baseScale * 1.6f, 0.25f).SetEase(Ease.OutQuad))
                .Join(() => material.TweenColor(new Color(1f, 0.55f, 0.2f), 0.25f))
                .AppendInterval(0.4f)
                .Append(() => cube.TweenLocalScale(baseScale, 0.2f))
                .Join(() => cube.TweenLocalPosition(basePosition, 0.4f).SetEase(Ease.InOutCubic))
                .Join(() => material.TweenColor(new Color(0.25f, 0.6f, 1f), 0.4f))
                .AppendCallback(() => lastEvent = "sequence complete")
                .OnComplete(() => lastEvent = "sequence complete (OnComplete)")
                .Play();
        }

        private void OnGUI()
        {
            EnsureStyles();

            GUILayout.BeginArea(new Rect(12f, 12f, 470f, 260f), boxStyle);
            GUILayout.Label("CatAnnaDev - Tween Sequence demo", titleStyle);
            GUILayout.Space(4f);
            GUILayout.Label(
                "Chain and parallelise tweens on a timeline:\n" +
                "Append (after previous) / Join (parallel) / AppendInterval / AppendCallback.\n" +
                "Each step is a factory so 'from' is captured when the step actually starts.",
                bodyStyle);

            GUILayout.Space(6f);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Play")) PlaySequence();
            if (GUILayout.Button("Pause")) current?.Pause();
            if (GUILayout.Button("Resume")) current?.Resume();
            if (GUILayout.Button("Kill")) current?.Kill();
            GUILayout.EndHorizontal();

            GUILayout.Space(6f);
            GUILayout.Label("active sequences : " + SequenceRunner.ActiveCount, bodyStyle);
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

        private static void EnsureCamera()
        {
            if (Camera.main != null) return;
            GameObject go = new GameObject("Main Camera");
            go.tag = "MainCamera";
            Camera camera = go.AddComponent<Camera>();
            camera.transform.position = new Vector3(0f, 2.5f, -7f);
            camera.transform.rotation = Quaternion.Euler(12f, 0f, 0f);
        }

        private static void EnsureLight()
        {
            GameObject go = new GameObject("Directional Light");
            Light light = go.AddComponent<Light>();
            light.type = LightType.Directional;
            go.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        }
    }
}
