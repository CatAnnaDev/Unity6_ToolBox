using CatAnnaDev.Utils;
using UnityEditor;
using UnityEngine;

namespace CatAnnaDev.Editor
{
    [CustomPropertyDrawer(typeof(EnableIfAttribute))]
    internal sealed class EnableIfDrawer : PropertyDrawer
    {
        private bool ShouldEnable(SerializedProperty property)
        {
            EnableIfAttribute enableIf = (EnableIfAttribute)attribute;
            bool condition = CatAnnaDevEditorGUI.EvaluateBool(property, enableIf.ConditionMember, true);
            return enableIf.Invert ? !condition : condition;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            bool previous = GUI.enabled;
            GUI.enabled = previous && ShouldEnable(property);
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = previous;
        }
    }
}
