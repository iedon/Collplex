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
        public static string HmacSHA1Hash(string message, string secret)
        {
            secret ??= string.Empty;
            var encoding = new UTF8Encoding();
            byte[] keyByte = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            using var hmacsha1 = new HMACSHA1(keyByte);
            byte[] hashmessage = hmacsha1.ComputeHash(messageBytes);
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

        public static string JsonSerialize<T>(T rawObject)
            => JsonSerializer.Serialize(rawObject, Constants.JsonSerializerOptionsGlobal);

        public static T JsonDeserialize<T>(string jsonText)
            => JsonSerializer.Deserialize<T>(jsonText, Constants.JsonSerializerOptionsGlobal);

        public async static Task<T> JsonDeserializeAsync<T>(Stream utf8JsonStream)
            => await JsonSerializer.DeserializeAsync<T>(utf8JsonStream, Constants.JsonSerializerOptionsGlobal);
    }
}
