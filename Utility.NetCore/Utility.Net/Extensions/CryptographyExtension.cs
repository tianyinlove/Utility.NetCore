using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Utility.Extensions
{
    /// <summary>
    /// 加解密扩展
    /// </summary>
    public static class CryptographyExtension
    {
        /// <summary>
        /// 用md5计算hash
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Md5(this string text)
        {
            if (text == null)
            {
                return null;
            }
            using (var md5 = MD5.Create())
            {
                return BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(text))).ToLower().Replace("-", "");
            }
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="key">base64密钥</param>
        /// <param name="plainText">明文</param>
        /// <returns></returns>
        public static string RsaEncrypt(this string plainText, string key)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportCspBlob(Convert.FromBase64String(key));
                var plainData = Encoding.UTF8.GetBytes(plainText);
                var cipherData = rsa.Encrypt(plainData, false);
                return Convert.ToBase64String(cipherData);
            }
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="key">base64密钥</param>
        /// <param name="cipherText">密文</param>
        /// <returns></returns>
        public static string RsaDecrypt(this string cipherText, string key)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportCspBlob(Convert.FromBase64String(key));
                var cipherData = Convert.FromBase64String(cipherText);
                var plainData = rsa.Decrypt(cipherData, false);
                return Encoding.UTF8.GetString(plainData);
            }
        }

    }
}
