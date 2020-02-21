using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Collplex.Core;
using Collplex.Models;
using static Collplex.Models.ResponsePacket.Types;

namespace Collplex.Core
{
    // 因为程序有自己的协议格式，因此忽视 HTTP 的响应格式，将所有错误的 HTTP 代码重写为 200-OK，并发送自己格式的错误数据包
    public static class ErrorHandler
    {
        public static Action<IApplicationBuilder> commonErrorHandler = new Action<IApplicationBuilder>(handler => {
            handler.Run(async context =>
            {
                var originalStatusCode = context.Response.StatusCode;
                context.Response.StatusCode = StatusCodes.Status200OK;
                context.Response.ContentType = Constants.JsonContentType + "; charset=utf-8";
                await context.Response.WriteAsync(Utils.JsonSerialize(MakeCommonErrorResponse(originalStatusCode)));
            });
        });

        // 将 404, 403, 500, 502 等框架抛出的标准 HTTP 错误统一转化为程序自己的格式
        private static object MakeCommonErrorResponse(int? statusCode)
        {
            if (statusCode == null) statusCode = 500;
            switch (statusCode) {
                default: case 500: return PacketHandler.MakeResponse(ResponseCodeType.ServerException);
                case 415: case 400: return PacketHandler.MakeResponse(ResponseCodeType.BadRequest);
                case 403: return PacketHandler.MakeResponse(ResponseCodeType.Forbidden);
                case 404: return PacketHandler.MakeResponse(ResponseCodeType.NotFound);
                case 502: return PacketHandler.MakeResponse(ResponseCodeType.BadGateway);
                case 503: return PacketHandler.MakeResponse(ResponseCodeType.ServiceUnavailable);
            }
        }
    }
}
