using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Manager
{
    public class TripleDES
    {
        public static byte[] Encrypt(byte[] input, string key)
        {
            TripleDESCryptoServiceProvider desCryptoProvider = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider hashMD5Provider = new MD5CryptoServiceProvider();

            // Za hashovan kljuc
            byte[] keyBuffer;

            // Za kriptovanu poruku
            byte[] byteBuff;
            byteBuff = input;

            keyBuffer = hashMD5Provider.ComputeHash(Encoding.UTF8.GetBytes(key));

            desCryptoProvider.Key = keyBuffer;
            desCryptoProvider.Mode = CipherMode.ECB;

            byte[] encrypted =
                desCryptoProvider.CreateEncryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length);

            return encrypted;
        }

        public static byte[] Decrypt(byte[] input, string key)
        {
            TripleDESCryptoServiceProvider desCryptoProvider = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider hashMD5Provider = new MD5CryptoServiceProvider();

            // Za hashovan kljuc
            byte[] keyBuffer;

            // Za dekriptovanu poruku
            byte[] byteBuff;
            byteBuff = input;

            keyBuffer = hashMD5Provider.ComputeHash(Encoding.UTF8.GetBytes(key));
            desCryptoProvider.Key = keyBuffer;
            desCryptoProvider.Mode = CipherMode.ECB;

            byte[] plaintext =
                desCryptoProvider.CreateDecryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length);

            return plaintext;
        }
    }
}
