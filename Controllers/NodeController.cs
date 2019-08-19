using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Collplex.Core;
using Collplex.Models;
using Collplex.Models.Node;

namespace Collplex.Controllers
{
    public class NodeController : AppBaseController
    {
        public NodeController() {}

        [Route("/node")]
        public ResponsePacket RegisterService()
            => PacketHandler.MakeResponse(ResponseCodeType.METHOD_NOT_ALLOWED);

        /*
         * 子节请求互入口
         */
        [Route("/node")]
        [HttpPost]
        public async Task<ResponsePacket> NodeMain([FromBody] NodePacketInbound request)
        {
            if (!PacketHandler.ValidateNodePacketInbound(request))
                return PacketHandler.MakeResponse(ResponseCodeType.BAD_REQUEST);

            request.ClientId = request.ClientId.ToLower();
            var client = await NodeHelper.GetClient(request.ClientId);
            if (client == null)
                return PacketHandler.MakeResponse(ResponseCodeType.NODE_INVALID_CLIENT_ID);

            string jsonPayload = PacketHandler.DecryptNodePacketInbound(request, client.ClientSecret);
            if (jsonPayload == null)
                return PacketHandler.MakeResponse(ResponseCodeType.BAD_REQUEST);

            NodeActionEnum action = Enum.Parse<NodeActionEnum>(request.Action.ToUpper());
            switch (action) {
                case NodeActionEnum.REGISTER:
                {
                    InboundRegisterRequest payload;
                    try
                    {
                        payload = Utils.JsonDeSerialize<InboundRegisterRequest>(jsonPayload);
                    }
                    catch
                    {
                        return PacketHandler.MakeResponse(ResponseCodeType.INVALID_BODY);
                    }
                    return await RegisterService(request.ClientId, client, payload);
                }
                case NodeActionEnum.DESTROY:
                {
                    InboundDestroyRequest payload;
                    try
                    {
                        payload = Utils.JsonDeSerialize<InboundDestroyRequest>(jsonPayload);
                    }
                    catch
                    {
                        return PacketHandler.MakeResponse(ResponseCodeType.INVALID_BODY);
                    }
                    return await DestroyService(request.ClientId, payload);
                }
                case NodeActionEnum.LIST:
                    return await ListServices(client, request);
                default: return PacketHandler.MakeResponse(ResponseCodeType.BAD_REQUEST);
            }
        }


        /*  加密接口
         *  [使用用户的 iv 加密返回]
         * 子节点业务注册与续命
         */
        private async Task<ResponsePacket> RegisterService(string clientId, Client client, InboundRegisterRequest data)
        {
            if (data == null || data.Services == null)
                return PacketHandler.MakeResponse(ResponseCodeType.INVALID_BODY);

            // 锁定节点，可以有效避免并发注册的问题
            string nodeLock = await NodeHelper.LockNode(clientId);
            if (string.IsNullOrEmpty(nodeLock))
                return PacketHandler.MakeResponse(ResponseCodeType.NODE_LOCK_TIMEOUT);

            try
            {
                NodeData nodeData = await NodeHelper.GetNodeData(clientId);
                if (nodeData == null) nodeData = new NodeData();  // 注册中心从未有过记录，为其新增记录

                foreach (var serviceToRegister in data.Services) // 用户提交的 Service
                {
                    if (string.IsNullOrEmpty(serviceToRegister.Key) || string.IsNullOrEmpty(serviceToRegister.Name) || string.IsNullOrEmpty(serviceToRegister.NodeUrl))
                        return PacketHandler.MakeResponse(ResponseCodeType.INVALID_BODY);

                    serviceToRegister.Key = serviceToRegister.Key.ToLower(); // 业务 Key 强制小写

                    NodeData.Types.NodeService service = null;
                    foreach (var currentService in nodeData.Services) // 在注册中心中已经存在的 Service
                    {
                        if (currentService.Key == serviceToRegister.Key)
                        {
                            service = currentService;
                            break;
                        }
                    }

                    if (service == null) // 此业务未注册，填充新服务信息，然后加到注册中心
                    {
                        // 自定义业务超出数量限制
                        if (nodeData.Services.Count >= client.MaxServices)
                            return PacketHandler.MakeResponse(ResponseCodeType.NODE_REG_CUSTOM_SVC_LIMIT);

                        service = new NodeData.Types.NodeService
                        {
                            Key = serviceToRegister.Key,
                            RegTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                        };

                        nodeData.Services.Add(service);
                    }

                    // 更新业务信息
                    service.Name = serviceToRegister.Name;
                    service.NodeUrl = serviceToRegister.NodeUrl;
                    service.Private = serviceToRegister.Private;

                    // 此业务已经注册且本次存活 / 新注册的业务，为其更新过期时间
                    service.ExpireTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + client.RegIntervalSeconds;
                }

                if (!await NodeHelper.SetNodeData(clientId, nodeData))
                    return PacketHandler.MakeResponse(ResponseCodeType.NODE_OPERATION_FAILED);
                return PacketHandler.MakeResponse(ResponseCodeType.OK);
            }
            finally
            {
                await NodeHelper.ReleaseNode(clientId, nodeLock);
            }
        }


        /*  加密接口
         *  [使用用户的 iv 加密返回]
         *  子节点销毁业务
         */
        private async Task<ResponsePacket> DestroyService(string clientId, InboundDestroyRequest data)
        {
            if (data == null || data.Keys == null)
                return PacketHandler.MakeResponse(ResponseCodeType.INVALID_BODY);

            // 锁定节点，可以有效避免并发注册的问题
            string nodeLock = await NodeHelper.LockNode(clientId);
            if (string.IsNullOrEmpty(nodeLock))
                return PacketHandler.MakeResponse(ResponseCodeType.NODE_LOCK_TIMEOUT);

            try
            {
                NodeData nodeData = await NodeHelper.GetNodeData(clientId);
                if (nodeData == null) // 本身就没有注册任何业务，直接返回空
                    return PacketHandler.MakeResponse(ResponseCodeType.OK);

                foreach (var key in data.Keys)
                {
                    if (string.IsNullOrEmpty(key))
                        return PacketHandler.MakeResponse(ResponseCodeType.INVALID_BODY);

                    NodeData.Types.NodeService service = null;
                    foreach (var currentService in nodeData.Services) // 在注册中心中已经存在的 Service
                    {
                        if (currentService.Key == key)
                        {
                            service = currentService;
                            break;
                        }
                    }
                    if (service == null) // 当前请求的业务本身未注册
                        continue;        // 跳到下一个请求销毁的业务

                    nodeData.Services.Remove(service); // 销毁业务
                }
                if (!await NodeHelper.SetNodeData(clientId, nodeData))
                    return PacketHandler.MakeResponse(ResponseCodeType.NODE_OPERATION_FAILED);
                return PacketHandler.MakeResponse(ResponseCodeType.OK);
            }
            finally
            {
                await NodeHelper.ReleaseNode(clientId, nodeLock);
            }
        }


        /*  加密接口
         *  [使用用户的 iv 加密返回]
         *  子节点获取当前所注册的业务信息
         */
        private async Task<ResponsePacket> ListServices(Client client, NodePacketInbound request)
        {
            NodeData nodeData = await NodeHelper.GetNodeData(request.ClientId);
            if (nodeData == null) nodeData = new NodeData(); // 没有找到节点，就建一个空的临时对象，用于序列化空服务响应
            return PacketHandler.MakeResponse(ResponseCodeType.OK, PacketHandler.MakeNodeEncryptedPayload(nodeData.Services, client.ClientSecret, request.Iv));
        }
    }
}
