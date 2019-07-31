using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4;
using MG_IdentityProvider.Entities;
using MG_IdentityProvider.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MG_IdentityProvider
{
    public class Startup
    {
        public IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<MGUserContext>(opts => opts.UseSqlServer(_config.GetConnectionString("MGUserDBConnectionString")));
            services.AddScoped<IMGUserRepository, MGUserRepository>();
            services.AddSingleton<IPasswordHasher, PasswordHasher>();

            services.AddMvc();

            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddMGUserStore()
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApiResources())
                .AddInMemoryClients(Config.GetClients());
            services.AddAuthentication()
                .AddGoogle("Google", options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme; //cookie handler configured by identityserver4 for external service

                    options.ClientId = "272433830027-keh9juh59oq6k51dd0ved6a0jlaqslpv.apps.googleusercontent.com";
                    options.ClientSecret = "MR4nmb_lL0lAK0SJ4HOOudYY";
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, MGUserContext ctx, IPasswordHasher passwordHasher)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseIdentityServer();

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();

            //seed db
            if (env.IsDevelopment())
            {
                ctx.Database.Migrate();
                ctx.EnsureSeedDataForContext(passwordHasher);
            }       
        }
    }
}
