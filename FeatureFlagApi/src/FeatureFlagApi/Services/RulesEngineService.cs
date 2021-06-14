using FeatureFlagApi.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FeatureFlagApi.Services
{
    public interface IRulesEngineService
    {
        EvaluationResponse Run(EvaluationRequest input);
    }

    public class RulesEngineService : IRulesEngineService
    {
        private const bool DEFAULT_FOR_ANY_FEATURE_THAT_DOES_NOT_EXIST = false;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IFeatureRepository _featureRepository;
        private readonly IHttpRequestHeaderExactMatchRuleService _httpRequestHeaderExactMatchRuleService;
        private readonly IJwtPayloadParseMatchInListRuleService _jwtPayloadParseMatchInListRuleService;


        public RulesEngineService(IHttpContextAccessor httpContextAccessor,
            IFeatureRepository featureRepository,
            IHttpRequestHeaderExactMatchRuleService httpRequestHeaderExactMatchRuleService, 
            IJwtPayloadParseMatchInListRuleService jwtPayloadParseMatchInListRuleService)
        {
            _httpContextAccessor = httpContextAccessor;
            _featureRepository = featureRepository;
            _httpRequestHeaderExactMatchRuleService = httpRequestHeaderExactMatchRuleService;
            _jwtPayloadParseMatchInListRuleService = jwtPayloadParseMatchInListRuleService;
        }
        public EvaluationResponse Run(EvaluationRequest input)
        {
            var result = new EvaluationResponse();
            if (input == null || input.Features == null || !input.Features.Any())
            {
                return result;
            }


            foreach (var requestedFeature in input.Features)
            {
                var featureToEvaluate = _featureRepository.GetAll().FirstOrDefault(o =>
                o.Name.Equals(requestedFeature, StringComparison.InvariantCultureIgnoreCase));
                if (featureToEvaluate == null)
                {
                    /* Even if the feature has not been defined
                     in the backend.  If the request asked for it
                    we tell them it's 'theFeatureIsOff'
                    */
                    result.Features.Add(new Model.FeatureEvaluationResult
                    {
                        Name = requestedFeature,
                        IsOn = DEFAULT_FOR_ANY_FEATURE_THAT_DOES_NOT_EXIST
                    });
                    continue;
                }

                var rulesResultOfIsFeatureOn = RunAllRules(featureToEvaluate.Rules);
                result.Features.Add(new Model.FeatureEvaluationResult
                {
                    Name = requestedFeature,
                    IsOn = rulesResultOfIsFeatureOn
                });
            }

            return result;
        }

        public bool RunAllRules(List<Model.Rule> rules)
        {

            var runningResult = true;
            foreach (var rule in rules)
            {
                switch (rule.Type)
                {
                    case Model.ruleType.boolean:
                        runningResult = BooleanRule(rule.Meta);
                        break;
                    case Model.ruleType.httpRequestHeaderExactMatch:
                        runningResult = _httpRequestHeaderExactMatchRuleService.Run(rule.Meta);
                        break;
                    case ruleType.jwtPayloadClaimMatchesValueInList:
                        runningResult = _jwtPayloadParseMatchInListRuleService.Run(rule.Meta);
                        break;
                }
                if (runningResult == Constants.Common.THIS_FEATURE_IS_OFF)
                {
                    /*We only support inclusive AND for the running of multiple rules.
                     * The moment a rule evaluates to 'theFeatureIsOff'
                     * we can short-circut the foreach
                     */
                    break;
                }
            }

            return runningResult;
        }
        

        public bool BooleanRule(string meta)
        {
            if (!Boolean.Parse(meta))
            {
                return Constants.Common.THIS_FEATURE_IS_OFF;
            }
            else
            {
                return Constants.Common.THIS_FEATURE_IS_ON;
            }
        }

    }

}
