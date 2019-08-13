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
        public async Task<ResponsePacket> DefaultResponse()
        {
            var name = await MainDb.Settings.Where(setting => setting.Key == "NAME").Select(setting => setting.Value).FirstOrDefaultAsync();
            var version = await MainDb.Settings.Where(setting => setting.Key == "VERSION").Select(setting => setting.Value).FirstOrDefaultAsync();
            var copyright = await MainDb.Settings.Where(setting => setting.Key == "COPYRIGHT").Select(setting => setting.Value).FirstOrDefaultAsync();
            return PacketHandler.GenerateResponse(ResponseCodeType.OK, new {
                name,
                version,
                copyright,
            });
        }
    }
}
