using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeatureFlagApi.Services
{
    public interface IHttpRequestHeaderExactMatchRuleService
    {
        bool Run(string meta);
    }

    public class HttpRequestHeaderExactMatchRuleService : IHttpRequestHeaderExactMatchRuleService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpRequestHeaderExactMatchRuleService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public bool Run(string meta)
        {

            var metaRuleObject = JsonConvert.DeserializeObject<MetaHttpRequestHeaderExactMatch>(meta);
            if (_httpContextAccessor.HttpContext.Request.Headers.TryGetValue(metaRuleObject.Header, out var outHeaderValue))
            {
                var headerValue = outHeaderValue.FirstOrDefault(o => o.Equals(metaRuleObject.Value, StringComparison.InvariantCultureIgnoreCase));
                if (!string.IsNullOrWhiteSpace(headerValue))
                {
                    return Constants.Common.THIS_FEATURE_IS_ON;
                }
            }
            return Constants.Common.THIS_FEATURE_IS_OFF;
        }
    }

    public class MetaHttpRequestHeaderExactMatch
    {
        public string Header { get; set; }
        public string Value { get; set; }
    }
}
