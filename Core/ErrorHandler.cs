using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Collplex.Core;
using Collplex.Models;
using Microsoft.AspNetCore.Builder;

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
                context.Response.ContentType = Constants.JsonContentType;
                await context.Response.WriteAsync(Utils.JsonSerialize(MakeCommonErrorResponse(originalStatusCode)));
            });
        });

        // 将 404, 403, 500, 502 等框架抛出的标准 HTTP 错误统一转化为程序自己的格式
        private static ResponsePacket MakeCommonErrorResponse(int? statusCode)
        {
            if (statusCode == null) statusCode = 500;
            switch (statusCode) {
                default: case 500: return PacketHandler.MakeResponse(ResponseCodeType.SERVER_EXCEPTION);
                case 415: case 400: return PacketHandler.MakeResponse(ResponseCodeType.BAD_REQUEST);
                case 403: return PacketHandler.MakeResponse(ResponseCodeType.FORBIDDEN);
                case 404: return PacketHandler.MakeResponse(ResponseCodeType.NOT_FOUND);
                case 502: return PacketHandler.MakeResponse(ResponseCodeType.BAD_GATEWAY);
                case 503: return PacketHandler.MakeResponse(ResponseCodeType.SERVICE_UNAVAILABLE);
            }
        }
    }
}
