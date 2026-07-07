using System;
using CatAnnaDev.Utils;
using UnityEditor;
using UnityEngine;

namespace CatAnnaDev.Editor
{
    [CustomPropertyDrawer(typeof(SerializableGuid))]
    internal sealed class SerializableGuidDrawer : PropertyDrawer
    {
        private const float ButtonWidth = 76f;
        private const float Gap = 4f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty p0 = property.FindPropertyRelative("part0");
            SerializedProperty p1 = property.FindPropertyRelative("part1");
            SerializedProperty p2 = property.FindPropertyRelative("part2");
            SerializedProperty p3 = property.FindPropertyRelative("part3");

            if (p0 == null || p1 == null || p2 == null || p3 == null)
            {
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }

            EditorGUI.BeginProperty(position, label, property);
            Rect content = EditorGUI.PrefixLabel(position, label);

            Rect fieldRect = new Rect(
                content.x,
                content.y,
                content.width - ButtonWidth - Gap,
                content.height);
            Rect buttonRect = new Rect(
                content.xMax - ButtonWidth,
                content.y,
                ButtonWidth,
                content.height);

            string display = BuildDisplay(p0, p1, p2, p3);
            bool previous = GUI.enabled;
            GUI.enabled = false;
            EditorGUI.TextField(fieldRect, display);
            GUI.enabled = previous;

            if (GUI.Button(buttonRect, "Regenerate"))
            {
                AssignGuid(Guid.NewGuid(), p0, p1, p2, p3);
            }

            EditorGUI.EndProperty();
        }

        private static string BuildDisplay(
            SerializedProperty p0, SerializedProperty p1, SerializedProperty p2, SerializedProperty p3)
        {
            byte[] bytes = new byte[16];
            BitConverter.GetBytes((uint)p0.longValue).CopyTo(bytes, 0);
            BitConverter.GetBytes((uint)p1.longValue).CopyTo(bytes, 4);
            BitConverter.GetBytes((uint)p2.longValue).CopyTo(bytes, 8);
            BitConverter.GetBytes((uint)p3.longValue).CopyTo(bytes, 12);
            Guid guid = new Guid(bytes);
            return guid == Guid.Empty ? "(empty)" : guid.ToString("N");
        }

        private static void AssignGuid(
            Guid guid, SerializedProperty p0, SerializedProperty p1, SerializedProperty p2, SerializedProperty p3)
        {
            byte[] bytes = guid.ToByteArray();
            p0.longValue = BitConverter.ToUInt32(bytes, 0);
            p1.longValue = BitConverter.ToUInt32(bytes, 4);
            p2.longValue = BitConverter.ToUInt32(bytes, 8);
            p3.longValue = BitConverter.ToUInt32(bytes, 12);
        }
    }
}
