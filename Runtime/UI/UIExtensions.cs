using UnityEngine;
using UnityEngine.UI;
using CatAnnaDev.Tweening;

namespace CatAnnaDev.UI
{
    public static class UIExtensions
    {
        public static TweenHandle TweenColor(this Graphic graphic, Color to, float duration)
        {
            Graphic g = graphic;
            return TweenManager.EnsureInstance().CreateColor(g, g.color, to, duration, value =>
            {
                if (g) g.color = value;
            });
        }

        public static TweenHandle TweenFade(this Graphic graphic, float to, float duration)
        {
            Graphic g = graphic;
            Color from = g.color;
            Color target = new Color(from.r, from.g, from.b, to);
            return TweenManager.EnsureInstance().CreateColor(g, from, target, duration, value =>
            {
                if (g) g.color = value;
            });
        }

        public static TweenHandle TweenAnchoredPosition(this RectTransform rect, Vector2 to, float duration)
        {
            RectTransform r = rect;
            Vector3 from = r.anchoredPosition;
            return TweenManager.EnsureInstance().CreateVector3(r, from, to, duration, value =>
            {
                if (r) r.anchoredPosition = value;
            });
        }

        public static TweenHandle TweenSizeDelta(this RectTransform rect, Vector2 to, float duration)
        {
            RectTransform r = rect;
            Vector3 from = r.sizeDelta;
            return TweenManager.EnsureInstance().CreateVector3(r, from, to, duration, value =>
            {
                if (r) r.sizeDelta = value;
            });
        }
    }
}
