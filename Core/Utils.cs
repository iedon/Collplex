using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Collplex.Core
{
    public static class Utils
    {
        public static string CommonEncrypt(string rawData, string clientSecret, string iv)
            => Aes256CBCEncrypt(rawData, clientSecret, iv);

        public static string CommonDecrypt(string rawData, string clientSecret, string iv)
            => Aes256CBCDecrypt(rawData, clientSecret, iv);

        public static string Aes256CBCEncrypt(string plainText, string key, string iv)
        {
            var aes = new AesCryptoServiceProvider
            {
                KeySize = 256,
                BlockSize = 128,
                Key = Encoding.UTF8.GetBytes(key),
                IV = Convert.FromBase64String(iv),
                Padding = PaddingMode.PKCS7,
                Mode = CipherMode.CBC
            };

            var crypto = aes.CreateEncryptor(aes.Key, aes.IV);
            var rawBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] encrypted = crypto.TransformFinalBlock(rawBytes, 0, rawBytes.Length);

            crypto.Dispose();
            aes.Dispose();
            return Convert.ToBase64String(encrypted);
        }

        public static string Aes256CBCDecrypt(string base64encoded, string key, string iv)
        {
            var aes = new AesCryptoServiceProvider
            {
                KeySize = 256,
                BlockSize = 128,
                Key = Encoding.UTF8.GetBytes(key),
                IV = Convert.FromBase64String(iv),
                Padding = PaddingMode.PKCS7,
                Mode = CipherMode.CBC
            };

            var crypto = aes.CreateDecryptor(aes.Key, aes.IV);
            var rawBytes = Convert.FromBase64String(base64encoded);
            byte[] decrypted = crypto.TransformFinalBlock(rawBytes, 0, rawBytes.Length);

            crypto.Dispose();
            aes.Dispose();
            return Encoding.UTF8.GetString(decrypted);
        }

        public static string HmacSHA256Hash(string message, string secret)
        {
            secret ??= "";
            var encoding = new UTF8Encoding();
            byte[] keyByte = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            using var hmacsha256 = new HMACSHA256(keyByte);
            byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
            return Convert.ToBase64String(hashmessage);
        }

        public static string SHA256Hash(string message)
        {
            var encoding = new UTF8Encoding();
            byte[] messageBytes = encoding.GetBytes(message);
            using var sha256hash = SHA256.Create();
            byte[] hashmessage = sha256hash.ComputeHash(messageBytes);
            return Convert.ToBase64String(hashmessage);
        }

        public static string SHA1Hash(string message)
        {
            var encoding = new UTF8Encoding();
            byte[] messageBytes = encoding.GetBytes(message);
            using var sha1hash = SHA1.Create();
            byte[] hashmessage = sha1hash.ComputeHash(messageBytes);
            return Convert.ToBase64String(hashmessage);
        }

        public static byte[] GetRandomBytes(int size)
        {
            byte[] data = new byte[size];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetBytes(data);
            }
            return data;
        }

        public static string JsonSerialize<T>(T rawObject)
            => JsonSerializer.Serialize(rawObject, Constants.JsonSerializerOptionsGlobal);

        public static T JsonDeserialize<T>(string jsonText)
            => JsonSerializer.Deserialize<T>(jsonText, Constants.JsonSerializerOptionsGlobal);

        public async static Task<T> JsonDeserializeAsync<T>(Stream utf8JsonStream)
            => await JsonSerializer.DeserializeAsync<T>(utf8JsonStream, Constants.JsonSerializerOptionsGlobal);
    }
}
