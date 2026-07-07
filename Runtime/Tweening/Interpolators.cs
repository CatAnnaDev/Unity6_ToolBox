using UnityEngine;

namespace CatAnnaDev.Tweening
{
    public static class Interpolators
    {
        public static float Lerp(float a, float b, float t) => Mathf.LerpUnclamped(a, b, t);

        public static Vector2 Lerp(Vector2 a, Vector2 b, float t) => Vector2.LerpUnclamped(a, b, t);

        public static Vector3 Lerp(Vector3 a, Vector3 b, float t) => Vector3.LerpUnclamped(a, b, t);

        public static Vector4 Lerp(Vector4 a, Vector4 b, float t) => Vector4.LerpUnclamped(a, b, t);

        public static Quaternion Slerp(Quaternion a, Quaternion b, float t) => Quaternion.SlerpUnclamped(a, b, t);

        public static Color Lerp(Color a, Color b, float t) => Color.LerpUnclamped(a, b, t);

        public static float LerpEased(float a, float b, Ease ease, float t)
            => Mathf.LerpUnclamped(a, b, Easing.Evaluate(ease, t));

        public static Vector2 LerpEased(Vector2 a, Vector2 b, Ease ease, float t)
            => Vector2.LerpUnclamped(a, b, Easing.Evaluate(ease, t));

        public static Vector3 LerpEased(Vector3 a, Vector3 b, Ease ease, float t)
            => Vector3.LerpUnclamped(a, b, Easing.Evaluate(ease, t));

        public static Quaternion SlerpEased(Quaternion a, Quaternion b, Ease ease, float t)
            => Quaternion.SlerpUnclamped(a, b, Easing.Evaluate(ease, t));

        public static Color LerpEased(Color a, Color b, Ease ease, float t)
            => Color.LerpUnclamped(a, b, Easing.Evaluate(ease, t));
    }
}
