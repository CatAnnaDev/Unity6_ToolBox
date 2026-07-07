using UnityEngine;

namespace CatAnnaDev.Cameras
{
    [AddComponentMenu("CatAnnaDev/Camera/Camera Follow")]
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);
        [SerializeField] private float smoothTime = 0.15f;
        [SerializeField] private float maxSpeed = Mathf.Infinity;
        [SerializeField] private bool lookAtTarget;

        private Vector3 velocity;

        public Transform Target => target;

        public CameraFollow SetTarget(Transform value) { target = value; return this; }
        public CameraFollow SetOffset(Vector3 value) { offset = value; return this; }
        public CameraFollow SetSmoothTime(float value) { smoothTime = value; return this; }
        public CameraFollow SetMaxSpeed(float value) { maxSpeed = value; return this; }
        public CameraFollow SetLookAt(bool value) { lookAtTarget = value; return this; }

        public void SnapToTarget()
        {
            if (target == null) return;
            transform.position = target.position + offset;
            velocity = Vector3.zero;
        }

        private void LateUpdate()
        {
            if (target == null) return;

            Vector3 desired = target.position + offset;
            transform.position = Vector3.SmoothDamp(transform.position, desired, ref velocity, smoothTime, maxSpeed, Time.deltaTime);
            if (lookAtTarget) transform.LookAt(target);
        }
    }
}
