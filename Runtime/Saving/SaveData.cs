using System;
using System.Collections.Generic;
using UnityEngine;

namespace CatAnnaDev.Saving
{
    [Serializable]
    public sealed class SaveData : ISerializationCallbackReceiver
    {
        public const int CurrentVersion = 1;

        [SerializeField] private int version = CurrentVersion;
        [SerializeField] private string[] keys = Array.Empty<string>();
        [SerializeField] private string[] values = Array.Empty<string>();

        private readonly Dictionary<string, string> entries = new Dictionary<string, string>(StringComparer.Ordinal);

        public SaveData()
        {
        }

        public SaveData(int dataVersion)
        {
            version = dataVersion;
        }

        public int Version
        {
            get { return version; }
            set { version = value; }
        }

        public int Count
        {
            get { return entries.Count; }
        }

        public Dictionary<string, string>.KeyCollection Keys
        {
            get { return entries.Keys; }
        }

        public bool Has(string key)
        {
            return !string.IsNullOrEmpty(key) && entries.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            return !string.IsNullOrEmpty(key) && entries.Remove(key);
        }

        public void Clear()
        {
            entries.Clear();
        }

        public void SetString(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            entries[key] = value ?? string.Empty;
        }

        public bool TryGetString(string key, out string value)
        {
            if (!string.IsNullOrEmpty(key))
            {
                return entries.TryGetValue(key, out value);
            }

            value = null;
            return false;
        }

        public string GetString(string key, string fallback = "")
        {
            string value;
            return TryGetString(key, out value) ? value : fallback;
        }

        public void SetInt(string key, int value)
        {
            SetString(key, value.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }

        public int GetInt(string key, int fallback = 0)
        {
            string raw;
            int parsed;
            if (TryGetString(key, out raw) && int.TryParse(raw, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out parsed))
            {
                return parsed;
            }

            return fallback;
        }

        public void SetFloat(string key, float value)
        {
            SetString(key, value.ToString("R", System.Globalization.CultureInfo.InvariantCulture));
        }

        public float GetFloat(string key, float fallback = 0f)
        {
            string raw;
            float parsed;
            if (TryGetString(key, out raw) && float.TryParse(raw, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out parsed))
            {
                return parsed;
            }

            return fallback;
        }

        public void SetBool(string key, bool value)
        {
            SetString(key, value ? "1" : "0");
        }

        public bool GetBool(string key, bool fallback = false)
        {
            string raw;
            if (TryGetString(key, out raw))
            {
                return raw == "1" || string.Equals(raw, "true", StringComparison.OrdinalIgnoreCase);
            }

            return fallback;
        }

        public void SetObject(string key, object value)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            if (value == null)
            {
                Remove(key);
                return;
            }

            SaveEnvelope envelope = new SaveEnvelope
            {
                type = value.GetType().AssemblyQualifiedName,
                json = JsonUtility.ToJson(value)
            };

            entries[key] = JsonUtility.ToJson(envelope);
        }

        public void SetObject<T>(string key, T value)
        {
            SetObject(key, (object)value);
        }

        public bool TryGetObject(string key, out object value)
        {
            value = null;

            string raw;
            if (!TryGetString(key, out raw) || string.IsNullOrEmpty(raw))
            {
                return false;
            }

            SaveEnvelope envelope = JsonUtility.FromJson<SaveEnvelope>(raw);
            if (envelope == null || string.IsNullOrEmpty(envelope.type))
            {
                return false;
            }

            Type resolved = Type.GetType(envelope.type);
            if (resolved == null)
            {
                return false;
            }

            value = JsonUtility.FromJson(envelope.json ?? string.Empty, resolved);
            return value != null;
        }

        public bool TryGetObject<T>(string key, out T value)
        {
            string raw;
            if (TryGetString(key, out raw) && !string.IsNullOrEmpty(raw))
            {
                SaveEnvelope envelope = JsonUtility.FromJson<SaveEnvelope>(raw);
                if (envelope != null && !string.IsNullOrEmpty(envelope.json))
                {
                    value = JsonUtility.FromJson<T>(envelope.json);
                    return value != null;
                }
            }

            value = default;
            return false;
        }

        public T GetObject<T>(string key, T fallback = default)
        {
            T value;
            return TryGetObject(key, out value) ? value : fallback;
        }

        public string[] SnapshotKeys()
        {
            string[] result = new string[entries.Count];
            entries.Keys.CopyTo(result, 0);
            return result;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            int count = entries.Count;
            keys = new string[count];
            values = new string[count];

            int index = 0;
            foreach (KeyValuePair<string, string> pair in entries)
            {
                keys[index] = pair.Key;
                values[index] = pair.Value;
                index++;
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            entries.Clear();

            if (keys == null || values == null)
            {
                return;
            }

            int count = Math.Min(keys.Length, values.Length);
            for (int i = 0; i < count; i++)
            {
                string key = keys[i];
                if (string.IsNullOrEmpty(key))
                {
                    continue;
                }

                entries[key] = values[i] ?? string.Empty;
            }
        }

        [Serializable]
        private sealed class SaveEnvelope
        {
            public string type;
            public string json;
        }
    }
}
