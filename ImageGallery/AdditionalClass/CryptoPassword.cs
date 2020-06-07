using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ImageGallery.AdditionalClass
{
    public class CryptoPassword
    {
        private static byte[] IV = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };

        public string Encrypt(string oldpassword)   //шифрование
        {
            byte[] bytes = Encoding.Unicode.GetBytes(oldpassword);
            
            SymmetricAlgorithm crypt = Aes.Create();
            HashAlgorithm hash = MD5.Create();

            crypt.BlockSize = 128;
            crypt.Key = hash.ComputeHash(Encoding.Unicode.GetBytes("6tSeqDZMWJbcmtvh"));
            crypt.IV = IV;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, crypt.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(bytes, 0, bytes.Length);
                }

                return Convert.ToBase64String(memoryStream.ToArray());
            }
        }

        public string Decrypt(string encryptPassword)   //расшифровка
        {
            byte[] bytes = Convert.FromBase64String(encryptPassword);

            SymmetricAlgorithm crypt = Aes.Create();
            HashAlgorithm hash = MD5.Create();

            crypt.Key = hash.ComputeHash(Encoding.Unicode.GetBytes("6tSeqDZMWJbcmtvh"));
            crypt.IV = IV;

            using (MemoryStream memoryStream = new MemoryStream(bytes))
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, crypt.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    byte[] decryptedBytes = new byte[bytes.Length];

                    cryptoStream.Read(decryptedBytes, 0, decryptedBytes.Length);
                    
                    return Encoding.Unicode.GetString(decryptedBytes).TrimEnd('\0');
                }
            }
        }
    }
}
