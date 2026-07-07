using UnityEngine;

namespace CatAnnaDev.Utils
{
    public static class GizmoUtils
    {
        public static void DrawWireSphere(Vector3 center, float radius, Color color)
        {
            Color previous = Gizmos.color;
            Gizmos.color = color;
            Gizmos.DrawWireSphere(center, radius);
            Gizmos.color = previous;
        }

        public static void DrawWireCube(Vector3 center, Vector3 size, Color color)
        {
            Color previous = Gizmos.color;
            Gizmos.color = color;
            Gizmos.DrawWireCube(center, size);
            Gizmos.color = previous;
        }

        public static void DrawArrow(Vector3 start, Vector3 end, Color color, float headSize = 0.25f)
        {
            Color previous = Gizmos.color;
            Gizmos.color = color;
            Gizmos.DrawLine(start, end);
            Vector3 direction = end - start;
            float length = direction.magnitude;
            if (length > Mathf.Epsilon)
            {
                direction /= length;
                Vector3 up = Mathf.Abs(Vector3.Dot(direction, Vector3.up)) > 0.99f ? Vector3.right : Vector3.up;
                Vector3 right = Vector3.Cross(direction, up).normalized;
                Vector3 back = -direction * headSize;
                Vector3 side = right * headSize * 0.5f;
                Gizmos.DrawLine(end, end + back + side);
                Gizmos.DrawLine(end, end + back - side);
            }
            Gizmos.color = previous;
        }

        public static void DrawWireCircle(Vector3 center, float radius, Vector3 normal, Color color, int segments = 32)
        {
            Color previous = Gizmos.color;
            Gizmos.color = color;
            Vector3 forward = Vector3.Slerp(normal, -normal, 0.5f);
            Vector3 right = Vector3.Cross(normal, forward).normalized * radius;
            forward = forward.normalized * radius;
            float step = Mathf.PI * 2f / segments;
            Vector3 prev = center + right;
            for (int i = 1; i <= segments; i++)
            {
                float angle = step * i;
                Vector3 next = center + right * Mathf.Cos(angle) + forward * Mathf.Sin(angle);
                Gizmos.DrawLine(prev, next);
                prev = next;
            }
            Gizmos.color = previous;
        }

        public static void DrawBounds(Bounds bounds, Color color)
        {
            DrawWireCube(bounds.center, bounds.size, color);
        }

        public static void DrawPoint(Vector3 position, float size, Color color)
        {
            Color previous = Gizmos.color;
            Gizmos.color = color;
            float h = size * 0.5f;
            Gizmos.DrawLine(position - Vector3.right * h, position + Vector3.right * h);
            Gizmos.DrawLine(position - Vector3.up * h, position + Vector3.up * h);
            Gizmos.DrawLine(position - Vector3.forward * h, position + Vector3.forward * h);
            Gizmos.color = previous;
        }

#if UNITY_EDITOR
        public static void DrawText(Vector3 position, string text, Color color)
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = color;
            UnityEditor.Handles.Label(position, text, style);
        }
#else
        public static void DrawText(Vector3 position, string text, Color color)
        {
        }
#endif
    }
}
