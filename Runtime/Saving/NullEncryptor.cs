namespace CatAnnaDev.Saving
{
    public sealed class NullEncryptor : ISaveEncryptor
    {
        public static readonly NullEncryptor Shared = new NullEncryptor();

        public byte[] Encrypt(byte[] plain)
        {
            return plain;
        }

        public byte[] Decrypt(byte[] cipher)
        {
            return cipher;
        }
    }
}
