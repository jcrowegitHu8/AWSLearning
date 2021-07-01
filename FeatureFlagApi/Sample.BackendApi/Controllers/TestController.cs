using FeatureFlagApi.SDK;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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

        [HttpGet]
        public String Get()
        {
            var demo = "Sample_BackendApi_Feature";
            if (_featureFlagService.FeatureIsOn(demo))
                return $"{demo} is ON";

            return $"{demo} is OFF";
        }
    }
}
