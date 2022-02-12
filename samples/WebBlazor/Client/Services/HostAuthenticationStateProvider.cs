// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;

namespace Blazor.Client.Services
{
    // thanks to Bernd Hirschmann for this code
    // https://github.com/berhir/BlazorWebAssemblyCookieAuth
    public class HostAuthenticationStateProvider : AuthenticationStateProvider
    {
        private static readonly TimeSpan UserCacheRefreshInterval = TimeSpan.FromSeconds(60);
        private readonly NavigationManager _navigation;
        private readonly HttpClient _client;
        private readonly ILogger<HostAuthenticationStateProvider> _logger;

        private ClaimsPrincipal _cachedUser = new ClaimsPrincipal(new ClaimsIdentity());

        private DateTimeOffset _userLastCheck = DateTimeOffset.FromUnixTimeSeconds(0);

        public HostAuthenticationStateProvider(NavigationManager navigation, HttpClient client,
            ILogger<HostAuthenticationStateProvider> logger)
        {
            _navigation = navigation;
            _client = client;
            _logger = logger;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return new AuthenticationState(await GetUser(true));
        }

        private async ValueTask<ClaimsPrincipal> GetUser(bool useCache = false)
        {
            DateTimeOffset now = DateTimeOffset.Now;
            if (useCache && now < _userLastCheck + UserCacheRefreshInterval)
            {
                _logger.LogDebug("Taking user from cache");
                return _cachedUser;
            }

            _logger.LogDebug("Fetching user");
            _cachedUser = await FetchUser();
            _userLastCheck = now;

            return _cachedUser;
        }

        private async Task<ClaimsPrincipal> FetchUser()
        {
            try
            {
                _logger.LogInformation("Fetching user information.");
                HttpResponseMessage response = await _client.GetAsync("bff/user");

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    List<ClaimRecord> claims = await response.Content.ReadFromJsonAsync<List<ClaimRecord>>();

                    ClaimsIdentity identity = new ClaimsIdentity(
                        nameof(HostAuthenticationStateProvider),
                        "name",
                        "role");

                    foreach ((string type, object value) in claims!)
                    {
                        identity.AddClaim(new Claim(type, value.ToString() ?? string.Empty));
                    }

                    return new ClaimsPrincipal(identity);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Fetching user failed.");
            }

            return new ClaimsPrincipal(new ClaimsIdentity());
        }

        private record ClaimRecord(string Type, object Value);
    }
}
