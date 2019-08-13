using Collplex.Models.Node;
using Google.Protobuf;
using System;
using System.Threading.Tasks;

namespace Collplex.Core
{
    public static class NodeHelper
    {
        // 操作子节点时所使用的锁的前缀，项目固定值。
        private static readonly string LockPrefix = "lock_";

        /* 获取子节点数据 */
        public static async Task<Node> GetNode(string clientId)
        {
            var db = Constants.Redis.GetDatabase();
            byte[] rawBytes = await db.StringGetAsync(Constants.KeyPrefix + clientId);
            Node node = null;
            if (rawBytes != null) node = Node.Parser.ParseFrom(rawBytes);
            return node;
        }

        /* 设置子节点数据 */
        public static async Task<bool> SetNode(string clientId, Node node)
        {
            var db = Constants.Redis.GetDatabase();
            return await db.StringSetAsync(Constants.KeyPrefix + clientId, node.ToByteArray());
        }

        /* 锁定子节点的操作权限，即使未注册也能(必须)锁定(防止并发注册造成数据混乱) */
        public static async Task<string> LockNode(string clientId)
        {
            var db = Constants.Redis.GetDatabase();
            string lockKeyName = Constants.KeyPrefix + LockPrefix + clientId;
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
            string lockKeyName = Constants.KeyPrefix + LockPrefix + clientId;
            return await db.LockReleaseAsync(lockKeyName, lockToken);
        }

    }
}
