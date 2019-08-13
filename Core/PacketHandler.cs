﻿using Collplex.Models;
using Collplex.Models.Node;
using System;
using System.Text;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace Collplex.Core
{
    public static class PacketHandler
    {
        private static string GetDescByCode(ResponseCodeType code)
        {
            switch (code) {
                case ResponseCodeType.OK:
                    return "ok";
                case ResponseCodeType.SERVER_EXCEPTION:
                    return "server error";
                case ResponseCodeType.NOT_FOUND:
                    return "not found";
                case ResponseCodeType.FORBIDDEN:
                    return "forbidden";
                case ResponseCodeType.BAD_GATEWAY:
                    return "bad gateway";
                case ResponseCodeType.BAD_REQUEST:
                    return "bad request";
                case ResponseCodeType.SERVICE_UNAVAILABLE:
                    return "service unavailable";
                case ResponseCodeType.METHOD_NOT_ALLOWED:
                    return "method not allowed";
                case ResponseCodeType.INVALID_BODY:
                    return "invalid body";
                case ResponseCodeType.NODE_OPERATION_FAILED:
                    return "node: operation failed";
                case ResponseCodeType.NODE_INVALID_CLIENT_ID_OR_SECRET:
                    return "node: invalid client_id or client_secret";
                case ResponseCodeType.NODE_REG_CUSTOM_URL_LIMIT:
                    return "node: custom url limit exceeded";
                case ResponseCodeType.NODE_LOCK_TIMEOUT:
                    return "node: lock timeout";
                case ResponseCodeType.SVC_INVALID_CLIENT_ID:
                    return "service: invalid client_id";
                default:
                    return "unknown";
            }
        }

        public static ResponsePacket GenerateResponse(ResponseCodeType code, object data = null) => new ResponsePacket
        {
            Code = code,
            Message = GetDescByCode(code),
            Data = data ?? "",
        };

        public static bool ValidateNodeRequestInbound(NodeRequestInbound request)
        {
            if (request == null
                || request.ClientId == null
                || request.Signature == null
                || request.Timestamp == null
                || request.Action == null
                || request.Iv == null
                || request.Data == null)
                return false;

            // 防止重放攻击，如果用户 Timestamp 造假，则会在后面 DecryptNodeRequestInbound() 签名验证环节被发现和拦截。
            var currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (request.Timestamp < currentTimestamp - Constants.NodeRequestInboundAntiReplaySeconds || request.Timestamp > currentTimestamp + Constants.NodeRequestInboundAntiReplaySeconds)
            {
                return false;
            }
            return true;
        }

        public static string DecryptNodeRequestInbound(NodeRequestInbound request,string clientSecret)
        {
            /* 加密和签名流程：
             * 原始数据 -> 用 clientSecret 作密钥 进行 AES-256-CBC 加密 -> Base64 -> 【得到处理后的数据】
             *          -> 使用 clientId + clientSecret 作密钥 对处理后的数据进行 HmacSHA256 签名 -> 将签名后的 base64 串 + UNIX 时间戳 再次进行 SHA256 哈希
             *          -> 【得到最终签名(base64)】
             */
            try
            {
                string hmacSha256Sign = HmacSHA256Hash(request.Data, request.ClientId + clientSecret); // 第一轮：HmacSHA256 签名
                string signature = SHA256Hash(hmacSha256Sign + request.Timestamp); // 第二轮：SHA256 哈希
                if (signature != request.Signature) return null;
                string ret = Aes256CBCDecrypt(request.Data, clientSecret, request.Iv);
                return ret;
            }
            catch
            {
                return null;
            }
        }

        public static string MakeNodeEncryptedPayload(object serializable, string key, string iv)
            => Aes256CBCEncrypt(
                    JsonConvert.SerializeObject(serializable,
                        Formatting.None,
                        new JsonSerializerSettings {
                            ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
                        }
                    ),
                    key,
                    iv
               );

        private static string Aes256CBCEncrypt(string plainText, string key, string iv)
        {
            var aes = new AesCryptoServiceProvider
            {
                KeySize = 256,
                BlockSize = 128,
                Key = Encoding.UTF8.GetBytes(key),
                IV = Encoding.UTF8.GetBytes(iv),
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

        private static string Aes256CBCDecrypt(string base64encoded, string key, string iv)
        {
            var aes = new AesCryptoServiceProvider
            {
                KeySize = 256,
                BlockSize = 128,
                Key = Encoding.UTF8.GetBytes(key),
                IV = Encoding.UTF8.GetBytes(iv),
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

        private static string HmacSHA256Hash(string message, string secret)
        {
            secret = secret ?? "";
            var encoding = new UTF8Encoding();
            byte[] keyByte = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                return Convert.ToBase64String(hashmessage);
            }
        }

        private static string SHA256Hash(string message)
        {
            var encoding = new UTF8Encoding();
            byte[] messageBytes = encoding.GetBytes(message);
            using (var sha256hash = SHA256.Create())
            {
                byte[] hashmessage = sha256hash.ComputeHash(messageBytes);
                return Convert.ToBase64String(hashmessage);
            }
        }

        public static bool ValidateRequest(ServiceRequest request)
        {
            if (request == null || request.ClientId == null || request.Key == null)
                return false;
            return true;
        }

    }
}
