#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace CatAnnaDev.Samples
{
    internal static class SamplesMenu
    {
        private const string Root = "Tools/CatAnnaDev/Samples/";

        private static void Spawn<T>(string label) where T : MonoBehaviour
        {
            GameObject go = new GameObject(label);
            go.AddComponent<T>();
            Undo.RegisterCreatedObjectUndo(go, "Create " + label);
            Selection.activeGameObject = go;
            EditorGUIUtility.PingObject(go);
            SceneView.FrameLastActiveSceneView();
        }

        [MenuItem(Root + "Pooling Demo", false, 0)]
        private static void CreatePooling() => Spawn<PoolingDemo>("CatAnnaDev Pooling Demo");

        [MenuItem(Root + "Events Demo", false, 1)]
        private static void CreateEvents() => Spawn<EventsDemo>("CatAnnaDev Events Demo");

        [MenuItem(Root + "Services Demo", false, 2)]
        private static void CreateServices() => Spawn<ServicesDemo>("CatAnnaDev Services Demo");

        [MenuItem(Root + "State Machine Demo", false, 3)]
        private static void CreateStateMachine() => Spawn<StateMachineDemo>("CatAnnaDev State Machine Demo");

        [MenuItem(Root + "Timers Demo", false, 4)]
        private static void CreateTimers() => Spawn<TimersDemo>("CatAnnaDev Timers Demo");

        [MenuItem(Root + "Tweening Demo", false, 5)]
        private static void CreateTweening() => Spawn<TweeningDemo>("CatAnnaDev Tweening Demo");

        [MenuItem(Root + "Scheduling Demo", false, 6)]
        private static void CreateScheduling() => Spawn<SchedulingDemo>("CatAnnaDev Scheduling Demo");

        [MenuItem(Root + "Saving Demo", false, 7)]
        private static void CreateSaving() => Spawn<SavingDemo>("CatAnnaDev Saving Demo");

        [MenuItem(Root + "Scriptable Architecture Demo", false, 8)]
        private static void CreateScriptableArchitecture() => Spawn<ScriptableArchitectureDemo>("CatAnnaDev Scriptable Architecture Demo");

        [MenuItem(Root + "Audio Demo", false, 9)]
        private static void CreateAudio() => Spawn<AudioDemo>("CatAnnaDev Audio Demo");

        [MenuItem(Root + "VFX Demo", false, 10)]
        private static void CreateVFX() => Spawn<VFXDemo>("CatAnnaDev VFX Demo");

        [MenuItem(Root + "Utils Demo", false, 11)]
        private static void CreateUtils() => Spawn<UtilsDemo>("CatAnnaDev Utils Demo");

        [MenuItem(Root + "Attributes Showcase", false, 12)]
        private static void CreateAttributes() => Spawn<AttributesShowcase>("CatAnnaDev Attributes Showcase");
    }
}
#endif
