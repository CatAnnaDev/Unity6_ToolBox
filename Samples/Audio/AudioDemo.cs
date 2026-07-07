using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using CatAnnaDev.Audio;

namespace CatAnnaDev.Samples
{
    public sealed class AudioDemo : MonoBehaviour
    {
        private const int SampleRate = 44100;

        private SoundData beep2D;
        private SoundData zap3D;
        private SoundData limitedSound;
        private AudioClip musicTrackA;
        private AudioClip musicTrackB;

        private readonly List<AudioClip> generatedClips = new List<AudioClip>();
        private readonly List<SoundEmitter> tracked = new List<SoundEmitter>();
        private readonly List<Marker> markers = new List<Marker>();

        private GameObject createdListener;
        private bool musicOnTrackA = true;
        private string lastAction = "nothing yet";
        private Rect windowRect = new Rect(12f, 12f, 430f, 470f);
        private GUIStyle boxStyle;

        private void Awake()
        {
            EnsureAudioListener();

            beep2D = BuildSoundData(
                "Beep2D",
                GenerateTone("beep", 440f, 0.15f, false),
                spatialBlend: 0f,
                maxConcurrent: 8,
                cooldown: 0f);

            zap3D = BuildSoundData(
                "Zap3D",
                GenerateTone("zap", 220f, 0.22f, true),
                spatialBlend: 1f,
                maxConcurrent: 8,
                cooldown: 0f,
                minDistance: 2f,
                maxDistance: 30f);

            limitedSound = BuildSoundData(
                "LimitedDrone",
                GenerateTone("drone", 700f, 0.9f, false),
                spatialBlend: 0f,
                maxConcurrent: 3,
                cooldown: 0f);

            musicTrackA = GenerateTone("musicA", 330f, 2f, false, loopSmooth: true);
            musicTrackB = GenerateTone("musicB", 494f, 2f, false, loopSmooth: true);
            generatedClips.Add(musicTrackA);
            generatedClips.Add(musicTrackB);
        }

        private void Update()
        {
            PruneTracked();
            UpdateMarkers();

            if (DemoInput.GetKeyDown(KeyCode.Alpha1))
            {
                PlayBeep2D();
            }

            if (DemoInput.GetKeyDown(KeyCode.Alpha2))
            {
                PlayZap3D();
            }

            if (DemoInput.GetKeyDown(KeyCode.Alpha3))
            {
                SpamLimited();
            }

            if (DemoInput.GetKeyDown(KeyCode.Alpha4))
            {
                AudioManager.Instance.StopAllSounds();
                lastAction = "StopAllSounds()";
            }

            if (DemoInput.GetKeyDown(KeyCode.M))
            {
                ToggleMusic();
            }

            if (DemoInput.GetKeyDown(KeyCode.N))
            {
                AudioManager.Instance.StopMusic(1.5f);
                lastAction = "StopMusic(1.5)";
            }

            if (DemoInput.GetKeyDown(KeyCode.D))
            {
                AudioManager.Instance.DuckMusic(0.7f, 1f);
                lastAction = "DuckMusic(0.7, 1.0)";
            }
        }

        private void PlayBeep2D()
        {
            SoundEmitter emitter = AudioManager.Instance.PlaySound2D(beep2D);
            Track(emitter);
            lastAction = "PlaySound2D(beep) flat stereo";
        }

        private void PlayZap3D()
        {
            Vector3 position = new Vector3(Random.Range(-8f, 8f), 0f, Random.Range(-2f, 12f));
            SoundEmitter emitter = AudioManager.Instance.PlaySoundAtPosition(zap3D, position);
            Track(emitter);
            SpawnMarker(position);
            lastAction = "PlaySoundAtPosition(zap) at " + position.ToString("F1");
        }

        private void SpamLimited()
        {
            for (int i = 0; i < 6; i++)
            {
                Track(AudioManager.Instance.PlaySound(limitedSound));
            }

            lastAction = "Fired 6x LimitedDrone (MaxConcurrent = 3)";
        }

        private void ToggleMusic()
        {
            AudioClip next = musicOnTrackA ? musicTrackB : musicTrackA;
            AudioManager.Instance.CrossfadeMusic(next, 2f);
            musicOnTrackA = !musicOnTrackA;
            lastAction = "CrossfadeMusic(" + (musicOnTrackA ? "A" : "B") + ", 2.0)";
        }

        private void Track(SoundEmitter emitter)
        {
            if (emitter != null)
            {
                tracked.Add(emitter);
            }
        }

        private void PruneTracked()
        {
            for (int i = tracked.Count - 1; i >= 0; i--)
            {
                if (tracked[i] == null || !tracked[i].IsPlaying)
                {
                    tracked.RemoveAt(i);
                }
            }
        }

        private int CountPlaying(SoundData data)
        {
            int count = 0;
            for (int i = 0; i < tracked.Count; i++)
            {
                if (tracked[i] != null && tracked[i].IsPlaying && tracked[i].ActiveData == data)
                {
                    count++;
                }
            }

            return count;
        }

        private void OnGUI()
        {
            if (boxStyle == null)
            {
                boxStyle = new GUIStyle(GUI.skin.box);
                boxStyle.fontSize = 12;
                boxStyle.alignment = TextAnchor.UpperLeft;
                boxStyle.wordWrap = true;
            }

            windowRect = GUILayout.Window(GetEntityId().GetHashCode(), windowRect, DrawWindow, "CatAnnaDev  -  Audio Demo");
        }

        private void DrawWindow(int id)
        {
            AudioManager audio = AudioManager.Instance;

            GUILayout.Label(
                "AudioManager plays pooled SoundData through voice-limited emitters. " +
                "All clips below are generated procedurally in code (sine / square waves) - no audio assets.",
                boxStyle);

            GUILayout.Space(6f);
            GUILayout.Label("CONTROLS", boxStyle);
            GUILayout.Label(
                "[1] 2D beep (PlaySound2D)\n" +
                "[2] 3D zap at random spot (PlaySoundAtPosition)\n" +
                "[3] Fire drone 6x  ->  capped at MaxConcurrent = 3\n" +
                "[4] StopAllSounds\n" +
                "[M] Crossfade music A <-> B\n" +
                "[N] Fade out music     [D] Duck music",
                boxStyle);

            GUILayout.Space(6f);
            if (GUILayout.Button("1  -  Play 2D beep"))
            {
                PlayBeep2D();
            }

            if (GUILayout.Button("2  -  Play 3D zap"))
            {
                PlayZap3D();
            }

            if (GUILayout.Button("3  -  Spam limited drone (x6)"))
            {
                SpamLimited();
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("4  -  Stop all SFX"))
            {
                audio.StopAllSounds();
                lastAction = "StopAllSounds()";
            }

            if (GUILayout.Button("M  -  Crossfade music"))
            {
                ToggleMusic();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("N  -  Stop music"))
            {
                audio.StopMusic(1.5f);
                lastAction = "StopMusic(1.5)";
            }

            if (GUILayout.Button("D  -  Duck music"))
            {
                audio.DuckMusic(0.7f, 1f);
                lastAction = "DuckMusic(0.7, 1.0)";
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(6f);
            GUILayout.Label("Master  " + audio.MasterVolume.ToString("F2"), boxStyle);
            audio.MasterVolume = GUILayout.HorizontalSlider(audio.MasterVolume, 0f, 1f);
            GUILayout.Label("Music   " + audio.MusicVolume.ToString("F2"), boxStyle);
            audio.MusicVolume = GUILayout.HorizontalSlider(audio.MusicVolume, 0f, 1f);
            GUILayout.Label("SFX     " + audio.SfxVolume.ToString("F2"), boxStyle);
            audio.SfxVolume = GUILayout.HorizontalSlider(audio.SfxVolume, 0f, 1f);

            GUILayout.Space(6f);
            GUILayout.Label("STATUS", boxStyle);
            GUILayout.Label(
                "Music playing : " + audio.IsMusicPlaying + "  (next toggle -> " + (musicOnTrackA ? "B" : "A") + ")\n" +
                "Active SFX voices : " + tracked.Count + "\n" +
                "Drone voices : " + CountPlaying(limitedSound) + " / " + limitedSound.MaxConcurrent + "\n" +
                "Last action : " + lastAction,
                boxStyle);

            GUI.DragWindow(new Rect(0f, 0f, 10000f, 24f));
        }

        private SoundData BuildSoundData(
            string dataName,
            AudioClip clip,
            float spatialBlend,
            int maxConcurrent,
            float cooldown,
            float minDistance = 1f,
            float maxDistance = 500f)
        {
            SoundData data = ScriptableObject.CreateInstance<SoundData>();
            data.name = dataName;

            SetPrivate(data, "clips", new[] { clip });
            SetPrivate(data, "spatialBlend", spatialBlend);
            SetPrivate(data, "maxConcurrent", maxConcurrent);
            SetPrivate(data, "cooldown", cooldown);
            SetPrivate(data, "minDistance", minDistance);
            SetPrivate(data, "maxDistance", maxDistance);
            SetPrivate(data, "volumeRange", new Vector2(0.9f, 1f));
            SetPrivate(data, "pitchRange", new Vector2(0.95f, 1.05f));
            SetPrivate(data, "loop", false);

            generatedClips.Add(clip);
            return data;
        }

        private static void SetPrivate(SoundData target, string fieldName, object value)
        {
            FieldInfo field = typeof(SoundData).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            if (field != null)
            {
                field.SetValue(target, value);
            }
        }

        private static AudioClip GenerateTone(string clipName, float frequency, float seconds, bool square, bool loopSmooth = false)
        {
            int total = Mathf.Max(1, Mathf.RoundToInt(SampleRate * seconds));
            float[] samples = new float[total];
            float step = frequency / SampleRate;
            float phase = 0f;

            for (int i = 0; i < total; i++)
            {
                float wave = Mathf.Sin(phase * 2f * Mathf.PI);
                if (square)
                {
                    wave = wave >= 0f ? 1f : -1f;
                }

                float envelope = loopSmooth ? 1f : ComputeEnvelope(i, total);
                samples[i] = wave * 0.35f * envelope;

                phase += step;
                if (phase >= 1f)
                {
                    phase -= 1f;
                }
            }

            AudioClip clip = AudioClip.Create(clipName, total, 1, SampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private static float ComputeEnvelope(int index, int total)
        {
            int fade = Mathf.Min(1024, total / 4);
            if (fade <= 0)
            {
                return 1f;
            }

            if (index < fade)
            {
                return index / (float)fade;
            }

            if (index > total - fade)
            {
                return (total - index) / (float)fade;
            }

            return 1f;
        }

        private void EnsureAudioListener()
        {
#if UNITY_2023_1_OR_NEWER
            AudioListener existing = FindFirstObjectByType<AudioListener>();
#else
            AudioListener existing = FindObjectOfType<AudioListener>();
#endif
            if (existing != null)
            {
                return;
            }

            createdListener = new GameObject("CatAnnaDev.DemoListener");
            createdListener.AddComponent<AudioListener>();
            createdListener.transform.position = Vector3.zero;
        }

        private void SpawnMarker(Vector3 position)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.name = "CatAnnaDev.SfxMarker";
            sphere.transform.position = position;
            sphere.transform.localScale = Vector3.one * 0.6f;

            Collider collider = sphere.GetComponent<Collider>();
            if (collider != null)
            {
                Destroy(collider);
            }

            markers.Add(new Marker { Go = sphere, ExpireTime = Time.time + 0.6f });
        }

        private void UpdateMarkers()
        {
            for (int i = markers.Count - 1; i >= 0; i--)
            {
                if (Time.time >= markers[i].ExpireTime)
                {
                    if (markers[i].Go != null)
                    {
                        Destroy(markers[i].Go);
                    }

                    markers.RemoveAt(i);
                }
            }
        }

        private void OnDisable()
        {
            if (AudioManager.HasInstance)
            {
                AudioManager.Instance.StopAllSounds();
                AudioManager.Instance.StopMusic(0f);
            }

            for (int i = 0; i < markers.Count; i++)
            {
                if (markers[i].Go != null)
                {
                    Destroy(markers[i].Go);
                }
            }

            markers.Clear();

            for (int i = 0; i < generatedClips.Count; i++)
            {
                if (generatedClips[i] != null)
                {
                    Destroy(generatedClips[i]);
                }
            }

            generatedClips.Clear();

            if (createdListener != null)
            {
                Destroy(createdListener);
                createdListener = null;
            }
        }

        private struct Marker
        {
            public GameObject Go;
            public float ExpireTime;
        }
    }
}
