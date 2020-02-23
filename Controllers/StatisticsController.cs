using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Collplex.Core;
using Collplex.Models;
using Collplex.Core.LoadBalancing;
using static Collplex.Models.ResponsePacket.Types;

namespace Collplex.Controllers
{
    // 统计信息控制器
    [Produces(Constants.JsonContentType)]
    public class StatisticsController : AppBaseController
    {
        [Route("~/statistics")]
        public object StatisticsMain()
        {
            var response = new Dictionary<string, Dictionary<string, Dictionary<string, Statistics> > >();
            foreach (var client in LoadBalancer.SessionContexts)
            {
                var serviceMap = new Dictionary<string, Dictionary<string, Statistics> >();
                foreach (var service in client.Value)
                {
                    var hashMap = new Dictionary<string, Statistics>();
                    foreach (var hash in service.Value)
                    {
                        hashMap.Add(hash.Key, new Statistics {
                             CurrentRequests = hash.Value.GetCurrentRequests(),
                             FailedRequests = hash.Value.GetFailedRequests(),
                             FinishedRequests = hash.Value.GetFinishedRequests()
                        });
                    }
                    serviceMap.Add(service.Key, hashMap);
                }
                response.Add(client.Key, serviceMap);
            }
            return PacketHandler.MakeResponse(ResponseCodeType.Ok, response);
        }

    }
}
