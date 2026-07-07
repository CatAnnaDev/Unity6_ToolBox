using CatAnnaDev.Utils;
using UnityEditor;
using UnityEngine;

namespace CatAnnaDev.Editor
{
    [CustomPropertyDrawer(typeof(Optional<>))]
    internal sealed class OptionalDrawer : PropertyDrawer
    {
        private const float ToggleWidth = 18f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty value = property.FindPropertyRelative("value");
            if (value == null)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            return EditorGUI.GetPropertyHeight(value, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty enabled = property.FindPropertyRelative("enabled");
            SerializedProperty value = property.FindPropertyRelative("value");

            if (enabled == null || value == null)
            {
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }

            EditorGUI.BeginProperty(position, label, property);

            Rect valueRect = new Rect(
                position.x,
                position.y,
                position.width - ToggleWidth,
                position.height);

            bool previous = GUI.enabled;
            GUI.enabled = previous && enabled.boolValue;
            EditorGUI.PropertyField(valueRect, value, label, true);
            GUI.enabled = previous;

            Rect toggleRect = new Rect(
                position.xMax - ToggleWidth,
                position.y,
                ToggleWidth,
                EditorGUIUtility.singleLineHeight);
            enabled.boolValue = EditorGUI.Toggle(toggleRect, enabled.boolValue);

            EditorGUI.EndProperty();
        }
    }
}
