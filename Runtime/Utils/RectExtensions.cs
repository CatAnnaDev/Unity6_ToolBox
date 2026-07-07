using UnityEngine;

namespace CatAnnaDev.Utils
{
    public static class RectExtensions
    {
        public static Rect WithX(this Rect rect, float x)
        {
            rect.x = x;
            return rect;
        }

        public static Rect WithY(this Rect rect, float y)
        {
            rect.y = y;
            return rect;
        }

        public static Rect WithWidth(this Rect rect, float width)
        {
            rect.width = width;
            return rect;
        }

        public static Rect WithHeight(this Rect rect, float height)
        {
            rect.height = height;
            return rect;
        }

        public static Rect Expand(this Rect rect, float amount)
        {
            return new Rect(rect.x - amount, rect.y - amount, rect.width + amount * 2f, rect.height + amount * 2f);
        }

        public static Rect Expand(this Rect rect, float horizontal, float vertical)
        {
            return new Rect(rect.x - horizontal, rect.y - vertical, rect.width + horizontal * 2f, rect.height + vertical * 2f);
        }

        public static Rect Shrink(this Rect rect, float amount)
        {
            return rect.Expand(-amount);
        }

        public static Vector2 RandomPoint(this Rect rect)
        {
            return new Vector2(Random.Range(rect.xMin, rect.xMax), Random.Range(rect.yMin, rect.yMax));
        }

        public static Rect Encapsulate(this Rect rect, Vector2 point)
        {
            float xMin = Mathf.Min(rect.xMin, point.x);
            float yMin = Mathf.Min(rect.yMin, point.y);
            float xMax = Mathf.Max(rect.xMax, point.x);
            float yMax = Mathf.Max(rect.yMax, point.y);
            return Rect.MinMaxRect(xMin, yMin, xMax, yMax);
        }

        public static bool Contains(this Rect rect, Rect other)
        {
            return rect.xMin <= other.xMin && rect.xMax >= other.xMax && rect.yMin <= other.yMin && rect.yMax >= other.yMax;
        }

        public static Rect Split(this Rect rect, int index, int count, float spacing = 0f)
        {
            float total = rect.width - spacing * (count - 1);
            float each = total / count;
            return new Rect(rect.x + (each + spacing) * index, rect.y, each, rect.height);
        }

        public static Rect SplitVertical(this Rect rect, int index, int count, float spacing = 0f)
        {
            float total = rect.height - spacing * (count - 1);
            float each = total / count;
            return new Rect(rect.x, rect.y + (each + spacing) * index, rect.width, each);
        }
    }
}
