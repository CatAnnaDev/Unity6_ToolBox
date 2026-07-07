using System;
using UnityEngine;
using CatAnnaDev.Saving;

namespace CatAnnaDev.Samples
{
    [AddComponentMenu("CatAnnaDev/Samples/Saving Demo")]
    public sealed class SavingDemo : MonoBehaviour, ISaveable
    {
        private const string Slot = "demo-slot-0";
        private const string Key = "cat-anna-dev.saving-demo";

        private static readonly string[] NamePool = { "Anna", "Nyx", "Pixel", "Cobalt", "Juno" };

        private GameObject cube;
        private Material cubeMaterial;
        private Camera cam;

        private string playerName = "Anna";
        private int score;
        private int nameIndex;

        private string status = "Ready. Move the cube, then press Save.";
        private string savePath = string.Empty;
        private GUIStyle boxStyle;
        private GUIStyle titleStyle;

        public string SaveKey
        {
            get { return Key; }
        }

        private void OnEnable()
        {
            SaveSystem.Register(this);
            SaveSystem.Saved += HandleSaved;
            SaveSystem.Loaded += HandleLoaded;
            SaveSystem.Deleted += HandleDeleted;
        }

        private void OnDisable()
        {
            SaveSystem.Saved -= HandleSaved;
            SaveSystem.Loaded -= HandleLoaded;
            SaveSystem.Deleted -= HandleDeleted;
            SaveSystem.Unregister(this);
        }

        private void Start()
        {
            EnsureCamera();
            BuildCube();
            RefreshSavePath();
        }

        private void OnDestroy()
        {
            if (cube != null)
            {
                Destroy(cube);
            }

            if (cubeMaterial != null)
            {
                Destroy(cubeMaterial);
            }
        }

        private void Update()
        {
            if (cube == null)
            {
                return;
            }

            Vector3 move = Vector3.zero;
            if (DemoInput.GetKey(KeyCode.W)) move.z += 1f;
            if (DemoInput.GetKey(KeyCode.S)) move.z -= 1f;
            if (DemoInput.GetKey(KeyCode.A)) move.x -= 1f;
            if (DemoInput.GetKey(KeyCode.D)) move.x += 1f;

            if (move != Vector3.zero)
            {
                cube.transform.position += move.normalized * (4f * Time.deltaTime);
            }

            if (DemoInput.GetKeyDown(KeyCode.Space))
            {
                score += 10;
            }

            if (DemoInput.GetKeyDown(KeyCode.N))
            {
                nameIndex = (nameIndex + 1) % NamePool.Length;
                playerName = NamePool[nameIndex];
            }

            if (DemoInput.GetKeyDown(KeyCode.F5)) SaveSystem.Save(Slot);
            if (DemoInput.GetKeyDown(KeyCode.F9)) SaveSystem.Load(Slot);
        }

        public object CaptureState()
        {
            return new DemoState
            {
                playerName = playerName,
                score = score,
                cubePosition = cube != null ? cube.transform.position : Vector3.zero
            };
        }

        public void RestoreState(object state)
        {
            DemoState restored = state as DemoState;
            if (restored == null)
            {
                return;
            }

            playerName = restored.playerName;
            score = restored.score;

            if (cube != null)
            {
                cube.transform.position = restored.cubePosition;
            }
        }

        private void OnGUI()
        {
            EnsureStyles();

            float width = 480f;
            float height = 360f;
            GUILayout.BeginArea(new Rect(12f, 12f, width, height), boxStyle);

            GUILayout.Label("CatAnnaDev - Saving Demo", titleStyle);
            GUILayout.Label("SaveSystem persists any registered ISaveable to a named slot on disk. This component captures a player name, a score and the cube's position, then writes them through SaveSystem.Save.");

            GUILayout.Space(6f);
            GUILayout.Label("Controls: W/A/S/D move cube - Space +10 score - N cycle name - F5 save - F9 load");

            GUILayout.Space(6f);
            GUILayout.Label("Live state:  name = " + playerName + "   score = " + score + "   pos = " + FormatPosition());
            GUILayout.Label("Slot: '" + Slot + "'   exists on disk: " + SaveSystem.HasSlot(Slot));
            GUILayout.Label("File: " + savePath);

            GUILayout.Space(8f);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save", GUILayout.Height(30f))) SaveSystem.Save(Slot);
            if (GUILayout.Button("Load", GUILayout.Height(30f))) SaveSystem.Load(Slot);
            if (GUILayout.Button("Delete", GUILayout.Height(30f))) SaveSystem.Delete(Slot);
            GUILayout.EndHorizontal();

            GUILayout.Space(4f);
            if (GUILayout.Button("Peek slot contents (ReadData)", GUILayout.Height(26f)))
            {
                PeekSlot();
            }

            GUILayout.Space(6f);
            GUILayout.Label("Status: " + status);

            GUILayout.EndArea();
        }

        private void PeekSlot()
        {
            SaveData data;
            if (!SaveSystem.ReadData(Slot, out data))
            {
                status = "No slot on disk yet. Press Save first.";
                return;
            }

            DemoState stored;
            if (data.TryGetObject(Key, out stored) && stored != null)
            {
                status = "Slot holds  name=" + stored.playerName + " score=" + stored.score + " pos=" + stored.cubePosition;
            }
            else
            {
                status = "Slot exists but had no entry for this demo key.";
            }
        }

        private void HandleSaved(string slot)
        {
            if (slot == Slot)
            {
                RefreshSavePath();
                status = "Saved to '" + slot + "'.";
            }
        }

        private void HandleLoaded(string slot)
        {
            if (slot == Slot)
            {
                status = "Loaded from '" + slot + "'.";
            }
        }

        private void HandleDeleted(string slot)
        {
            if (slot == Slot)
            {
                status = "Deleted slot '" + slot + "'.";
            }
        }

        private void RefreshSavePath()
        {
            savePath = System.IO.Path.Combine(SaveSystem.SaveDirectory, Slot + SaveSystem.Serializer.Extension);
        }

        private string FormatPosition()
        {
            if (cube == null)
            {
                return "(none)";
            }

            Vector3 p = cube.transform.position;
            return "(" + p.x.ToString("0.0") + ", " + p.y.ToString("0.0") + ", " + p.z.ToString("0.0") + ")";
        }

        private void BuildCube()
        {
            cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = "SavingDemoCube";
            cube.transform.position = Vector3.zero;

            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
            {
                shader = Shader.Find("Standard");
            }

            if (shader != null)
            {
                cubeMaterial = new Material(shader);
                cubeMaterial.color = new Color(0.35f, 0.7f, 1f);
                Renderer renderer = cube.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material = cubeMaterial;
                }
            }
        }

        private void EnsureCamera()
        {
            cam = Camera.main;
            if (cam == null)
            {
                GameObject camObject = new GameObject("SavingDemoCamera");
                cam = camObject.AddComponent<Camera>();
                camObject.tag = "MainCamera";
            }

            cam.transform.position = new Vector3(0f, 6f, -8f);
            cam.transform.rotation = Quaternion.Euler(35f, 0f, 0f);
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.09f, 0.10f, 0.13f);
        }

        private void EnsureStyles()
        {
            if (boxStyle == null)
            {
                boxStyle = new GUIStyle(GUI.skin.box);
                boxStyle.padding = new RectOffset(12, 12, 12, 12);
                boxStyle.alignment = TextAnchor.UpperLeft;
                boxStyle.normal.textColor = Color.white;
            }

            if (titleStyle == null)
            {
                titleStyle = new GUIStyle(GUI.skin.label);
                titleStyle.fontSize = 16;
                titleStyle.fontStyle = FontStyle.Bold;
                titleStyle.normal.textColor = Color.white;
            }
        }

        [Serializable]
        private sealed class DemoState
        {
            public string playerName;
            public int score;
            public Vector3 cubePosition;
        }
    }
}
