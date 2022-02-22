using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using N8T.Infrastructure.Auth;
using N8T.Infrastructure.Cache;

namespace IdentityService.Infrastructure.Handlers
{
    public class AuthHandler : IAuthorizationHandler
    {
        private readonly ISecurityContextAccessor _securityContextAccessor;
        private readonly ICacheService<string> _cacheService;

        public AuthHandler(ISecurityContextAccessor securityContextAccessor,
            ICacheService<string> cacheService)
        {
            _securityContextAccessor = securityContextAccessor ?? throw new ArgumentNullException(nameof(securityContextAccessor));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        }

        /// <summary>Makes a decision if authorization is allowed.</summary>
        /// <param name="context">The authorization information.</param>
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            string token = _securityContextAccessor.TokenValue;
            if (string.IsNullOrEmpty(token) ||
                _securityContextAccessor.IsAuthenticated == false ||
                _cacheService.Get(token) is not null ||
                context.HasFailed)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            IEnumerable<IAuthorizationRequirement> requirements =
                context.PendingRequirements.Where(x => x is IAuthRequest);
            foreach (IAuthorizationRequirement requirement in requirements)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
