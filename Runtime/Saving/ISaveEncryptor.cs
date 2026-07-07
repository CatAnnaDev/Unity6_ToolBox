namespace CatAnnaDev.Saving
{
    public interface ISaveEncryptor
    {
        byte[] Encrypt(byte[] plain);

        byte[] Decrypt(byte[] cipher);
    }
}
