using UnityEngine;

namespace CatAnnaDev.Cameras
{
    [AddComponentMenu("CatAnnaDev/Camera/Camera Shaker")]
    public class CameraShaker : MonoBehaviour
    {
        [SerializeField] private float maxPositionOffset = 0.5f;
        [SerializeField] private float maxRotationOffset = 8f;
        [SerializeField] private float frequency = 22f;
        [SerializeField] private float traumaDecay = 1.4f;
        [SerializeField] private float traumaExponent = 2f;

        private float trauma;
        private float seed;
        private Vector3 baseLocalPosition;
        private Quaternion baseLocalRotation;

        public static CameraShaker Active { get; private set; }

        public float Trauma => trauma;

        private void Awake()
        {
            baseLocalPosition = transform.localPosition;
            baseLocalRotation = transform.localRotation;
            seed = Random.value * 64f;
        }

        private void OnEnable() => Active = this;

        private void OnDisable()
        {
            if (Active == this) Active = null;
            transform.localPosition = baseLocalPosition;
            transform.localRotation = baseLocalRotation;
        }

        public void AddTrauma(float amount) => trauma = Mathf.Clamp01(trauma + amount);

        public void SetTrauma(float amount) => trauma = Mathf.Clamp01(amount);

        public void SetBase(Vector3 localPosition, Quaternion localRotation)
        {
            baseLocalPosition = localPosition;
            baseLocalRotation = localRotation;
        }

        private void LateUpdate()
        {
            if (trauma <= 0f)
            {
                transform.localPosition = baseLocalPosition;
                transform.localRotation = baseLocalRotation;
                return;
            }

            float shake = Mathf.Pow(trauma, traumaExponent);
            float t = Time.unscaledTime * frequency;

            Vector3 offset = new Vector3(
                Mathf.PerlinNoise(seed, t) * 2f - 1f,
                Mathf.PerlinNoise(seed + 1f, t) * 2f - 1f,
                Mathf.PerlinNoise(seed + 2f, t) * 2f - 1f) * (maxPositionOffset * shake);

            Vector3 rotation = new Vector3(
                Mathf.PerlinNoise(seed + 3f, t) * 2f - 1f,
                Mathf.PerlinNoise(seed + 4f, t) * 2f - 1f,
                Mathf.PerlinNoise(seed + 5f, t) * 2f - 1f) * (maxRotationOffset * shake);

            transform.localPosition = baseLocalPosition + offset;
            transform.localRotation = baseLocalRotation * Quaternion.Euler(rotation);

            trauma = Mathf.Max(0f, trauma - traumaDecay * Time.unscaledDeltaTime);
        }
    }
}
