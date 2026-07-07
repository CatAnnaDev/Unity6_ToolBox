using UnityEngine;
using UnityEngine.Audio;
using CatAnnaDev.Utils;

namespace CatAnnaDev.Audio
{
    [CreateAssetMenu(fileName = "SoundData", menuName = "CatAnnaDev/Audio/Sound Data")]
    public sealed class SoundData : ScriptableObject
    {
        [Title("Clips")]
        [SerializeField] private AudioClip[] clips = new AudioClip[0];
        [SerializeField] private bool avoidRepeatLastClip = true;

        [Title("Randomization")]
        [MinMaxSlider(0f, 1f)] [SerializeField] private Vector2 volumeRange = new Vector2(1f, 1f);
        [MinMaxSlider(-3f, 3f)] [SerializeField] private Vector2 pitchRange = new Vector2(1f, 1f);

        [Title("Playback")]
        [SerializeField] private bool loop;
        [SerializeField] private AudioMixerGroup output;
        [SerializeField] private AudioChannel channel;
        [Range(0, 256)] [SerializeField] private int priority = 128;

        [Title("Spatialization")]
        [Range(0f, 1f)] [SerializeField] private float spatialBlend;
        [SerializeField] private AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic;
        [SerializeField] private float minDistance = 1f;
        [SerializeField] private float maxDistance = 500f;

        [Title("Voice Limiting")]
        [SerializeField] private int maxConcurrent = 8;
        [SerializeField] private float cooldown;

        [System.NonSerialized] private int lastClipIndex = -1;

        public bool Loop
        {
            get { return loop; }
        }

        public AudioMixerGroup Output
        {
            get { return output; }
        }

        public AudioChannel Channel
        {
            get { return channel; }
        }

        public int Priority
        {
            get { return priority; }
        }

        public float SpatialBlend
        {
            get { return spatialBlend; }
        }

        public AudioRolloffMode RolloffMode
        {
            get { return rolloffMode; }
        }

        public float MinDistance
        {
            get { return minDistance; }
        }

        public float MaxDistance
        {
            get { return maxDistance; }
        }

        public int MaxConcurrent
        {
            get { return maxConcurrent < 1 ? 1 : maxConcurrent; }
        }

        public float Cooldown
        {
            get { return cooldown; }
        }

        public bool HasClips
        {
            get { return clips != null && clips.Length > 0; }
        }

        public AudioClip NextClip()
        {
            if (!HasClips)
            {
                return null;
            }

            if (clips.Length == 1)
            {
                lastClipIndex = 0;
                return clips[0];
            }

            int index = Random.Range(0, clips.Length);
            if (avoidRepeatLastClip && index == lastClipIndex)
            {
                index = (index + 1) % clips.Length;
            }

            lastClipIndex = index;
            AudioClip picked = clips[index];
            if (picked == null)
            {
                return FindFirstNonNullClip();
            }

            return picked;
        }

        public float NextVolume()
        {
            float min = Mathf.Min(volumeRange.x, volumeRange.y);
            float max = Mathf.Max(volumeRange.x, volumeRange.y);
            return Mathf.Clamp01(Random.Range(min, max));
        }

        public float NextPitch()
        {
            float min = Mathf.Min(pitchRange.x, pitchRange.y);
            float max = Mathf.Max(pitchRange.x, pitchRange.y);
            return Random.Range(min, max);
        }

        private AudioClip FindFirstNonNullClip()
        {
            for (int i = 0; i < clips.Length; i++)
            {
                if (clips[i] != null)
                {
                    lastClipIndex = i;
                    return clips[i];
                }
            }

            return null;
        }

        private void OnValidate()
        {
            if (minDistance < 0f)
            {
                minDistance = 0f;
            }

            if (maxDistance < minDistance)
            {
                maxDistance = minDistance;
            }

            if (maxConcurrent < 1)
            {
                maxConcurrent = 1;
            }

            if (cooldown < 0f)
            {
                cooldown = 0f;
            }
        }
    }
}
