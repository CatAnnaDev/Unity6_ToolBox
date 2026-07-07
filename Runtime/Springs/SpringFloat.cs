using UnityEngine;

namespace CatAnnaDev.Springs
{
    public sealed class SpringFloat
    {
        private const float MaxStep = 1f / 120f;

        public float Stiffness;
        public float Damping;
        public float Value;
        public float Velocity;

        public SpringFloat(float stiffness = 120f, float damping = 14f, float value = 0f)
        {
            Stiffness = stiffness;
            Damping = damping;
            Value = value;
            Velocity = 0f;
        }

        public void Reset(float value)
        {
            Value = value;
            Velocity = 0f;
        }

        public float Update(float target, float deltaTime)
        {
            if (deltaTime <= 0f) return Value;

            int steps = Mathf.Clamp(Mathf.CeilToInt(deltaTime / MaxStep), 1, 16);
            float sdt = deltaTime / steps;
            for (int i = 0; i < steps; i++)
            {
                float force = Stiffness * (target - Value) - Damping * Velocity;
                Velocity += force * sdt;
                Value += Velocity * sdt;
            }

            return Value;
        }
    }
}
