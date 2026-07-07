using System.Reflection;
using UnityEngine;
using CatAnnaDev.Pooling;
using CatAnnaDev.VFX;

namespace CatAnnaDev.Samples
{
    public sealed class VFXDemo : MonoBehaviour
    {
        [SerializeField] private float spawnRadius = 4f;
        [SerializeField] private int burstCount = 12;
        [SerializeField] private int prewarmCount = 8;

        private GameObject particlePrefab;
        private Material particleMaterial;
        private VFXData vfxData;
        private int totalPlayCalls;
        private string lastAction = "nothing yet";

        private void OnEnable()
        {
            particleMaterial = BuildParticleMaterial();
            particlePrefab = BuildParticlePrefab(particleMaterial);
            vfxData = WrapPrefabInData(particlePrefab);
        }

        private void OnDisable()
        {
            if (PoolManager.HasInstance && particlePrefab != null)
            {
                PoolManager.Instance.ClearPool(particlePrefab);
            }

            if (vfxData != null)
            {
                Destroy(vfxData);
                vfxData = null;
            }

            if (particlePrefab != null)
            {
                Destroy(particlePrefab);
                particlePrefab = null;
            }

            if (particleMaterial != null)
            {
                Destroy(particleMaterial);
                particleMaterial = null;
            }
        }

        private void Update()
        {
            if (DemoInput.GetKeyDown(KeyCode.Space))
            {
                SpawnOne();
            }

            if (DemoInput.GetKeyDown(KeyCode.B))
            {
                SpawnBurst(burstCount);
            }

            if (DemoInput.GetKeyDown(KeyCode.P))
            {
                PrewarmPool();
            }

            if (DemoInput.GetKeyDown(KeyCode.C))
            {
                ClearPool();
            }
        }

        private void SpawnOne()
        {
            Vector3 position = RandomPosition();
            Quaternion rotation = Random.rotationUniform;
            VFXManager.Instance.Play(vfxData, position, rotation);
            totalPlayCalls++;
            lastAction = "Played 1 effect via VFXData";
        }

        private void SpawnBurst(int count)
        {
            for (int i = 0; i < count; i++)
            {
                VFXManager.Instance.Play(vfxData, RandomPosition(), Random.rotationUniform);
                totalPlayCalls++;
            }
            lastAction = "Played a burst of " + count + " effects";
        }

        private void PrewarmPool()
        {
            VFXManager.Instance.Prewarm(particlePrefab, prewarmCount);
            lastAction = "Prewarmed pool by " + prewarmCount;
        }

        private void ClearPool()
        {
            if (PoolManager.HasInstance)
            {
                PoolManager.Instance.ClearPool(particlePrefab);
            }
            lastAction = "Cleared inactive pooled instances";
        }

        private Vector3 RandomPosition()
        {
            Vector3 offset = Random.insideUnitSphere * spawnRadius;
            offset.y = Mathf.Abs(offset.y);
            return transform.position + offset;
        }

        private static Material BuildParticleMaterial()
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
            if (shader == null)
            {
                shader = Shader.Find("Particles/Standard Unlit");
            }
            if (shader == null)
            {
                shader = Shader.Find("Sprites/Default");
            }

            Material material = new Material(shader);
            material.color = new Color(0.3f, 0.8f, 1f, 1f);
            return material;
        }

        private static GameObject BuildParticlePrefab(Material material)
        {
            GameObject root = new GameObject("VFXDemo_ParticlePrefab");
            root.SetActive(false);

            ParticleSystem system = root.AddComponent<ParticleSystem>();

            ParticleSystem.MainModule main = system.main;
            main.duration = 1f;
            main.loop = false;
            main.playOnAwake = false;
            main.startLifetime = 1.1f;
            main.startSpeed = 5f;
            main.startSize = 0.18f;
            main.gravityModifier = 0.4f;
            main.startColor = new Color(0.4f, 0.9f, 1f, 1f);
            main.maxParticles = 200;

            ParticleSystem.EmissionModule emission = system.emission;
            emission.enabled = true;
            emission.rateOverTime = 0f;
            emission.SetBursts(new[] { new ParticleSystem.Burst(0f, (short)40) });

            ParticleSystem.ShapeModule shape = system.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 0.15f;

            ParticleSystemRenderer renderer = root.GetComponent<ParticleSystemRenderer>();
            renderer.material = material;

            root.AddComponent<PooledParticle>();
            return root;
        }

        private static VFXData WrapPrefabInData(GameObject prefab)
        {
            VFXData data = ScriptableObject.CreateInstance<VFXData>();
            data.name = "VFXDemo_RuntimeData";

            FieldInfo prefabField = typeof(VFXData).GetField("prefab", BindingFlags.NonPublic | BindingFlags.Instance);
            if (prefabField != null)
            {
                prefabField.SetValue(data, prefab);
            }

            FieldInfo yawField = typeof(VFXData).GetField("randomizeYaw", BindingFlags.NonPublic | BindingFlags.Instance);
            if (yawField != null)
            {
                yawField.SetValue(data, true);
            }

            return data;
        }

        private void OnGUI()
        {
            const float width = 430f;
            GUILayout.BeginArea(new Rect(12f, 12f, width, 460f), GUI.skin.box);

            GUIStyle title = new GUIStyle(GUI.skin.label) { fontSize = 18, fontStyle = FontStyle.Bold };
            GUIStyle body = new GUIStyle(GUI.skin.label) { fontSize = 13, wordWrap = true };
            GUIStyle stat = new GUIStyle(GUI.skin.label) { fontSize = 13, fontStyle = FontStyle.Bold };

            GUILayout.Label("CatAnnaDev VFX Demo", title);
            GUILayout.Label(
                "VFXManager plays a ParticleSystem prefab through a pool. " +
                "The prefab is built in code, wrapped in a VFXData asset at runtime, " +
                "and each played instance auto-returns to the pool when its particles finish " +
                "(PooledParticle in Auto / StopCallback mode).",
                body);

            GUILayout.Space(6f);
            GUILayout.Label("Controls", stat);
            GUILayout.Label("Space  -  play 1 effect at a random spot", body);
            GUILayout.Label("B      -  play a burst of " + burstCount + " effects", body);
            GUILayout.Label("P      -  prewarm the pool by " + prewarmCount, body);
            GUILayout.Label("C      -  clear inactive pooled instances", body);

            GUILayout.Space(6f);
            if (GUILayout.Button("Play 1 (Space)")) SpawnOne();
            if (GUILayout.Button("Play burst of " + burstCount + " (B)")) SpawnBurst(burstCount);
            if (GUILayout.Button("Prewarm " + prewarmCount + " (P)")) PrewarmPool();
            if (GUILayout.Button("Clear pool (C)")) ClearPool();

            GUILayout.Space(6f);
            GUILayout.Label("Pool status", stat);
            if (PoolManager.HasInstance && particlePrefab != null &&
                PoolManager.Instance.TryGetStats(particlePrefab, out PoolStats stats))
            {
                GUILayout.Label("active (on screen)  : " + stats.active, body);
                GUILayout.Label("inactive (reserve)  : " + stats.inactive, body);
                GUILayout.Label("total instances     : " + stats.total, body);
                GUILayout.Label("peak active         : " + stats.peakActive, body);
                GUILayout.Label("reuse ratio         : " + stats.ReuseRatio.ToString("0.00"), body);
            }
            else
            {
                GUILayout.Label("Pool not created yet. Press Space to spawn.", body);
            }

            GUILayout.Space(4f);
            GUILayout.Label("Play() calls so far : " + totalPlayCalls, body);
            GUILayout.Label("Last action         : " + lastAction, body);

            GUILayout.EndArea();
        }
    }
}
