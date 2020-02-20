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
                ResponseCodeType.NODE_NETWORK_EXCEPTION => "node: network exception",
                ResponseCodeType.NODE_RESPONSE_TIMEDOUT => "node: response timed out",
                ResponseCodeType.SVC_INVALID_CLIENT_ID => "service: invalid clientId",
                ResponseCodeType.SVC_NOT_FOUND => "service: requested key not found",
                ResponseCodeType.SVC_UNAVAILABLE => "service: unavailable",
                _ => "unknown",
            };
        }

        public static ResponsePacket MakeResponse(ResponseCodeType code, object data = null) => new ResponsePacket
        {
            Code = code,
            Message = GetMessageByCode(code),
            Data = data ?? "",
        };

        public static bool MakeNodeRequest(object serializable, string clientId, string clientSecret, out NodeRequest packetOutbound)
        {
            string rawData = Utils.JsonSerialize(serializable);
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(); // 生成时间戳
            try
            {
                string signature = MakeSignature(rawData, clientId, clientSecret, timestamp);
                packetOutbound = new NodeRequest
                {
                    Signature = signature,
                    Timestamp = timestamp,
                    Data = rawData
                };
            }
            catch
            {
                packetOutbound = null;
                return false;
            }
            return true;
        }

        public static bool ValidateServiceRequest(ServiceRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.ClientId) || string.IsNullOrEmpty(request.Key))
                return false;
            return true;
        }

        public static bool ValidateRPCRequest(RPCRequest request)
        {
            if (request == null
                || string.IsNullOrEmpty(request.ClientId)
                || string.IsNullOrEmpty(request.Signature)
                || request.Timestamp <= 0
                || string.IsNullOrEmpty(request.Action)
                || string.IsNullOrEmpty(request.Data))
                return false;

            // 防止重放攻击，如果用户 Timestamp 造假，则会在后面 GetRPCPayload() 签名验证环节被发现和拦截。
            var currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (request.Timestamp < currentTimestamp - Constants.NodePacketInboundAntiReplaySeconds || request.Timestamp > currentTimestamp + Constants.NodePacketInboundAntiReplaySeconds)
            {
                return false;
            }
            return true;
        }

        public static string MakeSignature(string data, string clientId, string clientSecret, long timestamp)
            => Utils.HmacSHA1Hash(data, timestamp.ToString() + clientId + clientSecret);

        public static string GetRPCPayload(RPCRequest request, string clientSecret)
        {
            try
            {
                string signature = MakeSignature(request.Data, request.ClientId, clientSecret, request.Timestamp);
                if (signature != request.Signature) return null;
                return request.Data;
            }
            catch
            {
                return null;
            }
        }

    }
}
