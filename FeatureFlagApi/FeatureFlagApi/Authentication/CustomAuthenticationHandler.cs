using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using cost = FeatureFlag.Shared.Constants;

namespace FeatureFlagApi.Authentication
{
    public class BasicAuthenticationOptions: AuthenticationSchemeOptions
    {

    }
    public class CustomAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
    {
        public CustomAuthenticationHandler(
            IOptionsMonitor<BasicAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock
            ): base(options, logger, encoder, clock)
        {

        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if(!Request.Headers.ContainsKey(cost.Headers.TENANT_ID))
            {
                return AuthenticateResult.Fail("Unauthorized");
            }

            string tenantIdHeader = Request.Headers[cost.Headers.TENANT_ID];
            if(string.IsNullOrWhiteSpace(tenantIdHeader))
            {
                return AuthenticateResult.NoResult();
            }

            try
            {
                return CreateTicketWithTenantClaim(tenantIdHeader);
            }
            catch (Exception ex)
            {

                return AuthenticateResult.Fail(ex.Message);
            }

        }

        private AuthenticateResult CreateTicketWithTenantClaim(string tenantHeader)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, Scheme.Name),
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new System.Security.Principal.GenericPrincipal(identity, null);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return AuthenticateResult.Success(ticket);
        }
    }
}
