﻿using Collplex.Models.Node;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Collplex.Core.LoadBalancing
{
    public static partial class LoadBalancer
    {
        private static readonly Random random = new Random();
        public static NodeData.Types.NodeService Random(List<NodeData.Types.NodeService> services,
                                                        ConcurrentDictionary<string, SessionContext> keyContext,
                                                        out SessionContext hitSessionContext)
        {
            var availableServices = services.Where(s => s.ExpireTimestamp > DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            var count = availableServices.Count();
            NodeData.Types.NodeService result;
            if (count == 0)
            {
                result = services.FirstOrDefault();
            }
            else
            {
                result = availableServices.ElementAt(random.Next(count));
            }
            hitSessionContext = GetSessionContext(keyContext, result.Hash);
            return result;
        }
    }
}
