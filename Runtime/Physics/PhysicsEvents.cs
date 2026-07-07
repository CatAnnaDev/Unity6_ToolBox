using System;
using UnityEngine;

namespace CatAnnaDev.Physics
{
    [AddComponentMenu("CatAnnaDev/Physics/Physics Events")]
    public class PhysicsEvents : MonoBehaviour
    {
        public event Action<Collision> CollisionEnter;
        public event Action<Collision> CollisionStay;
        public event Action<Collision> CollisionExit;
        public event Action<Collider> TriggerEnter;
        public event Action<Collider> TriggerStay;
        public event Action<Collider> TriggerExit;

        private void OnCollisionEnter(Collision c) => CollisionEnter?.Invoke(c);
        private void OnCollisionStay(Collision c) => CollisionStay?.Invoke(c);
        private void OnCollisionExit(Collision c) => CollisionExit?.Invoke(c);
        private void OnTriggerEnter(Collider other) => TriggerEnter?.Invoke(other);
        private void OnTriggerStay(Collider other) => TriggerStay?.Invoke(other);
        private void OnTriggerExit(Collider other) => TriggerExit?.Invoke(other);
    }
}
