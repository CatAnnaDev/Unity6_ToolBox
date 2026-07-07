using System.IO;
using System.Text;

namespace CatAnnaDev.Saving
{
    public sealed class BinarySaveSerializer : ISaveSerializer
    {
        private const uint Magic = 0x43415653;
        private const byte Format = 1;

        public string Extension
        {
            get { return ".sav"; }
        }

        public byte[] Serialize(SaveData data)
        {
            if (data == null)
            {
                data = new SaveData();
            }

            using (MemoryStream stream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8))
            {
                writer.Write(Magic);
                writer.Write(Format);
                writer.Write(data.Version);

                string[] keys = data.SnapshotKeys();
                writer.Write(keys.Length);

                for (int i = 0; i < keys.Length; i++)
                {
                    string key = keys[i];
                    string value;
                    data.TryGetString(key, out value);

                    writer.Write(key);
                    writer.Write(value ?? string.Empty);
                }

                writer.Flush();
                return stream.ToArray();
            }
        }

        public SaveData Deserialize(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return new SaveData();
            }

            using (MemoryStream stream = new MemoryStream(bytes, false))
            using (BinaryReader reader = new BinaryReader(stream, Encoding.UTF8))
            {
                uint magic = reader.ReadUInt32();
                if (magic != Magic)
                {
                    throw new InvalidDataException("Unrecognized CatAnnaDev binary save header.");
                }

                reader.ReadByte();

                int version = reader.ReadInt32();
                SaveData data = new SaveData(version);

                int count = reader.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    string key = reader.ReadString();
                    string value = reader.ReadString();
                    data.SetString(key, value);
                }

                return data;
            }
        }
    }
}
