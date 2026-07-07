using UnityEngine;

namespace CatAnnaDev.Utils
{
    public sealed class FPSCounter : MonoBehaviour
    {
        [SerializeField]
        private float smoothing = 0.1f;

        [SerializeField]
        private bool drawOnGui = true;

        [SerializeField]
        private int fontSize = 24;

        [SerializeField]
        private Vector2 screenOffset = new Vector2(10f, 10f);

        private float smoothedDeltaTime;
        private GUIStyle style;

        public float CurrentFps
        {
            get { return smoothedDeltaTime > 0f ? 1f / smoothedDeltaTime : 0f; }
        }

        public float MillisecondsPerFrame
        {
            get { return smoothedDeltaTime * 1000f; }
        }

        private void Awake()
        {
            smoothedDeltaTime = Time.unscaledDeltaTime;
        }

        private void Update()
        {
            float delta = Time.unscaledDeltaTime;
            smoothedDeltaTime = Mathf.Lerp(smoothedDeltaTime, delta, smoothing <= 0f ? 1f : smoothing);
        }

        private void OnGUI()
        {
            if (!drawOnGui)
            {
                return;
            }
            if (style == null)
            {
                style = new GUIStyle(GUI.skin.label);
            }
            style.fontSize = fontSize;
            style.normal.textColor = ColorForFps(CurrentFps);

            string text = string.Format("{0:0.} FPS  ({1:0.0} ms)", CurrentFps, MillisecondsPerFrame);
            Rect rect = new Rect(screenOffset.x, screenOffset.y, 400f, fontSize + 8f);
            GUI.Label(rect, text, style);
        }

        private static Color ColorForFps(float fps)
        {
            if (fps >= 55f)
            {
                return Color.green;
            }
            if (fps >= 30f)
            {
                return Color.yellow;
            }
            return Color.red;
        }
    }
}
