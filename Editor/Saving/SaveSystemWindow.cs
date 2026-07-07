using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CatAnnaDev.Saving;
using UnityEditor;
using UnityEngine;

namespace CatAnnaDev.Editor
{
    public sealed class SaveSystemWindow : EditorWindow
    {
        private struct SlotInfo
        {
            public string Slot;
            public string Path;
            public long Size;
            public DateTime LastWrite;
            public bool Exists;
        }

        private static readonly string[] SizeUnits = { "B", "KB", "MB", "GB", "TB" };

        private readonly List<SlotInfo> slots = new List<SlotInfo>();
        private Vector2 scroll;
        private GUIStyle headerStyle;
        private GUIStyle pathStyle;
        private GUIStyle metaStyle;
        private double lastRefreshTime;

        [MenuItem("Tools/CatAnnaDev/Save System")]
        private static void Open()
        {
            SaveSystemWindow window = GetWindow<SaveSystemWindow>(false, "Save System", true);
            window.minSize = new Vector2(420f, 240f);
            window.Show();
        }

        private void OnEnable()
        {
            Refresh();
        }

        private void OnFocus()
        {
            Refresh();
        }

        private void Refresh()
        {
            slots.Clear();

            string directory = SaveDirectory();
            string extension = SaveExtension();

            if (!string.IsNullOrEmpty(directory) && Directory.Exists(directory))
            {
                string[] files = Directory.GetFiles(directory, "*" + extension, SearchOption.TopDirectoryOnly);
                Array.Sort(files, StringComparer.Ordinal);

                for (int i = 0; i < files.Length; i++)
                {
                    string file = files[i];
                    SlotInfo info = new SlotInfo();
                    info.Slot = Path.GetFileNameWithoutExtension(file);
                    info.Path = file;

                    try
                    {
                        FileInfo fileInfo = new FileInfo(file);
                        info.Exists = fileInfo.Exists;
                        info.Size = info.Exists ? fileInfo.Length : 0L;
                        info.LastWrite = info.Exists ? fileInfo.LastWriteTime : DateTime.MinValue;
                    }
                    catch (Exception exception)
                    {
                        Debug.LogWarning("SaveSystemWindow failed to read info for '" + file + "': " + exception.Message);
                        info.Exists = false;
                    }

                    slots.Add(info);
                }
            }

            lastRefreshTime = EditorApplication.timeSinceStartup;
            Repaint();
        }

        private static string SaveDirectory()
        {
            return SaveSystem.SaveDirectory;
        }

        private static string SaveExtension()
        {
            ISaveSerializer serializer = SaveSystem.Serializer;
            string extension = serializer != null ? serializer.Extension : ".sav";
            return string.IsNullOrEmpty(extension) ? ".sav" : extension;
        }

        private void EnsureStyles()
        {
            if (headerStyle == null)
            {
                headerStyle = new GUIStyle(EditorStyles.boldLabel);
                headerStyle.fontSize = 12;
            }

            if (pathStyle == null)
            {
                pathStyle = new GUIStyle(EditorStyles.miniLabel);
                pathStyle.wordWrap = true;
            }

            if (metaStyle == null)
            {
                metaStyle = new GUIStyle(EditorStyles.miniLabel);
                metaStyle.alignment = TextAnchor.MiddleRight;
            }
        }

        private void OnGUI()
        {
            EnsureStyles();
            DrawToolbar();
            DrawDirectoryLine();
            DrawSlots();
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(70f)))
            {
                Refresh();
            }

            if (GUILayout.Button("Open Folder", EditorStyles.toolbarButton, GUILayout.Width(90f)))
            {
                RevealDirectory();
            }

            GUILayout.FlexibleSpace();

            GUILayout.Label(slots.Count + (slots.Count == 1 ? " slot" : " slots"), EditorStyles.miniLabel);

            using (new EditorGUI.DisabledScope(slots.Count == 0))
            {
                if (GUILayout.Button("Delete All", EditorStyles.toolbarButton, GUILayout.Width(80f)))
                {
                    DeleteAll();
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawDirectoryLine()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Directory", GUILayout.Width(70f));

            string directory = SaveDirectory();
            EditorGUILayout.SelectableLabel(directory, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));

            EditorGUILayout.EndHorizontal();
        }

        private void DrawSlots()
        {
            if (slots.Count == 0)
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("No save slots found in the save directory.", MessageType.Info);
                return;
            }

            scroll = EditorGUILayout.BeginScrollView(scroll);

            for (int i = 0; i < slots.Count; i++)
            {
                DrawSlot(slots[i]);
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawSlot(SlotInfo info)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(info.Slot, headerStyle);
            GUILayout.FlexibleSpace();
            GUILayout.Label(FormatSize(info.Size) + "   " + FormatDate(info.LastWrite), metaStyle);
            EditorGUILayout.EndHorizontal();

            GUILayout.Label(info.Path, pathStyle);

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Reveal In Finder", GUILayout.Width(130f)))
            {
                EditorUtility.RevealInFinder(info.Path);
            }

            if (GUILayout.Button("Delete", GUILayout.Width(80f)))
            {
                DeleteSlot(info);
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        private void RevealDirectory()
        {
            string directory = SaveDirectory();
            if (string.IsNullOrEmpty(directory))
            {
                return;
            }

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            EditorUtility.RevealInFinder(directory);
        }

        private void DeleteSlot(SlotInfo info)
        {
            bool confirmed = EditorUtility.DisplayDialog(
                "Delete Save Slot",
                "Delete save slot '" + info.Slot + "'?\n\nThis cannot be undone.",
                "Delete",
                "Cancel");

            if (!confirmed)
            {
                return;
            }

            if (!SaveSystem.Delete(info.Slot))
            {
                TryDeleteFile(info.Path);
            }

            Refresh();
        }

        private void DeleteAll()
        {
            bool confirmed = EditorUtility.DisplayDialog(
                "Delete All Save Slots",
                "Delete all " + slots.Count + " save slots?\n\nThis cannot be undone.",
                "Delete All",
                "Cancel");

            if (!confirmed)
            {
                return;
            }

            for (int i = 0; i < slots.Count; i++)
            {
                SlotInfo info = slots[i];
                if (!SaveSystem.Delete(info.Slot))
                {
                    TryDeleteFile(info.Path);
                }
            }

            Refresh();
        }

        private static void TryDeleteFile(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch (Exception exception)
            {
                Debug.LogError("SaveSystemWindow failed to delete '" + path + "': " + exception.Message);
            }
        }

        private static string FormatSize(long bytes)
        {
            if (bytes <= 0L)
            {
                return "0 B";
            }

            double size = bytes;
            int unit = 0;
            while (size >= 1024.0 && unit < SizeUnits.Length - 1)
            {
                size /= 1024.0;
                unit++;
            }

            string number = unit == 0
                ? size.ToString("0", CultureInfo.InvariantCulture)
                : size.ToString("0.##", CultureInfo.InvariantCulture);

            return number + " " + SizeUnits[unit];
        }

        private static string FormatDate(DateTime date)
        {
            if (date == DateTime.MinValue)
            {
                return "-";
            }

            return date.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
        }
    }
}
