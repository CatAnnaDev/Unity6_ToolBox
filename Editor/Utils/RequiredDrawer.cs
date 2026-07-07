using CatAnnaDev.Utils;
using UnityEditor;
using UnityEngine;

namespace CatAnnaDev.Editor
{
    [CustomPropertyDrawer(typeof(RequiredAttribute))]
    internal sealed class RequiredDrawer : PropertyDrawer
    {
        private const float Spacing = 2f;

        private bool IsMissing(SerializedProperty property)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.ObjectReference:
                    return property.objectReferenceValue == null;
                case SerializedPropertyType.String:
                    return string.IsNullOrEmpty(property.stringValue);
                case SerializedPropertyType.ExposedReference:
                    return property.exposedReferenceValue == null;
                case SerializedPropertyType.ManagedReference:
                    return string.IsNullOrEmpty(property.managedReferenceFullTypename);
                default:
                    return false;
            }
        }

        private string BuildMessage(SerializedProperty property)
        {
            RequiredAttribute required = (RequiredAttribute)attribute;
            if (!string.IsNullOrEmpty(required.Message))
            {
                return required.Message;
            }

            return property.displayName + " is required and must be assigned.";
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUI.GetPropertyHeight(property, label, true);
            if (IsMissing(property))
            {
                float width = EditorGUIUtility.currentViewWidth - 40f;
                height += Spacing + CatAnnaDevEditorGUI.GetHelpBoxHeight(BuildMessage(property), width);
            }

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float fieldHeight = EditorGUI.GetPropertyHeight(property, label, true);
            Rect fieldRect = new Rect(position.x, position.y, position.width, fieldHeight);
            EditorGUI.PropertyField(fieldRect, property, label, true);

            if (IsMissing(property))
            {
                float boxHeight = CatAnnaDevEditorGUI.GetHelpBoxHeight(BuildMessage(property), position.width);
                Rect boxRect = new Rect(position.x, fieldRect.yMax + Spacing, position.width, boxHeight);
                CatAnnaDevEditorGUI.DrawHelpBox(boxRect, BuildMessage(property), MessageType.Error);
            }
        }
    }
}
