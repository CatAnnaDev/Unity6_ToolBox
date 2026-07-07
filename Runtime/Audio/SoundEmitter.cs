using System;
using UnityEngine;
using CatAnnaDev.Pooling;

namespace CatAnnaDev.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class SoundEmitter : MonoBehaviour, IPoolable
    {
        [SerializeField] private AudioSource source;

        private SoundData activeData;
        private AudioChannel activeChannel;
        private Transform followTarget;
        private float dataVolume = 1f;
        private float externalScale = 1f;
        private bool playing;
        private bool autoDespawn;

        public event Action<SoundEmitter> Finished;

        public SoundData ActiveData
        {
            get { return activeData; }
        }

        public bool IsPlaying
        {
            get { return playing; }
        }

        public AudioSource Source
        {
            get { return source; }
        }

        public void OnSpawn()
        {
            EnsureSource();
            ResetSource();
            playing = false;
            autoDespawn = false;
            followTarget = null;
            externalScale = 1f;
            dataVolume = 1f;
        }

        public void OnDespawn()
        {
            DetachChannel();
            if (source != null)
            {
                source.Stop();
                source.clip = null;
                source.outputAudioMixerGroup = null;
            }

            activeData = null;
            followTarget = null;
            playing = false;
            autoDespawn = false;
        }

        public void Play(SoundData data, Vector3 position, float volumeScale, Transform follow = null)
        {
            if (data == null)
            {
                return;
            }

            EnsureSource();

            AudioClip clip = data.NextClip();
            if (clip == null)
            {
                playing = false;
                RaiseFinished();
                return;
            }

            activeData = data;
            externalScale = volumeScale;
            followTarget = follow;
            dataVolume = data.NextVolume();

            transform.position = follow != null ? follow.position : position;

            AttachChannel(data.Channel);

            source.clip = clip;
            source.loop = data.Loop;
            source.pitch = data.NextPitch();
            source.priority = data.Priority;
            source.outputAudioMixerGroup = data.Output;
            source.spatialBlend = data.SpatialBlend;
            source.rolloffMode = data.RolloffMode;
            source.minDistance = data.MinDistance;
            source.maxDistance = data.MaxDistance;
            source.volume = ComputeVolume();

            autoDespawn = !data.Loop;
            playing = true;
            source.Play();
        }

        public void SetExternalScale(float volumeScale)
        {
            externalScale = volumeScale;
            ApplyVolume();
        }

        public void SetFollowTarget(Transform target)
        {
            followTarget = target;
        }

        public void Stop()
        {
            if (source != null)
            {
                source.Stop();
            }

            if (playing)
            {
                playing = false;
                RaiseFinished();
            }
        }

        public void FadeOutAndStop(float duration)
        {
            if (!playing || duration <= 0f)
            {
                Stop();
                return;
            }

            StopAllCoroutines();
            StartCoroutine(FadeOutRoutine(duration));
        }

        private void LateUpdate()
        {
            if (!playing)
            {
                return;
            }

            if (followTarget != null)
            {
                transform.position = followTarget.position;
            }

            if (autoDespawn && !source.isPlaying)
            {
                playing = false;
                RaiseFinished();
            }
        }

        private System.Collections.IEnumerator FadeOutRoutine(float duration)
        {
            float startVolume = source.volume;
            float elapsed = 0f;
            while (elapsed < duration && source.isPlaying)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                source.volume = Mathf.Lerp(startVolume, 0f, t);
                yield return null;
            }

            Stop();
        }

        private void AttachChannel(AudioChannel channel)
        {
            if (activeChannel == channel)
            {
                return;
            }

            DetachChannel();
            activeChannel = channel;
            if (activeChannel != null)
            {
                activeChannel.VolumeChanged += OnChannelVolumeChanged;
            }
        }

        private void DetachChannel()
        {
            if (activeChannel != null)
            {
                activeChannel.VolumeChanged -= OnChannelVolumeChanged;
                activeChannel = null;
            }
        }

        private void OnChannelVolumeChanged(AudioChannel channel)
        {
            ApplyVolume();
        }

        private void ApplyVolume()
        {
            if (source != null && playing)
            {
                source.volume = ComputeVolume();
            }
        }

        private float ComputeVolume()
        {
            float channelVolume = activeChannel != null ? activeChannel.Volume : 1f;
            return Mathf.Clamp01(dataVolume * externalScale * channelVolume);
        }

        private void EnsureSource()
        {
            if (source == null)
            {
                source = GetComponent<AudioSource>();
                if (source == null)
                {
                    source = gameObject.AddComponent<AudioSource>();
                }
            }
        }

        private void ResetSource()
        {
            source.playOnAwake = false;
            source.loop = false;
            source.clip = null;
            source.volume = 1f;
            source.pitch = 1f;
            source.spatialBlend = 0f;
            source.outputAudioMixerGroup = null;
        }

        private void RaiseFinished()
        {
            Action<SoundEmitter> handler = Finished;
            if (handler != null)
            {
                handler(this);
            }
        }
    }
}
