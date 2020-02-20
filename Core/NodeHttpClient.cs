using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Collplex.Models;
using Collplex.Models.Node;

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
        public async Task<object> RequestNodeService(Uri nodeServiceUrl, NodePayload outboundRequest, int timeout, string clientId, string clientSecret)
        {
            Client.Timeout = TimeSpan.FromSeconds(timeout);
            if (!PacketHandler.MakeNodeRequest(outboundRequest, clientId, clientSecret, out NodeRequest packetOutbound))
                return null;

            using var content = new StringContent(Utils.JsonSerialize(packetOutbound), Encoding.UTF8, Constants.JsonContentType);
            using var response = await Client.PostAsync(nodeServiceUrl, content);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            try
            {
                using Stream httpBody = await response.Content.ReadAsStreamAsync();

                // 这里使用流解析响应数据包，提高性能
                ResponsePacket responsePacket = await Utils.JsonDeserializeAsync<ResponsePacket>(httpBody);
                if (responsePacket.Code != ResponseCodeType.OK) return null;
   
                return Utils.JsonDeserialize<object>(responsePacket.Data.ToString());
            }
            catch
            {
                return null;
            }
        }
    }
}
