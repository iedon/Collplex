using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Collplex.Core;
using Collplex.Models;
using Collplex.Models.Node;
using Collplex.Core.LoadBalancing;
using static Collplex.Models.ResponsePacket.Types;
using static Collplex.Models.Client.Types.LoadBalancerConfiguration.Types;
using static Collplex.Models.Node.NodeData.Types;

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

        [Route("~/")]
        public object ServiceMain()
            => PacketHandler.MakeResponse(ResponseCodeType.MethodNotAllowed);

        /*
         * 业务入口
         */
        [Route("~/")]
        [HttpPost]
        public async Task<object> ServiceMain([FromBody] ServiceRequest request)
        {
            if (!PacketHandler.ValidateServiceRequest(request))
            {
                // Bad Request 不记录日志，以防垃圾信息堆积。
                return PacketHandler.MakeResponse(ResponseCodeType.BadRequest);
            }

            request.Key = (request.Key ?? string.Empty).ToLower();
            request.ClientId = request.ClientId.ToLower();

            Client client = await NodeHelper.GetClient(request.ClientId);
            if (client == null)
            {
                // clientId 找不到就不记录日志。否则数据库中会充满这些未注册的 clientId 的垃圾数据。
                return PacketHandler.MakeResponse(ResponseCodeType.SvcInvalidClientId);
            }

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
                    Key = request.Key,
                    Ip = HttpContext.Connection.RemoteIpAddress.ToString(),
                    UA = userAgentSingle,
                    Data = (Constants.LogUserPayload && request.Data != null) ? request.Data.ToString() : string.Empty,
                    ResponseCode = ResponseCodeType.Ok,
                };
            }

            NodeData nodeData = await NodeHelper.GetNodeData(request.ClientId);
            if (nodeData == null)
            {
                if (Constants.LogUserRequest) LogRequest(requestLog, request.ClientId, ResponseCodeType.SvcNotFound, requestWatch);
                return PacketHandler.MakeResponse(ResponseCodeType.SvcNotFound);
            }

            // 找到业务 Key 与其注册的所有 URL 以便负载均衡器选择
            var services = new List<NodeData.Types.NodeService>();
            foreach (NodeData.Types.NodeService svc in nodeData.Services)
            {
                if (svc.Key == request.Key && svc.Private == false) services.Add(svc);
            }

            if (services.Count == 0) // 服务未注册(一个URL都没有注册)
            {
                if (Constants.LogUserRequest) LogRequest(requestLog, request.ClientId, ResponseCodeType.SvcNotFound, requestWatch);
                return PacketHandler.MakeResponse(ResponseCodeType.SvcNotFound);
            }

            var keyContext = LoadBalancer.GetKeyContext(request.ClientId, request.Key);
            LoadBalanceType loadBalancerType = LoadBalanceType.NoLoadBalance;
            foreach (var config in client.LoadBalancerConfigurations)
            {
                if (config.Key == request.Key) {
                    loadBalancerType = config.Type;
                    break;
                }
            }
            NodeService serviceToUse = LoadBalancer.Lease(loadBalancerType, services, keyContext, HttpContext.Connection.RemoteIpAddress.GetHashCode(), out var hitSessionContext);
            if (serviceToUse == null) // 负载均衡器返回无可用业务备选 (业务已过期)
            {
                if (Constants.LogUserRequest) LogRequest(requestLog, request.ClientId, ResponseCodeType.SvcUnavailable, requestWatch);
                return PacketHandler.MakeResponse(ResponseCodeType.SvcUnavailable);
            }

            hitSessionContext.IncrementCurrentRequests();
            try
            {
                var remoteHeaders = new Dictionary<string, string[]>();
                foreach (var header in HttpContext.Request.Headers)
                {
                    remoteHeaders.Add(header.Key, header.Value.ToArray());
                }
                var data = await httpClient.RequestNodeService(new Uri(serviceToUse.NodeUrl), request.Data, client.Timeout, request.ClientId, client.ClientSecret, HttpContext.Connection.RemoteIpAddress.ToString(), HttpContext.Connection.RemotePort, remoteHeaders);
                if (data == null)
                {
                    if (Constants.LogUserRequest) LogRequest(requestLog, request.ClientId, ResponseCodeType.NodeResponseError, requestWatch);
                    hitSessionContext.IncrementFailedRequests();
                    return PacketHandler.MakeResponse(ResponseCodeType.NodeResponseError);
                }

                if (Constants.LogUserRequest) LogRequest(requestLog, request.ClientId, ResponseCodeType.Ok, requestWatch);
                return PacketHandler.MakeResponse(ResponseCodeType.Ok, data);
            }
            catch (TaskCanceledException)
            {
                hitSessionContext.IncrementFailedRequests();
                if (Constants.LogUserRequest) LogRequest(requestLog, request.ClientId, ResponseCodeType.NodeResponseTimedout, requestWatch);
                return PacketHandler.MakeResponse(ResponseCodeType.NodeResponseTimedout);
            }
            catch
            {
                hitSessionContext.IncrementFailedRequests();
                if (Constants.LogUserRequest) LogRequest(requestLog, request.ClientId, ResponseCodeType.NodeNetworkException, requestWatch);
                return PacketHandler.MakeResponse(ResponseCodeType.NodeNetworkException);
            }
            finally
            {
                hitSessionContext.DecrementCurrentRequests();
                hitSessionContext.IncrementFinishedRequests();
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
