﻿// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


using System.Collections.Generic;
using Duende.IdentityServer.Models;

namespace IdentityServerHost
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[] { new IdentityResources.OpenId(), new IdentityResources.Profile() };

        public static IEnumerable<ApiScope> ApiScopes =>
            new[] { new ApiScope("api", new[] { "name" }) };

        public static IEnumerable<Client> Clients =>
            new[]
            {
                new Client
                {
                    ClientId = "spa",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,
                    RedirectUris = { "https://localhost:5002/signin-oidc" },

                    //FrontChannelLogoutUri = "https://localhost:5002/signout-oidc",
                    BackChannelLogoutUri = "https://localhost:5002/bff/backchannel",
                    PostLogoutRedirectUris = { "https://localhost:5002/signout-callback-oidc" },
                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "api" }
                }
            };
    }
}
