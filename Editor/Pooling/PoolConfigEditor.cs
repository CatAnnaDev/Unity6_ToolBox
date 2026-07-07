using UnityEditor;
using UnityEngine;
using CatAnnaDev.Pooling;

namespace CatAnnaDev.Editor
{
    [CustomEditor(typeof(PoolConfig))]
    [CanEditMultipleObjects]
    public sealed class PoolConfigEditor : UnityEditor.Editor
    {
        SerializedProperty _prefab;
        SerializedProperty _prewarmCount;
        SerializedProperty _expandPolicy;
        SerializedProperty _maxSize;
        SerializedProperty _cullExcess;
        SerializedProperty _cullCheckInterval;
        SerializedProperty _persistAcrossScenes;
        SerializedProperty _id;
        SerializedProperty _category;

        void OnEnable()
        {
            _prefab = serializedObject.FindProperty("prefab");
            _prewarmCount = serializedObject.FindProperty("prewarmCount");
            _expandPolicy = serializedObject.FindProperty("expandPolicy");
            _maxSize = serializedObject.FindProperty("maxSize");
            _cullExcess = serializedObject.FindProperty("cullExcess");
            _cullCheckInterval = serializedObject.FindProperty("cullCheckInterval");
            _persistAcrossScenes = serializedObject.FindProperty("persistAcrossScenes");
            _id = serializedObject.FindProperty("id");
            _category = serializedObject.FindProperty("category");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Prefab", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_prefab);

            EditorGUILayout.Space(6f);
            EditorGUILayout.LabelField("Sizing", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_prewarmCount, new GUIContent("Prewarm Count"));
            EditorGUILayout.PropertyField(_expandPolicy, new GUIContent("Expand Policy"), true);
            EditorGUILayout.PropertyField(_maxSize, new GUIContent("Max Size (0 = unbounded)"));

            EditorGUILayout.Space(6f);
            EditorGUILayout.LabelField("Culling", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_cullExcess, new GUIContent("Cull Excess"));
            using (new EditorGUI.DisabledScope(!_cullExcess.boolValue))
            {
                EditorGUILayout.PropertyField(_cullCheckInterval, new GUIContent("Cull Check Interval"));
            }

            EditorGUILayout.Space(6f);
            EditorGUILayout.LabelField("Identity", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_id, new GUIContent("Id"));
            EditorGUILayout.PropertyField(_category, new GUIContent("Category"));
            EditorGUILayout.PropertyField(_persistAcrossScenes, new GUIContent("Persist Across Scenes"));

            serializedObject.ApplyModifiedProperties();

            DrawValidation();
            DrawRuntimeActions();
        }

        void DrawValidation()
        {
            EditorGUILayout.Space(6f);

            if (_prefab.hasMultipleDifferentValues) return;

            if (_prefab.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("Prefab is not assigned. This config cannot create a pool until a prefab is set.", MessageType.Error);
            }

            int maxSize = _maxSize.intValue;
            int prewarm = _prewarmCount.intValue;
            if (maxSize > 0 && maxSize < prewarm)
            {
                EditorGUILayout.HelpBox(
                    "Max Size (" + maxSize + ") is smaller than Prewarm Count (" + prewarm + "). Prewarm will stop early at the ceiling.",
                    MessageType.Warning);
            }

            if (_prefab.objectReferenceValue is GameObject go)
            {
                if (go.scene.IsValid() && go.scene.rootCount > 0)
                {
                    EditorGUILayout.HelpBox("Prefab reference points to a scene object, not a project prefab asset.", MessageType.Warning);
                }
            }

            if (_cullExcess.boolValue && _cullCheckInterval.floatValue <= 0f)
            {
                EditorGUILayout.HelpBox("Cull Excess is enabled but Cull Check Interval is 0. Excess will never be culled automatically.", MessageType.Warning);
            }
        }

        void DrawRuntimeActions()
        {
            if (!Application.isPlaying) return;
            if (targets.Length != 1) return;

            PoolConfig config = (PoolConfig)target;
            if (config == null || !config.IsValid) return;

            EditorGUILayout.Space(8f);
            EditorGUILayout.LabelField("Play Mode", EditorStyles.boldLabel);

            if (GUILayout.Button("Prewarm In Play Mode"))
            {
                PoolManager manager = PoolManager.Instance;
                if (manager != null)
                {
                    GameObjectPool pool = manager.CreatePool(config, false);
                    if (pool != null)
                    {
                        int count = config.PrewarmCount > 0 ? config.PrewarmCount : 1;
                        pool.Prewarm(count);
                    }
                }
            }

            if (PoolManager.HasInstance)
            {
                PoolManager manager = PoolManager.InstanceOrNull;
                if (manager != null && manager.TryGetPool(config.Prefab, out GameObjectPool live))
                {
                    PoolStats s = live.GetStats();
                    EditorGUILayout.HelpBox(
                        "Live pool - active " + s.active + ", inactive " + s.inactive + ", spawned " + s.totalSpawned + ", misses " + s.misses,
                        MessageType.Info);
                }
            }
        }
    }
}
