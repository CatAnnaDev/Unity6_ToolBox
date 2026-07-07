using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using CatAnnaDev.Input;
using CatAnnaDev.Lifecycle;
#endif

namespace CatAnnaDev.Samples
{
    public sealed class InputDemo : MonoBehaviour
    {
        private GUIStyle boxStyle;
        private GUIStyle titleStyle;
        private GUIStyle bodyStyle;

#if ENABLE_INPUT_SYSTEM
        private InputAction move;
        private int started;
        private int performed;
        private int canceled;
        private string lastPhase = "none yet";

        private void Start()
        {
            InputAction jump = new InputAction("Jump", InputActionType.Button, "<Keyboard>/space");
            jump.OnStarted(() => { started++; lastPhase = "started"; })
                .OnPerformed(() => { performed++; lastPhase = "performed"; })
                .OnCanceled(() => { canceled++; lastPhase = "canceled"; })
                .Enable()
                .DisposeWith(gameObject);

            move = new InputAction("Move", InputActionType.Value);
            move.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d");
            move.Enable();

            gameObject.OnDestroyed(() =>
            {
                move.Disable();
                move.Dispose();
            });
        }

        private void OnGUI()
        {
            EnsureStyles();

            GUILayout.BeginArea(new Rect(12f, 12f, 460f, 300f), boxStyle);
            GUILayout.Label("CatAnnaDev - Input System demo", titleStyle);
            GUILayout.Space(4f);
            GUILayout.Label(
                "Fluent binding with auto-dispose tied to the GameObject lifecycle:\n" +
                "jump.OnPerformed(...).OnCanceled(...).Enable().DisposeWith(gameObject).",
                bodyStyle);

            GUILayout.Space(8f);
            GUILayout.Label("Jump  [Space]", titleStyle);
            GUILayout.Label(
                "started : " + started +
                "   performed : " + performed +
                "   canceled : " + canceled +
                "\nlast phase : " + lastPhase,
                bodyStyle);

            GUILayout.Space(6f);
            Vector2 axis = move.ReadValue<Vector2>();
            GUILayout.Label("Move  [WASD]", titleStyle);
            GUILayout.Label("value : " + axis.ToString("0.00"), bodyStyle);
            GUILayout.EndArea();
        }
#else
        private void OnGUI()
        {
            EnsureStyles();
            GUILayout.BeginArea(new Rect(12f, 12f, 460f, 120f), boxStyle);
            GUILayout.Label("CatAnnaDev - Input System demo", titleStyle);
            GUILayout.Space(4f);
            GUILayout.Label("The Input System package is not enabled in this project.", bodyStyle);
            GUILayout.EndArea();
        }
#endif

        private void EnsureStyles()
        {
            if (boxStyle != null) return;
            boxStyle = new GUIStyle(GUI.skin.box) { padding = new RectOffset(12, 12, 12, 12) };
            titleStyle = new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold };
            bodyStyle = new GUIStyle(GUI.skin.label) { wordWrap = true };
        }
    }
}
