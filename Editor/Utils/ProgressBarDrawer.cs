using CatAnnaDev.Utils;
using UnityEditor;
using UnityEngine;

namespace CatAnnaDev.Editor
{
    [CustomPropertyDrawer(typeof(ProgressBarAttribute))]
    internal sealed class ProgressBarDrawer : PropertyDrawer
    {
        private static GUIStyle centeredLabel;

        private static GUIStyle CenteredLabel
        {
            get
            {
                if (centeredLabel == null)
                {
                    centeredLabel = new GUIStyle(EditorStyles.boldLabel)
                    {
                        alignment = TextAnchor.MiddleCenter,
                        normal = { textColor = Color.white }
                    };
                }

                return centeredLabel;
            }
        }

        private static bool TryGetValue(SerializedProperty property, out float value)
        {
            if (property.propertyType == SerializedPropertyType.Float)
            {
                value = property.floatValue;
                return true;
            }

            if (property.propertyType == SerializedPropertyType.Integer)
            {
                value = property.longValue;
                return true;
            }

            value = 0f;
            return false;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight + 4f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!TryGetValue(property, out float value))
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            ProgressBarAttribute bar = (ProgressBarAttribute)attribute;
            float max = bar.MaxValue;
            if (!string.IsNullOrEmpty(bar.MaxValueMember) &&
                CatAnnaDevEditorGUI.TryReadFloatMember(property, bar.MaxValueMember, out float memberMax))
            {
                max = memberMax;
            }

            if (max <= 0f)
            {
                max = 1f;
            }

            float fill = Mathf.Clamp01(value / max);
            string caption = string.IsNullOrEmpty(bar.Label) ? property.displayName : bar.Label;
            string text = caption + "  " + value.ToString("0.##") + " / " + max.ToString("0.##");

            Rect barRect = new Rect(position.x, position.y + 2f, position.width, EditorGUIUtility.singleLineHeight);
            DrawBar(barRect, fill, bar.BarColor, text);
        }

        private static void DrawBar(Rect rect, float fill, Color color, string text)
        {
            Color background = EditorGUIUtility.isProSkin
                ? new Color(0.16f, 0.16f, 0.16f, 1f)
                : new Color(0.65f, 0.65f, 0.65f, 1f);
            EditorGUI.DrawRect(rect, background);

            if (fill > 0f)
            {
                Rect fillRect = new Rect(rect.x, rect.y, rect.width * fill, rect.height);
                EditorGUI.DrawRect(fillRect, color);
            }

            EditorGUI.LabelField(rect, text, CenteredLabel);
        }
    }
}
