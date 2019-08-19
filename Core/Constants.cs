using System;
using System.Collections.Generic;
using System.Reflection;
using StackExchange.Redis;

namespace Collplex.Core
{
    // 此静态类的作用是在内存中维护一份配置供程序的各个模块使用
    public static class Constants
    {
        public static string JsonContentType = "application/json";

        /* ---------- 以下这些属性都在初始化的时候设置 ---------- */

        // 本程序集名称(项目名称, 不是全称)
        public static string AppName { get; set; }

        // 本程序集版本(项目版本, 不是文件版本)
        public static string AppVersion { get; set; }

        // 在 Startup.cs 中初始化此 Redis 调制器全局静态对象
        public static ConnectionMultiplexer Redis { get; set; }

        // Redis 键名前缀
        public static string KeyPrefix { get; set; }

        // Redis 获锁超时时间
        public static uint AcquireLockTimeoutSeconds { get; set; }

        // Redis 超时时间(锁最大能够存在的时间)
        public static uint LockTimeoutSeconds { get; set; }

        // 反重放攻击用，允许子节点请求的时间戳波动范围
        public static uint NodePacketInboundAntiReplaySeconds { get; set; }

        // 本中心节点请求子节点业务时所使用的 User-Agent
        public static string NodeHttpClientUserAgent { get; set; }

        // 用作请求日志服务的 MongoDB 数据库连接字符串
        public static string MongoDBConnectionString { get; set; }

    }
}
