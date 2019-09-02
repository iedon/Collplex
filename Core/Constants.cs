using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using MongoDB.Driver;
using StackExchange.Redis;

namespace Collplex.Core
{
    // 此静态类的作用是在内存中维护一份配置供程序的各个模块使用
    public static class Constants
    {
        public static string JsonContentType = "application/json";
        public static JsonSerializerOptions JsonSerializerOptionsGlobal = new JsonSerializerOptions()
        {
            // 不美化输出(即采用压缩输出而不格式化)
            WriteIndented = false,
            // 采用驼峰命名法命名输出变量
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
        };

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

        // 本中心节点请求子节点业务时所使用的 HttpClient 的 User-Agent
        public static string NodeHttpClientUserAgent { get; set; }

        // 在 Startup.cs 中初始化此 MongoDB 对象
        public static MongoClient MongoDB { get; set; }

        // 是否记录子节点请求
        public static bool LogUserRequest { get; set; }

        // LogUserRequest 为 false 时，此项无效。是否记录子节点请求过来的数据体(Body)，如果为是，则用户访问日志中将记录用户的请求数据体
        public static bool LogUserPayload { get; set; }

        // 请求子节点业务时所使用的 HttpClient 的生命周期，单位秒
        public static double NodeHttpClientLifeTimeSeconds { get; set; }
    }
}
