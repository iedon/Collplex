using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Collplex.Models.Node
{
    public class OutboundRequest
    {
        public string RemoteIp { get; set; }
        public int RemotePort { get; set; }
        public string UserAgent { get; set; }
        public object Payload { get; set; }
    }
}
