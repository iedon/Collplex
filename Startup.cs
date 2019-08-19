﻿using System;
using System.Net;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using Newtonsoft.Json.Serialization;
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
                string[] allowProxyIPs = Configuration.GetValue<string[]>("AllowedProxyIPs");
                if (allowProxyIPs != null && allowProxyIPs.Length != 0)
                {
                    foreach (string ip in allowProxyIPs)
                    {
                        options.KnownProxies.Add(IPAddress.Parse(ip.Trim()));
                    }
                }
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            // -- 配置子节点业务注册中心 Redis 数据库(初始化全局静态 Redis 调制器对象)
            var dbSettings = Configuration.GetSection("DbSettings");
            var redisSettings = dbSettings.GetSection("Redis");
            Constants.Redis = ConnectionMultiplexer.Connect(redisSettings.GetValue<string>("ConfigurationString"));
            Constants.KeyPrefix = redisSettings.GetValue<string>("KeyPrefix");
            Constants.AcquireLockTimeoutSeconds = redisSettings.GetValue<uint>("AcquireLockTimeoutSeconds");
            Constants.LockTimeoutSeconds = redisSettings.GetValue<uint>("LockTimeoutSeconds");
            Constants.MongoDBConnectionString = dbSettings.GetSection("MongoDB").GetValue<string>("ConnectionString");

            // 配置其他信息
            Constants.NodePacketInboundAntiReplaySeconds = Configuration.GetValue<uint>("NodePacketInboundAntiReplaySeconds");
            Constants.AppName = Assembly.GetEntryAssembly().GetName().Name;
            Constants.AppVersion = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            Constants.NodeHttpClientUserAgent = Constants.AppName + "/" + Constants.AppVersion;

            // 注入子节点访问客户端
            services.AddHttpClient<NodeHttpClient>();

            // 注入 MongoDB 服务日志上下文
            services.AddSingleton<IMongoRepository<ServiceLog>, MongoRepository<ServiceLog>>();

            // 注入路由和控制器
            services.AddRouting();
            services.AddControllers().ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            })/*.AddJsonOptions(options => {    目前用不了 System.Text.Json
                // 不美化输出
                options.JsonSerializerOptions.WriteIndented = false;
                // 采用驼峰命名法命名输出变量
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            })*/
            .AddNewtonsoftJson(options =>
           options.SerializerSettings.ContractResolver =
              new CamelCasePropertyNamesContractResolver());
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseForwardedHeaders();

            // 设置路由与 Endpoint
            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());

            // 设置全局异常捕获与统一错误处理
            app.UseExceptionHandler(ErrorHandler.commonErrorHandler);
            app.UseStatusCodePages(ErrorHandler.commonErrorHandler);
        }
    }
}
