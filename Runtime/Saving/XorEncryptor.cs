using System;
using System.Text;

namespace CatAnnaDev.Saving
{
    public sealed class XorEncryptor : ISaveEncryptor
    {
        private readonly byte[] key;

        public XorEncryptor(string passphrase)
        {
            if (string.IsNullOrEmpty(passphrase))
            {
                throw new ArgumentException("XorEncryptor requires a non-empty passphrase.", "passphrase");
            }

            key = Encoding.UTF8.GetBytes(passphrase);
        }

        public XorEncryptor(byte[] keyBytes)
        {
            if (keyBytes == null || keyBytes.Length == 0)
            {
                throw new ArgumentException("XorEncryptor requires a non-empty key.", "keyBytes");
            }

            key = (byte[])keyBytes.Clone();
        }

        public byte[] Encrypt(byte[] plain)
        {
            return Transform(plain);
        }

        public byte[] Decrypt(byte[] cipher)
        {
            return Transform(cipher);
        }

        private byte[] Transform(byte[] input)
        {
            if (input == null || input.Length == 0)
            {
                return input;
            }

            byte[] output = new byte[input.Length];
            int keyLength = key.Length;
            for (int i = 0; i < input.Length; i++)
            {
                output[i] = (byte)(input[i] ^ key[i % keyLength]);
            }

            return output;
        }
    }
}
