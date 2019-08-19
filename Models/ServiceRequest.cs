using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Collplex.Models
{
    public class ServiceRequest
    {
        public string ClientId { get; set; }
        public string Key { get; set; }
        public object Data { get; set; }
    }
}
