using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FeatureFlagApi.Logging;
using Microsoft.AspNetCore.Mvc;

namespace FeatureFlagApi.Controllers
{
    [Route("api/[controller]")]
    public class TestsController : ControllerBase
    {
        private readonly IAWSStructuredLogger _logger;

        public TestsController(IAWSStructuredLogger logger)
        {
            _logger = logger;
        }

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

        [Route("structuredlogging")]
        [HttpGet]
        public string StructuredLogging()
        {
            _logger.LogInfo("Info before any enrichment");
            _logger.EnrichWithCorrelationId(Guid.NewGuid());
            _logger.LogError("Error Level Logging after enriched with correlationid.");
            return "Success";
        }


    }
}
