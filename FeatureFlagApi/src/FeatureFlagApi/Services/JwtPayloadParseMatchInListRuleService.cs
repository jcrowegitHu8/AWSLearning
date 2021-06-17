using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FeatureFlagApi.Services
{
    public interface IJwtPayloadParseMatchInListRuleService
    {
        bool Run(string meta);
    }
    public class JwtPayloadParseMatchInListRuleService : IJwtPayloadParseMatchInListRuleService
    {
        private readonly IAuthHeaderService _authHeaderService;

        public JwtPayloadParseMatchInListRuleService(IAuthHeaderService httpContextAccessor)
        {
            _authHeaderService = httpContextAccessor;
        }

        public bool Run(string meta)
        {
            if(string.IsNullOrWhiteSpace(meta))
            {
                return Constants.Common.THIS_FEATURE_IS_OFF;
            }
        
            var metaRuleObject = JsonConvert.DeserializeObject<MetaJwtParseMatchInList>(meta);
            var headerValue = _authHeaderService.GetTokenOnly();
            if (string.IsNullOrWhiteSpace(headerValue))
            {
                return Constants.Common.THIS_FEATURE_IS_OFF;
            }

            var handler = new JwtSecurityTokenHandler();
            if (!TryGetJWTPayloadAsString(headerValue, out var jsonString))
            {
                return Constants.Common.THIS_FEATURE_IS_OFF;
            }
            var jsonObject = JToken.Parse(jsonString);

            var jsonToken = jsonObject.SelectToken(metaRuleObject.Path);
            if (jsonToken != null)
            {
                if (metaRuleObject.List.Split(',').Contains(jsonToken.ToString()))
                {
                    return Constants.Common.THIS_FEATURE_IS_ON;
                }
            }

            return Constants.Common.THIS_FEATURE_IS_OFF;
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


    }

    public class MetaJwtParseMatchInList
    {
        public string Path { get; set; }
        public string List { get; set; }
    }
}
