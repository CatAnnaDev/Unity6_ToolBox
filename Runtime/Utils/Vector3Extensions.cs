using UnityEngine;

namespace CatAnnaDev.Utils
{
    public static class Vector3Extensions
    {
        public static Vector3 WithX(this Vector3 v, float x)
        {
            v.x = x;
            return v;
        }

        public static Vector3 WithY(this Vector3 v, float y)
        {
            v.y = y;
            return v;
        }

        public static Vector3 WithZ(this Vector3 v, float z)
        {
            v.z = z;
            return v;
        }

        public static Vector3 With(this Vector3 v, float? x = null, float? y = null, float? z = null)
        {
            return new Vector3(x ?? v.x, y ?? v.y, z ?? v.z);
        }

        public static Vector3 AddX(this Vector3 v, float x)
        {
            v.x += x;
            return v;
        }

        public static Vector3 AddY(this Vector3 v, float y)
        {
            v.y += y;
            return v;
        }

        public static Vector3 AddZ(this Vector3 v, float z)
        {
            v.z += z;
            return v;
        }

        public static Vector3 Flatten(this Vector3 v)
        {
            v.y = 0f;
            return v;
        }

        public static Vector2 ToVector2XY(this Vector3 v)
        {
            return new Vector2(v.x, v.y);
        }

        public static Vector2 ToVector2XZ(this Vector3 v)
        {
            return new Vector2(v.x, v.z);
        }

        public static float ManhattanDistance(this Vector3 a, Vector3 b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
        }

        public static Vector3 RandomInsideRadius(this Vector3 center, float radius)
        {
            return center + Random.insideUnitSphere * radius;
        }

        public static Vector3 RandomInsideRadiusFlat(this Vector3 center, float radius)
        {
            Vector2 circle = Random.insideUnitCircle * radius;
            return new Vector3(center.x + circle.x, center.y, center.z + circle.y);
        }

        public static bool IsWithin(this Vector3 v, Vector3 target, float distance)
        {
            return (v - target).sqrMagnitude <= distance * distance;
        }

        public static Vector3 ClampMagnitudeMin(this Vector3 v, float minMagnitude)
        {
            float sqr = v.sqrMagnitude;
            if (sqr < minMagnitude * minMagnitude && sqr > 0f)
            {
                return v.normalized * minMagnitude;
            }
            return v;
        }

        public static Vector3 SnapToGrid(this Vector3 v, float gridSize)
        {
            return new Vector3(
                Mathf.Round(v.x / gridSize) * gridSize,
                Mathf.Round(v.y / gridSize) * gridSize,
                Mathf.Round(v.z / gridSize) * gridSize);
        }

        public static Vector3 DirectionTo(this Vector3 from, Vector3 to)
        {
            return (to - from).normalized;
        }

        public static bool Approximately(this Vector3 a, Vector3 b, float epsilon = 0.0001f)
        {
            return (a - b).sqrMagnitude <= epsilon * epsilon;
        }

        public static Vector3 Abs(this Vector3 v)
        {
            return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
        }

        public static float MaxComponent(this Vector3 v)
        {
            return Mathf.Max(v.x, Mathf.Max(v.y, v.z));
        }

        public static float MinComponent(this Vector3 v)
        {
            return Mathf.Min(v.x, Mathf.Min(v.y, v.z));
        }
    }
}
