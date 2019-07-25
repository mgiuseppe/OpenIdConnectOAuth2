using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MG_IdentityProvider
{
    public static class Config
    {
        public static List<TestUser> GetUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "d860efca-22d9-47fd-8249-791ba61b07c7",
                    Username = "Giuseppe",
                    Password = "password",

                    Claims = new List<Claim>
                    {
                        new Claim("given_name", "Giuseppe"),
                        new Claim("family_name", "Underwood"),
                        new Claim("address", "tree road 52"),
                        new Claim("role", "FreeUser"),
                        new Claim("subscriptionLevel", "FreeUser"),
                        new Claim("country", "it")
                    }
                },
                new TestUser
                {
                    SubjectId = "b7539694-97e7-4dfe-84da-b4256e1ff5c7",
                    Username = "Giancarlo",
                    Password = "password",
                    
                    Claims = new List<Claim>
                    {
                        new Claim("given_name", "Giancarlo"),
                        new Claim("family_name", "Underwood"),
                        new Claim("address", "tree road 42"),
                        new Claim("role", "PayingUser"),
                        new Claim("subscriptionLevel", "PayingUser"),
                        new Claim("country", "be")
                    }
                },
            };
        }

        // identity-related resources (scopes)
        public static List<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Address(),
                new IdentityResource(
                    "roles",            //scope name
                    "Your role(s)",     //display name shown to the user when asking consent
                    new[] { "role" }),  //claim types tied to the scope
                new IdentityResource(
                    "country",
                    "The country you're living in",
                    new[] { "country" }),
                new IdentityResource(
                    "subscriptionLevel",
                    "Your subscription level",
                    new[] { "subscriptionLevel" })
            };
        }

        public static List<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource(
                    "imagegalleryapi", // scope name 
                    "Image Gallery API", // display name  
                    new[] { "role" }) // claim types to add to the access token
            };
        }

        public static List<Client> GetClients()
        {
            return new List<Client>()
            {
                new Client {
                    ClientName = "Image Gallery",
                    ClientId = "imagegalleryclient",
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    //IdentityTokenLifetime = 300, //in secs - default 5min
                    //AuthorizationCodeLifetime = 300, //in secs - default 5min
                    AccessTokenLifetime = 120, //in secs - default 1h
                    AllowOfflineAccess = true,
                    UpdateAccessTokenClaimsOnRefresh = true,
                    RedirectUris = new List<string>
                    {
                        "https://localhost:44356/signin-oidc"
                    },
                    PostLogoutRedirectUris =
                    {
                        "https://localhost:44356/signout-callback-oidc"
                    },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Address,
                        "roles",
                        "imagegalleryapi",
                        "country",
                        "subscriptionLevel"
                    },
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    }
                }
            };
        }
    }
}
