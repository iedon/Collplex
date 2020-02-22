using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Collplex.Models
{
    public class Statistics
    {
        public int CurrentRequests { get; set; }
        public long FinishedRequests { get; set; }
        public long FailedRequests { get; set; }
    }
}
