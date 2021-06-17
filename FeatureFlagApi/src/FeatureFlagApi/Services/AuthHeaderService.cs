using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeatureFlagApi.Services
{
    public class AuthHeaderService : IAuthHeaderService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;


        public AuthHeaderService(IHttpContextAccessor httpContextAccessor)
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

    }

    public interface IAuthHeaderService
    {
        string GetTokenOnly();
    }

}
