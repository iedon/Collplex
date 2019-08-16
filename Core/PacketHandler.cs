using System;
using Collplex.Models;
using Collplex.Models.Node;

namespace Collplex.Core
{
    public static class PacketHandler
    {
        private static string GetMessageByCode(ResponseCodeType code)
        {
            return code switch
            {
                ResponseCodeType.OK => "ok",
                ResponseCodeType.SERVER_EXCEPTION => "server error",
                ResponseCodeType.NOT_FOUND => "not found",
                ResponseCodeType.FORBIDDEN => "forbidden",
                ResponseCodeType.BAD_GATEWAY => "bad gateway",
                ResponseCodeType.BAD_REQUEST => "bad request",
                ResponseCodeType.SERVICE_UNAVAILABLE => "service unavailable",
                ResponseCodeType.METHOD_NOT_ALLOWED => "method not allowed",
                ResponseCodeType.INVALID_BODY => "invalid body",
                ResponseCodeType.NODE_OPERATION_FAILED => "node: operation failed",
                ResponseCodeType.NODE_INVALID_CLIENT_ID => "node: invalid clientId",
                ResponseCodeType.NODE_REG_CUSTOM_SVC_LIMIT => "node: could not register more custom services",
                ResponseCodeType.NODE_LOCK_TIMEOUT => "node: lock timeout",
                ResponseCodeType.NODE_RESPONSE_ERROR => "node: response error",
                ResponseCodeType.NODE_RESPONSE_TIMEDOUT => "node: response timed out",
                ResponseCodeType.SVC_INVALID_CLIENT_ID => "service: invalid clientId",
                ResponseCodeType.SVC_NOT_FOUND => "service: requested key not found",
                _ => "unknown",
            };
        }

        public static ResponsePacket MakeResponse(ResponseCodeType code, object data = null) => new ResponsePacket
        {
            Code = code,
            Message = GetMessageByCode(code),
            Data = data ?? "",
        };

        public static bool MakeNodePacketOutbound(object serializable, string clientId, string clientSecret, out NodePacketOutbound packetOutbound, out string iv)
        {
            string rawData = Utils.JsonSerialize(serializable);
            iv = Convert.ToBase64String(Utils.GetRandomBytes(16)) ; // 生成随机初始化向量
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(); // 生成时间戳
            try
            {
                string encryptedData = Utils.CommonEncrypt(rawData, clientSecret, iv);
                string signature = MakeSignature(encryptedData, clientId, clientSecret, timestamp);
                packetOutbound = new NodePacketOutbound
                {
                    Signature = signature,
                    Timestamp = timestamp,
                    Iv = iv,
                    Data = encryptedData
                };
            }
            catch
            {
                packetOutbound = null;
                return false;
            }
            return true;
        }

        public static bool ValidateRequest(ServiceRequest request)
        {
            if (request == null || request.ClientId == null || request.Key == null)
                return false;
            return true;
        }

        public static bool ValidateNodePacketInbound(NodePacketInbound request)
        {
            if (request == null
                || request.ClientId == null
                || request.Signature == null
                || request.Timestamp <= 0
                || request.Action == null
                || request.Iv == null
                || request.Data == null)
                return false;

            // 防止重放攻击，如果用户 Timestamp 造假，则会在后面 DecryptNodePacketInbound() 签名验证环节被发现和拦截。
            var currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (request.Timestamp < currentTimestamp - Constants.NodePacketInboundAntiReplaySeconds || request.Timestamp > currentTimestamp + Constants.NodePacketInboundAntiReplaySeconds)
            {
                return false;
            }
            return true;
        }

        public static string MakeSignature(string encryptedData, string clientId, string clientSecret, long timestamp)
        {
            /* 加密和签名流程：
             * 原始数据 -> 用 clientSecret 作密钥 进行 AES-256-CBC 加密 -> Base64 -> 【得到处理后的数据】
             *          -> 使用 clientId + clientSecret 作密钥 对处理后的数据进行 HmacSHA256 签名 -> 将签名后的 base64 串 + UNIX 时间戳 再次进行 SHA256 哈希
             *          -> 【得到最终签名(base64)】
             */
            string hmacSha256Sign = Utils.HmacSHA256Hash(encryptedData, clientId + clientSecret); // 第一轮：HmacSHA256 签名
            return Utils.SHA256Hash(hmacSha256Sign + timestamp); // 第二轮：SHA256 哈希
        }

        public static string DecryptNodePacketInbound(NodePacketInbound request, string clientSecret)
        {
            /* 加密和签名流程：
             * 原始数据 -> 用 clientSecret 作密钥 进行 AES-256-CBC 加密 -> Base64 -> 【得到处理后的数据】
             *          -> 使用 clientId + clientSecret 作密钥 对处理后的数据进行 HmacSHA256 签名 -> 将签名后的 base64 串 + UNIX 时间戳 再次进行 SHA256 哈希
             *          -> 【得到最终签名(base64)】
             */
            try
            {
                string signature = MakeSignature(request.Data, request.ClientId, clientSecret, request.Timestamp);
                if (signature != request.Signature) return null;
                string ret = Utils.CommonDecrypt(request.Data, clientSecret, request.Iv);
                return ret;
            }
            catch
            {
                return null;
            }
        }

        public static string MakeNodeEncryptedPayload(object serializable, string key, string iv)
            => Utils.CommonEncrypt(Utils.JsonSerialize(serializable), key, iv);
    }
}
