using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CatAnnaDev.Saving
{
    public sealed class AesEncryptor : ISaveEncryptor
    {
        private const int IvSize = 16;
        private const int KeySize = 32;

        private readonly byte[] key;

        public AesEncryptor(string passphrase)
        {
            if (string.IsNullOrEmpty(passphrase))
            {
                throw new ArgumentException("AesEncryptor requires a non-empty passphrase.", "passphrase");
            }

            using (SHA256 sha = SHA256.Create())
            {
                key = sha.ComputeHash(Encoding.UTF8.GetBytes(passphrase));
            }
        }

        public byte[] Encrypt(byte[] plain)
        {
            if (plain == null)
            {
                return Array.Empty<byte>();
            }

            using (Aes aes = Aes.Create())
            {
                aes.KeySize = KeySize * 8;
                aes.Key = key;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.GenerateIV();

                using (MemoryStream stream = new MemoryStream())
                {
                    stream.Write(aes.IV, 0, aes.IV.Length);

                    using (ICryptoTransform transform = aes.CreateEncryptor())
                    using (CryptoStream crypto = new CryptoStream(stream, transform, CryptoStreamMode.Write))
                    {
                        crypto.Write(plain, 0, plain.Length);
                        crypto.FlushFinalBlock();
                    }

                    return stream.ToArray();
                }
            }
        }

        public byte[] Decrypt(byte[] cipher)
        {
            if (cipher == null || cipher.Length <= IvSize)
            {
                return Array.Empty<byte>();
            }

            byte[] iv = new byte[IvSize];
            Buffer.BlockCopy(cipher, 0, iv, 0, IvSize);

            using (Aes aes = Aes.Create())
            {
                aes.KeySize = KeySize * 8;
                aes.Key = key;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.IV = iv;

                using (MemoryStream input = new MemoryStream(cipher, IvSize, cipher.Length - IvSize, false))
                using (ICryptoTransform transform = aes.CreateDecryptor())
                using (CryptoStream crypto = new CryptoStream(input, transform, CryptoStreamMode.Read))
                using (MemoryStream output = new MemoryStream())
                {
                    crypto.CopyTo(output);
                    return output.ToArray();
                }
            }
        }
    }
}
