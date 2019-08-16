using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Collplex.Core;
using Collplex.Models;
using Collplex.Models.Node;

namespace Collplex.Controllers
{
    // 业务控制器
    public class ServiceController : AppBaseController
    {
        private readonly NodeHttpClient httpClient;

        public ServiceController(NodeHttpClient httpClient)
        {
            this.httpClient = httpClient;
        }


        [Route("/service")]
        public ResponsePacket ServiceMain()
            => PacketHandler.MakeResponse(ResponseCodeType.METHOD_NOT_ALLOWED);

        /*
         * 业务入口
         */
        [Route("/service")]
        [HttpPost]
        public async Task<ResponsePacket> ServiceMain([FromBody] ServiceRequest request)
        {
            if (!PacketHandler.ValidateRequest(request))
                return PacketHandler.MakeResponse(ResponseCodeType.BAD_REQUEST);

            ClientContext.Types.Client client = await NodeHelper.GetClient(request.ClientId);
            NodeData nodeData = await NodeHelper.GetNodeData(request.ClientId);
            if (client == null || nodeData == null)
                return PacketHandler.MakeResponse(ResponseCodeType.SVC_INVALID_CLIENT_ID);

            NodeData.Types.NodeService service = null;
            foreach (NodeData.Types.NodeService svc in nodeData.Services)
            {
                if (svc.Key == request.Key.ToLower())
                {
                    service = svc;
                    break;
                }
            }

            if (service == null) // 服务未注册
                return PacketHandler.MakeResponse(ResponseCodeType.SVC_NOT_FOUND);

            try
            {
                // 获取并处理最终用户的 User-Agent 头部。
                string userAgentSingle = string.Empty;
                if (HttpContext.Request.Headers.TryGetValue("user-agent", out var uaHeaders) && uaHeaders.Count > 0)
                {
                    // 因为 ASP.NET 允许最终用户有多个头，而 UA 正常情况下只发送一个，因此只取一个值
                    userAgentSingle = uaHeaders.ToArray()[0];
                }
                OutboundRequest outboundRequest = new OutboundRequest()
                {
                    RemoteIp = HttpContext.Connection.RemoteIpAddress.ToString(),
                    RemotePort = HttpContext.Connection.RemotePort,
                    UserAgent = userAgentSingle,
                    Payload = request.Data
                };

                object data = await httpClient.RequestNodeService(service.NodeUrl, outboundRequest, client.Timeout, request.ClientId, client.ClientSecret);
                if (data == null)
                {
                    return PacketHandler.MakeResponse(ResponseCodeType.NODE_RESPONSE_ERROR);
                }
                return PacketHandler.MakeResponse(ResponseCodeType.OK, data);
            }
            catch (TaskCanceledException)
            {
                return PacketHandler.MakeResponse(ResponseCodeType.NODE_RESPONSE_TIMEDOUT);
            }
            catch
            {
                return PacketHandler.MakeResponse(ResponseCodeType.NODE_RESPONSE_ERROR);
            }
        }

    }
}
