using FeatureFlagApi.Controllers.Features;
using FeatureFlagApi.Model;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private const bool theFeatureIsOff = false;
        private const bool theFeatureIsOn = true;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public RulesEngineService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
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
                var ruleToRun = InMemoryFeatureService._features.FirstOrDefault(o =>
                o.Name.Equals(requestedFeature, StringComparison.InvariantCultureIgnoreCase));
                if (ruleToRun == null)
                {
                    result.Features.Add(new Model.FeatureEvaluationResult
                    {
                        Name = requestedFeature,
                        IsOn = DEFAULT_FOR_ANY_FEATURE_THAT_DOES_NOT_EXIST
                    });
                    continue;
                }

                var rulesResultOfIsFeatureOn = RunAllRules(ruleToRun.Rules);
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
                        runningResult = HttpRequestHeaderExactMatchRule(rule.Meta);
                        break;
                    case ruleType.jwtParseMatchInList:
                        runningResult = JwtParseMatchInList(rule.Meta);
                        break;
                }
                if (runningResult == theFeatureIsOff)
                {
                    break;
                }
            }

            return runningResult;
        }

        public bool JwtParseMatchInList(string meta)
        {
            //var jwt = "(the JTW here)";
            //var handler = new JwtSecurityTokenHandler();
            //var token = handler.ReadJwtToken(jwt);

            return theFeatureIsOff;
        }

        public bool HttpRequestHeaderExactMatchRule(string meta)
        {
            var metaRuleObject = JsonConvert.DeserializeObject<MetaHttpRequestHeaderExactMatch>(meta);
            if (_httpContextAccessor.HttpContext.Request.Headers.TryGetValue(metaRuleObject.Header, out var outHeaderValue))
            {
                var headerValue = outHeaderValue.FirstOrDefault(o => o.Equals(metaRuleObject.Value, StringComparison.InvariantCultureIgnoreCase));
                if (!string.IsNullOrWhiteSpace(headerValue))
                {
                    return theFeatureIsOn;
                }
            }
            return theFeatureIsOff;
        }

        public bool BooleanRule(string meta)
        {
            if (!Boolean.Parse(meta))
            {
                return theFeatureIsOff;
            }
            else
            {
                return theFeatureIsOn;
            }
        }

    }

}
