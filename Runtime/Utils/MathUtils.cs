using UnityEngine;

namespace CatAnnaDev.Utils
{
    public static class MathUtils
    {
        public static float Remap(float value, float inMin, float inMax, float outMin, float outMax)
        {
            return outMin + (value - inMin) * (outMax - outMin) / (inMax - inMin);
        }

        public static float RemapClamped(float value, float inMin, float inMax, float outMin, float outMax)
        {
            float t = Mathf.Clamp01((value - inMin) / (inMax - inMin));
            return Mathf.Lerp(outMin, outMax, t);
        }

        public static float SmoothStep(float from, float to, float t)
        {
            t = Mathf.Clamp01(t);
            t = t * t * (3f - 2f * t);
            return Mathf.Lerp(from, to, t);
        }

        public static float SmootherStep(float from, float to, float t)
        {
            t = Mathf.Clamp01(t);
            t = t * t * t * (t * (t * 6f - 15f) + 10f);
            return Mathf.Lerp(from, to, t);
        }

        public static float Wrap(float value, float min, float max)
        {
            float range = max - min;
            if (range <= 0f)
            {
                return min;
            }
            return min + Mathf.Repeat(value - min, range);
        }

        public static int WrapInt(int value, int min, int max)
        {
            int range = max - min;
            if (range <= 0)
            {
                return min;
            }
            int result = (value - min) % range;
            if (result < 0)
            {
                result += range;
            }
            return min + result;
        }

        public static float PingPong(float value, float length)
        {
            return Mathf.PingPong(value, length);
        }

        public static bool Approximately(float a, float b, float epsilon = 0.0001f)
        {
            return Mathf.Abs(a - b) <= epsilon;
        }

        public static Vector3 ClosestPointOnLine(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
        {
            Vector3 direction = lineEnd - lineStart;
            float length = direction.magnitude;
            if (length < Mathf.Epsilon)
            {
                return lineStart;
            }
            direction /= length;
            float dot = Mathf.Clamp(Vector3.Dot(point - lineStart, direction), 0f, length);
            return lineStart + direction * dot;
        }

        public static Vector3 ClosestPointOnInfiniteLine(Vector3 linePoint, Vector3 lineDirection, Vector3 point)
        {
            lineDirection.Normalize();
            float dot = Vector3.Dot(point - linePoint, lineDirection);
            return linePoint + lineDirection * dot;
        }

        public static float ExpDecay(float current, float target, float decay, float deltaTime)
        {
            return target + (current - target) * Mathf.Exp(-decay * deltaTime);
        }

        public static Vector3 ExpDecay(Vector3 current, Vector3 target, float decay, float deltaTime)
        {
            float factor = Mathf.Exp(-decay * deltaTime);
            return target + (current - target) * factor;
        }

        public static float DampedSpring(float current, float target, ref float velocity, float damping, float frequency, float deltaTime)
        {
            float angular = frequency * 2f * Mathf.PI;
            float f = 1f + 2f * deltaTime * damping * angular;
            float oo = angular * angular;
            float hoo = deltaTime * oo;
            float hhoo = deltaTime * hoo;
            float detInv = 1f / (f + hhoo);
            float detX = f * current + deltaTime * velocity + hhoo * target;
            float detV = velocity + hoo * (target - current);
            velocity = detV * detInv;
            return detX * detInv;
        }

        public static float MoveTowardsAngle(float current, float target, float maxDelta)
        {
            return Mathf.MoveTowardsAngle(current, target, maxDelta);
        }

        public static float SignedAngle(Vector2 from, Vector2 to)
        {
            return Vector2.SignedAngle(from, to);
        }

        public static float RoundToNearest(float value, float nearest)
        {
            if (nearest <= 0f)
            {
                return value;
            }
            return Mathf.Round(value / nearest) * nearest;
        }

        public static int NextPowerOfTwo(int value)
        {
            if (value < 1)
            {
                return 1;
            }
            value--;
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            return value + 1;
        }

        public static bool IsPowerOfTwo(int value)
        {
            return value > 0 && (value & (value - 1)) == 0;
        }

        public static float InverseLerpUnclamped(float a, float b, float value)
        {
            if (Mathf.Approximately(a, b))
            {
                return 0f;
            }
            return (value - a) / (b - a);
        }
    }
}
