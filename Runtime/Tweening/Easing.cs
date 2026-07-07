using UnityEngine;

namespace CatAnnaDev.Tweening
{
    public enum Ease
    {
        Linear,
        InSine, OutSine, InOutSine,
        InQuad, OutQuad, InOutQuad,
        InCubic, OutCubic, InOutCubic,
        InQuart, OutQuart, InOutQuart,
        InQuint, OutQuint, InOutQuint,
        InExpo, OutExpo, InOutExpo,
        InCirc, OutCirc, InOutCirc,
        InBack, OutBack, InOutBack,
        InElastic, OutElastic, InOutElastic,
        InBounce, OutBounce, InOutBounce
    }

    public static class Easing
    {
        const float BackC1 = 1.70158f;
        const float BackC2 = BackC1 * 1.525f;
        const float BackC3 = BackC1 + 1f;
        const float ElasticC4 = (2f * Mathf.PI) / 3f;
        const float ElasticC5 = (2f * Mathf.PI) / 4.5f;
        const float BounceN1 = 7.5625f;
        const float BounceD1 = 2.75f;

        public static float Evaluate(Ease ease, float t)
        {
            return ease switch
            {
                Ease.Linear => t,
                Ease.InSine => InSine(t),
                Ease.OutSine => OutSine(t),
                Ease.InOutSine => InOutSine(t),
                Ease.InQuad => InQuad(t),
                Ease.OutQuad => OutQuad(t),
                Ease.InOutQuad => InOutQuad(t),
                Ease.InCubic => InCubic(t),
                Ease.OutCubic => OutCubic(t),
                Ease.InOutCubic => InOutCubic(t),
                Ease.InQuart => InQuart(t),
                Ease.OutQuart => OutQuart(t),
                Ease.InOutQuart => InOutQuart(t),
                Ease.InQuint => InQuint(t),
                Ease.OutQuint => OutQuint(t),
                Ease.InOutQuint => InOutQuint(t),
                Ease.InExpo => InExpo(t),
                Ease.OutExpo => OutExpo(t),
                Ease.InOutExpo => InOutExpo(t),
                Ease.InCirc => InCirc(t),
                Ease.OutCirc => OutCirc(t),
                Ease.InOutCirc => InOutCirc(t),
                Ease.InBack => InBack(t),
                Ease.OutBack => OutBack(t),
                Ease.InOutBack => InOutBack(t),
                Ease.InElastic => InElastic(t),
                Ease.OutElastic => OutElastic(t),
                Ease.InOutElastic => InOutElastic(t),
                Ease.InBounce => InBounce(t),
                Ease.OutBounce => OutBounce(t),
                Ease.InOutBounce => InOutBounce(t),
                _ => t
            };
        }

        public static float InSine(float t) => 1f - Mathf.Cos((t * Mathf.PI) * 0.5f);
        public static float OutSine(float t) => Mathf.Sin((t * Mathf.PI) * 0.5f);
        public static float InOutSine(float t) => -(Mathf.Cos(Mathf.PI * t) - 1f) * 0.5f;

        public static float InQuad(float t) => t * t;
        public static float OutQuad(float t) => 1f - (1f - t) * (1f - t);
        public static float InOutQuad(float t)
            => t < 0.5f ? 2f * t * t : 1f - (-2f * t + 2f) * (-2f * t + 2f) * 0.5f;

        public static float InCubic(float t) => t * t * t;
        public static float OutCubic(float t)
        {
            float f = 1f - t;
            return 1f - f * f * f;
        }
        public static float InOutCubic(float t)
        {
            if (t < 0.5f) return 4f * t * t * t;
            float f = -2f * t + 2f;
            return 1f - f * f * f * 0.5f;
        }

        public static float InQuart(float t) => t * t * t * t;
        public static float OutQuart(float t)
        {
            float f = 1f - t;
            return 1f - f * f * f * f;
        }
        public static float InOutQuart(float t)
        {
            if (t < 0.5f) return 8f * t * t * t * t;
            float f = -2f * t + 2f;
            return 1f - f * f * f * f * 0.5f;
        }

        public static float InQuint(float t) => t * t * t * t * t;
        public static float OutQuint(float t)
        {
            float f = 1f - t;
            return 1f - f * f * f * f * f;
        }
        public static float InOutQuint(float t)
        {
            if (t < 0.5f) return 16f * t * t * t * t * t;
            float f = -2f * t + 2f;
            return 1f - f * f * f * f * f * 0.5f;
        }

        public static float InExpo(float t) => t <= 0f ? 0f : Mathf.Pow(2f, 10f * t - 10f);
        public static float OutExpo(float t) => t >= 1f ? 1f : 1f - Mathf.Pow(2f, -10f * t);
        public static float InOutExpo(float t)
        {
            if (t <= 0f) return 0f;
            if (t >= 1f) return 1f;
            return t < 0.5f
                ? Mathf.Pow(2f, 20f * t - 10f) * 0.5f
                : (2f - Mathf.Pow(2f, -20f * t + 10f)) * 0.5f;
        }

        public static float InCirc(float t) => 1f - Mathf.Sqrt(1f - t * t);
        public static float OutCirc(float t)
        {
            float f = t - 1f;
            return Mathf.Sqrt(1f - f * f);
        }
        public static float InOutCirc(float t)
        {
            if (t < 0.5f)
            {
                float a = 2f * t;
                return (1f - Mathf.Sqrt(1f - a * a)) * 0.5f;
            }
            float b = -2f * t + 2f;
            return (Mathf.Sqrt(1f - b * b) + 1f) * 0.5f;
        }

        public static float InBack(float t) => BackC3 * t * t * t - BackC1 * t * t;
        public static float OutBack(float t)
        {
            float f = t - 1f;
            return 1f + BackC3 * f * f * f + BackC1 * f * f;
        }
        public static float InOutBack(float t)
        {
            if (t < 0.5f)
            {
                float a = 2f * t;
                return (a * a * ((BackC2 + 1f) * a - BackC2)) * 0.5f;
            }
            float b = 2f * t - 2f;
            return (b * b * ((BackC2 + 1f) * b + BackC2) + 2f) * 0.5f;
        }

        public static float InElastic(float t)
        {
            if (t <= 0f) return 0f;
            if (t >= 1f) return 1f;
            return -Mathf.Pow(2f, 10f * t - 10f) * Mathf.Sin((t * 10f - 10.75f) * ElasticC4);
        }
        public static float OutElastic(float t)
        {
            if (t <= 0f) return 0f;
            if (t >= 1f) return 1f;
            return Mathf.Pow(2f, -10f * t) * Mathf.Sin((t * 10f - 0.75f) * ElasticC4) + 1f;
        }
        public static float InOutElastic(float t)
        {
            if (t <= 0f) return 0f;
            if (t >= 1f) return 1f;
            return t < 0.5f
                ? -(Mathf.Pow(2f, 20f * t - 10f) * Mathf.Sin((20f * t - 11.125f) * ElasticC5)) * 0.5f
                : (Mathf.Pow(2f, -20f * t + 10f) * Mathf.Sin((20f * t - 11.125f) * ElasticC5)) * 0.5f + 1f;
        }

        public static float InBounce(float t) => 1f - OutBounce(1f - t);
        public static float OutBounce(float t)
        {
            if (t < 1f / BounceD1)
                return BounceN1 * t * t;
            if (t < 2f / BounceD1)
            {
                t -= 1.5f / BounceD1;
                return BounceN1 * t * t + 0.75f;
            }
            if (t < 2.5f / BounceD1)
            {
                t -= 2.25f / BounceD1;
                return BounceN1 * t * t + 0.9375f;
            }
            t -= 2.625f / BounceD1;
            return BounceN1 * t * t + 0.984375f;
        }
        public static float InOutBounce(float t)
            => t < 0.5f
                ? (1f - OutBounce(1f - 2f * t)) * 0.5f
                : (1f + OutBounce(2f * t - 1f)) * 0.5f;
    }
}
