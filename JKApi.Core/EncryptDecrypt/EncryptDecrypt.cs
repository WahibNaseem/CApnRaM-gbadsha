using System;
using System.Security.Cryptography;
using System.Text;
using JK.Resources;

namespace JKApi.Core.EncryptDecrypt
{
    public class EncryptDecrypt : IEncryptDecrypt
    {
        private static string Key => Convert.ToString(WebConfigResource.EncKey);

        public string Encrypt(string data)
        {
            try
            {
                var rijndaelCipher = new RijndaelManaged
                {
                    Mode = CipherMode.CBC,
                    Padding = PaddingMode.PKCS7,
                    KeySize = 0x80,
                    BlockSize = 0x80
                };

                var pwdBytes = Encoding.UTF8.GetBytes(Key);
                var keyBytes = new byte[0x10];
                var len = pwdBytes.Length;
                if (len > keyBytes.Length)
                {
                    len = keyBytes.Length;
                }

                Array.Copy(pwdBytes, keyBytes, len);
                rijndaelCipher.Key = keyBytes;
                rijndaelCipher.IV = keyBytes;
                var transform = rijndaelCipher.CreateEncryptor();
                var plainText = Encoding.UTF8.GetBytes(data);

                return Convert.ToBase64String (transform.TransformFinalBlock(plainText, 0, plainText.Length)).Replace("/", ",,").Replace("+", "~");
            }
            catch
            {
                return string.Empty;
            }
        }

        public string Decrypt(string data)
        {
            try
            {
                //data = HttpUtility.UrlDecode(data);
                data = data.Replace(",,", "/").Replace("~", "+");
                var rijndaelCipher = new RijndaelManaged
                {
                    Mode = CipherMode.CBC,
                    Padding = PaddingMode.PKCS7,
                    KeySize = 0x80,
                    BlockSize = 0x80
                };

                var encryptedData = Convert.FromBase64String(data);
                var pwdBytes = Encoding.UTF8.GetBytes(Key);
                var keyBytes = new byte[0x10];
                var len = pwdBytes.Length;

                if (len > keyBytes.Length)
                {
                    len = keyBytes.Length;
                }

                Array.Copy(pwdBytes, keyBytes, len);
                rijndaelCipher.Key = keyBytes;
                rijndaelCipher.IV = keyBytes;
                var plainText = rijndaelCipher.CreateDecryptor().TransformFinalBlock(encryptedData, 0, encryptedData.Length);

                return Encoding.UTF8.GetString(plainText);
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
