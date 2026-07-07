using System;
using UnityEngine;

namespace CatAnnaDev.Audio
{
    [CreateAssetMenu(fileName = "AudioChannel", menuName = "CatAnnaDev/Audio/Audio Channel")]
    public sealed class AudioChannel : ScriptableObject
    {
        [SerializeField] private string channelName = "Channel";
        [SerializeField, Range(0f, 1f)] private float defaultVolume = 1f;
        [SerializeField] private bool muted;

        [NonSerialized] private float runtimeVolume = 1f;
        [NonSerialized] private bool initialized;

        public event Action<AudioChannel> VolumeChanged;

        public string ChannelName
        {
            get { return channelName; }
        }

        public float Volume
        {
            get
            {
                EnsureInitialized();
                return muted ? 0f : runtimeVolume;
            }
            set
            {
                EnsureInitialized();
                float clamped = Mathf.Clamp01(value);
                if (Mathf.Approximately(clamped, runtimeVolume))
                {
                    return;
                }

                runtimeVolume = clamped;
                RaiseVolumeChanged();
            }
        }

        public bool Muted
        {
            get { return muted; }
            set
            {
                if (muted == value)
                {
                    return;
                }

                muted = value;
                RaiseVolumeChanged();
            }
        }

        public void ResetToDefault()
        {
            runtimeVolume = Mathf.Clamp01(defaultVolume);
            muted = false;
            initialized = true;
            RaiseVolumeChanged();
        }

        private void OnEnable()
        {
            initialized = false;
            EnsureInitialized();
        }

        private void EnsureInitialized()
        {
            if (initialized)
            {
                return;
            }

            runtimeVolume = Mathf.Clamp01(defaultVolume);
            initialized = true;
        }

        private void RaiseVolumeChanged()
        {
            Action<AudioChannel> handler = VolumeChanged;
            if (handler != null)
            {
                handler(this);
            }
        }
    }
}
