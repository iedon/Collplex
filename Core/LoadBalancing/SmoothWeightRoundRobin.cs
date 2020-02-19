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
        public static NodeData.Types.NodeService SmoothWeightRoundRobin(List<NodeData.Types.NodeService> services,
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
                // 原始权重之和
                int weightSum = 0;
                // 最大当前权重
                int maxCurrentWeight = 0;
                // 最大当前权重对象下标
                int maxCurrentWeightIndex = 0;
                foreach (NodeData.Types.NodeService service in availableServices)
                {
                    weightSum += service.Weight;
                    SessionContext currentContext = GetSessionContext(keyContext, service.Hash);
                    int currentWeight = currentContext.GetCurrentLoadBalanceWeight();
                    if (currentWeight > maxCurrentWeight)
                    {
                        hitSessionContext = currentContext;
                        maxCurrentWeightIndex = index;
                        maxCurrentWeight = currentWeight;
                    }
                    index++;
                }

                if (maxCurrentWeight != 0)
                {
                    result = availableServices.ElementAt(maxCurrentWeightIndex);

                    // 重新调整权重
                    hitSessionContext.SetCurrentLoadBalanceWeight(maxCurrentWeight - weightSum);
                    foreach (NodeData.Types.NodeService service in availableServices)
                    {
                        SessionContext currentContext = GetSessionContext(keyContext, service.Hash);
                        currentContext.SetCurrentLoadBalanceWeight(currentContext.GetCurrentLoadBalanceWeight() + service.Weight);
                    }
                }
            }
            return result;
        }
    }
}
