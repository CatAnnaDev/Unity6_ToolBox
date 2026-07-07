using UnityEngine;
using CatAnnaDev;
using CatAnnaDev.Pooling;

namespace CatAnnaDev.VFX
{
    public enum VFXReturnMode
    {
        Auto,
        StopCallback,
        PollIsAlive,
        FixedLifetime,
        Manual
    }

    [DisallowMultipleComponent]
    public sealed class PooledParticle : MonoBehaviour, IPoolable
    {
        [SerializeField] private VFXReturnMode returnMode = VFXReturnMode.Auto;
        [SerializeField] private float pollInterval = 0.1f;
        [SerializeField] private float maxLifetime = 0f;
        [SerializeField] private bool restartOnSpawn = true;
        [SerializeField] private bool clearOnDespawn = true;

        private ParticleSystem[] systems;
        private ParticleSystem primary;
        private bool cached;
        private bool warnedNoSystems;

        private VFXReturnMode activeMode;
        private bool monitoring;
        private bool returning;
        private float elapsed;
        private float pollTimer;
        private float capTime;

        public bool IsPlaying
        {
            get { return monitoring && !returning; }
        }

        public ParticleSystem PrimarySystem
        {
            get
            {
                EnsureCached();
                return primary;
            }
        }

        public void SetLifetime(float seconds)
        {
            maxLifetime = seconds < 0f ? 0f : seconds;
        }

        public void SetReturnMode(VFXReturnMode mode)
        {
            returnMode = mode;
        }

        private void Awake()
        {
            EnsureCached();
        }

        public void OnSpawn()
        {
            returning = false;
            elapsed = 0f;
            pollTimer = 0f;
            EnsureCached();

            if (systems == null || systems.Length == 0)
            {
                if (!warnedNoSystems)
                {
                    warnedNoSystems = true;
                    CatLog.Warn("PooledParticle has no ParticleSystem in its hierarchy: " + name, this);
                }
                activeMode = VFXReturnMode.Manual;
                monitoring = false;
                return;
            }

            if (restartOnSpawn)
            {
                PlayAll();
            }

            activeMode = ResolveMode();
            ConfigureStopAction();
            capTime = ResolveCapTime();
            monitoring = activeMode != VFXReturnMode.Manual;
        }

        public void OnDespawn()
        {
            monitoring = false;
            returning = false;
            if (clearOnDespawn)
            {
                StopAndClear();
            }
        }

        private void Update()
        {
            if (!monitoring)
            {
                return;
            }

            elapsed += Time.deltaTime;

            if (capTime > 0f && elapsed >= capTime)
            {
                ReturnToPool();
                return;
            }

            if (activeMode == VFXReturnMode.PollIsAlive)
            {
                pollTimer += Time.deltaTime;
                if (pollTimer >= pollInterval)
                {
                    pollTimer = 0f;
                    if (!AnyAlive())
                    {
                        ReturnToPool();
                    }
                }
            }
        }

        private void OnParticleSystemStopped()
        {
            if (activeMode == VFXReturnMode.StopCallback)
            {
                ReturnToPool();
            }
        }

        public void ReturnToPool()
        {
            if (returning)
            {
                return;
            }

            returning = true;
            monitoring = false;
            Pool.Despawn(gameObject);
        }

        private void EnsureCached()
        {
            if (cached)
            {
                return;
            }

            systems = GetComponentsInChildren<ParticleSystem>(true);
            primary = SelectPrimary(systems);
            cached = true;
        }

        private ParticleSystem SelectPrimary(ParticleSystem[] all)
        {
            if (all == null || all.Length == 0)
            {
                return null;
            }

            ParticleSystem local = GetComponent<ParticleSystem>();
            if (local != null)
            {
                return local;
            }

            ParticleSystem best = all[0];
            float bestDuration = TotalDuration(best);
            for (int i = 1; i < all.Length; i++)
            {
                float d = TotalDuration(all[i]);
                if (d > bestDuration)
                {
                    bestDuration = d;
                    best = all[i];
                }
            }

            return best;
        }

        private VFXReturnMode ResolveMode()
        {
            if (returnMode != VFXReturnMode.Auto)
            {
                return returnMode;
            }

            if (primary == null)
            {
                return VFXReturnMode.Manual;
            }

            if (primary.main.loop)
            {
                return maxLifetime > 0f ? VFXReturnMode.FixedLifetime : VFXReturnMode.Manual;
            }

            if (primary.gameObject == gameObject)
            {
                return VFXReturnMode.StopCallback;
            }

            return VFXReturnMode.PollIsAlive;
        }

        private float ResolveCapTime()
        {
            if (maxLifetime > 0f)
            {
                return maxLifetime;
            }

            if (activeMode == VFXReturnMode.FixedLifetime && primary != null)
            {
                return TotalDuration(primary);
            }

            return 0f;
        }

        private void ConfigureStopAction()
        {
            if (activeMode != VFXReturnMode.StopCallback || primary == null)
            {
                return;
            }

            ParticleSystem.MainModule main = primary.main;
            main.stopAction = ParticleSystemStopAction.Callback;
        }

        private void PlayAll()
        {
            for (int i = 0; i < systems.Length; i++)
            {
                ParticleSystem s = systems[i];
                if (s == null)
                {
                    continue;
                }

                s.Clear(false);
                s.Play(false);
            }
        }

        private void StopAndClear()
        {
            if (systems == null)
            {
                return;
            }

            for (int i = 0; i < systems.Length; i++)
            {
                ParticleSystem s = systems[i];
                if (s == null)
                {
                    continue;
                }

                s.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }

        private bool AnyAlive()
        {
            for (int i = 0; i < systems.Length; i++)
            {
                ParticleSystem s = systems[i];
                if (s != null && s.IsAlive(false))
                {
                    return true;
                }
            }

            return false;
        }

        private static float TotalDuration(ParticleSystem system)
        {
            if (system == null)
            {
                return 0f;
            }

            ParticleSystem.MainModule main = system.main;
            return main.duration + main.startLifetime.constantMax + main.startDelay.constantMax;
        }
    }
}
