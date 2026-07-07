using System.Collections.Generic;
using CatAnnaDev.ScriptableArchitecture;
using UnityEngine;

namespace CatAnnaDev.Samples
{
    [AddComponentMenu("CatAnnaDev/Samples/Scriptable Architecture Demo")]
    public sealed class ScriptableArchitectureDemo : MonoBehaviour
    {
        private FloatVariable health;
        private IntVariable score;

        private FloatReference damagePerHit;

        private GameObjectRuntimeSet enemies;

        private readonly List<GameObject> spawnedEnemies = new List<GameObject>();

        private string healthLog = "waiting for changes...";
        private string scoreLog = "waiting for changes...";
        private string setLog = "empty";

        private int spawnCounter;

        private void OnEnable()
        {
            health = ScriptableObject.CreateInstance<FloatVariable>();
            health.name = "HealthVariable";
            health.InitialValue = 100f;
            health.ResetOnPlay = false;
            health.SetValueSilently(100f);
            health.OnValueChanged += HandleHealthChanged;

            score = ScriptableObject.CreateInstance<IntVariable>();
            score.name = "ScoreVariable";
            score.InitialValue = 0;
            score.ResetOnPlay = false;
            score.SetValueSilently(0);
            score.OnValueChanged += HandleScoreChanged;

            damagePerHit = new FloatReference(10f);

            enemies = ScriptableObject.CreateInstance<GameObjectRuntimeSet>();
            enemies.name = "EnemyRuntimeSet";
            enemies.OnItemAdded += HandleEnemyAdded;
            enemies.OnItemRemoved += HandleEnemyRemoved;
            enemies.OnChanged += HandleSetChanged;
        }

        private void OnDisable()
        {
            if (health != null)
            {
                health.OnValueChanged -= HandleHealthChanged;
                Destroy(health);
                health = null;
            }

            if (score != null)
            {
                score.OnValueChanged -= HandleScoreChanged;
                Destroy(score);
                score = null;
            }

            if (enemies != null)
            {
                enemies.OnItemAdded -= HandleEnemyAdded;
                enemies.OnItemRemoved -= HandleEnemyRemoved;
                enemies.OnChanged -= HandleSetChanged;
                Destroy(enemies);
                enemies = null;
            }

            for (int i = 0; i < spawnedEnemies.Count; i++)
            {
                if (spawnedEnemies[i] != null)
                {
                    Destroy(spawnedEnemies[i]);
                }
            }

            spawnedEnemies.Clear();
        }

        private void HandleHealthChanged(float value)
        {
            healthLog = "OnValueChanged -> health = " + value.ToString("0.0");
        }

        private void HandleScoreChanged(int value)
        {
            scoreLog = "OnValueChanged -> score = " + value;
        }

        private void HandleEnemyAdded(GameObject go)
        {
            setLog = "added " + go.name;
        }

        private void HandleEnemyRemoved(GameObject go)
        {
            setLog = "removed " + go.name;
        }

        private void HandleSetChanged()
        {
        }

        private void SpawnEnemy()
        {
            GameObject enemy = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            enemy.name = "Enemy_" + spawnCounter;
            spawnCounter++;
            enemy.transform.position = new Vector3(Random.Range(-4f, 4f), 0.5f, Random.Range(-4f, 4f));

            spawnedEnemies.Add(enemy);
            enemies.Add(enemy);
        }

        private void RemoveLastEnemy()
        {
            for (int i = spawnedEnemies.Count - 1; i >= 0; i--)
            {
                GameObject candidate = spawnedEnemies[i];
                if (candidate != null && enemies.Contains(candidate))
                {
                    enemies.Remove(candidate);
                    spawnedEnemies.RemoveAt(i);
                    Destroy(candidate);
                    return;
                }
            }
        }

        private void Update()
        {
            if (DemoInput.GetKeyDown(KeyCode.Space))
            {
                health.Add(-damagePerHit.Value);
            }

            if (DemoInput.GetKeyDown(KeyCode.H))
            {
                health.Add(15f);
            }

            if (DemoInput.GetKeyDown(KeyCode.R))
            {
                health.ResetToInitial();
                health.ForceSetValue(health.InitialValue);
            }

            if (DemoInput.GetKeyDown(KeyCode.Alpha1))
            {
                score.Add(1);
            }

            if (DemoInput.GetKeyDown(KeyCode.Alpha0))
            {
                score.SetValue(0);
            }

            if (DemoInput.GetKeyDown(KeyCode.C))
            {
                damagePerHit.UseConstant = !damagePerHit.UseConstant;
            }

            if (DemoInput.GetKeyDown(KeyCode.E))
            {
                SpawnEnemy();
            }

            if (DemoInput.GetKeyDown(KeyCode.X))
            {
                RemoveLastEnemy();
            }

            if (DemoInput.GetKeyDown(KeyCode.K))
            {
                enemies.Clear();

                for (int i = 0; i < spawnedEnemies.Count; i++)
                {
                    if (spawnedEnemies[i] != null)
                    {
                        Destroy(spawnedEnemies[i]);
                    }
                }

                spawnedEnemies.Clear();
                setLog = "cleared";
            }
        }

        private void OnGUI()
        {
            const float width = 460f;

            GUILayout.BeginArea(new Rect(10f, 10f, width, 620f), GUI.skin.box);

            GUIStyle title = new GUIStyle(GUI.skin.label);
            title.fontSize = 18;
            title.fontStyle = FontStyle.Bold;

            GUIStyle body = new GUIStyle(GUI.skin.label);
            body.fontSize = 13;
            body.wordWrap = true;

            GUILayout.Label("Scriptable Architecture Demo", title);

            GUILayout.Label(
                "ScriptableObject Variables hold shared state as assets. Anything can " +
                "subscribe to OnValueChanged without knowing who mutates the value. " +
                "A Reference resolves either a constant or a Variable. A RuntimeSet " +
                "tracks a live collection of members.",
                body);

            GUILayout.Space(6f);
            GUILayout.Label("FloatVariable  (health)", title);
            GUILayout.Label("Value: " + health.Value.ToString("0.0") + "   Initial: " + health.InitialValue.ToString("0.0"), body);
            GUILayout.Label(healthLog, body);

            float sliderHealth = GUILayout.HorizontalSlider(health.Value, 0f, 100f);
            if (!Mathf.Approximately(sliderHealth, health.Value))
            {
                health.SetValue(sliderHealth);
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Space: -damage"))
            {
                health.Add(-damagePerHit.Value);
            }

            if (GUILayout.Button("H: +15"))
            {
                health.Add(15f);
            }

            if (GUILayout.Button("R: reset"))
            {
                health.ForceSetValue(health.InitialValue);
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(6f);
            GUILayout.Label("IntVariable  (score)", title);
            GUILayout.Label("Value: " + score.Value, body);
            GUILayout.Label(scoreLog, body);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("1: +1"))
            {
                score.Add(1);
            }

            if (GUILayout.Button("0: set 0"))
            {
                score.SetValue(0);
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(6f);
            GUILayout.Label("FloatReference  (damage per hit)", title);
            GUILayout.Label(
                "UseConstant: " + damagePerHit.UseConstant +
                "   Resolved Value: " + damagePerHit.Value.ToString("0.0"),
                body);
            GUILayout.Label(
                damagePerHit.UseConstant
                    ? "reading its own constant (10)"
                    : "reading the bound FloatVariable",
                body);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("C: toggle constant/variable"))
            {
                damagePerHit.UseConstant = !damagePerHit.UseConstant;
            }

            if (GUILayout.Button("bind health variable"))
            {
                damagePerHit.Variable = health;
                damagePerHit.UseConstant = false;
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(6f);
            GUILayout.Label("GameObjectRuntimeSet  (enemies)", title);
            GUILayout.Label("Count: " + enemies.Count + "   Last event: " + setLog, body);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("E: spawn"))
            {
                SpawnEnemy();
            }

            if (GUILayout.Button("X: remove last"))
            {
                RemoveLastEnemy();
            }

            if (GUILayout.Button("K: clear"))
            {
                enemies.Clear();

                for (int i = 0; i < spawnedEnemies.Count; i++)
                {
                    if (spawnedEnemies[i] != null)
                    {
                        Destroy(spawnedEnemies[i]);
                    }
                }

                spawnedEnemies.Clear();
                setLog = "cleared";
            }
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }
    }
}
