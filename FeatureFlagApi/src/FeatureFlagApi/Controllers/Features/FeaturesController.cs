using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FeatureFlagApi.Controllers.Features;
using FeatureFlagApi.Model;
using FeatureFlagApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FeatureFlagApi.Controllers
{
    [Route("api/[controller]")]
    public class FeaturesController : ControllerBase
    {
        private readonly IRulesEngineService rulesEngineService;

        public FeaturesController(IRulesEngineService rulesEngineService)
        {
            this.rulesEngineService = rulesEngineService;
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<Feature> Get()
        {
            return InMemoryFeatureService._features;
        }

        [HttpPost]
        public EvaluationResponse Evaluate(EvaluationRequest request)
        {
            var result = rulesEngineService.Run(request);
            return result;
        }

    }

}
