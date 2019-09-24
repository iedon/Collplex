using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Collplex.Core;
using Collplex.Models;

namespace Collplex.Controllers
{
    public class DefaultController : AppBaseController
    {
        public DefaultController() {}

        [Route("~/")]
        public static ResponsePacket DefaultResponse()
            => PacketHandler.MakeResponse(ResponseCodeType.OK, new {
                    Name = Constants.AppName,
                    Version = Constants.AppVersion
               });
    }
}
