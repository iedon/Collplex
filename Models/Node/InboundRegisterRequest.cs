using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Collplex.Models;

namespace Collplex.Models.Node
{
    public class InboundRegisterRequest
    {
        public class RegisterElement
        {
            // 子节点业务 Key
            public string Key;

            // 子节点的友好名称
            public string Name;

            // 子节点服务所在的 URL
            public string NodeUrl;

            // 子节点的服务是否必须经过中心服务器的鉴权后才能被访问
            public bool RequireAuth;
        }

        // 子节点注册的业务元组
        public List<RegisterElement> Service;
    }
}
