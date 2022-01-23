﻿using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace soulgram.identity
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[] {
                    new IdentityResources.OpenId(),
                    new IdentityResources.Profile(),
                    new IdentityResources.Email(),
                   };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new("posts"),
                new(IdentityServerConstants.LocalApi.ScopeName)
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new()
                {
                    AllowedScopes =
                    {
                        IdentityServerConstants.LocalApi.ScopeName,
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Email,
                        "posts"
                    },
                    ClientId = "test_human_client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    RequireClientSecret = false,
                },
                new () {
                    ClientId = "soulgram_ui",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "posts"
                    },
                    PostLogoutRedirectUris = {"http://localhost:4200/"},
                    AllowedCorsOrigins = {"http://localhost:4200"},
                    AllowAccessTokensViaBrowser = true,
                    AllowOfflineAccess = true,
                    RequireClientSecret = false

                }
            };

        public static IEnumerable<ApiResource> Apis = new List<ApiResource>
        {
            new(IdentityServerConstants.LocalApi.ScopeName),
            new("posts")
        };
    }
}