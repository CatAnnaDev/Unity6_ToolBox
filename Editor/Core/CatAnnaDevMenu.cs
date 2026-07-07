using System.IO;
using UnityEditor;
using UnityEngine;

namespace CatAnnaDev.Editor
{
    internal static class CatAnnaDevMenu
    {
        private const string ResourcesFolder = "Assets/CatAnnaDev/Resources";
        private const string SettingsAssetPath = "Assets/CatAnnaDev/Resources/CatAnnaDevSettings.asset";
        private const string ReadmePath = "Assets/CatAnnaDev/README.md";

        [MenuItem("Tools/CatAnnaDev/Create Settings Asset", false, 0)]
        private static void CreateSettingsAsset()
        {
            CatAnnaDevSettings existing = AssetDatabase.LoadAssetAtPath<CatAnnaDevSettings>(SettingsAssetPath);
            if (existing != null)
            {
                Selection.activeObject = existing;
                EditorGUIUtility.PingObject(existing);
                CatLog.Info("Settings asset already exists at " + SettingsAssetPath);
                return;
            }

            EnsureFolder(ResourcesFolder);

            CatAnnaDevSettings settings = ScriptableObject.CreateInstance<CatAnnaDevSettings>();
            AssetDatabase.CreateAsset(settings, SettingsAssetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Selection.activeObject = settings;
            EditorGUIUtility.PingObject(settings);
            CatLog.Info("Created settings asset at " + SettingsAssetPath);
        }

        [MenuItem("Tools/CatAnnaDev/Documentation", false, 20)]
        private static void OpenDocumentation()
        {
            string absolute = Path.GetFullPath(ReadmePath);
            if (File.Exists(absolute))
            {
                EditorUtility.RevealInFinder(absolute);
                return;
            }

            EditorUtility.DisplayDialog(
                "CatAnnaDev",
                "README not found at " + ReadmePath + ".",
                "OK");
        }

        [MenuItem("Tools/CatAnnaDev/Select Settings", false, 21)]
        private static void SelectSettings()
        {
            CatAnnaDevSettings settings = AssetDatabase.LoadAssetAtPath<CatAnnaDevSettings>(SettingsAssetPath);
            if (settings == null)
            {
                CreateSettingsAsset();
                return;
            }

            Selection.activeObject = settings;
            EditorGUIUtility.PingObject(settings);
        }

        [MenuItem("Tools/CatAnnaDev/Open Project Settings", false, 22)]
        private static void OpenProjectSettings()
        {
            SettingsService.OpenProjectSettings("Project/CatAnnaDev");
        }

        internal static void EnsureFolder(string folder)
        {
            if (AssetDatabase.IsValidFolder(folder))
            {
                return;
            }

            string parent = Path.GetDirectoryName(folder).Replace('\\', '/');
            string leaf = Path.GetFileName(folder);

            if (!AssetDatabase.IsValidFolder(parent))
            {
                EnsureFolder(parent);
            }

            AssetDatabase.CreateFolder(parent, leaf);
        }
    }
}
