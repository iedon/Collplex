using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc; // remove this in asp.net core 3.0
using Microsoft.EntityFrameworkCore;
using Collplex.Core;
using Collplex.Models;
using Collplex.Models.Node;

namespace Collplex.Controllers
{
    public class DefaultController : AppBaseController
    {
        public DefaultController(MainContext context) : base (context) {}

        [Route("~/")]
        public ResponsePacket DefaultResponse()
            => PacketHandler.MakeResponse(ResponseCodeType.OK, new {
                    Name = Constants.AppName,
                    Version = Constants.AppVersion
               });
    }
}
