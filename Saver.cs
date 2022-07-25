using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;

namespace Potato
{
    internal static class Saver
    {
        private readonly static byte[] keyBytes =
        {
            0x86, 0x1e, 0xa9, 0x51, 0x1d, 0x03, 0x61, 0x80,
            0x57, 0x72, 0xc8, 0x68, 0xf5, 0xa0, 0x48, 0x9d,
            0x01, 0xbe, 0x48, 0xfd, 0x71, 0x9b, 0xdb, 0xb0,
            0x10, 0x6e, 0xc3, 0xb2, 0xec, 0x68, 0x0d, 0xae
        };
        private readonly static byte[] IV =
        {
            0xcf, 0xa8, 0xbd, 0x61, 0xe3, 0x56, 0xc2, 0x15,
            0xa9, 0x3a, 0x0c, 0x7f, 0x02, 0x9a, 0xc2, 0xe2
        };
        public static void Save(string fileName, object obj)
        {
            string json = JsonSerializer.Serialize(obj);
            string encryptedJson = Encrypt(json);
            File.WriteAllText(fileName, encryptedJson);
        }
        public static T Load<T>(string fileName)
        {
            string encryptedJson = File.ReadAllText(fileName);
            string json = Decrypt(encryptedJson);
            return JsonSerializer.Deserialize<T>(json);
        }
        public static string Encrypt(string plainText)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] cipherTextBytes = null;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = keyBytes;
                aesAlg.IV = IV;
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aesAlg.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(plainTextBytes, 0, plainTextBytes.Length);
                        cs.Close();
                    }
                    cipherTextBytes = ms.ToArray();
                }
            }
            return Convert.ToBase64String(cipherTextBytes);
        }
        public static string Decrypt(string encryptedText)
        {
            byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
            byte[] plainTextBytes = null;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = keyBytes;
                aesAlg.IV = IV;
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aesAlg.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherTextBytes, 0, cipherTextBytes.Length);
                        cs.Close();
                    }
                    plainTextBytes = ms.ToArray();
                }
            }
            return Encoding.UTF8.GetString(plainTextBytes);
        }
    }
}