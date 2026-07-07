using UnityEngine;
using CatAnnaDev.Cameras;

namespace CatAnnaDev.Samples
{
    public sealed class CameraDemo : MonoBehaviour
    {
        private Transform target;
        private float moveSpeed = 6f;

        private GUIStyle boxStyle;
        private GUIStyle titleStyle;
        private GUIStyle bodyStyle;

        private void Awake()
        {
            EnsureLight();
            BuildGround();

            target = GameObject.CreatePrimitive(PrimitiveType.Capsule).transform;
            target.name = "Target";
            target.position = new Vector3(0f, 1f, 0f);
            Paint(target.gameObject, new Color(0.3f, 0.8f, 1f));

            BuildCameraRig();
        }

        private void Update()
        {
            float h = DemoInput.GetAxisRaw("Horizontal");
            float v = DemoInput.GetAxisRaw("Vertical");
            target.position += new Vector3(h, 0f, v) * (moveSpeed * Time.deltaTime);

            if (DemoInput.GetKeyDown(KeyCode.Space)) CameraShake.Shake(0.6f);
            if (DemoInput.GetKeyDown(KeyCode.Return)) CameraShake.Shake(1f);
        }

        private void BuildCameraRig()
        {
            Camera existing = Camera.main;
            if (existing != null) existing.gameObject.SetActive(false);

            GameObject rig = new GameObject("Camera Rig");
            rig.transform.position = new Vector3(0f, 6f, -9f);

            GameObject camGo = new GameObject("Main Camera");
            camGo.tag = "MainCamera";
            camGo.transform.SetParent(rig.transform, false);
            camGo.AddComponent<Camera>();
            camGo.Shaker();

            rig.Follow(target)
                .SetOffset(new Vector3(0f, 6f, -9f))
                .SetSmoothTime(0.18f)
                .SetLookAt(true);
        }

        private void OnGUI()
        {
            EnsureStyles();

            GUILayout.BeginArea(new Rect(12f, 12f, 460f, 220f), boxStyle);
            GUILayout.Label("CatAnnaDev - Camera demo", titleStyle);
            GUILayout.Space(4f);
            GUILayout.Label(
                "Rig root smooth-follows the target (CameraFollow); child camera adds\n" +
                "trauma-based Perlin shake (CameraShaker). Move target with WASD.",
                bodyStyle);

            GUILayout.Space(6f);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Shake 0.6  [Space]")) CameraShake.Shake(0.6f);
            if (GUILayout.Button("Shake 1.0  [Enter]")) CameraShake.Shake(1f);
            GUILayout.EndHorizontal();

            GUILayout.Space(6f);
            GUILayout.Label("trauma : " + (CameraShaker.Active != null ? CameraShaker.Active.Trauma.ToString("0.00") : "no shaker"), bodyStyle);
            GUILayout.EndArea();
        }

        private static void BuildGround()
        {
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.localScale = Vector3.one * 4f;
            Paint(ground, new Color(0.3f, 0.32f, 0.36f));
        }

        private static void Paint(GameObject go, Color color)
        {
            Material material = new Material(Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard"));
            material.color = color;
            go.GetComponent<Renderer>().material = material;
        }

        private void EnsureStyles()
        {
            if (boxStyle != null) return;
            boxStyle = new GUIStyle(GUI.skin.box) { padding = new RectOffset(12, 12, 12, 12) };
            titleStyle = new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold };
            bodyStyle = new GUIStyle(GUI.skin.label) { wordWrap = true };
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
