using UnityEngine;
using CatAnnaDev.Springs;

namespace CatAnnaDev.Samples
{
    public sealed class SpringDemo : MonoBehaviour
    {
        private Transform target;
        private Transform snappy;
        private Transform loose;

        private readonly SpringVector3 snappySpring = new SpringVector3(stiffness: 220f, damping: 26f);
        private readonly SpringVector3 looseSpring = new SpringVector3(stiffness: 90f, damping: 9f);

        private float angle;
        private bool orbiting = true;

        private GUIStyle boxStyle;
        private GUIStyle titleStyle;
        private GUIStyle bodyStyle;

        private void Awake()
        {
            EnsureCamera();
            EnsureLight();

            target = BuildMarker("Target", new Color(1f, 0.9f, 0.2f), 0.6f).transform;
            snappy = BuildMarker("Snappy Spring", new Color(0.3f, 0.8f, 1f), 1f).transform;
            loose = BuildMarker("Loose Spring", new Color(1f, 0.45f, 0.5f), 1f).transform;

            snappySpring.Reset(Vector3.zero);
            looseSpring.Reset(Vector3.zero);
        }

        private void Update()
        {
            float dt = Time.deltaTime;

            if (orbiting) angle += dt * 1.6f;
            Vector3 targetPos = new Vector3(Mathf.Cos(angle) * 3f, 0.5f + Mathf.Sin(angle * 2f) * 0.5f, Mathf.Sin(angle) * 3f);
            target.position = targetPos;

            snappy.position = snappySpring.Update(targetPos, dt);
            loose.position = looseSpring.Update(targetPos, dt);
        }

        private void OnGUI()
        {
            EnsureStyles();

            GUILayout.BeginArea(new Rect(12f, 12f, 460f, 250f), boxStyle);
            GUILayout.Label("CatAnnaDev - Springs demo", titleStyle);
            GUILayout.Space(4f);
            GUILayout.Label(
                "Damped springs follow a moving target with no fixed duration.\n" +
                "Blue = stiff/high-damping (snappy). Red = soft/low-damping (overshoots).\n" +
                "spring.Update(target, dt) each frame; stable via internal substepping.",
                bodyStyle);

            GUILayout.Space(6f);
            if (GUILayout.Button(orbiting ? "Stop target" : "Orbit target")) orbiting = !orbiting;

            GUILayout.Space(6f);
            GUILayout.Label("snappy vel : " + snappySpring.Velocity.magnitude.ToString("0.00"), bodyStyle);
            GUILayout.Label("loose vel  : " + looseSpring.Velocity.magnitude.ToString("0.00"), bodyStyle);
            GUILayout.EndArea();
        }

        private static GameObject BuildMarker(string name, Color color, float scale)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = name;
            go.transform.localScale = Vector3.one * scale;
            Material material = new Material(Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard"));
            material.color = color;
            go.GetComponent<Renderer>().material = material;
            Destroy(go.GetComponent<Collider>());
            return go;
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
            camera.transform.position = new Vector3(0f, 4f, -8f);
            camera.transform.rotation = Quaternion.Euler(22f, 0f, 0f);
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
