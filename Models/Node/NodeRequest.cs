using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Collplex.Models.Node
{
    /* 此类是本中心节点向子节点发送的请求 */
    public class NodeRequest
    {
        // 中心节点请求签名
        public string Signature { get; set; }

        // 中心节点响应时间戳
        public long Timestamp { get; set; }

        // 请求数据
        public string Data { get; set; }
    }
}
