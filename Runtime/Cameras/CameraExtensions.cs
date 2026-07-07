using UnityEngine;

namespace CatAnnaDev.Cameras
{
    public static class CameraExtensions
    {
        public static CameraShaker Shaker(this GameObject go)
        {
            CameraShaker shaker = go.GetComponent<CameraShaker>();
            return shaker != null ? shaker : go.AddComponent<CameraShaker>();
        }

        public static CameraShaker Shaker(this Component component) => component.gameObject.Shaker();

        public static CameraFollow Follow(this GameObject go, Transform target)
        {
            CameraFollow follow = go.GetComponent<CameraFollow>();
            if (follow == null) follow = go.AddComponent<CameraFollow>();
            return follow.SetTarget(target);
        }

        public static CameraFollow Follow(this Component component, Transform target) => component.gameObject.Follow(target);
    }
}
