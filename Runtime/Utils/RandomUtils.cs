using System;
using UnityEngine;

namespace CatAnnaDev.Utils
{
    [Serializable]
    public struct RandomState
    {
        [SerializeField]
        private uint state;

        public RandomState(uint seed)
        {
            state = seed == 0u ? 0x9E3779B9u : seed;
        }

        public static RandomState FromTime()
        {
            return new RandomState((uint)Environment.TickCount);
        }

        public uint NextUInt()
        {
            uint x = state;
            x ^= x << 13;
            x ^= x >> 17;
            x ^= x << 5;
            state = x;
            return x;
        }

        public int NextInt(int minInclusive, int maxExclusive)
        {
            if (maxExclusive <= minInclusive)
            {
                return minInclusive;
            }
            uint range = (uint)(maxExclusive - minInclusive);
            return minInclusive + (int)(NextUInt() % range);
        }

        public float NextFloat()
        {
            return (NextUInt() & 0xFFFFFF) / (float)0x1000000;
        }

        public float NextFloat(float min, float max)
        {
            return min + NextFloat() * (max - min);
        }

        public bool NextBool()
        {
            return (NextUInt() & 1u) == 1u;
        }
    }

    public static class RandomUtils
    {
        public static bool Chance(float probability)
        {
            return UnityEngine.Random.value < probability;
        }

        public static int RandomSign()
        {
            return UnityEngine.Random.value < 0.5f ? -1 : 1;
        }

        public static int WeightedIndex(float[] weights)
        {
            if (weights == null || weights.Length == 0)
            {
                return -1;
            }
            float total = 0f;
            for (int i = 0; i < weights.Length; i++)
            {
                total += weights[i];
            }
            if (total <= 0f)
            {
                return UnityEngine.Random.Range(0, weights.Length);
            }
            float roll = UnityEngine.Random.value * total;
            float running = 0f;
            for (int i = 0; i < weights.Length; i++)
            {
                running += weights[i];
                if (roll <= running)
                {
                    return i;
                }
            }
            return weights.Length - 1;
        }

        public static Vector2 PointInsideUnitCircle()
        {
            return UnityEngine.Random.insideUnitCircle;
        }

        public static Vector3 PointInsideUnitSphere()
        {
            return UnityEngine.Random.insideUnitSphere;
        }

        public static Vector3 PointOnUnitSphere()
        {
            return UnityEngine.Random.onUnitSphere;
        }

        public static Vector2 PointOnUnitCircle()
        {
            float angle = UnityEngine.Random.value * Mathf.PI * 2f;
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }

        public static Vector3 PointInsideAnnulus(float innerRadius, float outerRadius)
        {
            float angle = UnityEngine.Random.value * Mathf.PI * 2f;
            float min2 = innerRadius * innerRadius;
            float max2 = outerRadius * outerRadius;
            float radius = Mathf.Sqrt(UnityEngine.Random.Range(min2, max2));
            return new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
        }

        public static Vector3 PointInBounds(Bounds bounds)
        {
            return new Vector3(
                UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
                UnityEngine.Random.Range(bounds.min.y, bounds.max.y),
                UnityEngine.Random.Range(bounds.min.z, bounds.max.z));
        }

        public static Color RandomColor(float saturation = 0.7f, float value = 0.9f)
        {
            return UnityEngine.Color.HSVToRGB(UnityEngine.Random.value, saturation, value);
        }

        public static Color RandomColorBetween(Color a, Color b)
        {
            return Color.Lerp(a, b, UnityEngine.Random.value);
        }

        public static Quaternion RandomYaw()
        {
            return Quaternion.Euler(0f, UnityEngine.Random.value * 360f, 0f);
        }

        public static float RandomGaussian(float mean = 0f, float standardDeviation = 1f)
        {
            float u1 = 1f - UnityEngine.Random.value;
            float u2 = 1f - UnityEngine.Random.value;
            float normal = Mathf.Sqrt(-2f * Mathf.Log(u1)) * Mathf.Sin(2f * Mathf.PI * u2);
            return mean + standardDeviation * normal;
        }
    }
}
