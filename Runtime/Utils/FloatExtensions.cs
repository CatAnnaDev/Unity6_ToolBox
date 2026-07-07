using UnityEngine;

namespace CatAnnaDev.Utils
{
    public static class FloatExtensions
    {
        public static float Remap(this float value, float inMin, float inMax, float outMin, float outMax)
        {
            return outMin + (value - inMin) * (outMax - outMin) / (inMax - inMin);
        }

        public static float RemapClamped(this float value, float inMin, float inMax, float outMin, float outMax)
        {
            float t = Mathf.Clamp01((value - inMin) / (inMax - inMin));
            return Mathf.Lerp(outMin, outMax, t);
        }

        public static float Remap01(this float value, float inMin, float inMax)
        {
            return (value - inMin) / (inMax - inMin);
        }

        public static bool Approximately(this float value, float other, float epsilon = 0.0001f)
        {
            return Mathf.Abs(value - other) <= epsilon;
        }

        public static float RoundToNearest(this float value, float nearest)
        {
            if (nearest <= 0f)
            {
                return value;
            }
            return Mathf.Round(value / nearest) * nearest;
        }

        public static float Snap(this float value, float step)
        {
            return value.RoundToNearest(step);
        }

        public static float ToRadians(this float degrees)
        {
            return degrees * Mathf.Deg2Rad;
        }

        public static float ToDegrees(this float radians)
        {
            return radians * Mathf.Rad2Deg;
        }

        public static float Clamped01(this float value)
        {
            return Mathf.Clamp01(value);
        }

        public static float Sign(this float value)
        {
            return value < 0f ? -1f : 1f;
        }

        public static bool IsBetween(this float value, float min, float max)
        {
            return value >= min && value <= max;
        }

        public static float Wrap(this float value, float min, float max)
        {
            float range = max - min;
            if (range <= 0f)
            {
                return min;
            }
            return min + Mathf.Repeat(value - min, range);
        }

        public static int ToIntFloor(this float value)
        {
            return Mathf.FloorToInt(value);
        }

        public static int ToIntRound(this float value)
        {
            return Mathf.RoundToInt(value);
        }

        public static int ToIntCeil(this float value)
        {
            return Mathf.CeilToInt(value);
        }
    }
}
