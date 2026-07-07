using System.Text;
using UnityEngine;

namespace CatAnnaDev.Saving
{
    public sealed class JsonSaveSerializer : ISaveSerializer
    {
        private readonly bool prettyPrint;

        public JsonSaveSerializer() : this(true)
        {
        }

        public JsonSaveSerializer(bool prettyPrint)
        {
            this.prettyPrint = prettyPrint;
        }

        public string Extension
        {
            get { return ".json"; }
        }

        public byte[] Serialize(SaveData data)
        {
            if (data == null)
            {
                return System.Array.Empty<byte>();
            }

            string json = JsonUtility.ToJson(data, prettyPrint);
            return Encoding.UTF8.GetBytes(json);
        }

        public SaveData Deserialize(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return new SaveData();
            }

            string json = Encoding.UTF8.GetString(bytes);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            return data ?? new SaveData();
        }
    }
}
