using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace N8T.Infrastructure.Auth
{
    public class SecurityContextAccessor : ISecurityContextAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<SecurityContextAccessor> _logger;

        public SecurityContextAccessor(IHttpContextAccessor httpContextAccessor,
            ILogger<SecurityContextAccessor> logger)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        }
        public HttpContext HttpContext => _httpContextAccessor.HttpContext;

        public string UserId => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public string Role => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;

        public string TokenValue
        {
            get
            {
                StringValues? token = _httpContextAccessor.HttpContext?.Request?.Headers["Authorization"];
                return token.ToString()?.Replace("Bearer ", string.Empty);
            }
        }

        public string TokenType
        {
            get
            {
                StringValues? token = _httpContextAccessor.HttpContext?.Request?.Headers["Authorization"];
                return token.ToString()?.Split(" ", StringSplitOptions.RemoveEmptyEntries)[0];
            }
        }


        public bool IsAuthenticated
        {
            get
            {
                bool? isAuthenticated = _httpContextAccessor.HttpContext?.User?.Identities?.FirstOrDefault()
                    ?.IsAuthenticated;
                return isAuthenticated.HasValue && isAuthenticated.Value;
            }
        }

    }
}
