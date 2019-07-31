using MG_IdentityProvider.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MG_IdentityProvider
{
    public static class IdentityServerBuilderExtensions
    {
        public static IIdentityServerBuilder AddMGUserStore(this IIdentityServerBuilder builder)
        {
            builder.Services.AddScoped<IMGUserRepository, MGUserRepository>();
            builder.AddProfileService<MGUserProfileService>(); //used by identity server to connect to user and profile store
            return builder;
        }
    }
}
