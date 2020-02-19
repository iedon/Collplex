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
        public static NodeData.Types.NodeService SourceIpHash(int sourceIpAddressHashCode,
                                                        List<NodeData.Types.NodeService> services,
                                                        ConcurrentDictionary<string, SessionContext> keyContext,
                                                        out SessionContext hitSessionContext)
        {
            var availableServices = services.Where(s => s.ExpireTimestamp > DateTimeOffset.UtcNow.ToUnixTimeSeconds()).Select(s => s);
            var count = availableServices.Count();
            NodeData.Types.NodeService result = null;
            if (count == 0)
            {
                hitSessionContext = null;
            }
            else
            {
                int length = availableServices.Count();
                int hashCode = Math.Abs(sourceIpAddressHashCode);
                int index = hashCode % length;
                result = availableServices.ElementAt(index);
                hitSessionContext = GetSessionContext(keyContext, result.Hash);
            }
            return result;
        }
    }
}
