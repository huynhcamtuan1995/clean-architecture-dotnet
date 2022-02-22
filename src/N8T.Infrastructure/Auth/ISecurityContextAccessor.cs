using Microsoft.AspNetCore.Http;

namespace N8T.Infrastructure.Auth
{
    public interface ISecurityContextAccessor
    {
        string UserId { get; }
        string Role { get; }
        string TokenValue { get; }
        string TokenType { get; }
        bool IsAuthenticated { get; }
        HttpContext HttpContext { get; }
    }
}
