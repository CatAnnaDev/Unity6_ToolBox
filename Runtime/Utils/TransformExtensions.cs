using UnityEngine;

namespace CatAnnaDev.Utils
{
    public static class TransformExtensions
    {
        public static void ResetLocal(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        public static void Reset(this Transform transform)
        {
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        public static Transform SetPositionX(this Transform transform, float x)
        {
            Vector3 p = transform.position;
            p.x = x;
            transform.position = p;
            return transform;
        }

        public static Transform SetPositionY(this Transform transform, float y)
        {
            Vector3 p = transform.position;
            p.y = y;
            transform.position = p;
            return transform;
        }

        public static Transform SetPositionZ(this Transform transform, float z)
        {
            Vector3 p = transform.position;
            p.z = z;
            transform.position = p;
            return transform;
        }

        public static Transform SetLocalPositionX(this Transform transform, float x)
        {
            Vector3 p = transform.localPosition;
            p.x = x;
            transform.localPosition = p;
            return transform;
        }

        public static Transform SetLocalPositionY(this Transform transform, float y)
        {
            Vector3 p = transform.localPosition;
            p.y = y;
            transform.localPosition = p;
            return transform;
        }

        public static Transform SetLocalPositionZ(this Transform transform, float z)
        {
            Vector3 p = transform.localPosition;
            p.z = z;
            transform.localPosition = p;
            return transform;
        }

        public static Transform AddPosition(this Transform transform, Vector3 delta)
        {
            transform.position += delta;
            return transform;
        }

        public static void DestroyChildren(this Transform transform)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Object.Destroy(transform.GetChild(i).gameObject);
            }
        }

        public static void DestroyChildrenImmediate(this Transform transform)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Object.DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }

        public static void DetachChildren(this Transform transform, bool worldPositionStays = true)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                transform.GetChild(i).SetParent(null, worldPositionStays);
            }
        }

        public static T GetOrAddComponent<T>(this Transform transform) where T : Component
        {
            T component = transform.GetComponent<T>();
            if (component == null)
            {
                component = transform.gameObject.AddComponent<T>();
            }
            return component;
        }

        public static Transform FindDeepChild(this Transform transform, string childName)
        {
            int count = transform.childCount;
            for (int i = 0; i < count; i++)
            {
                Transform child = transform.GetChild(i);
                if (child.name == childName)
                {
                    return child;
                }
            }
            for (int i = 0; i < count; i++)
            {
                Transform result = transform.GetChild(i).FindDeepChild(childName);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        public static void SetLayerRecursively(this Transform transform, int layer)
        {
            transform.gameObject.layer = layer;
            int count = transform.childCount;
            for (int i = 0; i < count; i++)
            {
                transform.GetChild(i).SetLayerRecursively(layer);
            }
        }

        public static void LookAt2D(this Transform transform, Vector3 target, Vector3 up)
        {
            Vector3 direction = target - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        public static void LookAt2D(this Transform transform, Vector3 target)
        {
            transform.LookAt2D(target, Vector3.up);
        }

        public static void CopyFrom(this Transform transform, Transform source, bool copyScale = true)
        {
            transform.position = source.position;
            transform.rotation = source.rotation;
            if (copyScale)
            {
                transform.localScale = source.localScale;
            }
        }

        public static Vector3 DirectionTo(this Transform transform, Transform target)
        {
            return (target.position - transform.position).normalized;
        }

        public static float DistanceTo(this Transform transform, Transform target)
        {
            return Vector3.Distance(transform.position, target.position);
        }

        public static bool IsWithinDistance(this Transform transform, Transform target, float distance)
        {
            return (target.position - transform.position).sqrMagnitude <= distance * distance;
        }
    }
}
