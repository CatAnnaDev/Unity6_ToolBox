using UnityEditor;
using UnityEngine;

namespace CatAnnaDev.Editor
{
    internal sealed class AboutWindow : EditorWindow
    {
        private static readonly string[] Features =
        {
            "Object Pooling",
            "Event Bus",
            "Service Locator and Singletons",
            "State Machine",
            "Timers and Scheduling",
            "Tweening",
            "Save System",
            "Scriptable Architecture",
            "Audio and VFX helpers",
            "Inspector Utilities"
        };

        private Vector2 scroll;

        [MenuItem("Tools/CatAnnaDev/About", false, 100)]
        private static void Open()
        {
            AboutWindow window = GetWindow<AboutWindow>(true, "About CatAnnaDev", true);
            window.minSize = new Vector2(360f, 420f);
            window.maxSize = new Vector2(520f, 640f);
            window.Show();
        }

        private void OnGUI()
        {
            scroll = EditorGUILayout.BeginScrollView(scroll);

            GUILayout.Space(8f);

            GUIStyle titleStyle = new GUIStyle(EditorStyles.largeLabel)
            {
                fontSize = 20,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            GUILayout.Label(PackVersion.Name, titleStyle);

            GUIStyle versionStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                alignment = TextAnchor.MiddleCenter
            };
            GUILayout.Label("Version " + PackVersion.Version, versionStyle);

            GUILayout.Space(10f);
            DrawSeparator();
            GUILayout.Space(10f);

            EditorGUILayout.LabelField("A drop-in developer quality-of-life toolkit for Unity.", EditorStyles.wordWrappedLabel);

            GUILayout.Space(10f);
            EditorGUILayout.LabelField("Included Systems", EditorStyles.boldLabel);
            GUILayout.Space(4f);

            for (int i = 0; i < Features.Length; i++)
            {
                EditorGUILayout.LabelField("  - " + Features[i]);
            }

            GUILayout.Space(12f);
            DrawSeparator();
            GUILayout.Space(12f);

            if (GUILayout.Button("Open Project Settings", GUILayout.Height(26f)))
            {
                SettingsService.OpenProjectSettings("Project/CatAnnaDev");
            }

            GUILayout.Space(4f);

            if (GUILayout.Button("Create Settings Asset", GUILayout.Height(26f)))
            {
                EditorApplication.ExecuteMenuItem("Tools/CatAnnaDev/Create Settings Asset");
            }

            GUILayout.Space(8f);
            EditorGUILayout.LabelField(PackVersion.Display, EditorStyles.centeredGreyMiniLabel);

            EditorGUILayout.EndScrollView();
        }

        private static void DrawSeparator()
        {
            Rect rect = EditorGUILayout.GetControlRect(false, 1f);
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.35f));
        }
    }
}
