using CatAnnaDev.Utils;
using UnityEditor;
using UnityEngine;

namespace CatAnnaDev.Editor
{
    [CustomPropertyDrawer(typeof(MinMaxRange))]
    internal sealed class MinMaxRangeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty min = property.FindPropertyRelative("min");
            SerializedProperty max = property.FindPropertyRelative("max");
            if (min == null || max == null)
            {
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }

            EditorGUI.BeginProperty(position, label, property);
            Rect content = EditorGUI.PrefixLabel(position, label);
            MinMaxRangeFieldLayout.DrawFloatPair(content, min, max);
            EditorGUI.EndProperty();
        }
    }

    [CustomPropertyDrawer(typeof(MinMaxRangeInt))]
    internal sealed class MinMaxRangeIntDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty min = property.FindPropertyRelative("min");
            SerializedProperty max = property.FindPropertyRelative("max");
            if (min == null || max == null)
            {
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }

            EditorGUI.BeginProperty(position, label, property);
            Rect content = EditorGUI.PrefixLabel(position, label);
            MinMaxRangeFieldLayout.DrawIntPair(content, min, max);
            EditorGUI.EndProperty();
        }
    }

    internal static class MinMaxRangeFieldLayout
    {
        private const float LabelWidth = 30f;
        private const float Gap = 6f;

        public static void DrawFloatPair(Rect content, SerializedProperty min, SerializedProperty max)
        {
            float half = (content.width - Gap) * 0.5f;
            Rect minRect = new Rect(content.x, content.y, half, content.height);
            Rect maxRect = new Rect(content.x + half + Gap, content.y, half, content.height);

            float previousWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = LabelWidth;

            min.floatValue = EditorGUI.FloatField(minRect, "Min", min.floatValue);
            float maxValue = EditorGUI.FloatField(maxRect, "Max", max.floatValue);
            max.floatValue = Mathf.Max(maxValue, min.floatValue);

            EditorGUIUtility.labelWidth = previousWidth;
        }

        public static void DrawIntPair(Rect content, SerializedProperty min, SerializedProperty max)
        {
            float half = (content.width - Gap) * 0.5f;
            Rect minRect = new Rect(content.x, content.y, half, content.height);
            Rect maxRect = new Rect(content.x + half + Gap, content.y, half, content.height);

            float previousWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = LabelWidth;

            min.intValue = EditorGUI.IntField(minRect, "Min", min.intValue);
            int maxValue = EditorGUI.IntField(maxRect, "Max", max.intValue);
            max.intValue = Mathf.Max(maxValue, min.intValue);

            EditorGUIUtility.labelWidth = previousWidth;
        }
    }
}
