using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Configuration;
using Core.Logger;

namespace Core.Helpers
{
    public class DataEncrytion
    {
        static private byte[] key = { };
        static private byte[] IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xab, 0xcd, 0xef };

        // Encryption key for Text
        static private string TextEncryptionKey = ConfigurationManager.AppSettings["EncodeKey"].Trim();//"Z#5?@k%7";

        #region Text Encryption Methods
        public string Decrypt(string stringToDecrypt)
        {
            byte[] inputByteArray = new byte[stringToDecrypt.Length + 1];
            try
            {
                key = System.Text.Encoding.UTF8.GetBytes(TextEncryptionKey.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                inputByteArray = Convert.FromBase64String(stringToDecrypt);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(key, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                System.Text.Encoding encoding = System.Text.Encoding.UTF8;
                return encoding.GetString(ms.ToArray());
            }
            catch (Exception e)
            {
                LoggerFactory.CreateLogger().Log(e.Message, LogType.Error);
                return null;
            }

        }

        public string Encrypt(string stringToEncrypt)
        {
            try
            {
                key = System.Text.Encoding.UTF8.GetBytes(TextEncryptionKey.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByteArray = Encoding.UTF8.GetBytes(stringToEncrypt);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(key, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                return Convert.ToBase64String(ms.ToArray());
            }
            catch (Exception e)
            {
                LoggerFactory.CreateLogger().Log(e.Message, LogType.Error);
                return null;
            }
        }
        #endregion

        #region MD5 Encryption
        public string EncryptMD5(string token)
        {
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(token));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
        #endregion
    }
}