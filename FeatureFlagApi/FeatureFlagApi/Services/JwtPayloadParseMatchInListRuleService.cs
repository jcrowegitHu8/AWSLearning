using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using cts = FeatureFlag.Shared.Constants;

namespace FeatureFlagApi.Services
{
    public interface IJwtPayloadParseMatchInListRuleService
    {
        bool Run(string meta);
    }
    public class JwtPayloadParseMatchInListRuleService : IJwtPayloadParseMatchInListRuleService
    {
        private readonly IRequestHeaderService _authHeaderService;

        public JwtPayloadParseMatchInListRuleService(IRequestHeaderService httpContextAccessor)
        {
            _authHeaderService = httpContextAccessor;
        }

        public bool Run(string meta)
        {
            if (string.IsNullOrWhiteSpace(meta))
            {
                return cts.Common.THIS_FEATURE_IS_OFF;
            }

            var metaRuleObject = JsonConvert.DeserializeObject<MetaJwtParseMatchInList>(meta);
            var headerValue = _authHeaderService.GetTokenOnly();
            if (string.IsNullOrWhiteSpace(headerValue))
            {
                return cts.Common.THIS_FEATURE_IS_OFF;
            }

            var handler = new JwtSecurityTokenHandler();
            if (!TryGetJWTPayloadAsString(headerValue, out var jsonString))
            {
                return cts.Common.THIS_FEATURE_IS_OFF;
            }
            var jsonObject = JToken.Parse(jsonString);

            var jsonToken = jsonObject.SelectToken(metaRuleObject.Path);
            if (jsonToken != null)
            {
                if (metaRuleObject.List != null
                    && metaRuleObject.List.ToUpper().Split(metaRuleObject.Delimiter)
                    .Contains(jsonToken.ToString().ToUpper()))
                {
                    return cts.Common.THIS_FEATURE_IS_ON;
                }
            }

            return cts.Common.THIS_FEATURE_IS_OFF;
        }

        private bool TryGetJWTPayloadAsString(string jwtInput, out string result)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            result = string.Empty;
            //Check if readable token (string is in a JWT format)
            var readableToken = jwtHandler.CanReadToken(jwtInput);

            if (readableToken == true)
            {
                try
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
                catch (ArgumentException)
                {
                    //Some kind of JWT Garbage token
                    return false;
                }
            }
            return false;
        }


    }

    public class MetaJwtParseMatchInList
    {
        public MetaJwtParseMatchInList()
        {
            Delimiter = cts.Common.DEFAULT_DELIMITER;
        }
        public string Path { get; set; }
        public string List { get; set; }
        public string Delimiter { get; set; }
    }
}
