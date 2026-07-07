using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CatAnnaDev.Editor
{
    internal sealed class CatAnnaDevSettingsProvider : SettingsProvider
    {
        private const string SettingsPath = "Project/CatAnnaDev";
        private const string ResourcesFolder = "Assets/CatAnnaDev/Resources";
        private const string SettingsAssetPath = "Assets/CatAnnaDev/Resources/CatAnnaDevSettings.asset";

        private SerializedObject serialized;

        private SerializedProperty enableLogging;
        private SerializedProperty logLevel;
        private SerializedProperty logColorHex;
        private SerializedProperty logInBuilds;
        private SerializedProperty defaultPoolCapacity;
        private SerializedProperty defaultPoolMaxSize;

        private CatAnnaDevSettingsProvider(string path, SettingsScope scope)
            : base(path, scope)
        {
        }

        [SettingsProvider]
        private static SettingsProvider Create()
        {
            CatAnnaDevSettingsProvider provider = new CatAnnaDevSettingsProvider(SettingsPath, SettingsScope.Project);
            provider.keywords = new HashSet<string>(new[]
            {
                "CatAnnaDev", "logging", "log", "pool", "pooling", "settings", "verbose"
            });
            return provider;
        }

        public override void OnActivate(string searchContext, UnityEngine.UIElements.VisualElement rootElement)
        {
            BindSettings();
        }

        private void BindSettings()
        {
            CatAnnaDevSettings asset = LoadOrCreate();
            serialized = new SerializedObject(asset);

            enableLogging = serialized.FindProperty("enableLogging");
            logLevel = serialized.FindProperty("logLevel");
            logColorHex = serialized.FindProperty("logColorHex");
            logInBuilds = serialized.FindProperty("logInBuilds");
            defaultPoolCapacity = serialized.FindProperty("defaultPoolCapacity");
            defaultPoolMaxSize = serialized.FindProperty("defaultPoolMaxSize");
        }

        public override void OnGUI(string searchContext)
        {
            if (serialized == null || serialized.targetObject == null)
            {
                BindSettings();
            }

            serialized.Update();

            EditorGUILayout.Space(6f);
            EditorGUIUtility.labelWidth = 200f;

            EditorGUILayout.LabelField("Logging", EditorStyles.boldLabel);
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(enableLogging, new GUIContent("Enable Logging"));
                EditorGUILayout.PropertyField(logLevel, new GUIContent("Log Level"));

                Color previous = ParseColor(logColorHex.stringValue);
                EditorGUI.BeginChangeCheck();
                Color picked = EditorGUILayout.ColorField(new GUIContent("Log Color"), previous);
                if (EditorGUI.EndChangeCheck())
                {
                    logColorHex.stringValue = "#" + ColorUtility.ToHtmlStringRGB(picked);
                }

                EditorGUILayout.PropertyField(logColorHex, new GUIContent("Log Color Hex"));
                EditorGUILayout.PropertyField(logInBuilds, new GUIContent("Log In Builds"));
            }

            EditorGUILayout.Space(8f);
            EditorGUILayout.LabelField("Pooling", EditorStyles.boldLabel);
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(defaultPoolCapacity, new GUIContent("Default Pool Capacity"));
                EditorGUILayout.PropertyField(defaultPoolMaxSize, new GUIContent("Default Pool Max Size"));

                if (defaultPoolCapacity.intValue < 0)
                {
                    defaultPoolCapacity.intValue = 0;
                }

                if (defaultPoolMaxSize.intValue < 1)
                {
                    defaultPoolMaxSize.intValue = 1;
                }
            }

            if (serialized.ApplyModifiedProperties())
            {
                EditorUtility.SetDirty(serialized.targetObject);
            }

            EditorGUILayout.Space(12f);
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Ping Asset", GUILayout.Width(120f)))
                {
                    EditorGUIUtility.PingObject(serialized.targetObject);
                }

                if (GUILayout.Button("Save", GUILayout.Width(120f)))
                {
                    AssetDatabase.SaveAssets();
                }
            }
        }

        private static Color ParseColor(string hex)
        {
            if (!string.IsNullOrEmpty(hex) && ColorUtility.TryParseHtmlString(hex, out Color color))
            {
                return color;
            }

            return Color.white;
        }

        private static CatAnnaDevSettings LoadOrCreate()
        {
            CatAnnaDevSettings asset = AssetDatabase.LoadAssetAtPath<CatAnnaDevSettings>(SettingsAssetPath);
            if (asset != null)
            {
                return asset;
            }

            CatAnnaDevMenu.EnsureFolder(ResourcesFolder);

            asset = ScriptableObject.CreateInstance<CatAnnaDevSettings>();
            AssetDatabase.CreateAsset(asset, SettingsAssetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return asset;
        }
    }
}
