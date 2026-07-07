using System.Collections.Generic;
using CatAnnaDev.Utils;
using UnityEditor;
using UnityEngine;

namespace CatAnnaDev.Editor
{
    [CustomPropertyDrawer(typeof(SerializableDictionary<,>))]
    internal sealed class SerializableDictionaryDrawer : PropertyDrawer
    {
        private const float RemoveButtonWidth = 22f;
        private const float ColumnGap = 4f;
        private const float RowPadding = 2f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight;
            if (!property.isExpanded)
            {
                return height;
            }

            SerializedProperty keys = property.FindPropertyRelative("keys");
            SerializedProperty values = property.FindPropertyRelative("values");
            if (keys == null || values == null)
            {
                return height + EditorGUIUtility.singleLineHeight;
            }

            int count = Mathf.Min(keys.arraySize, values.arraySize);
            for (int i = 0; i < count; i++)
            {
                height += EditorGUIUtility.standardVerticalSpacing + RowHeight(keys, values, i);
            }

            height += EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight;

            if (HasDuplicateKeys(keys, count))
            {
                height += EditorGUIUtility.standardVerticalSpacing +
                          CatAnnaDevEditorGUI.GetHelpBoxHeight(
                              "Duplicate keys detected. Later entries overwrite earlier ones.",
                              EditorGUIUtility.currentViewWidth - 40f);
            }

            return height;
        }

        private static float RowHeight(SerializedProperty keys, SerializedProperty values, int index)
        {
            float keyHeight = EditorGUI.GetPropertyHeight(keys.GetArrayElementAtIndex(index), true);
            float valueHeight = EditorGUI.GetPropertyHeight(values.GetArrayElementAtIndex(index), true);
            return Mathf.Max(keyHeight, valueHeight) + RowPadding;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty keys = property.FindPropertyRelative("keys");
            SerializedProperty values = property.FindPropertyRelative("values");
            if (keys == null || values == null)
            {
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }

            EditorGUI.BeginProperty(position, label, property);

            int count = Mathf.Min(keys.arraySize, values.arraySize);
            Rect headerRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            property.isExpanded = EditorGUI.Foldout(
                headerRect, property.isExpanded, label.text + "  (" + count + ")", true);

            if (!property.isExpanded)
            {
                EditorGUI.EndProperty();
                return;
            }

            EditorGUI.indentLevel++;
            float y = headerRect.yMax + EditorGUIUtility.standardVerticalSpacing;

            for (int i = 0; i < count; i++)
            {
                float rowHeight = RowHeight(keys, values, i);
                Rect rowRect = new Rect(position.x, y, position.width, rowHeight);
                DrawRow(rowRect, keys, values, i, ref count);
                y += rowHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            Rect addRect = new Rect(
                position.x,
                y,
                position.width,
                EditorGUIUtility.singleLineHeight);
            if (GUI.Button(addRect, "Add Entry"))
            {
                keys.arraySize++;
                values.arraySize++;
            }

            y += addRect.height + EditorGUIUtility.standardVerticalSpacing;

            if (HasDuplicateKeys(keys, Mathf.Min(keys.arraySize, values.arraySize)))
            {
                float boxHeight = CatAnnaDevEditorGUI.GetHelpBoxHeight(
                    "Duplicate keys detected. Later entries overwrite earlier ones.", position.width);
                Rect boxRect = new Rect(position.x, y, position.width, boxHeight);
                EditorGUI.HelpBox(boxRect,
                    "Duplicate keys detected. Later entries overwrite earlier ones.", MessageType.Warning);
            }

            EditorGUI.indentLevel--;
            EditorGUI.EndProperty();
        }

        private static void DrawRow(
            Rect rowRect, SerializedProperty keys, SerializedProperty values, int index, ref int count)
        {
            float columnWidth = (rowRect.width - RemoveButtonWidth - ColumnGap * 2f) * 0.5f;

            Rect keyRect = new Rect(rowRect.x, rowRect.y, columnWidth, rowRect.height - RowPadding);
            Rect valueRect = new Rect(
                keyRect.xMax + ColumnGap, rowRect.y, columnWidth, rowRect.height - RowPadding);
            Rect removeRect = new Rect(
                valueRect.xMax + ColumnGap, rowRect.y, RemoveButtonWidth, EditorGUIUtility.singleLineHeight);

            EditorGUI.PropertyField(keyRect, keys.GetArrayElementAtIndex(index), GUIContent.none, true);
            EditorGUI.PropertyField(valueRect, values.GetArrayElementAtIndex(index), GUIContent.none, true);

            if (GUI.Button(removeRect, "-"))
            {
                keys.DeleteArrayElementAtIndex(index);
                values.DeleteArrayElementAtIndex(index);
                count = Mathf.Min(keys.arraySize, values.arraySize);
            }
        }

        private static bool HasDuplicateKeys(SerializedProperty keys, int count)
        {
            if (count < 2)
            {
                return false;
            }

            HashSet<string> seen = new HashSet<string>();
            for (int i = 0; i < count && i < keys.arraySize; i++)
            {
                string token = KeyToken(keys.GetArrayElementAtIndex(i));
                if (token == null)
                {
                    continue;
                }

                if (!seen.Add(token))
                {
                    return true;
                }
            }

            return false;
        }

        private static string KeyToken(SerializedProperty key)
        {
            switch (key.propertyType)
            {
                case SerializedPropertyType.String:
                    return "s:" + key.stringValue;
                case SerializedPropertyType.Integer:
                    return "i:" + key.longValue;
                case SerializedPropertyType.Boolean:
                    return "b:" + key.boolValue;
                case SerializedPropertyType.Float:
                    return "f:" + key.doubleValue;
                case SerializedPropertyType.Enum:
                    return "e:" + key.enumValueIndex;
                case SerializedPropertyType.ObjectReference:
                    return "o:" + (key.objectReferenceValue != null
                        ? key.objectReferenceValue.GetEntityId().ToString()
                        : "null");
                default:
                    return null;
            }
        }
    }
}
