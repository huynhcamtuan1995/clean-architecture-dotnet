using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace N8T.Infrastructure.Auth
{
    public class AuthBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull, IRequest<TResponse>
        where TResponse : notnull
    {
        private readonly IEnumerable<IAuthorizationRequirement> _authorizationRequirements;
        private readonly IAuthorizationService _authorizationService;
        private readonly ISecurityContextAccessor _securityContextAccessor;
        private readonly ILogger<AuthBehavior<TRequest, TResponse>> _logger;

        public AuthBehavior(
            ISecurityContextAccessor securityContextAccessor,
            IAuthorizationService authorizationService,
            IEnumerable<IAuthorizationRequirement> authorizationRequirements,
            ILogger<AuthBehavior<TRequest, TResponse>> logger)
        {
            _securityContextAccessor = securityContextAccessor ?? throw new ArgumentNullException(nameof(securityContextAccessor));
            _authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
            _authorizationRequirements =
                authorizationRequirements ?? throw new ArgumentNullException(nameof(authorizationRequirements));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<TResponse> Handle(TRequest request,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            if (request is not IAuthRequest)
            {
                return await next();
            }

            _logger.LogInformation("[{Prefix}] Starting AuthBehavior", nameof(AuthBehavior<TRequest, TResponse>));

            ClaimsPrincipal currentUser = _securityContextAccessor.HttpContext?.User;
            if (currentUser == null)
            {
                throw new UnauthorizedAccessException("You need to login.");
            }

            AuthorizationResult result = await _authorizationService.AuthorizeAsync(
                currentUser, null,
                _authorizationRequirements.Where(x => x is TRequest));
            if (!result.Succeeded)
            {
                throw new UnauthorizedAccessException("Unauthorized");
            }

            return await next();
        }
    }
}
