using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using CatAnnaDev.Tweening;

namespace CatAnnaDev.UI
{
    [AddComponentMenu("CatAnnaDev/UI/Cat Button")]
    public class CatButton : MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerDownHandler,
        IPointerUpHandler,
        IPointerClickHandler,
        ISubmitHandler
    {
        private enum VisualState { Normal, Hover, Pressed, Disabled }

        [Header("Target")]
        [SerializeField] private Graphic targetGraphic;

        [Header("Interaction")]
        [SerializeField] private bool interactable = true;

        [Header("Scale")]
        [SerializeField] private bool animateScale = true;
        [SerializeField] private float hoverScale = 1.06f;
        [SerializeField] private float pressScale = 0.94f;

        [Header("Tint")]
        [SerializeField] private bool animateTint = true;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color hoverColor = new Color(0.9f, 0.9f, 0.9f, 1f);
        [SerializeField] private Color pressedColor = new Color(0.75f, 0.75f, 0.75f, 1f);
        [SerializeField] private Color disabledColor = new Color(0.7f, 0.7f, 0.7f, 0.5f);

        [Header("Motion")]
        [SerializeField] private float duration = 0.12f;
        [SerializeField] private Ease ease = Ease.OutQuad;

        [Header("Click")]
        [SerializeField] private UnityEvent onClick = new UnityEvent();

        public event Action Clicked;

        private Vector3 baseScale = Vector3.one;
        private bool isHovered;
        private bool isPressed;

        public bool Interactable
        {
            get => interactable;
            set => SetInteractable(value);
        }

        public Graphic TargetGraphic => targetGraphic;
        public UnityEvent OnClickEvent => onClick;

        private void Reset()
        {
            targetGraphic = GetComponent<Graphic>();
            if (targetGraphic != null) normalColor = targetGraphic.color;
        }

        private void Awake()
        {
            if (targetGraphic == null) targetGraphic = GetComponent<Graphic>();
            baseScale = transform.localScale;
            ApplyState(interactable ? VisualState.Normal : VisualState.Disabled, instant: true);
        }

        private void OnDisable()
        {
            isHovered = false;
            isPressed = false;
        }

        public CatButton OnClick(Action callback)
        {
            if (callback != null) Clicked += callback;
            return this;
        }

        public CatButton SetInteractable(bool value)
        {
            if (interactable == value) return this;
            interactable = value;
            if (!interactable)
            {
                isHovered = false;
                isPressed = false;
            }
            RefreshState();
            return this;
        }

        public CatButton WithTarget(Graphic graphic)
        {
            targetGraphic = graphic;
            if (targetGraphic != null && !Application.isPlaying) normalColor = targetGraphic.color;
            return this;
        }

        public CatButton WithHoverScale(float scale)
        {
            hoverScale = scale;
            animateScale = true;
            return this;
        }

        public CatButton WithPressScale(float scale)
        {
            pressScale = scale;
            animateScale = true;
            return this;
        }

        public CatButton WithColors(Color normal, Color hover, Color pressed, Color disabled)
        {
            normalColor = normal;
            hoverColor = hover;
            pressedColor = pressed;
            disabledColor = disabled;
            animateTint = true;
            RefreshState();
            return this;
        }

        public CatButton WithMotion(float animDuration, Ease animEase)
        {
            duration = animDuration;
            ease = animEase;
            return this;
        }

        public void Click()
        {
            if (!interactable) return;
            onClick.Invoke();
            Clicked?.Invoke();
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData e)
        {
            isHovered = true;
            RefreshState();
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData e)
        {
            isHovered = false;
            RefreshState();
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData e)
        {
            if (!interactable) return;
            isPressed = true;
            RefreshState();
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData e)
        {
            isPressed = false;
            RefreshState();
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData e)
        {
            if (e.button != PointerEventData.InputButton.Left) return;
            Click();
        }

        void ISubmitHandler.OnSubmit(BaseEventData e) => Click();

        private void RefreshState()
        {
            if (!interactable)
            {
                ApplyState(VisualState.Disabled);
                return;
            }

            if (isPressed) ApplyState(VisualState.Pressed);
            else if (isHovered) ApplyState(VisualState.Hover);
            else ApplyState(VisualState.Normal);
        }

        private void ApplyState(VisualState state, bool instant = false)
        {
            float scaleFactor = state switch
            {
                VisualState.Hover => hoverScale,
                VisualState.Pressed => pressScale,
                _ => 1f
            };

            Color color = state switch
            {
                VisualState.Hover => hoverColor,
                VisualState.Pressed => pressedColor,
                VisualState.Disabled => disabledColor,
                _ => normalColor
            };

            if (animateScale)
            {
                transform.KillTweens();
                Vector3 targetScale = baseScale * scaleFactor;
                if (instant || duration <= 0f) transform.localScale = targetScale;
                else transform.TweenLocalScale(targetScale, duration).SetEase(ease);
            }

            if (animateTint && targetGraphic != null)
            {
                targetGraphic.KillTweens();
                if (instant || duration <= 0f) targetGraphic.color = color;
                else targetGraphic.TweenColor(color, duration).SetEase(ease);
            }
        }
    }
}
