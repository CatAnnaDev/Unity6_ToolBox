using UnityEngine;
using CatAnnaDev;
using CatAnnaDev.Pooling;
using CatAnnaDev.Services;

namespace CatAnnaDev.VFX
{
    public sealed class VFXManager : PersistentSingleton<VFXManager>
    {
        public GameObject Play(VFXData data, Vector3 position, Quaternion rotation)
        {
            if (!ValidateData(data))
            {
                return null;
            }

            return SpawnConfigured(data, position, rotation, null, false);
        }

        public GameObject Play(VFXData data, Vector3 position, Vector3 surfaceNormal)
        {
            if (!ValidateData(data))
            {
                return null;
            }

            Quaternion rotation = data.AlignToNormal ? NormalRotation(surfaceNormal) : Quaternion.identity;
            return SpawnConfigured(data, position, rotation, null, false);
        }

        public GameObject Play(VFXData data, Transform target)
        {
            if (!ValidateData(data))
            {
                return null;
            }

            if (target == null)
            {
                CatLog.Warn("VFXManager.Play received a null target.", this);
                return null;
            }

            if (data.FollowTarget)
            {
                return SpawnConfigured(data, target.position, target.rotation, target, true);
            }

            return SpawnConfigured(data, target.position, target.rotation, null, false);
        }

        public GameObject PlayAttached(VFXData data, Transform parent)
        {
            if (!ValidateData(data))
            {
                return null;
            }

            if (parent == null)
            {
                CatLog.Warn("VFXManager.PlayAttached received a null parent.", this);
                return null;
            }

            return SpawnConfigured(data, parent.position, parent.rotation, parent, true);
        }

        public GameObject Play(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            return Play(prefab, position, rotation, 1f);
        }

        public GameObject Play(GameObject prefab, Vector3 position, Quaternion rotation, float scale)
        {
            if (prefab == null)
            {
                CatLog.Error("VFXManager.Play received a null prefab.", this);
                return null;
            }

            GameObject instance = Pool.Spawn(prefab, position, rotation, null);
            ApplyScale(instance, scale);
            return instance;
        }

        public GameObject PlayAttached(GameObject prefab, Transform parent)
        {
            return PlayAttached(prefab, parent, 1f);
        }

        public GameObject PlayAttached(GameObject prefab, Transform parent, float scale)
        {
            if (prefab == null)
            {
                CatLog.Error("VFXManager.PlayAttached received a null prefab.", this);
                return null;
            }

            if (parent == null)
            {
                CatLog.Warn("VFXManager.PlayAttached received a null parent.", this);
                return null;
            }

            GameObject instance = Pool.Spawn(prefab, parent.position, parent.rotation, parent);
            ApplyScale(instance, scale);
            return instance;
        }

        public GameObject PlayOneShot(GameObject prefab, Vector3 position, Quaternion rotation, float lifetime)
        {
            return PlayOneShot(prefab, position, rotation, lifetime, 1f);
        }

        public GameObject PlayOneShot(GameObject prefab, Vector3 position, Quaternion rotation, float lifetime, float scale)
        {
            if (prefab == null)
            {
                CatLog.Error("VFXManager.PlayOneShot received a null prefab.", this);
                return null;
            }

            GameObject instance = Pool.Spawn(prefab, position, rotation, null);
            ApplyScale(instance, scale);

            if (lifetime > 0f)
            {
                Pool.Despawn(instance, lifetime);
            }

            return instance;
        }

        public GameObject PlayDecal(GameObject prefab, Vector3 position, Vector3 surfaceNormal, float lifetime)
        {
            return PlayDecal(prefab, position, surfaceNormal, lifetime, 1f);
        }

        public GameObject PlayDecal(GameObject prefab, Vector3 position, Vector3 surfaceNormal, float lifetime, float scale)
        {
            if (prefab == null)
            {
                CatLog.Error("VFXManager.PlayDecal received a null prefab.", this);
                return null;
            }

            GameObject instance = Pool.Spawn(prefab, position, NormalRotation(surfaceNormal), null);
            ApplyScale(instance, scale);

            if (lifetime > 0f)
            {
                Pool.Despawn(instance, lifetime);
            }

            return instance;
        }

        public void Stop(GameObject instance)
        {
            if (instance == null)
            {
                return;
            }

            Pool.Despawn(instance);
        }

        public void Stop(GameObject instance, float delay)
        {
            if (instance == null)
            {
                return;
            }

            Pool.Despawn(instance, delay);
        }

        public void Prewarm(VFXData data)
        {
            if (!ValidateData(data))
            {
                return;
            }

            int count = data.PrewarmCount > 0 ? data.PrewarmCount : 1;
            Pool.Prewarm(data.Prefab, count);
        }

        public void Prewarm(GameObject prefab, int count)
        {
            if (prefab == null)
            {
                CatLog.Error("VFXManager.Prewarm received a null prefab.", this);
                return;
            }

            Pool.Prewarm(prefab, count < 1 ? 1 : count);
        }

        private GameObject SpawnConfigured(VFXData data, Vector3 position, Quaternion baseRotation, Transform parent, bool attach)
        {
            Quaternion rotation = baseRotation * Quaternion.Euler(data.EulerOffset);
            if (data.RandomizeYaw)
            {
                rotation = rotation * Quaternion.Euler(0f, Random.value * 360f, 0f);
            }

            Vector3 worldPosition = position + rotation * data.PositionOffset;

            GameObject instance = attach
                ? Pool.Spawn(data.Prefab, worldPosition, rotation, parent)
                : Pool.Spawn(data.Prefab, worldPosition, rotation, null);

            ApplyScale(instance, data.Scale);

            if (instance != null && data.LifetimeOverride > 0f && instance.TryGetComponent(out PooledParticle particle))
            {
                particle.SetLifetime(data.LifetimeOverride);
            }

            return instance;
        }

        private static void ApplyScale(GameObject instance, float scale)
        {
            if (instance == null)
            {
                return;
            }

            float applied = scale <= 0f ? 1f : scale;
            instance.transform.localScale = Vector3.one * applied;
        }

        private static Quaternion NormalRotation(Vector3 normal)
        {
            if (normal.sqrMagnitude < 0.0001f)
            {
                return Quaternion.identity;
            }

            return Quaternion.FromToRotation(Vector3.up, normal.normalized);
        }

        private bool ValidateData(VFXData data)
        {
            if (data == null)
            {
                CatLog.Error("VFXManager received a null VFXData.", this);
                return false;
            }

            if (!data.IsValid)
            {
                CatLog.Error("VFXData '" + data.name + "' has no prefab assigned.", this);
                return false;
            }

            return true;
        }
    }
}
