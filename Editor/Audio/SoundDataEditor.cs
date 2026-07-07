using System.Reflection;
using CatAnnaDev.Audio;
using UnityEditor;
using UnityEngine;

namespace CatAnnaDev.Editor
{
    [CustomEditor(typeof(SoundData))]
    public sealed class SoundDataEditor : UnityEditor.Editor
    {
        private const string RandomizePrefsKey = "CatAnnaDev.SoundDataEditor.Randomize";
        private const string PreviewSourceName = "CatAnnaDev_SoundDataPreview";

        private static MethodInfo _playPreviewMethod;
        private static MethodInfo _stopPreviewMethod;
        private static bool _reflectionResolved;

        private SoundData _data;
        private SerializedProperty _clipsProperty;
        private AudioSource _fallbackSource;
        private bool _randomizePreview;
        private bool _usingFallback;

        private void OnEnable()
        {
            _data = target as SoundData;
            _clipsProperty = serializedObject.FindProperty("clips");
            _randomizePreview = EditorPrefs.GetBool(RandomizePrefsKey, true);
        }

        private void OnDisable()
        {
            StopPreview();
            DestroyFallbackSource();
        }

        public override bool RequiresConstantRepaint()
        {
            return _usingFallback && _fallbackSource != null && _fallbackSource.isPlaying;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);

            DrawClipSummary();
            DrawRandomizeToggle();
            DrawPreviewControls();
        }

        private void DrawClipSummary()
        {
            int count = CountValidClips();
            string label = count == 1 ? "1 clip assigned" : count + " clips assigned";
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField(label, EditorStyles.miniLabel);
                if (count == 0)
                {
                    EditorGUILayout.LabelField("Assign at least one clip to preview.", EditorStyles.miniLabel);
                }
            }
        }

        private void DrawRandomizeToggle()
        {
            EditorGUI.BeginChangeCheck();
            bool value = EditorGUILayout.ToggleLeft(
                new GUIContent("Randomize Preview", "Pick a clip at random and apply the volume and pitch ranges on each preview."),
                _randomizePreview);
            if (EditorGUI.EndChangeCheck())
            {
                _randomizePreview = value;
                EditorPrefs.SetBool(RandomizePrefsKey, value);
            }
        }

        private void DrawPreviewControls()
        {
            bool hasClips = CountValidClips() > 0;
            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUI.DisabledScope(!hasClips))
                {
                    if (GUILayout.Button("Preview", GUILayout.Height(24f)))
                    {
                        Preview();
                    }
                }

                if (GUILayout.Button("Stop", GUILayout.Height(24f)))
                {
                    StopPreview();
                }
            }
        }

        private void Preview()
        {
            StopPreview();

            if (_data == null || !_data.HasClips)
            {
                return;
            }

            AudioClip clip = _randomizePreview ? _data.NextClip() : FirstValidClip();
            if (clip == null)
            {
                return;
            }

            if (_randomizePreview)
            {
                float volume = _data.NextVolume();
                float pitch = _data.NextPitch();
                PlayWithFallback(clip, volume, pitch);
                return;
            }

            if (!TryPlayWithAudioUtil(clip))
            {
                PlayWithFallback(clip, 1f, 1f);
            }
        }

        private void PlayWithFallback(AudioClip clip, float volume, float pitch)
        {
            AudioSource source = EnsureFallbackSource();
            source.clip = clip;
            source.volume = Mathf.Clamp01(volume);
            source.pitch = pitch;
            source.loop = false;
            source.spatialBlend = 0f;
            source.playOnAwake = false;
            source.Play();
            _usingFallback = true;
        }

        private void StopPreview()
        {
            TryStopAudioUtil();

            if (_fallbackSource != null)
            {
                _fallbackSource.Stop();
            }

            _usingFallback = false;
        }

        private AudioSource EnsureFallbackSource()
        {
            if (_fallbackSource != null)
            {
                return _fallbackSource;
            }

            GameObject host = new GameObject(PreviewSourceName)
            {
                hideFlags = HideFlags.HideAndDontSave
            };
            _fallbackSource = host.AddComponent<AudioSource>();
            _fallbackSource.playOnAwake = false;
            return _fallbackSource;
        }

        private void DestroyFallbackSource()
        {
            if (_fallbackSource == null)
            {
                return;
            }

            GameObject host = _fallbackSource.gameObject;
            _fallbackSource = null;
            if (host != null)
            {
                Object.DestroyImmediate(host);
            }
        }

        private int CountValidClips()
        {
            if (_clipsProperty == null || !_clipsProperty.isArray)
            {
                return 0;
            }

            int valid = 0;
            for (int i = 0; i < _clipsProperty.arraySize; i++)
            {
                if (_clipsProperty.GetArrayElementAtIndex(i).objectReferenceValue != null)
                {
                    valid++;
                }
            }

            return valid;
        }

        private AudioClip FirstValidClip()
        {
            if (_clipsProperty == null || !_clipsProperty.isArray)
            {
                return null;
            }

            for (int i = 0; i < _clipsProperty.arraySize; i++)
            {
                Object reference = _clipsProperty.GetArrayElementAtIndex(i).objectReferenceValue;
                if (reference is AudioClip clip)
                {
                    return clip;
                }
            }

            return null;
        }

        private static bool TryPlayWithAudioUtil(AudioClip clip)
        {
            ResolveReflection();
            if (_playPreviewMethod == null)
            {
                return false;
            }

            try
            {
                ParameterInfo[] parameters = _playPreviewMethod.GetParameters();
                object[] arguments = parameters.Length == 3
                    ? new object[] { clip, 0, false }
                    : new object[] { clip };
                _playPreviewMethod.Invoke(null, arguments);
                return true;
            }
            catch
            {
                _playPreviewMethod = null;
                return false;
            }
        }

        private static void TryStopAudioUtil()
        {
            ResolveReflection();
            if (_stopPreviewMethod == null)
            {
                return;
            }

            try
            {
                _stopPreviewMethod.Invoke(null, null);
            }
            catch
            {
                _stopPreviewMethod = null;
            }
        }

        private static void ResolveReflection()
        {
            if (_reflectionResolved)
            {
                return;
            }

            _reflectionResolved = true;

            System.Type audioUtil = typeof(AudioImporter).Assembly.GetType("UnityEditor.AudioUtil");
            if (audioUtil == null)
            {
                return;
            }

            const BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            _playPreviewMethod = audioUtil.GetMethod("PlayPreviewClip", flags)
                ?? audioUtil.GetMethod("PlayClip", flags);
            _stopPreviewMethod = audioUtil.GetMethod("StopAllPreviewClips", flags)
                ?? audioUtil.GetMethod("StopAllClips", flags);
        }
    }
}
