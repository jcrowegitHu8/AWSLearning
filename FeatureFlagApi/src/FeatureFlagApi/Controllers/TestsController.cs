using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace FeatureFlagApi.Controllers
{
    [Route("api/[controller]")]
    public class TestsController : ControllerBase
    {
        // GET api/values
        [Route("ping")]
        [HttpGet]
        public string Ping()
        {
            return "Pong";
        }
    }
}
