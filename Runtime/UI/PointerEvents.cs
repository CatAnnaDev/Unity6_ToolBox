using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace CatAnnaDev.UI
{
    [AddComponentMenu("CatAnnaDev/UI/Pointer Events")]
    public class PointerEvents : MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerDownHandler,
        IPointerUpHandler,
        IPointerClickHandler,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler,
        IScrollHandler,
        ISelectHandler,
        IDeselectHandler,
        ISubmitHandler
    {
        [Serializable] public class PointerUnityEvent : UnityEvent<PointerEventData> { }

        [SerializeField] private PointerUnityEvent onEnter = new PointerUnityEvent();
        [SerializeField] private PointerUnityEvent onExit = new PointerUnityEvent();
        [SerializeField] private PointerUnityEvent onDown = new PointerUnityEvent();
        [SerializeField] private PointerUnityEvent onUp = new PointerUnityEvent();
        [SerializeField] private PointerUnityEvent onClick = new PointerUnityEvent();

        public event Action<PointerEventData> PointerEnter;
        public event Action<PointerEventData> PointerExit;
        public event Action<PointerEventData> PointerDown;
        public event Action<PointerEventData> PointerUp;
        public event Action<PointerEventData> PointerClick;
        public event Action<PointerEventData> BeginDrag;
        public event Action<PointerEventData> Drag;
        public event Action<PointerEventData> EndDrag;
        public event Action<PointerEventData> Scroll;
        public event Action<BaseEventData> Select;
        public event Action<BaseEventData> Deselect;
        public event Action<BaseEventData> Submit;

        public bool IsHovered { get; private set; }
        public bool IsPressed { get; private set; }
        public bool IsDragging { get; private set; }

        public PointerUnityEvent OnEnterEvent => onEnter;
        public PointerUnityEvent OnExitEvent => onExit;
        public PointerUnityEvent OnDownEvent => onDown;
        public PointerUnityEvent OnUpEvent => onUp;
        public PointerUnityEvent OnClickEvent => onClick;

        public PointerEvents OnPointerEnter(Action callback) => Add(ref PointerEnter, callback);
        public PointerEvents OnPointerEnter(Action<PointerEventData> callback) => Add(ref PointerEnter, callback);
        public PointerEvents OnPointerExit(Action callback) => Add(ref PointerExit, callback);
        public PointerEvents OnPointerExit(Action<PointerEventData> callback) => Add(ref PointerExit, callback);
        public PointerEvents OnPointerDown(Action callback) => Add(ref PointerDown, callback);
        public PointerEvents OnPointerDown(Action<PointerEventData> callback) => Add(ref PointerDown, callback);
        public PointerEvents OnPointerUp(Action callback) => Add(ref PointerUp, callback);
        public PointerEvents OnPointerUp(Action<PointerEventData> callback) => Add(ref PointerUp, callback);
        public PointerEvents OnClick(Action callback) => Add(ref PointerClick, callback);
        public PointerEvents OnClick(Action<PointerEventData> callback) => Add(ref PointerClick, callback);
        public PointerEvents OnBeginDrag(Action<PointerEventData> callback) => Add(ref BeginDrag, callback);
        public PointerEvents OnDrag(Action<PointerEventData> callback) => Add(ref Drag, callback);
        public PointerEvents OnEndDrag(Action<PointerEventData> callback) => Add(ref EndDrag, callback);
        public PointerEvents OnScroll(Action<PointerEventData> callback) => Add(ref Scroll, callback);
        public PointerEvents OnSelect(Action callback) => Add(ref Select, callback);
        public PointerEvents OnDeselect(Action callback) => Add(ref Deselect, callback);
        public PointerEvents OnSubmit(Action callback) => Add(ref Submit, callback);

        void IPointerEnterHandler.OnPointerEnter(PointerEventData e)
        {
            IsHovered = true;
            onEnter.Invoke(e);
            PointerEnter?.Invoke(e);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData e)
        {
            IsHovered = false;
            onExit.Invoke(e);
            PointerExit?.Invoke(e);
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData e)
        {
            IsPressed = true;
            onDown.Invoke(e);
            PointerDown?.Invoke(e);
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData e)
        {
            IsPressed = false;
            onUp.Invoke(e);
            PointerUp?.Invoke(e);
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData e)
        {
            onClick.Invoke(e);
            PointerClick?.Invoke(e);
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData e)
        {
            IsDragging = true;
            BeginDrag?.Invoke(e);
        }

        void IDragHandler.OnDrag(PointerEventData e) => Drag?.Invoke(e);

        void IEndDragHandler.OnEndDrag(PointerEventData e)
        {
            IsDragging = false;
            EndDrag?.Invoke(e);
        }

        void IScrollHandler.OnScroll(PointerEventData e) => Scroll?.Invoke(e);

        void ISelectHandler.OnSelect(BaseEventData e) => Select?.Invoke(e);

        void IDeselectHandler.OnDeselect(BaseEventData e) => Deselect?.Invoke(e);

        void ISubmitHandler.OnSubmit(BaseEventData e) => Submit?.Invoke(e);

        private void OnDisable()
        {
            IsHovered = false;
            IsPressed = false;
            IsDragging = false;
        }

        private PointerEvents Add(ref Action<PointerEventData> target, Action callback)
        {
            if (callback != null) target += _ => callback();
            return this;
        }

        private PointerEvents Add(ref Action<PointerEventData> target, Action<PointerEventData> callback)
        {
            if (callback != null) target += callback;
            return this;
        }

        private PointerEvents Add(ref Action<BaseEventData> target, Action callback)
        {
            if (callback != null) target += _ => callback();
            return this;
        }
    }
}
