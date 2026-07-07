using System.Collections.Generic;
using UnityEngine;
using CatAnnaDev.Utils;

namespace CatAnnaDev.Saving
{
    [DisallowMultipleComponent]
    [AddComponentMenu("CatAnnaDev/Saving/Saveable Entity")]
    public sealed class SaveableEntity : MonoBehaviour, ISaveable
    {
        [SerializeField, ReadOnly] private SerializableGuid id;
        [SerializeField] private bool registerOnEnable = true;

        private readonly List<ISaveable> componentSaveables = new List<ISaveable>();
        private bool registered;

        public string SaveKey
        {
            get { return id.Value; }
        }

        public bool HasStableId
        {
            get { return id.IsValid; }
        }

        private void Awake()
        {
            EnsureId();
            CollectComponentSaveables();
        }

        private void OnEnable()
        {
            if (registerOnEnable)
            {
                Register();
            }
        }

        private void OnDisable()
        {
            Unregister();
        }

        public void Register()
        {
            if (registered)
            {
                return;
            }

            EnsureId();
            SaveSystem.Register(this);
            registered = true;
        }

        public void Unregister()
        {
            if (!registered)
            {
                return;
            }

            SaveSystem.Unregister(this);
            registered = false;
        }

        public object CaptureState()
        {
            if (componentSaveables.Count == 0)
            {
                CollectComponentSaveables();
            }

            SaveData bundle = new SaveData();
            for (int i = 0; i < componentSaveables.Count; i++)
            {
                ISaveable saveable = componentSaveables[i];
                string key = saveable.SaveKey;
                if (string.IsNullOrEmpty(key))
                {
                    continue;
                }

                object state = saveable.CaptureState();
                if (state != null)
                {
                    bundle.SetObject(key, state);
                }
            }

            return bundle;
        }

        public void RestoreState(object state)
        {
            SaveData bundle = state as SaveData;
            if (bundle == null)
            {
                return;
            }

            if (componentSaveables.Count == 0)
            {
                CollectComponentSaveables();
            }

            for (int i = 0; i < componentSaveables.Count; i++)
            {
                ISaveable saveable = componentSaveables[i];
                string key = saveable.SaveKey;
                object componentState;
                if (!string.IsNullOrEmpty(key) && bundle.TryGetObject(key, out componentState) && componentState != null)
                {
                    saveable.RestoreState(componentState);
                }
            }
        }

        public void CollectComponentSaveables()
        {
            componentSaveables.Clear();
            GetComponents(bufferComponents);
            for (int i = 0; i < bufferComponents.Count; i++)
            {
                ISaveable saveable = bufferComponents[i] as ISaveable;
                if (saveable != null && !ReferenceEquals(saveable, this))
                {
                    componentSaveables.Add(saveable);
                }
            }

            bufferComponents.Clear();
        }

        private void EnsureId()
        {
            if (!id.IsValid)
            {
                id = SerializableGuid.NewGuid();
            }
        }

        private readonly List<Component> bufferComponents = new List<Component>();

#if UNITY_EDITOR
        private void Reset()
        {
            if (!id.IsValid)
            {
                id = SerializableGuid.NewGuid();
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }

        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                return;
            }

            if (!id.IsValid)
            {
                id = SerializableGuid.NewGuid();
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }
#endif
    }
}
