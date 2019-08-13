using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc; // remove when updating asp.net core 3.0
using Collplex.Core;
using Collplex.Models;

namespace Collplex.Controllers
{
    // 因为程序有自己的协议格式，因此忽视 HTTP 的响应格式，将所有错误的 HTTP 代码重写为 200-OK
    public class ErrorController : AppBaseController
    {
        public ErrorController(MainContext context) : base(context) {}

        // 服务端发生未知异常
        [Route("/error/exception")]
        public OkObjectResult FireServerException()
        {
            return Ok(PacketHandler.GenerateResponse(ResponseCodeType.SERVER_EXCEPTION));
        }

        // 其他的响应基本都是因为未注册的路由(404),403,502 等
        [Route("/error/{statusCode}")]
        public OkObjectResult FireStatus(int? statusCode)
        {
            switch (statusCode) {
                default: case 500: return Ok(PacketHandler.GenerateResponse(ResponseCodeType.SERVER_EXCEPTION));
                case 403: return Ok(PacketHandler.GenerateResponse(ResponseCodeType.FORBIDDEN));
                case 404: return Ok(PacketHandler.GenerateResponse(ResponseCodeType.NOT_FOUND));
                case 502: return Ok(PacketHandler.GenerateResponse(ResponseCodeType.BAD_GATEWAY));
                case 503: return Ok(PacketHandler.GenerateResponse(ResponseCodeType.SERVICE_UNAVAILABLE));
            }
        }
    }
}
