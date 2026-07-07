using System;
using System.Collections.Generic;
using UnityEngine;

namespace CatAnnaDev.Utils
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<TKey> keys = new List<TKey>();

        [SerializeField]
        private List<TValue> values = new List<TValue>();

        public SerializableDictionary()
        {
        }

        public SerializableDictionary(IDictionary<TKey, TValue> source)
            : base(source)
        {
        }

        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();
            keys.Capacity = Count;
            values.Capacity = Count;
            foreach (KeyValuePair<TKey, TValue> pair in this)
            {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            Clear();
            int size = Mathf.Min(keys.Count, values.Count);
            for (int i = 0; i < size; i++)
            {
                if (keys[i] == null)
                {
                    continue;
                }
                this[keys[i]] = values[i];
            }
        }
    }
}
