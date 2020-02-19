using Collplex.Models.Node;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Collplex.Models.Client.Types;

namespace Collplex.Core.LoadBalancing
{
    public static partial class LoadBalancer
    {

        // 业务会话上下文结构 全局静态对象
        public static ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentDictionary<string, SessionContext>>> SessionContexts = new ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentDictionary<string, SessionContext>>>();

        // 通过客户ID，业务键获取业务会话上下文，没有则会新建
        public static ConcurrentDictionary<string, SessionContext> GetKeyContext(string clientId, string serviceKey)
        {
            var clients = SessionContexts.GetOrAdd(clientId, key => new ConcurrentDictionary<string, ConcurrentDictionary<string, SessionContext>>());
            return clients.GetOrAdd(serviceKey, key => new ConcurrentDictionary<string, SessionContext>());
        }

        // 通过客户ID，业务键获取业务会话上下文，没有则会新建
        public static SessionContext GetSessionContext(ConcurrentDictionary<string, SessionContext> keyContext, string hashId)
            => keyContext.GetOrAdd(hashId, key => new SessionContext());


        public static NodeData.Types.NodeService Lease(LoadBalancerConfiguration.Types.LoadBalanceType type,
                                                       List<NodeData.Types.NodeService> services,
                                                       ConcurrentDictionary<string, SessionContext> keyContext,
                                                       int sourceIpAddressHashCode,
                                                       out SessionContext hitSessionContext)
        => type switch
            {
                LoadBalancerConfiguration.Types.LoadBalanceType.NoLoadBalance => NoLoadBalance(services, keyContext, out hitSessionContext),
                LoadBalancerConfiguration.Types.LoadBalanceType.SmoothWeightRoundRobin => SmoothWeightRoundRobin(services, keyContext, out hitSessionContext),
                LoadBalancerConfiguration.Types.LoadBalanceType.LeastRequests => LeastConnections(services, keyContext, out hitSessionContext),
                LoadBalancerConfiguration.Types.LoadBalanceType.Random => Random(services, keyContext, out hitSessionContext),
                LoadBalancerConfiguration.Types.LoadBalanceType.SourceIpHash => SourceIpHash(sourceIpAddressHashCode, services, keyContext, out hitSessionContext),
                _ => NoLoadBalance(services, keyContext, out hitSessionContext),
            };
    }
}
