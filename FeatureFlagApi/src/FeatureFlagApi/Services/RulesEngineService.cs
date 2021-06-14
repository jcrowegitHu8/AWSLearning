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
        private const bool theFeatureIsOff = false;
        private const bool theFeatureIsOn = true;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IFeatureRepository _featureRepository;


        public RulesEngineService(IHttpContextAccessor httpContextAccessor, IFeatureRepository featureRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _featureRepository = featureRepository;
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
                var ruleToRun = _featureRepository.GetAll().FirstOrDefault(o =>
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
                    case ruleType.jwtPayloadClaimMatchesValueInList:
                        runningResult = JwtPayloadParseMatchInListRule(rule.Meta);
                        break;
                }
                if (runningResult == theFeatureIsOff)
                {
                    break;
                }
            }

            return runningResult;
        }

        private bool TryGetJWTPayloadAsString(string jwtInput, out string result)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            result = string.Empty;
            //Check if readable token (string is in a JWT format)
            var readableToken = jwtHandler.CanReadToken(jwtInput);

            if (readableToken != true)
            {
                // "The token doesn't seem to be in a proper JWT format.";
                return false;
            }
            if (readableToken == true)
            {
                var token = jwtHandler.ReadJwtToken(jwtInput);

                //Extract the headers of the JWT
                //var headers = token.Header;
                //var jwtHeader = "{";
                //foreach (var h in headers)
                //{
                //    jwtHeader += '"' + h.Key + "\":\"" + h.Value + "\",";
                //}
                //jwtHeader += "}";
                //txtJwtOut.Text = "Header:\r\n" + JToken.Parse(jwtHeader).ToString(Formatting.Indented);

                //Extract the payload of the JWT
                var claims = token.Claims;
                var jwtPayload = "{";
                foreach (Claim c in claims)
                {
                    jwtPayload += '"' + c.Type + "\":\"" + c.Value + "\",";
                }
                jwtPayload += "}";
                result += JToken.Parse(jwtPayload).ToString(Formatting.Indented);
                return true;
            }
            return false;
        }

        public bool JwtPayloadParseMatchInListRule(string meta)
        {
            var metaRuleObject = JsonConvert.DeserializeObject<MetaJwtParseMatchInList>(meta);
            if (!_httpContextAccessor.HttpContext.Request.Headers.TryGetValue("Authorization", out var outJWT))
            {
                return theFeatureIsOff;
            }
            var headerValue = outJWT.FirstOrDefault(o => o.StartsWith("Bearer", StringComparison.InvariantCultureIgnoreCase));
            if (string.IsNullOrWhiteSpace(headerValue))
            {
                return theFeatureIsOff;
            }

            var handler = new JwtSecurityTokenHandler();
            headerValue = headerValue.Replace("Bearer ", string.Empty);
            if(!TryGetJWTPayloadAsString(headerValue,  out var jsonString))
            {
                return theFeatureIsOff;
            }
            var jsonObject = JToken.Parse(jsonString);

            var jsonToken = jsonObject.SelectToken(metaRuleObject.Path);
            if (jsonToken != null)
            {
                if (metaRuleObject.List.Split(',').Contains(jsonToken.ToString()))
                {
                    return theFeatureIsOn;
                }
            }

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
