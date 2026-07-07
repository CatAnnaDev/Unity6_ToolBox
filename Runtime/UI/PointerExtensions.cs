using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CatAnnaDev.UI
{
    public static class PointerExtensions
    {
        public static PointerEvents Pointer(this GameObject go)
        {
            PointerEvents events = go.GetComponent<PointerEvents>();
            return events != null ? events : go.AddComponent<PointerEvents>();
        }

        public static PointerEvents Pointer(this Component component) => component.gameObject.Pointer();

        public static PointerEvents OnClick(this GameObject go, Action callback) => go.Pointer().OnClick(callback);
        public static PointerEvents OnClick(this GameObject go, Action<PointerEventData> callback) => go.Pointer().OnClick(callback);
        public static PointerEvents OnClick(this Component c, Action callback) => c.Pointer().OnClick(callback);

        public static PointerEvents OnHover(this GameObject go, Action enter, Action exit = null)
            => go.Pointer().OnPointerEnter(enter).OnPointerExit(exit);
        public static PointerEvents OnHover(this Component c, Action enter, Action exit = null)
            => c.Pointer().OnPointerEnter(enter).OnPointerExit(exit);

        public static PointerEvents OnPointerEnter(this GameObject go, Action callback) => go.Pointer().OnPointerEnter(callback);
        public static PointerEvents OnPointerExit(this GameObject go, Action callback) => go.Pointer().OnPointerExit(callback);
        public static PointerEvents OnPointerDown(this GameObject go, Action callback) => go.Pointer().OnPointerDown(callback);
        public static PointerEvents OnPointerUp(this GameObject go, Action callback) => go.Pointer().OnPointerUp(callback);

        public static PointerEvents OnDrag(this GameObject go, Action<PointerEventData> callback) => go.Pointer().OnDrag(callback);
        public static PointerEvents OnBeginDrag(this GameObject go, Action<PointerEventData> callback) => go.Pointer().OnBeginDrag(callback);
        public static PointerEvents OnEndDrag(this GameObject go, Action<PointerEventData> callback) => go.Pointer().OnEndDrag(callback);

        public static CatButton AsButton(this GameObject go)
        {
            CatButton button = go.GetComponent<CatButton>();
            return button != null ? button : go.AddComponent<CatButton>();
        }

        public static CatButton AsButton(this Component component) => component.gameObject.AsButton();
    }
}
