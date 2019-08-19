using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Collplex.Models;

namespace Collplex.Models.Node
{
    public class InboundRegisterRequest
    {
        public class ServiceRegisterElement
        {
            // 子节点业务 Key
            public string Key;

            // 子节点的友好名称
            public string Name;

            // 子节点服务所在的 URL
            public string NodeUrl;

            // 子节点的服务是是否只对同主体的其他子节点开放，如果为 False，则子节点的业务外网可达
            public bool Private;
        }

        // 子节点注册的业务元组
        public List<ServiceRegisterElement> Services;
    }
}
