using Collplex.Core.LoadBalancing;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Collplex.Core
{
    public class SessionContextGC : IHostedService, IDisposable
    {
        private Timer timer;
        public Task StartAsync(CancellationToken stoppingToken)
        {
            timer = new Timer(DoSessionContextGC, null, TimeSpan.Zero, TimeSpan.FromSeconds(Constants.SessionContextGCIntervalSeconds));
            return Task.CompletedTask;
        }

        private void DoSessionContextGC(object state)
        {
            foreach (var clientContext in LoadBalancer.SessionContexts)
            {
                foreach (var keyContext in clientContext.Value)
                {
                    foreach (var sessionContext in keyContext.Value.To)
                    {
                        if (sessionContext.Value.GetActiveTimestamp() < DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                        {
                            sessionContext.UpdateActiveTimestamp();
                        }
                    }
                }
            }
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose() => timer?.Dispose();
        
    }
}
