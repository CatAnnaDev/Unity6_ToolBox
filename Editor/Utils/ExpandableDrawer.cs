using CatAnnaDev.Utils;
using UnityEditor;
using UnityEngine;

namespace CatAnnaDev.Editor
{
    [CustomPropertyDrawer(typeof(ExpandableAttribute))]
    internal sealed class ExpandableDrawer : PropertyDrawer
    {
        private const float BoxPadding = 4f;
        private const string ScriptPropertyName = "m_Script";

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight;
            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                return height;
            }

            Object target = property.objectReferenceValue;
            if (!property.isExpanded || target == null)
            {
                return height;
            }

            SerializedObject nested = new SerializedObject(target);
            height += BoxPadding * 2f + EditorGUIUtility.standardVerticalSpacing;

            SerializedProperty iterator = nested.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;
                if (iterator.propertyPath == ScriptPropertyName)
                {
                    continue;
                }

                height += EditorGUI.GetPropertyHeight(iterator, true) + EditorGUIUtility.standardVerticalSpacing;
            }

            nested.Dispose();
            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }

            EditorGUI.BeginProperty(position, label, property);

            Rect line = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            Object target = property.objectReferenceValue;

            float labelWidth = EditorGUIUtility.labelWidth;
            Rect foldoutRect = new Rect(line.x, line.y, labelWidth, line.height);
            Rect objectRect = new Rect(line.x + labelWidth, line.y, line.width - labelWidth, line.height);

            if (target != null)
            {
                property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label, true);
            }
            else
            {
                EditorGUI.LabelField(foldoutRect, label);
            }

            EditorGUI.PropertyField(objectRect, property, GUIContent.none);

            if (target == null || !property.isExpanded)
            {
                EditorGUI.EndProperty();
                return;
            }

            DrawNested(position, line, target);
            EditorGUI.EndProperty();
        }

        private static void DrawNested(Rect position, Rect line, Object target)
        {
            float boxTop = line.yMax + EditorGUIUtility.standardVerticalSpacing;
            float boxHeight = position.yMax - boxTop;
            Rect boxRect = new Rect(position.x, boxTop, position.width, boxHeight);
            GUI.Box(boxRect, GUIContent.none, EditorStyles.helpBox);

            SerializedObject nested = new SerializedObject(target);
            nested.Update();

            EditorGUI.indentLevel++;
            float y = boxTop + BoxPadding;
            float innerX = boxRect.x + BoxPadding;
            float innerWidth = boxRect.width - BoxPadding * 2f;

            SerializedProperty iterator = nested.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;
                if (iterator.propertyPath == ScriptPropertyName)
                {
                    continue;
                }

                float propHeight = EditorGUI.GetPropertyHeight(iterator, true);
                Rect propRect = new Rect(innerX, y, innerWidth, propHeight);
                EditorGUI.PropertyField(propRect, iterator, true);
                y += propHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            EditorGUI.indentLevel--;

            if (nested.ApplyModifiedProperties())
            {
                EditorUtility.SetDirty(target);
            }

            nested.Dispose();
        }
    }
}
