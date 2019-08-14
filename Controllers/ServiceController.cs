using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc; // remove this in asp.net core 3.0
using Collplex.Core;
using Collplex.Models;
using Collplex.Models.Node;
using Microsoft.EntityFrameworkCore;

namespace Collplex.Controllers
{
    // 业务控制器
    public class ServiceController : AppBaseController
    {
        private readonly MainContext context;
        private readonly NodeHttpClient httpClient;

        public ServiceController(MainContext context, NodeHttpClient httpClient) : base(context)
        {
            this.context = context;
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

            Node node = await NodeHelper.GetNode(request.ClientId);
            if (node == null)
                return PacketHandler.MakeResponse(ResponseCodeType.SVC_INVALID_CLIENT_ID);

            Node.Types.Service service = null;
            foreach (Node.Types.Service svc in node.Services)
            {
                if (svc.Key == request.Key)
                {
                    service = svc;
                    break;
                }
            }

            if (service == null) // 服务未注册
                return PacketHandler.MakeResponse(ResponseCodeType.SVC_NOT_FOUND);

            if (service.Type == Node.Types.Service.Types.ServiceType.Custom)
            {
                try
                {
                    object data = await httpClient.RequestNodeService(service.NodeUrl, request.Data, node.Config.Timeout, request.ClientId, node.Config.ClientSecret);
                    if (data == null)
                    {
                        return PacketHandler.MakeResponse(ResponseCodeType.NODE_RESPONSE_ERROR, data);
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

            return PacketHandler.MakeResponse(ResponseCodeType.OK, service);
        }
    }
}
