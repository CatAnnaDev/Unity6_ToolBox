using UnityEngine;
using CatAnnaDev.Physics;

namespace CatAnnaDev.Samples
{
    public sealed class PhysicsDemo : MonoBehaviour
    {
        private int triggerInside;
        private int triggerEnterTotal;
        private int triggerExitTotal;
        private int landings;
        private string lastEvent = "none yet";

        private Material fallingMaterial;
        private GUIStyle boxStyle;
        private GUIStyle titleStyle;
        private GUIStyle bodyStyle;

        private void Awake()
        {
            fallingMaterial = BuildMaterial(new Color(0.30f, 0.75f, 1f));
            EnsureCamera();
            EnsureLight();
            BuildGround();
            BuildTriggerZone();
        }

        private void Update()
        {
            if (DemoInput.GetKeyDown(KeyCode.Space)) DropCube();
        }

        private void BuildGround()
        {
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ground.name = "Ground";
            ground.transform.position = new Vector3(0f, -1f, 0f);
            ground.transform.localScale = new Vector3(12f, 1f, 12f);
            ground.GetComponent<Renderer>().material = BuildMaterial(new Color(0.3f, 0.32f, 0.36f));

            ground.OnCollisionEnter(c =>
            {
                landings++;
                lastEvent = "collision: " + c.collider.name + " landed";
            });
        }

        private void BuildTriggerZone()
        {
            GameObject zone = GameObject.CreatePrimitive(PrimitiveType.Cube);
            zone.name = "TriggerZone";
            zone.transform.position = new Vector3(0f, 2.5f, 0f);
            zone.transform.localScale = new Vector3(6f, 2f, 6f);

            Collider collider = zone.GetComponent<Collider>();
            collider.isTrigger = true;

            Renderer renderer = zone.GetComponent<Renderer>();
            renderer.material = BuildTransparentMaterial(new Color(0.2f, 0.9f, 0.4f, 0.18f));

            zone.PhysicsEvents()
                .OnTriggerEnter(other =>
                {
                    triggerInside++;
                    triggerEnterTotal++;
                    lastEvent = "trigger enter: " + other.name;
                })
                .OnTriggerExit(other =>
                {
                    triggerInside--;
                    triggerExitTotal++;
                    lastEvent = "trigger exit: " + other.name;
                });
        }

        private void DropCube()
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = "Cube";
            cube.transform.position = new Vector3(Random.Range(-2f, 2f), 7f, Random.Range(-2f, 2f));
            cube.GetComponent<Renderer>().material = fallingMaterial;
            cube.AddComponent<Rigidbody>();
            Destroy(cube, 6f);
        }

        private void OnGUI()
        {
            EnsureStyles();

            GUILayout.BeginArea(new Rect(12f, 12f, 460f, 300f), boxStyle);
            GUILayout.Label("CatAnnaDev - Physics events demo", titleStyle);
            GUILayout.Space(4f);
            GUILayout.Label(
                "Zero-boilerplate collision/trigger callbacks. No custom MonoBehaviour:\n" +
                "zone.OnTriggerEnter(...).OnTriggerExit(...) and ground.OnCollisionEnter(...).",
                bodyStyle);

            GUILayout.Space(6f);
            if (GUILayout.Button("Drop a cube   [Space]")) DropCube();

            GUILayout.Space(8f);
            GUILayout.Label("Trigger zone (green)", titleStyle);
            GUILayout.Label(
                "currently inside : " + triggerInside +
                "\nenter total : " + triggerEnterTotal +
                "   exit total : " + triggerExitTotal,
                bodyStyle);

            GUILayout.Space(4f);
            GUILayout.Label("Ground collisions : " + landings, bodyStyle);
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

        private static Material BuildMaterial(Color color)
        {
            Material material = new Material(Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard"));
            material.color = color;
            return material;
        }

        private static Material BuildTransparentMaterial(Color color)
        {
            Material material = BuildMaterial(color);
            material.SetFloat("_Surface", 1f);
            material.SetFloat("_Mode", 3f);
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.renderQueue = 3000;
            material.color = color;
            return material;
        }

        private static void EnsureCamera()
        {
            if (Camera.main != null) return;
            GameObject go = new GameObject("Main Camera");
            go.tag = "MainCamera";
            Camera camera = go.AddComponent<Camera>();
            camera.transform.position = new Vector3(0f, 6f, -11f);
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
