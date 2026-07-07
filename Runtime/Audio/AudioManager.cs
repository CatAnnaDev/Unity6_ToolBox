using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using CatAnnaDev;
using CatAnnaDev.Pooling;
using CatAnnaDev.Services;

namespace CatAnnaDev.Audio
{
    public sealed class AudioManager : PersistentSingleton<AudioManager>
    {
        [SerializeField] private SoundEmitter emitterPrefab;
        [SerializeField] private AudioMixerGroup musicOutput;
        [SerializeField, Range(0f, 1f)] private float masterVolume = 1f;
        [SerializeField, Range(0f, 1f)] private float musicVolume = 1f;
        [SerializeField, Range(0f, 1f)] private float sfxVolume = 1f;
        [SerializeField] private int prewarmEmitters = 8;

        private readonly Dictionary<SoundData, List<SoundEmitter>> activeBySound =
            new Dictionary<SoundData, List<SoundEmitter>>();
        private readonly Dictionary<SoundData, float> lastPlayTime =
            new Dictionary<SoundData, float>();
        private readonly List<SoundEmitter> allActive = new List<SoundEmitter>();

        private MusicVoice voiceA;
        private MusicVoice voiceB;
        private MusicVoice activeVoice;
        private MusicVoice idleVoice;

        private Coroutine crossfadeRoutine;
        private Coroutine duckRoutine;
        private float duckMultiplier = 1f;

        public float MasterVolume
        {
            get { return masterVolume; }
            set
            {
                masterVolume = Mathf.Clamp01(value);
                RefreshActiveSfxScale();
            }
        }

        public float MusicVolume
        {
            get { return musicVolume; }
            set { musicVolume = Mathf.Clamp01(value); }
        }

        public float SfxVolume
        {
            get { return sfxVolume; }
            set
            {
                sfxVolume = Mathf.Clamp01(value);
                RefreshActiveSfxScale();
            }
        }

        public bool IsMusicPlaying
        {
            get { return activeVoice != null && activeVoice.Source.isPlaying; }
        }

        private float SfxScale
        {
            get { return masterVolume * sfxVolume; }
        }

        private float MusicEffectiveVolume
        {
            get { return masterVolume * musicVolume * duckMultiplier; }
        }

        protected override void OnSingletonAwake()
        {
            EnsureEmitterPrefab();
            BuildMusicVoices();

            if (prewarmEmitters > 0 && emitterPrefab != null)
            {
                Pool.Prewarm(emitterPrefab.gameObject, prewarmEmitters);
            }
        }

        public SoundEmitter PlaySound(SoundData data)
        {
            return InternalPlay(data, GetListenerPosition(), null, null);
        }

        public SoundEmitter PlaySoundAtPosition(SoundData data, Vector3 position)
        {
            return InternalPlay(data, position, null, null);
        }

        public SoundEmitter PlaySoundFollow(SoundData data, Transform target)
        {
            Vector3 position = target != null ? target.position : GetListenerPosition();
            return InternalPlay(data, position, null, target);
        }

        public SoundEmitter PlaySound2D(SoundData data)
        {
            return InternalPlay(data, GetListenerPosition(), 0f, null);
        }

        public void StopSound(SoundData data)
        {
            if (data == null)
            {
                return;
            }

            List<SoundEmitter> list;
            if (!activeBySound.TryGetValue(data, out list))
            {
                return;
            }

            for (int i = list.Count - 1; i >= 0; i--)
            {
                list[i].Stop();
            }
        }

        public void StopAllSounds()
        {
            for (int i = allActive.Count - 1; i >= 0; i--)
            {
                allActive[i].Stop();
            }
        }

        public void PlayMusic(AudioClip clip, float fade = 1f)
        {
            CrossfadeMusic(clip, fade);
        }

        public void CrossfadeMusic(AudioClip clip, float duration)
        {
            if (clip == null)
            {
                StopMusic(duration);
                return;
            }

            EnsureMusicVoices();

            if (crossfadeRoutine != null)
            {
                StopCoroutine(crossfadeRoutine);
                crossfadeRoutine = null;
            }

            MusicVoice incoming = idleVoice;
            MusicVoice outgoing = activeVoice;

            incoming.Source.clip = clip;
            incoming.Source.outputAudioMixerGroup = musicOutput;
            incoming.Source.loop = true;
            incoming.Fade = 0f;
            incoming.Source.volume = 0f;
            incoming.Source.Play();

            activeVoice = incoming;
            idleVoice = outgoing;

            crossfadeRoutine = StartCoroutine(CrossfadeRoutine(incoming, outgoing, Mathf.Max(0f, duration)));
        }

        public void StopMusic(float fade = 1f)
        {
            if (activeVoice == null)
            {
                return;
            }

            if (crossfadeRoutine != null)
            {
                StopCoroutine(crossfadeRoutine);
                crossfadeRoutine = null;
            }

            crossfadeRoutine = StartCoroutine(FadeOutMusicRoutine(activeVoice, Mathf.Max(0f, fade)));
        }

        public void DuckMusic(float amount, float duration)
        {
            EnsureMusicVoices();

            float target = Mathf.Clamp01(1f - Mathf.Clamp01(amount));
            if (duckRoutine != null)
            {
                StopCoroutine(duckRoutine);
                duckRoutine = null;
            }

            duckRoutine = StartCoroutine(DuckRoutine(target, Mathf.Max(0.01f, duration)));
        }

        private SoundEmitter InternalPlay(SoundData data, Vector3 position, float? spatialOverride, Transform follow)
        {
            if (data == null)
            {
                CatLog.Warn("AudioManager.InternalPlay called with null SoundData.", this);
                return null;
            }

            if (!data.HasClips)
            {
                CatLog.Warn("SoundData '" + data.name + "' has no clips assigned.", data);
                return null;
            }

            float now = Time.unscaledTime;
            if (data.Cooldown > 0f)
            {
                float last;
                if (lastPlayTime.TryGetValue(data, out last) && now - last < data.Cooldown)
                {
                    return null;
                }
            }

            List<SoundEmitter> list = GetActiveList(data);
            if (list.Count >= data.MaxConcurrent)
            {
                SoundEmitter oldest = list[0];
                oldest.Stop();
            }

            EnsureEmitterPrefab();
            if (emitterPrefab == null)
            {
                CatLog.Error("AudioManager could not create a SoundEmitter prefab.", this);
                return null;
            }

            SoundEmitter emitter = Pool.Spawn(emitterPrefab);
            if (emitter == null)
            {
                return null;
            }

            emitter.Finished += OnEmitterFinished;
            list.Add(emitter);
            allActive.Add(emitter);
            lastPlayTime[data] = now;

            emitter.Play(data, position, SfxScale, follow);

            if (spatialOverride.HasValue && emitter.Source != null)
            {
                emitter.Source.spatialBlend = spatialOverride.Value;
            }

            return emitter;
        }

        private void OnEmitterFinished(SoundEmitter emitter)
        {
            emitter.Finished -= OnEmitterFinished;

            SoundData data = emitter.ActiveData;
            if (data != null)
            {
                List<SoundEmitter> list;
                if (activeBySound.TryGetValue(data, out list))
                {
                    list.Remove(emitter);
                }
            }
            else
            {
                RemoveFromAllLists(emitter);
            }

            allActive.Remove(emitter);
            Pool.Despawn(emitter);
        }

        private void RemoveFromAllLists(SoundEmitter emitter)
        {
            foreach (KeyValuePair<SoundData, List<SoundEmitter>> pair in activeBySound)
            {
                if (pair.Value.Remove(emitter))
                {
                    return;
                }
            }
        }

        private List<SoundEmitter> GetActiveList(SoundData data)
        {
            List<SoundEmitter> list;
            if (!activeBySound.TryGetValue(data, out list))
            {
                list = new List<SoundEmitter>(data.MaxConcurrent);
                activeBySound[data] = list;
            }

            return list;
        }

        private void RefreshActiveSfxScale()
        {
            float scale = SfxScale;
            for (int i = 0; i < allActive.Count; i++)
            {
                allActive[i].SetExternalScale(scale);
            }
        }

        private void Update()
        {
            ApplyMusicVolume(voiceA);
            ApplyMusicVolume(voiceB);
        }

        private void ApplyMusicVolume(MusicVoice voice)
        {
            if (voice == null || voice.Source == null)
            {
                return;
            }

            voice.Source.volume = voice.Fade * MusicEffectiveVolume;
        }

        private IEnumerator CrossfadeRoutine(MusicVoice incoming, MusicVoice outgoing, float duration)
        {
            float outgoingStart = outgoing != null ? outgoing.Fade : 0f;

            if (duration <= 0f)
            {
                incoming.Fade = 1f;
                if (outgoing != null)
                {
                    outgoing.Fade = 0f;
                    outgoing.Source.Stop();
                }

                crossfadeRoutine = null;
                yield break;
            }

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                incoming.Fade = t;
                if (outgoing != null)
                {
                    outgoing.Fade = outgoingStart * (1f - t);
                }

                yield return null;
            }

            incoming.Fade = 1f;
            if (outgoing != null)
            {
                outgoing.Fade = 0f;
                outgoing.Source.Stop();
                outgoing.Source.clip = null;
            }

            crossfadeRoutine = null;
        }

        private IEnumerator FadeOutMusicRoutine(MusicVoice voice, float duration)
        {
            float start = voice.Fade;

            if (duration <= 0f)
            {
                voice.Fade = 0f;
                voice.Source.Stop();
                voice.Source.clip = null;
                crossfadeRoutine = null;
                yield break;
            }

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                voice.Fade = Mathf.Lerp(start, 0f, t);
                yield return null;
            }

            voice.Fade = 0f;
            voice.Source.Stop();
            voice.Source.clip = null;
            crossfadeRoutine = null;
        }

        private IEnumerator DuckRoutine(float target, float duration)
        {
            float attack = Mathf.Min(0.12f, duration * 0.25f);
            float start = duckMultiplier;

            float elapsed = 0f;
            while (elapsed < attack && attack > 0f)
            {
                elapsed += Time.unscaledDeltaTime;
                duckMultiplier = Mathf.Lerp(start, target, elapsed / attack);
                yield return null;
            }

            duckMultiplier = target;

            float release = Mathf.Max(0.01f, duration - attack);
            elapsed = 0f;
            while (elapsed < release)
            {
                elapsed += Time.unscaledDeltaTime;
                duckMultiplier = Mathf.Lerp(target, 1f, elapsed / release);
                yield return null;
            }

            duckMultiplier = 1f;
            duckRoutine = null;
        }

        private Vector3 GetListenerPosition()
        {
            AudioListener listener = FindListener();
            if (listener != null)
            {
                return listener.transform.position;
            }

            return Vector3.zero;
        }

        private static AudioListener FindListener()
        {
#if UNITY_2023_1_OR_NEWER
            return FindFirstObjectByType<AudioListener>();
#else
            return FindObjectOfType<AudioListener>();
#endif
        }

        private void EnsureEmitterPrefab()
        {
            if (emitterPrefab != null)
            {
                return;
            }

            GameObject template = new GameObject("CatAnnaDev.SoundEmitter");
            template.SetActive(false);
            template.transform.SetParent(transform, false);

            AudioSource src = template.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.loop = false;
            src.spatialBlend = 0f;

            emitterPrefab = template.AddComponent<SoundEmitter>();
        }

        private void BuildMusicVoices()
        {
            voiceA = CreateMusicVoice("CatAnnaDev.MusicA");
            voiceB = CreateMusicVoice("CatAnnaDev.MusicB");
            activeVoice = voiceA;
            idleVoice = voiceB;
        }

        private void EnsureMusicVoices()
        {
            if (voiceA == null || voiceB == null)
            {
                BuildMusicVoices();
            }
        }

        private MusicVoice CreateMusicVoice(string name)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(transform, false);

            AudioSource src = go.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.loop = true;
            src.spatialBlend = 0f;
            src.volume = 0f;
            src.outputAudioMixerGroup = musicOutput;

            MusicVoice voice = new MusicVoice();
            voice.Source = src;
            voice.Fade = 0f;
            return voice;
        }

        private sealed class MusicVoice
        {
            public AudioSource Source;
            public float Fade;
        }
    }
}
