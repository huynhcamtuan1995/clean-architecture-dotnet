using System;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AppContracts.Dtos;
using IdentityService.AppCore.Core.Models;
using IdentityService.AppCore.Enums;
using IdentityService.AppCore.Interfaces;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using N8T.Core.Helpers;
using N8T.Infrastructure.Cache;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace IdentityService.Infrastructure.Implementations
{
    public class IdentityUserService : IIdentityUserService
    {
        private readonly ICacheService<string> _cacheService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly int _expiry;
        private readonly string _secretKey;
        private readonly string _validAudience;
        private readonly string _validIssuer;

        public IdentityUserService(IConfiguration configuration,
            ICacheService<string> cacheService,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _cacheService = cacheService;
            _userManager = userManager;
            _signInManager = signInManager;
            // change style GetValue<T> to GetSection Value because identity library version conflicts
            _secretKey = configuration.GetSection("Security:Tokens:Key")?.Value;
            _expiry = int.Parse(configuration.GetSection("Security:Tokens:Expiry").Value);
            //_validIssuer = configuration.GetSection("Security:Tokens:Issuer");
            //_validAudience = configuration.GetSection("Security:Tokens:Audience"); 
        }

        public async Task<ApplicationUser> GetUserByUserName(string username)
        {
            return await _userManager.FindByNameAsync(username);
        }
        public async Task<ApplicationUser> GetUserByEmail(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        #region register
        public async Task<(Guid userId, AccessTokenDto accessToken)> RegisterAsync(CustomerDto customerDto)
        {
            string userEmail = customerDto.Email;

            ApplicationUser user = new ApplicationUser
            {
                UserName = customerDto.UserName,
                Email = userEmail,
                IsEnabled = true,
                EmailConfirmed = true,
                FirstName = customerDto.UserName,
                LastName = customerDto.LastName,
            };

            IdentityResult registered = await _userManager.CreateAsync(user, customerDto.Password);
            return registered.Succeeded
                ? ((await GetUserByEmail(userEmail))!.Id, await GenerateAccessTokenAsync(user))
                : default((Guid Id, AccessTokenDto));
        }
        #endregion

        #region login
        public async Task<AccessTokenDto> AuthenticateAsync(AuthenticateDto request)
        {
            ApplicationUser user = await ValidUserAsync(request.UserName, request.Password);
            if (user == null)
            {
                return null;
            }

            //RefreshToken refreshToken = GenerateRefreshToken(ipAddress);
            //user.RefreshTokens.Add(refreshToken);

            await _userManager.UpdateAsync(user);
            return await GenerateAccessTokenAsync(user);
        }
        public async Task<ApplicationUser> ValidUserAsync(string username, string password)
        {
            ApplicationUser user = await GetUserByUserName(username);
            if (user == null)
            {
                // User not found;
                return null;
            }

            if (user.IsEnabled != true)
            {
                // User was disabled
                return null;
            }

            SignInResult signInResult =
                await _signInManager.PasswordSignInAsync(user, password, false, true);
            return signInResult.Succeeded ? user : null;
        }
        public async Task<AccessTokenDto> GenerateAccessTokenAsync(CustomerDto customer, string role = RoleEnum.User)
        {
            ApplicationUser user = customer.Adapt<ApplicationUser>();
            user.UpdateRole(role);
            return await GenerateAccessTokenAsync(user);
        }
        public Task<AccessTokenDto> GenerateAccessTokenAsync(ApplicationUser user)
        {
            DateTime now = DateTime.UtcNow;
            DateTime expireAt = now.AddMinutes(_expiry);

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                //Issuer = _validIssuer,
                //Audience = _validAudience,
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("userid", user.Id.ToString()),
                    new Claim("fullname" , $"{user.FirstName} {user.LastName}"),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.NameIdentifier, user.Email),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim("scope", "api")
                }),
                Expires = expireAt,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_secretKey)),
                    SecurityAlgorithms.HmacSha256)
            };

            SecurityToken token = handler.CreateToken(descriptor);
            string accessToken = handler.WriteToken(token);

            return Task.FromResult(new AccessTokenDto
            {
                Username = user.UserName,
                Email = user.Email,
                Role = user.Role,
                AccessToken = accessToken,
                RefreshToken = string.Empty,
                CreateAt = now.ToString(CultureInfo.InvariantCulture),
                ExpireAt = expireAt.ToString(CultureInfo.InvariantCulture)
            });
        }
        public Task<AccessTokenDto> RefreshAccessTokenAsync(string token)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region logout
        public Task<bool> RevokeAccessTokenAsync(string token)
        {
            return Task.FromResult(_cacheService.Set(token, DateTimeHelper.NewSystemDateTime()));
        }
        public Task<bool> RevokeRefreshTokenAsync(string token)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
