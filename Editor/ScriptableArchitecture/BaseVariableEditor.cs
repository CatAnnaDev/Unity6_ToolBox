using CatAnnaDev.ScriptableArchitecture;
using UnityEditor;
using UnityEngine;

namespace CatAnnaDev.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(BaseVariable), true)]
    public sealed class BaseVariableEditor : UnityEditor.Editor
    {
        private const string ScriptProperty = "m_Script";
        private const string RuntimeValueProperty = "runtimeValue";

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawScriptField();
            DrawPropertiesExcluding(serializedObject, ScriptProperty, RuntimeValueProperty);

            serializedObject.ApplyModifiedProperties();

            DrawRuntimeSection();
        }

        private void DrawScriptField()
        {
            SerializedProperty script = serializedObject.FindProperty(ScriptProperty);
            if (script == null)
            {
                return;
            }

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.PropertyField(script);
            }
        }

        private void DrawRuntimeSection()
        {
            EditorGUILayout.Space();

            SerializedProperty runtime = serializedObject.FindProperty(RuntimeValueProperty);

            if (Application.isPlaying)
            {
                serializedObject.Update();

                if (runtime != null)
                {
                    EditorGUILayout.PropertyField(runtime, new GUIContent("Runtime Value"));
                }
                else
                {
                    DrawBoxedRuntimeValue();
                }

                serializedObject.ApplyModifiedProperties();

                if (GUILayout.Button("Reset To Initial"))
                {
                    ResetSelectedToInitial();
                }

                Repaint();
                return;
            }

            if (runtime != null)
            {
                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUILayout.PropertyField(runtime, new GUIContent("Runtime Value (Play Mode)"));
                }
            }

            EditorGUILayout.HelpBox("Enter Play Mode to edit the live runtime value.", MessageType.Info);
        }

        private void DrawBoxedRuntimeValue()
        {
            if (serializedObject.isEditingMultipleObjects)
            {
                EditorGUILayout.HelpBox("Runtime value is active.", MessageType.Info);
                return;
            }

            BaseVariable variable = target as BaseVariable;
            if (variable == null)
            {
                return;
            }

            object boxed = variable.BoxedValue;
            string display = boxed != null ? boxed.ToString() : "null";
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.LabelField("Runtime Value", display);
            }
        }

        private void ResetSelectedToInitial()
        {
            Object[] targeted = targets;
            for (int i = 0; i < targeted.Length; i++)
            {
                BaseVariable variable = targeted[i] as BaseVariable;
                if (variable == null)
                {
                    continue;
                }

                Undo.RecordObject(variable, "Reset Variable");
                variable.ResetToInitial();
                EditorUtility.SetDirty(variable);
            }

            serializedObject.Update();
        }
    }
}
