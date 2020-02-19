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
        private long activeTimestamp;

        public SessionContext()
        {
            currentLoadBalanceWeight = (int)Constants.NodeDefaultWeight;
            currentRequests = 0;
            finishedRequests = 0;
            activeTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        public int GetCurrentRequests() => currentRequests;
        public long GetFinishedRequests() => finishedRequests;
        public long GetActiveTimestamp() => activeTimestamp;

        public int IncrementCurrentRequests() => Interlocked.Increment(ref currentRequests);
        public int DecrementCurrentRequests() => Interlocked.Decrement(ref currentRequests);
        public long IncrementFinishedRequests() => Interlocked.Increment(ref finishedRequests);
        public long DecrementFinishedRequests() => Interlocked.Decrement(ref finishedRequests);
        public long UpdateActiveTimestamp() => Interlocked.Exchange(ref activeTimestamp, DateTimeOffset.UtcNow.ToUnixTimeSeconds() + Cons);

        public int GetCurrentLoadBalanceWeight() => currentLoadBalanceWeight;
        public int SetCurrentLoadBalanceWeight(int weight) => Interlocked.Exchange(ref currentLoadBalanceWeight, weight);
    }
}
