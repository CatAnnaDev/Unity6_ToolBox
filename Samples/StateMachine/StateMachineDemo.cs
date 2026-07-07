using System.Collections.Generic;
using UnityEngine;
using CatAnnaDev.StateMachine;

namespace CatAnnaDev.Samples
{
    public sealed class StateMachineDemo : MonoBehaviour
    {
        private const float IdleDuration = 1.5f;
        private const float PatrolRange = 8f;
        private const float PatrolSpeed = 2f;
        private const float ChaseSpeed = 4f;
        private const float ChaseRadius = 3f;
        private const float LoseRadius = 5f;
        private const float TargetMoveSpeed = 6f;
        private const float GroundY = 0.5f;

        private static readonly Color IdleColor = new Color(0.55f, 0.55f, 0.6f);
        private static readonly Color PatrolColor = new Color(0.3f, 0.75f, 0.4f);
        private static readonly Color ChaseColor = new Color(0.85f, 0.3f, 0.3f);

        private global::CatAnnaDev.StateMachine.StateMachine _machine;
        private readonly List<Object> _owned = new List<Object>();

        private GameObject _agent;
        private GameObject _target;
        private Renderer _agentRenderer;

        private bool _forceIdle;
        private bool _forcePatrol;
        private bool _forceChase;

        private string _lastTransition = "(none yet)";

        private void Start()
        {
            EnsureCamera();
            EnsureLight();
            CreateGround();

            _agent = CreatePrimitive(PrimitiveType.Cube, "FsmAgent", new Vector3(0f, GroundY, 0f));
            _agentRenderer = _agent.GetComponent<Renderer>();

            _target = CreatePrimitive(PrimitiveType.Sphere, "FsmTarget", new Vector3(6f, GroundY, 0f));
            SetColor(_target.GetComponent<Renderer>(), new Color(0.35f, 0.55f, 0.95f));

            _machine = BuildMachine();
            _machine.OnStateChanged += HandleStateChanged;
        }

        private global::CatAnnaDev.StateMachine.StateMachine BuildMachine()
        {
            StateMachineBuilder builder = new StateMachineBuilder();

            builder.State("Idle")
                .OnEnter(() => Recolor(IdleColor))
                .OnTick(TickIdle)
                .TransitionTo("Patrol").After(IdleDuration);

            builder.State("Patrol")
                .OnEnter(() => Recolor(PatrolColor))
                .OnTick(TickPatrol)
                .TransitionTo("Chase").When(() => TargetDistance() <= ChaseRadius);

            builder.State("Chase")
                .OnEnter(() => Recolor(ChaseColor))
                .OnTick(TickChase)
                .TransitionTo("Patrol").When(() => TargetDistance() >= LoseRadius);

            builder.AnyTransitionTo("Idle").When(ConsumeForceIdle);
            builder.AnyTransitionTo("Patrol").When(ConsumeForcePatrol);
            builder.AnyTransitionTo("Chase").When(ConsumeForceChase);

            builder.Initial("Idle");
            return builder.Build();
        }

        private void Update()
        {
            ReadTargetInput();
            ReadForceKeys();
            _machine?.Tick();
        }

        private void ReadTargetInput()
        {
            Vector3 move = new Vector3(DemoInput.GetAxisRaw("Horizontal"), 0f, DemoInput.GetAxisRaw("Vertical"));
            if (move.sqrMagnitude > 1f)
            {
                move.Normalize();
            }

            Vector3 position = _target.transform.position + move * (TargetMoveSpeed * Time.deltaTime);
            position.y = GroundY;
            _target.transform.position = position;
        }

        private void ReadForceKeys()
        {
            if (DemoInput.GetKeyDown(KeyCode.Alpha1))
            {
                _forceIdle = true;
            }

            if (DemoInput.GetKeyDown(KeyCode.Alpha2))
            {
                _forcePatrol = true;
            }

            if (DemoInput.GetKeyDown(KeyCode.Alpha3))
            {
                _forceChase = true;
            }
        }

        private void TickIdle()
        {
            _agent.transform.Rotate(0f, 60f * Time.deltaTime, 0f);
        }

        private void TickPatrol()
        {
            float x = Mathf.PingPong(Time.time * PatrolSpeed, PatrolRange) - PatrolRange * 0.5f;
            Vector3 position = _agent.transform.position;
            position.x = x;
            position.z = 0f;
            _agent.transform.position = position;
        }

        private void TickChase()
        {
            Vector3 goal = _target.transform.position;
            goal.y = _agent.transform.position.y;
            _agent.transform.position = Vector3.MoveTowards(_agent.transform.position, goal, ChaseSpeed * Time.deltaTime);
        }

        private float TargetDistance()
        {
            Vector3 a = _agent.transform.position;
            Vector3 b = _target.transform.position;
            a.y = 0f;
            b.y = 0f;
            return Vector3.Distance(a, b);
        }

        private bool ConsumeForceIdle()
        {
            if (!_forceIdle)
            {
                return false;
            }

            _forceIdle = false;
            return true;
        }

        private bool ConsumeForcePatrol()
        {
            if (!_forcePatrol)
            {
                return false;
            }

            _forcePatrol = false;
            return true;
        }

        private bool ConsumeForceChase()
        {
            if (!_forceChase)
            {
                return false;
            }

            _forceChase = false;
            return true;
        }

        private void HandleStateChanged(IState from, IState to)
        {
            string fromName = from == null ? "(start)" : from.ToString();
            _lastTransition = fromName + " -> " + to;
        }

        private void Recolor(Color color)
        {
            if (_agentRenderer != null)
            {
                SetColor(_agentRenderer, color);
            }
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10f, 10f, 430f, 320f), GUI.skin.box);

            GUIStyle title = new GUIStyle(GUI.skin.label) { fontSize = 16, fontStyle = FontStyle.Bold };
            GUIStyle body = new GUIStyle(GUI.skin.label) { fontSize = 12, wordWrap = true };

            GUILayout.Label("CatAnnaDev StateMachine Demo", title);
            GUILayout.Label(
                "A finite state machine drives the cube. States are wired with the fluent " +
                "StateMachineBuilder: Idle waits, Patrol paces along X, Chase homes on the " +
                "blue sphere. Transitions fire on a timer and on distance to the sphere.",
                body);

            GUILayout.Space(6f);
            GUILayout.Label("CONTROLS", title);
            GUILayout.Label("WASD / Arrows : move the blue target sphere", body);
            GUILayout.Label("1 : force Idle     2 : force Patrol     3 : force Chase", body);
            GUILayout.Label("Move the sphere within " + ChaseRadius.ToString("0.#") + " units to trigger Chase automatically.", body);

            GUILayout.Space(6f);
            GUILayout.Label("STATUS", title);
            string current = _machine?.CurrentState == null ? "(none)" : _machine.CurrentState.ToString();
            GUILayout.Label("Current state : " + current, body);
            GUILayout.Label("Last transition : " + _lastTransition, body);
            GUILayout.Label("Distance to target : " + TargetDistance().ToString("0.00"), body);

            GUILayout.EndArea();
        }

        private void EnsureCamera()
        {
            if (Camera.main != null)
            {
                return;
            }

            GameObject cameraObject = new GameObject("FsmDemoCamera");
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.tag = "MainCamera";
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.12f, 0.13f, 0.16f);
            cameraObject.transform.position = new Vector3(0f, 11f, -10f);
            cameraObject.transform.LookAt(Vector3.zero);
            _owned.Add(cameraObject);
        }

        private void EnsureLight()
        {
            if (FindObjectOfType<Light>() != null)
            {
                return;
            }

            GameObject lightObject = new GameObject("FsmDemoLight");
            Light light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.1f;
            lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            _owned.Add(lightObject);
        }

        private void CreateGround()
        {
            GameObject ground = CreatePrimitive(PrimitiveType.Plane, "FsmGround", Vector3.zero);
            ground.transform.localScale = new Vector3(2f, 1f, 2f);
            SetColor(ground.GetComponent<Renderer>(), new Color(0.2f, 0.2f, 0.23f));
        }

        private GameObject CreatePrimitive(PrimitiveType type, string name, Vector3 position)
        {
            GameObject go = GameObject.CreatePrimitive(type);
            go.name = name;
            go.transform.position = position;

            Collider collider = go.GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = false;
            }

            _owned.Add(go);
            return go;
        }

        private static void SetColor(Renderer renderer, Color color)
        {
            Material material = renderer.material;
            if (material.HasProperty("_BaseColor"))
            {
                material.SetColor("_BaseColor", color);
            }

            if (material.HasProperty("_Color"))
            {
                material.SetColor("_Color", color);
            }

            material.color = color;
        }

        private void OnDestroy()
        {
            if (_machine != null)
            {
                _machine.OnStateChanged -= HandleStateChanged;
            }

            for (int i = 0; i < _owned.Count; i++)
            {
                if (_owned[i] != null)
                {
                    Destroy(_owned[i]);
                }
            }

            _owned.Clear();
        }
    }
}
