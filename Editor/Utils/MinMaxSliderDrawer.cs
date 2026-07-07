using CatAnnaDev.Utils;
using UnityEditor;
using UnityEngine;

namespace CatAnnaDev.Editor
{
    [CustomPropertyDrawer(typeof(MinMaxSliderAttribute))]
    internal sealed class MinMaxSliderDrawer : PropertyDrawer
    {
        private const float FieldWidth = 52f;
        private const float Gap = 4f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Vector2)
            {
                EditorGUI.LabelField(position, label.text, "Use MinMaxSlider on a Vector2 field.");
                return;
            }

            MinMaxSliderAttribute slider = (MinMaxSliderAttribute)attribute;
            EditorGUI.BeginProperty(position, label, property);

            Rect content = EditorGUI.PrefixLabel(position, label);
            Vector2 range = property.vector2Value;
            float min = range.x;
            float max = range.y;

            bool showValues = slider.ShowValues;
            Rect sliderRect = content;

            EditorGUI.BeginChangeCheck();

            if (showValues)
            {
                Rect minRect = new Rect(content.x, content.y, FieldWidth, content.height);
                Rect maxRect = new Rect(content.xMax - FieldWidth, content.y, FieldWidth, content.height);
                sliderRect = new Rect(minRect.xMax + Gap, content.y,
                    content.width - FieldWidth * 2f - Gap * 2f, content.height);

                min = EditorGUI.FloatField(minRect, min);
                EditorGUI.MinMaxSlider(sliderRect, ref min, ref max, slider.Min, slider.Max);
                max = EditorGUI.FloatField(maxRect, max);
            }
            else
            {
                EditorGUI.MinMaxSlider(sliderRect, ref min, ref max, slider.Min, slider.Max);
            }

            if (EditorGUI.EndChangeCheck())
            {
                min = Mathf.Clamp(min, slider.Min, slider.Max);
                max = Mathf.Clamp(max, slider.Min, slider.Max);
                if (min > max)
                {
                    min = max;
                }

                property.vector2Value = new Vector2(min, max);
            }

            EditorGUI.EndProperty();
        }
    }
}
