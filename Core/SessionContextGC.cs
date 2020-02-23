using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Collplex.Core.LoadBalancing;

namespace Collplex.Core
{
    public class SessionContextGC : IHostedService, IDisposable
    {
        private Timer timer;
        private readonly ILogger<SessionContextGC> logger;

        public SessionContextGC(ILogger<SessionContextGC> logger)
        {
            this.logger = logger;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            timer = new Timer(async s => await DoSessionContextGCAsync(s), null, TimeSpan.Zero, TimeSpan.FromSeconds(Constants.SessionContextGCIntervalSeconds));
            return Task.CompletedTask;
        }

        // 回收无用的会话上下文
        // 上下文结构 <clientId, <key, <hash, 会话信息> > >
        //            ^ 客户ID,  ^业务键  ^候选服务的hash
        // 一个 Key 下可能有多个 nodeUrl 可供使用。使用负载均衡器找到此时应该使用的 nodeUrl。
        private async Task DoSessionContextGCAsync(object state)
        {
            try
            {
                var allClients = await NodeHelper.GetAllClients();

                // 第一步
                // 从会话上下文中找到并删除当前已经不再可用的客户(比如客户被管理端删除了)
                var activeClients = new List<string>();
                var clientsToRemove = LoadBalancer.SessionContexts.Where(c =>
                {
                    bool found = false;
                    foreach (var registeredClient in allClients)
                    {
                        if (c.Key == registeredClient.Key)
                        {
                            found = true;
                            activeClients.Add(registeredClient.Key);
                        }
                    }
                    return !found;
                }).Select(c => c.Key);
                foreach (var clientToRemove in clientsToRemove) LoadBalancer.SessionContexts.TryRemove(clientToRemove, out _);

                // 从会话上下文中找到并删除在可用客户中不再存在的服务
                foreach (var activeClient in activeClients)
                {
                    var registeredServices = LoadBalancer.SessionContexts.Where(l => l.Key == activeClient).FirstOrDefault().Value.Select(k => k.Key);
                    var activeServices = new List<string>();
                    foreach (var registeredService in registeredServices)
                    {
                        bool found = false;
                        var nodeData = await NodeHelper.GetNodeData(activeClient);
                        if (nodeData != null)
                        {
                            foreach (var service in nodeData.Services)
                            {
                                if (service.Key == registeredService)
                                {
                                    found = true;
                                    activeServices.Add(registeredService);
                                }
                            }
                        }
                        if (!found)
                        {
                            LoadBalancer.SessionContexts.Where(l => l.Key == activeClient).FirstOrDefault().Value.TryRemove(registeredService, out _);
                        }

                    }

                    // 从会话上下文中找到并删除在可用业务中不再存在的负载均衡节点(即同业务键名不同业务URL)
                    foreach (var activeService in activeServices)
                    {
                        var registeredHashes = LoadBalancer.SessionContexts.Where(l => l.Key == activeClient).FirstOrDefault().Value.Where(s => s.Key == activeService).FirstOrDefault().Value.Select(h => h.Key);
                        foreach (var registeredHash in registeredHashes)
                        {
                            bool found = false;
                            var nodeData = await NodeHelper.GetNodeData(activeClient);
                            if (nodeData != null)
                            {
                                foreach (var service in nodeData.Services)
                                {
                                    if (service.Hash == registeredHash) found = true;
                                }
                            }
                            if (!found)
                            {
                                LoadBalancer.SessionContexts.Where(l => l.Key == activeClient).FirstOrDefault().Value.Where(s => s.Key == activeService).FirstOrDefault().Value.TryRemove(registeredHash, out _);
                            }
                        }
                    }
                }

                // 第二步
                // 删除所有客户中的所有业务中的已过期的会话
                var nullClientIds = allClients.Where(c => c.Value == null).Select(c => c.Key);
                foreach (var nullClientId in nullClientIds) LoadBalancer.SessionContexts.TryRemove(nullClientId, out _);

                var validClientIds = allClients.Where(c => c.Value != null).Select(c => c.Key);
                foreach (var validClientId in validClientIds)
                {
                    var nodeData = await NodeHelper.GetNodeData(validClientId);
                    if (nodeData != null)
                    {
                        var expiredHashes = nodeData.Services.Where(s => s.ExpireTimestamp < DateTimeOffset.UtcNow.ToUnixTimeSeconds()).Select(s => s.Hash);
                        foreach (var expiredHash in expiredHashes)
                        {
                            var client = LoadBalancer.SessionContexts.Where(c => c.Key == validClientId).FirstOrDefault();
                            if (client.Key == null || client.Value == null) return;

                            string keyToGC = null;
                            string hashToGC = null;
                            foreach (var key in client.Value)
                            {
                                foreach (var hash in key.Value)
                                {
                                    if (hash.Key == expiredHash)
                                    {
                                        keyToGC = key.Key;
                                        hashToGC = expiredHash;
                                    }
                                }
                            }
                            if (keyToGC != null && hashToGC != null)
                            {
                                var key = client.Value.Where(k => k.Key == keyToGC).FirstOrDefault();
                                key.Value.TryRemove(hashToGC, out _);
                                if (key.Value.Count == 0)
                                {
                                    client.Value.TryRemove(keyToGC, out _);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                logger.LogError("Error recycling SessionContext: " + exception.Message);
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
