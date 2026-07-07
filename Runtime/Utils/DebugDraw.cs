using UnityEngine;

namespace CatAnnaDev.Utils
{
    public static class DebugDraw
    {
        public static void Line(Vector3 start, Vector3 end, Color color, float duration = 0f)
        {
            Debug.DrawLine(start, end, color, duration);
        }

        public static void Ray(Vector3 origin, Vector3 direction, Color color, float duration = 0f)
        {
            Debug.DrawRay(origin, direction, color, duration);
        }

        public static void Arrow(Vector3 start, Vector3 end, Color color, float headSize = 0.25f, float duration = 0f)
        {
            Debug.DrawLine(start, end, color, duration);
            Vector3 direction = end - start;
            float length = direction.magnitude;
            if (length < Mathf.Epsilon)
            {
                return;
            }
            direction /= length;
            Vector3 up = Mathf.Abs(Vector3.Dot(direction, Vector3.up)) > 0.99f ? Vector3.right : Vector3.up;
            Vector3 right = Vector3.Cross(direction, up).normalized;
            Vector3 back = -direction * headSize;
            Vector3 side = right * headSize * 0.5f;
            Debug.DrawLine(end, end + back + side, color, duration);
            Debug.DrawLine(end, end + back - side, color, duration);
        }

        public static void WireSphere(Vector3 center, float radius, Color color, int segments = 16, float duration = 0f)
        {
            float step = Mathf.PI * 2f / segments;
            for (int i = 0; i < segments; i++)
            {
                float a = step * i;
                float b = step * (i + 1);
                Vector3 x0 = new Vector3(Mathf.Cos(a), Mathf.Sin(a), 0f) * radius;
                Vector3 x1 = new Vector3(Mathf.Cos(b), Mathf.Sin(b), 0f) * radius;
                Vector3 y0 = new Vector3(Mathf.Cos(a), 0f, Mathf.Sin(a)) * radius;
                Vector3 y1 = new Vector3(Mathf.Cos(b), 0f, Mathf.Sin(b)) * radius;
                Vector3 z0 = new Vector3(0f, Mathf.Cos(a), Mathf.Sin(a)) * radius;
                Vector3 z1 = new Vector3(0f, Mathf.Cos(b), Mathf.Sin(b)) * radius;
                Debug.DrawLine(center + x0, center + x1, color, duration);
                Debug.DrawLine(center + y0, center + y1, color, duration);
                Debug.DrawLine(center + z0, center + z1, color, duration);
            }
        }

        public static void WireCube(Vector3 center, Vector3 size, Color color, float duration = 0f)
        {
            Vector3 h = size * 0.5f;
            Vector3 p0 = center + new Vector3(-h.x, -h.y, -h.z);
            Vector3 p1 = center + new Vector3(h.x, -h.y, -h.z);
            Vector3 p2 = center + new Vector3(h.x, -h.y, h.z);
            Vector3 p3 = center + new Vector3(-h.x, -h.y, h.z);
            Vector3 p4 = center + new Vector3(-h.x, h.y, -h.z);
            Vector3 p5 = center + new Vector3(h.x, h.y, -h.z);
            Vector3 p6 = center + new Vector3(h.x, h.y, h.z);
            Vector3 p7 = center + new Vector3(-h.x, h.y, h.z);
            Debug.DrawLine(p0, p1, color, duration);
            Debug.DrawLine(p1, p2, color, duration);
            Debug.DrawLine(p2, p3, color, duration);
            Debug.DrawLine(p3, p0, color, duration);
            Debug.DrawLine(p4, p5, color, duration);
            Debug.DrawLine(p5, p6, color, duration);
            Debug.DrawLine(p6, p7, color, duration);
            Debug.DrawLine(p7, p4, color, duration);
            Debug.DrawLine(p0, p4, color, duration);
            Debug.DrawLine(p1, p5, color, duration);
            Debug.DrawLine(p2, p6, color, duration);
            Debug.DrawLine(p3, p7, color, duration);
        }

        public static void Circle(Vector3 center, float radius, Vector3 normal, Color color, int segments = 32, float duration = 0f)
        {
            Vector3 forward = Vector3.Slerp(normal, -normal, 0.5f);
            Vector3 right = Vector3.Cross(normal, forward).normalized * radius;
            forward = forward.normalized * radius;
            float step = Mathf.PI * 2f / segments;
            Vector3 previous = center + right;
            for (int i = 1; i <= segments; i++)
            {
                float angle = step * i;
                Vector3 next = center + right * Mathf.Cos(angle) + forward * Mathf.Sin(angle);
                Debug.DrawLine(previous, next, color, duration);
                previous = next;
            }
        }

        public static void Cross(Vector3 center, float size, Color color, float duration = 0f)
        {
            float h = size * 0.5f;
            Debug.DrawLine(center - Vector3.right * h, center + Vector3.right * h, color, duration);
            Debug.DrawLine(center - Vector3.up * h, center + Vector3.up * h, color, duration);
            Debug.DrawLine(center - Vector3.forward * h, center + Vector3.forward * h, color, duration);
        }
    }
}
