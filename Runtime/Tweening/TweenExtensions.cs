using UnityEngine;

namespace CatAnnaDev.Tweening
{
    public static class TweenExtensions
    {
        public static TweenHandle TweenPosition(this Transform transform, Vector3 to, float duration)
        {
            Transform tr = transform;
            return TweenManager.EnsureInstance().CreateVector3(tr, tr.position, to, duration, value =>
            {
                if (tr) tr.position = value;
            });
        }

        public static TweenHandle TweenLocalPosition(this Transform transform, Vector3 to, float duration)
        {
            Transform tr = transform;
            return TweenManager.EnsureInstance().CreateVector3(tr, tr.localPosition, to, duration, value =>
            {
                if (tr) tr.localPosition = value;
            });
        }

        public static TweenHandle TweenLocalScale(this Transform transform, Vector3 to, float duration)
        {
            Transform tr = transform;
            return TweenManager.EnsureInstance().CreateVector3(tr, tr.localScale, to, duration, value =>
            {
                if (tr) tr.localScale = value;
            });
        }

        public static TweenHandle TweenRotation(this Transform transform, Quaternion to, float duration)
        {
            Transform tr = transform;
            return TweenManager.EnsureInstance().CreateQuaternion(tr, tr.rotation, to, duration, value =>
            {
                if (tr) tr.rotation = value;
            });
        }

        public static TweenHandle TweenLocalRotation(this Transform transform, Quaternion to, float duration)
        {
            Transform tr = transform;
            return TweenManager.EnsureInstance().CreateQuaternion(tr, tr.localRotation, to, duration, value =>
            {
                if (tr) tr.localRotation = value;
            });
        }

        public static TweenHandle TweenRotation(this Transform transform, Vector3 eulerTo, float duration)
            => transform.TweenRotation(Quaternion.Euler(eulerTo), duration);

        public static TweenHandle TweenAlpha(this CanvasGroup canvasGroup, float to, float duration)
        {
            CanvasGroup cg = canvasGroup;
            return TweenManager.EnsureInstance().CreateFloat(cg, cg.alpha, to, duration, value =>
            {
                if (cg) cg.alpha = value;
            });
        }

        public static TweenHandle TweenColor(this Material material, Color to, float duration)
        {
            Material mat = material;
            return TweenManager.EnsureInstance().CreateColor(mat, mat.color, to, duration, value =>
            {
                if (mat) mat.color = value;
            });
        }

        public static TweenHandle TweenColor(this Material material, int propertyId, Color to, float duration)
        {
            Material mat = material;
            int id = propertyId;
            return TweenManager.EnsureInstance().CreateColor(mat, mat.GetColor(id), to, duration, value =>
            {
                if (mat) mat.SetColor(id, value);
            });
        }

        public static TweenHandle TweenFloat(this Material material, int propertyId, float to, float duration)
        {
            Material mat = material;
            int id = propertyId;
            return TweenManager.EnsureInstance().CreateFloat(mat, mat.GetFloat(id), to, duration, value =>
            {
                if (mat) mat.SetFloat(id, value);
            });
        }

        public static TweenHandle TweenColor(this SpriteRenderer renderer, Color to, float duration)
        {
            SpriteRenderer sr = renderer;
            return TweenManager.EnsureInstance().CreateColor(sr, sr.color, to, duration, value =>
            {
                if (sr) sr.color = value;
            });
        }

        public static TweenHandle TweenIntensity(this Light light, float to, float duration)
        {
            Light lt = light;
            return TweenManager.EnsureInstance().CreateFloat(lt, lt.intensity, to, duration, value =>
            {
                if (lt) lt.intensity = value;
            });
        }

        public static void KillTweens(this Component component, bool complete = false)
            => TweenManager.KillTweensOf(component, complete);
    }
}
