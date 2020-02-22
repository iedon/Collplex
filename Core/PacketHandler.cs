using System;
using System.Collections.Generic;
using Google.Protobuf;
using Collplex.Models;
using Collplex.Models.Node;
using static Collplex.Models.ResponsePacket.Types;

namespace Collplex.Core
{
    public static class PacketHandler
    {
        private static string GetMessageByCode(ResponseCodeType code)
            => code switch
            {
                ResponseCodeType.Ok => "ok",
                ResponseCodeType.ServerException => "server error",
                ResponseCodeType.NotFound => "not found",
                ResponseCodeType.Forbidden => "forbidden",
                ResponseCodeType.BadGateway => "bad gateway",
                ResponseCodeType.BadRequest => "bad request",
                ResponseCodeType.ServiceUnavailable => "service unavailable",
                ResponseCodeType.MethodNotAllowed => "method not allowed",
                ResponseCodeType.InvalidBody => "invalid body",
                ResponseCodeType.NodeOperationFailed => "node: operation failed",
                ResponseCodeType.NodeInvalidClientId => "node: invalid clientId",
                ResponseCodeType.NodeRegLimit => "node: could not register more services",
                ResponseCodeType.NodeLockTimeout => "node: lock timeout",
                ResponseCodeType.NodeResponseError => "node: response error",
                ResponseCodeType.NodeNetworkException => "node: network exception",
                ResponseCodeType.NodeResponseTimedout => "node: response timed out",
                ResponseCodeType.SvcInvalidClientId => "service: invalid clientId",
                ResponseCodeType.SvcNotFound => "service: requested key not found",
                ResponseCodeType.SvcUnavailable => "service: unavailable",
                _ => "unknown",
            };

        public static byte[] MakeRPCResponse(ResponseCodeType code, object data = null)
            => (new ResponsePacket
            {
                Code = code,
                Message = GetMessageByCode(code),
                Data = (data != null ? Utils.JsonSerialize(data) : string.Empty),
            }).ToByteArray();

        public static object MakeResponse(ResponseCodeType code, object data = null)
            => new
            {
                Code = code,
                Message = GetMessageByCode(code),
                Data = data ?? "",
            };

        public static bool MakeRPCRequestOut(object serializable, string clientId, string clientSecret, string remoteIp, int remotePort, Dictionary<string, string[]> remoteHeaders, out RPCRequestOut packetOutbound)
        {
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(); // 生成时间戳
            try
            {
                RPCRequestOutPayload payload = new RPCRequestOutPayload
                {
                    RemoteIp = remoteIp,
                    RemotePort = remotePort,
                    RemoteHeaders = remoteHeaders,
                    Data = serializable
                };
                string rawData = Utils.JsonSerialize(payload);
                string signature = MakeSignature(rawData, clientId, clientSecret, timestamp);
                packetOutbound = new RPCRequestOut
                {
                    Signature = signature,
                    Timestamp = timestamp,
                    Data = rawData,
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

        public static bool ValidateRPCRequest(RPCRequestIn request)
        {
            if (request == null
                || string.IsNullOrEmpty(request.ClientId)
                || string.IsNullOrEmpty(request.Signature)
                || request.Timestamp <= 0
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

        public static string GetRPCPayload(RPCRequestIn request, string clientSecret)
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
