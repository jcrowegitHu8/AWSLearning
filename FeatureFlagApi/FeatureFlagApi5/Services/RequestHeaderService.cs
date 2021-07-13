using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeatureFlagApi5.Services
{
    public class RequestHeaderService : IRequestHeaderService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;


        public RequestHeaderService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetTokenOnly()
        {
            if (!_httpContextAccessor.HttpContext.Request.Headers.TryGetValue("Authorization", out var outJWT))
            {
                return string.Empty;
            }
            var headerValue = outJWT.FirstOrDefault(o => o.StartsWith("Bearer", StringComparison.InvariantCultureIgnoreCase));
            if (string.IsNullOrWhiteSpace(headerValue))
            {
                return string.Empty;
            }
            return headerValue.Replace("Bearer ", string.Empty);

        }

        public string GetFirstNotNullOrWhitespaceValue(string key)
        {
            if (!_httpContextAccessor.HttpContext.Request.Headers.TryGetValue(key, out var outHeader))
            {
                return string.Empty;
            }
            var headerValue = outHeader.FirstOrDefault(o => !string.IsNullOrWhiteSpace(o));
            if (string.IsNullOrWhiteSpace(headerValue))
            {
                return string.Empty;
            }
            return headerValue;

        }

    }

    public interface IRequestHeaderService
    {
        string GetTokenOnly();
        string GetFirstNotNullOrWhitespaceValue(string key);
    }

}
