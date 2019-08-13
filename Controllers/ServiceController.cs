using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
        public ServiceController(MainContext context) : base(context)
        {
            this.context = context;
        }

        [Route("/service")]
        public ResponsePacket ServiceMain()
            => PacketHandler.GenerateResponse(ResponseCodeType.METHOD_NOT_ALLOWED);

        /*
         * 业务入口
         */
        [Route("/service")]
        [HttpPost]
        public async Task<ResponsePacket> ServiceMain([FromBody] ServiceRequest request)
        {
            if (!PacketHandler.ValidateRequest(request))
                return PacketHandler.GenerateResponse(ResponseCodeType.BAD_REQUEST);

            Node node = await NodeHelper.GetNode(request.ClientId);
            if (node == null)
                return PacketHandler.GenerateResponse(ResponseCodeType.SVC_INVALID_CLIENT_ID);

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
                return PacketHandler.GenerateResponse(ResponseCodeType.NOT_FOUND);

            if (service.Type == Node.Types.Service.Types.ServiceType.Custom)
            {

            }

            return PacketHandler.GenerateResponse(ResponseCodeType.OK, service);
        }
    }
}
