using Collplex.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Collplex.Models.Node
{
    public class SessionContext
    {
        private int currentLoadBalanceWeight;
        private int currentRequests;
        private long finishedRequests;
        private long failedRequests;

        public SessionContext()
        {
            currentLoadBalanceWeight = 0;
            currentRequests = 0;
            finishedRequests = 0;
            failedRequests = 0;
        }

        public int GetCurrentRequests() => currentRequests;
        public long GetFinishedRequests() => finishedRequests;
        public long GetFailedRequests() => failedRequests;

        public int IncrementCurrentRequests() => Interlocked.Increment(ref currentRequests);
        public int DecrementCurrentRequests() => Interlocked.Decrement(ref currentRequests);
        public long IncrementFinishedRequests() => Interlocked.Increment(ref finishedRequests);
        public long IncrementFailedRequests() => Interlocked.Increment(ref failedRequests);

        public int GetCurrentLoadBalanceWeight() => currentLoadBalanceWeight;
        public int SetCurrentLoadBalanceWeight(int weight) => Interlocked.Exchange(ref currentLoadBalanceWeight, weight);
    }
}
