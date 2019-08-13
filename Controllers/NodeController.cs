using Collplex.Core;
using Collplex.Models;
using Google.Protobuf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Collplex.Models.Node;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Collplex.Controllers
{
    public class NodeController : AppBaseController
    {
        private readonly MainContext context;
        public NodeController(MainContext context) : base(context)
        {
            this.context = context;
        }

        [Route("/node")]
        public ResponsePacket RegisterService()
            => PacketHandler.GenerateResponse(ResponseCodeType.METHOD_NOT_ALLOWED);

        /*
         * 子节请求互入口
         */
        [Route("/node")]
        [HttpPost]
        public async Task<ResponsePacket> NodeMain([FromBody] NodeRequestInbound request)
        {
            if (!PacketHandler.ValidateNodeRequestInbound(request))
                return PacketHandler.GenerateResponse(ResponseCodeType.BAD_REQUEST);

            var requestCollege = await context.Colleges.Where(college => college.ClientId == request.ClientId)
                .Select(college => college)
                .FirstOrDefaultAsync();
            if (requestCollege == null)
                return PacketHandler.GenerateResponse(ResponseCodeType.NODE_INVALID_CLIENT_ID_OR_SECRET);

            string jsonPayload = PacketHandler.DecryptNodeRequestInbound(request, requestCollege.ClientSecret);
            if (jsonPayload == null)
                return PacketHandler.GenerateResponse(ResponseCodeType.BAD_REQUEST);

            NodeActionEnum action = (NodeActionEnum)Enum.Parse(typeof(NodeActionEnum), request.Action.ToUpper());
            switch (action) {
                case NodeActionEnum.REGISTER:
                {
                    InboundRegisterRequest payload = null;
                    try
                    {
                        payload = JsonConvert.DeserializeObject<InboundRegisterRequest>(jsonPayload);
                    }
                    catch
                    {
                        return PacketHandler.GenerateResponse(ResponseCodeType.INVALID_BODY);
                    }
                    return await RegisterService(requestCollege, payload);
                }
                case NodeActionEnum.DESTROY:
                {
                    InboundDestroyRequest payload = null;
                    try
                    {
                        payload = JsonConvert.DeserializeObject<InboundDestroyRequest>(jsonPayload);
                    }
                    catch
                    {
                        return PacketHandler.GenerateResponse(ResponseCodeType.INVALID_BODY);
                    }
                    return await DestroyService(requestCollege, payload);
                }
                case NodeActionEnum.LIST:
                    return await ListServices(request, requestCollege);
                default: return PacketHandler.GenerateResponse(ResponseCodeType.BAD_REQUEST);
            }
        }


        /*  加密接口
         *  [使用用户的 iv 加密返回]
         * 子节点业务注册与续命
         */
        private async Task<ResponsePacket> RegisterService(Colleges college, InboundRegisterRequest data)
        {
            if (data == null || data.Service == null)
                return PacketHandler.GenerateResponse(ResponseCodeType.INVALID_BODY);

            // 锁定节点，可以有效避免并发注册的问题
            string nodeLock = await NodeHelper.LockNode(college.ClientId);
            if (nodeLock == null)
                return PacketHandler.GenerateResponse(ResponseCodeType.NODE_LOCK_TIMEOUT);

            try
            {
                Node node = await NodeHelper.GetNode(college.ClientId);
                if (node == null) node = new Node(); // 注册中心从未有过记录，为其新增记录

                foreach (var serviceToRegister in data.Service) // 用户提交的 Service
                {
                    if (serviceToRegister.Key == null)
                        return PacketHandler.GenerateResponse(ResponseCodeType.INVALID_BODY);

                    serviceToRegister.Key = serviceToRegister.Key.ToLower(); // 业务 Key 强制小写

                    Node.Types.Service service = null;
                    foreach (var currentService in node.Services) // 在注册中心中已经存在的 Service
                    {
                        if (currentService.Key == serviceToRegister.Key)
                        {
                            service = currentService;
                            break;
                        }
                    }
                    if (service == null) // 此业务未注册，填充新服务信息，然后加到注册中心
                    {
                        service = new Node.Types.Service
                        {
                            Key = serviceToRegister.Key,
                            RegTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                            /* 判断 Key 是否在内建的基础业务枚举中，如果是，则标记为基础业务，如果不是，则标记为自定义业务 */
                            Type = Enum.IsDefined(typeof(BaseServiceEnum), serviceToRegister.Key) ? Node.Types.Service.Types.ServiceType.Basic : Node.Types.Service.Types.ServiceType.Custom,
                        };

                        // 自定义业务超出数量限制
                        if (node.Services.Count + Enum.GetValues(typeof(BaseServiceEnum)).Length >= college.MaxUrls)
                            return PacketHandler.GenerateResponse(ResponseCodeType.NODE_REG_CUSTOM_URL_LIMIT);

                        node.Services.Add(service);
                    }

                    // 更新业务信息
                    service.Name = serviceToRegister.Name;
                    service.NodeUrl = serviceToRegister.NodeUrl;

                    // 此业务已经注册且本次存活 / 新注册的业务，为其更新过期时间
                    service.ExpireTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + college.RegInterval;
                }

                // 更新子节点信息
                node.Config.RegInterval = college.RegInterval;
                node.Config.Timeout = college.Timeout;

                if (!await NodeHelper.SetNode(college.ClientId, node))
                    return PacketHandler.GenerateResponse(ResponseCodeType.NODE_OPERATION_FAILED);
                return PacketHandler.GenerateResponse(ResponseCodeType.OK);
            }
            finally
            {
                await NodeHelper.ReleaseNode(college.ClientId, nodeLock);
            }
        }


        /*  加密接口
         *  [使用用户的 iv 加密返回]
         *  子节点销毁业务
         */
        private async Task<ResponsePacket> DestroyService(Colleges college, InboundDestroyRequest data)
        {
            if (data == null || data.Key == null)
                return PacketHandler.GenerateResponse(ResponseCodeType.INVALID_BODY);

            // 锁定节点，可以有效避免并发注册的问题
            string nodeLock = await NodeHelper.LockNode(college.ClientId);
            if (nodeLock == null)
                return PacketHandler.GenerateResponse(ResponseCodeType.NODE_LOCK_TIMEOUT);

            try
            {
                Node node = await NodeHelper.GetNode(college.ClientId);
                if (node == null) // 本身就没有注册任何业务，直接返回空
                    return PacketHandler.GenerateResponse(ResponseCodeType.OK);

                foreach (var key in data.Key)
                {
                    if (key == null)
                        return PacketHandler.GenerateResponse(ResponseCodeType.INVALID_BODY);

                    Node.Types.Service service = null;
                    foreach (var currentService in node.Services) // 在注册中心中已经存在的 Service
                    {
                        if (currentService.Key == key)
                        {
                            service = currentService;
                            break;
                        }
                    }
                    if (service == null) // 当前请求的业务本身未注册
                        continue;        // 跳到下一个请求销毁的业务

                    node.Services.Remove(service); // 销毁业务
                }
                if (!await NodeHelper.SetNode(college.ClientId, node))
                    return PacketHandler.GenerateResponse(ResponseCodeType.NODE_OPERATION_FAILED);
                return PacketHandler.GenerateResponse(ResponseCodeType.OK);
            }
            finally
            {
                await NodeHelper.ReleaseNode(college.ClientId, nodeLock);
            }
        }


        /*  加密接口
         *  [使用用户的 iv 加密返回]
         *  子节点获取当前所注册的业务信息
         */
        private async Task<ResponsePacket> ListServices(NodeRequestInbound request, Colleges college)
        {
            Node node = await NodeHelper.GetNode(college.ClientId);
            if (node == null) node = new Node(); // 没有注册过服务或找不到则不反回 null，而是一个空的服务集合对象
            return PacketHandler.GenerateResponse(ResponseCodeType.OK, PacketHandler.MakeNodeEncryptedPayload(node.Services, college.ClientSecret, request.Iv));
        }
    }
}
