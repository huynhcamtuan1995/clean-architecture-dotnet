using System;
using System.Threading.Tasks;
using AppContracts.Dtos;
using IdentityService.AppCore.Core.Models;

namespace IdentityService.AppCore.Interfaces
{
    public interface IIdentityUserService
    {
        Task<ApplicationUser> ValidUserAsync(string username, string password);
        Task<(Guid userId, AccessTokenDto accessToken)> RegisterAsync(CustomerDto customerDto);
        Task<AccessTokenDto> AuthenticateAsync(AuthenticateDto request);
        Task<bool> RevokeAccessTokenAsync(string token);
        Task<bool> RevokeRefreshTokenAsync(string token);
        Task<AccessTokenDto> RefreshAccessTokenAsync(string token);
    }
}
