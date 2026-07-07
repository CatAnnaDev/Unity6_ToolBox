using UnityEngine;
using CatAnnaDev.Tweening;

namespace CatAnnaDev.Samples
{
    public sealed class TweeningDemo : MonoBehaviour
    {
        static readonly Ease[] EaseCycle =
        {
            Ease.Linear,
            Ease.OutQuad,
            Ease.InOutCubic,
            Ease.OutBack,
            Ease.OutElastic,
            Ease.OutBounce
        };

        static readonly LoopType[] LoopCycle =
        {
            LoopType.PingPong,
            LoopType.Restart,
            LoopType.Incremental,
            LoopType.None
        };

        GameObject cube;
        Transform cubeTransform;
        Material cubeMaterial;
        Camera demoCamera;
        Light demoLight;

        Vector3 basePosition;
        Vector3 baseScale;
        Quaternion baseRotation;
        Color baseColor;

        int easeIndex;
        int loopIndex;
        int loopCount = -1;
        float duration = 1.25f;
        string lastAction = "none yet";
        int completedTweens;

        Rect windowRect = new Rect(12f, 12f, 360f, 470f);
        GUIStyle titleStyle;
        GUIStyle bodyStyle;
        GUIStyle boxStyle;

        void Start()
        {
            EnsureSceneRig();

            cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = "CatAnnaDev.TweenTarget";
            cubeTransform = cube.transform;
            cubeTransform.position = new Vector3(2.2f, 0f, 0f);

            baseColor = new Color(0.30f, 0.65f, 1f, 1f);
            cubeMaterial = BuildMaterial(baseColor);
            cube.GetComponent<Renderer>().sharedMaterial = cubeMaterial;

            basePosition = cubeTransform.position;
            baseScale = cubeTransform.localScale;
            baseRotation = cubeTransform.rotation;
        }

        void EnsureSceneRig()
        {
            if (Camera.main == null)
            {
                var camGo = new GameObject("CatAnnaDev.DemoCamera");
                demoCamera = camGo.AddComponent<Camera>();
                camGo.tag = "MainCamera";
                camGo.transform.position = new Vector3(2.2f, 1.6f, -7f);
                camGo.transform.LookAt(new Vector3(2.2f, 0f, 0f));
                demoCamera.clearFlags = CameraClearFlags.SolidColor;
                demoCamera.backgroundColor = new Color(0.08f, 0.09f, 0.12f, 1f);
            }

            if (FindAnyLight() == null)
            {
                var lightGo = new GameObject("CatAnnaDev.DemoLight");
                demoLight = lightGo.AddComponent<Light>();
                demoLight.type = LightType.Directional;
                demoLight.intensity = 1.1f;
                lightGo.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            }
        }

        static Light FindAnyLight()
        {
#if UNITY_2023_1_OR_NEWER
            return Object.FindFirstObjectByType<Light>();
#else
            return Object.FindObjectOfType<Light>();
#endif
        }

        static Material BuildMaterial(Color color)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            if (shader == null) shader = Shader.Find("Sprites/Default");
            var mat = new Material(shader);
            mat.color = color;
            return mat;
        }

        Ease CurrentEase => EaseCycle[easeIndex];
        LoopType CurrentLoop => LoopCycle[loopIndex];

        void PlayPosition()
        {
            cubeTransform.KillTweens();
            cubeTransform.position = basePosition;
            Vector3 target = basePosition + new Vector3(0f, 2.4f, 0f);
            cubeTransform.TweenPosition(target, duration)
                .SetEase(CurrentEase)
                .SetLoops(loopCount, CurrentLoop)
                .OnComplete(() => completedTweens++);
            lastAction = "Position -> up " + Describe();
        }

        void PlayScale()
        {
            cubeTransform.KillTweens();
            cubeTransform.localScale = baseScale;
            cubeTransform.TweenLocalScale(baseScale * 1.8f, duration)
                .SetEase(CurrentEase)
                .SetLoops(loopCount, CurrentLoop)
                .OnComplete(() => completedTweens++);
            lastAction = "Scale -> 1.8x " + Describe();
        }

        void PlayRotation()
        {
            cubeTransform.KillTweens();
            cubeTransform.rotation = baseRotation;
            cubeTransform.TweenRotation(new Vector3(0f, 180f, 0f), duration)
                .SetEase(CurrentEase)
                .SetLoops(loopCount, CurrentLoop)
                .OnComplete(() => completedTweens++);
            lastAction = "Rotation -> 180deg " + Describe();
        }

        void PlayColor()
        {
            TweenManager.KillTweensOf(cubeMaterial);
            cubeMaterial.color = baseColor;
            cubeMaterial.TweenColor(new Color(1f, 0.35f, 0.55f, 1f), duration)
                .SetEase(CurrentEase)
                .SetLoops(loopCount, CurrentLoop)
                .OnComplete(() => completedTweens++);
            lastAction = "Color -> pink " + Describe();
        }

        void PlayCombined()
        {
            ResetCube();
            cubeTransform.TweenPosition(basePosition + new Vector3(0f, 2.4f, 0f), duration)
                .SetEase(CurrentEase)
                .SetLoops(loopCount, LoopType.PingPong);
            cubeTransform.TweenLocalScale(baseScale * 1.5f, duration)
                .SetEase(CurrentEase)
                .SetLoops(loopCount, LoopType.PingPong);
            cubeTransform.TweenRotation(new Vector3(0f, 360f, 0f), duration)
                .SetEase(Ease.Linear)
                .SetLoops(loopCount, LoopType.Restart);
            cubeMaterial.TweenColor(new Color(1f, 0.85f, 0.2f, 1f), duration)
                .SetEase(CurrentEase)
                .SetLoops(loopCount, LoopType.PingPong);
            lastAction = "Combined position+scale+rotation+color " + Describe();
        }

        void OneShotViaTweener()
        {
            Tweener.To(0f, 1f, duration, t =>
            {
                if (cubeTransform)
                    cubeTransform.position = basePosition + new Vector3(Mathf.Sin(t * Mathf.PI * 2f) * 1.5f, 0f, 0f);
            }).SetEase(CurrentEase);
            lastAction = "Tweener.To float sweep " + Describe();
        }

        void ResetCube()
        {
            cubeTransform.KillTweens();
            TweenManager.KillTweensOf(cubeMaterial);
            cubeTransform.position = basePosition;
            cubeTransform.localScale = baseScale;
            cubeTransform.rotation = baseRotation;
            cubeMaterial.color = baseColor;
            lastAction = "reset";
        }

        string Describe()
        {
            string loops = loopCount < 0 ? "infinite" : loopCount.ToString();
            return "(ease=" + CurrentEase + ", loop=" + CurrentLoop + ", loops=" + loops + ")";
        }

        void Update()
        {
            if (DemoInput.GetKeyDown(KeyCode.Alpha1)) PlayPosition();
            if (DemoInput.GetKeyDown(KeyCode.Alpha2)) PlayScale();
            if (DemoInput.GetKeyDown(KeyCode.Alpha3)) PlayRotation();
            if (DemoInput.GetKeyDown(KeyCode.Alpha4)) PlayColor();
            if (DemoInput.GetKeyDown(KeyCode.Alpha5)) PlayCombined();
            if (DemoInput.GetKeyDown(KeyCode.Alpha6)) OneShotViaTweener();
            if (DemoInput.GetKeyDown(KeyCode.E)) easeIndex = (easeIndex + 1) % EaseCycle.Length;
            if (DemoInput.GetKeyDown(KeyCode.L)) loopIndex = (loopIndex + 1) % LoopCycle.Length;
            if (DemoInput.GetKeyDown(KeyCode.I)) loopCount = loopCount < 0 ? 2 : -1;
            if (DemoInput.GetKeyDown(KeyCode.P)) TweenManager.PauseAll();
            if (DemoInput.GetKeyDown(KeyCode.R)) TweenManager.ResumeAll();
            if (DemoInput.GetKeyDown(KeyCode.K)) TweenManager.KillAll();
            if (DemoInput.GetKeyDown(KeyCode.Backspace)) ResetCube();
        }

        void OnGUI()
        {
            EnsureStyles();
            windowRect = GUILayout.Window(GetEntityId().GetHashCode(), windowRect, DrawWindow, "CatAnnaDev Tweening", boxStyle);
        }

        void DrawWindow(int id)
        {
            GUILayout.Label("Fluent tweens on one cube", titleStyle);
            GUILayout.Label(
                "Tween Transform position, scale and rotation, plus Material color, " +
                "through the fluent extension API. Chain SetEase / SetLoops to shape motion.",
                bodyStyle);

            GUILayout.Space(6f);
            GUILayout.Label("Keys: 1 pos  2 scale  3 rot  4 color  5 combined  6 Tweener.To", bodyStyle);
            GUILayout.Label("E cycle ease   L cycle loop   I toggle loop count", bodyStyle);
            GUILayout.Label("P pause all   R resume all   K kill all   Backspace reset", bodyStyle);

            GUILayout.Space(6f);
            GUILayout.Label("Ease:  " + CurrentEase, bodyStyle);
            GUILayout.Label("Loop:  " + CurrentLoop + "   count: " + (loopCount < 0 ? "infinite" : loopCount.ToString()), bodyStyle);
            GUILayout.Label("Duration: " + duration.ToString("0.00") + "s", bodyStyle);
            duration = GUILayout.HorizontalSlider(duration, 0.25f, 3f);

            GUILayout.Space(6f);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Position")) PlayPosition();
            if (GUILayout.Button("Scale")) PlayScale();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Rotation")) PlayRotation();
            if (GUILayout.Button("Color")) PlayColor();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Combined")) PlayCombined();
            if (GUILayout.Button("Tweener.To")) OneShotViaTweener();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Next Ease")) easeIndex = (easeIndex + 1) % EaseCycle.Length;
            if (GUILayout.Button("Next Loop")) loopIndex = (loopIndex + 1) % LoopCycle.Length;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Kill All")) TweenManager.KillAll();
            if (GUILayout.Button("Reset")) ResetCube();
            GUILayout.EndHorizontal();

            GUILayout.Space(6f);
            GUILayout.Label("Active tweens: " + TweenManager.ActiveCount, bodyStyle);
            GUILayout.Label("Completed callbacks: " + completedTweens, bodyStyle);
            GUILayout.Label("Last action: " + lastAction, bodyStyle);

            GUI.DragWindow(new Rect(0f, 0f, 10000f, 24f));
        }

        void EnsureStyles()
        {
            if (boxStyle == null)
            {
                boxStyle = new GUIStyle(GUI.skin.window) { fontSize = 13 };
            }
            if (titleStyle == null)
            {
                titleStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 15,
                    fontStyle = FontStyle.Bold,
                    normal = { textColor = new Color(0.75f, 0.9f, 1f) }
                };
            }
            if (bodyStyle == null)
            {
                bodyStyle = new GUIStyle(GUI.skin.label) { fontSize = 12, wordWrap = true };
            }
        }

        void OnDisable()
        {
            if (TweenManager.HasInstance)
            {
                if (cubeTransform) cubeTransform.KillTweens();
                if (cubeMaterial) TweenManager.KillTweensOf(cubeMaterial);
            }
        }

        void OnDestroy()
        {
            if (cube) Destroy(cube);
            if (cubeMaterial) Destroy(cubeMaterial);
            if (demoCamera) Destroy(demoCamera.gameObject);
            if (demoLight) Destroy(demoLight.gameObject);
        }
    }
}
