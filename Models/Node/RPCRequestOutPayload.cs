using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Collplex.Models.Node
{
    public class RPCRequestOutPayload
    {
        // 远程用户 IP
        public string RemoteIp { get; set; }

        // 远程用户端口
        public int RemotePort { get; set; }

        // 远程用户 HTTP 请求头
        public Dictionary<string, string[]> RemoteHeaders { get; set; }

        // 数据
        public object Data { get; set; }
    }
}
