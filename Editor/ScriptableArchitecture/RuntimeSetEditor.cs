using System.Collections.Generic;
using CatAnnaDev.ScriptableArchitecture;
using UnityEditor;
using UnityEngine;

namespace CatAnnaDev.Editor
{
    public abstract class RuntimeSetEditor<T> : UnityEditor.Editor where T : Object
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            RuntimeSet<T> set = target as RuntimeSet<T>;
            if (set == null)
            {
                return;
            }

            EditorGUILayout.Space();

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Members are populated at runtime.", MessageType.Info);
                return;
            }

            IReadOnlyList<T> items = set.Items;
            EditorGUILayout.LabelField("Members", set.Count.ToString());

            using (new EditorGUI.DisabledScope(true))
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    if (items.Count == 0)
                    {
                        EditorGUILayout.LabelField("(empty)");
                    }

                    for (int i = 0; i < items.Count; i++)
                    {
                        EditorGUILayout.ObjectField(items[i], typeof(T), true);
                    }
                }
            }

            Repaint();
        }
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(GameObjectRuntimeSet))]
    public sealed class GameObjectRuntimeSetEditor : RuntimeSetEditor<GameObject>
    {
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(TransformRuntimeSet))]
    public sealed class TransformRuntimeSetEditor : RuntimeSetEditor<Transform>
    {
    }
}
