using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using CatAnnaDev.Services;
using UnityEditor;
using UnityEngine;

namespace CatAnnaDev.Editor
{
    public sealed class ServiceLocatorWindow : EditorWindow
    {
        private const string LocatorFieldName = "services";
        private const float RefreshInterval = 0.5f;

        private static readonly GUIContent WindowTitle = new GUIContent("Service Locator");

        private readonly List<ServiceRow> rows = new List<ServiceRow>(32);

        private Vector2 scroll;
        private double nextRefreshTime;
        private FieldInfo cachedServicesField;
        private bool cachedFieldResolved;
        private bool autoRefresh = true;

        [MenuItem("Tools/CatAnnaDev/Services")]
        private static void Open()
        {
            ServiceLocatorWindow window = GetWindow<ServiceLocatorWindow>();
            window.titleContent = WindowTitle;
            window.minSize = new Vector2(320f, 200f);
            window.Show();
        }

        private void OnEnable()
        {
            RebuildRows();
        }

        private void OnInspectorUpdate()
        {
            if (!autoRefresh)
            {
                return;
            }

            if (!Application.isPlaying)
            {
                return;
            }

            if (EditorApplication.timeSinceStartup < nextRefreshTime)
            {
                return;
            }

            nextRefreshTime = EditorApplication.timeSinceStartup + RefreshInterval;
            RebuildRows();
            Repaint();
        }

        private void OnGUI()
        {
            DrawToolbar();

            if (!Application.isPlaying)
            {
                DrawNotPlayingGuidance();
                return;
            }

            if (rows.Count == 0)
            {
                EditorGUILayout.HelpBox(
                    "No services are currently registered. Register one with ServiceLocator.Register<T>(instance).",
                    MessageType.Info);
                return;
            }

            DrawHeader();
            DrawRows();
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            int count = Application.isPlaying ? SafeCount() : 0;
            GUILayout.Label("Registered: " + count, EditorStyles.toolbarButton, GUILayout.Width(140f));

            GUILayout.FlexibleSpace();

            autoRefresh = GUILayout.Toggle(autoRefresh, "Auto Refresh", EditorStyles.toolbarButton, GUILayout.Width(100f));

            if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(70f)))
            {
                RebuildRows();
                Repaint();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawNotPlayingGuidance()
        {
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox(
                "The Service Locator is a runtime container. Enter Play Mode to inspect the services registered through ServiceLocator.Register<T>().",
                MessageType.Info);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Usage", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Register", "ServiceLocator.Register<IMyService>(instance);");
            EditorGUILayout.LabelField("Resolve", "var svc = ServiceLocator.Get<IMyService>();");
            EditorGUILayout.LabelField("Try", "ServiceLocator.TryGet<IMyService>(out var svc);");
            EditorGUILayout.LabelField("Remove", "ServiceLocator.Unregister<IMyService>();");
        }

        private void DrawHeader()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Service Type", EditorStyles.boldLabel, GUILayout.MinWidth(140f));
            EditorGUILayout.LabelField("Value", EditorStyles.boldLabel, GUILayout.MinWidth(140f));
            EditorGUILayout.LabelField("Lifecycle", EditorStyles.boldLabel, GUILayout.Width(70f));
            EditorGUILayout.EndHorizontal();
        }

        private void DrawRows()
        {
            scroll = EditorGUILayout.BeginScrollView(scroll);

            for (int i = 0; i < rows.Count; i++)
            {
                ServiceRow row = rows[i];

                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent(row.TypeName, row.FullTypeName), GUILayout.MinWidth(140f));
                EditorGUILayout.LabelField(row.Value, GUILayout.MinWidth(140f));
                EditorGUILayout.LabelField(row.IsService ? "IService" : "-", GUILayout.Width(70f));

                using (new EditorGUI.DisabledScope(!row.HasUnityObject))
                {
                    if (GUILayout.Button("Ping", GUILayout.Width(48f)) && row.HasUnityObject)
                    {
                        EditorGUIUtility.PingObject(row.UnityObject);
                        Selection.activeObject = row.UnityObject;
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }

        private void RebuildRows()
        {
            rows.Clear();

            if (!Application.isPlaying)
            {
                return;
            }

            IDictionary map = ResolveServiceMap();
            if (map == null)
            {
                return;
            }

            foreach (DictionaryEntry entry in map)
            {
                Type key = entry.Key as Type;
                object value = entry.Value;

                string typeName = key != null ? key.Name : "<unknown>";
                string fullName = key != null ? key.FullName : typeName;
                string valueText = DescribeValue(value);

                UnityEngine.Object unityObject = value as UnityEngine.Object;

                rows.Add(new ServiceRow(
                    typeName,
                    fullName,
                    valueText,
                    value is IService,
                    unityObject));
            }

            rows.Sort(CompareRows);
        }

        private static int CompareRows(ServiceRow a, ServiceRow b)
        {
            return string.Compare(a.TypeName, b.TypeName, StringComparison.OrdinalIgnoreCase);
        }

        private static string DescribeValue(object value)
        {
            if (value == null)
            {
                return "<null>";
            }

            UnityEngine.Object unityObject = value as UnityEngine.Object;
            if (unityObject != null)
            {
                return unityObject.name + " (" + value.GetType().Name + ")";
            }

            string text;
            try
            {
                text = value.ToString();
            }
            catch (Exception exception)
            {
                text = "<ToString threw: " + exception.GetType().Name + ">";
            }

            if (string.IsNullOrEmpty(text))
            {
                return value.GetType().Name;
            }

            return text;
        }

        private int SafeCount()
        {
            IDictionary map = ResolveServiceMap();
            if (map != null)
            {
                return map.Count;
            }

            return ServiceLocator.Count;
        }

        private IDictionary ResolveServiceMap()
        {
            FieldInfo field = ResolveServicesField();
            if (field == null)
            {
                return null;
            }

            return field.GetValue(null) as IDictionary;
        }

        private FieldInfo ResolveServicesField()
        {
            if (cachedFieldResolved)
            {
                return cachedServicesField;
            }

            cachedFieldResolved = true;
            cachedServicesField = typeof(ServiceLocator).GetField(
                LocatorFieldName,
                BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

            return cachedServicesField;
        }

        private readonly struct ServiceRow
        {
            public readonly string TypeName;
            public readonly string FullTypeName;
            public readonly string Value;
            public readonly bool IsService;
            public readonly UnityEngine.Object UnityObject;

            public ServiceRow(string typeName, string fullTypeName, string value, bool isService, UnityEngine.Object unityObject)
            {
                TypeName = typeName;
                FullTypeName = fullTypeName;
                Value = value;
                IsService = isService;
                UnityObject = unityObject;
            }

            public bool HasUnityObject
            {
                get { return UnityObject != null; }
            }
        }
    }
}
