using UnityEngine;
using UnityEditor;
using CatAnnaDev.VFX;

namespace CatAnnaDev.Editor
{
    [CustomEditor(typeof(VFXData))]
    public sealed class VFXDataEditor : UnityEditor.Editor
    {
        private SerializedProperty prefab;
        private SerializedProperty followTarget;
        private SerializedProperty alignToNormal;
        private SerializedProperty positionOffset;
        private SerializedProperty eulerOffset;
        private SerializedProperty randomizeYaw;
        private SerializedProperty scale;
        private SerializedProperty lifetimeOverride;
        private SerializedProperty prewarmCount;

        private GameObject spawnedPreview;

        private void OnEnable()
        {
            prefab = serializedObject.FindProperty("prefab");
            followTarget = serializedObject.FindProperty("followTarget");
            alignToNormal = serializedObject.FindProperty("alignToNormal");
            positionOffset = serializedObject.FindProperty("positionOffset");
            eulerOffset = serializedObject.FindProperty("eulerOffset");
            randomizeYaw = serializedObject.FindProperty("randomizeYaw");
            scale = serializedObject.FindProperty("scale");
            lifetimeOverride = serializedObject.FindProperty("lifetimeOverride");
            prewarmCount = serializedObject.FindProperty("prewarmCount");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(prefab);

            DrawPrefabValidation();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Placement", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(followTarget);
            EditorGUILayout.PropertyField(alignToNormal);
            EditorGUILayout.PropertyField(positionOffset);
            EditorGUILayout.PropertyField(eulerOffset);
            EditorGUILayout.PropertyField(randomizeYaw);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Transform", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(scale);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Lifetime", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(lifetimeOverride);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Pooling", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(prewarmCount);

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();
            DrawSpawnControls();
        }

        private void DrawPrefabValidation()
        {
            GameObject target = prefab.objectReferenceValue as GameObject;
            if (target == null)
            {
                EditorGUILayout.HelpBox("Assign a prefab to make this VFXData playable.", MessageType.Warning);
                return;
            }

            if (target.GetComponentInChildren<ParticleSystem>(true) == null)
            {
                EditorGUILayout.HelpBox("The assigned prefab has no ParticleSystem in its hierarchy. It will still spawn, but this VFXData may not behave as an effect.", MessageType.Warning);
            }
        }

        private void DrawSpawnControls()
        {
            VFXData data = (VFXData)target;

            using (new EditorGUI.DisabledScope(!Application.isPlaying || !data.IsValid))
            {
                if (GUILayout.Button("Spawn At Scene View"))
                {
                    SpawnAtSceneView(data);
                }
            }

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Enter play mode to spawn this effect at the scene view pivot.", MessageType.Info);
            }

            using (new EditorGUI.DisabledScope(spawnedPreview == null))
            {
                if (GUILayout.Button("Stop Spawned Preview"))
                {
                    StopPreview();
                }
            }
        }

        private void SpawnAtSceneView(VFXData data)
        {
            if (!VFXManager.HasInstance)
            {
                Debug.LogWarning("No VFXManager instance is active in the scene.");
            }

            Vector3 position = ResolveSceneViewPivot();
            StopPreview();
            spawnedPreview = VFXManager.Instance.Play(data, position, Quaternion.identity);
        }

        private void StopPreview()
        {
            if (spawnedPreview != null && VFXManager.HasInstance)
            {
                VFXManager.Instance.Stop(spawnedPreview);
            }

            spawnedPreview = null;
        }

        private static Vector3 ResolveSceneViewPivot()
        {
            SceneView view = SceneView.lastActiveSceneView;
            if (view != null)
            {
                return view.pivot;
            }

            return Vector3.zero;
        }
    }
}
