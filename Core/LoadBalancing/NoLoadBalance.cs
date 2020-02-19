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
        public static NodeData.Types.NodeService NoLoadBalance(List<NodeData.Types.NodeService> services,
                                                        ConcurrentDictionary<string, SessionContext> keyContext,
                                                        out SessionContext hitSessionContext)
        {
            NodeData.Types.NodeService result = services.Where(s => s.ExpireTimestamp > DateTimeOffset.UtcNow.ToUnixTimeSeconds()).FirstOrDefault();
            if (result == null)
            {
                hitSessionContext = null;
            }
            else
            {
                hitSessionContext = GetSessionContext(keyContext, result.Hash);
            }
            return result;
        }
    }
}
