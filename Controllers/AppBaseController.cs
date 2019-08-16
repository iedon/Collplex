using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Collplex.Controllers
{
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/json")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class AppBaseController : ControllerBase
    {
        public AppBaseController() {}
    }
}
