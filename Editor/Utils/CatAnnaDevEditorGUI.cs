using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace CatAnnaDev.Editor
{
    internal static class CatAnnaDevEditorGUI
    {
        private const BindingFlags MemberFlags =
            BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        public static SerializedProperty FindSiblingProperty(SerializedProperty property, string siblingName)
        {
            if (property == null || string.IsNullOrEmpty(siblingName))
            {
                return null;
            }

            string path = property.propertyPath;
            int lastDot = path.LastIndexOf('.');
            if (lastDot < 0)
            {
                return property.serializedObject.FindProperty(siblingName);
            }

            string parentPath = path.Substring(0, lastDot);
            return property.serializedObject.FindProperty(parentPath + "." + siblingName);
        }

        public static bool EvaluateBool(SerializedProperty property, string memberName, bool fallback)
        {
            if (string.IsNullOrEmpty(memberName))
            {
                return fallback;
            }

            SerializedProperty sibling = FindSiblingProperty(property, memberName);
            if (sibling != null && sibling.propertyType == SerializedPropertyType.Boolean)
            {
                return sibling.boolValue;
            }

            object parent = GetParentObject(property);
            if (TryReadBoolMember(parent, memberName, out bool value))
            {
                return value;
            }

            object root = property.serializedObject.targetObject;
            if (!ReferenceEquals(root, parent) && TryReadBoolMember(root, memberName, out bool rootValue))
            {
                return rootValue;
            }

            return fallback;
        }

        public static bool TryReadFloatMember(SerializedProperty property, string memberName, out float value)
        {
            value = 0f;
            if (string.IsNullOrEmpty(memberName))
            {
                return false;
            }

            SerializedProperty sibling = FindSiblingProperty(property, memberName);
            if (sibling != null)
            {
                if (sibling.propertyType == SerializedPropertyType.Float)
                {
                    value = sibling.floatValue;
                    return true;
                }

                if (sibling.propertyType == SerializedPropertyType.Integer)
                {
                    value = sibling.longValue;
                    return true;
                }
            }

            object parent = GetParentObject(property);
            return TryReadFloatMember(parent, memberName, out value)
                   || TryReadFloatMember(property.serializedObject.targetObject, memberName, out value);
        }

        public static object GetParentObject(SerializedProperty property)
        {
            if (property == null)
            {
                return null;
            }

            string path = property.propertyPath.Replace(".Array.data[", "[");
            object current = property.serializedObject.targetObject;
            string[] elements = path.Split('.');
            for (int i = 0; i < elements.Length - 1; i++)
            {
                current = GetElementValue(current, elements[i]);
                if (current == null)
                {
                    return null;
                }
            }

            return current;
        }

        private static object GetElementValue(object source, string element)
        {
            if (source == null)
            {
                return null;
            }

            int bracket = element.IndexOf('[');
            if (bracket >= 0)
            {
                string name = element.Substring(0, bracket);
                string indexText = element.Substring(bracket + 1, element.Length - bracket - 2);
                if (!int.TryParse(indexText, out int index))
                {
                    return null;
                }

                object collection = GetMemberValue(source, name);
                return GetIndexedValue(collection, index);
            }

            return GetMemberValue(source, element);
        }

        private static object GetIndexedValue(object collection, int index)
        {
            if (collection is System.Collections.IList list)
            {
                return index >= 0 && index < list.Count ? list[index] : null;
            }

            if (collection is System.Collections.IEnumerable enumerable)
            {
                System.Collections.IEnumerator enumerator = enumerable.GetEnumerator();
                for (int i = 0; i <= index; i++)
                {
                    if (!enumerator.MoveNext())
                    {
                        return null;
                    }
                }

                return enumerator.Current;
            }

            return null;
        }

        private static object GetMemberValue(object source, string name)
        {
            Type type = source.GetType();
            while (type != null)
            {
                FieldInfo field = type.GetField(name, MemberFlags);
                if (field != null)
                {
                    return field.GetValue(source);
                }

                PropertyInfo property = type.GetProperty(name, MemberFlags);
                if (property != null && property.CanRead)
                {
                    return property.GetValue(source, null);
                }

                type = type.BaseType;
            }

            return null;
        }

        private static bool TryReadBoolMember(object target, string name, out bool value)
        {
            value = false;
            if (target == null)
            {
                return false;
            }

            Type type = target.GetType();
            while (type != null)
            {
                FieldInfo field = type.GetField(name, MemberFlags);
                if (field != null && field.FieldType == typeof(bool))
                {
                    value = (bool)field.GetValue(target);
                    return true;
                }

                PropertyInfo property = type.GetProperty(name, MemberFlags);
                if (property != null && property.CanRead && property.PropertyType == typeof(bool))
                {
                    value = (bool)property.GetValue(target, null);
                    return true;
                }

                MethodInfo method = type.GetMethod(name, MemberFlags, null, Type.EmptyTypes, null);
                if (method != null && method.ReturnType == typeof(bool))
                {
                    value = (bool)method.Invoke(target, null);
                    return true;
                }

                type = type.BaseType;
            }

            return false;
        }

        private static bool TryReadFloatMember(object target, string name, out float value)
        {
            value = 0f;
            if (target == null)
            {
                return false;
            }

            Type type = target.GetType();
            while (type != null)
            {
                FieldInfo field = type.GetField(name, MemberFlags);
                if (field != null && IsNumeric(field.FieldType))
                {
                    value = Convert.ToSingle(field.GetValue(target));
                    return true;
                }

                PropertyInfo property = type.GetProperty(name, MemberFlags);
                if (property != null && property.CanRead && IsNumeric(property.PropertyType))
                {
                    value = Convert.ToSingle(property.GetValue(target, null));
                    return true;
                }

                MethodInfo method = type.GetMethod(name, MemberFlags, null, Type.EmptyTypes, null);
                if (method != null && IsNumeric(method.ReturnType))
                {
                    value = Convert.ToSingle(method.Invoke(target, null));
                    return true;
                }

                type = type.BaseType;
            }

            return false;
        }

        private static bool IsNumeric(Type type)
        {
            return type == typeof(float) || type == typeof(int) || type == typeof(double)
                   || type == typeof(long) || type == typeof(short) || type == typeof(byte)
                   || type == typeof(uint) || type == typeof(ulong) || type == typeof(ushort);
        }

        public static void DrawHelpBox(Rect position, string message, MessageType type)
        {
            EditorGUI.HelpBox(position, message, type);
        }

        public static float GetHelpBoxHeight(string message, float width)
        {
            GUIContent content = new GUIContent(message);
            float height = EditorStyles.helpBox.CalcHeight(content, width);
            return Mathf.Max(height, EditorGUIUtility.singleLineHeight + 8f);
        }

        public static void DrawSeparator(Rect position)
        {
            Color color = EditorGUIUtility.isProSkin
                ? new Color(0.4f, 0.4f, 0.4f, 1f)
                : new Color(0.6f, 0.6f, 0.6f, 1f);
            EditorGUI.DrawRect(position, color);
        }
    }
}
