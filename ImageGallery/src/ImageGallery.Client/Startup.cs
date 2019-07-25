using IdentityModel;
using ImageGallery.Client.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace ImageGallery.Client
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            
            //clear the ClaimTypeMap to keep the original claim types
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        }
 
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            //Authorization policies
            services.AddAuthorization(authorizationOptions =>
            {
                authorizationOptions.AddPolicy("CanOrderFrame", policyBuilder =>
                {
                    policyBuilder.RequireAuthenticatedUser();
                    policyBuilder.RequireClaim("country", "be", "en", "us"); //any of these
                    policyBuilder.RequireClaim("subscriptionLevel", "PayingUser");
                });
            });

            // register an IHttpContextAccessor so we can access the current
            // HttpContext in services by injecting it
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // register an IImageGalleryHttpClient
            services.AddScoped<IImageGalleryHttpClient, ImageGalleryHttpClient>();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            }).AddCookie("Cookies", options =>
            {
                options.AccessDeniedPath = "/Authorization/AccessDenied";
            })
            .AddOpenIdConnect("oidc", options =>
            {
                options.SignInScheme = "Cookies";
                options.Authority = "https://localhost:44359";
                options.ClientId = "imagegalleryclient";
                options.ClientSecret = "secret";
                //options.CallbackPath = new PathString("..."); //default = https://localhost:44356/signin-oidc
                //options.SignedOutCallbackPath = new PathString("..."); //default = https://localhost:44356/signout-callback-oidc
                options.ResponseType = "code id_token";
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("address");
                options.Scope.Add("roles");
                options.Scope.Add("country");
                options.Scope.Add("subscriptionLevel");
                options.Scope.Add("imagegalleryapi");
                options.Scope.Add("offline_access");
                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.ClaimActions.Remove("amr"); //remove the claim action that filter out the amr claim
                options.ClaimActions.DeleteClaim("sid"); //filter out the sid claim
                options.ClaimActions.DeleteClaim("idp");
                options.ClaimActions.MapUniqueJsonKey("role", "role");
                options.ClaimActions.MapUniqueJsonKey("country", "country");
                options.ClaimActions.MapUniqueJsonKey("subscriptionLevel", "subscriptionLevel");

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = JwtClaimTypes.GivenName,
                    RoleClaimType = JwtClaimTypes.Role
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Shared/Error");
            }

            app.UseAuthentication();

            app.UseStaticFiles();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Gallery}/{action=Index}/{id?}");
            });
        }
    }
}
