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
        /// <summary>
        /// Determine if the API is able to process requests 
        /// </summary>
        /// <returns></returns>
        [Route("ping")]
        [HttpGet]
        public string Ping()
        {
            return "Pong";
        }

        /// <summary>
        /// Determines if the API can successfully connect to dependents systems (i.e Database)
        /// </summary>
        /// <returns>Info of success or failure or a correlationId to go search your logs for the details of the failure </returns>
        [Route("connectivity")]
        [HttpGet]
        public string Connectivity()
        {
            return "Success";
        }
    }
}
