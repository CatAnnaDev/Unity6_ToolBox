using CatAnnaDev.Utils;
using UnityEditor;
using UnityEngine;

namespace CatAnnaDev.Editor
{
    [CustomPropertyDrawer(typeof(HideIfAttribute))]
    internal sealed class HideIfDrawer : PropertyDrawer
    {
        private bool ShouldShow(SerializedProperty property)
        {
            HideIfAttribute hideIf = (HideIfAttribute)attribute;
            bool condition = CatAnnaDevEditorGUI.EvaluateBool(property, hideIf.ConditionMember, false);
            return !condition;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (ShouldShow(property))
            {
                return EditorGUI.GetPropertyHeight(property, label, true);
            }

            return -EditorGUIUtility.standardVerticalSpacing;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (ShouldShow(property))
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }
    }
}
