using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Collplex.Controllers
{
    /* 基控制器 */
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class AppBaseController : ControllerBase
    {
        public AppBaseController() {}
    }
}
