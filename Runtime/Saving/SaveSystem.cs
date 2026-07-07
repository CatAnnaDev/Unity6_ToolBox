using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace CatAnnaDev.Saving
{
    public static class SaveSystem
    {
        private static readonly List<ISaveable> saveables = new List<ISaveable>();
        private static readonly Dictionary<string, ISaveable> saveablesByKey = new Dictionary<string, ISaveable>(StringComparer.Ordinal);

        private static ISaveSerializer serializer = new JsonSaveSerializer(true);
        private static ISaveEncryptor encryptor = NullEncryptor.Shared;
        private static string directoryOverride;
        private static SynchronizationContext mainContext;

        public static event Action<string> Saved;
        public static event Action<string> Loaded;
        public static event Action<string> Deleted;

        public static int Version = SaveData.CurrentVersion;

        public static ISaveSerializer Serializer
        {
            get { return serializer; }
            set { serializer = value ?? new JsonSaveSerializer(true); }
        }

        public static ISaveEncryptor Encryptor
        {
            get { return encryptor; }
            set { encryptor = value ?? NullEncryptor.Shared; }
        }

        public static string SaveDirectory
        {
            get
            {
                if (!string.IsNullOrEmpty(directoryOverride))
                {
                    return directoryOverride;
                }

                return Path.Combine(Application.persistentDataPath, "Saves");
            }
            set { directoryOverride = value; }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            saveables.Clear();
            saveablesByKey.Clear();
            mainContext = SynchronizationContext.Current;
        }

        public static void Configure(ISaveSerializer saveSerializer, ISaveEncryptor saveEncryptor, string directory)
        {
            Serializer = saveSerializer;
            Encryptor = saveEncryptor;
            directoryOverride = directory;
        }

        public static void Register(ISaveable saveable)
        {
            if (saveable == null)
            {
                return;
            }

            string key = saveable.SaveKey;
            if (string.IsNullOrEmpty(key))
            {
                CatLog.Warn("SaveSystem ignored a saveable with an empty SaveKey.");
                return;
            }

            ISaveable existing;
            if (saveablesByKey.TryGetValue(key, out existing))
            {
                if (!ReferenceEquals(existing, saveable))
                {
                    CatLog.Warn("SaveSystem already has a saveable registered for key '" + key + "'. The new registration was ignored.");
                }

                return;
            }

            saveables.Add(saveable);
            saveablesByKey.Add(key, saveable);
        }

        public static void Unregister(ISaveable saveable)
        {
            if (saveable == null)
            {
                return;
            }

            string key = saveable.SaveKey;
            ISaveable existing;
            if (!string.IsNullOrEmpty(key) && saveablesByKey.TryGetValue(key, out existing) && ReferenceEquals(existing, saveable))
            {
                saveablesByKey.Remove(key);
            }

            saveables.Remove(saveable);
        }

        public static void UnregisterAll()
        {
            saveables.Clear();
            saveablesByKey.Clear();
        }

        public static SaveData CaptureAll()
        {
            SaveData data = new SaveData(Version);
            for (int i = 0; i < saveables.Count; i++)
            {
                ISaveable saveable = saveables[i];
                if (saveable == null)
                {
                    continue;
                }

                string key = saveable.SaveKey;
                if (string.IsNullOrEmpty(key))
                {
                    continue;
                }

                try
                {
                    object state = saveable.CaptureState();
                    if (state != null)
                    {
                        data.SetObject(key, state);
                    }
                }
                catch (Exception exception)
                {
                    CatLog.Error("SaveSystem failed to capture state for key '" + key + "': " + exception.Message);
                }
            }

            return data;
        }

        public static void RestoreAll(SaveData data)
        {
            if (data == null)
            {
                return;
            }

            for (int i = 0; i < saveables.Count; i++)
            {
                ISaveable saveable = saveables[i];
                if (saveable == null)
                {
                    continue;
                }

                string key = saveable.SaveKey;
                if (string.IsNullOrEmpty(key))
                {
                    continue;
                }

                object state;
                if (!data.TryGetObject(key, out state) || state == null)
                {
                    continue;
                }

                try
                {
                    saveable.RestoreState(state);
                }
                catch (Exception exception)
                {
                    CatLog.Error("SaveSystem failed to restore state for key '" + key + "': " + exception.Message);
                }
            }
        }

        public static bool Save(string slot)
        {
            if (!IsValidSlot(slot))
            {
                CatLog.Error("SaveSystem.Save called with an invalid slot name.");
                return false;
            }

            SaveData data = CaptureAll();
            return WriteData(slot, data);
        }

        public static bool WriteData(string slot, SaveData data)
        {
            if (!IsValidSlot(slot) || data == null)
            {
                return false;
            }

            try
            {
                byte[] bytes = EncodeBytes(data);
                WriteBytesAtomic(SlotPath(slot), bytes);
                RaiseOnMain(Saved, slot);
                return true;
            }
            catch (Exception exception)
            {
                CatLog.Error("SaveSystem.Save failed for slot '" + slot + "': " + exception.Message);
                return false;
            }
        }

        public static bool Load(string slot)
        {
            SaveData data;
            if (!ReadData(slot, out data))
            {
                return false;
            }

            RestoreAll(data);
            RaiseOnMain(Loaded, slot);
            return true;
        }

        public static bool ReadData(string slot, out SaveData data)
        {
            data = null;

            if (!IsValidSlot(slot))
            {
                return false;
            }

            string path = SlotPath(slot);
            if (!File.Exists(path))
            {
                return false;
            }

            try
            {
                byte[] bytes = File.ReadAllBytes(path);
                data = DecodeBytes(bytes);
                return data != null;
            }
            catch (Exception exception)
            {
                CatLog.Error("SaveSystem.Load failed for slot '" + slot + "': " + exception.Message);
                return false;
            }
        }

        public static bool Delete(string slot)
        {
            if (!IsValidSlot(slot))
            {
                return false;
            }

            string path = SlotPath(slot);
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                    RaiseOnMain(Deleted, slot);
                    return true;
                }

                return false;
            }
            catch (Exception exception)
            {
                CatLog.Error("SaveSystem.Delete failed for slot '" + slot + "': " + exception.Message);
                return false;
            }
        }

        public static bool HasSlot(string slot)
        {
            return IsValidSlot(slot) && File.Exists(SlotPath(slot));
        }

        public static string[] ListSlots()
        {
            string directory = SaveDirectory;
            if (!Directory.Exists(directory))
            {
                return Array.Empty<string>();
            }

            string extension = serializer.Extension;
            string[] files = Directory.GetFiles(directory, "*" + extension, SearchOption.TopDirectoryOnly);
            string[] slots = new string[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                slots[i] = Path.GetFileNameWithoutExtension(files[i]);
            }

            Array.Sort(slots, StringComparer.Ordinal);
            return slots;
        }

        public static Task<bool> SaveAsync(string slot, Action<bool> onComplete = null)
        {
            if (!IsValidSlot(slot))
            {
                CatLog.Error("SaveSystem.SaveAsync called with an invalid slot name.");
                InvokeOnMain(onComplete, false);
                return Task.FromResult(false);
            }

            SaveData data = CaptureAll();
            byte[] bytes;
            try
            {
                bytes = EncodeBytes(data);
            }
            catch (Exception exception)
            {
                CatLog.Error("SaveSystem.SaveAsync encode failed for slot '" + slot + "': " + exception.Message);
                InvokeOnMain(onComplete, false);
                return Task.FromResult(false);
            }

            string path = SlotPath(slot);
            return Task.Run(() =>
            {
                bool success;
                try
                {
                    WriteBytesAtomic(path, bytes);
                    success = true;
                }
                catch (Exception exception)
                {
                    CatLog.Error("SaveSystem.SaveAsync failed for slot '" + slot + "': " + exception.Message);
                    success = false;
                }

                if (success)
                {
                    RaiseOnMain(Saved, slot);
                }

                InvokeOnMain(onComplete, success);
                return success;
            });
        }

        public static Task<bool> LoadAsync(string slot, Action<bool> onComplete = null)
        {
            if (!IsValidSlot(slot) || !HasSlot(slot))
            {
                InvokeOnMain(onComplete, false);
                return Task.FromResult(false);
            }

            string path = SlotPath(slot);
            return Task.Run(() =>
            {
                SaveData data = null;
                bool decoded;
                try
                {
                    byte[] bytes = File.ReadAllBytes(path);
                    data = DecodeBytes(bytes);
                    decoded = data != null;
                }
                catch (Exception exception)
                {
                    CatLog.Error("SaveSystem.LoadAsync failed for slot '" + slot + "': " + exception.Message);
                    decoded = false;
                }

                if (!decoded)
                {
                    InvokeOnMain(onComplete, false);
                    return false;
                }

                PostToMain(() =>
                {
                    RestoreAll(data);
                    Action<string> handler = Loaded;
                    if (handler != null)
                    {
                        handler(slot);
                    }

                    if (onComplete != null)
                    {
                        onComplete(true);
                    }
                });

                return true;
            });
        }

        private static byte[] EncodeBytes(SaveData data)
        {
            byte[] serialized = serializer.Serialize(data);
            return encryptor.Encrypt(serialized);
        }

        private static SaveData DecodeBytes(byte[] bytes)
        {
            byte[] decrypted = encryptor.Decrypt(bytes);
            return serializer.Deserialize(decrypted);
        }

        private static void WriteBytesAtomic(string path, byte[] bytes)
        {
            string directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string tempPath = path + ".tmp";
            File.WriteAllBytes(tempPath, bytes);

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            File.Move(tempPath, path);
        }

        private static string SlotPath(string slot)
        {
            return Path.Combine(SaveDirectory, slot + serializer.Extension);
        }

        private static bool IsValidSlot(string slot)
        {
            if (string.IsNullOrEmpty(slot))
            {
                return false;
            }

            char[] invalid = Path.GetInvalidFileNameChars();
            for (int i = 0; i < invalid.Length; i++)
            {
                if (slot.IndexOf(invalid[i]) >= 0)
                {
                    return false;
                }
            }

            return true;
        }

        private static void RaiseOnMain(Action<string> handler, string slot)
        {
            if (handler == null)
            {
                return;
            }

            PostToMain(() => handler(slot));
        }

        private static void InvokeOnMain(Action<bool> callback, bool value)
        {
            if (callback == null)
            {
                return;
            }

            PostToMain(() => callback(value));
        }

        private static void PostToMain(Action action)
        {
            if (action == null)
            {
                return;
            }

            if (mainContext != null && mainContext != SynchronizationContext.Current)
            {
                mainContext.Post(delegate { action(); }, null);
            }
            else
            {
                action();
            }
        }
    }
}
