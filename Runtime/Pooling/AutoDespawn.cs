using UnityEngine;

namespace CatAnnaDev.Pooling
{
    public enum AutoDespawnMode
    {
        Lifetime,
        ParticleSystemFinished,
        AudioSourceStopped
    }

    [DisallowMultipleComponent]
    [AddComponentMenu("CatAnnaDev/Pooling/Auto Despawn")]
    public sealed class AutoDespawn : MonoBehaviour
    {
        [SerializeField] AutoDespawnMode mode = AutoDespawnMode.Lifetime;
        [SerializeField, Min(0f)] float lifetime = 2f;
        [SerializeField] bool useUnscaledTime;
        [SerializeField, Min(0f)] float startGuard = 0.05f;

        ParticleSystem _particleSystem;
        AudioSource _audioSource;
        float _elapsed;
        bool _armed;

        public AutoDespawnMode Mode
        {
            get => mode;
            set => mode = value;
        }

        public float Lifetime
        {
            get => lifetime;
            set => lifetime = value < 0f ? 0f : value;
        }

        void Awake()
        {
            _particleSystem = GetComponentInChildren<ParticleSystem>(true);
            _audioSource = GetComponent<AudioSource>();
        }

        void OnEnable()
        {
            _elapsed = 0f;
            _armed = true;
        }

        void OnDisable()
        {
            _armed = false;
        }

        void Update()
        {
            if (!_armed) return;

            float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            _elapsed += dt;

            switch (mode)
            {
                case AutoDespawnMode.Lifetime:
                    if (_elapsed >= lifetime) Fire();
                    break;

                case AutoDespawnMode.ParticleSystemFinished:
                    if (_elapsed >= startGuard && (_particleSystem == null || !_particleSystem.IsAlive(true))) Fire();
                    break;

                case AutoDespawnMode.AudioSourceStopped:
                    if (_elapsed >= startGuard && (_audioSource == null || !_audioSource.isPlaying)) Fire();
                    break;
            }
        }

        void Fire()
        {
            _armed = false;
            Pool.Despawn(gameObject);
        }
    }
}
