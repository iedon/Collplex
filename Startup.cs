using System;
using System.Net;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using MongoDB.Driver;
using Collplex.Core;
using Collplex.Models;


/*  iEdon Collplex Tiny MicroService SuperNode  */
/*           Copyright (C) 2019 iEdon.          */


namespace Collplex
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // 读取并注入允许的反向代理主机配置
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                var allowedProxyIPs = Configuration.GetSection("AllowedProxyIPs").AsEnumerable().Where(ip => ip.Value != null).Select(ip => ip);
                foreach (var ip in allowedProxyIPs)
                {
                    options.KnownProxies.Add(IPAddress.Parse(ip.Value.Trim()));
                }
                options.ForwardLimit = Configuration.GetValue<int>("XForwardedForLimit", 1);
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            // -- 配置子节点业务注册中心 Redis 数据库(初始化全局静态 Redis 调制器对象)
            var dbSettings = Configuration.GetSection("DbSettings");
            var redisSettings = dbSettings.GetSection("Redis");
            Constants.Redis = ConnectionMultiplexer.Connect(redisSettings.GetValue<string>("ConfigurationString"));
            Constants.KeyPrefix = redisSettings.GetValue<string>("KeyPrefix");
            Constants.AcquireLockTimeoutSeconds = redisSettings.GetValue<uint>("AcquireLockTimeoutSeconds", 5);
            Constants.LockTimeoutSeconds = redisSettings.GetValue<uint>("LockTimeoutSeconds", 10);

            // -- 配置 MongoDB 业务日志数据库
            Constants.MongoDB = new MongoClient(dbSettings.GetSection("MongoDB").GetValue<string>("ConfigurationString"));

            // 配置其他信息
            Constants.NodeDefaultWeight = Configuration.GetValue<uint>("NodeDefaultWeight", 1);
            Constants.SessionContextGCIntervalSeconds = Configuration.GetValue<uint>("SessionContextGCIntervalSeconds", 3600);
            Constants.WriteSessionContextToRedisIntervalSeconds = Configuration.GetValue<uint>("WriteSessionContextToRedisIntervalSeconds", 0);
            Constants.NodePacketInboundAntiReplaySeconds = Configuration.GetValue<uint>("NodePacketInboundAntiReplaySeconds", 600);
            Constants.AppName = Assembly.GetEntryAssembly().GetName().Name;
            Constants.AppVersion = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            Constants.NodeHttpClientUserAgent = Constants.AppName + "/" + Constants.AppVersion;
            Constants.LogUserRequest = Configuration.GetValue<bool>("LogUserRequest", false);
            Constants.LogUserPayload = Configuration.GetValue<bool>("LogUserPayload", false);
            Constants.NodeHttpClientLifeTimeSeconds = Configuration.GetValue<double>("NodeHttpClientLifeTimeSeconds", 300);

            // 注入子节点访问客户端
            services.AddHttpClient<NodeHttpClient>().SetHandlerLifetime(TimeSpan.FromSeconds(Constants.NodeHttpClientLifeTimeSeconds));

            // 注入 MongoDB 服务日志上下文
            services.AddSingleton<IMongoRepository<ServiceLog>, MongoRepository<ServiceLog>>();

            // 注入路由和控制器
            services.AddRouting();
            services.AddControllers()
            .ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            })
            .AddJsonOptions(options => {
                options.JsonSerializerOptions.WriteIndented = Constants.JsonSerializerOptionsGlobal.WriteIndented;
                options.JsonSerializerOptions.PropertyNamingPolicy = Constants.JsonSerializerOptionsGlobal.PropertyNamingPolicy;
                options.JsonSerializerOptions.DictionaryKeyPolicy = Constants.JsonSerializerOptionsGlobal.DictionaryKeyPolicy;
            });

            // 注入后台计划任务
            if (Constants.SessionContextGCIntervalSeconds != 0) services.AddHostedService<SessionContextGC>();
        }

        public static void Configure(IApplicationBuilder app)
        {
            app.UseForwardedHeaders();

            // 设置全局异常捕获与统一错误处理
            app.UseExceptionHandler(ErrorHandler.commonErrorHandler);
            app.UseStatusCodePages(ErrorHandler.commonErrorHandler);

            // 设置路由与 Endpoint
            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
