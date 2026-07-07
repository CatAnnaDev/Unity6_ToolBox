using System;
using System.Collections.Generic;
using CatAnnaDev.StateMachine;
using UnityEditor;
using UnityEngine;

namespace CatAnnaDev.Editor
{
    [CustomEditor(typeof(StateMachineRunner), true)]
    public sealed class StateMachineRunnerEditor : UnityEditor.Editor
    {
        private const int MaxHistoryEntries = 24;

        private readonly List<string> _history = new List<string>(MaxHistoryEntries);

        private StateMachineRunner _runner;
        private StateMachine.StateMachine _subscribedMachine;
        private Action<IState, IState> _handler;
        private Vector2 _historyScroll;

        private GUIStyle _currentStateStyle;
        private GUIStyle _historyEntryStyle;

        private void OnEnable()
        {
            _runner = target as StateMachineRunner;
            _handler = OnStateChanged;
        }

        private void OnDisable()
        {
            UnsubscribeFromMachine();
        }

        public override bool RequiresConstantRepaint()
        {
            return Application.isPlaying;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Runtime", EditorStyles.boldLabel);

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Enter Play Mode to inspect live state and transition history.", MessageType.Info);
                return;
            }

            EnsureStyles();
            SyncSubscription();

            StateMachine.StateMachine machine = _runner != null ? _runner.Machine : null;
            if (machine == null)
            {
                EditorGUILayout.HelpBox("No StateMachine assigned. Build or SetMachine on the runner.", MessageType.Warning);
                return;
            }

            DrawCurrentState(machine);
            DrawHistory();
        }

        private void DrawCurrentState(StateMachine.StateMachine machine)
        {
            string name = ResolveStateName(machine.CurrentState);
            bool running = _runner != null && _runner.IsRunning;

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Current State", EditorStyles.miniBoldLabel);
                EditorGUILayout.LabelField(name, _currentStateStyle);
                EditorGUILayout.LabelField("Running", running ? "Yes" : "No", EditorStyles.miniLabel);
            }
        }

        private void DrawHistory()
        {
            EditorGUILayout.Space();
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Transition History", EditorStyles.boldLabel);
                if (GUILayout.Button("Clear", EditorStyles.miniButton, GUILayout.Width(56f)))
                {
                    _history.Clear();
                }
            }

            if (_history.Count == 0)
            {
                EditorGUILayout.LabelField("No transitions recorded yet.", EditorStyles.miniLabel);
                return;
            }

            using (EditorGUILayout.ScrollViewScope scroll = new EditorGUILayout.ScrollViewScope(_historyScroll, GUILayout.MaxHeight(180f)))
            {
                _historyScroll = scroll.scrollPosition;
                for (int i = _history.Count - 1; i >= 0; i--)
                {
                    EditorGUILayout.LabelField(_history[i], _historyEntryStyle);
                }
            }
        }

        private void SyncSubscription()
        {
            StateMachine.StateMachine machine = _runner != null ? _runner.Machine : null;
            if (ReferenceEquals(machine, _subscribedMachine))
            {
                return;
            }

            UnsubscribeFromMachine();

            if (machine != null)
            {
                machine.OnStateChanged += _handler;
                _subscribedMachine = machine;
            }
        }

        private void UnsubscribeFromMachine()
        {
            if (_subscribedMachine != null && _handler != null)
            {
                _subscribedMachine.OnStateChanged -= _handler;
            }

            _subscribedMachine = null;
        }

        private void OnStateChanged(IState previous, IState next)
        {
            string from = previous == null ? "(none)" : ResolveStateName(previous);
            string to = ResolveStateName(next);
            string stamp = Time.realtimeSinceStartup.ToString("0.00");
            string entry = string.Concat("[", stamp, "s]  ", from, "  ->  ", to);

            _history.Add(entry);
            if (_history.Count > MaxHistoryEntries)
            {
                _history.RemoveAt(0);
            }

            Repaint();
        }

        private static string ResolveStateName(IState state)
        {
            if (state == null)
            {
                return "(none)";
            }

            if (state is State typed)
            {
                return typed.Name;
            }

            if (state is DelegateState delegateState)
            {
                return delegateState.Name;
            }

            return state.ToString();
        }

        private void EnsureStyles()
        {
            if (_currentStateStyle == null)
            {
                _currentStateStyle = new GUIStyle(EditorStyles.boldLabel);
                _currentStateStyle.fontSize = 15;
            }

            if (_historyEntryStyle == null)
            {
                _historyEntryStyle = new GUIStyle(EditorStyles.miniLabel);
                _historyEntryStyle.richText = false;
                _historyEntryStyle.wordWrap = false;
            }
        }
    }
}
