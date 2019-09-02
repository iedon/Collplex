using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Collplex.Core;
using Collplex.Models;
using Collplex.Models.Node;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Collplex.Controllers
{
    // 业务控制器
    public class ServiceController : AppBaseController
    {
        private readonly ILogger<ServiceController> logger;
        private readonly NodeHttpClient httpClient;
        private readonly IMongoRepository<ServiceLog> mongoRepository;

        public ServiceController(ILogger<ServiceController> logger, NodeHttpClient httpClient, IMongoRepository<ServiceLog> mongoRepository)
        {
            this.logger = logger;
            this.httpClient = httpClient;
            this.mongoRepository = mongoRepository;
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
            Stopwatch requestWatch = null;
            if (Constants.LogUserRequest)
            {
                // 初始化请求计时器并开始计时处理时间
                requestWatch = new Stopwatch();
                requestWatch.Start();
            }

            // 获取并处理最终用户的 User-Agent 头部。
            string userAgentSingle = string.Empty;
            if (HttpContext.Request.Headers.TryGetValue("user-agent", out var uaHeaders) && uaHeaders.Count > 0)
            {
                // 因为 ASP.NET 允许最终用户有多个头，而 UA 正常情况下只发送一个，因此只取一个值
                userAgentSingle = uaHeaders.ToArray()[0];
            }

            ServiceLog requestLog = null;
            if (Constants.LogUserRequest)
            {
                requestLog = new ServiceLog()
                {
                    RequestBegin = DateTime.Now,
                    Key = request.Key ?? string.Empty,
                    Ip = HttpContext.Connection.RemoteIpAddress.ToString(),
                    UA = userAgentSingle,
                    Data = (Constants.LogUserPayload && request.Data != null) ? request.Data.ToString() : string.Empty,
                    ResponseCode = ResponseCodeType.OK,
                };
            }

            if (!PacketHandler.ValidateRequest(request))
            {
                // Bad Request 不记录日志，以防垃圾信息堆积。
                if (Constants.LogUserRequest) requestWatch.Stop();
                return PacketHandler.MakeResponse(ResponseCodeType.BAD_REQUEST);
            }

            request.ClientId = request.ClientId.ToLower();
            Client client = await NodeHelper.GetClient(request.ClientId);
            NodeData nodeData = await NodeHelper.GetNodeData(request.ClientId);
            if (client == null)
            {
                // clientId 找不到就不记录日志。否则数据库中会充满这些未注册的 clientId 的垃圾数据。
                if (Constants.LogUserRequest) requestWatch.Stop();
                return PacketHandler.MakeResponse(ResponseCodeType.SVC_INVALID_CLIENT_ID);
            }
            if (nodeData == null)
            {
                if (Constants.LogUserRequest) LogRequest(requestLog, request.ClientId, ResponseCodeType.SVC_NOT_FOUND, requestWatch);
                return PacketHandler.MakeResponse(ResponseCodeType.SVC_NOT_FOUND);
            }

            NodeData.Types.NodeService service = null;
            foreach (NodeData.Types.NodeService svc in nodeData.Services)
            {
                if (svc.Key == request.Key.ToLower() && svc.Private == false)
                {
                    service = svc;
                    break;
                }
            }

            if (service == null) // 服务未注册
            {
                if (Constants.LogUserRequest) LogRequest(requestLog, request.ClientId, ResponseCodeType.SVC_NOT_FOUND, requestWatch);
                return PacketHandler.MakeResponse(ResponseCodeType.SVC_NOT_FOUND);
            }

            if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() > service.ExpireTimestamp) // 业务已过期
            {
                if (Constants.LogUserRequest) LogRequest(requestLog, request.ClientId, ResponseCodeType.SVC_UNAVAILABLE, requestWatch);
                return PacketHandler.MakeResponse(ResponseCodeType.SVC_UNAVAILABLE);
            }

            try
            {
                OutboundRequest outboundRequest = new OutboundRequest()
                {
                    RemoteIp = HttpContext.Connection.RemoteIpAddress.ToString(),
                    UserAgent = userAgentSingle,
                    Payload = request.Data
                };

                object data = await httpClient.RequestNodeService(service.NodeUrl, outboundRequest, client.Timeout, request.ClientId, client.ClientSecret);
                if (data == null)
                {
                    if (Constants.LogUserRequest) LogRequest(requestLog, request.ClientId, ResponseCodeType.NODE_RESPONSE_ERROR, requestWatch);
                    return PacketHandler.MakeResponse(ResponseCodeType.NODE_RESPONSE_ERROR);
                }
                if (Constants.LogUserRequest) LogRequest(requestLog, request.ClientId, ResponseCodeType.OK, requestWatch);
                return PacketHandler.MakeResponse(ResponseCodeType.OK, data);
            }
            catch (TaskCanceledException)
            {
                if (Constants.LogUserRequest) LogRequest(requestLog, request.ClientId, ResponseCodeType.NODE_RESPONSE_TIMEDOUT, requestWatch);
                return PacketHandler.MakeResponse(ResponseCodeType.NODE_RESPONSE_TIMEDOUT);
            }
            catch
            {
                if (Constants.LogUserRequest) LogRequest(requestLog, request.ClientId, ResponseCodeType.NODE_NETWORK_EXCEPTION, requestWatch);
                return PacketHandler.MakeResponse(ResponseCodeType.NODE_NETWORK_EXCEPTION);
            }
        }

        private void LogRequest(ServiceLog requestLog, string clientId, ResponseCodeType responseCode, Stopwatch requestWatch)
        {
            try
            {
                requestWatch.Stop();
                requestLog.ResponseCode = responseCode;
                requestLog.TimeTakenMs = requestWatch.ElapsedMilliseconds;
                mongoRepository.Add(requestLog, clientId, requestLog.GetType().Name);
            }
            catch (Exception exception)
            {
                logger.LogError("Error logging service request: " + exception.Message);
            }
        }
    }
}
