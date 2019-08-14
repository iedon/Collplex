using System;
using System.Net;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Collplex.Core;
using Collplex.Models;

/* Easy University Service Integration */
/*      Copyright (C) 2019 iEdon.      */

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
                string AllowProxy = Configuration.GetValue<string>("AllowedProxyIP");
                if (AllowProxy != string.Empty)
                {
                    string[] IPSets = AllowProxy.Trim().Split(",");
                    foreach (string IP in IPSets)
                    {
                        options.KnownProxies.Add(IPAddress.Parse(IP.Trim()));
                    }
                }
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            // 注入数据库
            var DbSettings = Configuration.GetSection("DbSettings");

            // -- 注入帐号 MySQL 数据库
            var MySQLSettings = DbSettings.GetSection("MySQL");
            services.AddDbContextPool<MainContext>(options =>
            {
                options.UseMySql(MySQLSettings.GetValue<string>("ConnectionString"));
            }, MySQLSettings.GetValue<int>("PoolSize"));

            // -- 配置子节点业务注册中心 Redis 数据库(初始化全局静态 Redis 调制器对象)
            var RedisSettings = DbSettings.GetSection("Redis");
            Constants.Redis = ConnectionMultiplexer.Connect(RedisSettings.GetValue<string>("ConfigurationString"));
            Constants.KeyPrefix = RedisSettings.GetValue<string>("KeyPrefix");
            Constants.AcquireLockTimeoutSeconds = RedisSettings.GetValue<uint>("AcquireLockTimeoutSeconds");
            Constants.LockTimeoutSeconds = RedisSettings.GetValue<uint>("LockTimeoutSeconds");

            // 配置其他信息
            Constants.NodePacketInboundAntiReplaySeconds = Configuration.GetValue<uint>("NodePacketInboundAntiReplaySeconds");
            Constants.AppName = Assembly.GetEntryAssembly().GetName().Name;
            Constants.AppVersion = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            Constants.NodeHttpClientUserAgent = Constants.AppName + "/" + Constants.AppVersion;

            // 注入子节点访问客户端
            services.AddHttpClient<NodeHttpClient>();

            // 注入 MVC remove this for 3.0
            services.AddMvc()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.SuppressModelStateInvalidFilter = true;
                });
            //services.AddRouting();
            //services.AddControllers();
        }

        //                                             IHostEnvironment env
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILogger<Startup> logger)
        {
            app.UseForwardedHeaders();
            //app.UseRouting();
            
            // 设置全局异常捕获中间件
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status200OK;
                    context.Response.ContentType = Constants.JsonContentType;

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        logger.LogError("Global exception captured: " + contextFeature.Error.Message + Environment.NewLine + contextFeature.Error.StackTrace);
                        await context.Response.WriteAsync(Utils.JsonSerialize(PacketHandler.MakeResponse(ResponseCodeType.SERVER_EXCEPTION)));
                    }
                });
            });

            // 将其他错误页面的处理重定向到 /Error，以便响应按照统一格式
            app.UseStatusCodePagesWithReExecute("/error/{0}");

            /*app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Default}/{action=Index}/{id?}");
            });*/

            app.UseMvc(routes => // remove this for 3.0
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Default}/{action=Index}/{id?}");
            });
        }
    }
}
