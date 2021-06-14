using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FeatureFlagApi.Model;
using FeatureFlagApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FeatureFlagApi.Controllers
{
    [Route("api/[controller]")]
    public class FeaturesController : ControllerBase
    {
        private readonly IRulesEngineService rulesEngineService;
        private readonly IFeatureRepository featureRepository;

        public FeaturesController(IRulesEngineService rulesEngineService, 
            IFeatureRepository featureRepository)
        {
            this.rulesEngineService = rulesEngineService;
            this.featureRepository = featureRepository;
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<Feature> Get()
        {
            return featureRepository.GetAll();
        }

        //[Authorize]
        [HttpPost]
        public EvaluationResponse Evaluate(EvaluationRequest request)
        {
            var result = rulesEngineService.Run(request);
            return result;
        }

    }

}
