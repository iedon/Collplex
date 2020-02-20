using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Collplex.Models;

namespace Collplex.Models.Node
{
    public class RPCRegisterServicePayload
    {
        public class ServiceRegisterElement
        {
            // 子节点业务 Key
            public string Key { get; set; }

            // 子节点的友好名称
            public string Name { get; set; }

            // 子节点服务所在的 URL
            public string NodeUrl { get; set; }

            // 子节点负载均衡权重
            public int? Weight { get; set; }

            // 子节点的服务是是否只对同主体的其他子节点开放，如果为 False，则子节点的业务外网可达
            public bool Private { get; set; }
        }

        // 子节点注册的业务元组
        public List<ServiceRegisterElement> Services { get; set; }
    }
}
