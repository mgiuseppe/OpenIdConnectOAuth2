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
