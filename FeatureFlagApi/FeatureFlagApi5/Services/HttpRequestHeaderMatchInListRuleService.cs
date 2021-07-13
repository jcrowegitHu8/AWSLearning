using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cts = FeatureFlag.Shared.Constants;

namespace FeatureFlagApi5.Services
{
    public class httpRequestHeaderMatchInListRuleService
    {

    }

    public interface IHttpRequestHeaderMatchInListRuleService
    {
        bool Run(string meta);
    }

    public class HttpRequestHeaderMatchInListRuleService : IHttpRequestHeaderMatchInListRuleService
    {
        private readonly IRequestHeaderService _httpContextAccessor;

        public HttpRequestHeaderMatchInListRuleService(IRequestHeaderService httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public bool Run(string meta)
        {
            if (meta == null)
            {
                return cts.Common.THIS_FEATURE_IS_OFF;
            }
            var metaRuleObject = JsonConvert.DeserializeObject<MetaHttpRequestHeaderMatchInList>(meta);
            var headerValue = _httpContextAccessor.GetFirstNotNullOrWhitespaceValue(metaRuleObject.Header);
            if (!string.IsNullOrWhiteSpace(headerValue))
            {
                var compareList = metaRuleObject.List.ToUpper().Split(metaRuleObject.Delimiter);
                if (compareList.Contains(headerValue.ToUpper()))
                {
                    return cts.Common.THIS_FEATURE_IS_ON;
                }
            }
            return cts.Common.THIS_FEATURE_IS_OFF;
        }
    }

    public class MetaHttpRequestHeaderMatchInList
    {
        public MetaHttpRequestHeaderMatchInList()
        {
            Delimiter = cts.Common.DEFAULT_DELIMITER;
            List = string.Empty;
        }
        public string Header { get; set; }
        public string List { get; set; }

        public string Delimiter { get; set; }
    }
}
