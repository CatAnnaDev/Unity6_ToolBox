using System;
using UnityEngine;

namespace CatAnnaDev.Physics
{
    public static class PhysicsExtensions
    {
        public static PhysicsEvents PhysicsEvents(this GameObject go)
        {
            PhysicsEvents events = go.GetComponent<PhysicsEvents>();
            return events != null ? events : go.AddComponent<PhysicsEvents>();
        }

        public static PhysicsEvents PhysicsEvents(this Component component) => component.gameObject.PhysicsEvents();

        public static PhysicsEvents OnCollisionEnter(this PhysicsEvents e, Action<Collision> callback) { if (callback != null) e.CollisionEnter += callback; return e; }
        public static PhysicsEvents OnCollisionStay(this PhysicsEvents e, Action<Collision> callback) { if (callback != null) e.CollisionStay += callback; return e; }
        public static PhysicsEvents OnCollisionExit(this PhysicsEvents e, Action<Collision> callback) { if (callback != null) e.CollisionExit += callback; return e; }
        public static PhysicsEvents OnTriggerEnter(this PhysicsEvents e, Action<Collider> callback) { if (callback != null) e.TriggerEnter += callback; return e; }
        public static PhysicsEvents OnTriggerStay(this PhysicsEvents e, Action<Collider> callback) { if (callback != null) e.TriggerStay += callback; return e; }
        public static PhysicsEvents OnTriggerExit(this PhysicsEvents e, Action<Collider> callback) { if (callback != null) e.TriggerExit += callback; return e; }

        public static PhysicsEvents OnTriggerEnterTag(this PhysicsEvents e, string tag, Action<Collider> callback)
            => e.OnTriggerEnter(other => { if (other.CompareTag(tag)) callback(other); });
        public static PhysicsEvents OnTriggerExitTag(this PhysicsEvents e, string tag, Action<Collider> callback)
            => e.OnTriggerExit(other => { if (other.CompareTag(tag)) callback(other); });
        public static PhysicsEvents OnCollisionEnterTag(this PhysicsEvents e, string tag, Action<Collision> callback)
            => e.OnCollisionEnter(c => { if (c.collider.CompareTag(tag)) callback(c); });

        public static PhysicsEvents OnTriggerEnter(this GameObject go, Action<Collider> callback) => go.PhysicsEvents().OnTriggerEnter(callback);
        public static PhysicsEvents OnTriggerExit(this GameObject go, Action<Collider> callback) => go.PhysicsEvents().OnTriggerExit(callback);
        public static PhysicsEvents OnTriggerStay(this GameObject go, Action<Collider> callback) => go.PhysicsEvents().OnTriggerStay(callback);
        public static PhysicsEvents OnCollisionEnter(this GameObject go, Action<Collision> callback) => go.PhysicsEvents().OnCollisionEnter(callback);
        public static PhysicsEvents OnCollisionExit(this GameObject go, Action<Collision> callback) => go.PhysicsEvents().OnCollisionExit(callback);
        public static PhysicsEvents OnCollisionStay(this GameObject go, Action<Collision> callback) => go.PhysicsEvents().OnCollisionStay(callback);
        public static PhysicsEvents OnTriggerEnterTag(this GameObject go, string tag, Action<Collider> callback) => go.PhysicsEvents().OnTriggerEnterTag(tag, callback);

        public static PhysicsEvents2D PhysicsEvents2D(this GameObject go)
        {
            PhysicsEvents2D events = go.GetComponent<PhysicsEvents2D>();
            return events != null ? events : go.AddComponent<PhysicsEvents2D>();
        }

        public static PhysicsEvents2D PhysicsEvents2D(this Component component) => component.gameObject.PhysicsEvents2D();

        public static PhysicsEvents2D OnCollisionEnter2D(this PhysicsEvents2D e, Action<Collision2D> callback) { if (callback != null) e.CollisionEnter += callback; return e; }
        public static PhysicsEvents2D OnCollisionStay2D(this PhysicsEvents2D e, Action<Collision2D> callback) { if (callback != null) e.CollisionStay += callback; return e; }
        public static PhysicsEvents2D OnCollisionExit2D(this PhysicsEvents2D e, Action<Collision2D> callback) { if (callback != null) e.CollisionExit += callback; return e; }
        public static PhysicsEvents2D OnTriggerEnter2D(this PhysicsEvents2D e, Action<Collider2D> callback) { if (callback != null) e.TriggerEnter += callback; return e; }
        public static PhysicsEvents2D OnTriggerStay2D(this PhysicsEvents2D e, Action<Collider2D> callback) { if (callback != null) e.TriggerStay += callback; return e; }
        public static PhysicsEvents2D OnTriggerExit2D(this PhysicsEvents2D e, Action<Collider2D> callback) { if (callback != null) e.TriggerExit += callback; return e; }

        public static PhysicsEvents2D OnTriggerEnterTag2D(this PhysicsEvents2D e, string tag, Action<Collider2D> callback)
            => e.OnTriggerEnter2D(other => { if (other.CompareTag(tag)) callback(other); });

        public static PhysicsEvents2D OnTriggerEnter2D(this GameObject go, Action<Collider2D> callback) => go.PhysicsEvents2D().OnTriggerEnter2D(callback);
        public static PhysicsEvents2D OnTriggerExit2D(this GameObject go, Action<Collider2D> callback) => go.PhysicsEvents2D().OnTriggerExit2D(callback);
        public static PhysicsEvents2D OnCollisionEnter2D(this GameObject go, Action<Collision2D> callback) => go.PhysicsEvents2D().OnCollisionEnter2D(callback);
        public static PhysicsEvents2D OnCollisionExit2D(this GameObject go, Action<Collision2D> callback) => go.PhysicsEvents2D().OnCollisionExit2D(callback);
    }
}
