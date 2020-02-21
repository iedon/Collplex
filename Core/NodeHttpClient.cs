using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using Collplex.Models;
using Collplex.Models.Node;
using static Collplex.Models.ResponsePacket.Types;

namespace Collplex.Core
{
    public class NodeHttpClient
    {
        public HttpClient Client { get; }

        public NodeHttpClient(HttpClient client)
        {
            client.DefaultRequestHeaders.Add("user-agent", Constants.NodeHttpClientUserAgent);
            Client = client;
        }

        /* 如果任务超时，会由 PostAsync Task 发出 TaskCanceledException */
        public async Task<object> RequestNodeService(Uri nodeServiceUrl, object data, int timeout, string clientId, string clientSecret, string remoteIp, int remotePort, Dictionary<string, string[]> remoteHeaders)
        {
            Client.Timeout = TimeSpan.FromSeconds(timeout);
            if (!PacketHandler.MakeRPCRequestOut(data, clientId, clientSecret, remoteIp, remotePort, remoteHeaders, out RPCRequestOut packetOutbound))
                return null;
            
            using var content = new ByteArrayContent(packetOutbound.ToByteArray());
            content.Headers.ContentType = new MediaTypeHeaderValue(Constants.ProtobufContentType);
            using var response = await Client.PostAsync(nodeServiceUrl, content);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            try
            {
                using var httpBody = await response.Content.ReadAsStreamAsync();

                // 这里使用流解析响应数据包，提高性能
                ResponsePacket responsePacket = ResponsePacket.Parser.ParseFrom(httpBody);
                if (responsePacket.Code != ResponseCodeType.Ok) return null;

                return Utils.JsonDeserialize<object>(responsePacket.Data);
            }
            catch
            {
                return null;
            }
        }
    }
}
