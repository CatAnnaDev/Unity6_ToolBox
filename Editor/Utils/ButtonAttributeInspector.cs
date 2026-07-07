using System;
using System.Collections.Generic;
using System.Reflection;
using CatAnnaDev.Utils;
using UnityEditor;
using UnityEngine;

namespace CatAnnaDev.Editor
{
    internal static class InspectorButtonRenderer
    {
        private const BindingFlags MethodFlags =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

        public static void DrawButtons(UnityEngine.Object[] targets)
        {
            if (targets == null || targets.Length == 0 || targets[0] == null)
            {
                return;
            }

            List<MethodInfo> methods = CollectButtonMethods(targets[0].GetType());
            if (methods.Count == 0)
            {
                return;
            }

            EditorGUILayout.Space();
            for (int i = 0; i < methods.Count; i++)
            {
                DrawButton(methods[i], targets);
            }
        }

        private static void DrawButton(MethodInfo method, UnityEngine.Object[] targets)
        {
            ButtonAttribute button = method.GetCustomAttribute<ButtonAttribute>();
            string label = string.IsNullOrEmpty(button.Label)
                ? ObjectNames.NicifyVariableName(method.Name)
                : button.Label;

            bool playing = Application.isPlaying;
            bool usable = button.Activity == ButtonActivity.Always
                          || (button.Activity == ButtonActivity.EditorOnly && !playing)
                          || (button.Activity == ButtonActivity.PlayModeOnly && playing);

            bool previous = GUI.enabled;
            GUI.enabled = previous && usable;

            if (GUILayout.Button(label))
            {
                Invoke(method, targets);
            }

            GUI.enabled = previous;
        }

        private static void Invoke(MethodInfo method, UnityEngine.Object[] targets)
        {
            for (int i = 0; i < targets.Length; i++)
            {
                UnityEngine.Object target = targets[i];
                if (target == null)
                {
                    continue;
                }

                Undo.RecordObject(target, method.Name);
                object result = method.Invoke(target, null);
                if (result is System.Collections.IEnumerator routine && target is MonoBehaviour behaviour)
                {
                    behaviour.StartCoroutine(routine);
                }

                EditorUtility.SetDirty(target);
            }
        }

        private static List<MethodInfo> CollectButtonMethods(Type type)
        {
            List<MethodInfo> result = new List<MethodInfo>();
            HashSet<string> seen = new HashSet<string>();

            Type current = type;
            while (current != null && current != typeof(MonoBehaviour) && current != typeof(object))
            {
                MethodInfo[] methods = current.GetMethods(MethodFlags);
                for (int i = 0; i < methods.Length; i++)
                {
                    MethodInfo method = methods[i];
                    if (method.GetParameters().Length != 0)
                    {
                        continue;
                    }

                    if (method.GetCustomAttribute<ButtonAttribute>() == null)
                    {
                        continue;
                    }

                    if (seen.Add(method.Name))
                    {
                        result.Add(method);
                    }
                }

                current = current.BaseType;
            }

            return result;
        }
    }

    public abstract class ButtonEditorBase : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            InspectorButtonRenderer.DrawButtons(targets);
        }
    }

#if CATANNADEV_GLOBAL_INSPECTOR
    [CanEditMultipleObjects]
    [CustomEditor(typeof(MonoBehaviour), true)]
    internal sealed class GlobalButtonInspector : ButtonEditorBase
    {
    }
#endif
}
