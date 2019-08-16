using System;
using System.Collections.Generic;
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
            client.DefaultRequestHeaders.Add("User-Agent", Constants.NodeHttpClientUserAgent);
            Client = client;
        }

        /* 如果任务超时，会由 PostAsync Task 发出 TaskCanceledException */
        public async Task<object> RequestNodeService(string nodeServiceUrl, OutboundRequest outboundRequest, int timeout, string clientId, string clientSecret)
        {
            Client.Timeout = TimeSpan.FromSeconds(timeout);
            if (!PacketHandler.MakeNodePacketOutbound(outboundRequest, clientId, clientSecret, out NodePacketOutbound packetOutbound, out string iv))
                return null;

            using var content = new StringContent(Utils.JsonSerialize(packetOutbound), Encoding.UTF8, Constants.JsonContentType);
            using var response = await Client.PostAsync(nodeServiceUrl, content);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            try
            {
                string httpBody = await response.Content.ReadAsStringAsync();
                ResponsePacket responsePacket = Utils.JsonDeSerialize<ResponsePacket>(httpBody);
                if (responsePacket.Code != ResponseCodeType.OK)
                {
                    return null;
                }
                string decryptedData = Utils.CommonDecrypt(responsePacket.Data.ToString(), clientSecret, iv);
                return Utils.JsonDeSerialize<object>(decryptedData);
            }
            catch
            {
                return null;
            }
        }
    }
}
