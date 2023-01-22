using LathBotBack.Config;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace LathBotFront._2FA
{
    public class RijndaelManagedEncryption
    {
        #region Rijndael Encryption
        /// <summary>
        /// Encrypt the given text and give the byte array back as a BASE64 string
        /// </summary>
        /// <param name="text" />The text to encrypt
        /// <param name="salt" />The pasword salt
        /// <returns>The encrypted text</returns>
        public static byte[] EncryptRijndael(string text, string salt)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException(nameof(text));

            using var aesAlg = NewRijndaelManaged(salt);
            var encrypter = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            byte[] encrypted;
            using (var msEncrypt = new MemoryStream())
            {
                using var csEncrypt = new CryptoStream(msEncrypt, encrypter, CryptoStreamMode.Write);
                using (var swEncrypt = new StreamWriter(csEncrypt))
                {
                    swEncrypt.Write(text);
                }
                encrypted = msEncrypt.ToArray();
            }

            return encrypted;
        }
        #endregion

        #region Rijndael Dycryption
        /// <summary>
        /// Decrypts the given text
        /// </summary>
        /// <param name="cipherText" />The encrypted BASE64 text
        /// <param name="salt" />The pasword salt
        /// <returns>The decrypted text</returns>
        public static string DecryptRijndael(byte[] cipherText, string salt)
        {
            if (cipherText is null || cipherText.Length <= 0)
                throw new ArgumentNullException(nameof(cipherText));

            string text;

            using (var aesAlg = NewRijndaelManaged(salt))
            {
                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using var msDecrypt = new MemoryStream(cipherText);
                using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                using var srDecrypt = new StreamReader(csDecrypt);
                text = srDecrypt.ReadToEnd();
            }
            return text;
        }
        #endregion

        #region NewRijndaelManaged
        /// <summary>
        /// Create a new RijndaelManaged class and initialize it
        /// </summary>
        /// <param name="salt" />The pasword salt
        /// <returns></returns>
        private static RijndaelManaged NewRijndaelManaged(string salt)
        {
            if (salt == null)
                throw new ArgumentNullException(nameof(salt));
            var saltBytes = Encoding.ASCII.GetBytes(salt);
            var key = new Rfc2898DeriveBytes(ReadConfig.Config.RijndaelInputKey, saltBytes);

            var aesAlg = new RijndaelManaged();
            aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
            aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);

            return aesAlg;
        }
        #endregion
    }
}
