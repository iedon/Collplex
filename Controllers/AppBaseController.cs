using System;
using System.Collections.Generic;
using Collplex.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Collplex.Controllers
{
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces(Constants.JsonContentType)]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class AppBaseController : ControllerBase
    {
        public AppBaseController() {}
    }
}
