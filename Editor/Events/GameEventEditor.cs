using System;
using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using CatAnnaDev.Events;

namespace CatAnnaDev.Editor
{
    [CustomEditor(typeof(GameEvent), true)]
    public class GameEventEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GameEvent evt = (GameEvent)target;

            EditorGUILayout.Space();
            using (new EditorGUI.DisabledScope(!Application.isPlaying))
            {
                string label = Application.isPlaying ? "Raise" : "Raise (enter play mode)";
                if (GUILayout.Button(label, GUILayout.Height(28)))
                {
                    evt.Raise();
                }
            }

            GameEventEditorUtility.DrawRuntimeListeners(evt, this);
        }
    }

    public abstract class GameEventValueEditor<T> : UnityEditor.Editor
    {
        private T pendingValue;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GameEvent<T> evt = (GameEvent<T>)target;

            EditorGUILayout.Space();
            pendingValue = DrawValueField(pendingValue);

            using (new EditorGUI.DisabledScope(!Application.isPlaying))
            {
                string label = Application.isPlaying ? "Raise" : "Raise (enter play mode)";
                if (GUILayout.Button(label, GUILayout.Height(28)))
                {
                    evt.Raise(pendingValue);
                }
            }

            if (Application.isPlaying && evt.HasLastValue)
            {
                EditorGUILayout.LabelField("Last Value", ToDisplay(evt.LastValue));
            }

            GameEventEditorUtility.DrawRuntimeListeners(evt, this);
        }

        protected abstract T DrawValueField(T current);

        protected virtual string ToDisplay(T value)
        {
            return value != null ? value.ToString() : "<null>";
        }
    }

    [CustomEditor(typeof(IntGameEvent))]
    public sealed class IntGameEventEditor : GameEventValueEditor<int>
    {
        protected override int DrawValueField(int current)
        {
            return EditorGUILayout.IntField("Value", current);
        }
    }

    [CustomEditor(typeof(FloatGameEvent))]
    public sealed class FloatGameEventEditor : GameEventValueEditor<float>
    {
        protected override float DrawValueField(float current)
        {
            return EditorGUILayout.FloatField("Value", current);
        }
    }

    [CustomEditor(typeof(BoolGameEvent))]
    public sealed class BoolGameEventEditor : GameEventValueEditor<bool>
    {
        protected override bool DrawValueField(bool current)
        {
            return EditorGUILayout.Toggle("Value", current);
        }
    }

    [CustomEditor(typeof(StringGameEvent))]
    public sealed class StringGameEventEditor : GameEventValueEditor<string>
    {
        protected override string DrawValueField(string current)
        {
            return EditorGUILayout.TextField("Value", current);
        }

        protected override string ToDisplay(string value)
        {
            return value != null ? "\"" + value + "\"" : "<null>";
        }
    }

    [CustomEditor(typeof(Vector3GameEvent))]
    public sealed class Vector3GameEventEditor : GameEventValueEditor<Vector3>
    {
        protected override Vector3 DrawValueField(Vector3 current)
        {
            return EditorGUILayout.Vector3Field("Value", current);
        }
    }

    [CustomEditor(typeof(GameObjectGameEvent))]
    public sealed class GameObjectGameEventEditor : GameEventValueEditor<GameObject>
    {
        protected override GameObject DrawValueField(GameObject current)
        {
            return (GameObject)EditorGUILayout.ObjectField("Value", current, typeof(GameObject), true);
        }

        protected override string ToDisplay(GameObject value)
        {
            return value != null ? value.name : "<null>";
        }
    }

    internal static class GameEventEditorUtility
    {
        private const BindingFlags FieldFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

        internal static void DrawRuntimeListeners(object evt, UnityEditor.Editor owner)
        {
            if (!Application.isPlaying)
            {
                return;
            }

            IList listeners = GetPrivateList(evt, "listeners");
            IList actions = GetPrivateList(evt, "actions");

            int listenerCount = listeners != null ? listeners.Count : 0;
            int actionCount = actions != null ? actions.Count : 0;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Runtime Listeners", (listenerCount + actionCount).ToString(), EditorStyles.boldLabel);

            EditorGUI.indentLevel++;
            DrawList("Components", listeners);
            DrawList("Delegates", actions);
            if (listenerCount + actionCount == 0)
            {
                EditorGUILayout.LabelField("None registered");
            }
            EditorGUI.indentLevel--;

            if (owner != null)
            {
                owner.Repaint();
            }
        }

        private static void DrawList(string header, IList list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }

            EditorGUILayout.LabelField(header + " (" + list.Count + ")");
            EditorGUI.indentLevel++;
            for (int i = 0; i < list.Count; i++)
            {
                object item = list[i];
                UnityEngine.Object unityObject = item as UnityEngine.Object;
                if (unityObject != null)
                {
                    using (new EditorGUI.DisabledScope(true))
                    {
                        EditorGUILayout.ObjectField(unityObject, typeof(UnityEngine.Object), true);
                    }
                }
                else
                {
                    EditorGUILayout.LabelField(Describe(item));
                }
            }
            EditorGUI.indentLevel--;
        }

        private static string Describe(object item)
        {
            if (item == null)
            {
                return "<null>";
            }

            Delegate del = item as Delegate;
            if (del != null)
            {
                string targetName = del.Target != null ? del.Target.GetType().Name : "static";
                return targetName + "." + del.Method.Name;
            }

            return item.ToString();
        }

        private static IList GetPrivateList(object obj, string fieldName)
        {
            if (obj == null)
            {
                return null;
            }

            Type type = obj.GetType();
            while (type != null)
            {
                FieldInfo field = type.GetField(fieldName, FieldFlags);
                if (field != null)
                {
                    return field.GetValue(obj) as IList;
                }

                type = type.BaseType;
            }

            return null;
        }
    }
}
