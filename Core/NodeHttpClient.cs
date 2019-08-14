using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Collplex.Models;
using Collplex.Models.Node;
using Newtonsoft.Json;

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
        public async Task<object> RequestNodeService(string nodeServiceUrl, object requestData, int timeout, string clientId, string clientSecret)
        {
            Client.Timeout = TimeSpan.FromSeconds(timeout);
            if (!PacketHandler.MakeNodePacketOutbound(requestData, clientId, clientSecret, out NodePacketOutbound packetOutbound, out string iv))
                return null;

            using (var content = new StringContent(
                    JsonConvert.SerializeObject(packetOutbound, Formatting.None, new JsonSerializerSettings
                        {
                            ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
                        }),
                    Encoding.UTF8, "application/json")
                  )
            {
                using (var response = await Client.PostAsync(nodeServiceUrl, content))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        return null;
                    }
                    try
                    {
                        string httpBody = await response.Content.ReadAsStringAsync();
                        ResponsePacket responsePacket = JsonConvert.DeserializeObject<ResponsePacket>(httpBody);
                        if (responsePacket.Code != ResponseCodeType.OK)
                        {
                            return null;
                        }
                        string decryptedData = PacketHandler.CommonDecrypt(responsePacket.Data.ToString(), clientSecret, iv);
                        return JsonConvert.DeserializeObject<object>(decryptedData);
                    }
                    catch
                    {
                        return null;
                    }
                }
            }
        }
    }
}
