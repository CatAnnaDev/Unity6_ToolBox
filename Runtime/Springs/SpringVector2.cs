using UnityEngine;

namespace CatAnnaDev.Springs
{
    public sealed class SpringVector2
    {
        private const float MaxStep = 1f / 120f;

        public float Stiffness;
        public float Damping;
        public Vector2 Value;
        public Vector2 Velocity;

        public SpringVector2(float stiffness = 120f, float damping = 14f, Vector2 value = default)
        {
            Stiffness = stiffness;
            Damping = damping;
            Value = value;
            Velocity = Vector2.zero;
        }

        public void Reset(Vector2 value)
        {
            Value = value;
            Velocity = Vector2.zero;
        }

        public Vector2 Update(Vector2 target, float deltaTime)
        {
            if (deltaTime <= 0f) return Value;

            int steps = Mathf.Clamp(Mathf.CeilToInt(deltaTime / MaxStep), 1, 16);
            float sdt = deltaTime / steps;
            for (int i = 0; i < steps; i++)
            {
                Vector2 force = Stiffness * (target - Value) - Damping * Velocity;
                Velocity += force * sdt;
                Value += Velocity * sdt;
            }

            return Value;
        }
    }
}
