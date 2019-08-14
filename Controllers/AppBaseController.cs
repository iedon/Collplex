using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Collplex.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc; // remove when updating asp.net core 3.0

namespace Collplex.Controllers
{
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/json")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class AppBaseController : ControllerBase
    {
        protected readonly MainContext MainDb;
        public AppBaseController(MainContext context)
        {
            MainDb = context;
        }
    }
}
