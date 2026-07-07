using UnityEngine;
using CatAnnaDev.Pooling;

namespace CatAnnaDev.Samples
{
    public sealed class PoolingDemo : MonoBehaviour
    {
        const int PrewarmCount = 16;
        const int BurstCount = 25;
        const float Lifetime = 2.5f;

        GameObject _autoPrefab;
        GameObject _manualPrefab;
        Material _autoMaterial;
        Material _manualMaterial;
        Material _naiveMaterial;

        GameObject _ground;
        GameObject _createdCamera;
        GameObject _createdLight;

        int _naiveInstantiateTotal;
        int _lastBurstSize;
        string _lastAction = "none yet";

        GUIStyle _boxStyle;
        GUIStyle _titleStyle;
        GUIStyle _bodyStyle;

        void Awake()
        {
            _autoMaterial = BuildMaterial(new Color(0.30f, 0.75f, 1.00f));
            _manualMaterial = BuildMaterial(new Color(1.00f, 0.62f, 0.25f));
            _naiveMaterial = BuildMaterial(new Color(0.85f, 0.30f, 0.35f));

            _autoPrefab = BuildCubePrefab("PooledCube_Auto", _autoMaterial, true);
            _manualPrefab = BuildCubePrefab("PooledCube_Manual", _manualMaterial, false);

            EnsureGround();
            EnsureCamera();
            EnsureLight();
        }

        void Start()
        {
            Pool.Prewarm(_autoPrefab, PrewarmCount);
            Pool.Prewarm(_manualPrefab, PrewarmCount);
            _lastAction = "prewarmed " + PrewarmCount + " of each pool on Start";
        }

        void Update()
        {
            if (DemoInput.GetKeyDown(KeyCode.Alpha1)) { SpawnAutoBurst(); }
            if (DemoInput.GetKeyDown(KeyCode.Alpha2)) { SpawnManualBurst(); }
            if (DemoInput.GetKeyDown(KeyCode.Alpha3)) { SpawnNaiveBurst(); }
            if (DemoInput.GetKeyDown(KeyCode.Alpha4)) { PrewarmMore(); }
            if (DemoInput.GetKeyDown(KeyCode.Alpha5)) { ClearPools(); }
        }

        void OnGUI()
        {
            EnsureStyles();

            var area = new Rect(12f, 12f, 620f, 560f);
            GUILayout.BeginArea(area, _boxStyle);

            GUILayout.Label("CatAnnaDev - Pool demo", _titleStyle);
            GUILayout.Space(4f);
            GUILayout.Label(
                "Pool reuses GameObjects instead of Instantiate/Destroy every time.\n" +
                "Spawn hands you a live instance from an inactive stack (or grows it);\n" +
                "Despawn returns it for reuse. Reused spawns cost no allocation and no\n" +
                "GC, so bursts of short-lived objects (bullets, hits, debris) stay cheap.\n" +
                "Watch 'misses' stay low and 'reuse' climb toward 1.00 as you re-spawn.",
                _bodyStyle);

            GUILayout.Space(6f);
            GUILayout.Label("Controls", _titleStyle);
            GUILayout.Label(
                "[1] / button  Spawn burst, AutoDespawn returns each after " + Lifetime.ToString("0.0") + "s\n" +
                "[2] / button  Spawn burst, Pool.Despawn(go, " + Lifetime.ToString("0.0") + "s) returns each\n" +
                "[3] / button  Naive Instantiate + Destroy burst (no pooling, allocates)\n" +
                "[4] / button  Pool.Prewarm both pools with " + PrewarmCount + " more instances\n" +
                "[5] / button  Pool.ClearAll (destroys every pooled instance)",
                _bodyStyle);

            GUILayout.Space(6f);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("1: Auto burst")) { SpawnAutoBurst(); }
            if (GUILayout.Button("2: Manual-delay burst")) { SpawnManualBurst(); }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("3: Naive Instantiate/Destroy")) { SpawnNaiveBurst(); }
            if (GUILayout.Button("4: Prewarm more")) { PrewarmMore(); }
            GUILayout.EndHorizontal();

            if (GUILayout.Button("5: ClearAll")) { ClearPools(); }

            GUILayout.Space(8f);
            GUILayout.Label("Auto pool (blue, AutoDespawn)", _titleStyle);
            GUILayout.Label(StatsLine(_autoPrefab), _bodyStyle);

            GUILayout.Space(4f);
            GUILayout.Label("Manual pool (orange, Pool.Despawn delay)", _titleStyle);
            GUILayout.Label(StatsLine(_manualPrefab), _bodyStyle);

            GUILayout.Space(4f);
            GUILayout.Label("Naive baseline (red, no pool)", _titleStyle);
            GUILayout.Label(
                "instantiated total : " + _naiveInstantiateTotal +
                "  (each is a fresh alloc + Destroy, never reused)",
                _bodyStyle);

            GUILayout.Space(6f);
            GUILayout.Label(
                "last: " + _lastAction + "  (burst size " + _lastBurstSize + ")",
                _bodyStyle);

            GUILayout.EndArea();
        }

        void SpawnAutoBurst()
        {
            for (int i = 0; i < BurstCount; i++)
            {
                GameObject go = Pool.Spawn(_autoPrefab, SpawnPoint(), Random.rotation);
                if (go == null) continue;
                Launch(go);
            }
            _lastBurstSize = BurstCount;
            _lastAction = "spawned auto burst (AutoDespawn returns them)";
        }

        void SpawnManualBurst()
        {
            for (int i = 0; i < BurstCount; i++)
            {
                GameObject go = Pool.Spawn(_manualPrefab, SpawnPoint(), Random.rotation);
                if (go == null) continue;
                Launch(go);
                Pool.Despawn(go, Lifetime);
            }
            _lastBurstSize = BurstCount;
            _lastAction = "spawned manual burst (Pool.Despawn delay returns them)";
        }

        void SpawnNaiveBurst()
        {
            for (int i = 0; i < BurstCount; i++)
            {
                GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                go.name = "NaiveCube";
                go.transform.SetPositionAndRotation(SpawnPoint(), Random.rotation);
                go.GetComponent<Renderer>().sharedMaterial = _naiveMaterial;
                Launch(go);
                Destroy(go, Lifetime);
                _naiveInstantiateTotal++;
            }
            _lastBurstSize = BurstCount;
            _lastAction = "spawned naive burst (Instantiate now, Destroy after delay)";
        }

        void PrewarmMore()
        {
            Pool.Prewarm(_autoPrefab, PrewarmCount);
            Pool.Prewarm(_manualPrefab, PrewarmCount);
            _lastBurstSize = 0;
            _lastAction = "prewarmed " + PrewarmCount + " more into each pool";
        }

        void ClearPools()
        {
            Pool.ClearAll();
            _lastBurstSize = 0;
            _lastAction = "Pool.ClearAll (pooled instances destroyed)";
        }

        void Launch(GameObject go)
        {
            Rigidbody rb = go.GetComponent<Rigidbody>();
            if (rb == null) rb = go.AddComponent<Rigidbody>();
            rb.linearVelocity = new Vector3(Random.Range(-3f, 3f), Random.Range(6f, 10f), Random.Range(-3f, 3f));
            rb.angularVelocity = Random.insideUnitSphere * 6f;
        }

        Vector3 SpawnPoint()
        {
            return new Vector3(Random.Range(-2f, 2f), 1f, Random.Range(-2f, 2f));
        }

        string StatsLine(GameObject prefab)
        {
            if (PoolManager.HasInstance && PoolManager.Instance.TryGetStats(prefab, out PoolStats s))
            {
                return "active=" + s.active +
                       "  inactive=" + s.inactive +
                       "  total=" + s.total +
                       "  peak=" + s.peakActive + "\n" +
                       "spawned=" + s.totalSpawned +
                       "  released=" + s.totalReleased +
                       "  misses=" + s.misses +
                       "  reuse=" + s.ReuseRatio.ToString("0.00");
            }
            return "no pool yet (press a spawn key)";
        }

        GameObject BuildCubePrefab(string prefabName, Material material, bool withAutoDespawn)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = prefabName;
            go.transform.SetParent(transform, false);
            go.GetComponent<Renderer>().sharedMaterial = material;
            go.AddComponent<Rigidbody>();

            if (withAutoDespawn)
            {
                AutoDespawn despawn = go.AddComponent<AutoDespawn>();
                despawn.Mode = AutoDespawnMode.Lifetime;
                despawn.Lifetime = Lifetime;
            }

            go.SetActive(false);
            return go;
        }

        Material BuildMaterial(Color color)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            if (shader == null) shader = Shader.Find("Sprites/Default");

            Material material = new Material(shader);
            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
            if (material.HasProperty("_Color")) material.SetColor("_Color", color);
            return material;
        }

        void EnsureGround()
        {
            _ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            _ground.name = "PoolingDemoGround";
            _ground.transform.localScale = new Vector3(3f, 1f, 3f);
            _ground.GetComponent<Renderer>().sharedMaterial = BuildMaterial(new Color(0.18f, 0.18f, 0.2f));
        }

        void EnsureCamera()
        {
            if (Camera.main != null) return;

            _createdCamera = new GameObject("PoolingDemoCamera");
            Camera camera = _createdCamera.AddComponent<Camera>();
            _createdCamera.tag = "MainCamera";
            _createdCamera.transform.position = new Vector3(0f, 6f, -12f);
            _createdCamera.transform.rotation = Quaternion.Euler(20f, 0f, 0f);
            camera.backgroundColor = new Color(0.08f, 0.09f, 0.12f);
            camera.clearFlags = CameraClearFlags.SolidColor;
        }

        void EnsureLight()
        {
            if (FindFirstObjectByType<Light>() != null) return;

            _createdLight = new GameObject("PoolingDemoLight");
            Light light = _createdLight.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.1f;
            _createdLight.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        }

        void EnsureStyles()
        {
            if (_boxStyle != null) return;

            _boxStyle = new GUIStyle(GUI.skin.box)
            {
                padding = new RectOffset(12, 12, 12, 12),
                alignment = TextAnchor.UpperLeft
            };

            _titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 15,
                fontStyle = FontStyle.Bold
            };

            _bodyStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                richText = false,
                wordWrap = false
            };
        }

        void OnDisable()
        {
            Pool.ClearAll();

            if (_autoPrefab != null) Destroy(_autoPrefab);
            if (_manualPrefab != null) Destroy(_manualPrefab);
            if (_ground != null) Destroy(_ground);
            if (_createdCamera != null) Destroy(_createdCamera);
            if (_createdLight != null) Destroy(_createdLight);

            if (_autoMaterial != null) Destroy(_autoMaterial);
            if (_manualMaterial != null) Destroy(_manualMaterial);
            if (_naiveMaterial != null) Destroy(_naiveMaterial);
        }
    }
}
