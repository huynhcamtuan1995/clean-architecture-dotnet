using System.Threading.Tasks;
using AppContracts.Dtos;
using IdentityService.AppCore.Core.Models;

namespace IdentityService.AppCore.Interfaces
{
    public interface IUserIdentityService
    {
        Task<ApplicationUser> ValidUserAsync(string username, string password);
        Task<ApplicationUser> RegisterAsync(CustomerDto customerDto);
        Task<AccessTokenDto> Authenticate(UserLoginDto request);
        Task<bool> SignOutAsync(string token);
    }
}
