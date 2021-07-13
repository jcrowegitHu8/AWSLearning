using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FeatureFlag.Shared.Models;
using FeatureFlagApi5.Model;
using FeatureFlagApi5.Services;

namespace FeatureFlagApi5.Controllers
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

        /// <summary>
        /// Get All features currently in the FeatureStore
        /// </summary>
        /// <returns>All FeatureStore information</returns>
        [HttpGet]
        public FeatureStoreModel Get()
        {
            return featureRepository.GetAll();
        }

        /// <summary>
        /// Determine if the list of requested features is ON/OFF
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Just the list of featues and their ON/OFF status</returns>
        [HttpPost]
        public EvaluationResponse Evaluate([FromBody] EvaluationRequest request)
        {
            var result = rulesEngineService.Run(request);
            return result;
        }

    }

}
