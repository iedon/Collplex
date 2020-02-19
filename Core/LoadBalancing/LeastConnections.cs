using Collplex.Models.Node;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Collplex.Core.LoadBalancing
{
    public static partial class LoadBalancer
    {
        public static NodeData.Types.NodeService LeastConnections(List<NodeData.Types.NodeService> services,
                                                                  ConcurrentDictionary<string, SessionContext> keyContext,
                                                                  out SessionContext hitSessionContext)
        {
            NodeData.Types.NodeService result = services.FirstOrDefault();
            hitSessionContext = GetSessionContext(keyContext, result.Hash);
            var availableServices = services.Where(s => s.ExpireTimestamp > DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            var count = availableServices.Count();
            if (count != 0)
            {
                int index = 0;
                int leastConnectionsIndex = 0;
                int leastConnections = int.MaxValue;
                foreach (NodeData.Types.NodeService service in availableServices)
                {
                    SessionContext currentContext = GetSessionContext(keyContext, service.Hash);
                    int currentRequests = currentContext.GetCurrentRequests();
                    if (currentRequests < leastConnections)
                    {
                        hitSessionContext = currentContext;
                        leastConnectionsIndex = index;
                        leastConnections = currentRequests;
                    }
                    index++;
                }
                if (leastConnections != int.MaxValue)
                    result = availableServices.ElementAt(leastConnectionsIndex);
            }
            return result;
        }
    }
}
