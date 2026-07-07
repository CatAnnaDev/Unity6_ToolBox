using System.Collections.Generic;
using UnityEngine;
using CatAnnaDev.Utils;

namespace CatAnnaDev.Samples
{
    public sealed class UtilsDemo : MonoBehaviour
    {
        private const int MaxLogLines = 10;

        private readonly List<string> log = new List<string>();

        private readonly List<string> lootTable = new List<string>
        {
            "Sword", "Shield", "Potion", "Gold", "Gem"
        };

        private readonly WeightedList<string> rarity = new WeightedList<string>();

        private readonly SerializableDictionary<string, int> inventory = new SerializableDictionary<string, int>();

        private MinMaxRange spawnRadius = new MinMaxRange(2f, 6f);

        private GameObject cube;
        private Material cubeMaterial;
        private Color baseColor = new Color(0.26f, 0.60f, 0.88f);

        private GUIStyle panelStyle;
        private GUIStyle titleStyle;
        private GUIStyle bodyStyle;
        private bool stylesReady;

        private void Awake()
        {
            cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = "UtilsDemo Cube";
            cube.transform.Reset();
            cube.transform.SetPositionY(0.5f);

            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
            {
                shader = Shader.Find("Standard");
            }
            if (shader == null)
            {
                shader = Shader.Find("Sprites/Default");
            }
            cubeMaterial = new Material(shader);
            ApplyCubeColor(baseColor);
            cube.GetComponent<Renderer>().sharedMaterial = cubeMaterial;

            rarity.Add("Common", 70f);
            rarity.Add("Rare", 25f);
            rarity.Add("Legendary", 5f);

            inventory["Gold"] = 100;
            inventory["Potion"] = 3;

            if (Camera.main == null)
            {
                GameObject camGo = new GameObject("Main Camera");
                camGo.tag = "MainCamera";
                Camera cam = camGo.AddComponent<Camera>();
                cam.transform.position = new Vector3(0f, 3f, -8f);
                cam.transform.LookAt(Vector3.zero);
            }

            Append("Ready. Use the buttons or number keys 1-9.");
        }

        private void Update()
        {
            if (DemoInput.GetKeyDown(KeyCode.Alpha1)) DemoTransformAndVector();
            if (DemoInput.GetKeyDown(KeyCode.Alpha2)) DemoColor();
            if (DemoInput.GetKeyDown(KeyCode.Alpha3)) DemoRemap();
            if (DemoInput.GetKeyDown(KeyCode.Alpha4)) DemoRandomUtils();
            if (DemoInput.GetKeyDown(KeyCode.Alpha5)) DemoShuffleAndPick();
            if (DemoInput.GetKeyDown(KeyCode.Alpha6)) DemoWeightedList();
            if (DemoInput.GetKeyDown(KeyCode.Alpha7)) DemoMinMaxRange();
            if (DemoInput.GetKeyDown(KeyCode.Alpha8)) DemoDictionaryAndPool();
            if (DemoInput.GetKeyDown(KeyCode.Alpha9)) DemoCoroutineRunner();
        }

        private void DemoTransformAndVector()
        {
            Vector3 target = Vector3.zero.RandomInsideRadiusFlat(spawnRadius.Random).WithY(0.5f);
            cube.transform.position = target;
            cube.transform.AddPosition(Vector3.up * 0.01f);

            Vector3 snapped = target.SnapToGrid(0.5f);
            float manhattan = target.ManhattanDistance(Vector3.zero);

            Append("Moved cube to " + Format(target));
            Append("SnapToGrid(0.5) -> " + Format(snapped) + "  ManhattanDist=" + manhattan.ToString("F2"));
        }

        private void DemoColor()
        {
            Color picked = RandomUtils.RandomColor();
            baseColor = picked;
            ApplyCubeColor(picked);

            Color lighter = picked.Lighten(0.4f);
            Color inverted = picked.Invert();

            Append("RandomColor -> " + picked.ToHex());
            Append("Lighten(0.4)=" + lighter.ToHex() + "  Invert=" + inverted.ToHex());
            Append("Luminance=" + picked.Luminance().ToString("F2"));
        }

        private void DemoRemap()
        {
            float mouseX = DemoInput.MousePosition.x;
            float remapped = MathUtils.Remap(mouseX, 0f, Screen.width, -5f, 5f);
            cube.transform.SetPositionX(remapped);

            float clamped = MathUtils.RemapClamped(mouseX, 0f, Screen.width, 0f, 1f);
            Append("Remap mouseX " + mouseX.ToString("F0") + " [0.." + Screen.width + "] -> x=" + remapped.ToString("F2"));
            Append("RemapClamped -> t=" + clamped.ToString("F2") + " (move mouse, press 3 again)");
        }

        private void DemoRandomUtils()
        {
            float[] weights = { 1f, 3f, 6f };
            int index = RandomUtils.WeightedIndex(weights);
            Vector3 inSphere = RandomUtils.PointInsideUnitSphere();

            Append("WeightedIndex({1,3,6}) -> " + index + " (heavier = last)");
            Append("PointInsideUnitSphere -> " + Format(inSphere));
            Append("Chance(0.5)=" + RandomUtils.Chance(0.5f) + "  RandomSign=" + RandomUtils.RandomSign());
        }

        private void DemoShuffleAndPick()
        {
            lootTable.Shuffle();
            string pick = lootTable.RandomElement();

            Append("Shuffle -> [" + string.Join(", ", lootTable) + "]");
            Append("RandomElement -> " + pick);
        }

        private void DemoWeightedList()
        {
            string drop = rarity.Sample();
            Append("WeightedList.Sample -> " + drop + "  (70/25/5)");
        }

        private void DemoMinMaxRange()
        {
            float radius = spawnRadius.Random;
            Append("MinMaxRange[" + spawnRadius.Min + ".." + spawnRadius.Max + "].Random -> " + radius.ToString("F2"));
            Append("Center=" + spawnRadius.Center.ToString("F2") + "  Contains(4)=" + spawnRadius.Contains(4f));
        }

        private void DemoDictionaryAndPool()
        {
            inventory["Gold"] = inventory.GetOrDefault("Gold", 0) + 10;

            using (ListPool<string>.Get(out List<string> scratch))
            {
                foreach (KeyValuePair<string, int> pair in inventory)
                {
                    scratch.Add(pair.Key + " x" + pair.Value);
                }
                Append("Inventory (via pooled list) -> " + string.Join(", ", scratch));
            }

            Append("ListPool released the scratch list back to the pool.");
        }

        private void DemoCoroutineRunner()
        {
            Append("CoroutineRunner.RunDelayed(1s) scheduled...");
            CoroutineRunner.RunDelayed(1f, () =>
            {
                ApplyCubeColor(baseColor.Invert());
                Append("Delayed callback fired: cube color inverted.");
            });
        }

        private void ApplyCubeColor(Color color)
        {
            if (cubeMaterial == null)
            {
                return;
            }
            if (cubeMaterial.HasProperty("_BaseColor"))
            {
                cubeMaterial.SetColor("_BaseColor", color);
            }
            if (cubeMaterial.HasProperty("_Color"))
            {
                cubeMaterial.SetColor("_Color", color);
            }
        }

        private static string Format(Vector3 v)
        {
            return "(" + v.x.ToString("F2") + ", " + v.y.ToString("F2") + ", " + v.z.ToString("F2") + ")";
        }

        private void Append(string line)
        {
            log.Add(line);
            while (log.Count > MaxLogLines)
            {
                log.RemoveAt(0);
            }
        }

        private void OnGUI()
        {
            EnsureStyles();

            GUILayout.BeginArea(new Rect(10f, 10f, 540f, 620f), panelStyle);

            GUILayout.Label("CatAnnaDev Utils Tour", titleStyle);
            GUILayout.Label(
                "A grab-bag of extension methods and helpers for everyday gameplay code:\n" +
                "Transform/Vector/Color extensions, MathUtils, RandomUtils, collection\n" +
                "helpers, WeightedList, MinMaxRange, SerializableDictionary, pooling and\n" +
                "CoroutineRunner. Each button calls the real API and logs the result.",
                bodyStyle);

            GUILayout.Space(6f);

            DrawRow("1  Transform + Vector3", "Move/snap the cube", DemoTransformAndVector);
            DrawRow("2  Color extensions", "Recolor + hex/lighten/invert", DemoColor);
            DrawRow("3  MathUtils.Remap", "Map mouse X to world X", DemoRemap);
            DrawRow("4  RandomUtils", "WeightedIndex + point in sphere", DemoRandomUtils);
            DrawRow("5  Shuffle + RandomElement", "On the loot list", DemoShuffleAndPick);
            DrawRow("6  WeightedList.Sample", "70/25/5 rarity roll", DemoWeightedList);
            DrawRow("7  MinMaxRange.Random", "Sample a float range", DemoMinMaxRange);
            DrawRow("8  Dictionary + ListPool", "GetOrDefault + pooled list", DemoDictionaryAndPool);
            DrawRow("9  CoroutineRunner", "RunDelayed(1s) callback", DemoCoroutineRunner);

            GUILayout.Space(8f);
            GUILayout.Label("Output:", titleStyle);
            for (int i = 0; i < log.Count; i++)
            {
                GUILayout.Label(log[i], bodyStyle);
            }

            GUILayout.EndArea();
        }

        private void DrawRow(string key, string description, System.Action action)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(key, GUILayout.Width(210f)))
            {
                action();
            }
            GUILayout.Label(description, bodyStyle);
            GUILayout.EndHorizontal();
        }

        private void EnsureStyles()
        {
            if (stylesReady)
            {
                return;
            }

            panelStyle = new GUIStyle(GUI.skin.box);
            panelStyle.padding = new RectOffset(12, 12, 12, 12);

            titleStyle = new GUIStyle(GUI.skin.label);
            titleStyle.fontSize = 16;
            titleStyle.fontStyle = FontStyle.Bold;

            bodyStyle = new GUIStyle(GUI.skin.label);
            bodyStyle.fontSize = 12;
            bodyStyle.wordWrap = true;

            stylesReady = true;
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
    }
}
