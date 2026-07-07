using UnityEngine;

namespace CatAnnaDev.Utils
{
    public static class GameObjectExtensions
    {
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            if (component == null)
            {
                component = gameObject.AddComponent<T>();
            }
            return component;
        }

        public static bool HasComponent<T>(this GameObject gameObject) where T : Component
        {
            return gameObject.GetComponent<T>() != null;
        }

        public static bool TryGetComponentInParent<T>(this GameObject gameObject, out T component) where T : Component
        {
            component = gameObject.GetComponentInParent<T>();
            return component != null;
        }

        public static bool TryGetComponentInChildren<T>(this GameObject gameObject, out T component) where T : Component
        {
            component = gameObject.GetComponentInChildren<T>();
            return component != null;
        }

        public static void SetLayerRecursively(this GameObject gameObject, int layer)
        {
            gameObject.layer = layer;
            Transform transform = gameObject.transform;
            int count = transform.childCount;
            for (int i = 0; i < count; i++)
            {
                transform.GetChild(i).gameObject.SetLayerRecursively(layer);
            }
        }

        public static bool IsInLayerMask(this GameObject gameObject, LayerMask mask)
        {
            return (mask.value & (1 << gameObject.layer)) != 0;
        }

        public static void Enable(this GameObject gameObject)
        {
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }
        }

        public static void Disable(this GameObject gameObject)
        {
            if (gameObject.activeSelf)
            {
                gameObject.SetActive(false);
            }
        }

        public static void ToggleActive(this GameObject gameObject)
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }

        public static void SetActiveSafe(this GameObject gameObject, bool active)
        {
            if (gameObject != null && gameObject.activeSelf != active)
            {
                gameObject.SetActive(active);
            }
        }

        public static string GetHierarchyPath(this GameObject gameObject)
        {
            Transform current = gameObject.transform;
            string path = current.name;
            current = current.parent;
            while (current != null)
            {
                path = string.Concat(current.name, "/", path);
                current = current.parent;
            }
            return path;
        }

        public static void DestroyAllChildren(this GameObject gameObject)
        {
            gameObject.transform.DestroyChildren();
        }
    }
}
