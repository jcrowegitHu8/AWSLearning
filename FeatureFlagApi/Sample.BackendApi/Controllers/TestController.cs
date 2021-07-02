using FeatureFlagApi.SDK;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sample.BackendApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;
        private readonly IFeatureFlagService _featureFlagService;

        public TestController(ILogger<TestController> logger, IFeatureFlagService featureFlagService)
        {
            _logger = logger;
            _featureFlagService = featureFlagService;
        }

        /// <summary>
        /// Get Sync Example
        /// </summary>
        /// <returns></returns>
        [Route("Sync")]
        [HttpGet]
        public String Get()
        {
            var demo = "Sample_BackendApi_Feature";
            var result = _featureFlagService.FeatureIsOn(demo);
            return $"Sync - {demo} is {result}";

        }

        /// <summary>
        /// Get Async Example with CancellationToken
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Route("Async")]
        [HttpGet]
        public async Task<String> Get(CancellationToken cancellationToken)
        {
            var demo = "Sample_BackendApi_Feature";
            var result =await  _featureFlagService.FeatureIsOnAsync(demo, cancellationToken);
            return $"Async - {demo} is {result}";
        }
    }
}
