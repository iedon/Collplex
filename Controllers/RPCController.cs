using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Collplex.Core;
using Collplex.Models;
using Collplex.Models.Node;
using Collplex.Core.LoadBalancing;

namespace Collplex.Controllers
{
    public class RPCController : AppBaseController
    {
        public RPCController() {}

        [Route("/rpc")]
        public ResponsePacket RegisterService() => PacketHandler.MakeResponse(ResponseCodeType.METHOD_NOT_ALLOWED);

        /*
         * 子节请求互入口
         */
        [Route("/rpc")]
        [HttpPost]
        public async Task<ResponsePacket> RPCMain([FromBody] RPCRequest request)
        {
            if (!PacketHandler.ValidateRPCRequest(request))
                return PacketHandler.MakeResponse(ResponseCodeType.BAD_REQUEST);

            request.ClientId = request.ClientId.ToLower();
            var client = await NodeHelper.GetClient(request.ClientId);
            if (client == null)
                return PacketHandler.MakeResponse(ResponseCodeType.NODE_INVALID_CLIENT_ID);

            string jsonPayload = PacketHandler.GetRPCPayload(request, client.ClientSecret);
            if (jsonPayload == null)
                return PacketHandler.MakeResponse(ResponseCodeType.BAD_REQUEST);

            RPCActionEnum action = Enum.Parse<RPCActionEnum>(request.Action.ToUpper());
            switch (action) {
                case RPCActionEnum.REGISTER:
                {
                    RPCRegisterServicePayload payload;
                    try
                    {
                        payload = Utils.JsonDeserialize<RPCRegisterServicePayload>(jsonPayload);
                    }
                    catch
                    {
                        return PacketHandler.MakeResponse(ResponseCodeType.INVALID_BODY);
                    }
                    return await RegisterService(request.ClientId, client, payload);
                }
                case RPCActionEnum.LIST:
                    return await ListServices(request.ClientId);
                case RPCActionEnum.GET:
                {
                    RPCGetServicePayload payload;
                    try
                    {
                        payload = Utils.JsonDeserialize<RPCGetServicePayload>(jsonPayload);
                    }
                    catch
                    {
                        return PacketHandler.MakeResponse(ResponseCodeType.INVALID_BODY);
                    }
                    return await GetService(request.ClientId, payload);
                }
                case RPCActionEnum.CALL:
                {
                    RPCCallServicePayload payload;
                    try
                    {
                        payload = Utils.JsonDeserialize<RPCCallServicePayload>(jsonPayload);
                    }
                    catch
                    {
                        return PacketHandler.MakeResponse(ResponseCodeType.INVALID_BODY);
                    }
                    return await CallService(request.ClientId, payload);
                }
                case RPCActionEnum.DESTROY:
                {
                    RPCDestroyServicePayload payload;
                    try
                    {
                        payload = Utils.JsonDeserialize<RPCDestroyServicePayload>(jsonPayload);
                    }
                    catch
                    {
                        return PacketHandler.MakeResponse(ResponseCodeType.INVALID_BODY);
                    }
                    return await DestroyService(request.ClientId, payload);
                }
                default: return PacketHandler.MakeResponse(ResponseCodeType.BAD_REQUEST);
            }
        }


        /*
         * 子节点业务注册与续命
         */
        private async Task<ResponsePacket> RegisterService(string clientId, Client client, RPCRegisterServicePayload data)
        {
            if (data == null || data.Services == null)
                return PacketHandler.MakeResponse(ResponseCodeType.INVALID_BODY);

            // 既然对 NodeData 实现了 Redis 读写锁，为啥还要在这里对请求做加锁？
            // 因为这是一个陷阱，NodeData 读是共享的，虽然写入可以线程安全，但是在高并发下多个注册请求会导致A写入的数据被B覆盖。
            // 因为是业务周期注册，对性能要求不高，所以适合加锁。
            // 因此在网关被 Web 服务器负载均衡情况下，子节点中心网关的 URL 不应该为负载均衡的 URL，需要设置为一个固定的值(或 Stick 具体某一个)
            using (await Constants.NodeEditLock.EnterAsync())
            {
                // 锁定 Redis 节点，避免负载均衡下其他网关并发注册的问题
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
                            if (currentService.Key == serviceToRegister.Key && currentService.NodeUrl == serviceToRegister.NodeUrl)
                            {
                                service = currentService;
                                break;
                            }
                        }

                        if (service == null) // 此业务未注册，填充新服务信息，然后加到注册中心
                        {
                            // 自定义业务超出数量限制(0 代表无限制)
                            if (client.MaxServices != 0 && nodeData.Services.Count >= client.MaxServices)
                                return PacketHandler.MakeResponse(ResponseCodeType.NODE_REG_CUSTOM_SVC_LIMIT);

                            service = new NodeData.Types.NodeService
                            {
                                Hash = Utils.SHA1Hash(serviceToRegister.Key + serviceToRegister.NodeUrl),
                                Key = serviceToRegister.Key,
                                RegTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                            };

                            nodeData.Services.Add(service);
                        }

                        // 更新业务信息
                        service.Name = serviceToRegister.Name;
                        service.NodeUrl = serviceToRegister.NodeUrl;
                        service.Weight = serviceToRegister.Weight ?? (int)Constants.NodeDefaultWeight;
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
        }


        /*
         *  子节点销毁业务
         */
        private async Task<ResponsePacket> DestroyService(string clientId, RPCDestroyServicePayload data)
        {
            if (data == null || data.Services == null)
                return PacketHandler.MakeResponse(ResponseCodeType.INVALID_BODY);

            using (await Constants.NodeEditLock.EnterAsync())
            {
                // 锁定 Redis 节点，避免负载均衡下其他网关并发注册的问题
                string nodeLock = await NodeHelper.LockNode(clientId);
                if (string.IsNullOrEmpty(nodeLock))
                    return PacketHandler.MakeResponse(ResponseCodeType.NODE_LOCK_TIMEOUT);

                try
                {
                    NodeData nodeData = await NodeHelper.GetNodeData(clientId);
                    if (nodeData == null) // 本身就没有注册任何业务，直接返回空
                        return PacketHandler.MakeResponse(ResponseCodeType.OK);

                    foreach (var serviceToDestroy in data.Services)
                    {
                        if (string.IsNullOrEmpty(serviceToDestroy.Key))
                            return PacketHandler.MakeResponse(ResponseCodeType.INVALID_BODY);

                        NodeData.Types.NodeService service = null;
                        foreach (var currentService in nodeData.Services) // 在注册中心中已经存在的 Service
                        {
                            if (currentService.Key == serviceToDestroy.Key && currentService.NodeUrl == serviceToDestroy.NodeUrl)
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
        }


        /*
         *  子节点获取当前所注册的业务信息
         */
        private async Task<ResponsePacket> ListServices(string clientId)
        {
            NodeData nodeData = await NodeHelper.GetNodeData(clientId);
            if (nodeData == null) nodeData = new NodeData(); // 没有找到节点，就建一个空的临时对象，用于序列化空服务响应
            return PacketHandler.MakeResponse(ResponseCodeType.OK, Utils.JsonSerialize(nodeData.Services));
        }


        /*
         *  子节点获取某业务的信息
         */
        private async Task<ResponsePacket> GetService(string clientId, RPCGetServicePayload data)
        {
            NodeData nodeData = await NodeHelper.GetNodeData(clientId);
            if (nodeData == null)
            {
                return PacketHandler.MakeResponse(ResponseCodeType.NODE_OPERATION_FAILED);
            }
            var relatedServices = new List<NodeData.Types.NodeService>();
            foreach (var currentService in nodeData.Services)
            {
                if (currentService.Key == data.Key) relatedServices.Add(currentService);
            }
            return PacketHandler.MakeResponse(ResponseCodeType.OK, Utils.JsonSerialize(relatedServices));
        }


        /*
         *  子节点调用同节点下其他业务，返回经过负载均衡器选择后的 URL
         */
        private async Task<ResponsePacket> CallService(string clientId, RPCCallServicePayload data)
        {
            Client client = await NodeHelper.GetClient(clientId);
            if (client == null)
            {
                return PacketHandler.MakeResponse(ResponseCodeType.NODE_OPERATION_FAILED);
            }

            NodeData nodeData = await NodeHelper.GetNodeData(clientId);
            if (nodeData == null)
            {
                return PacketHandler.MakeResponse(ResponseCodeType.NODE_OPERATION_FAILED);
            }

            var relatedServices = new List<NodeData.Types.NodeService>();
            foreach (var currentService in nodeData.Services)
            {
                if (currentService.Key == data.Key) relatedServices.Add(currentService);
            }

            var keyContext = LoadBalancer.GetKeyContext(clientId, data.Key);
            Client.Types.LoadBalancerConfiguration.Types.LoadBalanceType loadBalancerType = Client.Types.LoadBalancerConfiguration.Types.LoadBalanceType.NoLoadBalance;
            foreach (var config in client.LoadBalancerConfigurations)
            {
                if (config.Key == data.Key)
                {
                    loadBalancerType = config.Type;
                    break;
                }
            }
            NodeData.Types.NodeService serviceToUse = LoadBalancer.Lease(loadBalancerType, relatedServices, keyContext, HttpContext.Connection.RemoteIpAddress.GetHashCode(), out _);
            if (serviceToUse == null) // 负载均衡器返回无可用业务备选 (业务已过期)
            {
                return PacketHandler.MakeResponse(ResponseCodeType.SVC_UNAVAILABLE);
            }

            return PacketHandler.MakeResponse(ResponseCodeType.OK, serviceToUse.NodeUrl);
        }
    }
}
