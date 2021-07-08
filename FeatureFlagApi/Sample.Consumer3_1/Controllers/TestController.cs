using FeatureFlagApi.SDK;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sample.Consumer3_1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;
        private readonly IFeatureFlagService _featureFlagService;
        private readonly IConfiguration _configuration;

        public TestController(ILogger<TestController> logger,
            IFeatureFlagService featureFlagService,
            IConfiguration configuration)
        {
            _logger = logger;
            _featureFlagService = featureFlagService;
            _configuration = configuration;
        }

        /// <summary>
        /// Get Sync Example
        /// </summary>
        /// <returns></returns>
        [Route("Sync")]
        [HttpGet]
        public string Get()
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
        public async Task<string> Get(CancellationToken cancellationToken)
        {
            var demo = "Sample_BackendApi_Feature";
            var result = await _featureFlagService.FeatureIsOnAsync(demo, cancellationToken);
            return $"Async - {demo} is {result}";
        }

        [Route("Connectivity")]
        [HttpGet]
        public async Task<IActionResult> Connectivity(string route, CancellationToken cancellationToken)
        {
            var client = new HttpClient();
            var url = _configuration.GetValue<string>("FeatureFlagApiUrl");
            var fullUrl = url + route;

            client.BaseAddress = new Uri(url);
            try
            {
                var response = await client.GetAsync(route);
                if (response.IsSuccessStatusCode)
                {
                    return Ok($"The connection to '{fullUrl}' was successful.");
                }
                var errorStatus = $"{(int)response.StatusCode}:{response.StatusCode.ToString()} - {response.ReasonPhrase}" ;
                var errorPayload = await response.Content.ReadAsStringAsync();
                return StatusCode((int)HttpStatusCode.ServiceUnavailable, $"{errorStatus} | {fullUrl} |{errorPayload}");
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.ServiceUnavailable, $"{fullUrl} | {ex.Message}");
            }
        }


    }
}
