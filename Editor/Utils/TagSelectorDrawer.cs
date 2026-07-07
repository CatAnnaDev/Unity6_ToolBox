using CatAnnaDev.Utils;
using UnityEditor;
using UnityEngine;

namespace CatAnnaDev.Editor
{
    [CustomPropertyDrawer(typeof(TagSelectorAttribute))]
    internal sealed class TagSelectorDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();
            string current = string.IsNullOrEmpty(property.stringValue) ? "Untagged" : property.stringValue;
            string selected = EditorGUI.TagField(position, label, current);
            if (EditorGUI.EndChangeCheck())
            {
                property.stringValue = selected;
            }

            EditorGUI.EndProperty();
        }
    }
}
