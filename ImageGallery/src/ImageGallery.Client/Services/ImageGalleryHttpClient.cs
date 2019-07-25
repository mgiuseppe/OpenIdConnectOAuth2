using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ImageGallery.Client.Services
{
    public class ImageGalleryHttpClient : IImageGalleryHttpClient
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private HttpClient _httpClient = new HttpClient();

        public ImageGalleryHttpClient(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        
        public async Task<HttpClient> GetClient()
        {
            var currentContext = _httpContextAccessor.HttpContext;

            //get token expiration 
            var expires_at = await currentContext.GetTokenAsync("expires_at");

            //get access token - renew tokens if it's almost expired
            var accessToken = string.IsNullOrEmpty(expires_at) || IsTokenAlmostExpired(expires_at) 
                ? await RenewTokensAsync()
                : await currentContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            
            if (!string.IsNullOrWhiteSpace(accessToken))
                _httpClient.SetBearerToken(accessToken);

            _httpClient.BaseAddress = new Uri("https://localhost:44323/");
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            return _httpClient;
        }

        private bool IsTokenAlmostExpired(string expires_at) => DateTime.Parse(expires_at).AddSeconds(-60).ToUniversalTime() < DateTime.UtcNow;
        
        public async Task<string> RenewTokensAsync()
        {
            // get the current HttpContext to access the tokens
            var currentContext = _httpContextAccessor.HttpContext;

            // get the IDP metadata
            var discoveryClient = new DiscoveryClient("https://localhost:44359");
            var metadataResponse = await discoveryClient.GetAsync();

            // create a new token client to get a new token
            var tokenClient = new TokenClient(metadataResponse.TokenEndpoint, "imagegalleryclient", "secret");

            // get the saved refresh token
            var currentRefreshToken = await currentContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

            // refresh the tokens
            var tokenResult = await tokenClient.RequestRefreshTokenAsync(currentRefreshToken);

            if (!tokenResult.IsError)
            {
                // compute new expiration
                var expiresAt = DateTime.UtcNow + TimeSpan.FromSeconds(tokenResult.ExpiresIn);

                // update the tokens and expiration value
                var updatedTokens = new List<AuthenticationToken>
                {
                    new AuthenticationToken { Name = OpenIdConnectParameterNames.IdToken, Value = tokenResult.IdentityToken },
                    new AuthenticationToken { Name = OpenIdConnectParameterNames.AccessToken, Value = tokenResult.AccessToken },
                    new AuthenticationToken { Name = OpenIdConnectParameterNames.RefreshToken, Value = tokenResult.RefreshToken },
                    new AuthenticationToken { Name = "expires_at", Value = expiresAt.ToString("o", CultureInfo.InvariantCulture) }
                };

                // get authenticateResult containg the current principal and properties (using the schema Cookies)
                var currentAuthenticateResult = await currentContext.AuthenticateAsync("Cookies");

                // store the updated tokens after removing old tokens
                currentAuthenticateResult.Properties.StoreTokens(updatedTokens);

                // sign in
                await currentContext.SignInAsync("Cookies",
                    currentAuthenticateResult.Principal,
                    currentAuthenticateResult.Properties);

                return tokenResult.AccessToken;
            }
            else
                throw new Exception("Problem encountered while refreshing tokens.", tokenResult.Exception);
        }
    }
}

