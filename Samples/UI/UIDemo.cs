using System;
using CatAnnaDev.Tweening;
using CatAnnaDev.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CatAnnaDev.Samples
{
    public sealed class UIDemo : MonoBehaviour
    {
        private Text status;
        private int clicks;

        private void Start()
        {
            EnsureEventSystem();

            Canvas canvas = BuildCanvas();
            status = CreateLabel(
                canvas.transform,
                "Hover and click the buttons",
                new Vector2(0f, 200f),
                260f
            );

            MakeButton(
                    canvas.transform,
                    "Play",
                    new Vector2(0f, 90f),
                    new Color(0.20f, 0.55f, 0.95f)
                )
                .WithHoverScale(1.08f)
                .WithPressScale(0.92f)
                .WithMotion(0.14f, Ease.OutBack)
                .OnClick(() => Report("Play clicked"));

            MakeButton(
                    canvas.transform,
                    "Options",
                    new Vector2(0f, 10f),
                    new Color(0.35f, 0.35f, 0.42f)
                )
                .WithColors(
                    new Color(0.35f, 0.35f, 0.42f),
                    new Color(0.45f, 0.45f, 0.55f),
                    new Color(0.25f, 0.25f, 0.32f),
                    new Color(0.3f, 0.3f, 0.3f, 0.5f)
                )
                .OnClick(() => Report("Options clicked"));

            CatButton locked = MakeButton(
                    canvas.transform,
                    "Locked",
                    new Vector2(0f, -70f),
                    new Color(0.6f, 0.2f, 0.2f)
                )
                .OnClick(() => Report("This should never fire"));
            locked.SetInteractable(false);

            Image raw = CreatePanel(
                canvas.transform,
                new Vector2(0f, -160f),
                new Vector2(220f, 60f),
                new Color(0.15f, 0.5f, 0.3f)
            );
            CreateLabel(raw.transform, "Raw pointer API", Vector2.zero, 220f);
            raw.gameObject.OnHover(
                    () =>
                        raw
                            .rectTransform.TweenSizeDelta(new Vector2(240f, 66f), 0.12f)
                            .SetEase(Ease.OutQuad),
                    () =>
                        raw
                            .rectTransform.TweenSizeDelta(new Vector2(220f, 60f), 0.12f)
                            .SetEase(Ease.OutQuad)
                )
                .OnClick(() => Report("Raw panel clicked"));
        }

        private CatButton MakeButton(
            Transform parent,
            string label,
            Vector2 anchoredPos,
            Color color
        )
        {
            Image image = CreatePanel(parent, anchoredPos, new Vector2(220f, 64f), color);
            CreateLabel(image.transform, label, Vector2.zero, 220f);
            return image.AsButton().WithTarget(image);
        }

        private void Report(string message)
        {
            clicks++;
            if (status != null)
                status.text = message + "   (" + clicks + " clicks)";
        }

        private static Canvas BuildCanvas()
        {
            GameObject go = new GameObject("UIDemo Canvas");
            Canvas canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = go.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            go.AddComponent<GraphicRaycaster>();
            return canvas;
        }

        private static Image CreatePanel(
            Transform parent,
            Vector2 anchoredPos,
            Vector2 size,
            Color color
        )
        {
            GameObject go = new GameObject("Panel");
            go.transform.SetParent(parent, false);
            Image image = go.AddComponent<Image>();
            image.color = color;
            RectTransform rect = image.rectTransform;
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;
            rect.anchoredPosition = anchoredPos;
            return image;
        }

        private static Text CreateLabel(
            Transform parent,
            string content,
            Vector2 anchoredPos,
            float width
        )
        {
            GameObject go = new GameObject("Label");
            go.transform.SetParent(parent, false);
            Text text = go.AddComponent<Text>();
            text.text = content;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
            text.fontSize = 24;
            text.raycastTarget = false;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (text.font == null)
                text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            RectTransform rect = text.rectTransform;
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(width, 60f);
            rect.anchoredPosition = anchoredPos;
            return text;
        }

        private static void EnsureEventSystem()
        {
            if (EventSystem.current != null)
                return;

            GameObject go = new GameObject("EventSystem");
            go.AddComponent<EventSystem>();

            Type inputSystemModule = Type.GetType(
                "UnityEngine.InputSystem.UI.InputSystemUIInputModule, " + "Unity.InputSystem"
            );
            if (inputSystemModule != null)
                go.AddComponent(inputSystemModule);
            else
                go.AddComponent<StandaloneInputModule>();
        }
    }
}
