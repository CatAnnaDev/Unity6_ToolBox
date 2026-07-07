using UnityEngine;

namespace CatAnnaDev.Utils
{
    public static class QuaternionExtensions
    {
        public static Quaternion WithEulerX(this Quaternion q, float x)
        {
            Vector3 e = q.eulerAngles;
            e.x = x;
            return Quaternion.Euler(e);
        }

        public static Quaternion WithEulerY(this Quaternion q, float y)
        {
            Vector3 e = q.eulerAngles;
            e.y = y;
            return Quaternion.Euler(e);
        }

        public static Quaternion WithEulerZ(this Quaternion q, float z)
        {
            Vector3 e = q.eulerAngles;
            e.z = z;
            return Quaternion.Euler(e);
        }

        public static Quaternion Flip(this Quaternion q)
        {
            return q * Quaternion.Euler(0f, 180f, 0f);
        }

        public static Quaternion ShortestRotationTo(this Quaternion from, Quaternion to)
        {
            if (Quaternion.Dot(from, to) < 0f)
            {
                return to * Quaternion.Inverse(Multiply(from, -1f));
            }
            return to * Quaternion.Inverse(from);
        }

        public static Quaternion SmoothDamp(this Quaternion current, Quaternion target, ref Quaternion velocity, float smoothTime, float deltaTime)
        {
            if (deltaTime < Mathf.Epsilon)
            {
                return current;
            }

            float dot = Quaternion.Dot(current, target);
            float sign = dot > 0f ? 1f : -1f;
            target = Multiply(target, sign);

            Vector4 result = new Vector4(
                Mathf.SmoothDamp(current.x, target.x, ref velocity.x, smoothTime, Mathf.Infinity, deltaTime),
                Mathf.SmoothDamp(current.y, target.y, ref velocity.y, smoothTime, Mathf.Infinity, deltaTime),
                Mathf.SmoothDamp(current.z, target.z, ref velocity.z, smoothTime, Mathf.Infinity, deltaTime),
                Mathf.SmoothDamp(current.w, target.w, ref velocity.w, smoothTime, Mathf.Infinity, deltaTime)).normalized;

            Vector4 derivativeError = new Vector4(velocity.x, velocity.y, velocity.z, velocity.w);
            derivativeError -= Vector4.Dot(result, derivativeError) * result;
            velocity = new Quaternion(derivativeError.x, derivativeError.y, derivativeError.z, derivativeError.w);

            return new Quaternion(result.x, result.y, result.z, result.w);
        }

        private static Quaternion Multiply(Quaternion q, float scalar)
        {
            return new Quaternion(q.x * scalar, q.y * scalar, q.z * scalar, q.w * scalar);
        }
    }
}
