namespace CatAnnaDev.Saving
{
    public interface ISaveSerializer
    {
        string Extension { get; }

        byte[] Serialize(SaveData data);

        SaveData Deserialize(byte[] bytes);
    }
}
