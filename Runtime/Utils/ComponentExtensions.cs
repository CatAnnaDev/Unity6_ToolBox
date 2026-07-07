using UnityEngine;

namespace CatAnnaDev.Utils
{
    public static class ComponentExtensions
    {
        public static T GetOrAddComponent<T>(this Component component) where T : Component
        {
            return component.gameObject.GetOrAddComponent<T>();
        }

        public static bool HasComponent<T>(this Component component) where T : Component
        {
            return component.GetComponent<T>() != null;
        }

        public static T GetComponentInParentOrSelf<T>(this Component component) where T : Component
        {
            T found = component.GetComponent<T>();
            if (found != null)
            {
                return found;
            }
            return component.GetComponentInParent<T>();
        }

        public static T GetComponentInChildrenOrSelf<T>(this Component component, bool includeInactive = false) where T : Component
        {
            T found = component.GetComponent<T>();
            if (found != null)
            {
                return found;
            }
            return component.GetComponentInChildren<T>(includeInactive);
        }

        public static void Enable(this Component component)
        {
            component.gameObject.Enable();
        }

        public static void Disable(this Component component)
        {
            component.gameObject.Disable();
        }
    }
}
