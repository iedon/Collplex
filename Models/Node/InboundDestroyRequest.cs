using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Collplex.Models;

namespace Collplex.Models.Node
{
    public class InboundDestroyRequest
    {
        // 子节点的业务 Key 元组
        public List<string> Key;
    }
}
