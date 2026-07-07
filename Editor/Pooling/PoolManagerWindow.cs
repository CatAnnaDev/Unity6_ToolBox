using UnityEditor;
using UnityEngine;
using CatAnnaDev.Pooling;

namespace CatAnnaDev.Editor
{
    public sealed class PoolManagerWindow : EditorWindow
    {
        const float RepaintInterval = 0.5f;
        const int PrewarmStep = 10;

        Vector2 _scroll;
        double _nextRepaint;
        bool _autoRefresh = true;

        static readonly GUIContent TitleContent = new GUIContent("Pool Diagnostics");

        GUIStyle _headerStyle;
        GUIStyle _rowLabelStyle;
        GUIStyle _idStyle;

        [MenuItem("Tools/CatAnnaDev/Pool Diagnostics")]
        static void Open()
        {
            PoolManagerWindow window = GetWindow<PoolManagerWindow>();
            window.titleContent = TitleContent;
            window.minSize = new Vector2(560f, 240f);
            window.Show();
        }

        void OnEnable()
        {
            EditorApplication.update += OnEditorUpdate;
            _nextRepaint = EditorApplication.timeSinceStartup + RepaintInterval;
        }

        void OnDisable()
        {
            EditorApplication.update -= OnEditorUpdate;
        }

        void OnEditorUpdate()
        {
            if (!_autoRefresh) return;
            if (!Application.isPlaying) return;

            double now = EditorApplication.timeSinceStartup;
            if (now >= _nextRepaint)
            {
                _nextRepaint = now + RepaintInterval;
                Repaint();
            }
        }

        void EnsureStyles()
        {
            if (_headerStyle == null)
            {
                _headerStyle = new GUIStyle(EditorStyles.miniBoldLabel)
                {
                    alignment = TextAnchor.MiddleLeft
                };
            }
            if (_rowLabelStyle == null)
            {
                _rowLabelStyle = new GUIStyle(EditorStyles.label)
                {
                    alignment = TextAnchor.MiddleLeft
                };
            }
            if (_idStyle == null)
            {
                _idStyle = new GUIStyle(EditorStyles.miniLabel)
                {
                    alignment = TextAnchor.MiddleLeft,
                    fontStyle = FontStyle.Italic
                };
            }
        }

        void OnGUI()
        {
            EnsureStyles();
            DrawToolbar();

            if (!Application.isPlaying)
            {
                EditorGUILayout.Space(8f);
                EditorGUILayout.HelpBox(
                    "Enter Play Mode to inspect live pools. Pool statistics are only available at runtime.",
                    MessageType.Info);
                return;
            }

            if (!PoolManager.HasInstance)
            {
                EditorGUILayout.Space(8f);
                EditorGUILayout.HelpBox(
                    "No PoolManager is alive yet. It is created on demand the first time you Spawn, Prewarm, or create a pool.",
                    MessageType.Info);
                return;
            }

            PoolManager manager = PoolManager.InstanceOrNull;
            if (manager == null)
            {
                EditorGUILayout.Space(8f);
                EditorGUILayout.HelpBox("PoolManager instance is unavailable.", MessageType.Warning);
                return;
            }

            int poolCount = manager.PoolCount;
            if (poolCount == 0)
            {
                EditorGUILayout.Space(8f);
                EditorGUILayout.HelpBox("PoolManager is alive but no pools have been created yet.", MessageType.Info);
                return;
            }

            DrawSummary(manager, poolCount);
            DrawHeaderRow();

            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            for (int i = 0; i < poolCount; i++)
            {
                GameObjectPool pool = manager.PoolAt(i);
                if (pool == null) continue;
                DrawPoolRow(pool);
            }
            EditorGUILayout.EndScrollView();
        }

        void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            GUI.enabled = Application.isPlaying && PoolManager.HasInstance;
            if (GUILayout.Button("Clear All", EditorStyles.toolbarButton, GUILayout.Width(80f)))
            {
                PoolManager manager = PoolManager.InstanceOrNull;
                if (manager != null) manager.ClearAll();
            }
            GUI.enabled = true;

            GUILayout.FlexibleSpace();

            _autoRefresh = GUILayout.Toggle(_autoRefresh, "Auto Refresh", EditorStyles.toolbarButton, GUILayout.Width(96f));
            if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(64f)))
            {
                Repaint();
            }

            EditorGUILayout.EndHorizontal();
        }

        void DrawSummary(PoolManager manager, int poolCount)
        {
            int totalActive = 0;
            int totalInactive = 0;
            int totalSpawned = 0;
            int totalMisses = 0;

            for (int i = 0; i < poolCount; i++)
            {
                GameObjectPool pool = manager.PoolAt(i);
                if (pool == null) continue;
                PoolStats s = pool.GetStats();
                totalActive += s.active;
                totalInactive += s.inactive;
                totalSpawned += s.totalSpawned;
                totalMisses += s.misses;
            }

            EditorGUILayout.Space(4f);
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUILayout.Label("Pools: " + poolCount, EditorStyles.miniLabel, GUILayout.Width(80f));
            GUILayout.Label("Active: " + totalActive, EditorStyles.miniLabel, GUILayout.Width(90f));
            GUILayout.Label("Inactive: " + totalInactive, EditorStyles.miniLabel, GUILayout.Width(100f));
            GUILayout.Label("Spawned: " + totalSpawned, EditorStyles.miniLabel, GUILayout.Width(110f));
            GUILayout.Label("Misses: " + totalMisses, EditorStyles.miniLabel, GUILayout.Width(100f));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        void DrawHeaderRow()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Pool", _headerStyle, GUILayout.MinWidth(140f), GUILayout.ExpandWidth(true));
            GUILayout.Label("Active", _headerStyle, GUILayout.Width(56f));
            GUILayout.Label("Inactive", _headerStyle, GUILayout.Width(64f));
            GUILayout.Label("Peak", _headerStyle, GUILayout.Width(56f));
            GUILayout.Label("Spawned", _headerStyle, GUILayout.Width(70f));
            GUILayout.Label("Misses", _headerStyle, GUILayout.Width(56f));
            GUILayout.Label("Reuse", _headerStyle, GUILayout.Width(56f));
            GUILayout.Space(4f);
            GUILayout.Label("", _headerStyle, GUILayout.Width(148f));
            EditorGUILayout.EndHorizontal();

            Rect line = EditorGUILayout.GetControlRect(false, 1f);
            EditorGUI.DrawRect(line, new Color(0f, 0f, 0f, 0.25f));
        }

        void DrawPoolRow(GameObjectPool pool)
        {
            PoolStats s = pool.GetStats();

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical(GUILayout.MinWidth(140f), GUILayout.ExpandWidth(true));
            string label = ResolveLabel(pool);
            GUILayout.Label(label, _rowLabelStyle);
            string subtitle = ResolveSubtitle(pool);
            if (!string.IsNullOrEmpty(subtitle)) GUILayout.Label(subtitle, _idStyle);
            EditorGUILayout.EndVertical();

            GUILayout.Label(s.active.ToString(), _rowLabelStyle, GUILayout.Width(56f));
            GUILayout.Label(s.inactive.ToString(), _rowLabelStyle, GUILayout.Width(64f));
            GUILayout.Label(s.peakActive.ToString(), _rowLabelStyle, GUILayout.Width(56f));
            GUILayout.Label(s.totalSpawned.ToString(), _rowLabelStyle, GUILayout.Width(70f));
            GUILayout.Label(s.misses.ToString(), _rowLabelStyle, GUILayout.Width(56f));
            GUILayout.Label(s.ReuseRatio.ToString("0.00"), _rowLabelStyle, GUILayout.Width(56f));

            GUILayout.Space(4f);
            if (GUILayout.Button("Prewarm +" + PrewarmStep, EditorStyles.miniButtonLeft, GUILayout.Width(84f)))
            {
                pool.Prewarm(PrewarmStep);
                Repaint();
            }
            if (GUILayout.Button("Clear", EditorStyles.miniButtonRight, GUILayout.Width(56f)))
            {
                pool.Clear();
                Repaint();
            }

            EditorGUILayout.EndHorizontal();
        }

        static string ResolveLabel(GameObjectPool pool)
        {
            if (!string.IsNullOrEmpty(pool.Id)) return pool.Id;
            GameObject prefab = pool.Prefab;
            return prefab != null ? prefab.name : "(missing prefab)";
        }

        static string ResolveSubtitle(GameObjectPool pool)
        {
            GameObject prefab = pool.Prefab;
            string prefabName = prefab != null ? prefab.name : "(missing prefab)";

            bool hasId = !string.IsNullOrEmpty(pool.Id);
            bool hasCategory = !string.IsNullOrEmpty(pool.Category);

            if (hasId && hasCategory) return prefabName + "  -  " + pool.Category;
            if (hasId) return prefabName;
            if (hasCategory) return pool.Category;
            return null;
        }
    }
}
