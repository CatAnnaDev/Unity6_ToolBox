using UnityEngine;

namespace CatAnnaDev.Utils
{
    public static class Vector2Extensions
    {
        public static Vector2 WithX(this Vector2 v, float x)
        {
            v.x = x;
            return v;
        }

        public static Vector2 WithY(this Vector2 v, float y)
        {
            v.y = y;
            return v;
        }

        public static Vector2 AddX(this Vector2 v, float x)
        {
            v.x += x;
            return v;
        }

        public static Vector2 AddY(this Vector2 v, float y)
        {
            v.y += y;
            return v;
        }

        public static Vector3 ToVector3XY(this Vector2 v, float z = 0f)
        {
            return new Vector3(v.x, v.y, z);
        }

        public static Vector3 ToVector3XZ(this Vector2 v, float y = 0f)
        {
            return new Vector3(v.x, y, v.y);
        }

        public static Vector2 Rotate(this Vector2 v, float degrees)
        {
            float rad = degrees * Mathf.Deg2Rad;
            float sin = Mathf.Sin(rad);
            float cos = Mathf.Cos(rad);
            return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
        }

        public static Vector2 Perpendicular(this Vector2 v)
        {
            return new Vector2(-v.y, v.x);
        }

        public static float ManhattanDistance(this Vector2 a, Vector2 b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }

        public static Vector2 RandomInsideRadius(this Vector2 center, float radius)
        {
            Vector2 offset = Random.insideUnitCircle * radius;
            return center + offset;
        }

        public static bool IsWithin(this Vector2 v, Vector2 target, float distance)
        {
            return (v - target).sqrMagnitude <= distance * distance;
        }

        public static Vector2 ClampMagnitudeMin(this Vector2 v, float minMagnitude)
        {
            float sqr = v.sqrMagnitude;
            if (sqr < minMagnitude * minMagnitude && sqr > 0f)
            {
                return v.normalized * minMagnitude;
            }
            return v;
        }

        public static Vector2 SnapToGrid(this Vector2 v, float gridSize)
        {
            return new Vector2(Mathf.Round(v.x / gridSize) * gridSize, Mathf.Round(v.y / gridSize) * gridSize);
        }

        public static float Angle(this Vector2 v)
        {
            return Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
        }

        public static bool Approximately(this Vector2 a, Vector2 b, float epsilon = 0.0001f)
        {
            return (a - b).sqrMagnitude <= epsilon * epsilon;
        }
    }
}
