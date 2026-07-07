using System;
using UnityEngine;

namespace CatAnnaDev.Physics
{
    [AddComponentMenu("CatAnnaDev/Physics/Physics Events 2D")]
    public class PhysicsEvents2D : MonoBehaviour
    {
        public event Action<Collision2D> CollisionEnter;
        public event Action<Collision2D> CollisionStay;
        public event Action<Collision2D> CollisionExit;
        public event Action<Collider2D> TriggerEnter;
        public event Action<Collider2D> TriggerStay;
        public event Action<Collider2D> TriggerExit;

        private void OnCollisionEnter2D(Collision2D c) => CollisionEnter?.Invoke(c);
        private void OnCollisionStay2D(Collision2D c) => CollisionStay?.Invoke(c);
        private void OnCollisionExit2D(Collision2D c) => CollisionExit?.Invoke(c);
        private void OnTriggerEnter2D(Collider2D other) => TriggerEnter?.Invoke(other);
        private void OnTriggerStay2D(Collider2D other) => TriggerStay?.Invoke(other);
        private void OnTriggerExit2D(Collider2D other) => TriggerExit?.Invoke(other);
    }
}
