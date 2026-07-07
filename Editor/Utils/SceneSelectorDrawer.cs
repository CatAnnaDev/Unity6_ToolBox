using System.Collections.Generic;
using CatAnnaDev.Utils;
using UnityEditor;
using UnityEngine;

namespace CatAnnaDev.Editor
{
    [CustomPropertyDrawer(typeof(SceneSelectorAttribute))]
    internal sealed class SceneSelectorDrawer : PropertyDrawer
    {
        private static readonly List<string> NameBuffer = new List<string>();
        private static readonly List<GUIContent> ContentBuffer = new List<GUIContent>();

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            SceneSelectorAttribute attr = (SceneSelectorAttribute)attribute;
            BuildSceneList(attr.OnlyBuildScenes);

            EditorGUI.BeginProperty(position, label, property);

            if (NameBuffer.Count == 0)
            {
                Rect labelRect = EditorGUI.PrefixLabel(position, label);
                EditorGUI.HelpBox(labelRect, "No scenes available.", MessageType.None);
                EditorGUI.EndProperty();
                return;
            }

            int currentIndex = NameBuffer.IndexOf(property.stringValue);
            bool missing = currentIndex < 0;
            if (missing)
            {
                NameBuffer.Add(property.stringValue);
                ContentBuffer.Add(new GUIContent(
                    string.IsNullOrEmpty(property.stringValue)
                        ? "(none)"
                        : property.stringValue + " (missing)"));
                currentIndex = NameBuffer.Count - 1;
            }

            EditorGUI.BeginChangeCheck();
            int selected = EditorGUI.Popup(position, label, currentIndex, ContentBuffer.ToArray());
            if (EditorGUI.EndChangeCheck() && selected >= 0 && selected < NameBuffer.Count)
            {
                property.stringValue = NameBuffer[selected];
            }

            EditorGUI.EndProperty();
        }

        private static void BuildSceneList(bool onlyBuildScenes)
        {
            NameBuffer.Clear();
            ContentBuffer.Clear();

            if (onlyBuildScenes)
            {
                EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
                for (int i = 0; i < scenes.Length; i++)
                {
                    if (!scenes[i].enabled)
                    {
                        continue;
                    }

                    string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenes[i].path);
                    NameBuffer.Add(sceneName);
                    ContentBuffer.Add(new GUIContent(sceneName));
                }

                return;
            }

            string[] guids = AssetDatabase.FindAssets("t:Scene");
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(path);
                NameBuffer.Add(sceneName);
                ContentBuffer.Add(new GUIContent(sceneName, path));
            }
        }
    }
}
