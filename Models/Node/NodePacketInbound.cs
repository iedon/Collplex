using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Collplex.Models.Node
{
    /* 此类是子节点向本中心节点发送的请求 */
    public class NodePacketInbound
    {
        // 子节点请求签名
        public string Signature;

        // 子节点响应时间戳
        public long Timestamp;

        // 子节点应用ID
        public string ClientId;

        // 子节点请求的业务
        public string Action;

        // 子节点加密初始化向量
        public string Iv;

        // 子节点交互所用到的数据
        public string Data;
    }
}
