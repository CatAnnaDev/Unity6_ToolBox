using UnityEngine;
using CatAnnaDev.Timers;

namespace CatAnnaDev.Samples
{
    public sealed class TimersDemo : MonoBehaviour
    {
        private const float CountdownDuration = 5f;
        private const float StopwatchTarget = 8f;
        private const float DefaultTicksPerSecond = 4f;

        private CountdownTimer countdown;
        private StopwatchTimer stopwatch;
        private FrequencyTimer frequency;

        private int countdownCompletions;
        private int targetsReached;
        private float lastLap;
        private float pulseValue;

        private GUIStyle titleStyle;
        private GUIStyle bodyStyle;
        private GUIStyle boxStyle;
        private Texture2D barBackground;
        private Texture2D barFill;
        private bool stylesReady;

        private void OnEnable()
        {
            countdown = new CountdownTimer(CountdownDuration)
            {
                Loop = true
            };
            countdown.OnComplete += HandleCountdownComplete;
            countdown.Start();

            stopwatch = new StopwatchTimer(StopwatchTarget);
            stopwatch.OnTargetReached += HandleTargetReached;
            stopwatch.Start();

            frequency = new FrequencyTimer(DefaultTicksPerSecond);
            frequency.OnTick += HandleFrequencyTick;
            frequency.Start();
        }

        private void OnDisable()
        {
            if (countdown != null)
            {
                countdown.OnComplete -= HandleCountdownComplete;
                countdown.Stop();
                countdown = null;
            }

            if (stopwatch != null)
            {
                stopwatch.OnTargetReached -= HandleTargetReached;
                stopwatch.Stop();
                stopwatch = null;
            }

            if (frequency != null)
            {
                frequency.OnTick -= HandleFrequencyTick;
                frequency.Stop();
                frequency = null;
            }

            if (barBackground != null)
            {
                Destroy(barBackground);
                barBackground = null;
            }

            if (barFill != null)
            {
                Destroy(barFill);
                barFill = null;
            }
        }

        private void HandleCountdownComplete()
        {
            countdownCompletions++;
        }

        private void HandleTargetReached()
        {
            targetsReached++;
        }

        private void HandleFrequencyTick()
        {
            pulseValue = pulseValue > 0.5f ? 0f : 1f;
        }

        private void EnsureStyles()
        {
            if (stylesReady)
            {
                return;
            }

            barBackground = MakeTexture(new Color(0.15f, 0.15f, 0.18f, 1f));
            barFill = MakeTexture(new Color(0.30f, 0.75f, 0.95f, 1f));

            Texture2D panel = MakeTexture(new Color(0f, 0f, 0f, 0.78f));

            titleStyle = new GUIStyle
            {
                fontSize = 20,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white }
            };

            bodyStyle = new GUIStyle
            {
                fontSize = 13,
                richText = true,
                wordWrap = true,
                normal = { textColor = new Color(0.88f, 0.88f, 0.90f, 1f) }
            };

            boxStyle = new GUIStyle
            {
                padding = new RectOffset(14, 14, 12, 14),
                normal = { background = panel }
            };

            stylesReady = true;
        }

        private static Texture2D MakeTexture(Color color)
        {
            Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false)
            {
                hideFlags = HideFlags.HideAndDontSave
            };
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }

        private void OnGUI()
        {
            EnsureStyles();

            GUILayout.BeginArea(new Rect(12f, 12f, 460f, 640f), boxStyle);

            GUILayout.Label("CatAnnaDev Timers", titleStyle);
            GUILayout.Space(4f);
            GUILayout.Label(
                "Frame-driven timers running on the PlayerLoop. No coroutines, no " +
                "MonoBehaviour Update needed. Countdown fires OnComplete, Stopwatch " +
                "measures elapsed time and Lap, Frequency fires OnTick N times/sec.",
                bodyStyle);

            GUILayout.Space(8f);
            GUILayout.Label("<b>Controls</b>", bodyStyle);
            GUILayout.Label(
                "Space  toggle all    R  reset all\n" +
                "1  countdown +/- loop    2  stopwatch Lap\n" +
                "[  slower ticks    ]  faster ticks",
                bodyStyle);

            GUILayout.Space(10f);
            DrawCountdown();
            GUILayout.Space(10f);
            DrawStopwatch();
            GUILayout.Space(10f);
            DrawFrequency();

            GUILayout.Space(10f);
            GUILayout.Label("Active timers on manager: " + TimerManager.ActiveCount, bodyStyle);

            GUILayout.EndArea();

            HandleKeys();
        }

        private void DrawCountdown()
        {
            GUILayout.Label("<b>CountdownTimer</b>  (loop = " + countdown.Loop + ")", bodyStyle);
            GUILayout.Label(
                "Remaining " + countdown.Remaining.ToString("0.00") + "s   " +
                "state " + countdown.State + "   completions " + countdownCompletions,
                bodyStyle);
            DrawBar(countdown.Progress);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Start")) { countdown.Start(); }
            if (GUILayout.Button("Pause")) { countdown.Pause(); }
            if (GUILayout.Button("Resume")) { countdown.Resume(); }
            if (GUILayout.Button("Reset")) { countdown.Reset(); }
            if (GUILayout.Button("Loop")) { countdown.Loop = !countdown.Loop; }
            GUILayout.EndHorizontal();
        }

        private void DrawStopwatch()
        {
            GUILayout.Label("<b>StopwatchTimer</b>  (target " + StopwatchTarget.ToString("0") + "s)", bodyStyle);
            GUILayout.Label(
                "Elapsed " + stopwatch.Elapsed.ToString("0.00") + "s   " +
                "state " + stopwatch.State + "   last lap " + lastLap.ToString("0.00") + "s   " +
                "reached " + targetsReached,
                bodyStyle);
            DrawBar(stopwatch.Progress);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Start")) { stopwatch.Start(); }
            if (GUILayout.Button("Pause")) { stopwatch.Pause(); }
            if (GUILayout.Button("Resume")) { stopwatch.Resume(); }
            if (GUILayout.Button("Lap")) { lastLap = stopwatch.Lap(); }
            if (GUILayout.Button("Reset")) { stopwatch.Reset(); }
            GUILayout.EndHorizontal();
        }

        private void DrawFrequency()
        {
            GUILayout.Label(
                "<b>FrequencyTimer</b>  (" + frequency.TicksPerSecond.ToString("0.0") +
                " ticks/s, interval " + frequency.Interval.ToString("0.000") + "s)",
                bodyStyle);
            GUILayout.Label(
                "Ticks " + frequency.TickCount + "   state " + frequency.State +
                "   pulse " + (pulseValue > 0.5f ? "ON" : "off"),
                bodyStyle);
            DrawBar(frequency.Progress);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Start")) { frequency.Start(); }
            if (GUILayout.Button("Pause")) { frequency.Pause(); }
            if (GUILayout.Button("Resume")) { frequency.Resume(); }
            if (GUILayout.Button("Slower")) { frequency.SetFrequency(Mathf.Max(1f, frequency.TicksPerSecond - 1f)); }
            if (GUILayout.Button("Faster")) { frequency.SetFrequency(frequency.TicksPerSecond + 1f); }
            GUILayout.EndHorizontal();
        }

        private void DrawBar(float progress)
        {
            Rect rect = GUILayoutUtility.GetRect(430f, 16f);
            GUI.DrawTexture(rect, barBackground);
            Rect fill = new Rect(rect.x, rect.y, rect.width * Mathf.Clamp01(progress), rect.height);
            GUI.DrawTexture(fill, barFill);
        }

        private void HandleKeys()
        {
            Event e = Event.current;
            if (e == null || e.type != EventType.KeyDown)
            {
                return;
            }

            switch (e.keyCode)
            {
                case KeyCode.Space:
                    countdown.Toggle();
                    stopwatch.Toggle();
                    frequency.Toggle();
                    break;
                case KeyCode.R:
                    countdown.Restart();
                    stopwatch.Restart();
                    frequency.Restart();
                    countdownCompletions = 0;
                    targetsReached = 0;
                    lastLap = 0f;
                    break;
                case KeyCode.Alpha1:
                    countdown.Loop = !countdown.Loop;
                    break;
                case KeyCode.Alpha2:
                    lastLap = stopwatch.Lap();
                    break;
                case KeyCode.LeftBracket:
                    frequency.SetFrequency(Mathf.Max(1f, frequency.TicksPerSecond - 1f));
                    break;
                case KeyCode.RightBracket:
                    frequency.SetFrequency(frequency.TicksPerSecond + 1f);
                    break;
            }
        }
    }
}
