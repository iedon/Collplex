using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Collplex.Core
{
    // 此静态类的作用是在内存中维护一份配置供程序的各个模块使用
    public static class Constants
    {
        public static string AssemblyName = Assembly.GetName().Name;

        /* ---------- 以下这些属性都在初始化的时候由配置文件读入 ---------- */

        // 在 Startup.cs 中初始化此 Redis 调制器全局静态对象
        public static ConnectionMultiplexer Redis { get; set; }
        // Redis 键名前缀
        public static string KeyPrefix { get; set; }
        // Redis 获锁超时时间
        public static uint AcquireLockTimeoutSeconds { get; set; }
        // Redis 超时时间(锁最大能够存在的时间)
        public static uint LockTimeoutSeconds { get; set; }
        // 反重放攻击用，允许子节点请求的时间戳波动范围
        public static uint NodeRequestInboundAntiReplaySeconds { get; set; }

    }
}
