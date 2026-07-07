using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using CatAnnaDev.UI;

namespace CatAnnaDev.Samples
{
    public sealed class UIControlsDemo : MonoBehaviour
    {
        private Text status;
        private int clicks;

        private void Start()
        {
            EnsureEventSystem();
            Canvas canvas = BuildCanvas();

            status = CreateLabel(canvas.transform, "Interact with the controls", new Vector2(0f, 170f), 360f, 26);

            BuildButton(canvas.transform, "Click me", new Vector2(0f, 90f))
                .OnClick(() => Report("button clicked"));

            BuildToggle(canvas.transform, "Enable feature", new Vector2(0f, 20f))
                .OnToggled(on => Report("toggle = " + on));

            BuildSlider(canvas.transform, new Vector2(0f, -60f))
                .OnChanged(v => Report("slider = " + v.ToString("0.00")));
        }

        private void Report(string message)
        {
            clicks++;
            if (status != null) status.text = message + "   (" + clicks + " events)";
        }

        private Button BuildButton(Transform parent, string label, Vector2 pos)
        {
            Image image = CreatePanel(parent, pos, new Vector2(220f, 56f), new Color(0.20f, 0.55f, 0.95f));
            CreateLabel(image.transform, label, Vector2.zero, 220f, 24);
            Button button = image.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            return button;
        }

        private Toggle BuildToggle(Transform parent, string label, Vector2 pos)
        {
            GameObject go = new GameObject("Toggle");
            go.transform.SetParent(parent, false);
            RectTransform rect = go.AddComponent<RectTransform>();
            Anchor(rect, new Vector2(240f, 40f), pos);

            Image box = CreatePanel(go.transform, new Vector2(-100f, 0f), new Vector2(32f, 32f), new Color(0.85f, 0.85f, 0.9f));
            Image check = CreatePanel(box.transform, Vector2.zero, new Vector2(20f, 20f), new Color(0.2f, 0.6f, 0.3f));
            CreateLabel(go.transform, label, new Vector2(30f, 0f), 200f, 22);

            Toggle toggle = go.AddComponent<Toggle>();
            toggle.targetGraphic = box;
            toggle.graphic = check;
            toggle.isOn = false;
            return toggle;
        }

        private Slider BuildSlider(Transform parent, Vector2 pos)
        {
            GameObject go = new GameObject("Slider");
            go.transform.SetParent(parent, false);
            RectTransform rect = go.AddComponent<RectTransform>();
            Anchor(rect, new Vector2(300f, 30f), pos);

            Image background = go.AddComponent<Image>();
            background.color = new Color(0.2f, 0.2f, 0.25f);

            GameObject fillArea = new GameObject("Fill Area");
            fillArea.transform.SetParent(go.transform, false);
            RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
            Stretch(fillAreaRect, new Vector2(5f, 5f), new Vector2(-5f, -5f));

            Image fill = CreatePanel(fillArea.transform, Vector2.zero, new Vector2(10f, 0f), new Color(0.2f, 0.55f, 0.95f));
            Stretch(fill.rectTransform, Vector2.zero, Vector2.zero);

            GameObject handleArea = new GameObject("Handle Slide Area");
            handleArea.transform.SetParent(go.transform, false);
            RectTransform handleAreaRect = handleArea.AddComponent<RectTransform>();
            Stretch(handleAreaRect, new Vector2(10f, 0f), new Vector2(-10f, 0f));

            Image handle = CreatePanel(handleArea.transform, Vector2.zero, new Vector2(24f, 24f), Color.white);

            Slider slider = go.AddComponent<Slider>();
            slider.fillRect = fill.rectTransform;
            slider.handleRect = handle.rectTransform;
            slider.targetGraphic = handle;
            slider.direction = Slider.Direction.LeftToRight;
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = 0.5f;
            return slider;
        }

        private static Canvas BuildCanvas()
        {
            GameObject go = new GameObject("UIControls Canvas");
            Canvas canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = go.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            go.AddComponent<GraphicRaycaster>();
            return canvas;
        }

        private static Image CreatePanel(Transform parent, Vector2 pos, Vector2 size, Color color)
        {
            GameObject go = new GameObject("Panel");
            go.transform.SetParent(parent, false);
            Image image = go.AddComponent<Image>();
            image.color = color;
            Anchor(image.rectTransform, size, pos);
            return image;
        }

        private static Text CreateLabel(Transform parent, string content, Vector2 pos, float width, int fontSize)
        {
            GameObject go = new GameObject("Label");
            go.transform.SetParent(parent, false);
            Text text = go.AddComponent<Text>();
            text.text = content;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
            text.fontSize = fontSize;
            text.raycastTarget = false;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (text.font == null) text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            Anchor(text.rectTransform, new Vector2(width, 44f), pos);
            return text;
        }

        private static void Anchor(RectTransform rect, Vector2 size, Vector2 pos)
        {
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;
            rect.anchoredPosition = pos;
        }

        private static void Stretch(RectTransform rect, Vector2 offsetMin, Vector2 offsetMax)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
        }

        private static void EnsureEventSystem()
        {
            if (EventSystem.current != null) return;
            GameObject go = new GameObject("EventSystem");
            go.AddComponent<EventSystem>();
            Type inputSystemModule = Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
            if (inputSystemModule != null) go.AddComponent(inputSystemModule);
            else go.AddComponent<StandaloneInputModule>();
        }
    }
}
