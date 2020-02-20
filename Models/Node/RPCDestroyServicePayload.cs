using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Collplex.Models;

namespace Collplex.Models.Node
{
    public class RPCDestroyServicePayload
    {
        public class ServiceDestroyElement {

            // 子节点业务 Key
            public string Key { get; set; }

            // 子节点服务所在的 URL
            public string NodeUrl { get; set; }
        }

        // 子节点的业务 Key 元组
        public List<ServiceDestroyElement> Services { get; set; }
    }
}
