using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Collplex.Core;
using Collplex.Models;
using Collplex.Models.Node;
using Collplex.Core.LoadBalancing;
using static Collplex.Models.ResponsePacket.Types;
using static Collplex.Models.Node.NodeData.Types;
using static Collplex.Models.Node.RPCRequestIn.Types;

namespace Collplex.Controllers
{
    /* RPC 业务控制器 */
    [Produces(Constants.ProtobufContentType)]
    public class RPCController : AppBaseController
    {
        public RPCController() {}

        [Route("~/rpc")]
        public IActionResult ServiceMain()
            => File(PacketHandler.MakeRPCResponse(ResponseCodeType.MethodNotAllowed), Constants.ProtobufContentType);

        /*
         * 子节请求互入口
         */
        [Route("~/rpc")]
        [HttpPost]
        public async Task<IActionResult> RPCMain()
        {
            RPCRequestIn request;
            try
            {
                using var stream = Request.BodyReader.AsStream();
                request = RPCRequestIn.Parser.ParseFrom(stream);
            }
            catch
            {
                return File(PacketHandler.MakeRPCResponse(ResponseCodeType.BadRequest), Constants.ProtobufContentType);
            }

            if (!PacketHandler.ValidateRPCRequest(request))
                return File(PacketHandler.MakeRPCResponse(ResponseCodeType.BadRequest), Constants.ProtobufContentType);

            request.ClientId = request.ClientId.ToLower();
            var client = await NodeHelper.GetClient(request.ClientId);
            if (client == null)
                return File(PacketHandler.MakeRPCResponse(ResponseCodeType.NodeInvalidClientId), Constants.ProtobufContentType);

            string jsonPayload = PacketHandler.GetRPCPayload(request, client.ClientSecret);
            if (jsonPayload == null)
                return File(PacketHandler.MakeRPCResponse(ResponseCodeType.BadRequest), Constants.ProtobufContentType);

            switch (request.Action) {
                case RPCActionType.Register:
                {
                    RPCRegisterServicePayload payload;
                    try
                    {
                        payload = Utils.JsonDeserialize<RPCRegisterServicePayload>(jsonPayload);
                    }
                    catch
                    {
                        return File(PacketHandler.MakeRPCResponse(ResponseCodeType.InvalidBody), Constants.ProtobufContentType);
                        }
                    return File(await RegisterService(request.ClientId, client, payload), Constants.ProtobufContentType);
                    }
                case RPCActionType.List:
                    return File(await ListServices(request.ClientId), Constants.ProtobufContentType);
                case RPCActionType.Get:
                {
                    RPCGetServicePayload payload;
                    try
                    {
                        payload = Utils.JsonDeserialize<RPCGetServicePayload>(jsonPayload);
                    }
                    catch
                    {
                        return File(PacketHandler.MakeRPCResponse(ResponseCodeType.InvalidBody), Constants.ProtobufContentType);
                        }
                    return File(await GetService(request.ClientId, payload), Constants.ProtobufContentType);
                    }
                case RPCActionType.Call:
                {
                    RPCCallServicePayload payload;
                    try
                    {
                        payload = Utils.JsonDeserialize<RPCCallServicePayload>(jsonPayload);
                    }
                    catch
                    {
                        return File(PacketHandler.MakeRPCResponse(ResponseCodeType.InvalidBody), Constants.ProtobufContentType);
                        }
                    return File(await CallService(request.ClientId, payload), Constants.ProtobufContentType);
                    }
                case RPCActionType.Destroy:
                {
                    RPCDestroyServicePayload payload;
                    try
                    {
                        payload = Utils.JsonDeserialize<RPCDestroyServicePayload>(jsonPayload);
                    }
                    catch
                    {
                        return File(PacketHandler.MakeRPCResponse(ResponseCodeType.InvalidBody), Constants.ProtobufContentType);
                        }
                    return File(await DestroyService(request.ClientId, payload), Constants.ProtobufContentType);
                    }
                default: return File(PacketHandler.MakeRPCResponse(ResponseCodeType.BadRequest), Constants.ProtobufContentType);
            }
        }


        /*
         * 子节点业务注册与续命
         */
        private async Task<byte[]> RegisterService(string clientId, Client client, RPCRegisterServicePayload data)
        {
            if (data == null || data.Services == null)
                return PacketHandler.MakeRPCResponse(ResponseCodeType.InvalidBody);

            // 锁定 Redis 节点，避免负载均衡下其他网关并发注册的问题
            string nodeLock = await NodeHelper.LockNode(clientId);
            if (string.IsNullOrEmpty(nodeLock))
                return PacketHandler.MakeRPCResponse(ResponseCodeType.NodeLockTimeout);

            try
            {
                NodeData nodeData = await NodeHelper.GetNodeData(clientId);
                if (nodeData == null) nodeData = new NodeData();  // 注册中心从未有过记录，为其新增记录

                foreach (var serviceToRegister in data.Services) // 用户提交的 Service
                {
                    if (string.IsNullOrEmpty(serviceToRegister.Key) || string.IsNullOrEmpty(serviceToRegister.Name) || string.IsNullOrEmpty(serviceToRegister.NodeUrl))
                        return PacketHandler.MakeRPCResponse(ResponseCodeType.InvalidBody);

                    serviceToRegister.Key = serviceToRegister.Key.ToLower(); // 业务 Key 强制小写

                    NodeService service = null;
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
                            return PacketHandler.MakeRPCResponse(ResponseCodeType.NodeRegLimit);

                        service = new NodeService
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
                    return PacketHandler.MakeRPCResponse(ResponseCodeType.NodeOperationFailed);
                return PacketHandler.MakeRPCResponse(ResponseCodeType.Ok);
            }
            finally
            {
                await NodeHelper.ReleaseNode(clientId, nodeLock);
            }
        }


        /*
         *  子节点销毁业务
         */
        private async Task<byte[]> DestroyService(string clientId, RPCDestroyServicePayload data)
        {
            if (data == null || data.Services == null)
                return PacketHandler.MakeRPCResponse(ResponseCodeType.InvalidBody);

            // 锁定 Redis 节点，避免负载均衡下其他网关并发注册的问题
            string nodeLock = await NodeHelper.LockNode(clientId);
            if (string.IsNullOrEmpty(nodeLock))
                return PacketHandler.MakeRPCResponse(ResponseCodeType.NodeLockTimeout);

            try
            {
                NodeData nodeData = await NodeHelper.GetNodeData(clientId);
                if (nodeData == null) // 本身就没有注册任何业务，直接返回空
                    return PacketHandler.MakeRPCResponse(ResponseCodeType.Ok);

                foreach (var serviceToDestroy in data.Services)
                {
                    if (string.IsNullOrEmpty(serviceToDestroy.Key))
                        return PacketHandler.MakeRPCResponse(ResponseCodeType.InvalidBody);

                    NodeService service = null;
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
                    return PacketHandler.MakeRPCResponse(ResponseCodeType.NodeOperationFailed);
                return PacketHandler.MakeRPCResponse(ResponseCodeType.Ok);
            }
            finally
            {
                await NodeHelper.ReleaseNode(clientId, nodeLock);
            }
        }


        /*
         *  子节点获取当前所注册的业务信息
         */
        private async Task<byte[]> ListServices(string clientId)
        {
            NodeData nodeData = await NodeHelper.GetNodeData(clientId);
            if (nodeData == null) nodeData = new NodeData(); // 没有找到节点，就建一个空的临时对象，用于序列化空服务响应
            return PacketHandler.MakeRPCResponse(ResponseCodeType.Ok, Utils.JsonSerialize(nodeData.Services));
        }


        /*
         *  子节点获取某业务的信息
         */
        private async Task<byte[]> GetService(string clientId, RPCGetServicePayload data)
        {
            NodeData nodeData = await NodeHelper.GetNodeData(clientId);
            if (nodeData == null)
            {
                return PacketHandler.MakeRPCResponse(ResponseCodeType.NodeOperationFailed);
            }
            var relatedServices = new List<NodeService>();
            foreach (var currentService in nodeData.Services)
            {
                if (currentService.Key == data.Key) relatedServices.Add(currentService);
            }
            return PacketHandler.MakeRPCResponse(ResponseCodeType.Ok, Utils.JsonSerialize(relatedServices));
        }


        /*
         *  子节点调用同节点下其他业务，返回经过负载均衡器选择后的 URL
         */
        private async Task<byte[]> CallService(string clientId, RPCCallServicePayload data)
        {
            Client client = await NodeHelper.GetClient(clientId);
            if (client == null)
            {
                return PacketHandler.MakeRPCResponse(ResponseCodeType.NodeOperationFailed);
            }

            NodeData nodeData = await NodeHelper.GetNodeData(clientId);
            if (nodeData == null)
            {
                return PacketHandler.MakeRPCResponse(ResponseCodeType.NodeOperationFailed);
            }

            var relatedServices = new List<NodeService>();
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
            NodeService serviceToUse = LoadBalancer.Lease(loadBalancerType, relatedServices, keyContext, HttpContext.Connection.RemoteIpAddress.GetHashCode(), out var hitSessionContext);
            if (serviceToUse == null) // 负载均衡器返回无可用业务备选 (业务已过期)
            {
                return PacketHandler.MakeRPCResponse(ResponseCodeType.SvcUnavailable);
            }
            hitSessionContext.IncrementFinishedRequests();
            return PacketHandler.MakeRPCResponse(ResponseCodeType.Ok, serviceToUse.NodeUrl);
        }
    }
}
