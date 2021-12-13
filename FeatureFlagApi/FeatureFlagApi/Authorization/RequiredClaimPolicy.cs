using FeatureFlagApi.Logging;
using FeatureFlagApi.Services;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using cnst = FeatureFlag.Shared.Constants;

namespace FeatureFlagApi.Authorization
{
    public class RequiredClaimPolicy : IAuthorizationRequirement
    {
    }

    public class RequiredClaimPolicyHandler : AuthorizationHandler<RequiredClaimPolicy>
    {
        private readonly AWSStructuredLogger _logger;
        private readonly IRequestHeaderService _requestHeaderService;

        public RequiredClaimPolicyHandler(AWSStructuredLogger logger,
            IRequestHeaderService requestHeaderService)
        {
            _logger = logger;
            _requestHeaderService = requestHeaderService;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RequiredClaimPolicy requirement)
        {
            if(!_requestHeaderService.JWTTokenHasTenantClaim())
            {
                _logger.LogError($"The api requires the header {cnst.Headers.TENANT_ID}");
                context.Fail();
                return Task.CompletedTask;
            }
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }

}
