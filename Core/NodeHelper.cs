using System;
using System.Threading.Tasks;
using Google.Protobuf;
using Collplex.Models;
using Collplex.Models.Node;

namespace Collplex.Core
{
    public static class NodeHelper
    {
        // 操作子节点时所使用的锁的键前缀，内部固定值。
        private static readonly string LockPrefix = "lock_";

        // 客户元数据的键前缀，内部固定值。
        private static readonly string MetaDataPrefix = "meta_";

        // 子节点业务数据的键前缀，内部固定值。
        private static readonly string NodePrefix = "node_";

        /* 获得客户 */
        public static async Task<Client> GetClient(string clientId)
        {
            var db = Constants.Redis.GetDatabase();
            byte[] rawBytes = await db.StringGetAsync(Constants.KeyPrefix + MetaDataPrefix + clientId);
            if (rawBytes != null) return Client.Parser.ParseFrom(rawBytes);
            return null;
        }

        /* 获取子节点数据 */
        public static async Task<NodeData> GetNodeData(string clientId)
        {
            var db = Constants.Redis.GetDatabase();
            byte[] rawBytes = await db.StringGetAsync(Constants.KeyPrefix + NodePrefix + clientId);
            NodeData nodeData = null;
            if (rawBytes != null) nodeData = NodeData.Parser.ParseFrom(rawBytes);
            return nodeData;
        }

        /* 设置子节点数据 */
        public static async Task<bool> SetNodeData(string clientId, NodeData nodeData)
        {
            var db = Constants.Redis.GetDatabase();
            return await db.StringSetAsync(Constants.KeyPrefix + NodePrefix + clientId, nodeData.ToByteArray());
        }

        /* 锁定子节点的操作权限，即使未注册也能(必须)锁定(防止并发注册造成数据混乱) */
        public static async Task<string> LockNode(string clientId)
        {
            var db = Constants.Redis.GetDatabase();
            string lockKeyName = Constants.KeyPrefix + LockPrefix + NodePrefix + clientId;
            string lockToken = Guid.NewGuid().ToString();
            uint lockCounter = 0;
            uint maxCounters = Constants.AcquireLockTimeoutSeconds * 1000 / 50;
            while (lockCounter < maxCounters)
            {
                if (!await db.LockTakeAsync(lockKeyName, lockToken, TimeSpan.FromSeconds(Constants.LockTimeoutSeconds)))
                {
                    lockCounter++;
                    await Task.Delay(50); // 让当前任务休眠，转移控制权处理更多任务
                    continue;
                }
                return lockToken; // 锁定成功，返回锁标识
            }
            return null; // 获取锁超时，锁定失败
        }

        /* 释放子节点锁 */
        public static async Task<bool> ReleaseNode(string clientId, string lockToken)
        {
            var db = Constants.Redis.GetDatabase();
            string lockKeyName = Constants.KeyPrefix + LockPrefix + NodePrefix + clientId;
            return await db.LockReleaseAsync(lockKeyName, lockToken);
        }

    }
}
