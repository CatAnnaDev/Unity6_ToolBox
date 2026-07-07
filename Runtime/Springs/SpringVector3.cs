using UnityEngine;

namespace CatAnnaDev.Springs
{
    public sealed class SpringVector3
    {
        private const float MaxStep = 1f / 120f;

        public float Stiffness;
        public float Damping;
        public Vector3 Value;
        public Vector3 Velocity;

        public SpringVector3(float stiffness = 120f, float damping = 14f, Vector3 value = default)
        {
            Stiffness = stiffness;
            Damping = damping;
            Value = value;
            Velocity = Vector3.zero;
        }

        public void Reset(Vector3 value)
        {
            Value = value;
            Velocity = Vector3.zero;
        }

        public Vector3 Update(Vector3 target, float deltaTime)
        {
            if (deltaTime <= 0f) return Value;

            int steps = Mathf.Clamp(Mathf.CeilToInt(deltaTime / MaxStep), 1, 16);
            float sdt = deltaTime / steps;
            for (int i = 0; i < steps; i++)
            {
                Vector3 force = Stiffness * (target - Value) - Damping * Velocity;
                Velocity += force * sdt;
                Value += Velocity * sdt;
            }

            return Value;
        }
    }
}
